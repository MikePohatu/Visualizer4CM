using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public class SccmSoftwareUpdate : SccmDeployableItem
    {
        public SccmSoftwareUpdate(): base() { }
        public SccmSoftwareUpdate(IResultObject resource) : base(resource) { }
    }
}
