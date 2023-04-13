using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Linq.Expressions;
using TagsManagement.DomainModels;
using TagsManagement.Repositories.Interfaces;

namespace TagsManagement.Repositories.Implements.ADONET
{
    public abstract class ADOTagRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity, new() // new() is for creating new instance of TEntity in ADO.NET
    {
        private readonly IConfiguration _configuration;  // net 7.0 (get environment variables)
        public ADOTagRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<TEntity> GetByIdAsync(string id)
        {

            throw new NotImplementedException();
        }
        public IQueryable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }
        public async Task<List<TEntity>> GetAllTagsAsync()
        {
            // preference: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/asynchronous-programming
            List<TEntity> result = new List<TEntity>();
            TEntity e = null;   // unassigned local variable
            string sqlConString = _configuration["ConnectionStrings:DefaultConnection"]; // specify to match your appsettings.json


            using (SqlConnection conn = new SqlConnection(sqlConString))
            using (SqlCommand comm = new SqlCommand("SELECT * FROM Tags", conn))
            {
                await conn.OpenAsync();

                using (var r = comm.ExecuteReader())
                {
                    // loop through all records
                    foreach (DbDataRecord s in r)
                    {
                        e = new TEntity();
                        // hand written assign code:
                        e.Id = s["Id"] as string;
                        e.CreatedDate = DateTime.Parse(s["CreatedDate"] as string);
                        e.Description = s["Description"] as string;
                        e.Level = Convert.ToInt16(s["Level"]);// nullable column: https://stackoverflow.com/questions/1772025/sql-data-reader-handling-null-column-values
                        result.Add(e);
                    }
                    await r.CloseAsync();   // seems to no need async (same performance)
                }

                // close to save resources:
                await conn.CloseAsync();    // seems to no need async (same performance)
            }

            return result;
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
        public async Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
        public void Update(TEntity entity)
        {
            // implementation code
        }
        public void Delete(TEntity entity)
        {
            // implementation code
        }
        public void DeleteAll()
        {
            // implementation code
        }
    }
}
