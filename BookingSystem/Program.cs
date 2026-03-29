using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; 
using Microsoft.IdentityModel.Tokens;

namespace BookingSystem
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter: Bearer {token}"
				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
			});

			builder.Services.AddDbContext<AppDbContext>(options =>
				options.UseSqlServer(
					@"Data Source=.;Initial Catalog=BookingDb;Integrated Security=True;TrustServerCertificate=True"
				)
			);

			builder.Services.AddScoped<TokenService>();
			builder.Services.AddScoped<BookingService>();

			builder.Services.AddHttpContextAccessor();
			

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					var key = builder.Configuration["Jwt:Key"];

					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,

						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],

						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(key!)
						)
					};
				});

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("AssignedDoctorOnly", policy =>
					policy.Requirements.Add(new AssignedDoctorRequirement()));
			});
			builder.Services.AddScoped<IAuthorizationHandler, AssignedDoctorHandler>();
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAngular", policy =>
				{
					policy.WithOrigins("http://localhost:4200")
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});
			var app = builder.Build();
			app.UseCors("AllowAngular");
			app.UseSwagger();
			app.UseSwaggerUI();

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}