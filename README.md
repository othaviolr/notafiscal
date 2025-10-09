Sistema de Nota Fiscal - Microserviços
Sistema de impressão de nota fiscal desenvolvido com arquitetura de microserviços em .NET 8.
Arquitetura
O projeto é dividido em 2 microserviços independentes:

ServiceEstoque: Gerencia produtos e controle de saldo
ServiceFaturamento: Gerencia notas fiscais e processamento de impressão

Cada serviço possui seu próprio banco de dados PostgreSQL, garantindo isolamento de dados.

Estoque: http://localhost:5001/swagger
Faturamento: http://localhost:5002/swagger

Funcionalidades
ServiceEstoque (porta 5001)

Cadastrar produtos
Listar produtos
Controlar saldo (disponível e reservado)
Reservar produtos
Confirmar ou compensar reservas

ServiceFaturamento (porta 5002)

Cadastrar nota fiscal
Adicionar produtos na nota
Listar notas fiscais
Imprimir nota fiscal

Fluxo de Impressão
O sistema usa o padrão Saga para garantir consistência entre os microserviços:

Faturamento reserva produtos no Estoque
Estoque valida saldo e cria reserva temporária
Faturamento confirma a impressão
Se tudo ok: Estoque baixa definitivamente e nota é fechada
Se falhar: Estoque compensa automaticamente (devolve o saldo)

Simulação de Falha
O endpoint de confirmação tem 50% de chance de falhar propositalmente para demonstrar a recuperação automática do sistema através da compensação.
Controle de Concorrência
Ambos os serviços implementam controle otimista de concorrência usando o campo Version nas entidades principais.

Estrutura do Projeto
solution/
├── ServiceEstoque/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Api/
├── ServiceFaturamento/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Api/
└── docker-compose.yml
