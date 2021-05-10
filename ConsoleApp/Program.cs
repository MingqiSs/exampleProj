using System;
using System.Diagnostics;
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


            GetTypeDe();


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

        public static void GetTypeDe()
        {
            var Watch = new Stopwatch();
            object objName = string.Empty;


            var type = GetPerson().GetType();
            Watch.Start();
            for (int i = 0; i < 1000000; i++)
            {
                objName = type.GetProperty("Name").GetValue(GetPerson());
            }
            Watch.Stop();
            Console.WriteLine(Watch.Elapsed);


           dynamic  dynPerson = GetPerson();
            Watch.Restart();
            for (int i = 0; i < 1000000; i++)
            {
                objName = dynPerson.Name;
            }
            Watch.Stop();
            Console.WriteLine(Watch.Elapsed);

        }
        static object GetPerson()
        {
            return new Person { Name = "Leo" };
        }
    }
    public class Person
    {
        public string Name { get; set; }

    }


}
