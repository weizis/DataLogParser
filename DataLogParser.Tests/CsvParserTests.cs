using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLogParser.Core.Parsing;
using DataLogParser.Core.Models;

namespace DataLogParser.Tests
{
    [TestClass]
    public class CsvParserTests
    {
        private const string SampleCsv = "Timestamp,SensorId,Value\n2024-12-01T12:00:00Z,A1,23.5\n2024-12-01T12:00:01Z,A1,23.7\n2024-12-01T12:00:02Z,A2,19.2";

        [TestMethod]
        public void Parse_ValidCsv_ReturnsCorrectNumberOfRecords()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleCsv));
            var parser = new CsvParser();

            var result = parser.Parse(stream).ToList();

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void Parse_ValidCsv_FirstRecordHasCorrectValues()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleCsv));
            var parser = new CsvParser();

            var result = parser.Parse(stream).ToList();

            var first = result[0];
            Assert.AreEqual(new DateTime(2024, 12, 1, 12, 0, 0), first.Timestamp);
            Assert.AreEqual("A1", first.SensorId);
            Assert.AreEqual(23.5, first.Value);
        }

        [TestMethod]
        public async Task ParseAsync_ValidCsv_ReturnsCorrectRecords()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleCsv));
            var parser = new CsvParser();
            var result = new List<SensorRecord>();

            await foreach (var record in parser.ParseAsync(stream))
            {
                result.Add(record);
            }

            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]

        public void Parse_EmptyCsv_ReturnsNoRecords()
        {
            string emptyCsv = "Timestamp,SensorId,Value";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(emptyCsv));
            var parser = new CsvParser();

            var result = parser.Parse(stream).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Parse_InvalidLines_SkipsBadRows()
        {
            string csv = "Timestamp,SensorId,Value\nInvalidLine\n2024-12-01T12:00:00Z,A1,23.5\n,,\n2024-12-01T12:00:01Z,A1,23.7";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var parser = new CsvParser();

            var result = parser.Parse(stream).ToList();

            Assert.AreEqual(2, result.Count); // должно пропустить неверные строки
            Assert.AreEqual(23.5, result[0].Value);
            Assert.AreEqual(23.7, result[1].Value);
        }
        [TestMethod]
        public void Debug_Parser()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(SampleCsv));
            var parser = new CsvParser();

            var result = parser.Parse(stream).ToList();

            Console.WriteLine($"Records found: {result.Count}");
            foreach (var r in result)
            {
                Console.WriteLine($"  {r.Timestamp} | {r.SensorId} | {r.Value}");
            }

            Assert.IsTrue(result.Count > 0);
        }


    }
}