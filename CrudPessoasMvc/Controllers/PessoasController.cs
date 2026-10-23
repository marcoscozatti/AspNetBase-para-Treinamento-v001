// =====================================================================
// CONTROLLER (o "C" do MVC)
// ---------------------------------------------------------------------
// O Controller recebe as requisições do navegador, conversa com o Model
// (banco de dados via Entity Framework) e escolhe qual View (tela) mostrar.
//
// Fluxo de uma requisição:
//   Navegador -> /Pessoas/Edit/5 -> PessoasController.Edit(5)
//             -> busca a pessoa no banco -> devolve a View "Edit" preenchida
//
// Convenção de rotas (definida no Program.cs):
//   /{controller}/{action}/{id?}
//   Ex.: /Pessoas/Details/3 -> Controller "Pessoas", Action "Details", id = 3
//
// CRUD = Create (criar), Read (ler), Update (atualizar), Delete (excluir)
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudPessoasMvc.Models;

namespace CrudPessoasMvc.Controllers
{
    // Todo controller herda de "Controller" e, por convenção,
    // o nome termina com "Controller" (a rota usa só "Pessoas").
    public class PessoasController : Controller
    {
        // Campo privado que guarda o DbContext (acesso ao banco).
        // "readonly" = só pode ser atribuído no construtor.
        private readonly AppDbContext _context;

        // INJEÇÃO DE DEPENDÊNCIA:
        // Não criamos o AppDbContext com "new". O ASP.NET cria e nos entrega
        // pronto, porque registramos ele no Program.cs (AddDbContext).
        public PessoasController(AppDbContext context)
        {
            _context = context;
        }

        // =============================================================
        // READ (Listar) — GET: /Pessoas  ou  /Pessoas/Index
        // =============================================================
        // "async Task<IActionResult>" = método assíncrono: enquanto espera o
        // banco responder, o servidor fica livre para atender outras pessoas.
        public async Task<IActionResult> Index()
        {
            // Equivale a: SELECT * FROM Pessoas ORDER BY Nome
            var pessoas = await _context.Pessoas
                                        .OrderBy(p => p.Nome)
                                        .ToListAsync();

            // Envia a lista para a View Views/Pessoas/Index.cshtml
            return View(pessoas);
        }

        // =============================================================
        // READ (Detalhes) — GET: /Pessoas/Details/5
        // =============================================================
        // "int? id" -> o "?" indica que o id pode vir vazio (null).
        public async Task<IActionResult> Details(int? id)
        {
            // Se nenhum id foi informado na URL, devolvemos erro 404.
            if (id == null)
            {
                return NotFound();
            }

            // Equivale a: SELECT TOP 1 * FROM Pessoas WHERE Id = @id
            var pessoa = await _context.Pessoas
                                       .FirstOrDefaultAsync(p => p.Id == id);

            // Se não existe pessoa com esse id, também é 404.
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // =============================================================
        // CREATE (Criar) — parte 1: GET /Pessoas/Create
        // Apenas MOSTRA o formulário vazio.
        // =============================================================
        public IActionResult Create()
        {
            return View();
        }

        // =============================================================
        // CREATE (Criar) — parte 2: POST /Pessoas/Create
        // RECEBE os dados digitados no formulário e grava no banco.
        // =============================================================
        // [HttpPost] -> este método só responde a envios de formulário (POST).
        // [ValidateAntiForgeryToken] -> proteção contra ataques CSRF
        //   (garante que o formulário veio do nosso próprio site).
        // [Bind("Nome,Idade")] -> só aceita esses campos do formulário.
        //   O Id fica de fora porque é gerado pelo banco (IDENTITY).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,Idade")] Pessoa pessoa)
        {
            // ModelState.IsValid verifica as regras do Model
            // ([Required], [StringLength], [Range]...).
            if (ModelState.IsValid)
            {
                _context.Add(pessoa);              // Marca o objeto para INSERT
                await _context.SaveChangesAsync(); // Executa o INSERT no banco

                // TempData guarda uma mensagem que sobrevive a UM redirecionamento.
                TempData["Mensagem"] = "Pessoa cadastrada com sucesso!";

                // Redireciona para a listagem (padrão PRG: Post-Redirect-Get,
                // evita que um F5 reenvie o formulário e duplique o registro).
                return RedirectToAction(nameof(Index));
            }

            // Se houve erro de validação, mostra o formulário de novo
            // com os dados digitados e as mensagens de erro.
            return View(pessoa);
        }

        // =============================================================
        // UPDATE (Editar) — parte 1: GET /Pessoas/Edit/5
        // Busca a pessoa e mostra o formulário PREENCHIDO.
        // =============================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // FindAsync busca diretamente pela chave primária.
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // =============================================================
        // UPDATE (Editar) — parte 2: POST /Pessoas/Edit/5
        // Recebe os dados alterados e grava no banco.
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Idade")] Pessoa pessoa)
        {
            // Segurança: o id da URL deve ser o mesmo id do formulário.
            if (id != pessoa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pessoa);           // Marca o objeto para UPDATE
                    await _context.SaveChangesAsync(); // Executa o UPDATE no banco

                    TempData["Mensagem"] = "Pessoa atualizada com sucesso!";
                }
                // Essa exceção ocorre se o registro foi apagado por outra
                // pessoa enquanto este usuário estava editando.
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExiste(pessoa.Id))
                    {
                        return NotFound();
                    }
                    throw; // Outro erro inesperado: deixa o ASP.NET tratar.
                }

                return RedirectToAction(nameof(Index));
            }

            return View(pessoa);
        }

        // =============================================================
        // DELETE (Excluir) — parte 1: GET /Pessoas/Delete/5
        // Mostra uma tela de CONFIRMAÇÃO (nunca apagamos direto num GET).
        // =============================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas
                                       .FirstOrDefaultAsync(p => p.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // =============================================================
        // DELETE (Excluir) — parte 2: POST /Pessoas/Delete/5
        // Executa a exclusão depois que o usuário confirmou.
        // =============================================================
        // [ActionName("Delete")] -> em C# não podemos ter dois métodos com a
        // mesma assinatura (Delete(int)), então o método se chama
        // DeleteConfirmed, mas para o MVC ele continua sendo a action "Delete".
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa != null)
            {
                _context.Pessoas.Remove(pessoa);   // Marca o objeto para DELETE
                await _context.SaveChangesAsync(); // Executa o DELETE no banco

                TempData["Mensagem"] = "Pessoa excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar (privado): verifica se existe pessoa com o id.
        // Equivale a: SELECT CASE WHEN EXISTS(SELECT 1 FROM Pessoas WHERE Id=@id) ...
        private bool PessoaExiste(int id)
        {
            return _context.Pessoas.Any(e => e.Id == id);
        }
    }
}
