using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AEF;
using Tests.Actors;
using System.Threading;
using System.Threading.Tasks;


namespace Tests
{

    public class AEFBasicTests
    {
        [Test]
        public void SimpleTestTell()
        {
            bool f = false;


            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();
            
            var m = new acttestmsg()
            {
                act = () =>
                {
                    f = true;
                    return 0;
                },
                msg = "test message"
            };

            act.Tell(m);
            while (!f)
            {
                Thread.SpinWait(0);
            }
            Console.WriteLine("test complete");

        }

        [Test]
        public void SimpleTestAskContX()
        {
            bool f = false;

            int res = 0;

            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new acttestmsg()
            {
                act = () =>
                {

                    return 10;
                },
                msg = "test message"
            };

            Task<int> tsk = null;
            tsk = act.Ask<int>((x) =>
           {
               res = x;
               if (tsk == null) return;
               if (tsk.IsCompleted) return;
               f = true;
           }, m);

            tsk.Wait();

            Assert.IsTrue(f);
            Assert.AreEqual(10, res);
            Assert.AreEqual(10, tsk.Result);
            Console.WriteLine("test complete");
        }

        [Test]
        public void SimpleTestAskContXErr()
        {
            bool f = false;

            int res = 0;

            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new acttestmsg()
            {
                act = () =>
                {

                    return 10/res;
                },
                msg = "test message"
            };

            Task<int> tsk = null;
            tsk = act.Ask<int>((x) =>
            {
                res = x;
                if (tsk == null) return;
                if (tsk.IsCompleted) return;
                f = true;
            }, m);

            try
            {
                tsk.Wait();
            }
            catch (AggregateException ae) { }

            Assert.IsFalse(f);
            Assert.AreEqual(0, res);
            Assert.IsNotNull(tsk.Exception);
            Console.WriteLine("test complete");
        }

        [Test]
        public void SimpleTestAskContXEx()
        {
            bool f = false;

            Exception e = null;
            int res = 0;

            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new acttestmsg()
            {
                act = () =>
                {

                    return 10 ;
                },
                msg = "test message"
            };

            Task<int> tsk = null;
            tsk = act.Ask<int>((x, ex) =>
            {
                if (e == null) res = x;

                e = ex;
                if (tsk == null) return;
                if (tsk.IsCompleted | tsk.IsFaulted) return;
                f = true;
            }, m);

            
            tsk.Wait();
            


            Assert.IsTrue(f);
            Assert.IsNull(tsk.Exception);
            Assert.AreEqual(10, res);
            Assert.IsNull(e);
            Console.WriteLine("test complete");
        }

        [Test]
        public void SimpleTestAskContXExErr()
        {
            bool f = false;

            Exception e = null;
            int res = 0;

            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new acttestmsg()
            {
                act = () =>
                {

                    return 10 / res;
                },
                msg = "test message"
            };

            Task<int> tsk = null;
            tsk = act.Ask<int>((x, ex) =>
            {
                if (ex == null) res = x;

                e = ex;
                if (tsk == null) return;
                if (tsk.IsCompleted | tsk.IsFaulted) return;
                f = true;
            }, m);

            try
            {
                tsk.Wait();
            }
            catch (AggregateException ae) { }


            Assert.IsTrue(f);
            Assert.IsNotNull(tsk.Exception);
            Assert.AreEqual(0, res);
            Assert.IsNotNull(e);
            Console.WriteLine("test complete");
        }

        [Test]
        public void SimpleTestAskWithoutCont()
        {
            //int res = 0;

            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new acttestmsg()
            {
                act = () =>
                {

                    return 10 ;
                },
                msg = "test message"
            };

            Task<int> tsk = null;
            tsk = act.Ask<int>(m);

            tsk.Wait();

            Assert.AreEqual(10, tsk.Result);
        }

        [Test]
        public void SimpleTestAskWithoutContErr()
        {
            int res = 0;

            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new acttestmsg()
            {
                act = () =>
                {

                    return 10/res;
                },
                msg = "test message"
            };

            Task<int> tsk = null;
            tsk = act.Ask<int>(m);

            try
            {
                tsk.Wait();
            }
            catch (AggregateException ae) { }

            Assert.IsNotNull(tsk.Exception);
        }


        [Test]
        public void SimpleTestStopActor()
        {
            bool f = false;


            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();
            acts.StopActor(act);
            var m = new acttestmsg()
            {
                act = () =>
                {
                    f = true;
                    return 0;
                },
                msg = "test message"
            };

            var tsk = act.Ask<int>(m);
            try
            {
                tsk.Wait();
            }
            catch (AggregateException e) { }
            
            Assert.IsFalse(f);
            Assert.IsNotNull(tsk.Exception);
            Console.WriteLine("test complete");

        }

        [Test]
        public void SimpleTestRestartActor()
        {
            


            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();

            var m = new statemsg() { newstate = 10 };

            var tsk = act.Ask<int>(m);
            tsk.Wait();

            m = new statemsg() { newstate = 10 };

            tsk = act.Ask<int>(m);
            tsk.Wait();

            Assert.AreEqual(10, tsk.Result);

            acts.RestartActor(act,null);

            m = new statemsg() { newstate = 10 };

            tsk = act.Ask<int>(m);
            tsk.Wait();

            Assert.AreEqual(0, tsk.Result);

            Console.WriteLine("test complete");

        }
    }
}
