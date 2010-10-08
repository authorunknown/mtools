using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            while (true)
            {
                //Console.Out.WriteLine("Testing writing to stdout... " + rnd.Next().ToString());
                //Console.Error.WriteLine("Testing writing to stderr..." + rnd.Next().ToString());
                Thread.Sleep(250);
            }
        }
    }
}
