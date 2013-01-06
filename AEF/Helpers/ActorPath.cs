using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AEF.Helpers
{
    public class ActorPath:IEquatable<ActorPath>
    {
        private string[] SelfPath = null;


        public ActorPath(string SelfName, ActorPath Path) {

            SelfPath = new string[Path.SelfPath.Length + 1];
            for (int i = 0; i < Path.SelfPath.Length; i++)
                SelfPath[i] = Path.SelfPath[i];

            SelfPath[SelfPath.Length - 1] = SelfName;
        
        }
        public ActorPath(string Path)
        {
            if (Path == null) throw new ArgumentNullException();
            char[] separator = new char[2] { '\\', '/' };
            SelfPath = Path.Split(separator);

        }

        public string GetChildName(ActorPath path)
        {
            if (path.SelfPath.Length <= SelfPath.Length) return null;
            return path.SelfPath[SelfPath.Length];
        }

        public bool Equals(ActorPath other)
        {
            if (SelfPath.Length != other.SelfPath.Length) return false;
            bool f = true;

            for (int i = 0; i < SelfPath.Length; i++)
            {
                f &= SelfPath[i] == other.SelfPath[i];
                if (!f) break;
            }
            return f;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < SelfPath.Length; i++)
            {
                s += SelfPath[i]+"\\";
            }
            s = s.Remove(s.Length - 1);
            return s;
        }

        public override bool Equals(object obj)
        {
            if (obj is ActorPath) return Equals((ActorPath)obj);

            return false;
        }

        public override int GetHashCode()
        {
            return SelfPath.
                Select((x) => { return x.GetHashCode(); }).
                Aggregate((a, b) => { return a ^ b; });

        }

        public static bool operator ==(ActorPath a, ActorPath b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ActorPath a, ActorPath b)
        {
            return !a.Equals(b);
        }
    }
}
