using Microsoft.UI.Xaml;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Exeon.Helpers
{

    public class WindowHelper
    {
        private IntPtr hWnd;
        private Size? minSize;
        private Size? maxSize;
        private SUBCLASSPROC subClassDelegate = null!;

        public WindowHelper(Window window)
        {
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        }

        public void SetMinMaxBounds(Size? minSize = null, Size? maxSize = null)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;

            subClassDelegate = new SUBCLASSPROC(WindowSubClass);
            bool bReturn = SetWindowSubclass(hWnd, subClassDelegate, 0, 0);
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_GETMINMAXINFO:
                    {
                        MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO))!;

                        if (minSize.HasValue)
                        {
                            mmi.ptMinTrackSize.X = minSize.Value.Width;
                            mmi.ptMinTrackSize.Y = minSize.Value.Height;
                        }

                        if (maxSize.HasValue)
                        {
                            mmi.ptMaxTrackSize.X = maxSize.Value.Width;
                            mmi.ptMaxTrackSize.Y = maxSize.Value.Height;
                        }

                        Marshal.StructureToPtr(mmi, lParam, false);
                        return 0;
                    }
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        public delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        public static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        public const int WM_GETMINMAXINFO = 0x0024;

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public Point ptReserved;
            public Point ptMaxSize;
            public Point ptMaxPosition;
            public Point ptMinTrackSize;
            public Point ptMaxTrackSize;
        }
    }
}
