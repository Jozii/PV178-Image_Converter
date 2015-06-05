using System.Windows.Media.Imaging;

namespace ImageConverter.BusinessLogic
{
    public interface IFormatEncoder
    {
        void EncodeIntoJPEG(string outputFile, BitmapSource source, int compression);
        void EncodeIntoPNG(string outputFile, BitmapSource source);
        void EncodeIntoTiff(string outputFile, BitmapSource source);
        void EncodeIntoGIF(string outputFile, BitmapSource source);
        void EncodeIntoBMP(string outputFile, BitmapSource source);
    }
}
