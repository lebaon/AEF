﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using AEF.Tests.ForAEF;
using System.Reflection;


namespace TestRunner
{
    
    class Program
    {
        
        static void Main(string[] args)
        {

            var t = new ActorIerarhyTests();
            t.ChildsStoppedInPriorityOrder();


            Console.ReadLine();
        }
    }
}
