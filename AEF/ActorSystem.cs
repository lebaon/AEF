using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class ActorSystem : ActorRefFactory, AEF.Helpers.IFluent
    {
        private ActorCore core = new ActorCore();
        public override ActorRef CreateActor<T>()
        {
            return CreateActor(new ActorInstanceGenerator(typeof(T)));
        }

        public override ActorRef CreateActor<T>(params object[] args)
        {
            return CreateActor(new ActorInstanceGenerator(typeof(T), args));
        }

        public override ActorRef CreateActor(Func<Actor> Gener)
        {
            return CreateActor(new ActorInstanceGenerator(Gener));
        }

        internal override ActorRef CreateActor(ActorInstanceGenerator Gener)
        {
            var Result = core.CreateActor(Gener, core.DefaultActor);
            
            return Result;
            
        }
        public void StopActor(ActorRef actor)
        {
            core.StopActor(actor,core.DefaultActor);
        }

        public void RestartActor(ActorRef actor)
        {
            Exception e=core.RestartActor(actor, core.DefaultActor);
            if (e != null) throw e;
        }
    }
}
