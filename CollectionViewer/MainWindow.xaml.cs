using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private Graph _usergraph;
        private Graph _devgraph;
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
                bool connected = this.TryConnect(this._connector, loginviewmodel, loginwindow);
                if (connected == true)
                {
                    loginwindow.Close();
                    this._site = loginviewmodel.Site;
                    this._connector.Query(this._site);
                    this.BuildTreeAllCollections(this._connector);
                }
            };

            loginwindow.ShowDialog();
            
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
                    if (col != null )
                    {
                        //workaround for nodes not redrawing on update. Swap to other tab then back
                        this.maintabctrl.SelectedItem = this.devtab;
                        this.maintabctrl.SelectedItem = this.usertab;
                        library = this._userlibrary;
                    }
                }

                if (col != null)
                {
                    if (isolatechk.IsChecked == false)
                    {
                        if (this._filteredview == true) { this.BuildTreeAllCollections(this._connector); }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else
                    {
                        this._filteredview = true;
                        Graph foundgraph = this.BuildTreeCollectionPath(this._connector, collectionid);
                        if (foundgraph != null)
                        { this.BuildGraphCollectionRelationships(foundgraph, library, this._connector.GetCollectionDependencies(col.ID, this._site)); }
                    }
                }
            }
        }

        private void BuildTreeAllCollections(SccmConnector connector)
        {
            MsaglHelpers.ConfigureGViewer(deviceColViewer);
            MsaglHelpers.ConfigureGViewer(userColViewer);

            //build the device graph
            Graph devgraph = new Graph("devgraph");

            this._devlibrary = connector.DeviceCollectionLibrary;
            this.BuildGraph(devgraph, this._devlibrary.GetAllCollections());
            this.deviceColViewer.Graph = devgraph;

            //build the user graph
            Graph usergraph = new Graph("usergraph");

            this._userlibrary = connector.UserCollectionLibrary;
            this.BuildGraph(usergraph, this._userlibrary.GetAllCollections());
            this.userColViewer.Graph = usergraph;
        }

        private Graph BuildTreeCollectionPath(SccmConnector connector, string collectionid)
        {
            Graph returngraph = null;
            this.ClearHighlightedCollections();

            MsaglHelpers.ConfigureGViewer(deviceColViewer);
            MsaglHelpers.ConfigureGViewer(userColViewer);
            SccmCollection searchcol;

            Graph devgraph = new Graph("devgraph");
            Graph usergraph = new Graph("usergraph");

            //build the device graph
            this._devlibrary = connector.DeviceCollectionLibrary;
            searchcol = this._devlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildGraph(devgraph, searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
                returngraph = devgraph;
            }
            this.deviceColViewer.Graph = devgraph;
            
            //build the user graph          
            this._userlibrary = connector.UserCollectionLibrary;
            searchcol = this._userlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildGraph(usergraph, searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
                returngraph = usergraph;
            }
            this.userColViewer.Graph = usergraph;

            return returngraph;            
        }

        private void BuildGraph(Graph graph, List<SccmCollection> collections)
        {
            foreach (SccmCollection col in collections)
            {
                if (graph.FindNode(col.ID) == null)
                {
                    CollectionNode newnode = new CollectionNode(col.ID, col);
                    graph.AddNode(newnode);
                }

                if (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false)
                {
                    if (graph.FindNode(col.LimitingCollectionID) == null)
                    {
                        SccmCollection limcol = this._devlibrary.GetCollection(col.LimitingCollectionID);
                        CollectionNode newlimnode = new CollectionNode(col.ID, col);
                        graph.AddNode(newlimnode);
                    }
                    graph.AddEdge(col.LimitingCollectionID, col.ID);
                }
            }
        }

        private void BuildGraphCollectionRelationships(Graph graph, CollectionLibrary library, List<SccmCollectionRelationship> relationships)
        {
            foreach (SccmCollectionRelationship colrel in relationships)
            {
                Node sourcenode = graph.FindNode(colrel.SourceCollectionID);
                Node targetnode = graph.FindNode(colrel.DependentCollectionID);

                if (sourcenode == null)
                {
                    SccmCollection col = library.GetCollection(colrel.SourceCollectionID);
                    sourcenode = new CollectionNode(col.ID, col);
                    graph.AddNode(sourcenode);
                }

                if (targetnode == null)
                {
                    SccmCollection col = library.GetCollection(colrel.SourceCollectionID);
                    targetnode = new CollectionNode(col.ID, col);
                    graph.AddNode(targetnode);
                }

                Edge newedge = new Edge(sourcenode, targetnode, ConnectionToGraph.Connected);

                if (colrel.Type == SccmCollectionRelationship.RelationShipType.Exclue)
                {  
                    newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Red;
                    //graph.AddEdge(colrel.DependentCollectionID, colrel.SourceCollectionID);
                }
                else if (colrel.Type == SccmCollectionRelationship.RelationShipType.Include)
                { newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue; }
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
