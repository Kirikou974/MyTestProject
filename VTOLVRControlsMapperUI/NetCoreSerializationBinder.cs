using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Valve.Newtonsoft.Json.Serialization;

namespace VTOLVRControlsMapperUI
{
    //Thanks to https://stackoverflow.com/questions/50190568/net-standard-4-7-1-could-not-load-system-private-corelib-during-serialization
    //.Net core application generates JSON with system.private.corelib classes
    //In game, we expect to have mscorlib (old name of system.private.corelib)
    //This class helps renaming system.private.corelib to mscorlib when generating the JSON file from the UI
    public class NetCoreSerializationBinder : DefaultSerializationBinder
    {
        private static readonly Regex regex = new Regex(
            @"System\.Private\.CoreLib(, Version=[\d\.]+)?(, Culture=[\w-]+)(, PublicKeyToken=[\w\d]+)?");

        private static readonly ConcurrentDictionary<Type, (string assembly, string type)> cache =
            new ConcurrentDictionary<Type, (string, string)>();

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            base.BindToName(serializedType, out assemblyName, out typeName);

            if (cache.TryGetValue(serializedType, out var name))
            {
                assemblyName = name.assembly;
                typeName = name.type;
            }
            else
            {
                if (assemblyName.Contains("System.Private.CoreLib", StringComparison.OrdinalIgnoreCase))
                    assemblyName = regex.Replace(assemblyName, "mscorlib");

                if (typeName.Contains("System.Private.CoreLib", StringComparison.OrdinalIgnoreCase))
                    typeName = regex.Replace(typeName, "mscorlib");

                cache.TryAdd(serializedType, (assemblyName, typeName));
            }
        }
    }
}
