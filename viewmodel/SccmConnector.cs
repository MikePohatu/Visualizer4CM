using System.Linq;
using System.Collections.Generic;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace viewmodel
{
    public class SccmConnector
    {
        private WqlConnectionManager _connection = new WqlConnectionManager();

        public bool Connect(string server)
        {
            try { this._connection.Connect(server); }
            catch { return false; }
            return true;
        }

        public bool Connect(string authuser, string authpw, string authdomain, string server)
        {
            try { this._connection?.Connect(server, authdomain + "\\" + authuser, authpw); }
            catch { return false; }
            return true;
        }

        public CollectionLibrary GetCollectionLibrary(CollectionType type)
        {
            CollectionLibrary library = new CollectionLibrary();

            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='" + (int)type + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection col = Factory.GetCollectionFromSMS_CollectionResults(resource);
                        library.AddCollection(col);
                    }
                }
            }
            catch { }
            return library;
        }

        public CollectionLibrary GetAllCollectionsLibrary()
        {
            CollectionLibrary library = new CollectionLibrary();

            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection col = Factory.GetCollectionFromSMS_CollectionResults(resource);
                        library.AddCollection(col);
                    }
                }
            }
            catch { }
            return library;
        }

        public List<SccmCollection> GetCollectionsFromSearch(string search, CollectionType type)
        {
            List<SccmCollection> items = new List<SccmCollection>();

            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='" + (int)type + "' AND Name LIKE '%" + search + "%'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection col = Factory.GetCollectionFromSMS_CollectionResults(resource);
                        items.Add(col);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();

            }
            catch { }
            return items;
        }

        /// <summary>
        /// Does a collection search, but returns a list of ISccmObjects
        /// </summary>
        /// <param name="search"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ISccmObject> GetCollectionSccmObjectsFromSearch(string search, CollectionType type)
        {
            List<ISccmObject> items = new List<ISccmObject>();
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='" + (int)type + "' AND Name LIKE '%" + search + "%'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection col = Factory.GetCollectionFromSMS_CollectionResults(resource);
                        items.Add(col);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<SccmCollectionRelationship> GetCollectionDependencies(string collectionid)
        {
            List<SccmCollectionRelationship> relationships = new List<SccmCollectionRelationship>();
            try
            {
                // This query selects all collections
                string query = "select * from SMS_CollectionDependencies WHERE DependentCollectionID='" + collectionid + "'";
                
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollectionRelationship colrel = new SccmCollectionRelationship();
                        colrel.DependentCollectionID = resource["DependentCollectionID"].StringValue;
                        colrel.SetType(resource["RelationshipType"].IntegerValue);
                        colrel.SourceCollectionID = resource["SourceCollectionID"].StringValue;
                        relationships.Add(colrel);
                    }
                }    
            }
            catch { }
            return relationships;
        }

        public Dictionary<string,SccmApplication> GetAllApplications()
        {
            Dictionary<string,SccmApplication> applications = new Dictionary<string, SccmApplication>();
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Application WHERE IsLatest='TRUE'";
                
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        applications.Add(app.ID,app);
                    }
                }
                
            }
            catch { }
            return applications;
        }

        /// <summary>
        /// Get a dictionary of SccmApplication objects based on a list of CI_IDs
        /// </summary>
        /// <param name="applicationids"></param>
        /// <returns></returns>
        public Dictionary<string, SccmApplication> GetApplicationDictionaryFromIDs(List<string> applicationids)
        {
            Dictionary<string, SccmApplication> applications = new Dictionary<string, SccmApplication>();
            string queryapplist = "";
            int count = 0;

            //build up the query
            foreach (string appid in applicationids)
            {
                if (count == 0)
                {
                    queryapplist = "CI_ID='" + appid + "'";
                    count++;
                }
                else { queryapplist = queryapplist + " OR CI_ID='" + appid + "'"; }
            }

            try
            {
                string query = "select * from SMS_Application WHERE IsLatest='TRUE' AND (" + queryapplist + ")";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        applications.Add(app.ID, app);
                    }
                }

            }
            catch { }
            return applications;
        }

        public List<SccmApplication> GetApplicationsListFromSearch(string search)
        {
            List<SccmApplication> items = new List<SccmApplication>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_Application WHERE IsLatest='TRUE'"; }
                else { query = "select * from SMS_Application WHERE LocalizedDisplayName LIKE '%" + search + "%' AND IsLatest='TRUE'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        items.Add(app);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetApplicationsSccmObjectsListFromSearch(string search)
        {
            List<ISccmObject> items = new List<ISccmObject>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_Application WHERE IsLatest='TRUE'"; }
                else { query = "select * from SMS_Application WHERE LocalizedDisplayName LIKE '%" + search + "%' AND IsLatest='TRUE'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        items.Add(app);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<SccmApplicationRelationship> GetApplicationRelationships(string applicationciid)
        {
            List<SccmApplicationRelationship> items = new List<SccmApplicationRelationship>();
            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_AppDependenceRelation WHERE FromApplicationCIID='" + applicationciid + "'"
                    + " OR ToApplicationCIID='" + applicationciid + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplicationRelationship apprel = Factory.GetAppRelationshipFromSMS_AppDependenceRelationResults(resource);
                        items.Add(apprel);
                    }
                }
            }
            catch { }
            return items;
        }

        public List<IDeployment> GetSoftwareItemDeployments(string ciname)
        {
            List<IDeployment> items = new List<IDeployment>();
            
            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentSummary WHERE SoftwareName='" + ciname + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentSummary dep = Factory.GetDeploymentSummaryFromSMS_DeploymentSummaryResults(resource);
                        items.Add(dep);
                    }
                }
                //items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        //public List<IDeployment> GetSoftwareUpdateDeployments(string updatename)
        //{
        //    List<IDeployment> items = new List<IDeployment>();

        //    try
        //    {
        //        // This query selects all relationships of the specified app ID
        //        int type = (int)SccmItemType.SoftwareUpdate;
        //        string query = "select * from SMS_DeploymentInfo WHERE TargetName='" + updatename + "' AND DeploymentType='" + type.ToString() + "'";

        //        // Run query
        //        using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
        //        {
        //            // Enumerate through the collection of objects returned by the query.
        //            foreach (IResultObject resource in results)
        //            {
        //                SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
        //                items.Add(dep);
        //            }
        //        }
        //        //items = items.OrderBy(o => o.Name).ToList();
        //    }
        //    catch { }
        //    return items;
        //}


        public List<IDeployment> GetSoftwareUpdateGroupDeployments(string updategrouname)
        {
            List<IDeployment> items = new List<IDeployment>();

            try
            {
                // This query selects all relationships of the specified app ID
                int type = (int)SccmItemType.SoftwareUpdateGroup;
                string query = "select * from SMS_DeploymentSummary WHERE SoftwareName='" + updategrouname + "' AND FeatureType='" + type.ToString() + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentSummary dep = Factory.GetDeploymentSummaryFromSMS_DeploymentSummaryResults(resource);
                        items.Add(dep);
                    }
                }
                //items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;

        }

        public List<IDeployment> GetCollectionDeployments(string collectionid)
        {
            List<IDeployment> items = new List<IDeployment>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentSummary WHERE CollectionID='" + collectionid + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                    SMS_DeploymentSummary dep = Factory.GetDeploymentSummaryFromSMS_DeploymentSummaryResults(resource);                       
                        items.Add(dep);
                    }
                }
                //items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<IDeployment> GetDeploymentsFromSearch(string deploymentname)
        {
            List<IDeployment> items = new List<IDeployment>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentInfo WHERE DeploymentName LIKE '%" + deploymentname + "%'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
                        items.Add(dep);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetDeploymentSccmObjectsFromSearch(string deploymentname)
        {
            List<ISccmObject> items = new List<ISccmObject>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentInfo WHERE DeploymentName LIKE '%" + deploymentname + "%'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
                        items.Add(dep);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetSoftwareUpdateSccmObjectsFromSearch(string ciname)
        {
            List<ISccmObject> items = new List<ISccmObject>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select CI_ID,LocalizedDisplayName from SMS_SoftwareUpdate WHERE LocalizedDisplayName LIKE '%" + ciname + "%'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmDeployableItem dep = new SccmSoftwareUpdate();
                        dep.Name = ResultObjectHandler.GetString(resource, "LocalizedDisplayName");
                        dep.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                        items.Add(dep);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetSoftwareUpdateGroupSccmObjectsFromSearch(string ciname)
        {
            List<ISccmObject> items = new List<ISccmObject>();

            try
            {
                int type = (int)SccmItemType.SoftwareUpdateGroup;
                // This query selects all relationships of the specified app ID
                string query = "select DISTINCT CI_ID,SoftwareName from SMS_DeploymentSummary WHERE SoftwareName LIKE '%" + ciname + "%' AND FeatureType='" + type.ToString() + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmDeployableItem dep = new SccmSoftwareUpdateGroup();
                        dep.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                        dep.Name = ResultObjectHandler.GetString(resource, "SoftwareName");
                        items.Add(dep);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetTaskSequenceSccmObjectsFromSearch(string search)
        {
            List<ISccmObject> items = new List<ISccmObject>();
            try
            {
                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_TaskSequencePackage"; }
                else { query = "select * from SMS_TaskSequencePackage WHERE Name LIKE '%" + search + "%'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmTaskSequence app = Factory.GetTaskSequenceFromSMS_TaskSequenceResults(resource);
                        items.Add(app);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public SccmDevice GetDevice(string name)
        {
            try
            {
                string query = "select * from SMS_FullCollectionMembership WHERE Name LIKE '" +  name + "'";
                SccmDevice cmresource = new SccmDevice();
                int count = 0;
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        if (count == 0)
                        {
                            cmresource.ID = ResultObjectHandler.GetString(resource,"ResourceID");
                            cmresource.Name = ResultObjectHandler.GetString(resource, "Name");
                            count++;
                        }
                        cmresource.CollectionIDs.Add(resource["CollectionID"].StringValue);
                    }
                }
                if (count != 0) { return cmresource; }
            }
            catch
            { }
            return null;
        }

        public SccmUser GetUser(string name)
        {
            try
            {
                string query = "select * from SMS_FullCollectionMembership WHERE SMSID LIKE '" + name.Replace(@"\",@"\\") + "'";
                SccmUser cmresource = new SccmUser();
                int count = 0;
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        if (count == 0)
                        {
                            cmresource.ID = resource["ResourceID"].StringValue;
                            cmresource.Name = resource["Name"].StringValue;
                            count++;
                        }
                        cmresource.CollectionIDs.Add(resource["CollectionID"].StringValue);
                    }
                }
                if (count != 0) { return cmresource; }
            }
            catch
            { }
            return null;
        }

        public PackageLibrary GetPackageLibrary()
        {
            PackageLibrary library = new PackageLibrary();
            try
            {
                foreach (SccmPackage package in this.GetPackagesFromSearch(null))
                { library.AddPackage(package); }

                //get programs
                int type = (int)PackageType.RegularSoftwareDistribution;
                string query = "select * from SMS_Program WHERE PackageType='" + type + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmPackageProgram item = Factory.GetPackageProgramFromSMS_ProgramResults(resource);
                        library.AddPackageProgram(item);
                    }
                }
            }
            catch { }
            return library;
        }

        public void PopulatePackageChildren(SccmPackage package)
        {
            package.Programs.Clear();
            try
            {
                //get programs
                int type = (int)PackageType.RegularSoftwareDistribution;
                string query = "select * from SMS_Program WHERE PackageType='" + type + "' AND PackageID='" + package.ID + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmPackageProgram item = Factory.GetPackageProgramFromSMS_ProgramResults(resource);
                        if (item != null) { package.Programs.Add(item); }
                    }
                }
            }
            catch { }
        }

        public List<SccmPackage> GetPackagesFromSearch(string search)
        {
            List<SccmPackage> items = new List<SccmPackage>();
            try
            {
                // This query selects all collections
                int type = (int)PackageType.RegularSoftwareDistribution;

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_PackageBaseclass WHERE PackageType='" + type + "'"; }
                else { query = "select * from SMS_PackageBaseclass WHERE Name LIKE '%" + search + "%' AND PackageType='" + type + "'"; }
                             

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmPackage item = Factory.GetPackageFromSMS_PackageBaseclassResults(resource);
                        if (item != null) { items.Add(item); }
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetPackageSccmObjectsFromSearch(string search)
        {
            List<ISccmObject> items = new List<ISccmObject>();

            try
            {
                int type = (int)PackageType.RegularSoftwareDistribution;

                // This query selects all relationships of the specified app ID
                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_PackageBaseclass WHERE PackageType='" + type + "'"; }
                else { query = "select * from SMS_PackageBaseclass WHERE Name LIKE '%" + search + "%' AND PackageType='" + type + "'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmPackage dep = Factory.GetPackageFromSMS_PackageBaseclassResults(resource);
                        if (dep != null) { items.Add(dep); }
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<IDeployment> GetSMS_DeploymentInfoDeployments(string targettame, SccmItemType itemtype)
        {
            List<IDeployment> items = new List<IDeployment>();

            try
            {
                // This query selects all relationships of the specified app ID
                int type = (int)itemtype;
                string query = "select * from SMS_DeploymentInfo WHERE TargetName='" + targettame + "' AND DeploymentType='" + type.ToString() + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
                        items.Add(dep);
                    }
                }
                //items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<SccmConfigurationBaseline> GetConfigurationBaselinesListFromSearch(string search)
        {
            List<SccmConfigurationBaseline> items = new List<SccmConfigurationBaseline>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_ConfigurationBaselineInfo WHERE IsLatest='TRUE'"; }
                else { query = "select * from SMS_ConfigurationBaselineInfo WHERE LocalizedDisplayName LIKE '%" + search + "%'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        //SccmConfigurationBaseline item = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        //items.Add(item);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<ISccmObject> GetConfigurationBaselineSccmObjectsListFromSearch(string search)
        {
            List<ISccmObject> items = new List<ISccmObject>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_ConfigurationBaselineInfo"; }
                else { query = "select * from SMS_ConfigurationBaselineInfo WHERE LocalizedDisplayName LIKE '%" + search + "%'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmConfigurationBaseline item = Factory.GetConfigurationBaselineFromSMS_ConfigurationBaselineInfo(resource);
                        items.Add(item);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }

        public List<SccmConfigurationItem> GetConfigurationItemsListFromSearch(string search)
        {
            List<SccmConfigurationItem> items = new List<SccmConfigurationItem>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_ConfigurationItemSettingReference WHERE IsLatest='TRUE'"; }
                else { query = "select * from SMS_ConfigurationItemSettingReference WHERE SettingName LIKE '%" + search + "%'"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        //SccmConfigurationItem item = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        //items.Add(item);
                    }
                }
                items = items.OrderBy(o => o.Name).ToList();
            }
            catch { }
            return items;
        }
    }
}
