using System.Windows;

namespace PrimalEditor.GameProject;

public partial class NewProjectView
{
    public NewProjectView()
    {
        InitializeComponent();
    }

    private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NewProject)DataContext;
        var projectPath = vm.CreateProject((ProjectTemplate)TemplateListBox.SelectedItem);
        var dialogResult = false;
        var win = Window.GetWindow(this);
        if (!string.IsNullOrEmpty(projectPath))
        {
            dialogResult = true;
        }
        win!.DialogResult = dialogResult;
        win.Close();
    }
}