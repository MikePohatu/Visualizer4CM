using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmApplication: ViewModelBase
    {
        private string _ciid;
        public string CIID
        {
            get { return this._ciid; }
            set
            {
                this._ciid = value;
                this.OnPropertyChanged(this, "CIID");
            }
        }
        private string _name;
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

        public bool IsDeployed { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSuperseded { get; set; }
        public bool IsSuperseding { get; set; }
        public bool IsLatest { get; set; }

        public SccmApplication() { }
        public SccmApplication(IResultObject resource)
        {
            this.CIID = resource["CI_ID"].IntegerValue.ToString();
            this.Name = resource["LocalizedDisplayName"].StringValue;
            this.IsDeployed = resource["IsDeployed"].BooleanValue;
            this.IsEnabled = resource["IsEnabled"].BooleanValue;
            this.IsSuperseded = resource["IsSuperseded"].BooleanValue;
            this.IsSuperseding = resource["IsSuperseding"].BooleanValue;
            this.IsLatest = resource["IsLatest"].BooleanValue;
        }

        public new string ToString()
        {
            return this._name;
        }
    }
}
