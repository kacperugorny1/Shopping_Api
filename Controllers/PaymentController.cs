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

    [HttpGet("GetAllPayments")]
    public IEnumerable<Payment>? GetAllPayments(){
        return _entityFramework.Payments;
    }

    [HttpGet("GetMyPayments")]
    public IEnumerable<PaymentConcanated>? GetMyPayments(){
        int userId; 
        if(!Int32.TryParse(User.FindFirst("userId")?.Value, out userId))
            throw new Exception("Falied to parse userId");
        return _entityFramework.PaymentsConcanated?.Where(u => u.WhoId == userId);
    }

    [HttpGet("GetPaymentsForMe")]
    public IEnumerable<PaymentConcanated>? GetPaymentsForMe(){
        int userId;
        if(!Int32.TryParse(User.FindFirst("userId")?.Value, out userId))
            throw new Exception("Falied to parse userId");
        return _entityFramework.PaymentsConcanated?.Where(u => u.ToWhoId == userId);
    }


    [HttpPut("AddPayment")]
    public IActionResult AddPayment(IEnumerable<PaymentForm> payments){
        foreach(PaymentForm payment in payments){    
            Payment paymentToDb = new(){
                PayId = 0,
                ToWhoId = payment.ToWhoId,
                WhoId = payment.WhoId,
                Cost = payment.Cost,
                DateAdded = DateTime.Now
            };
            _entityFramework.Add(paymentToDb);
            
            PaymentConcanated? paymentConcInDb = _entityFramework.PaymentsConcanated?.SingleOrDefault(u =>
            (u.ToWhoId == payment.ToWhoId && u.WhoId == payment.WhoId));
            // if(paymentConcInDb != null)
            //     _entityFramework.PaymentsConcanated?.Remove(paymentConcInDb);
            if(paymentConcInDb == null){
                PaymentConcanated paymentConcToDb = new(){
                    PayId = 0,
                    ToWhoId = payment.ToWhoId,
                    WhoId = payment.WhoId,
                    Cost = payment.Cost
                };
                _entityFramework.Add(paymentConcToDb);
            }
            else{
                paymentConcInDb.Cost += payment.Cost;
            }
        }
        if(_entityFramework.SaveChanges() > 0)
            return Ok();
        throw new Exception("Falied to add payments");
    }
            
    [HttpPut("MakeAPayment")]
    public IActionResult MakeAPayment(PayingForm payment){
        int userId; 
        if(!Int32.TryParse(User.FindFirst("userId")?.Value, out userId))
            throw new Exception("Falied to parse userId");
        // foreach(PaymentConcanated paymentInDb in _entityFramework?.PaymentsConcanated){
        //     if(paymentInDb.ToWhoId == payment.ToWhoId && paymentInDb.WhoId == userId)
        //         paymentInDb.Cost -= payment.Cost;
        //     if(paymentInDb.Cost == 0)
        //         _entityFramework.Remove(paymentInDb);
        // }
        PaymentConcanated? paymentInDb = _entityFramework.PaymentsConcanated?.FirstOrDefault(u => 
            u.ToWhoId == payment.ToWhoId && u.WhoId == userId
        );
        if(paymentInDb != null){
            paymentInDb.Cost -= payment.Cost;
            if(paymentInDb.Cost == 0)
                _entityFramework.Remove(paymentInDb);
        }
        
        if(_entityFramework.SaveChanges() > 0)
            return Ok();
        throw new Exception("Falied to make a payment");
        
        

    }
}