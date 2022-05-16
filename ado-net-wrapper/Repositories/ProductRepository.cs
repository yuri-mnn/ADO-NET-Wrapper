using ado_net_wrapper.Models;
using ado_net_wrapper.Repositories.DBConfig;
using ado_wrapper_lib;
using Microsoft.Data.SqlClient;

namespace ado_net_wrapper.Repositories;

public class ProductRepository : IRepository<Product>
{
    private IAdoDBContext _db;
    public ProductRepository(IAdoDBContext db)
    {
        _db = db;
    }
    public void Create(Product item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Product? Get(int id)
    {
        var adoParams = new AdoParams(Name: "Production.GetAllProduct",
                                                      DBParams: new SqlParameter[] { new SqlParameter("@Id", id) },
                                                      Connection: null,
                                                      Transaction: null,
                                                      Timeout: 120);
        return _db.Execute<SqlParameter[], Product>(adoParams, TypeExecuter.Procedure)?.ToList().FirstOrDefault();
    }

    public IEnumerable<Product>? GetAll()
    {
        var adoParams = new AdoParams(Name: "Production.GetAllProduct");
        return _db.Execute<SqlParameter[], Product>(adoParams, TypeExecuter.Procedure);
    }

    public void Update(Product item)
    {
        throw new NotImplementedException();
    }
}
