﻿// PeerCastStation, a P2P streaming servent.
// Copyright (C) 2013 PROGRE (djyayutto@gmail.com)
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Threading;
using System.Windows;
using PeerCastStation.Core;
using PeerCastStation.WPF.Properties;

namespace PeerCastStation.WPF
{
  [Plugin]
  public class UserInterface
    : IPlugin
  {
    public string Name { get { return "GUI by WPF"; } }
    public bool IsUsable { get { return true; } }

    MainWindow mainWindow;
    MainViewModel viewModel;
    Thread notifyIconThread;
    NotifyIconManager notifyIconManager;
    Thread mainThread;
    private AppCastReader versionChecker;
    public void Start(PeerCastApplication application)
    {
      notifyIconThread = new Thread(() =>
      {
        notifyIconManager = new NotifyIconManager(application.PeerCast);
        notifyIconManager.CheckVersionClicked += (sender, e) => versionChecker.CheckVersion();
        notifyIconManager.QuitClicked         += (sender, e) => application.Stop();
        notifyIconManager.ShowWindowClicked   += (sender, e) => {
          if (mainWindow!=null) {
            mainWindow.Dispatcher.Invoke(new Action(() => {
              mainWindow.Show();
            }));
          }
        };
        versionChecker = new AppCastReader(
          new Uri(Settings.Default.UpdateURL, UriKind.Absolute),
          Settings.Default.CurrentVersion);
        versionChecker.NewVersionFound += (sender, e) => {
          notifyIconManager.NewVersionInfo = e.VersionDescription;
        };
        versionChecker.CheckVersion();
        notifyIconManager.Run();
      });
      notifyIconThread.SetApartmentState(ApartmentState.STA);
      notifyIconThread.Start();

      mainThread = new Thread(() =>
      {
        var app = new Application();
        viewModel = new MainViewModel(application);
        var settings = application.Settings.Get<WPFSettings>();
        mainWindow = new MainWindow(viewModel);
        if (settings.ShowWindowOnStartup) mainWindow.Show();
        app.Run();
        viewModel.Dispose();
      });
      mainThread.SetApartmentState(ApartmentState.STA);
      mainThread.Start();
    }

    public void Stop()
    {
      if (mainWindow!=null) {
        mainWindow.Dispatcher.Invoke(new Action(() => {
          Application.Current.Shutdown();
        }));
      }
      notifyIconManager.Dispose();
      mainThread.Join();
      notifyIconThread.Join();
    }
  }
}
