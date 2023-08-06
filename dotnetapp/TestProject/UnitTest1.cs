using System.Collections.Generic;
using System.Linq;
using dotnetapp.Controllers;
using dotnetapp.Exceptions;
using dotnetapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Reflection;

namespace dotnetapp.Tests
{
    [TestFixture]
public class RideSharingTests
{
    private Type programType = typeof(SlotController);

    private DbContextOptions<RideSharingDbContext> _dbContextOptions;

    [SetUp]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<RideSharingDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Add test data to the in-memory database
            var ride = new Ride
            {
                RideID = 1,
                DepartureLocation = "Location A",
                Destination = "Location B",
                DateTime = DateTime.Parse("2023-08-30"),
                MaximumCapacity = 4
            };

            dbContext.Rides.Add(ride);
            dbContext.SaveChanges();
        }
    }

    [TearDown]
    public void TearDown()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Clear the in-memory database after each test
            dbContext.Database.EnsureDeleted();
        }
    }

    private MethodInfo GetMethodInfo(object controller, string methodName, bool isHttpPost)
        {
            Type controllerType = controller.GetType();
            MethodInfo method = controllerType.GetMethod(methodName);

            if (method == null)
            {
                Assert.Fail($"{methodName} method not found.");
            }

            int parametersLength = isHttpPost ? 3 : 1;

            if (method.GetParameters().Length != parametersLength)
            {
                Assert.Fail($"Invalid number of parameters for {methodName} method.");
            }

            if (isHttpPost)
            {
                if (method.GetParameters()[0].ParameterType != typeof(int) ||
                    method.GetParameters()[1].ParameterType != typeof(Commuter))
                {
                    Assert.Fail($"Invalid parameter types for {methodName} method.");
                }
            }
            else
            {
                if (method.GetParameters()[0].ParameterType != typeof(int))
                {
                    Assert.Fail($"Invalid parameter types for {methodName} method.");
                }
            }

            return method;
        }
        private MethodInfo GetJoinRideMethodInfo(SlotController controller)
        {
            Type controllerType = controller.GetType();
            MethodInfo joinRideMethod = controllerType.GetMethod("JoinRide", new[] { typeof(int), typeof(Commuter) });

            Assert.IsNotNull(joinRideMethod, "JoinRide method not found.");
            return joinRideMethod;
        }


    [Test]
        public void JoinRide_ValidCommuter_JoinsSuccessfully()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
                var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("Details", result.ActionName);
                Assert.AreEqual("Ride", result.ControllerName);

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                Assert.IsNotNull(ride);
                Assert.AreEqual(1, ride.Commuters.Count);
                Assert.AreEqual(4, ride.MaximumCapacity);
            }
        }


    [Test]
        public void JoinRide_ValidCommuter_JoinsSuccessfully2()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
                var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                Assert.IsNotNull(ride);
                Assert.AreEqual(1, ride.Commuters.Count);
            }
        }


    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully3()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);      

                // Act
            var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;            
            var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);

            Assert.AreEqual(4, ride.MaximumCapacity);
        }
    }




    [Test]
    public void JoinRide_ValidCommuter_JoinsSuccessfully1()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
                var result = joinRideMethod.Invoke(slotController, new object[] { 1, commuter }) as RedirectToActionResult;
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Ride", result.ControllerName);
        }
    }



    [Test]
    public void JoinRide_RideNotFound_ReturnsNotFoundResult()
    {
        using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
            // Arrange
            var slotController = new SlotController(dbContext);
            var commuter = new Commuter
            {
                Name = "John Doe",
                Email = "johndoe@example.com",
                Phone = "1234567890"
            };

            // Act
            // var result = slotController.JoinRide(2, commuter) as NotFoundResult;
            MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act
            var result = joinRideMethod.Invoke(slotController, new object[] { 2, commuter }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
    
    [Test]
        public void JoinRide_MaximumCapacityReached_ThrowsException()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter1 = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                var commuter2 = new Commuter
                {
                    Name = "Jane Smith",
                    Email = "janesmith@example.com",
                    Phone = "9876543210"
                };

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                ride.Commuters.Add(commuter1);
                ride.Commuters.Add(commuter2);
                ride.MaximumCapacity = 2;

                dbContext.SaveChanges();

                var commuter3 = new Commuter
                {
                    Name = "Alice Johnson",
                    Email = "alicejohnson@example.com",
                    Phone = "5555555555"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act & Assert
                try
                {
                    joinRideMethod.Invoke(slotController, new object[] { 1, commuter3 });
                    Assert.Fail("Expected RideSharingException, but no exception was thrown.");
                }
                catch (TargetInvocationException ex)
                {
                    // Unwrap the inner exception and check its type
                    Assert.IsTrue(ex.InnerException is RideSharingException, "Expected RideSharingException, but got: " + ex.InnerException.GetType().Name);
                }
            }
        }


    [Test]
        public void JoinRide_MaximumCapacityReached_ThrowsExceptionwith_message()
        {
            // Arrange
            using (var dbContext = new RideSharingDbContext(_dbContextOptions))
            {
                var slotController = new SlotController(dbContext);
                var commuter1 = new Commuter
                {
                    Name = "John Doe",
                    Email = "johndoe@example.com",
                    Phone = "1234567890"
                };

                var commuter2 = new Commuter
                {
                    Name = "Jane Smith",
                    Email = "janesmith@example.com",
                    Phone = "9876543210"
                };

                var ride = dbContext.Rides.Include(r => r.Commuters).FirstOrDefault(r => r.RideID == 1);
                ride.Commuters.Add(commuter1);
                ride.Commuters.Add(commuter2);
                ride.MaximumCapacity = 2;

                dbContext.SaveChanges();

                var commuter3 = new Commuter
                {
                    Name = "Alice Johnson",
                    Email = "alicejohnson@example.com",
                    Phone = "5555555555"
                };

                MethodInfo joinRideMethod = GetJoinRideMethodInfo(slotController);

                // Act & Assert
                try
                {
                    joinRideMethod.Invoke(slotController, new object[] { 1, commuter3 });
                    Assert.Fail("Expected RideSharingException, but no exception was thrown.");
                }
                catch (TargetInvocationException ex)
                {
                    // Unwrap the inner exception and check its type
                    Assert.IsTrue(ex.InnerException is RideSharingException, "Expected RideSharingException, but got: " + ex.InnerException.GetType().Name);
                    Assert.AreEqual("Maximum capacity reached", ex.InnerException.Message);
                }
            }
        }

//     [Test]
// public void JoinRide_DestinationSameAsDeparture_ReturnsViewWithValidationError()
// {
//     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//     {
//         // Arrange
//         var slotController = new SlotController(dbContext);
//         var commuter = new Commuter
//         {
//             Name = "John Doe",
//             Email = "johndoe@example.com",
//             Phone = "1234567890"
//         };

//         // Act
//         var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         ride.Destination = ride.DepartureLocation; // Set the destination as the same as departure
//         dbContext.SaveChanges();

//         var result = slotController.JoinRide(1, commuter) as ViewResult;

//         // Assert
//         Assert.IsNotNull(result);
//         Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("Destination"));
//     }
// }

// [Test]
// public void JoinRide_MaximumCapacityNotPositiveInteger_ReturnsViewWithValidationError()
// {
//     using (var dbContext = new RideSharingDbContext(_dbContextOptions))
//     {
//         // Arrange
//         var slotController = new SlotController(dbContext);
//         var commuter = new Commuter
//         {
//             Name = "John Doe",
//             Email = "johndoe@example.com",
//             Phone = "1234567890"
//         };

//         // Act
//         var ride = dbContext.Rides.FirstOrDefault(r => r.RideID == 1);
//         ride.MaximumCapacity = -5; // Set a negative value for MaximumCapacity
//         dbContext.SaveChanges();

//         var result = slotController.JoinRide(1, commuter) as ViewResult;

//         // Assert
//         Assert.IsNotNull(result);
//         Assert.IsFalse(result.ViewData.ModelState.IsValid);
//         Assert.IsTrue(result.ViewData.ModelState.ContainsKey("MaximumCapacity"));
//     }
// }


[Test]
        public void RideClassExists()
        {
            // Arrange
            Type rideType = typeof(Ride);

            // Act & Assert
            Assert.IsNotNull(rideType, "Ride class not found.");
        }
[Test]
public void CommuterClassExists()
{
    Type commuterType = typeof(Commuter);

            // Act & Assert
            Assert.IsNotNull(commuterType, "Commuter class not found.");
}



[Test]
public void ApplicationDbContextContainsDbSetSlotProperty()
{
    // var context = new ApplicationDbContext();
using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {
    var propertyInfo = dbContext.GetType().GetProperty("Rides");

    Assert.IsNotNull(propertyInfo);
    Assert.AreEqual(typeof(DbSet<Ride>), propertyInfo.PropertyType);
        }
}

[Test]
public void ApplicationDbContextContainsDbSetBookingProperty()
{
    // var context = new ApplicationDbContext();
    using (var dbContext = new RideSharingDbContext(_dbContextOptions))
        {

    var propertyInfo = dbContext.GetType().GetProperty("Commuters");

    Assert.IsNotNull(propertyInfo);
    Assert.AreEqual(typeof(DbSet<Commuter>), propertyInfo.PropertyType);
}
}

        [Test]
        public void Commuter_Properties_CommuterID_ReturnExpectedDataTypes()
        {
            // // Arrange
            // Commuter commuter = new Commuter();
           
            // Assert.That(commuter.CommuterID, Is.TypeOf<int>());
            Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = commuter.GetType().GetProperty("CommuterID");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "CommuterID property not found.");
            Assert.AreEqual(typeof(int), propertyInfo.PropertyType, "CommuterID property type is not int.");
        }

         [Test]
        public void Commuter_Properties_Name_ReturnExpectedDataTypes()
        {
            // Arrange
            // Commuter commuter = new Commuter();
            // commuter.Name= "";
           
            // Assert.That(commuter.Name, Is.TypeOf<string>());
            Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = commuter.GetType().GetProperty("Name");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "CommuterName property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "CommuterName property type is not string.");
        }

         [Test]
        public void Commuter_Properties_Email_ReturnExpectedDataTypes()
        {
            // Arrange
            // Commuter commuter = new Commuter();
            // commuter.Email = "";
           
            // Assert.That(commuter.Email, Is.TypeOf<string>());
            Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = commuter.GetType().GetProperty("Email");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "Email property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "Email property type is not string.");
        }

         [Test]
        public void Commuter_Properties_Phone_ReturnExpectedDataTypes()
        {
            // Arrange
            // Commuter commuter = new Commuter();
            // commuter.Phone = "";
           
            // Assert.That(commuter.Phone, Is.TypeOf<string>());
            Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = commuter.GetType().GetProperty("Phone");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "Phone property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "Phone property type is not int.");
        }

         [Test]
        public void Commuter_Properties_RideID_ReturnExpectedDataTypes()
        {
            // Arrange
            // Commuter commuter = new Commuter();
           
            // Assert.That(commuter.RideID, Is.TypeOf<int>());
            Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = commuter.GetType().GetProperty("RideID");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "RideID property not found.");
            Assert.AreEqual(typeof(int), propertyInfo.PropertyType, "RideID property type is not int.");
        }
         [Test]
        public void Ride_Properties_RideID_ReturnExpectedDataTypes()
        {
            // Arrange
            Ride ride = new Ride();
           
            // Assert.That(ride.RideID, Is.TypeOf<int>());
            // Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = ride.GetType().GetProperty("RideID");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "RideID property not found.");
            Assert.AreEqual(typeof(int), propertyInfo.PropertyType, "RideID property type is not int.");
        }

        [Test]
        public void Ride_Properties_DepartureLocation_ReturnExpectedDataTypes()
        {
            // Arrange
            Ride ride = new Ride();
            // ride.DepartureLocation = "";
            // Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = ride.GetType().GetProperty("DepartureLocation");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "DepartureLocation property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "DepartureLocation property type is not int.");
           
            // Assert.That(ride.DepartureLocation, Is.TypeOf<string>());
        }

        [Test]
        public void Ride_Properties_Destination_ReturnExpectedDataTypes()
        {
            // Arrange
            Ride ride = new Ride();
            // ride.Destination = "";Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = ride.GetType().GetProperty("Destination");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "Destination property not found.");
            Assert.AreEqual(typeof(string), propertyInfo.PropertyType, "Destination property type is not int.");
           
            // Assert.That(ride.Destination, Is.TypeOf<string>());
        }

        [Test]
        public void Ride_Properties_DateTime_ReturnExpectedDataTypes()
        {
            // Arrange
            Ride ride = new Ride();
            // Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = ride.GetType().GetProperty("DateTime");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "DateTime property not found.");
            Assert.AreEqual(typeof(DateTime), propertyInfo.PropertyType, "DateTime property type is not int.");
           
            // Assert.That(ride.DateTime, Is.TypeOf<DateTime>());
        }

        [Test]
        public void Ride_Properties_MaximumCapacity_ReturnExpectedDataTypes()
        {
            // Arrange
            Ride ride = new Ride();
            // Commuter commuter = new Commuter();
            PropertyInfo propertyInfo = ride.GetType().GetProperty("MaximumCapacity");

            // Act & Assert
            Assert.IsNotNull(propertyInfo, "MaximumCapacity property not found.");
            Assert.AreEqual(typeof(int), propertyInfo.PropertyType, "MaximumCapacity property type is not int.");
           
            // Assert.That(ride.MaximumCapacity, Is.TypeOf<int>());
        }

        [Test]
        public void Commuter_Ride_ReturnsExpectedValue()
        {
            // Arrange
            Ride expectedRide = new Ride { RideID = 2 };
            Commuter commuter = new Commuter { Ride = expectedRide };

            // Assert
            Assert.AreEqual(expectedRide, commuter.Ride);
        }

        [Test]
        public void Commuter_Properties_CommuterID_ReturnExpectedValues()
        {
            // Arrange
            int expectedCommuterID = 1;

            // Act
            Commuter commuter = new Commuter
            {
                CommuterID = expectedCommuterID,
            };

            // Assert
            Assert.AreEqual(expectedCommuterID, commuter.CommuterID);
        }

        [Test]
        public void Commuter_Properties_Name_ReturnExpectedValues()
        {
            
            string expectedName = "John Doe";

            // Act
            Commuter commuter = new Commuter
            {
                Name = expectedName,
            };

            // Assert
            Assert.AreEqual(expectedName, commuter.Name);
        }

        [Test]
        public void Commuter_Properties_Email_ReturnExpectedValues()
        {
            string expectedEmail = "john@example.com";

            // Act
            Commuter commuter = new Commuter
            {
                Email = expectedEmail,
            };

            Assert.AreEqual(expectedEmail, commuter.Email);
        }

        [Test]
        public void Commuter_Properties_Phone_ReturnExpectedValues()
        {
            
            string expectedPhone = "1234567890";

            // Act
            Commuter commuter = new Commuter
            {
                Phone = expectedPhone,
            };

            Assert.AreEqual(expectedPhone, commuter.Phone);
        }

        [Test]
        public void Commuter_Properties_RideID_ReturnExpectedValues()
        {
            int expectedRideID = 2;

            // Act
            Commuter commuter = new Commuter
            {
                RideID = expectedRideID
            };
            Assert.AreEqual(expectedRideID, commuter.RideID);
        }

   
}

}