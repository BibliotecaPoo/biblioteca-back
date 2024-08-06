using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Biblioteca.Application.Configuration;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Validators.Administrador;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;

namespace Biblioteca.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly IPasswordHasher<Administrador> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(INotificator notificator, IMapper mapper, IAdministradorRepository administradorRepository,
        IPasswordHasher<Administrador> passwordHasher, IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings) : base(notificator, mapper)
    {
        _administradorRepository = administradorRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<TokenDto?> Login(LoginDto dto)
    {
        if (!await ValidacoesParaLogin(dto))
            return null;

        var administrador = await _administradorRepository.FirstOrDefault(a => a.Email == dto.Email);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(administrador, administrador.Senha, dto.Senha);
        if (resultado != PasswordVerificationResult.Failed)
        {
            return new TokenDto
            {
                Token = await GerarToken(administrador)
            };
        }

        Notificator.Handle("Não foi possível fazer o login.");
        return null;
    }

    private async Task<string> GerarToken(Administrador administrador)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, administrador.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, administrador.Nome));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, administrador.Email));

        var key = await _jwtService.GetCurrentSigningCredentials();
        
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = key
        });

        return tokenHandler.WriteToken(token);
    }

    private async Task<bool> ValidacoesParaLogin(LoginDto dto)
    {
        var administrador = Mapper.Map<Administrador>(dto);
        var validador = new LoginValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(administrador);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        return true;
    }
}