### Getting Started

Run the below commands.
```bash
dotnet ef database update
dotnet run
```

Other Optional Commands

```bash
dotnet aspnet-codegenerator identity -dc YourAppName.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation;Account.Manage.PersonalData" --force
dotnet ef migrations add CreateInitialSchema
```