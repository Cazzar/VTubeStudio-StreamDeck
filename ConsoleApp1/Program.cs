using System;
using System.IO;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllLines("argv.txt", args);
            while (true)
            {
                Thread.Sleep(100000);
            }
        }
    }
}
