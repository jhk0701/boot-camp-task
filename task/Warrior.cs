using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    class Warrior : Character
    {
        public Warrior(string name)
        {
            Name = name;

            Class = EClass.Warrior;
            Initialize(Class);
        }

        public override void GetDamage(int val)
        {
            base.GetDamage(val);
        }
    }
}
