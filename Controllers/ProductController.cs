using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NG_Core_Auth.Data;
using NG_Core_Auth.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/values
        [HttpGet("[action]")]
        [Authorize(Policy = "RequireLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel formData)
        {
            var newProduct = new ProductModel
            {
                Name = formData.Name,
                ImageUrl = formData.ImageUrl,
                Description = formData.Description,
                OutOfStock = formData.OutOfStock,
                Price = formData.Price
            };

            await _db.Products.AddAsync(newProduct);
            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product was added Successfully"));
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProduct = _db.Products.FirstOrDefault(p => p.ProductId == id);
            if (findProduct == null)
            {
                return NotFound();
            }

            findProduct.Name = formData.Name;
            findProduct.Description = formData.Description;
            findProduct.ImageUrl = formData.ImageUrl;
            findProduct.OutOfStock = formData.OutOfStock;
            findProduct.Price = formData.Price;

            _db.Entry(findProduct).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product with id " + id + " is updated"));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProduct = await _db.Products.FindAsync(id);
            if (findProduct == null)
            {
                return NotFound();
            }

            _db.Products.Remove(findProduct);
            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product with id " + id + " is Deleted."));
        }
    }
}
