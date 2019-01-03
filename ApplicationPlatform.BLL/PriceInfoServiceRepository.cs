using ApplicationPlatform.DAL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.BLL
{
    public class PriceInfoServiceRepository : BaseServiceRepository<PriceInfo>, IPriceInfoServiceRepository
    {
        /// <summary>
        /// 构造函数，通过仓储工厂调用dal中的具体的仓储
        /// </summary>
        /// <param name="currentRepository"></param>
        public PriceInfoServiceRepository()
            : base(RepositoryFactory.PriceIfoRepository)
        {
        }
    }
}