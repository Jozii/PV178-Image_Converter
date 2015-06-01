using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageConverter.BusinessLogic
{
    public class FormatEncoder : IFormatEncoder
    {
        public void EncodeIntoJPEG(string inputFile, string outputFile, BitmapSource source, int compression)
        {
            if (compression == 100)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                using (FileStream fs = new FileStream(outputFile, FileMode.Create))
                {
                    encoder.Save(fs);
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
            }else
            {
                VaryQualityLevel(inputFile, outputFile, compression);
            }
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
            GC.WaitForPendingFinalizers();
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
        private static void VaryQualityLevel(string inputFile, string outputFile, int compression)
        {
            Bitmap bmp = new Bitmap(inputFile);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

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
    }
}
