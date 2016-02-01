using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyOnChange
{
  class EntryPoint
  {
    static int Main(string[] args)
    {
      var container = new TinyIoC.TinyIoCContainer();
      container.RegisterInterfaceImplementations("CopyOnChange");
      container.RegisterInterfaceImplementations("CopyOnChange.Factory");

      var program = container.Resolve<IProgram>();
      try { return program.Run(args); }
      catch (Exception ex)
      {
        Console.WriteLine("The following error occurred:{0}{1}", Environment.NewLine, ex);
        return 1;
      }
    }
  }
}
