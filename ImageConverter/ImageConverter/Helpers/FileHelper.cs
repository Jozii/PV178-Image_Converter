using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.Helpers
{
    public static class FileHelper
    {
        public static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }
        public static bool IsTextAllowedInFileName(string text)
        {
            IEnumerable<char> list = Path.GetInvalidFileNameChars();
            bool result = !(list.Contains(text[0]));
            return result;
        }
        public static string GetFilesToString(IEnumerable<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in list)
            {
                sb.Append(Path.GetFileName(s) + " ");
            }
            return sb.ToString();
        }

        public static string GetFilesToMessageBox(IEnumerable<string> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in list)
            {
                sb.Append(s + '\n');
            }
            return sb.ToString();
        }

        public static Format GetFormatFromFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".jpeg":
                case ".jpg":
                    return Format.JPEG;
                case ".png":
                    return Format.PNG;
                case ".tiff":
                    return Format.Tiff;
                case ".gif":
                    return Format.GIF;
                case ".bmp":
                    return Format.BMP;
                default:
                    return Format.JPEG;
            }
        }

        public static string GetExtensionFromFormat(Format format)
        {
            switch (format)
            {
                case Format.JPEG:
                    return ".jpeg";
                case Format.PNG:
                    return ".png";
                case Format.Tiff:
                    return ".tiff";
                case Format.GIF:
                    return ".gif";
                case Format.BMP:
                    return ".bmp";
                default:
                    return null;
            }
        }
    }
}
