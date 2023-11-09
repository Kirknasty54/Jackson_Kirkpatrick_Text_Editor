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
        EditOperation editOperation;
        Timer timer;
        public Form1() {
            InitializeComponent();
            
            //get list of available fonts on the system and then add them to the combobox currFontText. also set the text in the combo box to the default font
            string[] fontNames = System.Drawing.FontFamily.Families.Select(family => family.Name).ToArray(); 
            currFontText.Items.AddRange(fontNames);
            currFontText.Text = textBody.SelectionFont.Name;
            editOperation = new EditOperation();
            timer = new Timer();
            timer.Tick+=MyTimerTick;
            timer.Interval = 10;
        }

        //if the current file is save
        private bool saved = false;
        //name of current opened file 
        private string currFile = string.Empty;
        //initalize the openfile and savefile dialogs, also create dictionary that contains the fontstyles of bold italic and underline so that we can enable text to be all 3 a once 
        private OpenFileDialog ofd = new OpenFileDialog();
        private SaveFileDialog sfd = new SaveFileDialog();
        private Dictionary<FontStyle, bool> styleEnabled = new Dictionary<FontStyle, bool> {
            {FontStyle.Bold, false},
            {FontStyle.Italic, false},
            {FontStyle.Underline, false}
        };

        private void MyTimerTick(object sender, EventArgs e) {
            //this timer serves as a state save, all text that is added within the timer iteration will be added to the undoredo stack
            timer.Stop();
            editOperation.AddUndoRedo(textBody.Text);
            UpdateView();
        }

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
            //upon saving the current unnamed file, it takes the first 4 words in the textbox, similar to what word does when it takes the first few words as the document title
            if(currFile == "")
                sfd.FileName = string.Join(" ", textBody.Text.Split().Take(4));
            
            //this should only show txt, pdf, and dox files. 
            //will auto prompt to save as a txt files
            sfd.Filter = "Text Files (*.txt)|*.txt|PDF Files (*.pdf)|*.pdf|Word Documents (*.docx)|*.docx|All Files (*.*)|*.*";
            sfd.FilterIndex = 1;

            //sees if the file has extension of txt and sets the title of the form to the current file and then appends Word's Sequel to the end of it, again cause I like how it looks
            if (DialogResult.OK == sfd.ShowDialog()) {
                if(Path.GetExtension(sfd.FileName)== ".txt")
                    textBody.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
                currFile = sfd.FileName;
                this.Text = Path.GetFileName(currFile) + " - Word's Sequel";
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e) {
            //pretty self explanitory. closes the form/application 
            this.Close();
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e) {
            //checks to see if the file or changes made to the file werew saved, if not then give the user a warning that changes were not saved
            if (!saved) {
                if (MessageBox.Show("Some changes were not saved. Are you sure you want to exit?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel){e.Cancel = true;}
            }
        }

        //this just maximizes the form upon start up. i like this better
        private void Form1_Load(object sender, EventArgs e) {this.WindowState = FormWindowState.Maximized;}

        //sets either selected text font or the future text font to bold
        private void boldBtn_Click(object sender, EventArgs e) {changeFont(FontStyle.Bold);}
        //sets either selected text font or the future text font to italic 
        private void italicBtn_Click(object sender, EventArgs e) {changeFont(FontStyle.Italic);}
        //sets either selected text font or the future text font to underline
        private void underlineBtn_Click(object sender, EventArgs e) {changeFont(FontStyle.Underline);}
        //increase either selected test size or future text size by 1
        private void increaseSizeBtn_Click(object sender, EventArgs e) {changeFontSize(1.0f);}
        //decrease either selected test size or future text size by 1
        private void decreseSizeBtn_Click(object sender, EventArgs e) {changeFontSize(-1.0f);}

        //actually changes the font style, allows for text to have multipel different font styles. ex both bold and italic, italic and bold, bold and underline, bold italic and underline, etc.
        private void changeFont(FontStyle fontStyle) {
            styleEnabled[fontStyle] = !styleEnabled[fontStyle];
            Font currfont = textBody.SelectionFont;
            //utilize bitwise operators to turn onn or off bits for font style
            if (styleEnabled[fontStyle]){textBody.SelectionFont = new Font(currfont, currfont.Style | fontStyle); }
            else{textBody.SelectionFont = new Font(currfont, currfont.Style & ~fontStyle); }
        }

        //actually changes font size. i have it add the val to the current font size and simply have the decrease size event use a negative value to decrease the font size. so i dont have two make 2 functions when i can just do 1
        private void changeFontSize(float val) {
            //makes sure that the selectionfont exists. and if the newsize if smaller than 12, then set minimum to 12
            if(textBody.SelectionFont != null) {
                float newSize = textBody.SelectionFont.Size + val;
                if(newSize <= 12){
                    MessageBox.Show("Minmum font size is 12.\nAny lower and this program will literally implode.\nAnd federal agents will be sent to your house.", "Warning", MessageBoxButtons.OK);
                    newSize = 12.0f;
                }else if(newSize >= 100) {
                    MessageBox.Show("Maximum font size is 100.\nAny larger and this program will literally implode.\nAnd federal agents will be sent to your house.", "Warning", MessageBoxButtons.OK);
                    newSize = 100.0f;

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

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            try{
                textBody.Text = editOperation.UndoClicked();
                UpdateView();
            }catch(Exception) {
                textBody.Text = "";
            }
        }

                //if (MessageBox.Show("Some changes were not saved. Are you sure you want to exit?", "Warning", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            textBody.Text = editOperation.RedoClicked();
            UpdateView();
        }

        private void textBody_TextChanged(object sender, EventArgs e) {
            if(editOperation.TextAreaTextChangeRequired) {timer.Start();}
            else {editOperation.TextAreaTextChangeRequired = false;}
            UpdateView();
        }

        private void UpdateView() {
            undoToolStripMenuItem.Enabled = editOperation.CanUndo() ? true : false;
            redoToolStripMenuItem.Enabled = editOperation.CanRedo() ? true : false;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            if(!string.IsNullOrEmpty(textBody.Text)){Clipboard.SetText(textBody.Text); }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            //checks to 
            if(Clipboard.ContainsText()){
                int selStart = textBody.SelectionStart;
                int selLength = textBody.SelectionLength;

                if(selLength > 0){textBody.Text += Clipboard.GetText(TextDataFormat.Text).ToString();}
                textBody.Text = textBody.Text.Insert(selStart, Clipboard.GetText(TextDataFormat.Text).ToString());
                textBody.SelectionStart = selStart + Clipboard.GetText(TextDataFormat.Text).Length;
                textBody.SelectionLength = 0;
            }
        }

    

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {

        }
    }
}
