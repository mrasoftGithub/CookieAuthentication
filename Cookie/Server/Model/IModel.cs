using Cookie.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cookie.Server.Model
{
    public interface IModel
    {
        Task<Lid> HaalopLid(int ID);

        Task<Lid> HaalopLid(string Email, string Password);
    }
}
