using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public class UndoRedo {
    private Stack<string> undoStack;
    private Stack<string> redoStack;

    public string CurrentItem { get; private set; }
    public event EventHandler<UndoRedoEventArgs> UndoHappened;
    public event EventHandler<UndoRedoEventArgs> RedoHappened;

    public UndoRedo() {
        undoStack = new Stack<string>();
        redoStack = new Stack<string>();
    }

    public void Clear() {
        undoStack.Clear();
        redoStack.Clear();
        CurrentItem = null;
    }

    public void AddItem(string item) {
        if (!string.IsNullOrEmpty(CurrentItem))
            undoStack.Push(CurrentItem);
        CurrentItem = item;
        redoStack.Clear();
    }

    public void Undo() {
        if (undoStack.Count > 0) {
            redoStack.Push(CurrentItem);
            CurrentItem = undoStack.Pop();
            UndoHappened?.Invoke(this, new UndoRedoEventArgs(CurrentItem));
        }
    }

    public void Redo() {
        if (redoStack.Count > 0) {
            undoStack.Push(CurrentItem);
            CurrentItem = redoStack.Pop();
            RedoHappened?.Invoke(this, new UndoRedoEventArgs(CurrentItem));
        }
    }

    public bool CanUndo() {
        return undoStack.Count > 0;
    }

    public bool CanRedo() {
        return redoStack.Count > 0;
    }

    public List<string> UndoItems() {
        return undoStack.ToList();
    }

    public List<string> RedoItems() {
        return redoStack.ToList();
    }
}

public class UndoRedoEventArgs : EventArgs {
    public string CurrentItem { get; private set; }

    public UndoRedoEventArgs(string currentItem) {
        CurrentItem = currentItem;
    }
}
