using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageConverter.BusinessLogic
{
    public interface IBitmapSourceLoader
    {
        BitmapSource Load(string file);
    }
}
