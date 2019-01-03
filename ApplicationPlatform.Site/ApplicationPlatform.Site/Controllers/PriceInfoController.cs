using ApplicationPlatform.BLL;
using ApplicationPlatform.IBLL;
using ApplicationPlatform.Models;
using ApplicationPlatform.Site.Attributes;
using ApplicationPlatform.Site.Utilities;
using ApplicationPlatform.Site.ViewModels.ApplicationInfoViewModels;
using ApplicationPlatform.Utilities;
using ApplicationPlatform.Utilities.NodeModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebMatrix.WebData;
using Excel = Microsoft.Office.Interop.Excel;

namespace ApplicationPlatform.Site.Controllers
{
    [Description(No = 1, Name = "PriceManagement")]
    public class PriceInfoController : Controller
    {
        private IPriceInfoServiceRepository PriceInfoService = new PriceInfoServiceRepository();
        //
        // GET: /PriceInfo/
        [RoleAuthorize]
        [Description(No = 1, Name = "PriceList")]
        public ActionResult PriceListView()
        { 
            return View(); 
        }
        public ActionResult CreatePriceListView(int? currentPageNum, int? pageSize)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, priceCount;
            Expression<Func<PriceInfo, bool>> where = null;
            Expression<Func<PriceInfo, string>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false;
            whereDateTime = x => x.Product;
            var priceInfoes = PriceInfoService.FindPaged(pageSize.Value, ref pageNum, out priceCount, out pageCount, where, false, whereDateTime).ToList();
            List<Price> PriceViews = new List<Price>();
            int j = 1;
            foreach (PriceInfo itemApp in priceInfoes)
            {
                Price item = new Price();
                item.unitprice = itemApp.Price.ToString();
                item.product = itemApp.Product;
                item.project = itemApp.Project;
                item.type = itemApp.Type;
                item.item = itemApp.Item;
                item.subitem = itemApp.Subitem;
                item.serialNumber = j.ToString();
                item.requirementId = itemApp.Id.ToString();
                PriceViews.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = priceCount, rows = PriceViews };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        public ActionResult CreatePriceListSearchView(int? currentPageNum, int? pageSize, string searchText)
        {
            //当前登录用户名
            string UserName = WebSecurity.CurrentUserName;
            if (!currentPageNum.HasValue)
            {
                currentPageNum = 1;
            }
            if (!pageSize.HasValue)
            {
                pageSize = ApplicationInfoListViewModel.DefaultPageSize;
            }
            string tempPageNum = Request.Params["page"];
            currentPageNum = Convert.ToInt32(tempPageNum);
            int pageNum = currentPageNum.Value, pageCount, priceCount;
            Expression<Func<PriceInfo, bool>> where = null;
            Expression<Func<PriceInfo, string>> whereDateTime = null;
            where = x => x.Id != null && x.IsDelete == false && x.Product.Contains(searchText) && x.Project.Contains(searchText) && x.Type.Contains(searchText) && x.Item.Contains(searchText) && x.Subitem.Contains(searchText);
            whereDateTime = x => x.Product;
            var priceInfoes = PriceInfoService.FindPaged(pageSize.Value, ref pageNum, out priceCount, out pageCount, where, false, whereDateTime).ToList();
            List<Price> PriceViews = new List<Price>();
            int j = 1;
            foreach (PriceInfo itemApp in priceInfoes)
            {
                Price item = new Price();
                item.unitprice = itemApp.Price.ToString();
                item.product = itemApp.Product;
                item.project = itemApp.Project;
                item.type = itemApp.Type;
                item.item = itemApp.Item;
                item.subitem = itemApp.Subitem;
                item.serialNumber = j.ToString();
                item.requirementId = itemApp.Id.ToString();
                PriceViews.Add(item);
                j++;
            }
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            var response = new { total = priceCount, rows = PriceViews };
            var Data = Jss.Serialize(response);
            var content = Content(Data);
            return Content(Data);
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "UploadPrice")]
        public ActionResult SubmitPrice(HttpPostedFileBase[] fileToUpload)
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();
            if (fileToUpload[0] != null)
            {
                Stream inputStream = fileToUpload[0].InputStream;
                string fileSaveFolder = Server.MapPath("~/App_Data/PriceList");
                //如果目标不存在，则创建
                if (!Directory.Exists(fileSaveFolder))
                {
                    Directory.CreateDirectory(fileSaveFolder);
                }
                byte[] buffer = new byte[inputStream.Length];
                inputStream.Read(buffer, 0, buffer.Length);
                string strFileMd5 = Md5Helper.Encrypt(buffer);
                //名称格式一律以md5命名。
                string strNewName = strFileMd5 + Path.GetExtension(fileToUpload[0].FileName);
                string fileSavePath = Path.Combine(fileSaveFolder, strNewName);
                if (!System.IO.File.Exists(fileSavePath))
                {
                    fileToUpload[0].SaveAs(fileSavePath);
                }
                ImportPriceExcel(fileSavePath);
            }
            else
            {
                var response0 = new { code = 0 };
                var Data0 = Jss.Serialize(response0);
                return Content(Data0);
            }
            var response1 = new { code = 1};
            var Data1 = Jss.Serialize(response1);
            return Content(Data1);
        }
        [RoleAuthorize]
        [Description(No = 1, Name = "DownloadPrice")]
        public void DownloadPrice()
        {
            JavaScriptSerializer Jss = new JavaScriptSerializer();

            string fileToOpen = Server.MapPath("~/App_Data/PriceList.xlsx");

            string filePath=ExportPriceExcel(fileToOpen);
            if (!string.IsNullOrEmpty(filePath))
            {
                //以字符流的形式下载文件
                FileStream fs = new FileStream(filePath, FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                Response.ContentType = "application/octet-stream";
                //通知浏览器下载文件而不是打开
                string fileNameTemp = HttpUtility.UrlEncode("PriceList.xlsx", System.Text.Encoding.UTF8);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNameTemp);
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }

        private void ImportPriceExcel(string FilePath)
        {
            Excel.Application ExcelApp;
            Excel.Workbooks xlsWorkBooks;
            Excel.Workbook xlsWorkBook;
            Excel.Worksheet xlsWorkSheet;
            //
            //设置程序运行语言
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //创建Application
            ExcelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application")) as Excel.Application;
            //设置是否显示警告窗体
            ExcelApp.DisplayAlerts = false;
            //设置是否显示Excel
            ExcelApp.Visible = false;
            //禁止刷新屏幕
            ExcelApp.ScreenUpdating = false;
            xlsWorkBooks = ExcelApp.Workbooks;
            xlsWorkBook = xlsWorkBooks.Open(FilePath);
            xlsWorkSheet = xlsWorkBook.Worksheets[1];
            try
            {
                var prices = PriceInfoService.FindAll(x => x.Id != null && x.IsDelete == false);
                foreach(PriceInfo item in prices)
                {
                    item.IsDelete = true;
                    PriceInfoService.Update(item);
                }
                int num = xlsWorkSheet.UsedRange.Rows.Count;
                bool start = false;
                for (int i = 1; i <= num; i++)
                {
                    if (xlsWorkSheet.Cells[i, 1].Value.ToString() == "NO.")
                    { start = true; i++; }
                    if (!start)
                    { continue; }
                    PriceInfo priceInfo = new PriceInfo();
                    if (xlsWorkSheet.Cells[i, 2].Value == null || xlsWorkSheet.Cells[i, 3].Value == null || xlsWorkSheet.Cells[i, 4].Value == null || xlsWorkSheet.Cells[i, 5].Value == null || xlsWorkSheet.Cells[i, 6].Value == null || xlsWorkSheet.Cells[i, 7].Value==null)
                    {continue ;}
                    priceInfo.Product = xlsWorkSheet.Cells[i, 2].Value.ToString();
                    priceInfo.Project = xlsWorkSheet.Cells[i, 3].Value.ToString();
                    priceInfo.Type = xlsWorkSheet.Cells[i, 4].Value.ToString();
                    priceInfo.Item = xlsWorkSheet.Cells[i, 5].Value.ToString();
                    priceInfo.Subitem = xlsWorkSheet.Cells[i, 6].Value.ToString();
                    priceInfo.Price = Convert.ToInt32(xlsWorkSheet.Cells[i, 7].Value.ToString());
                    PriceInfoService.Add(priceInfo);
                    int count = PriceInfoService.SaveChanges();
                }
                xlsWorkBook.Close();
                xlsWorkBook = null;
                xlsWorkBooks.Close();
                xlsWorkBooks = null;
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); }
            finally
            {
                if (xlsWorkBook != null)
                {
                    //xlsWorkBook.Close();
                    xlsWorkBook = null;
                }
                if (xlsWorkBooks != null)
                {
                    //xlsWorkBooks.Close();
                    xlsWorkBooks = null;
                }
                if (ExcelApp != null)
                {
                    CloseExcelDLSB(ExcelApp);
                }
            }
        }
        private string ExportPriceExcel(string FilePath)
        {
            Excel.Application ExcelApp;
            Excel.Workbooks xlsWorkBooks;
            Excel.Workbook xlsWorkBook;
            Excel.Worksheet xlsWorkSheet;
            //
            //设置程序运行语言
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //创建Application
            ExcelApp = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application")) as Excel.Application;
            //设置是否显示警告窗体
            ExcelApp.DisplayAlerts = false;
            //设置是否显示Excel
            ExcelApp.Visible = false;
            //禁止刷新屏幕
            ExcelApp.ScreenUpdating = false;
            xlsWorkBooks = ExcelApp.Workbooks;
            xlsWorkBook = xlsWorkBooks.Add(FilePath);
            xlsWorkSheet = xlsWorkBook.Worksheets[1];
            try
            {
                var prices = PriceInfoService.FindAll(x => x.Id != null && x.IsDelete == false);
                int i = 0;
                foreach (PriceInfo item in prices)
                {
                    //NO.
                    xlsWorkSheet.Cells[2 + i, 1].Value = (i + 1).ToString();
                    //Product
                    xlsWorkSheet.Cells[2 + i, 2].Value = item.Product;
                    //Project
                    xlsWorkSheet.Cells[2 + i, 3].Value = item.Project;
                    //Type
                    xlsWorkSheet.Cells[2 + i, 4].Value = item.Type;
                    //Item
                    xlsWorkSheet.Cells[2 + i, 5].Value = item.Item;
                    //Subitem
                    xlsWorkSheet.Cells[2 + i, 6].Value = item.Subitem;
                    //UnitPrice
                    xlsWorkSheet.Cells[2 + i, 7].Value = item.Price;
                    i++;
                }

                string fileToSave = Server.MapPath("~/App_Data/PriceList");
                fileToSave = Path.Combine(fileToSave, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")+".xlsx");
                xlsWorkBook.SaveAs(fileToSave);
                xlsWorkBook.Close();
                xlsWorkBook = null;
                xlsWorkBooks.Close();
                xlsWorkBooks = null;
                return fileToSave;
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); return ""; }
            finally
            {
                if (xlsWorkBook != null)
                {
                    //xlsWorkBook.Close();
                    xlsWorkBook = null;
                }
                if (xlsWorkBooks != null)
                {
                    //xlsWorkBooks.Close();
                    xlsWorkBooks = null;
                }
                if (ExcelApp != null)
                {
                    CloseExcelDLSB(ExcelApp);
                }
            }
        }
        private void CloseExcelDLSB(Excel.Application xlApp)
        {
            try
            {
                xlApp.Quit();
                IntPtr hwnd = new IntPtr(xlApp.Hwnd);
                int iD = 0;
                GetWindowThreadProcessId(hwnd, out iD);
                Process.GetProcessById(iD).Kill();
                xlApp = null;
            }
            catch (Exception ex)
            { ExceptionLogHelp.WriteLog(ex); }
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


    }
}
