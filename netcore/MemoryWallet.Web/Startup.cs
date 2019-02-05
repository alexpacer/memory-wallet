using Akka.Actor;
using Akka.Configuration;
using MemoryWallet.Web.Actors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemoryWallet.Web
{
    public delegate IActorRef SportsBookManagerSystemActorProvider();

    public delegate ActorSelection SportsBookManagerRemoteActorPRovider();
    
    public class Startup
    {
        private readonly ILogger _startupLogger;
        private readonly ILogger<IActorRef> _actorLogger;

        public Startup(ILogger<Startup> startupLogger, ILogger<IActorRef> actorLogger)
        {
            _startupLogger = startupLogger;
            _actorLogger = actorLogger;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = ConfigurationFactory.ParseString(@"
akka {  
    actor {
        provider = remote
    }
    remote {
        dot-netty.tcp {
            port = 0
            hostname = 0.0.0.0
            public-hostname = localhost
        }
    }
}
");
            services.AddSingleton(_ => ActorSystem.Create("SportsbookStore", config));

            services.AddSingleton<SportsBookManagerSystemActorProvider>(p =>
            {
                var actorSystem = p.GetService<ActorSystem>();

                var b = actorSystem.ActorOf(SportsBookManagerActor.Props(_actorLogger));
                return () => b;
            });

            services.AddSingleton<SportsBookManagerRemoteActorPRovider>(p =>
            {
                var actorSelection = p.GetService<ActorSystem>();
                return () => actorSelection.ActorSelection("akka.tcp://MemoryWallet@localhost:8080/user/Sportsbook/PlayerManager");
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            applicationLifetime.ApplicationStarted.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>();
            });

            applicationLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetService<ActorSystem>().Terminate().Wait();
            });

            app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
}