using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TagsManagement.Repositories;
using TagsManagement.Repositories.Implements;
using TagsManagement.Repositories.Interfaces;
using TagsManagement.Filters;
using TagsManagement.Middlewares;
using System.Globalization;
using TagsManagement.Services.Implements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    }
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// adding CustomAuthorizeFilter:
builder.Services.AddMvc(options =>
{
    //options.Filters.Add(new CustomAuthorizeFilter()); // bug: it deny all controller
});


#region DbContext & Identity:
builder.Services.AddDbContext<EFAppDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))); // (Microsoft.EntityFrameworkCore.SqlServer)

// to use 'AddDefaultIdentity': install package 'Microsoft.AspNetCore.Identity.UI' !!! NET 7
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<EFAppDbContext>();



builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = true;  // 0 - 9
    options.Password.RequireNonAlphanumeric = true; // @!#$%^&*
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 4;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Every email is unique! (ko trùng email)

    // Cấu hình đăng nhập. (important!!!)
    // user may get 401 satus code in authorized enpoints after registration if email is not confirm !!!
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
    options.SignIn.RequireConfirmedEmail = false; // Cấu hình confirm email after register (email must exists)
    options.User.RequireUniqueEmail = false;


});

#endregion

#region Response time header middleware:
// https://www.codeproject.com/Tips/5337523/Response-Time-Header-in-ASP-NET-Core
builder.Services.AddScoped<IActionResponseTimeStopwatch, ActionResponseTimeStopwatch>();

/*Filter*/
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new ResponseTimeFilter());
});

builder.Services.AddScoped<IMiddlewareResponseTimeStopwatch, MiddlewareResponseTimeStopwatch>();
#endregion

builder.Services.AddSwaggerGen();

// Inject the unit of work as a scoped dependency
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<ITagService, TagService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region inject API key auth middleware:
//app.UseMiddleware<ApiKeyMiddleware>();
#endregion

#region inject custom response model middleware:
app.UseMiddleware<CustomResponseMiddleware>();
#endregion

#region inject RequestCultureMiddleware directly to app.Use (1)
// @source: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-7.0
//app.Use(async (context, next) =>
//{
//    var cultureQuery = context.Request.Query["culture"];
//    if (!string.IsNullOrWhiteSpace(cultureQuery))
//    {
//        var culture = new CultureInfo(cultureQuery);

//        CultureInfo.CurrentCulture = culture;
//        CultureInfo.CurrentUICulture = culture;
//    }

//    // Call the next delegate/middleware in the pipeline.
//    await next(context);
//});
#endregion

#region inject RequestCultureMiddleware class
app.UseRequestCulture();
#endregion

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#region inject RequestCultureMiddleware directly to app.Run (2) (override all the responses of all controllers)
// @source: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-7.0

//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync(
//        $"CurrentCulture.DisplayName: {CultureInfo.CurrentCulture.DisplayName}");
//});
#endregion

#region directly inject middleware
//static void HandleBranch(IApplicationBuilder app)
//{
//    app.Run(async context =>
//    {
//        var branchVer = context.Request.Query["branch"];
//        await context.Response.WriteAsync($"Branch used = {branchVer}");
//    });
//}

//app.MapWhen(context => context.Request.Query.ContainsKey("branch"), HandleBranch);

// override all routes with this response:
//app.Run(async context =>
//{
//    await context.Response.WriteAsync("Hello from non-Map delegate.");
//});
#endregion


app.Run();
