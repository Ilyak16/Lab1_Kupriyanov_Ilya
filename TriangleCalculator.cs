using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_Kupriyanov_Ilya
{
    public class TriangleCalculator
    {
        private static readonly object _logLock = new object();
        private const string LogFileName = "triangle_project.log";
        // Допуск для сравнения float-чисел (погрешность IEEE-754)
        private const float Epsilon = 1e-5f;

        /// <summary>
        /// Вычисляет вид треугольника и координаты его вершин для отрисовки в поле 100x100 px.
        /// </summary>
        public static (string Type, List<(int X, int Y)> Coordinates) ProcessTriangle(string strA, string strB, string strC)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string[] parameters = { strA ?? "", strB ?? "", strC ?? "" };

            var invalidCoords = new List<(int X, int Y)> { (-2, -2), (-2, -2), (-2, -2) };
            var notTriangleCoords = new List<(int X, int Y)> { (-1, -1), (-1, -1), (-1, -1) };

            try
            {
                // 1. Парсинг и валидация (InvariantCulture гарантирует точку как десятичный разделитель)
                if (!float.TryParse(strA, NumberStyles.Float, CultureInfo.InvariantCulture, out float a) ||
                    !float.TryParse(strB, NumberStyles.Float, CultureInfo.InvariantCulture, out float b) ||
                    !float.TryParse(strC, NumberStyles.Float, CultureInfo.InvariantCulture, out float c))
                {
                    throw new FormatException("Входные строки не являются корректными числами.");
                }

                if (a <= 0f || b <= 0f || c <= 0f)
                {
                    throw new ArgumentException("Длины сторон должны быть строго положительными.");
                }

                // 2. Проверка неравенства треугольника
                if (a + b <= c || a + c <= b || b + c <= a)
                {
                    string type = "не треугольник";
                    LogSuccess(timestamp, parameters, type, notTriangleCoords);
                    return (type, notTriangleCoords);
                }

                // 3. Определение типа треугольника
                bool eqAB = MathF.Abs(a - b) < Epsilon;
                bool eqBC = MathF.Abs(b - c) < Epsilon;
                bool eqAC = MathF.Abs(a - c) < Epsilon;

                string triType = (eqAB && eqBC) ? "равносторонний" :
                                 (eqAB || eqBC || eqAC) ? "равнобедренный" : "разносторонний";

                // 4. Вычисление "сырых" координат
                // Размещаем A в (0,0), B на оси X на расстоянии a.
                // C вычисляется как пересечение окружностей радиусов b (от A) и c (от B).
                float xC = (b * b + a * a - c * c) / (2f * a);
                float yC = MathF.Sqrt(MathF.Max(0f, b * b - xC * xC));

                var rawCoords = new List<(float X, float Y)>
                {
                    (0f, 0f),
                    (a, 0f),
                    (xC, yC)
                };

                // 5. Масштабирование под поле 100x100 px
                float minX = rawCoords.Min(p => p.X);
                float maxX = rawCoords.Max(p => p.X);
                float minY = rawCoords.Min(p => p.Y);
                float maxY = rawCoords.Max(p => p.Y);

                float width = maxX - minX;
                float height = maxY - minY;
                float maxDim = MathF.Max(width, height);

                // Сохраняем пропорции. Отступ 5px со всех сторон → рабочая область 90x90.
                float scale = maxDim > 0f ? 90f / maxDim : 1f;
                float offsetX = 5f, offsetY = 5f;

                var scaledCoords = rawCoords.Select(p =>
                {
                    int sx = Math.Clamp((int)MathF.Round((p.X - minX) * scale + offsetX), 0, 99);
                    int sy = Math.Clamp((int)MathF.Round((p.Y - minY) * scale + offsetY), 0, 99);
                    return (sx, sy);
                }).ToList();

                LogSuccess(timestamp, parameters, triType, scaledCoords);
                return (triType, scaledCoords);
            }
            catch (Exception ex)
            {
                // Программа не прерывается, возвращает пустую строку и (-2,-2)
                LogFailure(timestamp, parameters, "", invalidCoords, ex);
                return ("", invalidCoords);
            }
        }

        // --- Логирование ---
        private static void LogSuccess(string time, string[] parameters, string type, List<(int X, int Y)> coords)
        {
            string coordsStr = string.Join(", ", coords.Select(c => $"({c.X}, {c.Y})"));
            string logMsg = $"[{time}] УСПЕХ | Параметры: [{string.Join(", ", parameters)}] | " +
                            $"Результат: Тип='{type}', Координаты=[{coordsStr}]";
            WriteLog(logMsg);
        }

        private static void LogFailure(string time, string[] parameters, string type, List<(int X, int Y)> coords, Exception ex)
        {
            string coordsStr = string.Join(", ", coords.Select(c => $"({c.X}, {c.Y})"));
            string logMsg = $"[{time}] ОШИБКА | Параметры: [{string.Join(", ", parameters)}] | " +
                            $"Результат: Тип=\"{type}\", Координаты=[{coordsStr}] | " +
                            $"Ошибка: {ex.Message}\nТрассировка стека:\n{ex.StackTrace}";
            WriteLog(logMsg);
        }

        private static void WriteLog(string message)
        {
            lock (_logLock)
            {
                // Запись в файл + дублирование в консоль для отладки
                File.AppendAllText(LogFileName, message + Environment.NewLine);
                Console.WriteLine(message);
            }
        }
    }
}
