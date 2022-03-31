
namespace DatabaseCrud.Models;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Person()
    {

    }

    public Person(string name)
    {
        this.Name = name;
    }
}
