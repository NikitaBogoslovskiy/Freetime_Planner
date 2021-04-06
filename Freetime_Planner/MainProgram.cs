using System;
using static System.Console;

namespace Freetime_Planner
{
    class MainProgram
    {
        public static bool Works = true;
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Bot.Authorize();
                    while (true) { }
                }
                catch (Exception) { }
            }
        }
    }
}
