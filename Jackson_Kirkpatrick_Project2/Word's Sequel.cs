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
using System.Diagnostics.Eventing.Reader;

namespace Jackson_Kirkpatrick_Project2 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            
            //get list of available fonts on the system and then add them to the combobox currFontText. also set the text in the combo box to the default t
            string[] fontNames = System.Drawing.FontFamily.Families.Select(family => family.Name).ToArray(); 
            currFontText.Items.AddRange(fontNames);
            currFontText.Text = textBody.SelectionFont.Name;
        }

        //if the current file is save
        private bool saved = false;
        //name of current opened file 
        private string currFile = string.Empty;
        private OpenFileDialog ofd = new OpenFileDialog();
        private SaveFileDialog sfd = new SaveFileDialog();
        private UndoRedo unreStack = new UndoRedo();
        private Dictionary<FontStyle, bool> styleEnabled = new Dictionary<FontStyle, bool> {
            {FontStyle.Bold, false},
            {FontStyle.Italic, false},
            {FontStyle.Underline, false}
        };

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
                sfd.FileName = "Untitled.txt";
            

            sfd.Filter = "Text Files (*.txt)|*.txt|PDF Files (*.pdf)|*.pdf|Word Documents (*.docx)|*.docx|All Files (*.*)|*.*";
            sfd.FilterIndex = 1;

            if (DialogResult.OK == sfd.ShowDialog()) {
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
            changeFont(FontStyle.Bold);
        }

        private void italicBtn_Click(object sender, EventArgs e) {
            changeFont(FontStyle.Italic);
        }

        private void underlineBtn_Click(object sender, EventArgs e) {
            changeFont(FontStyle.Underline);
        }

        private void increaseSizeBtn_Click(object sender, EventArgs e) {
            changeFontSize(1.0f);
        }

        private void decreseSizeBtn_Click(object sender, EventArgs e) {
            changeFontSize(-1.0f);
        }


        private void changeFont(FontStyle fontStyle) {
            styleEnabled[fontStyle] = !styleEnabled[fontStyle];
            Font currfont = textBody.SelectionFont;
            if (styleEnabled[fontStyle]) 
                textBody.SelectionFont = new Font(currfont, currfont.Style | fontStyle);
            else
                textBody.SelectionFont = new Font(currfont, currfont.Style & ~fontStyle);
        }
        private void changeFontSize(float val) {
            if(textBody.SelectionFont != null) {
                float newSize = textBody.SelectionFont.Size + val;
                if(newSize <= 12){
                    MessageBox.Show("Minmum font size is 12.\nAny lower and this program will literally implode.\nAnd federal agents will be sent to your house.", "Warning", MessageBoxButtons.OK);
                    newSize = 12.0f;
                }else if(newSize >= 100) {
                    MessageBox.Show("Maximum font size is 100.\nAny larger and this program will literally implode.\nAnd federal agents will be sent to your house.", "Warning", MessageBoxButtons.OK);
                    newSize = 12.0f;

                }
                textBody.SelectionFont = new Font(textBody.SelectionFont.FontFamily, newSize, textBody.SelectionFont.Style);
            }
        }

        private void currFontText_SelectedIndexChanged(object sender, EventArgs e) {
            string selectedFont = currFontText.SelectedItem.ToString();
            Font newFont = new Font(selectedFont, 12);
            int selStart = textBody.SelectionStart;
            int selLength = textBody.SelectionLength;
            textBody.SelectionFont = newFont;
            textBody.Select(selStart, selLength);
        }

        private void textBody_TextChanged(object sender, EventArgs e) {
            string[] textBodyArray = textBody.Text.Split(' ');
            foreach(string word in textBodyArray) {
                unreStack.AddItem(word);
            }
        }

        private void textBody_KeyDown(object sender, KeyEventArgs e) {
            if (e.Control && e.KeyCode == Keys.Z) {
                e.SuppressKeyPress = true;
                if(unreStack.CanUndo()){
                    int selStart = textBody.SelectionStart;
                    textBody.Text = unreStack.Undo();
                    textBody.SelectionStart = Math.Min(selStart, textBody.Text.Length);
                }
            }else if(e.Control && e.KeyCode == Keys.Y) {
                e.SuppressKeyPress = true;
                if(unreStack.CanRedo()){
                    int selStart = textBody.SelectionStart;
                    textBody.Text = unreStack.Redo();
                    textBody.SelectionStart = Math.Min(selStart, textBody.Text.Length);
                }
            }
            else {
                string[] currText = textBody.Text.Split(' ');
                textBody.Redo();
                foreach(string word in currText){unreStack.AddItem(word);}
            }
        }
    }
}
