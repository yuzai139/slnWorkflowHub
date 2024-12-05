
using apiWorkflowHub.ContextModels;
using Microsoft.EntityFrameworkCore;

namespace apiWorkflowHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // ���U HttpClient
            builder.Services.AddHttpClient();

            // ���UDI�`�J 
            builder.Services.AddDbContext<SOPMarketContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SOPMarket"));
            });



            //���U�}��Cors
            string PolicyName = "ALL";//�i�H�[�J�ܦh���P��Policy(�Ҥ��q�B�A���q)
            builder.Services.AddCors(options => {
                options.AddPolicy(name: PolicyName, policy =>//�Ĥ@��Policy
                {
                    policy.WithOrigins("http://localhost:4200",  // Angular �}�o����             " +
                     "https://localhost:4200", "https://localhost:7151")   // �p�G�ϥ� HTTPS"
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials(); ;
                });
            });


            // �K�[ HttpClient �A��
            builder.Services.AddHttpClient();
            // Add services to the container.
            //�j�p�g�]�w�B
            builder.Services.AddControllers(); //�쥻���o�� : builder.Services.AddControllers();


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
            // CORS �����󥲶��b UseRouting ����AUseAuthorization ���e

            app.UseRouting();
            app.UseCors("ALL");//�}�޽u�A�n��bUseHttpRedirection();�e��

            app.UseHttpsRedirection();

            // �ҥ��R�A���A��
            app.UseStaticFiles(); // �o��N�X���\�X�� wwwroot �ؿ������R�A���

            app.UseAuthorization();


            app.MapControllers();
            
            app.Run();
        }
    }
}
