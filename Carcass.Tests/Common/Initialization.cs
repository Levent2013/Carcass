using System;
using System.Reflection;
using Autofac;

namespace Carcass.Tests.Common
{
    public static class Initialization
    {
        public static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        // known bug in VS 2012 - it does not read assemblyBinding from config file, so do it manually
        public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            if (name.Name == "Autofac")
            {
                return typeof(ContainerBuilder).Assembly;
            }

            return null;
        }
    }


}
