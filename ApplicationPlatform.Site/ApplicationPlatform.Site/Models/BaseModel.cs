using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.Models
{
    public class BaseModel
    {
        public int CurrentPageNum { get; set; }
        public int PageSize { get; set; }
    }
}