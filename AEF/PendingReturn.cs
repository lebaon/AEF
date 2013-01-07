using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF
{
    public class PendingReturn
    {
        internal ActorRef Asker { get; set; }
        internal Type ReturnType { get; set; }
        internal Action<object, Exception> ReturnCode { get; set; }
        internal ActorRef Self { get; set; }
        internal bool IsPending { get; set; }

        public void Return(object Result)
        {
            if (!ReturnType.IsAssignableFrom(Result.GetType())) throw new AEFException();
            Asker.Send(new AnswerMessage()
                    {
                        ReturnValue = Result,
                        ReturnCode = ReturnCode,
                        Sender = Self
                    });
        }
        public void ReturnException(Exception ex) {

            Asker.Send(new AnswerMessage()
            {
                ReturnValue = ex,
                ReturnCode = ReturnCode,
                Sender = Self
            });
        
        
        }
    }
}
