using System.Drawing;
using System.Windows.Forms;

namespace ForensicScenarios.Tools
{
    public static class ScreenCapture
    {
        public static Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }

                return bitmap;
            }
        }
    }
}
