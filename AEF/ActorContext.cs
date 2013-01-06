using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class ActorContext:ActorRefFactory,AEF.Helpers.IFluent
    {
        private ActorRef Self_;
        
        public ActorRef Self
        {
            get { return Self_; }
            internal set { Self_ = value; }
        }

        internal ActorCore System { get; set; }
        internal Message Msg { get; set; }

        public ActorRef Sender { get { return Msg.Sender; } }
        public bool SenderAsked { get { return Msg is AskMessage; } }

        public ActorRef Parent { get { return Self_.parent; } }
        public ActorRef[] Childs { get { return Self_.childs.Keys.ToArray(); } }

        public void StopActor(ActorRef actor)
        {
            if (actor == Self) throw new AEFActorStopException();
            System.StopActor(actor,Self);

        }
        public void RestartActor(ActorRef actor)
        {
            if (actor == Self) throw new AEFActorRestartException();
            Exception e= System.RestartActor(actor,Self);
            if (e != null) throw e;
        }
        

        public override ActorRef CreateActor<T>() 
        {
             return CreateActor(new ActorInstanceGenerator(typeof(T)));
        }
        public override ActorRef CreateActor<T>(params object[] args) 
        {
            return CreateActor(new ActorInstanceGenerator(typeof(T), args));
        }
        internal override ActorRef CreateActor(ActorInstanceGenerator Gener)
        {
            var Result = System.CreateActor(Gener, Self_,null);
           
            
            return Result;
        }
        public override ActorRef CreateActor(Func<Actor> Gener)
        {
            return CreateActor(new ActorInstanceGenerator(Gener));
        }

        public override ActorRef CreateActor<T>(string Name)
        {
            return CreateActor(Name,new ActorInstanceGenerator(typeof(T)));
        }
        public override ActorRef CreateActor<T>(string Name,params object[] args)
        {
            return CreateActor(Name,new ActorInstanceGenerator(typeof(T), args));
        }
        internal override ActorRef CreateActor(string Name,ActorInstanceGenerator Gener)
        {
            var Result = System.CreateActor(Gener, Self_, Name);


            return Result;
        }
        public override ActorRef CreateActor(string Name,Func<Actor> Gener)
        {
            return CreateActor(Name,new ActorInstanceGenerator(Gener));
        }

        public override ActorRef FindActorByPath(string Path)
        {
            return System.FindActorByPath(Path);
        }
    
    
    }
}
