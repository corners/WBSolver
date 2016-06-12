using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WBSolver;
using System.Collections.Generic;

namespace WBUnitTests
{
    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        public void Solve_Diagonal()
        {
            var puzzle = new char[,]
            {
                { 'C', 'Z', 'Z' },
                { 'Z', 'A', 'Z' },
                { 'Z', 'Z', 'T' },
            };

            var d = new Dictionary();
            d.AddWord("CAT");

            var p = new Puzzle(puzzle, d.Match);

            var actual = p.Solve();

            var expected = new List<string> { "CAT" };
            Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
        }

        [TestMethod]
        public void Solve_Row()
        {
            var puzzle = new char[,]
            {
                { 'Z', 'Z', 'Z' },
                { 'C', 'A', 'T' },
                { 'Z', 'Z', 'Z' },
            };

            var d = new Dictionary();
            d.AddWord("CAT");

            var p = new Puzzle(puzzle, d.Match);

            var actual = p.Solve();

            var expected = new List<string> { "CAT" };
            Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
        }

        [TestMethod]
        public void Solve_Column()
        {
            var puzzle = new char[,]
            {
                { 'C', 'Z', 'Z' },
                { 'A', 'Z', 'Z' },
                { 'T', 'Z', 'Z' },
            };

            var d = new Dictionary();
            d.AddWord("CAT");

            var p = new Puzzle(puzzle, d.Match);

            var actual = p.Solve();

            var expected = new List<string> { "CAT" };
            Assert.IsTrue(Enumerable.SequenceEqual(expected, actual));
        }
    }
}
