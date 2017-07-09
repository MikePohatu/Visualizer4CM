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

        public MainWindow()
        {
            InitializeComponent();
            this.Start();
        }

        private void Start()
        {
            this._connector = new SccmConnector();
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
            if (this._highlightedcollections != null) {
                foreach (SccmCollection hlcol in this._highlightedcollections) { hlcol.IsHighlighted = false; }
                this._highlightedcollections.Clear();
            }
            if (string.IsNullOrWhiteSpace(collectionid) == true) { this.BuildTreeAllCollections(this._connector); }
            else { this.BuildTreeCollectionPath(this._connector, collectionid); }

                //SccmCollection col = this._devlibrary.GetCollection(collectionid);
                //if (col == null ) { col = this._userlibrary.GetCollection(collectionid); }
                //if (col != null)
                //{
                //    this._highlightedcollections = col.HighlightCollectionPathList();
                //}
        }

        private void BuildTreeAllCollections(SccmConnector connector)
        {
            MsaglHelpers.ConfigureGViewer(DeviceColViewer);
            MsaglHelpers.ConfigureGViewer(UserColViewer);

            //build the device graph
            Graph devgraph = new Graph("devgraph");

            this._devlibrary = connector.DeviceCollectionLibrary;
            foreach (SccmCollection col in this._devlibrary.GetAllCollections())
            {
                CollectionNode newnode = new CollectionNode(col.ID, col);
                MsaglHelpers.ConfigureNode(newnode, col);
                devgraph.AddNode(newnode);

                if ((string.IsNullOrWhiteSpace(col.ID) == false) && (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false))
                {
                    devgraph.AddEdge(col.LimitingCollectionID, col.ID);
                }
            }
            this.DeviceColViewer.Graph = devgraph;

            //build the user graph
            Graph usergraph = new Graph("usergraph");

            this._userlibrary = connector.UserCollectionLibrary;
            foreach (SccmCollection col in this._userlibrary.GetAllCollections())
            {
                CollectionNode newnode = new CollectionNode(col.ID, col);
                MsaglHelpers.ConfigureNode(newnode, col);
                usergraph.AddNode(newnode);

                if ((string.IsNullOrWhiteSpace(col.ID) == false) && (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false))
                {
                    usergraph.AddEdge(col.LimitingCollectionID, col.ID);
                }
            }
            this.UserColViewer.Graph = usergraph;
        }

        private void BuildTreeCollectionPath(SccmConnector connector, string collectionid)
        {
            Dictionary<string, object> foundcols = new Dictionary<string, object>();
            bool devfound = false;
            bool userfound = false;

            MsaglHelpers.ConfigureGViewer(DeviceColViewer);
            MsaglHelpers.ConfigureGViewer(UserColViewer);
            SccmCollection searchcol;

            //build the device graph
            Graph devgraph = new Graph("devgraph");
            this._devlibrary = connector.DeviceCollectionLibrary;
            
            searchcol = this._devlibrary.GetCollection(collectionid);
            if (searchcol != null) {
                devfound = true;
                foreach (SccmCollection col in searchcol.GetCollectionPathList())
                {
                    object outob;
                    if (foundcols.TryGetValue(col.ID, out outob)==false)
                    {
                        CollectionNode newnode = new CollectionNode(col.ID, col);
                        MsaglHelpers.ConfigureNode(newnode, col);
                        devgraph.AddNode(newnode);
                    }
                    if ((string.IsNullOrWhiteSpace(col.ID) == false) && (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false))
                    { devgraph.AddEdge(col.LimitingCollectionID, col.ID); }

                }  
            }
            this.DeviceColViewer.Graph = devgraph;

            //build the user graph
            Graph usergraph = new Graph("usergraph");
            this._userlibrary = connector.UserCollectionLibrary;
            searchcol = this._userlibrary.GetCollection(collectionid);
            if (searchcol != null)
            {
                userfound = true;
                foreach (SccmCollection col in searchcol.GetCollectionPathList())
                {
                    object outob;
                    if (foundcols.TryGetValue(col.ID, out outob) == false)
                    {
                        CollectionNode newnode = new CollectionNode(col.ID, col);
                        MsaglHelpers.ConfigureNode(newnode, col);
                        usergraph.AddNode(newnode);
                    }
                    if ((string.IsNullOrWhiteSpace(col.ID) == false) && (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false))
                    { usergraph.AddEdge(col.LimitingCollectionID, col.ID); }

                } 
            }
            this.UserColViewer.Graph = usergraph;
        }
    }
}
