using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AEF.Helpers;
using System.Reflection;


namespace AEF
{
    public class ActorRef : AEF.Helpers.IFluent
    {
        internal ActorInstanceGenerator Gen { get; set; }
        private Arbitr<Message, NOPMessage> Arbitr;
        private MethodFinder MethodFinder;
        private Actor actor;
        private ActorCore system;
        private bool Stopped = false;
        private bool ProcRunning = false;
        internal ActorRef parent = null;
        private Guid ID = Guid.NewGuid();
        private string Name_ = null;
        internal ActorPath Path { get; private set; }


        public string FullName { get { return Path.ToString(); } }
        public string Name { get { return Name_; } }

        internal ConcurrentDictionary<ActorRef, ActorRef> childs = new ConcurrentDictionary<ActorRef, ActorRef>();
        internal bool Suspended { get { return Arbitr.Suspended; } }


        internal void RunPostStop(ActorRef initiator)
        {
            try
            {
                actor.Context = new ActorContext() { Msg = new Message() { Sender = initiator },
                    System = system, Self = this };
                actor.PostStop();
            }
            catch (Exception e)
            {
                ProcPostStopException(e);
            }
        }
        internal bool RunPredRestart(ActorRef initiator,ref Exception e)
        {
            bool res = true;
            try
            {
                actor.Context = new ActorContext()
                {
                    Msg = new Message() { Sender = initiator },
                    System = system,
                    Self = this
                };
                actor.PredRestart();
            }
            catch(Exception ex)
            {
                res = false;
                e = ex;
            }

            return res;
        }
        internal bool RunPostRestart(ActorRef initiator, ref Exception e)
        {
            bool res = true;
            try
            {
                actor.Context = new ActorContext()
                {
                    Msg = new Message() { Sender = initiator },
                    System = system,
                    Self = this
                };
                actor.PostRestart();
            }
            catch (Exception ex)
            {
                res = false;
                e = ex;
            }

            return res;

        }
        internal void RunPredStart(ActorRef initiator)
        {
            actor.Context = new ActorContext()
            {
                Msg = new Message() { Sender = initiator },
                System = system,
                Self = this
            };
            actor.PredStart();
        }

        private void ProcMsgInStoppedActor(Message msg) { }
        private void ProcNotHandledMsg(Message msg) { }
        private void ProcActorException(Exception e) {
            EndProcRunning();
            if (e is AEFActorStopException) { system.StopActor(this, this); return; }
            if (e is AEFActorRestartException)
            {
                Task.Factory.StartNew(() => { system.RestartActor(this, this); });
                return;
            }
            system.SuspendActor(this);
            parent.Send(new ExceptionMessage() { e = e, Sender = this });
        }
        internal void ProcPostStopException(Exception e) { }

        private void ProcAnswerMessage(AnswerMessage msg) {
            try
            {
                if (msg.ReturnValue is Exception) msg.ReturnCode(null,(Exception) msg.ReturnValue);
                else msg.ReturnCode(msg.ReturnValue,null);

                
            }
            catch (TargetInvocationException e)
            {
                ProcActorException(e.InnerException);
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
            catch (TargetInvocationException e)
            {
                ProcActorException(e.InnerException);
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
                ProcActorException(e.InnerException);
            }
        
        
        
        
        }
        private void ProcExceptionMessage(ExceptionMessage msg)
        {
            try
            {
                var doing = actor.ChildException(msg.e);
                if (doing == ExceptionDecision.Stop) system.StopActor(msg.Sender,this);
                if (doing == ExceptionDecision.Resume) system.ResumeActor(msg.Sender);
                if (doing == ExceptionDecision.Restart)
                {
                    Exception e=system.RestartActor(msg.Sender, this);
                    if (e != null) throw e;
                }
                if (doing == ExceptionDecision.Excalation) throw msg.e;
            }
            catch (Exception e)
            {
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

            object SavedTSV = ThreadStaticStorage.Value<object>();
            ThreadStaticStorage.SetValue(this);
            actor.Context = new ActorContext() { Msg = msg, System = system, Self = this };
            if (msg is AnswerMessage) ProcAnswerMessage((AnswerMessage)msg);
            else if (msg is AskMessage) ProcAskMessage((AskMessage)msg);
            else if (msg is TellMessage) ProcTellMessage((TellMessage)msg);
            else if (msg is ExceptionMessage) ProcExceptionMessage((ExceptionMessage)msg);
            ThreadStaticStorage.SetValue(SavedTSV);


            ProcRunning = false;
        }
        private void ExceptionArbitrHandler(Exception e) { }
        private void EndProcRunning()
        {
            ProcRunning = false;
        }

        internal ActorRef(ActorCore system, ActorRef parent)
        {
            Name_ = ID.ToString();
            this.system = system;
            this.parent = parent;
            Path = new ActorPath(Name, parent.Path);
            Arbitr = new Arbitr<Message, NOPMessage>(ProcMessage, ExceptionArbitrHandler);
        }
        internal ActorRef(ActorCore system, ActorRef parent,string name)
        {
            Name_ = name;
            this.system = system;
            this.parent = parent;
            if (parent != null) Path = new ActorPath(Name, parent.Path);
            else Path = new ActorPath(Name);
            Arbitr = new Arbitr<Message, NOPMessage>(ProcMessage, ExceptionArbitrHandler);
        }
        
        internal void SetActor(Actor act)
        {
            actor = act;
            MethodFinder = new MethodFinder(act.GetType());
        }
        internal void RunActor()
        {
            
            if (Arbitr.Suspended) Arbitr.Resume();
        }
        internal void Stop()
        {
            Stopped = true;
            
            
            while (ProcRunning) { Thread.SpinWait(0); }
            foreach (var i in childs.Keys)
            {
                system.StopActor(i, this);
            }
            system.RemoveChild(parent, this);
            
            if (Arbitr.Suspended) Arbitr.Resume();
        }       
        internal void Suspend()
        {
            Arbitr.Suspend();

            while (ProcRunning) { Thread.SpinWait(0); }

            foreach (var i in childs.Keys)
            {
                system.SuspendActor(i);
            }
        }
        internal void Resume()
        {
            foreach (var i in childs.Keys)
            {
                system.ResumeActor(i);
            }
            Arbitr.Resume();
        }
        internal void Send(Message msg)
        {
            Arbitr.Send(msg);
        }
        internal void AddChild(ActorRef child)
        {
            childs[child] = child;
        }
        internal void RemoveChild(ActorRef child)
        {
            ActorRef delchild;
            childs.TryRemove(child,out delchild);
        }
        internal ActorRef FindActor(ActorPath fpath)
        {
            if (this.Path == fpath) return this;

            string fchildname = this.Path.GetChildName(fpath);

            var t = childs.Where((x) => { return x.Key.Name == fchildname; });
            if (t.Count() == 0) return null;
            return system.FindActorByPath(t.First().Key, fpath);
        }


        public void Tell(params object[] args)
        {
            Send(new TellMessage()
            {
                args = args,
                Sender = ThreadStaticStorage.Value<ActorRef>() != null ? 
                ThreadStaticStorage.Value<ActorRef>() : 
                system.UserActor
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
                system.UserActor,
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
                system.UserActor,
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
                system.UserActor,
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
