using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.BLL
{
    public class UserInfoServiceRepository : BaseServiceRepository<UserInfo>, IUserInfoServiceRepository
    {
        /// <summary>
        /// 构造函数，通过仓储工厂调用dal中的具体的仓储
        /// </summary>
        /// <param name="currentRepository"></param>
        public UserInfoServiceRepository()
            : base(RepositoryFactory.UserInfoRepository)
        {
        }
    }
}