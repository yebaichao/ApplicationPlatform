using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Utilities.NodeModels
{
    public class CItem
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public CItem(string Id, string Text)
        {
            this.Id = Id;
            this.Text = Text;
        }
    }
}