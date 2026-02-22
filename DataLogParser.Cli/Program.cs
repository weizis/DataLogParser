using System;
using System.IO;
using System.Linq;
using DataLogParser.Core.Parsing;
using DataLogParser.Core.Analysis;

namespace DataLogParser.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Data Log Parser");

            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: укажите путь к файлу!");
                Console.WriteLine("Пример: dotnet run data.csv");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Ошибка: файл не найден - {filePath}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
                return;
            }

            try
            {
                Console.WriteLine($"Чтение файла: {filePath}");
                Console.WriteLine(new string('-', 50));

                using var stream = File.OpenRead(filePath);

                ICsvParser parser = new CsvParser();
                var records = parser.Parse(stream).ToList(); // ToList() здесь один раз

                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"Найдено записей: {records.Count}");
                Console.WriteLine();

                if (records.Count == 0)
                {
                    Console.WriteLine("Нет данных для анализа!");
                    Console.ReadKey();
                    return;
                }

                var stats = new StatisticsCalculator();

                Console.WriteLine(" ОБЩАЯ СТАТИСТИКА:");
                Console.WriteLine($"   Среднее: {stats.Average(records):F2}");
                Console.WriteLine($"   Минимум: {stats.Min(records):F2}");
                Console.WriteLine($"   Максимум: {stats.Max(records):F2}");
                Console.WriteLine();

                Console.WriteLine(" СТАТИСТИКА ПО ДАТЧИКАМ:");
                var bySensor = stats.AverageBySensor(records);
                foreach (var sensor in bySensor)
                {
                    Console.WriteLine($"   {sensor.Key}: {sensor.Value:F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка при обработке: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}