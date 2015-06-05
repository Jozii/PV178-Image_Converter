using System.Collections.Generic;
using System.ComponentModel;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public interface ISizeConverter
    {
        bool Resize(string file, int width, int height, string outputFileName, KeepAspectRatio ratio,
            bool enlargeSmallerImages, Format outputFormat);
    }
}
