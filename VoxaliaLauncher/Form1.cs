using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace VoxaliaLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Validates a username as correctly formatted.
        /// </summary>
        /// <param name="str">The username to validate</param>
        /// <returns>Whether the username is valid</returns>
        public static bool ValidateUsername(string str)
        {
            // Length = 4-15
            if (str.Length < 4 || str.Length > 15)
            {
                return false;
            }
            // Starts A-Z
            if (!(str[0] >= 'a' && str[0] <= 'z') && !(str[0] >= 'A' && str[0] <= 'Z'))
            {
                return false;
            }
            // All symbols are A-Z, 0-9, _
            for (int i = 0; i < str.Length; i++)
            {
                if (!(str[i] >= 'a' && str[i] <= 'z') && !(str[i] >= 'A' && str[i] <= 'Z')
                    && !(str[i] >= '0' && str[i] <= '9') && !(str[i] == '_'))
                {
                    return false;
                }
            }
            // Valid if all tests above passed
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            if (!ValidateUsername(username))
            {
                MessageBox.Show("That username is not valid!");
                return;
            }
            Process process = new Process();
            process.StartInfo.FileName = "Voxalia.exe";
            process.StartInfo.Arguments = "--username " + username;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.UseShellExecute = false;
            process.Exited += new EventHandler(process_Exited);
            process.EnableRaisingEvents = true;
            process.Start();
            this.WindowState = FormWindowState.Minimized;
        }

        void process_Exited(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Focus();
        }
    }
}
