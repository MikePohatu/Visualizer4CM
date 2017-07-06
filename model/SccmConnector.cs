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

        public SccmConnector()
        {
            string configfile = @"c:\testauth.xml";

            XElement x;

            x = XmlHandler.Read(configfile);
            string authpw = XmlHandler.GetStringFromXElement(x, "Password", null);
            string authuser = XmlHandler.GetStringFromXElement(x, "User", null);
            string authdomain = XmlHandler.GetStringFromXElement(x, "Domain", null);

            this._connection = new WqlConnectionManager();
            this._connection.Connect("syscenter03.home.local", authdomain + "\\" + authuser, authpw);
            this._devlibrary = this.GetDeviceCollectionLibrary(_connection, "001");
            this._userlibrary = this.GetUserCollectionLibrary(_connection, "001");
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
