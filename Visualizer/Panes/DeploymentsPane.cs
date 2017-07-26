using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Msagl.GraphViewerGdi;
using viewmodel;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace Visualizer.Panes
{
    public class DeploymentsPane : BasePane
    {
        protected List<SccmCollection> _highlightedcollections = new List<SccmCollection>();
        protected bool _filteredview = true;

        protected DeploymentsTabControl _pane;
        public DeploymentsTabControl Pane { get { return this._pane; } }

        protected CollectionType CollectionsType { get; set; }

        protected List<ISccmObject> _searchresults;
        public List<ISccmObject> SearchResults
        {
            get { return this._searchresults; }
            set
            {
                this._searchresults = value;
                this.OnPropertyChanged(this, "SearchResults");
            }
        }

        protected ISccmObject _selectedresult;
        public ISccmObject SelectedResult
        {
            get { return this._selectedresult; }
            set
            {
                this._selectedresult = value;
                this.OnPropertyChanged(this, "SelectedResult");
            }
        }

        public DeploymentsPane(SccmConnector connector):base(connector)
        {
            this._pane = new DeploymentsTabControl();
            MsaglHelpers.ConfigureCollectionsGViewer(this._pane.gviewer);
            this._pane.DataContext = this;
            this._pane.buildbtn.Click += this.OnBuildButtonPressed;
            this._pane.searchresultslb.MouseDoubleClick += this.OnSearchResultsListDoubleClick;
            this._pane.gviewer.MouseClick += this.OnGViewerMouseClicked;
            this._pane.gviewer.MouseDoubleClick += this.OnGViewerMouseDoubleClick;
            this._pane.searchbtn.Click += this.OnSearchButtonPressed;
            this._pane.searchtb.KeyUp += this.OnSearchKeyUp;

            this.CollectionsType = CollectionType.Device;
            this._header = "Deployments";
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
                        else if (this.SelectedNode.SccmObject is SccmSoftwareUpdateGroup) { this._pane.modecombo.Text = "Update Group"; }
                        else if (this.SelectedNode.SccmObject is SMS_DeploymentSummary) { this._pane.modecombo.Text = "Deployment"; }
                        else if (this.SelectedNode.SccmObject is SccmSoftwareUpdate) { this._pane.modecombo.Text = "Update"; }
                        else if (this.SelectedNode.SccmObject is SccmTaskSequence) { this._pane.modecombo.Text = "Task Sequence"; }
                        else if (this.SelectedNode.SccmObject is SccmPackage) { this._pane.modecombo.Text = "Package"; }
                        else if (this.SelectedNode.SccmObject is SccmPackageProgram) { this._pane.modecombo.Text = "Package"; }
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
            this.StartProcessing();

            Task.Run(() => this.NotifyProgress("Searching"));
            if (this._pane.modecombo.Text == "Collection")
            { await Task.Run(() => this.SearchResults = this._connector.GetCollectionSccmObjectsFromSearch(this._searchtext, this.CollectionsType)); }

            else if (this._pane.modecombo.Text == "Deployment")
            { await Task.Run(() => this.SearchResults = this._connector.GetDeploymentSccmObjectsFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Application")
            { await Task.Run(() => this.SearchResults = this._connector.GetApplicationsSccmObjectsListFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Update Group")
            { await Task.Run(() => this.SearchResults = this._connector.GetSoftwareUpdateGroupSccmObjectsFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Update")
            { await Task.Run(() => this.SearchResults = this._connector.GetSoftwareUpdateSccmObjectsFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Task Sequence")
            { await Task.Run(() => this.SearchResults = this._connector.GetTaskSequenceSccmObjectsFromSearch(this._searchtext)); }

            else if (this._pane.modecombo.Text == "Package")
            { await Task.Run(() => this.SearchResults = this._connector.GetPackageSccmObjectsFromSearch(this._searchtext)); }

            this.FinishProcessing();
            this.NotifyFinishSearchWithCount(this.SearchResults.Count);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Await.Warning", "CS4014:Await.Warning")]
        protected async override void BuildGraph()
        {
            if (this._selectedresult != null)
            {
                this.StartProcessing();
                //string mode = this._pane.modecombo.Text;
                this.ClearHighlightedCollections();
                Task.Run(() => this.NotifyProgress("Querying server"));

                await Task.Run(() =>
                {
                    if (this._selectedresult != null)
                    {
                        if (this._selectedresult.Type == SccmItemType.Package) { this.ProcessBuildPackage((SccmPackage)this._selectedresult); }
                        else if (this._selectedresult.Type == SccmItemType.Collection) { this.ProcessBuildCollection((SccmCollection)this._selectedresult); }
                        else if (this._selectedresult.Type == SccmItemType.SoftwareUpdate) { this.ProcessBuildSoftwareUpdate((SccmSoftwareUpdate)this._selectedresult); }
                        else { this.ProcessDeployableItem(this._selectedresult); }
                    }
                });

                this.UpdateProgressMessage_ForAsync("Building graph");
                await Task.Run(() => this.UpdatePaneToTabControl());
                this.FinishProcessing();
            }
            else { this.NotificationText = "Nothing selected"; }
        }

        private void ProcessBuildPackage(SccmPackage package)
        {
            List<IDeployment> deployments = null;
            this._connector.PopulatePackageChildren(package);
            deployments = this._connector.GetSMS_DeploymentInfoDeployments(this._selectedresult.Name, SccmItemType.PackageProgram);

            this.UpdateProgressMessage_ForAsync("Building tree");
            this._graph = TreeBuilder.BuildPackageDeploymentsTree(this._connector, package, deployments);
        }

        private void ProcessBuildCollection(SccmCollection collection)
        {
            List<IDeployment> deployments = null;
            deployments = this._connector.GetCollectionDeployments(this._selectedresult.ID);
            this.UpdateProgressMessage_ForAsync("Building tree");
            this._graph = TreeBuilder.BuildCollectionDeploymentsTree(this._connector, collection, deployments);
        }

        private void ProcessBuildSoftwareUpdate(SccmSoftwareUpdate update)
        {
            List<IDeployment> deployments = null;
            deployments = this._connector.GetSMS_DeploymentInfoDeployments(update.Name, this._selectedresult.Type);

            this.UpdateProgressMessage_ForAsync("Building tree");
            this._graph = TreeBuilder.BuildCIDeploymentsTree(this._connector, update, deployments);
        }

        private void ProcessDeployableItem(ISccmObject sccmobject)
        {
            List<IDeployment> deployments = null;
            deployments = this._connector.GetSoftwareItemDeployments(sccmobject.Name);

            this.UpdateProgressMessage_ForAsync("Building tree");
            this._graph = TreeBuilder.BuildCIDeploymentsTree(this._connector, sccmobject, deployments);
        }
    }
}
