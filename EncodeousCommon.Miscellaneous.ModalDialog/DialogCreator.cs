﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EncodeousCommon.Sys.Windows;

namespace EncodeousCommon.Miscellaneous.ModalDialog
{
    public class DialogCreator
    {
        string desktop;
        Form modaldialog;
        int brightness;
        Bitmap screenshot;
        public DialogCreator(string DesktopName, Form form, int BackgroundBrightness)
        {
            this.DesktopName = DesktopName;
            Modaldialog = form;
            Brightness = BackgroundBrightness;
        }
        public DialogCreator(string DesktopName, Form form)
        {
            this.DesktopName = DesktopName;
            Modaldialog = form;
            Brightness = 0;
        }
        public string DesktopName { get => desktop; set => desktop = value; }
        public Form Modaldialog { get => modaldialog; set => modaldialog = value; }
        public int Brightness { get => brightness; set => brightness = value; }
        public void CreateDialog()
        {
            Desktop cdsk = Desktop.DesktopOfCurrentThread();
            Desktop dsk = Desktop.CreateDesktop(DesktopName, Desktop.DESKTOP_ACCESS.GENERIC_ALL);
            
            //Screenshots the current screen.
            Rectangle rect = Screen.PrimaryScreen.Bounds;
            PixelFormat pf;
            pf = PixelFormat.Format32bppArgb;
            Bitmap BM = new Bitmap(rect.Width, rect.Height, pf);
            Graphics g = Graphics.FromImage(BM);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, rect.Size);
            //Changes the brightness of the bitmap according to the BGDarkness value.
            screenshot = SetBrightness(new Bitmap(BM), Brightness);
            var a = new Task(() =>
            {
                Desktop.SetCurrentThreadDesktop(dsk.Handle);
                MDBG mdbg = new MDBG
                {
                    TopMost = true,
                    BackgroundImage = screenshot
                };
                mdbg.Size = screenshot.Size;
                mdbg.Show();
                dsk.Show();
                modaldialog.TopMost = true;
                Window.ChangeWindowState(mdbg.Handle, Window.ShowWindowCommands.Show);
                Window.ChangeWindowState(mdbg.Handle, Window.ShowWindowCommands.Maximize);
                modaldialog.ShowDialog();
                Modaldialog.Close();

                mdbg.Close();
            });
            a.Start();
            a.Wait();

            cdsk.Show();
            dsk.Close();
            cdsk.Close();
        }
        Bitmap SetBrightness(Bitmap bitmap, int amount)
        {
            if (amount < -255 || amount > 255)
                return bitmap;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            int nVal = 0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width * 3;

                for (int y = 0; y < bitmap.Height; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nVal = (int)(p[0] + amount);

                        if (nVal < 0) nVal = 0;
                        if (nVal > 255) nVal = 255;

                        p[0] = (byte)nVal;

                        ++p;
                    }
                    p += nOffset;
                }
            }

            bitmap.UnlockBits(bmData);

            return bitmap;
        }
    }
}
