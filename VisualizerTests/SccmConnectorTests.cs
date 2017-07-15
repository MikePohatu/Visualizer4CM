using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using NUnit.Framework;
using viewmodel;

namespace VisualizerTests
{
    [TestFixture]
    public class SccmConnectorTests
    {
        [Test]
        [TestCase("Notepad++-copy", ExpectedResult = 16797702)]
        [TestCase("Notepad+", ExpectedResult = 16797698)]
        public int GetApplicationID(string appname)
        {
            SccmConnector connector = this.CreateAndConnectConnector();
            List<int> appids = connector.GetApplicationsListFromSearch(appname);
            foreach (int id in appids) { return id; }
            return -1;
        }

        [Test]
        [TestCase("SMS00001", ExpectedResult = null)]
        [TestCase("00100014", ExpectedResult = "SMS00001")]
        public string GetCollectionLimitingIDTest(string collectionid)
        {
            SccmConnector connector = this.CreateAndConnectConnector();
            CollectionLibrary library = connector.GetDeviceCollectionLibrary();
            return library?.GetCollection(collectionid)?.LimitingCollectionID;
        }

        [Test]
        [TestCase("Test1", ExpectedResult = "16777229")]
        public string GetDeviceQueryTest(string devicename)
        {
            SccmConnector connector = this.CreateAndConnectConnector();                
            SccmDevice device = connector.GetDevice(devicename);
            return device?.ID;

        }

        private SccmConnector CreateAndConnectConnector()
        {
            string configfile = @"C:\testauth.xml";

            XElement x;

            x = XmlHandler.Read(configfile);
            string authpw = XmlHandler.GetStringFromXElement(x, "Password", null);
            string authuser = XmlHandler.GetStringFromXElement(x, "User", null);
            string authdomain = XmlHandler.GetStringFromXElement(x, "Domain", null);
            string server = XmlHandler.GetStringFromXElement(x, "Server", null);

            SccmConnector connector = new SccmConnector();
            connector.Connect(authuser, authpw, authdomain, server);
            return connector;
        }
    }
}
