using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Msagl.Drawing;
using System;
using viewmodel;
using CollectionViewer.Panes;
using CollectionViewer.Auth;

namespace CollectionViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph[] _graphs = new Graph[2];
        private UserPane _userpane;
        private DevicePane _devicepane;
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
            this._connector.QueryAll(this._site);
            this._userpane = new UserPane(this._connector);

            this._devicepane = new DevicePane(this._connector);
            this._devicepane.RedrawRequired += this.OnRedrawRequired;
            TabItem devtabitem = new TabItem();
            //devtabitem.DataContext = this._devicepane;
            devtabitem.Header = this._devicepane.Header;
            Frame devtabtrame = new Frame();
            devtabtrame.Content = this._devicepane.Page;
            devtabitem.Content = devtabtrame;
            maintabctrl.Items.Add(devtabitem);


            this._userpane = new UserPane(this._connector);
            this._userpane.RedrawRequired += this.OnRedrawRequired;
            TabItem usertabitem = new TabItem();
            //devtabitem.DataContext = this._devicepane;
            usertabitem.Header = this._userpane.Header;
            Frame usertabframe = new Frame();
            usertabframe.Content = this._userpane.Page;
            usertabitem.Content = usertabframe;
            maintabctrl.Items.Add(usertabitem);
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

        //workaround to make tab redraw
        private void OnRedrawRequired(object sender, EventArgs e)
        {
            object tab = this.maintabctrl.SelectedItem;
            this.maintabctrl.SelectedItem = null;
            this.maintabctrl.SelectedItem = tab;
        }
    }
}
