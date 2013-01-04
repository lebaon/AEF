using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class RestartFaultActor:Actor
    {
        public override void PostRestart()
        {
            base.PostRestart();
            throw new Exception();
        }

        public int handler1(acttestmsg msg)
        {
            return msg.act();
        }

    }
}
