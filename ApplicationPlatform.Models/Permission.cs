using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Models
{
    public class Permission
    {
        /// <summary>
        /// Permission Id
        /// </summary>
        public virtual int Id
        {
            set;
            get;
        }
        /// <summary>
        /// Permission Action No
        /// </summary>
        public virtual int ActionNo
        {
            set;
            get;
        }

        /// <summary>
        /// Controller No
        /// </summary>
        public virtual int ControllerNo
        {
            set;
            get;
        }

        /// <summary>
        /// Controller Name
        /// </summary>
        public virtual string ControllerName
        {
            set;
            get;
        }

        /// <summary>
        /// Permission Action Name
        /// </summary>
        public virtual string ActionName
        {
            set;
            get;
        }

        /// <summary>
        /// Controller
        /// </summary>
        public virtual string Controller
        {
            set;
            get;
        }

        /// <summary>
        /// Action
        /// </summary>
        public virtual string Action
        {
            set;
            get;
        }
        /// <summary>
        /// RoleInfoes
        /// </summary>
        public virtual List<RoleInfo> RoleInfoes
        {
            set;
            get;
        }
    }
}