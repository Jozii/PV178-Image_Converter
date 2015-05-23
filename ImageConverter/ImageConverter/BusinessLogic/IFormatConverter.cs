using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface IFormatConverter
    {
        //int Convert(string file, Format outputFormat, string outputFileName, int compression = 100, bool overwriteOutput = false);
        IEnumerable<string> Convert(IEnumerable<string> files, Format outputFormat, string outputFileName,
            int compression = 100, bool overwriteOutput = false, BackgroundWorker bw = null);
    }
}
