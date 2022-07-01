using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Mocks;

internal static class TestDBContext
{
    public static UsersDBContext CreateInMemoryContext()
    {
        var context = new UsersDBContext(CreateOptions());
        context.Database.EnsureCreated();
        return context;
    }

    public static Mock<UsersDBContext> CreateContextMock()
    {
        return new Mock<UsersDBContext>(new object[] { CreateOptions() });
    }

    private static DbContextOptions CreateOptions()
        => new DbContextOptionsBuilder<UsersDBContext>()
                      .UseInMemoryDatabase(Guid.NewGuid().ToString())
                      .Options;
}

