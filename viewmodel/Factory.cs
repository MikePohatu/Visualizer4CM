using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public static class Factory
    {
        public static ISccmObject GetSccmObject(SccmDeploymentSummary deployment)
        {
            if (deployment.FeatureType == SccmDeploymentSummary.CIType.Application)
            {
                var newitem = new SccmApplication();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.CIID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmDeploymentSummary.CIType.SoftwareUpdateGroup)
            {
                var newitem = new SccmSoftwareUpdateGroup();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.CIID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmDeploymentSummary.CIType.TaskSequence)
            {
                var newitem = new SccmTaskSequence();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.PackageID;
                return newitem;
            }
            return null;
        }
    }
}
