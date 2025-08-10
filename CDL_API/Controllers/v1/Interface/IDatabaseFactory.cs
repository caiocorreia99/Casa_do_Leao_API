using Microsoft.EntityFrameworkCore;

namespace CDL.Api.Controllers.v1.Interface
{
    public interface IDatabaseFactory<T> where T : DbContext
    {
        T Create();
    }
}
