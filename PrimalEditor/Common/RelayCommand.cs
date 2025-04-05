using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace PrimalEditor;

public class RelayCommand<T>(Action<T> execute, Predicate<T>? canExecute = null) : ICommand
{
    private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    
    public bool CanExecute(object? parameter)
    {
        return canExecute?.Invoke((T)parameter!) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute((T)parameter!);
    }
}