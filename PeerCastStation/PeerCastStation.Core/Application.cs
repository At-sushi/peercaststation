﻿using System;
using System.Collections.Generic;

namespace PeerCastStation.Core
{
  public abstract class PeerCastApplication
  {
    private static PeerCastApplication current;
    public static PeerCastApplication Current {
      get { return current; }
      set { current = value; }
    }
    public abstract PecaSettings Settings { get; }
    public abstract IEnumerable<IPlugin> Plugins { get; }
    public abstract PeerCast PeerCast { get; }
    public abstract string BasePath { get; }
    public abstract void Stop(int exit_code);
    public void Stop()
    {
      Stop(0);
    }

    public abstract void SaveSettings();
    public PeerCastApplication()
    {
      if (current==null) current = this;
    }
  }
}
