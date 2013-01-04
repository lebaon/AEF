using System;
using System.Collections.Concurrent;

using System.Text;
using NUnit.Framework;
using System.Threading;
using System.Diagnostics;

namespace AEF.Tests.Other
{
    class intoNunitBags
    {
        [ThreadStatic]
        public static int ts = 0;
        public void bagOfwhileInterlock()
        {
            int f = 0;
            Action act = () =>
            {
                double x = 0.5;
                for (int i = 0; i < 10000; i++)
                {
                    x = x * x;
                }
                Interlocked.Exchange(ref f, 1);
            };

            var t = act.BeginInvoke(null, null);
            //int y = 0;
            //var qu = new ConcurrentQueue<int>();
           
            while (f == 0)
            {
                //Console.WriteLine();
                //ct=System.Threading.Thread.CurrentThread;
                Thread.SpinWait(0);
                //Interlocked.Exchange(ref y, 0);
                //Math.Abs(10);
                //qu.TryDequeue(out y);
                //ts = 0;
                
            }
            
            act.EndInvoke(t);
            Console.WriteLine("test complete");
        }
    }
    
    public class Bags
    {
        [Test]
        public void bagOfwhile()
        {
            bool f = false;
            Action act = () =>
            {
                double x = 0.5;
                for (int i = 0; i < 10000; i++)
                {
                    x = x * x;
                }
                f = true; 
            };

            var t = act.BeginInvoke(null, null);
            
            while (!f)
            {
                Thread.SpinWait(0);
                
            }

            act.EndInvoke(t);
            Console.WriteLine("test complete");
        }

        [Test]
        public void bagOfwhileInterlock()
        {
            int  f = 0;
            Action act = () =>
            {
                double x = 0.5;
                for (int i = 0; i < 10000; i++)
                {
                    x = x * x;
                }
                Interlocked.Exchange(ref f, 1);
            };

            var t = act.BeginInvoke(null, null);

            while (f == 0)
            {
                Thread.SpinWait(0);
            }

            act.EndInvoke(t);
            Console.WriteLine("test complete");
        }

        private bool F = false;
        private bool T = false;
        private void thrmethod()
        {
            double x = 0.5;
            for (int i = 0; i < 10000; i++)
            {
                x = x * x;
            }
            F = true; 
        }

        [Test]
        public void bagOfwhileWithoutLambda()
        {
            F = false;
            Action act = thrmethod;

            var t = act.BeginInvoke(null, null);

            while (!F) { Thread.SpinWait(0); }

            act.EndInvoke(t);
            Console.WriteLine("test complete");
        }

        private void thrmethod1()
        {
            double x = 0.5;
            for (int i = 0; i < 10000; i++)
            {
                x = x * x;
            }
            F = true;
            while (!T) { Thread.SpinWait(0); }
        }

        [Test]
        public void bagOfwhileWithoutLambda1()
        {
            F = false;
            Action act = thrmethod1;

            var t = act.BeginInvoke(null, null);

            while (!F) { Thread.SpinWait(0); }
            Console.WriteLine("while complete");
            T = true;
            act.EndInvoke(t);
            Console.WriteLine("test complete");
        }

        [Test]
        public void externaltest()
        {
            var t = new intoNunitBags();
            t.bagOfwhileInterlock();
        }

        private Stopwatch st = new Stopwatch();
        [Test]
        public void delaySpinWait()
        {
            st.Reset();
            st.Start();
            Thread.SpinWait(1);
            st.Stop();
            Console.WriteLine("Elapsed ms by 1 invoke {0}",st.ElapsedMilliseconds);
            st.Reset();
            st.Start();
            int i = 0;
            int n = 1000000;
            while (i < n)
            {
                
                i++;
            }
            st.Stop();
            long correction = st.ElapsedMilliseconds;
            Console.WriteLine("Correction = {0}", correction);
            st.Reset();
            st.Start();
             i = 0;

            while (i < n)
            {
                Thread.SpinWait(0);
                i++;
            }
            st.Stop();
            Console.WriteLine("Elapsed ms by {1} invoke {0}", st.ElapsedMilliseconds-correction,n);
            st.Reset();
            st.Start();
            i = 0;
            int y = 0;
            while (i < n)
            {
                Interlocked.Exchange(ref y, 0);
                i++;
            }
            st.Stop();
            Console.WriteLine("Elapsed ms by {1} interlocked exchange {0}", st.ElapsedMilliseconds - correction,n);
        }
    }
}
