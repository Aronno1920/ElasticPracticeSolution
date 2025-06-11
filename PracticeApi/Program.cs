using Microsoft.EntityFrameworkCore;
using Nest;
using PracticeApi.Data;
using PracticeApi.Entities.Seeder;
using PracticeApi.Middleware;
using PracticeApi.Services;

var builder = WebApplication.CreateBuilder(args);


/*--------SETUP SERVICES--------*/
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


/*--------SETUP DEPENDENCY--------*/
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


/*--------Register Elasticsearch--------*/
///////// For Multiple Indexing /////////
var settings = new ConnectionSettings(new Uri("http://localhost:9200"));
var client = new ElasticClient(settings);

builder.Services.AddSingleton<IElasticClient>(client);
builder.Services.AddScoped<ElasticService>();
builder.Services.AddHostedService<ElasticSyncService>();
///////// For Multiple Indexing /////////


///////// For Single Indexing /////////
//var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex("products");
//var client = new ElasticClient(settings);
//builder.Services.AddSingleton<IElasticClient>(client);

//builder.Services.AddScoped<ElasticService>();
//builder.Services.AddHostedService<ElasticSyncService>();
///////// For Single Indexing /////////

var app = builder.Build();

/*--------Register Middleware--------*/
app.UseMiddleware<ResponseTimeMiddleware>();


/*--------SEED DATA--------*/
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
    await ProductSeeder.SeedAsync(context);
    await CustomerSeeder.SeedAsync(context);
}


/*--------BUILD SERVICES--------*/
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

/*--------RUN APPLICATION--------*/
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
