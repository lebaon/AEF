using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    internal class ActorCore
    {

        private ActorRef UserActor_;
        internal ActorRef UserActor { get { return UserActor_; } }
        private ActorRef RootActor;
        private ActorRef LogActor;


        public ActorCore()
        {
            RootActor = CreateActorEx(new ActorInstanceGenerator(typeof(Actors.RootActor)), null, "");
            //UserActor_ = CreateActor(new ActorInstanceGenerator(typeof(Actors.UserActor)), RootActor, "user");
            //var SystemActor = CreateActor(new ActorInstanceGenerator(typeof(Actors.SystemActor)), RootActor, "system");
            //LogActor = CreateActor(new ActorInstanceGenerator(typeof(Log.LogActor)), RootActor, "log");
            UserActor_ = FindActorByPath("\\user");
            LogActor = FindActorByPath("\\system\\log");

        }
        public ActorCore(Log.Ilogger logger)
        {
            RootActor = CreateActorEx(new ActorInstanceGenerator(typeof(Actors.RootActor)), null, "");
            //UserActor_ = CreateActor(new ActorInstanceGenerator(typeof(Actors.UserActor)), RootActor, "user");
            //var SystemActor = CreateActor(new ActorInstanceGenerator(typeof(Actors.SystemActor)), RootActor, "system");
            //LogActor = CreateActor(new ActorInstanceGenerator(typeof(Log.LogActor)), RootActor, "log");
            UserActor_ = FindActorByPath("\\user");
            LogActor = FindActorByPath("\\system\\log");
            LogActor.Ask<int>(logger).Wait();
        }

        public void ProcMsgInStoppedActor(Message msg, ActorRef act)
        {
            if (!LogActor.isStopped)
                LogActor.Tell("ProcMsgInStoppedActor", msg, act);
        }
        public void ProcNotHandledMsg(Message msg, ActorRef act)
        {
            if (!LogActor.isStopped)
                LogActor.Tell("ProcNotHandledMsg", msg, act);

        }
        public void ProcPostStopException(Exception e, ActorRef act)
        {
            if (!LogActor.isStopped)
                LogActor.Tell("ProcPostStopException", e, act);
        }
        public void ProcUserActorStoppedByException(Exception e, ActorRef act)
        {
            if (!LogActor.isStopped)
                LogActor.Tell("ProcUserActorStoppedByException", e, act);
        }
        public void ProcSystemActorRestartedByException(Exception e, ActorRef act)
        {
            if (!LogActor.isStopped)
                LogActor.Tell("ProcSystemActorRestartedByException", e, act);

        }

        internal ActorRef CreateActor(ActorInstanceGenerator Gener, ActorRef parent, string Name)
        {
            if (Name == "") Name = null;
            if (Name != null)
                if (Name.Contains('\\') | Name.Contains('/')) throw new ArgumentException();
            return CreateActorEx(Gener, parent, Name);
        }

        private ActorRef CreateActorEx(ActorInstanceGenerator Gener, ActorRef parent, string Name)
        {
            ActorRef res;

            if (Name != null) res = new ActorRef(this, parent, Name);
            else res = new ActorRef(this, parent);
            res.Gen = Gener;
            Actor t = res.Gen.CreateActorInstance();


            res.SetActor(t);
            res.RunPredStart(parent);
            res.RunActor();

            if (parent != null) AddChild(parent, res);
            return res;
        }

        public ActorRef FindActorByPath(string path)
        {
            var fpath = new Helpers.ActorPath(path);
            return FindActorByPath(RootActor, fpath);
        }

        public ActorRef FindActorByPath(ActorRef actor, Helpers.ActorPath path)
        {
            return actor.FindActor(path);

        }

        private void AddChild(ActorRef Parent, ActorRef Child)
        {
            Parent.AddChild(Child);
        }
        public void RemoveChild(ActorRef Parent, ActorRef Child)
        {
            if (Parent != null) Parent.RemoveChild(Child);
        }
        public void StopActor(ActorRef actor, ActorRef initiator)
        {
            if (actor == initiator | initiator.childs.ContainsKey(actor))
            {
                actor.Stop();
                actor.RunPostStop(initiator);
                if (actor.parent != null) actor.parent.Send(new ChildStopMessage() { Sender = actor });

            }
            else
            {
                throw new AEFException("Нельзя останавливать актор, не являющийся прямым потомком");
            }
        }

        public void SuspendActor(ActorRef actor)
        {
            actor.Suspend();

        }

        public void StopSystem()
        {
            StopActor(RootActor, RootActor);
        }

        public void ResumeActor(ActorRef actor)
        {
            actor.Resume();
        }

        public Exception RestartActor(ActorRef actor, ActorRef initiator)
        {
            if (actor == initiator | initiator.childs.ContainsKey(actor))
            {
                if (!actor.Suspended) SuspendActor(actor);
                Exception e = null;
                if (!actor.RunPredRestart(initiator, ref e))
                {
                    StopActor(actor, initiator);
                    return e;
                }
                var t = actor.Gen.CreateActorInstance();
                actor.SetActor(t);
                if (!actor.RunPostRestart(initiator, ref e))
                {
                    StopActor(actor, initiator);
                    return e;
                }

                actor.RunActor();
                if (actor.parent != null) actor.parent.Send(new ChildRestartMessage() { Sender = actor });

                return e;
            }
            else
            {
                throw new AEFException("Нельзя перезапускать актор, не являющийся прямым потомком");
            }

        }



    }
}
