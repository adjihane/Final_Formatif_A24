using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using WebAPI.Controllers;
using WebAPI.Exceptions;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests;

[TestClass]
public class SeatsControllerTests
{
    [TestMethod]
    public void ReserveSeat()
    {
        //mock du service (en premier pour pouvoir l'injecter dans le construteur du controlleur)
        Mock<SeatsService> ServiceMock = new Mock<SeatsService>();

        //Mock du controller
        Mock<SeatsController> ControllerMock = new Mock<SeatsController>(ServiceMock.Object) { CallBase = true };

        //configuration de la propriete userid
        ControllerMock.Setup(t => t.UserId).Returns("1");


        //creer un seat
        Seat seat = new Seat() { Id = 1 };

        //configuration de la reservation du seat
        ServiceMock.Setup(s => s.ReserveSeat("1", 1)).Returns(seat);

        //configuration de la reservation du seat avec le service 
        var actionresult = ControllerMock.Object.ReserveSeat(1);
        
        //resulat attendu
        var result = actionresult.Result as OkObjectResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ReserveSeatUnauthorized()
    {
        //mock du service (en premier pour pouvoir l'injecter dans le construteur du controlleur)
        Mock<SeatsService> ServiceMock = new Mock<SeatsService>();

        //Mock du controller
        Mock<SeatsController> ControllerMock = new Mock<SeatsController>(ServiceMock.Object) { CallBase = true };

        //configuration de la propriete userid
        ControllerMock.Setup(t => t.UserId).Returns("1");

        //configuration de la reservation du seat pour throw une exception donc
        //peut importe les parametre on veut juste voir si cv retourner Unauthorized à la fin
        ServiceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatAlreadyTakenException()); ;

        //configuration de la reservation du seat avec le service 
        var actionresult = ControllerMock.Object.ReserveSeat(1);

        //resulat attendu
        var result = actionresult.Result as UnauthorizedResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ReserveSeatNotFound()
    {
        //mock du service (en premier pour pouvoir l'injecter dans le construteur du controlleur)
        Mock<SeatsService> ServiceMock = new Mock<SeatsService>();

        //Mock du controller
        Mock<SeatsController> ControllerMock = new Mock<SeatsController>(ServiceMock.Object) { CallBase = true };

        //configuration de la propriete userid
        ControllerMock.Setup(t => t.UserId).Returns("1");

        //creer un seat
        Seat seat = new Seat() { Number = 4 };

        //configuration de la reservation du seat pour throw une exception donc
        //peut importe les parametre on veut juste voir si cv retourner Unauthorized à la fin
        ServiceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatOutOfBoundsException()); 

        //configuration de la reservation du seat avec le service 
        var actionresult = ControllerMock.Object.ReserveSeat(seat.Number);

        //resulat attendu
        var result = actionresult.Result as NotFoundObjectResult;
        Assert.IsNotNull(result);

        //verifier que les message sont identique
        Assert.AreEqual("Could not find " + seat.Number, result.Value);
    }

    [TestMethod]
    public void ReserveSeatBadRequest()
    {
        //mock du service (en premier pour pouvoir l'injecter dans le construteur du controlleur)
        Mock<SeatsService> ServiceMock = new Mock<SeatsService>();

        //Mock du controller
        Mock<SeatsController> ControllerMock = new Mock<SeatsController>(ServiceMock.Object) { CallBase = true };

        //configuration de la propriete userid
        ControllerMock.Setup(t => t.UserId).Returns("1");

        //configuration de la reservation du seat pour throw une exception donc
        //peut importe les parametre on veut juste voir si cv retourner Unauthorized à la fin
        ServiceMock.Setup(s => s.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new UserAlreadySeatedException());

        //configuration de la reservation du seat avec le service 
        var actionresult = ControllerMock.Object.ReserveSeat(1);

        //resulat attendu
        var result = actionresult.Result as BadRequestResult;
        Assert.IsNotNull(result);

    }
}
