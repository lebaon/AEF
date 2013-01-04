using System;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using AEF.Helpers;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace AEF.Tests.ForHelpers.ThreadHelper.DinamicTests
{
    class Message { }
    class TaskMessage : Message { }
    class NOPMessage : Message { }
    
     public class DinamicTests
    {
        int SharedVar = 0;
        int ActiveMsgCount = 0;
        Stopwatch st = new Stopwatch();
        Random rand = new Random();

        int SVManipulateInOneMsgCount = 10;
        int TotalMessageCount = 100000;
        int MaxActiveMsgCount = 100;
        [Test]
        public void ManipulateSharedVarTest()
        {
            ManipulateSharedVar();
            Assert.AreEqual(SharedVar, 0);
        }
        public void ManipulateSharedVar()
        {
            int n = SVManipulateInOneMsgCount;
            for (int i = 0; i < n + 1; i++)
            {
                SharedVar = SharedVar + i;
                SharedVar = SharedVar - (n - i);
            }

        }

        void TSend(Arbitr<Message, NOPMessage> a, Message msg)
        {
            Interlocked.Increment(ref ActiveMsgCount);
            a.Send(msg);
        }

        void MessageProc(Message msg, Arbitr<Message, NOPMessage> a)
        {
            if (msg is TaskMessage) a.AddUserTaskToLine(new Task(() =>
            {
                ManipulateSharedVar();
                Interlocked.Decrement(ref ActiveMsgCount);
            }));
            else
            {
                ManipulateSharedVar();
                Interlocked.Decrement(ref ActiveMsgCount);
            }
        }

        [Test]
        public void SerialOneThreadTest()
        {
            Exception e = null;

            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>((x) => { MessageProc(x, a); },
                (x) => { e = x; });
            int i = 0;


            while (i < TotalMessageCount)
            {
                if (ActiveMsgCount < MaxActiveMsgCount)
                {
                    if (rand.NextDouble() > 0.5)
                    {
                        TSend(a, new Message());
                    }
                    else
                    {
                        TSend(a, new TaskMessage());
                    }
                    i++;
                    continue;
                }
                Thread.SpinWait(0);

            }


            while (ActiveMsgCount > 0) { Thread.SpinWait(0); }

            Assert.AreEqual(SharedVar, 0);
            Assert.IsNull(e);



        }

        [Test]
        public void SerialOneThreadBenhmark()
        {
            Exception e = null;

            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>((x) => { MessageProc(x, a); },
                (x) => { e = x; });
            int i = 0;

            //сначала померим скорость чистых сообщений
            st.Reset();
            st.Start();
            i = 0;
            while (i < TotalMessageCount)
            {
                if (ActiveMsgCount < MaxActiveMsgCount)
                {
                    TSend(a, new Message());
                    i++;
                    continue;
                }
                Thread.SpinWait(0);

            }
            while (ActiveMsgCount > 0) { Thread.SpinWait(0); }
            st.Stop();
            Console.WriteLine("Скорость обработки чистых сообщений {0} в мс",
                TotalMessageCount / (st.ElapsedMilliseconds != 0 ? st.ElapsedMilliseconds : 1));

            // теперь скорость чистых сообщений-тэсков
            st.Reset();
            st.Start();
            i = 0;
            while (i < TotalMessageCount)
            {
                if (ActiveMsgCount < MaxActiveMsgCount)
                {
                    TSend(a, new TaskMessage());
                    i++;
                    continue;
                }
                Thread.SpinWait(0);

            }
            while (ActiveMsgCount > 0) { Thread.SpinWait(0); }
            st.Stop();
            Console.WriteLine("Скорость обработки чистых тэсков {0} в мс",
                TotalMessageCount / (st.ElapsedMilliseconds != 0 ? st.ElapsedMilliseconds : 1));
            // а теперь смешанный режим 50\50
            st.Reset();
            st.Start();
            i = 0;
            while (i < TotalMessageCount)
            {
                if (ActiveMsgCount < MaxActiveMsgCount)
                {
                    if (i % 2 > 0)
                    {
                        TSend(a, new Message());
                    }
                    else
                    {
                        TSend(a, new TaskMessage());
                    }
                    i++;
                    continue;
                }
                Thread.SpinWait(0);

            }
            while (ActiveMsgCount > 0) { Thread.SpinWait(0); }
            st.Stop();
            Console.WriteLine("Скорость обработки сообщений в смешанном режиме 50х50 {0} в мс",
                TotalMessageCount / (st.ElapsedMilliseconds != 0 ? st.ElapsedMilliseconds : 1));
            Assert.AreEqual(SharedVar, 0);
            Assert.IsNull(e);



        }

        private void MsgSender(Func<Message> GenFunc, Arbitr<Message, NOPMessage> arb, int count)
        {
            int i = 0;
            while (i < count)
            {
                if (ActiveMsgCount < MaxActiveMsgCount)
                {
                    TSend(arb, GenFunc());
                    i++;
                    continue;
                }
                Thread.SpinWait(0);
            }
            Console.WriteLine("Отправленно {0} сообщений",i);
        }
        [Test]
        public void MultiThreadBenhmark()
        {
            Exception e = null;

            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>((x) => { MessageProc(x, a); },
                (x) => { e = x; });


            Action<Func<Message>, Arbitr<Message, NOPMessage>, int> tr1 = MsgSender;
            Action<Func<Message>, Arbitr<Message, NOPMessage>, int> tr2 = MsgSender;

            st.Reset();
            st.Start();

            var t1 = tr1.BeginInvoke(() => { return new Message(); }, a, TotalMessageCount, null, null);
            var t2 = tr2.BeginInvoke(() => { return new Message(); }, a, TotalMessageCount, null, null);

            tr1.EndInvoke(t1);
            tr2.EndInvoke(t2);


            Console.WriteLine("Осталось {0} сообщений", ActiveMsgCount);
            int prev = ActiveMsgCount;
            while (ActiveMsgCount > 0) {
                if (prev != ActiveMsgCount)
                {
                    Console.WriteLine("Осталось {0} сообщений", ActiveMsgCount);
                    prev = ActiveMsgCount;
                    continue;
                }
                Thread.SpinWait(0);
            }

            st.Stop();
            Console.WriteLine("Скорость обработки чистых сообщений {0} в мс",
                TotalMessageCount / (st.ElapsedMilliseconds != 0 ? st.ElapsedMilliseconds : 1));

            Assert.AreEqual(SharedVar, 0);
            Assert.IsNull(e);
            Assert.IsNotNull(a);

        }


        [Test]
        public void MultiThreadSuspendTest()
        {
            Exception e = null;

            Arbitr<Message, NOPMessage> a = null;
            a = new Arbitr<Message, NOPMessage>((x) => { MessageProc(x, a); },
                (x) => { e = x; });

            int SuspendCount = 0;

            Action<Func<Message>, Arbitr<Message, NOPMessage>, int> tr1 = MsgSender;
            Action<Func<Message>, Arbitr<Message, NOPMessage>, int> tr2 = MsgSender;
            Action tr3 = () =>
            {
                while (ActiveMsgCount > 0)
                {
                    
                    a.Resume();
                    SuspendCount++;
                    Thread.Sleep(10);
                    a.Suspend();
                    Thread.Sleep(10);
                }
                a.Resume();
            };

            st.Reset();
            st.Start();
            a.Suspend();
            TSend(a, new Message());
            var t3 = tr3.BeginInvoke(null, null);
            var t1 = tr1.BeginInvoke(() => { return new Message(); }, a, TotalMessageCount, null, null);
            var t2 = tr2.BeginInvoke(() => { return new Message(); }, a, TotalMessageCount, null, null);
            

            tr1.EndInvoke(t1);
            tr2.EndInvoke(t2);
            tr3.EndInvoke(t3);

            while (ActiveMsgCount > 0) { Thread.SpinWait(0); }

            st.Stop();
            Console.WriteLine("Произведенно {0} суспендов", SuspendCount);
            Console.WriteLine("Скорость обработки  сообщений {0} в мс",
                TotalMessageCount / (st.ElapsedMilliseconds != 0 ? st.ElapsedMilliseconds : 1));

            Assert.AreEqual(SharedVar, 0);
            Assert.IsNull(e);

        }


        
        [Test]
        public void ThreadStorageTest()
        {
            bool done = false;
            bool ok = true;
            Action a1 = () =>
            {
                int i = 0;
                int d = 1;
                while (!done)
                {
                    ThreadStaticStorage.SetValue(i);
                    if (ThreadStaticStorage.Value<int>() != i) ok = false;
                    i += d;
                }
            };
            Action a2 = () =>
            {
                int i = 0;
                int d = -1;
                while (!done)
                {
                    ThreadStaticStorage.SetValue(i);
                    if (ThreadStaticStorage.Value<int>() != i) ok = false;
                    i += d;
                }
            };

            var t1 = a1.BeginInvoke(null, null);
            var t2 = a2.BeginInvoke(null, null);

            Thread.Sleep(500);

            done = true;
            a1.EndInvoke(t1);
            a2.EndInvoke(t2);

            Assert.IsTrue(ok);
        }


        
    }
}
