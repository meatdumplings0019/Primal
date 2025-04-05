using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PrimalEditor.GameProject;

public partial class OpenProjectView
{
    public OpenProjectView()
    {
        InitializeComponent();

        Loaded += (s, e) =>
        {
            var item = (ListBoxItem)ProjectsListBox.ItemContainerGenerator
                .ContainerFromIndex(ProjectsListBox.SelectedIndex);
            item?.Focus();
        };
    }

    private void OnOpen_Button_Click(object sender, RoutedEventArgs e)
    {
        OpenSelectedProject();
    }

    private void OpenSelectedProject()
    {
        var project = OpenProject.Open((ProjectData)ProjectsListBox.SelectedItem);
        var win = Window.GetWindow(this);
        var dialogResult = true;
        win!.DataContext = project;
        win.DialogResult = dialogResult;
        win.Close();
    }

    private void OnListBoxItem_Mouse_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        OpenSelectedProject();
    }
}