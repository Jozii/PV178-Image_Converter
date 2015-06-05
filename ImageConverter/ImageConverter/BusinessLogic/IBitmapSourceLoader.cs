using System.Windows.Media.Imaging;

namespace ImageConverter.BusinessLogic
{
    public interface IBitmapSourceLoader
    {
        BitmapSource Load(string file);
    }
}
