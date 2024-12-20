using Microsoft.Win32;
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

namespace RafikovaGlazkiSave
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgents = new Agent();

        private List<Product> Items;
        private ProductSale currentProductSale = new ProductSale();

        public AddEditPage(Agent SelectedAgents)
        {
            InitializeComponent();

            if (SelectedAgents != null)
                _currentAgents = SelectedAgents;

            var allProducts = RafikovaGlazkiSaveEntities.GetContext().Product.ToList();

            var currentProductSales = RafikovaGlazkiSaveEntities.GetContext().ProductSale.ToList();
            currentProductSales = currentProductSales.Where(p => p.AgentID == _currentAgents.ID).ToList();

            History.ItemsSource = currentProductSales;

            DataContext = currentProductSale;


            DataContext = _currentAgents;

            LoadItems();
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog()==true)
            {
                _currentAgents.Logo=myOpenFileDialog.FileName;

                LogoImage.Source=new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentAgents.Title))
                errors.AppendLine("Укажите наименование агента");
            if (string.IsNullOrWhiteSpace(_currentAgents.Address))
                errors.AppendLine("Укажите адрес агента");
            if (string.IsNullOrWhiteSpace(_currentAgents.DirectorName))
                errors.AppendLine("Укажите ФИО директора");
            if (ComboType.SelectedItem==null)
                errors.AppendLine("Укажите тип агента");
            else
            {
                _currentAgents.AgentTypeID = ComboType.SelectedIndex + 1;
            }
            if (string.IsNullOrWhiteSpace(_currentAgents.Priority.ToString()))
                errors.AppendLine("Укажите приоритет агента");
            if (_currentAgents.Priority<=0)
                errors.AppendLine("Укажите положительный приоритет агента");
            if (string.IsNullOrWhiteSpace(_currentAgents.INN))
                errors.AppendLine("Укажите ИНН агента");
            if (string.IsNullOrWhiteSpace(_currentAgents.KPP))
                errors.AppendLine("Укажите КПП агента");
            if (string.IsNullOrWhiteSpace(_currentAgents.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgents.Phone.Replace("+", "").Replace("(", " ").Replace("-", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
            }
            if (string.IsNullOrWhiteSpace(_currentAgents.Email))
                errors.AppendLine("Укажите почту агента");
            if (errors.Length>0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentAgents.ID==0)
                RafikovaGlazkiSaveEntities.GetContext().Agent.Add(_currentAgents);
            try
            {
                RafikovaGlazkiSaveEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var _currentAgent = (sender as Button).DataContext as Agent;

            var currentProductSale = RafikovaGlazkiSaveEntities.GetContext().ProductSale.ToList();
            currentProductSale = currentProductSale.Where(p => p.AgentID == _currentAgent.ID).ToList();

            if (currentProductSale.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существует история реализации продуктов");
            else
            {
                var currentAgentPriorityHistory = RafikovaGlazkiSaveEntities.GetContext().AgentPriorityHistory.ToList();
                var currentShop = RafikovaGlazkiSaveEntities.GetContext().Shop.ToList();
                currentAgentPriorityHistory = currentAgentPriorityHistory.Where(p => p.AgentID == _currentAgent.ID).ToList();
                currentShop = currentShop.Where(p => p.AgentID == _currentAgent.ID).ToList();


                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        RafikovaGlazkiSaveEntities.GetContext().Agent.Remove(_currentAgent);

                        if (currentAgentPriorityHistory.Count != 0)
                        {
                            for (int i = 0; currentAgentPriorityHistory.Count == i; i++)
                                RafikovaGlazkiSaveEntities.GetContext().AgentPriorityHistory.Remove(currentAgentPriorityHistory[i]);
                        }
                        if (currentShop.Count != 0)
                        {
                            for (int i = 0; currentShop.Count == i; i++)
                                RafikovaGlazkiSaveEntities.GetContext().Shop.Remove(currentShop[i]);
                        }
                        RafikovaGlazkiSaveEntities.GetContext().SaveChanges();

                        MessageBox.Show("Информация удалена!");
                        Manager.MainFrame.GoBack();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void LoadItems()
        {
            // Здесь вы загружаете элементы из базы данных
            Items = RafikovaGlazkiSaveEntities.GetContext().Product.ToList();
            ComboTitle.ItemsSource = Items; // Устанавливаем источник данных для ComboBox
        }

        private void ComboTitle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboTitle.SelectedItem is Product selectedProduct)
            {
                // Подставляем значение Title в TextBox
                SearchBoxForComboBox.Text = selectedProduct.Title; // Убедитесь, что у вас есть TextBox с именем SearchBoxForComboBox
            }
        }

        private void AddHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ComboTitle.SelectedItem as Product;

            // Получаем количество из TextBox (например, для количества продукта)
            int productCount;
            if (!int.TryParse(CountProductTB.Text, out productCount) || productCount <= 0)
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (selectedProduct != null)
            {
                // Создаем новый объект ProductSale
                var newSale = new ProductSale
                {
                    AgentID = _currentAgents.ID, // Указываем ID агента
                    ProductID = selectedProduct.ID, // Указываем ID выбранного продукта
                    SaleDate = StartDate.SelectedDate ?? DateTime.Now, // Указываем дату продажи (если выбрана)
                    ProductCount = productCount // Указываем количество, введенное пользователем
                };

                // Добавляем новый объект в контекст и сохраняем изменения
                RafikovaGlazkiSaveEntities.GetContext().ProductSale.Add(newSale);
                RafikovaGlazkiSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("информация сохранена");

                // Обновляем список продаж
                LoadProductSalesForCurrentAgent();

                // Очистка полей ввода (по желанию)
                ComboTitle.SelectedItem = null;
                CountProductTB.Clear();
                StartDate.SelectedDate = null;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для добавления.");
            }
        }

        private void LoadProductSalesForCurrentAgent()
        {
            var currentProductSales = RafikovaGlazkiSaveEntities.GetContext().ProductSale
                .Where(ps => ps.AgentID == _currentAgents.ID) // Предполагается, что у ProductSale есть поле AgentID
                .ToList();

            // Устанавливаем источник данных для списка продаж
            History.ItemsSource = currentProductSales;
        }


        private void DelHistory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = History.SelectedItems.Cast<ProductSale>().ToList();

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите хотя бы один элемент для удаления.");
                return;
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление? ", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = RafikovaGlazkiSaveEntities.GetContext();

                        // Удаляем каждый выбранный элемент из контекста
                        foreach (var item in selectedItems)
                        {
                            context.ProductSale.Remove(item);
                        }

                        // Сохраняем изменения в базе данных
                        context.SaveChanges();

                        LoadProductSalesForCurrentAgent();

                        MessageBox.Show("Элементы успешно удалены.");
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }

                }
            }
        }
        private void SearchBoxForComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBoxForComboBox.Text.ToLower();
            var filteredItems = Items.Where(p => p.Title.ToLower().Contains(searchText)).ToList();
            ComboTitle.ItemsSource = filteredItems;

            // Если ничего не найдено, показываем все элементы
            if (string.IsNullOrEmpty(searchText))
            {
                ComboTitle.ItemsSource = Items;
            }
        }
    }
}
