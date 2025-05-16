using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using WebAPI.Exceptions;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests;

[TestClass]
public class SeatsControllerTests
{
    Mock<SeatsService> servicemock;
    Mock<SeatsController> controllermock;

    public SeatsControllerTests()
    {
        servicemock = new Mock<SeatsService>();
        controllermock = new Mock<SeatsController>(servicemock.Object) { CallBase = true };

        //Setup du userID
        controllermock.Setup(t => t.UserId).Returns("12345");
    }


    [TestMethod]
    public void ReserveSeat() 
    { 

        Seat seat = new Seat() { Id = 1, Number = 1 };

        servicemock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Returns(seat);

        var actionresult = controllermock.Object.ReserveSeat(seat.Number);
        var result = actionresult.Result as OkObjectResult;
        Assert.IsNotNull(result);
    }


    [TestMethod]
    public void ReserveSeatUnauthorized()
    {
        servicemock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatAlreadyTakenException());

        var actionresult = controllermock.Object.ReserveSeat(1);
        var result = actionresult.Result as UnauthorizedResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void NotFound()
    {
        servicemock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatOutOfBoundsException());

        var actionresult = controllermock.Object.ReserveSeat(1);
        var result = actionresult.Result as NotFoundObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual( "Could not find " + 1, result.Value);
    }

    [TestMethod]
    public void BadRequest()
    {
        servicemock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new UserAlreadySeatedException());

        var actionresult = controllermock.Object.ReserveSeat(1);
        var result = actionresult.Result as BadRequestResult;
        Assert.IsNotNull(result);
    }
}
