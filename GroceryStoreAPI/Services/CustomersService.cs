using GroceryStoreAPI.Interfaces;
using GroceryStoreAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Services
{
    public class CustomersService : ICustomersService
    {
        private readonly string jsonFile = @"C:\repos\ctc-software\autobooks-interview-dotnet3\GroceryStoreAPI\database.json";

        private async Task<string> ReadJsonFromFile()
        {
            return await File.ReadAllTextAsync(jsonFile);
        }

        private async Task WriteJsonToFile(dynamic jObject)
        {
            await File.WriteAllTextAsync(jsonFile, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        }

        private bool Exists(string json, int id)
        {
            var jsonObj = JObject.Parse(json);
            var dbCustomers = JsonConvert.DeserializeObject<DBCustomers>(json);
            return dbCustomers.customers.Any(c => c.Id == id);
        }

        public async Task CreateAsync(JObject request)
        {
            var json = await ReadJsonFromFile();
            Customer addCustomer = request.ToObject<Customer>();
            if (Exists(json, addCustomer.Id))
            {
                throw new Exception($"Customer with Id {(int)request["id"]} exist.");
            }
            var dbCustomers = JsonConvert.DeserializeObject<DBCustomers>(json);
            dbCustomers.customers.Add(addCustomer);
            await WriteJsonToFile(dbCustomers);
        }

        public async Task DeleteAsync(int id)
        {
            var json = await ReadJsonFromFile();
            if (Exists(json, id))
            {
                throw new Exception($"Customer with Id {id} does not exist.");
            }
            var dbCustomers = JsonConvert.DeserializeObject<DBCustomers>(json);
            var custtoRemove = dbCustomers.customers.Where(c=>c.Id == id).FirstOrDefault();
            dbCustomers.customers.Remove(custtoRemove);
            await WriteJsonToFile(dbCustomers);
        }

        public async Task<List<Customer>> GetAsync()
        {
            var json = JObject.Parse( await File.ReadAllTextAsync(jsonFile));
            var customers = json["customers"].ToObject<List<Customer>>();
            return customers;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customers = await GetAsync();
            return customers.Where(c => c.Id == id).FirstOrDefault();
        }

        public async Task UpdateAsync(int id, JObject request)
        {
            var json = await ReadJsonFromFile();
            Customer updateCustomer = request.ToObject<Customer>();
            if (!Exists(json, id))
            {
                throw new Exception($"Customer with Id {(int)request["id"]} does not exist.");
            }
            var dbCustomers = JsonConvert.DeserializeObject<DBCustomers>(json);
            foreach(var customer in dbCustomers.customers)
            {
                if(customer.Id == id)
                {
                    customer.Name = updateCustomer.Name;
                }
            }
            await WriteJsonToFile(dbCustomers);
        }
    }
}
