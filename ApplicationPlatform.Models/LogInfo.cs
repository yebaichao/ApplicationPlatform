using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class LogInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int Id { set; get; }
        /// <summary>
        /// UserName
        /// </summary>
        [Display(Name="UserName")]
        public string UserName { get; set; }
        /// <summary>
        /// Content
        /// </summary>
        [Display(Name = "Content")]
        public string Content { get; set; }
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
    }
}