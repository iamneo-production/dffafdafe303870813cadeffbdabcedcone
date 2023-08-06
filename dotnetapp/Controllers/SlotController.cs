using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Exceptions; 
using dotnetapp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Controllers
{

    public class SlotController : Controller
    {
        public IActionResult JoinRide(int rideId)
        {
            // Write code to fetch ride details
            // If maximumCapacity reached Throw an exception
        }
            
        [HttpPost]
        public IActionResult JoinRide(int rideId, Commuter commuter)
        {
            // write code to join the ride 
            // If maximum Capacity reached Throw an exceptions
        }
    }
}