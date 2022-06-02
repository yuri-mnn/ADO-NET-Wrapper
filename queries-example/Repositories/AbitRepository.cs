using ado_net_wrapper.Repositories.DBConfig;
using AdoWrapper;
using AdoWrapper.Attributes;
using Microsoft.Data.SqlClient;

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
            var query1 = new SqlParameter[]
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@Abit", Convert.DBNull),
                new SqlParameter("@IDPP", Convert.DBNull)
            };
            var adoParams = new AdoParams(name: "dbo.fnCheckAbitIdPPIsOkon", dbParams: query);
            return new AbitResult(_db.ExecuteScalar<bool>(adoParams, TypeExecuter.Function));
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
    
    public record AbitQuery (int ID, int? Abit, [property: DBColumn(Name = "IDPP")] int? idPp132132134);
    
    public class AbitQuery1 {
        public AbitQuery1(int id, int? abit, int? idPP)
        {
            ID = id;
            Abit = abit;
            IDPP = idPP;
        }


        public int ID { get; }
        public int? Abit { get; set; }
        [property: DBColumn(Mapped = true)]
        public int? IDPP { get; set; }
    }
}
