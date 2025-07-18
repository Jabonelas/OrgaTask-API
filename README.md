# OrgaTask API

## Visão Geral

OrgaTask API é uma API RESTful desenvolvida para gerenciar tarefas e autenticação de usuários, servindo como backend para múltiplos clientes (Web, Desktop, e em breve Mobile). O projeto foi construído com foco em boas práticas de arquitetura, segurança, e documentação, utilizando tecnologias modernas do ecossistema .NET.

## Tecnologias Utilizadas

- **Core**: .NET 8
- **Banco de Dados**: SQLite
- **ORM**: Entity Framework Core
- **Autenticação**: JWT Bearer Tokens
- **Documentação**: Swagger/OpenAPI

## Padrões Arquiteturais

- **Service Layer**: Separação entre controllers e lógica de negócio
- **Repository Pattern**: Abstração do acesso a dados
- **Unit of Work**: Gerenciamento transacional
- **DTOs**: Transferência de dados
- **Injeção de Dependência**: Nativa do .NET (IServiceCollection)

## Funcionalidades

- Autenticação e autorização via JWT
- CRUD de tarefas (criar, listar, atualizar, excluir)
- Documentação interativa via Swagger
- Validação de dados e tratamento de erros

## Pré-requisitos

- .NET 8 SDK
- SQLite (ou use o banco embutido no projeto)
- Postman ou cURL (para testar a API)

## Como Executar o Projeto

1. Clone o repositório:
```bash
git clone https://github.com/Jabonelas/OrgaTask-API.git
cd OrgaTask-API
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Configure o banco de dados:

O arquivo do banco de dados (`Banco.db`) já está incluído no projeto.

Caso necessário, aplique as migrações:
```bash
dotnet ef database update
```

4. Execute a API:
```bash
dotnet run
```

5. Acesse a documentação Swagger em: https://localhost:7091/swagger


## Exemplo de Uso

Autenticação

Envie uma requisição POST para /api/usuarios/login:

```bash
curl -X 'POST' \
  'https://localhost:7091/api/usuarios/login' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "login": "string",
  "senha": "string"
}'
```

Resposta:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

Listar Tarefas

Use o token JWT no header Authorization:

```bash
curl -X 'GET' \
  'https://localhost:7091/api/tarefas' \
  -H 'accept: text/plain' \
  -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR...'
```

Resposta:

```json
[
  {
    "id": 255,
    "titulo": "Otimizar consultas SQL",
    "descricao": "Analisar e melhorar performance das queries do sistema",
    "prioridade": "Média",
    "prazo": 8,
    "status": "Pendente",
    "data": null
  }
]
```


<img width="1448" height="993" alt="image" src="https://github.com/user-attachments/assets/349e5bf2-9ac4-45c6-a0ba-37061ea3efe9" />



![image](https://github.com/user-attachments/assets/1fd2ed5d-e121-4ddf-bb80-b9e3e97990ee)

<img width="1446" height="935" alt="image" src="https://github.com/user-attachments/assets/0bbb4fc8-4aaa-47b3-b8b8-6341f29f8d68" />


Contribuições

Sinta-se à vontade para abrir issues ou enviar pull requests. Todas as contribuições são bem-vindas!


