using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppSeleniumAPI
{
    public class ChromeWApp : IWhatsAppDriver
    {
        ChromeOptions ChromeOP;
        /// <summary>
        /// Make a new ChromeWhatsapp Instance
        /// </summary>
        public ChromeWApp()
        {
            ChromeOP = new ChromeOptions() { LeaveBrowserRunning = false };
        }

        /// <summary>
        /// Starts the chrome driver with settings
        /// </summary>
        public override void StartDriver()
        {
            HasStartedCheck();
            var drive = new ChromeDriver(ChromeOP);
            base.StartDriver(drive);
        }
       
    }
}
