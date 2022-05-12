using Microsoft.EntityFrameworkCore;
using NetCore.Data.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    //2. Fluent API
    //상속
    //CodeFirstDbContext :자식클래스
    //DbContext : 부모클래스
    public class CodeFirstDbContext :DbContext
    {
        //생성자 상속
        public CodeFirstDbContext(DbContextOptions<CodeFirstDbContext> options) : base(options)
        {

        }

        //DB 테이블 리스트 지정 (7.)
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRolesByUser> UserRolesByUsers { get; set; }


        //메서드 상속,  부모클래스 DbContext에서 OnModelCreating 메서드가 virtual키워드로 지정되어 있어야만 상속 가능
        //OnModelCreating가 DbContext에 있는데, 그것을 상속 받아온다
        //메서드에는 override를 사용
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //4가지 작업
            //DB 테이블 이름 변경
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");

            //복합키 지정
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });

            //컬럼키 지정
            modelBuilder.Entity<User>(e =>
            {
                //HasDefaultValue 회원 탈퇴여부
                e.Property(c => c.IsMembershipWithdrawn).HasDefaultValue(value: false);
            });

            //인덱스 지정 : 중복이 되지 않도록IsUnique(unique: true) 
            modelBuilder.Entity<User>().HasIndex(c => new { c.UserEmail }).IsUnique(unique: true);
        }

    }
}
