using System.Collections.Generic;
using System.ComponentModel;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface IFormatConverter
    {
        bool ConvertFormat(string file, Format outputFormat, string outputFileName, int compression = 100,
            bool overwriteOutput = false);
    }
}
