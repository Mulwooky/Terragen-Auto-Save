using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerragenAutoSave
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        public const int SW_Max = 3;
        string file = null;
        
        public Form1()
        {
            InitializeComponent();

            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            try
            {
                file = localKey.OpenSubKey(@"Software\TerragenAutoSave").GetValue("TGDLocation").ToString();
            }
            catch
            { 
                DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
                if (result == DialogResult.OK) // Test result.
                {
                    file = openFileDialog1.FileName;
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
                    key.CreateSubKey("TerragenAutoSave");
                    key = key.OpenSubKey("TerragenAutoSave", true);
                    key.SetValue("TGDLocation", file);
                }
            }
            
            timer1.Enabled = true;
            timer1.Start();
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = file.ToString()
                }
            };
            process.Start();
            Thread.Sleep(5000);
            FocusStartProcess("tgd");
            localKey.Dispose();
            
        }

        private void FocusProcess(string procName)
        {
            Process[] objProcesses = System.Diagnostics.Process.GetProcessesByName(procName);
            if (objProcesses.Length > 0)
            {
                IntPtr hWnd = IntPtr.Zero;
                hWnd = objProcesses[0].MainWindowHandle;
                //ShowWindowAsync(new HandleRef(null, hWnd), SW_Max);
                SetForegroundWindow(objProcesses[0].MainWindowHandle);
                SendKeys.Send("^s");
            }
            else
            {
                notifyIcon1.Visible = false;
                notifyIcon1.Icon = null;
                notifyIcon1.Dispose();
                Application.Exit();
            }
        }

        private void FocusStartProcess(string procName)
        {
            Process[] objProcesses = System.Diagnostics.Process.GetProcessesByName(procName);
            if (objProcesses.Length > 0)
            {
                IntPtr hWnd = IntPtr.Zero;
                hWnd = objProcesses[0].MainWindowHandle;
                ShowWindowAsync(new HandleRef(null, hWnd), SW_Max);
                SetForegroundWindow(objProcesses[0].MainWindowHandle);
            }
            else
            {
                notifyIcon1.Visible = false;
                notifyIcon1.Icon = null;
                notifyIcon1.Dispose();
                Application.Exit();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            FocusProcess("tgd");
        }

        private void autoSaveOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoSaveOnToolStripMenuItem.Checked = true;
            autoSaveOffToolStripMenuItem.Checked = false;
            timer1.Enabled = true;
            timer1.Stop();
            timer1.Start();
        }

        private void autoSaveOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoSaveOnToolStripMenuItem.Checked = false;
            autoSaveOffToolStripMenuItem.Checked = true;
            timer1.Enabled = false;
            timer1.Stop();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            notifyIcon1.Icon = null;
            notifyIcon1.Dispose();
            Application.Exit();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Process[] objProcesses = System.Diagnostics.Process.GetProcessesByName("TGD");
            if (objProcesses.Length > 0)
            {
            }
            else
            {
                notifyIcon1.Visible = false;
                notifyIcon1.Icon = null;
                notifyIcon1.Dispose();
                Application.Exit();
            }
        }
    }
}
