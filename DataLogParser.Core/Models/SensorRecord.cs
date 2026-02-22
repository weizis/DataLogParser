using System;

namespace DataLogParser.Core.Models
{
    public class SensorRecord
    {
        public DateTime Timestamp { get; set; }
        public string SensorId { get; set; } = "";
        public double Value { get; set; }
    }
}