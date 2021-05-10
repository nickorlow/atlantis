using System.Collections.Generic;

namespace CavCash.Core
{
    public class ChainRequest
    {
        public int BlockNumber { get; set; }
        public string BlockHash { get; set; }
    }

    public class ChainResponse
    {
        public List<Block> Blockchain { get; set; }
    }
}