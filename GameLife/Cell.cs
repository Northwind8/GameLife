using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameLife
{
    public class Cell
    {
        // Переменная имеющая { get; set} - это свойства. Без { get; set; } - это переменная или поле
        public static List<Cell> cells = new List<Cell>(); // Наш буфер клеток для поиска
        public Rectangle rect; // Инициализация нашего квадратика на форме
        public Coord Position { get; } // Свойство координат нашей клетки
        public int CellCount { get; set; } // Свойство Количество соседей

        public Cell this[Coord coord] // Метод класса, который называется индексатором (Его мы и будем тестировать)
        {
            get
            {
                return cells.OfType<Cell>().Where(x => (x.Position.X == coord.X) && (x.Position.Y == coord.Y)).First(); // Проходим по нашему буферу клеток и ищем те клетки у которых координаты совпадают с теми, что мы передаем в наш метод-индексатор
            }
        }

        public Cell(Coord position, int Size, Brush Fill, Brush Stroke) // Конструктор инициализации нашей клетки
        {
            rect = new Rectangle() { Width = Size + 1, Height = Size + 1, Fill = Fill, Stroke = Stroke }; // Создаем квадрат с нужными размерами и заливкой
            Cycle = CycleLife.Empty; // Говорим, что по умолчанию он будет мертвым
            Position = position; // Ставим ему координаты
            cells.Add(this); // Добавляем в буфер
        }

        public enum CycleLife // Перечисление 
        {
            Empty, // Пустая (мертвая)
            New, // Живая
        }
        private CycleLife state;
        public CycleLife Cycle // Свойство с перечислением
        {
            get { return state; }
            set
            {
                state = value;
            }
        }

     
    

        public static void ClearItems()
        {
            cells.Clear();
        }

        public Brush Death()
        {
            Cycle = CycleLife.Empty;

            return Brushes.Green;
        }
        public Brush GiveLife()
        {
            Cycle = CycleLife.New;

            return Brushes.Red;
        }



    }
}
