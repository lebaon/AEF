using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public abstract class ActorRefFactory : AEF.Helpers.IFluent
    {
        public abstract ActorRef CreateActor<T>() where T : Actor;
        public abstract ActorRef CreateActor<T>(params object[] args) where T : Actor;
        public abstract ActorRef CreateActor(Func<Actor> Gener);
        internal abstract ActorRef CreateActor(ActorInstanceGenerator Gener);
        public abstract ActorRef CreateActor<T>(string Name) where T : Actor;
        public abstract ActorRef CreateActor<T>(string Name,params object[] args) where T : Actor;
        public abstract ActorRef CreateActor(string Name,Func<Actor> Gener);
        internal abstract ActorRef CreateActor(string Name,ActorInstanceGenerator Gener);
        public abstract ActorRef FindActorByPath(string Path);
    }
}
