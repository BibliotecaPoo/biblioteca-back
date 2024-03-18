using AutoMapper;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Auth

        CreateMap<DTOs.Auth.LoginDto, Usuario>().ReverseMap();

        #endregion
        
        #region Usuario

        CreateMap<DTOs.Usuario.UsuarioDto, Usuario>().ReverseMap();
        CreateMap<DTOs.Usuario.AdicionarUsuarioDto, Usuario>().ReverseMap();
        CreateMap<DTOs.Usuario.AtualizarUsuarioDto, Usuario>().ReverseMap();

        #endregion
    }
}