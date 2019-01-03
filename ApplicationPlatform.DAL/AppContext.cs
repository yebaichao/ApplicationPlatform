using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.DAL
{
    public class AppContext : DbContext
    {
        /// <summary>
        /// name:数据库连接字符串
        /// </summary>
        public AppContext()
            : base("name=DefaultConnection")
        {

            this.Configuration.ProxyCreationEnabled = true;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RoleInfo> RoleInfoes { get; set; }
        public DbSet<UserInfo> UserInfoes { set; get; }
        public DbSet<ApplicationInfo> ApplicationInfoes { set; get; }
        public DbSet<LogInfo> LogInfoes { set; get; }
        public DbSet<CListItem> CListItems { get; set; }
        public DbSet<PriceInfo> PriceInfoes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //多对多关系
            //角色与权限多对多关系
            modelBuilder.Entity<RoleInfo>().HasMany(r => r.Permissions).WithMany(o => o.RoleInfoes).Map(f =>
            {
                f.MapLeftKey("RoleInfo_Id");
                f.MapRightKey("Permission_Id");
            });
            //用户与角色多对多关系
            modelBuilder.Entity<RoleInfo>().HasMany(r => r.UserInfoes).WithMany(o => o.RoleInfoes).Map(f =>
            {
                f.MapLeftKey("RoleInfo_Id");
                f.MapRightKey("UserInfo_Id");
            });
            //UserInfo与CListItem多对多关系
            modelBuilder.Entity<CListItem>().HasMany(r => r.UserInfoes).WithMany(o => o.CListItems).Map(f =>
            {
                f.MapLeftKey("CListItem_Id");
                f.MapRightKey("UserInfo_Id");
            });
            ////主题与附件多对多关系
            //modelBuilder.Entity<ThreadInfo>().HasMany(r => r.AttachInfoes).WithMany(o => o.ThreadInfoes).Map(f =>
            //{
            //    f.MapLeftKey("ThreadInfo_Id");
            //    f.MapRightKey("AttachInfo_Id");
            //});
            ////版块与主题多对多关系
            //modelBuilder.Entity<BoardInfo>().HasMany(r => r.ThreadInfoes).WithMany(o => o.BoardInfoes).Map(f =>
            //{
            //    f.MapLeftKey("BoardInfo_Id");
            //    f.MapRightKey("ThreadInfo_Id");
            //});
            ////部门与用户一对多关系
            //modelBuilder.Entity<DepartmentInfo>().HasMany(r => r.UserInfoes).WithRequired(o => o.Department).Map(f => { f.MapKey("dept_users_Id"); });
            ////主题与附件一对多关系
            ////modelBuilder.Entity<ThreadInfo>().HasMany(r => r.AttachInfoes).WithRequired(o => o.ThreadInfo).Map(f => { f.MapKey("thread_attches_Id"); });
            ////回复与附件一对多关系
            ////modelBuilder.Entity<ReplyInfo>().HasMany(r => r.AttachInfoes).WithRequired(o => o.ReplyInfo).Map(f => { f.MapKey("reply_attches_Id"); });
            ////主题与回复一对多关系
            //modelBuilder.Entity<ThreadInfo>().HasMany(r => r.ReplyInfoes).WithRequired(o => o.ThreadInfo).Map(f => { f.MapKey("thread_replys_Id"); });
            ////分区与版块一对多关系
            //modelBuilder.Entity<PartInfo>().HasMany(r => r.BoardInfoes).WithRequired(o => o.PartInfo).Map(f => { f.MapKey("part_boards_Id"); });
            ////分区与公告一对多关系
            ////modelBuilder.Entity<PartInfo>().HasMany(r => r.BulletinInfoes).WithRequired(o => o.PartInfo).Map(f => { f.MapKey("part_bulletins_Id"); });
            ////版块与主题一对多关系
            ////modelBuilder.Entity<BoardInfo>().HasMany(r => r.ThreadInfoes).WithRequired(o => o.BoardInfo).Map(f => { f.MapKey("board_threads_Id"); });
            ////版块与公告一对多关系
            ////modelBuilder.Entity<BoardInfo>().HasMany(r => r.BulletinInfoes).WithRequired(o => o.BoardInfo).Map(f => { f.MapKey("board_bulletins_Id"); });
            base.OnModelCreating(modelBuilder);
        }

    }
}