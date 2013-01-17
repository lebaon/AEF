using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF.Log
{
    public interface Ilogger
    {
        void ProcMsgInStoppedActor(ActorRef Sender,object[] args, Type ReturnType, ActorRef act);
        void ProcNotHandledMsg(ActorRef Sender, object[] args, Type ReturnType, ActorRef act);
        void ProcPostStopException(Exception e, ActorRef act);
        void ProcUserActorStoppedByException(Exception e, ActorRef act);
        void ProcSystemActorRestartedByException(Exception e, ActorRef act);
    }
}
