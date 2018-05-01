using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WhatsAppApi;
using WhatsAppApi.Account;
using WhatsAppSeleniumAPI;

namespace whatsboot
{
    class Program
    {

        static void Main(string[] args)
        {
            Program x = new Program();
            x.MainS(null);

        }

        IWhatsAppDriver _driver;
        void MainS(string[] args)
        {
            Start(new WhatsAppSeleniumAPI.ChromeWApp());
        }

        void Start(IWhatsAppDriver driver)
        {

            Commands.Initialize();

            _driver = driver;
            driver.StartDriver();
            //Wait till we are on the login page
            while (!driver.OnLoginPage())
            {
                Console.WriteLine("Not on login page");
                Thread.Sleep(1000);
            }

            Thread.Sleep(500);

            while (driver.OnLoginPage())
            {
                Console.WriteLine("Please login");
                Thread.Sleep(5000);
            }
            Console.WriteLine("You have logged in");
            driver.OnMsgRecieved += OnMsgRec;
            Task.Run(() => driver.MessageScanner());

            Console.WriteLine("Use CTRL+C to exit");

            while (true)
            {
                if (!driver.IsPhoneConnected())
                {
                    Console.WriteLine("Phone is not connected");
                }
                Thread.Sleep(10000);
            }
        }

        private void OnMsgRec(IWhatsAppDriver.MsgArgs arg)
        {
            Console.WriteLine(arg.Sender + " Wrote: " + arg.Msg + " at " + arg.TimeStamp);

            //try
            //{
                _driver.SendMessage(Commands.ProcessMessage(arg.Msg, new User("testID", arg.Sender, "888888888", "TestChatID", "1")), arg.Sender);
                return;
            //}
            //catch (Exception)
            //{
            //    return;
            //}

        }

    }
}
