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


    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "words.txt");
            var d = Dictionary.Load(path);

            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p = new Puzzle(puzzle, d.Match);
            var words = p.Solve();
        }
    }
}
