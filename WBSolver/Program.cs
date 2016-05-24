using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    sealed class Puzzle
    {
        struct Point
        {
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;

            public Point Add(Point pt)
            {
                return new Point(this.x + pt.x, this.y + pt.y);
            }
        }

        public Puzzle(char[,] board, Func<IEnumerable<char>, WordMatch> isMatch)
        {
            _board = board;
            _isMatch = isMatch;
        }

        readonly Func<IEnumerable<char>, WordMatch> _isMatch;
        readonly char[,] _board;


        List<Point> Surrounding(Point pt, IList<Point> path)
        {
            var offsets = new[]
            {
                new Point(-1, -1), new Point(0, -1), new Point (1, -1),
                new Point(-1, 0),                    new Point (1, 0),
                new Point(-1, 1),  new Point(0, 1),  new Point (1, 1),
            };

            return offsets.Select(o => pt.Add(o))
                          .Where(Inbounds)
                          .Except(path)
                          .ToList();
        }

        bool Inbounds(Point pt)
        {
            return 0 <= pt.x && pt.x <= _board.GetUpperBound(0)
                && 0 <= pt.y && pt.y <= _board.GetUpperBound(1);
        }

        IEnumerable<List<Point>> GetPaths(List<Point> path)
        {
            var match = _isMatch(path.Select(ToChar));
            if (match == WordMatch.MatchStart || match == WordMatch.MatchWod)
            {
                var hd = path[path.Count - 1];
                if (match == WordMatch.MatchWod)
                    yield return path;

                var surrounds = Surrounding(hd, path);
                foreach (var s in surrounds)
                {
                    var newPath = path.Union(new[] { s }).ToList();
                    foreach (var ps in GetPaths(newPath))
                    {
                        yield return ps;
                    }
                }
            }
        }

        char ToChar(Point pt)
        {
            return _board[pt.x, pt.y];
        }

        string ToString(IEnumerable<char> chs)
        {
            return new string(chs.ToArray());
        }
        
        public List<string> Solve()
        {
            var points = Enumerable.Range(0, _board.GetLength(0))
                                   .SelectMany(x => Enumerable.Range(0, _board.GetLength(1)).Select(y => new Point(x, y)));

            var words = points.Select(p => GetPaths(new List<Point>() { p }))
                              .SelectMany(paths => paths)
                              .Select(pts => pts.Select(ToChar))
                              .Select(ToString)
                              .ToList();
            return words;
        }
    }

    static class Util
    {
        public static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            // TODO: Validation and special-casing for arrays.Count == 0
            int minorLength = arrays[0].Length;
            T[,] ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        public static char[,] StringToBoard(int size, string s)
        {
            var chs = s.ToCharArray();
            var s2 = Enumerable.Range(0, size - 1)
                               .Select(n => chs.Skip(n * size).Take(size).ToArray())
                               .ToList();
            return Util.CreateRectangularArray<char>(s2.ToList());
        }
    }


    class Dictionary
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
    }

    class Program
    {
        static bool IsWord(string s)
        {
            var chs = s.ToCharArray();
            return s.Length > 1 && chs.All(char.IsLetter);
        }

        static void Main(string[] args)
        {
            var wordList = LoadWords(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "words.txt"));

            var d = new Dictionary();
            foreach (var word in wordList.Select(w => w.ToUpperInvariant()).Where(IsWord))
            {
                d.AddWord(word);
            }

            var puzzle = new char[,]
            {
                { 'A', 'B' },
                { 'C', 'D' }
            };
            var p = new Puzzle(puzzle, d.Match);
            var words = p.Solve();
        }

        private static string[] LoadWords(string p)
        {
            return File.ReadAllLines(p);
        }
    }
}
