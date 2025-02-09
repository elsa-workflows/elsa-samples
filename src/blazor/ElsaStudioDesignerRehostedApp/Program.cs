using ElsaStudioDesignerRehostedApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Insert this BEFORE UseStaticFiles middleware.
app.UseMiddleware<WasmAssetsRewritingMiddleware>();

// Use Static Files instead of MapStaticAssets.
app.UseStaticFiles();
//app.MapStaticAssets();
app.MapRazorPages();
    //.WithStaticAssets();

app.Run();