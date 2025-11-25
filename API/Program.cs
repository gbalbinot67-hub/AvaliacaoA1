using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();


builder.Services.AddCors(options =>
    options.AddPolicy("AcessoTotal",
        configs => configs
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod())
);

var app = builder.Build();

app.MapGet("/", () => "Guilherme Balbinot");

app.MapGet("/api/chamado/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Chamados.Any())
    {
        return Results.Ok(ctx.Chamados.ToList());
    }
    return Results.NotFound("Nenhum chamado encontrado");
});


app.MapPost("/api/chamado/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Chamado chamado) =>
{
    chamado.Status = "Aberto"; 
    
    ctx.Chamados.Add(chamado);
    ctx.SaveChanges();
    return Results.Created("", chamado);
});


app.MapPatch("/api/chamado/alterar/{id}", ([FromServices] AppDataContext ctx, [FromRoute] string id) =>
{
    Chamado? chamado = ctx.Chamados.Find(id);
    
    if (chamado is null) return Results.NotFound("Chamado não encontrado");
    {
        chamado.Status = "Resolvido";
    }

    ctx.Chamados.Update(chamado);
    ctx.SaveChanges();
    return Results.Ok(chamado);
});


app.MapGet("/api/chamado/naoresolvidos", ([FromServices] AppDataContext ctx) =>
{
    var lista = ctx.Chamados
        .Where(x => x.Status == "Aberto" || x.Status == "Em atendimento")
        .ToList();
        
    return Results.Ok(lista);
});


app.MapGet("/api/chamado/resolvidos", ([FromServices] AppDataContext ctx) =>
{
    var lista = ctx.Chamados
        .Where(x => x.Status == "Resolvido")
        .ToList();

    return Results.Ok(lista);
});

app.UseCors("AcessoTotal");
app.Run();
