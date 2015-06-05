using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using ImageConverter.BusinessLogic.Enumerations;
using ImageConverter.Helpers;
using ImageConverter.Logging;

namespace ImageConverter.BusinessLogic
{
    public class FormatConverter : IFormatConverter
    {
        private readonly IBitmapSourceLoader _loader;
        private IXMLLog _log;
        private readonly IFormatEncoder _encoder;
        public FormatConverter(IBitmapSourceLoader loader, IXMLLog log, IFormatEncoder encoder)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }
            _log = log;
            if (loader == null)
            {
                _log.Error("FormatConverter: loader is null");
                throw new ArgumentNullException("loader");
            }
            
            if (encoder == null)
            {
                _log.Error("FormatConverter: encoder is null");
                throw new ArgumentNullException("encoder");
            }
            _loader = loader;
            _encoder = encoder;
        }

        public bool ConvertFormat(string file, Format outputFormat, string outputFileName, int compression = 100, bool overwriteOutput = false)
        {
            if (outputFileName == null)
            {
                _log.Error("Convert: outputFileName is null");
                throw new ArgumentNullException("outputFileName");
            }
            if (file == null)
            {
                _log.Error("Convert: file is null");
                throw new ArgumentNullException("file");
            }
            if (compression < 0 || compression > 100)
            {
                _log.Error("Convert: compression limits");
                throw new FormatException("compression");
            }
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
                        return false;
                }
                return true;
            }
            catch (FileNotFoundException ex)
            {
                _log.Error("Convert: file " + file + "was not found " + ex);
                return false;
            }
            catch (NotSupportedException ex)
            {
                _log.Error("Convert: File " + file + " is not an image " + ex);
                return false;
            }
            catch (OutOfMemoryException ex)
            {
                _log.Error("Convert: Out of memory " + ex);
                return false;
            }
            catch (Exception ex)
            {
                _log.Error("Convert: " + ex);
                return false;
            }
        }
    }
}
