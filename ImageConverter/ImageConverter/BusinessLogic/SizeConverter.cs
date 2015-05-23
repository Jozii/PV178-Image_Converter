using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageConverter.BusinessLogic.Enumerations;

namespace ImageConverter.BusinessLogic
{
    class SizeConverter : ISizeConverter
    {
        private readonly IBitmapSourceLoader _loader;
        private IXMLLog _log;

        public SizeConverter(IBitmapSourceLoader loader, IXMLLog log)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (log == null)
                throw new ArgumentNullException("log");
            _loader = loader;
            _log = log;
        }
        public int Resize(string file, int width, int height, string outputFileName, KeepAspectRatio ratio, bool enlargeSmallerImages,
            bool canOverwrite = false)
        {
            if (outputFileName == null)
                throw new ArgumentNullException("outputFileName");
            if (file == null)
                throw new ArgumentNullException("file");
            if (width < 0)
                throw new ArgumentException("width");
            if (height < 0)
                throw new ArgumentException("height");
            try
            {
                BitmapSource source = _loader.Load(file);
                int oldWidth = (int) source.Width;
                int oldHeight = (int) source.Height;
                if (ratio == KeepAspectRatio.WIDTH)
                {
                    
                }
                if (ratio == KeepAspectRatio.HEIGHT)
                {
                    
                }
                BitmapFrame result = Resize(BitmapFrame.Create(source),width,height,BitmapScalingMode.HighQuality);
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

        public IEnumerable<string> Resize(IEnumerable<string> files, int width, int height, string outputFileName, KeepAspectRatio ratio,
            bool enlargeSmallerImages, bool canOverwrite = false)
        {

            return null;
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
