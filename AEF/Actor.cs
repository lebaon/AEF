using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class Actor:AEF.Helpers.IFluent
    {
        public ActorContext Context { get; set; }

        public virtual void ChildStop() { }
        public virtual void ChildRestart() { }


        public virtual ExceptionDecision ChildException(Exception e)
        {
            return ExceptionDecision.Excalation;
        }
        public virtual void PredStart() { }
        public virtual void PostStop() { }
        public virtual void PostRestart() { }
        public virtual void PredRestart()
        {
            var t = Context.ChildsPriority.GroupBy((x) => { return x.Item2; }).
                OrderBy((x) => { return x.First().Item2; }).
                ToArray();

            for (int i = 0; i < t.Length; i++)
            {
                var tt = t[i].ToArray();
                for (int j = 0; j < tt.Length; j++)
                {
                    Context.StopActor(tt[j].Item1);
                }
            }
            foreach (var i in Context.Childs)
                Context.StopActor(i);
        }
    }
}
