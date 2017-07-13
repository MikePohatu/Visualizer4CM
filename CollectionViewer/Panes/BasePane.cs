using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System;
using Microsoft.Msagl.Drawing;
using viewmodel;
using System.Threading.Tasks;
using System.Threading;

namespace CollectionViewer.Panes
{
    public abstract class BasePane : ViewModelBase
    {
        protected SccmConnector _connector;
        protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = false;
        protected int _progresscount = 0;
        protected bool _building = false;

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

        protected bool _controlsenabled = true;
        public bool ControlsEnabled
        {
            get { return this._controlsenabled; }
            set
            {
                this._controlsenabled = value;
                this.OnPropertyChanged(this, "ControlsEnabled");
            }
        }

        // Constructor
        public BasePane(SccmConnector connector)
        {
            this._connector = connector;
            this._pane = new ResourceTabControl();
            MsaglHelpers.ConfigureGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.searchbtn.Click += this.OnFindButtonPressed;
            this._pane.searchtb.KeyUp += this.OnFindKeyUp;
            this._pane.buildtb.KeyUp += this.OnBuildKeyUp;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.gviewer.AsyncLayoutProgress += this.OnProgressUpdate;
            //this._pane.abortbtn.Click += OnAbortButtonClick;
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
            }
            this._highlightedcollections.Clear();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected async void BuildGraph()
        {
            this.ControlsEnabled = false;
            this._building = true;
            this.ClearHighlightedCollections();
            string mode = this._pane.modecombo.Text;
            Task.Run(() => this.NotifyProgress("Building"));
            await Task.Run(() => this._graph = this.FindCollectionID(this._collectiontext, mode));
            await Task.Run(() => this.UpdatePaneToTabControl());
            this._building = false;
            this.ControlsEnabled = true;
        }

        /// <summary>
        /// Adds notification with dots slowly working. Adds the dots to the specified string
        /// </summary>
        /// <param name="basetext"></param>
        protected void NotifyProgress(string basetext)
        {
            int count = 0;
            this.NotificationText = basetext;

            while (this._building == true)
            {
                if (count < 5)
                {
                    this.NotificationText = this.NotificationText + ".";
                    count++;
                }
                else
                {
                    this.NotificationText = basetext;
                    count = 0;
                }
                Thread.Sleep(500);
            }
            this.NotificationText = null;
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
        protected void OnBuildKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.BuildGraph();
            }
        }

        protected void OnBuildButtonPressed(object sender, RoutedEventArgs e)
        {
            this.BuildGraph();
        }

        protected void OnProgressUpdate(object sender, EventArgs e)
        {
            if (this._progresscount < 2)
            {
                this.NotificationText = this.NotificationText + ".";
                this._progresscount++;
            }
            else { this.NotificationText = null; }

        }

        //protected void OnAbortButtonClick(object sender, RoutedEventArgs e)
        //{
        //    this._pane.gviewer.AbortAsyncLayout();
        //    this.NotificationText = "Build aborted";
        //    this._progresscount = 0;
        //    this.NotificationText = null;
        //}

        protected void Redraw()
        {
            this._pane.gviewer.Invalidate();
        }
    }
}
