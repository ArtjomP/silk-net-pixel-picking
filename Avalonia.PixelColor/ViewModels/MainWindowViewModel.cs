﻿using Avalonia.PixelColor.Controls;
using Avalonia.PixelColor.Utils.OpenGl;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Avalonia.PixelColor.ViewModels;


public sealed class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        Scenes = (OpenGlScenesEnum[])Enum.GetValues(typeof(OpenGlScenesEnum));
        SelectedScene = Scenes.First();
        var canExecute = this
            .WhenAnyValue(
                o => o.ScreenShotsFolder,
                o => o.ScreenShotControl)
            .Select(o => !String.IsNullOrEmpty(o.Item1) && o.Item2 is not null);
        MakeScreenShotCommand = ReactiveCommand.Create(MakeScreenShot);
    }

    public ICommand MakeScreenShotCommand { get; }

    private void MakeScreenShot()
    {
        try
        {
            var screenShotControl = ScreenShotControl;
            if (screenShotControl is not null)
            {
                var folder = ScreenShotsFolder;
                Directory.CreateDirectory(folder);
                var filename = $"{Guid.NewGuid()}.bmp";
                var fullName = Path.Combine(folder, filename);
                screenShotControl.MakeScreenShot(fullName);
            }
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    public IEnumerable<OpenGlScenesEnum> Scenes { get; }

    [Reactive]
    public OpenGlScenesEnum SelectedScene
    {
        get;
        set;
    }

    [Reactive]
    public IScreenShotControl? ScreenShotControl { get; set; }

    [Reactive]
    public String ScreenShotsFolder { get; set; } = String.Empty;

    [Reactive]
    public String Error { get; set; } = String.Empty;
}