using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ApplicationPlatform.Site.Utilities
{
    /// <summary>
    /// MD5加密工具类
    /// </summary>
    public class Md5Helper
    {
        /// <summary>
        /// 字节数组加密
        /// </summary>
        /// <param name="sourceBytes">源字节数组</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(byte[] sourceBytes)
        {
            //创建加密器
            MD5 md5Provider = new MD5CryptoServiceProvider();

            //加密源字节
            byte[] encryptBytes = md5Provider.ComputeHash(sourceBytes);

            #region foreach遍历转换方式

            //创建拼接器
            var encryptStringBuilder = new StringBuilder();
            //16进制转换
            foreach (byte byt in encryptBytes)
                encryptStringBuilder.Append(byt.ToString("x2"));
            //获取转后字符转
            string encryptString = encryptStringBuilder.ToString();

            #endregion

            return encryptString;
        }

        /// <summary>
        /// 以Unicode编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">Unicode字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string UnicodeEncrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.Unicode.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以Unicode编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">Unicode字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string UnicodeEncrypt16(string sourceString)
        {
            return UnicodeEncrypt32(sourceString).Substring(8, 16);
        }

        /// <summary>
        /// 以默认编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">默认编码字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string DefaultEncrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.Default.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以默认编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">默认字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string DefaultEncrypt16(string sourceString)
        {
            return DefaultEncrypt32(sourceString).Substring(8, 16);
        }

        /// <summary>
        /// 以ASCII编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">ASCII字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string AsciiEncrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.ASCII.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以ASCII编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">ASCII字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string AsciiEncrypt16(string sourceString)
        {
            return AsciiEncrypt32(sourceString).Substring(8, 16);
        }

        /// <summary>
        /// 以UTF32编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">UTF32字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Utf32Encrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.UTF32.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以UTF32编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">UTF32字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Utf32Encrypt16(string sourceString)
        {
            return Utf32Encrypt32(sourceString).Substring(8, 16);
        }

        /// <summary>
        /// 以UTF8编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">UTF8字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Utf8Encrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以UTF8编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">UTF8字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Utf8Encrypt16(string sourceString)
        {
            return Utf8Encrypt32(sourceString).Substring(8, 16);
        }

        /// <summary>
        /// 以UTF7编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">UTF7字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Utf7Encrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.UTF7.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以UTF7编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">UTF7字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Utf7Encrypt16(string sourceString)
        {
            return Utf8Encrypt32(sourceString).Substring(8, 16);
        }

        /// <summary>
        /// 以BigEndianUnicode编码解析源字符串并加密为32位MD5字符串
        /// </summary>
        /// <param name="sourceString">BigEndianUnicode字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string BigEndianUnicodeEncrypt32(string sourceString)
        {
            byte[] sourceBytes = Encoding.BigEndianUnicode.GetBytes(sourceString);

            return Encrypt(sourceBytes);
        }

        /// <summary>
        /// 以BigEndianUnicode编码解析源字符串并加密为16位MD5字符串
        /// </summary>
        /// <param name="sourceString">BigEndianUnicode字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string BigEndianUnicodeEncrypt16(string sourceString)
        {
            return BigEndianUnicodeEncrypt32(sourceString).Substring(8, 16);
        }
    }
}