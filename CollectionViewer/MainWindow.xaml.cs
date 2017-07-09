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
        private List<SccmCollection> _highlightedcollections;
        private SccmConnector _connector;
        private bool _filteredview;

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
                    this._connector.Query(loginviewmodel.Site);
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

            if (string.IsNullOrWhiteSpace(collectionid) == false)
            {
                SccmCollection col = this._devlibrary.GetCollection(collectionid);
                if (col != null)
                {
                    //workaround for nodes not redrawing on update. Swap to other tab then back
                    this.maintabctrl.SelectedItem = this.usertab;
                    this.maintabctrl.SelectedItem = this.devtab;
                }
                else
                {
                    col = this._userlibrary.GetCollection(collectionid);
                    if (col != null )
                    {
                        //workaround for nodes not redrawing on update. Swap to other tab then back
                        this.maintabctrl.SelectedItem = this.devtab;
                        this.maintabctrl.SelectedItem = this.usertab;
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
                        this.BuildTreeCollectionPath(this._connector, collectionid);
                    }
                }
            }
            this.UpdateLayout();
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

        private void BuildTreeCollectionPath(SccmConnector connector, string collectionid)
        {
            this.ClearHighlightedCollections();

            MsaglHelpers.ConfigureGViewer(deviceColViewer);
            MsaglHelpers.ConfigureGViewer(userColViewer);
            SccmCollection searchcol;

            //build the device graph
            Graph devgraph = new Graph("devgraph");
            this._userlibrary = connector.DeviceCollectionLibrary;
            searchcol = this._devlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                BuildGraph(devgraph, searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
            }           

            //build the user graph
            Graph usergraph = new Graph("usergraph");
            this._userlibrary = connector.UserCollectionLibrary;
            searchcol = this._userlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                BuildGraph(usergraph, searchcol.GetCollectionPathList());
                this.HighlightCollection(searchcol);
            }         

            this.deviceColViewer.Graph = devgraph;
            this.userColViewer.Graph = usergraph;
        }

        private void BuildGraph(Graph graph, List<SccmCollection> collections)
        {
            Dictionary<string, object> foundcols = new Dictionary<string, object>();
            foreach (SccmCollection col in collections)
            {
                object outob;
                if (foundcols.TryGetValue(col.ID, out outob) == false)
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
