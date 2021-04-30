using System;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        private
        static void Main(string[] args)
        {

            //char[] s = new char[] { 'h', 'e', 'l', 'l', '0' };
            //ReverseString(s);
            //Console.WriteLine(string.Join(",", s));




            Console.ReadLine();
        }

        /// <summary>
        /// 反转字符串 等同于Array.Reverse
        /// </summary>
        ///todo: 交换开头下标和结尾下标的字符，
        //递归处理中间剩下的字符。 
        /// <param name="s"></param>
        public static void ReverseString(char[] s, int i = 0)
        {
            // 递归出口
            if (i >= s.Length / 2) return;
            char temp;
            temp = s[i];
            s[i] = s[s.Length - 1 - i];
            s[s.Length - 1 - i] = temp;
            ReverseString(s, i + 1);
        }
    }
}
