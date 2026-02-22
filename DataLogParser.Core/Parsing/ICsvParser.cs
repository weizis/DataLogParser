using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataLogParser.Core.Models;

namespace DataLogParser.Core.Parsing
{
    public interface ICsvParser
    {
        IEnumerable<SensorRecord> Parse(Stream stream);
    }
}
