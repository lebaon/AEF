using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AEF.Helpers;
using System.Reflection;


namespace AEF
{
    public class ActorRef
    {
        internal ActorInstanceGenerator Gen { get; set; }
        private Arbitr<Message, NOPMessage> Arbitr;
        private MethodFinder MethodFinder;
        internal Actor actor;
        private ActorSystem system;
        private bool Stopped = false;
        private bool ProcRunning = false;

        private void ProcMsgInStoppedActor(Message msg) { }
        private void ProcNotHandledMsg(Message msg) { }
        private void ProcActorException(Exception e) { }
        internal void ProcPostStopException(Exception e) { }

        private void ProcAnswerMessage(AnswerMessage msg) {
            try
            {
                if (msg.ReturnValue is Exception) msg.ReturnCode(null,(Exception) msg.ReturnValue);
                else msg.ReturnCode(msg.ReturnValue,null);

                
            }
            catch (Exception e)
            {
                ProcActorException(e);
            }

        
        }
        private void ProcTellMessage(TellMessage msg) {
            var m = MethodFinder.GetMethodByParamsAndReturnValueType(null, msg.args);
            if (m == null)
            {
                ProcNotHandledMsg(msg);
                return;
            }
            try
            {
                object ret = m.Invoke(actor, msg.args);
                
            }
            catch (Exception e)
            {
                ProcActorException(e);
            }

        }
        private void ProcAskMessage(AskMessage msg) {
            var m = MethodFinder.GetMethodByParamsAndReturnValueType(msg.ReturnType, msg.args);
            if (m == null)
            {
                ProcNotHandledMsg(msg);
                msg.Sender.Send(new AnswerMessage()
                {
                    ReturnValue = new AEFException("Нет такого метода"),
                    ReturnCode = msg.ReturnCode
                });
                return;
            }
            try
            {
                object ret = m.Invoke(actor, msg.args);


                msg.Sender.Send(new AnswerMessage()
                {
                    ReturnValue = ret,
                    ReturnCode = msg.ReturnCode,
                    Sender = this
                });
            }
            catch (TargetInvocationException e)
            {
                msg.Sender.Send(new AnswerMessage()
                {
                    ReturnValue = e.InnerException,
                    ReturnCode = msg.ReturnCode,
                    Sender = this
                });
                ProcActorException(e);
            }
        
        
        
        
        }

        private void ProcMessage(Message msg)
        {
            if (Stopped)
            {
                if (msg is AskMessage) msg.Sender.Send(new AnswerMessage()
                {
                    ReturnValue = new AEFException("ActorIsStopped"),
                    ReturnCode = ((AskMessage)msg).ReturnCode,
                    Sender = this
                });
                ProcMsgInStoppedActor(msg);
                return;
            }
            ProcRunning = true;
            
            ThreadStaticStorage.SetValue(this);
            actor.Context = new ActorContext() { Msg = msg, System = system, Self = this };
            if (msg is AnswerMessage) ProcAnswerMessage((AnswerMessage)msg);
            else
                if (msg is AskMessage) ProcAskMessage((AskMessage)msg);
                else
                    if (msg is TellMessage) ProcTellMessage((TellMessage)msg);
            ThreadStaticStorage.SetValue(null);
            

            ProcRunning = false;
        }
        private void ExceptionArbitrHandler(Exception e) { }

        public ActorRef(ActorSystem system)
        {
            
            this.system = system;
            Arbitr = new Arbitr<Message, NOPMessage>(ProcMessage, ExceptionArbitrHandler);
        }

        internal void RunActor(Actor act)
        {
            actor = act;
            MethodFinder = new MethodFinder(act.GetType());
            if (Arbitr.Suspended) Arbitr.Resume();
        }

        internal void Stop()
        {
            Stopped = true;
            while (ProcRunning) { Thread.SpinWait(0); }
        }

        internal void ContextStop()
        {
            Stopped = true;
        }

        internal void Suspend()
        {
            Arbitr.Suspend();
            while (ProcRunning) { Thread.SpinWait(0); }
        }

        internal void Send(Message msg)
        {
            Arbitr.Send(msg);
        }

        public void Tell(params object[] args)
        {
            Send(new TellMessage()
            {
                args = args,
                Sender = ThreadStaticStorage.Value<ActorRef>() != null ? 
                ThreadStaticStorage.Value<ActorRef>() : 
                system.DefaultActor
            });
        }
        
        

        public Task<T> Ask<T>(params object[] args)
        {
            TaskHelper<T> th = new TaskHelper<T>();
            Send(new AskMessage()
            {
                args = args,
                Sender = ThreadStaticStorage.Value<ActorRef>() != null ?
                ThreadStaticStorage.Value<ActorRef>() :
                system.DefaultActor,
                ReturnType = typeof(T),
                ReturnCode = th.RunTask
            });
            return th.Task;
        }

        public Task<T> Ask<T>(Action<T> Continuation, params object[] args)
        {
            TaskHelper<T> th = new TaskHelper<T>();
            Send(new AskMessage()
            {
                args = args,
                Sender = ThreadStaticStorage.Value<ActorRef>() != null ?
                ThreadStaticStorage.Value<ActorRef>() :
                system.DefaultActor,
                ReturnType = typeof(T),
                ReturnCode = (x, e) =>
                {
                    if (e == null) Continuation((T)x);
                    th.RunTask(x, e);
                }
            });
            return th.Task;
        }

        public Task<T> Ask<T>(Action<T, Exception> Continuation, params object[] args)
        {
            TaskHelper<T> th = new TaskHelper<T>();
            Send(new AskMessage()
            {
                args = args,
                Sender = ThreadStaticStorage.Value<ActorRef>() != null ?
                ThreadStaticStorage.Value<ActorRef>() :
                system.DefaultActor,
                ReturnType = typeof(T),
                ReturnCode = (x, e) =>
                {
                    if (e == null) Continuation((T)x, e);
                    else Continuation(default(T), e);
                    th.RunTask(x, e);
                }
            });
            return th.Task;
        }
    }
}
