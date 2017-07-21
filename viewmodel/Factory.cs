using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public static class Factory
    {
        public static SccmDeployableItem GetSccmDeployableItem(SMS_DeploymentSummary deployment)
        {
            if (deployment.FeatureType == SccmItemType.Application)
            {
                var newitem = new SccmApplication();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.CIID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmItemType.SoftwareUpdateGroup)
            {
                var newitem = new SccmSoftwareUpdateGroup();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.CIID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmItemType.SoftwareUpdate)
            {
                var newitem = new SccmSoftwareUpdate();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.CIID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmItemType.TaskSequence)
            {
                var newitem = new SccmTaskSequence();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.PackageID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmItemType.Package)
            {
                var newitem = new SccmPackage();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.PackageID;
                return newitem;
            }

            else if (deployment.FeatureType == SccmItemType.ConfigurationBaseline)
            {
                var newitem = new SccmConfigurationBaseline();
                newitem.Name = deployment.SoftwareName;
                newitem.ID = deployment.CIID;
                return newitem;
            }
            return null;
        }

        public static SccmDeployableItem GetSccmDeployableItemFromSMS_DeploymentSummary(IResultObject resource)
        {
            SccmItemType featuretype = (SccmItemType)ResultObjectHandler.GetInt(resource, "FeatureType");

            if (featuretype == SccmItemType.Application)
            {
                var newitem = new SccmApplication();
                newitem.Name = ResultObjectHandler.GetString(resource,"SoftwareName");
                newitem.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                return newitem;
            }

            else if (featuretype == SccmItemType.SoftwareUpdateGroup)
            {
                var newitem = new SccmSoftwareUpdateGroup();
                newitem.Name = ResultObjectHandler.GetString(resource, "SoftwareName");
                newitem.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                return newitem;
            }

            else if (featuretype == SccmItemType.SoftwareUpdate)
            {
                var newitem = new SccmSoftwareUpdate();
                newitem.Name = ResultObjectHandler.GetString(resource, "SoftwareName");
                newitem.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                return newitem;
            }

            else if (featuretype == SccmItemType.TaskSequence)
            {
                var newitem = new SccmTaskSequence();
                newitem.Name = ResultObjectHandler.GetString(resource, "SoftwareName");
                newitem.ID = ResultObjectHandler.GetString(resource, "PackageID");
                return newitem;
            }

            else if (featuretype == SccmItemType.Package)
            {
                var newitem = new SccmPackage();
                newitem.Name = ResultObjectHandler.GetString(resource, "SoftwareName");
                newitem.ID = ResultObjectHandler.GetString(resource, "PackageID");
                return newitem;
            }

            else if (featuretype == SccmItemType.ConfigurationBaseline)
            {
                var newitem = new SccmConfigurationBaseline();
                newitem.Name = ResultObjectHandler.GetString(resource, "SoftwareName");
                newitem.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                return newitem;
            }
            return null;
        }

    }
}
