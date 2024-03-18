using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Application.Notifications;
using Biblioteca.Core.Enum;
using Biblioteca.Core.Extensions;
using Biblioteca.Core.Settings;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Validators.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetDevPack.Security.Jwt.Core.Interfaces;

namespace Biblioteca.Application.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(INotificator notificator, IMapper mapper, IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher, IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings) : base(notificator, mapper)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<UsuarioDto?> Registrar(AdicionarUsuarioDto dto)
    {
        if (!await ValidacoesParaRegistrarUsuario(dto))
            return null;

        var registrarUsuario = Mapper.Map<Usuario>(dto);
        registrarUsuario.Senha = _passwordHasher.HashPassword(registrarUsuario, dto.Senha);

        _usuarioRepository.Adicionar(registrarUsuario);
        return await CommitChanges() ? Mapper.Map<UsuarioDto>(registrarUsuario) : null;
    }

    public async Task<TokenDto?> Login(LoginDto dto)
    {
        if (!await ValidacoesParaLogin(dto))
            return null;

        var usuario = await _usuarioRepository.ObterPorEmail(dto.Email);
        if (usuario == null || !usuario.Ativo)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, dto.Senha);
        if (resultado != PasswordVerificationResult.Failed)
        {
            return new TokenDto
            {
                Token = await GerarToken(usuario)
            };
        }

        Notificator.Handle("Não foi possível fazer o login.");
        return null;
    }

    public async Task<string> GerarToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nome));
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));

        if ((bool)(!usuario.SuperUsuario)!)
        {
            claimsIdentity.AddClaim(new Claim("TipoUsuario", ETipoUsuario.Comum.ToDescriptionString()));
        }
        else
        {
            claimsIdentity.AddClaim(new Claim("TipoUsuario", ETipoUsuario.Administrador.ToDescriptionString()));
        }

        var key = await _jwtService.GetCurrentSigningCredentials();
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Emissor,
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = key,
            Audience = _jwtSettings.ComumValidoEm
        });

        return tokenHandler.WriteToken(token);
    }

    private async Task<bool> ValidacoesParaRegistrarUsuario(AdicionarUsuarioDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        var validador = new ValidadorParaAdicionarUsuario();

        var resultadoDaValidacao = await validador.ValidateAsync(usuario);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        var usuarioComEmailExistente = await _usuarioRepository.ObterPorEmail(usuario.Email);
        if (usuarioComEmailExistente != null)
        {
            Notificator.Handle("Já existe um usuário cadastrado com o email informado.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaLogin(LoginDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        var validador = new ValidadorParaLogin();

        var resultadoDaValidacao = await validador.ValidateAsync(usuario);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        return true;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _usuarioRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}