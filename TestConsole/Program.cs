using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<Type> controlTypes = ControlsHelper.GetDerivedTypes<IControl>();
            Type vtolMod = typeof(VTOLMOD);
            MethodInfo[] methodInfos = vtolMod.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            MethodInfo findObjectsOfType = methodInfos.First(m => m.IsGenericMethod && m.Name == nameof(VTOLMOD.FindObjectsOfType));
            Console.WriteLine(findObjectsOfType.Name);
            //MethodInfo info = vtolMod.Getge(nameof(VTOLMOD.FindObjectsOfType), BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(UnityEngine.Object) }, null);
            //Console.WriteLine(info.Name);
            //foreach (var item in controlTypes)
            //{
            //    Console.WriteLine("-----------------------");
            //    Console.WriteLine(item.Name);
            //    Console.WriteLine(item.BaseType.GenericTypeArguments[0].Name);
            //    //foreach (var subitem in item.BaseType.GenericTypeArguments)
            //    //{
            //    //    Console.WriteLine(subitem.Name);
            //    //} 
            //}
            Console.ReadLine();
        }
    }
}