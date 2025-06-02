# OrgaTask API

![.NET](https://img.shields.io/badge/.NET-8-%23512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-8-%23512BD4)
![Swagger](https://img.shields.io/badge/Swagger-UI-%2385EA2D)


## 📌 **Sobre o OrgaTask**  
**Sistema central** para gestão de tarefas, onde usuários podem:  
- Criar/gerenciar tarefas com prioridades e status  
- Acessar de múltiplos frontends (web e desktop)  
- Ter dados sincronizados em tempo real  
 

> Backend do ecossistema OrgaTask - API RESTful para gerenciamento de tarefas


![image](https://github.com/user-attachments/assets/fff9a5bb-4cdd-4c92-9a8f-a70ca60ad3ff)

## 📋 Visão Geral
API central do sistema OrgaTask que fornece endpoints para:
- Autenticação de usuários com JWT
- CRUD de usuários
- CRUD de tarefas com prioridade e status
- Gerenciamento de projetos

## 🌐 Ecossistema OrgaTask
Esta API é consumida por:
- [OrgaTask Blazor WebAssembly](https://github.com/Jabonelas/OrgaTask-Blazor-WebAssembly) (Versão Web)
- [OrgaTask Windows Forms](https://github.com/Jabonelas/OrgaTask-Windows-Forms) (Versão Desktop)

- 📊 Arquitetura do Sistema

![OrganizacaoOrgaTask](https://github.com/user-attachments/assets/bae20b56-ace7-4ef0-8d14-7fe13f1d9d31)
Figura 1: Visão geral da integração entre os componentes do OrgaTask.
A API central (Backend) serve dados para os frontends Web e Desktop.


## 🛠 Tecnologias
- **Core**: .NET 8
- **Banco de Dados**: SQLite
- **ORM (Object Relational Mapping)**: Entity Framework
- **Autenticação**: JWT Bearer Tokens
- **Documentação**: Swagger
- **Padrões Arquiteturais**:
  - **Service Layer**: Separação clara entre controllers e lógica de negócio
  - **Repository Pattern**: Abstração do acesso a dados
  - **Unit of Work**: Gerenciamento transacional e agrupamento de operações em repositórios
  - **DTOs**: para transferência de dados
  - **Injeção de Dependência**: Nativa do .NET (IServiceCollection)

## 🚀 Como Executar
1. Clone o repositório:
   ```bash
   git clone https://github.com/Jabonelas/OrgaTask-API.git
