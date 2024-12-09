using Microsoft.IdentityModel.Tokens;
using System.Text;
using JWTAuthorization.Services;
//using OnlineLearningPlatform.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OnlineLearningPlatform.Services;
using System.Security.Claims;
using Microsoft.Extensions.FileProviders;

//using OnlineLearningPlatform.Services;

var builder = WebApplication.CreateBuilder(args);


var sendGridApiKey = builder.Configuration["SendGridApiKey"];
builder.Services.AddSingleton(new EmailService(sendGridApiKey));

// Fetch JWT settings from configuration
var jwtval = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtval["Key"]);

// MongoDB configuration
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDbConnectionString");
    return new MongoClient(connectionString); // Create a MongoDB client instance
});

// Register IMongoDatabase to be injected into controllers and services
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration["MongoDbDatabaseName"]); // Retrieve MongoDB database name
});

// Register MongoDbContext (used in services to interact with MongoDB)
//builder.Services.AddScoped<MongoDbContext>();
// Program.cs
builder.Services.AddSignalR();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<QuizService>();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddSingleton<ContactService>();


// Add CORS policy to allow requests from the Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication using JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtval["Issuer"],
        ValidAudience = jwtval["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key) // Use the key for signing and validation
    };
});

// Authorization (Role-based policies)
// Program.cs
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("InstructorOnly", policy =>
    //    policy.RequireClaim(ClaimTypes.Role, "Instructor"));

    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("RequireInstructorRole", policy =>
        policy.RequireRole("Instructor", "Admin"));

    options.AddPolicy("RequireLearnerRole", policy =>
        policy.RequireRole("Learner", "Instructor", "Admin"));

    options.AddPolicy("RequireSupportRole", policy =>
        policy.RequireRole("Support", "Admin"));

    options.AddPolicy("InstructorOnly", policy =>
        policy.RequireRole("Instructor"));

    options.AddPolicy("LearnerOnly", policy =>
        policy.RequireRole("Learner"));

    options.AddPolicy("AdminOnly", policy =>

        policy.RequireRole("Admin"));


        options.AddPolicy("RequireLearnerRole", policy =>
            policy.RequireRole("Learner"));

});


// Register other services like JwtService
builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<CourseService>();

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 500 * 1024 * 1024);



var app = builder.Build();

// Configure middlewares for development and production
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAngularApp");

app.UseAuthentication(); // Ensure authentication is used before authorization
app.UseAuthorization();  // Ensure authorization is used after authentication



app.MapControllers(); // Map the controllers for the API endpoints
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});

app.Run();
