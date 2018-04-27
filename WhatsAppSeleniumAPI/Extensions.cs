using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppSeleniumAPI
{
    public static class Extensions
    {
        public static string ToWhatsappText(this string inp)
        {
            return inp.Replace("\n", (OpenQA.Selenium.Keys.Shift + OpenQA.Selenium.Keys.Enter + OpenQA.Selenium.Keys.LeftShift))
                .Replace(':', (char)0xFF1A); 
        }

    }
}
