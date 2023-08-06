using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetapp.Exceptions; 

using dotnetapp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetapp.Controllers
{
public class RideController : Controller
{
    public IActionResult AvailableRides()
    {
        // Write code to fetch availableRides from the DB
    }


    public IActionResult Details(int id)
    {
        // Write code to get Details of JoinedCommuter, AvailableSeats ...
    }
}
}