using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.Classes
{
    public class User
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        //Password => GUIDSalt변경 16.
        public string GUIDSalt { get; set; }
        //16.
        public string RNGSalt { get; set; }
        //16.
        public string PasswordHash { get; set; }
        //멤버변수 하나 추가 (12. )
        public int AccessFailedCount { get; set; }
        public bool IsMembershipWithdrawn { get; set; }
        public System.DateTime JoinedUtcDate { get; set; }


        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
