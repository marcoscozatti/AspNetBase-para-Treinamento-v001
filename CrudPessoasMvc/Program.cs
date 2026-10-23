// =====================================================================
// Program.cs — PONTO DE PARTIDA da aplicação
// ---------------------------------------------------------------------
// Aqui (1) registramos os SERVIÇOS que a aplicação usa e
//      (2) configuramos o PIPELINE (o caminho que cada requisição percorre).
// =====================================================================

using Microsoft.EntityFrameworkCore;
using CrudPessoasMvc.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// 1) REGISTRO DE SERVIÇOS (Injeção de Dependência)
// ---------------------------------------------------------------------

// Habilita o padrão MVC: Controllers + Views.
builder.Services.AddControllersWithViews();

// Registra o nosso DbContext (Entity Framework) e diz:
//   - usar o SQL Server (UseSqlServer);
//   - com a string de conexão "CrudConsoleDB" que está no appsettings.json.
// A partir daqui, qualquer Controller que pedir um AppDbContext no
// construtor vai recebê-lo pronto (uma instância nova a cada requisição).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrudConsoleDB")));

var app = builder.Build();

// ---------------------------------------------------------------------
// 2) PIPELINE DE REQUISIÇÕES (middlewares, executados em ordem)
// ---------------------------------------------------------------------

// Em produção, erros vão para uma página amigável em vez de mostrar detalhes técnicos.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: obriga o navegador a usar HTTPS. Padrão de 30 dias.
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redireciona HTTP -> HTTPS
app.UseStaticFiles();      // Permite servir arquivos da pasta wwwroot (CSS, JS, imagens)

app.UseRouting();          // Descobre qual Controller/Action atende a URL

app.UseAuthorization();

// Rota padrão: /{controller}/{action}/{id?}
// Definimos "Pessoas/Index" como página inicial, então ao abrir o site
// o usuário já cai direto na listagem do CRUD.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pessoas}/{action=Index}/{id?}");

app.Run(); // Inicia o servidor web
