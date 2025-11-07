# 📚 GerenciadorDeLivraria

API REST desenvolvida em **.NET 5** para o gerenciamento de livros de uma livraria.  
O projeto implementa um **CRUD completo** (Create, Read, Update, Delete) com validações, documentação interativa via **Swagger**, suporte a **SQLite**, **paginação**, **ordenação** e **filtros**.

---

## 🚀 Tecnologias Utilizadas

- **.NET 5 (C#)**  
- **Entity Framework Core 5.0.17**  
- **SQLite** (banco de dados local)  
- **Swagger / Swashbuckle** (documentação da API)  
- **AutoMapper** (mapeamento entre entidades e DTOs)  

---

## 📦 Estrutura do Projeto

```
GerenciadorDeLivraria/
├── Controllers/
│   └── BooksController.cs        → Endpoints CRUD dos livros
├── Data/
│   └── AppDbContext.cs           → Contexto EF Core (SQLite)
├── Domain/
│   ├── BaseEntity.cs             → Classe base com CreatedAt/UpdatedAt
│   ├── Book.cs                   → Entidade principal de Livro
│   └── Genre.cs                  → Enum com gêneros válidos
├── DTOs/
│   └── BookDtos.cs               → DTOs de entrada e saída
├── Mapping/
│   └── MappingProfile.cs         → Configuração AutoMapper
├── Swagger/
│   ├── Examples.cs               → Exemplos no Swagger
│   └── PaginationHeadersOperationFilter.cs → Headers de paginação
├── Startup.cs / Program.cs       → Configuração geral do app
├── appsettings.json              → Conexão com SQLite
└── README.md                     → Este arquivo
```

---

## 🛠️ Como Executar o Projeto

### 📌 Pré-requisitos
- **Visual Studio 2019** (ou superior) com o workload **.NET 5**
- **SDK do .NET 5** instalado  
- **SQLite** (opcional, pois o EF cria o banco automaticamente)

---

### 🧩 Passos para rodar

1. **Clonar o repositório**
   ```bash
   git clone https://github.com/seu-usuario/GerenciadorDeLivraria.git
   cd GerenciadorDeLivraria
   ```

2. **Abrir o projeto no Visual Studio 2019**
   - Vá em **Arquivo → Abrir → Projeto/Solução**
   - Selecione o arquivo `GerenciadorDeLivraria.sln`

3. **Restaurar pacotes NuGet**
   - Menu: **Tools → NuGet Package Manager → Package Manager Console**
   - Execute:
     ```powershell
     Update-Package -reinstall
     ```

4. **Aplicar as migrações do banco de dados**
   > (gera o arquivo `GerenciadorDeLivraria.db`)
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```

5. **Executar o projeto**
   - Pressione **F5** ou clique em **Iniciar Depuração**
   - O navegador abrirá automaticamente com o Swagger.

---

## 🌐 Visualizando o Portfólio (Swagger UI)

Após iniciar o projeto, acesse a URL abaixo no navegador:

```
https://localhost:<porta>/swagger
```

> A porta é exibida no console de execução, ex.:  
> `Now listening on: https://localhost:7043`

Você verá uma **documentação interativa** com todos os endpoints da API:

| Método | Endpoint | Descrição |
|--------|-----------|-----------|
| **POST** | `/api/books` | Cadastrar um novo livro |
| **GET** | `/api/books` | Listar todos os livros (filtros, paginação, ordenação) |
| **GET** | `/api/books/{id}` | Buscar um livro pelo ID |
| **PUT** | `/api/books/{id}` | Atualizar informações de um livro |
| **DELETE** | `/api/books/{id}` | Excluir um livro |

> Cada endpoint possui exemplos automáticos e modelos de requisição/resposta no Swagger UI.

---

## 🔎 Exemplos de Uso (Swagger)

### ➕ Criar um Livro
**POST** `/api/books`
```json
{
  "title": "Engenharia de Dados na Prática",
  "author": "Anderson Guastuci",
  "genre": "tecnologia",
  "price": 120.50,
  "stock": 12
}
```

### 📄 Listar Livros
**GET** `/api/books?title=code&page=1&pageSize=10&sortBy=price&sortDir=desc`

### ✏️ Atualizar Livro
**PUT** `/api/books/{id}`
```json
{
  "title": "Engenharia de Dados Moderna",
  "author": "Anderson Guastuci",
  "genre": "tecnologia",
  "price": 135.00,
  "stock": 15
}
```

### ❌ Excluir Livro
**DELETE** `/api/books/{id}`

---

## 🧠 Regras de Negócio

- `title` e `author` são obrigatórios e **únicos** (não pode haver duplicatas).  
- `price` e `stock` devem ser **≥ 0**.  
- `genre` deve estar entre os valores válidos (`ficção`, `romance`, `mistério`, `tecnologia`, etc.).  
- Campos de auditoria:
  - `CreatedAt` → preenchido na criação.  
  - `UpdatedAt` → atualizado a cada modificação.  

---

## 📑 Status Codes

| Código | Significado | Quando ocorre |
|--------|--------------|----------------|
| **200** | OK | Sucesso nas consultas |
| **201** | Created | Livro criado com sucesso |
| **204** | No Content | Atualização/Exclusão sem retorno |
| **400** | Bad Request | Dados inválidos |
| **404** | Not Found | Livro não encontrado |
| **409** | Conflict | Duplicidade (mesmo título e autor) |
| **500** | Internal Server Error | Erro inesperado no servidor |

---

## 🧭 Paginação e Ordenação

Parâmetros disponíveis no `GET /api/books`:

| Parâmetro | Tipo | Padrão | Descrição |
|------------|------|---------|-----------|
| `page` | int | 1 | Página atual |
| `pageSize` | int | 20 | Itens por página (máx. 100) |
| `sortBy` | string | `title` | Campo de ordenação |
| `sortDir` | string | `asc` | Direção (`asc` ou `desc`) |

> Headers retornados:
> - `X-Total-Count` → total de registros  
> - `X-Page` → página atual  
> - `X-Page-Size` → tamanho da página  

---

## 💾 Banco de Dados

O projeto usa **SQLite**.  
O arquivo do banco (`GerenciadorDeLivraria.db`) é criado automaticamente após o comando `Update-Database`.

Para visualizar o conteúdo:
- Use a extensão **SQLite Viewer** no VS Code, ou
- Abra com o programa **DB Browser for SQLite**.

---

## 🧑‍💻 Autor

**Anderson Guastuci**  
