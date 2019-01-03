using ApplicationPlatform.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.DAL
{
    public static class RepositoryFactory
    {
        public static IUserInfoRepository UserInfoRepository { get { return new UserInfoRepository(); } }
        public static ILogInfoRepository LogInfoRepository { get { return new LogInfoRepository(); } }
        public static IApplicationInfoRepository ApplicationInfoRepository { get { return new ApplicationInfoRepository(); } }
        public static IRoleInfoRepository RoleInfoRepository { get { return new RoleInfoRepository(); } }
        public static IPermissionRepository PermissionRepository { get { return new PermissionRepository(); } }
        public static ICListItemRepository CListItemRepository { get { return new CListItemRepository(); } }
        public static IPriceInfoRepository PriceIfoRepository { get { return new PriceInfoRepository(); } }
    }
}