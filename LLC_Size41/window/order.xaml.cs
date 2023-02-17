using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LLC_Size41.classes;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class order : Window
    {
        private string edit_order_id = String.Empty;
        public order()
        {
            InitializeComponent();
            Variables.orderClosed = false;
            FillData();
        }

        private void Order_OnClosing(object sender, CancelEventArgs e)
        {
            if (Variables.orderClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                        "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    Variables.orderClosed = true;
            } 
        }
        private void FillData()
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = @"SELECT order_id,                                
		                            (SELECT CONCAT(user.user_surname, ' ', SUBSTRING(user.user_name, 1, 1), '.', 
		                            SUBSTRING(user.user_patronymic, 1, 1), '.') FROM user WHERE user.user_id = `order`.order_userID),                                    
                                    order_date,                                     
                                    order_deliverydate,                                     
                                    order_status                                     
                                FROM `order` ";
                
                switch (SortBox.Text)
                {
                    case "Статус заказа(сначала завершённые)":
                        sql += "ORDER BY order_status ASC";
                        break;
                    case "Статус заказа(сначала новые)":
                        sql += "ORDER BY order_status DESC";
                        break;
                }

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    List<OrderGridItem> orderList = new List<OrderGridItem>();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderList.Add(new OrderGridItem
                            {
                                OrderNum = reader.GetString(0), OrderFIO = reader.GetString(1),
                                OrderDate = reader.GetDateTime(2).ToString("dd.MM.yyyy"),
                                OrderDeliveryDate = reader.GetDateTime(3).ToString("dd.MM.yyyy"),
                                OrderStatus = reader.GetString(4)
                            });
                        }
                    }
                    ordersGrid.ItemsSource = orderList;
                    CountGrid.Content = "Количество записей: " + ordersGrid.Items.Count;
                }
            }
        }

        private void OrdersGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            ordersGrid.Columns[0].Header = "Номер заказа";
            ordersGrid.Columns[1].Header = "ФИО клиента";
            ordersGrid.Columns[2].Header = "Дата заказа";
            ordersGrid.Columns[3].Header = "Дата доставки";
            ordersGrid.Columns[4].Header = "Статус";
        }

        private void SortBoxItem_OnLostFocus(object sender, RoutedEventArgs e)
        {
            FillData();
        }

        private void OrdersGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditOrderBtn.IsEnabled = true;
            edit_order_id = (ordersGrid.Columns[0].GetCellContent(ordersGrid.SelectedItems[0]) as TextBlock).Text;
            var date = ordersGrid.Columns[3].GetCellContent(ordersGrid.SelectedItems[0]) as TextBlock;
            DateTime datetime =
                DateTime.ParseExact(
                    (ordersGrid.Columns[3].GetCellContent(ordersGrid.SelectedItems[0]) as TextBlock).Text, "dd.MM.yyyy",
                    null);
            DeliveryDateBox.SelectedDate = datetime;
            DeliveryDateBox.DisplayDateStart = datetime.AddDays(3);
            DeliveryDateBox.DisplayDateEnd = datetime.AddDays(9);

            switch ((ordersGrid.Columns[4].GetCellContent(ordersGrid.SelectedItems[0]) as TextBlock).Text)
            {
                case "Новый":
                    StatusBox.SelectedIndex = 0;
                    break;
                case "Завершен":
                    StatusBox.SelectedIndex = 1;
                    break;
            }
        }

        private void EditOrderBtn_OnClick(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Variables.ConnStr))
            {
                conn.Open();
                string sql =
                    String.Format(
                        "UPDATE `order` SET order_deliverydate = '{0}', order_status = '{1}' WHERE order_id = {2};",
                        DeliveryDateBox.DisplayDate.ToString("yyyy-MM-dd"), StatusBox.Text, edit_order_id); 
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Изменения внесены!");
                    FillData();
                }
            }
        }

        private void BackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.orderClosed = true;
            new main().Show();
            Close();
        }
    }
}