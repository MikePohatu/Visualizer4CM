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
                this._devlibrary = this.GetDeviceCollectionLibrary(this._connection);
                this._userlibrary = this.GetUserCollectionLibrary(this._connection);
            }
            catch { return; }
        }


        public CollectionLibrary GetDeviceCollectionLibrary(WqlConnectionManager connection)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='2'";
                CollectionLibrary library = new CollectionLibrary();

                // Run query
                using (IResultObject results = connection.QueryProcessor.ExecuteQuery(query))
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

        public CollectionLibrary GetUserCollectionLibrary(WqlConnectionManager connection)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection WHERE CollectionType='1'";
                CollectionLibrary library = new CollectionLibrary();

                // Run query
                using (IResultObject results = connection.QueryProcessor.ExecuteQuery(query))
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
            try
            {
                // This query selects all collections
                string query = "select * from SMS_CollectionDependencies WHERE DependentCollectionID='" + collectionid + "'";
                List<SccmCollectionRelationship> relationships = new List<SccmCollectionRelationship>();
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
                return relationships;
            }
            catch
            {
                return null;
            }
        }

        public List<SccmApplication> GetApplications()
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Application";
                List<SccmApplication> applications = new List<SccmApplication>();
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplication app = new SccmApplication();
                        app.CIID = resource["CI_ID"].IntegerValue;
                        app.IsDeployed = resource["IsDeployed"].BooleanValue;
                        app.IsEnabled = resource["IsEnabled"].BooleanValue;
                        app.IsSuperseded = resource["IsSuperseded"].BooleanValue;
                        app.IsSuperseding = resource["IsSuperseding"].BooleanValue;
                        applications.Add(app);
                    }
                }
                return applications;
            }
            catch
            {
                return null;
            }
        }

        public List<SccmApplicationRelationship> GetApplicationDependencyRelationships(string applicationciid)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_AppDependenceRelation WHERE FromApplicationCIID='" + applicationciid + "'";

                List<SccmApplicationRelationship> dependencies = null;
                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        SccmApplicationRelationship apprel = new SccmApplicationRelationship();
                        apprel.FromApplicationCIID = resource["FromApplicationCIID"].IntegerValue;
                        apprel.ToApplicationCIID = resource["ToApplicationCIID"].IntegerValue;
                        apprel.ToDeploymentTypeCIID = resource["ToDeploymentTypeCIID"].IntegerValue;
                        apprel.FromDeploymentTypeCIID = resource["FromDeploymentTypeCIID"].IntegerValue;
                        dependencies.Add(apprel);
                    }
                }
                return dependencies;
            }
            catch
            {
                return null;
            }
        }


        public SccmDevice GetDevice(string devicename)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_FullCollectionMembership WHERE Name='" + devicename + "'";
                SccmDevice device = new SccmDevice();

                // Run query
                using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
                {
                    // Enumerate through the collection of objects returned by the query.
                    foreach (IResultObject resource in results)
                    {
                        device.ID = resource["ResourceID"].StringValue;
                        device.Name = resource["Name"].StringValue;
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
