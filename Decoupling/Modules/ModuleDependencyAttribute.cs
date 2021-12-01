using System;

namespace Decoupling.Module
{
    public class ModuleDependencyAttribute : Attribute
    {
        public string[] ModuleNames { get; set; }

        public ModuleDependencyAttribute(params string[] moduleNames)
        {
            ModuleNames = moduleNames;
        }
    }
}
