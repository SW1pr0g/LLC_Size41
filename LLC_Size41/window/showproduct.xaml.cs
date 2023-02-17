using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LLC_Size41.classes;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class showproduct : Window
    {
        private string photoname = String.Empty;
        public showproduct()
        {
            InitializeComponent();
            FillData();
            Variables.showproductClosed = false;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {            
            Variables.showproductClosed = true;
            new main().Show();
            Close();
        }
        private void FillData()
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = String.Format(@"SELECT product_article AS 'Артикул',                                
		                                product_name AS 'Наименование',                                     
		                                (SELECT category.category_name FROM category WHERE category.category_id = product.product_categoryID) AS 'Категория',                                     
		                                (SELECT manufacturer.manufacturer_name FROM manufacturer WHERE manufacturer.manufacturer_id = product.product_manufacturerID) AS 'Производитель',                                     
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
                    CountGrid.Content = "Количество записей: " + productsGrid.Items.Count.ToString();
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

        private void ProductsGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            productsGrid.Columns[0].Width = DataGridLength.Auto;
            productsGrid.Columns[1].Width = DataGridLength.Auto;
            productsGrid.Columns[5].Width = DataGridLength.Auto;
        }

        private void AddToTrash_OnClick(object sender, RoutedEventArgs e)
        {
            ProductsGrid_OnMouseLeftButtonUp(null, null);
            var a = Variables.trash.Find(x => x.ArticleNum == ArticleBox.Text);
            if (a != null)
            {
                if (Convert.ToInt32(QuantityBox.Text) > a.Count)
                {
                    int i = Variables.trash.IndexOf(a);
                    Variables.trash[i] = new TrashItem { ArticleNum = ArticleBox.Text, Name = NameBox.Text, Price = Convert.ToDouble(PriceBox.Text), Discount = Convert.ToInt32(DiscountBox.Text), Manufacturer = ManufacturerBox.Text, Count = a.Count+1, };
                    MessageBox.Show(String.Format("Товар под артикулом {0} добавлен в корзину!", ArticleBox.Text));
                }
                else
                    MessageBox.Show("На складе нет больше товара!");
            }
            else
            {
                if (Convert.ToInt32(QuantityBox.Text) > 1)
                {
                    Variables.trash.Add(new TrashItem { ArticleNum = ArticleBox.Text, Name = NameBox.Text, Price = Convert.ToDouble(PriceBox.Text), Discount = Convert.ToInt32(DiscountBox.Text), Manufacturer = ManufacturerBox.Text, Count = 1, });
                    MessageBox.Show(String.Format("Товар под артикулом {0} добавлен в корзину!", ArticleBox.Text));
                    classes.Variables.trashVisible = true;
                }
                else
                    MessageBox.Show("На складе нет больше товара!");
            }

        }

        private void ProductsGrid_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                string sql = String.Format(@"SELECT product_article,product_name,product_cost,(SELECT manufacturer.manufacturer_name FROM manufacturer WHERE manufacturer.manufacturer_id = product.product_manufacturerID),                                 
                (SELECT supplier.supplier_name FROM supplier WHERE supplier.supplier_id = product.product_supplierID),                                     
                (SELECT category.category_name FROM category WHERE category.category_id = product.product_categoryID),product_discount,product_quantityinstock,product_desc,                                
                product_photoname FROM product WHERE product_article = '{0}';", ((DataRowView)productsGrid.SelectedItems[0]).Row["Артикул"]);
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArticleBox.Text = reader.GetString(0);
                            NameBox.Text = reader.GetString(1);
                            PriceBox.Text = reader.GetString(2);
                            ManufacturerBox.Text = reader.GetString(3);
                            SupplierBox.Text = reader.GetString(4);
                            CategoryBox.Text = reader.GetString(5);
                            DiscountBox.Text = reader.GetString(6);
                            QuantityBox.Text = reader.GetString(7);
                            DescBox.Text = reader.GetString(8);
                            if (reader.GetString(9) != String.Empty)
                            {
                                var uriSource = new Uri(Directory.GetCurrentDirectory() + "\\images\\product\\" + reader.GetString(9), UriKind.Absolute);
                                ProductImage.Source = new BitmapImage(uriSource);
                            }
                            else
                            {
                                var uriSource = new Uri(Directory.GetCurrentDirectory() + "\\images\\product\\" + reader.GetString(9), UriKind.Absolute);
                                ProductImage.Source = new BitmapImage(uriSource);
                            }
                        }
                    }
                }
            }
        }

        private void Showproduct_OnClosing(object sender, CancelEventArgs e)
        {
            if (Variables.showproductClosed == false)
            {
                if (MessageBox.Show("Вы действительно хотите закрыть приложение?",
                        "Выход из приложения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    Variables.showproductClosed = true;
            } 
        }
    }
}
