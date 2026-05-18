using AthletaNet.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Inyección del servicio maestro
builder.Services.AddSingleton<GymService>();

// 2. CONFIGURACIÓN CRUCIAL DE SESIONES
builder.Services.AddDistributedMemoryCache();

// Configuración correcta de la sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);

    // Metemos estas dos opciones dentro del objeto Cookie para que compile perfecto
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. ACTIVACIÓN DEL MIDDLEWARE
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();