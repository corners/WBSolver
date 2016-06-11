﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBSolver
{
    public struct Point
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

    public sealed class Puzzle
    {
        public Puzzle(char[,] board, Func<IEnumerable<char>, WordMatch> isMatch)
        {
            _board = board;
            _isMatch = isMatch;
        }

        public Puzzle(int length, IList<char> board, Func<IEnumerable<char>, WordMatch> isMatch)
        {
            _board = new char[length, length];
            foreach (var y in Enumerable.Range(0, length))
            {
                foreach (var x in Enumerable.Range(0, length))
                {
                    _board[y, x] = board[(y * length) + x];
                }
            }
            _isMatch = isMatch;
        }

        readonly Func<IEnumerable<char>, WordMatch> _isMatch;
        readonly char[,] _board;

        public IEnumerable<KeyValuePair<Point, char>> Values()
        {
            foreach (var y in Enumerable.Range(0, _board.GetLength(0)))
            {
                foreach (var x in Enumerable.Range(0, _board.GetLength(1)))
                {
                    yield return new KeyValuePair<Point, char>(new Point(x, y), _board[y, x]);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var y in Enumerable.Range(0, _board.GetLength(0)))
            {
                foreach (var x in Enumerable.Range(0, _board.GetLength(1)))
                {
                    sb.Append(_board[y, x]);
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        
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

        readonly char Blank = ' ';

        public Puzzle RemovePath(IEnumerable<Point> path)
        {
            var lookup = new HashSet<Point>(path);
            var ps = Values().Select(a => lookup.Contains(a.Key) ? Blank : a.Value).ToList();
            return new Puzzle(_board.GetLength(0), ps, _isMatch);
        }
    }
}
