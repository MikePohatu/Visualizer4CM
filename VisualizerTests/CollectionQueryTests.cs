using System;
using System.Windows;
using System.Xml.Linq;
using Visualizer;
using NUnit.Framework;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using viewmodel;

namespace VisualizerTests
{
    [TestFixture]
    public class CollectionQueryTests
    {
        [Test]
        [TestCase(ExpectedResult = true)]
        public bool BuildTest()
        {
            CollectionLibrary library = new CollectionLibrary();
            SccmCollection allsys = new SccmCollection();
            allsys.ID = "SMS00001";
            allsys.Name = "All Systems";
            library.AddCollection(allsys);
            SccmCollection laballsys = new SccmCollection("00100014", "LAB All Systems", "SMS00001");
            library.AddCollection(laballsys);
            MessageBoxResult result = MessageBox.Show("Does this look right?", "Eval", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) { return true; }
            else { return false; }
        }
    }
}
