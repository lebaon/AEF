using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class setstate { public int x { get; set; } }
    class getstate { }
    class setpra
    {
        public Action<int> pra { get; set; }
    }
    class selfstop { }
    class selfrestart { }


    class PredRestartTestActor:Actor
    {
        private int state = 0;
        private Action<int> pra = null;
        public override void PredRestart()
        {
            base.PredRestart();
            if (pra != null) pra(state);
        }
        public int handler1(setstate msg)
        {
            state = msg.x;
            return state;

        }
        public int handler2(getstate msg)
        {
            return state;
        }
        public int handler3(setpra msg)
        {
            pra = msg.pra;
            return 0;
        }

        public virtual int handler4(selfstop msg)
        {

            try
            {
                Context.StopActor(Context.Self);
            }
            catch { }

            return 0;

        }

        public virtual int handler5(selfrestart msg)
        {

            try
            {
                Context.RestartActor(Context.Self);
            }
            catch { }

            return 0;

        }
    
    
    }

    class SelfRestartableActor : PredRestartTestActor
    {
        public override int handler4(selfstop msg)
        {
            Context.StopActor(Context.Self);
            return 0;
        }

        public override int handler5(selfrestart msg)
        {
            Context.RestartActor(Context.Self);
            return 0;
        }
    }
}
