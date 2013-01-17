using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;
using AEF.Attributes;

namespace AEF.Log
{
    class LogActor:Actor
    {
        private Ilogger logger;

        
        public LogActor()
        {
            this.logger = new ConsoleLogger();
        }

        public int SetLogger(Ilogger log)
        {
            logger = log;
            return 0;
        }


        [Equal(0,"ProcMsgInStoppedActor")]
        public void ProcMsgInStoppedActor(string name,Message msg, ActorRef act)
        {

            if (msg is AskMessage)
            {
                var t = (AskMessage)msg;
                logger.ProcMsgInStoppedActor(t.Sender, t.args, t.ReturnType, act);
                return;
            }
            if (msg is TellMessage)
            {
                var t = (TellMessage)msg;
                logger.ProcMsgInStoppedActor(t.Sender, t.args, null, act);
            }
        }

        [Equal(0, "ProcNotHandledMsg")]
        public void ProcNotHandledMsg(string name, Message msg, ActorRef act)
        {
            //LogActor.Tell("ProcNotHandledMsg", msg, act);
            if (msg is AskMessage)
            {
                var t = (AskMessage)msg;
                logger.ProcNotHandledMsg(t.Sender, t.args, t.ReturnType, act);
                return;
            }
            if (msg is TellMessage)
            {
                var t = (TellMessage)msg;
                logger.ProcNotHandledMsg(t.Sender, t.args, null, act);
            }
        }

        [Equal(0, "ProcPostStopException")]
        public void ProcPostStopException(string name, Exception e, ActorRef act)
        {
            logger.ProcPostStopException(e, act);
        }

        [Equal(0, "ProcUserActorStoppedByException")]
        public void ProcUserActorStoppedByException(string name, Exception e, ActorRef act)
        {
            logger.ProcUserActorStoppedByException(e, act);
        }

        [Equal(0, "ProcSystemActorRestartedByException")]
        public void ProcSystemActorRestartedByException(string name, Exception e, ActorRef act)
        {
            logger.ProcSystemActorRestartedByException(e, act);

        }
    }
}
