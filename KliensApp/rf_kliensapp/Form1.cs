using static System.Net.WebRequestMethods;
using System.Net.Http;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace rf_kliensapp
{
    public partial class Form1 : Form
    {
        private const string baseUrl = "http://20.234.113.211:8094";
        //generate new api key for use. 
        private const string key = "1-9e93f163-adec-4411-ae89-e022399605cf";

        public Form1()
        {
            InitializeComponent();
            GetProducts();
        }
        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (productsListView.SelectedItems.Count > 0)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to delete the selected domain?", "Delete?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ProductListItem selectedItem = (ProductListItem)productsListView.SelectedItems[0];
                    string productId = selectedItem.Bvin;
                    DeleteProduct(productId, true);
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.");
            }
        }
        private async void DeleteProduct(string productId, bool isVerbose)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string deleteEndpointUrl = $"/DesktopModules/Hotcakes/API/rest/v1/products/{productId}?key={key}";
            HttpResponseMessage response = client.DeleteAsync(deleteEndpointUrl).Result;

            if (response.IsSuccessStatusCode && isVerbose)
            {
                GetProducts();
                MessageBox.Show("Product deleted successfully");
                btnDeleteProduct.Text = "Please Choose a Product to Delete";
            }
            else if (response.IsSuccessStatusCode && !isVerbose) { }
            else
            {
                // Handle error gracefully
                string errorContent = await response.Content.ReadAsStringAsync();
                // Display error message (e.g., MessageBox) or log the error
                if (isVerbose)
                {
                    MessageBox.Show($"Failed to delete product. Please contact the developers of the application and show this Error Message: {errorContent} ");
                }
                else
                {
                    MessageBox.Show($"Failed to update product. Please contact the developers of the application and show this Error Message: {errorContent} ");
                }
            }
        }
        private async void GetProducts()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync("/DesktopModules/Hotcakes/API/rest/v1/products?key=" + key).Result;

            if (response.IsSuccessStatusCode)
            {

                var res = response.Content.ReadAsStringAsync().Result;
                Root root = JsonConvert.DeserializeObject<Root>(res);
                productsListView.Items.Clear();
                currentProductName.Text = string.Empty;
                currentProductPrice.Text = string.Empty;

                foreach (var product in root.Content.Products)
                {
                    productsListView.Items.Add(new ProductListItem()
                    {
                        ProductName = product.ProductName,
                        Bvin = product.Bvin,
                        Price = product.SitePrice
                    });
                    productsListView.ValueMember = "Bvin";
                    productsListView.DisplayMember = "ProductName";
                }
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                productsListView.Items.Add($"Error loading products.  Please contact the developers of the application and show this Error Message: {errorContent} ");
            }
        }
        private void productsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProductListItem selectedItem = (ProductListItem)productsListView.SelectedItems[0];
            string productName = selectedItem.ProductName;
            btnDeleteProduct.Text = "Delete " + productName;

            //Update Product set data
            currentProductName.Text = productName;
            currentProductPrice.Text = selectedItem.Price.ToString();
        }
        private void buttonAddProduct_Click(object sender, EventArgs e)
        {
            string name = txtProductName.Text;
            string price = txtProductPrice.Text;
            AddProduct(name, price, true);

        }
        public async Task<bool> AddProduct(string name, string price, bool isVerbose)
        {
            if (!IsValidProductName(name) || !IsValidPrice(price))
            {
                MessageBox.Show("Product name or price is invalid. Please check and try again.");
                return false;
            }

            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var product = new Product()
            {
                ProductName = name,
                Sku = name.Replace(".", ""),
                SitePrice = double.Parse(price),
                InventoryMode = 100,
                CustomProperties = new List<CustomProperty>(),
                ShippingDetails = new ShippingDetails(),
                CreationDateUtc = DateTime.Now,
                UrlSlug = name.Replace(".", ""),
                MinimumQty = 1,
                IsAvailableForSale = true,
                Tabs = new List<Tab>(),
                StoreId = 1,
                ImageFileSmallAlternateText = name + " " + name,
                ImageFileMediumAlternateText = name + " " + name,
            };

            var jsonContent = JsonConvert.SerializeObject(product);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var addProductEndpointUrl = $"/DesktopModules/Hotcakes/API/rest/v1/products?key={key}";

            HttpResponseMessage response = await client.PostAsync(addProductEndpointUrl, content);
            if (response.IsSuccessStatusCode)
            {
                if (isVerbose)
                {
                    MessageBox.Show("Product added successfully!");
                }
                GetProducts(); // Refresh the product list
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Failed to add product: {errorContent}");
                return false;
            }
        }

        bool IsValidProductName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            // Regex pattern to match the domain product name 
            string pattern = @"^[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[\.]+[a-zA-Z0-9]{2,}$";
            return Regex.IsMatch(name, pattern);
        }
        bool IsValidPrice(string priceString)
        {
            if (string.IsNullOrEmpty(priceString))
            {
                return false;
            }

            // Try parsing the price into a double
            double price;
            // Checking if the double is in the correct format: (15.23)
            string pattern = @"^\d+(\.\d+)?$";
            return double.TryParse(priceString, out price) && Regex.IsMatch(priceString, pattern);
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (productsListView.SelectedItems.Count > 0)
            {
                string name = updateProductName.Text;
                string price = updateProductPrice.Text;
                ProductListItem selectedItem = (ProductListItem)productsListView.SelectedItems[0];
                string selectedBvin = selectedItem.Bvin;
                UpdateProduct(name, price, selectedBvin);
            }
            else
            {
                MessageBox.Show("Please select a product to update.");
            }
        }
        private async void UpdateProduct(string name, string price, string bvin)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string updateProductEndpointUrl;
            string sku = name.Replace(".", "");

            if (IsValidProductName(name) && IsValidPrice(price))
            {
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to update the selected domain?", "Update?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        // 1. Get the existing product by its BVIN 
                        updateProductEndpointUrl = $"/DesktopModules/Hotcakes/API/rest/v1/products/{bvin}?key={key}";

                        // 2. Retrieve the existing product details 
                        var response = await client.GetAsync(updateProductEndpointUrl);
                        if (!response.IsSuccessStatusCode)
                        {
                            MessageBox.Show($"Failed to fetch product for update.  Please contact the developers of the application and show this Error Message: {response.StatusCode}");
                            return;
                        }

                        var contentString = response.Content.ReadAsStringAsync().Result;
                        var productToUpdate = JsonConvert.DeserializeObject<ProductResponse>(contentString);

                        // 3. Modify the Product
                        productToUpdate.Content.ProductName = name;
                        productToUpdate.Content.Sku = sku;
                        productToUpdate.Content.SitePrice = double.Parse(price);
                        productToUpdate.Content.CreationDateUtc = DateTime.Now;
                        productToUpdate.Content.UrlSlug = sku;
                        productToUpdate.Content.ImageFileSmallAlternateText = name + " " + name;
                        productToUpdate.Content.ImageFileMediumAlternateText = name + " " + name;


                        // 4. Serialize and Send Update
                        var jsonContent = JsonConvert.SerializeObject(productToUpdate);
                        DeleteProduct(bvin, false);
                        AddProduct(name, price, false);
                        GetProducts();
                    }
                }
            }
            else
            {
                buttonUpdateProduct.Text = "Please fix formatting issues!";
                if (!IsValidProductName(name) && (!IsValidPrice(price)))
                {
                    MessageBox.Show("Product name and Price is not valid. Product name should have one dot and not be empty. (domain) Price should be a valid number (e.g: 15.24)");
                }
                // Show error messages to the user if the regex checks don't pass.
                else if (!IsValidProductName(name))
                {
                    MessageBox.Show("Product name is not valid. It should have one dot and not be empty. (domain)");
                }

                else if (!IsValidPrice(price))
                {
                    MessageBox.Show("Price is not valid. Please enter a valid double value. (E.g: 15.22)");
                }
            }
        }
    }
}