using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace task
{
    class Utility
    {
        /// <summary>
        /// 원하는 숫자 입력받기
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="minNum"></param>
        /// <param name="maxNum"></param>
        /// <returns></returns>
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

        /// <summary>
        /// string builder를 이용한 문자열 만들기
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static string GetString(params string[] args)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < args.Length; i++)
                sb.Append(args[i]);

            return sb.ToString();
        }

        /// <summary>
        /// 여러 문자열 화면에 띄우기
        /// </summary>
        /// <param name="args"></param>
        public static void ShowScript(params string[] args)
        {
            Console.WriteLine(GetString(args));
        }

        public static void AppendString(ref StringBuilder sb, params string[] args) 
        {
            for (int i = 0; i < args.Length; i++)
                sb.Append(args[i]);
        }
    }
}
