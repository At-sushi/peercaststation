﻿using System;
using System.Windows.Controls;
using PeerCastStation.Core;
using PeerCastStation.WPF.ChannelLists;
using PeerCastStation.WPF.ChannelLists.Dialogs;
using PeerCastStation.WPF.CoreSettings;
using PeerCastStation.WPF.CoreSettings.Dialogs;
using PeerCastStation.WPF.Dialogs;
using PeerCastStation.WPF.Properties;

namespace PeerCastStation.WPF
{
  class WindowManager : IDisposable
  {
    private bool disposed;
    private MainWindow window;
    private volatile bool isShow;
    public bool IsShow { get { return isShow; } }

    public void ShowMainWindow(PeerCastApplication application, Settings settings)
    {
      window = new MainWindow();
      Initialize(window, application, settings);

      isShow = true;
      window.ShowDialog();
      isShow = false;
    }

    private void Initialize(
      MainWindow window, PeerCastApplication application, Settings settings)
    {
      var viewModel = new MainWindowViewModel(application.PeerCast);
      window.DataContext = viewModel;
      Load(settings, viewModel);
      window.Closing += (sender, e) => Save(viewModel, settings);
      window.Closed += (sender, e) => application.Stop();

      window.VersionInfoButton.Click += (sender, e) =>
        {
          var dialog = new VersionInfoWindow
          {
            Owner = window,
            DataContext = new VersionInfoViewModel(application)
          };
          dialog.ShowDialog();
        };

      window.Channels.BroadcastButton.Click += (sender, e) =>
        {
          var dialog = new BroadcastWindow
          {
            Owner = window,
            DataContext = new BroadcastViewModel(application.PeerCast)
          };
          dialog.ShowDialog();
        };

      Initialize(window.Setting, application.PeerCast);
    }

    private void Initialize(SettingControl setting, PeerCast peerCast)
    {
      setting.Ports.AddItemButton.Click += (sender, e) =>
        {
          var dialog = new ListenerEditWindow
          {
            Owner = window,
            DataContext = new ListenerEditViewModel(peerCast)
          };
          dialog.ShowDialog();
          setting.Ports.GetBindingExpression(UserControl.DataContextProperty).UpdateTarget();
        };

      setting.YellowPagesList.AddItemButton.Click += (sender, e) =>
        {
          var dialog = new YellowPagesEditWindow
          {
            Owner = window,
            DataContext = new YellowPagesEditViewModel(peerCast)
          };
          dialog.ShowDialog();
          setting.YellowPagesList.GetBindingExpression(UserControl.DataContextProperty).UpdateTarget();
        };
    }

    private void Load(Settings settings, MainWindowViewModel mainWindow)
    {
      var log = mainWindow.Log;
      log.LogLevel = settings.LogLevel;
      log.IsOutputToGui = settings.LogToGUI;
      log.IsOutputToConsole = settings.LogToConsole;
      log.IsOutputToFile = settings.LogToFile;
      log.OutputFileName = settings.LogFileName;
    }

    private void Save(MainWindowViewModel mainWindow, Settings settings)
    {
      var log = mainWindow.Log;
      settings.LogLevel = log.LogLevel;
      settings.LogToGUI = log.IsOutputToGui;
      settings.LogToConsole = log.IsOutputToConsole;
      settings.LogToFile = log.IsOutputToFile;
      settings.LogFileName = log.OutputFileName;
      settings.Save();
    }

    protected void ThrowExceptionIfDisposed()
    {
      if (disposed)
      {
        throw new ObjectDisposedException(GetType().ToString());
      }
    }

    ~WindowManager()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
      Dispose(true); 
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposed)
      {
        return;
      }
      disposed = true;
      if (disposing)
      {
        // マネージリソースの解放処理
        if (window != null && isShow)
        {
          window.Dispatcher.Invoke(new Action(() =>
          {
            if (isShow)
              window.Close();
          }));
        }
      }
      // アンマネージリソースの解放処理
    }
  }
}
