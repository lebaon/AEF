using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    internal class ActorCore
    {

        private ActorRef DefaultActor_;
        internal ActorRef DefaultActor { get { return DefaultActor_; } }

        public ActorCore()
        {
            DefaultActor_ = CreateActor(new ActorInstanceGenerator(typeof(AEF.Actors.DefaultActor)), null);
        }


        
        internal  ActorRef CreateActor(ActorInstanceGenerator Gener,ActorRef parent)
        {
            var res = new ActorRef(this,parent);
            res.Gen = Gener;
            Actor t = res.Gen.CreateActorInstance();


            res.SetActor(t);
            res.RunPredStart(parent);
            res.RunActor();

            if (parent != null) AddChild(parent, res);
            return res;
        }


        private void AddChild(ActorRef Parent, ActorRef Child)
        {
            Parent.AddChild(Child);
        }
        public void RemoveChild(ActorRef Parent, ActorRef Child)
        {
            if (Parent != null) Parent.RemoveChild(Child);
        }
        public void StopActor(ActorRef actor,ActorRef initiator) {
            if (actor == initiator | initiator.childs.ContainsKey(actor))
            {
                actor.Stop();
                actor.RunPostStop(initiator);
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
                if (!actor.RunPredRestart(initiator,ref e))
                {
                    StopActor(actor,initiator);
                    return e;
                }
                var t = actor.Gen.CreateActorInstance();
                actor.SetActor(t);
                if(!actor.RunPostRestart(initiator,ref e))
                {
                    StopActor(actor, initiator);
                    return e;
                }
                
                actor.RunActor();
                return e;
            }
            else
            {
                throw new AEFException("Нельзя перезапускать актор, не являющийся прямым потомком");
            }

        }
    
        
    
    }
}
