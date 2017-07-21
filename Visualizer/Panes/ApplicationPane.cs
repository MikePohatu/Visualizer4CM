using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using viewmodel;
using System.Threading.Tasks;
using Microsoft.Msagl.GraphViewerGdi;

namespace Visualizer.Panes
{
    public class ApplicationPane : BasePane
    {
        protected SccmApplication _highlightedapplication;

        protected ApplicationTabControl _pane;
        public ApplicationTabControl Pane { get { return this._pane; } }

        protected new List<SccmApplication> _searchresults;
        public new List<SccmApplication> SearchResults
        {
            get { return this._searchresults; }
            set
            {
                this._searchresults = value;
                this.OnPropertyChanged(this, "SearchResults");
            }
        }

        protected new SccmApplication _selectedresult;
        public new SccmApplication SelectedResult
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
            MsaglHelpers.ConfigureApplicationsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.searchbtn.KeyUp += this.OnBuildKeyUp;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.searchresultslb.MouseDoubleClick += this.OnSearchResultsListDoubleClick;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            this._pane.searchtb.KeyUp += this.OnSearchKeyUp;
            //this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            this._pane.gviewer.MouseDoubleClick += this.OnGViewerMouseDoubleClick;
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

        protected void OnSearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.UpdateSearchResults();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private async void UpdateSearchResults()
        {
            this.ControlsEnabled = false;
            this._processing = true;

            Task.Run(() => this.NotifyProgress("Searching"));
            await Task.Run(() => this.SearchResults = this._connector.GetApplicationsListFromSearch(this._searchtext));

            this._processing = false;
            this.ControlsEnabled = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            if (this.SelectedResult != null)
            {
                this.ControlsEnabled = false;
                this._processing = true;
                if (this._highlightedapplication != null) { this._highlightedapplication.IsHighlighted = false; }

                Task.Run(() => this.NotifyProgress("Building"));
                await Task.Run(() => this._graph = TreeBuilder.BuildApplicationTree(this._connector, this.SelectedResult));

                this.UpdateProgressMessage("Updating view");
                await Task.Run(() => this.UpdatePaneToTabControl());

                this._highlightedapplication = this.SelectedResult;
                this.SelectedResult.IsHighlighted = true;
                this.Redraw();
                this._processing = false;
                this.ControlsEnabled = true;
            }
            else
            {
                this.NotificationText = "Please select a search result";
            }
        }

        protected virtual void OnGViewerMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GViewer viewer = (GViewer)sender;
            object selected = viewer.SelectedObject;
            if (selected != null)
            {
                this.SelectedNode = selected as SccmNode;
                if (this.SelectedNode != null)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.SearchText = this.SelectedNode.SccmObject.Name;
                    }
                }
            }
        }
    }
}
