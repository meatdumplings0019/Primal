using System.ComponentModel;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace PrimalEditor;

[DataContract(IsReference = true)]
public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}