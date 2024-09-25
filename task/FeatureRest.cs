using DataDefinition;

namespace task
{
    class FeatureRest : FeatureRoot
    {
        int _restCost = 500;

        public FeatureRest(string name, IScene parent) : base(name, parent) { }

        
        public override void Set()
        {
            Utility.ShowScript(
                "휴식하기\n",
                $"{_restCost} G를 내면 체력을 회복할 수 있습니다. (보유 골드 : {Parent.Player.Gold} G)\n\n",

                "1. 휴식하기\n0. 나가기\n"
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

            if (Parent.Player.Gold >= _restCost)
            {
                Parent.Player.Cure();
                Parent.Player.Gold -= _restCost;

                Console.Clear();
                Set();

                Utility.ShowScript("휴식을 완료했습니다.");
                DataSet.GetInstance().Save(Parent.Player);
                Act();
            }
            else
            {
                Utility.ShowScript("Gold 가 부족합니다.");
                Act();
            }
        }
    }
}
