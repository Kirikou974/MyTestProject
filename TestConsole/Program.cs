using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper.Controls;
using VTOLVRControlsMapper.Core;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MethodInfo> methodsInfo = typeof(Interactable).GetMethods().ToList();
            MethodInfo methodInfo = methodsInfo.Find(m =>
                m.GetCustomAttribute<ControlAttribute>().SupportedBehavior == ControllerActionBehavior.HoldOn
            );
            Console.WriteLine(methodInfo.Name);
            Console.ReadLine();
        }
    }
}
