using ApplicationPlatform.DAL;
using ApplicationPlatform.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApplicationPlatform.Site.ViewModels.ApplicationInfoViewModels
{
    public class ApplicationPermissionViewModel
    {
        private DbContext context = new AppContext();
        public List<UserInfo> userInfoes { get; set; }
        public List<CListItem> cListItems { get; set; }
        public UserInfo selectedUserInfo { get; set; }
        public Dictionary<string, bool> myFileNames { get; set; }
        public ApplicationPermissionViewModel(UserInfo userInfo)
        {
            userInfoes = context.Set<UserInfo>().Include("CListItems").Where(x => x.Id != null && x.IsDelete == false).OrderBy(x => x.UserName).ToList();
            cListItems = context.Set<CListItem>().Include("UserInfoes").Where(x => x.Id !=null && x.IsDelete == false).ToList();
            if (userInfo == null)
            {
                var UserInfo = context.Set<UserInfo>().Include("CListItems").Where(e => e.UserName == "admin").FirstOrDefault();
                selectedUserInfo = UserInfo;
            }
            else
            {
                selectedUserInfo = userInfo;
            }
        }
        public Dictionary<string, bool> GetMyApplicaitonNames(UserInfo userInfo)
        {
            Dictionary<string, bool> _myFileNames = new Dictionary<string, bool>();
            UserInfo _userInfo = context.Set<UserInfo>().Include("CListItems").Where(c => c.Id == userInfo.Id).FirstOrDefault();
            foreach (CListItem myFile in cListItems)
            {
                bool a = false;
                foreach (CListItem _myFile in _userInfo.CListItems)
                {
                    if (myFile.Id == _myFile.Id)
                    { a = true; break; }
                    else { a = false; }
                }
                _myFileNames.Add(myFile.Id.ToString(),a);
            }

            myFileNames = _myFileNames;
            return _myFileNames;
        }
    }
}