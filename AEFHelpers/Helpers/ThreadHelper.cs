using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AEF.Helpers
{
    class TaskQ
    {
        private Task CurrentTask = null;
        public void AddTaskToCurrentEx(Task NewTask, Action<Exception> ExceptionHandler)
        {
            Task PrevTask = Interlocked.Exchange(ref CurrentTask, NewTask);
            if (PrevTask == null)
            {
                NewTask.Start();
            }
            else
            {
                PrevTask.ContinueWith((x) =>
                {
                    if (x.Exception != null)
                    {
                        try { ExceptionHandler(x.Exception); }
                        catch { }
                    }
                    try { NewTask.Start(); }
                    catch (InvalidOperationException e) { }
                });
            }
        }
        public void AddTaskToCurrent(Task NewTask)
        {
            Task PrevTask = Interlocked.Exchange(ref CurrentTask, NewTask);
            if (PrevTask == null)
            {
                NewTask.Start();
            }
            else
            {
                PrevTask.ContinueWith((x) =>
                {
                    try { NewTask.Start(); }
                    catch (InvalidOperationException e) { }
                });
            }
        }
        public void SyncRunCurrentTask()
        {
            CurrentTask.RunSynchronously();
        }
    }

    public class ThreadStaticStorage
    {
        [ThreadStatic]
        private static object value = null;
        public static T Value<T>()
        {
            return (T)value;
        }
        public static void SetValue(object val)
        {
            value = val;
        }

    }

    class UnBreakSignal
    {
        private int value = 0;
        public bool isTrue { get { return value == 1; } }
        public void Set(bool NewValue)
        {
            Interlocked.Exchange(ref value, NewValue ? 1 : 0);
        }
    }
    class ImpulseSignal
    {
        private int value = 0;

        public ImpulseSignal() { }
        public ImpulseSignal(bool state)
        {
            value = state ? 1 : 0;
        }

        public bool isTrue { get { return value == 1; } }
        public void Pulse()
        {
            Interlocked.Exchange(ref value, 1);
        }
        public bool GetPulse()
        {
            return Interlocked.Exchange(ref value, 0) == 1;
        }
    }

    public class Arbitr<Tmsg, TNOPmsg>
        where Tmsg : class
        where TNOPmsg : Tmsg, new()
    {
        private ConcurrentQueue<Tmsg> MsgQueue = new ConcurrentQueue<Tmsg>();

        private ImpulseSignal NTR = new ImpulseSignal(true); //NeedRunTask
        private ImpulseSignal NTC = new ImpulseSignal(false); //NewTaskCreated
        private UnBreakSignal Suspended_ = new UnBreakSignal();
        private UnBreakSignal SuspendComplete = new UnBreakSignal();
        private TaskQ TaskQ = new TaskQ();
        private bool TaskFin = false;
        private bool NeedFinishNTR = false;
        private bool NRUT = false;

        private Action<Tmsg> Handler = null;
        private Action<Exception> ExceptionHandler = null;

        public Arbitr(Action<Tmsg> Handler, Action<Exception> ExceptionHandler)
        {
            if (Handler == null | ExceptionHandler == null) throw new ArgumentNullException();
            this.Handler = Handler;
            this.ExceptionHandler = ExceptionHandler;

        }

        private ImpulseSignal SuspendLock = new ImpulseSignal(true);
        private ImpulseSignal ResumeLock = new ImpulseSignal(true);

        public bool Suspended { get { return Suspended_.isTrue; } }

        private void ProcMessage(Tmsg msg)
        {
            Handler(msg);
        }
        private void ProcException(Exception e)
        {
            ExceptionHandler(e);
        }

        private void ProcessMessage()
        {

            Tmsg msg = null;
            MsgQueue.TryDequeue(out msg);
            if (msg == null) return;
            if (msg is TNOPmsg) return;

            try
            {
                ProcMessage(msg);

            }
            catch (Exception e)
            {

                ProcException(e);
            }



        }
        private void AsyncRecieve()
        {
            if (TaskFin) return;
            NeedFinishNTR = false;
            while (true)
            {
                if (Suspended)
                {
                    SuspendComplete.Set(true);
                    return;
                }
                if (NRUT)
                {
                    NRUT = false;
                    return;
                }
                if (MsgQueue.IsEmpty) break;
                ProcessMessage();

            }

            NTC.GetPulse();
            NTR.Pulse();
            NeedFinishNTR = true;
            while (true)
            {
                if (MsgQueue.IsEmpty) return;
                if (NRUT)
                {
                    NRUT = false;
                    return;
                }
                if (Suspended)
                {
                    FinishCurrentTaskLine();
                    SuspendComplete.Set(true);
                    return;

                }
                if (!NTR.isTrue) return;
                ProcessMessage();
            }
        }
        private void FinishCurrentTaskLine()
        {
            if (!NeedFinishNTR) return;
            if (!NTR.GetPulse())
            {
                while (!NTC.GetPulse()) ; //дождемся, пока новый таск точно не будет создан
                TaskFin = true; //что б созданный таск сразу завершился
                TaskQ.SyncRunCurrentTask();
                TaskFin = false;

            }
        }
        private Task CreateRecvTask()
        {
            return new Task(AsyncRecieve);
        }




        public void AddUserTaskToLine(Task task)
        {
            if (NRUT) throw new InvalidOperationException("Нельзя вызывать AddUserTaskToLine(Task task) 2 раза на одно сообщение");
            NRUT = true;
            FinishCurrentTaskLine();
            TaskQ.AddTaskToCurrent(task);
            TaskQ.AddTaskToCurrentEx(CreateRecvTask(), ProcException);
        }
        public void Send(Tmsg msg)
        {

            MsgQueue.Enqueue(msg);
            if (NTR.GetPulse())
            {
                TaskQ.AddTaskToCurrent(CreateRecvTask());
                NTC.Pulse();

            }

        }
        public void Suspend()
        {
            while (!SuspendLock.GetPulse()) ;
            if (Suspended) return;
            SuspendComplete.Set(false);
            Suspended_.Set(true);

            Send(new TNOPmsg());
            SuspendLock.Pulse();
        }
        public void Resume()
        {
            while (!ResumeLock.GetPulse()) ;
            if (!Suspended) throw new InvalidOperationException("Спячку еще даже не иницировали, нечего резюмить");
            while (!SuspendComplete.isTrue) ;
            Suspended_.Set(false);
            NTR.Pulse();
            Send(new TNOPmsg());
            ResumeLock.Pulse();


        }
    }

}
