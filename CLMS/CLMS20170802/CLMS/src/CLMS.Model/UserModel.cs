using CLMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLMS.Model
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int State { get; set; }
        public string Email { get; set; }
        public string Telphone { get; set; }
        public string RealName { get; set; }
        public string Password { get; set; }
        public UserType RoleId { get; set; }
        public string RoleName { get; set; }
        public int OrganId { get; set; }
        public string OrganName { get; set; }
    }
}
