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
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='" + (int)type + "' ORDER BY Name";
                CollectionLibrary library = new CollectionLibrary();

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
                return library;
            }
            catch
            {
                return null;
            }
        }

        public CollectionLibrary GetAllCollectionsLibrary()
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection ORDER BY Name";
                CollectionLibrary library = new CollectionLibrary();

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
                return library;
            }
            catch
            {
                return null;
            }
        }

        public List<SccmCollection> GetCollectionsFromSearch(string search, CollectionType type)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='" + (int)type + "' AND Name LIKE '%" + search + "%' ORDER BY Name";
                List<SccmCollection> newlist = new List<SccmCollection>();

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection col = Factory.GetCollectionFromSMS_CollectionResults(resource);
                        newlist.Add(col);
                    }
                }
                return newlist;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Does a collection search, but returns a list of ISccmObjects
        /// </summary>
        /// <param name="search"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<ISccmObject> GetCollectionSccmObjectsFromSearch(string search, CollectionType type)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='" + (int)type + "' AND Name LIKE '%" + search + "%' ORDER BY Name";
                List<ISccmObject> newlist = new List<ISccmObject>();

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection col = Factory.GetCollectionFromSMS_CollectionResults(resource);
                        newlist.Add(col);
                    }
                }
                return newlist;
            }
            catch
            {
                return null;
            }
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
                string query = "select * from SMS_Application WHERE IsLatest='TRUE' AND (" + queryapplist + ") ORDER BY LocalizedDisplayName";

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
            List<SccmApplication> applications = new List<SccmApplication>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_Application WHERE IsLatest='TRUE' ORDER BY LocalizedDisplayName"; }
                else { query = "select * from SMS_Application WHERE LocalizedDisplayName LIKE '%" + search + "%' AND IsLatest='TRUE' ORDER BY LocalizedDisplayName"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        applications.Add(app);
                    }
                }
            }
            catch { }
            return applications;
        }

        public List<ISccmObject> GetApplicationsSccmObjectsListFromSearch(string search)
        {
            List<ISccmObject> applications = new List<ISccmObject>();
            try
            {
                // This query selects all collections

                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_Application WHERE IsLatest='TRUE' ORDER BY LocalizedDisplayName"; }
                else { query = "select * from SMS_Application WHERE LocalizedDisplayName LIKE '%" + search + "%' AND IsLatest='TRUE' ORDER BY LocalizedDisplayName"; }

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = Factory.GetApplicationFromSMS_ApplicationResults(resource);
                        applications.Add(app);
                    }
                }
            }
            catch { }
            return applications;
        }

        public List<SccmApplicationRelationship> GetApplicationRelationships(string applicationciid)
        {
            List<SccmApplicationRelationship> relationships = new List<SccmApplicationRelationship>();

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
                        relationships.Add(apprel);
                    }
                }              
            }
            catch { }
            return relationships;
        }

        public List<IDeployment> GetSoftwareItemDeployments(string ciname)
        {
            List<IDeployment> deployments = new List<IDeployment>();
            
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
                        deployments.Add(dep);
                    }
                }
            }
            catch { }
            return deployments;
        }

        public List<IDeployment> GetSoftwareUpdateDeployments(string updatename)
        {
            List<IDeployment> deployments = new List<IDeployment>();

            try
            {
                // This query selects all relationships of the specified app ID
                int type = (int)SccmItemType.SoftwareUpdate;
                string query = "select * from SMS_DeploymentInfo WHERE TargetName='" + updatename + "' AND DeploymentType='" + type.ToString() + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
                        deployments.Add(dep);
                    }
                }
            }
            catch { }
            return deployments;

        }


        public List<IDeployment> GetSoftwareUpdateGroupDeployments(string updategrouname)
        {
            List<IDeployment> deployments = new List<IDeployment>();

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
                        deployments.Add(dep);
                    }
                }
            }
            catch { }
            return deployments;

        }

        public List<IDeployment> GetCollectionDeployments(string collectionid)
        {
            List<IDeployment> deployments = new List<IDeployment>();

            //try
            //{
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentSummary WHERE CollectionID='" + collectionid + "'";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                    SMS_DeploymentSummary dep = Factory.GetDeploymentSummaryFromSMS_DeploymentSummaryResults(resource);                       
                        deployments.Add(dep);
                    }
                }
            //}
            //catch { }
            return deployments;
        }

        public List<IDeployment> GetDeploymentsFromSearch(string deploymentname)
        {
            List<IDeployment> deployments = new List<IDeployment>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentInfo WHERE DeploymentName LIKE '%" + deploymentname + "%' ORDER BY DeploymentName";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
                        deployments.Add(dep);
                    }
                }
            }
            catch { }
            return deployments;
        }

        public List<ISccmObject> GetDeploymentSccmObjectsFromSearch(string deploymentname)
        {
            List<ISccmObject> deployments = new List<ISccmObject>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select * from SMS_DeploymentInfo WHERE DeploymentName LIKE '%" + deploymentname + "%' ORDER BY DeploymentName";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SMS_DeploymentInfo dep = Factory.GetDeploymentInfoFromSMS_DeploymentInfoResults(resource);
                        deployments.Add(dep);
                    }
                }
            }
            catch { }
            return deployments;
        }

        public List<ISccmObject> GetSoftwareUpdateSccmObjectsFromSearch(string ciname)
        {
            List<ISccmObject> CIs = new List<ISccmObject>();

            try
            {
                // This query selects all relationships of the specified app ID
                string query = "select CI_ID,LocalizedDisplayName from SMS_SoftwareUpdate WHERE LocalizedDisplayName LIKE '%" + ciname + "%' ORDER BY LocalizedDisplayName";

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmDeployableItem dep = new SccmSoftwareUpdate();
                        dep.Name = ResultObjectHandler.GetString(resource, "LocalizedDisplayName");
                        dep.ID = ResultObjectHandler.GetString(resource, "CI_ID");
                        CIs.Add(dep);
                    }
                }
            }
            catch { }
            return CIs;
        }

        public List<ISccmObject> GetSoftwareUpdateGroupSccmObjectsFromSearch(string ciname)
        {
            List<ISccmObject> CIs = new List<ISccmObject>();

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
                        CIs.Add(dep);
                    }
                }
            }
            catch { }
            return CIs;
        }

        public List<ISccmObject> GetTaskSequenceSccmObjectsFromSearch(string search)
        {
            List<ISccmObject> items = new List<ISccmObject>();
            try
            {
                string query;
                if (string.IsNullOrWhiteSpace(search)) { query = "select * from SMS_TaskSequencePackage ORDER BY Name"; }
                else { query = "select * from SMS_TaskSequencePackage WHERE Name LIKE '%" + search + "%' ORDER BY Name"; }

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
            try
            {
                int type = (int)PackageType.RegularSoftwareDistribution;
                string query = "select * from SMS_Program WHERE PackageType='" + type + "' ORDER BY PackageName";
                PackageLibrary library = new PackageLibrary();

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
                return library;
            }
            catch
            {
                return null;
            }
        }

        public List<SccmPackage> GetPackagesFromSearch(string search)
        {
            List<SccmPackage> newlist = new List<SccmPackage>();
            try
            {
                // This query selects all collections
                int type = (int)PackageType.RegularSoftwareDistribution;
                //string query = "select * from SMS_Program WHERE Name LIKE '%" + search + "%' AND PackageType='" + type + "' ORDER BY PackageName";
                string query = "select * from SMS_PackageBaseclass WHERE Name LIKE '%" + search + "%' AND PackageType='" + type + "'";               

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmPackage item = Factory.GetPackageFromSMS_PackageBaseclassResults(resource);
                        newlist.Add(item);
                    }
                }
            }
            catch
            { }

            return newlist;
        }
    }
}
