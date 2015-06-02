using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    class SizeConverter : ISizeConverter
    {
        private readonly IBitmapSourceLoader _loader;
        private readonly IXMLLog _log;
        private readonly IFormatEncoder _encoder;
        public SizeConverter(IBitmapSourceLoader loader, IXMLLog log, IFormatEncoder encoder)
        {
            if (log == null)
                throw new ArgumentNullException("log");
            _log = log;
            if (loader == null)
            {
                _log.Error("SizeConverter: loader is null");
                throw new ArgumentNullException("loader");
            }
            if (encoder == null)
            {
                _log.Error("SizeConverter: encoder is null");
                throw new ArgumentNullException("encoder");
            }
            _loader = loader;
            _encoder = encoder;
        }
        private bool Resize(string file, int width, int height, string outputFileName, KeepAspectRatio ratio, bool enlargeSmallerImages)
        {
            if (outputFileName == null)
            {
                _log.Error("Resize: outputFileName is null");
                throw new ArgumentNullException("outputFileName");
            }
            if (file == null)
            {
                _log.Error("Resize: file is null");
                throw new ArgumentNullException("file");
            }
            if (width < 0)
            {
                _log.Error("Resize: width is less than 0");
                throw new ArgumentException("width");
            }
            if (height < 0)
            {
                _log.Error("Resize: height is less than 0");
                throw new ArgumentException("height");
            }
            try
            {
                BitmapSource source = _loader.Load(file);
                int oldWidth = (int) source.Width;
                int oldHeight = (int) source.Height;
                if (!enlargeSmallerImages && (oldWidth < width || oldHeight < height))
                {
                    width = oldWidth;
                    height = oldHeight;
                }
                double aspectRatio;
                switch (ratio)
                {
                    case KeepAspectRatio.HEIGHT:
                        aspectRatio = ((double) oldWidth)/oldHeight;
                        width = (int) (height*aspectRatio);
                        break;
                    case KeepAspectRatio.WIDTH:
                        aspectRatio = ((double) oldHeight)/oldWidth;
                        height = (int) (width*aspectRatio);
                        break;
                    case KeepAspectRatio.NONE:
                        break;
                    default:
                        throw new ArgumentException("ratio");
                }
                BitmapFrame result = Resize(BitmapFrame.Create(source), width, height, BitmapScalingMode.HighQuality);
                string extension = Path.GetExtension(file);
                switch (extension)
                {
                    case ".jpeg":
                    case ".jpg":
                        _encoder.EncodeIntoJPEG(file, outputFileName, result, 100);
                        break;
                    case ".png":
                        _encoder.EncodeIntoPNG(outputFileName, result);
                        break;
                    case ".tiff":
                        _encoder.EncodeIntoTiff(outputFileName, result);
                        break;
                    case ".gif":
                        _encoder.EncodeIntoGIF(outputFileName, result);
                        break;
                    case ".bmp":
                        _encoder.EncodeIntoBMP(outputFileName, result);
                        break;
                    default:
                        return false;
                }
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return true;
            }
            catch (FileNotFoundException ex)
            {
                _log.Error("Resize: File " + file + " was not found " + ex);
                return false;
            }
            catch (NotSupportedException ex)
            {
                _log.Error("Resize: File " + file + " is not an image " + ex);
                return false;
            }
            catch (OutOfMemoryException ex)
            {
                _log.Error("Resize: Out of memory " + ex);
                return false;
            }
            catch (Exception ex)
            {
                _log.Error("Resize: " + ex);
                return false;
            }
        }

        public IEnumerable<string> Resize(IEnumerable<string> files, int width, int height, string outputFileName, KeepAspectRatio ratio,
            bool enlargeSmallerImages, bool overwriteOutput = false, BackgroundWorker bw = null)
        {
            if (files == null)
            {
                _log.Error("Resize: files are null");
                throw new ArgumentNullException("files");
            }
            if (string.IsNullOrEmpty(outputFileName))
            {
                _log.Error("Resize: outputFileName is null or empty");
                throw new ArgumentNullException("outputFileName");
            }
            if (width < 0)
            {
                _log.Error("Resize: Width is less than zero");
                throw new ArgumentException("width");
            }
            if (height < 0)
            {
                _log.Error("Resize: Height is less than zero");
                throw new ArgumentException("height");
            }
            List<string> list = new List<string>();
            int i = 0;
            int max = files.Count();
            if (max == 1)
            {
                outputFileName += Path.GetExtension(files.First());
                if (!overwriteOutput)
                {
                    if (File.Exists(outputFileName))
                        outputFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                }
                if (!Resize(files.First(),width,height,outputFileName,ratio,enlargeSmallerImages))
                {
                    list.Add(files.First());
                }
                if (bw != null)
                {
                    bw.ReportProgress(100, files.First());
                }
                return list;
            }
            int currentFile = 0;
            foreach (string file in files)
            {
                string tempFileName;
                string extension = Path.GetExtension(file);
                if (overwriteOutput)
                {
                    tempFileName = FileNameGenerator.GetFileName(outputFileName + extension, ref i);
                }
                else
                {
                    tempFileName = FileNameGenerator.UniqueFileName(outputFileName + extension, ref i);
                }
                if (!Resize(file, width, height, tempFileName, ratio, enlargeSmallerImages))
                {
                    list.Add(file);
                }
                currentFile++;
                if (bw != null)
                {
                    int report = (int)((double)currentFile / max * 100.00);
                    bw.ReportProgress(report, file);
                }
            }
            GC.WaitForPendingFinalizers();
            GC.Collect();
            return list;
        }
        private static BitmapFrame Resize(BitmapFrame photo, int width, int height,BitmapScalingMode scalingMode)
        {
            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(
                group, scalingMode);
            group.Children.Add(
                new ImageDrawing(photo,
                    new Rect(0, 0, width, height)));
            var targetVisual = new DrawingVisual();
            var targetContext = targetVisual.RenderOpen();
            targetContext.DrawDrawing(group);
            var target = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Default);
            targetContext.Close();
            target.Render(targetVisual);
            var targetFrame = BitmapFrame.Create(target);
            return targetFrame;
        }
    }
}
