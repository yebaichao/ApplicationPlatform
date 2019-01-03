using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.ViewModels
{
    public class PagedListViewModel<TModel>
    {
        public const int DefaultPageSize = 10;
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPageNum { get; set; }
        public int PageCount { get; set; }

        public bool HasPreviousPage
        {
            get
            {
                return CurrentPageNum > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return CurrentPageNum < PageCount;
            }
        }

        public List<TModel> PagedModels { get; set; }

        public List<int> PageSizeList { get; private set; }

        public PagedListViewModel(int totalCount, int currentPageNum, int pageSize, List<TModel> pagedModels)
        {
            PageSize = pageSize;
            PageSizeList = new List<int>();
            ResetPageSizeList(totalCount);
            UpdatePageBySpecifiedPagedModels(totalCount, pageSize, currentPageNum, pagedModels);
        }

        public PagedListViewModel(int currentPageNum, int pageSize, List<TModel> allModels)
        {
            PageSize = pageSize;
            PageSizeList = new List<int>();
            ResetPageSizeList(allModels.Count);
            UpdatePageByAllModels(pageSize, currentPageNum, allModels);
        }

        private void ResetPageSizeList(int totalCount)
        {
            PageSizeList.Clear();

            AddPageSize(DefaultPageSize);

            if (totalCount > 10)
            {
                AddPageSize(10);
            }

            if (totalCount > 20)
            {
                AddPageSize(20);
            }

            if (totalCount > 50)
            {
                AddPageSize(50);
            }

            if (totalCount > 100)
            {
                AddPageSize(100);
            }

            AddPageSize(totalCount);
        }

        private void AddPageSize(int pageSize)
        {
            if (!PageSizeList.Contains(pageSize))
            {
                PageSizeList.Add(pageSize);
            }
        }

        public void UpdatePageBySpecifiedPagedModels(int totalCount, int pageSize, int currentPageNum, List<TModel> pagedModels)
        {
            if (!PageSizeList.Contains(pageSize))
            {
                PageSizeList.Insert(0, pageSize);
            }

            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPageNum = currentPageNum;

            PageCount = TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;

            PagedModels = pagedModels;
        }

        public void UpdatePageByAllModels(int pageSize, int currentPageNum, List<TModel> allModels)
        {
            if (!PageSizeList.Contains(pageSize))
            {
                PageSizeList.Insert(0, pageSize);
            }

            TotalCount = allModels.Count;
            PageSize = pageSize;
            CurrentPageNum = currentPageNum;

            PageCount = TotalCount % PageSize == 0 ? TotalCount / PageSize : TotalCount / PageSize + 1;

            if (CurrentPageNum <= 1)
            {
                CurrentPageNum = 1;
            }
            if (CurrentPageNum >= PageCount)
            {
                CurrentPageNum = PageCount;
            }

            PagedModels = allModels.Skip((CurrentPageNum - 1) * PageSize).Take(PageSize).ToList();
        }
    }
}