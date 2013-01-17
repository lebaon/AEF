using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF;
using AEF.Tests.Actors;
using System.Threading;
using System.Threading.Tasks;
using AEF.Log;

namespace AEF.Tests.ForLog
{
    public class LogTests
    {

        [Test]
        public void MsgInStoppedActor()
        {
            var tl = new TestLogger();
            
            var actf = new ActorSystem(tl);
            var act = actf.CreateActor<PostStopFaultActor>();
            try
            {
                act.Ask<int>(new stopmsg()).Wait();
            }
            catch { }

            tl.setted = false;
            act.Ask<int>("fgh", 123);

            while (!tl.setted)
            {
                Thread.SpinWait(0);
            }

            Assert.AreEqual("fgh", tl.args[0]);
            Assert.AreEqual(123, tl.args[1]);
            Assert.AreEqual("ProcMsgInStoppedActor", tl.mname);
            Assert.AreEqual(tl.ReturnType, typeof(int));
            Assert.AreEqual(tl.Act, act);
        }


        [Test]
        public void MsgNotHandled()
        {
            var tl = new TestLogger();

            var actf = new ActorSystem(tl);
            var act = actf.CreateActor<PostStopFaultActor>();
            

            tl.setted = false;
            act.Ask<int>("fgh", 123);

            while (!tl.setted)
            {
                Thread.SpinWait(0);
            }

            Assert.AreEqual("fgh", tl.args[0]);
            Assert.AreEqual(123, tl.args[1]);
            Assert.AreEqual("ProcNotHandledMsg", tl.mname);
            Assert.AreEqual(tl.ReturnType, typeof(int));
            Assert.AreEqual(tl.Act, act);
        }


        [Test]
        public void PostStopEception()
        {
            var tl = new TestLogger();

            var actf = new ActorSystem(tl);
            var act = actf.CreateActor<PostStopFaultActor>();

            act.Ask<int>(new Action(() => { throw new Exception(); })).Wait();
            tl.setted = false;
            try
            {
                act.Ask<int>(new stopmsg()).Wait();
            }
            catch { }

            
            

            while (!tl.setted)
            {
                Thread.SpinWait(0);
            }


            Assert.AreEqual("ProcPostStopException", tl.mname);
            Assert.IsNotNull(tl.ex);
            
            Assert.AreEqual(tl.Act, act);
        }

        [Test]
        public void UserActorException()
        {
            var tl = new TestLogger();

            var actf = new ActorSystem(tl);
            var act = actf.CreateActor<PostStopFaultActor>();

            tl.setted = false;

            try
            {
                act.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } }).Wait();
            }
            catch { }


            while (!tl.setted)
            {
                Thread.SpinWait(0);
            }


            Assert.AreEqual("ProcUserActorStoppedByException", tl.mname);
            Assert.IsNotNull(tl.ex);


            Assert.AreEqual(tl.Act, act);
        }

    }
}
