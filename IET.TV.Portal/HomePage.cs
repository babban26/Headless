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

            driver = new OpenQA.Selenium.PhantomJS.PhantomJSDriver();

            st.Chrome_Setup(driver, log);

          //  driver = new ChromeDriver(@"D:\All-Projects\Headless.Testing\IET.TV.Portal\bin\Debug\Driver");
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
            log.Info("searchVideoVerification::::");

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

                    log.Info("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    Console.WriteLine("Web Video Title:" + webvideoTitle + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    searchresultDetails.Click();

                    uf.isJqueryActive(driver);

                    break;
                }
            }
        }


        #endregion

        [Test]
        public void Video_Search()
        {
            Console.WriteLine("Done");

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("overlaySpinner")));

            driver.FindElement(By.Id("searchtextbox")).SendKeys("video");

            Console.WriteLine("Done");
        }

        [Test]
        public void Login()
        {

            #region Credential

            string username = "jahnavi.ette@northgate-is.com";
            string password = "R@ve54321";
            string userFullName = "jahnavi ette";

            #endregion

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("overlaySpinner")));

            Console.WriteLine("Home page is loaded.");

            // Navigating to Login page
            driver.FindElement(By.Id("login")).FindElement(By.TagName("a")).Click();
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("overlay")));
            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("overlaySpinner")));

            Console.WriteLine("Navigated to Login page.");

            // login operation
            iWait.Until(ExpectedConditions.ElementToBeClickable(By.Id("UserName")));
            driver.FindElement(By.Id("UserName")).SendKeys(username);
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.ClassName("login-btn")).Click();

            Console.WriteLine("User is logged in.");

            // waiting till welcome message displayed
            iWait.Until(ExpectedConditions.ElementIsVisible(By.Id("loginWelcomeMessage")));
            string welcomeMesssage = driver.FindElement(By.Id("message_head")).Text.Trim();

            // Verifying the user's email id on welcome message
            Assert.AreEqual(true, welcomeMesssage.Contains(username), "User's email ID is not displayed on welcome message");

            IWebElement btnOK = driver.FindElement(By.Id("loginWelcomeMessage"))
                                      .FindElement(By.ClassName("modal-footer")).FindElement(By.ClassName("btn-success"));

            btnOK.Click();

            iWait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("loginWelcomeMessage")));

            string actualFullUsername = driver.FindElement(By.Id("message")).Text.Trim();
            actualFullUsername = actualFullUsername.Replace("Welcome:", "").Trim();

            Assert.AreEqual(userFullName, actualFullUsername, "Username is not displayed after login");

            //Logout
            driver.FindElement(By.Id("accountLi")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.Id("anchorLogout")).Click();
                
            Console.WriteLine("User is logged out.");
        }

        [Test]
        public void PhantomTest()
        {
           searchVideoVerification("VID77A8DC");
          
        }

        [Test]
        public void HeadlessNegativeTest()
        {
            searchVideoVerification("VID77A8DC");
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
