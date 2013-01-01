using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public abstract class ActorRefFactory
    {
        public abstract ActorRef CreateActor<T>() where T : Actor;
        public abstract ActorRef CreateActor<T>(params object[] args) where T : Actor;
        public abstract ActorRef CreateActor(Func<Actor> Gener);
        internal abstract ActorRef CreateActor(ActorInstanceGenerator Gener);
    }
}
