using System;
using System.Windows;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class auth : Window
    {
        public auth()
        {
            InitializeComponent();
        }

        private void auth_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void LogBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {                
                try
                {
                    conn.Open();                   

                }
                catch (MySqlException)
                {
                    MessageBox.Show("Ошибка подключения к БД!");
                }                
                string sql = String.Format("SELECT CONCAT(user_surname, ' ', SUBSTRING(user_name, 1, 1), '.', SUBSTRING(user_patronymic, 1, 1),), user_role FROM user WHERE user_login='{0}' AND user_password='{1}';", LoginBox.Text, PasswordBox.Password);

                string name = string.Empty;
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    try
                    {
                        name = cmd.ExecuteScalar().ToString();
                        MessageBox.Show("Авторизирован пользователь - " + name);
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Ошибка! Неправильный логин или пароль.");
                    }                    
                }
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
