using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jackson_Kirkpatrick_Project2 {
    public class UndoRedo {
        private Stack<string> undoStack;
        private Stack<string> redoStack;
        public UndoRedo() {
            undoStack = new Stack<string>();
            redoStack = new Stack<string>();
        }

        public void Clear() {undoStack.Clear(); redoStack.Clear();}
        public void AddItem(string item) {undoStack.Push(item);}

        public string Undo() {
            string item = undoStack.Pop();
            redoStack.Push(item);
            return undoStack.First();
        }

        public string Redo() {
            if(redoStack.Count == 0) {return undoStack.First().ToString();}
            string item = redoStack.Pop();
            undoStack.Push(item);
            return undoStack.First();
        }

        public bool CanUndo() {return  undoStack.Count != 0 | undoStack.Count > 0;}
        public bool CanRedo() {return redoStack.Count > 0 | redoStack.Count != 0;}

        public List<string> UndoItems() {return undoStack.ToList();}

        public List<string> RedoItems() {return redoStack.ToList();}


    }
}
