using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium.Remote;
using System.Runtime.InteropServices;

namespace StackoverflowGetFanaticBadge
{
    public class BrowserEngine : IDisposable
    {
        private IWebDriver _chrome;
        private bool _isWin;
        private string _chromeUrl;

        public BrowserEngine(string chromeUrl)
        {
            _isWin = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            _chromeUrl = chromeUrl;

            // just give some time to upload chrome container
            if(!_isWin)
                Thread.Sleep(10000);

            Init();
        }

        private void Init()
        {
            if (_chrome != null)
            {
                this.Dispose();
            }

            var options = new ChromeOptions();
            options.AddArguments("--headless"); // hide UI
            
            if(_isWin) 
            {
                // WINDOWS
                options.AddArguments("--disable-gpu");
                _chrome = new ChromeDriver(options);
            }
            else
            {
                // DOCKER LINUX
                options.AddArgument("no-sandbox");
                _chrome = new RemoteWebDriver(new Uri($"{_chromeUrl}/wd/hub"), options);
            }
        }

        /// <summary>
        /// Open "https://stackoverflow.com/"
        /// click 'Log in' button
        /// input your login and password
        /// click 'Submit' button
        /// make sure that login was successful
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="url"></param>
        public bool MakeLogin(string user, string pass, string url)
        {
            var navigate = _chrome.Navigate();
            navigate.GoToUrl(url);
            Thread.Sleep(1000);

            var link = _chrome.FindElements(By.PartialLinkText("Log in")).First();
            link.Click();
            Thread.Sleep(1000);

            var emailInput = _chrome.FindElements(By.Id("email")).First();
            emailInput.SendKeys(user);
            Thread.Sleep(500);

            
            var passInput = _chrome.FindElements(By.Id("password")).First();
            passInput.SendKeys(pass);
            Thread.Sleep(500);

            var submitButton = _chrome.FindElements(By.Id("submit-button")).First();
            submitButton.Click();
            Thread.Sleep(2000);

            return _chrome.FindElement(By.TagName("body")).Text.Contains("Top Questions");
        }

        /// <summary>
        /// ChromeDriver should be always close manually for avoiding memory leak
        /// </summary>
        public void Dispose()
        {
            _chrome.Close();
            _chrome.Dispose();
            _chrome = null;
        }
    }
}
