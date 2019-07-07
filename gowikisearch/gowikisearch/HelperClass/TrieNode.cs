using System.Collections.Generic;

namespace gowikisearch.HelperClass
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> SubNodes { get; set; }
        public bool IsWord { get; set; }

        public TrieNode()
        {
            SubNodes = new Dictionary<char, TrieNode>();
            IsWord = false;
        }
    }
}