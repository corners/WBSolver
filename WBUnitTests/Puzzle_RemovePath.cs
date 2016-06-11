using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WBSolver;

namespace WBUnitTests
{
    [TestClass]
    public class Puzzle_RemovePath
    {
        [TestMethod]
        public void TestMethod1()
        {
            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p = new Puzzle(puzzle, (x) => WordMatch.Unknown);
            var actual = p.ToString();

            var expected = "ATGCARCDO";
            Assert.AreEqual(expected, actual);
        }
    }
}
