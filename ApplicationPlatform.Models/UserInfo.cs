using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class UserInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int Id { set; get; }

        /// <summary>
        /// UserName
        /// </summary>
        [Display(Name = "UserName")]
        public string UserName { set; get; }
        /// <summary>
        /// Sex
        /// </summary>
        [Display(Name = "Sex")]
        public string Sex { set; get; }
        /// <summary>
        /// WeChat
        /// </summary>
        [Display(Name = "WeChat")]
        public string WeChat { set; get; }
        /// <summary>
        /// LastLoginTime
        /// </summary>
        [Display(Name = "LastLoginTime")]
        public DateTime? LastLoginTime { set; get; }
        /// <summary>
        /// Profile
        /// </summary>
        [Display(Name = "Profile")]
        public string Profile { set; get; }
        /// <summary>
        /// RegistTime
        /// </summary>
        [Display(Name = "RegistTime")]
        public DateTime RegistTime { set; get; }
        /// <summary>
        /// Email Address
        /// </summary>
        [Display(Name = "Email Address")]
        public string Email { set; get; }
        /// <summary>
        /// Phone Number
        /// </summary>
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// IsDelete
        /// </summary>
        [Display(Name = "IsDelete")]
        public bool IsDelete { set; get; }
        /// <summary>
        /// Role List
        /// </summary>
        public virtual List<RoleInfo> RoleInfoes { get; set; }
        public virtual List<CListItem> CListItems { get; set; }

        public UserInfo()
        {
            RoleInfoes = new List<RoleInfo>();
            CListItems = new List<CListItem>();
        }
    }
}