using System.Windows;
using System.Collections.Generic;
using viewmodel;
using System.Threading.Tasks;

namespace Visualizer.Panes
{
    public class ApplicationPane : BasePane
    {
        protected Dictionary<string, SccmApplication> _applications;
        protected bool _queryisrun;

        protected ApplicationTabControl _pane;
        public ApplicationTabControl Pane { get { return this._pane; } }

        protected string _applicationtext;
        public string ApplicationText
        {
            get { return this._applicationtext; }
            set
            {
                this._applicationtext = value;
                this.OnPropertyChanged(this, "ApplicationText");
            }
        }

        public ApplicationPane(SccmConnector connector): base(connector)
        {
            this._header = "Applications";
            this._findlabeltext = "Application:";
            this._pane = new ApplicationTabControl();
            MsaglHelpers.ConfigureGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.buildtb.KeyUp += this.OnBuildKeyUp;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
        }

        protected void QueryApplications()
        {
            this._applications = this._connector.GetApplications();
            this._queryisrun = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            if (string.IsNullOrWhiteSpace(this._applicationtext) == false)
            {
                this.ControlsEnabled = false;
                this._building = true;
                List<string> appids = this._connector.GetApplicationIDsFromSearch(this._applicationtext?.Trim());
                Task.Run(() => this.NotifyProgress("Building"));
                if (this._queryisrun == false) { await Task.Run(() => this.QueryApplications()); }
                await Task.Run(() => this._graph = TreeBuilder.BuildApplicationTree(this._connector, this._applications, appids));
                await Task.Run(() => this.UpdatePaneToTabControl());
                this._building = false;
                this.ControlsEnabled = true;
            }
            else
            {
                this.NotificationText = "Please enter an application name";
            }
        }

        protected void UpdatePaneToTabControl()
        {
            this._pane.gviewer.Graph = this._graph;
        }

        protected void Redraw()
        {
            this._pane.gviewer.Invalidate();
        }
    }
}
