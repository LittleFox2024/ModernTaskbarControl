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
        
        object autoHideTbReg = null;
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
        if (iconCombineReg != null)
        {
            int iconLocation = (int)iconCombineReg;
            tbButtonsCombobox.SelectedIndex = iconLocation;
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
        Debug.WriteLine("autoHideTbChecked");
    }

    private void autoHideTbCheck_Unchecked(object sender, RoutedEventArgs e)
    {

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
