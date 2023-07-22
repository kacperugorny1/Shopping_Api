using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Data;
using Shopping.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Shopping.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase{
        private readonly DataContextEF _entityFramework;
        public PaymentController(IConfiguration config){
            _entityFramework = new(config);
        }

        [HttpGet]
        public IEnumerable<Payment>

}