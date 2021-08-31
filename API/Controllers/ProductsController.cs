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
    public class ProductsController : BaseApiController
    {
        private List<string> languageCodes;
        private DatabaseHelper databaseHelper;

        string activeLanguage;

        public ProductsController()
        {
            databaseHelper = new DatabaseHelper();
            adjustLanguageCodes();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            List<ProductDto> productDtoList = databaseHelper.GetListOfProductDtos(connectionString);

            List<Product> productList = databaseHelper.GetListOfProducts(connectionString, productDtoList, activeLanguage);               
            return productList;
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            ProductDto productDto = databaseHelper.GetProductDtoFromProductsTable(connectionString, id);

            var product = databaseHelper.GetProductFromDatabase(connectionString, productDto, activeLanguage);

            string productInfo = product.Id + ". " + product.Description + " - " + product.ProductType  + "\n";
            Console.WriteLine(productInfo);

            return product;
        }

        [HttpPost("{addProduct}")]
        public ActionResult<DetailedProductDto> AddProduct(AddProductDto addProductDto)
        {
            if(!databaseHelper.IsProductCategoryIdValid(connectionString, addProductDto.ProductCategoryId))
            {
                return BadRequest("Product category id is invalid");
            }
            if(!databaseHelper.IsLanguageCodeValid(connectionString, addProductDto.LanguageCode))
            {
                return BadRequest("Language code is invalid");
            }

            return databaseHelper.InsertProductToDatabase(connectionString, addProductDto);
        }

        [HttpPut]
        public ActionResult<Product> EditProduct(DetailedProductDto newProductDto)
        {
            if(!databaseHelper.IsProductIdValid(connectionString, newProductDto.Id))
            {
                return BadRequest("Product id is invalid");
            }

            ProductDto productDto = databaseHelper.UpdateProduct(connectionString, newProductDto);

            return databaseHelper.GetProductFromDatabase(connectionString, productDto, newProductDto.LanguageCode);
        }

        [HttpDelete("delete-product/{productId}")]
        public ActionResult DeleteProduct(int productId)
        {
            if(!databaseHelper.IsProductIdValid(connectionString, productId))
            {
                return BadRequest("Product id is invalid");
            }
            
            databaseHelper.DeleteProductFromDatabase(connectionString, productId);
            return Ok();

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

            activeLanguage = languageCodes[1];
        }
    }

}