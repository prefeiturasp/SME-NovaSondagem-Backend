namespace SME.Sondagem.API.Configuracoes
{
    public static class RegistraConfiguracaoCors
    {
        public static void Registrar(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.AddPolicy("CorsPolicy", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                }
                else
                {
                    var allowedOriginsString = builder.Configuration["Cors:AllowedOrigins"];
                    var allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsString)
                        ? []
                        : allowedOriginsString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    options.AddPolicy("CorsPolicy", policy =>
                    {
                        if (allowedOrigins.Length > 0)
                        {
                            policy.WithOrigins(allowedOrigins)
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials();
                        }
                        else
                        {
                            policy.WithOrigins("https://localhost")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader();
                        }
                    });
                }
            });
        }
    }
}
