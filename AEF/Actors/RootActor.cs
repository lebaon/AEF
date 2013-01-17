using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Actors
{
    class RootActor:Actor
    {
        public override ExceptionDecision ChildException(Exception e)
        {
            return ExceptionDecision.Resume;
        }
        public override void PredStart()
        {
            var ua=Context.CreateActor("user", new ActorInstanceGenerator(typeof(UserActor)));
            var sa=Context.CreateActor("system", new ActorInstanceGenerator(typeof(SystemActor)));
            Context.SetPriority(ua, 0);
            Context.SetPriority(sa, 1);
        }
    }
}
