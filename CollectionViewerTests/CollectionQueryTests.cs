using System;
using System.Xml.Linq;
using CollectionViewer;
using NUnit.Framework;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace CollectionViewerTests
{
    [TestFixture]
    public class CollectionQueryTests
    {
        [Test]
        [TestCase("SMS00001", ExpectedResult = null)]
        [TestCase("00100014", ExpectedResult = "SMS00001")]
        public string GetCollectionLimitingID(string collectionid)
        {
            string configfile = AppDomain.CurrentDomain.BaseDirectory + @"testauth.xml";

            XElement x;

            x = XmlHandler.Read(configfile);
            string authpw = XmlHandler.GetStringFromXElement(x, "Password", null);
            string authuser = XmlHandler.GetStringFromXElement(x, "User", null);
            string authdomain = XmlHandler.GetStringFromXElement(x, "Domain", null);

            WqlConnectionManager connection = new WqlConnectionManager();
            connection.Connect("syscenter03.home.local", authdomain + "\\" + authuser, authpw);
            SccmConnector connector = new SccmConnector();
            CollectionLibrary library = connector.GetCollectionLibrary(connection,"001");
            return library?.GetCollection(collectionid)?.LimitingCollectionID;
        }

        private void ReadConfigFile()
        {
            
        }
    }
}
