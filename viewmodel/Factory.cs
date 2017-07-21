using Microsoft.ConfigurationManagement.ManagementProvider;

namespace viewmodel
{
    public static class Factory
    {
        public static SccmDeployableItem GetSccmDeployableItemFromDeploymentSummary(SMS_DeploymentSummary deployment)
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

        public static SccmDeployableItem GetSccmDeployableItemFromSMS_DeploymentSummaryResults(IResultObject resource)
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

        public static SccmApplication GetApplicationFromSMS_ApplicationResults(IResultObject resource)
        {
            SccmApplication item = new SccmApplication();

            item.IsDeployed = ResultObjectHandler.GetBool(resource, "IsDeployed");
            item.IsEnabled = ResultObjectHandler.GetBool(resource, "IsEnabled");
            item.IsSuperseded = ResultObjectHandler.GetBool(resource, "IsSuperseded");
            item.IsSuperseding = ResultObjectHandler.GetBool(resource, "IsSuperseding");
            item.IsLatest = ResultObjectHandler.GetBool(resource, "IsLatest");
            item.ID = ResultObjectHandler.GetString(resource, "CI_ID");
            item.Name = ResultObjectHandler.GetString(resource, "LocalizedDisplayName");

            return item;
        }

        public static SccmCollection GetCollectionFromSMS_CollectionResults(IResultObject resource)
        {
            SccmCollection item = new SccmCollection();

            item.ID = ResultObjectHandler.GetString(resource, "CollectionID");
            item.Name = ResultObjectHandler.GetString(resource, "Name");
            item.LimitingCollectionID = ResultObjectHandler.GetString(resource, "LimitToCollectionID");
            item.Comment = ResultObjectHandler.GetString(resource, "Comment");
            item.IncludeExcludeCollectionCount = ResultObjectHandler.GetInt(resource, "IncludeExcludeCollectionsCount");
            int typeint = ResultObjectHandler.GetInt(resource, "CollectionType");
            item.Type = (CollectionType)typeint;

            return item;
        }

        public static SccmApplicationRelationship GetAppRelationshipFromSMS_AppDependenceRelationResults(IResultObject resource)
        {
            SccmApplicationRelationship item = new SccmApplicationRelationship();
            item.FromApplicationCIID = ResultObjectHandler.GetString(resource, "FromApplicationCIID");
            item.ToApplicationCIID = ResultObjectHandler.GetString(resource, "ToApplicationCIID");
            item.ToDeploymentTypeCIID = ResultObjectHandler.GetString(resource, "ToDeploymentTypeCIID");
            item.FromDeploymentTypeCIID = ResultObjectHandler.GetString(resource, "FromDeploymentTypeCIID");
            item.SetType(ResultObjectHandler.GetInt(resource, "TypeFlag"));

            return item;
        }

        public static SMS_DeploymentSummary GetDeploymentSummaryFromSMS_DeploymentSummaryResults(IResultObject resource)
        {
            SMS_DeploymentSummary item = new SMS_DeploymentSummary();

            item.CollectionID = ResultObjectHandler.GetString(resource, "CollectionID");
            item.CollectionName = ResultObjectHandler.GetString(resource, "CollectionName");
            item.DeploymentID = ResultObjectHandler.GetString(resource, "DeploymentID");
            item.DeploymentIntent = ResultObjectHandler.GetInt(resource, "DeploymentIntent");
            item.SoftwareName = ResultObjectHandler.GetString(resource, "SoftwareName");
            item.PackageID = ResultObjectHandler.GetString(resource, "PackageID");
            item.CIID = ResultObjectHandler.GetString(resource, "CI_ID");
            item.SoftwareName = ResultObjectHandler.GetString(resource, "SoftwareName");
            item.FeatureType = (SccmItemType)ResultObjectHandler.GetInt(resource, "FeatureType");
            return item;
        }

        public static SMS_DeploymentInfo GetDeploymentInfoFromSMS_DeploymentInfoResults(IResultObject resource)
        {
            SMS_DeploymentInfo item = new SMS_DeploymentInfo();

            item.CollectionID = ResultObjectHandler.GetString(resource, "CollectionID");
            item.CollectionName = ResultObjectHandler.GetString(resource, "CollectionName");
            item.DeploymentID = ResultObjectHandler.GetString(resource, "DeploymentID");
            item.DeploymentIntent = ResultObjectHandler.GetInt(resource, "DeploymentIntent");
            item.DeploymentName = ResultObjectHandler.GetString(resource, "DeploymentName");
            item.DeploymentType = ResultObjectHandler.GetInt(resource, "DeploymentType");
            item.DeploymentTypeID = ResultObjectHandler.GetInt(resource, "DeploymentTypeID");
            item.FeatureType = (SccmItemType)ResultObjectHandler.GetInt(resource, "FeatureType");
            item.TargetID = ResultObjectHandler.GetString(resource, "TargetID");
            item.TargetSecurityTypeID = ResultObjectHandler.GetInt(resource, "TargetSecurityTypeID");
            item.TargetSubName = ResultObjectHandler.GetString(resource, "TargetSubName");

            return item;
        }
    }
}
