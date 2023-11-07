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

        //if the current file is save
        private bool saved = false;
        //name of current opened file 
        private string currFile = string.Empty;
        OpenFileDialog ofd = new OpenFileDialog();
        SaveFileDialog sfd = new SaveFileDialog();

        private void newToolStripMenuItem1_Click(object sender, EventArgs e) {
            //if the textbody has text entered ask user if okay with losing contents since they will be opening a new file 
            //if user agrees, then we set current file to none and empty the textBody
            if(textBody.Text != string.Empty) {
                if(DialogResult.OK == MessageBox.Show("Current content will be lost.", "Contine Anyway?", MessageBoxButtons.OKCancel)) {
                    currFile = string.Empty;
                    textBody.Text = string.Empty;
                    this.Text = "Word's Sequel";
                }
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e) {
            try {
                //if user selected a file then get the name of the selected a file
                //and load text of file into textBody
                //also add the name of the file into the form title (i like how it looks) 
                if(DialogResult.OK == ofd.ShowDialog()) {
                    currFile = ofd.FileName;
                    textBody.LoadFile(currFile, RichTextBoxStreamType.PlainText);
                    this.Text = Path.GetFileName(currFile) + " - Word's Sequel";
                }
            }

            //here to catch any unexpected errors
            catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e) {
            //sets save as true 
            saved = true;
            //if no file is currently loaded then we know we need to save as
            if(currFile == "")
                saveAsToolStripMenuItem1_Click(sender, e);
            else
                textBody.SaveFile(currFile);
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e) {
            if(currFile == "")
                sfd.FileName = "Untitled";

            if(DialogResult.OK == sfd.ShowDialog()) {
                if(Path.GetExtension(sfd.FileName)== ".txt")
                    textBody.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
                currFile = sfd.FileName;
                this.Text = Path.GetFileName(currFile) + " - Word's Sequel";
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e) {
            if (!saved) {
                if (MessageBox.Show("Some changes were not saved. Are you sure you want to exit?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Maximized;
        }

        private void boldBtn_Click(object sender, EventArgs e) {
            int selStart = textBody.SelectionStart;
            int selLength = textBody.SelectionLength;

            textBody.SelectionFont = new Font(textBody.Font, FontStyle.Bold);
            
            textBody.SelectionStart = textBody.SelectionStart + textBody.SelectionLength;
            textBody.SelectionLength = 0;

            textBody.SelectionFont = textBody.Font;
            textBody.Select(selStart, selLength);
        }

        private void italicBtn_Click(object sender, EventArgs e) {

        }

        private void underlineBtn_Click(object sender, EventArgs e) {

        }

        private void increaseSizeBtn_Click(object sender, EventArgs e) {

        }

        private void decreseSizeBtn_Click(object sender, EventArgs e) {

        }
    }
}
