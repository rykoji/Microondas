using Microondas.WebApplication.Exceptions;
using Microondas.WebApplication.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Microondas.Test.Domain.ApiTests;

public class ExceptionMiddlewareTest
{
    [Fact]
    public async Task InvokeAsync_SemExcecao_PassaParaProximoMiddleware()
    {
        var chamado = false;
        RequestDelegate next = _ => { chamado = true; return Task.CompletedTask; };
        var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);

        await middleware.InvokeAsync(new DefaultHttpContext());

        Assert.True(chamado);
    }

    [Fact]
    public async Task InvokeAsync_BusinessException_RetornaStatus400()
    {
        RequestDelegate next = _ => throw new BusinessException("erro de negócio");
        var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(400, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ExcecaoGenerica_RetornaStatus500()
    {
        RequestDelegate next = _ => throw new Exception("erro genérico");
        var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(500, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_BusinessException_RespostaContemMensagem()
    {
        const string mensagem = "regra de negócio violada";
        RequestDelegate next = _ => throw new BusinessException(mensagem);
        var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Contains(mensagem, body);
    }

    [Fact]
    public async Task InvokeAsync_UnauthorizedAccessException_RetornaStatus401()
    {
        RequestDelegate next = _ => throw new UnauthorizedAccessException("acesso negado");
        var middleware = new ExceptionMiddleware(next, NullLogger<ExceptionMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(401, context.Response.StatusCode);
    }
}
