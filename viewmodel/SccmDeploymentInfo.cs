﻿using System;
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
    public class SccmDeploymentInfo: ViewModelBase, ISccmObject
    {
        public enum FeatureTypeValue { Package=2, Application=1, SoftwareUpdate=37, TaskSequence=7 }

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

        
        public string Name { get { return this._deploymentname; } }

        protected int _deploymenttype;
        public int DeploymentType
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

        protected FeatureTypeValue _featuretype;
        public FeatureTypeValue FeatureType
        {
            get { return this._featuretype; }
            set
            {
                this._featuretype = value;
                this.OnPropertyChanged(this, "FeatureType");
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

        public SccmDeploymentInfo() { }
        public SccmDeploymentInfo(IResultObject resource)
        {
            this.CollectionID = ResultObjectHandler.GetString(resource,"CollectionID");
            this.CollectionName = ResultObjectHandler.GetString(resource, "CollectionName");
            this.DeploymentID = ResultObjectHandler.GetString(resource, "DeploymentID");
            this.DeploymentIntent = ResultObjectHandler.GetInt(resource,"DeploymentIntent");
            this.DeploymentName = ResultObjectHandler.GetString(resource, "DeploymentName");
            this.DeploymentType = ResultObjectHandler.GetInt(resource, "DeploymentType");
            this.DeploymentTypeID = ResultObjectHandler.GetInt(resource, "DeploymentTypeID");
            this.FeatureType = (FeatureTypeValue)ResultObjectHandler.GetInt(resource, "FeatureType");
            this.TargetID = ResultObjectHandler.GetString(resource, "TargetID");
            this.TargetSecurityTypeID = ResultObjectHandler.GetInt(resource, "TargetSecurityTypeID");
            this.TargetSubName = ResultObjectHandler.GetString(resource, "TargetSubName");
        }
    }
}
