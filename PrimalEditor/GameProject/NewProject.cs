﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameProject;

[DataContract]
public class ProjectTemplate
{
    [DataMember]
    public required string ProjectType { get; set; }
    [DataMember]
    public required string ProjectFile { get; set; }
    [DataMember]
    public required List<string> Folders { get; set; }

    public byte[] Icon { get; set; } = [];
    public byte[] Screenshot { get; set; } = [];
    public string IconFilePath { get; set; } = string.Empty;
    public string ScreenshotFilePath { get; set; } = string.Empty;
    public string ProjectFilePath { get; set; } = string.Empty;
}

public class NewProject : ViewModelBase
{
    //TODO: get the path from the installation location
    private const string TemplatePath = @"..\..\PrimalEditor\ProjectTemplates\";
    private string _projectProjectName = "NewProject";
    public string ProjectName
    {
        get => _projectProjectName;
        set
        {
            if (_projectProjectName == value) return;
            _projectProjectName = value;
            ValidateProjectPath();
            OnPropertyChanged(nameof(ProjectName));
        }
    }

    private string _projectProjectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProjects\";
    public string ProjectPath
    {
        get => _projectProjectPath;
        set
        {
            if (_projectProjectPath == value) return;
            _projectProjectPath = value;
            ValidateProjectPath();
            OnPropertyChanged(nameof(ProjectPath));
        }
    }

    private bool _isValid;
    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid == value) return;
            _isValid = value;
            OnPropertyChanged(nameof(IsValid));
        }
    }

    private string _errorMsg = string.Empty;

    public string ErrorMsg
    {
        get => _errorMsg;
        set
        {
            if (_errorMsg == value) return;
            _errorMsg = value;
            OnPropertyChanged(nameof(ErrorMsg));
        }
    }
    
    private readonly ObservableCollection<ProjectTemplate> _projectTemplates = [];
    public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates 
    { get; }

    private void ValidateProjectPath()
    {
        var path = ProjectPath;
        if (!Path.EndsInDirectorySeparator(path)) path += @"\";
        path += $@"{ProjectName}\";

        IsValid = false;
        ErrorMsg = string.Empty;

        if (string.IsNullOrWhiteSpace(ProjectName.Trim())) ErrorMsg = "Type in a project name.";
        else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1) ErrorMsg = "Invalid character(s) used in project name.";
        else if (string.IsNullOrWhiteSpace(ProjectPath.Trim())) ErrorMsg = "Select a valid project folder.";
        else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) ErrorMsg = "Invalid character(s) used in project path.";
        else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) ErrorMsg = "Select project folder already exists and is not empty.";
        else IsValid = true;
    }

    public string CreateProject(ProjectTemplate template)
    {
        ValidateProjectPath();
        if (!IsValid) return string.Empty;
        
        if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
        var path = $@"{ProjectPath}{ProjectName}\";

        try
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            foreach (var folder in template.Folders)
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, folder)));

            var dirInfo = new DirectoryInfo(path + @".Primal\");
            dirInfo.Attributes |= FileAttributes.Hidden;
            File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
            File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

            var projectXml = File.ReadAllText(template.ProjectFilePath);
            projectXml = string.Format(projectXml, ProjectName, ProjectPath);
            var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
            File.WriteAllText(projectPath, projectXml);
            
            return path;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            // TODO: log error
            return string.Empty;
        }
    }

    public NewProject()
    {
        ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
        try
        {
            var templatesFiles = Directory.GetFiles(TemplatePath, "template.xml", SearchOption.AllDirectories);
            Debug.Assert(templatesFiles.Length != 0, "No template files found");
            foreach (var file in templatesFiles)
            {
                var template = Serializer.FromFile<ProjectTemplate>(file);
                template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "Icon.png"));
                template.Icon = File.ReadAllBytes(template.IconFilePath);
                template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "Screenshot.png"));
                template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, template.ProjectFile));

                _projectTemplates.Add(template);
            }
            ValidateProjectPath();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            // TODO: log error
        }
    }
}