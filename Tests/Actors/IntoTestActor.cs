﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEF;

namespace AEF.Tests.Actors
{
   
    class IntoTestActor:Actor
    {
        int state = 0;
        Action ps = null;
        public override void PredStart()
        {
            base.PredStart();
            state = 10;
        }
        public override void PostRestart()
        {
            base.PostRestart();
             state = 15;
            
        }

        public override void PostStop()
        {
            base.PostStop();
            if (ps != null) ps();
        }

        public ActorRef handler1(crerateactormsg msg)
        {
            return Context.CreateActor<IntoTestActor>();
        }
        public int handler2(acttestmsg msg)
        {
            Console.WriteLine(msg.msg);
            return msg.act();

        }
        public Task<bool> handler3(senderdetectmsg msg)
        {
            return msg.dest.Ask<bool>(msg.msg);
        }
        public bool handler4(senderdetectmsginto msg)
        {
            return Context.Sender == msg.sender;

        }
        public int handler5(statemsg msg)
        {
            return state;
        }
        public int handler6(poststoptestmsg msg)
        {
            ps = msg.poststop;
            return 0;
        }
    }
}
