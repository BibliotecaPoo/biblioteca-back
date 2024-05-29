# Biblioteca Acadêmica API ✅

## 🎯 Objetivo
Esse projeto consiste em uma API Rest para gerenciar uma biblioteca acadêmica. Desenvolvido com acesso de administrador, o sistema permite cadastrar alunos, livros e realizar empréstimos ou renovações de livros disponíveis para os alunos.

## 🌐 Funcionalidades
- Registro de alunos pelo administrador.
- Obtenção de um aluno específico ou de todos os alunos cadastrados.
- Cadastro de livros disponíveis na biblioteca pelo administrador.
- Inserção/atualização de imagens de capa para livros.
- Obtenção de um livro específico ou de todos os livros cadastrados.
- Realização de empréstimo de livros para alunos.
- Obtenção de um empréstimo específico ou de todos os empréstimos cadastrados.
- Renovação de empréstimo de livros para alunos.

## 🏛️ Arquitetura
O projeto está organizado em camadas distintas:

- API: Contém os controllers e endpoints da aplicação.
- Application: Responsável pela lógica de negócio e serviços da aplicação.
- Domain: Define as entidades e modelos de domínio da aplicação.
- Infra.Data: Responsável pela persistência dos dados, utilizando o Entity Framework Core com MySQL.

## 💻 Tecnologias e dependências utilizadas
- C# e .NET 6 para o desenvolvimento.
- Entity Framework Core para o mapeamento objeto-relacional.
- MySQL como banco de dados.
- AutoMapper para mapeamento de objetos.
- FluentValidation para validação de dados.
- ScottBrady91.AspNetCore.Identity.Argon2PasswordHasher para hash de senhas.

## ▶️ Como rodar o projeto
1. Clone este repositório.
2. Abra o projeto em sua IDE favorita.
3. Navegue até o arquivo [appsettings.Example.json](src/Biblioteca.API/appsettings.Example.json).
4. Configure a conexão com o banco de dados MySQL na seção ``ConnectionStrings``.
5. Informe na seção ``JwtSettings`` o caminho para armazenar a chave secreta.
6. Informe na seção ``StorageSettings`` o caminho para armazenar imagens.
7. Após terminar a configuração do ``appsettings.Example.json``, lembre-se de modificar a extensão "Example" para o nome do ambiente.
8. Faça o restore dos pacotes NuGet. Use o comando: ``dotnet restore``.
9. Utilize um sistema gerenciador de banco de dados como o MySQL Workbench.
10. Por padrão, será gerado na tabela de administradores um administrador com email e senha pré-definidos. Você precisará dessas informações para realizar a autenticação. Como a senha do administrador está em formato hash, você deve alterá-la.
11. Para alterar a senha do administrador, navegue até o arquivo [20240508162538_AdicionandoAdministradorPadrao.cs](src/Biblioteca.Infra.Data/Migrations/20240508162538_AdicionandoAdministradorPadrao.cs).
12. Informe a nova senha já em formato hash.
13. Certifique-se de que o Entity Framework Core Tools está instalado. Caso não esteja, instale com o comando: ``dotnet tool install --global dotnet-ef``.
14. Aplique as migrações do Entity Framework Core para atualizar o banco de dados. Utilize o comando: ``dotnet ef database update``.
15. Abra o terminal e navegue até a pasta Biblioteca.API.
16. Execute o comando ``dotnet run`` para iniciar a aplicação.
17. Acesse a API documentada pelo Swagger.
