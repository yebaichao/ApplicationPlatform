using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace ApplicationPlatform.DAL
{
    public class ContextFactory
    {
        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <returns></returns>
        public static DbContext GetDbContext()
        {
            AppContext _AppContext = CallContext.GetData("DefaultConnection") as AppContext;
            if (_AppContext == null)
            {
                _AppContext = new AppContext();
                IDatabaseInitializer<AppContext> dbInitializer = null;
                if (_AppContext.Database.Exists())
                {
                    //如果数据库已经存在
                    //dbInitializer = new DropCreateDatabaseIfModelChanges<AppContext>();
                    dbInitializer = new CreateDatabaseIfNotExists<AppContext>();
                }
                else
                {
                    //总是先删除然后再创建
                    dbInitializer = new DropCreateDatabaseAlways<AppContext>();
                }
                dbInitializer.InitializeDatabase(_AppContext);
                CallContext.SetData("DefaultConnection", _AppContext);
                return _AppContext;
            }

            return _AppContext;
        }
    }
}