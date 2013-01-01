using System;
using System.Linq;
using System.Reflection;

namespace AEF.Helpers
{
    public static class ReflectionHelper
    {

        public static bool ContainsMethod(this MethodInfo[] methods, MethodInfo key)
        {
            if (methods.Length == 0) return false;
            return methods.Select(
                (x) =>
                {
                    return x.GetSignature().Equal(key.GetSignature()) &
                        x.Name == key.Name;
                }).Aggregate(
                (a, b) => { return a | b; });
        }
        
        public static Type[] GetTypes(this object[] args)
        {
            return args.Select((arg) => arg != null ? arg.GetType() : typeof(void)).ToArray();
        }

        public static bool CanStore(this Type[] store, Type[] value)
        {
            if (store.Length != value.Length) return false;
            if (store.Length == 0) return true;
            return store.Select((x, i) => { return x.IsAssignableFrom(value[i]); }).Aggregate((a, b) => { return a && b; });
        }
        public static bool CanStore(this Signature store, Signature value)
        {
            return store.ReturnType.IsAssignableFrom(value.ReturnType) && store.ParameterTypes.CanStore(value.ParameterTypes);
        }

        public static Signature GetSignature(this MethodInfo method)
        {
            Signature sig = new Signature();

            sig.ReturnType = method.ReturnType;
            sig.ParameterTypes = method.GetParameters().Select((p) => { return p.ParameterType; }).ToArray();

            return sig;
        }

        public static int Distance(this Type[] one, Type[] two)
        {
            if (one.Length != two.Length) return -1;
            if (one.Length == 0) return 0;
            return one.Length - one.Select(
                (t, i) => { return t == two[i] ? 1 : 0; }).Aggregate(
                (a, b) => { return a + b; });
        }

        public static int Distance(this Signature one, Signature two)
        {
            return one.ParameterTypes.Distance(two.ParameterTypes) + (one.ReturnType == two.ReturnType ? 0 : 1);
        }
    }
}
