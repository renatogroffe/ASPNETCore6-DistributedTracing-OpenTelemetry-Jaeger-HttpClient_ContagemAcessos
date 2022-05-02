using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using APIConsumoContagem.Clients;
using APIConsumoContagem.Logging;
using APIConsumoContagem.Models;
using APIConsumoContagem.Tracing;

namespace APIConsumoContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ConsumoAPIContagemController : ControllerBase
{
    private readonly ILogger<ConsumoAPIContagemController> _logger;
    private readonly APIContagemClient _contagemClient;
    private readonly ActivitySource _activitySource;

    public ConsumoAPIContagemController(
        ILogger<ConsumoAPIContagemController> logger,
        APIContagemClient contagemClient)
    {
        _activitySource = OpenTelemetryExtensions.CreateActivitySource();
        using var activity =
            _activitySource.StartActivity($"Construtor ({nameof(ConsumoAPIContagemController)})");
        activity!.SetTag("horario", $"{DateTime.Now:HH:mm:ss dd/MM/yyyy}");

        _logger = logger;
        _contagemClient = contagemClient;

    }

    [HttpGet]
    public ResultadoContador Get()
    {
        using var activity =
            _activitySource.StartActivity($"{nameof(Get)} ({nameof(ConsumoAPIContagemController)})");

        var resultado = _contagemClient.ObterDadosContagem();
        _logger.LogValorAtual(resultado.ValorAtual);

        activity!.SetTag("valorContador", resultado.ValorAtual);
        activity!.SetTag("origem", OpenTelemetryExtensions.Local);
        activity!.SetTag("producer", resultado.Producer);
        activity!.SetTag("kernel", resultado.Kernel);
        activity!.SetTag("framework", resultado.Framework);
        activity!.SetTag("mensagem", resultado.Mensagem);

        return resultado;
    }
}