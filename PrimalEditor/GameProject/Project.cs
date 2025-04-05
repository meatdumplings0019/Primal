using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using PrimalEditor.Utilities;

namespace PrimalEditor.GameProject;

[DataContract(Name = "Game")]
public class Project : ViewModelBase
{
    public static string Extension { get; } = ".primal";
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Path { get; set; }

    private string FullPath => $"{Path}{Name}{Extension}";

    [DataMember(Name = "Scenes")]
    private readonly ObservableCollection<Scene> _scenes = [];

    public ReadOnlyObservableCollection<Scene> Scenes
    { get; private set; }

    private Scene _activeScene;
    [DataMember]
    public Scene ActiveScene
    {
        get => _activeScene;
        set
        {
            if (_activeScene == value) return;
            _activeScene = value;
            OnPropertyChanged(nameof(ActiveScene));
        }
    }
    
    public static Project Current => (Project)Application.Current.MainWindow!.DataContext;
    
    public static Project Load(string file)
    {
        Debug.Assert(File.Exists(file));
        return Serializer.FromFile<Project>(file);
    }
    
    public void Unload()
    { }
    
    public static void Save(Project project)
    {
        Serializer.ToFile(project ,project.FullPath);
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
        OnPropertyChanged(nameof(Scenes));

        ActiveScene = Scenes.FirstOrDefault(x => x.IsActive)!;
    }

    public Project(string name, string path)
    {
        Name = name;
        Path = path;
        
        OnDeserialized(default);
    }
}