using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cazzar.StreamDeck.VTubeStudio
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventRegistrationAttribute : Attribute
    {
        
    }

    public class EventRegistrar
    {
        private static bool _called = false;

        public static void CallAll()
        {
            if (_called)
            {
                return;
            }
            foreach (var type in typeof(EventRegistrar).Assembly.GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static).Where(mtd => mtd.GetCustomAttribute<EventRegistrationAttribute>() != null);

                foreach (var method in methods)
                {
                    method.Invoke(null, null);
                }
            }

            _called = true;
        }
    }
}
