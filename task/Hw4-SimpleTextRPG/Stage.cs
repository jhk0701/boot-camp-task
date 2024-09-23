using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hw4_SimpleTextRPG
{
    class Stage
    {
        public int Level { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsPlayerWin {  get; set; }

        public enum EMonster : int
        {
            Goblin = 0,
            Dragon,
        }
        public ICharacter CreateMonster(EMonster type)
        {
            switch (type)
            {
                case EMonster.Goblin:
                    return new Goblin();
                case EMonster.Dragon:
                    return new Dragon();
                default:
                    return new Goblin();
            }
        }
        Dictionary<int, int> _stage = new Dictionary<int, int>() {
            { 1, (int)EMonster.Goblin},
            { 2, (int)EMonster.Dragon}
        };

        ICharacter _player;
        ICharacter _monster;
        int _turn = 0; // 홀수턴 : 플레이어, 짝수턴 : 몬스터

        public Stage(int num, ICharacter player)
        {
            Level = num;

            _player = player;
            _monster = CreateMonster((EMonster)_stage[num]);
        }

        public int GetStageCount() { return _stage.Count; }

        public void Start()
        {
            IsPlaying = true;
            Console.WriteLine("{0}이 나타났다!", _monster.Name);
        }

        public void ProceedTurn()
        {
            Console.Clear();
            _turn++;
            bool isPlayerTurn = _turn % 2 == 1;
            Console.WriteLine("{0}번째 턴, {1} 차례입니다.\n", _turn, isPlayerTurn ? _player.Name : _monster.Name);

            if (isPlayerTurn)
            {
                Console.WriteLine("행동을 결정하세요.");
                Console.WriteLine("1. 공격한다.");
                Console.WriteLine("2. 도망친다.");

                string input = Console.ReadLine();
                int result = 0;

                while (!int.TryParse(input, out result)) {
                    Console.WriteLine("잘못 입력하셨습니다. 다시 입력하세요.");
                    input = Console.ReadLine();
                }

                if (result.Equals(1)) // 공격
                {
                    Console.WriteLine("{0}의 공격!", _player.Name);
                    Console.WriteLine("{0}은 {1}의 데미지를 입었다!", _monster.Name, _player.Attack);
                    _monster.TakeDamage(_player.Attack);                    
                }
                else // 도망
                {
                    End(true);
                    Console.Clear();
                    Console.WriteLine("몬스터에게서 도망쳤습니다.");
                }
            }
            else
            {
                Console.WriteLine("{0}의 공격!", _monster.Name);
                Console.WriteLine("{0}은 {1}의 데미지를 입었다!", _player.Name, _monster.Attack);
                _player.TakeDamage(_monster.Attack);
                
            }

            Thread.Sleep(1000);
        }

        public void CheckStatus()
        {
            if (_player.IsDead) {
                Console.WriteLine("{0}이 쓰러졌습니다.", _player.Name);
            }
            else if (_monster.IsDead) {
                Console.WriteLine("{0}를 쓰러뜨렸습니다.", _monster.Name);
            }

            if (_player.IsDead || _monster.IsDead)
                End();
        }

        void End(bool isRun = false)
        {
            IsPlaying = false;
            IsPlayerWin = _monster.IsDead;
        }

        public void SelectReward()
        {
            Console.Clear();
            Console.WriteLine("스테이지를 클리어 했습니다!");
            Console.WriteLine("보상을 골라주세요.\n1. 체력 포션\n2. 힘 포션");
            string input = Console.ReadLine();
            int result = 0;

            while (!int.TryParse(input, out result))
            {
                Console.WriteLine("잘못 입력하셨습니다. 다시 입력하세요.");
                input = Console.ReadLine();
            }

            Console.Clear();

            IItem item;

            if (result.Equals(1))
            {
                item = new HealthPotion();
                item.Use(_player as Warrior);
            }
            else if (result.Equals(2)) 
            {
                //Console.WriteLine("힘 포션을 사용합니다.");
                //Thread.Sleep(1000);
                item = new StrengthPotion();
                item.Use(_player as Warrior);
            }
        }
    }
}
