using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw4_SimpleTextRPG
{
    class HealthPotion : IItem
    {
        public string Name { get; set; }
        public void Use(Warrior warrior)
        {
            int val = 500;
            Console.WriteLine("체력 포션을 사용합니다.");
            Console.WriteLine("{0}만큼의 체력이 상승합니다.",  val);
            warrior.Health += val;
            Thread.Sleep(1000);
        }
    }
}
