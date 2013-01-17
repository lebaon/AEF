using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Actors
{
    class UserActor:Actor
    {
        public override ExceptionDecision ChildException(Exception e)
        {
            Context.System.ProcUserActorStoppedByException(e, Context.Sender);
            return ExceptionDecision.Stop;
        }

    }
}
