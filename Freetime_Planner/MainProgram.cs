using System;
using static System.Console;

namespace Freetime_Planner
{
    class MainProgram
    {
        public static bool Works = true;
        static void Main(string[] args)
        {
            if (Bot.Authorize())
            {
                while (Works)
                {
                    switch (ReadLine().ToLower())
                    {
                        case "exit":
                            Bot.ExitActions();
                            Environment.Exit(0); break;
                        default: break;
                    }
                }
            }
            WriteLine("Нажмите ENTER чтобы выйти...");
            ReadLine();

        }
    }
}
