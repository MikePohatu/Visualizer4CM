namespace viewmodel
{
    /// <summary>
    /// Class represents the SMS_DeploymentSummary WMI class
    /// </summary>
    public class SMS_DeploymentSummary: ViewModelBase, IDeployment
    {
        protected string _collectionid;
        public string CollectionID
        {
            get { return this._collectionid; }
            set
            {
                this._collectionid = value;
                this.OnPropertyChanged(this, "CollectionID");
            }
        }

        protected string _collectionname;
        public string CollectionName
        {
            get { return this._collectionname; }
            set
            {
                this._collectionname = value;
                this.OnPropertyChanged(this, "CollectionName");
            }
        }

        protected string _deploymentid;
        public string DeploymentID
        {
            get { return this._deploymentid; }
            set
            {
                this._deploymentid = value;
                this.OnPropertyChanged(this, "DeploymentID");
            }
        }
        public string ID { get { return this._deploymentid; } }

        protected int _deploymentintent;
        public int DeploymentIntent
        {
            get { return this._deploymentintent; }
            set
            {
                this._deploymentintent = value;
                this.OnPropertyChanged(this, "DeploymentIntent");
            }
        }

        protected string _packageid;
        public string PackageID
        {
            get { return this._packageid; }
            set
            {
                this._packageid = value;
                this.OnPropertyChanged(this, "PackageID");
            }
        }
        public string Name { get { return this._deploymentid; } }

        protected SccmItemType _featuretype;
        public SccmItemType FeatureType
        {
            get { return this._featuretype; }
            set
            {
                this._featuretype = value;
                this.OnPropertyChanged(this, "FeatureType");
            }
        }

        protected string _ciid;
        public string CIID
        {
            get { return this._ciid; }
            set
            {
                this._ciid = value;
                this.OnPropertyChanged(this, "CIID");
            }
        }

        protected string _softwarename;
        public string SoftwareName
        {
            get { return this._softwarename; }
            set
            {
                this._softwarename = value;
                this.OnPropertyChanged(this, "SoftwareName");
            }
        }

        private bool _ishighlighted;
        public bool IsHighlighted
        {
            get { return this._ishighlighted; }
            set
            {
                this._ishighlighted = value;
                this.OnPropertyChanged(this, "IsHighlighted");
            }
        }
    }
}
