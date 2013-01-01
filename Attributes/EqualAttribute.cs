using System;


namespace AEF.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class EqualAttribute:Attribute
    {
        public object EqualValue { get; set; }
        public int ParamNumber { get; set; }
        public EqualAttribute(int n,object value)
        {
            EqualValue = value;
            ParamNumber = n;
        }
    }
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class NotEqualAttribute : Attribute
    {
        public object EqualValue { get; set; }
        public int ParamNumber { get; set; }
        public NotEqualAttribute(int n, object value)
        {
            EqualValue = value;
            ParamNumber = n;
        }
    }
}
