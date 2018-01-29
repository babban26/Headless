using System;
using NUnit.Framework;
using OpenQA.Selenium.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Threading;
using System.Drawing;
using System.IO;
using OpenQA.Selenium.Support.UI;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Win32;
//using Microsoft.Expression.Encoder.ScreenCapture;
using Utilities.Config;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using System.Linq;
using System.Net.NetworkInformation;
using OpenQA.Selenium.PhantomJS;    //for PhantomJSDriver();
using NReco.PhantomJS;
using OpenQA.Selenium.Support.Events;
using System.Runtime.CompilerServices;
using System.Reflection; //var phantomJS = new PhantomJS();


namespace Utility_Classes
{

    public class Utility_Functions
    {

        // This is to configure logger mechanism for Utilities.Config
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string evidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy");

        string chromeVideosEvidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Chrome\\Videos";

        string firefoxVideosEvidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Firefox\\Videos";

        string iEVideosEvidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\IE\\Videos";

        string chromeScreenShotsEvidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Chrome\\ScreenShots";

        string firefoxScreenShotsEvidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Firefox\\ScreenShots";

        string iEScreenShotsEvidencePath = "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\IE\\ScreenShots";

        int screenHeight, screenWidth;

        Configuration cf = new Configuration();


        // This function returns screen height 
        public int getScreenHeight(IWebDriver driver)
        {

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            return Int32.Parse(js.ExecuteScript("return screen.height").ToString());
        }

        // This function returns screen width 
        public int getScreenWidth(IWebDriver driver)
        {

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            return Int32.Parse(js.ExecuteScript("return screen.width").ToString());
        }

        // This function returns currently running browser
        public string getRunningBrowser(IWebDriver driver, IWebElement element)
        {
            string result, currentBrowser = null, ie10Result = null, ie11Result = null, ie9Result = null;

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            result = (string)js.ExecuteScript("return navigator.userAgent", element);

            ie11Result = (string)js.ExecuteScript("return !((window.ActiveXObject) && " + "ActiveXObject" + ")" + "", element).ToString().ToLower();

            ie10Result = (string)js.ExecuteScript("return navigator.appVersion.indexOf('MSIE 10')", element).ToString();

            ie9Result = (string)js.ExecuteScript("return navigator.appVersion.indexOf('MSIE 9')", element).ToString();

            if (result.Contains("Chrome"))
            {
                currentBrowser = "Chrome";
            }
            else if (result.Contains("Firefox"))
            {
                currentBrowser = "Firefox";
            }
            else if (result.Contains("PhantomJS"))
            {
                currentBrowser = "PhantomJS";
            }
            else if (!ie9Result.Equals("-1"))
            {
                currentBrowser = "IE9";
            }
            else if (!ie10Result.Equals("-1"))
            {
                currentBrowser = "IE10";
            }
            else if (ie11Result.Equals("true"))
            {
                currentBrowser = "IE11";
            }

            Console.WriteLine("Running Browser" + currentBrowser);

            log.Info("Currently running browser is " + currentBrowser);

            return currentBrowser;
        }

        // This functions checks if registry entry is present for IE11 on system otherwise set accordingly
        public Boolean checkIE11RegistryPresence()
        {

            Boolean regFlag = false;

            try
            {
                string bit64keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BFCACHE";  // FEATURE_BFCACHE subkey may or may not be present, and should be created if it is not present

                string bit32keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BFCACHE"; // FEATURE_BFCACHE subkey may or may not be present, and should be created if it is not present

                string bit64_32_ValueName = "iexplore.exe";

                RegistryKey bit64IE11 = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BFCACHE");

                RegistryKey bit32IE11 = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BFCACHE");

                if (Environment.Is64BitOperatingSystem)
                {

                    if (Registry.GetValue(bit64keyName, bit64_32_ValueName, null) == null)
                    {

                        log.Info("Registry doesn't Exists for IE11 in Win 64 bit" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        // Create 64 bit registry for IE 11 with ValueName = iexplore.exe and Value = 0       

                        bit64IE11.SetValue(bit64_32_ValueName, 0);

                        regFlag = true;

                    }
                    else
                    {
                        regFlag = true;                 // Registry Entry already present
                    }

                }
                else
                {
                    if (Registry.GetValue(bit32keyName, bit64_32_ValueName, null) == null)
                    {
                        log.Info("Registry doesn't Exists for IE11 in Win 32 bit" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                        // Create 64 bit registry for IE 11 with ValueName = iexplore.exe and Value = 0

                        bit32IE11.SetValue(bit64_32_ValueName, 0);

                        regFlag = true;

                    }
                    else
                    {
                        regFlag = true;                 // Registry Entry already present
                    }
                }

                return regFlag;
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                return regFlag;
            }


        }

        // This function performs scroll down on page

        public Boolean scrollDown(IWebDriver driver)
        {

            Boolean scrollStatDown = false;

            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)", "");
                scrollStatDown = true;

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                scrollStatDown = false;
            }

            return scrollStatDown;
        }

        // This function performs scroll up on page
        public Boolean scrollUp(IWebDriver driver)
        {
            Boolean scrollStatUp = false;

            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("window.scrollTo(0, 0)", "");
                scrollStatUp = true;

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                scrollStatUp = false;
            }


            return scrollStatUp;
        }

        // This functions does file delete operation
        public Boolean deleteFile(String path)
        {
            try
            {
                if (System.IO.File.Exists(@"D:\Automation Documentation.doc"))
                {
                    System.IO.File.Delete(@"D:\Atomation Project.docx");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                return false;
            }
        }

        // This function is used to set the implicit wait
        //public Boolean setImplicitWait(IWebDriver driver, int timeOutValue, String durationType)
        //{
        //    if (durationType.ToLower().Equals("millisecond"))
        //    {
        //        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(timeOutValue));
        //        return true;
        //    }
        //    else if (durationType.ToLower().Equals("second"))
        //    {
        //        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeOutValue));
        //        return true;
        //    }
        //    else if (durationType.ToLower().Equals("minute"))
        //    {
        //        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(timeOutValue));
        //        return true;
        //    }
        //    else if (durationType.ToLower().Equals("hour"))
        //    {
        //        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromHours(timeOutValue));
        //        return true;
        //    }
        //    else if (durationType.ToLower().Equals("day"))
        //    {
        //        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromDays(timeOutValue));
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        // This function returns thumb position of scrollbar
        public int getScrollTop(IWebDriver driver)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            int position = Convert.ToInt32(js.ExecuteScript("return document.body.scrollTop"));
            return position;
        }

        // This function returns page width
        public int getWindowWidth(IWebDriver driver)
        {
            long windowWidth = (long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.offsetWidth");
            return (int)windowWidth;
        }

        // This function returns page height
        public int getWindowHeight(IWebDriver driver)
        {
            long windoHeight = (long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.body.parentNode.scrollHeight");
            return (int)windoHeight;
        }

        // This function returns Viewport width
        public int getViewportWidth(IWebDriver driver)
        {
            long viewportWidth = (long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.clientWidth");
            return (int)viewportWidth;
        }

        // This function returns Viewport height
        public int getViewportHeight(IWebDriver driver)
        {
            long viewportHeight = (long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight");
            return (int)viewportHeight;
        }

        // This function returns Scroll height
        public int getScrollHeight(IWebDriver driver)
        {
            long scrollHeight = (long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight");

            return (int)scrollHeight;
        }

        // This function handles alert on site
        public void alertHandling(IWebDriver driver, String popUpType, String action, String valueForPromptpopUp)
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                if (alert != null)
                {
                    if (popUpType.ToLower().Equals("alert"))
                        alert.Accept();
                    else if (popUpType.ToLower().Equals("confirm"))
                    {
                        if (action.ToLower().Equals("accept"))
                            alert.Accept();
                        else if (action.ToLower().Equals("dismiss"))
                            alert.Dismiss();
                    }
                    else if (popUpType.ToLower().Equals("prompt"))
                    {
                        alert.SendKeys(valueForPromptpopUp);
                        if (action.ToLower().Equals("accept"))
                            alert.Accept();
                        else if (action.ToLower().Equals("dismiss"))
                            alert.Dismiss();
                    }
                    else
                    {
                        log.Info("Pop up is not identified" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
            }
        }

        // This function takes the screenshot
        public Boolean TakeScreenshot(IWebDriver driver, String browserType, String testName, Boolean headerPresent, IList<IWebElement> headerElements)
        {
            Boolean flag = false;

            try
            {
                if (browserType.ToLower().Equals("firefox"))
                {
                    #region taking screenshot for firefox browser
                    Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                  //  ss.SaveAsFile(evidencePath + "\\Firefox\\ScreenShots\\" + testName + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    flag = true;
                    #endregion
                }
                else if (browserType.ToLower().Equals("other"))
                {
                    #region taking screen shot for other type of browser
                    Bitmap stitchedImage = null;
                    Screenshot screenshot = null;
                    int totalWidth = getWindowWidth(driver);
                    int totalHeight = getWindowHeight(driver);

                    // Get the Size of the Viewport

                    int viewportWidth = getViewportWidth(driver);
                    int viewportHeight = getViewportHeight(driver);

                    // Split the Screen in multiple Rectangles
                    List<Rectangle> rectangles = new List<Rectangle>();

                    // Loop until the Total Height is reached
                    for (int i = 0; i < totalHeight; i += viewportHeight)
                    {
                        int newHeight = viewportHeight;
                        // Fix if the Height of the Element is too big
                        if (i + viewportHeight > totalHeight)
                        {
                            newHeight = totalHeight - i;     //absolute function
                        }

                        // Loop until the Total Width is reached
                        for (int ii = 0; ii < totalWidth; ii += viewportWidth)
                        {
                            int newWidth = viewportWidth;
                            // Fix if the Width of the Element is too big
                            if (ii + viewportWidth > totalWidth)
                            {
                                newWidth = totalWidth - ii;
                            }

                            // Create and add the Rectangle
                            Rectangle currRect = new Rectangle(ii, i, newWidth, newHeight);  //Rectangle(x,y,width,height)
                            rectangles.Add(currRect);
                        }
                    }

                    // Build the Image
                    stitchedImage = new Bitmap(totalWidth, totalHeight);
                    // Get all Screenshots and stitch them together
                    Rectangle previous = Rectangle.Empty;

                    foreach (var rectangle in rectangles)
                    {
                        // Calculate the Scrolling (if needed)
                        if (previous != Rectangle.Empty)
                        {
                            // Check if static header is present. if yes, hide header elements

                            if (headerPresent)
                            {
                                foreach (var p in headerElements)
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.visibility='hidden'", p);
                                }
                            }

                            int xDiff = rectangle.Right - previous.Right;   //rectangle.right returns The x-coordinate of the right side of the rectangle.
                            int yDiff = rectangle.Bottom - previous.Bottom;  //he y-coordinate of the bottom of the rectangle.

                            log.Info("X Diff :: " + xDiff + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            log.Info("Y Diff :: " + yDiff + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                            // Scroll
                            //selenium.RunScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                            ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));

                            System.Threading.Thread.Sleep(500);
                        }

                        // Take Screenshot
                        screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                        if (!(rectangles.Count > 1))
                        {
                           // screenshot.SaveAsFile(evidencePath + "\\Chrome\\ScreenShots\\" + testName + "_" + DateTime.Now.ToString("hh_mm_ss") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        }

                        // Build an Image out of the Screenshot
                        Image screenshotImage;
                        using (MemoryStream memStream = new MemoryStream(screenshot.AsByteArray))
                        {
                            screenshotImage = Image.FromStream(memStream);
                        }

                        // Calculate the Source Rectangle
                        Rectangle sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);

                        // Copy the Image
                        using (Graphics g = Graphics.FromImage(stitchedImage))
                        {
                            g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                        }
                        previous = rectangle;
                    }

                    if (rectangles.Count > 1)
                    {
                        stitchedImage.Save(evidencePath + "\\Chrome\\ScreenShots\\" + testName + "_" + DateTime.Now.ToString("hh_mm_ss") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }

                    flag = true;
                    #endregion
                }

            }
            catch (Exception e)
            {
                log.Error(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                return false;
            }
            return flag;
        }

        public void phantomScreenShot(IWebDriver driver, String browserType, String testName)
        {
            var phantomJS = new PhantomJS();
            string driverURL = driver.Url.ToString();
            string evidencePath = "C:/Project/IETTVWebPortal/Evidence/" + DateTime.Now.ToString("dd-MMM-yyyy");
            String path = evidencePath + "/Chrome/ScreenShots/" + testName + "_" + DateTime.Now.ToString("hh_mm_ss") + ".png";
            String adminUserName = cf.readingXMLFile("AdminPortal", "Login", "userName", "Config.xml");
            String adminPassword = cf.readingXMLFile("AdminPortal", "Login", "passWord", "Config.xml");

            phantomJS.RunScript("var fs = require('fs');fs.changeWorkingDirectory('" + path + "');var page= require('webpage').create();page.settings.userName = '" + adminUserName + "';page.settings.password = '" + adminPassword + "';page.viewportSize = { width: 1920, height: 1080 };page.open('" + driverURL + "', function (status) {if (status !== 'success') {console.log('Unable to load url');phantom.exit();} else {forcedRenderTimeout = setTimeout(function () {page.render('" + path + "'); phantom.exit(); }, 10000); }});", null);


        }

        // This function set the fluentwait
        public DefaultWait<IWebDriver> fluentTimeout(IWebDriver driver, String durationType, int timeoutValue, int pollingTimeValue)
        {
            // Waiting 30 seconds for an element to be present on the page, checking
            // for its presence once every 5 seconds.

            if (durationType.ToLower().Equals("millisecond"))
            {
                DefaultWait<IWebDriver> wait = setMillisecondFluentTimeOut(driver, timeoutValue, pollingTimeValue);
                return wait;
            }
            else if (durationType.ToLower().Equals("second"))
            {
                DefaultWait<IWebDriver> wait = setSecondFluentTimeOut(driver, timeoutValue, pollingTimeValue);
                return wait;
            }
            else if (durationType.ToLower().Equals("minute"))
            {
                DefaultWait<IWebDriver> wait = setMinuteFluentTimeOut(driver, timeoutValue, pollingTimeValue);
                return wait;
            }
            else if (durationType.ToLower().Equals("hour"))
            {
                DefaultWait<IWebDriver> wait = setHourFluentTimeOut(driver, timeoutValue, pollingTimeValue);
                return wait;
            }
            else if (durationType.ToLower().Equals("day"))
            {
                DefaultWait<IWebDriver> wait = setDayrFluentTimeOut(driver, timeoutValue, pollingTimeValue);
                return wait;
            }
            else
            {
                return null;
            }

        }

        public DefaultWait<IWebDriver> setMillisecondFluentTimeOut(IWebDriver driver, int timeoutValue, int pollingTimeValue)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(driver);
            wait.Timeout = TimeSpan.FromMilliseconds(timeoutValue);
            wait.PollingInterval = TimeSpan.FromMilliseconds(pollingTimeValue);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }

        public DefaultWait<IWebDriver> setSecondFluentTimeOut(IWebDriver driver, int timeoutValue, int pollingTimeValue)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(driver);
            wait.Timeout = TimeSpan.FromSeconds(timeoutValue);
            wait.PollingInterval = TimeSpan.FromMilliseconds(pollingTimeValue);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }

        public DefaultWait<IWebDriver> setMinuteFluentTimeOut(IWebDriver driver, int timeoutValue, int pollingTimeValue)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(driver);
            wait.Timeout = TimeSpan.FromMinutes(timeoutValue);
            wait.PollingInterval = TimeSpan.FromSeconds(pollingTimeValue);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }

        public DefaultWait<IWebDriver> setHourFluentTimeOut(IWebDriver driver, int timeoutValue, int pollingTimeValue)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(driver);
            wait.Timeout = TimeSpan.FromHours(timeoutValue);
            wait.PollingInterval = TimeSpan.FromMinutes(pollingTimeValue);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }

        public DefaultWait<IWebDriver> setDayrFluentTimeOut(IWebDriver driver, int timeoutValue, int pollingTimeValue)
        {
            DefaultWait<IWebDriver> wait = new DefaultWait<IWebDriver>(driver);
            wait.Timeout = TimeSpan.FromDays(timeoutValue);
            wait.PollingInterval = TimeSpan.FromHours(pollingTimeValue);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
            return wait;
        }

        // This function returns the SHA hash value for given file
        public String calculateSHA1(String path)
        {
            string SHACode = null;
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                using (var stream = System.IO.File.OpenRead(path))
                {
                    SHACode = BitConverter.ToString(sha1.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
            return SHACode;
        }

        // This function downloads the provided file
        public void download(String Url, String path, String filename)
        {
            WebClient client = new WebClient();
            client.DownloadFile(Url, path + filename);
        }

        // This function returns the status code of provided URL
        public String getStatusCode(Uri Url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
                webRequest.Timeout = 60000;
                webRequest.AllowAutoRedirect = true;
                HttpWebResponse wResp = (HttpWebResponse)webRequest.GetResponse();
                Thread.Sleep(2000);
                string wRespStatusCode = wResp.StatusCode.ToString();
                log.Info("Status Code:=" + wRespStatusCode + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                wResp.Close();
               // wResp.Dispose();
                return wRespStatusCode;
            }
            catch (WebException we)
            {
                log.Error(we.Message + "\n" + we.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                string wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode.ToString();              // Need to check this line of code
                Thread.Sleep(2000);
                we.Response.Close();
               // we.Response.Dispose();
                return wRespStatusCode;
            }
        }

        // This function disconnects the network
        public void disconnectNetwork()
        {

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "ipconfig";
            info.Arguments = "/release";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;

            // This is to run the command prompt as admin
            if (Environment.OSVersion.Version.Major >= 6)
            {
                info.Verb = "runas";
            }
            Process process = Process.Start(info);
            process.WaitForExit();
        }

        // This function connects to the network
        public void connectNetwork()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "ipconfig";
            info.Arguments = "/renew";
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(info);
            p.WaitForExit();
        }

        // This function uploads the provided file
        public void uploadfile(IWebElement fileInput, String uploadPath)
        {
             String fullPath = Path.GetFullPath(uploadPath);
             log.Info("File path to upload:: " + fullPath + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
             fileInput.SendKeys(fullPath);
             Thread.Sleep(1000);
        }

        public void RemoveStyleAttribute(IWebElement element, IJavaScriptExecutor executor)
        {
            executor.ExecuteScript("arguments[0].setAttribute(arguments[1],arguments[2]);", element, "style", "");
        }

        // This function performs image comparision
        public Boolean imageComparision(Bitmap downloadedImage, Bitmap systemImage)
        {
            Boolean flag = true;

            if (downloadedImage.Width == systemImage.Width && downloadedImage.Height == systemImage.Height)
            {
                for (int row = 0; row < downloadedImage.Width; row++)
                {
                    for (int j = 0; j < downloadedImage.Height; j++)
                    {
                        String img1_ref = downloadedImage.GetPixel(row, j).ToString();
                        String img2_ref = systemImage.GetPixel(row, j).ToString();
                        if (img1_ref != img2_ref)
                        {
                            flag = false;
                            break;
                        }

                    }

                }
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        // This function highlights the element when exception occurs
        public void highlighElement(IWebDriver driver, String functionNameWhereExceptionOccur)
        {
            // Need to write code to read Xpath from XML file
            String xpath = "//*[@id='tsf']/div[2]/div[3]/cnter";

            Boolean flag = true;

            ReadOnlyCollection<IWebElement> element1 = null;

            // Set the lower limit of Xpath
            int indexOfLastSlash = -1;

            while (flag)
            {
                element1 = (ReadOnlyCollection<IWebElement>)driver.FindElements(By.XPath(xpath));

                // Check if it is a valid element for given xpath
                if (element1.Count != 0)
                {
                    IWebElement ele = driver.FindElement(By.XPath(xpath));

                    IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;

                    // Highlight the element with red border
                    executor.ExecuteScript("arguments[0].style.border='1px solid  red'", ele);

                    // Set the flag false tpo break the while loop
                    flag = false;
                }
                else
                {
                    indexOfLastSlash = xpath.LastIndexOf('/');

                    if (indexOfLastSlash > 1)
                    {
                        // Set the new xpath till the last slash index
                        xpath = xpath.Substring(0, indexOfLastSlash);
                        log.Info("Xpath After change ::: " + xpath + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                    }
                    else
                    {
                        log.Info("Element is not tracable by XPath" + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                        flag = false;
                    }
                }
            } // close of while

            TakeScreenshot(driver, "firefox", functionNameWhereExceptionOccur, false, null);
        }


        //This function will wait for the jQuery function execution
        public Boolean isJqueryActive(IWebDriver driver)
        {
            Boolean ajaxIsComplete = false;
            for (int i = 1; i <= 60; i++)
            {
                ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                log.Info("ajaxIsComplete :: " + ajaxIsComplete + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                if (ajaxIsComplete)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            return ajaxIsComplete;

        }

        //This function will wait for the javascript function execution
        public Boolean isJavaScriptActive(IWebDriver driver)
        {
            Boolean javaScriptIsComplete = false;
            for (int i = 1; i <= 60; i++)
            {
                String state = ((IJavaScriptExecutor)driver).ExecuteScript(@"return document.readyState").ToString();
                javaScriptIsComplete = state.Equals("complete", StringComparison.InvariantCultureIgnoreCase) || state.Equals("loaded", StringComparison.InvariantCultureIgnoreCase);

                log.Info("Java Script Complete :: " + javaScriptIsComplete + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());
                if (javaScriptIsComplete)
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            return javaScriptIsComplete;

        }


        public Boolean IsPageLoaded(IWebDriver driver)
        {
            Boolean pageLoad = false;

            for (int i = 0; i < 60; i++)
            {
                String loading = "return document.readyState";

                IJavaScriptExecutor execute = (IJavaScriptExecutor)driver;
                String loadingState = execute.ExecuteScript(loading).ToString();
                Console.WriteLine("Waiting for page load ....");
                if (loadingState.Equals("complete"))
                {
                    pageLoad = true;
                    break;
                }
                Thread.Sleep(1000);
            }
            return pageLoad;



        }

        // This function generates new Guid
        public string getGuid()
        {
            Guid g;

            // Create and display the value of  GUID.
            g = Guid.NewGuid();

            string output = g.ToString().Replace("-", "");

            log.Info("GUID :: " + output + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            return output;
        }

        public string getShortGuid()
        {
            Guid g;

            // Create and display the value of  GUID.
            g = Guid.NewGuid();

            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            string output = string.Format("{0:x}", i - DateTime.Now.Ticks).Substring(10);

            return output;
            //return string.Format("{0:x}", i - DateTime.Now.Ticks);

            //string output = g.ToString().Replace("-", "");

            //log.Info("GUID :: " + output + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

            //return output;
        }


        public void OpenNewTab(IWebDriver driver)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("window.open();");

            // driver.SwitchTo().DefaultContent();
            //  driver.FindElement(By.TagName("body")).SendKeys(OpenQA.Selenium.Keys.Control + 't');

        }

        public void ShiftToPreviousTab(IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("body")).SendKeys(OpenQA.Selenium.Keys.Control + OpenQA.Selenium.Keys.Tab);
        }

        public void SwitchToAdminTab(IWebDriver driver, string browserType)
        {
            driver.FindElement(By.CssSelector("body")).SendKeys(OpenQA.Selenium.Keys.Control + OpenQA.Selenium.Keys.NumberPad1);

            if (browserType.Equals("Firefox"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.First());
            }
            else if (browserType.Equals("Chrome"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.First());
            }
            else if (browserType.Equals("IE"))
            {

            }
            else if (browserType.Equals("PhantomJS"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.First());
            }

        }

        public void SwitchToExternalPortal(IWebDriver driver, string browserType)
        {
            driver.FindElement(By.CssSelector("body")).SendKeys(OpenQA.Selenium.Keys.Control + OpenQA.Selenium.Keys.NumberPad1);

            if (browserType.Equals("Firefox"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            else if (browserType.Equals("Chrome"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            else if (browserType.Equals("IE"))
            {

            }
        }

        public void SwitchToWebTab(IWebDriver driver, string browserType)
        {

            IWait<IWebDriver> iWait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            iWait.Until(d => driver.FindElement(By.CssSelector("body")));

            driver.FindElement(By.CssSelector("body")).SendKeys(OpenQA.Selenium.Keys.Control + OpenQA.Selenium.Keys.NumberPad2);

            if (browserType.Equals("Firefox"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            else if (browserType.Equals("Chrome"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            else if (browserType.Equals("IE10"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            else if (browserType.Equals("PhantomJS"))
            {
                driver.SwitchTo().Window(driver.WindowHandles.Last());
            }
            Thread.Sleep(2000);
        }

        public void NavigateWebPortal(Configuration cf, IWebDriver driver)
        {
            String appURL = cf.readingXMLFile("WebPortal", "Login", "startURL", "Config.xml");

            driver.Navigate().GoToUrl(appURL);

            driver.Manage().Window.Maximize();
        }

        public void NavigateYopMail(Configuration cf, IWebDriver driver)
        {
            String appURL = cf.readingXMLFile("WebPortal", "Yopmail", "startURL", "Config.xml");

            driver.Navigate().GoToUrl(appURL);

            driver.Manage().Window.Maximize();
        }

        public void NavigateExternalPortal(Configuration cf, IWebDriver driver)
        {
            String appURL = cf.readingXMLFile("ExternalPortal", "ExternalPortalLink", "startURL", "Config.xml");

            driver.Navigate().GoToUrl(appURL);

            driver.Manage().Window.Maximize();
        }

        //This funtion will create the Evidence folder for Web Portal for today's date if it is not created, which will store all the videos of the test cases executed today.
        public void CreateOrReplaceVideoFolder()
        {
            try
            {
                List<String> lstEvedincePath = cf.readSysConfigFile("WebPortal", "Evidence", "SysConfig.xml");

                string keepEvidence = lstEvedincePath.ElementAt(0).ToString();

                bool exists = System.IO.Directory.Exists(evidencePath);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(evidencePath);
                    System.IO.Directory.CreateDirectory(chromeVideosEvidencePath);
                    System.IO.Directory.CreateDirectory(firefoxVideosEvidencePath);
                    System.IO.Directory.CreateDirectory(iEVideosEvidencePath);

                    System.IO.Directory.CreateDirectory(chromeScreenShotsEvidencePath);
                    System.IO.Directory.CreateDirectory(firefoxScreenShotsEvidencePath);
                    System.IO.Directory.CreateDirectory(iEScreenShotsEvidencePath);

                }
                if (keepEvidence == "No")
                {
                    string fPath = "C:\\Project\\IETTVWebPortal\\Evidence";
                    emptyEvidence(fPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                Assert.AreEqual(true, false);
            }

        }

        //This function will capture the Video for all the tests who has implemented this function
        //public void ScreenCap(ScreenCaptureJob job, string testName, IWebDriver driver)
        //{
        //    screenHeight = getScreenHeight(driver);

        //    screenWidth = getScreenWidth(driver);

        //    // You can capture a window by setting its coordinates here
        //    job.CaptureRectangle = new Rectangle(0, 0, 1364, 768);

        //    // Include the mouse pointer in the captured video
        //    job.CaptureMouseCursor = true;

        //    string[] testDriverName = testName.Split('.');

        //    if (testDriverName[1].ToString() == "Chrome")
        //        job.OutputScreenCaptureFileName = evidencePath + "\\Chrome\\Videos\\" + testName + "_" + DateTime.Now.ToString("hh_mm_ss") + ".wmv";
        //    else if (testDriverName[1].ToString() == "Firefox")
        //        job.OutputScreenCaptureFileName = evidencePath + "\\Firefox\\Videos\\" + testName + "_" + DateTime.Now.ToString("hh_mm_ss") + ".wmv";
        //    else
        //        job.OutputScreenCaptureFileName = evidencePath + "\\IE\\Videos\\" + testName + "_" + DateTime.Now.ToString("hh_mm_ss") + ".wmv";


        //    // Do some capture
        //    job.Start();
        //}

        //This function will delete all the videos from evidence folder (If the Evidence key is set to NO in sysxml config file) except today's date folder
        private void emptyEvidence(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                if (dir.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") && dir.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Chrome" && dir.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Firefox" && dir.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\IE")
                    fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                emptyEvidence(di.FullName);
                if (di.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") && di.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Chrome" && di.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\Firefox" && di.FullName != "C:\\Project\\IETTVWebPortal\\Evidence\\" + DateTime.Now.ToString("dd-MMM-yyyy") + "\\IE")
                    di.Delete();
            }
        }

        //This function return month format "MMMM" to "mmm" format(Eg:January into Jan)
        public String convertIntommm(String date)
        {

            switch (date)
            {
                case "January":
                    return "Jan";
                case "February":
                    return "Feb";
                case "March":
                    return "Mar";
                case "April":
                    return "Apr";
                case "May":
                    return "May";
                case "June":
                    return "Jun";
                case "July":
                    return "Jul";
                case "August":
                    return "Aug";
                case "September":
                    return "Sep";
                case "October":
                    return "Oct";
                case "November":
                    return "Nov";
                case "December":
                    return "Dec";

                default:
                    return null;
            }
        }

        //This function return month format int to "MMM" format(Eg:1 into January)
        public String convertIntoMMM(int date)
        {

            switch (date)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";

                default:
                    return null;
            }
        }

        public Boolean sikuliPortCheck()
        {
            bool portReady = true;

            HttpListener httpListner = new HttpListener();

            try
            {
                // httpListner.Prefixes.Add("http://localhost:8081/");

                httpListner.Start();

                int portCount = 0;

                while (PortInUse(8080))
                {
                    Console.WriteLine("8080 is Listening");

                    log.Info("Port 8080 is listening");

                    portCount++;

                    Thread.Sleep(1000);

                    if (portCount > 60)
                    {
                        portReady = false;

                        break;
                    }

                    portReady = false;
                }

                Console.WriteLine("Is 8080 Ready? " + portReady);

                log.Info("Is 8080 Ready? " + portReady);

                httpListner.Close();

            }
            catch (Exception e)
            {
                log.Info("Error while checking sikuli port at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                portReady = false;

                httpListner.Close();
            }

            return portReady;
        }

        public static bool PortInUse(int port)
        {
            bool inUse = false;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

        public Boolean jettyServiceUpCheck()
        {
            String sikuliConnectText = null;

            int sikuliTimeout = 0;

            bool jettyUp = false;

            while (sikuliConnectText != "Welcome to the Sikuli API!" && sikuliTimeout < 60)
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://localhost:8080/sikuli/api/wait");

                    webRequest.Timeout = 60000;

                    webRequest.AllowAutoRedirect = true;

                    HttpWebResponse wResp = (HttpWebResponse)webRequest.GetResponse();

                    string wRespStatusCode = wResp.StatusCode.ToString();

                    Stream stream = wResp.GetResponseStream();

                    StreamReader sr = new StreamReader(stream);

                    sikuliConnectText = sr.ReadToEnd().ToString();

                    Console.WriteLine("Status Code:=" + wRespStatusCode + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    wResp.Close();

                  //  wResp.Dispose();
                }

                catch (WebException we)
                {
                    Console.WriteLine(we.Message + "\n" + we.StackTrace + " at line:" + new StackTrace(true).GetFrame(0).GetFileLineNumber());

                    jettyUp = false;

                    Thread.Sleep(1500);
                }

                Console.WriteLine("Connect Text:" + sikuliConnectText + "TimeOut" + sikuliTimeout);

                log.Info("Connect Text:" + sikuliConnectText + "TimeOut" + sikuliTimeout);

                sikuliTimeout++;

                if (sikuliTimeout > 60)
                {
                    jettyUp = false;

                    break;
                }

                if (sikuliConnectText == "Welcome to the Sikuli API!")
                {
                    jettyUp = true;

                    break;
                }

            }

            return jettyUp;
        }

        //public Boolean IsElementPresent(IWebDriver driver, By element, int timeoutValue)
        //{
        //    try
        //    {
        //        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(timeoutValue));
        //        driver.FindElement(element);
        //        return true;


        //    }
        //    catch (Exception)
        //    {

        //        return false;
        //    }
        //}
   
    }
}

public class EventFire : EventFiringWebDriver
{
    // This is to configure logger mechanism for Utilities.Config
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    string preFindMethod = string.Empty;
    //event handler::::

    public EventFire(IWebDriver driver)
        : base(driver)
    {
        this.Navigating += new EventHandler<WebDriverNavigationEventArgs>(this.BeforeNavigating);
        this.Navigated += new EventHandler<WebDriverNavigationEventArgs>(this.AfterNavigate);
        this.ElementClicking += new EventHandler<WebElementEventArgs>(Click);
        this.FindingElement += new EventHandler<FindElementEventArgs>(FindElementOperation);

    }

    void BeforeNavigating(object sender, WebDriverNavigationEventArgs e)
    {
        log.Info("Before Navigating to ::::::::::::::::::::::::: " + e.Driver.Url);
    }

    public void AfterNavigate(object sender, WebDriverNavigationEventArgs e)
    {
        log.Info("Navigated to ::::::::::::::::::::::::: " + e.Driver.Url);

    }

    public void FindElementOperation(object sender, FindElementEventArgs e)
    {
        string currentFindMethod = e.FindMethod.ToString();

        if (!preFindMethod.Equals(currentFindMethod))
            log.Info("Interacting with element  :::::::::::::::::::: " + e.FindMethod);

        preFindMethod = currentFindMethod;

    }

    public void Click(object sender, WebElementEventArgs e)
    {
        log.Info("Interacting with element with Text :::::::::::::::::::: " + e.Element.Text);

    }

}


















