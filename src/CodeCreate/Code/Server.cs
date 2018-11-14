
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCreate.Code
{
    public class Server
    {
        private string serverName;

        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        private string dbName;

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                return "server=" + serverName + ";database=" + dbName + ";uid=" + userName + ";pwd=" + password;
            }
            else
            {
                return "server=" + serverName + ";database=" + dbName + ";Integrated Security=true";
            }
        }
    }
}
