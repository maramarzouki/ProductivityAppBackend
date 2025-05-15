using Microsoft.Extensions.Hosting;
using Repository.AdminRepository;
using Repository.ChallengeRepositoy;
using Repository.HabitRepository;
using Repository.ProjectRepository;
using Repository.ReminderRepository;
using Repository.TaskRepository;
using Repository.UserRepository;
using Service.Admin;
using Service.ChallengeService;
using Service.EmailSender;
using Service.HabitService;
using Service.ProjectService;
using Service.ReminderService;
using Service.TaskService;
using Service.UserService;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5256");
//builder.WebHost.UseUrls("http://localhost:5256");   


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5256);                    // HTTP
    options.ListenAnyIP(5257, listenOptions =>    // HTTPS
    {
        listenOptions.UseHttps();                // will use the dev certificate
    });
});

// 1a) Needed by Session:
builder.Services.AddDistributedMemoryCache();

// 1b) Add Session itself:
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IChallengeRepository, ChallengeRepository>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddHostedService<Service.BackgroundServices.DailyChallengeChecker>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/openApi/v1.json", "Open API V1");
//    });
//}


//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
