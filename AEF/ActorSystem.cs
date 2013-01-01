using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class ActorSystem:ActorRefFactory
    {

        private ActorRef DefaultActor_;
        internal ActorRef DefaultActor { get { return DefaultActor_; } }

        public ActorSystem()
        {
            DefaultActor_ = CreateActor<AEF.Actors.DefaultActor>();
        }

        public  override ActorRef CreateActor<T>() 
        {
            return CreateActor(new ActorInstanceGenerator(typeof(T)));
        }

        public override ActorRef CreateActor<T>(params object[] args) 
        {
           return CreateActor(new ActorInstanceGenerator(typeof(T),  args));
        }

        public override ActorRef CreateActor(Func<Actor> Gener)
        {
            return CreateActor(new ActorInstanceGenerator(Gener));
        }

        internal override ActorRef CreateActor(ActorInstanceGenerator Gener)
        {
            var res = new ActorRef(this);
            res.Gen = Gener;
            Actor t = res.Gen.CreateActorInstance();

            t.PredStart();
            res.RunActor(t);


            return res;
        }

        public void StopActor(ActorRef actor) {
            actor.Stop();
            try
            {
                actor.actor.PostStop();
            }
            catch (Exception e)
            {
                actor.ProcPostStopException(e);
            }
        }
        internal void ContextStopActor(ActorRef actor)
        {
            actor.ContextStop();
            try
            {
                actor.actor.PostStop();
            }
            catch (Exception e)
            {
                actor.ProcPostStopException(e);
            }
        }
        public void RestartActor(ActorRef actor, object cause)
        {
            actor.Suspend();
            var t = actor.Gen.CreateActorInstance();

            t.PostRestart(cause);
            actor.RunActor(t);

        }
    
        
    
    }
}
