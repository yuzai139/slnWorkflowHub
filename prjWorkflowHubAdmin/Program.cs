using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ���UDI�`�J
builder.Services.AddDbContext<SOPMarketContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SOPMarket"));
});


// �]�w CORS �W�h
builder.Services.AddCors(options =>
{
    options.AddPolicy("All",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();


builder.Services.AddEndpointsApiExplorer(); // �K�[ Swagger �A�� - 1
builder.Services.AddSwaggerGen(); // �K�[ Swagger �A�� - 2

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

	// �ҥ� Swagger �M Swagger UI
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "API ��� V1");
		options.RoutePrefix = "swagger"; // Swagger �����|�� "/swagger"
	});
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles();

app.UseRouting();

// �ҥ� CORS �����n��
app.UseCors("All");

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");
app.MapRazorPages();

app.Run();
