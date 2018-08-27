using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Media;

namespace Channy.Controls2 {
    static class WindowHelper {
        /// <summary>
        /// Activate a window from anywhere by attaching to the foreground window
        /// </summary>
        public static void GlobalActivate(this Window w) {
            //Get the process ID for this window's thread
            var interopHelper = new WindowInteropHelper(w);
            var thisWindowThreadId = WinApi.GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);

            //Get the process ID for the foreground window's thread
            var currentForegroundWindow = WinApi.GetForegroundWindow();
            var currentForegroundWindowThreadId = WinApi.GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);

            //Attach this window's thread to the current window's thread
            WinApi.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);

            //Set the window position
            WinApi.SetWindowPos(interopHelper.Handle, new IntPtr(0), 0, 0, 0, 0, WinApi.SWP_NOSIZE | WinApi.SWP_NOMOVE | WinApi.SWP_SHOWWINDOW);

            //Detach this window's thread from the current window's thread
            WinApi.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);

            //Show and activate the window
            if (w.WindowState == WindowState.Minimized) w.WindowState = WindowState.Normal;
            w.Show();
            w.Activate();
        }

        //public static bool IsCurrentOSContains(string name) {
        //    var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
        //    string productName = (string)reg.GetValue("ProductName");

        //    return productName.Contains(name);
        //}

        //public static bool IsWindows8() {
        //    return IsCurrentOSContains("Windows 8.0");
        //}

        ///// Check if it's Windows 8.1
        //public static bool IsWindows8Dot1() {
        //    return IsCurrentOSContains("Windows 8.1");
        //}

        ///// Check if it's Windows 10
        //public static bool IsWindows10() {
        //    return IsCurrentOSContains("Windows 10");
        //}

        public static T FindVisualChild<T>(DependencyObject parent, string childName = "")
            where T : DependencyObject {
            // Confirm parent and childName are valid. 
            if (parent == null) {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null) {
                    // recursively drill down the tree
                    foundChild = FindVisualChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                } else if (!string.IsNullOrEmpty(childName)) {
                    // If the child's name is set for search
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName) {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                } else {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (parentObject is T parent) {
                return parent;
            } else {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }
    }
}
