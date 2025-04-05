using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameProject;

[DataContract]
public class ProjectData
{
    [DataMember]
    public required string ProjectName { get; set; }
    [DataMember]
    public required string ProjectPath { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    public string FullPath => $"{ProjectPath}{ProjectName}{Project.Extension}";
    public byte[] Icon { get; set; } = [];
    public byte[] Screenshot { get; set; } = [];
}

[DataContract]
public class ProjectDataList
{
    [DataMember]
    public required List<ProjectData> Projects { get; set; }
}

public class OpenProject
{
    private static readonly string ApplicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PrimalEditor\";
    private static readonly string ProjectDataPath = $"{ApplicationDataPath}ProjectData.xml";
    // ReSharper disable once InconsistentNaming
    private static readonly ObservableCollection<ProjectData> _projects = [];
    public static ReadOnlyObservableCollection<ProjectData> Projects 
    { get; } = new(_projects);
    
    private static void ReadProjectData()
    {
        if (!File.Exists(ProjectDataPath)) return;
        var projects = Serializer.FromFile<ProjectDataList>(ProjectDataPath).Projects.OrderByDescending(x => x.Date);
        _projects.Clear();
        foreach (var project in projects)
        {
            if (!File.Exists(project.FullPath)) continue;
            project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Primal\Icon.png");
            project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}\.Primal\Screenshot.png");
            _projects.Add(project);
        }
    }

    private static void WriteProjectData()
    {
        var projects = _projects.OrderBy(x => x.Date).ToList();
        Serializer.ToFile(new ProjectDataList { Projects = projects }, ProjectDataPath);
    }
    
    public static Project Open(ProjectData data)
    {
        ReadProjectData();
        var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
        if (project != null)
        {
            project.Date = DateTime.Now;
        }
        else
        {
            project = data;
            project.Date = DateTime.Now;
            _projects.Add(project);
        }
        WriteProjectData();

        return Project.Load(project.FullPath);
    }

    static OpenProject()
    {
        try
        {
            if (!Directory.Exists(ApplicationDataPath)) Directory.CreateDirectory(ApplicationDataPath);
            ReadProjectData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            // TODO: log error
        }
    }
}