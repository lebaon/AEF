using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class Actor
    {
        public ActorContext Context { get; set; }

        public virtual void PredStart() { }
        public virtual void PostStop() { }
        public virtual void PostRestart(object cause) { }
    }
}
