﻿using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.ViewModels.ApplicationInfoViewModels
{
    public class ApplicationInfoListViewModel : PagedListViewModel<ApplicationInfo>
    {
        /// <summary>
        /// 从数据库获取分页后的数据，然后设置到PagedListViewModel。
        /// </summary>
        /// <param name="totalCount">总数。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="currentPageNum">当前页序号。</param>
        /// <param name="pagedModels">当前页对象列表。</param>
        public ApplicationInfoListViewModel(int totalCount, int currentPageNum, int pageSize, List<ApplicationInfo> pagedModels)
            : base(totalCount, pageSize, currentPageNum, pagedModels)
        {

        }

        /// <summary>
        /// 获取所有实例，由PagedListViewModel进行分页。
        /// </summary>
        /// <param name="pageSize">每页大小。</param>
        /// <param name="currentPageNum">当前页序号。</param>
        /// <param name="allModels">所有实例。</param>
        public ApplicationInfoListViewModel(int currentPageNum, int pageSize, List<ApplicationInfo> allModels)
            : base(pageSize, currentPageNum, allModels)
        {

        }
    }
}