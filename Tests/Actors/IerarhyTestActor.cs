using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class getparentmsg { }
    class getchildsmsg { }
    class createchildactor { }
    class stopchild
    {
        public ActorRef child { get; set; }
    }
    class seteh
    {
        public Func<ExceptionDecision> eh { get; set; }
    }
    
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
        public virtual ActorRef handler3(createchildactor msg)
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

        public override ActorRef handler3(createchildactor msg)
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
}
