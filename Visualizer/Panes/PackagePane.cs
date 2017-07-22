using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using viewmodel;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public class PackagePane: BasePane
    {
        protected PackageLibrary _library;
        protected List<SccmPackage> _highlightedpackages = new List<SccmPackage>();

        protected PackageTabControl _pane;
        public PackageTabControl Pane { get { return this._pane; } }

        protected CollectionType CollectionsType { get; set; }

        protected List<SccmPackage> _searchresults;
        public List<SccmPackage> SearchResults
        {
            get { return this._searchresults; }
            set
            {
                this._searchresults = value;
                this.OnPropertyChanged(this, "SearchResults");
            }
        }

        protected SccmPackage _selectedresult;
        public SccmPackage SelectedResult
        {
            get { return this._selectedresult; }
            set
            {
                this._selectedresult = value;
                this.OnPropertyChanged(this, "SelectedResult");
            }
        }

        public PackagePane(SccmConnector connector):base(connector)
        {
            this._library = connector.GetPackageLibrary();
            this._header = "Packages";
            this._pane = new PackageTabControl();
            MsaglHelpers.ConfigureCollectionsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.searchresultslb.MouseDoubleClick += this.OnSearchResultsListDoubleClick;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            this._pane.gviewer.MouseDoubleClick += this.OnGViewerMouseDoubleClick;
            //this._pane.abortbtn.Click += OnAbortButtonClick;
            //this._pane.findbtn.Click += this.OnFindButtonPressed;
            //this._pane.findresourcetb.KeyUp += this.OnFindKeyUp;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            this._pane.searchtb.KeyUp += this.OnSearchKeyUp;
        }

        protected void ClearHighlightedCollections()
        {
            foreach (SccmPackage col in this._highlightedpackages)
            {
                col.IsHighlighted = false;
            }
            this._highlightedpackages.Clear();
        }

        protected void OnSearchButtonPressed(object sender, RoutedEventArgs e) { this.UpdateSearchResults(); }
        protected void OnSearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.UpdateSearchResults();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        private async void UpdateSearchResults()
        {
            this.StartProcessing();

            Task.Run(() => this.NotifyProgress("Searching"));
            await Task.Run(() => this.SearchResults = this._connector.GetPackagesFromSearch(this._searchtext));

            this.FinishProcessing();
            this.NotifyFinishSearchWithCount(this.SearchResults.Count);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            this.StartProcessing();
            this.ClearHighlightedCollections();
            Task.Run(() => this.NotifyProgress("Building"));

            string collectionid = this._selectedresult?.ID;

            await Task.Run(() => this._graph = this.BuildGraphTree(collectionid));
            await Task.Run(() => this.UpdatePaneToTabControl());
            this.FinishProcessing();
        }

        public Graph BuildGraphTree(string rootcollectionid)
        {
            Graph graph = null;
            if (string.IsNullOrWhiteSpace(rootcollectionid) == false)
            {
                SccmPackage package = this._library.GetPackage(rootcollectionid);
                if (package != null)
                {
                    graph = TreeBuilder.BuildPackagesTree(this._library, package);
                    package.IsHighlighted = true;
                }
            }
            else
            {
                this.NotificationText = "Please select something";
            }
            return graph;
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
