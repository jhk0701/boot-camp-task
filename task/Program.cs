using DataDefinition;

namespace task
{
    class Program
    {
        static void Main(string[] args)
        {
            Utility.ShowScript("스파르타 던전에 오신 여러분 환영합니다.");

            // 데이터 로드 구간
            Character player;
            if (DataSet.GetInstance().Load())
            {
                // 데이터 로드
                player = DataSet.GetInstance().LoadCharater();
            }
            else
            {
                // 없거나 유효하지 않으면 캐릭터 생성
                player = CreateCharacter();
                DataSet.GetInstance().Save(player); // 최초 save
            }


            // 마을로 이동
            IScene town = new Town("스파르타");
            town.ArriveScene(player);
        }

        static Character CreateCharacter()
        {
            Utility.ShowScript("원하시는 이름을 설정해주세요.\n");
            string name = Console.ReadLine();

            Console.Clear();
            Utility.ShowScript(
                $"입력하신 이름은 {name} 입니다.\n\n",

                "1. 저장\n2. 취소\n"
            );
            int act = Utility.GetNumber("원하시는 행동을 선택해주세요.", 1, 2);
            
            // 2. 다시 반복
            if(act == 2)
            {
                Console.Clear();
                return CreateCharacter();
            }

            Console.Clear();
            Utility.ShowScript(
                "원하시는 직업을 선택해주세요.\n\n",

                "1. 전사\n2. 도적\n3. 궁수"
            );
            act = Utility.GetNumber("원하시는 행동을 선택해주세요.", 1, 3);

            // 직업 선택 > 플레이 저장 후 진행
            // 저장 후 마을로 이동
            switch ((EClass)act)
            {
                case EClass.Warrior:
                    return new Warrior(name);
                case EClass.Thief:
                    return new Thief(name);
                case EClass.Archer:
                    return new Archer(name);
                default:
                    return new Warrior(name);
            }
        }

    }
}
