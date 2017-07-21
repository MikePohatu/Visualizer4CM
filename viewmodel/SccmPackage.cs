using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmPackage : SccmDeployableItem
    {
        public override SccmItemType Type { get { return SccmItemType.Package; } }
        public SccmPackage(): base() { }
        public SccmPackage(IResultObject resource) : base(resource) { }
    }
}
