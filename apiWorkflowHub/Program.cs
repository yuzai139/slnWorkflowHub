
using apiWorkflowHub.ContextModels;
using Microsoft.EntityFrameworkCore;

namespace apiWorkflowHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 註冊 HttpClient
            builder.Services.AddHttpClient();

            // 註冊DI注入 
            builder.Services.AddDbContext<SOPMarketContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SOPMarket"));
            });



            //註冊開放Cors
            string PolicyName = "ALL";//可以加入很多不同的Policy(甲公司、乙公司)
            builder.Services.AddCors(options => {
                options.AddPolicy(name: PolicyName, policy =>//第一個Policy
                {
                    policy.WithOrigins("http://localhost:4200",  // Angular 開發環境             " +
                     "https://localhost:4200", "https://localhost:7151")   // 如果使用 HTTPS"
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); ;
                });
            });


            // 添加 HttpClient 服務
            builder.Services.AddHttpClient();
            // Add services to the container.
            //大小寫設定處
            builder.Services.AddControllers(); //原本長這樣 : builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            // CORS 中間件必須在 UseRouting 之後，UseAuthorization 之前

            app.UseRouting();
            app.UseCors("ALL");//開管線，要放在UseHttpRedirection();前面

            app.UseHttpsRedirection();

            // 啟用靜態文件服務
            app.UseStaticFiles(); // 這行代碼允許訪問 wwwroot 目錄中的靜態文件

            app.UseAuthorization();


            app.MapControllers();
            
            app.Run();
        }
    }
}
