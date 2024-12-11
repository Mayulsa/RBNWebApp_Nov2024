using Blazored.LocalStorage;
using RBNWebApp_Nov2024.Class;
using RBNWebApp_Nov2024.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredLocalStorage();
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

app.Run();
