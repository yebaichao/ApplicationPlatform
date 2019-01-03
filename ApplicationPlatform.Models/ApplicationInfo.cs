using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class ApplicationInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Key]
        public int Id { get; set; }
         /// <summary>
         /// UserName
         /// </summary>
         [Display(Name = "UserName")]
         public string UserName { get; set; }
         /// <summary>
         /// Site
         /// </summary>
         [Display(Name = "Site")]
         public string Site { get; set; }
         /// <summary>
         /// Project
         /// </summary>
         [Display(Name = "Project")]
         public string Project { get; set; }
         /// <summary>
         /// Type1
         /// </summary>
         [Display(Name = "Item")]
         public string Item { get; set; }
         /// <summary>
         /// Stage
         /// </summary>
         [Display(Name = "Stage")]
         public string Stage { get; set; }
         /// <summary>
         /// Type
         /// </summary>
         [Display(Name = "Type")]
         public string Type { get; set; }
         /// <summary>
         /// Num
         /// </summary>
         [Display(Name = "Num")]
         public int Num { set; get; }
         /// <summary>
         /// CreateTime
         /// </summary>
         [Display(Name = "CreateTime")]
         public DateTime CreateTime { get; set; }
         /// <summary>
         /// StartTime
         /// </summary>
         [Display(Name = "StartTime")]
         public DateTime? StartTime { get; set; }
         /// <summary>
         /// EndTime
         /// </summary>
         [Display(Name = "EndTime")]
         public DateTime? EndTime { get; set; }
         /// <summary>
         /// Status
         /// </summary>
         [Display(Name = "Status")]
         public string Status { get; set; }
         /// <summary>
         /// IsTechApproved
         /// </summary>
         [Display(Name = "IsTechApproved")]
         public bool IsTechApproved { set; get; }
         /// <summary>
         /// TechApprovedUser
         /// </summary>
         [Display(Name = "TechApprovedUser")]
         public string TechApprovedUser { set; get; }
         /// <summary>
         /// IsComApproved
         /// </summary>
         [Display(Name = "IsComApproved")]
         public bool IsComApproved { set; get; }
         /// <summary>
         /// ComApprovedUser
         /// </summary>
         [Display(Name = "ComApprovedUser")]
         public string ComApprovedUser { set; get; }
         /// <summary>
         /// TechDsp
         /// </summary>
         [Display(Name = "TechDsp")]
         public string TechDsp { set; get; }
         /// <summary>
         /// ComDsp
         /// </summary>
         [Display(Name = "ComDsp")]
         public string ComDsp { set; get; }
         /// <summary>
         /// Description
         /// </summary>
         [Display(Name = "Description")]
         public string Description { set; get; }
        /// <summary>
        /// IsDelete
        /// </summary>
        [Display(Name = "IsDelete")]
        public bool IsDelete { set; get; }
        /// <summary>
        /// IsCompleted
        /// </summary>
        [Display(Name = "IsCompleted")]
        public bool IsCompleted { get; set; }
        /// <summary>
        /// ArrangeUser
        /// </summary>
        [Display(Name = "ArrangeUser")]
        public string ArrangeUser { get; set; }
        /// <summary>
        /// Product
        /// </summary>
        [Display(Name="Product")]   
        public string Product { get; set; }
        /// <summary>
        /// Subitem
        /// </summary>
        [Display(Name = "Subitem")]
        public string Subitem { get; set; }
        /// <summary>
        /// Statuses
        /// </summary>
        [Display(Name = "Statuses")]
        public string Statuses { get; set; }
        /// <summary>
        /// IsSaved
        /// </summary>
        [Display(Name = "IsSaved")]
        public bool IsSaved { get; set; }
        /// <summary>
        /// SavedTime
        /// </summary>
        [Display(Name = "SavedTime")]
        public DateTime? SavedTime { get; set; }
        /// <summary>
        /// ATD
        /// </summary>
        [Display(Name = "ATD")]
        public DateTime? ATD { get; set; }
        /// <summary>
        /// UnitPrice
        /// </summary>
        public double UnitPrice { get; set; }
    }
}