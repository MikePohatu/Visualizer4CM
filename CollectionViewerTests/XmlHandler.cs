using System;
using System.IO;
using System.Xml.Linq;

namespace CollectionViewerTests
{
    public static class XmlHandler
    {
        public static void Write(string pPath, XElement pElement)
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), pElement);
            string dirPath = Path.GetDirectoryName(pPath);
            if (!Directory.Exists(dirPath)) { Directory.CreateDirectory(dirPath); }
            xDoc.Save(pPath);
        }

        public static XElement Read(string pPath)
        {
            //LoadOptions options = new LoadOptions;
            //LoadOptions.PreserveWhitespace = true;
            XElement temp = null;
            if (!File.Exists(pPath)) 
            { throw new FileNotFoundException("File not found: " + pPath); }
            try { temp = XElement.Load(pPath); }
            catch (Exception e)
            { throw new InvalidOperationException("Unable to read xml file: " + pPath + Environment.NewLine + e.Message); }

            return temp;
        }


        //XElement functions
        public static string GetStringFromXElement(XElement InputXml, string XName, string DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return x.Value; }
            else { return DefaultValue; }
        }

        public static int GetIntFromXElement(XElement InputXml, string XName, int DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return Convert.ToInt32(x.Value); }
            else { return DefaultValue; }
        }

        public static double GetDoubleFromXElement(XElement InputXml, string XName, double DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XElement x;

            x = InputXml.Element(XName);
            if (x != null)
            {
                if (x.Value.ToUpper() == "AUTO") { return Double.NaN; }
                else { return Convert.ToDouble(x.Value); }
            }
            else { return DefaultValue; }
        }

        public static bool GetBoolFromXElement(XElement InputXml, string XName, bool DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XElement x;

            x = InputXml.Element(XName);
            if (x != null) { return Convert.ToBoolean(x.Value); }
            else { return DefaultValue; }
        }

        //XAttribute functions
        public static string GetStringFromXAttribute(XElement InputXml, string XName, string DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return x.Value; }
            else { return DefaultValue; }
        }

        public static int GetIntFromXAttribute(XElement InputXml, string XName, int DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return Convert.ToInt32(x.Value); }
            else { return DefaultValue; }
        }

        public static double GetDoubleFromXAttribute(XElement InputXml, string XName, double DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null)
            {
                if (x.Value.ToUpper() == "AUTO") { return Double.NaN; }
                else { return Convert.ToDouble(x.Value); }
            }
            else { return DefaultValue; }
        }

        public static bool GetBoolFromXAttribute(XElement InputXml, string XName, bool DefaultValue)
        {
            if (InputXml == null) { return DefaultValue; }

            XAttribute x;

            x = InputXml.Attribute(XName);
            if (x != null) { return Convert.ToBoolean(x.Value); }
            else { return DefaultValue; }
        }
    }
}
