using ado_net_wrapper.Repositories.DBConfig;
using Microsoft.AspNetCore.Mvc;

namespace ado_net_wrapper.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;

    public ProductController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Список всех продуктов
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _unitOfWork.ProductRepository.GetAll();
        if (products is null || (products?.Count() ?? 0) == 0)
            return NoContent();
        else
            return Ok(products);
    }

    /// <summary>
    /// Получить продукт по id
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("{id}")]
    public IActionResult GetProduct(int id)
    {
        var product = _unitOfWork.ProductRepository.Get(id);
        if (product is null)
            return NoContent();
        else
            return Ok(product);
    }
}
