using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.DataModels
{
    //사용자소유 권한
    public class UserRolesByUser
    {
        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string UserId { get; set; }

        [Key, StringLength(50), Column(TypeName = "varchar(50)")]
        public string RoleId { get; set; }

        [Required]
        public DateTime OwnedUtcDate { get; set; }

        public virtual User User { get; set; }

        //1:1 관계 (7. code first)
        public virtual UserRole UserRole { get; set; } 
    }
}
