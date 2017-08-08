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
    public class IssuesPane : BasePane
    {
        protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = true;

        protected IssuesTabControl _pane;
        public IssuesTabControl Pane { get { return this._pane; } }

        protected List<SccmCollection> _searchresults;
        public List<SccmCollection> SearchResults
        {
            get { return this._searchresults; }
            set
            {
                this._searchresults = value;
                this.OnPropertyChanged(this, "SearchResults");
            }
        }

        protected SccmCollection _selectedresult;
        public SccmCollection SelectedResult
        {
            get { return this._selectedresult; }
            set
            {
                this._selectedresult = value;
                this.OnPropertyChanged(this, "SelectedResult");
            }
        }

        protected CollectionType CollectionsType { get; set; }

        public IssuesPane(SccmConnector connector):base(connector)
        {
            this._pane = new IssuesTabControl();
            MsaglHelpers.ConfigureCollectionsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            //this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            //this._pane.searchresultslb.MouseDoubleClick += this.OnSearchResultsListDoubleClick;
            //this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            //this._pane.gviewer.MouseDoubleClick += this.OnGViewerMouseDoubleClick;
            ////this._pane.abortbtn.Click += OnAbortButtonClick;
            //this._pane.findbtn.Click += this.OnFindButtonPressed;
            //this._pane.findresourcetb.KeyUp += this.OnFindKeyUp;
            //this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            //this._pane.searchtb.KeyUp += this.OnSearchKeyUp;
        }

        protected void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
            }
            this._highlightedcollections.Clear();
        }

        protected void OnSearchButtonPressed(object sender, RoutedEventArgs e) { this.UpdateSearchResults(); }
        protected void OnSearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.UpdateSearchResults();
            }
        }

        protected void OnFindButtonPressed(object sender, RoutedEventArgs e) { this.Find(); }
        protected void Find()
        {
            this.ClearHighlightedCollections();
            if (string.IsNullOrWhiteSpace(this._findtext) == false)
            {
                SccmDevice dev = this._connector.GetDevice(this._findtext.Trim());
                if (dev != null) { this._highlightedcollections = TreeBuilder.HighlightCollectionMembers(this._graph, dev.CollectionIDs); }
            }
            else
            {
                SccmCollection col = this._library.GetCollection(this._findtext);
                if (col != null) { col.IsHighlighted = true; }
            }
            this.Redraw();
        }
        protected void OnFindKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Find();
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
            await Task.Run(() => this.SearchResults = this._connector.GetCollectionsFromSearch(this._searchtext,this.CollectionsType));

            this.FinishProcessing();
            this.NotifyFinishSearchWithCount(this.SearchResults.Count);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            this.StartProcessing();
            this.ClearHighlightedCollections();
            //string mode = this._pane.modecombo.Text;
            
            if (this._library == null)
            {
                Task.Run(() => this.NotifyProgress("Initializing library"));
                await Task.Run(() => this._library = this._connector.GetCollectionLibrary(CollectionsType));
            }
            else { Task.Run(() => this.NotifyProgress("Building")); }

            string collectionid = this._selectedresult?.ID;

            
            //await Task.Run(() => this._graph = this.BuildGraphTree(collectionid, mode));
            await Task.Run(() => this.UpdatePaneToTabControl());
            this.FinishProcessing();
        }

        public Graph BuildGraphTree(string rootcollectionid, string mode)
        {
            Graph graph = null;
            if (string.IsNullOrWhiteSpace(rootcollectionid) == false)
            {
                SccmCollection col = this._library.GetCollection(rootcollectionid);
                if (col != null)
                {
                    if (mode == "Context")
                    {
                        if (this._filteredview == true) { graph = TreeBuilder.BuildCollectionTreeAllCollections(this._library); }
                        else { graph = this._graph; }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else if (mode == "Mesh")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildCollectionTreeMeshMode(this._connector, this._library, rootcollectionid);
                    }
                    else if (mode == "Limiting")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildCollectionTreeLimitingMode(this._library, rootcollectionid);
                    }
                    col.IsHighlighted = true;
                }
            }
            else
            {
                this._filteredview = false;
                graph = TreeBuilder.BuildCollectionTreeAllCollections(this._library);
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
