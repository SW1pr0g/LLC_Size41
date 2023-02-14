using System.Windows;

namespace LLC_Size41.window
{
    public partial class main : Window
    {
        string role, name;
        public main(string name, string role)
        {
            InitializeComponent();
            LoadUserData(name, role);
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
            switch (role)
            {
                case "Менеджер":
                    ShowProduct.Visibility = Visibility.Visible;
                    MakeOrder.Visibility = Visibility.Visible;
                    ProductList.Visibility = Visibility.Visible;
                    break;
                case "Администратор":
                    ShowProduct.Visibility = Visibility.Visible;
                    MakeOrder.Visibility = Visibility.Visible;
                    ProductList.Visibility = Visibility.Visible;
                    UsersList.Visibility = Visibility.Visible;
                    break;
                default:
                    ShowProduct.Visibility = Visibility.Visible;
                    MakeOrder.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void LoadUserData(string nameStr, string roleStr)
        {
            name = nameStr;
            role = roleStr;
            NameLabel.Content += name;
            RoleLabel.Content += role;
        }
    }
}