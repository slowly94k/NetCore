using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.DataModels
{
    //1. 데이터 어노테이션 (7.code-first방식)
    public class User
    {
        [Key, StringLength(50), Column(TypeName ="varchar(50)")]
        public string UserId { get; set; }

        //UserName은 필수 값이기 대문에 Required
        [Required, StringLength(100), Column(TypeName ="nvarchar(100)")]
        public string UserName { get; set; }

        [Required, StringLength(320), Column(TypeName = "varchar(320)")]
        public string UserEmail { get; set; }

        [Required, StringLength(130), Column(TypeName = "nvarchar(130)")]
        public string Password { get; set; }

        [Required]
        public bool IsMembershipWithdrawn { get; set; }

        [Required]
        public DateTime JoinedUtcDate { get; set; }

        //FK 지정
        //User.cs와 UserRolesByUser의 UserId가 연결되어서 지정 (7. code first)
        [ForeignKey("UserId")]
        public virtual ICollection<UserRolesByUser> UserRolesByUsers { get; set; }
    }
}
