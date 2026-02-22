using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using DataLogParser.Core.Models;

namespace DataLogParser.Core.Parsing
{
    public class CsvParser : ICsvParser
    {
        private readonly CultureInfo _parsingCulture;

        public CsvParser(bool useInvariantCulture = true)
        {
            _parsingCulture = useInvariantCulture
                ? CultureInfo.InvariantCulture
                : CultureInfo.CurrentCulture;
        }

        public IEnumerable<SensorRecord> Parse(Stream stream)
        {
            Console.WriteLine("НОВАЯ ВЕРСИЯ ПАРСЕРА!");
            using var reader = new StreamReader(stream);

            string? line;
            bool isFirstLine = true;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                if (TryParseLine(line, lineNumber, out var record))
                {
                    yield return record;
                }
            }
        }

        public async IAsyncEnumerable<SensorRecord> ParseAsync(
            Stream stream,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(stream);

            string? line;
            bool isFirstLine = true;
            int lineNumber = 0;

            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                lineNumber++;

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                if (TryParseLine(line, lineNumber, out var record))
                {
                    yield return record;
                }
            }
        }

        private bool TryParseLine(string line, int lineNumber, out SensorRecord record)
        {
            record = null!;

            if (string.IsNullOrWhiteSpace(line))
                return false;

            var parts = line.Split(',');

            if (parts.Length < 3)
                return false;

            // Парсим дату
            if (!DateTime.TryParse(
                    parts[0].Trim(),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var timestamp))
                return false;

            // Парсим число
            var valueText = parts[2].Trim();
            double value;

            // Пробуем разные варианты
            bool parsed =
                // Вариант 1: с точкой (инвариантная культура)
                double.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, out value) ||

                // Вариант 2: с запятой (русская культура)
                double.TryParse(valueText, NumberStyles.Any, new CultureInfo("ru-RU"), out value) ||

                // Вариант 3: замена запятой на точку
                double.TryParse(valueText.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out value) ||

                // Вариант 4: замена точки на запятую
                double.TryParse(valueText.Replace('.', ','), NumberStyles.Any, new CultureInfo("ru-RU"), out value);

            if (!parsed)
                return false;

            record = new SensorRecord
            {
                Timestamp = timestamp,
                SensorId = parts[1].Trim(),
                Value = value
            };

            return true;
        }
    }
}