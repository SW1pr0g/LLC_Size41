using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class showproduct : Window
    {
        public showproduct()
        {
            InitializeComponent();
            FillData();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {            
            new main().Show();
            this.Close();
        }
        private void FillData()
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = String.Format(@"SELECT product_article AS 'Артикул',                                
		                                product_name AS 'Наименование',                                     
		                                product_desc AS 'Описание',                                     
		                                (SELECT category.category_name FROM category WHERE category.category_id = product.product_categoryID) AS 'Категория',                                     
		                                (SELECT supplier.supplier_name FROM supplier WHERE supplier.supplier_id = product.product_supplierID) AS 'Поставщик',                                     
		                                (SELECT manufacturer.manufacturer_name FROM manufacturer WHERE manufacturer.manufacturer_id = product.product_manufacturerID) AS 'Производитель',                                     
		                                product_photoname AS 'Фотография',                                     
		                                product_cost AS 'Стоимость',                                     
		                                product_discount AS 'Скидка',                                     
		                                product_quantityinstock AS 'Количество на складе'                                     
		                        FROM product WHERE product_name LIKE '%{0}%'", SearchBox.Text);

                switch (FilterBox.Text)
                {
                    case "0 - 9,99%":
                        sql += " AND  product_discount <= 9.99";
                        break;
                    case "10 - 14,99%":
                        sql += " AND  product_discount > 9.99 AND product_discount <= 14.99";
                        break;
                    case "15% и более":
                        sql += " AND  product_discount > 14.99";
                        break;
                }

                switch (SortBox.Text)
                {
                    case "По возрастанию":
                        sql += "ORDER BY product_cost ASC";
                        break;
                    case "По убыванию":
                        sql += "ORDER BY product_cost DESC";
                        break;
                }

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    productsGrid.ItemsSource = dt.DefaultView;
                }
            }
        }

        private void FirstLoad()
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = @"SELECT product_article AS 'Артикул',                                
		                                product_name AS 'Наименование',                                     
		                                product_desc AS 'Описание',                                     
		                                (SELECT category.category_name FROM category WHERE category.category_id = product.product_categoryID) AS 'Категория',                                     
		                                (SELECT supplier.supplier_name FROM supplier WHERE supplier.supplier_id = product.product_supplierID) AS 'Поставщик',                                     
		                                (SELECT manufacturer.manufacturer_name FROM manufacturer WHERE manufacturer.manufacturer_id = product.product_manufacturerID) AS 'Производитель',                                     
		                                product_photoname AS 'Фотография',                                     
		                                product_cost AS 'Стоимость',                                     
		                                product_discount AS 'Скидка',                                     
		                                product_quantityinstock AS 'Количество на складе'                                     
		                        FROM product";
                
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                    productsGrid.ItemsSource = dt.DefaultView;
                }
            }
        }
        
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillData();
        }

        private void NoFilter_OnSelected(object sender, RoutedEventArgs e)
        {
            FillData();
        }
    }
}
