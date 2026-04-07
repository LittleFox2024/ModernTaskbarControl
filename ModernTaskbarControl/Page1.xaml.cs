using ABI.Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernTaskbarControl;


/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Page1 : Page

{
    public const string exAdvRegKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    public const string exRegKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
    bool regLock = true;

    public Page1()

    {
        InitializeComponent();
        object iconSizeReg = null;
        object iconCombineReg = null;
        object centerTbReg = null;


        // Get registry values for taskbar settings
        try
        {
            iconCombineReg = Registry.GetValue(exAdvRegKeyPath, "TaskbarGlomLevel", null);
        } catch { Debug.WriteLine("Failure with registry."); }

        try
        {
            iconSizeReg = Registry.GetValue(exAdvRegKeyPath, "IconSizePreference", null);
        } catch { Debug.WriteLine("Failure with registry."); }

        try
        {
            centerTbReg = Registry.GetValue(exAdvRegKeyPath, "TaskbarAl", null);
            Debug.WriteLine("centerTbReg: " + centerTbReg);
        } catch { Debug.WriteLine("Failure with registry."); }


        // Registry key checks


        if (TaskbarControl.getAutoHide())
        {
            autoHideTbCheck.IsChecked = true;
        }


        if (iconCombineReg != null)
        {
            int iconCombine = (int)iconCombineReg;
            tbButtonsCombobox.SelectedIndex = iconCombine;
        }

        if (iconSizeReg != null)
        {
            int iconSize = (int)iconSizeReg;
            Debug.WriteLine("iconSize: " + iconSize);
            if (iconSize == 0){ useSmallIconsCheck.IsChecked = true; }
        }

        if (centerTbReg != null)
        {
            int centerTb = (int)centerTbReg;
            Debug.WriteLine("centerTb: " + centerTb);
            centerTbCombobox.SelectedIndex = centerTb;
        }
        regLock = false;
    }

    private void autoHideTbCheck_Checked(object sender, RoutedEventArgs e)
    {
        if (regLock) { return; }
        TaskbarControl.SetAutoHide(true);
    }

    private void autoHideTbCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        if (regLock) { return; }
        TaskbarControl.SetAutoHide(false);
    }

    private void useSmallIconsCheck_Checked(object sender, RoutedEventArgs e)
    {
        //If Registry.SetValue won't work, I'll just use a workaround. くそ野郎.
        if (regLock) { return; }
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c reg add HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced /v IconSizePreference /t REG_DWORD /d 0 /f",
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }

    private void useSmallIconsCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        if (regLock) { return; }
        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c reg add HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced /v IconSizePreference /t REG_DWORD /d 1 /f",
            CreateNoWindow = true,
            UseShellExecute = false
        });

    }

    private void centerTbCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (regLock) { return; }
    }

    private void tbButtonsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (regLock) { return; }
    }

    private void selectIconsBtn_Click(object sender, RoutedEventArgs e)
    {
        if (regLock) { return; }
    }
}

public class TaskbarControl
{
    private const int ABM_SETSTATE = 10;
    private const int ABS_AUTOHIDE = 1;
    private const int ABS_ALWAYSONTOP = 2; // Standard state

    [StructLayout(LayoutKind.Sequential)]
    private struct APPBARDATA
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public IntPtr lParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left, top, right, bottom;
    }

    [DllImport("shell32.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern uint SHAppBarMessage(uint dwMessage, ref APPBARDATA pData);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    public static void SetAutoHide(bool enable)
    {
        APPBARDATA abd = new APPBARDATA();
        abd.cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA));
        abd.hWnd = FindWindow("Shell_TrayWnd", null);
        abd.lParam = (IntPtr)(enable ? ABS_AUTOHIDE : ABS_ALWAYSONTOP);

        SHAppBarMessage(ABM_SETSTATE, ref abd);
    }

    public static bool getAutoHide()
    {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = (uint)Marshal.SizeOf(typeof(APPBARDATA));
            abd.hWnd = FindWindow("Shell_TrayWnd", null);
    
            uint state = SHAppBarMessage(ABM_SETSTATE, ref abd);
            return (state & ABS_AUTOHIDE) == ABS_AUTOHIDE;
    }
}