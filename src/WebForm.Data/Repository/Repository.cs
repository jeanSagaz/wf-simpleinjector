using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebForm.Data.Interfaces;

namespace WebForm.Data.Repository
{
    public class Repository : IRepository
    {
        public async Task Add()
        {
            Console.WriteLine("Add");

            await Task.CompletedTask;
        }

        public async Task Update()
        {
            Console.WriteLine("Update");

            await Task.CompletedTask;
        }
    }
}
