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
    /// 상태보기 기능
    /// </summary>
    class FeatureStatus : FeatureRoot
    {
        public FeatureStatus(string name, IScene parent) : base(name, parent) { }

        public override void Set()
        {
            Utility.ShowScript("상태 보기\n캐릭터의 정보가 표시됩니다.\n");

            Character player = Parent.Player;
            string className = DataSet.GetInstance().CharacterInitDatas[(int)Parent.Player.Class].name;
            Utility.ShowScript(
                $"Lv. {string.Format("{0:0#}", player.Level)} ({string.Format("{0:N2}", player.Exp / (float)player.Level * 100f)}%)\n",
                $"{player.Name} ( {className} )\n",
                $"공격력 : {player.BaseAttack} {(player.EquipAttack > 0 ? $"(+{player.EquipAttack})" : "")}\n",
                $"방어력 : {player.BaseDefense} {(player.EquipDefense > 0 ? $"(+{player.EquipDefense})" : "")}\n",
                $"체력 : {player.Health} / {player.MaxHealth}\n",
                $"Gold : {player.Gold} G\n\n",

                "0. 나가기\n"
            );
        }

        public override void Act()
        {
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 0);
            Parent.Start();
        }

    }
}
