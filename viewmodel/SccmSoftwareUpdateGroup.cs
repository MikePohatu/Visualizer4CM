using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmSoftwareUpdateGroup : SccmDeployableItem
    {
        public SccmSoftwareUpdateGroup(): base() { }
        public SccmSoftwareUpdateGroup(IResultObject resource) : base(resource) { }
    }
}
