using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAtrributes
{
    public class Assert
    {
        public static void AreEqual(object a, object b)
        {
            if (a == null | b == null) throw new Exception(String.Format("Обьекты не равны, {0} и {1}", a, b));
            if (!a.Equals(b)) throw new Exception(String.Format("Обьекты не равны, {0} и {1}", a, b));
        }
        public static void IsFalse(bool b)
        {
            if (b) throw new Exception("Выражение не равно False");
        }
        public static void IsTrue(bool b)
        {
            if (!b) throw new Exception("Выражение не равно True");
        }
        public static void IsNotNull(object o)
        {
            if (o == null) throw new Exception("Выражение равно Null");
        }
        public static void IsNull(object o)
        {
            if (o != null) throw new Exception("Выражение не равно Null");
        }
    }
}
