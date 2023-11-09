using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jackson_Kirkpatrick_Project2 {
    internal class EditOperation {
        private UndoRedo unre;
        private bool txtAreaTextChangeRequired=true;

        public EditOperation() {
            unre = new UndoRedo();
        }
        public string DateTime_Now(){ return DateTime.Now.ToString();}

        public bool TextAreaTextChangeRequired{ 
            get{
                return txtAreaTextChangeRequired;
            }

            set{
                txtAreaTextChangeRequired = value;
            }
        }

        public string UndoClicked(){
            txtAreaTextChangeRequired = false;
            return unre.Undo();
        }

        public string RedoClicked() {
            txtAreaTextChangeRequired = false;
            return unre.Redo();
        }

        public void AddUndoRedo(string item) {unre.AddItem(item);}
        public bool CanUndo(){return unre.CanUndo();}
        public bool CanRedo(){return unre.CanRedo();}
    }
}
