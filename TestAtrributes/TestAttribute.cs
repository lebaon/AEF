using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAtrributes
{

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
    }
}
