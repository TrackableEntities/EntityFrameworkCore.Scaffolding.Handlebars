# Handlebars Scaffolding Tests

## Windows Intel Setup

- Use SQL Server Management Studio to connect to SQL Server
- The easiest is to use **LocalDb**, which is installed with Visual Studio.
- Connect to: `(localdb)\MsSqlLocalDb`.

## MacOS arm64 Setup (M Series)

- Install [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Run Docker SQL Server instance for arm64

```
docker run -e "ACCEPT_EULA=1" -e "MSSQL_SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1433:1433 -d --name=sql mcr.microsoft.com/azure-sql-edge
```

- Add VS Code [Extension for SQL Server](https://marketplace.visualstudio.com/items?itemName=ms-mssql.mssql)
  - Connect with username `sa` and password `MyPass@word`
  - Enable trust server certificate when prompted
  - See [here](https://learn.microsoft.com/en-us/sql/tools/visual-studio-code/sql-server-develop-use-vscode?view=sql-server-ver16) for help connecting and writing commands and queries
- Set connection string with Secret Manager

```
dotnet user-secrets set "ConnectionStrings:NorthwindTestContext" "Server=localhost; Database=NorthwindTestDb; User ID=sa;Password=MyPass@word; TrustServerCertificate=True;"
```
