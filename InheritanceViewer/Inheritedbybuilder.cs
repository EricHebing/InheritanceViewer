using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InheritanceViewer
{
    using InheritanceInformation = Dictionary<string, List<string>>;
    public class Inheritedbybuilder
    {
        //Standard-constructor
        public Inheritedbybuilder()
        {

        }

        //gets a list of classes and their inheritances. Returns a list of classes and a list of the classes that inherit from this one
        public InheritanceInformation BuildInheritedByList(Dictionary<string, List<string>> class_inheritances)
        {
            InheritanceInformation lclasses_inherited_by = new InheritanceInformation();

            foreach (var entry in class_inheritances)
            {
                foreach (var baseclass in entry.Value)
                {
                    if(!lclasses_inherited_by.ContainsKey(baseclass))
                    {
                        lclasses_inherited_by.Add(baseclass, new List<string>());
                    }
                    lclasses_inherited_by[baseclass].Add(entry.Key);
                }
            }
            return lclasses_inherited_by;
        }

    }
}
