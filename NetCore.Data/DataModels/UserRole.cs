using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.DataModels
{
    public class UserRole
    {
        [Key, StringLength(50), Column(TypeName ="varchar(50)")]
        public string RoleId { get; set; }

        [Required, StringLength(100), Column(TypeName = "nvarchar(100)")]
        public string RoleName { get; set; }

        [Required]
        public byte RolePriority { get; set; }

        [Required]
        public DateTime ModifiedUtcDate { get; set; }

        //User.cs 와 UserRole.cs의 RoleId가 연결되어서 지정 (7. code first)
        [ForeignKey("RoleId")]
        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
