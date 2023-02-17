using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xaml;
using LLC_Size41.classes;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class trash : Window
    {
        private string article_current = String.Empty;
        public trash()
        {
            InitializeComponent();
            LoadData();
            if (classes.Variables.role != "Гость")
            {
                SurnameBox.IsEnabled = false;
                NameBox.IsEnabled = false;
                PatronymicBox.IsEnabled = false;
            }
        }

        private void BackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            new main().Show();
            this.Close();
        }

        private void OrderBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (SurnameBox.Text == String.Empty || NameBox.Text == String.Empty || PatronymicBox.Text == String.Empty ||
                PickPointBox.SelectedIndex == -1)
            {
                string str = classes.Variables.role;
                MessageBox.Show("Ошибка! Не выбран пункт выдачи или не заполнены поля ФИО.");
                return;
            }
            else
            {
                if (NameBox.IsEnabled == true)
                {
                    using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
                    {
                        conn.Open();
                        
                        string passwd = String.Empty;
                        string[] arr = { "1","2","3","4","5","6","7","8","9","B","C","D","F","G","H","J","K","L","M","N","P","Q","R","S","T","V","W","X","Z","b","c","d","f","g","h","j","k","m","n","p","q","r","s","t","v","w","x","z","A","E","U","Y","a","e","i","o","u","y" };
                        Random rnd = new Random();
                        for (int i = 0; i < 7; i ++)
                        {
                            passwd += arr[rnd.Next(0, 57)];
                        }
                        
                        char[] vowels = "aeuoyi".ToCharArray();
                        char[] consonants = "qwrtpsdfghjklzxcvbnm".ToCharArray();

                        
                        StringBuilder newNick = new StringBuilder();
                        
                        while (newNick.Length < 15)
                        {
                            bool firstVowel = rnd.Next(0, 2) == 0 ? true : false;

                            if (firstVowel)
                            {
                                newNick.Append(vowels[rnd.Next(0, vowels.Length)]);
                                newNick.Append(consonants[rnd.Next(0, consonants.Length)]);
                            }
                            else
                            {
                                newNick.Append(consonants[rnd.Next(0, consonants.Length)]);
                                newNick.Append(vowels[rnd.Next(0, vowels.Length)]);
                            }
                        }
                        if (15 % 2 != 0) newNick.Remove(newNick.Length - 1, 1);

                        newNick[0] = char.ToUpper(newNick[0]);

                        string sql = String.Format(@"INSERT INTO user (user_surname, user_name, user_patronymic,
                                            user_login, user_password, user_role) VALUES
                                                ('{0}', '{1}', '{2}', '{3}', '{4}', 'Клиент')", SurnameBox.Text, NameBox.Text, PatronymicBox.Text, newNick, passwd);
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
                    {
                        conn.Open();
                        string sql = String.Format(@"INSERT INTO order");
                        using (MySqlCommand)
                    }
                    string[] data = new string[Convert.ToInt32(command.ExecuteScalar())];

                    //сохраняем отчёт
                    wordOtchets_print.otchetClients_print(Environment.CurrentDirectory + "\\wordDocs\\otchetClients_print.docx", data, NameAdmin.Text);
                    commandString = "UPDATE otchets_quantity SET quantity = " + (Convert.ToInt32(otchets_quantity.Text) + 1).ToString() + " WHERE id = 1";
                    command.CommandText = commandString;
                    command.ExecuteReader();
                    command.Connection.Close();
                }
            }
        }

        private void LoadData()
        {
            #region LoadPickup
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = "SELECT pickpoint_address FROM pickpoint;";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PickPointBox.Items.Add(reader.GetString(0));
                        }
                    }
                }
            }
            #endregion

            SurnameBox.Text = classes.Variables.surname;
            NameBox.Text = classes.Variables.name;
            PatronymicBox.Text = classes.Variables.patronymic;

            DataTable dt = ToDataTable(classes.Variables.trash);
            OrderGrid.ItemsSource = dt.DefaultView;

            OrderDate.Content = "Дата: " + DateTime.Now.ToString("dd.MM.yyyy");
            OrderDeliveryDate.Content = "Дата доставки: " + DateTime.Now.AddDays(3).ToString("dd.MM.yyyy");
            
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = "SELECT order_id, order_code FROM `order` WHERE order_id=(SELECT MAX(order_id) FROM `order`);";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrderNum.Content = "Номер заказа: " + (reader.GetInt32(0) + 1).ToString();
                            OrderCode.Content = "Код: " + (reader.GetInt32(1) + 1).ToString();
                        }
                    }
                }
            }

            double fullprice = 0.0;
            for (int i = 0; i < classes.Variables.trash.Count(); i++)
            {
                fullprice += (Convert.ToDouble(classes.Variables.trash[i].Price) *
                              Convert.ToDouble(classes.Variables.trash[i].Count)) -
                             ((Convert.ToDouble(classes.Variables.trash[i].Price) *
                               Convert.ToDouble(classes.Variables.trash[i].Count)) *
                              (Convert.ToDouble(classes.Variables.trash[i].Discount) / 100));
            }

            FullPrice.Content = "Полная сумма: " +  fullprice.ToString();
        }

        private void DeleteFromTrash_OnClick(object sender, RoutedEventArgs e)
        {
            OrderGrid_OnMouseLeftButtonUp(null, null);
            
            var a = Variables.trash.Find(x => x.ArticleNum == article_current);
            classes.Variables.trash.Remove(a);
            LoadData();
            if (Variables.trash.Count == 0)
                Variables.trashVisible = false;
            MessageBox.Show("Указанные позиции удалены из корзины!");
        }

        private void OrderGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            OrderGrid.Columns[0].Header = "Артикул";
            OrderGrid.Columns[1].Header = "Стоимость";
            OrderGrid.Columns[2].Header = "Скидка";
            OrderGrid.Columns[3].Header = "Производитель";
            OrderGrid.Columns[4].Header = "Количество";
        }

        private void OrderGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = String.Format("SELECT product_name, product_photoname FROM product WHERE product_article = '{0}';", ((DataRowView)OrderGrid.SelectedItems[0]).Row[0]);
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            article_current = ((DataRowView)OrderGrid.SelectedItems[0]).Row[0].ToString();
                            NameProductBox.Text = reader.GetString(0);
                            PriceBox.Text = ((DataRowView)OrderGrid.SelectedItems[0]).Row[1].ToString();
                            DiscountBox.Text = ((DataRowView)OrderGrid.SelectedItems[0]).Row[2].ToString();
                            ManufacturerBox.Text = ((DataRowView)OrderGrid.SelectedItems[0]).Row[3].ToString();
                            var uriSource = new Uri(Directory.GetCurrentDirectory() + "\\images\\product\\" + reader.GetString(1), UriKind.Absolute);
                            ProductImage.Source = new BitmapImage(uriSource);
                        }
                    }
                }
            }
        }
        
        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}