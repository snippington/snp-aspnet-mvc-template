dotnet aspnet-codegenerator identity -dc YourAppName.Data.ApplicationDbContext --files "Account.Register;Account.Login;Account.Logout;Account.RegisterConfirmation;Account.Manage.PersonalData" --force
dotnet ef migrations add CreateInitialSchema
dotnet ef database update