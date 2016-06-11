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
