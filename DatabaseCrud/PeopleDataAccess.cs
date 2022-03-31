using Dapper;
using DatabaseCrud.Models;
using System.Data;
namespace DatabaseCrud;
public class PeopleDataAccess : IDisposable
{
    private readonly IDbConnection _databaseProvider;
    public PeopleDataAccess(IDbConnection provider)
    {
        if (provider is null) throw new ArgumentNullException(nameof(provider));
        _databaseProvider = provider;
    }

    public void Dispose()
    {
        _databaseProvider.Dispose();
        GC.Collect();
    }

    public bool EnsureDatabaseExist()
    {

        try
        {
            _databaseProvider.Query("Create Table People (Id integer Primary Key NOT NULL UNIQUE , Name Text)");
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    public void DropData()
    {
        _databaseProvider.Query("DROP TABLE PEOPLE");
    }

    public IEnumerable<Person> GetAllPeople()
    {
        return _databaseProvider.Query<Person>("SELECT * From People");
    }

    public IEnumerable<Person> GetPeoplebyName(string name)
    {
        return _databaseProvider.Query<Person>($"Select * From People Where name =@name", new { name });
    }
    public IEnumerable<Person> GetPeoplebyNameBAD(string name)
    {
        return _databaseProvider.Query<Person>($"Select * From People Where name ='{name}'");
    }
    public bool InsertPerson(string name)
    {
        try
        {
            _databaseProvider.Query<Person>("INSERT INTO PEOPLE (Name) Values(@name)", new { name });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public bool DeletePerson(int id)
    {

        var temp = _databaseProvider.QueryFirstOrDefault<Person>("Select * From People Where Id ==@id", new { id });
        if (temp is default(Person)) return false;
        _databaseProvider.Query<Person>("DELETE FROM PEOPLE WHERE Id ==@id", new { id });
        return true;

    }
}
