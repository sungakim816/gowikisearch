using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace gowikisearch.HelperClass
{
    public class TrieDataStructure
    {
        private TrieNode root;
        private List<string> wordList;
        public TrieDataStructure()
        {
            Initialize();
        }

        protected void Initialize()
        {
            root = new TrieNode();
            wordList = new List<string>();
        }

        public void Add(string key)
        {
            if(Contains(key))
            {
                return;
            }
            TrieNode node = root;
            foreach (char k in key.ToLower())
            {
                if (!node.SubNodes.ContainsKey(k))
                {
                    node.SubNodes.Add(k, new TrieNode());
                }
                node = node.SubNodes[k];
            }
            node.IsWord = true;
        }

        public List<string> Suggestions(string word, short limit = 10)
        {
            if (string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word))
            {
                return new List<string>();
            }
            wordList.Clear();
            TrieNode node = this.root;
            bool notFound = false;
            string tempWord = "";
            foreach (char letter in word.ToLower())
            {
                if (!node.SubNodes.ContainsKey(letter))
                {
                    notFound = true;
                    break;
                }
                tempWord += letter;
                node = node.SubNodes[letter];
            }

            if (notFound)
            {
                return new List<string>();
            }
            SuggestionRecursion(node, tempWord, limit);
            return wordList;
        }

        protected void SuggestionRecursion(TrieNode node, string word, int count)
        {
            if (node == null || string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word) || count <= 0)
            {
                return;
            }
            if (node.IsWord)
            {
                count--;
                wordList.Add(word);
            }
            foreach (var key in node.SubNodes.Keys)
            {
                SuggestionRecursion(node.SubNodes[key], word + key, count--);
            }
        }
        // return true if prefix exist within the Trie
        public bool Contains(string word)
        {
            if (string.IsNullOrEmpty(word) || string.IsNullOrWhiteSpace(word))
            {
                return false;
            }
            word = word.Trim().ToLower();
            TrieNode node = root;
            foreach(char letter in word)
            {
                if(!node.SubNodes.ContainsKey(letter))
                {
                    return false;
                }
                node = node.SubNodes[letter];
            }
            return true;
        }

        public void Populate(IEnumerable<string> keys)
        {
            if(keys.Count() == 0)
            {
                return;
            }
            foreach (var key in keys)
            {
                Add(key);
            }
        }

        public void Populate(FileStream f)
        {
            var streamReader = new StreamReader(f, Encoding.UTF8);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                Add(line);
            }
            f.Close();
        }
    }
}