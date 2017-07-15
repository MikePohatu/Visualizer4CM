using System.Windows;
using System.Collections.Generic;
using viewmodel;
using System.Threading.Tasks;

namespace Visualizer.Panes
{
    public class ApplicationPane : BasePane
    {
        protected SccmApplication _highlightedapplication;

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
            MsaglHelpers.ConfigureApplicationsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.searchbtn.KeyUp += this.OnBuildKeyUp;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
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

            Task.Run(() => this.NotifyProgress("Searching"));
            await Task.Run(() => this.SearchResults = this._connector.GetApplicationsListFromSearch(this._searchtext));

            this._processing = false;
            this.ControlsEnabled = true;
        }
    }
}
