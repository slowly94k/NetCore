using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Data
{
    //DbContext 상속을 받음
    public class DBFirstDbContext : DbContext
    {
        //생성자 상속
        public DBFirstDbContext(DbContextOptions<DBFirstDbContext> options) : base(options)
        {

        }

        //DB테이블 리스트 지정
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserRolesByUser> UserRolesByUsers { get; set; }


        //메서드 상속,  부모클래스 DbContext에서 OnModelCreating 메서드가 virtual키워드로 지정되어 있어야만 상속 가능
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //DB 테이블 이름 변경 및 매핑
            //매핑을 연결해서 "DB테이블 리스트 지정"에 있는 데이터를 사용할 수 있도록 매핑시켜주는 역할을 한다 (9.)
            modelBuilder.Entity<User>().ToTable(name: "User");
            modelBuilder.Entity<UserRole>().ToTable(name: "UserRole");
            modelBuilder.Entity<UserRolesByUser>().ToTable(name: "UserRolesByUser");

            //복합키 지정 (9.)
            modelBuilder.Entity<UserRolesByUser>().HasKey(c => new { c.UserId, c.RoleId });
        }

    }
}
