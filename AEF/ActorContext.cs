using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class ActorContext:ActorRefFactory
    {
        private ActorRef Self_;
        
        public ActorRef Self
        {
            get { return Self_; }
            internal set { Self_ = value; }
        }

        internal ActorSystem System { get; set; }
        internal Message Msg { get; set; }

        public ActorRef Sender { get { return Msg.Sender; } }
        public bool SenderAsked { get { return Msg is AskMessage; } }

        public void Stop()
        {
            System.ContextStopActor(Self_);
        }

        public override ActorRef CreateActor<T>() 
        {
            return System.CreateActor<T>();
        }

        public override ActorRef CreateActor<T>(params object[] args) 
        {
            return System.CreateActor<T>(args);
        }



        internal override ActorRef CreateActor(ActorInstanceGenerator Gener)
        {
            return System.CreateActor(Gener);
        }


        public override ActorRef CreateActor(Func<Actor> Gener)
        {
            return System.CreateActor(Gener);
        }
    }
}
