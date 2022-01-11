using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace fesbGameSDK
{
    public partial class Highscore : Form
    {
        public Highscore()
        {
            InitializeComponent();
        }

        public string tekst;
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Highscore_Load(object sender, EventArgs e)
        {
            label1.Text = tekst;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
