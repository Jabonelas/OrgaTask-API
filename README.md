# OrgaTask API

![.NET](https://img.shields.io/badge/.NET-8-%23512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-8-%23512BD4)
![Swagger](https://img.shields.io/badge/Swagger-UI-%2385EA2D)

> Backend do ecossistema OrgaTask - API RESTful para gerenciamento de tarefas

<div align="center">
![image](https://github.com/user-attachments/assets/fff9a5bb-4cdd-4c92-9a8f-a70ca60ad3ff)

  
  <img src="docs/api-screenshot.png" alt="Swagger UI" width="600">
  
</div>

## 📋 Visão Geral
API central do sistema OrgaTask que fornece endpoints para:
- Autenticação de usuários com JWT
- CRUD de usuários
- CRUD de tarefas com prioridade e status
- Gerenciamento de projetos

## 🌐 Ecossistema OrgaTask
Esta API é consumida por:
- [OrgaTask Blazor WebAssembly](https://github.com/Jabonelas/OrgaTask-Blazor-WebAssembly) (Web)
- [OrgaTask Windows Forms](https://github.com/Jabonelas/OrgaTask-Windows-Forms) (Desktop)

## 🛠 Tecnologias
- .NET 8
- Entity Framework Core
- SQLite
- JWT para autenticação
- Swagger para documentação
- Injeção de Dependência nativa
- Design Patterns (Repository, Unit of Work)

