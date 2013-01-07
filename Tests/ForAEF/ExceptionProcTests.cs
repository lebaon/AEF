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
    class ExceptionProcTests
    {
        [Test]
        public void RootActorExceptionProcStrategyIsStop()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<SimpleTestActor>();

            bool f = false;

            var tsk = act.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk.Wait(); }
            catch { }

            tsk = act.Ask<int>(new statemsg() { newstate = 10 });
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsTrue(f);


        }

        [Test]
        public void DefaultExProcStrategyIsExalation()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<IerarhyTestActor>();

            bool f = false;

            var tsk = act.Ask<ActorRef>(new createchildactormsg());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);

            var tsk2 = act2.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk2.Wait(); }
            catch { }

            tsk2 = act.Ask<int>(new statemsg() { newstate = 10 });
            try { tsk2.Wait(); }
            catch { f = true; }

            Assert.IsTrue(f);

        }

        [Test]
        public void ExProcStrategyIsStop()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<ExceptionActor>();

            var tsk1 = act1.Ask<ActorRef>(new createchildactormsg());
            tsk1.Wait();
            var act2 = tsk1.Result;

            var tsk = act1.Ask<int>(new seteh() { eh = () => { return ExceptionDecision.Stop; } });
            tsk.Wait();

            tsk = act2.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk.Wait(); }
            catch { }

            bool f = false;

            tsk = act2.Ask<int>(new getstate());
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsTrue(f);

            f = false;

            tsk = act1.Ask<int>(new getstate());
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsFalse(f);
        }

        [Test]
        public void ExProcStrategyIsRestart()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<ExceptionActor>();

            var tsk1 = act1.Ask<ActorRef>(new createchildactormsg());
            tsk1.Wait();
            var act2 = tsk1.Result;

            var tsk = act1.Ask<int>(new seteh() { eh = () => { return ExceptionDecision.Restart; } });
            tsk.Wait();

            tsk = act2.Ask<int>(new setstate() { x = 37 });
            tsk.Wait();

            tsk = act2.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk.Wait(); }
            catch { }

            bool f = false;

            f = false;

            tsk = act1.Ask<int>(new getstate());
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsFalse(f);

            tsk = act2.Ask<int>(new getstate());
            tsk.Wait();

            Assert.AreEqual(0, tsk.Result);
        }

        [Test]
        public void ExProcStrategyIsResume()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<ExceptionActor>();

            var tsk1 = act1.Ask<ActorRef>(new createchildactormsg());
            tsk1.Wait();
            var act2 = tsk1.Result;

            var tsk = act1.Ask<int>(new seteh() { eh = () => { return ExceptionDecision.Resume; } });
            tsk.Wait();

            tsk = act2.Ask<int>(new setstate() { x = 37 });
            tsk.Wait();

            tsk = act2.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk.Wait(); }
            catch { }

            bool f = false;

            f = false;

            tsk = act1.Ask<int>(new getstate());
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsFalse(f);

            tsk = act2.Ask<int>(new getstate());
            tsk.Wait();

            Assert.AreEqual(37, tsk.Result);
        }


        [Test]
        public void ExeptionInExProcEqualsExcalation()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<ExceptionActor>();

            var tsk1 = act1.Ask<ActorRef>(new createchildactormsg());
            tsk1.Wait();
            var act2 = tsk1.Result;

            var tsk = act1.Ask<int>(new seteh() { eh = () => { throw new Exception(); } });
            tsk.Wait();

            tsk = act2.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk.Wait(); }
            catch { }

            bool f = false;

            tsk = act2.Ask<int>(new getstate());
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsTrue(f);

            f = false;

            tsk = act1.Ask<int>(new getstate());
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsTrue(f);
        }

        [Test]
        public void OnParentExeptionChildsSuspendedToo()
        {
            bool f = false;
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<ExceptionActor>();

            var tsk1 = act1.Ask<ActorRef>(new createchildactormsg());
            tsk1.Wait();
            var act2 = tsk1.Result;

            tsk1 = act2.Ask<ActorRef>(new createchildactormsg());
            tsk1.Wait();
            var act3 = tsk1.Result;

            var tsk = act1.Ask<int>(new seteh() { eh = () => {

                var tskx = act3.Ask<int>(new getstate());
                f = tskx.Wait(100);
                return ExceptionDecision.Stop;
            } });
            tsk.Wait();

            tsk = act2.Ask<int>(new acttestmsg() { act = () => { throw new Exception(); } });
            try { tsk.Wait(); }
            catch { }

            Assert.IsFalse(f);

        }


        [Test]
        public void FaultOnRestartStoppingActor()
        {
            bool f = false;
            var acts = new ActorSystem();

            var act = acts.CreateActor<RestartFaultActor>();
            try
            {
                acts.RestartActor(act);
            }
            catch  { }

            var tsk = act.Ask<int>(new acttestmsg() { act = () => { return 0; } });
            try { tsk.Wait(); }
            catch { f = true; }

            Assert.IsTrue(f);
           


        }
    }
}
