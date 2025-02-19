using Library.Domain.Abstractions;
using Library.Domain.Book;
using Library.Domain.Borrowing;
using Library.Domain.Services;
using Library.Domain.User;
using Library.Infrastructure;
using Library.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(connectionString);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(provider =>
{
    var connectionString = provider.GetRequiredService<string>();
    return new UnitOfWork(connectionString);
});

builder.Services.AddScoped<IBookRepository>(provider =>
{
    var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
    return ((UnitOfWork)unitOfWork).BookRepository;
});

builder.Services.AddScoped<IBorrowingRepository>(provider =>
{
    var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
    return ((UnitOfWork)unitOfWork).BorrowingRepository;
});

builder.Services.AddScoped<IUserRepository>(provider =>
{
    var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
    return ((UnitOfWork)unitOfWork).UserRepository;
});

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddSingleton<DatabaseSeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    seeder.SeedBooks(); // Seed the Books table
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();