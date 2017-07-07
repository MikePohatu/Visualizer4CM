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
                    connector.Query(loginviewmodel.Site);

                    MsaglHelpers.ConfigureGViewer(DeviceColViewer);
                    MsaglHelpers.ConfigureGViewer(UserColViewer);

                    Graph devgraph = new Graph("graph");
                    //The easiest way to build a graph is to create the edges of the graph like in the example below.
                    foreach (SccmCollection col in connector.DeviceCollectionLibrary.GetAllCollections())
                    {
                        Node newnode = new Node(col.ID);
                        MsaglHelpers.ConfigureNode(newnode, col);
                        devgraph.AddNode(newnode);

                        if ((string.IsNullOrWhiteSpace(col.ID) == false) && (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false))
                        {
                            devgraph.AddEdge(col.LimitingCollectionID, col.ID);
                        }
                    }
                    this.DeviceColViewer.Graph = devgraph;
                    Graph usergraph = new Graph("graph");
                    //The easiest way to build a graph is to create the edges of the graph like in the example below.
                    foreach (SccmCollection col in connector.UserCollectionLibrary.GetAllCollections())
                    {
                        Node newnode = new Node(col.ID);
                        MsaglHelpers.ConfigureNode(newnode, col);
                        usergraph.AddNode(newnode);

                        if ((string.IsNullOrWhiteSpace(col.ID) == false) && (string.IsNullOrWhiteSpace(col.LimitingCollectionID) == false))
                        {
                            usergraph.AddEdge(col.LimitingCollectionID, col.ID);
                        }
                    }
                    this.UserColViewer.Graph = usergraph;
                    loginwindow.Close();
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
                loginviewmodel.ToolTipMessage = loginviewmodel.DeniedMessage;
                tt.IsOpen = true;
            }
            else
            {
                loginviewmodel.ToolTipMessage = string.Empty;
                tt.IsOpen = false;
            }

            return connected;
        }
    }
}
