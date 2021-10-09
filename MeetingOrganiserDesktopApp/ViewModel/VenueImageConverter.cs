using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MeetingOrganiserDesktopApp.ViewModel
{
    public class VenueImageConverter : IValueConverter
    {
        public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (!(value is Byte[]))
                return Binding.DoNothing;

            try
            {
                using (MemoryStream stream = new MemoryStream(value as Byte[]))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    return image;
                }
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
