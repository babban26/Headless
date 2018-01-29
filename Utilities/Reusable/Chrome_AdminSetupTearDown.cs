using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Utility_Classes;
using Utilities.Config;
using Utilities.Object_Repository;
using log4net;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Expression.Encoder.ScreenCapture;
using Sikuli4Net.sikuli_UTIL;
using Sikuli4Net.sikuli_REST;


namespace IETTVAdminportal.Reusable
{
    class Chrome_AdminSetupTearDown
    {
        ScreenCaptureJob job = new ScreenCaptureJob();                                          // Instantiate object for Screen capture

        AdminAuth au = new AdminAuth();                                                         // Instantiate object for Authentication

        Configuration cf = new Configuration();                                                 // Instantiate object for Configuration

        Utility_Functions uf = new Utility_Functions();                                         // Instantiate object for Utility Functons 

        List<string> globList;

        APILauncher launcher;

        int screenHeight, screenWidth;

        string recordEvidence = "";

        string keepScreenShots = "";
        string testname = null;
        string appURL, newappURL;
        private Object_Repository_Class OR = new Object_Repository_Class();

        string authPath, username, password;

        public string Chrome_Setup(IWebDriver driver, ILog log, IJavaScriptExecutor executor)
        {

            try
            {
                testname = NUnit.Framework.TestContext.CurrentContext.Test.FullName;

                List<String> lstRecordEvidence = cf.readSysConfigFile("WebPortal", "Evidence", "SysConfig.xml");

                recordEvidence = lstRecordEvidence.ElementAt(1).ToString().ToLower();

                keepScreenShots = lstRecordEvidence.ElementAt(2).ToString().ToLower();

                if (recordEvidence == "yes")
                {
                    job = new ScreenCaptureJob();

                    uf.ScreenCap(job, testname, driver);
                }

                screenHeight = uf.getScreenHeight(driver);

                screenWidth = uf.getScreenWidth(driver);

                log.Info("Screen Height:" + screenHeight + "Screen Width:" + screenWidth + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(300));

               driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

              //driver.Manage().Window.Size = new Size(1600, 900);
                driver.Manage().Window.Size = new Size(screenWidth, screenHeight);

                driver.Manage().Cookies.DeleteAllCookies();

                // This will read the application URL from Config.XML
                username = cf.readingXMLFile("AdminPortal", "Login", "userName", "Config.xml");

                password = cf.readingXMLFile("AdminPortal", "Login", "passWord", "Config.xml");

                appURL = cf.readingXMLFile("AdminPortal", "Login", "startURL", "Config.xml");

                newappURL = "http://" + username + ":" + password + "@" + appURL;

                driver.Navigate().GoToUrl(newappURL);

                globList = cf.readSysConfigFile("Global", "Automation", "SysConfig.xml");

                // This is to check if Sikuli setting is set to 'Yes'

                if (globList.ElementAt(0).ToString().ToLower().Equals("yes"))
                {

                    // This is to check if Sikuli setting is set to 'Yes' then check for sikuli port avaibility and launch sikuli

                    bool sikuliCheck = uf.sikuliPortCheck();

                    Console.WriteLine("Sikuli Check:" + sikuliCheck);

                    log.Info("Sikuli Check:" + sikuliCheck);

                    if (sikuliCheck)
                    {
                        log.Info("Launching Sikuli as sikuli port is available");

                        launcher = new APILauncher(true);

                        launcher.Start();
                    }
                    else
                    {
                        log.Info("Sikuli port is unavailable. Please free the 8080 port in order to start Sikuli");
                    }

                    // This is to check if Sikuli Jetty service is up. Try 90 sec for jetty service to get up

                    bool jettyUpCheck = uf.jettyServiceUpCheck();

                    Console.WriteLine("Jetty Up Check:" + jettyUpCheck);

                    log.Info("Jetty Up Check:" + jettyUpCheck);

                    if (jettyUpCheck)
                    {
                        log.Info("Jetty service is up");
                    }
                    else
                    {
                        log.Info("Jetty service is not running");
                    }

                    Boolean statLogin = au.sikuliAuthLogin("Chrome");

                    log.Info("Login Status:" + statLogin + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                }
            }
            catch (Exception e)
            {
                log.Error("Error occurred in Setup" + e.Message + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

            return newappURL;
        }

        public void Chrome_TearDown(IWebDriver driver, ILog log)
        {
            try
            {

                if (driver != null)
                    Thread.Sleep(5000);
                driver.Quit();

                if (recordEvidence == "yes")
                {
                    this.job.Stop();

                    this.job = null;
                }

                log.Info("Test Completed" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

                this.job.Stop();

                Assert.AreEqual(true, false);
            }

            try
            {
                // This is to check if Sikuli setting is set to 'Yes' and then stop the launcher

                if (globList.ElementAt(0).ToString().ToLower().Equals("yes"))
                {
                    launcher.Stop();
                }
            }
            catch (Exception e)
            {
                Process[] proc = Process.GetProcessesByName("javaw");

                proc[0].Kill();

                log.Info("Javaw process killed");
            }
        }

        public void Chrome_SetUpTearDowm(IWebDriver driver, ILog log, Boolean headerpresent)
        {
            String title = null;
            int titleCnt = 0;

            while (title == null && titleCnt < 10)
            {
                titleCnt++;

                try
                {
                    title = driver.Title;
                }
                catch (UnhandledAlertException)
                {
                    log.Info("Handling Alert ::::::::::::::::::::::::::::::::::");
                    driver.SwitchTo().Alert().Accept();
                }
            }
            try
            {
                if (keepScreenShots == "yes")
                {
                    IList<IWebElement> headerElements = null;

                    if (headerpresent)
                    {
                        int headerCount = driver.FindElements(By.Id(OR.readingXMLFile("Header", "TopHeader", "TVAdminPortalOR.xml"))).Count();
                        int logoCount = driver.FindElements(By.ClassName(OR.readingXMLFile("Header", "LogoHeader", "TVAdminPortalOR.xml"))).Count();

                        if (headerCount >= 1 || logoCount >= 1)
                        {

                            IWebElement ele = driver.FindElement(By.Id(OR.readingXMLFile("Header", "TopHeader", "TVAdminPortalOR.xml")));
                            IWebElement ele1 = driver.FindElement(By.ClassName(OR.readingXMLFile("Header", "LogoHeader", "TVAdminPortalOR.xml")));
                            headerElements = new List<IWebElement>();
                            headerElements.Add(ele);
                            headerElements.Add(ele1);

                        }
                        else
                            headerpresent = false;
                    }

                    uf.TakeScreenshot(driver, "other", testname, headerpresent, headerElements);
                    Thread.Sleep(2000);

                }
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                log.Info("\n \n --------------------------------------------------------------------------------------------------------------------------------");

                Assert.AreEqual(true, false);
            }
        }

    }
}
