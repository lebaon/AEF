using System;


namespace AEF
{
    class ActorInstanceGenerator
    {
        private Func<Actor> Generator;
        public ActorInstanceGenerator(Type t, object[] args)
        {
            Generator = () => (Actor)Activator.CreateInstance(t, args);
        }
        public ActorInstanceGenerator(Func<Actor> Gener)
        {
            Generator = Gener;
        }
        public ActorInstanceGenerator(Type t)
        {
            Generator = () => (Actor)Activator.CreateInstance(t);
        }

        public Actor CreateActorInstance()
        {
            return Generator();
        }
        

    }
}
