# 🗓️ Roadmap de Desenvolvimento - ManiaDeLimpezaApp

**Período:** 01/10/2025 a 01/12/2025 (9 semanas)  
**Equipe:** 2 desenvolvedores (1 Frontend + 1 Backend)  
**Arquitetura:** Mobile-first PWA + API REST

---

## 📊 Visão Geral do Cronograma

| Fase | Período | Duração | Descrição |
|------|---------|---------|-----------|
| **MVP** | 01/10 - 12/11 | 6 semanas | Funcionalidades essenciais |
| **Fase 2** | 13/11 - 26/11 | 2 semanas | Agenda e serviços |
| **Fase 3** | 27/11 - 01/12 | 1 semana | Avaliações e CRM |

---

## 🎯 MVP - Funcionalidades Essenciais
**Período:** 01/10/2025 - 12/11/2025 (6 semanas)

### Semana 1 (01/10 - 08/10) - Setup e Autenticação
#### Backend Developer
- [X] Configuração do projeto (.NET Core Web API)
- [X] Estrutura de pastas e arquitetura
- [X] Configuração do Entity Framework
- [X] Implementação de autenticação JWT
- [X] Modelos de dados: User, Company, Employee
- [Andamento] Endpoints de autenticação e autorização

#### Frontend Developer
- [X] Setup do projeto PWA (React)
- [X] Configuração de roteamento
- [X] Design system básico (mobile-first)
- [X] Telas de login e registro
- [ ] Configuração de interceptadores HTTP
- [ ] Integração com API de autenticação

### Semana 2 (09/10 - 15/10) - Gestão de Empresa e Colaboradores
#### Backend Developer
- [ ] **UC01** - API para Modificar Empresa
- [ ] **UC02** - API para cadastrar colaboradores
- [ ] Validações de perfil (Admin vs Colaborador)
- [ ] Middleware de autorização por roles
- [ ] Testes unitários para autenticação

#### Frontend Developer
- [ ] Tela de criação de empresa
- [ ] Tela de cadastro de colaboradores
- [X] Dashboard inicial (navegação)
- [ ] Componentes de formulário reutilizáveis
- [ ] Validações frontend

### Semana 3 (16/10 - 22/10) - Gestão de Clientes
#### Backend Developer
- [ ] **UC04/UC05** - API para cadastro de clientes
- [ ] **UC06** - API para relacionamento do cliente
- [ ] Modelo de dados: Customer, CustomerRelationship
- [ ] Endpoints CRUD para clientes
- [ ] Integração com agenda telefônica (API)

#### Frontend Developer
- [ ] **UC04** - Interface para importar contatos
- [X] **UC05** - Formulário de cadastro manual
- [X] Lista de clientes com busca
- [X] Tela de detalhes do cliente
- [X] **UC06** - Gestão de informações de relacionamento

### Semana 4 (23/10 - 29/10) - Orçamentos (Parte 1)
#### Backend Developer
- [ ] **UC07** - API para criar orçamentos
- [ ] Modelo de dados: Budget, BudgetItem
- [ ] Cálculos automáticos de valores
- [ ] **UC08** - API para campos customizáveis
- [ ] Validações de negócio

#### Frontend Developer
- [ ] **UC07** - Tela de criação de orçamentos
- [ ] Componente de itens dinâmicos
- [ ] Cálculos automáticos no frontend
- [ ] **UC08** - Interface para campos extras
- [ ] Prévia do orçamento

### Semana 5 (30/10 - 05/11) - Orçamentos (Parte 2) e Exportação
#### Backend Developer
- [ ] **UC09** - API para exportação PDF
- [ ] **UC09** - API para exportação de Imagem
- [ ] Geração de templates de orçamento
- [ ] API para configurações de empresa
- [ ] Otimizações de performance

#### Frontend Developer
- [ ] **UC09** - Interface de exportação
- [ ] Prévia de PDF/Imagem
- [ ] Compartilhamento de orçamentos
- [ ] Melhorias de UX
- [ ] Responsividade mobile

### Semana 6 (06/11 - 12/11) - Finalizações MVP
#### Backend Developer
- [ ] **UC03** - API para configurações de orçamento
- [ ] Testes de integração
- [ ] Documentação da API (Swagger)
- [ ] Deploy em ambiente de staging
- [ ] Monitoramento e logs

#### Frontend Developer
- [ ] **UC03** - Tela de configurações
- [ ] Polimentos de UI/UX
- [ ] Testes E2E principais fluxos
- [ ] PWA manifest e service worker
- [ ] Deploy frontend

---

## 🗓️ Fase 2 - Agenda e Serviços
**Período:** 13/11/2025 - 26/11/2025 (2 semanas)

### Semana 7 (13/11 - 19/11) - Agenda de Serviços
#### Backend Developer
- [ ] **UC10** - API para agendamento de serviços
- [ ] **UC11** - API para visualização de agenda
- [ ] Modelo de dados: ServiceSchedule
- [ ] Integração com orçamentos aprovados
- [ ] Validações de conflito de agenda

#### Frontend Developer
- [ ] **UC10** - Interface de agendamento
- [ ] **UC11** - Calendário de serviços
- [ ] Componentes de data/hora
- [ ] Visualização semanal/mensal
- [ ] Notificações de agendamento

### Semana 8 (20/11 - 26/11) - Gestão de Serviços em Andamento
#### Backend Developer
- [ ] **UC12** - API para iniciar serviços
- [ ] **UC13** - API para gerenciar itens em andamento
- [ ] **UC14** - API para finalizar serviços
- [ ] Status de serviços e controle de estado
- [ ] Dashboard de serviços em andamento

#### Frontend Developer
- [ ] **UC12** - Interface para iniciar serviços
- [ ] **UC13** - Dashboard de itens em andamento
- [ ] **UC14** - Finalização de serviços
- [ ] Indicadores visuais de progresso
- [ ] Notificações de status

---

## ⭐ Fase 3 - Avaliações e CRM Proativo
**Período:** 27/11/2025 - 01/12/2025 (1 semana)

### Semana 9 (27/11 - 01/12) - Sprint Final
#### Backend Developer
- [ ] **UC15** - API para links de avaliação
- [ ] **UC16** - API para dashboard de avaliações
- [ ] **UC17-UC21** - APIs para CRM proativo
- [ ] **UC22** - API para templates de mensagens
- [ ] Automações e agendamento de tarefas

#### Frontend Developer
- [ ] **UC15** - Página pública de avaliação
- [ ] **UC16** - Dashboard de avaliações
- [ ] **UC17** - Dashboard de relacionamento
- [ ] **UC22** - Configuração de templates
- [ ] Refinamentos finais e deploy

---

## 🎯 Marcos de Entrega (Milestones)

| Data | Marco | Entregas |
|------|-------|----------|
| **08/10** | M1 - Autenticação | Login, registro, setup completo |
| **22/10** | M2 - Gestão Básica | Empresas, colaboradores, clientes |
| **05/11** | M3 - Orçamentos | Criação e exportação de orçamentos |
| **12/11** | M4 - MVP Completo | Todas funcionalidades essenciais |
| **26/11** | M5 - Fase 2 Completa | Agenda e serviços em andamento |
| **01/12** | M6 - Produto Final | Avaliações e CRM proativo |

---

## 🔧 Stack Tecnológica Recomendada

### Backend
- **.NET 8 Web API** - Framework principal
- **Entity Framework Core** - ORM
- **SQL Server** - Banco de dados
- **JWT** - Autenticação
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validações
- **xUnit** - Testes

### Frontend
- **React 18** - Framework principal
- **TypeScript** - Tipagem
- **Vite** - Build tool
- **React Router** - Roteamento
- **Axios** - HTTP client
- **Material-UI** - Design system
- **PWA** - Progressive Web App

### DevOps
- **Azure DevOps** - CI/CD
- **Docker** - Containerização
- **Azure App Service** - Hospedagem

---

## ⚠️ Riscos e Mitigações

| Risco | Probabilidade | Impacto | Mitigação |
|-------|---------------|---------|-----------|
| Complexidade da exportação PDF | Alta | Médio | Usar bibliotecas prontas (iTextSharp/.NET) |
| Integração agenda telefônica | Média | Alto | Implementar alternativas (importação CSV) |
| Performance em mobile | Média | Alto | Testes constantes em dispositivos reais |
| Escopo creep Fase 3 | Alta | Médio | Definir MVP mínimo para cada UC |

---

## 📋 Definição de Pronto (DoD)

### Para cada funcionalidade:
- [ ] Código revisado e aprovado
- [ ] Testes unitários com cobertura > 80%
- [ ] Documentação atualizada
- [ ] Testado em ambiente de staging
- [ ] Responsivo para mobile
- [ ] Validações de segurança implementadas

### Para cada milestone:
- [ ] Demo funcional gravada
- [ ] Deploy realizado com sucesso
- [ ] Feedback da equipe coletado
- [ ] Ajustes críticos implementados

---

## 👥 Responsabilidades por Sprint

### Reuniões:
- **Daily:** 15min às 9h (Slack/Teams)
- **Planning:** Segunda-feira, 1h
- **Review:** Sexta-feira, 30min
- **Retrospective:** Sexta-feira, 30min

### Comunicação:
- **Bloqueios:** Comunicar imediatamente
- **Mudanças de escopo:** Aprovação conjunta
- **Code review:** Obrigatório para todos os PRs

---

📌 **Observação:** Este roadmap é dinâmico e pode ser ajustado conforme necessário durante o desenvolvimento. Priorize sempre a qualidade e funcionalidade sobre velocidade.
