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

        public MainWindow()
        {
            InitializeComponent();
            this.Start();
        }

        private void Start()
        {
            SccmConnector connector = new SccmConnector();
            LoginViewModel loginviewmodel = new LoginViewModel();
            LoginWindow loginwindow = new LoginWindow();

            loginwindow.DataContext = loginviewmodel;

            loginwindow.cancelbtn.Click += (sender, e) => {
                loginwindow.Close();
                this.Close();
            };

            loginwindow.okbtn.Click += (sender, e) => {
                bool connected = this.TryConnect(connector, loginviewmodel, loginwindow);
                if (connected == true)
                {
                    loginwindow.Close();
                    connector.Query(loginviewmodel.Site);

                    MsaglHelpers.ConfigureGViewer(DeviceColViewer);
                    MsaglHelpers.ConfigureGViewer(UserColViewer);

                    Graph devgraph = new Graph("graph");

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
                    Graph usergraph = new Graph("graph");

                    this._userlibrary = connector.DeviceCollectionLibrary;
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
            };

            loginwindow.ShowDialog();
            
        }

        public bool TryConnect(SccmConnector connector, LoginViewModel loginviewmodel, LoginWindow loginwindow)
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

            SccmCollection col = this._devlibrary.GetCollection(collectionid);
            if (col == null ) { col = this._userlibrary.GetCollection(collectionid); }
            if (col != null)
            {
                this._highlightedcollections = col.HighlightCollectionPath();
            }
            //this.UserColViewer.
        }
    }
}
