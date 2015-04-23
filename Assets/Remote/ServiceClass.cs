using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Remoting;

namespace RemoteClass
{
    public class ObjectConstruct
    {

    }
    public class XObject
    {
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        public void Set(string name, object value)
        {
            if (Values.ContainsKey(name))
                Values.Add(name, value);
            else
                Values[name] = value;
        }

        public object Get(string name)
        {
            if (Values.ContainsKey(name))
                return Values[name];
            return null;
        }
        public T Get<T>(string name, T defaultValue)
        {
            if (Values.ContainsKey(name))
                return (T)(Values[name]);

            return defaultValue;
        }
    }
    public static class XObjectExpand
    {
        public static void Move(this XObject obj, UnityEngine.Vector3 position)
        {
            obj.Set("position", position);
        }
    }
    public class ServiceClass : MarshalByRefObject
    {
        public ServiceClass()
        {
            Console.WriteLine("ServiceClass created.");
            XObject obj = new XObject();


            obj.Move(UnityEngine.Vector3.zero);
            obj.Get("position", UnityEngine.Vector3.zero);
        }

        public string VoidCall()
        {
            Console.WriteLine("VoidCall called.");
            return "You are calling the void call on the ServiceClass.";
        }

        public int GetServiceCode()
        {
            return this.GetHashCode();
        }

        public string TimeConsumingRemoteCall()
        {
            Console.WriteLine("TimeConsumingRemoteCall called.");

            for (int i = 0; i < 20000; i++)
            {
                Console.Write("Counting: " + i.ToString());
                Console.Write("\r");
            }
            return "This is a time-consuming call.";
        }
        int vlaue = 0;
        public int Increment()
        {
            return this.vlaue++;
        }

        public int Sum(int a, int b)
        {
            for (int i = 0; i < 20000; ++i)
            {
                Console.Write("Counting: " + i.ToString());
                Console.Write("\r");
            }

            return a + b;
        }
    }
}
