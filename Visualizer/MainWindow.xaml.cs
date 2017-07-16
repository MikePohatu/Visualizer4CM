using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Msagl.Drawing;
using System.Windows.Input;
using viewmodel;
using Visualizer.Panes;
using Visualizer.Auth;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph[] _graphs = new Graph[2];
        private UserPane _userpane;
        private DevicePane _devicepane;
        private ApplicationPane _apppane;
        private List<SccmCollection> _highlightedcollections;
        private SccmConnector _connector;
        private string _site;

        public MainWindow()
        {
            InitializeComponent();
            this.Login();
        }

        private void Login()
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
                if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); }
            };

            loginwindow.pwdbx.KeyUp += (sender, e) => {
                if (e.Key == Key.Enter)
                { if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); } }    
            };

            loginwindow.usertb.KeyUp += (sender, e) => {
                if (e.Key == Key.Enter)
                { if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); } }
            };

            loginwindow.domaintb.KeyUp += (sender, e) => {
                if (e.Key == Key.Enter)
                { if (loginviewmodel.TryConnect(this._connector, loginwindow.pwdbx.Password) == true) { this.Startup(loginwindow, loginviewmodel); } }
            };

            loginwindow.ShowDialog();
        }

        private void Startup(LoginWindow loginwindow, LoginViewModel loginviewmodel)
        {
            loginwindow.Close();
            this._site = loginviewmodel.Site;
            this._connector.QueryAll(this._site);

            this._devicepane = new DevicePane(this._connector);
            TabItem devtabitem = new TabItem();
            devtabitem.Header = this._devicepane.Header;
            devtabitem.Content = this._devicepane.Pane;
            maintabctrl.Items.Add(devtabitem);


            this._userpane = new UserPane(this._connector);
            TabItem usertabitem = new TabItem();
            usertabitem.Header = this._userpane.Header;
            usertabitem.Content = this._userpane.Pane;
            maintabctrl.Items.Add(usertabitem);

            this._apppane = new ApplicationPane(this._connector);
            TabItem apptabitem = new TabItem();
            apptabitem.Header = this._apppane.Header;
            apptabitem.Content = this._apppane.Pane;
            maintabctrl.Items.Add(apptabitem);
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
    }
}
