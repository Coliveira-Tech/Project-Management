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
- Para o limite de 20 tarefas por projeto contam todas as tarefas, independente do status?
- O limite de 20 tarefas por projeto pode mudar? (o ideal é ser um parâmetro no sistema)
- O prazo de 30 dias para relatórios pode mudar? (o ideal é ser um parâmetro no sistema)
- Está previsto adicionar pessoas ao projeto para que determinado projeto seja visível somente para aqueles incluídos no projeto?
- Está previsto adição de dependência entra as tarefas? (por ex. a tarefa X esta impedida pela tarefa Y)
- Esta prevista a funcionalidade de editar comentários?

# Melhorias

### Código

- Implementação de cache, principalmente nos endpoints de relatórios
- Integração com identity provider
- Criação de uma tabela de parametrização
- Criação de times para ser possível associar o time ao projeto

### Infraestrutura

- Hospedar o projeto em uma nuvem publica (Azure ou AWS), da forma que foi estruturado, o projeto esta totalmente preparado para a nuvem.
- Criar esteira de CI/CD contemplando automaticamente cobertura de testes unitários antes de subir para a branch principal
- Manter a api no Azure Container Registry (ou equivalente da AWS)

OBS: Escrevi alguns anos atrás um tutorial "do zero à nuvem" para a criação de esteiras de CI/CD no azure e está [disponível aqui](https://karlos-oliveira.medium.com/)

### Padrões

Por mais que seja um MVP, fiz o possivel para seguir as melhores práticas de desenvolvimento e padrões de desenvolvimento, sendo assim, se fosse o caso do projeto crescer e ter a necessidade de escalar, seria muito fácil mudar para uma estrutura mais completa como o DDD ou até mesmo uma arquitetura hexagonal.
