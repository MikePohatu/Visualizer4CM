using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace viewmodel
{
    public class SccmConnector
    {
        private WqlConnectionManager _connection = new WqlConnectionManager();
        private CollectionLibrary _devlibrary;
        private CollectionLibrary _userlibrary;

        public CollectionLibrary DeviceCollectionLibrary { get { return this._devlibrary; } }
        public CollectionLibrary UserCollectionLibrary { get { return this._userlibrary; } }

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

        public void QueryAll(string site)
        {
            try
            {
                this._devlibrary = this.GetDeviceCollectionLibrary();
                this._userlibrary = this.GetUserCollectionLibrary();
            }
            catch { return; }
        }


        public CollectionLibrary GetDeviceCollectionLibrary()
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='2'";
                CollectionLibrary library = new CollectionLibrary();

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                { 
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection collection = new SccmCollection();
                        collection.ID = resource["CollectionID"].StringValue;
                        collection.Name = resource["Name"].StringValue;
                        collection.LimitingCollectionID = resource["LimitToCollectionID"].StringValue;
                        collection.Comment = resource["Comment"].StringValue;
                        collection.IncludeExcludeCollectionCount = resource["IncludeExcludeCollectionsCount"].IntegerValue;

                        library.AddCollection(collection);
                    }
                }
                return library;
            }
            catch
            {
                return null;
            }
        }

        public CollectionLibrary GetUserCollectionLibrary()
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='1'";
                CollectionLibrary library = new CollectionLibrary();

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmCollection collection = new SccmCollection();
                        collection.ID = resource["CollectionID"].StringValue;
                        collection.Name = resource["Name"].StringValue;
                        collection.LimitingCollectionID = resource["LimitToCollectionID"].StringValue;
                        collection.Comment = resource["Comment"].StringValue;

                        library.AddCollection(collection);
                    }
                }
                return library;
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

        public Dictionary<string,SccmApplication> GetApplications()
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
                        SccmApplication app = new SccmApplication();
                        app.CIID = resource["CI_ID"].IntegerValue.ToString();
                        app.Name = resource["LocalizedDisplayName"].StringValue;
                        app.IsDeployed = resource["IsDeployed"].BooleanValue;
                        app.IsEnabled = resource["IsEnabled"].BooleanValue;
                        app.IsSuperseded = resource["IsSuperseded"].BooleanValue;
                        app.IsSuperseding = resource["IsSuperseding"].BooleanValue;
                        app.IsLatest = resource["IsLatest"].BooleanValue;
                        applications.Add(app.CIID,app);
                    }
                }
                
            }
            catch { }
            return applications;
        }

        public List<string> GetApplicationIDsFromSearch(string search)
        {
            List<string> appids = new List<string>();
            try
            {
                // This query selects all collections
                string query = "select CI_ID,LocalizedDisplayName from SMS_Application WHERE LocalizedDisplayName LIKE '%" + search + "%' AND IsLatest='TRUE'";
                
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        string ciid = resource["CI_ID"].IntegerValue.ToString();
                        appids.Add(ciid);
                    }
                }             
            }
            catch { }
            return appids;
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
                        SccmApplicationRelationship apprel = new SccmApplicationRelationship();
                        apprel.FromApplicationCIID = resource["FromApplicationCIID"].IntegerValue.ToString();
                        apprel.ToApplicationCIID = resource["ToApplicationCIID"].IntegerValue.ToString();
                        apprel.ToDeploymentTypeCIID = resource["ToDeploymentTypeCIID"].IntegerValue.ToString();
                        apprel.FromDeploymentTypeCIID = resource["FromDeploymentTypeCIID"].IntegerValue.ToString();
                        apprel.SetType(resource["TypeFlag"].IntegerValue);
                        relationships.Add(apprel);
                    }
                }              
            }
            catch { }
            return relationships;
        }


        public SccmDevice GetDevice(string devicename)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_FullCollectionMembership WHERE Name='" + devicename + "'";
                SccmDevice device = new SccmDevice();
                int count = 0;
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        if (count == 0)
                        {
                            device.ID = resource["ResourceID"].StringValue;
                            device.Name = resource["Name"].StringValue;
                            count++;
                        }                   
                        device.CollectionIDs.Add(resource["CollectionID"].StringValue);
                    }
                }
                return device;
            }
            catch
            {
                return null;
            }
        }
    }
}
