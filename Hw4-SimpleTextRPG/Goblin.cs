using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hw4_SimpleTextRPG
{
    class Goblin : Monster
    {
        public Goblin() : base()
        {
            Name = "고블린";

            Health = 30;
            Attack = 10;
        }
    }
}
