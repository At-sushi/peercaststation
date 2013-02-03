﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PeerCastStation.Core;
using PeerCastStation.WPF.Commons;
using PeerCastStation.WPF.CoreSettings.Dialogs;

namespace PeerCastStation.WPF.CoreSettings
{
  class SettingViewModel : ViewModelBase
  {
    private readonly PeerCast peerCast;

    private readonly ListViewModel<PortListItem> ports
      = new ListViewModel<PortListItem>();
    public ListViewModel<PortListItem> Ports
    {
      get
      {
        ports.Items = peerCast.OutputListeners
          .Select(listener => new PortListItem(listener)).ToArray();
        return ports;
      }
    }
    internal OutputListener SelectedListener
    {
      get { return ports.SelectedItem.Listener; }
    }
    public bool IsPortSelected { get { return ports.SelectedItem != null; } }

    internal ListenerEditViewModel ListenerEdit
    {
      get { return new ListenerEditViewModel(peerCast); }
    }

    public bool? IsLocalRelay
    {
      get
      {
        return SelectedListener.GetFromLocalOutputAccepts(OutputStreamType.Relay);
      }
      set
      {
        SelectedListener.SetToLocalOutputAccepts(OutputStreamType.Relay, value);
        OnPropertyChanged("IsLocalRelay");
        OnPropertyChanged("Ports");
      }
    }

    public bool? IsLocalDirect
    {
      get
      {
        return SelectedListener.GetFromLocalOutputAccepts(OutputStreamType.Play);
      }
      set
      {
        SelectedListener.SetToLocalOutputAccepts(OutputStreamType.Play, value);
        OnPropertyChanged("IsLocalDirect");
        OnPropertyChanged("Ports");
      }
    }

    public bool? IsLocalInterface
    {
      get
      {
        return SelectedListener.GetFromLocalOutputAccepts(OutputStreamType.Interface);
      }
      set
      {
        SelectedListener.SetToLocalOutputAccepts(OutputStreamType.Interface, value);
        OnPropertyChanged("IsLocalInterface");
        OnPropertyChanged("Ports");
      }
    }

    public bool? IsGlobalRelay
    {
      get
      {
        return SelectedListener.GetFromGlobalOutputAccepts(OutputStreamType.Relay);
      }
      set
      {
        SelectedListener.SetToGlobalOutputAccepts(OutputStreamType.Relay, value);
        OnPropertyChanged("IsGlobalRelay");
        OnPropertyChanged("Ports");
      }
    }

    public bool? IsGlobalDirect
    {
      get
      {
        return SelectedListener.GetFromGlobalOutputAccepts(OutputStreamType.Play);
      }
      set
      {
        SelectedListener.SetToGlobalOutputAccepts(OutputStreamType.Play, value);
        OnPropertyChanged("IsGlobalDirect");
        OnPropertyChanged("Ports");
      }
    }

    public bool? IsGlobalInterface
    {
      get
      {
        return SelectedListener.GetFromGlobalOutputAccepts(OutputStreamType.Interface);
      }
      set
      {
        SelectedListener.SetToGlobalOutputAccepts(OutputStreamType.Interface, value);
        OnPropertyChanged("IsGlobalInterface");
        OnPropertyChanged("Ports");
      }
    }

    private readonly OtherSettingViewModel otherSetting;
    public OtherSettingViewModel OtherSetting
    {
      get { return otherSetting; }
    }

    private readonly ListViewModel<YellowPageItem> yellowPagesList
      = new ListViewModel<YellowPageItem>();
    public ListViewModel<YellowPageItem> YellowPagesList
    {
      get
      {
        yellowPagesList.Items = peerCast.YellowPages
          .Select(yp => new YellowPageItem(yp)).ToArray();
        return yellowPagesList;
      }
    }

    internal YellowPagesEditViewModel YellowPagesEdit
    {
      get { return new YellowPagesEditViewModel(peerCast); }
    }

    internal SettingViewModel(PeerCast peerCast)
    {
      this.peerCast = peerCast;
      otherSetting = new OtherSettingViewModel(peerCast.AccessController);

      ports.SelectedItemChanged += (sender, e) =>
        {
          OnPropertyChanged("IsPortSelected");
          OnPropertyChanged("IsLocalRelay");
          OnPropertyChanged("IsLocalDirect");
          OnPropertyChanged("IsLocalInterface");
          OnPropertyChanged("IsGlobalRelay");
          OnPropertyChanged("IsGlobalDirect");
          OnPropertyChanged("IsGlobalInterface");
        };
      ports.ItemRemoving += (sender, e) =>
        {
          peerCast.StopListen(e.Item.Listener);
          OnPropertyChanged("Ports");
        };

      yellowPagesList.ItemRemoving += (sender, e) =>
        {
          peerCast.RemoveYellowPage(e.Item.YellowPageClient);
          OnPropertyChanged("YellowPagesList");
        };
    }
  }
}
