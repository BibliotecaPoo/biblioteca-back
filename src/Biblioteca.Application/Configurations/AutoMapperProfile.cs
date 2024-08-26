using AutoMapper;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Configurations;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Auth

        CreateMap<LoginDto, Administrador>();

        #endregion

        #region Usuario

        CreateMap<Usuario, UsuarioDto>();
        CreateMap<AdicionarUsuarioDto, Usuario>();
        CreateMap<AtualizarUsuarioDto, Usuario>();

        #endregion

        #region Livro

        CreateMap<Livro, LivroDto>()
            .ForMember(dest => dest.StatusLivro, opt => opt.MapFrom(src => src.StatusLivro.ToString()));
        CreateMap<AdicionarLivroDto, Livro>();
        CreateMap<AtualizarLivroDto, Livro>();

        #endregion

        #region Emprestimo

        CreateMap<Emprestimo, EmprestimoDto>()
            .ForMember(dest => dest.StatusEmprestimo, opt => opt.MapFrom(src => src.StatusEmprestimo.ToString()))
            .ForMember(dest => dest.Usuario, opt => opt.MapFrom(src => src.Usuario))
            .ForMember(dest => dest.Livro, opt => opt.MapFrom(src => src.Livro));

        #endregion
    }
}