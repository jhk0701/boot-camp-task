using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw4_SimpleTextRPG
{
    class Monster : ICharacter
    {
        public string Name { get; set; }
        int _health;
        public int Health
        {
            get { return _health; }
            set
            {
                if (IsDead) return;

                _health = value;

                if (_health <= 0)
                {
                    _health = 0;
                    IsDead = true;

                    OnDead?.Invoke();
                }
            }
        }
        public int Attack { get; set; }
        public bool IsDead { get; set; }

        public delegate void DeadEventHandler();
        public DeadEventHandler OnDead;

        public Monster()
        {

            IsDead = false;
            OnDead = () =>
            {
                Console.WriteLine("{0}이/가 죽었습니다.", Name);
            };
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
        }
    }
}
