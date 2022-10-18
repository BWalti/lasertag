namespace Admin.Api;

public static class WebApplicationExtensions
{
    public static WebApplication UseDevelopmentDefaults(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(corsBuilder =>
            {
                corsBuilder.WithOrigins("http://127.0.0.1:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                corsBuilder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseEndpoints(_ => { });

#pragma warning disable S1075
            app.UseSpa(x =>
            {
                x.UseProxyToSpaDevelopmentServer("http://localhost:5173");
            });
# pragma warning restore S1075
        }

        return app;
    }
}