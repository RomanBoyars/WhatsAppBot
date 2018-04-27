using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WhatsAppSeleniumAPI
{
    public abstract class IWhatsAppDriver
    {

        protected IWebDriver driver;
        IReadOnlyCollection<IWebElement> messages = null;
        public bool HasStarted { get; protected set; }

        private const string UNREAD_MESSAGES_XPATH = "//div[@class='_2EXPL CxUIE']";
        private const string TITLE_XPATH = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[3]/div[1]/div[1]/*/div/div/div[@class=\"_2EXPL CxUIE\"]/div/div/div[@class=\"_25Ooe\"]";
        private const string UNREAD_MESSAGE_COUNT_XPATH = "div/div/div/span/div/span[@class=\"OUeyt\"]";
        private const string QR_CODE_XPATH = "//img[@alt='Scan me!']";
        private const string MAIN_APP_CLASS = "app";
        private const string ALERT_PHONE_NOT_CONNECTED_CLASS = "icon-alert-phone";
        private const string NAME_TAG_XPATH = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[3]/div[1]/header[1]/div[2]/div[1]/div[1]/span[1]";
        private const string INCOME_MESSAGES_XPATH = "//div[contains(@class,'message-in')]//div[@class='Tkt2p']";
        private const string SELECTABLE_MESSAGE_TEXT_CLASS = "selectable-text";
        private const string READ_MESSAGES_XPATH = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[3]/div[1]/div[1]/*/div/div/div[@class=\"_2EXPL\"]";
        private const string CHAT_INPUT_TEXT_XPATH = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[3]/div[1]/footer[1]/div[1]/div[2]/div[1]/div[2]";
        private const string ALL_CHATS_TITLE_XPATH = "/html[1]/body[1]/div[1]/div[1]/div[1]/div[2]/div[1]/div[3]/div[1]/div[1]/*/div/div/div/div/div/div[@class=\"_25Ooe\"]";

        public IWebDriver WebDriver
        {
            get
            {
                if (driver != null)
                {
                    return driver;
                }
                throw new NullReferenceException("Call StartDriver() before using WebDriver!");
            }
        }

        private EventFiringWebDriver _eventDriver;

        public EventFiringWebDriver EventDriver
        {
            get
            {
                if (_eventDriver != null)
                {
                    return _eventDriver;
                }
                throw new NullReferenceException("Call StartDriver() before using WebDriver!");
            }
        }

        public class MsgArgs : EventArgs
        {
            public MsgArgs(string message, string sender)
            {
                TimeStamp = DateTime.Now;
                Msg = message;
                Sender = sender;
            }

            public string Msg { get; }

            public string Sender { get; }

            public DateTime TimeStamp { get; }
        }

        public delegate void MsgRecievedEventHandler(MsgArgs e);

        public event MsgRecievedEventHandler OnMsgRecieved;

        protected void Raise_RecievedMessage(string Msg, string Sender)
        {
            OnMsgRecieved?.Invoke(new MsgArgs(Msg, Sender));
        }

        public bool OnLoginPage()
        {
            try
            {
                if (driver.FindElement(By.XPath(QR_CODE_XPATH)) != null)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public bool IsPhoneConnected()
        {
            try
            {
                if (driver.FindElement(By.ClassName(ALERT_PHONE_NOT_CONNECTED_CLASS)) != null)
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
            return true;
        }

        public virtual void StartDriver()
        {
            //can't start a driver twice
            HasStartedCheck();
            HasStarted = true;
        }

        public async void MessageScanner()
        {
            while (true)
            {
                IReadOnlyCollection<IWebElement> unread = driver.FindElements(By.XPath(UNREAD_MESSAGES_XPATH));

                foreach (IWebElement x in unread.ToArray())
                {
                    try
                    {
                        x.Click();
                        await Task.Delay(1000); //Let it load
                        var Pname = "";
                        var message_text = GetLastestText(out Pname);
                        Raise_RecievedMessage(message_text, Pname);
                        Console.WriteLine("Отпарвил");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\n" + ex.Source);
                    }
                }

                //CurrentDialogScanning
                if (messages !=null && unread.Count == 0 && messages.Count != GetIncomingMessages().Count() )
                {
                    try
                    {
                        await Task.Delay(1000); //Let it load
                        var Pname = "";
                        var message_text = GetLastestText(out Pname);
                        Raise_RecievedMessage(message_text, Pname);
                        Console.WriteLine("Отпарвил");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\n" + ex.Source);
                    }
                }

                await Task.Delay(10); //don't allow too much overhead
            }
        }


        public string GetLastestText(out string Pname) 
        {
            var nametag = driver.FindElement(By.XPath(NAME_TAG_XPATH));
            Pname = nametag.GetAttribute("title");
            messages = GetIncomingMessages();
            var newmessage = messages.OrderBy(x => x.Location.Y).Reverse().First(); //Get latest message
            var message_text_raw = newmessage.FindElement(By.ClassName(SELECTABLE_MESSAGE_TEXT_CLASS));
            return Regex.Replace(message_text_raw.Text, "<!--(.*?)-->", "");
        }

        public IReadOnlyCollection<IWebElement> GetIncomingMessages()
        {
            IReadOnlyCollection<IWebElement> result = null;
            try
            {
                result = driver.FindElements(By.XPath(INCOME_MESSAGES_XPATH));
            }
            catch (Exception)
            {
                return result;
            }
            return result;
        }

        public void SendMessage(string message, string person = null)
        {
            if (person != null)
            {
                SetActivePerson(person);
            }
            var outp = message.ToWhatsappText();
            var chatbox = driver.FindElement(By.XPath(CHAT_INPUT_TEXT_XPATH));
            chatbox.Click();
            chatbox.SendKeys(message);
            chatbox.SendKeys(Keys.Enter);
        }

        public void SetActivePerson(string person)
        {
            IReadOnlyCollection<IWebElement> AllChats = driver.FindElements(By.XPath(ALL_CHATS_TITLE_XPATH));
            foreach (var title in AllChats)
            {
                if (title.GetAttribute("title") == person)
                {
                    title.Click();
                    Thread.Sleep(300);
                    return;
                }
            }
            Console.WriteLine("Can't find person, not sending");
        }

        public virtual void StartDriver(IWebDriver driver)
        {
            this.driver = driver;
            driver.Navigate().GoToUrl("https://web.whatsapp.com");
            _eventDriver = new EventFiringWebDriver(WebDriver);
        }

        protected void HasStartedCheck(bool Invert = false)
        {
            if (HasStarted ^ Invert)
            {
                throw new NotSupportedException(String.Format("Driver has {0} already started", Invert ? "not" : ""));
            }
        }

    }
}
