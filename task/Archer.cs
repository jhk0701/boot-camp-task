using DataDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace task
{
    class Archer : Character
    {
        public Archer(string name)
        {
            Name = name;

            Class = EClass.Archer;
            Initialize(Class);
        }

        public override void GetDamage(int val)
        {
            base.GetDamage(val);
        }
    }
}
