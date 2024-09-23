using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw4_SimpleTextRPG
{
    interface ICharacter
    {
        /// <summary>
        /// 캐릭터 이름 프로퍼티
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 체력
        /// </summary>
        int Health { get; set; }

        /// <summary>
        /// 공격력
        /// </summary>
        int Attack { get; set; }

        /// <summary>
        /// 생사 여부
        /// </summary>
        bool IsDead { get; set; }

        /// <summary>
        /// 데미지 메서드
        /// </summary>
        /// <param name="damage"></param>
        void TakeDamage(int damage);
    }
}
