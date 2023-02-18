using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using AIS_exchangeOffice.classes;
using LLC_Size41.classes;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class special : Window
    {
        public special()
        {
            InitializeComponent();
            Variables.specialClosed = true;
        }

        private void Special_OnClosing(object sender, CancelEventArgs e)
        {
            if (Variables.specialClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                        "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    Variables.specialClosed = true;
            } 
        }

        private void BackBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.specialClosed = true;
            new main().Show();
            Close();
        }

        private void CreateOtchet_OnClick(object sender, RoutedEventArgs e)
        {
            if (DateStart.Text == String.Empty || DateEnd.Text == String.Empty)
                MessageBox.Show("Даты не выбраны!");
            else if (DateStart.DisplayDate > DateEnd.DisplayDate)
                MessageBox.Show("Ошибка! Начальная дата должна быть больше конечной даты.");
            else
            {
                using (MySqlConnection conn = new MySqlConnection(Variables.ConnStr))
                {
                    conn.Open();
                    string sql = String.Format(@"SELECT order_id, order_date, order_deliverydate FROM `order` 
                                                WHERE order_status = 'Завершён' AND order_deliverydate > '{0}' AND order_deliverydate < '{1}';",
                        DateStart.DisplayDate.ToString("yyyy-MM-dd"), DateEnd.DisplayDate.ToString("yyyy-MM-dd"));
                    
                    string[] data;
                    int count = 0;
            
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            count = dt.Rows.Count;
                            data = new string[count];
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                int i = 0;
                                while (reader.Read())
                                {
                                    data[i] = reader.GetString(0) + " " +
                                              reader.GetString(1).Substring(0, 9).Replace('/', '.') + " " +
                                              reader.GetString(2).Substring(0, 9).Replace('/', '.');
                                    i++;
                                }
                            }
                            else
                            {
                                MessageBox.Show("По указанному промежутку времени нет заказов!");
                                return;
                            }
                        }
                    }

                    otchet_print.Start(Environment.CurrentDirectory + "\\exampleWord\\otchet_print.docx", data,
                        DateStart.DisplayDate.ToString("dd.MM.yyyy"),
                        DateEnd.DisplayDate.ToString("dd.MM.yyyy"), count.ToString(), Variables.surname + " " +
                        Variables.name[0] + "." +
                        Variables.patronymic[0] + ".");
                }
            }
        }
    }
}