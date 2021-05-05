using System;
using static System.Console;

namespace Freetime_Planner
{
    class MainProgram
    {
        public static bool Works = true;
        static void Main(string[] args)
        {
            if(Bot.Authorize())
                while (true) { }
        }
    }
}
