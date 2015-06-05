using System.Collections.Generic;
using System.ComponentModel;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface IFormatConverter
    {
        IEnumerable<string> Convert(IEnumerable<string> files, Format outputFormat, string outputFileName,
            int compression = 100, bool overwriteOutput = false, BackgroundWorker bw = null);
    }
}
