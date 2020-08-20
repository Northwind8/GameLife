using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace GameLife
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged //INotifyPropertyChanged нужен для того, чтобы привязать наши свойства с нашей визуальной частью (окном приложения)
    {

        // Клетка = квадраты. Могу писать либо так, либо так
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); // Класс таймер, который запускает нашу игру и обновляет кадры
        // Есть логическая часть - это наш класс (его еще называют Code-behind), а есть наш интерфейс - это класс MainWindows.xaml.cs
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // DataContext нужен для того, чтобы наш интерфейс программы получил все данные для привязки
            canvas.MouseWheel += ReSize; // Событие колесика мыши. Сюда передаем метод, который меняет размер наших квадратиков в игре
            dispatcherTimer.Tick += new EventHandler(Timer1_Tick); // Событие на покадровую отрисовку. Грубо - это старты игры. Вызывает метод каждые уст. секунды
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,10); // Интервал обновления - 10 мс. Можно поменять
        }
        Dictionary<Coord, Cell> cells = new Dictionary<Coord, Cell>(); // Словарь, который запоминает наши квадраты по координатам

        private void ReSize(object sender, MouseWheelEventArgs e) // Метод изменения размера квадратов
        {
            canvas.Children.Clear(); // Очищаем полотно
            cells.Clear(); // Очищаем наш словарь
            Cell.ClearItems(); // Очищаем буффер наших клеток
            if (e.Delta > 0) // Если колесико крутим вверх
            {
                Size += 10; // то увеличиваем размер на 10
            }
            else Size -= 10; // иначе уменьшаем, если покрутили вниз
            Draw(); // Отрисовка наших квадратов
        }


        private void Timer1_Tick(object sender, EventArgs e) // Наш таймер
        {
            foreach (var cell in cells) CountCell(cell); // Запускаем метод подсчета соседей для каждой ячейки
            
            foreach (var cell in cells) // Перечисляем каждую ячейку
            {
                Game(cell.Value); // Передаем ячейку в метод Game
            }

            Count = cells.Values.OfType<Cell>().Where(x => x.Cycle == Cell.CycleLife.New).Count(); // Считаем наши живые клетки
          //Кол-во = словарь.Значения.ТипКлеток.Где(жизненный цикл клетки == живая).СчитаемКоличество
        }

        void Game(Cell cell) // Метод самой логики игры
        {
            if (cell.Cycle == Cell.CycleLife.Empty && cell.CellCount == 3) cell.rect.Fill = cell.GiveLife(); // Если ячейка пустая и рядом 3 соседа, то возрождаем
            if (cell.Cycle == Cell.CycleLife.New && cell.CellCount == 2 || cell.Cycle == Cell.CycleLife.New && cell.CellCount == 3) // Если Ячейка живая и рядом либо 2, либо 3 соседа, то живем
            {
                cell.rect.Fill = cell.GiveLife(); // Красим в красный и даем жизнь клетке
            }
            else // иначе умираем
            {
                cell.rect.Fill = cell.Death(); // Красив в зеленые и омертвляем клетку
            }
        }

        void CountCell(KeyValuePair<Coord, Cell> cell) // Метод для поиска соседей у ячейки
        {
            cell.Value.CellCount = 0; // Свойство, которое отвечает за количество живых соседей. Предварительно обнуляем всех соседей у квадрата
            for (int sx = -1; sx <= 1; sx++) // Двойной цикл, который бегает вокруг нашей ячейки и ищет живых соседей
                for (int sy = -1; sy <= 1; sy++)
                    if (!(sx == 0 && sy == 0)) // Проверка, если это наша ячейка, то ничего не делаем
                        if (cells[new Coord((cell.Key.X + sx + (int)(Width / Size)) % (int)(Width / Size), (cell.Key.Y + sy + (int)(Height / Size)) % (int)(Height / Size))].Cycle != Cell.CycleLife.Empty) cell.Value.CellCount++;
            // Если вокруг нашей ячейки есть живой сосед, то мы инкрементируем переменную (считаем).
        }
        void Draw() // Метод отрисовки наших клеток
        {
            for (int x = 0; x < Width / Size; x++) // Цикл по координатной плоскости X
            {
                for (int y = 0; y < Height / Size; y++) // Цикл по координатной плоскости Y
                {
                    Cell cell = new Cell(new Coord(x,y), Size, Brushes.Green, Brushes.Green); // Создаем клетку с параметрами
                   
                    cell.rect.MouseLeftButtonDown += (s, e) => { (s as Rectangle).Fill = Brushes.Red; cell.GiveLife(); }; // Даем ей событие при котором клетка будет оживляться по нажатию на левую кнопку мыши
                    cell.rect.MouseEnter += (s, e) => { if (e.LeftButton == MouseButtonState.Pressed) { (s as Rectangle).Fill = Brushes.Red; cell.GiveLife(); } }; // Событие, которое будет оживлять клетку, если ты навел курсор на нее и нажал на левую кнопку мыши
                    cell.Death(); // Убиваем все клетки сначала
                    Canvas.SetLeft(cell.rect, x * Size); // Метод для того, чтобы все клетки по оси X располагались упорядоченно (одна за другой)
                    Canvas.SetTop(cell.rect, y * Size); // Тоже самое, только по вертикали
                    cells.Add(cell.Position, cell); // Добавляем клетку с ее координатами в словарь
                    canvas.Children.Add(cell.rect); // Рисуем наши клетки на полотне
                }
            }
        }
     


        public event PropertyChangedEventHandler PropertyChanged; // Событие нужно, чтобы сообщать нашему интерфейсу о том, что изменилось свойство
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") // Метод, который будет вызывать событие выше. Атрибут CallerMemberName нужен для того, чтобы он сам подхватывал имя нужного нам свойства
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int count = 0; // Начальное число живых клеток
        public int Count // Свойство для подсчета живых клеток
        {
            get => count; // Возвращаем count выше, то есть 0 клеток
            set // Метод на установление значения. Все как на Java, только тут свойства с аксессорами set и get, а там методы
            {
                count = value; // Устанавливаем count наше значение
                NotifyPropertyChanged(); // Сообщаем интерфейсу наш Count
            }
        }
        int size = 30; // Начальный размер клеток

        public int Size // Размер клеток
        {
            get => size;
            set
            {
                if (value < 30) size = 30; // Если размер меньше 30, то ставим 30
                else if (value > 50) size = 50; // Если размер больше 50, то ставим 50
                else size = value; // Если больше 30 и меньше 50, то ставим ему это значение
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) // Когда загружает форма
        {
            Draw(); // Делаем отрисовку
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Кнопка старт
        {
            dispatcherTimer.Start(); // Стартуем. Включаем наш таймер, который запускает игру
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Кнопка стоп
        {
            dispatcherTimer.Stop(); // Тут все очевидно
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) // Кнопка рандомной генерации живых клеток
        {
            var rnd = new Random(); // Объявляем рандом класс
            foreach (var item in cells) // Проходимся по всем клеткам
            {
                if(rnd.Next(0,100) < 50) // Если выпало число меньше 50
                item.Value.rect.Fill = item.Value.GiveLife(); // то делаем клетку живой. Значением можно поменять, где 50 - это процент
            }
            Count = cells.Values.OfType<Cell>().Where(x => x.Cycle == Cell.CycleLife.New).Count(); // Обновляем счетчик живых клеток
        }
    }



    public struct Coord // Структура с координатами для просто представления
    {
        public int X; // Поле X
        public int Y; // Поле Y

        public Coord(int x, int y)
        {
            X = x; // Инициализируем X
            Y = y; // Инициализируем Y
        }

        public override string ToString() // Переопределям ToString() чтобы можно было легко отслеживать координаты клеток
        {
            return string.Format("X: {0}, Y: {1}", X, Y);
        }
    }


}
