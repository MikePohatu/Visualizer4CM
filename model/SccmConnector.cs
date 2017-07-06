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
        private CollectionLibrary _library;

        public CollectionLibrary Library { get { return this._library; } }

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
            this._library = this.GetCollectionLibrary(_connection, "001");
        }

        public CollectionLibrary GetCollectionLibrary(WqlConnectionManager connection, string siteCode)
        {
            try
            {
                // This query selects all collections
                string query = "select * from SMS_Collection";
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
