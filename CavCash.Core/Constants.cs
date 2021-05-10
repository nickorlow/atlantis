using System.Collections.Generic;
using NBitcoin;

namespace CavCash.Core
{
    public static class Constants
    {
        // We need to deprecate this and just remove it fro everywhere. I hate relying on this stupid library
        public static Network BitCoinNetwork = Network.RegTest;

        public static List<Difficulty> Difficulties = new List<Difficulty>()
        {
            new ()
            {
                BlockNumber = 1,
                HashPrefix = "abc"
            },
            new ()
            {
                BlockNumber = 5,
                HashPrefix = "abcd"
            },
            new ()
            {
                BlockNumber = 10,
                HashPrefix = "abcde"
            },
            new ()
            {
                BlockNumber = 150,
                HashPrefix = "abcdef"
            },
            new ()
            {
                BlockNumber = 200,
                HashPrefix = "abcdefa"
            },new ()
            {
                BlockNumber = 500,
                HashPrefix = "abcdefab"
            },

        };
    }

    public class Difficulty
    {
        public int BlockNumber { get; set; }
        public string HashPrefix { get; set; }
    }
}