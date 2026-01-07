using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using University_Admission_Portal.Data;
using University_Admission_Portal.Email_Notification;
using University_Admission_Portal.Interface;
using University_Admission_Portal.Models;
using University_Admission_Portal.Repository;
using University_Admission_Portal.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<UnivContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));


builder.Services.AddScoped<IUniversity<User, int>, UserRepository>();
builder.Services.AddScoped<IUniversity<Student, int>, StudentRepository>();
builder.Services.AddScoped<IUniversity<Staff, int>, StaffRepository>();
builder.Services.AddScoped<IUniversity<Course, int>, CourseRepository>();
builder.Services.AddScoped<IUniversity<Application, int>, ApplicationRepository>();
builder.Services.AddScoped<IApplication, ApplicationRepository>();



builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailService, EmailService>();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentservice, StudentService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();



//jwt authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(op =>
    {
        op.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

    });




//authorize btn in swagger
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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



//detected cycle error - while .include of other model
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAngular", pol => pol.WithOrigins("http://localhost:4200/")
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");


app.UseMiddleware<University_Admission_Portal.Middlewares.GlobalException>();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
