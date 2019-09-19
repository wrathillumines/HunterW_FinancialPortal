using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

namespace HunterW_FinancialPortal.Helpers
{
    public class UploadHelper
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                using (var image = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(image.RawFormat) ||
                        ImageFormat.Png.Equals(image.RawFormat);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}