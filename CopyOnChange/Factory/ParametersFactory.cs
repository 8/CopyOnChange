using CopyOnChange.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyOnChange.Factory
{
  interface IParametersFactory
  {
    ParametersModel GetParameters(string[] args);
  }

  class ParametersFactory : IParametersFactory
  {
    public ParametersModel GetParameters(string[] args)
    {
      var p = new ParametersModel();
      var optionSet = new Mono.Options.OptionSet();

      optionSet.Add("s=|source-file", "specify the source file to watch", s => p.SourceFile = s);
      optionSet.Add("t=|target-file", "specify the target file path", s => p.TargetFile = s);
      optionSet.Add("?|h|help", "prints this description", s => p.Action = ActionType.Help);
      p.OptionSet = optionSet;

      optionSet.Parse(args);

      return p;
    }
  }
}
