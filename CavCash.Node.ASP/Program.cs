using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CavCash.Node.ASP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
                        string str = "     ,gggg,         " +
                         "                    " +
                         "     ,gggg,         " +
                         "                    " +
                         "      \r\n" +
                         "   ,88\"\"\"Y8b,    " +
                         "                    " +
                         "      ,88\"\"\"Y8b, " +
                         "                    " +
                         "  ,dPYb,    \r\n" +
                         "  d8\"     `Y8      " +
                         "                    " +
                         "   d8\"     `Y8     " +
                         "                  IP" +
                         "\'`Yb    \r\n" +
                         " d8\'   8b  d8      " +
                         "                    " +
                         "  d8\'   8b  d8     " +
                         "                  I8" +
                         "  8I    \r\n" +
                         ",8I    \"Y88P\'     " +
                         "                    " +
                         "  ,8I    \"Y88P\'   " +
                         "                    " +
                         "I8  8\'    \r\n" +
                         "I8\'             ,gg" +
                         "gg,gg     ggg    gg " +
                         " I8\'             ,g" +
                         "ggg,gg    ,g,     I8" +
                         " dPgg,  \r\n" +
                         "d8             dP\" " +
                         " \"Y8I    d8\"Yb   8" +
                         "8bgd8             dP" +
                         "\"  \"Y8I   ,8\'8,  " +
                         "  I8dP\" \"8I \r\n" +
                         "Y8,           i8\'  " +
                         "  ,8I   dP  I8   8I " +
                         " Y8,           i8\' " +
                         "   ,8I  ,8\'  Yb   I" +
                         "8P    I8 \r\n" +
                         "`Yba,,_____, ,d8,   " +
                         ",d8b,,dP   I8, ,8I  " +
                         "`Yba,,_____, ,d8,   " +
                         ",d8b,,8\'_   8) ,d8 " +
                         "    I8,\r\n" +
                         "  `\"Y8888888 P\"Y88" +
                         "88P\"`Y88\"     \"Y8" +
                         "P\"     `\"Y8888888 " +
                         "P\"Y8888P\"`Y8P\' \"" +
                         "YY8P8P88P     `Y8";
            string[] cavcashLogo = str.Split('\n');

            ConsoleColor[] rainbow = new[]
            {
                ConsoleColor.Red,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkYellow,
                ConsoleColor.Yellow,
                ConsoleColor.Green,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkBlue,
                ConsoleColor.Blue,
                ConsoleColor.DarkMagenta,
                ConsoleColor.Magenta
            };
            
            for(int i = 0; i<cavcashLogo.Length; i++)
                CavConsole.WriteLine(cavcashLogo[i], rainbow[i]);

            Console.WriteLine("Node Version 1.0.0");
            Console.WriteLine("------------------");
            await CurrentNode.StartNode();
            
            CreateHostBuilder(args).Build().RunAsync();
            
            while (true)
            {
                if (true)
                {
                    Console.WriteLine("Press any key to mine the current block");
                    Console.Read();
                }

                await CurrentNode.MineCurrentBlock();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}