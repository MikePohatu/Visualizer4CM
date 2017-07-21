using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public abstract class SccmDeployableItem : ViewModelBase, ISccmObject
    {
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

        protected bool _ishighlighted;
        public bool IsHighlighted
        {
            get { return this._ishighlighted; }
            set
            {
                this._ishighlighted = value;
                this.OnPropertyChanged(this, "IsHighlighted");
            }
        }

        public abstract SccmItemType Type { get; }

        public SccmDeployableItem() { }
        public SccmDeployableItem(IResultObject resource)
        {
            this.ID = ResultObjectHandler.GetString(resource, "CI_ID");
            this.Name = ResultObjectHandler.GetString(resource, "LocalizedDisplayName");
        }
    }
}
