using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    class FeatureInventory : FeatureRoot
    {

        public FeatureInventory(string name, IScene parent) : base(name, parent) { }

        public override void Set()
        {
            Set();
        }

        /// <summary>
        /// 표시 옵션 0 : 일반, 1 : 장착 관리
        /// </summary>
        /// <param name="opt"></param>
        void Set(int opt = 0)
        {
            Utility.ShowScript(
                $"인벤토리{(opt == 0 ? "" : "-장착 관리")}\n",
                "보유 중인 아이템을 관리할 수 있습니다.\n\n",

                "[아이템 목록]\n"
            );

            // 목록 표기
            Item[] owned = Parent.Player.OwnedItems;
            Dictionary<int, bool> equipped = Parent.Player.IsEquipped;

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

        public override void Act()
        {
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 1);

            if (act == 0) // 나가기
            {
                Parent.Start();
                return;
            }

            // 장착 관리
            EquipItem();
        }

        void EquipItem()
        {
            Console.Clear();
            Set(1);

            Character player = Parent.Player;
            // 목록 표기
            Item[] owned = player.OwnedItems;
            Dictionary<int, bool> equipped = player.IsEquipped;

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, owned.Length);
            if (act == 0) // 나가기
            {
                Parent.Start();
                return;
            }

            // 장착 관리
            act--;
            player.EquipItem(owned[act]);
            DataSet.GetInstance().Save(player);

            EquipItem();
        }


    }
}
