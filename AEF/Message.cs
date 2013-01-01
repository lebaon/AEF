using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{

    class Message
    {
        public ActorRef Sender { get; set; }

    }
    class NOPMessage : Message { }
    class TellMessage : Message
    {
        public object[] args { get; set; }
    }
    class AskMessage : TellMessage
    {
        public Type ReturnType { get; set; }
        public Action<object, Exception> ReturnCode { get; set; }
    }
    class AnswerMessage : Message
    {
        public object ReturnValue { get; set; }
        public Action<object,Exception> ReturnCode { get; set; }
    }
}
