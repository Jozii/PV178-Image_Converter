using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface IConverter
    {
        IEnumerable<string> ConvertFormat(IEnumerable<string> files, Format outputFormat, string outputFileName,
            int compression = 100, bool overwriteOutput = false, BackgroundWorker bw = null);
        IEnumerable<string> Resize(IEnumerable<string> files, int width, int height, string outputFileName,KeepAspectRatio ratio,
            bool enlargeSmallerImages = false, bool overwriteOutput = false, BackgroundWorker bw = null);
        IEnumerable<string> ConvertFormatAndSize(IEnumerable<string> files, Format outputFormat, int width, int height, string outputFileName, KeepAspectRatio ratio,
            bool enlargeSmallerImages = false, int compression = 100, bool overwriteOutput = false, BackgroundWorker bw = null);
    }
}
