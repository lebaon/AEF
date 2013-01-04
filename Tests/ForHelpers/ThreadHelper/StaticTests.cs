using System;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using AEF.Helpers;

namespace AEF.Tests.ForHelpers.ThreadHelper.StaticTests
{
    class Message
    {
        public int x;
    }
    class NOPMessage : Message { }
    
    public class StaticTests
    {

        [Test]
        public void MessageSendAndRecieveTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            int y = -1;
            var a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y += z.x;
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Send(new Message() { x = 1 });
            bool completed = even.WaitOne(500);
            Assert.AreEqual(y, 0);
            Assert.IsTrue(completed);
            Assert.IsNull(e);

        }

        [Test]
        public void MessageProcExceptionTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);

            var a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    int q = 1 / z.x;
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Send(new Message() { x = 0 });
            bool completed = even.WaitOne(500);

            Assert.IsTrue(completed);
            Assert.IsNotNull(e);

        }

        [Test]
        public void MessageAfterExceptionTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            int y = 0;
            var a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y = 1 / z.x;
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Send(new Message() { x = 0 });
            bool completed = even.WaitOne(500);

            Assert.IsTrue(completed);
            Assert.IsNotNull(e);
            Assert.AreEqual(y, 0);

            e = null;
            a.Send(new Message() { x = 1 });
            completed = even.WaitOne(500);

            Assert.IsTrue(completed);
            Assert.IsNull(e);
            Assert.AreEqual(y, 1);


        }

        [Test]
        public void BasicSuspendResumeTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            int y = 0;
            var a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y += z.x;
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Send(new Message() { x = 1 });
            bool completed = even.WaitOne(500);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);

            a.Suspend();

            y = 0;
            a.Send(new Message() { x = 1 });
            completed = even.WaitOne(100);
            Assert.AreEqual(y, 0);
            Assert.IsFalse(completed);
            Assert.IsNull(e);

            a.Resume();

            completed = even.WaitOne(500);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
        }

        [Test]
        public void UserTaskTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            AutoResetEvent evenT = new AutoResetEvent(false);
            int y = 0;
            int t = 0;
            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y += z.x;
                    if (a != null)
                        a.AddUserTaskToLine(new Task(() =>
                        {
                            t++;
                            evenT.Set();
                        }));
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Send(new Message() { x = 1 });
            bool completed = even.WaitOne(500);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            completed = evenT.WaitOne(500);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            Assert.AreEqual(t, 1);

            a.Send(new Message() { x = -1 });
            completed = even.WaitOne(500);
            Assert.AreEqual(y, 0);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            completed = evenT.WaitOne(500);
            Assert.AreEqual(y, 0);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            Assert.AreEqual(t, 2);

        }

        [Test]
        public void UserTaskExceptionTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            
            int y = 0;
            int t = 0;
            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y += z.x;
                    if (a != null)
                        a.AddUserTaskToLine(new Task(() =>
                        {
                            t = 1 / t;
                            
                        }));
                    
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            even.Reset();
            a.Send(new Message() { x = 1 });
            bool completed = even.WaitOne();
            Assert.AreEqual(y, 1);


            Assert.IsTrue(completed);
            Assert.AreEqual(t, 0);
            Assert.IsNotNull(e);

        }

        [Test]
        public void SuspendInMessageRecieveTest()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            int y = 0;
            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y += z.x;

                    a.Suspend();
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Send(new Message() { x = 1 });
            bool completed = even.WaitOne(500);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);

            a.Send(new Message() { x = -1 });
            completed = even.WaitOne(100);
            Assert.AreEqual(y, 1);
            Assert.IsFalse(completed);
            Assert.IsNull(e);

            a.Resume();
            completed = even.WaitOne(500);
            Assert.AreEqual(y, 0);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            Assert.IsTrue(a.Suspended);

        }

        [Test]
        public void SuspendInTask()
        {
            Exception e = null;
            AutoResetEvent even = new AutoResetEvent(false);
            AutoResetEvent evenT = new AutoResetEvent(false);
            int y = 0;
            int t = 0;
            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>(
                (z) =>
                {
                    y += z.x;
                    a.AddUserTaskToLine(new Task(() =>
                    {
                        t++;
                        a.Suspend();
                        evenT.Set();
                    }));
                    even.Set();
                },
                (z) =>
                {
                    e = z;
                    even.Set();
                });

            a.Suspend();
            a.Send(new Message() { x = 1 });
            a.Send(new Message() { x = -1 });
            a.Resume();
            bool completed = even.WaitOne(500);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);

            completed = evenT.WaitOne(500);
            Assert.AreEqual(t, 1);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            Assert.AreEqual(y, 1);
            Assert.IsTrue(a.Suspended);

            completed = even.WaitOne(100);
            Assert.IsFalse(completed);

            a.Resume();

            completed = even.WaitOne(500);
            Assert.AreEqual(y, 0);
            Assert.IsTrue(completed);
            Assert.IsNull(e);

            completed = evenT.WaitOne(500);
            Assert.AreEqual(t, 2);
            Assert.IsTrue(completed);
            Assert.IsNull(e);
            Assert.AreEqual(y, 0);
            Assert.IsTrue(a.Suspended);

        }
    }
}
