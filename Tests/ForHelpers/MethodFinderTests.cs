using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF.Helpers;
using AEF.Attributes;
using System.Reflection;

namespace AEF.Tests.ForHelpers
{
    class testclass1
    {
        public void HandlerInt(int x) { }
        public void HandlerString(string x) { }

        private void Do1(Action a){}
        public void Do() { }
        public int RetIntHandler(object x) { return 0; }
        public string RetStrHandler(object x) { return ""; }

        public void HandlerHybrid1(string s, int n) { }
        public void HandlerHybrid2(int n, string s) { }
    }

    class testclass2
    {
        public void HandlerHybrid1(string s, int n) { }
        [Equal(0,"test1")]
        public void HandlerHybrid2(string s, int n) { }
        [Equal(0, "test2")]
        public void HandlerHybrid3(string s, int n) { }
        [Equal(0, "test3")]
        public void HandlerHybrid4(string s, int n) { }
        [Equal(0, "test4")]
        public void HandlerHybrid5(string s, int n) { }     
    }

    class testclass3
    {
        public void HandlerHybrid1(string s, int n) { }
        [NotEqual(0, "test1")]
        public void HandlerHybrid2(string s, int n) { }
    }

    class testclass4
    {
        public void Handler1(object o) { }
        public void Handler2(string s) { }
        [Equal(0, "test")]
        public void Handler3(object o,int n) { }
    }
    class testclass5
    {
        public string Handler() { return ""; }
    }
    
    
    public class MethodFinderTests
    {
        [Test]
        public void FindMethodWithoutRestrTest()
        {
            var mf = new MethodFinder(typeof(testclass1));

            object[] param = new object[] { 1 };
            MethodInfo m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerInt");

            param = new object[] { "test" };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerString");


            m = mf.GetMethodByParamsAndReturnValueType(typeof(int), param);
            Assert.AreEqual(m.Name, "RetIntHandler");


            m = mf.GetMethodByParamsAndReturnValueType(typeof(string), param);
            Assert.AreEqual(m.Name, "RetStrHandler");

            param = new object[] { "test", 1 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid1");

            param = new object[] { 1, "test" };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid2");

            param = new object[] { };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "Do");

            param = new object[] { };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(Dictionary<string,int>), param);
            Assert.IsNull(m);

            param = null;
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "Do");

            param = new object[] { new Action(()=>{}) };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.IsNull(m);

        }

        [Test]
        public void FindMethodWithRestrTest()
        {
            
            var mf = new MethodFinder(typeof(testclass2));

            object[] param = new object[] { "Hello,world",0 };
            MethodInfo m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid1");

            param = new object[] { "test1", 0 };
             m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid2");

            param = new object[] { "test2", 0 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid3");

            param = new object[] { "test3", 0 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid4");

            param = new object[] { "test4", 0 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid5");

            mf = new MethodFinder(typeof(testclass3));

            param = new object[] { "Hello,world", 0 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid2");

            param = new object[] { "test1", 0 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "HandlerHybrid1");

        }

        [Test]
        public void FindMethodByTypes()
        {
            var mf = new MethodFinder(typeof(testclass4));

            object[] param = new object[] { "Hello,world" };
            MethodInfo m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "Handler2");

             param = new object[] { 1 };
             m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "Handler1");

            param = new object[] { "test",0 };
            m = mf.GetMethodByParamsAndReturnValueType(typeof(void), param);
            Assert.AreEqual(m.Name, "Handler3");
        }

        [Test]
        public void NoFindBaseActorMethod()
        {
            var mf = new MethodFinder(typeof(testclass5));

            object[] param = new object[0];
            MethodInfo m = mf.GetMethodByParamsAndReturnValueType(typeof(string), param);
            Assert.AreEqual(m.Name, "Handler");
        }
    }
}
