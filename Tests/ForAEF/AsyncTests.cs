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
    public class AsyncTests
    {
        [Test]
        public void AsyncStopActor()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<AsyncTestActor>("parent");
            var act2 = act1.Ask<ActorRef>(new createchildactormsg()).Result;

            Assert.IsNotNull(actf.FindActorByPath("\\user\\parent\\child"));
            
            bool f = false;

            act1.Tell(new stopchildex() { child = act2, 
                childstopped = () => { f = true; } });

            while (!f)
            {
                Thread.SpinWait(0);
            }

            Assert.IsNull(actf.FindActorByPath("\\user\\parent\\child"));

        }


        [Test]
        public void AsyncRestartActor()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<AsyncTestActor>("parent");
            var act2 = act1.Ask<ActorRef>(new createchildactormsg()).Result;

            act2.Ask<int>(new setstate() { x = 15 }).Wait();
            Assert.AreEqual(15, act2.Ask<int>(new getstate()).Result);


            bool f = false;

            act1.Tell(new restartchildex()
            {
                child = act2,
                childrestarted = (e) => { f = true; }
            });

            while (!f)
            {
                Thread.SpinWait(0);
            }

            Assert.AreEqual(0, act2.Ask<int>(new getstate()).Result);

        }

        [Test]
        public void PendingAsk()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<AsyncTestActor>("parent");

            act1.Ask<int>(new setstate() { x = 15 }).Wait();
            Assert.AreEqual(15, act1.Ask<int>(new getstate()).Result);

            var tsk = act1.Ask<int>(new getstatepend());

            act1.Ask<int>(new setstate() { x = 10 }).Wait();
            Assert.AreEqual(10, act1.Ask<int>(new getstate()).Result);

            act1.Tell(new returnask());
            tsk.Wait();

            Assert.AreEqual(10, tsk.Result);
        }
    
    
    }
}
