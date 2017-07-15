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

        protected string _searchtext;
        public string SearchText
        {
            get { return this._searchtext; }
            set
            {
                this._searchtext = value;
                this.OnPropertyChanged(this, "SearchText");
            }
        }
        protected List<SccmApplication> _searchresults;
        public List<SccmApplication> SearchResults
        {
            get { return this._searchresults; }
            set
            {
                this._searchresults = value;
                this.OnPropertyChanged(this, "SearchResults");
            }
        }
        protected SccmApplication _selectedresult;
        public SccmApplication SelectedResult
        {
            get { return this._selectedresult; }
            set
            {
                this._selectedresult = value;
                this.OnPropertyChanged(this, "SelectedResult");
            }
        }

        public ApplicationPane(SccmConnector connector): base(connector)
        {
            this._header = "Applications";
            this._findlabeltext = "Application:";
            this._pane = new ApplicationTabControl();
            MsaglHelpers.ConfigureGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.searchbtn.KeyUp += this.OnBuildKeyUp;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
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
            if (this.SelectedResult != null)
            {
                this.ControlsEnabled = false;
                this._processing = true;
                //List<string> appids = this._connector.GetApplicationIDsFromSearch(this._searchtext?.Trim());
                Task.Run(() => this.NotifyProgress("Building"));
                
                await Task.Run(() => this._graph = TreeBuilder.BuildApplicationTree(this._connector, this._applications, this.SelectedResult));
                await Task.Run(() => this.UpdatePaneToTabControl());
                this._processing = false;
                this.ControlsEnabled = true;
            }
            else
            {
                this.NotificationText = "Please select a search result";
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

        protected void OnSearchButtonPressed(object sender, RoutedEventArgs e)
        {
            this.UpdateSearchResults();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private async void UpdateSearchResults()
        {
            this.ControlsEnabled = false;
            this._processing = true;
            //List<string> appids = this._connector.GetApplicationIDsFromSearch(this._searchtext?.Trim());
            Task.Run(() => this.NotifyProgress("Querying server"));
            if (this._queryisrun == false) { await Task.Run(() => this.QueryApplications()); }

            this.UpdateProgressMessage("Processing results");
            List<string> results = this._connector.GetApplicationIDsFromSearch(this._searchtext);
            List<SccmApplication> applicationresults = new List<SccmApplication>();

            this.UpdateProgressMessage("Updating results");
            foreach (string result in results)
            {
                SccmApplication outapp;
                if (this._applications.TryGetValue(result, out outapp) == true)
                {
                    applicationresults.Add(outapp);
                }
            }
            this.SearchResults = applicationresults;

            this._processing = false;
            this.ControlsEnabled = true;
        }
    }
}
