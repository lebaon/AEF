using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    class AEFException:Exception
    {
        public AEFException(string Message) : base(Message) { }
    }
}
