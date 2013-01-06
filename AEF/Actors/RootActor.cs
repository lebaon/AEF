﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Actors
{
    class RootActor:Actor
    {
        public override ExceptionDecision ChildException(Exception e)
        {
            return ExceptionDecision.Resume;
        }
    }
}
