using System.Collections.Generic;
using System.ComponentModel;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface ISizeConverter
    {
        IEnumerable<string> Resize(IEnumerable<string> files, int width, int height, string outputFileName, KeepAspectRatio ratio, bool enlargeSmallerImages, bool overwriteOutput = false, BackgroundWorker bw = null);
    }
}
