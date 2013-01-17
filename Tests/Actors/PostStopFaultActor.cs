using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class PostStopFaultActor:Actor
    {
        private Action ps = null;

        public override void PostStop()
        {
            if (ps != null) ps();
        }
        
        public int h1(stopmsg msg)
        {

            Context.StopActor(Context.Self);
            return 10;
        }

        public int h2(Action ps)
        {
            this.ps = ps;
            return 0;
        }
        public int h3(acttestmsg msg)
        {

            return msg.act();
        }
    }
}
