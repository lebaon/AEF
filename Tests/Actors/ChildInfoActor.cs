using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class ChildInfoActor:Actor
    {
        private Action chst = null;
        private Action chrst = null;

        public override void ChildStop()
        {
            if (chst != null) chst();
        }

        public override void ChildRestart()
        {
            if (chrst != null) chrst();
        }
        public ActorRef h1(crerateactormsg msg)
        {
            return Context.CreateActor<ChildInfoActor>("child");
        }
        public int h2(stopchild msg)
        {
            var t = Context.PendingReturn();
            Context.StopActor(msg.child, () =>
            {
                t.Return(0);



            });
            return 1;
        }
        public int h3(restartchild msg)
        {
            var t = Context.PendingReturn();
            Context.RestartActor(msg.child,(e)=>{
                t.Return(0);
                
            });
            return 1;
        }
        public int h4(selfstop msg)
        {
            Context.StopActor(Context.Self);
            return 0;
        }
        public int h5(selfrestart msg)
        {
            Context.RestartActor(Context.Self);
            return 0;
        }

        public int h6(Action setchst, Action setchrst)
        {
            this.chrst = setchrst;
            this.chst = setchst;
            return 0;
        }

    }
}
