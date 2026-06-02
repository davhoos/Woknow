using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Woknow
{
    public partial class formLoader : Form
    {
        public formLoader()
        {
            InitializeComponent();
        }

        private void formLoader_Load(object sender, EventArgs e)
        {
            
            progressBar1.Value = 0;
            progressBar1.Maximum = 100;

    
            timer1.Interval = 12; 
            timer1.Tick += Timer1_Tick; 
            timer1.Start(); 
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
           
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value += 5;
            }
            else
            {
                timer1.Stop();

                //MessageBox.Show("loading at the end...");
            }
        }

    }
}
