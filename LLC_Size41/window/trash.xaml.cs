using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xaml;
using AIS_exchangeOffice.classes;
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
            if (Variables.role != "Гость")
            {
                SurnameBox.IsEnabled = false;
                NameBox.IsEnabled = false;
                PatronymicBox.IsEnabled = false;
            }
            Variables.trashClosed = false;
        }

        private void BackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.trashClosed = true;
            new main().Show();
            Close();
        }

        private void OrderBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (SurnameBox.Text == String.Empty || NameBox.Text == String.Empty || PatronymicBox.Text == String.Empty ||
                PickPointBox.SelectedIndex == -1)
            {
                MessageBox.Show("Ошибка! Не выбран пункт выдачи или не заполнены поля ФИО.");
            }
            else
            {
                if (NameBox.IsEnabled)
                {
                    using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
                    {
                        conn.Open();

                        string passwd = String.Empty;
                        string[] arr = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Z", "b", "c", "d", "f", "g", "h", "j", "k", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z", "A", "E", "U", "Y", "a", "e", "i", "o", "u", "y" };
                        Random rnd = new Random();
                        for (int ii = 0; ii < 7; ii++)
                        {
                            passwd += arr[rnd.Next(0, 57)];
                        }

                        char[] vowels = "aeuoyi".ToCharArray();
                        char[] consonants = "qwrtpsdfghjklzxcvbnm".ToCharArray();


                        StringBuilder newNick = new StringBuilder();

                        while (newNick.Length < 15)
                        {
                            bool firstVowel = rnd.Next(0, 2) == 0;

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
                    string order_id = OrderNum.Content.ToString().Split(' ')[OrderNum.Content.ToString().Split(' ').Length - 1];
                    string order_date = Convert.ToDateTime(OrderDate.Content.ToString().Split(' ')[OrderDate.Content.ToString().Split(' ').Length - 1]).ToString("yyyy-MM-dd");
                    string order_deliverydate = Convert.ToDateTime(OrderDeliveryDate.Content.ToString().Split(' ')[OrderDeliveryDate.Content.ToString().Split(' ').Length - 1]).ToString("yyyy-MM-dd");
                    string order_code = OrderCode.Content.ToString().Split(' ')[OrderCode.Content.ToString().Split(' ').Length - 1];

                    int i = OrderGrid.Items.Count;
                    using (MySqlConnection conn = new MySqlConnection(Variables.ConnStr))
                    {
                        conn.Open();

                        string sqlOrder = String.Format(@"INSERT INTO `order` (order_id, order_date, order_deliverydate, order_pickpointID, order_userID, order_code, order_status)
                                                            VALUES ({0}, '{1}', '{2}', (SELECT pickpoint_id FROM pickpoint WHERE pickpoint_address = '{3}'), 
                                                            (SELECT user_id FROM user WHERE user_surname = '{4}'), {5}, '{6}');", order_id,
                                                            order_date, order_deliverydate, PickPointBox.Text, SurnameBox.Text, order_code
                                                            , "Новый");
                        using (MySqlCommand cmd = new MySqlCommand(sqlOrder, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        for (i = 0; i < OrderGrid.Items.Count; i++)
                        {
                            string for_article = ((DataRowView)OrderGrid.Items[i]).Row[0].ToString();
                            string for_productcount = ((DataRowView)OrderGrid.Items[i]).Row[5].ToString();
                            string sqlOrderProduct = String.Format(@"INSERT INTO `orderproduct` (order_id, product_articlenumber, product_count)
                                                            VALUES ({0}, '{1}', {2});", order_id, for_article, for_productcount);
                            using (MySqlCommand cmd = new MySqlCommand(sqlOrderProduct, conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                else
                {
                    string order_id = OrderNum.Content.ToString().Split(' ')[OrderNum.Content.ToString().Split(' ').Length - 1];
                    
                    string order_date = DateTime.ParseExact(
                            OrderDate.Content.ToString().Split(' ')[OrderDate.Content.ToString().Split(' ').Length - 1],
                            "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture)
                        .ToString("yyyy-MM-dd");
                    string order_deliverydate = DateTime.ParseExact(
                            OrderDeliveryDate.Content.ToString().Split(' ')[OrderDeliveryDate.Content.ToString().Split(' ').Length - 1],
                            "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture)
                        .ToString("yyyy-MM-dd");
                    string order_code = OrderCode.Content.ToString().Split(' ')[OrderCode.Content.ToString().Split(' ').Length - 1];

                    using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
                    {
                        conn.Open();

                        string sql = String.Format(@"INSERT INTO `order` (order_id, order_date, order_deliverydate, order_pickpointID, order_userID, order_code, order_status)
                                                            VALUES ({0}, '{1}', '{2}', (SELECT pickpoint_id FROM pickpoint WHERE pickpoint_address = '{3}'), 
                                                            (SELECT user_id FROM user WHERE user_surname = '{4}'), {5}, '{6}');", order_id,
                                                            order_date, order_deliverydate, PickPointBox.Text, SurnameBox.Text, order_code
                                                            , "Новый");
                        using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        string[] data = new string[OrderGrid.Items.Count];

                        for (int i = 0; i < OrderGrid.Items.Count; i++)
                        {
                            data[i] = ((DataRowView)OrderGrid.Items[i]).Row[1].ToString() + " " + ((DataRowView)OrderGrid.Items[0]).Row[5].ToString() +
                                      " " + ((DataRowView)OrderGrid.Items[i]).Row[2].ToString() + " " +
                                      (Convert.ToDouble(((DataRowView)OrderGrid.Items[i]).Row[2]) *
                                       Convert.ToInt32(((DataRowView)OrderGrid.Items[i]).Row[5]) *
                                       (Convert.ToDouble(((DataRowView)OrderGrid.Items[i]).Row[3]) / 100)).ToString() +
                                      " " + (Convert.ToDouble(((DataRowView)OrderGrid.Items[i]).Row[2]) *
                                             Convert.ToInt32(((DataRowView)OrderGrid.Items[i]).Row[5]) -
                                             (Convert.ToDouble(((DataRowView)OrderGrid.Items[i]).Row[2]) *
                                              Convert.ToInt32(((DataRowView)OrderGrid.Items[i]).Row[5]) *
                                              (Convert.ToDouble(((DataRowView)OrderGrid.Items[i]).Row[3]) / 100)))
                                      .ToString(); 
                            string for_article = ((DataRowView)OrderGrid.Items[i]).Row[0].ToString();
                            string for_productcount = ((DataRowView)OrderGrid.Items[i]).Row[5].ToString();
                            sql = String.Format(@"INSERT INTO `orderproduct` (order_id, product_articlenumber, product_count)
                                                            VALUES ({0}, '{1}', {2});", order_id, for_article, for_productcount);
                            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                            { 
                                cmd.ExecuteNonQuery();
                            }
                        }
                        Variables.trashVisible = false;
                        Variables.trash.Clear();
                        BackBtn_OnClick(null, null);
                        
                        order_print.Start(Environment.CurrentDirectory + "\\exampleWord\\order_print.docx", data,
                            order_id,
                            classes.Variables.surname + " " + classes.Variables.name[0] + "." +
                            classes.Variables.patronymic[0] + ".", order_date, order_deliverydate,
                            FullPrice.Content.ToString().Split(' ')[FullPrice.Content.ToString().Split(' ').Length - 1],
                            PickPointBox.Text);
                    }
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

            SurnameBox.Text = Variables.surname;
            NameBox.Text = Variables.name;
            PatronymicBox.Text = Variables.patronymic;

            DataTable dt = ToDataTable(Variables.trash);
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
                fullprice += (Convert.ToDouble(Variables.trash[i].Price) *
                              Convert.ToDouble(Variables.trash[i].Count)) -
                             ((Convert.ToDouble(Variables.trash[i].Price) *
                               Convert.ToDouble(Variables.trash[i].Count)) *
                              (Convert.ToDouble(Variables.trash[i].Discount) / 100));
            }

            FullPrice.Content = "Полная сумма: " +  fullprice;
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
            OrderGrid.Columns[1].Header = "Наименование";
            OrderGrid.Columns[2].Header = "Стоимость";
            OrderGrid.Columns[3].Header = "Скидка";
            OrderGrid.Columns[4].Header = "Производитель";
            OrderGrid.Columns[5].Header = "Количество";
        }

        private void OrderGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Variables.ConnStr))
            {
                conn.Open();
                string sql = String.Format("SELECT product_article, product_name, product_photoname FROM product WHERE product_article = '{0}';", ((DataRowView)OrderGrid.SelectedItems[0]).Row[0]);
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            article_current = ((DataRowView)OrderGrid.SelectedItems[0]).Row[0].ToString();
                            NameProductBox.Text = reader.GetString(1);
                            PriceBox.Text = ((DataRowView)OrderGrid.SelectedItems[0]).Row[2].ToString();
                            DiscountBox.Text = ((DataRowView)OrderGrid.SelectedItems[0]).Row[3].ToString();
                            ManufacturerBox.Text = ((DataRowView)OrderGrid.SelectedItems[0]).Row[4].ToString();
                            var uriSource = new Uri(Directory.GetCurrentDirectory() + "\\images\\product\\" + reader.GetString(2), UriKind.Absolute);
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

        private void Trash_OnClosing(object sender, CancelEventArgs e)
        {
            if (classes.Variables.trashClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                        "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    Variables.trashClosed = true;
            } 
        }
    }
}