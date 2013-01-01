using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace Tests.Actors
{
    class RestartFaultActor:Actor
    {
        public override void PostRestart(object cause)
        {
            base.PostRestart(cause);
            throw new Exception();
        }

    }
}
