🛡️ PENTDRIVEApi - Sistema de Vendas e Gestão de Estoque
Uma API RESTful desenvolvida em ASP.NET Core 9.0 para gerenciamento de produtos, controle de estoque (Movimentacoes) e processamento de vendas, incluindo simulação de gateway de pagamento e autenticação de usuários (JWT).

🚀 Tecnologias UtilizadasFramework: 
.NET 9.0 (ASP.NET Core)
Linguagem: C#
Banco de Dados: MySQL (Gerenciado pelo Entity Framework Core)
Padrão de Segurança: JSON Web Token (JWT)
Documentação: Swagger/OpenAPI

📦 Estrutura da APIA aplicação é dividida em Controllers, cada um responsável por um domínio específico:

Domínio            Controller                 Funcionalidades Principais

Vendas       VendasController              Processamento de vendas (ProcessarPagamento com transação atômica), consulta de vendas.

Estoque      MovimentacoesController       Registro de entradas e saídas de estoque, histórico de movimentações.

Produtos     ProductsController            CRUD (Criação, Leitura, Atualização, Exclusão) de produtos, controle de estoque.

Usuário      UsuarioController             Registro de novos usuários, autenticação (LoginRequest), geração de token JWT.

Endpoint de Vendas em Destaque

O endpoint principal de vendas garante a integridade dos dados:
POST /api/Vendas/ProcessarPagamento
Simula a aprovação/reprovação de um cartão de crédito (CVV inválido resulta em erro).
Executa uma transação atômica para garantir que a venda, a baixa no estoque do produto e o registro da movimentação de estoque ocorram ou falhem juntos.

🛠️ Pré-requisitos
Para executar este projeto localmente, você precisa ter instalado:
.NET 9.0 SDK ou superior.
Um ambiente de desenvolvimento (Visual Studio Code, Visual Studio ou JetBrains Rider).
Um servidor de banco de dados configurado (conforme a string de conexão em appsettings.json).

⚙️ Configuração e Instalação
1. Clonar o RepositórioBashgit clone https://docs.github.com/pt/repositories/creating-and-managing-repositories/quickstart-for-repositories
cd PENTDRIVEApi

2. Configurar o Banco de DadosCertifique-se de que sua DefaultConnection no arquivo appsettings.json esteja configurada corretamente para seu ambiente de banco de dados (SQL Server, MySQL, etc.).

3. Executar as MigrationsUse as ferramentas do Entity Framework Core para aplicar as migrations e criar o banco de dados:Bashdotnet ef database update

4. Instalar DependênciasGaranta que todas as dependências estejam restauradas:Bashdotnet restore

▶️ Como Rodar o Projeto
Você pode iniciar o projeto diretamente do terminal: dotnet run
A API estará acessível em: http://localhost:5031 (ou a porta configurada no seu launchSettings.json).

🌐 Documentação (Swagger)
A documentação interativa da API estará disponível no seu navegador após iniciar o projeto:http://localhost:5031/swagger

🔒 Autenticação
Todos os endpoints (exceto o login e registro de usuário) requerem autenticação ([Authorize]).
1. Obter Token: Utilize o endpoint POST /api/Usuario/login com suas credenciais para receber um token JWT.

2. Autorizar: No Swagger, clique no botão Authorize e insira o token no formato Bearer [SEU TOKEN].

✍️ Autor
[Queiroz]
[https://github.com/V-Queiroz]
[https://www.linkedin.com/in/vinicius-queiroz-8978b0204/]