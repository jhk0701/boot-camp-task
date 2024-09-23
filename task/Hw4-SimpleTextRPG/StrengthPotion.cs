using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw4_SimpleTextRPG
{
    class StrengthPotion : IItem
    {
        public string Name { get; set; }
        public void Use(Warrior warrior)
        {
            int val = 1000;
            Console.WriteLine("힘 포션을 사용합니다.");
            Console.WriteLine("{0}만큼의 힘이 상승합니다.", val);
            warrior.Attack += val;
            Thread.Sleep(1000);
        }
    }
}
