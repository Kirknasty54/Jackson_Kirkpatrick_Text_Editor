using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Jackson_Kirkpatrick_Project2 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e) {

        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e) {

        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
            if(File.Exists("file.txt"))
                textBody.Text = "Found it";
            else {
                textBody.Text = "Can't Find it";
            }
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e) {

        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
