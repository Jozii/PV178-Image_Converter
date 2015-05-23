using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageConverter.BusinessLogic
{
    public class BitmapSourceLoader : IBitmapSourceLoader
    {
        public BitmapSource Load(string file)
        {
            return new BitmapImage(new Uri(file));
        }
    }
}
