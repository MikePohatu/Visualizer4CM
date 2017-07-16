﻿using System;
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
                this._devlibrary = this.GetCollectionLibrary(CollectionType.Device);
                this._userlibrary = this.GetCollectionLibrary(CollectionType.User);
            }
            catch { return; }
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
                        library.AddCollection(new SccmCollection(resource));
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
                        newlist.Add(new SccmCollection(resource));
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
                        SccmApplication app = new SccmApplication(resource);
                        applications.Add(app.CIID,app);
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
                        SccmApplication app = new SccmApplication(resource);
                        applications.Add(app.CIID, app);
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
                        SccmApplication app = new SccmApplication(resource);
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

        //public SccmApplication GetApplication(string ciid)
        //{
        //    try
        //    {
        //        // This query selects all collections
        //        string query = "select * from SMS_Application WHERE IsLatest='TRUE' AND CI_ID='" + ciid + "'";

        //        // Run query
        //        using (IResultObject results = this._connection.QueryProcessor.ExecuteQuery(query))
        //        {
        //            // Enumerate through the collection of objects returned by the query.
        //            foreach (IResultObject resource in results)
        //            {
        //                SccmApplication app = new SccmApplication();
        //                app.CIID = resource["CI_ID"].IntegerValue.ToString();
        //                app.Name = resource["LocalizedDisplayName"].StringValue;
        //                app.IsDeployed = resource["IsDeployed"].BooleanValue;
        //                app.IsEnabled = resource["IsEnabled"].BooleanValue;
        //                app.IsSuperseded = resource["IsSuperseded"].BooleanValue;
        //                app.IsSuperseding = resource["IsSuperseding"].BooleanValue;
        //                app.IsLatest = resource["IsLatest"].BooleanValue;
        //                return app;
        //            }
        //        }

        //    }
        //    catch { }
        //    return null;
        //}

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
