using HranaOverHttp.Sdk;

namespace HranaClient.Example;

internal class Program
{
    static void Main(string[] args)
    {
        //format for system env variable
        //databaseName:OrganisationName:Token
        var tursoAuthCread = Environment.GetEnvironmentVariable("TURSO", EnvironmentVariableTarget.Machine)?.Split(':');
        if (tursoAuthCread?.Length != 3) { Console.WriteLine("Cannot get ENV variable. Please set it correctly."); return; }

        var constructUrl = new ConstructUrl(tursoAuthCread[0], tursoAuthCread[1]);
        var token = tursoAuthCread[2];
        var epiService = new HranaService(constructUrl, token);

        var response = epiService.ExecuteRequestsAsync(null,
        [
            new()
            {
                Type = "execute",
                Stmt = new()
                //{ Sql = "CREATE TABLE Person (fullname text)" },
                { Sql = "INSERT INTO Person(fullname) VALUES ('Ibrahim Hussain'), ('Zam zam')" },
            }
        ]).GetAwaiter().GetResult();

        var a = response;
    }
}
