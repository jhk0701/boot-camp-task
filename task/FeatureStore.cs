using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    /// <summary>
    /// 상점 기능
    /// </summary>
    class FeatureStore : FeatureRoot
    {
        // 상점 내 item 관련
        // id - is sold out
        Dictionary<int, bool> _isSoldOut = new Dictionary<int, bool>();

        public FeatureStore(string name, IScene parent) : base(name, parent) 
        {
            // 상점 관련
            // 데이터 읽기
            _isSoldOut = DataSet.GetInstance().GetGameData().ItemSellingInfo;
            if (_isSoldOut == null)
                _isSoldOut = new Dictionary<int, bool>();

            if (_isSoldOut.Count != DataSet.GetInstance().Items.Length)
            {
                Item[] items = DataSet.GetInstance().Items;
                for (int i = 0; i < items.Length; i++)
                {
                    if (_isSoldOut.ContainsKey(items[i].id))
                        continue;

                    _isSoldOut.Add(items[i].id, false);
                }
            }
        }

        public override void Set()
        {
            Item[] items = DataSet.GetInstance().Items;
            Set(ref items);
        }

        void Set(ref Item[] items, int opt = 0)
        {
            Utility.ShowScript(
                $"상점{(opt > 0 ? (opt == 1 ? "-아이템 구매" : "-아이템 판매") : "")}\n",
                "필요한 아이템을 얻을 수 있는 상점입니다.\n\n",

                $"[보유 골드]\n{Parent.Player.Gold} G\n\n",

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
                    $"| {(_isSoldOut[items[i].id] ? "구매완료" : items[i].price + " G"),-10}\n",
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

        public override void Act()
        {
            Item[] items = DataSet.GetInstance().Items;
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 2);

            switch (act)
            {
                case 0: // 나가기
                    Parent.Start();
                    break;
                case 1: // 구매
                    Console.Clear();
                    Set(ref items, act);

                    BuyItem(ref items);
                    break;
                case 2: // 판매
                    items = Parent.Player.OwnedItems;

                    Console.Clear();
                    Set(ref items, act);

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
                Parent.Start();
                return;
            }

            act--;

            if (_isSoldOut[items[act].id])
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
                BuyItem(ref items);
                return;
            }

            Character player = Parent.Player;

            if (items[act].price > player.Gold) // 골드 부족
                Utility.ShowScript("Gold가 부족합니다.");
            else // 구매 성사
            {
                player.Gold -= items[act].price;
                _isSoldOut[items[act].id] = true;

                // 인벤토리에 추가
                player.AddItem(items[act]);

                // 구매 후 정보 업데이트
                Console.Clear();
                Set(ref items, 1);

                Utility.ShowScript("구매를 완료했습니다.");
                DataSet.GetInstance().Save(player, _isSoldOut);
            }

            // 구매 계속 진행
            BuyItem(ref items);
        }

        /// <summary>
        /// 아이템 판매
        /// </summary>
        void SellItem()
        {
            Character player = Parent.Player;
            Item[] owned = player.OwnedItems;

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, owned.Length);

            if (act == 0) // 나가기
            {
                Parent.Start();
                return;
            }

            act--;
            // 판매 성사
            _isSoldOut[owned[act].id] = false;
            player.Gold += (int)(owned[act].price * 0.85f);

            // 인벤토리 제거
            // 장착 가능성 확인 및 처리
            player.RemoveItem(owned[act]);

            // 판매 후 정보 업데이트
            owned = player.OwnedItems;

            Console.Clear();
            Set(ref owned, 2);

            Utility.ShowScript("판매가 완료했습니다.");
            DataSet.GetInstance().Save(player, _isSoldOut);

            SellItem();
        }

    }
}
