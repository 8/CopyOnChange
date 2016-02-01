using CopyOnChange.Factory;
using CopyOnChange.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CopyOnChange
{
  interface IProgram
  {
    int Run(string[] args);
  }

  class Program : IProgram
  {
    private readonly IParametersFactory ParametersFactory;

    public Program(IParametersFactory parametersFactory)
    {
      this.ParametersFactory = parametersFactory;
    }

    #region Methods
    private void PrintHelp(ParametersModel p)
    {
      p.OptionSet.WriteOptionDescriptions(Console.Out);
    }

    private void CopyOnChange(ParametersModel p)
    {
      CopyOnChange(p.SourceFile, p.TargetFile);
    }

    private void CopyOnChange(string sourceFile, string targetFile)
    {
      if (string.IsNullOrEmpty(sourceFile))
        throw new ArgumentException("Sourcefile missing!");
      if (string.IsNullOrEmpty(targetFile))
        throw new ArgumentException("Targetfile missing!");

      using (var fsw = new FileSystemWatcher())
      {
        fsw.Changed += (o, e) =>
        {
          Console.Write("copying file...");
          try {
            File.Copy(sourceFile, targetFile, true);
            Console.WriteLine("done.");
          }
          catch { Console.WriteLine("failed."); }
        };

        string directoryPath = Path.GetDirectoryName(sourceFile);
        if (directoryPath == string.Empty)
          directoryPath = ".";

        fsw.NotifyFilter        = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
        fsw.Path                = directoryPath;
        fsw.Filter              = Path.GetFileName(sourceFile);
        fsw.EnableRaisingEvents = true;

        Console.WriteLine(string.Format("Waiting for change in file: '{0}' to copy to '{1}'", sourceFile, targetFile));

        /* keep blocking until we get terminated */
        while (true)
        {
          Thread.Sleep(1000);
        }
      }
    }

    public int Run(string[] args)
    {
      var p = this.ParametersFactory.GetParameters(args);
      return Run(p);
    }

    private int Run(ParametersModel p)
    {
      switch(p.Action)
      {
        case ActionType.Help: PrintHelp(p); break;
        case ActionType.CopyOnChange: CopyOnChange(p); break;
      }

      return 0;
    }

    #endregion
  }
}
