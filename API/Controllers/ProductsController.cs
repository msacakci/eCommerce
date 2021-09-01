using System;
using System.Collections.Generic;
using API.DTOs;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private List<string> languageCodes;
        private DatabaseHelper databaseHelper;
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ProductsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            if(_session.GetString("ActiveLanguage") == null)
                _session.SetString("ActiveLanguage", "");

            databaseHelper = new DatabaseHelper();
            adjustLanguageCodes();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            List<ProductDto> productDtoList = databaseHelper.GetListOfProductDtos(connectionString);

            List<Product> productList = databaseHelper.GetListOfProducts(connectionString, productDtoList, _session.GetString("ActiveLanguage"));               
            return productList;
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            ProductDto productDto = databaseHelper.GetProductDtoFromProductsTable(connectionString, id);

            var product = databaseHelper.GetProductFromDatabase(connectionString, productDto, _session.GetString("ActiveLanguage"));

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

        [HttpPut("{languageCode}")]
        public ActionResult<IEnumerable<Product>> ChangeLanguage(string languageCode)
        {
            _session.SetString("ActiveLanguage", languageCode);

            if(!languageCodes.Contains(_session.GetString("ActiveLanguage")))
            {
                Console.WriteLine("Language code error");
                return null;
            }        

            return GetProducts();
                 
        }

        private void adjustLanguageCodes()
        {
            languageCodes = new List<string>();
            GetLanguagesHelper getLanguagesHelper = new GetLanguagesHelper();

            languageCodes = getLanguagesHelper.GetLanguagesFromDatabase(connectionString);

            if(_session.GetString("ActiveLanguage") == "")
                _session.SetString("ActiveLanguage", languageCodes[0]);
        }
    }

}