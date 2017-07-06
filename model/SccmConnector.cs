using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace model
{
    public class SccmConnector
    {
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
