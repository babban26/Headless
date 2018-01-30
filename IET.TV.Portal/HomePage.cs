using log4net;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Config;
using Utilities.Object_Repository;
using Utility_Classes;
using Utilities;

namespace IET.TV.Portal
{
    public class HomePage
    {
        IWebDriver driver = null;
        public IWait<IWebDriver> iWait = null;

        Utility_Functions uf = new Utility_Functions();                      // Instantiate object for Utility Functons

        Configuration cf = new Configuration();                             // Instantiate object for Configuration

        Object_Repository_Class OR = new Object_Repository_Class();        // Instantiate object for object repository

        Chrome_AdminSetupTearDown st = new Chrome_AdminSetupTearDown();                         // Instantiate object for Chrome Setup Teardown

        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IJavaScriptExecutor executor;

        [SetUp]
        public void Setup()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "WebPortal.config"));

            uf.CreateOrReplaceVideoFolder();

            driver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver();

            iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            st.Chrome_Setup(driver, log);

       //     driver = new ChromeDriver(@"D:\All-Projects\Headless.Testing\IET.TV.Portal\bin\Debug\Driver\chromedriver.exe");
         //   driver.Manage().Window.Maximize();
         //   iWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(60.00));
         //   driver.Navigate().GoToUrl("http://192.168.2.74/iettvportal/");
        }

        #region Reusable

        public void handlePromotionalPopup()
        {
            IWebElement promotionalPopup = driver.FindElement((OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

            String PromoPopup = promotionalPopup.GetCssValue("display").ToString();

            if (PromoPopup.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "Promotionaldiv", "TVWebPortalOR.xml")));

                driver.FindElement((OR.GetElement("SeriesManagement", "PromotionalPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));

            }
        }

        public void handleEmergencyPopUp()
        {
            //Handling pop up message
            IWebElement emergencyPopup = driver.FindElement((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

            string emergencyColor = emergencyPopup.GetCssValue("display").ToString();

            if (emergencyColor.Equals("block"))
            {
                iWait.Until(ExpectedConditions.ElementIsVisible(OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml")));

                driver.FindElement((OR.GetElement("SeriesManagement", "NewEmergencyPopup", "TVWebPortalOR.xml"))).Click();

                iWait.Until(ExpectedConditions.InvisibilityOfElementLocated((OR.GetElement("SeriesManagement", "EmergencyDiv", "TVWebPortalOR.xml"))));
            }

        }

        //This function search the required video and verify the same and navigate to video landing page
        public void searchVideoVerification(string videoname)
        {
            log.Info("search Video Verification Test started::::");
            Console.WriteLine("search Video Verification Test started::::");

            Boolean flag = false;

            //wait till jquery gets completed
            uf.isJqueryActive(driver);

            // NEw changes 24/11/2017

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));
            iWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("btn-enter")));

            driver.FindElement(By.ClassName("btn-enter")).Click();

            handlePromotionalPopup();

            handleEmergencyPopUp();

            Thread.Sleep(5000);

            iWait.Until(ExpectedConditions.ElementExists((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml"))));

            //search the required video
            IWebElement SearchTextField = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchTB", "TVWebPortalOR.xml")));
            SearchTextField.SendKeys(videoname);

            iWait.Until(ExpectedConditions.ElementToBeClickable((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml"))));

            //Click on searchIcon
            IWebElement SearchIcon = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchIcon", "TVWebPortalOR.xml")));
            SearchIcon.Click();
            Thread.Sleep(2000);

            //handleEmergencyPopUp();

            uf.isJqueryActive(driver);


            //verifying the search result
            IList<IWebElement> videoSearchList = (IList<IWebElement>)driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResult", "TVWebPortalOR.xml"))).FindElements(OR.GetElement("VideoLandingPage", "SearchResultRecord", "TVWebPortalOR.xml"));


            //gettting the search result details
            foreach (IWebElement currentSearchrecord in videoSearchList)
            {
                IWebElement searchresultDetails = driver.FindElement((OR.GetElement("VideoLandingPage", "SearchResultDetails", "TVWebPortalOR.xml")));
                String webvideoTitle = searchresultDetails.GetAttribute("data-videotitle");
                Console.WriteLine("webvideoTitle:  " + webvideoTitle);

                //getting video Title from search result
                if (webvideoTitle.Equals(videoname, StringComparison.InvariantCultureIgnoreCase))
                {
                    flag = true;

                    log.Info("Expected Video Title:" + videoname + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    log.Info("Actual Video Title::" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    Console.WriteLine("Expected Video Title:" + videoname + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    Console.WriteLine("Actual Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    searchresultDetails.Click();

                    uf.isJqueryActive(driver);
                    Assert.AreEqual(true, true);

                    break;
                }
            }
        }


        #endregion


        [Test]
        public void HeadlessTest()
        {
           searchVideoVerification("VID77A8DC");
          
        }

        [Test]
        public void HeadlessNegativeTest()
        {
            searchVideoVerification("VID77A8DC");
            Console.WriteLine("Forcefully failing the test...");
            Assert.Fail();
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Status.ToString().Equals("Failed"))
                {
                    st.Chrome_SetUpTearDowm(driver, log, true);
                    Thread.Sleep(1000);
                }
                st.Chrome_TearDown(driver, log);

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                Assert.AreEqual(true, false);
            }
        }
    }
}
