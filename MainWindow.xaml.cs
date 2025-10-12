using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WlanTool
{
    public partial class MainWindow : System.Windows.Window
    {
   

        public MainWindow()
        {
            InitializeComponent();
            ReadWinKeyCode();
            TasksOverview();
            AutorunStartups();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // GOD MODE button
        {
            string godModeFolderName = "GodMode.{ED7BA470-8E54-465E-825C-99712043E01C}";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string godModeFolderPath = Path.Combine(desktopPath, godModeFolderName);

            if (!Directory.Exists(godModeFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(godModeFolderPath);
                    System.Windows.MessageBox.Show("✅ God Mode folder created on your Desktop!");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("❌ Error creating God Mode folder: " + ex.Message);
                    return;
                }
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = godModeFolderName,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("❌ Error opening folder: " + ex.Message);
            }
        }

        private void konec_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // moznost zmeny pozice okna na obrazovce
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove(); // umožní přetahování okna
        }

        private void ReadWinKeyCode()
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                    .OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SoftwareProtectionPlatform"))
                {
                    if (key != null)
                    {
                        object o = key.GetValue("BackupProductKeyDefault");
                        txbWinCode.Text = o != null ? o.ToString() : "Windows key not found!";
                    }
                    else
                    {
                        txbWinCode.Text = "Registry key not found!";
                    }
                }
            }
            catch (Exception ex)
            {
                txbWinCode.Text = "Error: " + ex.Message;
            }
        }

        private void TasksOverview()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "schtasks";
                process.StartInfo.Arguments = "/query /fo LIST /v";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder taskList = new StringBuilder();

                string taskName = "";
                string taskState = "";

                foreach (string line in lines)
                {
                    if (line.StartsWith("TaskName:"))
                    {
                        taskName = line.Substring(9).Trim();
                    }
                    else if (line.StartsWith("Status:"))
                    {
                        taskState = line.Substring(7).Trim();

                        if (!taskName.StartsWith("\\Microsoft\\Windows", StringComparison.OrdinalIgnoreCase) &&
                            !taskName.StartsWith("\\Microsoft\\Office", StringComparison.OrdinalIgnoreCase) &&
                            (taskState.Equals("Ready", StringComparison.OrdinalIgnoreCase) ||
                             taskState.Equals("Running", StringComparison.OrdinalIgnoreCase)))
                        {
                            taskList.AppendLine($"Task Name: {taskName}");
                            taskList.AppendLine($"State: {taskState}");
                            taskList.AppendLine(new string('-', 50));
                        }

                        taskName = "";
                        taskState = "";
                    }
                }

                if (taskList.Length > 0)
                {
                    rtbTaskOverview.AppendText(taskList.ToString());
                }
                else
                {
                    rtbTaskOverview.AppendText("No user-created Running or Ready tasks found.\n");
                }
            }
            catch (Exception ex)
            {
                rtbTaskOverview.AppendText($"Error: {ex.Message}\n");
            }
        }

        private void TasksOpenWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                //string regPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks";

                //Process.Start(new ProcessStartInfo
                //{
                //    FileName = "reg",
                //    Arguments = $@"add ""HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit"" /v LastKey /t REG_SZ /d ""{regPath}"" /f",
                //    UseShellExecute = false,
                //    CreateNoWindow = true
                //}).WaitForExit();

                Process.Start("taskschd.msc");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error opening taskschd: {ex.Message}", "Error");
            }
        }

        private void AutorunStartups()
        {
            try
            {

     

                using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    if (rk != null)
                    {
                        foreach (string valueName in rk.GetValueNames())
                        {
                            // Vytvoříme nový odstavce (Paragraph) pro každý název
                            Paragraph para = new Paragraph(new Run(valueName));
                            para.Margin = new Thickness(0); // Odstraní mezery mezi řádky
                   
                            para.FontSize = 12; // Velikost písma
                            para.LineHeight = 14; // Nastavení výšky řádku, aby byly řádky blíže k sobě
                            para.TextAlignment = TextAlignment.Left; // Volitelně, pro zarovnání vlevo

                            // Přidáme tento paragraf do dokumentu
                            rtbAutoruns2.Document.Blocks.Add(para);
                        }
                    }
                    else
                    {
                        rtbAutoruns2.AppendText("No autorun user applications found.\n");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Unexpected Error: " + ex.Message);
            }
        }

        private void AutorunStartups(object sender, RoutedEventArgs e)
        {
            OpenRegistryKey(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        }

        private void UserStartReg(object sender, RoutedEventArgs e)
        {
            OpenRegistryKey(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run");
        }

        private void OpenRegistryKey(string regPath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "reg",
                    Arguments = $@"add ""HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit"" /v LastKey /t REG_SZ /d ""{regPath}"" /f",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).WaitForExit();

                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error opening registry: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnWinStart_Click(object sender, RoutedEventArgs e)
        {
            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = startPath,
                UseShellExecute = true
            });
        }

        private void btnNetAdapters(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "ncpa.cpl",
                UseShellExecute = true
            });
        }

        private void btnCleanManager(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cleanmgr.exe",
                UseShellExecute = true
            });
        }

        private void btnImportWlanOnce(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "data(*.xml)|*.xml" };
            var result = ofd.ShowDialog();
            if (result == true)
            {
                string vnorit = ofd.FileName;

                if (!string.IsNullOrEmpty(vnorit))
                {
                    string strCmd = ($"/c netsh wlan add profile filename=\"{vnorit}\" user=all");
                    Process.Start("cmd.exe", strCmd);
                    System.Windows.MessageBox.Show("WLAN profile successfully imported!");
                }
                else
                {
                    System.Windows.MessageBox.Show("The file was not selected!");
                }
            }
        }

        private void btnBackuppWifi(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = System.Windows.MessageBox.Show(
                "Confirm export of ALL WiFi profiles?",
                "Export",
                MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes)
            {
                string currentPath = Directory.GetCurrentDirectory();
                string saveFolder = Path.Combine(currentPath, "WifiProfilesEXP");
                Directory.CreateDirectory(saveFolder);

                Process process = new Process();
                process.StartInfo.FileName = "netsh.exe";
                process.StartInfo.Arguments = $"wlan export profile key=clear folder=\"{saveFolder}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;

                process.Start();
                process.WaitForExit();

                System.Windows.MessageBox.Show("Export done!");
            }
        }

        private void btnExportKexLic(object sender, RoutedEventArgs e)
        {
            string machineName = Environment.MachineName;
            string txtBoxDavajCasy = DateTime.Now.ToString("d"); //cas pouze v CZ jazyce s teckami, pro eng dela lomitka = chyba, nedavat do nazvu...

            string path = @"ProductKey_" + machineName + ".txt";
            if (!System.IO.File.Exists(path))
            {
                string messageText = "Win product key saving...";
                string title = "State message";

                // Debug output to ensure txbWinCode is a TextBox
                Console.WriteLine(txbWinCode.GetType().ToString()); // Should output: System.Windows.Controls.TextBox

                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(txtBoxDavajCasy);
                    sw.WriteLine(machineName);
                    sw.WriteLine(txbWinCode.Text);  // Access the Text property of the TextBox
                                                    //sw.WriteLine("KONEC textu");

                    var result = System.Windows.MessageBox.Show(messageText, title, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        

            else
            {
                string Titulek = "something wrong...";
                string Text = "file exist or not writed... !";

                var resultError = System.Windows.MessageBox.Show(Text, Titulek, (MessageBoxButton)MessageBoxButtons.OK, (MessageBoxImage)MessageBoxIcon.Warning);

            }
        }


        private void btnSMBpath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window window2 = new smbMappingWindow1();
            window2.Show();
        }



        private void LicKeyVisibleUncheck(object sender, RoutedEventArgs e)
        {
            txbWinCode.Visibility = Visibility.Collapsed;
        }

        private void CheckBoxShowLicKeyH(object sender, RoutedEventArgs e)
        {
            txbWinCode.Visibility = Visibility.Visible;
        }

        private void btnControlPanel(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "control",
                UseShellExecute = true
            });
        }
    }
}
