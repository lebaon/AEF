using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class Actor:AEF.Helpers.IFluent
    {
        public ActorContext Context { get; set; }

        public virtual ExceptionDecision ChildException(Exception e)
        {
            return ExceptionDecision.Excalation;
        }
        public virtual void PredStart() { }
        public virtual void PostStop() { }
        public virtual void PostRestart() { }
        public virtual void PredRestart()
        {
            foreach (var i in Context.Childs)
                Context.StopActor(i);
        }
    }
}
