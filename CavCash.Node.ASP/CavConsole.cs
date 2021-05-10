using System;

namespace CavCash.Node.ASP
{
    public class CavConsole
    {
        public static void WriteLine(string s, ConsoleColor foreground, ConsoleColor background)
        {
            ConsoleColor currentFore = Console.ForegroundColor;
            ConsoleColor currentBack = Console.BackgroundColor;

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            
            Console.WriteLine(s);
            
            Console.ForegroundColor = currentFore;
            Console.BackgroundColor = currentBack;
        }
        
        public static void Write(string s, ConsoleColor foreground, ConsoleColor background)
        {
            ConsoleColor currentFore = Console.ForegroundColor;
            ConsoleColor currentBack = Console.BackgroundColor;

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            
            Console.Write(s);
            
            Console.ForegroundColor = currentFore;
            Console.BackgroundColor = currentBack;
        }
        
        /*public static void WriteLine(string s, ConsoleColor background)
        {
            ConsoleColor currentFore = Console.ForegroundColor;
            ConsoleColor currentBack = Console.BackgroundColor;

            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            
            Console.WriteLine(s);
            
            Console.ForegroundColor = currentFore;
            Console.BackgroundColor = currentBack;
        }*/
        
        public static void WriteLine(string s, ConsoleColor foreground)
        {
            ConsoleColor currentFore = Console.ForegroundColor;

            Console.ForegroundColor = foreground;
            
            Console.WriteLine(s);
            
            Console.ForegroundColor = currentFore;
        }
    }
}