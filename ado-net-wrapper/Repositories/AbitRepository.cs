using ado_net_wrapper.Repositories.DBConfig;
using ado_wrapper_lib;

namespace ado_net_wrapper.Repositories
{
    public class AbitRepository : IRepository<AbitResult>
    {
        private readonly IAdoDBContext _db;
        public AbitRepository(IAdoDBContext db) => _db = db;

        public void Create(AbitResult item)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public AbitResult? Get(int id)
        {
            AbitQuery query = new(id, null, null);
            var adoParams = new AdoParams(Name: "dbo.fnCheckAbitIdPPIsOkon", DBParams: query);
            return new AbitResult(_db.ExecuteScalar<AbitQuery, bool>(adoParams, TypeExecuter.Function));
        }

        public IEnumerable<AbitResult>? GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(AbitResult item)
        {
            throw new NotImplementedException();
        }
    }

    public record AbitResult(bool IsOkon);
    public record AbitQuery(int ID, int? Abit, int? IDPP);
}
