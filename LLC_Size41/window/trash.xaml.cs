using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
        }

        private void BackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            new main().Show();
            this.Close();
        }

        private void OrderBtn_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Распечатка!");
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

            OrderDate.Content += DateTime.Now.ToString("dd.MM.yyyy");
            OrderDeliveryDate.Content += DateTime.Now.AddDays(3).ToString("dd.MM.yyyy");
            
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
                            OrderNum.Content += (reader.GetInt32(0) + 1).ToString();
                            OrderCode.Content += (reader.GetInt32(1) + 1).ToString();
                        }
                    }
                }
            }
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