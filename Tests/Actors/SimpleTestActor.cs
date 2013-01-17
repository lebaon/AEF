using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
   
    class SimpleTestActor : Actor
    {
        private int state = 0;
        public void handler1(acttestmsg msg)
        {
            Console.WriteLine(msg.msg);
            msg.act();
            
            
        }
        public int handler2(acttestmsg msg)
        {
            Console.WriteLine(msg.msg);
            return msg.act();

        }
        public int handler3(statemsg msg)
        {
            int t = state;
            state = msg.newstate;
            return t;
        }

        public int handler4(stopmsg msg) {

            Context.StopActor(Context.Self);
            return 10;
        }
    
    }
}
