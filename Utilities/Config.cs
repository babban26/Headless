using System;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using System.Diagnostics;


namespace Utilities.Config
{
   public class Configuration
    {
         
        // This is to configure logger mechanism for Utilities.Config
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
   
        public String readingXMLFile(String projectName, String ModuleName, String key, String fileName)
        {
            Log.Info("\n Control is inside method : readingXMLFile of Configuration" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            try
            {
                string configDir = AppDomain.CurrentDomain.BaseDirectory + "Configuration\\" + fileName;

                Log.Info("Config Directory:=" + configDir + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                String requiredData = null;

                // This is to load the config.xml file

                XmlTextReader reader = new XmlTextReader(configDir);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                // Move the control to 'Configuration' node.
                XmlNode node = doc.SelectSingleNode("Configuration");

                XmlNodeList projectNodeList = node.SelectNodes("Project");
      
                //Iterating through Project node
                foreach (XmlNode tempNode in projectNodeList)
                {
                    //get the value of 'name' attribute of current project node
                    String attribute = tempNode.Attributes["name"].Value;

                    //checking whether the current Project node is the required node
                    if (attribute.Equals(projectName))
                    {

                        Log.Info("Required project node is present" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        // get all the child node of Project
                        XmlNodeList moduleNodeList = tempNode.ChildNodes;

                        String eleAttribute = null;

                        //Iterating through Child node of project node
                        foreach (XmlNode tempModuleNode in moduleNodeList)
                        {
                            String attributeOfModuleNode = tempModuleNode.Attributes["name"].Value;

                            if (attributeOfModuleNode.Equals(ModuleName))
                            {
                                XmlNodeList elemnetNodeList = tempModuleNode.ChildNodes;

                                foreach (XmlNode tempElementNode in elemnetNodeList)
                                {

                                    if (fileName.Equals("Config.xml"))
                                    { 
                                        eleAttribute = tempElementNode.Attributes["key"].Value;
                                    }
                                    else if (fileName.Equals("SysConfig.xml"))
                                    {
                                        eleAttribute = tempElementNode.Name;  
                                    }
                                    else if (fileName.Equals("TestCopy.xml"))
                                    {
                                        eleAttribute = tempElementNode.Name;
                                    }
                                     
                                    if (eleAttribute.Equals(key))
                                    {
                                        requiredData = tempElementNode.InnerText;
                                        break;
                                    }
                                }

                            }

                        }


                    }

                }


                reader.Close();
            //    reader.Dispose();

                Log.Info("Require Data from config.cs " + requiredData + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return requiredData;
            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return null;
            }
        }




        // This function returns complete config information from SysConfig.XML
        public List<String> readSysConfigFile(String portal, String moduleName, String fileName)
        {
            Log.Info("\n Control is inside method : readSysConfigFile of Configuration" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            try
            {
                List<String> randomDataList = new List<String>();

                //Storing the file path

                string sysConfigDir = AppDomain.CurrentDomain.BaseDirectory + @"\Configuration\" + fileName;

                Log.Info("Sys Config Directory:=" + sysConfigDir + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                XmlTextReader reader = new XmlTextReader(sysConfigDir);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);

                // move the control to 'Configuration' node.
                XmlNode node = doc.SelectSingleNode("Configuration");

                // get the all the node of 'Setting' node.
                XmlNodeList SettingeNodeList = node.SelectNodes("Project");

                // Iterating through Module node

                foreach (XmlNode tempSettingNode in SettingeNodeList)
                {

                    //get the value of 'Name' attribute of current Setting node.                    
                    String attribute = tempSettingNode.Attributes["name"].Value;

                    //checking whether the current Module node is the required node
                    if (attribute.Equals(portal))
                    {
                        //get all the child node of Setting node
                        XmlNodeList ModuleNodeList = tempSettingNode.ChildNodes;

                        foreach (XmlNode tempModuleNode in ModuleNodeList)
                        {
                            if ((tempModuleNode.Attributes["name"].Value).Equals(moduleName))
                            {
                                XmlNodeList dataListNodes = tempModuleNode.ChildNodes;   //Getting control of all nodes under our required module node.

                                foreach (XmlNode elementNode in dataListNodes)
                                {
                                    randomDataList.Add(elementNode.InnerText);
                                }
                                break;   //Break tempModuleNode for loop
                            }
                        }

                        break; //Break TempSettingNode for loop
                    }

                }

                reader.Close();
              //  reader.Dispose();

                return randomDataList;

            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return null;
            }
        }




        public void writingIntoXML(String portal, String moduleName, string NodeName, string Value, String fileName)
        {
            try
            {

                XmlDocument xmlDoc = new XmlDocument();

                string sysConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Configuration\" + fileName;  //Storing the file path

                xmlDoc.Load(sysConfigFilePath);

                XmlNode node = xmlDoc.SelectSingleNode("Configuration");

                // get the all the node of 'Configuration' node.
                XmlNodeList SettingeNodeList = node.SelectNodes("Project");

                Log.Info("No of Setting Node ::: " + SettingeNodeList.Count + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                //Iterating through Module node
                foreach (XmlNode tempSettingNode in SettingeNodeList)
                {
                    String attribute = tempSettingNode.Attributes["name"].Value;   //get the value of 'Name' attribute of current Setting node.
                 
                    //checking whether the current Module node is the required node
                    if (attribute.Equals(portal))
                    {
                        //get all the child node of Setting node
                        XmlNodeList ModuleNodeList = tempSettingNode.ChildNodes;

                        foreach (XmlNode tempModuleNode in ModuleNodeList)
                        {
                            if ((tempModuleNode.Attributes["name"].Value).Equals(moduleName))
                            {
                                // XmlNodeList dataListNodes = tempModuleNode.ChildNodes;   //Getting control of all nodes under our required module node.

                                XmlNode requiredNode = tempModuleNode.SelectSingleNode(NodeName);
                                requiredNode.InnerText = Value;
                                //  prefixNode.InnerText = (currentPrefixValue + 1).ToString();
                                break; //Break TempSettingNode for loop
                            }
                        }
                    }

                }
                xmlDoc.Save(sysConfigFilePath);

            }
            catch (Exception e)
            {
                Log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }

        }

        #region

        //public void writingExtraDataList(String portal, String moduleName, String fileName, List<String> extraDataList)
        //{
        //    XmlDocument xmlDoc = new XmlDocument();

        //    string sysConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Configuration\" + fileName;  //Storing the file path
        //    Console.WriteLine("OR Directory:=" + sysConfigFilePath);

        //    xmlDoc.Load(sysConfigFilePath);

        //    XmlNode node = xmlDoc.SelectSingleNode("Configuration");

        //    // get the all the node of 'Configuration' node.
        //    XmlNodeList SettingeNodeList = node.SelectNodes("Setting");

        //    Console.WriteLine("No of Setting Node ::: " + SettingeNodeList.Count);

        //    //Iterating through Module node
        //    foreach (XmlNode tempSettingNode in SettingeNodeList)
        //    {

        //        String attribute = tempSettingNode.Attributes["name"].Value;   //get the value of 'Name' attribute of current Setting node.
        //        Console.WriteLine("Name Attribute:: " + attribute);

        //        //checking whether the current Module node is the required node
        //        if (attribute.Equals(portal))
        //        {
        //            //get all the child node of Setting node
        //            XmlNodeList ModuleNodeList = tempSettingNode.ChildNodes;

        //            foreach (XmlNode tempModuleNode in ModuleNodeList)
        //            {
        //                if ((tempModuleNode.Attributes["name"].Value).Equals(moduleName))
        //                {
        //                     XmlNodeList dataListNodes = tempModuleNode.ChildNodes;   //Getting control of all nodes under our required module node.
                            
        //                     Console.WriteLine("Count List :: " + dataListNodes.Count);

        //                    int i= 2;
        //                    int j = 0;
        //                    foreach (String content in extraDataList)
        //                    {
        //                        dataListNodes.Item(i++).InnerText = extraDataList[j++];
        //                    }

                       
        //                    break; //Break TempSettingNode for loop
        //                }
        //            }
        //        }

        //    }





        //    xmlDoc.Save(sysConfigFilePath);
            
        //}
        #endregion
    }
}
