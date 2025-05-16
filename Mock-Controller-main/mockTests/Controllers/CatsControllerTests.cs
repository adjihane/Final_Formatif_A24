using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mock.depart.Controllers;
using mock.depart.Models;
using mock.depart.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.depart.Controllers.Tests
{
    [TestClass()]
    public class CatsControllerTests
    {
        Mock<CatsService> servicemock;
        Mock<CatsController> controllermock;
        public CatsControllerTests()
        {
            servicemock = new Mock<CatsService>() { };
            controllermock = new Mock<CatsController>(servicemock.Object) { CallBase = true };

            controllermock.Setup(s => s.UserId).Returns("12345");


        }

        [TestMethod]
        public void CatNotFound()
        {
            Cat? cat = new Cat() { Id = 1 };

            servicemock.Setup(s => s.Get(It.IsAny<int>())).Returns(value: null);
            var actionresult = controllermock.Object.DeleteCat(1);
            var result = actionresult.Result as NotFoundResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CatBadRequest()
        {
            Cat? cat = new Cat() { Id = 1, CatOwner = new CatOwner() { Id = "222" } };

            servicemock.Setup(s => s.Get(It.IsAny<int>())).Returns(cat);
            var actionresult = controllermock.Object.DeleteCat(1);
            var result = actionresult.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is not yours", result.Value);
        }

    }
}