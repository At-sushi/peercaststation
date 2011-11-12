﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using PeerCastStation.Core;

namespace PeerCastStation.GUI
{
  public class UserInterface
    : MarshalByRefObject,
      IUserInterface
  {
    public string Name
    {
      get { return "PeerCastStation.GUI"; }
    }

    MainForm mainForm;
    Thread mainThread;
    public void Start(IApplication app)
    {
      System.Windows.Forms.Application.EnableVisualStyles();
      mainThread = new Thread(() => {
        mainForm = new MainForm(app.PeerCast);
        System.Windows.Forms.Application.ApplicationExit += (sender, args) => {
          app.Stop();
        };
        System.Windows.Forms.Application.Run(mainForm);
      });
      mainThread.Start();
    }

    public void Stop()
    {
      System.Windows.Forms.Application.Exit();
      mainThread.Join();
    }
  }

  [Plugin(PluginType.UserInterface)]
  public class UserInterfaceFactory
    : MarshalByRefObject,
      IUserInterfaceFactory
  {
    public string Name
    {
      get { return "PeerCastStation.GUI"; }
    }

    public IUserInterface CreateUserInterface()
    {
      var domain = AppDomain.CreateDomain(this.Name);
      return domain.CreateInstanceAndUnwrap(typeof(UserInterface).Assembly.FullName, typeof(UserInterface).FullName) as UserInterface;
    }
  }
}
