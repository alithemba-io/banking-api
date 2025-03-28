using System;
using Microsoft.AspNetCore.Mvc;
using BankingApi.DTOs;
using BankingApi.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace BankingApi.Controllers;


[ApiController]
[Route("api/Controller")]
public class AuthController : ControllerBase{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService){
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public IActionResult Login([FromBody] LoginRequestDto request){
        if(string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password)){
            return BadRequest(new { message = "Username and password are required" });
        }

        var token = _authService.GenerateToken(request.Username);
        return Ok(new {Token = token});
    }


}