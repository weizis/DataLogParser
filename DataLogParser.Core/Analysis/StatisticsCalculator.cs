using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLogParser.Core.Models;

namespace DataLogParser.Core.Analysis
{
    public class StatisticsCalculator
    {
        public double Average(IEnumerable<SensorRecord> records)
        {
            if (!records.Any())
                return 0;

            return records.Average(r => r.Value);
        }
        public double Min(IEnumerable<SensorRecord> records)
        {
            if (!records.Any())
                return 0;
            return records.Min(r => r.Value);
        }
        public double Max(IEnumerable<SensorRecord> records)
        {
            if (!records.Any())
                return 0;
            return records.Max(r => r.Value);
        }
        public Dictionary<string, double> AverageBySensor(IEnumerable<SensorRecord> records)
        {
            return records
                .GroupBy(r => r.SensorId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(r => r.Value)
                );
        }
    }
}
