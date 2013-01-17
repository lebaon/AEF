using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    
    
    class IerarhyTestActor:Actor
    {
       
        public ActorRef handler1(getparentmsg msg)
        {
            return Context.Parent;
        }
        public ActorRef[] handler2(getchildsmsg msg)
        {
            return Context.Childs;
        }
        public virtual ActorRef handler3(createchildactormsg msg)
        {
            return Context.CreateActor<IerarhyTestActor>("child");
        }
        public int handler4(acttestmsg msg)
        {
            return msg.act();
        }

        public int handler5(stopchild msg)
        {
            Context.StopActor(msg.child);
            return 0;
        }

    }

    class ExceptionActor : IerarhyTestActor
    {
        private Func<ExceptionDecision> eh = null;
        private int state = 0;

        public override ExceptionDecision ChildException(Exception e)
        {
            if (eh != null) return eh();
            return base.ChildException(e);
        }
        public int handler6(seteh msg)
        {
            eh = msg.eh;
            return 0;

        }

        public override ActorRef handler3(createchildactormsg msg)
        {
            return Context.CreateActor<ExceptionActor>();
        }
        public int handler7(setstate msg)
        {
            state = msg.x;
            return state;

        }
        public int handler8(getstate msg)
        {
            return state;
        }
    }


    class IerarhyTestActorOvveride : IerarhyTestActor
    {
        public override void PredRestart()
        {
            //base.PredRestart();
        }
    }


    class ChildStopRestartActor : Actor
    {
        private Action stch = null;
        private Action rstch = null;

        public override void ChildRestart()
        {
            if (rstch != null) rstch();
        }


        public override void ChildStop()
        {
            if (stch != null) stch();
        }

        public virtual ActorRef handler3(createchildactormsg msg)
        {
            return Context.CreateActor<IerarhyTestActor>("child");
        }
        public int handler5(stopchild msg)
        {
            var t = Context.PendingReturn();
            Context.StopActor(msg.child, () =>
            {
                t.Return(0);
            });
            return 0;
        }

        public int handler6(restartchild msg)
        {
            var t = Context.PendingReturn();
            Context.RestartActor(msg.child, (e) =>
            {

                t.Return(0);


            });
            return 0;
        }

        public int handler(setchildactions msg)
        {
            stch = msg.stopchild;
            rstch = msg.restartchild;
            return 0;

        }

    }
}
