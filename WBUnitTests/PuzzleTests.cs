using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WBSolver;
using System.Collections.Generic;

namespace WBUnitTests
{
    [TestClass]
    public class PuzzleTests
    {
        [TestMethod]
        public void ToString()
        {
            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p = new Puzzle(puzzle, (x) => WordMatch.Unknown);
            var actual = p.ToString();

            var expected = "ATG\r\nCAR\r\nCDO\r\n";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Values()
        {
            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p1 = new Puzzle(puzzle, (x) => WordMatch.Unknown);
            var values = p1.Values().Select(a => a.Value).ToList();
            var p2 = new Puzzle(3, values, (x) => WordMatch.Unknown);

            var expected = p1.ToString();
            var actual = p2.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Columns()
        {
            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p = new Puzzle(puzzle, (x) => WordMatch.Unknown);
            var actual = p.Columns();
            var expected = new List<List<char>>
            {
                new List<char> { 'A', 'C', 'C' },
                new List<char> { 'T', 'A', 'D' },
                new List<char> { 'G', 'R', 'O' },
            };

            Assert.IsTrue(Enumerable.SequenceEqual(expected[0], actual[0]));
            Assert.IsTrue(Enumerable.SequenceEqual(expected[1], actual[1]));
            Assert.IsTrue(Enumerable.SequenceEqual(expected[2], actual[2]));
        }

        [TestMethod]
        public void Columns_ToBoard()
        {
            var expected = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var columns = new List<IList<char>>
            {
                new List<char> { 'A', 'C', 'C' },
                new List<char> { 'T', 'A', 'D' },
                new List<char> { 'G', 'R', 'O' },
            };

            var actual = Puzzle.ColumnsToBoard(columns);

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Assert.AreEqual(expected[y,x], actual[y,x]);
                }
            }
        }

        [TestMethod]
        public void RemovePath_NoDropdown()
        {
            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p = new Puzzle(puzzle, (x) => WordMatch.Unknown);
            var path = new[]
            {
                new Point(0, 0), new Point(1, 0), 
            };
            p = p.RemovePath(path);

            var actual = p.ToString();
            var expected = "  G\r\nCAR\r\nCDO\r\n";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RemovePath_Dropdown()
        {
            var puzzle = new char[,]
            {
                { 'A', 'T', 'G' },
                { 'C', 'A', 'R' },
                { 'C', 'D', 'O' },
            };
            var p = new Puzzle(puzzle, (x) => WordMatch.Unknown);
            var path = new[]
            {
                new Point(0, 0), new Point(1, 0),
                                 new Point(1, 1),
                                 new Point(1, 2),  new Point (2, 2)
            };
            p = p.RemovePath(path);

            var actual = p.ToString();
            var expected = "   \r\nC G\r\nC R\r\n";
            Assert.AreEqual(expected, actual);
        }
    }
}
