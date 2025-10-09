# Sistema de Nota Fiscal - Microserviços

Sistema de impressão de nota fiscal desenvolvido com arquitetura de microserviços em .NET 8.

## Arquitetura

O projeto é dividido em 2 microserviços independentes:

- **ServiceEstoque**: Gerencia produtos e controle de saldo
- **ServiceFaturamento**: Gerencia notas fiscais e processamento de impressão

Cada serviço possui seu próprio banco de dados PostgreSQL, garantindo isolamento de dados.

## Tecnologias

- .NET 8
- PostgreSQL
- Entity Framework Core
- Docker & Docker Compose
