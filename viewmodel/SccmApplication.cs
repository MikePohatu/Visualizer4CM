using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmApplication: SccmDeployableItem
    {
        public bool IsDeployed { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsSuperseded { get; set; }
        public bool IsSuperseding { get; set; }
        public bool IsLatest { get; set; }

        public SccmApplication() : base() { }
        public SccmApplication(IResultObject resource): base (resource)
        {
            this.IsDeployed = ResultObjectHandler.GetBool(resource, "IsDeployed");
            this.IsEnabled = ResultObjectHandler.GetBool(resource, "IsEnabled");
            this.IsSuperseded = ResultObjectHandler.GetBool(resource, "IsSuperseded");
            this.IsSuperseding = ResultObjectHandler.GetBool(resource, "IsSuperseding");
            this.IsLatest = ResultObjectHandler.GetBool(resource, "IsLatest");
        }

        public new string ToString()
        {
            return this._name;
        }
    }
}
