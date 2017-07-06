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
            SccmConnector connector;
            LoginViewModel login = new LoginViewModel();
            if (login.PassThrough == true) { connector = new SccmConnector("syscenter03.home.local","001"); }
            else { connector = new SccmConnector(login.Username, login.Password, login.Domain, login.Server, "001"); }
            
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
        }
    }
}
