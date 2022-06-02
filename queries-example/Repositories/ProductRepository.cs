using ado_net_wrapper.Models;
using ado_net_wrapper.Repositories.DBConfig;
using AdoWrapper;
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
        var adoParams = new AdoParams(name: "Production.GetAllProduct",
                                                      dbParams: new SqlParameter[] { new SqlParameter("@Id", id) },
                                                      connection: null,
                                                      transaction: null,
                                                      timeout: 120);
        return _db.Execute<Product>(adoParams, TypeExecuter.Procedure)?.ToList().FirstOrDefault();
    }

    public IEnumerable<Product>? GetAll()
    {
        var adoParams = new AdoParams(name: "Production.GetAllProduct");
        return _db.Execute<Product>(adoParams, TypeExecuter.Procedure);
    }

    public void Update(Product item)
    {
        throw new NotImplementedException();
    }
}
