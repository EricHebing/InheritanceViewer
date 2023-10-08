using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritanceViewer
{
    using InheritanceInformation = Dictionary<string, List<string>>;
    using Inheritance = Tuple<string, string>;
    using InheritanceLinks = List<Tuple<string,string>>;
    

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
        InheritanceLinks _inheritances = new InheritanceLinks();

        public List<string> Classes
        {
            get { return _classes; }
        }

        public InheritanceLinks Inheritances
        {
            get { return _inheritances; }
        }

        public Graphbuilder()
        {

        }

        public bool build_up_graph(InheritanceInformation ainheritances, InheritanceInformation ainheritances_by, List<string> Classes)
        {
            bool lsuccess = true;

            foreach (string lclass_name in Classes)
            {
                //class was already added, skip it to avoid duplicates in graph
                if (_classes.Contains(lclass_name))
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
        void add_inheritance_infos(InheritanceInformation InheritInfo, string aclass, InheritDirection direction )
        {
            _classes.Add(aclass);
            List<string> inheritances = new List<string>();

            //In case given class is not found in InheritanceInformation return as no additional Information can be gathered
            if (!InheritInfo.TryGetValue(aclass, out inheritances))
                return;

            foreach (var item in InheritInfo[aclass])
            {
                if (direction == InheritDirection.InheritedFrom)
                {
                    _inheritances.Add(new Inheritance(aclass, item));
                    
                }
                else if (direction == InheritDirection.InheritedBy)
                {
                    _inheritances.Add(new Inheritance(item, aclass));
                }
                add_inheritance_infos(InheritInfo, item, direction);
            }

        }


    }
}
