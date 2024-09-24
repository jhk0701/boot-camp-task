using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    class Thief : Character
    {
        public Thief(string name) 
        {
            Name = name;

            Class = EClass.Thief;
            Initialize(Class);
        }

        public override void GetDamage(int val)
        {
            base.GetDamage(val);
        }
    }
}
