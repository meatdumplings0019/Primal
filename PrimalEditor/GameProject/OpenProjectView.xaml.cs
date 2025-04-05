using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.GameProject;

public partial class OpenProjectView : UserControl
{
    public OpenProjectView()
    {
        InitializeComponent();
    }

    private void OnOpen_Button_Click(object sender, RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OpenSelectedProject()
    {
        var project = OpenProject.Open((ProjectData)ProjectsListBox.SelectedItem);
        var dialogResult = false;
        var win = Window.GetWindow(this);
        if (project != null)
        {
            dialogResult = true;
            win!.DataContext = project;
        }
        win!.DialogResult = dialogResult;
        win.Close();
    }

    private void OnListBoxItem_Mouse_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        OpenSelectedProject();
    }
}