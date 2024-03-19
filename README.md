# Snippington ASP.Net MVC Template
ASP.Net Core MVC Template with Identity and CRUD Controller.

### Installation

Install using the below command:
```bash
dotnet new install Snippington.Utility.Templates
```

### Getting Started

Create a new template using:

```bash
dotnet new snp-asp-mvc -n <YourAppName>
```

#### Automated Setup

Run the init script included in the project.

```bash
chmod +x ./init.sh
./init.sh
```

#### Manual Setting Up

Once you setup your template, run the below commands (only once):

```bash
dotnet ef migrations add CreateInitialSchema
dotnet ef database update
```

#### Optional

Optionally, regenerate the Login, Identity screens using the below command:

```bash
dotnet aspnet-codegenerator identity -dc YourAppName.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation;Account.Manage.PersonalData" --force
dotnet ef migrations add CreateInitialSchema
dotnet ef database update
```


