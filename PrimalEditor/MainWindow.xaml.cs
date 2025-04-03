using System.Windows;
using PrimalEditor.GameProject;

namespace PrimalEditor;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnMainWindowLoaded;
    }

    private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnMainWindowLoaded;
        OpenProjectBrowserDialog();
    }

    private void OpenProjectBrowserDialog()
    {
        var projectBrowser = new ProjectBrowserDialog();
        if (projectBrowser.ShowDialog() == false)
        {
            Application.Current.Shutdown();
        }
        else
        {
            
        }
    }
}