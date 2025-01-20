using Blazored.LocalStorage;
using Radzen;
using RBNWebApp_Nov2024.Class;
using RBNWebApp_Nov2024.Components;
using System.Diagnostics.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<UserCRUD>(sp =>
    new UserCRUD("Server=DESKTOP-VN0KBGI;Database=RBNWebApp_Nov2024_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"));
builder.Services.AddScoped<RefrigeratorCRUD>(sp =>
    new RefrigeratorCRUD("Server=DESKTOP-VN0KBGI;Database=RBNWebApp_Nov2024_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"));
builder.Services.AddScoped<DrawerCRUD>(sp =>
    new DrawerCRUD("Server=DESKTOP-VN0KBGI;Database=RBNWebApp_Nov2024_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"));
builder.Services.AddScoped<BloodUnitsCRUD>(sp =>
    new BloodUnitsCRUD("Server=DESKTOP-VN0KBGI;Database=RBNWebApp_Nov2024_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"));
builder.Services.AddScoped<ReservationCRUD>(sp =>
    new ReservationCRUD("Server=DESKTOP-VN0KBGI;Database=RBNWebApp_Nov2024_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//PARA ENTRAR EN SING IN
//email: contacto@cruzroja.do
//password: DefaultPass123!
//Ad email:admin@rbn.gob.do
//Ad password: DefaultPass123!

app.Run();
