﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using PeerCastStation.Core;
using PeerCastStation.WPF.Commons;

namespace PeerCastStation.WPF
{
  class LogViewModel : ViewModelBase
  {
    private readonly LogWriter guiWriter = new LogWriter(1000);
    private readonly Timer timer;
    private TextWriter logFileWriter;

    private int logLevel;
    public int LogLevel
    {
      get { return logLevel; }
      set
      {
        SetProperty("LogLevel", ref logLevel, value,
          () => Logger.Level = GetLogLevel(value));
      }
    }

    private bool isOutputToGui;
    public bool IsOutputToGui
    {
      get { return isOutputToGui; }
      set
      {
        SetProperty("IsOutputToGui", ref isOutputToGui, value,
          () => RefreshWriter(guiWriter, value));
      }
    }

    private bool isOutputToConsole;
    public bool IsOutputToConsole
    {
      get { return isOutputToConsole; }
      set
      {
        SetProperty("IsOutputToConsole", ref isOutputToConsole, value,
          () => RefreshWriter(Console.Error, value));
      }
    }

    private bool isOutputToFile;
    public bool IsOutputToFile
    {
      get { return isOutputToFile; }
      set
      {
        SetProperty("IsOutputToFile", ref isOutputToFile, value, () =>
        {
          if (logFileWriter != null)
          {
            RefreshWriter(logFileWriter, value);
          }
        });
      }
    }

    private string outputFileName;
    public string OutputFileName
    {
      get { return outputFileName; }
      set
      {
        SetProperty("OutputFileName", ref outputFileName, value, () =>
        {
          if (logFileWriter != null)
          {
            Logger.RemoveWriter(logFileWriter);
            logFileWriter.Close();
            logFileWriter = null;
          }
          logFileWriter = GetLogFileWriter(value);
          if (logFileWriter != null && IsOutputToFile)
            Logger.AddWriter(logFileWriter);
        });
      }
    }

    private string log;
    public string Log
    {
      get { return log; }
      private set { SetProperty("Log", ref log, value); }
    }

    private readonly ICommand clear;
    public ICommand Clear
    {
      get { return clear; }
    }

    public LogViewModel()
    {
      clear = new Command(() =>
      {
        guiWriter.Clear();
        Log = "";
      });

      var sc = SynchronizationContext.Current;
      timer = new Timer(o => sc.Send(p =>
      {
        Log = guiWriter.ToString();
      }, null), null, 1000, 1000);
    }

    private LogLevel GetLogLevel(int value)
    {
      switch (value)
      {
        case 0: return Core.LogLevel.None;
        case 1: return Core.LogLevel.Fatal;
        case 2: return Core.LogLevel.Error;
        case 3: return Core.LogLevel.Warn;
        case 4: return Core.LogLevel.Info;
        case 5: return Core.LogLevel.Debug;
        default: return Core.LogLevel.None;
      }
    }

    private void RefreshWriter(TextWriter writer, bool active)
    {
      Logger.RemoveWriter(writer);
      if (active)
      {
        Logger.AddWriter(writer);
      }
    }

    private StreamWriter GetLogFileWriter(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return null;
      try
      {
        return File.AppendText(fileName);
      }
      catch (UnauthorizedAccessException) { return null; }
      catch (ArgumentException) { return null; }
      catch (PathTooLongException) { return null; }
      catch (DirectoryNotFoundException) { return null; }
      catch (NotSupportedException) { return null; }
      catch (IOException) { return null; }
    }
  }
}
