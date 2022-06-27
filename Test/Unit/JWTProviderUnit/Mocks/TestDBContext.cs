using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Mocks;

internal static class TestDBContext
{
    public static UsersDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<UsersDBContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
        return new UsersDBContext(options);
    }
}

