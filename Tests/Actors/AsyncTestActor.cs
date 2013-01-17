using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    
    
    class AsyncTestActor:Actor
    {
        private int state = 0;
        private PendingReturn ar = null;
        public ActorRef h1(createchildactormsg msg)
        {
            return Context.CreateActor<AsyncTestActor>("child");
        }
        public void h2(stopchildex msg)
        {
            Context.StopActor(msg.child, msg.childstopped);

        }
        public void h3(restartchildex msg)
        {
            Context.RestartActor(msg.child, msg.childrestarted);
        }

        public int h4(setstate msg)
        {
            state = msg.x;
            return state;
        }

        public int h5(getstate msg)
        {
            return state;
        }

        public int h6(getstatepend msg)
        {
            ar = Context.PendingReturn();
            return default(int);
        }

        public void h7(returnask msg)
        {
            ar.Return(state);
        }


        
    }
}
