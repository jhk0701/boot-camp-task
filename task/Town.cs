using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // id - is sold out
        Dictionary<int, bool> isSoldOut = new Dictionary<int, bool>();

        public Town(string name)
        {
            Name = name;
            // 데이터 읽기
            isSoldOut = DataSet.GetInstance().GetGameData().ItemSellingInfo;
            if (isSoldOut == null)
                isSoldOut = new Dictionary<int, bool>();

            if (isSoldOut.Count != DataSet.GetInstance().Items.Length)
            {
                Item[] items = DataSet.GetInstance().Items;
                for (int i = 0; i < items.Length; i++)
                {
                    if (isSoldOut.ContainsKey(items[i].id))
                        continue;

                    isSoldOut.Add(items[i].id, false);
                }
            }
        }

        void ArriveScene()
        {
            Console.Clear();
            Utility.ShowScript(
                $"{Name} 마을에 오신 여러분 환영합니다.\n",
                "이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다."
            );

            Act();
        }

        public void ArriveScene(Character player)
        {
            Player = player;
            ArriveScene();
        }

        public void LeaveScene()
        {
            DataSet.GetInstance().Save(Player);
            Environment.Exit(0);
        }

        public void Act()
        {
            Utility.ShowScript("\n1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전입장\n5. 휴식하기\n0. 종료\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 5);

            switch (act)
            {
                case 0: // 나가기
                    LeaveScene();
                    break;
                case 1: // 상태 보기
                    ShowStatus();
                    break;
                case 2: // 인벤토리
                    ShowInventory();
                    break;
                case 3: // 상점
                    ShowStore();
                    break;
                case 4: // 던전
                    ShowDungeon();
                    break;
                case 5: // 휴식
                    ShowRest();
                    break;
            }
        }


        #region ### 상태 보기 ###

        void ShowStatus()
        {
            Console.Clear();
            Utility.ShowScript("상태 보기\n캐릭터의 정보가 표시됩니다.\n");

            string className = DataSet.GetInstance().CharacterInitDatas[(int)Player.Class].name;

            Utility.ShowScript(
                $"Lv. {string.Format("{0:0#}", Player.Level)} ({string.Format("{0:N2}", Player.Exp / (float)Player.Level * 100f)}%)\n",
                $"{Player.Name} ( {className} )\n",
                $"공격력 : {Player.BaseAttack} {(Player.EquipAttack > 0 ? $"(+{Player.EquipAttack})" : "")}\n",
                $"방어력 : {Player.BaseDefense} {(Player.EquipDefense > 0 ? $"(+{Player.EquipDefense})" : "")}\n",
                $"체력 : {Player.Health} / {Player.MaxHealth}\n",
                $"Gold : {Player.Gold} G\n"
            );


            Console.WriteLine("0. 나가기\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 0);

            ArriveScene();
        }

        #endregion


        #region ### 인벤토리 ###

        void ShowInventory()
        {
            Console.Clear();
            SetInventoryInfo();

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 1);

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

            // 목록 표기
            Item[] owned = Player.OwnedItems;
            Dictionary<int, bool> equipped = Player.IsEquipped;

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, owned.Length);
            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            // 장착 관리
            act--;
            Player.EquipItem(owned[act]);
            DataSet.GetInstance().Save(Player);

            EquipItem();
        }

        /// <summary>
        /// 인벤토리 정보 표시. 표시 옵션 0 : 일반, 1 : 장착 관리
        /// </summary>
        void SetInventoryInfo(int opt = 0)
        {
            Utility.ShowScript(
                $"인벤토리{(opt == 0 ? "" : "-장착 관리")}\n",
                "보유 중인 아이템을 관리할 수 있습니다.\n\n",
                
                "[아이템 목록]\n"
            );

            // 목록 표기
            Item[] owned = Player.OwnedItems;
            Dictionary<int, bool> equipped = Player.IsEquipped;

            for (int i = 0; i < owned.Length; i++)
            {
                bool isEquipped = equipped.ContainsKey(owned[i].id) && equipped[owned[i].id];
                Utility.ShowScript(
                    $"- {(opt == 0 ? "" : i + 1)} {(isEquipped ? "[E]" : "")}{owned[i].GetDesc()}",
                    $"{((i + 1) % 5 == 0 ? "\n" : "")}"
                );
            }

            Utility.ShowScript(
                "\n",
                opt == 0 ?
                "1. 장착 관리\n0. 나가기\n" :
                "0. 나가기\n"
            );
        }

        #endregion


        #region ### 상점 ###

        /// <summary>
        /// 상점 선택
        /// </summary>
        void ShowStore()
        {
            Item[] items = DataSet.GetInstance().Items;

            Console.Clear();
            SetStoreInfo(ref items);

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 2);

            switch (act)
            {
                case 0: // 나가기
                    ArriveScene();
                    break;
                case 1: // 구매
                    Console.Clear();
                    SetStoreInfo(ref items, act);

                    BuyItem(ref items);
                    break;
                case 2: // 판매
                    items = Player.OwnedItems;

                    Console.Clear();
                    SetStoreInfo(ref items, act);
                    SellItem();
                    break;
            }
        }

        /// <summary>
        /// 아이템 구매
        /// </summary>
        void BuyItem(ref Item[] items)
        {
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, items.Length);

            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            act--;
            if (isSoldOut[items[act].id])
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
                BuyItem(ref items);
                return;
            }

            if (items[act].price > Player.Gold) // 골드 부족
                Utility.ShowScript("Gold가 부족합니다.");
            else // 구매 성사
            {
                Player.Gold -= items[act].price;
                isSoldOut[items[act].id] = true;

                // 인벤토리에 추가
                Player.AddItem(items[act]);

                // 구매 후 정보 업데이트
                Console.Clear();
                SetStoreInfo(ref items, 1);

                Utility.ShowScript("구매를 완료했습니다.");
                DataSet.GetInstance().Save(Player, isSoldOut);
            }

            // 구매 계속 진행
            BuyItem(ref items);
        }

        /// <summary>
        /// 아이템 판매
        /// </summary>
        void SellItem()
        {
            Item[] owned = Player.OwnedItems;

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, owned.Length);

            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            act--;
            // 판매 성사
            isSoldOut[owned[act].id] = false;
            Player.Gold += (int)(owned[act].price * 0.85f);

            // 인벤토리 제거
            // 장착 가능성 확인 및 처리
            Player.RemoveItem(owned[act]);

            // 판매 후 정보 업데이트
            owned = Player.OwnedItems;

            Console.Clear();
            SetStoreInfo(ref owned, 2);

            Utility.ShowScript("판매가 완료했습니다.");
            DataSet.GetInstance().Save(Player, isSoldOut);

            SellItem();
        }

        /// <summary>
        /// 상점 정보 표시, 표시 옵션 0 : 일반, 1 : 구매, 2 : 판매
        /// </summary>
        /// <param name="opt"></param>
        void SetStoreInfo(ref Item[] items, int opt = 0)
        {
            Utility.ShowScript(
                $"상점{(opt > 0 ? (opt == 1 ? "-아이템 구매" : "-아이템 판매") : "")}\n",
                "필요한 아이템을 얻을 수 있는 상점입니다.\n\n",

                $"[보유 골드]\n{Player.Gold} G\n\n",

                "[아이템 목록]"
            );

            StringBuilder sb = new StringBuilder();
            // 목록 표기
            for (int i = 0; i < items.Length; i++)
            {
                Utility.AppendString(ref sb,
                    $"- {(opt == 0 ? "" : i + 1)} {items[i].GetDesc()}",

                    opt == 2 ? 
                    $"| {items[i].price * 0.85,-10} G\n" :
                    $"| {(isSoldOut[items[i].id] ? "구매완료" : items[i].price + " G"),-10}\n",
                    $"{((i + 1) % 5 == 0 ? "\n" : "")}"
                );
            }
            Console.WriteLine(sb.ToString());

            Utility.ShowScript(
                "\n",
                opt == 0 ?
                "1. 아이템 구매\n2. 아이템 판매\n0. 나가기\n" :
                "0. 나가기\n"
            );
        }


        #endregion


        #region ### 던전 ###

        void ShowDungeon()
        {
            Dungeon[] dungeons = DataSet.GetInstance().Dungeons;

            Console.Clear();
            Utility.ShowScript(
                "던전입장\n",
                "이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n",
                $"플레이어의 체력이 20이하라면 입장할 수 없습니다. (현재 체력 : {Player.Health})\n"
            );

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dungeons.Length; i++)
                sb.Append($"{i + 1}. {dungeons[i].name,-15}\t| 방어력 {dungeons[i].recommendedDefense} 이상 권장\n");

            sb.Append("0. 나가기\n");
            Console.WriteLine(sb.ToString());

            SelectDungeon(ref dungeons);
        }

        void SelectDungeon(ref Dungeon[] dungeons)
        {
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, dungeons.Length);

            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            if(Player.Health <= 20)
            {
                Utility.ShowScript(
                    "\"용사님, 부상이 너무 심하셔요.\"\n",
                    "던전에 들어가려는 순간 경비병이 막아섰다.\n"
                );

                SelectDungeon(ref dungeons);
                return;
            }

            EnterDungeon(dungeons[act - 1]);
        }

        void EnterDungeon(Dungeon dungeon)
        {
            Random r = new Random();
            bool isClear = true;

            // 던전 수행 여부 : 방어력
            if (dungeon.recommendedDefense > Player.Defense)
            {
                // 권장 방어력 미만 40% 확률 실패
                int p = r.Next(1, 101);
                isClear = p > 40;
            }

            Console.Clear();

            if (isClear)
            {
                // 권장 방어력 이상 던전 클리어

                // 권장 방어력에 따라 종료 시 체력 소모
                // 기본 체력 감소 : 20 ~ 35 중 랜덤
                // 추가 감소량 : 내 방어력 - 권장 방어력
                // 체력 소모 -= 기본 체력 감소 - 추가 감소량
                float damage = r.Next(20, 36) - (Player.Defense - dungeon.recommendedDefense);

                // 보상
                // 기본 골드 보상 + 공격력의 10 ~ 20% 만큼의 추가 보상
                float additivePer = r.Next((int)Player.Attack, (int)Player.Attack * 2 + 1) * 0.01f;
                int reward = (int)(dungeon.rewardGold * (1 + additivePer));

                Utility.ShowScript(
                    $"던전 클리어\n축하합니다!!\n{dungeon.name}을 클리어 하였습니다.\n\n",

                    "[탐험 결과]\n",
                    $"체력 {Player.Health} -> {(Player.Health - damage > 0 ? Player.Health - damage : 0)}\n",
                    $"Gold {Player.Gold} G -> {Player.Gold + reward} G\n"
                );

                Player.GetDamage((int)damage);
                Player.Gold += reward;

                // 클리어 시, 경험치 쌓기
                Player.Exp++;
            }
            else
            {
                // 실패 페널티 : 체력 절반 감소 (현재 체력 기준)
                int damage = (int)(Player.Health * 0.5f);

                Utility.ShowScript(
                    $"던전 실패\n{dungeon.name}을 실패했습니다.\n\n",

                    "[실패 페널티]\n",
                    $"체력 {Player.Health} -> {Player.Health - damage}\n"
                );

                Player.GetDamage(damage);
            }
            

            if (Player.Health == 0)
            {
                /// 데이터를 삭제할까
                Utility.ShowScript(
                    $"{Player.Name}이/가 사망에 이르는 부상을 입었습니다!\n",
                    "...\n",
                    "...\n",
                    "...\n",
                    "\"일어나세요, 용사여...\"\n"
                );

                Player.GetDamage(-1);
            }

            Utility.ShowScript("0. 나가기\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 0);

            DataSet.GetInstance().Save(Player);
            ArriveScene();
        }

        #endregion


        #region ### 휴식 ###

        int _restCost = 500;

        void ShowRest()
        {
            Console.Clear();
            SetRest();

            Rest();
        }

        void SetRest()
        {
            Utility.ShowScript(
                "휴식하기\n",
                $"{_restCost} G를 내면 체력을 회복할 수 있습니다. (보유 골드 : {Player.Gold} G)\n\n",

                "1. 휴식하기\n0. 나가기\n"
            );
        }

        void Rest()
        {
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 1);

            if (act == 0) // 나가기
            {
                ArriveScene();
                return;
            }

            if (Player.Gold >= _restCost) 
            {
                Player.Cure();
                Player.Gold -= _restCost;

                Console.Clear();
                SetRest();

                Utility.ShowScript("휴식을 완료했습니다.");
                DataSet.GetInstance().Save(Player);

                Rest();
            }
            else
            {
                Utility.ShowScript("Gold 가 부족합니다.");
                Rest();
            }
        }

        #endregion
    }
}
