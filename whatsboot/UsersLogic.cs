using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whatsboot
{
    class UsersLogic
    {
        List<User> Users;

        string filename = "Users.txt";

        public UsersLogic()
        {
            GetUsers();
        }

        public void SaveUsers()
        {
            FileStream fs = File.Open(filename, FileMode.Create);
            using (TextWriter tw = new StreamWriter(fs))
            {
                foreach (User u in Users)
                {
                    tw.WriteLine(u.id + ";" + u.username + ";" + u.phone + ";" + u.chatid + ";" + u.messengertype);
                }
            }
        }

        public void GetUsers()
        {
            string line;
            Users = new List<User>();
            try
            {
                StreamReader file = new StreamReader(filename);
                while ((line = file.ReadLine()) != null)
                {
                    string[] words = line.Split(';');
                    Users.Add(new User(words[0], words[1], words[2], words[3], words[4]));
                }

                file.Close();
            }
            catch (FileNotFoundException ex)
            {
                File.Create(filename);
            }
        }

        public bool CheckSubscribed(User user)
        {
            return Users.Exists(x => x.phone == user.phone && x.messengertype == user.messengertype); 
        }

        public void Subscribe (User user)
        {
            Users.Add(user);
            SaveUsers();
        }

        public void Unsubscribe (User user)
        {
            Users.Remove(Users.Find(x => x.phone == user.phone && x.messengertype == user.messengertype));
            SaveUsers();
        }

    }
}
