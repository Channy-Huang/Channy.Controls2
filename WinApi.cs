using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Channy.Controls2 {
    internal static class WinApi {
        public enum Sizing {
            WMSZ_BOTTOM = 6,
            WMSZ_BOTTOMLEFT = 7,
            WMSZ_BOTTOMRIGHT = 8,
            WMSZ_LEFT = 1,
            WMSZ_RIGHT = 2,
            WMSZ_TOP = 3,
            WMSZ_TOPLEFT = 4,
            WMSZ_TOPRIGHT = 5
        }

        public enum HitTest {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21,
        }

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);

        internal static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero) {
                var monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// 
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #region Nested type: MINMAXINFO

        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        #endregion

        #region Nested type: MONITORINFO

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MONITORINFO {
            /// <summary>
            /// </summary>            
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));

            /// <summary>
            /// </summary>            
            public RECT rcMonitor;

            /// <summary>
            /// </summary>            
            public RECT rcWork;

            /// <summary>
            /// </summary>            
            public int dwFlags;
        }

        #endregion

        #region Nested type: POINT

        /// <summary>
        /// POINT aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT {
            /// <summary>
            /// x coordinate of point.
            /// </summary>
            public int x;

            /// <summary>
            /// y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            /// Construct a point of coordinates (x,y).
            /// </summary>
            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        #endregion

        #region Nested type: RECT

        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal struct RECT {
            /// <summary> Win32 </summary>
            public int left;

            /// <summary> Win32 </summary>
            public int top;

            /// <summary> Win32 </summary>
            public int right;

            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly RECT Empty;

            /// <summary> Win32 </summary>
            public int Width {
                get { return Math.Abs(right - left); } // Abs needed for BIDI OS
            }

            /// <summary> Win32 </summary>
            public int Height {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public RECT(RECT rcSrc) {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty {
                get {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString() {
                if (this == Empty) {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom +
                       " }";
            }

            /// <summary> Determine if 2 RECT are equal (deep compare) </summary>
            public override bool Equals(object obj) {
                if (!(obj is RECT)) {
                    return false;
                }
                return (this == (RECT)obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 RECT are equal (deep compare)</summary>
            public static bool operator ==(RECT rect1, RECT rect2) {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right &&
                        rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 RECT are different(deep compare)</summary>
            public static bool operator !=(RECT rect1, RECT rect2) {
                return !(rect1 == rect2);
            }
        }

        #endregion

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern bool HideCaret(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ShowCaret")]
        public static extern bool ShowCaret(IntPtr hWnd);

        [DllImportAttribute("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        internal static extern bool ReleaseCapture();

        public const int CS_DropSHADOW = 0x20000;
        public const int GCL_STYLE = -26;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowRgn(IntPtr hwnd, IntPtr hRgn, bool bRedraw);

        [DllImportAttribute("gdi32.dll")]
        public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [DllImport("gdi32", EntryPoint = "CombineRgn")]
        public static extern int CombineRgn(IntPtr hDestRgn, IntPtr hSrcRgn1, IntPtr hSrcRgn2, int nCombineMode);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [DllImport("kernel32.dll")]
        static extern ulong VerSetConditionMask(ulong dwlConditionMask, uint dwTypeBitMask, byte dwConditionMask);

        [DllImport("kernel32.dll")]
        static extern bool VerifyVersionInfo([In] ref OsVersionInfoEx lpVersionInfo, uint dwTypeMask, ulong dwlConditionMask);

        [StructLayout(LayoutKind.Sequential)]
        struct OsVersionInfoEx {
            public uint OSVersionInfoSize;
            public uint MajorVersion;
            public uint MinorVersion;
            public uint BuildNumber;
            public uint PlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string CSDVersion;
            public ushort ServicePackMajor;
            public ushort ServicePackMinor;
            public ushort SuiteMask;
            public byte ProductType;
            public byte Reserved;
        }

        /// <summary>
        /// Add manifest file to your application and uncomment the following to make it work for Windows 8 and later.
        /// <!-- Windows Vista -->
        /// <supportedOS Id = "{e2011457-1546-43c5-a5fe-008deee3d3f0}" />
        /// <!--Windows 7 -->
        /// <supportedOS Id = "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" />
        ///
        /// <!--Windows 8 -->
        /// <supportedOS Id = "{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" />
        ///
        /// <!--Windows 8.1 -->
        /// <supportedOS Id = "{1f676c76-80e1-4239-95bb-83d0f6d0da78}" />
        ///
        /// <!--Windows 10 -->
        /// <supportedOS Id = "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
        /// </summary>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <param name="servicePackMajor"></param>
        /// <returns></returns>
        public static bool IsWindowsVersionOrGreater(uint majorVersion, uint minorVersion, ushort servicePackMajor) {
            OsVersionInfoEx osvi = new OsVersionInfoEx();
            osvi.OSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
            osvi.MajorVersion = majorVersion;
            osvi.MinorVersion = minorVersion;
            osvi.ServicePackMajor = servicePackMajor;
            // These constants initialized with corresponding definitions in
            // winnt.h (part of Windows SDK)
            const uint VER_MINORVERSION = 0x0000001;
            const uint VER_MAJORVERSION = 0x0000002;
            const uint VER_SERVICEPACKMAJOR = 0x0000020;
            const byte VER_GREATER_EQUAL = 3;
            ulong versionOrGreaterMask = VerSetConditionMask(VerSetConditionMask(VerSetConditionMask(0, VER_MAJORVERSION, VER_GREATER_EQUAL), VER_MINORVERSION, VER_GREATER_EQUAL), VER_SERVICEPACKMAJOR, VER_GREATER_EQUAL);
            uint versionOrGreaterTypeMask = VER_MAJORVERSION | VER_MINORVERSION | VER_SERVICEPACKMAJOR;
            return VerifyVersionInfo(ref osvi, versionOrGreaterTypeMask, versionOrGreaterMask);
        }

        /// <summary>
        /// Add manifest file to your application and uncomment the following to make it work for Windows 8 and later.
        /// <!-- Windows Vista -->
        /// <supportedOS Id = "{e2011457-1546-43c5-a5fe-008deee3d3f0}" />
        /// <!--Windows 7 -->
        /// <supportedOS Id = "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" />
        ///
        /// <!--Windows 8 -->
        /// <supportedOS Id = "{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" />
        ///
        /// <!--Windows 8.1 -->
        /// <supportedOS Id = "{1f676c76-80e1-4239-95bb-83d0f6d0da78}" />
        ///
        /// <!--Windows 10 -->
        /// <supportedOS Id = "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
        /// </summary>
        public static bool IsWindowsVistaOrGreater => IsWindowsVersionOrGreater(6, 0, 0);

        /// <summary>
        /// Add manifest file to your application and uncomment the following to make it work for Windows 8 and later.
        /// <!-- Windows Vista -->
        /// <supportedOS Id = "{e2011457-1546-43c5-a5fe-008deee3d3f0}" />
        /// <!--Windows 7 -->
        /// <supportedOS Id = "{35138b9a-5d96-4fbd-8e2d-a2440225f93a}" />
        ///
        /// <!--Windows 8 -->
        /// <supportedOS Id = "{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}" />
        ///
        /// <!--Windows 8.1 -->
        /// <supportedOS Id = "{1f676c76-80e1-4239-95bb-83d0f6d0da78}" />
        ///
        /// <!--Windows 10 -->
        /// <supportedOS Id = "{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
        /// </summary>
        public static bool IsWindows10OrGreater => IsWindowsVersionOrGreater(10, 0, 0);

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowCompositionAttributeData {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        public enum WindowCompositionAttribute {
            WCA_ACCENT_POLICY = 19
        }

        public enum AccentState {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AccentPolicy {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        internal const UInt32 SWP_NOSIZE = 0x0001;
        internal const UInt32 SWP_NOMOVE = 0x0002;
        internal const UInt32 SWP_SHOWWINDOW = 0x0040;

        internal static int RGN_AND = 1;
        internal static int RGN_OR = 2;
        internal static int RGN_XOR = 3;
        internal static int RGN_DIFF = 4;
        internal static int RGN_COPY = 5;
        internal static int RGN_MIN = RGN_AND;
        internal static int RGN_MAX = RGN_COPY;

        //[StructLayout(LayoutKind.Sequential)]
        //internal struct POINT
        //{
        //    internal int X;
        //    internal int Y;

        //    internal POINT(int x, int y)
        //    {
        //        this.X = x;
        //        this.Y = y;
        //    }
        //}

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateSolidBrush(uint color);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        internal static extern int FrameRgn(IntPtr hDC, IntPtr hRgn, IntPtr hBrush, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr target);

        [DllImport("user32.dll")]
        internal extern static bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, uint dwFlags);

        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false,
            SetLastError = true)]
        internal static extern void DwmIsCompositionEnabled(out bool pfEnable);

        /// <summary>
        /// Extend the window frame into the client area.
        /// </summary>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false,
            SetLastError = true)]
        internal static extern void DwmExtendFrameIntoClientArea(
            IntPtr hWnd,
            [In] ref MARGINS margins);

        /// <summary>
        /// Enable the blur effect on a specified window.
        /// </summary>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false, SetLastError = true)]
        internal static extern void DwmEnableBlurBehindWindow(IntPtr hWnd, DWM_BLURBEHIND pBlurBehind);

        /// <summary>
        /// The point of MARGINS structure that describes the margins to use when
        /// extending the frame into the client area.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MARGINS {
            // Width of the left border that retains its size.
            public int cxLeftWidth;

            // Width of the right border that retains its size.
            public int cxRightWidth;

            // Height of the top border that retains its size.
            public int cyTopHeight;

            // Height of the bottom border that retains its size.
            public int cyBottomHeight;

            public MARGINS(int margin) {
                cxLeftWidth = margin;
                cxRightWidth = margin;
                cyTopHeight = margin;
                cyBottomHeight = margin;
            }

            public MARGINS(int leftWidth, int rightWidth,
                int topHeight, int bottomHeight) {
                cxLeftWidth = leftWidth;
                cxRightWidth = rightWidth;
                cyTopHeight = topHeight;
                cyBottomHeight = bottomHeight;
            }

            /// <summary>
            /// Determine whether there is a negative value, or the value is valid
            /// for a Form.
            /// </summary>
            public bool IsNegativeOrOverride(System.Drawing.Size formClientSize) {
                return cxLeftWidth < 0
                    || cxRightWidth < 0
                    || cyBottomHeight < 0
                    || cyTopHeight < 0
                    || (cxLeftWidth + cxRightWidth) > formClientSize.Width
                    || (cyTopHeight + cyBottomHeight) > formClientSize.Height;
            }
        }

        /// <summary>
        /// Specify Desktop Window Manager (DWM) blur-behind properties. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DWM_BLURBEHIND {
            // Indicate the members of this structure have been set.
            public uint dwFlags;

            // The flag specify  whether the subsequent compositions of the window
            // blurring the content behind it or not.
            [MarshalAs(UnmanagedType.Bool)]
            public bool fEnable;

            // The region where the glass style will be applied.
            public IntPtr hRegionBlur;

            // Whether the windows color should be transited to match the maximized 
            // windows or not.
            [MarshalAs(UnmanagedType.Bool)]
            public bool fTransitionOnMaximized;

            // Flags used to indicate the  members contain valid information.
            public const uint DWM_BB_ENABLE = 0x00000001;
            public const uint DWM_BB_BLURREGION = 0x00000002;
            public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
        }

        internal struct Point {
            public int x;

            public int y;


            public Point(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        internal struct Size {
            public int cx;

            public int cy;


            public Size(int cx, int cy) {
                this.cx = cx;
                this.cy = cy;
            }
        }

        internal struct BLENDFUNCTION {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(byte blendOp, byte blendFlags, byte sourceConstantAlpha, byte alphaFormat) {
                this.BlendOp = blendOp;
                this.BlendFlags = blendFlags;
                this.SourceConstantAlpha = sourceConstantAlpha;
                this.AlphaFormat = alphaFormat;
            }
        }

        internal const int ULW_ALPHA = 2;

        internal const byte AC_SRC_OVER = 0;

        internal const byte AC_SRC_ALPHA = 1;

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImportAttribute("user32.dll")]
        internal extern static IntPtr GetDC(IntPtr handle);

        [DllImportAttribute("user32.dll", ExactSpelling = true)]
        internal extern static int ReleaseDC(IntPtr handle, IntPtr hDC);

        [DllImportAttribute("gdi32.dll")]
        internal extern static IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImportAttribute("gdi32.dll")]
        internal extern static bool DeleteDC(IntPtr hdc);

        [DllImportAttribute("gdi32.dll")]
        internal extern static IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ChangeWindowMessageFilter(int message, int flags);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetIconicThumbnail(
            IntPtr hwnd, IntPtr hbitmap, uint flags);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetIconicLivePreviewBitmap(
            IntPtr hwnd,
            IntPtr hbitmap,
            ref POINT ptClient,
            uint flags);
        [DllImport("dwmapi.dll")]
        internal static extern int DwmSetIconicLivePreviewBitmap(
            IntPtr hwnd, IntPtr hbitmap, IntPtr ptClient, uint flags);

        [DllImport("dwmapi.dll")]
        internal static extern int DwmInvalidateIconicBitmaps(IntPtr hwnd);

        internal enum DWMWINDOWATTRIBUTE {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY = 2,
            DWMWA_TRANSITIONS_FORCEDISABLED = 3,
            DWMWA_ALLOW_NCPAINT = 4,
            DWMWA_CAPTION_BUTTON_BOUNDS = 5,
            DWMWA_NONCLIENT_RTL_LAYOUT = 6,
            DWMWA_FORCE_ICONIC_REPRESENTATION = 7,
            DWMWA_FLIP3D_POLICY = 8,
            DWMWA_EXTENDED_FRAME_BOUNDS = 9,
            DWMWA_HAS_ICONIC_BITMAP = 10,
            DWMWA_DISALLOW_PEEK = 11,
            DWMWA_EXCLUDED_FROM_PEEK = 12,
            DWMWA_CLOAK = 13,
            DWMWA_CLOAKED = 14,
            DWMWA_FREEZE_REPRESENTATION = 15,
            DWMWA_LAST = 16
        };

        [DllImport("DwmApi.dll")]
        internal static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            //DWMWA_* values.
            DWMWINDOWATTRIBUTE dwAttribute,
            IntPtr pvAttributeValue,
            uint cbAttribute);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StretchBlt(
            IntPtr hDestDC, int destX, int destY, int destWidth, int destHeight,
            IntPtr hSrcDC, int srcX, int srcY, int srcWidth, int srcHeight,
            uint operation);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClientToScreen(
            IntPtr hwnd,
            ref POINT point);

        //[StructLayout(LayoutKind.Sequential)]
        //internal struct RECT
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;

        //    public RECT(int left, int top, int right, int bottom)
        //    {
        //        this.left = left;
        //        this.top = top;
        //        this.right = right;
        //        this.bottom = bottom;
        //    }
        //}

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hwnd, ref RECT rect);

        internal const int GWL_EXSTYLE = -20;
        internal const int WS_EX_LAYERED = 0x80000;
        internal static uint LWA_ALPHA = 0x02;
        internal static uint LWA_COLORKEY = 0x01;

        internal static int WM_NCLBUTTONDOWN = 0xA1;
        internal static int HT_CAPTION = 0x2;

        internal const int WS_CHILDWINDOW = 0x40000000;
        internal const int WS_EX_APPWINDOW = 0x40000;
        internal const int WS_EX_TOOLWINDOW = 0x00000080;
        internal const int GWL_STYLE = -16;

        [StructLayout(LayoutKind.Sequential)]
        public struct COMBOBOXINFO {
            public Int32 cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public ComboBoxButtonState buttonState;
            public IntPtr hwndCombo;
            public IntPtr hwndEdit;
            public IntPtr hwndList;
        }

        [DllImport("user32.dll")]
        public static extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO pcbi);

        public enum ComboBoxButtonState {
            STATE_SYSTEM_NONE = 0,
            STATE_SYSTEM_INVISIBLE = 0x00008000,
            STATE_SYSTEM_PRESSED = 0x00000008
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr BeginPaint(IntPtr hwnd, ref PAINTSTRUCT lpPaint);

        [StructLayout(LayoutKind.Sequential)]
        internal struct PAINTSTRUCT {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        internal enum TBPFLAG {
            TBPF_NOPROGRESS = 0,
            TBPF_INDETERMINATE = 0x1,
            TBPF_NORMAL = 0x2,
            TBPF_ERROR = 0x4,
            TBPF_PAUSED = 0x8
        }

        internal enum TBATFLAG {
            TBATF_USEMDITHUMBNAIL = 0x1,
            TBATF_USEMDILIVEPREVIEW = 0x2
        }

        internal enum THBMASK {
            THB_BITMAP = 0x1,
            THB_ICON = 0x2,
            THB_TOOLTIP = 0x4,
            THB_FLAGS = 0x8
        }

        internal enum THBFLAGS {
            THBF_ENABLED = 0,
            THBF_DISABLED = 0x1,
            THBF_DISMISSONCLICK = 0x2,
            THBF_NOBACKGROUND = 0x4,
            THBF_HIDDEN = 0x8
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct THUMBBUTTON {
            [MarshalAs(UnmanagedType.U4)]
            public THBMASK dwMask;
            public uint iId;
            public uint iBitmap;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szTip;
            [MarshalAs(UnmanagedType.U4)]
            public THBFLAGS dwFlags;
        }

        [ComImportAttribute()]
        [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ITaskbarList3 {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags);
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            void UnregisterTab(IntPtr hwndTab);
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, TBATFLAG tbatFlags);
            void ThumbBarAddButtons(
                IntPtr hwnd,
                uint cButtons,
                [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
            void ThumbBarUpdateButtons(
                IntPtr hwnd,
                uint cButtons,
                [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            void SetOverlayIcon(
              IntPtr hwnd,
              IntPtr hIcon,
              [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
            void SetThumbnailTooltip(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
            void SetThumbnailClip(
                IntPtr hwnd,
                /*[MarshalAs(UnmanagedType.LPStruct)]*/ ref RECT prcClip);
        }

        private static ITaskbarList3 taskbarList;
        internal static ITaskbarList3 TaskbarManager {
            get {
                if (taskbarList == null) {
                    lock (typeof(WinApi)) {
                        if (taskbarList == null) {
                            taskbarList = (ITaskbarList3)new CTaskbarList();
                            taskbarList.HrInit();
                        }
                    }
                }
                return taskbarList;
            }
        }

        [GuidAttribute("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterfaceAttribute(ClassInterfaceType.None)]
        [ComImportAttribute()]
        internal class CTaskbarList { }

        internal enum Messages {
            WM_NULL = 0x00,
            WM_CREATE = 0x01,
            WM_DESTROY = 0x02,
            WM_MOVE = 0x03,
            WM_SIZE = 0x05,
            WM_ACTIVATE = 0x06,
            WM_SETFOCUS = 0x07,
            WM_KILLFOCUS = 0x08,
            WM_ENABLE = 0x0A,
            WM_SETREDRAW = 0x0B,
            WM_SETTEXT = 0x0C,
            WM_GETTEXT = 0x0D,
            WM_GETTEXTLENGTH = 0x0E,
            WM_PAINT = 0x0F,
            WM_CLOSE = 0x10,
            WM_QUERYENDSESSION = 0x11,
            WM_QUIT = 0x12,
            WM_QUERYOPEN = 0x13,
            WM_ERASEBKGND = 0x14,
            WM_SYSCOLORCHANGE = 0x15,
            WM_ENDSESSION = 0x16,
            WM_SYSTEMERROR = 0x17,
            WM_SHOWWINDOW = 0x18,
            WM_CTLCOLOR = 0x19,
            WM_WININICHANGE = 0x1A,
            WM_SETTINGCHANGE = 0x1A,
            WM_DEVMODECHANGE = 0x1B,
            WM_ACTIVATEAPP = 0x1C,
            WM_FONTCHANGE = 0x1D,
            WM_TIMECHANGE = 0x1E,
            WM_CANCELMODE = 0x1F,
            WM_SETCURSOR = 0x20,
            WM_MOUSEACTIVATE = 0x21,
            WM_CHILDACTIVATE = 0x22,
            WM_QUEUESYNC = 0x23,
            WM_GETMINMAXINFO = 0x24,
            WM_PAINTICON = 0x26,
            WM_ICONERASEBKGND = 0x27,
            WM_NEXTDLGCTL = 0x28,
            WM_SPOOLERSTATUS = 0x2A,
            WM_DRAWITEM = 0x2B,
            WM_MEASUREITEM = 0x2C,
            WM_DELETEITEM = 0x2D,
            WM_VKEYTOITEM = 0x2E,
            WM_CHARTOITEM = 0x2F,

            WM_SETFONT = 0x30,
            WM_GETFONT = 0x31,
            WM_SETHOTKEY = 0x32,
            WM_GETHOTKEY = 0x33,
            WM_QUERYDRAGICON = 0x37,
            WM_COMPAREITEM = 0x39,
            WM_COMPACTING = 0x41,
            WM_WINDOWPOSCHANGING = 0x46,
            WM_WINDOWPOSCHANGED = 0x47,
            WM_POWER = 0x48,
            WM_COPYDATA = 0x4A,
            WM_CANCELJOURNAL = 0x4B,
            WM_NOTIFY = 0x4E,
            WM_INPUTLANGCHANGEREQUEST = 0x50,
            WM_INPUTLANGCHANGE = 0x51,
            WM_TCARD = 0x52,
            WM_HELP = 0x53,
            WM_USERCHANGED = 0x54,
            WM_NOTIFYFORMAT = 0x55,
            WM_CONTEXTMENU = 0x7B,
            WM_STYLECHANGING = 0x7C,
            WM_STYLECHANGED = 0x7D,
            WM_DISPLAYCHANGE = 0x7E,
            WM_GETICON = 0x7F,
            WM_SETICON = 0x80,

            WM_NCCREATE = 0x81,
            WM_NCDESTROY = 0x82,
            WM_NCCALCSIZE = 0x83,
            WM_NCHITTEST = 0x84,
            WM_NCPAINT = 0x85,
            WM_NCACTIVATE = 0x86,
            WM_GETDLGCODE = 0x87,
            WM_NCMOUSEMOVE = 0xA0,
            WM_NCLBUTTONDOWN = 0xA1,
            WM_NCLBUTTONUP = 0xA2,
            WM_NCLBUTTONDBLCLK = 0xA3,
            WM_NCRBUTTONDOWN = 0xA4,
            WM_NCRBUTTONUP = 0xA5,
            WM_NCRBUTTONDBLCLK = 0xA6,
            WM_NCMBUTTONDOWN = 0xA7,
            WM_NCMBUTTONUP = 0xA8,
            WM_NCMBUTTONDBLCLK = 0xA9,

            WM_KEYFIRST = 0x100,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
            WM_DEADCHAR = 0x103,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_SYSCHAR = 0x106,
            WM_SYSDEADCHAR = 0x107,
            WM_KEYLAST = 0x108,

            WM_IME_STARTCOMPOSITION = 0x10D,
            WM_IME_ENDCOMPOSITION = 0x10E,
            WM_IME_COMPOSITION = 0x10F,
            WM_IME_KEYLAST = 0x10F,

            WM_INITDIALOG = 0x110,
            WM_COMMAND = 0x111,
            WM_SYSCOMMAND = 0x112,
            WM_TIMER = 0x113,
            WM_HSCROLL = 0x114,
            WM_VSCROLL = 0x115,
            WM_INITMENU = 0x116,
            WM_INITMENUPOPUP = 0x117,
            WM_MENUSELECT = 0x11F,
            WM_MENUCHAR = 0x120,
            WM_ENTERIDLE = 0x121,

            WM_CTLCOLORMSGBOX = 0x132,
            WM_CTLCOLOREDIT = 0x133,
            WM_CTLCOLORLISTBOX = 0x134,
            WM_CTLCOLORBTN = 0x135,
            WM_CTLCOLORDLG = 0x136,
            WM_CTLCOLORSCROLLBAR = 0x137,
            WM_CTLCOLORSTATIC = 0x138,

            WM_MOUSEFIRST = 0x200,
            WM_MOUSEMOVE = 0x200,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_LBUTTONDBLCLK = 0x203,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RBUTTONDBLCLK = 0x206,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOUSEHWHEEL = 0x20E,

            WM_PARENTNOTIFY = 0x210,
            WM_ENTERMENULOOP = 0x211,
            WM_EXITMENULOOP = 0x212,
            WM_NEXTMENU = 0x213,
            WM_SIZING = 0x214,
            WM_CAPTURECHANGED = 0x215,
            WM_MOVING = 0x216,
            WM_POWERBROADCAST = 0x218,
            WM_DEVICECHANGE = 0x219,

            WM_MDICREATE = 0x220,
            WM_MDIDESTROY = 0x221,
            WM_MDIACTIVATE = 0x222,
            WM_MDIRESTORE = 0x223,
            WM_MDINEXT = 0x224,
            WM_MDIMAXIMIZE = 0x225,
            WM_MDITILE = 0x226,
            WM_MDICASCADE = 0x227,
            WM_MDIICONARRANGE = 0x228,
            WM_MDIGETACTIVE = 0x229,
            WM_MDISETMENU = 0x230,
            WM_ENTERSIZEMOVE = 0x231,
            WM_EXITSIZEMOVE = 0x232,
            WM_DROPFILES = 0x233,
            WM_MDIREFRESHMENU = 0x234,

            WM_IME_SETCONTEXT = 0x281,
            WM_IME_NOTIFY = 0x282,
            WM_IME_CONTROL = 0x283,
            WM_IME_COMPOSITIONFULL = 0x284,
            WM_IME_SELECT = 0x285,
            WM_IME_CHAR = 0x286,
            WM_IME_KEYDOWN = 0x290,
            WM_IME_KEYUP = 0x291,

            WM_MOUSEHOVER = 0x2A1,
            WM_NCMOUSELEAVE = 0x2A2,
            WM_MOUSELEAVE = 0x2A3,

            WM_CUT = 0x300,
            WM_COPY = 0x301,
            WM_PASTE = 0x302,
            WM_CLEAR = 0x303,
            WM_UNDO = 0x304,

            WM_RENDERFORMAT = 0x305,
            WM_RENDERALLFORMATS = 0x306,
            WM_DESTROYCLIPBOARD = 0x307,
            WM_DRAWCLIPBOARD = 0x308,
            WM_PAINTCLIPBOARD = 0x309,
            WM_VSCROLLCLIPBOARD = 0x30A,
            WM_SIZECLIPBOARD = 0x30B,
            WM_ASKCBFORMATNAME = 0x30C,
            WM_CHANGECBCHAIN = 0x30D,
            WM_HSCROLLCLIPBOARD = 0x30E,
            WM_QUERYNEWPALETTE = 0x30F,
            WM_PALETTEISCHANGING = 0x310,
            WM_PALETTECHANGED = 0x311,

            WM_HOTKEY = 0x312,
            WM_PRINT = 0x317,
            WM_PRINTCLIENT = 0x318,

            WM_HANDHELDFIRST = 0x358,
            WM_HANDHELDLAST = 0x35F,
            WM_PENWINFIRST = 0x380,
            WM_PENWINLAST = 0x38F,
            WM_COALESCE_FIRST = 0x390,
            WM_COALESCE_LAST = 0x39F,
            WM_DDE_FIRST = 0x3E0,
            WM_DDE_INITIATE = 0x3E0,
            WM_DDE_TERMINATE = 0x3E1,
            WM_DDE_ADVISE = 0x3E2,
            WM_DDE_UNADVISE = 0x3E3,
            WM_DDE_ACK = 0x3E4,
            WM_DDE_DATA = 0x3E5,
            WM_DDE_REQUEST = 0x3E6,
            WM_DDE_POKE = 0x3E7,
            WM_DDE_EXECUTE = 0x3E8,
            WM_DDE_LAST = 0x3E8,

            WM_USER = 0x400,
            WM_APP = 0x8000,

            WM_DWMCOMPOSITIONCHANGED = 0x31e,
            WM_DWMSENDICONICTHUMBNAIL = 0x0323,
            WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x326,

            SC_RESTORE = 0xf120,
            SC_CLOSE = 0xf060,

            CHANFUN_SET_PROGRESS = 0x1111,
            CHANFUN_SET_OVERLAY_ICON = 0X1112,
        }
    }
}
