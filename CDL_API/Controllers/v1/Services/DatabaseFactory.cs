using CDL.Api.Controllers.v1.Interface;
using Microsoft.EntityFrameworkCore;

namespace CDL.Api.Controllers.v1.Services
{
    public class DatabaseFactory<T> : IDatabaseFactory<T> where T : DbContext
    {
        private readonly DbContextOptions<T> options;

        public DatabaseFactory(DbContextOptions<T> options)
        {
            this.options = options;
        }

        /// <summary>
        /// Method to create a new instance of the DbContext object.
        /// It's use a Hack due the fact the connstraint new() on the where for T
        /// does not accept any parameters.
        /// In this case, it calls the reflection Activator to create a new instance
        /// to be used in the service select context
        /// </summary>
        /// <returns></returns>
        public T Create() => (T)Activator.CreateInstance(typeof(T), options)
            ?? throw new InvalidOperationException($"Cannot create an instance of {typeof(T).Name}.");
    }
}
