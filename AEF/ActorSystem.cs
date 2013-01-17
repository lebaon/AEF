using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class ActorSystem : ActorRefFactory, AEF.Helpers.IFluent
    {

        public ActorSystem()
        {
            core = new ActorCore();
        }
        public ActorSystem(Log.Ilogger logger)
        {
            core = new ActorCore(logger);
        }
        private ActorCore core;
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
            var Result = core.CreateActor(Gener, core.UserActor,null);
            
            return Result;
            
        }

        public override ActorRef CreateActor<T>(string Name)
        {
            return CreateActor(Name,new ActorInstanceGenerator(typeof(T)));
        }

        public override ActorRef CreateActor<T>(string Name,params object[] args)
        {
            return CreateActor(Name,new ActorInstanceGenerator(typeof(T), args));
        }

        public override ActorRef CreateActor(string Name,Func<Actor> Gener)
        {
            return CreateActor(Name,new ActorInstanceGenerator(Gener));
        }

        internal override ActorRef CreateActor(string Name,ActorInstanceGenerator Gener)
        {
            var Result = core.CreateActor(Gener, core.UserActor, Name);

            return Result;

        }

        public override ActorRef FindActorByPath(string Path)
        {
            return core.FindActorByPath(Path);
        }
        
        public void StopActor(ActorRef actor)
        {
            core.StopActor(actor,core.UserActor);
        }

        public void StopSystem()
        {
            core.StopSystem();
        }

        public void RestartActor(ActorRef actor)
        {
            Exception e=core.RestartActor(actor, core.UserActor);
            if (e != null) throw e;
        }
    }
}
