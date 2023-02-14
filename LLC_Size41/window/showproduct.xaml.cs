using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class showproduct : Window
    {
        public showproduct()
        {
            InitializeComponent();
            FillData(@"SELECT product_articlenum AS 'Артикул', 
		                            product_name AS 'Наименование',
                                    product_desc AS 'Описание',
                                    (SELECT category.category_name FROM category WHERE category.category_id = product.product_categoryID) AS 'Категория',
                                    (SELECT supplier.supplier_name FROM supplier WHERE supplier.supplier_id = product.product_supplierID) AS 'Поставщик',
                                    (SELECT manufacturer.manufacturer_name FROM manufacturer WHERE manufacturer.manufacturer_id = product.product_manufacturerID) AS 'Производитель',
                                    product_photoname AS 'Фотография',
                                    product_cost AS 'Стоимость',
                                    product_discount AS 'Скидка',
                                    product_quantityonbase AS 'Количество на складе'        
                            FROM product;");
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {            
            new main().Show();
            this.Close();
        }
        private void FillData(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                using (MySqlCommand cmdSel = new MySqlCommand(sql, conn))
                {
                    DataTable dt = new DataTable();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                    da.Fill(dt);
                    productsGrid.ItemsSource = dt.DefaultView;
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillData(String.Format(@"SELECT product_articlenum AS 'Артикул', 
		                            product_name AS 'Наименование',
                                    product_desc AS 'Описание',
                                    (SELECT category.category_name FROM category WHERE category.category_id = product.product_categoryID) AS 'Категория',
                                    (SELECT supplier.supplier_name FROM supplier WHERE supplier.supplier_id = product.product_supplierID) AS 'Поставщик',
                                    (SELECT manufacturer.manufacturer_name FROM manufacturer WHERE manufacturer.manufacturer_id = product.product_manufacturerID) AS 'Производитель',
                                    product_photoname AS 'Фотография',
                                    product_cost AS 'Стоимость',
                                    product_discount AS 'Скидка',
                                    product_quantityonbase AS 'Количество на складе'        
                            FROM product WHERE product_name LIKE '%{0}%';", SearchBox.Text));
        }
    }
}
