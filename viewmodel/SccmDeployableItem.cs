using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmDeployableItem : ViewModelBase, ISccmObject
    {
        public enum CIType { Package=2, Application=31, SoftwareUpdate=37, TaskSequence=7, SoftwareUpdateGroup = 5 }

        protected string _id;
        public string ID
        {
            get { return this._id; }
            set
            {
                this._id = value;
                this.OnPropertyChanged(this, "ID");
            }
        }

        protected string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
                this.OnPropertyChanged(this, "Name");
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

        public SccmDeployableItem() { }
        public SccmDeployableItem(IResultObject resource)
        {
            this.ID = ResultObjectHandler.GetString(resource, "CI_ID");
            this.Name = ResultObjectHandler.GetString(resource, "LocalizedDisplayName");
        }
    }
}
