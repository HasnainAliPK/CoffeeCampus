using CoffeeCampus;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

Add DbContext for database integration
builder.Services.AddDbContext<coffeeCampus>(options =>
{
	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
options.UseSqlServer(connectionString);
});


Add Identity services

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
	options.SignIn.RequireConfirmedAccount = true;
options.Password.RequireDigit = true;
options.Password.RequiredLength = 8;
options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<coffeeCampus>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detailed error page in development
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure users are authenticated
app.UseAuthorization();  // Manage access based on user roles or status

app.MapRazorPages();

app.Run();
