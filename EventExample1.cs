using System;
using System.IO;
using System.Threading;

namespace EventExample
{
    public class Programs
    {
        private static readonly Random random = new Random();
        private const int PairCount = 10;
        private static readonly string filename = "pairs.txt";
        private static readonly string sumFilename = "sums.txt";
        private static readonly string productFilename = "products.txt";

        public static event EventHandler GenerationCompleted;

        static void Main(string[] args)
        {
            Thread generatorThread = new Thread(GeneratePairs);
            generatorThread.Start();

            GenerationCompleted += SumPairs;
            GenerationCompleted += MultiplyPairs;

            generatorThread.Join();
            Console.WriteLine("Generation completed. Waiting for calculation threads to finish...");
        }

        static void GeneratePairs()
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                for (int i = 0; i < PairCount; i++)
                {
                    int number1 = random.Next(1, 101);
                    int number2 = random.Next(1, 101);
                    writer.WriteLine($"{number1},{number2}");
                    Thread.Sleep(500);
                }
            }
            OnGenerationCompleted();
        }

        static void OnGenerationCompleted()
        {
            GenerationCompleted?.Invoke(null, EventArgs.Empty);
        }

        static void SumPairs(object sender, EventArgs e)
        {
            using (StreamReader reader = new StreamReader(filename))
            using (StreamWriter writer = new StreamWriter(sumFilename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] numbers = line.Split(',');
                    int sum = int.Parse(numbers[0]) + int.Parse(numbers[1]);
                    writer.WriteLine(sum);
                }
            }
            Console.WriteLine("Sum calculation completed.");
        }

        static void MultiplyPairs(object sender, EventArgs e)
        {
            using (StreamReader reader = new StreamReader(filename))
            using (StreamWriter writer = new StreamWriter(productFilename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] numbers = line.Split(',');
                    int product = int.Parse(numbers[0]) * int.Parse(numbers[1]);
                    writer.WriteLine(product);
                }
            }
            Console.WriteLine("Product calculation completed.");
        }
    }
}
