using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF.Helpers;

namespace AEF.Tests.ForHelpers
{
    
    public class ReflectionHelperTests
    {
        [Test]
        public void ObjectArrayToTypesTest()
        {
            
            object[] obj = new object[5];
            obj[0] = new object();
            obj[1] = new int();
            obj[2] = new float();
            obj[3] = new Action(() => { });
            obj[4] = new List<int>();

            Type[] types = obj.GetTypes();

            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(types[i], obj[i].GetType());
            }
        }
        [Test]
        public void CanStoreTest()
        {

            object[] obj = new object[5];
            obj[0] = new object();
            obj[1] = new int();
            obj[2] = new float();
            obj[3] = new Action(() => { });
            obj[4] = new List<int>();

            Type[] types1 = obj.GetTypes();
            Type[] types2 = new Type[] { typeof(object), typeof(int), typeof(float), typeof(Delegate), typeof(IList<int>) };
            Assert.IsTrue(types2.CanStore(types1));
            Assert.IsFalse(types1.CanStore(types2));

            

            Signature s1 = new Signature() { ParameterTypes = types1, ReturnType = typeof(Int16) };
            Signature s2 = new Signature() { ParameterTypes = types2, ReturnType = typeof(object) };

            Assert.IsTrue(s2.CanStore(s1));
            Assert.IsFalse(s1.CanStore(s2));

            types1 = new Type[0];
            types2 = new Type[0];

            Assert.IsTrue(types2.CanStore(types1));
            Assert.IsTrue(types1.CanStore(types2));

             s1 = new Signature() { ParameterTypes = types1, ReturnType = typeof(void) };
             s2 = new Signature() { ParameterTypes = types2, ReturnType = typeof(void) };

             Assert.IsTrue(s2.CanStore(s1));
             Assert.IsTrue(s1.CanStore(s2));
        }

        public int testmethod1(Action act)
        {
            act();
            return 0;
        }

        [Test]
        public void GetSignatureTest()
        {

            Signature sig = this.GetType().GetMethod("testmethod1").GetSignature();

            Assert.AreEqual(sig.ReturnType, typeof(int));
            Assert.IsTrue(sig.ParameterTypes.Length == 1);
            Assert.AreEqual(sig.ParameterTypes[0], typeof(Action));
        }

        [Test]
        public void AsignableFromVoidTest()
        {
            Assert.IsTrue(typeof(object).IsAssignableFrom(typeof(void)));

        }

        [Test]
        public void DistanceTest()
        {
            

            Type[] types1 = new Type[] { typeof(object), typeof(int), typeof(float), typeof(Action), typeof(List<int>) };
            Type[] types2 = new Type[] { typeof(object), typeof(int), typeof(float), typeof(Delegate), typeof(IList<int>) };
            Assert.AreEqual(types1.Distance(types2), 2);
            Assert.AreEqual(types2.Distance(types1), 2);
            Assert.AreEqual(types1.Distance(types1), 0);

            Signature s1 = new Signature() { ParameterTypes = types1, ReturnType = typeof(Int16) };
            Signature s2 = new Signature() { ParameterTypes = types2, ReturnType = typeof(object) };

            Assert.AreEqual(s1.Distance(s2), 3);
            Assert.AreEqual(s2.Distance(s1), 3);
            Assert.AreEqual(s1.Distance(s1), 0);

            s1 = new Signature() { ParameterTypes = types1, ReturnType = typeof(Int16) };
            s2 = new Signature() { ParameterTypes = types2, ReturnType = typeof(Int16) };

            Assert.AreEqual(s1.Distance(s2), 2);
            Assert.AreEqual(s2.Distance(s1), 2);
        }
    
    }
}
