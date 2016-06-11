using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WBSolver
{
    public enum WordMatch
    {
        NoMatch = -1,
        Unknown = 0,
        MatchStart = 1,
        MatchWod = 2,
    }

    public class Dictionary
    {
        class Node
        {
            public Node(char letter, int length)
            {
                Letter = letter;
                Partials = new Dictionary<char, Node>();
                IsRoot = false;
                Length = length;
            }

            public Node()
            {
                Letter = char.MinValue;
                Partials = new Dictionary<char, Node>();
                IsRoot = true;
                Length = 0;
            }

            public readonly char Letter;
            public readonly Dictionary<char, Node> Partials;
            public bool IsWord;
            public readonly bool IsRoot;
            public readonly int Length;

            public void AddWord(IList<char> letters)
            {
                if (letters.Count > 0)
                {
                    var first = letters.First();
                    Node node;
                    if (!Partials.TryGetValue(first, out node))
                    {
                        node = new Node(first, Length + 1);
                        Partials.Add(first, node);
                    }
                    node.AddWord(letters.Skip(1).ToList());
                }
                else
                    IsWord = true;
            }

            public Node Lookup(Queue<char> letters)
            {
                if (letters.Count == 0)
                    return this;
                var hd = letters.Dequeue();
                Node node;
                if (Partials.TryGetValue(hd, out node))
                {
                    return node.Lookup(letters);
                }
                return null;
            }
        }

        readonly Node _root;

        public Dictionary()
        {
            _root = new Node();
        }

        public void AddWord(string word)
        {
            _root.AddWord(word.ToCharArray());
        }

        public WordMatch Match(IEnumerable<char> chrs)
        {
            var q = new Queue<char>(chrs);
            var node = _root.Lookup(q);
            if (node == null)
                return WordMatch.NoMatch;
            else if (node.IsWord && node.Length == chrs.Count())
                return WordMatch.MatchWod;
            else
                return WordMatch.MatchStart;
        }

        public static Dictionary Load(string path)
        {
            var wordList = File.ReadAllLines(path);
            var d = new Dictionary();
            foreach (var word in wordList.Select(w => w.ToUpperInvariant()).Where(IsWord))
            {
                d.AddWord(word);
            }
            return d;
        }

        static bool IsWord(string s)
        {
            var chs = s.ToCharArray();
            return s.Length > 1 && chs.All(char.IsLetter);
        }
    }
}
