using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Program
    {
        //private string item;

        public delegate string delege(string item, string item1);

        public static string xiao1(string item, string item1)
        {
            item += item1 + "1";
            return item;
        }

        public static string xiao2(string item2, string item1)
        {
            item2 += item1 + "2";
            return item2;
        }

        public static string VOiD(delege dele, string item, string item1)
        {
            return dele(item, item1);
        }

        static void Main(string[] args)
        {
           // Console.WriteLine(VOiD(xiao2, "aaa", "bbb"));
           

        }
    }
}
