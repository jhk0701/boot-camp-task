using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DataDefinition;

namespace task
{
    class FeatureDungeon : FeatureRoot
    {
        public FeatureDungeon(string name, IScene parent) : base(name, parent) { }


        public override void Set()
        {
            Dungeon[] dungeons = DataSet.GetInstance().Dungeons;

            Utility.ShowScript(
                "던전입장\n",
                "이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n",
                $"플레이어의 체력이 20이하라면 입장할 수 없습니다. (현재 체력 : {Parent.Player.Health})\n"
            );

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dungeons.Length; i++)
                sb.Append($"{i + 1}. {dungeons[i].name,-15}\t| 방어력 {dungeons[i].recommendedDefense} 이상 권장\n");

            sb.Append("0. 나가기\n");
            Console.WriteLine(sb.ToString());
        }

        public override void Act()
        {
            Dungeon[] dungeons = DataSet.GetInstance().Dungeons;

            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, dungeons.Length);

            if (act == 0) // 나가기
            {
                Parent.Start();
                return;
            }

            if (Parent.Player.Health <= 20)
            {
                Utility.ShowScript(
                    "\"용사님, 부상이 너무 심하셔요.\"\n",
                    "던전에 들어가려는 순간 경비병이 막아섰다.\n"
                );

                Act();
                return;
            }

            EnterDungeon(dungeons[act - 1]);
        }


        void EnterDungeon(Dungeon dungeon)
        {
            Random r = new Random();
            bool isClear = true;

            // 던전 수행 여부 : 방어력
            if (dungeon.recommendedDefense > Parent.Player.Defense)
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
                float damage = r.Next(20, 36) - (Parent.Player.Defense - dungeon.recommendedDefense);

                // 보상
                // 기본 골드 보상 + 공격력의 10 ~ 20% 만큼의 추가 보상
                float additivePer = r.Next((int)Parent.Player.Attack, (int)Parent.Player.Attack * 2 + 1) * 0.01f;
                int reward = (int)(dungeon.rewardGold * (1 + additivePer));

                Utility.ShowScript(
                    $"던전 클리어\n축하합니다!!\n{dungeon.name}을 클리어 하였습니다.\n\n",
                    "[탐험 결과]\n",
                    $"체력 {Parent.Player.Health} -> {(Parent.Player.Health - damage > 0 ? Parent.Player.Health - damage : 0)}\n",
                    $"Gold {Parent.Player.Gold} G -> {Parent.Player.Gold + reward} G\n"
                );

                Parent.Player.GetDamage((int)damage);
                Parent.Player.Gold += reward;

                // 클리어 시, 경험치 쌓기
                Parent.Player.Exp++;
            }
            else
            {
                // 실패 페널티 : 체력 절반 감소 (현재 체력 기준)
                int damage = (int)(Parent.Player.Health * 0.5f);

                Utility.ShowScript(
                    $"던전 실패\n{dungeon.name}을 실패했습니다.\n\n",

                    "[실패 페널티]\n",
                    $"체력 {Parent.Player.Health} -> {Parent.Player.Health - damage}\n"
                );

                Parent.Player.GetDamage(damage);
            }


            if (Parent.Player.Health == 0)
            {
                /// 데이터를 삭제할까
                Utility.ShowScript(
                    $"{Parent.Player.Name}이/가 사망에 이르는 부상을 입었습니다!\n",
                    "...\n",
                    "...\n",
                    "...\n",
                    "\"일어나세요, 용사여...\"\n"
                );

                Parent.Player.GetDamage(-1);
            }

            Utility.ShowScript("0. 나가기\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, 0);

            DataSet.GetInstance().Save(Parent.Player);
            Parent.Start();
        }
    }
}
