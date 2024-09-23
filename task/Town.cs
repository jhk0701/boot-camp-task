using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    class Town : IScene
    {
        public string Name { get; set; }
        public Character Player { get; set; }

        // 상점 내 item 관련
        bool[] isSoldOut;

        public Town(string name)
        {
            Name = name;
            // 데이터 읽기
            // 없으면 false 초기화
            isSoldOut = new bool[DataSet.GetInstance().Items.Length];
        }

        void ArriveScene()
        {
            Console.Clear();
            Console.WriteLine("{0} 마을에 오신 여러분 환영합니다.", Name);
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");

            Act();
        }
        public void ArriveScene(Character player)
        {
            Player = player;
            ArriveScene();
        }

        public void LeaveScene()
        {

        }

        public void Act()
        {
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기\n2. 인벤토리\n3. 상점\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 1, 3);

            switch (act)
            {
                case 1:
                    ShowStatus();
                    break;
                case 2:
                    ShowInventory();
                    break;
                case 3:
                    ShowStore();
                    break;
            }
        }


        void ShowStatus()
        {
            Console.Clear();
            StringBuilder sb = new StringBuilder();

            sb.Append("상태 보기\n캐릭터의 정보가 표시됩니다.\n");
            Console.WriteLine(sb.ToString());

            string className = "";
            switch (Player.Class)
            {
                case EClass.Warrior:
                    className = "전사";
                    break;
                case EClass.Thief:
                    className = "도적";
                    break;
                case EClass.Archer:
                    className = "궁수";
                    break;
                default:
                    className = "무직";
                    break;
            }
            
            sb.Clear();
            sb.Append($"Lv. {string.Format("{0:0#}", Player.Level)}\n");
            sb.Append($"{Player.Name} ( {className} )\n");
            sb.Append($"공격력 : {Player.Attack}\n");
            sb.Append($"방어력 : {Player.Defense}\n");
            sb.Append($"체력 : {Player.Health} / {Player.MaxHealth}\n");
            sb.Append($"Gold : {Player.Gold} G\n");

            Console.WriteLine(sb.ToString());

            Console.WriteLine("0. 나가기\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 0);

            ArriveScene();
        }

        
        void ShowInventory()
        {
            Console.Clear();
            SetInventoryInfo();

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 1, 2);

            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            // 장착 관리
            EquipItem();
        }

        void EquipItem()
        {
            Console.Clear();
            SetInventoryInfo(1);

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 0);

            // 장착 관리
            // ArriveScene();
        }

        /// <summary>
        /// 인벤토리 정보 표시. 표시 옵션 0 : 일반, 1 : 장착 관리
        /// </summary>
        void SetInventoryInfo(int opt = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"인벤토리{(opt == 0 ? "" : "-장착 관리")}");
            sb.Append("\n보유 중인 아이템을 관리할 수 있습니다.\n\n");
            sb.Append("[아이템 목록]\n");
            // 목록 표기
            Item[] ownned; 
            Dictionary<int, bool> equipped;
            Player.GetItem(out ownned, out equipped);

            for (int i = 0; i < ownned.Length; i++) 
            {
                bool isEquipped = equipped.ContainsKey(ownned[i].id) && equipped[ownned[i].id];
                if(opt == 0)
                    sb.Append($"- {(isEquipped ? "[E]" : "")}{ownned[i].GetDesc()}\n");
                else
                    sb.Append($"- {i + 1} {(isEquipped ? "[E]" : "")}{ownned[i].GetDesc()}\n");
            }

            if(opt == 0)
                sb.Append("\n1. 장착 관리\n0. 나가기\n");
            else
                sb.Append("\n0. 나가기\n");

            Console.WriteLine(sb.ToString());
        }


        /// <summary>
        /// 상점 선택
        /// </summary>
        void ShowStore()
        {
            Console.Clear();
            SetStoreInfo();

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 1);
            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            // 구매 선택
            Console.Clear();
            SetStoreInfo(1);

            BuyItem();
        }
        
        /// <summary>
        /// 아이템 구매
        /// </summary>
        void BuyItem()
        {
            Item[] items = DataSet.GetInstance().Items;
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, items.Length);

            if(act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            act--;
            if (isSoldOut[act])
                Console.WriteLine("이미 구매한 아이템입니다.");

            if (items[act].price > Player.Gold) // 골드 부족
                Console.WriteLine("Gold가 부족합니다.");
            else // 구매 성사
            {
                Player.Gold -= items[act].price;
                isSoldOut[act] = true;

                // 인벤토리에 추가
                Player.AddItem(items[act]);

                // 구매 후 정보 업데이트
                Console.Clear();
                SetStoreInfo(1);

                Console.WriteLine("구매를 완료했습니다.");
            }

            // 구매 계속 진행
            BuyItem();
        }

        /// <summary>
        /// 상점 정보 표시, 표시 옵션 0 : 일반, 1 : 구매, 2 : 판매
        /// </summary>
        /// <param name="opt"></param>
        void SetStoreInfo(int opt = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"상점{(opt > 0 ? (opt == 1 ? "-아이템 구매" : "아이템 판매") : "")}");
            sb.Append("\n필요한 아이템을 얻을 수 있는 상점입니다.\n\n");
            sb.Append($"[보유 골드]\n{Player.Gold} G\n\n");
            sb.Append("[아이템 목록\n");
            // 목록 표기
            Item[] items = DataSet.GetInstance().Items;
            for (int i = 0; i < items.Length; i++)
                if (opt == 1)
                    sb.Append($"- {i + 1} {items[i].GetDesc(isSoldOut[i])}\n");
                else
                    sb.Append($"- {items[i].GetDesc(isSoldOut[i])}\n");

            if (opt == 1)
                sb.Append("\n0. 나가기\n");
            else
                sb.Append("\n1. 아이템 구매\n0. 나가기\n");

            Console.WriteLine(sb.ToString());
        }
    }
}
