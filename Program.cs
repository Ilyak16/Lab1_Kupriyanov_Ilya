using System;
namespace Lab1_Kupriyanov_Ilya
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Калькулятор треугольников ===");
            Console.WriteLine("Введите длины трёх сторон (положительные вещественные числа).");
            Console.WriteLine("Для завершения программы нажмите Enter или введите 'exit' в любом поле.\n");

            while (true)
            {
                Console.Write("Сторона A: ");
                string inputA = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(inputA) || inputA.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                Console.Write("Сторона B: ");
                string inputB = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(inputB) || inputB.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                Console.Write("Сторона C: ");
                string inputC = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(inputC) || inputC.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

                Console.WriteLine("\n--- Результат ---");
                var (type, coords) = TriangleCalculator.ProcessTriangle(inputA, inputB, inputC);

                string typeDisplay = string.IsNullOrEmpty(type) ? "\"\" (некорректные данные)" : $"'{type}'";
                Console.WriteLine($"Тип треугольника: {typeDisplay}");

                Console.Write("Координаты вершин (A, B, C): [");
                Console.Write(string.Join(", ", coords));
                Console.WriteLine("]\n");
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("\n✅ Программа завершена. Все логи сохранены в файле triangle_project.log");
        }
    }
}
