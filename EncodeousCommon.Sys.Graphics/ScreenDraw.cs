using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using sGraphics = System.Drawing.Graphics;

namespace EncodeousCommon.Sys.Graphics
{
    public class ScreenDraw
    {
        public sGraphics GraphicsObject;
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);
        IntPtr desktopPtr = GetDC(IntPtr.Zero);

        public ScreenDraw()
        {
            GraphicsObject = sGraphics.FromHdc(desktopPtr);
        }

        ~ScreenDraw()
        {
            GraphicsObject.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }

        public void Dispose()
        {
            GraphicsObject.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }
    }
}
