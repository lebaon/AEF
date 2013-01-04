using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF.Helpers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IFluent
    {


        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();


        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();


        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();


        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object obj);

    }
}
