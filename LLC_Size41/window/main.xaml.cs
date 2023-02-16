using System;
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
            
        }

        private void exitUser_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Вы действительно хотите выйти из учётной записи?", "Выход из учетной записи",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                new auth().Show();
                this.Close();
            }                
        }
        private void checkPrivilegies()
        {
            switch (classes.Variables.role)
            {
                case "Менеджер":
                    ShowProduct.Visibility = Visibility.Visible;
                    ProductList.Visibility = Visibility.Visible;
                    break;
                case "Администратор":
                    ProductList.Visibility = Visibility.Visible;
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
            new showproduct().Show();
            this.Close();
        }

        private void ProductList_OnClick(object sender, RoutedEventArgs e)
        {
            new product().Show();
            this.Close();
        }

        private void TrashBtn_OnClick(object sender, RoutedEventArgs e)
        {
            new trash().Show();
            this.Close();
        }
    }
}