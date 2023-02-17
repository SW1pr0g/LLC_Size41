using System;
using System.ComponentModel;
using System.Windows;

namespace LLC_Size41.window
{
    public partial class main : Window
    {
        public main()
        {
            InitializeComponent();
            LoadUserData();
            checkPrivilegies();
            classes.Variables.mainClosed = false;
        }

        private void exitUser_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы действительно хотите выйти из учётной записи?", "Выход из учетной записи",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                classes.Variables.mainClosed = true;
                new auth().Show();
                Close();
            }                
        }
        private void checkPrivilegies()
        {
            switch (classes.Variables.role)
            {
                case "Менеджер":
                    ShowProduct.Visibility = Visibility.Visible;
                    ProductList.Visibility = Visibility.Visible;
                    OrderList.Visibility = Visibility.Visible;
                    break;
                case "Администратор":
                    ShowProduct.Height = 0;
                    ProductList.Visibility = Visibility.Visible;
                    OrderList.Visibility = Visibility.Visible;
                    SpecialAdds.Visibility = Visibility.Visible;
                    break;
                default:
                    ShowProduct.Visibility = Visibility.Visible;
                    break;
            }

            if (classes.Variables.trashVisible == true)
                TrashBtn.Visibility = Visibility.Visible;
            else
                TrashBtn.Visibility = Visibility.Hidden;
        }
        private void LoadUserData()
        {
            try
            {
                NameLabel.Content += classes.Variables.surname + " " + classes.Variables.name[0] + "." +
                                     classes.Variables.patronymic[0] + ".";
                RoleLabel.Content += classes.Variables.role;
            }
            catch (IndexOutOfRangeException)
            {
                NameLabel.Content += String.Empty;
                RoleLabel.Content += "Гость";
            }
        }

        private void ShowProduct_Click(object sender, RoutedEventArgs e)
        {
            classes.Variables.mainClosed = true;
            new showproduct().Show();
            Close();
        }

        private void ProductList_OnClick(object sender, RoutedEventArgs e)
        {
            classes.Variables.mainClosed = true;
            new product().Show();
            Close();
        }

        private void TrashBtn_OnClick(object sender, RoutedEventArgs e)
        {
            classes.Variables.mainClosed = true;
            new trash().Show();
            Close();
        }

        private void Main_OnClosing(object sender, CancelEventArgs e)
        {
            if (classes.Variables.mainClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                        "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    classes.Variables.mainClosed = true;
            } 
        }

        private void OrderList_OnClick(object sender, RoutedEventArgs e)
        {
            classes.Variables.mainClosed = true;
            new order().Show();
            Close();
        }
    }
}