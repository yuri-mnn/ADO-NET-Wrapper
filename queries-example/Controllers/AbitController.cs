using ado_net_wrapper.Repositories.DBConfig;
using Microsoft.AspNetCore.Mvc;

namespace ado_net_wrapper.Controllers;


[ApiController]
[Route("[controller]")]
public class AbitController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;

    public AbitController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    [HttpGet]
    public IActionResult GetAbitResult(int id)
    {
        var result = _unitOfWork.AbitRepository.Get(id);
        if (result is null)
            return NoContent();
        else
            return Ok(result);
    }
}


