using NUnit.Framework;
using System.Threading.Tasks;
using rf_kliensapp;
using System;
using System.Linq;

namespace rf_kliensapp.UploadTest
{
    [TestFixture]
    public class Form1Tests
    {
        public static string GenerateDomainName(string suffix)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var namePart = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return namePart + suffix;
        }

        private Form1 _form1;

        [SetUp]
        public void Setup()
        {
            _form1 = new Form1();
        }

        [TestCase(".com", "20", true)]
        [TestCase(".net", "30", true)]
        [TestCase("-fsa", "15", false)]
        [TestCase(".org", "0", true)]  
        [TestCase(".com", "100-50", false)]
        public async Task AddProduct_ShouldAddProduct_WhenCalledWithValidData(string suffix, string productPrice, bool expectedResult)
        {
            string productName = GenerateDomainName(suffix);  

            // Act
            bool result = await _form1.AddProduct(productName, productPrice, true);

            // Assert
            Assert.AreEqual(expectedResult, result, $"Testing with product: {productName} and price: {productPrice}. Expected {expectedResult}.");
        }
    }
}