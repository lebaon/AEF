using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF.Log
{
    class ConsoleLogger:Ilogger
    {
        void Printf(string s)
        {
            var tc = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(s);
            Console.ForegroundColor = tc;
        }
        
        void Ilogger.ProcMsgInStoppedActor(ActorRef Sender, object[] args, Type ReturnType, ActorRef act)
        {
            string s = "";
            s += "Message sended in stopped actor \n";
            s += String.Format("From: {0}\n", Sender.FullName);
            s += String.Format("To: {0}\n", act.FullName);
            s += String.Format("Return type: {0}\n", ReturnType != null ? ReturnType.ToString() : "null");
            for (int i = 0; i < args.Length; i++)
            {
                s += String.Format("Argument {0} : Type = {1}, Value = {2}\n", 
                    i, args[i].GetType(), args[i]);
            }
            Printf(s);
        }

        void Ilogger.ProcNotHandledMsg(ActorRef Sender, object[] args, Type ReturnType, ActorRef act)
        {
            string s = "";
            s += "Message has no handler in actor \n";
            s += String.Format("From: {0}\n", Sender.FullName);
            s += String.Format("To: {0}\n", act.FullName);
            s += String.Format("Return type: {0}\n", ReturnType != null ? ReturnType.ToString() : "null");
            for (int i = 0; i < args.Length; i++)
            {
                s += String.Format("Argument {0} : Type = {1}, Value = {2}\n",
                    i, args[i].GetType(), args[i]);
            }
            Printf(s);
        }

        void Ilogger.ProcPostStopException(Exception e, ActorRef act)
        {
            string s = "";
            s += "Exception in PostStop procedure\n";
            s += String.Format("In: {0}\n", act.FullName);
            s += String.Format("Exception: {0}\n", e);
            Printf(s);

        }

        void Ilogger.ProcUserActorStoppedByException(Exception e, ActorRef act)
        {
            string s = "";
            s += "User actor has been stopped by exception\n";
            s += String.Format("Actor: {0}\n", act.FullName);
            s += String.Format("Exception: {0}\n", e);
            Printf(s);
        }

        void Ilogger.ProcSystemActorRestartedByException(Exception e, ActorRef act)
        {
            string s = "";
            s += "System actor has been restarted by exception\n";
            s += String.Format("Actor: {0}\n", act.FullName);
            s += String.Format("Exception: {0}\n", e);
            Printf(s);
        }
    }
}
