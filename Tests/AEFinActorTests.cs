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
    public class AEFinActorTests
    {
        [Test]
        public void StopActorTest()
        {
            bool f = false;


            var acts = new ActorSystem();
            var act = acts.CreateActor<SimpleTestActor>();
            var sm = new stopmsg();
            var tsk = act.Ask<int>(sm);
            tsk.Wait();
            var m = new acttestmsg()
            {
                act = () =>
                {
                    f = true;
                    return 0;
                },
                msg = "test message"
            };

            tsk = act.Ask<int>(m);
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
        public void CreateActorTest()
        {



            var acts = new ActorSystem();
            var act = acts.CreateActor<IntoTestActor>();

            var tsk = act.Ask<ActorRef>(new crerateactormsg());
            tsk.Wait();

            Assert.IsNotNull(tsk.Result);
            var act2 = tsk.Result;

            var tsk2 = act2.Ask<int>(new acttestmsg() { msg = "test message", act = () => { return 10; } });
            tsk2.Wait();

            Assert.AreEqual(10, tsk2.Result);


        }
        [Test]
        public void SenderDetectTest()
        {

            var acts = new ActorSystem();
            var act = acts.CreateActor<IntoTestActor>();
            var act2 = acts.CreateActor<IntoTestActor>();
            var m = new senderdetectmsginto() { sender = act };
            var mm = new senderdetectmsg() { dest = act2, msg = m };
            var tsk = act.Ask<Task<bool>>(mm);
            tsk.Wait();
            tsk.Result.Wait();
            Assert.IsTrue(tsk.Result.Result);

        }

        [Test]
        public void PredStartTest()
        {

            var acts = new ActorSystem();
            var act = acts.CreateActor<IntoTestActor>();

            var tsk = act.Ask<int>(new statemsg());
            tsk.Wait();

            Assert.AreEqual(10, tsk.Result);

            acts.RestartActor(act, null);

            tsk = act.Ask<int>(new statemsg());
            tsk.Wait();

            Assert.AreEqual(0, tsk.Result);


        }

        [Test]
        public void PostRestartTest()
        {

            var acts = new ActorSystem();
            var act = acts.CreateActor<IntoTestActor>();

            var tsk = act.Ask<int>(new statemsg());
            tsk.Wait();

            Assert.AreEqual(10, tsk.Result);

            acts.RestartActor(act, null);

            tsk = act.Ask<int>(new statemsg());
            tsk.Wait();

            Assert.AreEqual(0, tsk.Result);

            acts.RestartActor(act, new testmsg());

            tsk = act.Ask<int>(new statemsg());
            tsk.Wait();

            Assert.AreEqual(15, tsk.Result);


        }

        [Test]
        public void PostStopTest()
        {
            bool f = false;
            var acts = new ActorSystem();
            var act = acts.CreateActor<IntoTestActor>();

            var m = new poststoptestmsg() { poststop = () => { f = true; } };
            var tsk = act.Ask<int>(m);
            tsk.Wait();
            Assert.IsFalse(f);
            acts.StopActor(act);

            Assert.IsTrue(f);




        }

        [Test]
        public void PredStartErrTest()
        {

            Exception e = null; 
            var acts = new ActorSystem();
            try
            {
                var act = acts.CreateActor<CreateFaultActor>();
            }
            catch (Exception ex)
            {

                e = ex;
            }
            Assert.IsNotNull(e);
        }

        [Test]
        public void PostRestartErrTest()
        {
            Exception e = null;
            var acts = new ActorSystem();
            var act = acts.CreateActor<RestartFaultActor>();
            try
            {
                acts.RestartActor(act,null);
            }
            catch (Exception ex) { e = ex; }

            Assert.IsNotNull(e);

        }

        [Test]
        public void PostStopErrTest()
        {
            Exception e = null;
            var acts = new ActorSystem();
            var act = acts.CreateActor<RestartFaultActor>();
            try
            {
                acts.StopActor(act);
            }
            catch (Exception ex) { e = ex; }

            Assert.IsNull(e);

        }

    }
}
