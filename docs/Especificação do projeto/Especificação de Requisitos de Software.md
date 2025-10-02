# 📑 Especificação de Requisitos de Software (ERS)  
**Projeto:** *ManiaDeLimpezaApp* (nome temporário)  
**Versão:** 1.0  
**Data:** 01/10/2025
**Autor:** Welber Reis

---

## 1. Introdução
### 1.1 Propósito
Este documento tem como objetivo descrever de forma detalhada os requisitos funcionais e não funcionais do **ManiaDeLimpezaApp**, um CRM mobile-first para prestadores de serviços em geral.  

### 1.2 Escopo
O sistema permitirá que **administradores e colaboradores**:  
- Cadastrem empresas, clientes e orçamentos.  
- Organizem serviços em andamento e agendamentos.  
- Coletem avaliações de clientes.  
- Recebam recomendações de contato para fortalecer relacionamento.  

O sistema terá suporte a exportação de orçamentos em **PDF** e **Imagem**, além de dashboards para acompanhamento de clientes, serviços e relacionamento.  

### 1.3 Definições, Acrônimos e Abreviações
- **CRM**: Customer Relationship Management.  
- **MVP**: Produto Mínimo Viável.  
- **Administrador**: usuário que cria a empresa e adiciona colaboradores.  
- **Colaborador**: usuário que gerencia clientes, orçamentos e serviços.  

---

## 2. Descrição Geral
### 2.1 Perspectiva do Produto
O sistema será desenvolvido como **aplicativo mobile-first com painel administrativo**. O cliente final não acessa diretamente o sistema, mas interage por meio de:  
- Orçamentos enviados (PDF/Imagem).  
- Links de avaliação de serviços.  
- Mensagens de acompanhamento/relacionamento.  

### 2.2 Funções do Produto
- Gestão de empresas e colaboradores.  
- Cadastro e gerenciamento de clientes.  
- Criação, configuração e exportação de orçamentos.  
- Agendamento e acompanhamento de serviços.  
- Avaliação de serviços pelos clientes.  
- Dashboard de relacionamento proativo.  

### 2.3 Usuários do Sistema
- **Administrador** → cria empresa, adiciona colaboradores, gerencia configurações.  
- **Colaborador** → gerencia clientes, orçamentos e serviços.  
- **Cliente** → interage via materiais gerados (orçamentos, mensagens, avaliações).  

### 2.4 Restrições
- Apenas administradores podem cadastrar colaboradores.  
- Todo orçamento deve conter valor total definido.  
- O sistema deve ser mobile-first.  

---

## 3. Requisitos Funcionais
### 3.1 Gestão de Empresa e Usuários
- **RF01** – O sistema deve permitir a criação de empresas.  
- **RF02** – O sistema deve permitir que apenas administradores cadastrem colaboradores.  
- **RF03** – O sistema deve manter perfis distintos (administrador e colaborador).  

### 3.2 Gestão de Clientes
- **RF04** – O sistema deve permitir cadastrar clientes via agenda telefônica.  
- **RF05** – O sistema deve permitir cadastrar clientes via formulário.  
- **RF06** – O sistema deve armazenar informações pessoais, de contato e endereço.  
- **RF07** – O sistema deve armazenar informações de relacionamento do cliente (livre e múltipla).  

### 3.3 Gestão de Orçamentos
- **RF08** – O sistema deve permitir criar orçamentos com itens contendo: descrição, quantidade, valor unitário e valor total.  
- **RF09** – O sistema deve calcular automaticamente o valor total de cada item, caso valor unitário e quantidade sejam preenchidos.  
- **RF10** – O sistema deve permitir inserir manualmente o valor total caso não haja quantidade ou valor unitário.  
- **RF11** – O sistema deve calcular automaticamente o valor final do orçamento como a soma dos itens.  
- **RF12** – O sistema deve permitir inserir condições de pagamento e desconto à vista.  
- **RF13** – O sistema deve permitir exportar orçamentos em PDF e Imagem.  
- **RF14** – O sistema deve permitir configurar campos adicionais para itens de orçamento.  

### 3.4 Agenda de Serviços (Roadmap Futuro)
- **RF15** – O sistema deve permitir agendar execução de serviços a partir de orçamentos aprovados.  
- **RF16** – O sistema deve exibir agenda em formato de calendário.  

### 3.5 Gestão de Serviços em Andamento (Roadmap Futuro)
- **RF17** – O sistema deve permitir colocar serviços em andamento.  
- **RF18** – O sistema deve exibir dashboard com itens em andamento, dias ativos e status de conclusão.  
- **RF19** – O sistema deve permitir finalizar serviços.  

### 3.6 Avaliação de Serviços (Roadmap Futuro)
- **RF20** – O sistema deve gerar link de avaliação para clientes.  
- **RF21** – O sistema deve permitir avaliação por estrelas em categorias configuráveis.  
- **RF22** – O sistema deve armazenar pontos positivos e negativos da avaliação.  
- **RF23** – O sistema deve exibir dashboard com lista de avaliações.  

### 3.7 Gestão de Relacionamento (Roadmap Futuro)
- **RF24** – O sistema deve exibir um dashboard de relacionamento com motivos de contato.  
- **RF25** – O sistema deve sugerir contatos automáticos baseados em regras configuráveis:  
  - Confirmação de serviço agendado (1 dia antes).  
  - Solicitação de avaliação (7 dias após conclusão).  
  - Follow-up de orçamento não agendado (2 e 7 dias).  
  - Oferta recorrente de serviço (6 meses, configurável).  
- **RF26** – O sistema deve permitir configurar templates de mensagens.  

---

## 4. Requisitos Não Funcionais
- **RNF01 – Usabilidade**: o sistema deve ser simples, intuitivo e otimizado para mobile.  
- **RNF02 – Desempenho**: o sistema deve carregar páginas em até 3 segundos em conexão 4G.  
- **RNF03 – Portabilidade**: deve ser compatível com Android e iOS (PWA ou app híbrido).  
- **RNF04 – Segurança**: autenticação com credenciais únicas, perfis de acesso diferenciados.  
- **RNF05 – Escalabilidade**: o sistema deve suportar crescimento para milhares de clientes sem perda de desempenho.  
- **RNF06 – Disponibilidade**: sistema deve estar disponível 99,5% do tempo mensal.  
- **RNF07 – Manutenibilidade**: código modular, documentado e com testes automatizados.  

---

## 5. Prioridade dos Requisitos
- **Alta**: Gestão de empresa, colaboradores, clientes e orçamentos.  
- **Média**: Exportação em PDF/Imagem, campos extras em orçamento.  
- **Baixa**: Agenda, serviços em andamento, avaliações, CRM proativo.  
