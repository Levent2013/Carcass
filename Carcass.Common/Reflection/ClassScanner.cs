using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Carcass.Common.Utility;

namespace Carcass.Common.Reflection
{
    public class ClassScanner
    {
        public ClassScanner(Assembly assembly)
        {
            Throw.IfNullArgument(assembly, "assembly");
            Types = new List<Type> (assembly.GetLoadableTypes());
        }

        public ClassScanner(IEnumerable<Assembly> assemblies)
        {
            Throw.IfNullArgument(assemblies, "assemblies");
            Types = new List<Type>(assemblies.SelectMany(a => a.GetLoadableTypes()));
        }

        public ClassScanner(Assembly assembly, Func<Type, bool> filter)
        {
            Throw.IfNullArgument(assembly, "assembly");
            Types = new List<Type>(assembly.GetLoadableTypes().Where(filter));
        }


        private List<Type> Types { get; set; }

        public IEnumerable<Type> GetPublicClasses()
        {
            return Types.Where(t =>
                t.IsClass
                && !t.IsAbstract
                && !t.IsInterface
                && !t.IsGenericType);
        }

        public IEnumerable<Type> FindClasses(string @namespace)
        {
            return GetPublicClasses().Where(t => !String.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(@namespace));
        }

        public void Process(string @namespace, Action<Type> action)
        {
            Throw.IfNullArgument(action, "action");

            var types = FindClasses(@namespace);
            foreach (var type in types)
            {
                action(type);
            }
        }

    }
}
