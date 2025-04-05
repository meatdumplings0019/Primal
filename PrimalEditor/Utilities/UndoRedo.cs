﻿using System.Collections.ObjectModel;
using System.Diagnostics;

namespace PrimalEditor.Utilities;

public interface IUndoRedo
{
    string Name { get; }
    void Undo();
    void Redo();
}

public class UndoRedoAction(string name) : IUndoRedo
{
    private Action _undoAction;
    private Action _redoAction;
    
    public string Name { get; } = name;
    public void Undo() => _undoAction();
    public void Redo() => _redoAction();

    public UndoRedoAction(Action undo, Action redo, string name) 
        : this(name)
    {
        Debug.Assert(undo != null && redo != null);
        _undoAction = undo;
        _redoAction = redo;
    }
}

public class UndoRedo
{
    private readonly ObservableCollection<IUndoRedo> _redoList = [];
    private readonly ObservableCollection<IUndoRedo> _undoList = [];
    public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }
    public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

    public void Reset()
    {
        _redoList.Clear();
        _undoList.Clear();
    }

    public void Add(IUndoRedo cmd)
    {
        _undoList.Add(cmd);
        _redoList.Clear();
    }

    public void Undo()
    {
        if (!_undoList.Any()) return;
        var cmd = _undoList.Last();
        _undoList.RemoveAt(_undoList.Count - 1);
        cmd.Undo();
        _redoList.Insert(0, cmd);
    }

    public void Redo()
    {
        if (!_redoList.Any()) return;
        var cmd = _redoList.First();
        _redoList.RemoveAt(0);
        cmd.Redo();
        _undoList.Add(cmd);
    }

    public UndoRedo()
    {
        RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
        UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
    }
}