using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF.Log;

namespace AEF.Tests.ForLog
{
    class TestLogger:Ilogger
    {
        public ActorRef Sender { get; set; }
        public object[] args { get; set; }
        public Type ReturnType { get; set; }
        public ActorRef Act { get; set; }
        public Exception ex { get; set; }
        public string mname { get; set; }
        public bool setted { get; set; }
        
        public void ProcMsgInStoppedActor(ActorRef Sender, object[] args, Type ReturnType, ActorRef act)
        {
            this.Sender = Sender;
            this.args = args;
            this.ReturnType = ReturnType;
            this.Act = act;
            mname = "ProcMsgInStoppedActor";
            setted = true;
        }

        public void ProcNotHandledMsg(ActorRef Sender, object[] args, Type ReturnType, ActorRef act)
        {
            this.Sender = Sender;
            this.args = args;
            this.ReturnType = ReturnType;
            this.Act = act;
            mname = "ProcNotHandledMsg";
            setted = true;
        }

        public void ProcPostStopException(Exception e, ActorRef act)
        {
            this.ex = e;
            this.Act = act;
            mname = "ProcPostStopException";
            setted = true;
        }

        public void ProcUserActorStoppedByException(Exception e, ActorRef act)
        {
            this.ex = e;
            this.Act = act;
            mname = "ProcUserActorStoppedByException";
            setted = true;
        }

        public void ProcSystemActorRestartedByException(Exception e, ActorRef act)
        {
            this.ex = e;
            this.Act = act;
            mname = "ProcSystemActorRestartedByException";
            setted = true;
        }
    }
}
