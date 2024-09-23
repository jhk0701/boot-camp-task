using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    class Thief : Character
    {
        public Thief(string name) 
        {
            Name = name;

            Class = DataDefinition.EClass.Thief;
            Level = 1;

            MaxHealth = 80;
            Health = MaxHealth;

            BaseAttack = 15;
            BaseDefense = 7;
        }

        public override void GetDamage(int val)
        {
            base.GetDamage(val);
        }
    }
}
