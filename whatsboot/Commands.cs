using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace whatsboot
{
    class Commands
    {
        private static string ConfigFilename = "Config.Json";
        private static Config config;
        private static UsersLogic Logic;

        public static void Initialize()
        {
            string configJson = File.ReadAllText(ConfigFilename);
            config = JsonConvert.DeserializeObject<Config>(configJson);
            Logic = new whatsboot.UsersLogic();
        }

        public static string ProcessMessage(string message, User user)
        {
            switch (message.ToLower())
            {
                case "/start":
                    if (Logic.CheckSubscribed(user))
                    {
                        return config.AlreadySubscribedMessage;
                    }
                    else
                    {
                        Logic.Subscribe(user);
                        return config.UserSubscribed;
                    }
                    break;
                    break;
                case "/bye":
                    Logic.Unsubscribe(user);
                    return config.UserUnsubscribed;
                    break;
                case "":
                    return config.UnsupportedMessageType;
                    break;
                default:
                    if (Logic.CheckSubscribed(user))
                    {
                        return config.AutoresponseText;
                    }
                    else
                    {
                        Logic.Subscribe(user);
                        return config.UserSubscribed;
                    }
                    break;
            }
            
        }

    }
}
