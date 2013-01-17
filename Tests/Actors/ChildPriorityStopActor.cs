using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class ChildPriorityStopActor:Actor
    {
        private Action st = null;

        public override void PostStop()
        {
            if (st != null) st();
        }
        
        public int h1(selfstop msg)
        {
            Context.StopActor(Context.Self);
            return 0;
        }
        public int h2(selfrestart msg)
        {
            Context.RestartActor(Context.Self);
            return 0;
        }
        public ActorRef h3(crerateactormsg msg)
        {
            return Context.CreateActor<ChildPriorityStopActor>("child");
        }

        public int h4(ActorRef child, int priority)
        {
            Context.SetPriority(child, priority);
            return 0;
        }

        public int h5(Action setst)
        {
            st = setst;
            return 0;
        }
        
        
    }
}
