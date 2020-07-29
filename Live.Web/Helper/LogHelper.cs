using System;
using System.IO;
using System.Text;

namespace Live.Web.Helper
{
    public class LogHelper
    {
        public static void Write(string message)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}/logs";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var file = new FileStream($"{path}/{date}.log", FileMode.Append, FileAccess.Write))
            {
                var bytes = Encoding.Default.GetBytes($"[{DateTime.Now:yyyy-MM-dd HH:mm:dd}]: {message}\r\n");
                file.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
