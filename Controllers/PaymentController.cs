using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Data;
using Shopping.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Shopping.Controllers;
// TO DO 
[Authorize]
[ApiController]
[Route("[controller]")]
public class PaymentController : ControllerBase{
        private readonly DataContextEF _entityFramework;
        public PaymentController(IConfiguration config){
            _entityFramework = new(config);
        }

        [HttpGet("GetAllPayments"), AllowAnonymous]
        public IEnumerable<Payment>? GetAllPayments(){
            return _entityFramework.Payments;
        }

        [HttpGet("GetMyPayments")]
        public IEnumerable<Payment>? GetMyPayments(){
            int userId; 
            if(!Int32.TryParse(User.FindFirst("userId")?.Value, out userId))
                throw new Exception("Falied to parse userId");
            return _entityFramework.Payments?.Where(u => u.WhoId == userId);
        }

        [HttpGet("GetPaymentsForMe")]
        public IEnumerable<Payment>? GetPaymentsForMe(){
            int userId;
            if(!Int32.TryParse(User.FindFirst("userId")?.Value, out userId))
                throw new Exception("Falied to parse userId");
            return _entityFramework.Payments?.Where(u => u.ToWhoId == userId);
        }


        [HttpPut]
        public IActionResult AddPayment(IEnumerable<PaymentForm> payments){
            foreach(PaymentForm payment in payments){    
                Payment paymentToDb = new(){
                    PayId = 0,
                    ToWhoId = payment.ToWhoId,
                    WhoId = payment.WhoId,
                    Cost = payment.Cost,
                    DateAdded = DateTime.Now
                };
                
                PaymentConcanated? paymentConcInDb = _entityFramework.PaymentsConcanated?.SingleOrDefault(u =>
                (u.ToWhoId == payment.ToWhoId && u.WhoId == payment.WhoId));
                if(paymentConcInDb != null)
                    _entityFramework.PaymentsConcanated?.Remove(paymentConcInDb);
                
                
                PaymentConcanated paymentConcToDb = new(){
                    PayId = 0,
                    ToWhoId = payment.ToWhoId,
                    WhoId = payment.WhoId,
                    Cost = payment.Cost + (paymentConcInDb == null?0:paymentConcInDb.Cost)
                };
                _entityFramework.Add(paymentConcToDb);
                _entityFramework.Add(paymentToDb);
            }
            if(_entityFramework.SaveChanges() == payments.Count() * 2)
                return Ok();
            throw new Exception("Falied to add payments");
        }
}