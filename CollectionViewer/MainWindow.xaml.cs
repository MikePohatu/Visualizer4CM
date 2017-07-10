﻿using System;
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
                bool connected = this.TryConnect(this._connector, loginviewmodel, loginwindow);
                if (connected == true)
                {
                    loginwindow.Close();
                    this._site = loginviewmodel.Site;
                    this._connector.Query(this._site);
                    Graph[] graphs = this.BuildTreeAllCollections(this._connector);
                    MsaglHelpers.ConfigureGViewer(this.deviceColViewer);
                    MsaglHelpers.ConfigureGViewer(this.userColViewer);
                    this.deviceColViewer.Graph = graphs[0];
                    this.userColViewer.Graph = graphs[1];
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
                        if (this._filteredview == true) { graphs = this.BuildTreeAllCollections(this._connector); }
                        this._filteredview = false;
                        this._highlightedcollections = col.HighlightCollectionPathList();
                    }
                    else
                    {
                        this._filteredview = true;
                        graphs = this.BuildGraphCollectionRelationships(this._connector, collectionid);
                    }
                }
            }
            this.deviceColViewer.Graph = graphs[0];
            this.userColViewer.Graph = graphs[1];
        }

        private Graph[] BuildTreeAllCollections(SccmConnector connector)
        {
            Graph[] graphs = new Graph[2];
            graphs[0] = new Graph("devgraph");
            graphs[1] = new Graph("usergraph");

            //build the device graph
            this._devlibrary = connector.DeviceCollectionLibrary;
            this.BuildGraph(graphs[0], this._devlibrary.GetAllCollections());

            //build the user graph
            this._userlibrary = connector.UserCollectionLibrary;
            this.BuildGraph(graphs[1], this._userlibrary.GetAllCollections());

            return graphs;
        }

        private Graph[] BuildTreeCollectionPath(SccmConnector connector, string collectionid)
        {
            this.ClearHighlightedCollections();
            SccmCollection searchcol;

            Graph[] graphs = new Graph[2];
            graphs[0] = new Graph("devgraph");
            graphs[1] = new Graph("usergraph");

            //build the device graph
            this._devlibrary = connector.DeviceCollectionLibrary;
            searchcol = this._devlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildGraph(graphs[0], searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
            }
            
            //build the user graph          
            this._userlibrary = connector.UserCollectionLibrary;
            searchcol = this._userlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildGraph(graphs[1], searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
            }

            return graphs;            
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

        private Graph[] BuildGraphCollectionRelationships(SccmConnector connector, string collectionid)
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
                this.BuildGraph(graphs[0], searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
                this.AddIncExclRelationshipEdges(relationships, graphs[0], connector.DeviceCollectionLibrary);
            }

            //build the user graph          
            searchcol = connector.UserCollectionLibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                this.BuildGraph(graphs[1], searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
                this.AddIncExclRelationshipEdges(relationships, graphs[1], connector.UserCollectionLibrary);
            }

            return graphs;
        }

        private void AddIncExclRelationshipEdges(List<SccmCollectionRelationship> relationships, Graph graph, CollectionLibrary library)
        {
            foreach (SccmCollectionRelationship colrel in relationships)
            {
                if (colrel.Type != SccmCollectionRelationship.RelationShipType.Limiting)
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
                    }

                    Edge newedge = graph.AddEdge(colrel.SourceCollectionID, colrel.Type.ToString() , colrel.DependentCollectionID);

                    if (colrel.Type == SccmCollectionRelationship.RelationShipType.Exclude)
                    { newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Red; }
                    else if (colrel.Type == SccmCollectionRelationship.RelationShipType.Include)
                    { newedge.Attr.Color = Microsoft.Msagl.Drawing.Color.Blue; }
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
