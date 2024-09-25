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
        public IFeature[] Features { get; set; }


        public Town(string name)
        {
            Name = name;

            Features = new IFeature[]{
                new FeatureStatus("상태 보기", this),
                new FeatureInventory("인벤토리", this),
                new FeatureStore("상점", this),
                new FeatureDungeon("던전입장", this),
                new FeatureRest("휴식하기", this),
            };
        }

        public void Start()
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
            Start();
        }

        public void LeaveScene()
        {
            DataSet.GetInstance().Save(Player);
            Environment.Exit(0);
        }

        public void Act()
        {
            Utility.ShowScript("\n1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전입장\n5. 휴식하기\n0. 종료\n");
            int act = Utility.GetNumber("원하시는 행동을 입력해주세요.", 0, Features.Length);

            if(act == 0) // 나가기
            {
                LeaveScene();
                return;
            }

            Features[act - 1].Start();
        }
    }
}
