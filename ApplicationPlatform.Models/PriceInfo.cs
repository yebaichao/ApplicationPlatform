using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class PriceInfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [Display(Name = "Price")]
        public int Price { get; set; }
        /// <summary>
        /// Product
        /// </summary>
         [Display(Name = "Product")]
        public string Product { get; set; }
        /// <summary>
         /// Project
        /// </summary>
         [Display(Name = "Project")]
        public string Project { get; set; }
        /// <summary>
         /// Type
        /// </summary>
         [Display(Name = "Type")]
        public string Type { get; set; }
        /// <summary>
         /// Item
        /// </summary>
        [Display(Name = "Item")]
        public string Item { get; set; }
        /// <summary>
         /// Subitem
        /// </summary>
         [Display(Name = "Subitem")]
        public string Subitem { get; set; }
        /// <summary>
        /// IsDelete
        /// </summary>
        [Display(Name = "IsDelete")]
        public bool IsDelete { get; set; }
    }
}