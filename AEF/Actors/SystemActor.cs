using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Actors
{
    class SystemActor:Actor
    {
        public override ExceptionDecision ChildException(Exception e)
        {
            Context.System.ProcSystemActorRestartedByException(e, Context.Sender);
            return ExceptionDecision.Restart;
        }
        public override void PredStart()
        {
            var la = Context.CreateActor("log", new ActorInstanceGenerator(typeof(Log.LogActor)));
            Context.SetPriority(la, 1);
        }
    }
}
