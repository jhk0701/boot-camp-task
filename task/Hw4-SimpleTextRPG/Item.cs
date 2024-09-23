using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw4_SimpleTextRPG
{
    interface IItem
    {
        /// <summary>
        /// 아이템 이름
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 사용 메서드. 매개변수 : 사용 주체
        /// </summary>
        /// <param name="character"></param>
        void Use(Warrior warrior);

    }
}