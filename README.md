# 🚀 Nova Sondagem Back End

> Projeto backend desenvolvido em **.NET 10**, responsável pela exibição de desempenho de alunos no SONDAGEM.

---

## 🧱 Tecnologias Utilizadas

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-6DB33F)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14-336791?logo=postgresql)
![ElasticSearch](https://img.shields.io/badge/ElasticSearch-Search-005571?logo=elasticsearch)

---

## 📌 Visão Geral

Este projeto foi construído utilizando **.NET 10**, seguindo boas práticas de desenvolvimento backend, com foco em **performance**, **manutenibilidade** e **escalabilidade**.

A aplicação realiza integração com sistemas externos para **consulta e indexação de dados**, centralizando informações acadêmicas.

---

## 🏗️ Arquitetura

- **ORM:** Entity Framework Core
- **Banco de Dados:** PostgreSQL
- **Busca de dados:**
  - 📚 **EOL** (fonte primária de dados)
  - 🔎 **ElasticSearch** (indexação e buscas avançadas)

A aplicação consome dados do EOL, persiste informações relevantes no banco e mantém índices no ElasticSearch para consultas performáticas.

---

## 🗄️ Banco de Dados

- **PostgreSQL**
- Migrations gerenciadas via **Entity Framework Core**
- Suporte a versionamento de schema

---

## 🔄 Fluxo de Dados

```mermaid
flowchart LR
    EOL[EOL] --> API[API .NET 10]
    API --> DB[(PostgreSQL)]
    API --> ES[ElasticSearch]

