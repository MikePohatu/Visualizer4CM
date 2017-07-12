using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace CollectionViewer.Panes
{
    public abstract class BasePane: ViewModelBase
    {
        protected SccmConnector _connector;
        protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = false;

        protected Graph _graph;
        public Graph Graph { get { return this._graph; } }

        protected ResourcePage _page;
        public Page Page { get { return this._page; } }

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
            //this._library = connector.DeviceCollectionLibrary;
            this._page = new ResourcePage();
            MsaglHelpers.ConfigureGViewer(this._page.gviewer);
            this._page.DataContext = this;
            this._page.searchbtn.Click += this.OnFindButtonPressed;
            this._page.buildbtn.Click += this.OnBuildButtonPressed;
            this._page.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            this._page.gviewer.GraphLoadingEnded += this.OnProgressFinished;
            this._page.abortbtn.Click += OnAbortButtonClick;
        }

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
            this._page.gviewer.Graph = this._graph;
        }

        protected void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
                Node node = this._graph.FindNode(col.ID);
                //this._page.gviewer.Invalidate();
            }
            this._highlightedcollections.Clear();
        }

        protected void OnProgressUpdate(object sender, EventArgs e)
        {
            this.NotificationText = this.NotificationText + ".";
        }

        protected void OnProgressFinished(object sender, EventArgs e)
        {
            this.NotificationText = null;
        }

        protected void OnAbortButtonClick(object sender, RoutedEventArgs e)
        {
            this._page.gviewer.AbortAsyncLayout();
            this.NotificationText = "Build aborted";
        }

        protected void OnBuildButtonPressed(object sender, RoutedEventArgs e)
        {
            this._graph = null;
            this.NotificationText = "Building.";
            this.ClearHighlightedCollections();
            this._graph = this.FindCollectionID(this._collectiontext, this._page.modecombo.Text);
            this.UpdatePaneToTabControl();
        }

        protected abstract void OnFindButtonPressed(object sender, RoutedEventArgs e);
    }
}
