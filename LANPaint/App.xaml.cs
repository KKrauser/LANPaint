﻿using System.Linq;
using System.Windows;
using LANPaint.Dialogs.FrameworkDialogs;
using LANPaint.Dialogs.Service;
using LANPaint.Services.Broadcast;
using LANPaint.Services.Broadcast.Factories;
using LANPaint.Services.IO;
using LANPaint.Services.Network;
using LANPaint.Services.Network.Watchers;
using LANPaint.ViewModels;
using LANPaint.Views;

namespace LANPaint;

public partial class App : Application
{
    private readonly INetworkWatcher _watcher;
    private readonly IBroadcastService _broadcastService;
    private readonly PaintViewModel _paintDataContext;

    public App()
    {
        var broadcastFactory = new ChainerFactory(16384);
        var networkServiceFactory = new NetworkServiceFactory();
        _watcher = networkServiceFactory.CreateWatcher();
        _broadcastService = new BroadcastService(broadcastFactory, _watcher, networkServiceFactory.CreateUtility());
        InitializeBroadcastService(networkServiceFactory, _broadcastService);
        var frameworkDialogFactory = new DefaultFrameworkDialogFactory();
        var dialogService = new DefaultDialogService(frameworkDialogFactory);
        var fileService = new DefaultFileService(new []{".lpsnp"});

        _paintDataContext = new PaintViewModel(_broadcastService, dialogService, fileService, networkServiceFactory);
    }
        
    //TODO: Move this stuff to some kind of Setup or ApplicationBootstrapper with DI container.
    //https://www.codeproject.com/Articles/812379/Using-Ninject-to-produce-a-loosely-coupled-modular
    private async void OnStartupHandler(object sender, StartupEventArgs e)
    {
        var paint = new Paint {DataContext = _paintDataContext,};
        if (e.Args.Length > 0) await _paintDataContext.ApplyFromFile(e.Args.First());
            
        paint.Show();
    }

    private void InitializeBroadcastService(INetworkServiceFactory networkServiceFactory, IBroadcastService broadcastService)
    {
        if (!_watcher.IsAnyNetworkAvailable) return;
        var networkUtility = networkServiceFactory.CreateUtility();
        var readyToUseInterface =
            _watcher.Interfaces.FirstOrDefault(networkInterface => networkUtility.IsReadyToUse(networkInterface));
        if (readyToUseInterface is not null) broadcastService.Initialize(networkUtility.GetIpAddress(readyToUseInterface));
    }

    private void OnExitHandler(object sender, ExitEventArgs e)
    {
        _paintDataContext.Dispose();
        _broadcastService.Dispose();
        _watcher.Dispose();
    }
}