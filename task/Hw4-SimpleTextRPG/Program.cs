namespace Hw4_SimpleTextRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("과제 4 - 간단한 텍스트 RPG");
            Console.WriteLine("플레이어 여러분을 환영합니다.");
            Console.Write("원하시는 이름을 입력해주세요 : ");
            string name = Console.ReadLine();

            // 플레이어 캐릭터 생성
            Warrior player = new Warrior(name);
                        
            int lv = 1;
            StartStage(lv, player);
        }

        static void StartStage(int lv, Warrior player)
        {
            Console.Clear();
            Console.WriteLine("스테이지를 시작하겠습니다.");
            Thread.Sleep(1000);

            Stage stage = new Stage(lv, player);
            stage.Start();
            Thread.Sleep(1000);

            do
            {
                stage.ProceedTurn();
                
                Thread.Sleep(1000);
                
                stage.CheckStatus();
            }
            while (stage.IsPlaying);


            if (stage.IsPlayerWin) {
                if (lv + 1 <= stage.GetStageCount())
                {
                    stage.SelectReward();
                    StartStage(lv + 1, player);
                }
                else
                    Console.WriteLine("축하합니다. 게임을 클리어했습니다.");
            }
            else
            {
                Console.WriteLine("게임을 종료합니다.");
            }
        }
    }
}
