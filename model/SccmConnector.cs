using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace model
{
    public class SccmConnector
    {
        private WqlConnectionManager _connection;
        private CollectionLibrary _devlibrary;
        private CollectionLibrary _userlibrary;

        public CollectionLibrary DeviceCollectionLibrary { get { return this._devlibrary; } }
        public CollectionLibrary UserCollectionLibrary { get { return this._userlibrary; } }

        public SccmConnector(string authuser, string authpw, string authdomain, string server, string site)
        {
            this._connection = new WqlConnectionManager();
            this._connection.Connect(server, authdomain + "\\" + authuser, authpw);
            this._devlibrary = this.GetDeviceCollectionLibrary(_connection, site);
            this._userlibrary = this.GetUserCollectionLibrary(_connection, "001");
        }

        public SccmConnector(string server, string site)
        {
            this._connection = new WqlConnectionManager();
            this._connection.Connect(server);
            this._devlibrary = this.GetDeviceCollectionLibrary(_connection, site);
            this._userlibrary = this.GetUserCollectionLibrary(_connection, site);
        }

        public CollectionLibrary GetDeviceCollectionLibrary(WqlConnectionManager connection, string siteCode)
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

        public CollectionLibrary GetUserCollectionLibrary(WqlConnectionManager connection, string siteCode)
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
    }
}
