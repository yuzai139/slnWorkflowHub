using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using prjWorkflowHubAdmin.ContextModels;
using prjWorkflowHubAdmin.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 註冊DI注入
builder.Services.AddDbContext<SOPMarketContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SOPMarket"));
});


// 設定 CORS 規則
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


builder.Services.AddEndpointsApiExplorer(); // 添加 Swagger 服務 - 1
builder.Services.AddSwaggerGen(); // 添加 Swagger 服務 - 2

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

	// 啟用 Swagger 和 Swagger UI
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "API 文件 V1");
		options.RoutePrefix = "swagger"; // Swagger 的路徑為 "/swagger"
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

// 啟用 CORS 中介軟體
app.UseCors("All");

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");
app.MapRazorPages();

app.Run();
