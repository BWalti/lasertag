namespace Admin.Api.Extensions;

#pragma warning disable S125
public static class WebApplicationExtensions
{
    public static WebApplication UseApiFallback404(this WebApplication app)
    {
        app.Use((ctx, next) =>
        {
            if (ctx.Request.Path.StartsWithSegments("/api", StringComparison.CurrentCulture))
            {
                ctx.Response.StatusCode = 404;
                return Task.CompletedTask;
            }

            return next();
        });

        return app;
    }

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

            app.UseOpenTelemetryPrometheusScrapingEndpoint();

#pragma warning disable ASP0014
            app.UseEndpoints(_ => { });
#pragma warning restore ASP0014
            app.UseApiFallback404();

#pragma warning disable S1075
            app.UseSpa(x => { x.UseProxyToSpaDevelopmentServer("http://localhost:5173"); });
#pragma warning restore S1075
        }

        return app;
    }
}
#pragma warning restore S125