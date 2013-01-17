using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class stopchildex
    {
        public ActorRef child { get; set; }
        public Action childstopped { get; set; }
    }
    class restartchildex
    {
        public ActorRef child { get; set; }
        public Action<Exception> childrestarted { get; set; }
    }

    class returnask { }
    class getstatepend { }

    class getparentmsg { }
    class getchildsmsg { }
    class createchildactormsg { }
    class setchildactions
    {
        public Action stopchild { get; set; }
        public Action restartchild { get; set; }
    }
    class stopchild
    {
        public ActorRef child { get; set; }
    }
    class restartchild
    {
        public ActorRef child { get; set; }
    }
    class seteh
    {
        public Func<ExceptionDecision> eh { get; set; }
    }

    class crerateactormsg { }
    class senderdetectmsginto
    {
        public ActorRef sender { get; set; }
    }

    class senderdetectmsg
    {
        public ActorRef dest { get; set; }
        public senderdetectmsginto msg { get; set; }
    }
    class poststoptestmsg
    {
        public Action poststop { get; set; }
    }

    class setstate { public int x { get; set; } }
    class getstate { }
    class setpra
    {
        public Action<int> pra { get; set; }
    }
    class selfstop { }
    class selfrestart { }
    class testmsg
    {
        public string msg { get; set; }
    }
    class acttestmsg : testmsg
    {
        public Func<int> act { get; set; }
    }
    class statemsg
    {
        public int newstate { get; set; }
    }
    class stopmsg { }

}
