using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ImageConverter.Logging
{
    public class XMLLog : IXMLLog
    {
        private readonly string _fileName;
        public XMLLog(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(fileName);
            
            _fileName = fileName;
            
            if (File.Exists(fileName))
            {
                if (!ControlFileStructure())
                {
                    File.Delete(_fileName);
                    CreateFile();
                }
            }
            else
            {
                CreateFile();
            }
        }

        private void CreateFile()
        {
            XDocument xDoc = new XDocument(new XDeclaration("1.0","utf-8",null));
            XElement rootElement = new XElement("log");
            XElement infosElement = new XElement("infos");
            XElement debugsElement = new XElement("debugs");
            XElement errorsElement = new XElement("errors");
            rootElement.Add(infosElement);
            rootElement.Add(debugsElement);
            rootElement.Add(errorsElement);
            xDoc.Add(rootElement);
            using (StreamWriter sw = new StreamWriter(_fileName))
            {
                xDoc.Save(sw);
            }
        }

        private bool ControlFileStructure()
        {
            XElement element = null;
            try
            {
                element = XElement.Load(_fileName);
            }
            catch (IOException)
            {
                return false;
            }
            if (element == null)
                return false;
            if (element.Name != "log")
                return false;
            int num = element.Descendants("infos").Count();
            if (num != 1)
                return false;
            num = element.Descendants("debugs").Count();
            if (num != 1)
                return false;
            num = element.Descendants("errors").Count();
            if (num != 1)
                return false;
            return true;
        }

        public void Info(string message)
        {
            XDocument xDoc = XDocument.Load(_fileName);
            XElement info = new XElement("info");
            info.SetValue(message);
            info.SetAttributeValue("time",GetTimestamp(DateTime.Now));
            var x = xDoc.XPathSelectElement("/log/infos");
            x.Add(info);
            using (StreamWriter sw = new StreamWriter(_fileName))
            {
                xDoc.Save(sw);
            }
        }

        public void Debug(string message)
        {
            XDocument xDoc = XDocument.Load(_fileName);
            XElement info = new XElement("debug");
            info.SetValue(message);
            info.SetAttributeValue("time", GetTimestamp(DateTime.Now));
            var x = xDoc.XPathSelectElement("/log/debugs");
            x.Add(info);
            using (StreamWriter sw = new StreamWriter(_fileName))
            {
                xDoc.Save(sw);
            }
        }

        public void Error(string message)
        {
            XDocument xDoc = XDocument.Load(_fileName);
            XElement info = new XElement("error");
            info.SetValue(message);
            info.SetAttributeValue("time", GetTimestamp(DateTime.Now));
            var x = xDoc.XPathSelectElement("/log/errors");
            x.Add(info);
            using (StreamWriter sw = new StreamWriter(_fileName))
            {
                xDoc.Save(sw);
            }
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy-MM-dd\\HH:mm:ss:fff");
        }
    }
}
