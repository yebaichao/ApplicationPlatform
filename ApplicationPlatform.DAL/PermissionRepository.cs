﻿using ApplicationPlatform.IDAL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.DAL
{
    /// <summary>
    /// 权限数据操作dal层
    /// </summary>
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
    }
}