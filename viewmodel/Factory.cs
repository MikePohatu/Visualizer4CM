using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace viewmodel
{
    public static class Factory
    {
        public static ISccmObject GetSccmObject(SccmDeployment deployment)
        {
            if (deployment.DeploymentType == SccmDeployment.CIType.Application)
            {
                var newitem = new SccmApplication();
                newitem.Name = deployment.TargetName;
                newitem.ID = deployment.TargetID;
                return newitem;
            }

            else if (deployment.DeploymentType == SccmDeployment.CIType.SoftwareUpdate)
            {
                var newitem = new SccmSoftwareUpdate();
                newitem.Name = deployment.TargetName;
                newitem.ID = deployment.TargetID;
                return newitem;
            }

            else if (deployment.DeploymentType == SccmDeployment.CIType.TaskSequence)
            {
                var newitem = new SccmTaskSequence();
                newitem.Name = deployment.TargetName;
                newitem.ID = deployment.TargetID;
                return newitem;
            }
            return null;
        }
    }
}
