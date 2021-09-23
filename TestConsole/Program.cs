using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Controls;
using VTOLVRControlsMapper.Core;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("____");
            Type type = VTOLVRControlsMapper.Main.GetMappingType(new List<Type>() { typeof(VRLever) });
            Console.WriteLine(type.Name);
            //IEnumerable<Type> types = VTOLVRControlsMapper.Main.FindAllDerivedTypes<IControl>();
            //foreach (var item in types)
            //{

                //Console.WriteLine(item.Name);
                //if (item.BaseType != null && item.BaseType.GenericTypeArguments != null)
                //{
                //    foreach (var subitem in item.BaseType.GenericTypeArguments)
                //    {
                //        Console.WriteLine(subitem.Name);
                //    }
                //}
            //}
            //foreach (var item in p)
            //{
            //    Console.WriteLine(item);
            //}
            Console.ReadLine();
        }
        static void CheckBaseType(Type baseType)
        {
            if(!baseType.IsGenericType) { 
               CheckBaseType(baseType.BaseType);
            }
            else
            {
                Console.WriteLine(baseType.Name);
                Console.WriteLine(baseType.GenericTypeArguments[0].Name);
            }
        }
    }
}
