using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ПримерWPF
{
    public partial class MainWindow : Window
    {
        // 1. БАГ (Зависание интерфейса / Threading): Измерение долгой задачи в главном потоке.
        // Если вызвать этот метод, всё окно WPF намертво зависнет (UI Freeze).
        private void FreezeUiButton_Click(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(5000); // Имитация долгой работы. Юзер подумает, что прога сдохла.
            MessageBox.Show("Готово!");
        }

        // 2. БАГ (Критический / Threads): Попытка изменить элемент интерфейса из другого потока.
        // Вызовет жесткий краш приложения: "The calling thread cannot access this object...".
        private void CrashButton_Click(object sender, RoutedEventArgs e)
        {
            Thread myThread = new Thread(() =>
            {
                // Ошибка! Нельзя напрямую менять свойства элементов WPF из фонового потока без Dispatcher.
                MyTextBlock.Text = "Привет из фона!";
            });
            myThread.Start();
        }

        // 3. БАГ (Универсальная уязвимость): Пароль зашит прямо в коде.
        // Сканер Гитхаба (CodeQL) сразу выкинет алерт "Hardcoded credential".
        private string DefaultUserToken = "WPF_SECRET_API_KEY_999231";

        // 4. БАГ (Утечка ресурсов): Не закрытый поток чтения.
        // Файл заблокируется операционной системой, сканер выдаст "Resource leak".
        public void ReadConfig()
        {
            StreamReader reader = new StreamReader("config.json");
            string content = reader.ReadToEnd();
            // Забыли reader.Close() или using!
        }
    }
}
