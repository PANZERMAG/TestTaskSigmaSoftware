using Microsoft.AspNetCore.Mvc;
using TestTask.Entities;
using TestTask.Services;

namespace TestTask.Controllers;

[ApiController]
[Route("[controller]")]
public class FormController : Controller
{
    private readonly FormService _formService;

    public FormController(FormService formService)
    {
        _formService = formService;
    }


    [HttpPost]
    public async Task<ActionResult<string>> PostForm([FromForm] UploadFormDto formDtoViewModel)
    {
        var result = await _formService.SaveForm(formDtoViewModel);
        return Ok(result);
    }
}