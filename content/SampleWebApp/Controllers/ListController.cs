using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers;

public class ListController : Controller
{
    private readonly ILogger<ListController> _logger;

    public ListController(ILogger<ListController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/list")]   
    [HttpGet("/list/index")]
    public IActionResult Index()
    {
        return View();
    }
}
