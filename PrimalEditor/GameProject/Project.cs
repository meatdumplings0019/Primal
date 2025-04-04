﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
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

    // ReSharper disable once MemberCanBePrivate.Global
    public static UndoRedo UndoRedo { get; } = new();

    public ICommand Undo { get; private set; }
    public ICommand Redo { get; private set; }
    
    public ICommand AddScene { get; private set; }
    public ICommand RemoveScene { get; private set; }
    
    private void AddSceneInternal(string sceneName)
    {
        Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));
        _scenes.Add(new Scene(this, sceneName));
    }

    private void RemoveSceneInternal(Scene scene)
    {
        Debug.Assert(_scenes.Contains(scene));
        _scenes.Remove(scene);
    }
    
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

        AddScene = new RelayCommand<object>(x =>
        {
            AddSceneInternal($"New Scene {_scenes.Count}");
            var newScene = _scenes.Last();
            var sceneIndex = _scenes.Count - 1;
            UndoRedo.Add(new UndoRedoAction(
                () => RemoveSceneInternal(newScene),
                () => _scenes.Insert(sceneIndex, newScene),
                $"Add {newScene.Name}"
            ));
        });
        
        RemoveScene = new RelayCommand<Scene>(x =>
        {
            var sceneIndex = _scenes.IndexOf(x);
            RemoveSceneInternal(x);
            
            UndoRedo.Add(new UndoRedoAction(
                () => _scenes.Insert(sceneIndex, x),
                () => RemoveSceneInternal(x),
                $"Remove {x.Name}"
            ));
        }, x => !x.IsActive);

        Undo = new RelayCommand<object>(x => UndoRedo.Undo());
        Redo = new RelayCommand<object>(x => UndoRedo.Redo());
    }

    public Project(string name, string path)
    {
        Name = name;
        Path = path;
        
        OnDeserialized(default);
    }
}