using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using viewmodel;

namespace CollectionViewer.Panes
{
    public class UserPane: ViewModelBase
    {
        private SccmConnector _connector;
        private CollectionLibrary _library;
        private List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        private bool _filteredview = false;

        private Graph _graph;
        public Graph Graph { get { return this._graph; } }

        private ResourcePage _page;
        public Page Page { get { return this._page; } }

        private string _searchtext;
        public string SearchText
        {
            get { return this._searchtext; }
            set
            {
                this._searchtext = value;
                this.OnPropertyChanged(this, "SearchText");
            }
        }

        private string _collectiontext;
        public string CollectionText
        {
            get { return this._collectiontext; }
            set
            {
                this._collectiontext = value;
                this.OnPropertyChanged(this, "CollectionText");
            }
        }

        private string _resourcetext;
        public string ResourceText
        {
            get { return this._resourcetext; }
            set
            {
                this._resourcetext = value;
                this.OnPropertyChanged(this, "ResourceText");
            }
        }

        private string _header = "User Collections";
        public string Header
        {
            get { return this._header; }
            set
            {
                this._header = value;
                this.OnPropertyChanged(this, "Header");
            }
        }

        private string _label;
        public string Label
        {
            get { return this._label; }
            set
            {
                this._label = value;
                this.OnPropertyChanged(this, "Label");
            }
        }

        public UserPane(SccmConnector connector)
        {
            this._connector = connector;
            this._library = connector.UserCollectionLibrary;
            this._page = new ResourcePage();
            MsaglHelpers.ConfigureGViewer(this._page.colviewer);
            this._page.DataContext = this;
            this._page.searchbtn.Click += this.OnFindButtonPressed;
            this._page.buildbtn.Click += this.OnBuildButtonPressed;
        }

        public void FindCollectionID(string collectionid, string mode)
        {
            TreeBuilder.ClearHighlightedCollections(this._highlightedcollections);

            if (string.IsNullOrWhiteSpace(collectionid) == false)
            {
                SccmCollection col = this._library.GetCollection(collectionid);
                if (col != null)
                {
                    if (mode == "Context")
                    {
                        if (this._filteredview == true) { this._graph = TreeBuilder.BuildTreeAllCollections(this._library); }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else if (mode == "Mesh")
                    {
                        this._filteredview = true;
                        this._graph = TreeBuilder.BuildTreeMeshMode(this._connector, this._library, collectionid);
                    }
                    else if (mode == "Limiting")
                    {
                        this._filteredview = true;
                        this._graph = TreeBuilder.BuildTreeLimitingMode(this._library, collectionid);
                    }
                }
            }
            else
            {
                if (this._filteredview == true)
                {
                    this._filteredview = false;
                }
                this._graph = TreeBuilder.BuildTreeAllCollections(this._library);
            }
        }

        private Graph BuildTreeAllCollections()
        {
            TreeBuilder.ClearHighlightedCollections(this._highlightedcollections);
            this._graph = new Graph();

            //build the user graph
            this._graph = TreeBuilder.BuildLimitingPath(this._library.GetAllCollections(), this._library);

            return this._graph;
        }

        private void FindMemberships()
        {
            if (string.IsNullOrWhiteSpace(this._searchtext) == false)
            {
                //SccmDevice dev = this._connector.GetUser(this._searchtext.Trim());
                //TreeBuilder.HighlightCollectionMembers(this._graph, dev.CollectionIDs);
            }
        }

        private void OnBuildButtonPressed(object sender, RoutedEventArgs e)
        {
            this.FindCollectionID(this._collectiontext, this._page.modecombo.Text);
            this.UpdatePaneToTabControl();
        }

        private void OnFindButtonPressed(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this._resourcetext) == false)
            {
                SccmDevice dev = this._connector.GetDevice(this._resourcetext.Trim());
                TreeBuilder.HighlightCollectionMembers(this._graph, dev.CollectionIDs);
            }
        }

        private void OnTextBoxFocused(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void UpdatePaneToTabControl()
        {
            this._page.colviewer.Graph = this._graph;
        }
    }
}
