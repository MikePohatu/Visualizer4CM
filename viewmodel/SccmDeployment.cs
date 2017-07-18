using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmDeployment: ViewModelBase
    {
        public enum CIType { Package=2, Application=31, SoftwareUpdate=37, TaskSequence=7 }

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

        protected string _deploymentname;
        public string DeploymentName
        {
            get { return this._deploymentname; }
            set
            {
                this._deploymentname = value;
                this.OnPropertyChanged(this, "DeploymentName");
            }
        }

        protected CIType _deploymenttype;
        public CIType DeploymentType 
        {
            get { return this._deploymenttype; }
            set
            {
                this._deploymenttype = value;
                this.OnPropertyChanged(this, "DeploymentType");
            }
        }

        protected int _deploymenttypeid;
        public int DeploymentTypeID
        {
            get { return this._deploymenttypeid; }
            set
            {
                this._deploymenttypeid = value;
                this.OnPropertyChanged(this, "DeploymentTypeID");
            }
        }

        protected string _targetid;
        public string TargetID
        {
            get { return this._targetid; }
            set
            {
                this._targetid = value;
                this.OnPropertyChanged(this, "TargetID");
            }
        }

        protected string _targetname;
        public string TargetName
        {
            get { return this._targetname; }
            set
            {
                this._targetname = value;
                this.OnPropertyChanged(this, "TargetName");
            }
        }

        protected int _targetsecuritytypeid;
        public int TargetSecurityTypeID
        {
            get { return this._targetsecuritytypeid; }
            set
            {
                this._targetsecuritytypeid = value;
                this.OnPropertyChanged(this, "TargetSecurityTypeID");
            }
        }

        protected string _targetsubname;
        public string TargetSubName
        {
            get { return this._targetsubname; }
            set
            {
                this._targetsubname = value;
                this.OnPropertyChanged(this, "TargetSubName");
            }
        }

        public SccmDeployment() { }
        public SccmDeployment(IResultObject resource)
        {
            this.CollectionID = resource["CollectionID"].StringValue;
            this.CollectionName = resource["CollectionName"].StringValue;
            this.DeploymentID = resource["DeploymentID"].StringValue;
            this.DeploymentIntent = resource["DeploymentIntent"].IntegerValue;
            this.DeploymentName = resource["DeploymentName"].StringValue;
            this.DeploymentType = (CIType)resource["DeploymentType"].IntegerValue;
            this.DeploymentTypeID = resource["DeploymentTypeID"].IntegerValue;
            this.TargetID = resource["TargetID"].StringValue;
            this.TargetName = resource["TargetName"].StringValue;
            this.TargetSubName = resource["TargetSubName"].StringValue;
            this.TargetSecurityTypeID = resource["TargetSecurityTypeID"].IntegerValue;
        }
    }
}
