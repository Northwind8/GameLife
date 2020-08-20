using System;
using System.Windows.Media;
using GameLife;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Cell[] cells = new Cell[2]; // Создаем массив из двух клеток
            cells[0] = new Cell(new Coord(1,1), 10, Brushes.Red,Brushes.Blue); // Заполняем массив первой клеткой
            cells[1] = new Cell(new Coord(23, 64), 10, Brushes.Red, Brushes.Blue); // Заполняем массив второй клеткой
            // Суть теста в том,что мы ищем нашу клетку по заданным координатам
            Assert.AreEqual(cells[0], cells[0][new Coord(cells[0].Position.X, cells[0].Position.Y)]); // Наш метод для тестов, который сравнивает нашу созданную клетку с той 
            Assert.AreEqual(cells[1], cells[1][new Coord(23, 64)]); // которую ищем при помощи метода индексатора, где первым параметром передается наша клетка, а вторым клетка с ее координатами
        }
    }
}
