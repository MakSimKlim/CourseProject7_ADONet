using CourseProjectADONet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32; // Добавлено для извлечения картинок из базы данных
using System.IO; // Добавлено для извлечения картинок из базы данных


namespace CourseProjectADONet
{
    public partial class MainWindow : Window
    {
        readonly CourseProjectADONetEntities db;

        public MainWindow()
        {
            InitializeComponent();
            db = new CourseProjectADONetEntities();
            LoadDataWalls();
            LoadDataColumns();
            LoadDataOverlaps();
            LoadDataTransitions();
            LoadDataSections(); // Загрузка данных для секций

        }

        private void LoadDataWalls()
        {
            var walls = db.Стены.Include("СтеныБет").ToList(); // Используется Include для загрузки связанных данных
            WallsComboBox.ItemsSource = walls;
        }

        private void LoadDataColumns()
        {
            var columns = db.Колонны.Include("КолоннБет").ToList();
            ColumnsComboBox.ItemsSource = columns;
        }

        private void LoadDataOverlaps()
        {
            var overlaps = db.Перекрытия.Include("ПерекрБет").ToList();
            OverlapsComboBox.ItemsSource = overlaps;
        }

        private void LoadDataTransitions()
        {
            var transitions = db.Переходные.Include("ПерехБет").ToList();
            TransitionsComboBox.ItemsSource = transitions;
        }

        private void LoadDataSections()
        {
            var sections = db.Секции.ToList(); // Загрузка всех секций
            QueryComboBox.ItemsSource = sections;
        }




        // ===================================  СТЕНЫ  =============================================================================

        //метод, который отображает изображение из базы данных в элементе Image в WPF
        private void WallsDisplayImageFromDatabase()
        {
            // Найдите стену, которая соответствует значению в TextBox
            var selectedWall = db.Стены.FirstOrDefault(w => w.Наименование == WallNameTextBox.Text);

            if (selectedWall != null)
            {
                byte[] imageBytes = selectedWall.Изображение;
                if (imageBytes != null)
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();

                        // Установите изображение для TransitionImage
                        WallImage.Source = image;
                    }
                }
            }
        }

        private void WallsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedWall = (Стены)WallsComboBox.SelectedItem;
            if (selectedWall != null)
            {
                WallNameTextBox.Text = selectedWall.Наименование;
                WallVolumeTextBox.Text = selectedWall.Объем.ToString();

                // Получите связанный объект СтеныБет
                var wallConcrete = db.СтеныБет.FirstOrDefault(wc => wc.IDСтены == selectedWall.IDСтены);
                if (wallConcrete != null)
                {
                    // Получите связанный объект Бетоны
                    var concrete = db.Бетоны.FirstOrDefault(c => c.IDБетона == wallConcrete.IDБетона);
                    if (concrete != null)
                    {
                        WallConcreteClassTextBox.Text = concrete.Класс;
                    }
                }
                // Отобразите изображение
                WallsDisplayImageFromDatabase();
            }
        }

        // Обработчик события клика мышью на изображении
        private void WallImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Создаем новое окно
            Window imageWindow = new Window
            {
                Title = "Увеличенное изображение",
                Width = 800,
                Height = 800
            };

            // Создаем новый Image контрол и устанавливаем его как контент окна
            Image largeImage = new Image
            {
                Source = WallImage.Source,
                Stretch = Stretch.Uniform
            };
            imageWindow.Content = largeImage;

            // Открываем окно
            imageWindow.Show();
        }

        private void SaveWallButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedWall = (Стены)WallsComboBox.SelectedItem;
            if (selectedWall != null)
            {
                selectedWall.Наименование = WallNameTextBox.Text;
                selectedWall.Объем = decimal.Parse(WallVolumeTextBox.Text);

                // Получите связанный объект СтеныБет
                var wallConcrete = db.СтеныБет.FirstOrDefault(wc => wc.IDСтены == selectedWall.IDСтены);
                if (wallConcrete != null)
                {
                    // Получите новый объект Бетоны на основе класса бетона из TextBox
                    var newConcrete = db.Бетоны.FirstOrDefault(c => c.Класс == WallConcreteClassTextBox.Text);
                    if (newConcrete != null)
                    {
                        // Обновите IDБетона в связующей таблице
                        wallConcrete.IDБетона = newConcrete.IDБетона;
                    }
                }

                db.SaveChanges();
            }
        }

        private void SaveImageWallButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedWall = (Стены)WallsComboBox.SelectedItem;
            if (selectedWall != null)
            {
                // столбец Изображение в таблице Стены хранит изображения в формате byte[]
                byte[] imageBytes = selectedWall.Изображение;
                if (imageBytes != null)
                {
                    // Создайте диалог для сохранения файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png"; // определение фильтров для типа файлов
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Сохраните изображение в выбранный файл
                        File.WriteAllBytes(saveFileDialog.FileName, imageBytes);
                    }
                }
            }

        }


        private void ClearWallButton_Click(object sender, RoutedEventArgs e)
        {
            WallImage.Source = null;
            WallsComboBox.SelectedItem = null;
            WallNameTextBox.Clear();
            WallVolumeTextBox.Clear();
            WallConcreteClassTextBox.Clear();
        }
        // ===================================  СТРОКИ ДЛЯ ЗАПРОСОВ  =============================================================================
        private void QueryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранную секцию
            var selectedSection = (Секции)QueryComboBox.SelectedItem;

            // Вычисляем суммарный объем для колонн
            var columnsVolume1 = (from c1 in db.КолоннЭтСек1
                                  join c in db.Колонны on c1.IDколонны equals c.IDколонны
                                  join e1 in db.ЭтажиСек1 on c1.IDЭтажаСек1 equals e1.IDЭтажаСек1
                                  where e1.IDСекции == selectedSection.IDСекции
                                  select c1.КоличКолоннНаЭтаже * c.ДлинаВСечении * c.ШиринаВСечении * e1.ВысотаСтенКолонн).Sum();

            var columnsVolume2 = (from c2 in db.КолоннЭтСек2
                                  join c in db.Колонны on c2.IDколонны equals c.IDколонны
                                  join e2 in db.ЭтажиСек2 on c2.IDЭтажаСек2 equals e2.IDЭтажаСек2
                                  where e2.IDСекции == selectedSection.IDСекции
                                  select c2.КоличКолоннНаЭтаже * c.ДлинаВСечении * c.ШиринаВСечении * e2.ВысотаСтенКолонн).Sum();

            // Вычисляем суммарный объем для стен
            var wallsVolume1 = (from w1 in db.СтеныЭтСек1
                                join w in db.Стены on w1.IDСтены equals w.IDСтены
                                join e1 in db.ЭтажиСек1 on w1.IDЭтажаСек1 equals e1.IDЭтажаСек1
                                where e1.IDСекции == selectedSection.IDСекции
                                select w1.КоличСтенНаЭтаже * w.Объем).Sum();

            var wallsVolume2 = (from w2 in db.СтеныЭтСек2
                                join w in db.Стены on w2.IDСтены equals w.IDСтены
                                join e2 in db.ЭтажиСек2 on w2.IDЭтажаСек2 equals e2.IDЭтажаСек2
                                where e2.IDСекции == selectedSection.IDСекции
                                select w2.КоличСтенНаЭтаже * w.Объем).Sum();

            // Вычисляем суммарный объем для перекрытий
            var overlaysVolume1 = (from o1 in db.ПерекрЭтСек1
                                   join o in db.Перекрытия on o1.IDПерекрытия equals o.IDПерекрытия
                                   join e1 in db.ЭтажиСек1 on o1.IDЭтажаСек1 equals e1.IDЭтажаСек1
                                   where e1.IDСекции == selectedSection.IDСекции
                                   select o1.КоличПерекрНаЭтаже * o.Объем).Sum();

            var overlaysVolume2 = (from o2 in db.ПерекрЭтСек2
                                   join o in db.Перекрытия on o2.IDПерекрытия equals o.IDПерекрытия
                                   join e2 in db.ЭтажиСек2 on o2.IDЭтажаСек2 equals e2.IDЭтажаСек2
                                   where e2.IDСекции == selectedSection.IDСекции
                                   select o2.КоличПерекрНаЭтаже * o.Объем).Sum();

            // Вычисляем суммарный объем для переходных площадок
            var platformsVolume1 = (from p1 in db.ПерехЭтСек1
                                    join p in db.Переходные on p1.IDПереходной equals p.IDПереходной
                                    join e1 in db.ЭтажиСек1 on p1.IDЭтажаСек1 equals e1.IDЭтажаСек1
                                    where e1.IDСекции == selectedSection.IDСекции
                                    select p1.КоличПерехНаЭтаже * p.Объем).Sum();

            var platformsVolume2 = (from p2 in db.ПерехЭтСек2
                                    join p in db.Переходные on p2.IDПереходной equals p.IDПереходной
                                    join e2 in db.ЭтажиСек2 on p2.IDЭтажаСек2 equals e2.IDЭтажаСек2
                                    where e2.IDСекции == selectedSection.IDСекции
                                    select p2.КоличПерехНаЭтаже * p.Объем).Sum();

            // Суммируем все объемы
            var totalVolume = columnsVolume1 + columnsVolume2 + wallsVolume1 + wallsVolume2 + overlaysVolume1 + overlaysVolume2 + platformsVolume1 + platformsVolume2;

            // Выводим результат в ResultTextBox
            ResultTextBox.Text = totalVolume.ToString();
        }


            // ===================================  КОЛОННЫ  =============================================================================

            //метод, который отображает изображение из базы данных в элементе Image в WPF
            private void ColumnsDisplayImageFromDatabase()
        {
            // Найдите колонну, которая соответствует значению в TextBox
            var selectedColumn = db.Колонны.FirstOrDefault(w => w.Наименование == ColumnNameTextBox.Text);

            if (selectedColumn != null)
            {
                byte[] imageBytes = selectedColumn.Изображение;
                if (imageBytes != null)
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();

                        // Установите изображение для TransitionImage
                        ColumnImage.Source = image;
                    }
                }
            }
        }

        private void ColumnsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedColumn = (Колонны)ColumnsComboBox.SelectedItem;
            if (selectedColumn != null)
            {
                ColumnNameTextBox.Text = selectedColumn.Наименование;


                // Вычисление объема колонны
                var columnVolume = (from c in db.КолоннЭтСек1
                                    join f in db.ЭтажиСек1 on c.IDЭтажаСек1 equals f.IDЭтажаСек1
                                    where c.IDколонны == selectedColumn.IDколонны
                                    select selectedColumn.ДлинаВСечении * selectedColumn.ШиринаВСечении * f.ВысотаСтенКолонн).FirstOrDefault();

                if (columnVolume == null)
                {
                    columnVolume = (from c in db.КолоннЭтСек2
                                    join f in db.ЭтажиСек2 on c.IDЭтажаСек2 equals f.IDЭтажаСек2
                                    where c.IDколонны == selectedColumn.IDколонны
                                    select selectedColumn.ДлинаВСечении * selectedColumn.ШиринаВСечении * f.ВысотаСтенКолонн).FirstOrDefault();
                }

                ColumnVolumeTextBox.Text = columnVolume.ToString();

                // Получите связанный объект СтеныБет
                var columnConcrete = db.КолоннБет.FirstOrDefault(wc => wc.IDколонны == selectedColumn.IDколонны);
                if (columnConcrete != null)
                {
                    // Получите связанный объект Бетоны
                    var concrete = db.Бетоны.FirstOrDefault(c => c.IDБетона == columnConcrete.IDБетона);
                    if (concrete != null)
                    {
                        ColumnConcreteClassTextBox.Text = concrete.Класс;
                    }
                }

                // Отобразите изображение
                ColumnsDisplayImageFromDatabase();


            }
        }

        // Обработчик события клика мышью на изображении
        private void ColumnImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Создаем новое окно
            Window imageWindow = new Window
            {
                Title = "Увеличенное изображение",
                Width = 800,
                Height = 800
            };

            // Создаем новый Image контрол и устанавливаем его как контент окна
            Image largeImage = new Image
            {
                Source = ColumnImage.Source,
                Stretch = Stretch.Uniform
            };
            imageWindow.Content = largeImage;

            // Открываем окно
            imageWindow.Show();
        }


        private void SaveColumnButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedColumn = (Колонны)ColumnsComboBox.SelectedItem;
            if (selectedColumn != null)
            {
                selectedColumn.Наименование = ColumnNameTextBox.Text;
                //selectedColumn.Объем = decimal.Parse(ColumnVolumeTextBox.Text);

                // Получите связанный объект КолоннБет
                var columnConcrete = db.КолоннБет.FirstOrDefault(wc => wc.IDколонны == selectedColumn.IDколонны);
                if (columnConcrete != null)
                {
                    // Получите новый объект Бетоны на основе класса бетона из TextBox
                    var newConcrete = db.Бетоны.FirstOrDefault(c => c.Класс == ColumnConcreteClassTextBox.Text);
                    if (newConcrete != null)
                    {
                        // Обновите IDБетона в связующей таблице
                        columnConcrete.IDБетона = newConcrete.IDБетона;
                    }
                }

                db.SaveChanges();
            }
        }



        private void SaveImageColumnButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedColumn = (Колонны)ColumnsComboBox.SelectedItem;
            if (selectedColumn != null)
            {
                // столбец Изображение в таблице Стены хранит изображения в формате byte[]
                byte[] imageBytes = selectedColumn.Изображение;
                if (imageBytes != null)
                {
                    // Создайте диалог для сохранения файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png"; // определение фильтров для типа файлов
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Сохраните изображение в выбранный файл
                        File.WriteAllBytes(saveFileDialog.FileName, imageBytes);
                    }
                }
            }

        }


        private void ClearColumnButton_Click(object sender, RoutedEventArgs e)
        {
            ColumnImage.Source = null;
            ColumnsComboBox.SelectedItem = null;
            ColumnNameTextBox.Clear();
            ColumnVolumeTextBox.Clear();
            ColumnConcreteClassTextBox.Clear();
        }

        // ===================================  ПЕРЕКРЫТИЯ  =============================================================================
        //метод, который отображает изображение из базы данных в элементе Image в WPF
        private void OverlapsDisplayImageFromDatabase()
        {
            // Найдите колонну, которая соответствует значению в TextBox
            var selectedOverlap = db.Перекрытия.FirstOrDefault(w => w.Наименование == OverlapNameTextBox.Text);

            if (selectedOverlap != null)
            {
                byte[] imageBytes = selectedOverlap.Изображение;
                if (imageBytes != null)
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();

                        // Установите изображение для TransitionImage
                        OverlapImage.Source = image;
                    }
                }
            }
        }
        private void OverlapsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedOverlap = (Перекрытия)OverlapsComboBox.SelectedItem;
            if (selectedOverlap != null)
            {
                OverlapNameTextBox.Text = selectedOverlap.Наименование;
                OverlapVolumeTextBox.Text = selectedOverlap.Объем.ToString();
                // Получите связанный объект СтеныБет
                var overlapConcrete = db.ПерекрБет.FirstOrDefault(wc => wc.IDПерекрытия == selectedOverlap.IDПерекрытия);
                if (overlapConcrete != null)
                {
                    // Получите связанный объект Бетоны
                    var concrete = db.Бетоны.FirstOrDefault(c => c.IDБетона == overlapConcrete.IDБетона);
                    if (concrete != null)
                    {
                        OverlapConcreteClassTextBox.Text = concrete.Класс;
                    }
                }

                // Отобразите изображение
                OverlapsDisplayImageFromDatabase();


            }
        }

        // Обработчик события клика мышью на изображении
        private void OverlapImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Создаем новое окно
            Window imageWindow = new Window
            {
                Title = "Увеличенное изображение",
                Width = 800,
                Height = 800
            };

            // Создаем новый Image контрол и устанавливаем его как контент окна
            Image largeImage = new Image
            {
                Source = OverlapImage.Source,
                Stretch = Stretch.Uniform
            };
            imageWindow.Content = largeImage;

            // Открываем окно
            imageWindow.Show();
        }

        private void SaveOverlapButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedOverlap = (Перекрытия)OverlapsComboBox.SelectedItem;
            if (selectedOverlap != null)
            {
                selectedOverlap.Наименование = OverlapNameTextBox.Text;
                selectedOverlap.Объем = decimal.Parse(OverlapVolumeTextBox.Text);

                // Получите связанный объект СтеныБет
                var overlapConcrete = db.ПерекрБет.FirstOrDefault(wc => wc.IDПерекрытия == selectedOverlap.IDПерекрытия);
                if (overlapConcrete != null)
                {
                    // Получите новый объект Бетоны на основе класса бетона из TextBox
                    var newConcrete = db.Бетоны.FirstOrDefault(c => c.Класс == OverlapConcreteClassTextBox.Text);
                    if (newConcrete != null)
                    {
                        // Обновите IDБетона в связующей таблице
                        overlapConcrete.IDБетона = newConcrete.IDБетона;
                    }
                }

                db.SaveChanges();
            }
        }


        private void SaveImageOverlapButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedOverlap = (Перекрытия)OverlapsComboBox.SelectedItem;
            if (selectedOverlap != null)
            {
                // столбец Изображение в таблице Стены хранит изображения в формате byte[]
                byte[] imageBytes = selectedOverlap.Изображение;
                if (imageBytes != null)
                {
                    // Создайте диалог для сохранения файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png"; // определение фильтров для типа файлов
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Сохраните изображение в выбранный файл
                        File.WriteAllBytes(saveFileDialog.FileName, imageBytes);
                    }
                }
            }

        }

        private void ClearOverlapButton_Click(object sender, RoutedEventArgs e)
        {
            OverlapImage.Source = null;
            OverlapsComboBox.SelectedItem = null;
            OverlapNameTextBox.Clear();
            OverlapVolumeTextBox.Clear();
            OverlapConcreteClassTextBox.Clear();
        }

        // ===================================  ПЕРЕХОДНЫЕ  =============================================================================

        //метод, который отображает изображение из базы данных в элементе Image в WPF
        private void TransitionsDisplayImageFromDatabase()
        {
            // Найдите колонну, которая соответствует значению в TextBox
            var selectedTransition = db.Переходные.FirstOrDefault(w => w.Наименование == TransitionNameTextBox.Text);

            if (selectedTransition != null)
            {
                byte[] imageBytes = selectedTransition.Изображение;
                if (imageBytes != null)
                {
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();

                        // Установите изображение для TransitionImage
                        TransitionImage.Source = image;
                    }
                }
            }
        }
        private void TransitionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTransition = (Переходные)TransitionsComboBox.SelectedItem;
            if (selectedTransition != null)
            {
                TransitionNameTextBox.Text = selectedTransition.Наименование;
                TransitionVolumeTextBox.Text = selectedTransition.Объем.ToString();

                // Получите связанный объект ПерехБет
                var transitionConcrete = db.ПерехБет.FirstOrDefault(wc => wc.IDПереходной == selectedTransition.IDПереходной);
                if (transitionConcrete != null)
                {
                    // Получите связанный объект Бетоны
                    var concrete = db.Бетоны.FirstOrDefault(c => c.IDБетона == transitionConcrete.IDБетона);
                    if (concrete != null)
                    {
                        TransitionConcreteClassTextBox.Text = concrete.Класс;
                    }
                }

                // Отобразите изображение
                TransitionsDisplayImageFromDatabase();


            }
        }

        // Обработчик события клика мышью на изображении
        private void TransitionImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Создаем новое окно
            Window imageWindow = new Window
            {
                Title = "Увеличенное изображение",
                Width = 800,
                Height = 800
            };

            // Создаем новый Image контрол и устанавливаем его как контент окна
            Image largeImage = new Image
            {
                Source = TransitionImage.Source,
                Stretch = Stretch.Uniform
            };
            imageWindow.Content = largeImage;

            // Открываем окно
            imageWindow.Show();
        }

        private void SaveTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTransition = (Переходные)TransitionsComboBox.SelectedItem;
            if (selectedTransition != null)
            {
                selectedTransition.Наименование = TransitionNameTextBox.Text;
                selectedTransition.Объем = decimal.Parse(TransitionVolumeTextBox.Text);

                // Получите связанный объект СтеныБет
                var transitionConcrete = db.ПерехБет.FirstOrDefault(wc => wc.IDПереходной == selectedTransition.IDПереходной);
                if (transitionConcrete != null)
                {
                    // Получите новый объект Бетоны на основе класса бетона из TextBox
                    var newConcrete = db.Бетоны.FirstOrDefault(c => c.Класс == TransitionConcreteClassTextBox.Text);
                    if (newConcrete != null)
                    {
                        // Обновите IDБетона в связующей таблице
                        transitionConcrete.IDБетона = newConcrete.IDБетона;
                    }
                }

                db.SaveChanges();
            }
        }


        private void SaveImageTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTransition = (Переходные)TransitionsComboBox.SelectedItem;
            if (selectedTransition != null)
            {
                // столбец Изображение в таблице Стены хранит изображения в формате byte[]
                byte[] imageBytes = selectedTransition.Изображение;
                if (imageBytes != null)
                {
                    // Создайте диалог для сохранения файла
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "JPEG Image|*.jpg|Bitmap Image|*.bmp|PNG Image|*.png"; // определение фильтров для типа файлов
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // Сохраните изображение в выбранный файл
                        File.WriteAllBytes(saveFileDialog.FileName, imageBytes);
                    }
                }
            }

        }
        private void ClearTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            TransitionImage.Source = null;
            TransitionsComboBox.SelectedItem = null;
            TransitionNameTextBox.Clear();
            TransitionVolumeTextBox.Clear();
            TransitionConcreteClassTextBox.Clear();
        }


    }
}
