# **MarketRecipes API**

A **API MarketRecipes** � uma solu��o desenvolvida em .NET 8 que oferece funcionalidades para gerenciar ingredientes, receitas e usu�rios. Ela utiliza o SQL Server como banco de dados e implementa opera��es CRUD (Create, Read, Update, Delete) para esses tr�s componentes principais. A API inclui autentica��o baseada em JWT para proteger endpoints sens�veis.

## **Estrutura do Projeto**

A estrutura do projeto � organizada em diferentes camadas para manter a separa��o de responsabilidades e facilitar a manuten��o do c�digo. A estrutura inclui:

- **Controllers**: Cont�m os controladores que lidam com as requisi��es HTTP e retornam as respostas apropriadas.
- **Data**: Cont�m o contexto do banco de dados e as configura��es de mapeamento das entidades.
- **Dtos**: Cont�m os objetos de transfer�ncia de dados utilizados para comunicar entre a API e os clientes.
- **Middlewares**: Cont�m middlewares personalizados para tratar requisi��es e respostas.
- **Migrations**: Cont�m as migra��es do Entity Framework para gerenciar a estrutura do banco de dados.
- **Models**: Cont�m as defini��es das entidades que representam as tabelas no banco de dados.
- **Services**: Cont�m a l�gica de neg�cios e servi�os auxiliares usados pelos controladores.

## **Autentica��o**

A API utiliza o JWT (JSON Web Token) para autentica��o. Endpoints sens�veis requerem que o cliente forne�a um token v�lido para acessar os recursos. A autentica��o � configurada atrav�s da depend�ncia `Microsoft.AspNetCore.Authentication.JwtBearer`.
Para obter o *token* � necess�rio criar um usu�rio e na sequ�ncia fazer POST com suas credenciais para a rota de LOGIN.

## **Depend�ncias Principais**

- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.6): Para autentica��o baseada em JWT.
- `Microsoft.AspNetCore.Mvc.NewtonsoftJson` (8.0.6): Para suporte a JSON com o Newtonsoft.
- `Microsoft.EntityFrameworkCore` (8.0.6): Para integra��o com o Entity Framework Core.
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.6): Para uso do SQL Server com o Entity Framework Core.
- `Microsoft.EntityFrameworkCore.Tools` (8.0.6): Ferramentas do Entity Framework Core.
- `Swashbuckle.AspNetCore` (6.6.2): Para integra��o com Swagger e gera��o da documenta��o da API.

## **Models e Valida��es**

1. **Ingredient**
   - **Id**: Identificador �nico do ingrediente.
   - **Name**: Nome do ingrediente (obrigat�rio).
   - **Cost**: Custo do ingrediente (obrigat�rio, deve ser maior que zero).
   - **Unit**: Unidade de medida do ingrediente (obrigat�rio).

2. **Recipe**
   - **Id**: Identificador �nico da receita.
   - **Name**: Nome da receita (obrigat�rio).
   - **Description**: Descri��o da receita.
   - **RecipeIngredients**: Lista de ingredientes utilizados na receita.
   - **TotalCost**: Custo total calculado com base nos ingredientes e quantidades.

3. **RecipeIngredient**
   - **RecipeId**: Identificador da receita.
   - **IngredientId**: Identificador do ingrediente.
   - **Quantity**: Quantidade do ingrediente na receita (obrigat�rio, deve ser maior que zero).

4. **User**
   - **Id**: Identificador �nico do usu�rio.
   - **Username**: Nome de usu�rio (obrigat�rio, �nico).
   - **Password**: Senha do usu�rio (obrigat�rio).

## Para criar Ingredientes:
- O custo deve ser maior que zero.
- A unidade � uma string livre.

## Para criar Receitas: 
- Voc� deve adicionar pelo menos 2 ingrediente.
- Os ingredientes devem ser existentes.
- A quantidade de ingredientes deve ser maior que zero.
- Voc� n�o pode criar Receitas com o mesmo nome.
- � poss�vel buscar receitas informando uma lista com IDs de ingredientes que voc� possui para o endpoint: `api/Recipes/byIngredients`.



## **Endpoints**

### **Usu�rios**
#### *Criar Usu�rio

**Endpoint**: POST ``/api/Users``
**Autentica��o**: N�o requer
**Request Body**:
```
{
  "username": "string",
  "password": "string"
}
```
**Response**:
- ``201 Created com os dados do usu�rio criado``.
- ``400 Bad Request se o nome de usu�rio j� estiver em uso ou os dados estiverem inv�lidos``-

#### *Obter Todos os Usu�rios

**Endpoint**: GET ``/api/Users``
**Autentica��o**: Requer token JWT
**Response**:
- ``200 OK com a lista de usu�rios``.
- ``Obter Usu�rio por ID``

*Endpoint*: GET ``/api/Users/{id}``
*Autentica��o*: Requer token JWT
*Response*:
- ``200 OK com os dados do usu�rio``.
- ``404 Not Found se o usu�rio n�o existir``.


#### *Atualizar Usu�rio

**Endpoint**: PUT ``/api/Users/{id}``
**Autentica��o**: Requer token JWT
```
{
  "username": "string",
  "password": "string"
}
```
**Response**:
- ``204 No Content se a atualiza��o for bem-sucedida``.
- ``404 Not Found se o usu�rio n�o existir``.

#### *Excluir Usu�rio

**Endpoint**: DELETE ``/api/Users/{id}``
**Autentica��o**: Requer token JWT
**Response**:
- ``204 No Content se a exclus�o for bem-sucedida``.
- ``404 Not Found se o usu�rio n�o existir``.

### **Login**

**Endpoint**: POST ``/api/Users/login``
**Autentica��o**: N�o requer
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
**Autentica��o**: Requer token JWT
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
`400 Bad Request se os dados estiverem inv�lidos`.

#### *Obter Ingrediente por ID

**Endpoint**: GET ``/api/Ingredient/{id}``
**Autentica��o**: N�o requer
**Response**:
``200 OK com os dados do ingrediente``.
``404 Not Found se o ingrediente n�o existir``.


#### *Obter Todos os Ingredientes
**Endpoint**: GET ``/api/Ingredient``
**Autentica��o**: N�o requer
**Response**:
- ``200 OK com a lista de ingredientes``.
- ``Excluir Ingrediente``

**Endpoint**: DELETE ``/api/Ingredient/{id}``
**Autentica��o**: Requer token JWT
**Response**:
- ``204 No Content se a exclus�o for bem-sucedida``.
- ``404 Not Found se o ingrediente n�o existir``.

#### *Atualizar Ingrediente

**Endpoint**: PUT ``/api/Ingredient/{id}``
**Autentica��o**: Requer token JWT
**Request Body**:
```
{
  "name": "string",
  "cost": 0.0,
  "unit": "string"
}
```
#### *Response:
- ``204 No Content se a atualiza��o for bem-sucedida``.
- ``404 Not Found se o ingrediente n�o existir``.
- ``500 Internal Server Error se ocorrer um erro durante a atualiza��o``.

### **Receitas**
#### *Criar Receita

**Endpoint**: POST ``/api/Recipes``
**Autentica��o**: Requer token JWT
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
- ``400 Bad Request se os dados estiverem inv�lidos ou se o nome da receita n�o for �nico``.

#### *Obter Receita por ID

**Endpoint**: GET ``/api/Recipes/{id}``
**Autentica��o**: N�o requer
**Response**:
- ``200 OK com os dados da receita``.
- ``404 Not Found se a receita n�o existir``.

#### *Obter Todas as Receitas

**Endpoint**: GET ``/api/Recipes``
**Autentica��o**: N�o requer
**Response**:
- ``200 OK com a lista de receitas``.

#### *Obter Receitas por Ingredientes

**Endpoint**: ``POST /api/Recipes/byIngredients``
**Autentica��o**: Requer token JWT
**Request Body**:
```
{
  "ingredientIds": [0]
}
```
**Response**:
- ``200 OK com a lista de receitas que cont�m todos os ingredientes fornecidos``

#### *Excluir Receita

**Endpoint*: DELETE ``/api/Recipes/{id}``
**Autentica��o**: Requer token JWT
**Response**:
- ``204 No Content se a exclus�o for bem-sucedida``.
- ``404 Not Found se a receita n�o existir``

#### *Atualizar Receita

**Endpoint**: PUT ``/api/Recipes/{id}``
**Autentica��o**: Requer token JWT
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
- ``204 No Content se a atualiza��o for bem-sucedida``.
- ``404 Not Found se a receita n�o existir``.
- ``500 Internal Server Error se ocorrer um erro durante a atualiza��o``-