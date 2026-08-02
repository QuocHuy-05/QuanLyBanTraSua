using System.Security.Cryptography;
using System.Text;

namespace DACK_LTW_Nhom4.Helpers
{
    /// Helper hash mat khau bang MD5.
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash chuoi dau vao bang MD5, tra ve chuoi hex 32 ky tu chu thuong.
        /// Vi du: HashMD5("123456") => "e10adc3949ba59abbe56e057f20f883e"
        /// </summary>
        public static string HashMD5(string input)
        {
            if (input == null)
            {
                input = "";
            }

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// So sanh mat khau plain text voi hash da luu trong DB.
        /// </summary>
        public static bool Verify(string plainPassword, string storedHash)
        {
            if (storedHash == null)
            {
                return false;
            }

            string computed = HashMD5(plainPassword);
            return string.Equals(computed, storedHash, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
