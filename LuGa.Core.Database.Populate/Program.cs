using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LuGa.Core.Database.Migrations
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string environment = Environment.GetEnvironmentVariable(Constants.Environment);

            if (String.IsNullOrWhiteSpace(environment))
                throw new ArgumentNullException("Environment not found in:" + Constants.Environment);

            Debug.WriteLine("Environment: {0}", environment);

            // all passwords should be stored in 
            // %APPDATA%\microsoft\UserSecrets\luga\secrets.json
            // https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            var cfg = builder.Build();

            var connectionString = cfg.GetConnectionString(Constants.ConnectionString);

            if (String.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("Connection string not found: " + Constants.ConnectionString);
            
            var upgrader = DeployChanges
                    .To
                    .MySqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();
            
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
            #if DEBUG   
                Console.ReadLine();
            #endif
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
