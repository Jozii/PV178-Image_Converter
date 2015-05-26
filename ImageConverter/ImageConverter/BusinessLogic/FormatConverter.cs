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
        private readonly IFormatEncoder _encoder;
        public FormatConverter(IBitmapSourceLoader loader, IXMLLog log, IFormatEncoder encoder)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (log == null) 
                throw new ArgumentNullException("log");
            if (encoder == null)
                throw new ArgumentNullException("encoder");
            _loader = loader;
            _log = log;
            _encoder = encoder;
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
                        _encoder.EncodeIntoJPEG(outputFileName, source, compression);
                        break;
                    case Format.GIF:
                        _encoder.EncodeIntoGIF(outputFileName, source);
                        break;
                    case Format.PNG:
                        _encoder.EncodeIntoPNG(outputFileName, source);
                        break;
                    case Format.Tiff:
                        _encoder.EncodeIntoTiff(outputFileName, source);
                        break;
                    case Format.BMP:
                        _encoder.EncodeIntoBMP(outputFileName, source);
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
                string tempFileName;
                if (overwriteOutput)
                {
                    tempFileName = FileNameGenerator.GetFileName(outputFileName, ref i);
                }
                else
                {
                    tempFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                }
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

       
    }
}
