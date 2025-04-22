# Project Management

## Pré-requisitos
- Docker 
- Docker Compose
- .Net Core 9.0
- Git

## Executando o Projeto

Primeiramente é necessário clonar o repositório para a sua máquina, para isso basta executar o comando abaixo na pasta de sua preferencia

```git clone https://github.com/karlos-oliveira/Project-Management.git```

Após clonar o repositório, abra o terminal de sua preferência e navegue até a pasta raiz do projeto (ao listar os arquivos você deve encontrar o arquivo ```docker-compose.yml```)

Digite o comando ```docker-compose up``` para subir o ambiente.

Após a conclusão do comando acima (que pode demorar alguns segundos) navegue até o endereço https://localhost:8081/swagger/index.html para acessar os endpoints da api

## Utilizando a API

Como não deve haver CRUD para usuário conforme instruções, a api já inicia com 3 usuários na base de dados sempre que é criada uma instancia nova, sendo assim, para criar um projeto novo ou acessar qualquer um dos endpoints, é necessário informar no header um usuário válido que é o equivalente de um usuário logado. Para alguns endpoints (Relatórios por exemplo) é necessário informar também a função dele, **importante lembrar que deve-se informar sempre o id da funcão e não o nome.**

Temos 3 usuários cadastrados por padrão, cada um com uma **Role/Função**:

| Id                                   | Nome           | Função id | Função        |
| ------------------------------------ | -------------- | --------- | ------------- |
| 3485751C-4840-41FA-BFCE-8054D8756CEE | Bill Gates     |     1     | Guest         |
| C6551A2C-E187-49C0-B929-4BBA525B14AE | Stive Jobs     |     3     | Manager       |
| 96A51FCA-20AB-4BFC-BE2F-B37E79632D06 | Linus Torvalds |     4     | Administrator |

## Testes Unitários

Para executar os testes unitários basta acessar a pasta raiz do projeto e digitar o seguinte comando:

```dotnet test .\ProjectManagement.sln```

# Perguntas para o PO

- A autenticação é feita por um serviço externo, porém temos a documentação de como é feita a chamada e de como funcionam as claims para definir as roles dos usuários? para este MVP é permitido informar no header a role e o usuário logado, porém foi feito assim somente para já deixar preparado para um identity provider.
- O limite de 20 tarefas por projeto pode mudar? (o ideal é ser um parâmetro no sistema)
- O prazo de 30 dias para relatórios pode mudar? (o ideal é ser um parâmetro no sistema)
- Está previsto adicionar pessoas ao projeto para que determinado projeto seja visível somente para aqueles incluídos no projeto?
- Está previsto adição de dependência entra as tarefas? (por ex. a tarefa X esta impedida pela tarefa Y)
- Esta prevista a funcionalidade de editar comentários?
