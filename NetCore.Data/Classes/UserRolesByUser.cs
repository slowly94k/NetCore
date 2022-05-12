using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.Classes
{
    public class UserRolesByUser
    {
        [Key]
        public string UserId { get; set; }
        [Key]
        public string RoleId { get; set; }
        public System.DateTime OwnedUtcDate { get; set; }

        public virtual User User { get; set; }
        public virtual UserRole UserRole { get; set; }
    }
}
