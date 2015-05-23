using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public class XMLLog : IXMLLog
    {
        public void LogFormatConversion(IEnumerable<string> files, Format outputFormat, int compression)
        {
            throw new NotImplementedException();
        }

        public void LogSizeConversion(IEnumerable<string> files, int width, int height, KeepAspectRatio ratio, bool enlargeSmallerImages)
        {
            throw new NotImplementedException();
        }
    }
}
