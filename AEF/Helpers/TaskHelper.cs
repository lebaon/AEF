using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEF.Helpers
{
    public class TaskHelper<T>
    {
        public Task<T> Task;
        private T value = default(T);
        private Exception e = null;
        public TaskHelper()
        {
            Task = new Task<T>(TaskProc);
        }
        private T TaskProc()
        {
            if (e == null) return value;
            else throw e;
        }
        public void RunTask(object val,Exception ex)
        {
            if(val!=null) value = (T)val;
            e = ex;
            Task.RunSynchronously();

        }
    }
}
