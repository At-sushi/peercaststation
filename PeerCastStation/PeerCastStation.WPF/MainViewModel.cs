﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PeerCastStation.Core;
using PeerCastStation.WPF.ChannelLists;
using PeerCastStation.WPF.Commons;
using PeerCastStation.WPF.CoreSettings;
using PeerCastStation.WPF.Dialogs;
using PeerCastStation.WPF.Logs;

namespace PeerCastStation.WPF
{
  class MainViewModel : ViewModelBase, IDisposable
  {
    private bool disposed;

    private readonly Timer timer;
    private readonly PeerCastApplication application;
    private readonly AppCastReader versionChecker;

    internal SynchronizationContext SynchronizationContext { private get; set; }

    public string PortStatus
    {
      get
      {
        var peerCast = application.PeerCast;
        return "リレー可能ポート:" + String.Join(", ",
          peerCast.OutputListeners.Where(listener =>
            (listener.GlobalOutputAccepts & OutputStreamType.Relay) != 0
          ).Select(
            listener => listener.LocalEndPoint.Port
          ).Distinct().Select(
            port => port.ToString()
          ).ToArray())
          + " " + (peerCast.IsFirewalled.HasValue
          ? peerCast.IsFirewalled.Value ? "未開放" : "開放"
          : "開放状態不明");
      }
    }

    private readonly ChannelListViewModel channelList;
    public ChannelListViewModel ChannelList { get { return channelList; } }

    private readonly SettingViewModel setting;
    public SettingViewModel Setting { get { return setting; } }

    private readonly LogViewModel log = new LogViewModel();
    public LogViewModel Log { get { return log; } }

    internal VersionInfoViewModel VersionInfo
    {
      get { return new VersionInfoViewModel(application); }
    }

    internal event NewVersionFoundEventHandler NewVersionFound
    {
      add { versionChecker.NewVersionFound += value; }
      remove { versionChecker.NewVersionFound -= value; }
    }

    internal MainViewModel(
      PeerCastApplication application, string updateUrl, DateTime currentVersion)
    {
      this.application = application;
      var peerCast = application.PeerCast;
      channelList = new ChannelListViewModel(peerCast);
      setting = new SettingViewModel(peerCast);

      timer = new Timer(o =>
      {
        if (SynchronizationContext == null)
          return;
        SynchronizationContext.Post(p => UpdateStatus(), null);
      }, null, 1000, 1000);

      versionChecker = new AppCastReader(
        new Uri(updateUrl, UriKind.Absolute), currentVersion);

      peerCast.ChannelAdded += OnChannelChanged;
      peerCast.ChannelRemoved += OnChannelChanged;
    }

    ~MainViewModel()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
      this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }
      this.disposed = true;
      if (disposing)
      {
        // マネージリソースの解放処理
      }
      // アンマネージリソースの解放処理
      application.PeerCast.ChannelAdded -= OnChannelChanged;
      application.PeerCast.ChannelRemoved -= OnChannelChanged;
      log.Dispose();
    }

    internal void CheckVersion()
    {
      versionChecker.CheckVersion();
    }

    private void UpdateStatus()
    {
      OnPropertyChanged("PortStatus");
      channelList.UpdateChannelList();
      log.UpdateLog();
    }

    private void OnChannelChanged(object sender, EventArgs e)
    {
      if (SynchronizationContext == null)
        return;
      SynchronizationContext.Post(o => channelList.UpdateChannelList(), null);
    }
  }
}
