using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System;
using Microsoft.Msagl.Drawing;
using viewmodel;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace CollectionViewer.Panes
{
    public abstract class BasePane: ViewModelBase
    {
        protected SccmConnector _connector;
        protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = false;
        protected int _progresscount = 0;
        protected Graph _graph;
        public Graph Graph { get { return this._graph; } }

        protected ResourceTabControl _pane;
        public ResourceTabControl Pane { get { return this._pane; } }

        protected string _notificationtext;
        public string NotificationText
        {
            get { return this._notificationtext; }
            set
            {
                this._notificationtext = value;
                this.OnPropertyChanged(this, "NotificationText");
            }
        }

        protected string _collectiontext;
        public string CollectionText
        {
            get { return this._collectiontext; }
            set
            {
                this._collectiontext = value;
                this.OnPropertyChanged(this, "CollectionText");
            }
        }

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

        protected string _header;
        public string Header
        {
            get { return this._header; }
            set
            {
                this._header = value;
                this.OnPropertyChanged(this, "Header");
            }
        }

        protected string _findlabeltext;
        public string FindLabelText
        {
            get { return this._findlabeltext; }
            set
            {
                this._findlabeltext = value;
                this.OnPropertyChanged(this, "FindLabelText");
            }
        }

        public BasePane(SccmConnector connector)
        {
            this._connector = connector;
            this._pane = new ResourceTabControl();
            MsaglHelpers.ConfigureGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.searchbtn.Click += this.OnFindButtonPressed;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            this._pane.abortbtn.Click += OnAbortButtonClick;
        }

        //protected async Task<Graph> AsyncFindCollectionID(string collectionid, string mode)
        //{
        //    return this.FindCollectionID(collectionid, mode);
        //}

        public Graph FindCollectionID(string collectionid, string mode)
        { 
            this.ClearHighlightedCollections();
            Graph graph = null;
            if (string.IsNullOrWhiteSpace(collectionid) == false)
            {
                SccmCollection col = this._library.GetCollection(collectionid);
                if (col != null)
                {
                    if (mode == "Context")
                    {
                        if (this._filteredview == true) { graph = TreeBuilder.BuildTreeAllCollections(this._library); }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else if (mode == "Mesh")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildTreeMeshMode(this._connector, this._library, collectionid);
                    }
                    else if (mode == "Limiting")
                    {
                        this._filteredview = true;
                        graph = TreeBuilder.BuildTreeLimitingMode(this._library, collectionid);
                    }
                    col.IsHighlighted = true;
                }
            }
            else
            {
                if (this._filteredview == true)
                {
                    this._filteredview = false;
                }
                graph = TreeBuilder.BuildTreeAllCollections(this._library);
            }
            return graph;
        }

        protected Graph BuildTreeAllCollections()
        {
            this.ClearHighlightedCollections();

            //build the user graph
            this._graph = TreeBuilder.BuildLimitingPath(this._library.GetAllCollections(),this._library);

            return this._graph;
        }

        protected void OnTextBoxFocused(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        protected void UpdatePaneToTabControl()
        {
            this._pane.gviewer.Graph = this._graph;
        }

        protected void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
                Node node = this._graph.FindNode(col.ID);
            }
            this._highlightedcollections.Clear();
        }

        protected void OnProgressUpdate(object sender, EventArgs e)
        {
            if (this._progresscount < 2 ) {
                this.NotificationText = this.NotificationText + ".";
                this._progresscount++;
            }
            else { this.NotificationText = null; }
            
        }

        protected void OnAbortButtonClick(object sender, RoutedEventArgs e)
        {
            this._pane.gviewer.AbortAsyncLayout();
            this.NotificationText = "Build aborted";
            this._progresscount = 0;
            this.NotificationText = null;
        }

        protected async void OnBuildButtonPressed(object sender, RoutedEventArgs e)
        {
            this.NotificationText = "Building";
            this.ClearHighlightedCollections();
            this._graph = await this.BuildGraph(this._collectiontext, this._pane.modecombo.Text);
            this.UpdatePaneToTabControl();
            this.NotificationText = null;
        }

        protected Task<Graph> BuildGraph(string collectionid, string mode)
        {
            return Task.Run(() =>
            {
                return this.FindCollectionID(collectionid, mode);
            });
        }

        protected abstract void OnFindButtonPressed(object sender, RoutedEventArgs e);
        
        protected void Redraw()
        {
            this._pane.gviewer.Invalidate();
        }
    }
}
