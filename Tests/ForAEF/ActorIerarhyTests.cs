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
    public class ActorIerarhyTests
    {
        [Test]
        public void AllActorHasParentTest()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<IerarhyTestActor>();

            var tsk = act.Ask<ActorRef>(new getparentmsg());
            tsk.Wait();

            Assert.IsNotNull(tsk.Result);
        }

        [Test]
        public void ChildsAndParentsTest()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<IerarhyTestActor>();

            var tsk = act.Ask<ActorRef>(new createchildactor());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);

            tsk = act2.Ask<ActorRef>(new getparentmsg());
            tsk.Wait();

            Assert.AreEqual(tsk.Result, act);

            var tsk2 = act.Ask<ActorRef[]>(new getchildsmsg());
            tsk2.Wait();

            Assert.Contains(act2, tsk2.Result);
        }

        [Test]
        public void StopParentStoppingChilds()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<IerarhyTestActor>();

            var tsk = act1.Ask<ActorRef>(new createchildactor());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);


            

            actf.StopActor(act1);

            bool f = false;
            try
            {
                act1.Ask<int>(new acttestmsg()
                {
                    act = () => { return 0; }
                })
                    .Wait(100);
            }
            catch
            {
                f = true;
            }

            Assert.IsTrue(f);
            f = false;
            try
            {
                act2.Ask<int>(new acttestmsg()
                {
                    act = () => { return 0; }
                })
                    .Wait(100);
            }
            catch
            {
                f = true;
            }

            Assert.IsTrue(f);
        }

        [Test]
        public void RestartParentStoppingChilds()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<IerarhyTestActor>();
            
            var tsk = act1.Ask<ActorRef>(new createchildactor());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);

            actf.RestartActor(act1);

            bool f = false;
            try
            {
                act1.Ask<int>(new acttestmsg()
                {
                    act = () => { return 0; }
                })
                    .Wait(100);
            }
            catch
            {
                f = true;
            }

            Assert.IsFalse(f);
            f = false;
            try
            {
                act2.Ask<int>(new acttestmsg()
                {
                    act = () => { return 0; }
                })
                    .Wait(100);
            }
            catch
            {
                f = true;
            }

            Assert.IsTrue(f);
        }

        [Test]
        public void RestartParentWithUserPredRestartNotStoppingChilds()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<IerarhyTestActorOvveride>();

            var tsk = act1.Ask<ActorRef>(new createchildactor());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);

            actf.RestartActor(act1);

            bool f = false;
            try
            {
                act1.Ask<int>(new acttestmsg()
                {
                    act = () => { return 0; }
                })
                    .Wait(100);
            }
            catch
            {
                f = true;
            }

            Assert.IsFalse(f);
            f = false;
            try
            {
                act2.Ask<int>(new acttestmsg()
                {
                    act = () => { return 0; }
                })
                    .Wait(100);
            }
            catch
            {
                f = true;
            }

            Assert.IsFalse(f);

        }

        [Test]
        public void StopChildRemoveItFromParentListOfChild()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<IerarhyTestActorOvveride>();

            var tsk = act1.Ask<ActorRef>(new createchildactor());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);

            act1.Ask<int>(new stopchild() { child = act2 }).Wait();

            var tsk2 = act1.Ask<ActorRef[]>(new getchildsmsg());
            tsk2.Wait();

            Assert.IsTrue(tsk2.Result.Where((child) => { return child == act2; }).Count() == 0);
        }


        [Test]
        public void ChildHasStopedOrRestartedOnlyByParent()
        {
            var actf = new ActorSystem();
            var act1 = actf.CreateActor<IerarhyTestActorOvveride>();

            var tsk = act1.Ask<ActorRef>(new createchildactor());
            tsk.Wait();

            var act2 = tsk.Result;
            Assert.IsNotNull(act2);

            bool f = false;

            try
            {

                actf.StopActor(act2);
            }
            catch
            {
                f = true;
            }

            Assert.IsTrue(f);
            f = false;

            try
            {

                actf.RestartActor(act2);
            }
            catch
            {
                f = true;
            }

            Assert.IsTrue(f);
        }


        [Test]
        public void PredRestartTest()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<PredRestartTestActor>();

            var tsk = act.Ask<int>(new setstate() { x = 20 });
            tsk.Wait();

            tsk = act.Ask<int>(new getstate());
            tsk.Wait();

            Assert.AreEqual(20, tsk.Result);

            int savestate = 0;
            tsk = act.Ask<int>(new setpra()
            {
                pra = (state) =>
                {

                    savestate = state;
                }
            });
            tsk.Wait();

            actf.RestartActor(act);

            Assert.AreEqual(20, savestate);

            tsk = act.Ask<int>(new getstate());
            tsk.Wait();

            Assert.AreEqual(0, tsk.Result);
        }

        [Test]
        public void BlockedSelfRestart()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<PredRestartTestActor>();
            var tsk = act.Ask<int>(new setstate() { x = 20 });
            tsk.Wait();

            
             tsk = act.Ask<int>(new selfrestart());
            tsk.Wait();

            tsk = act.Ask<int>(new getstate());
            tsk.Wait();

            Assert.AreEqual(20, tsk.Result);
        }

        [Test]
        public void BlockedSelfStop()
        {
            var actf = new ActorSystem();
            var act = actf.CreateActor<PredRestartTestActor>();

            var tsk = act.Ask<int>(new selfstop());
            tsk.Wait();

            tsk = act.Ask<int>(new getstate());
            tsk.Wait();
        }


        [Test]
        public void SelfStopThrowException()
        {
            bool f = false;
            var actf = new ActorSystem();
            var act = actf.CreateActor<SelfRestartableActor>();

            var tsk = act.Ask<int>(new selfstop());
            try
            {
                
                tsk.Wait();
            }
            catch { f = true; }

            Assert.IsTrue(f);
            
        }

        [Test]
        public void SelfRestartThrowException()
        {
            bool f = false;
            var actf = new ActorSystem();
            var act = actf.CreateActor<SelfRestartableActor>();

            var tsk = act.Ask<int>(new selfrestart());
            try
            {

                tsk.Wait();
            }
            catch { f = true; }

            Assert.IsTrue(f);

        }
    }
}
