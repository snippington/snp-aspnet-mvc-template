using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers;

public class TileListController : Controller
{
    private readonly ILogger<TileListController> _logger;

    public TileListController(ILogger<TileListController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/tilelist")]   
    [HttpGet("/tilelist/index")]
    public IActionResult Index()
    {
        return View();
    }
}
