using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF.Helpers;
using AEF.Tests.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace AEF.Tests.ForHelpers
{
    public class ActorPathTests
    {
        [Test]
        public void CreateFromStringAndToString()
        {
            string path ="abc\\def";
            var ap = new ActorPath(path);

            Assert.AreEqual(path, ap.ToString());

        }

        [Test]
        public void CreateFromParentAndToString()
        {
            var ap1 = new ActorPath("");
            var ap2 = new ActorPath("test", ap1);

            Assert.AreEqual("\\test", ap2.ToString());
        }

        [Test]
        public void EqualsTest()
        {
            var ap1 = new ActorPath("");
            var ap2 = new ActorPath("test", ap1);
            var ap3 = new ActorPath("\\test");

            Assert.IsTrue(ap2 == ap3);
            Assert.IsFalse(ap1 == ap3);

        }

        [Test]
        public void GetChildNameTest()
        {
            var ap1 = new ActorPath("");
            var ap3 = new ActorPath("\\test");
            Assert.AreEqual(ap1.GetChildName(ap3), "test");

        }
    }
}
