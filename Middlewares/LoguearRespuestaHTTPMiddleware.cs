namespace WebApiAutores.Middlewares
{
    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly Logger<LoguearRespuestaHTTPMiddleware> logger;

        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, Logger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            using (var ms = new MemoryStream())
            {
                var contextoOriginalRespuesta = context.Response.Body;
                contextoOriginalRespuesta = ms;
                await siguiente(context);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(contextoOriginalRespuesta);
                context.Response.Body = contextoOriginalRespuesta;
                logger.LogInformation(respuesta);
            }

        }

    }
}
