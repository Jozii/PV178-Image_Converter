using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Point = System.Drawing.Point;

namespace ImageConverter.BusinessLogic
{
    public class FormatEncoder : IFormatEncoder
    {
        public void EncodeIntoJPEG(string outputFile, BitmapSource source, int compression)
        {
            VaryQualityLevel(outputFile, source, compression);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void EncodeIntoPNG(string outputFile, BitmapSource source)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public void EncodeIntoTiff(string outputFile, BitmapSource source)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public void EncodeIntoGIF(string outputFile, BitmapSource source)
        {
            GifBitmapEncoder encoder = new GifBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public void EncodeIntoBMP(string outputFile, BitmapSource source)
        {
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        private static void VaryQualityLevel(string outputFile, BitmapSource source, int compression)
        {
            Bitmap bmp = GetBitmap(source);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            Encoder myEncoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (Int64)compression);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp.Save(outputFile, jpgEncoder, myEncoderParameters);
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
