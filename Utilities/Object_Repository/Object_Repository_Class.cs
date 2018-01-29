using System;
using System.Xml;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using OpenQA.Selenium;

namespace Utilities.Object_Repository
{
    public class Object_Repository_Class
    {

        // This is to configure logger mechanism for Utilities.Object Repository

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public String readingXMLFile(String moduleName, String key, String fileName)
        {
           // Log.Info("\n Control is inside method : readingXMLFile of OR" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            try
            {

                string orDir = AppDomain.CurrentDomain.BaseDirectory + @"\Object_Repository\" + fileName;

                //Log.Info("OR Directory:=" + orDir + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String requiredData = null;

                // This is to load the OR.xml file

                XmlTextReader reader = new XmlTextReader(orDir);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                // move the control to 'Properties' node.
                XmlNode node = doc.SelectSingleNode("properties");

                // get the list of 'Module' node.
                XmlNodeList moduleNodeList = node.SelectNodes("Module");

                // Iterating through Module node
                foreach (XmlNode tempNode in moduleNodeList)
                {
                    // get the value of 'name' attribute.
                    String attribute = tempNode.Attributes["name"].Value;
                    
                    // checking whether the current Module node is the required node
                    if (attribute.Equals(moduleName))
                    {

                       // Log.Info("Module Name is present" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                        
                        // get all the child node of Module

                        XmlNodeList ElementNodeList = tempNode.ChildNodes;
                        
                        foreach (XmlNode tempEleNode in ElementNodeList)
                        {
                            if (tempEleNode.NodeType.ToString().Equals("Element"))
                            {

                                // get the value of 'Key' attribute.
                                String eleAttribute = tempEleNode.Attributes["key"].Value;

                                // checking whether the current Element node is the required node

                                if (eleAttribute.Equals(key))
                                {
                                    requiredData = tempEleNode.InnerText;
                                    break;
                                }
                            }
                        }

                        break;
                    }

                }

                reader.Close();
                reader.Dispose();

               // Log.Info("require Data from Object_Repository_Class.cs " + requiredData + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return requiredData;
            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return null;
            }

        }

        public String readingXMLFileFromType(String moduleName, String key, String fileName)
        {
           // Log.Info("\n Control is inside method : readingXMLFile of OR" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            try
            {

                string orDir = AppDomain.CurrentDomain.BaseDirectory + @"\Object_Repository\" + fileName;

             //   Log.Info("OR Directory:=" + orDir + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String requiredData = null;

                // This is to load the OR.xml file

                XmlTextReader reader = new XmlTextReader(orDir);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                // move the control to 'Properties' node.
                XmlNode node = doc.SelectSingleNode("properties");

                // get the list of 'Module' node.
                XmlNodeList moduleNodeList = node.SelectNodes("Module");

                // Iterating through Module node
                foreach (XmlNode tempNode in moduleNodeList)
                {
                    // get the value of 'name' attribute.
                    String attribute = tempNode.Attributes["name"].Value;

                    // checking whether the current Module node is the required node
                    if (attribute.Equals(moduleName))
                    {

                        //Log.Info("Module Name is present" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        // get all the child node of Module

                        XmlNodeList ElementNodeList = tempNode.ChildNodes;

                        foreach (XmlNode tempEleNode in ElementNodeList)
                        {
                            if (tempEleNode.NodeType.ToString().Equals("Element"))
                            {

                                // get the value of 'Key' attribute.
                                String eleAttribute = tempEleNode.Attributes["key"].Value;

                                // checking whether the current Element node is the required node

                                if (eleAttribute.Equals(key))
                                {
                                    requiredData = tempEleNode.Attributes["type"].Value;
                                    break;
                                }
                            }
                        }

                        break;
                    }

                }

                reader.Close();
                reader.Dispose();

              //  Log.Info("require Data from Object_Repository_Class.cs " + requiredData + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return requiredData;
            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return null;
            }

        }

        public By GetElement(string moduleName, string key, string fileName)
        {
            

            string elementType = this.readingXMLFileFromType(moduleName, key, fileName);
            string elementName = this.readingXMLFile(moduleName,key,fileName);

            #region Null Handling

            By by = null;
            if (string.IsNullOrEmpty(elementType))
            {
                throw new Exception("No type present for the key : "+key+" in "+fileName);
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new Exception("No inner text present for the key : " + key + " in " + fileName);
            }

            #endregion

            switch (elementType.ToLower())
            {
                case "id":
                    by = By.Id(elementName);
                    break;
                case "classname":
                    by = By.ClassName(elementName);
                    break;
                case "cssselector":
                    by = By.CssSelector(elementName);
                    break;
                case "linktext":
                    by = By.LinkText(elementName);
                    break;
                case "name":
                    break;
                case "xpath":
                    by = By.XPath(elementName);
                    break;
                case "partiallinktext":
                    by = By.PartialLinkText(elementName);
                    break;
                case "tagname":
                    by = By.TagName(elementName);
                    break;
            }
            return by;
        }

        public By GetElement(string moduleName, string key, string fileName,int counter)
        {


            string elementType = this.readingXMLFileFromType(moduleName, key, fileName);
            string elementName = this.readingXMLFile(moduleName, key, fileName);

            #region Null Handling

            By by = null;
            if (string.IsNullOrEmpty(elementType))
            {
                throw new Exception("No type present for the key : " + key + " in " + fileName);
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new Exception("No inner text present for the key : " + key + " in " + fileName);
            }

            #endregion

            switch (elementType.ToLower())
            {
                case "id":
                    by = By.Id(elementName+counter);
                    break;
                case "classname":
                    by = By.ClassName(elementName+counter);
                    break;
                case "cssselector":
                    by = By.CssSelector(elementName+counter);
                    break;
                case "linktext":
                    by = By.LinkText(elementName+counter);
                    break;
                case "name":
                    by = By.Name(elementName+counter);
                    break;
                case "xpath":
                    by = By.XPath(elementName+counter);
                    break;
                case "partiallinktext":
                    by = By.PartialLinkText(elementName+counter);
                    break;
                case "tagname":
                    by = By.TagName(elementName+counter);
                    break;
            }
            return by;
        }
    }
}
