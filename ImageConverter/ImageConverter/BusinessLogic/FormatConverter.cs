using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    public class FormatConverter : IFormatConverter
    {
        private readonly IBitmapSourceLoader _loader;
        private IXMLLog _log;

        public FormatConverter(IBitmapSourceLoader loader, IXMLLog log)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (log == null) 
                throw new ArgumentNullException("log");
            _loader = loader;
            _log = log;
        }

        private int Convert(string file, Format outputFormat, string outputFileName, int compression = 100, bool overwriteOutput = false)
        {
            if (outputFileName == null) 
                throw new ArgumentNullException("outputFileName");
            if (file == null)
                throw new ArgumentNullException("file");
            if (compression < 0 || compression > 100)
                throw new FormatException("compression");
            try
            {
                BitmapSource source = _loader.Load(file);
                switch (outputFormat)
                {
                    case Format.JPEG:
                        EncodeIntoJPEG(outputFileName, source, compression);
                        break;
                    case Format.GIF:
                        EncodeIntoGIF(outputFileName, source);
                        break;
                    case Format.PNG:
                        EncodeIntoPNG(outputFileName, source);
                        break;
                    case Format.Tiff:
                        EncodeIntoTiff(outputFileName, source);
                        break;
                    case Format.BMP:
                        EncodeIntoBMP(outputFileName, source);
                        break;
                    default:
                        throw new FormatException("outputFormat");
                }
                return 0;
            }
            catch (FileNotFoundException)
            {
                return 1;
            }
            catch (NotSupportedException)
            {
                // file is not an image
                return 2;
            }
        }
        
        public IEnumerable<string> Convert(IEnumerable<string> files, Format outputFormat, string outputFileName,
            int compression = 100, bool overwriteOutput = false, BackgroundWorker bw = null)
        {
            if (files == null) 
                throw new ArgumentNullException("files");
            if (outputFileName == null)
                throw new ArgumentNullException("outputFileName");
            List<string> list = new List<string>();
            int i = 0;
            int max = files.Count();
            if (max == 1)
            {
                if (!overwriteOutput)
                {
                    if (File.Exists(outputFileName))
                        outputFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                }
                if (Convert(files.First(), outputFormat, outputFileName, compression) != 0)
                {
                    list.Add(files.First());
                }
                if (bw != null)
                {
                    bw.ReportProgress(100,files.First());
                }
                return list;
            }
            int currentFile = 0;
            foreach (string file in files)
            {
                string tempFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                if (Convert(file, outputFormat, tempFileName, compression) != 0)
                {
                    list.Add(file);
                }
                currentFile++;
                if (bw != null)
                {
                    int report = (int) ((double)currentFile / max * 100.00);
                    bw.ReportProgress(report,file);
                }
            }
            return list;
        }

        private void EncodeIntoJPEG(string outputFile, BitmapSource source, int compression)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile,FileMode.Create))
            {
                encoder.Save(fs);
            }
            if (compression != 100)
            {
                VaryQualityLevel(outputFile,compression);
            }
        }

        private void EncodeIntoPNG(string outputFile, BitmapSource source)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

        private void EncodeIntoTiff(string outputFile, BitmapSource source)
        {
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

        private void EncodeIntoGIF(string outputFile, BitmapSource source)
        {
            GifBitmapEncoder encoder = new GifBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }

        private void EncodeIntoBMP(string outputFile, BitmapSource source)
        {
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using (FileStream fs = new FileStream(outputFile, FileMode.Create))
            {
                encoder.Save(fs);
            }
        }
        private static void VaryQualityLevel(string file, int compression)
        {
            Bitmap bmp = new Bitmap(file);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;
            
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, compression);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp.Save(file, jpgEncoder, myEncoderParameters);
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
