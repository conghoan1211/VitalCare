using API.Common;
using API.Models;
using System.Text.RegularExpressions;

namespace API.Helper
{
    public static class Common
    {
        public static string PreparePhone(this string phone, out object result)
        {
            result = null;
            if (string.IsNullOrEmpty(phone)) return "Số điện thoại không được để trống";

            result = phone.Trim().Standardizing().RemoveWhitespace();
            return "";
        }

        public static string CheckPhone(this IQueryable<User> query, string phone)
        {
            string msg = phone.PreparePhone(out object result);
            if (msg.Length > 0) return msg;
            if (result.ObjToString().Length != 10) return "Số điện thoại phải có 10 chữ số";

            bool check = query.Any(u => u.Phone.Contains(result.ObjToString()));
            if (check) return "Số điện thoại đã được sử dụng";
            return "";
        }

        public static string CheckPhone(this string phone)
        {
            string msg = phone.PreparePhone(out object result);
            if (msg.Length > 0) return msg;
            if (result.ObjToString().Length != 10) return "Số điện thoại phải có 10 chữ số";
            return "";
        }

        public static string PrepareEmail(this string email, out object result)
        {
            result = null;
            if (string.IsNullOrEmpty(email)) return "Email không được để trống";

            result = email.Trim().RemoveWhitespace(); // Chuẩn hóa email
            return "";
        }

        public static bool IsValidEmailFormat(this string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Định dạng email cơ bản
            return Regex.IsMatch(email, emailPattern);
        }

        public static string CheckEmail(this IQueryable<User> query, string email)
        {
            string msg = email.PrepareEmail(out object result);
            if (msg.Length > 0) return msg;
            if (!result.ObjToString().IsValidEmailFormat()) return "Email không đúng định dạng";

            bool check = query.Any(u => u.Email.ToLower() == result.ObjToString().ToLower());
            if (check) return ConstMessage.EMAIL_EXISTED;

            return "";
        }
        public static bool IsS3Url(this string url)
        {
            if (url.IsEmpty()) return false;
            return url.StartsWith(UrlS3.UrlMain);
        }

        public static string ExtractKeyFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            return url.StartsWith(UrlS3.UrlMain) ? url.Replace(UrlS3.UrlMain, "") : null;
        }

        public static string GenerateOrderId()
        {
            DateTime now = DateTime.UtcNow;
            string orderId = now.ToString("yyMMddHH");
            string chars = Utils.GenRandomCharacter(5);

            return $"{orderId}{chars}";
        }

    }
}
