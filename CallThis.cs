using System;
using System.Collections.Generic;
using System.Text;

namespace FindUnuseableFile
{
    public class CallThis
    {
        private string str = "baseString";
        public CallThis(string param):this()
        {
            Console.WriteLine("calParams");
            str = param;
        }
        public CallThis()
        {
            Console.WriteLine("calDefault");
        }
        public void Print()
        {
            Console.WriteLine(str);
        }
    }
}
