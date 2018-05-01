using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whatsboot
{
    class User
    {
        public string id;
        public string username;
        public string phone;
        public string chatid;
        public string messengertype;

        public User(string id, string username, string phone, string chatid, string messengertype)
        {
            this.id = id;
            this.username = username;
            this.phone = phone;
            this.chatid = chatid;
            this.messengertype = messengertype;
        }

    }
}
