using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class CListItem
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public int ParentId { get; set; }
        public bool IsDelete { get; set; }
        public virtual List<UserInfo> UserInfoes { get; set; }
        public CListItem()
        {
            UserInfoes = new List<UserInfo>();
        }
    }
}