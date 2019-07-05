using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            this.Initialize();
        }

        protected void Initialize()
        {
            root = new TrieNode();
            wordList = new List<string>();
        }

        public void Add(string key)
        {
            TrieNode node = this.root;
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

        public List<string> Suggestions(string word)
        {
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
            this.SuggestionRecursion(node, tempWord);
            return wordList;
        }

        protected void SuggestionRecursion(TrieNode node, string word)
        {
            if (node.IsWord)
            {
                wordList.Add(word);
            }
            foreach (var n in node.SubNodes)
            {
                this.SuggestionRecursion(n.Value, word + n.Key);
            }
        }

        public void FormTrie(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                this.Add(key);
            }
        }

        public void Populate(FileStream f)
        {
            var streamReader = new StreamReader(f, Encoding.UTF8);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                this.Add(line);
            }
            f.Close();
        }
    }
}