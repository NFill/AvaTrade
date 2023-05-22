using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using AvaTrade.Data;
using AvaTrade.Domain.Entities;
using AvaTrade.Data.Repositories;
using AvaTrade.Services.Account;
using AvaTrade.Services.NewsCollect;
using AvaTrade.Services.Newses;
using AvaTrade.Data.Repositories.NewsRepo;

namespace AvaTrade.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      string connectionString = builder.Configuration.GetConnectionString("AvaTradeConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      builder.Services.AddDbContext<AvaTradeDbContext>(options =>
         options.UseSqlServer(connectionString));

      builder.Services.AddDefaultIdentity<AppUser>()
          .AddEntityFrameworkStores<AvaTradeDbContext>();

      builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
         options.TokenValidationParameters = new TokenValidationParameters
         {
           ValidateIssuer = true, 
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           ClockSkew = TimeSpan.Zero,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
         };
         options.Events = new JwtBearerEvents();
         options.Events.OnTokenValidated = async (tokenValidatedContext) =>
         {
           var ipAddress = tokenValidatedContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
           var accountService = tokenValidatedContext.Request.HttpContext.RequestServices.GetService<IAccountService>();
           var jwtToken = tokenValidatedContext.SecurityToken as JwtSecurityToken;
           if (!await accountService.IsTokenValid(jwtToken.RawData, ipAddress))
           {
             tokenValidatedContext.Fail("Invalid Token Details");
           }
         };
       });

      //TODO: Add methods to separate adding services and repositories

      builder.Services.AddControllers();
      // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
      builder.Services.AddEndpointsApiExplorer();
      builder.Services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swager JWT Bearer", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Name = "Authorization",
          Type = SecuritySchemeType.ApiKey,
          Scheme = "Bearer",
          BearerFormat = "JWT",
          In = ParameterLocation.Header,
          Description = "Enter JWT eith Bearer format"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id ="Bearer"},
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header
            },
            new string[]{ }
          }
        });
      });

      builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
      builder.Services.AddScoped<IChanges, Changes>();
      builder.Services.AddScoped<IAccountService, AccountService>();
      builder.Services.AddScoped<INewsService, NewsService>();
      builder.Services.AddScoped<INewsRepository, NewsRepository>();

      builder.Services.AddHostedService<NewsCollectService>();

      var app = builder.Build();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }

      app.UseHttpsRedirection();

      app.UseAuthentication();

      app.UseAuthorization();

      app.MapControllers();

      app.Run();
    }
  }
}