# 📑 Casos de Uso – 99Gestor

## 🎭 Atores
- **Administrador da Empresa** → cria a empresa, gerencia colaboradores, configurações e tem acesso a tudo.  
- **Colaborador** → gerencia clientes, orçamentos, serviços (mas não pode criar outros colaboradores).  
- **Cliente** → não acessa o app diretamente, mas pode receber orçamentos, mensagens e links para avaliações.  

---

## 🔹 1. Gestão da Empresa e Colaboradores
**UC01 – Criar Empresa**  
- Ator: Administrador  
- Descrição: cria uma nova empresa dentro do sistema.  

**UC02 – Cadastrar Colaborador**  
- Ator: Administrador  
- Descrição: adiciona colaboradores que terão acesso ao painel administrativo.  

**UC03 – Gerenciar Configurações de Orçamento**  
- Ator: Administrador  
- Descrição: definir campos customizados para itens de orçamento.  

---

## 🔹 2. Gestão de Clientes
**UC04 – Cadastrar Cliente via Agenda Telefônica**  
- Ator: Administrador / Colaborador  
- Descrição: importar contatos direto da agenda do celular.  

**UC05 – Cadastrar Cliente via Formulário**  
- Ator: Administrador / Colaborador  
- Descrição: preencher manualmente dados de cliente.  

**UC06 – Gerenciar Relacionamento do Cliente**  
- Ator: Administrador / Colaborador  
- Descrição: manter lista de informações pessoais relevantes (filhos, viagens, preferências etc.).  

---

## 🔹 3. Orçamentos
**UC07 – Criar Orçamento**  
- Ator: Administrador / Colaborador  
- Fluxo:  
  1. Adicionar itens com descrição, quantidade, valor unitário.  
  2. Sistema calcula automaticamente valor total por item (ou permite inserir manual).  
  3. Calcular valor final do orçamento = soma de itens.  
  4. Definir condições de pagamento e desconto à vista.  

**UC08 – Configurar Campos Extras do Orçamento**  
- Ator: Administrador  
- Descrição: adicionar campos customizados aos itens via página de configuração.  

**UC09 – Exportar Orçamento**  
- Ator: Administrador / Colaborador  
- Descrição: salvar orçamento como **PDF** ou **Imagem** para envio ao cliente.  

---

## 🔹 4. Agenda de Serviços (Feature Desejável)
**UC10 – Agendar Serviço**  
- Ator: Administrador / Colaborador  
- Descrição: vincular um orçamento aprovado a uma data/hora em calendário.  

**UC11 – Visualizar Agenda**  
- Ator: Administrador / Colaborador  
- Descrição: visão de calendário com serviços agendados.  

---

## 🔹 5. Gestão de Serviços em Andamento (Feature Desejável)
**UC12 – Iniciar Serviço**  
- Ator: Administrador / Colaborador  
- Descrição: colocar um orçamento como "em andamento".  

**UC13 – Gerenciar Itens em Andamento**  
- Ator: Administrador / Colaborador  
- Descrição: dashboard mostra itens, dias em andamento, concluir itens individualmente.  

**UC14 – Finalizar Serviço**  
- Ator: Administrador / Colaborador  
- Descrição: concluir execução do serviço.  

---

## 🔹 6. Avaliação de Serviços (Feature Desejável)
**UC15 – Gerar Link de Avaliação**  
- Ator: Administrador / Colaborador  
- Descrição: gerar link para cliente avaliar serviço (estrelas por categoria, pontos positivos/negativos).  

**UC16 – Acompanhar Avaliações**  
- Ator: Administrador / Colaborador  
- Descrição: dashboard com lista de avaliações recebidas.  

---

## 🔹 7. Gestão de Relacionamento (Feature Desejável)
**UC17 – Dashboard de Relacionamento**  
- Ator: Administrador / Colaborador  
- Descrição: exibir motivos proativos de contato com clientes.  

**Casos Automáticos no Dashboard:**  
- **UC18** – Serviço agendado para amanhã → Enviar mensagem de confirmação.  
- **UC19** – Serviço concluído há 7 dias → Enviar solicitação de avaliação.  
- **UC20** – Orçamento feito há 2 ou 7 dias sem agendamento → Enviar mensagem de reconexão.  
- **UC21** – Serviço realizado há mais de 6 meses → Enviar oferta especial (recorrência configurável).  

**UC22 – Configurar Templates de Mensagens**  
- Ator: Administrador  
- Descrição: gerenciar textos que serão usados nos contatos automáticos.  
