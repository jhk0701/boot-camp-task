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
            Level = 1;

            MaxHealth = 80;
            Health = MaxHealth;

            Attack = 15;
            Defense = 7;
        }

        public override void GetDamage(int val)
        {
            base.GetDamage(val);
        }
    }
}
