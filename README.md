# **MarketRecipes API**

A **API MarketRecipes** é uma solução desenvolvida em .NET 8 que oferece funcionalidades para gerenciar ingredientes, receitas e usuários. Ela utiliza o SQL Server como banco de dados e implementa operações CRUD (Create, Read, Update, Delete) para esses três componentes principais. A API inclui autenticação baseada em JWT para proteger endpoints sensíveis.

## **Estrutura do Projeto**

A estrutura do projeto é organizada em diferentes camadas para manter a separação de responsabilidades e facilitar a manutenção do código. A estrutura inclui:

- **Controllers**: Contém os controladores que lidam com as requisições HTTP e retornam as respostas apropriadas.
- **Data**: Contém o contexto do banco de dados e as configurações de mapeamento das entidades.
- **Dtos**: Contém os objetos de transferência de dados utilizados para comunicar entre a API e os clientes.
- **Middlewares**: Contém middlewares personalizados para tratar requisições e respostas.
- **Migrations**: Contém as migrações do Entity Framework para gerenciar a estrutura do banco de dados.
- **Models**: Contém as definições das entidades que representam as tabelas no banco de dados.
- **Services**: Contém a lógica de negócios e serviços auxiliares usados pelos controladores.

## **Autenticação**

A API utiliza o JWT (JSON Web Token) para autenticação. Endpoints sensíveis requerem que o cliente forneça um token válido para acessar os recursos. A autenticação é configurada através da dependência `Microsoft.AspNetCore.Authentication.JwtBearer`.
Para obter o *token* é necessário criar um usuário e na sequência fazer POST com suas credenciais para a rota de LOGIN.

## **Dependências Principais**

- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.6): Para autenticação baseada em JWT.
- `Microsoft.AspNetCore.Mvc.NewtonsoftJson` (8.0.6): Para suporte a JSON com o Newtonsoft.
- `Microsoft.EntityFrameworkCore` (8.0.6): Para integração com o Entity Framework Core.
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.6): Para uso do SQL Server com o Entity Framework Core.
- `Microsoft.EntityFrameworkCore.Tools` (8.0.6): Ferramentas do Entity Framework Core.
- `Swashbuckle.AspNetCore` (6.6.2): Para integração com Swagger e geração da documentação da API.

## **Models e Validações**

1. **Ingredient**
   - **Id**: Identificador único do ingrediente.
   - **Name**: Nome do ingrediente (obrigatório).
   - **Cost**: Custo do ingrediente (obrigatório, deve ser maior que zero).
   - **Unit**: Unidade de medida do ingrediente (obrigatório).

2. **Recipe**
   - **Id**: Identificador único da receita.
   - **Name**: Nome da receita (obrigatório).
   - **Description**: Descrição da receita.
   - **RecipeIngredients**: Lista de ingredientes utilizados na receita.
   - **TotalCost**: Custo total calculado com base nos ingredientes e quantidades.

3. **RecipeIngredient**
   - **RecipeId**: Identificador da receita.
   - **IngredientId**: Identificador do ingrediente.
   - **Quantity**: Quantidade do ingrediente na receita (obrigatório, deve ser maior que zero).

4. **User**
   - **Id**: Identificador único do usuário.
   - **Username**: Nome de usuário (obrigatório, único).
   - **Password**: Senha do usuário (obrigatório).

## Para criar Ingredientes:
- O custo deve ser maior que zero.
- A unidade é uma string livre.

## Para criar Receitas: 
- Você deve adicionar pelo menos 2 ingrediente.
- Os ingredientes devem ser existentes.
- A quantidade de ingredientes deve ser maior que zero.
- Você não pode criar Receitas com o mesmo nome.
- É possível buscar receitas informando uma lista com IDs de ingredientes que você possui para o endpoint: `api/Recipes/byIngredients`.



## **Endpoints**

### **Usuários**
#### *Criar Usuário

**Endpoint**: POST ``/api/Users``
**Autenticação**: Não requer
**Request Body**:
```
{
  "username": "string",
  "password": "string"
}
```
**Response**:
- ``201 Created com os dados do usuário criado``.
- ``400 Bad Request se o nome de usuário já estiver em uso ou os dados estiverem inválidos``-

#### *Obter Todos os Usuários

**Endpoint**: GET ``/api/Users``
**Autenticação**: Requer token JWT
**Response**:
- ``200 OK com a lista de usuários``.
- ``Obter Usuário por ID``

*Endpoint*: GET ``/api/Users/{id}``
*Autenticação*: Requer token JWT
*Response*:
- ``200 OK com os dados do usuário``.
- ``404 Not Found se o usuário não existir``.


#### *Atualizar Usuário

**Endpoint**: PUT ``/api/Users/{id}``
**Autenticação**: Requer token JWT
```
{
  "username": "string",
  "password": "string"
}
```
**Response**:
- ``204 No Content se a atualização for bem-sucedida``.
- ``404 Not Found se o usuário não existir``.

#### *Excluir Usuário

**Endpoint**: DELETE ``/api/Users/{id}``
**Autenticação**: Requer token JWT
**Response**:
- ``204 No Content se a exclusão for bem-sucedida``.
- ``404 Not Found se o usuário não existir``.

### **Login**

**Endpoint**: POST ``/api/Users/login``
**Autenticação**: Não requer
**Request Body**:
```
{
  "username": "string",
  "password": "string"
}
```
**Response**:
- ``200 OK com o token JWT``.
- ``401 Unauthorized se as credenciais estiverem incorretas``.

   

### **Ingredientes**

#### *Criar Ingrediente

**Endpoint**: POST ``/api/Ingredient``
**Autenticação**: Requer token JWT
**Request Body**:
```
{
  "ingredients": [
    {
      "name": "string",
      "cost": 0.0,
      "unit": "string"
    }
  ]
}
```
**Response**:
`200 OK com a lista de ingredientes criados`.
`400 Bad Request se os dados estiverem inválidos`.

#### *Obter Ingrediente por ID

**Endpoint**: GET ``/api/Ingredient/{id}``
**Autenticação**: Não requer
**Response**:
``200 OK com os dados do ingrediente``.
``404 Not Found se o ingrediente não existir``.


#### *Obter Todos os Ingredientes
**Endpoint**: GET ``/api/Ingredient``
**Autenticação**: Não requer
**Response**:
- ``200 OK com a lista de ingredientes``.
- ``Excluir Ingrediente``

**Endpoint**: DELETE ``/api/Ingredient/{id}``
**Autenticação**: Requer token JWT
**Response**:
- ``204 No Content se a exclusão for bem-sucedida``.
- ``404 Not Found se o ingrediente não existir``.

#### *Atualizar Ingrediente

**Endpoint**: PUT ``/api/Ingredient/{id}``
**Autenticação**: Requer token JWT
**Request Body**:
```
{
  "name": "string",
  "cost": 0.0,
  "unit": "string"
}
```
#### *Response:
- ``204 No Content se a atualização for bem-sucedida``.
- ``404 Not Found se o ingrediente não existir``.
- ``500 Internal Server Error se ocorrer um erro durante a atualização``.

### **Receitas**
#### *Criar Receita

**Endpoint**: POST ``/api/Recipes``
**Autenticação**: Requer token JWT
**Request Body**:
```
{
  "name": "string",
  "description": "string",
  "ingredients": [
    {
      "ingredientId": 0,
      "quantity": 0.0
    }
  ]
}
```
**Response**:
- ``201 Created com os dados da receita criada``.
- ``400 Bad Request se os dados estiverem inválidos ou se o nome da receita não for único``.

#### *Obter Receita por ID

**Endpoint**: GET ``/api/Recipes/{id}``
**Autenticação**: Não requer
**Response**:
- ``200 OK com os dados da receita``.
- ``404 Not Found se a receita não existir``.

#### *Obter Todas as Receitas

**Endpoint**: GET ``/api/Recipes``
**Autenticação**: Não requer
**Response**:
- ``200 OK com a lista de receitas``.

#### *Obter Receitas por Ingredientes

**Endpoint**: ``POST /api/Recipes/byIngredients``
**Autenticação**: Requer token JWT
**Request Body**:
```
{
  "ingredientIds": [0]
}
```
**Response**:
- ``200 OK com a lista de receitas que contêm todos os ingredientes fornecidos``

#### *Excluir Receita

**Endpoint*: DELETE ``/api/Recipes/{id}``
**Autenticação**: Requer token JWT
**Response**:
- ``204 No Content se a exclusão for bem-sucedida``.
- ``404 Not Found se a receita não existir``

#### *Atualizar Receita

**Endpoint**: PUT ``/api/Recipes/{id}``
**Autenticação**: Requer token JWT
**Request Body**:
```
{
  "name": "string",
  "description": "string",
  "ingredients": [
    {
      "ingredientId": 0,
      "quantity": 0.0
    }
  ]
}
```
**Response**:
- ``204 No Content se a atualização for bem-sucedida``.
- ``404 Not Found se a receita não existir``.
- ``500 Internal Server Error se ocorrer um erro durante a atualização``-