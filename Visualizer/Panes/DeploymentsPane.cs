using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using viewmodel;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public class DeploymentsPane : BasePane
    {
        //protected CollectionLibrary _library;
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = true;

        protected DeploymentsTabControl _pane;
        public DeploymentsTabControl Pane { get { return this._pane; } }

        protected CollectionType CollectionsType { get; set; }

        public DeploymentsPane(SccmConnector connector):base(connector)
        {
            this._pane = new DeploymentsTabControl();
            MsaglHelpers.ConfigureCollectionsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.gviewer.MouseClick += this.OnGViewerMouseClicked;
            this._pane.gviewer.MouseDoubleClick += this.OnGViewerMouseDoubleClick;
            //this._pane.abortbtn.Click += OnAbortButtonClick;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            this._pane.searchtb.KeyUp += this.OnSearchKeyUp;

            this.CollectionsType = CollectionType.Device;
            this._header = "Deployments";
            this._findlabeltext = "blah:";
            //this._library = connector.GetAllCollectionsLibrary();
        }

        protected void ClearHighlightedCollections()
        {
            foreach (SccmCollection col in this._highlightedcollections)
            {
                col.IsHighlighted = false;
            }
            this._highlightedcollections.Clear();
        }

        protected void OnSearchButtonPressed(object sender, RoutedEventArgs e) { this.UpdateSearchResults(); }
        protected void OnSearchKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.UpdateSearchResults();
            }
        }

        protected virtual void OnGViewerMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            GViewer viewer = (GViewer)sender;
            object selected = viewer.SelectedObject;
            if (selected != null)
            {
                this.SelectedNode = selected as SccmNode;
                if (this.SelectedNode != null)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.SearchText = this.SelectedNode.SccmObject.Name;
                        if (this.SelectedNode.SccmObject is SccmCollection) { this._pane.modecombo.Text = "Collection"; }
                        else if (this.SelectedNode.SccmObject is SccmApplication) { this._pane.modecombo.Text = "Application"; }
                        else if (this.SelectedNode.SccmObject is SccmSoftwareUpdate) { this._pane.modecombo.Text = "Update"; }
                        else if (this.SelectedNode.SccmObject is SccmDeploymentSummary) { this._pane.modecombo.Text = "Deployment"; }
                    }
                }               
            }
        }

        protected void UpdatePaneToTabControl()
        {
            this._pane.gviewer.Graph = this._graph;
        }

        protected void Redraw()
        {
            this._pane.gviewer.Invalidate();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected async void UpdateSearchResults()
        {
            this.ControlsEnabled = false;
            this._processing = true;

            Task.Run(() => this.NotifyProgress("Searching"));
            if (this._pane.modecombo.Text == "Collection")
            { await Task.Run(() => this.SearchResults = this._connector.GetCollectionSccmObjectsFromSearch(this._searchtext, this.CollectionsType)); }

            else if (this._pane.modecombo.Text == "Deployment")
            { await Task.Run(() => this.SearchResults = this._connector.GetDeploymentSccmObjectsFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Application")
            { await Task.Run(() => this.SearchResults = this._connector.GetApplicationsSccmObjectsListFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Update Group")
            { await Task.Run(() => this.SearchResults = this._connector.GetSoftwareUpdateSccmObjectsFromSearch(this._searchtext)); }

            this._processing = false;
            this.ControlsEnabled = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected override async void BuildGraph()
        {
            if (this._selectedresult != null)
            {
                string mode = this._pane.modecombo.Text;
                //string id = this._selectedresult?.ID;

                if (this._selectedresult != null)
                {
                    this.ControlsEnabled = false;
                    this._processing = true;
                    this.ClearHighlightedCollections();

                    Task.Run(() => this.NotifyProgress("Querying server"));
                    List<SccmDeploymentSummary> deployments = null;
                    if (this._selectedresult is SccmCollection)
                    {
                        await Task.Run(() => deployments = this._connector.GetCollectionDeployments(this._selectedresult.ID));
                    }
                    else if ((this._selectedresult is SccmDeployableItem) || (this._selectedresult is SccmApplication) || (this._selectedresult is SccmSoftwareUpdate))
                    {
                        await Task.Run(() => deployments = this._connector.GetSoftwareItemDeployments(this._selectedresult.Name));
                    }

                    this.UpdateProgressMessage("Building tree");
                    await Task.Run(() => 
                    {
                        if (this._selectedresult != null)
                        {
                            if (this._selectedresult is SccmCollection) { this._graph = TreeBuilder.BuildCollectionDeploymentsTree(this._connector, (SccmCollection)this._selectedresult, deployments); }
                            else { this._graph = TreeBuilder.BuildCIDeploymentsTree(this._connector, this._selectedresult, deployments); }

                            this._selectedresult.IsHighlighted = true;
                        }
                    });

                    this.UpdateProgressMessage("Building graph");
                    await Task.Run(() => this.UpdatePaneToTabControl());
                    this._processing = false;
                    this.ControlsEnabled = true;
                }
                else { this.NotificationText = "Nothing selected"; }
            }
            else
            {
                this.NotificationText = "Please select a search result";
            }
        }
    }
}
