    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface IXMLLog
    {
        void LogFormatConversion(IEnumerable<string> files , Format outputFormat, int compression);
        void LogSizeConversion(IEnumerable<string> files , int width, int height, KeepAspectRatio ratio, bool enlargeSmallerImages);
    }
}
