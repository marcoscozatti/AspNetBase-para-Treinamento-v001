// =====================================================================
// DbContext — a "ponte" entre o C# e o banco de dados
// ---------------------------------------------------------------------
// O DbContext é a classe principal do Entity Framework Core.
// Ele é responsável por:
//   1. Abrir e fechar a conexão com o banco (CrudConsoleDB);
//   2. Traduzir comandos C# (LINQ) em comandos SQL (SELECT, INSERT...);
//   3. Acompanhar as alterações feitas nos objetos (Change Tracking)
//      e gravá-las no banco quando chamamos SaveChanges().
// =====================================================================

using Microsoft.EntityFrameworkCore;

namespace CrudPessoasMvc.Models
{
    // Nossa classe herda de DbContext para ganhar todos esses recursos.
    public class AppDbContext : DbContext
    {
        // Construtor: recebe as "opções" (qual banco usar, string de conexão...).
        // Essas opções são configuradas no Program.cs e entregues aqui
        // automaticamente pela Injeção de Dependência do ASP.NET.
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet<Pessoa> representa a TABELA Pessoas.
        // Por meio dele fazemos as consultas e alterações, por exemplo:
        //   _context.Pessoas.ToList()      -> SELECT * FROM Pessoas
        //   _context.Pessoas.Add(pessoa)   -> INSERT INTO Pessoas ...
        //   _context.Pessoas.Remove(pessoa)-> DELETE FROM Pessoas ...
        public DbSet<Pessoa> Pessoas { get; set; }
    }
}
