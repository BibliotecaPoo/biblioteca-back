using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using AutoMapper;
using Biblioteca.Application.Configurations;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.Email;
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
    private readonly IEmailService _emailService;

    public AuthService(INotificator notificator, IMapper mapper, IAdministradorRepository administradorRepository,
        IPasswordHasher<Administrador> passwordHasher, IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings, IEmailService emailService) : base(notificator, mapper)
    {
        _administradorRepository = administradorRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _emailService = emailService;
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

    public async Task<bool> EsqueceuSenha(string email)
    {
        var administrador = await _administradorRepository.FirstOrDefault(a => a.Email == email);
        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (administrador.PedidoDeRecuperacaoDeSenha == true)
        {
            Notificator.Handle("Já existe um pedido de recuperação de senha em andamento para este administrador.");
            return false;
        }
        
        administrador.CodigoDeRecuperacaoDeSenha = GerarCodigoEsqueceuSenha();
        administrador.TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha = DateTime.Now.AddMinutes(5);
        administrador.PedidoDeRecuperacaoDeSenha = true;
        
        _administradorRepository.Atualizar(administrador);
        if (await _administradorRepository.UnitOfWork.Commit())
        {
            await _emailService.EnviarEmailParaRecuperarSenhaDoAdministrador(administrador);
            return true;
        }

        Notificator.Handle("Não foi possível solicitar a recuperação de senha");
        return false;

    }

    public async Task<bool> AlterarSenha(AlterarSenhaDto dto)
    {
        var administrador = await _administradorRepository
            .ObterAdministradorPorCodigoDeRecuperacaoDeSenha(dto.CodigoParaAlterarSenha);

        if (administrador == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }
        
        if (administrador.TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha.HasValue 
            && administrador.TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha.Value < DateTime.Now)
        {
            Notificator.Handle("O código de alteração de senha expirou.");
            return false;
        }

        if (!string.IsNullOrEmpty(dto.NovaSenha) && dto.NovaSenha != dto.ConfirmarNovaSenha)
        {
            Notificator.Handle("As senhas informadas não coincidem.");
            return false;
        }

        if (!ValidacaoDaSenha(dto.NovaSenha))
        {
            return false;
        }

        administrador.Senha = _passwordHasher.HashPassword(administrador, dto.NovaSenha);
        
        administrador.CodigoDeRecuperacaoDeSenha = null;
        administrador.TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha = null;
        administrador.PedidoDeRecuperacaoDeSenha = false;
        
        _administradorRepository.Atualizar(administrador);
        if (await _administradorRepository.UnitOfWork.Commit())
        {
            return true;
        }
        
        Notificator.Handle("Não foi possível alterar a senha do administrador");
        return false;
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
        var validador = new AdministradorValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(administrador);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        return true;
    }

    private bool ValidacaoDaSenha(string senha)
    {
        var senhaValidator = new SenhaAdministradorValidator();
        var result = senhaValidator.Validate(senha);

        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                Notificator.Handle(error.ErrorMessage);
            }

            return false;
        }

        return true;
    }
    
    private string GerarCodigoEsqueceuSenha()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(2));
    }

}