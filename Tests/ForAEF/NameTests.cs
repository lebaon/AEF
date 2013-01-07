using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF;
using AEF.Tests.Actors;
using System.Threading;
using System.Threading.Tasks;

namespace AEF.Tests.ForAEF
{
    public class NameTests
    {
        [Test]
        public void ActorMayHaveName()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<SimpleTestActor>("myactor");
            Assert.AreEqual("myactor", act.Name);
        }

        [Test]
        public void AllActorHasName()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<SimpleTestActor>();
            Assert.IsTrue(act.Name.Length > 0);
            Console.WriteLine("Created Actor name: {0}", act.Name);
        }

        [Test]
        public void UserActorDontHaveEmptyName()
        {

            var actf = new ActorSystem();
            var act = actf.CreateActor<SimpleTestActor>("");
            Assert.AreNotEqual("", act.Name);
        }

        [Test]
        public void ActorDontHaveSlashInName()
        {
            bool f = false;
            var actf = new ActorSystem();
            try
            {
                var act = actf.CreateActor<SimpleTestActor>("\\");
            }
            catch { f = true; }


            Assert.IsTrue(f);
            f = false;
            try
            {
                var act = actf.CreateActor<SimpleTestActor>("/");
            }
            catch { f = true; }


            Assert.IsTrue(f);

        }

        [Test]
        public void ActorHaveFullName()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<SimpleTestActor>("myactor");
            Assert.AreEqual("\\user\\myactor", act.FullName);
        }

        [Test]
        public void ActorFullNameIsIerarhy()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<IerarhyTestActor>("myactor");

            var act2 = act.Ask<ActorRef>(new createchildactormsg()).Result;

            Assert.AreEqual("\\user\\myactor\\child", act2.FullName);
        }


        [Test]
        public void FindActorByPath()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<IerarhyTestActor>("myactor");

            var act2 = act.Ask<ActorRef>(new createchildactormsg()).Result;

            var fact = actf.FindActorByPath("\\user\\myactor\\child");

            Assert.NotNull(fact);
            Assert.AreEqual(act2, fact);
        }

        [Test]
        public void FindActorByPathErr()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<IerarhyTestActor>("myactor");

            var act2 = act.Ask<ActorRef>(new createchildactormsg()).Result;

            var fact = actf.FindActorByPath("\\user\\myactor\\child3");

            Assert.IsNull(fact);
            
        }
    }
}
