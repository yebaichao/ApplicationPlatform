using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class RoleInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int Id { set; get; }
        /// <summary>
        /// RoleName
        /// </summary>
        [Display(Name = "RoleName")]
        public string RoleName { get; set; }
        /// <summary>
        /// RoleDescription
        /// </summary>
        [Display(Name = "RoleDescription")]
        public string RoleDescription { get; set; }
        /// <summary>
        /// CreateTime
        /// </summary>
        [Display(Name = "CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// IsDelete
        /// </summary>
        [Display(Name = "IsDelete")]
        public bool IsDelete { get; set; }
        /// <summary>
        /// User List
        /// </summary>
        public virtual List<UserInfo> UserInfoes { get; set; }
        /// <summary>
        /// Permissions
        /// </summary>
        public virtual List<Permission> Permissions { get; set; }

        public RoleInfo()
        {
            UserInfoes = new List<UserInfo>();
            Permissions = new List<Permission>();
        }
    }
}