using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF.Helpers;
using System.Threading.Tasks;

namespace AEF.Tests.ForHelpers
{
    
    public class TaskHelperTests
    {
        [Test]
        public void Test1()
        {
            var th = new TaskHelper<int>();
            var t = th.Task;
            
            Assert.IsNotNull(t);
            th.RunTask(10, null);
            Assert.IsTrue(t.IsCompleted);
            Assert.AreEqual(t.Result, 10);

        }
        [Test]
        public void Test2()
        {
            var th = new TaskHelper<int>();
            var t = th.Task;
            Assert.IsNotNull(t);
            var e =new InvalidProgramException();
            th.RunTask(null, e);
            Assert.IsTrue(t.IsFaulted);
            Assert.AreEqual(t.Exception.InnerException, e);
            

        }
    }
}
