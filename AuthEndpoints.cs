using BancoApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace BancoApi;

public record RegistrarUserRequest(string Username, string Password);

public record LoginRequest(string Username, string Password);

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/registrar", async (AuthService service, RegistrarUserRequest request) =>
        {
            var user = await service.Registrar(request.Username, request.Password);
            return Results.Ok(new { message = "Usuario criado com sucesso!", id = user.Id });
        });

        app.MapPost("/login", async (AuthService service, LoginRequest request) =>
        {
            var token = await service.Login(request.Username, request.Password);

            return Results.Ok(new { token = token });
        });

    }
}