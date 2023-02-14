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
            classes.Variables.authClosed = true;
        }

        private void auth_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (classes.Variables.authClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
            }            
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
                string sql = String.Format("SELECT CONCAT(user_surname, ' ', SUBSTRING(user_name, 1, 1), '.', SUBSTRING(user_patronymic, 1, 1), '.'), (SELECT role.role_name from role WHERE role.role_id = user.user_role) FROM user WHERE user_login='{0}' AND user_password='{1}';", LoginBox.Text, PasswordBox.Password);

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                classes.Variables.authClosed = true;
                                new main(reader.GetString(0), reader.GetString(1)).Show();
                                this.Close();                                
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка! Неправильный логин или пароль.");
                        }
                    }                   
                }
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            classes.Variables.authClosed = true;
            new main(String.Empty, "Гость").Show();
            this.Close(); 
        }
    }
}
