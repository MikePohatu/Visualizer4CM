using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmConfigurationBaseline : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.ConfigurationBaseline; } }
        public SccmConfigurationBaseline(): base() { }
        public SccmConfigurationBaseline(IResultObject resource) : base(resource) { }
    }
}
