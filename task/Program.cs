namespace task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.");

            // 데이터 로드 구간
            // 없거나 유효하지 않으면 캐릭터 생성
            Character player = CreateCharacter();
            player.Gold += 1000;

            // 캐릭터 생성 완료
            // 마을로 이동
            IScene town = new Town("스파르타");
            town.ArriveScene(player);
        }

        static Character CreateCharacter()
        {
            Console.WriteLine("원하시는 이름을 설정해주세요.\n");
            string name = Console.ReadLine();

            Console.WriteLine("\n입력하신 이름은 {0} 입니다.\n", name);
            Console.WriteLine("1. 저장\n2. 취소\n");
            int act = Utility.GetNumber("원하시는 행동을 선택해주세요.", 1, 2);
            
            // 2. 다시 반복
            if(act == 2)
                return CreateCharacter();

            Console.WriteLine("원하시는 직업을 선택해주세요.\n");
            Console.WriteLine("1. 전사\n2. 도적\n");
            act = Utility.GetNumber("원하시는 행동을 선택해주세요.", 1, 2);

            // 직업 선택 > 플레이 저장 후 진행
            // 저장 후 마을로 이동

            switch ((DataDefinition.EClass)act)
            {
                case DataDefinition.EClass.Warrior:
                    return new Warrior(name);
                case DataDefinition.EClass.Thief:
                    return new Thief(name);
                default:
                    return new Warrior(name);
            }
        }

    }
}
