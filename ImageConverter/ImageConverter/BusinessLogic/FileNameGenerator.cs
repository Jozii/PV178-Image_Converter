using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConverter.BusinessLogic
{
    public static class FileNameGenerator
    {
        public static string UniqueFileName(string fileName, ref int i)
        {
            string directory = Path.GetDirectoryName(fileName);
            string fileNameWithoutExtension = directory + "\\" + Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            while (File.Exists(fileNameWithoutExtension + i + extension) && i != Int32.MaxValue - 1)
            {
                i++;
                if (i == Int32.MaxValue)
                {
                    i = 0;
                    fileNameWithoutExtension += "img";
                }
            }
            return fileNameWithoutExtension + i + extension;
        }
    }
}
