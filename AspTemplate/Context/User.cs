using System;
using System.Collections.Generic;

namespace NewTemplate.Context
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int RoleId { get; set; }
    }
}
