using GroceryStoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Interfaces
{
    public interface ICustomersService
    {
        Task<List<Customer>> GetAsync();
        Task<Customer> GetByIdAsync(int id);
        Task CreateAsync(JObject request);
        Task UpdateAsync(int id, JObject request);
        Task DeleteAsync(int id);
    }
}
