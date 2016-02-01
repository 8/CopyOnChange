using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TinyIoC;

namespace CopyOnChange
{

  static class TinyIoCExtensions
  {
    public static object Resolve(this TinyIoCContainer ioc, string typeName)
    {
      var type = Type.GetType(typeName);
      return ioc.Resolve(type);
    }

    [Flags]
    public enum RegisterTypes
    {
      AsInterfaceTypes = 1,
      AsObjects = 2
    }

    [Flags]
    public enum RegisterOptions
    {
      AsMultiInstance = 1,
      AsSingleton = 2
    }

    public static IEnumerable<Type> GetInterfacesFromNamespace(string @namespace, Assembly[] assemblies = null)
    {
      if (assemblies == null)
        assemblies = new[] { Assembly.GetExecutingAssembly() };

      var interfaces = new List<Type>();
      foreach (var a in assemblies)
        foreach (var t in a.SafeGetTypes())
        {
          if (t.IsInterface && t.Namespace == @namespace)
            interfaces.Add(t);
        }
      return interfaces;
    }

    public static Type GetImplementingType(Type @interface, Assembly[] assemblies = null)
    {
      if (assemblies == null)
        assemblies = new[] { Assembly.GetExecutingAssembly() };

      var implementingTypes = new List<Type>();

      foreach (var a in assemblies)
        foreach (var t in a.SafeGetTypes())
        {
          if (t.IsClass && t.GetInterface(@interface.FullName) != null)
            implementingTypes.Add(t);
        }

      return implementingTypes.OrderBy(t => t.FullName.Length).FirstOrDefault();
    }

    public static void RegisterInterfaceImplementations(this TinyIoCContainer container,
                                                         string @namespace,
                                                         RegisterOptions options = RegisterOptions.AsMultiInstance,
                                                         RegisterTypes registerTypes = RegisterTypes.AsInterfaceTypes,
                                                         Assembly[] assemblies = null)
    {
      if (assemblies == null)
        assemblies = new[] { Assembly.GetExecutingAssembly() };

      var interfaces = GetInterfacesFromNamespace(@namespace, assemblies);

      foreach (var i in interfaces)
      {
        var implementation = GetImplementingType(i, assemblies);
        if ((registerTypes & RegisterTypes.AsObjects) == RegisterTypes.AsObjects)
        {
          var o = container.Register(typeof(object), implementation, i.Name);
          if (options == RegisterOptions.AsSingleton)
            o.AsSingleton();
          else
            o.AsMultiInstance();
        }
        if ((registerTypes & RegisterTypes.AsInterfaceTypes) == RegisterTypes.AsInterfaceTypes)
        {
          var o = container.Register(i, implementation);
          if (options == RegisterOptions.AsSingleton)
            o.AsSingleton();
          else
            o.AsMultiInstance();
        }
      }
    }
  }
}
