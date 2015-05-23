using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface ISizeConverter
    {
        int Resize(string file, int width, int height, string outputFileName, KeepAspectRatio ratio, bool enlargeSmallerImages, bool canOverwrite = false);
        IEnumerable<string> Resize(IEnumerable<string> files, int width, int height, string outputFileName, KeepAspectRatio ratio, bool enlargeSmallerImages, bool canOverwrite = false);
    }
}
