﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AEF;

namespace AEF.Tests.Actors
{
    class CreateFaultActor:Actor
    {
        public override void PredStart()
        {
            base.PredStart();
            throw new Exception();
        }
    }
}
