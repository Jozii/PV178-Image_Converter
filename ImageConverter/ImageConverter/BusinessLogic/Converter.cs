using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageConverter.BusinessLogic.Enumerations;
using ImageConverter.Helpers;
using ImageConverter.Logging;

namespace ImageConverter.BusinessLogic
{
    public class Converter : IConverter
    {
        private readonly IFormatConverter _formatConverter;
        private readonly ISizeConverter _sizeConverter;
        private readonly IXMLLog _log;

        public Converter(IFormatConverter formatConverter, ISizeConverter sizeConverter, IXMLLog log)
        {
            _formatConverter = formatConverter;
            _sizeConverter = sizeConverter;
            _log = log;
        }

        public IEnumerable<string> ConvertFormat(IEnumerable<string> files, Format outputFormat, string outputFileName,
            int compression = 100, bool overwriteOutput = false, BackgroundWorker bw = null)
        {
            if (files == null)
            {
                _log.Error("Convert: Files are null");
                throw new ArgumentNullException("files");
            }
            if (outputFileName == null)
            {
                _log.Error("Convert: outputFileName is null");
                throw new ArgumentNullException("outputFileName");
            }
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
                if (!_formatConverter.ConvertFormat(files.First(), outputFormat, outputFileName, compression))
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
                if (overwriteOutput)
                {
                    tempFileName = FileNameGenerator.GetFileName(outputFileName, ref i);
                }
                else
                {
                    tempFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                }
                if (!_formatConverter.ConvertFormat(file, outputFormat, tempFileName, compression))
                {
                    list.Add(file);
                }
                currentFile++;
                if (bw != null)
                {
                    int report = (int)((double)currentFile / max * 100.00);
                    bw.ReportProgress(report, file);
                }
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            return list;
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
            Format outputFormat;
            if (max == 1)
            {
                outputFileName += Path.GetExtension(files.First());
                if (!overwriteOutput)
                {
                    if (File.Exists(outputFileName))
                        outputFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                }
                outputFormat = FileHelper.GetFormatFromFileName(outputFileName);
                if (!_sizeConverter.Resize(files.First(), width, height, outputFileName, ratio, enlargeSmallerImages, outputFormat))
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
                outputFormat = FileHelper.GetFormatFromFileName(tempFileName);
                if (!_sizeConverter.Resize(file, width, height, tempFileName, ratio, enlargeSmallerImages, outputFormat))
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

        public IEnumerable<string> ConvertFormatAndSize(IEnumerable<string> files, Format outputFormat, int width, int height, string outputFileName,
            KeepAspectRatio ratio, bool enlargeSmallerImages, int compression = 100, bool overwriteOutput = false,
            BackgroundWorker bw = null)
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
                outputFileName += FileHelper.GetExtensionFromFormat(outputFormat);
                if (!overwriteOutput)
                {
                    if (File.Exists(outputFileName))
                        outputFileName = FileNameGenerator.UniqueFileName(outputFileName, ref i);
                }
                if (!_sizeConverter.Resize(files.First(), width, height, outputFileName, ratio, enlargeSmallerImages, outputFormat))
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
                string extension = FileHelper.GetExtensionFromFormat(outputFormat);
                if (overwriteOutput)
                {
                    tempFileName = FileNameGenerator.GetFileName(outputFileName + extension, ref i);
                }
                else
                {
                    tempFileName = FileNameGenerator.UniqueFileName(outputFileName + extension, ref i);
                }
                if (!_sizeConverter.Resize(file, width, height, tempFileName, ratio, enlargeSmallerImages, outputFormat))
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
    }
}
