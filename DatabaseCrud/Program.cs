using DatabaseCrud;
using DatabaseCrud.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Data;
using Cocona;

//Read configuration from appsettings.Json and build 
IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

//Get the connection string from the appsettings.json
string connectionString = config.GetConnectionString("Default");

//Instantiate The Database Provider 
IDbConnection provider = new SqliteConnection(connectionString);

//Injecting the db provider to PeopleDataAccess
using PeopleDataAccess db = new PeopleDataAccess(provider);

//Ensure the database exists
db.EnsureDatabaseExist();

//Starting Cocona console CLI helper library 
CoconaApp app = CoconaApp.CreateBuilder().Build();

//method to print people data
void PrintPeople(IEnumerable<Person> people)
{
    if (people.Count() == 0)
    {
        Console.WriteLine("No entry found");
        return;
    }
    Console.WriteLine("{0,-3}|{1}", "Id", "Name");
    foreach (Person person in people)
        Console.WriteLine("{0,-3:00}|{1}", person.Id, person.Name);
    Console.WriteLine();
}

app.AddCommand("Add", (string name) =>
{
    bool result = db.InsertPerson(name);
    if (result)
        Console.WriteLine($"{name} Has been added");
    else
        Console.WriteLine("Operation Failed");
}).WithDescription("Add new name to people database");



app.AddCommand("Remove", (int id) =>
{
    bool result = db.DeletePerson(id);
    if (result)
        Console.WriteLine($"Entry with the ID:{id} has been removed");
    else
        Console.WriteLine("Operation failed: no entry match the given id");
}).WithDescription("Remove person from people database by the provided id");




app.AddCommand("List", () =>
{
    var people = db.GetAllPeople();
    PrintPeople(people);
}).WithDescription("List every person in the people database");




app.AddCommand("Purge", () =>
{
    Console.WriteLine("Are you sure you want to remove every entry in the database?\nY/N");
    ConsoleKey key = Console.ReadKey().Key;
    Console.WriteLine();
    if (key == ConsoleKey.Y)
    {
        db.DropData();
        Console.WriteLine("Removed All Entries");
    }

}).WithDescription("Remove every entry from the database");




app.AddCommand("GenMock", (short amount) =>
{
    if (amount > 1000 || amount <= 0)
    {
        Console.WriteLine("Invalid Amount");
        return;
    }
    var names = File.ReadAllLines("Names.csv").OrderBy(x => Guid.NewGuid()).Take(amount);
    foreach (string name in names)
        db.InsertPerson(name);
    Console.WriteLine($"Successfully generated { amount } random entry");

}).WithDescription("Generate mock data by given amount. the amount cannot bypass 1000 or be less than 0");





app.AddCommand("FindByName", (string name) =>
{
    var people = db.GetPeoplebyName(name);
    PrintPeople(people);

}).WithDescription("Show people's names that match the provided name");


app.AddCommand("FindByNameInject", (string name) =>
{
    var people = db.GetPeoplebyNameBAD(name);
  
    PrintPeople(people);
}).WithDescription("Show people's names that match the provided name with the ability to execute SQL injections");










//Running the CLI helper lib

app.Run();

