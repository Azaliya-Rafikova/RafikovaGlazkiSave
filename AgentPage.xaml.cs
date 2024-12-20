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
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List <Agent> CurrentPageList = new List <Agent> ();
        List<Agent> TableList;

        public AgentPage()
        {
            InitializeComponent();

            var currentAgents = RafikovaGlazkiSaveEntities.GetContext().Agent.ToList();

            AgentListView.ItemsSource = currentAgents;

            ComboType.SelectedIndex = 0;
            ComboType2.SelectedIndex = 0;
            UpdateAgents();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void ComboType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void UpdateAgents()
        {
            var currentAgents = RafikovaGlazkiSaveEntities.GetContext().Agent.ToList();
            var currentPhoneEmailAgents = RafikovaGlazkiSaveEntities.GetContext().Agent.ToList();

            //var AgentType = RafikovaGlazkiSaveEntities.GetContext().AgentType.ToList();

            if (ComboType2.SelectedIndex == 1)
            {
                currentAgents = currentAgents.OrderBy(p => p.Title).ToList();
            }
            if (ComboType2.SelectedIndex == 2)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList();
            }
            if (ComboType2.SelectedIndex == 3)
            {
                currentAgents = currentAgents.OrderBy(p => p.Discount).ToList();
            }
            if (ComboType2.SelectedIndex == 4)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Discount).ToList();
            }
            if (ComboType2.SelectedIndex == 5)
            {
                currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
            }
            if (ComboType2.SelectedIndex == 6)
            {
                currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
            }

            if (ComboType.SelectedIndex == 0)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("МФО") || p.AgentType.Title.Contains("ООО") || p.AgentType.Title.Contains("ЗАО") || p.AgentType.Title.Contains("МКК") || p.AgentType.Title.Contains("ОАО") || p.AgentType.Title.Contains("ПАО"))).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("МФО"))).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ООО"))).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ЗАО"))).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("МКК"))).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ОАО"))).ToList();
            }
            if (ComboType.SelectedIndex == 6)
            {
                currentAgents = currentAgents.Where(p => (p.AgentType.Title.Contains("ПАО"))).ToList();
            }

            currentPhoneEmailAgents = currentAgents.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if(currentPhoneEmailAgents.Count == 0)
            {
                currentPhoneEmailAgents = currentAgents.Where(p => p.Phone.ToLower().Replace(" ", "").Replace("+","").Replace(" (", "").Replace(")", "").Replace("-", "").Contains(TBoxSearch.Text.ToLower())).ToList();
            }

            if (currentPhoneEmailAgents.Count == 0)
            {
                currentPhoneEmailAgents = currentAgents.Where(p => p.Email.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            }

            if (currentPhoneEmailAgents.Count == 0)
            {
                currentPhoneEmailAgents = currentAgents.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            }

            currentAgents = currentPhoneEmailAgents;

            AgentListView.ItemsSource = currentAgents.ToList();

            TableList = currentAgents;

            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;

                    case 2:
                        if (CurrentPage < CountPage - 1)
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }
            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;

                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                AgentListView.ItemsSource = CurrentPageList;
                AgentListView.Items.Refresh();
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString())-1);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                RafikovaGlazkiSaveEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AgentListView.ItemsSource = RafikovaGlazkiSaveEntities.GetContext().Agent.ToList();
                UpdateAgents();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void AgentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AgentListView.SelectedItems.Count > 1)
                ChangePriorityButton.Visibility = Visibility.Visible;
            else
                ChangePriorityButton.Visibility = Visibility.Hidden;
        }

        private void ChangePriorityButton_Click(object sender, RoutedEventArgs e)
        {
            int maxPriority = 0;
            foreach(Agent agent in AgentListView.SelectedItems)
            {
                if (agent.Priority > maxPriority)
                    maxPriority = agent.Priority;
            }
            SetWindow myWindow = new SetWindow(maxPriority);
            myWindow.ShowDialog();
            if (string.IsNullOrEmpty(myWindow.TBPriority.Text))
                MessageBox.Show("Изменения не произошли");
            else
            {
                int newPriority=Convert.ToInt32(myWindow.TBPriority.Text);
                foreach(Agent agent in AgentListView.SelectedItems)
                {
                    agent.Priority = newPriority;
                }
                try
                {
                    RafikovaGlazkiSaveEntities.GetContext().SaveChanges();
                    MessageBox.Show("информация сохранена");
                    UpdateAgents();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        
    }
}
