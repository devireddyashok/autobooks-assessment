using GroceryStoreAPI.Controllers;
using GroceryStoreAPI.Interfaces;
using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace GroceryStoreAPI.Test
{
    public class CustomersControllerShould
    {
        private readonly Mock<ICustomersService> _customersService = new Mock<ICustomersService>();
        private readonly List<Customer> _customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                Name = "Test"
            },
            new Customer
            {
                Name = "Test-2",
                Id = 2
            }
        };

        [Fact]
        public async Task Return_Customers()
        {
            _customersService.Setup(r => r.GetAsync()).ReturnsAsync(_customers);
            var controller = new CustomersController(_customersService.Object);
            var actionResult = await controller.GetAsync();
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            if (okResult.Value == null) return;
            var json = JsonConvert.SerializeObject(okResult.Value);
            var values = JsonConvert.DeserializeObject<List<Customer>>(json);
            if (values != null) Assert.Equal(_customers.Count, values.Count);
        }
        [Fact]
        public async Task Return_Customer_By_Id()
        {
            Customer expectedResult = _customers.Where(c => c.Id == 1).FirstOrDefault();
            _customersService.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(expectedResult);
            var controller = new CustomersController(_customersService.Object);
            var actionResult = await controller.GetByIdAsync(It.IsAny<int>());
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            if (okResult.Value == null) return;
            var json = JsonConvert.SerializeObject(okResult.Value);
            var result = JsonConvert.DeserializeObject<Customer>(json);
            if (result != null) 
                Assert.Equal(result.Name, expectedResult.Name);
        }
        [Fact]
        public async Task Return_No_Customer_Found()
        {
            _customersService.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(_customers.Where(c => c.Id == 3).FirstOrDefault());
            var controller = new CustomersController(_customersService.Object);
            var actionResult = await controller.GetByIdAsync(It.IsAny<int>());
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Null(okResult.Value);
        }
        [Fact]
        public async Task Create_Customer()
        {
            _customersService.Setup(r => r.CreateAsync(It.IsAny<JObject>()));
            var controller = new CustomersController(_customersService.Object);
            var actionResult = await controller.CreateAsync(It.IsAny<JObject>());
            var okResult = Assert.IsType<OkResult>(actionResult);
            _customersService.Verify(m => m.CreateAsync(It.IsAny<JObject>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }
        [Fact]
        public async Task Create_Customer_ThrowException_Exists()
        {
            _customersService.Setup(r => r.CreateAsync(It.IsAny<JObject>()))
                .Throws(new Exception($"Customer with Id {It.IsAny<int>()} exist."));
            var controller = new CustomersController(_customersService.Object);
            await Assert.ThrowsAsync<Exception>(() => controller.CreateAsync(It.IsAny<JObject>()));
        }
        [Fact]
        public async Task Update_Customer()
        {
            _customersService.Setup(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<JObject>()));
            var controller = new CustomersController(_customersService.Object);
            var actionResult = await controller.UpdateAsync(It.IsAny<int>(), It.IsAny<JObject>());
            var okResult = Assert.IsType<OkResult>(actionResult);
            _customersService.Verify(m => m.UpdateAsync(It.IsAny<int>(), It.IsAny<JObject>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }
        [Fact]
        public async Task Update_Customer_ThrowException_NotExists()
        {
            _customersService.Setup(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<JObject>()))
                 .Throws(new Exception($"Customer with Id {It.IsAny<int>()} does not exist."));
            var controller = new CustomersController(_customersService.Object);
            await Assert.ThrowsAsync<Exception>(() => controller.UpdateAsync(It.IsAny<int>(), It.IsAny<JObject>()));
        }
        [Fact]
        public async Task Delete_Customer()
        {
            _customersService.Setup(r => r.DeleteAsync(It.IsAny<int>()));
            var controller = new CustomersController(_customersService.Object);
            var actionResult = await controller.DeleteAsync(It.IsAny<int>());
            var okResult = Assert.IsType<OkResult>(actionResult);
            _customersService.Verify(m => m.DeleteAsync(It.IsAny<int>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }
        [Fact]
        public async Task Delete_Customer_ThrowException_NotExists()
        {
            _customersService.Setup(r => r.DeleteAsync(It.IsAny<int>()))
                .Throws(new Exception($"Customer with Id {It.IsAny<int>()} does not exist."));
            var controller = new CustomersController(_customersService.Object);
            await Assert.ThrowsAsync<Exception>(() => controller.DeleteAsync(It.IsAny<int>()));
        }
    }
}
