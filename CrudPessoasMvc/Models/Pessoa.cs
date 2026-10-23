// =====================================================================
// MODEL (o "M" do MVC)
// ---------------------------------------------------------------------
// O Model representa os DADOS da aplicação.
// Esta classe "Pessoa" espelha a tabela dbo.Pessoas do banco CrudConsoleDB:
//
//   CREATE TABLE Pessoas (
//       Id    INT IDENTITY PRIMARY KEY,   -> propriedade Id
//       Nome  NVARCHAR(100) NOT NULL,     -> propriedade Nome
//       Idade INT NOT NULL                -> propriedade Idade
//   )
//
// O Entity Framework usa esta classe para transformar cada LINHA da
// tabela em um OBJETO C# (e vice-versa). Isso se chama ORM
// (Object-Relational Mapping / Mapeamento Objeto-Relacional).
// =====================================================================

using System.ComponentModel.DataAnnotations;        // Atributos de validação ([Required], [Range]...)
using System.ComponentModel.DataAnnotations.Schema; // Atributos de mapeamento ([Table], [Key]...)

namespace CrudPessoasMvc.Models
{
    // [Table("Pessoas")] diz ao Entity Framework qual tabela do banco
    // corresponde a esta classe. Sem ele, o EF usaria o nome do DbSet.
    [Table("Pessoas")]
    public class Pessoa
    {
        // [Key] marca a CHAVE PRIMÁRIA da tabela.
        // Como a coluna Id é IDENTITY no SQL Server, o próprio banco gera
        // o valor automaticamente — nós nunca precisamos preenchê-lo ao inserir.
        [Key]
        public int Id { get; set; }

        // [Required]  -> o campo é obrigatório (equivale ao NOT NULL do banco).
        // [StringLength(100)] -> no máximo 100 caracteres (igual ao NVARCHAR(100)).
        // [Display]   -> texto que aparece nos rótulos (labels) das telas.
        // ErrorMessage -> mensagem mostrada ao usuário quando a validação falha.
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        // [Range] -> limita os valores aceitos. Evita idades negativas ou absurdas.
        [Required(ErrorMessage = "A idade é obrigatória.")]
        [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150.")]
        [Display(Name = "Idade")]
        public int Idade { get; set; }
    }
}
