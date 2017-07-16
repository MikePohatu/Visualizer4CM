using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;
using System.Threading;


namespace Visualizer.Panes
{
    public abstract class BaseResourcePane: BasePane
    {
        protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = false;

        protected ResourceTabControl _pane;
        public ResourceTabControl Pane { get { return this._pane; } }

        protected string _resourcetext;
        public string ResourceText
        {
            get { return this._resourcetext; }
            set
            {
                this._resourcetext = value;
                this.OnPropertyChanged(this, "ResourceText");
            }
        }

        public BaseResourcePane(SccmConnector connector):base(connector)
        {
            this._pane = new ResourceTabControl();
            MsaglHelpers.ConfigureCollectionsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.buildtb.KeyUp += this.OnBuildKeyUp;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            //this._pane.abortbtn.Click += OnAbortButtonClick;
            this._pane.searchbtn.Click += this.OnFindButtonPressed;
            this._pane.searchtb.KeyUp += this.OnFindKeyUp;
        }

        public Graph FindCollectionID(string collectionid, string mode)
        {
            Graph graph = null;
            if (string.IsNullOrWhiteSpace(collectionid) == false)
            {
                SccmCollection col = this._library.GetCollection(collectionid);
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
                        graph = TreeBuilder.BuildCollectionTreeMeshMode(this._connector, this._library, collectionid);
                    }
                    else if (mode == "Limiting")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildCollectionTreeLimitingMode(this._library, collectionid);
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

        protected void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
            }
            this._highlightedcollections.Clear();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            this.ControlsEnabled = false;
            this._processing = true;
            this.ClearHighlightedCollections();
            string mode = this._pane.modecombo.Text;
            Task.Run(() => this.NotifyProgress("Building"));
            await Task.Run(() => this._graph = this.FindCollectionID(this._collectiontext, mode));
            await Task.Run(() => this.UpdatePaneToTabControl());
            this._processing = false;
            this.ControlsEnabled = true;
        }

        protected void OnFindButtonPressed(object sender, RoutedEventArgs e) { this.Find(); }
        protected abstract void Find();
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
    }
}
