using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using API.DTOs;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;
 

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private List<string> languageCodes;
        private readonly string connectionString = @"Server=DESKTOP-NEBN67H\SQLEXPRESS;AttachDbFilename=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\eCommerce.mdf;Database=eCommerce;Trusted_Connection=Yes;";
        string activeLanguage;
        
        public ProductsController()
        {
            adjustLanguageCodes();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();

            List<ProductDto> productDtoList = databaseHelper.GetListOfProductDtos(connectionString);

            List<Product> productList = databaseHelper.GetListOfProducts(connectionString, productDtoList, activeLanguage);               
            return productList;

        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();

            ProductDto productDto = databaseHelper.GetProductDtoFromProductsTable(connectionString, id);

            var product = databaseHelper.GetProductFromDatabase(connectionString, productDto, activeLanguage);

            string productInfo = product.Id + ". " + product.Description + " - " + product.ProductType  + "\n";
            Console.WriteLine(productInfo);

            return product;
        }

        // [HttpGet("{languageCode}")]
        // public ActionResult<IEnumerable<Product>> GetProductsWithSpecificLanguage(string languageCode)
        // {
        //     activeLanguage = languageCode;

        //     if(!languageCodes.Contains(activeLanguage))
        //     {
        //         Console.WriteLine("Language code error");
        //         return null;
        //     }        
        //     else
        //     {
        //         return GetProducts();
        //     }     
        // }

        private void adjustLanguageCodes()
        {
            languageCodes = new List<string>();
            GetLanguagesHelper getLanguagesHelper = new GetLanguagesHelper();

            languageCodes = getLanguagesHelper.GetLanguagesFromDatabase(connectionString);

            activeLanguage = languageCodes[0];
        }
    }

}