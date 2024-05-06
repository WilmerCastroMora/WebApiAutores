namespace WebApiAutores.CositasVarias
{
    public interface IServiciocs
    {
        void RealizarTarea();
        public class ServicioA : IServiciocs
        {
            private readonly ILogger<ServicioA> logger;

            public ServicioA(ILogger<ServicioA> logger)
            {
                this.logger = logger;
            }
            public void RealizarTarea()
            {
            }
        }
        public class ServicioB : IServiciocs
        {
            public void RealizarTarea()
            {
            }
        }

        public class ServiceTransient
        {
            public Guid guid = Guid.NewGuid();
        }

        public class ServiceScoped
        {
            public Guid guid = Guid.NewGuid();
        }
        public class ServiceSingleton
        {
            public Guid guid = Guid.NewGuid();
        }


    }
}
