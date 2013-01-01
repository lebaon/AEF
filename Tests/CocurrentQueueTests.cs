using System;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using AEF.Helpers;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Tests
{
    public class CocurrentQueueTests
    {
        private Stopwatch st = new Stopwatch();
        private int TotalMessageCount = 10000000;
        private ConcurrentQueue<int> qu = new ConcurrentQueue<int>();

        private void Sender()
        {
            for (int i = 0; i < TotalMessageCount; i++)
            {
                qu.Enqueue(i);
            }
        }

        private void Reciever()
        {
            int y;
            for (int i = 0; i < TotalMessageCount; )
            {
                if (qu.TryDequeue(out y)) i++;
            }
        }

        private void Clearer()
        {
            int y;
            while (qu.TryDequeue(out y)) ;
        }

        private long Tester(int SCount, int RCount)
        {
            if (RCount > SCount) throw new Exception();

           

            Action[] senders = new Action[SCount];
            
            for (int i = 0; i < senders.Length; i++)
                senders[i] = new Action(Sender);

            Action[] recievers = new Action[RCount];

            for (int i = 0; i < recievers.Length; i++)
                recievers[i] = new Action(Reciever);

            IAsyncResult[] TS = new IAsyncResult[SCount];
            IAsyncResult[] TR = new IAsyncResult[RCount];

            st.Reset();
            st.Start();

            for (int i = 0; i < recievers.Length; i++)
                TR[i] = recievers[i].BeginInvoke(null, null);

            for (int i = 0; i < senders.Length; i++)
                TS[i] = senders[i].BeginInvoke(null, null);

            for (int i = 0; i < senders.Length; i++)
                senders[i].EndInvoke(TS[i]);

            for (int i = 0; i < recievers.Length; i++)
                recievers[i].EndInvoke(TR[i]);

            st.Stop();

            if (qu.Count != 0) Clearer();

            return st.ElapsedMilliseconds;
        }

        private void TesterMeter(int SCount, int RCount)
        {
            Console.WriteLine("Потоков отправителя: {0}", SCount);
            Console.WriteLine("Потоков получателя: {0}", RCount);
            Console.WriteLine("Всего потоков: {0}", SCount + RCount);
            long tm = Tester(SCount, RCount);
            tm = tm == 0 ? 1 : tm;
            double perf = (SCount * TotalMessageCount) / tm;
            Console.WriteLine("Производительность, обьектов/мс: {0}",perf);
        }

        [Test]
        public void ConcurrentQueueBenhmark1()
        {

            TesterMeter(1, 0);
        }
        [Test]
        public void ConcurrentQueueBenhmark2()
        {

            TesterMeter(1, 1);
        }
        [Test]
        public void ConcurrentQueueBenhmark3()
        {
            Console.WriteLine("Количество логическия процессоров: {0}", Environment.ProcessorCount);
            TesterMeter(Environment.ProcessorCount, 0);
        }
        [Test]
        public void ConcurrentQueueBenhmark4()
        {
            Console.WriteLine("Количество логическия процессоров: {0}", Environment.ProcessorCount);
            TesterMeter(Environment.ProcessorCount, 1);
        }
        [Test]
        public void ConcurrentQueueBenhmark5()
        {
            Console.WriteLine("Количество логическия процессоров: {0}", Environment.ProcessorCount);
            TesterMeter(Environment.ProcessorCount+1, 0);
        }
    }
}
