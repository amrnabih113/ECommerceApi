using System.Text;
using System.Threading.RateLimiting;
using ECommerce.core.configs;
using ECommerce.core.Middlewares;
using ECommerce.Data;
using ECommerce.DTOs;
using ECommerce.Infrastructure;
using ECommerce.Interfaces.Repositories;
using ECommerce.Interfaces.Services;
using ECommerce.Models;
using ECommerce.Repositories;
using ECommerce.Seeds;
using ECommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;


public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurations
        builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("Jwt"));
        builder.Services.Configure<EmailSettingsConfig>(
            builder.Configuration.GetSection("EmailSettings")
        );
        builder.Services.Configure<CloudinaryConfig>(
            builder.Configuration.GetSection("CloudinarySettings")
        );

        // Add Services

        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")
            )
        );

        // Identity Configuration
        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        // JWT Authentication
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 401;

                        var result = System.Text.Json.JsonSerializer.Serialize(
                            ApiResponse.ErrorResponse("Unauthorized: Invalid or missing token.")
                        );

                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 403;

                        var result = System.Text.Json.JsonSerializer.Serialize(
                            ApiResponse.ErrorResponse("Forbidden: You don't have access.")
                        );

                        return context.Response.WriteAsync(result);
                    }
                };
            });

        // Dependency Injection
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddHealthChecks();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
        builder.Services.AddScoped<IProductsService, ProductsService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        builder.Services.AddScoped<ICategoriesService, CategoriesService>();
        builder.Services.AddScoped<ICartsRepository, CartsRepository>();
        builder.Services.AddScoped<ICartsService, CartsService>();
        builder.Services.AddScoped<ICartItemsRepository, CartItemsRepository>();
        builder.Services.AddScoped<ICartItemsService, CartItemsService>();
        builder.Services.AddScoped<IReviewsRepository, ReviewsRepository>();
        builder.Services.AddScoped<IReviewsService, ReviewsService>();
        builder.Services.AddScoped<IAddressesService, AddressesService>();
        builder.Services.AddScoped<IProductImagesRepository, ProductImagesRepository>();
        builder.Services.AddScoped<IProductImagesService, ProductImagesService>();
        builder.Services.AddScoped<IProductVariantsRepository, ProductVariantsRepository>();
        builder.Services.AddScoped<IProductVariantsService, ProductVariantsService>();
        builder.Services.AddScoped<IWishListService, WishListService>();
        builder.Services.AddScoped<IProfileService, ProfileService>();
        builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
        builder.Services.AddScoped<IOrdersService, OrdersService>();
        builder.Services.AddScoped<ICouponsRepository, CouponsRepository>();
        builder.Services.AddScoped<IUserCouponsRepository, UserCouponsRepository>();
        builder.Services.AddScoped<ICouponsService, CouponsService>();
        builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
        builder.Services.AddScoped<IStripeService, StripeService>();
        builder.Services.AddScoped<IBannersRepository, BannersRepository>();
        builder.Services.AddScoped<IBannersService, BannersService>();

        // Rate Limiting Configuration
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("general", options =>
            {
                options.PermitLimit = 100;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 5;
            });

            options.AddFixedWindowLimiter("auth", options =>
            {
                options.PermitLimit = 5;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 0;
            });

            options.AddFixedWindowLimiter("orders", options =>
            {
                options.PermitLimit = 20;
                options.Window = TimeSpan.FromMinutes(1);
                options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 2;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        // Swagger Configuration
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "E-Commerce API",
                Version = "v1"
            });

            // Add JWT Bearer Definition
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token like: Bearer {your token}"
            });

            options.AddServer(new OpenApiServer
            {
                Url = "https://localhost:7286"
            });
        });

        var app = builder.Build();

        // Seed Roles
        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await RoleSeeder.SeedRolesAsync(roleManager);
        }

        // Seed Users
        using (var scope = app.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await UserSeeder.SeedUsersAsync(userManager, roleManager);
        }

        // Seed Data
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await BrandSeeder.SeedBrandsAsync(context);
            await CategorySeeder.SeedCategoriesAsync(context);
            await ProductSeeder.SeedProductsAsync(context);
            await ProductImageSeeder.SeedProductImagesAsync(context);
            await ProductVariantSeeder.SeedProductVariantsAsync(context);
            await ReviewSeeder.SeedReviewsAsync(context);
            await CartSeeder.SeedCartAsync(context);
            await WishListSeeder.SeedWishListAsync(context);
            await AddressSeeder.SeedAddressesAsync(context, userManager);
        }

        // Middleware Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        // Security headers middleware
        app.UseMiddleware<SecurityHeadersMiddleware>();

        // security 
        app.UseHttpsRedirection();
        app.UseHsts();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");
        await app.RunAsync();
    }
}