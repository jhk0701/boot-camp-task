using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    class Warrior : Character
    {
        public Warrior(string name)
        {
            Name = name;

            Class = DataDefinition.EClass.Warrior;
            Level = 1;

            MaxHealth = 100;
            Health = MaxHealth;

            Attack = 10;
            Defense = 10;
        }

        public override void GetDamage(int val)
        {
            base.GetDamage(val);
        }
    }
}
