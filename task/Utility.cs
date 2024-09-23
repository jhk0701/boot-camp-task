using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    class Utility
    {
        public static int GetNumber(string msg, int minNum = 1, int maxNum = 1)
        {
            Console.WriteLine(msg);
            string input = Console.ReadLine();
            int result = 0;

            while (!int.TryParse(input, out result) ||
                result < minNum ||
                result > maxNum)
            {
                Console.WriteLine("잘못된 입력입니다.");
                Console.WriteLine(msg);
                input = Console.ReadLine();
            }

            Console.WriteLine();
            return result;
        }
    }
}
