using System;
using System.Windows;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Threading;

namespace LLC_Size41.window
{
    public partial class auth : Window
    {
        bool _capthaEnabled = false;
        string capthaValue = String.Empty;
        public auth()
        {
            InitializeComponent();
            classes.Variables.authClosed = false;
            classes.Variables.trash.Clear();
            classes.Variables.surname = String.Empty;
            classes.Variables.name = String.Empty;
            classes.Variables.patronymic = String.Empty;
            classes.Variables.trashVisible = false;
        }

        private void auth_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (classes.Variables.authClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                        "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    classes.Variables.authClosed = true;
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
                string sql = String.Format("SELECT user_surname, user_name, user_patronymic, user_role FROM user WHERE user_login='{0}' AND user_password='{1}';", LoginBox.Text, PasswordBox.Password);

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (_capthaEnabled == true && CapthaText.Text != capthaValue)
                        {
                            MessageBox.Show("Ошибка! Неверный ввод проверки CAPTHA. Система заблокирована на 10 секунд.", "Ошибка ввода CAPTHA", 
                                MessageBoxButton.OK, MessageBoxImage.Stop);
                            Thread.Sleep(10000);
                            PutCapthaText();
                            CapthaText.Clear();
                            return;
                        }
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                classes.Variables.authClosed = true;
                                classes.Variables.surname = reader.GetString(0);
                                classes.Variables.name = reader.GetString(1);
                                classes.Variables.patronymic = reader.GetString(2);
                                classes.Variables.role = reader.GetString(3);
                                classes.Variables.authClosed = true;
                                new main().Show();
                                Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Ошибка! Неправильный логин или пароль.");
                            Captha.Visibility = Visibility.Visible;
                            Height = 600;
                            _capthaEnabled = true;
                            PutCapthaText();
                        }
                                                
                    }                   
                }
            }
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            classes.Variables.authClosed = true;
            classes.Variables.name = String.Empty;
            classes.Variables.role = "Гость";
            classes.Variables.authClosed = true;
            new main().Show();
            Close();
        }
        private void PutCapthaText()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            int value = rnd.Next(1, 5);
            switch (value)
            {
                case 1:
                    var uriSource = new Uri(@"/LLC_Size41;component/images/product/captha1.png", UriKind.Relative);
                    CapthaImg.Source = new BitmapImage(uriSource);
                    capthaValue = "4K[j";
                    break;
                case 2:
                    var uriSource1 = new Uri(@"/LLC_Size41;component/images/captha/captha2.png", UriKind.Relative);
                    CapthaImg.Source = new BitmapImage(uriSource1);
                    capthaValue = "im4/";
                    break;
                case 3:
                    var uriSource2 = new Uri(@"/LLC_Size41;component/images/captha/captha3.png", UriKind.Relative);
                    CapthaImg.Source = new BitmapImage(uriSource2);
                    capthaValue = "][qc";
                    break;
                case 4:
                    var uriSource3 = new Uri(@"/LLC_Size41;component/images/captha/captha4.png", UriKind.Relative);
                    CapthaImg.Source = new BitmapImage(uriSource3);
                    capthaValue = "rkp(";
                    break;
                default:
                    var uriSource4 = new Uri(@"/LLC_Size41;component/images/captha/captha1.png", UriKind.Relative);
                    CapthaImg.Source = new BitmapImage(uriSource4);
                    capthaValue = "4K[j";
                    break;
            }
        }
    }
}
