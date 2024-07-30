using Models;
using OrleansGeneratorBug;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorageAsDefault();
});

using var app = builder.Build();

app.MapGet("/", static () => "Welcome to Orleans!");

app.MapGet("/account/{id:required}/deposit/{amount:required}",
    static async (IGrainFactory grains, string id, decimal amount) =>
    {
        var grain = grains.GetGrain<IAccountingGrain>(id);

        var balance = await grain.Deposit(new Deposit(amount));

        return Results.Ok(balance);
    });

app.MapGet("/account/{id:required}/deposit2/{amount:required}",
    static async (IGrainFactory grains, string id, decimal amount) =>
    {
        var grain = grains.GetGrain<IAccountingGrain>(id);

        var balance = await grain.Deposit2(new Deposit2(amount));

        return Results.Ok(balance);
    });

app.Run();