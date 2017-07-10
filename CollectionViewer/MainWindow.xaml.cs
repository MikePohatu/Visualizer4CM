using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using model;
using CollectionViewer.Auth;

namespace CollectionViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CollectionLibrary _devlibrary;
        private CollectionLibrary _userlibrary;
        private Graph[] _graphs = new Graph[2];
        private List<SccmCollection> _highlightedcollections;
        private SccmConnector _connector;
        private bool _filteredview;
        private string _site;

        public MainWindow()
        {
            InitializeComponent();
            this.Start();
        }

        private void Start()
        {
            this._connector = new SccmConnector();
            this._highlightedcollections = new List<SccmCollection>();
            LoginViewModel loginviewmodel = new LoginViewModel();
            LoginWindow loginwindow = new LoginWindow();

            loginwindow.DataContext = loginviewmodel;

            loginwindow.cancelbtn.Click += (sender, e) => {
                loginwindow.Close();
                this.Close();
            };

            loginwindow.okbtn.Click += (sender, e) => {
                if (this.TryConnect(this._connector, loginviewmodel, loginwindow) == true)
                { this.Startup(loginwindow, loginviewmodel); }
            };

            //loginwindow.pwdbx.Enter

            loginwindow.ShowDialog();
            
        }

        private void Startup(LoginWindow loginwindow, LoginViewModel loginviewmodel)
        {
            loginwindow.Close();
            this._site = loginviewmodel.Site;
            this._connector.Query(this._site);
            this._devlibrary = this._connector.DeviceCollectionLibrary;
            this._userlibrary = this._connector.UserCollectionLibrary;
            Graph[] graphs = this.BuildTreeAllCollections(this._connector);
            MsaglHelpers.ConfigureGViewer(this.deviceColViewer);
            MsaglHelpers.ConfigureGViewer(this.userColViewer);
            this.deviceColViewer.Graph = graphs[0];
            this.userColViewer.Graph = graphs[1];

        }

        private bool TryConnect(SccmConnector connector, LoginViewModel loginviewmodel, LoginWindow loginwindow)
        {
            bool connected = false;

            if (loginviewmodel.PassThrough == true) { connected = connector.Connect(loginviewmodel.Server); }
            else { connected = connector.Connect(loginviewmodel.Username, loginwindow.pwdbx.Password, loginviewmodel.Domain, loginviewmodel.Server); }

            ToolTip tt = (ToolTip)loginwindow.maingrid.ToolTip;

            if (connected == false)
            {
                loginviewmodel.NotifyMessage = loginviewmodel.DeniedMessage;
                tt.IsOpen = true;
            }
            else
            {
                loginviewmodel.NotifyMessage = null;
                tt.IsOpen = false;
            }

            return connected;
        }

        private void OnFindButtonPressed(object sender, RoutedEventArgs e)
        {
            this.FindCollectionID(this.searchcoltb.Text);
        }

        private void OnTextBoxFocused(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void FindCollectionID(string collectionid)
        {
            Graph[] graphs = new Graph[2];
            this.ClearHighlightedCollections();
            CollectionLibrary library = null;
            if (string.IsNullOrWhiteSpace(collectionid) == false)
            {
                SccmCollection col = this._devlibrary.GetCollection(collectionid);
                if (col != null)
                {
                    //workaround for nodes not redrawing on update. Swap to other tab then back
                    this.maintabctrl.SelectedItem = this.usertab;
                    this.maintabctrl.SelectedItem = this.devtab;
                    library = this._devlibrary;
                }
                else
                {
                    col = this._userlibrary.GetCollection(collectionid);
                    if (col != null)
                    {
                        //workaround for nodes not redrawing on update. Swap to other tab then back
                        this.maintabctrl.SelectedItem = this.devtab;
                        this.maintabctrl.SelectedItem = this.usertab;
                        library = this._userlibrary;
                    }
                }

                if (col != null)
                {
                    if (modecombo.Text == "Context")
                    {
                        if (this._filteredview == true) { this.UpdateGraphs(this.BuildTreeAllCollections(this._connector)); }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else if (modecombo.Text == "Mesh")
                    {
                        this._filteredview = true;
                        graphs = this.BuildTreeMeshMode(this._connector, collectionid);
                        this.UpdateGraphs(graphs);
                    }
                    else if (modecombo.Text == "Limiting")
                    {
                        this._filteredview = true;
                        graphs = this.BuildTreeLimitingMode(this._connector, collectionid);
                        this.UpdateGraphs(graphs);
                    }
                }
            }
            else
            {
                if (this._filteredview == true)
                {
                    this._filteredview = false;
                }
                graphs = this.BuildTreeAllCollections(this._connector);
                this.UpdateGraphs(graphs);
            }

            if (string.IsNullOrWhiteSpace(this.searchdevtb.Text) == false)
            {
                SccmDevice dev = this._connector.GetDevice(this._site, this.searchdevtb.Text.Trim());
                this.HighlightCollectionMembers(graphs, dev.CollectionIDs);
            }
        }

        private void UpdateGraphs(Graph[] graphs)
        {
            this.deviceColViewer.Graph = graphs[0];
            this.userColViewer.Graph = graphs[1];
        }

        private void HighlightCollectionMembers(Graph[] graphs, List<string> collectionids)
        {
            foreach (Graph graph in graphs)
            {
                foreach (string colid in collectionids)
                {
                    CollectionNode node = graph?.FindNode(colid) as CollectionNode;
                    if (node != null)
                    {
                        this._highlightedcollections.Add(node.Collection);
                        node.Collection.IsMemberPresent = true;
                    }
                }
            }
        }

        private Graph[] BuildTreeAllCollections(SccmConnector connector)
        {
            this.ClearHighlightedCollections();
            Graph[] graphs = new Graph[2];
            graphs[0] = new Graph("devgraph");
            graphs[1] = new Graph("usergraph");

            //build the device graph
            this.BuildLimitingPath(graphs[0], connector.DeviceCollectionLibrary.GetAllCollections());

            //build the user graph
            this.BuildLimitingPath(graphs[1], connector.UserCollectionLibrary.GetAllCollections());

            return graphs;
        }

        private Graph[] BuildTreeLimitingMode(SccmConnector connector, string collectionid)
        {
            this.ClearHighlightedCollections();
            SccmCollection searchcol;

            Graph[] graphs = new Graph[2];
            graphs[0] = new Graph("devgraph");
            graphs[1] = new Graph("usergraph");

            //build the device graph
            searchcol = connector.DeviceCollectionLibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildLimitingPath(graphs[0], searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
            }
            
            //build the user graph          
            searchcol = connector.UserCollectionLibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildLimitingPath(graphs[1], searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
            }

            return graphs;            
        }

        private Graph[] BuildTreeMeshMode(SccmConnector connector, string collectionid)
        {
            this.ClearHighlightedCollections();
            SccmCollection searchcol;
            Graph[] graphs = new Graph[2];
            graphs[0] = new Graph("devgraph");
            graphs[1] = new Graph("usergraph");

            List<SccmCollectionRelationship> relationships = this._connector.GetCollectionDependencies(collectionid, this._site);

            //build the device graph
            searchcol = connector.DeviceCollectionLibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                graphs[0].AddNode(new CollectionNode(searchcol.ID, searchcol));
                this.HighlightCollection(searchcol);
                this.BuildMeshLinks(graphs[0], connector.DeviceCollectionLibrary, searchcol);
                //this.AddIncExclRelationshipEdges(relationships, graphs[0], connector.DeviceCollectionLibrary);
            }

            //build the user graph          
            searchcol = connector.UserCollectionLibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                graphs[1].AddNode(new CollectionNode(searchcol.ID, searchcol));
                this.HighlightCollection(searchcol);
                this.BuildMeshLinks(graphs[1], connector.UserCollectionLibrary, searchcol);
                //this.AddIncExclRelationshipEdges(relationships, graphs[1], connector.UserCollectionLibrary);
            }

            return graphs;
        }

        private void BuildMeshLinks(Graph graph,CollectionLibrary library, SccmCollection collection)
        {
            List<SccmCollectionRelationship> relationships = this._connector.GetCollectionDependencies(collection.ID, this._site);

            foreach (SccmCollectionRelationship colrel in relationships)
            {
                if (graph.FindNode(colrel.DependentCollectionID) == null)
                {
                    SccmCollection col = library.GetCollection(colrel.DependentCollectionID);
                    graph.AddNode(new CollectionNode(col.ID, col));
                }

                if (graph.FindNode(colrel.SourceCollectionID) == null)
                {
                    SccmCollection col = library.GetCollection(colrel.SourceCollectionID);
                    graph.AddNode(new CollectionNode(col.ID, col));
                    BuildMeshLinks(graph, library, col); //recursive build
                }

                Edge newedge = graph.AddEdge(colrel.DependentCollectionID, colrel.Type.ToString(), colrel.SourceCollectionID);

                if (colrel.Type == SccmCollectionRelationship.RelationShipType.Exclude)
                { newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Red; }
                else if (colrel.Type == SccmCollectionRelationship.RelationShipType.Include)
                { newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue; }
                else if (colrel.Type == SccmCollectionRelationship.RelationShipType.Limiting)
                { newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Black; }
            }
        }

        private void BuildLimitingPath(Graph graph, List<SccmCollection> collections)
        {
            foreach (SccmCollection col in collections)
            {
                Node colnode = graph.FindNode(col.ID);
                if (colnode == null)
                {
                    colnode = new CollectionNode(col.ID, col);
                    graph.AddNode(colnode);
                }

                if (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false)
                {
                    Node limitingnode = graph.FindNode(col.LimitingCollectionID);
                    if (limitingnode == null)
                    {
                        SccmCollection limcol = this._devlibrary.GetCollection(col.LimitingCollectionID);
                        limitingnode = new CollectionNode(col.ID, col);
                        graph.AddNode(limitingnode);
                    }

                    graph.AddEdge(col.ID, col.LimitingCollectionID);
                }
            }
        }

        private void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
            }
            this._highlightedcollections.Clear();
        }

        private void HighlightCollection(SccmCollection collection)
        {
            collection.IsHighlighted = true;
            this._highlightedcollections.Add(collection);
        }
    }
}
