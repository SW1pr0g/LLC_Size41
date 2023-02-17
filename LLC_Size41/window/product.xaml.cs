using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace LLC_Size41.window
{
    public partial class product : Window
    {
        private string current_article = String.Empty;
        private string image_path = Directory.GetCurrentDirectory() + "\\images\\product\\picture.png";
        private string image_name = String.Empty;
        public product()
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

        private void ProductsGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
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
                            current_article = reader.GetString(0);
                            NameBox.Text = reader.GetString(1);
                            PriceBox.Text = reader.GetString(2);

                            switch (reader.GetString(3))
                            {
                                case "Kari":
                                    ManufacturerBox.SelectedIndex = 0;
                                    break;
                                case "Marco Tozzi":
                                    ManufacturerBox.SelectedIndex = 1;
                                    break;
                                case "Рос":
                                    ManufacturerBox.SelectedIndex = 2;
                                    break;
                                case "Rieker":
                                    ManufacturerBox.SelectedIndex = 3;
                                    break;
                                case "Alessio Nesca":
                                    ManufacturerBox.SelectedIndex = 4;
                                    break;
                                case "CROSBY":
                                    ManufacturerBox.SelectedIndex = 5;
                                    break;
                            }

                            switch (reader.GetString(4))
                            {
                                case "Kari":
                                    SupplierBox.SelectedIndex = 0;
                                    break;
                                case "Обувь для вас":
                                    SupplierBox.SelectedIndex = 1;
                                    break;
                            }
                            
                            switch (reader.GetString(5))
                            {
                                case "Мужская обувь":
                                    CategoryBox.SelectedIndex = 0;
                                    break;
                                case "Женская обувь":
                                    CategoryBox.SelectedIndex = 1;
                                    break;
                            }
                            
                            DiscountBox.Text = reader.GetString(6);
                            QuantityBox.Text = reader.GetString(7);
                            DescBox.Text = reader.GetString(8);
                            if (reader.GetString(9) != String.Empty)
                            {
                                var uriSource = new Uri(Directory.GetCurrentDirectory() + "\\images\\product\\" + reader.GetString(9),
                                    UriKind.Absolute);
                                ProductImage.Source = new BitmapImage(uriSource);
                                EditBtn.IsEnabled = true;
                                DeleteBtn.IsEnabled = true;
                            }
                        }
                    }
                }
            }
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CategoryBox.SelectedIndex = -1;
            NameBox.Clear();
            PriceBox.Clear();
            ManufacturerBox.SelectedIndex = -1;
            DiscountBox.Clear();
            QuantityBox.Clear();
            SupplierBox.SelectedIndex = -1;
            DescBox.Clear();
            var uriSource = new Uri(Directory.GetCurrentDirectory() + "\\images\\product\\picture.png", UriKind.Absolute);
            ProductImage.Source = new BitmapImage(uriSource);
            FillData();
            EditBtn.IsEnabled = false;
            DeleteBtn.IsEnabled = false;
            image_path = Directory.GetCurrentDirectory() + "\\images\\product\\picture.png";
        }

        private void AddBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckFields() == true)
                return;
            var image_namestr = image_path.Split('\\');
            image_name = image_namestr[image_namestr.Length - 1];
            using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
            {
                conn.Open();
                Random rnd = new Random();
                string article = String.Empty;
                char[] ArticleSymbol = new[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

                article += ArticleSymbol[rnd.Next(0, 25)];
                article += rnd.Next(0, 9).ToString();
                article += rnd.Next(0, 5).ToString();
                article += rnd.Next(0, 8).ToString();
                article += ArticleSymbol[rnd.Next(0, 25)];
                article += rnd.Next(0, 6).ToString();

                string sql = String.Format(@"INSERT INTO product (product_article, product_name, product_cost, product_manufacturerID, 
                                                                    product_supplierID, product_categoryID, product_discount, product_quantityinstock,
                     product_desc, product_photoname) VALUES('{0}', '{1}', {2}, 
                                                             (SELECT manufacturer_id FROM manufacturer WHERE manufacturer_name = '{3}'),
                                                             (SELECT supplier_id FROM supplier WHERE supplier_name = '{4}'),
                                                             (SELECT category_id FROM category WHERE category_name = '{5}'),
                                                             {6}, {7}, '{8}', '{9}');", article, NameBox.Text, PriceBox.Text, ManufacturerBox.Text, SupplierBox.Text, CategoryBox.Text,
                    DiscountBox.Text, QuantityBox.Text, DescBox.Text, image_name);
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Добавлено!");
                    }
                    catch (MySqlException)
                    {
                        MessageBox.Show("Ошибка! Найдено похожее имя описания.", "Проверка заполнения", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    FillData();
                    if (image_name != "picture.png")
                        File.Copy(image_path, Directory.GetCurrentDirectory() + "\\images\\product\\" + image_name, true);
                }
            }
            ClearBtn_OnClick(null, null);
        }

        private void EditBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (CheckFields() == true)
                return;
            var res = MessageBox.Show(
                String.Format("Вы действительно хотите отредактировать товар под артикулом - {0}?", current_article),
                "Редактирование товара",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                var image_namestr = image_path.Split('\\');
                image_name = image_namestr[image_namestr.Length - 1];
                using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
                {
                    conn.Open();
                    string sql = String.Format(@"UPDATE product SET product_article = '{1}', product_name = '{2}', product_cost = {3},
                                                                    product_manufacturerID = (SELECT manufacturer_id FROM manufacturer WHERE manufacturer_name = '{4}'),
                                                                    product_supplierID = (SELECT supplier_id FROM supplier WHERE supplier_name = '{5}'),
                                                                    product_categoryID = (SELECT category_id FROM category WHERE category_name = '{6}'),
                                                                    product_discount = {7}, product_quantityinstock = {8}, product_desc = '{9}', product_photoname = '{10}'
                   WHERE product_article = '{0}';", current_article, current_article, NameBox.Text, PriceBox.Text, ManufacturerBox.Text, SupplierBox.Text, CategoryBox.Text,
                        DiscountBox.Text, QuantityBox.Text, DescBox.Text, image_name);
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Отредактировано!");
                        }
                        catch (MySqlException)
                        {
                            MessageBox.Show("Ошибка! Найдено похожее имя артикула, описания.", "Проверка заполнения", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            return;
                        }
                        FillData();
                        if (image_name != "picture.png")
                            File.Copy(image_path, Directory.GetCurrentDirectory() + "\\images\\product\\" + image_name, true);
                    }
                }
            }
        }

        private void DeleteBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(
                String.Format("Вы действительно хотите удалить товар под артикулом - {0}?", current_article),
                "Удаление товара",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                using (MySqlConnection conn = new MySqlConnection(classes.Variables.ConnStr))
                {
                    conn.Open();
                    string sql = String.Format("DELETE FROM product WHERE product_article = '{0}';", current_article);
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                            FillData();
                            MessageBox.Show("Удалено!");
                        }
                        catch (MySqlException)
                        {
                            MessageBox.Show("Товар находится в заказе! Удаление невозможно!");
                        }
                    }
                }
            }
        }

        private void ProductsGrid_OnSourceUpdated(object sender, DataTransferEventArgs e)
        {
            productsGrid.Columns[0].Width = DataGridLength.Auto;
            productsGrid.Columns[1].Width = DataGridLength.Auto;
            productsGrid.Columns[5].Width = DataGridLength.Auto;
        }

        private void EditImage_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG files (*.jpg)|*.jpg|PNG files (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                image_path = openFileDialog.FileName;
                var uriSource = new Uri(image_path, UriKind.Absolute);
                ProductImage.Source = new BitmapImage(uriSource);
                var image_namestr = image_path.Split('\\');
                image_name = image_namestr[image_namestr.Length - 1];
            }
        }

        private void DiscountBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text,0)) e.Handled = true;
        }

        private bool CheckFields()
        {
            bool res = false;
            if (NameBox.Text == String.Empty || PriceBox.Text == String.Empty ||
                ManufacturerBox.Text == String.Empty || SupplierBox.Text == String.Empty ||
                CategoryBox.Text == String.Empty || DiscountBox.Text == String.Empty ||
                QuantityBox.Text == String.Empty || DescBox.Text == String.Empty)
            {
                MessageBox.Show("Ошибка заполнения! Не все поля заполнены.", "Проверка заполнения", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                res = true;
            }

            return res;
        }
    }
}