using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace WlanTool
{
    /// <summary>
    /// Interakční logika pro smbMappingWindow1.xaml
    /// </summary>
    public partial class smbMappingWindow1 : Window
    {
        public smbMappingWindow1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Map drive
        {
            // Získání hodnot z textových polí
            string driveLetter = comboBox1LetterConnector.Text;
            string serverov = txbServer.Text;
            string networkPath = txbFolder.Text;
            string userNamase = txbUser.Text;

            // Zkontroluj, zda je vše správně vyplněno
            if (string.IsNullOrEmpty(driveLetter) || string.IsNullOrEmpty(serverov) || string.IsNullOrEmpty(networkPath) ||
                string.IsNullOrEmpty(userNamase) || string.IsNullOrEmpty(txbPasswordik.Password))
            {
                MessageBox.Show("Fill all fields!");
                return;
            }

            // Escape the backslashes in network path if needed
            networkPath = networkPath.Replace("\\", "\\\\"); // Ensure that all backslashes are escaped

            // Correct the command structure:
            string commandos = $"net use {driveLetter} \\\\{serverov}\\{networkPath} /user:{userNamase} \"{txbPasswordik.Password}\"";

            // Debugging message
            MessageBox.Show($"Running command: {commandos}");

            // Setup the process to run the command
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + commandos)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true, // Capture output
                RedirectStandardError = true  // Capture error
            };

            try
            {
                // Start the process
                Process process = new Process { StartInfo = processInfo };
                process.Start();

                // Capture output and error
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Check if there's any error from the process
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show($"Error while connecting: {error}");
                }
                else
                {
                    // Show the output from the process
                    MessageBox.Show($"Command executed successfully:\n{output}");
                }
            }
            catch (Exception ex)
            {
                // Display the exception message in case of failure
                MessageBox.Show($"An exception occurred: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
