using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebForm.Data.Interfaces
{
    public interface IRepository
    {
        Task Add();

        Task Update();
    }
}
