using RefactoringChallenge.Bussiness.Extensions;
using RefactoringChallenge.Data.Configuration;
using RefactoringChallenge.Data.Extensions;
using RefactoringChallenge.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

builder.Services.AddDatabase();
builder.Services.AddBussiness();
builder.Services.AddInfrastructure();

var host = builder.Build();
host.Run();