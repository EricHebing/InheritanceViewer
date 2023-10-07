using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritanceViewer
{
    enum InheritDirection
    {
        InheritedFrom,
        InheritedBy
    }


    public class Graphbuilder
    {
        //Classes are handled as nodes in the graph to built
        List<string> _classes = new List<string>();

        //inheritances are handled as edged in the graph to built
        List<Tuple<string, string>> _inheritances = new List<Tuple<string, string>>();

        public List<string> Classes
        {
            get { return _classes; }
        }

        public List<Tuple<string, string>> Inheritances
        {
            get { return _inheritances; }
        }




        public Graphbuilder()
        {

        }

        public bool build_up_graph(Dictionary<string, List<string>> ainheritances, Dictionary<string, List<string>> ainheritances_by, string aselected_class)
        {
            bool lsucces = true;

            add_inheritance_infos(ainheritances, aselected_class, InheritDirection.InheritedFrom);
            add_inheritance_infos(ainheritances_by, aselected_class, InheritDirection.InheritedBy);


            //aselected_class was added twice to the list, so delete it

            _classes = _classes.Distinct().ToList();

            return lsucces;
        }


        public bool build_up_graph(Dictionary<string, List<string>> ainheritances, Dictionary<string, List<string>> ainheritances_by, List<string> Classes)
        {
            bool lsuccess = true;

            foreach (string lclass_name in Classes)
            {
                //class was already added, skip it to avoid duplicates in graph
                if(_classes.Contains(lclass_name))
                {
                    continue;
                }

                add_inheritance_infos(ainheritances, lclass_name, InheritDirection.InheritedFrom);
                add_inheritance_infos(ainheritances_by, lclass_name, InheritDirection.InheritedBy);
            }

            //aselected_class was added twice to the list, so delete it

            _classes = _classes.Distinct().ToList();

            return lsuccess;
        }

        void add_inheritance_infos(Dictionary<string, List<string>> ainherit, string aclass, InheritDirection adirection )
        {
            _classes.Add(aclass);
            List<string> inheritances = new List<string>();

            if (!ainherit.TryGetValue(aclass, out inheritances))
                return;

            foreach (var item in ainherit[aclass])
            {
                if (adirection == InheritDirection.InheritedFrom)
                {
                    _inheritances.Add(new Tuple<string, string>(aclass, item));
                    
                }
                else if (adirection == InheritDirection.InheritedBy)
                {
                    _inheritances.Add(new Tuple<string, string>(item, aclass));
                }
                add_inheritance_infos(ainherit, item, adirection);
            }

        }


    }
}
