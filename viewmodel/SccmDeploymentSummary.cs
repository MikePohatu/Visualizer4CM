using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    /// <summary>
    /// Class represents the SMS_DeploymentSummary WMI class
    /// </summary>
    public class SccmDeploymentSummary: ViewModelBase, ISccmObject
    {
        public enum CIType { Package=2, Application=1, SoftwareUpdateGroup=5, TaskSequence=7 }

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

        protected CIType _featuretype;
        public CIType FeatureType
        {
            get { return this._featuretype; }
            set
            {
                this._featuretype = value;
                this.OnPropertyChanged(this, "FeatureType");
            }
        }

        //protected int _deploymenttypeid;
        //public int DeploymentTypeID
        //{
        //    get { return this._deploymenttypeid; }
        //    set
        //    {
        //        this._deploymenttypeid = value;
        //        this.OnPropertyChanged(this, "DeploymentTypeID");
        //    }
        //}

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

        //protected int _targetsecuritytypeid;
        //public int TargetSecurityTypeID
        //{
        //    get { return this._targetsecuritytypeid; }
        //    set
        //    {
        //        this._targetsecuritytypeid = value;
        //        this.OnPropertyChanged(this, "TargetSecurityTypeID");
        //    }
        //}

        //protected string _targetsubname;
        //public string TargetSubName
        //{
        //    get { return this._targetsubname; }
        //    set
        //    {
        //        this._targetsubname = value;
        //        this.OnPropertyChanged(this, "TargetSubName");
        //    }
        //}

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

        public SccmDeploymentSummary() { }
        public SccmDeploymentSummary(IResultObject resource)
        {
            this.CollectionID = ResultObjectHandler.GetString(resource,"CollectionID");
            this.CollectionName = ResultObjectHandler.GetString(resource, "CollectionName");
            this.DeploymentID = ResultObjectHandler.GetString(resource, "DeploymentID");
            this.DeploymentIntent = ResultObjectHandler.GetInt(resource,"DeploymentIntent");
            this.SoftwareName = ResultObjectHandler.GetString(resource, "SoftwareName");
            this.PackageID = ResultObjectHandler.GetString(resource, "PackageID");
            this.CIID = ResultObjectHandler.GetString(resource,"CI_ID");
            this.SoftwareName = ResultObjectHandler.GetString(resource, "SoftwareName");
            this.FeatureType = (CIType)ResultObjectHandler.GetInt(resource,"FeatureType");
        }
    }
}
