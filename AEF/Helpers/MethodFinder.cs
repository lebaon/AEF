using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AEF.Attributes;
using AEF;

namespace AEF.Helpers
{

    public class Signature
    {
        public Type[] ParameterTypes { get; set; }
        public Type ReturnType { get; set; }
        public bool Equal(Signature sig)
        {
            return this.ReturnType == sig.ReturnType && this.ParameterTypes.Length == sig.ParameterTypes.Length &&
                (this.ParameterTypes.Length == 0 ? true : this.ParameterTypes.Select(
                (x, i) =>
                {
                    return x == sig.ParameterTypes[i];
                }).Aggregate((a, b) => { return a & b; }));
        }
    }
    
    public class MethodFinder
    {

        private Dictionary<Signature, MethodInfo[]> MethodsBySignature = new Dictionary<Signature, MethodInfo[]>();
        private Dictionary<MethodInfo, Tuple<bool[], int>> BeRestrictions = new Dictionary<MethodInfo, Tuple<bool[], int>>();
        private Dictionary<Tuple<MethodInfo, int, object>, bool> Restrictions = new Dictionary<Tuple<MethodInfo, int, object>, bool>();
        private Dictionary<Tuple<MethodInfo, int>, bool> RestrPolar = new Dictionary<Tuple<MethodInfo, int>, bool>();

        public MethodInfo GetMethodByParamsAndReturnValueType(Type ReturnType, object[] args)
        {
            
            Signature sig = new Signature();
            if (args == null) args = new object[0];
            if (ReturnType != null)
                sig.ReturnType = ReturnType;
            else
                sig.ReturnType = typeof(void);
            sig.ParameterTypes = args.GetTypes();

            var t1 = MethodsBySignature.Keys.Where(
                (s) => { return s.CanStore(sig); }).Select(
                (s) => { return MethodsBySignature[s]; });
            if (t1.Count() == 0) return null;
            var t5 = t1.Aggregate(
                (a, b) => { return a.Concat(b).ToArray(); });
            

            var t2 = t5.Where(
                (m) =>
                {
                    var tup = BeRestrictions[m];
                    return tup.Item1.Length == 0 ? true : tup.Item1.Select((b, i) =>
                    {
                        return !b ? true : (RestrPolar[new Tuple<MethodInfo, int>(m, i)] ? Restrictions.ContainsKey(
                            new Tuple<MethodInfo, int, object>(m, i, args[i])) : !Restrictions.ContainsKey(
                            new Tuple<MethodInfo, int, object>(m, i, args[i])));
                    }).Aggregate((a, b) => { return a && b; });
                }).OrderByDescending(
                (m) => { return BeRestrictions[m].Item2; });
            var t3 = t2.GroupBy(
                (m) =>
                {
                    return BeRestrictions[m].Item2;
                });
            var t4 = t3.First().ToArray().
                OrderBy(
                (m) =>
                {
                    int dist = sig.Distance(m.GetSignature());
                    return dist;
                }).
                FirstOrDefault();
            return t4;


        }

        public MethodFinder(Type at)
        {
            //var methods =at.GetMethods(BindingFlags.Public);
            var BannedMethods = typeof(Actor).GetMethods();
            var methods = at.GetMethods();
            methods = methods.Where(
                (m) =>
                {
                    bool f = BannedMethods.ContainsMethod(m);
                    f = !f;
                    return f;
                }).ToArray();
            methods.Select((m) =>
            {
                var sig = m.GetSignature();
                if (!MethodsBySignature.ContainsKey(sig)) MethodsBySignature.Add(sig, new MethodInfo[0]);
                MethodsBySignature[sig] = MethodsBySignature[sig].Concat(new MethodInfo[] { m }).ToArray();

                bool[] restr = new bool[m.GetParameters().Length];
                int restrcount = 0;
                var tttx= m.GetCustomAttributes(true);
                tttx.Where(
                    (ca) => { return ca is EqualAttribute; }).Select(
                    (a) => { return (EqualAttribute)a; }).Select(
                    (atr, i) =>
                    {
                        if (atr.ParamNumber > restr.Length - 1) return atr;
                        restr[atr.ParamNumber] = true;
                        restrcount++;

                        Restrictions.Add(new Tuple<MethodInfo, int, object>(m, atr.ParamNumber, atr.EqualValue), true);
                        RestrPolar.Add(new Tuple<MethodInfo, int>(m, atr.ParamNumber), true);
                        return atr;
                    }).ToArray();

                tttx.Where(
                    (ca) => { return ca is NotEqualAttribute; }).Select(
                    (a) => { return (NotEqualAttribute)a; }).Select(
                    (atr, i) =>
                    {
                        if (atr.ParamNumber > restr.Length - 1) return atr;
                        restr[atr.ParamNumber] = true;
                        restrcount++;

                        Restrictions.Add(new Tuple<MethodInfo, int, object>(m, atr.ParamNumber, atr.EqualValue), false);
                        RestrPolar.Add(new Tuple<MethodInfo, int>(m, atr.ParamNumber), false);
                        return atr;
                    }).ToArray();

                BeRestrictions.Add(m, new Tuple<bool[], int>(restr, restrcount));


                return sig;
            }).ToArray();
        }



    }
}
