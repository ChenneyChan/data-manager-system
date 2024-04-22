using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABBDataManagerSystem.Bean
{
    public class UserInfo
    {
        string name { get; set; } = string.Empty;

        public string Name { get { return name; } set { name = value; } }

        string password { get; set; } = string.Empty;

        public string Password { get { return password; } set { password = value; } }

        int permissions { get; set; } = 0;
        public int Permissions { get { return permissions; } set { permissions = value; } }

        int id { get; set; } = int.MaxValue;
        public int Id { get { return id; } set { id = value; } }

        DateTime lastLoginTime { get; set; }
        public DateTime LastLoginTime { get { return lastLoginTime; } set { lastLoginTime = value; } }

        DateTime createTime { get; set; }
        public DateTime CreateTime { get { return createTime; } set { createTime = value; } }

        public bool IsVisitor { get; set; } = false;

        public UserInfo(string name, string password, int permissions)
        {
            this.name = name;
            this.password = password;
            this.permissions = permissions;
            this.createTime = DateTime.Now;
        }

        public UserInfo(bool isVisitor)
        {
            this.IsVisitor = isVisitor;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("ID: ").Append(Id).Append(", Name: ").Append(name).Append(", Permissions: ").Append(permissions);
            return sb.ToString();
        }

    }
}
