using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyOnChange.Model
{
  enum ActionType { CopyOnChange, Help };

  class ParametersModel
  {
    public OptionSet OptionSet { get; set; }
    public ActionType Action { get; set; }
    public string SourceFile { get; set; }
    public string TargetFile { get; set; }
  }
}
