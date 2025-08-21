using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RWA.Web.Application.Models;
using RWA.Web.Application.Services.Ldap;
using Vite.AspNetCore;
using RWA.Web.Application.Hubs;

namespace RWA.Web.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            
            // Add response caching to optimize status endpoint performance
            builder.Services.AddResponseCaching();
            builder.Services.AddMemoryCache();

            builder.Services.AddViteServices(options =>
            {
                options.Server.AutoRun = true;
                options.Server.Https = true;
            });

            builder.Services.Configure<LdapSettings>(builder.Configuration.GetSection("Ldap"));
            // Bind workflow status mapping options so advancement/error rules are configurable at runtime
            builder.Services.Configure<WorkflowStatusMappingOptions>(builder.Configuration.GetSection("WorkflowStatusMapping"));
            builder.Services.AddScoped<ILdapAuthService, LdapAuthService>();
            // Validation services - use FluentValidation-based service
            builder.Services.AddSingleton<Services.Validation.FluentValidationService>();

            // Register generic Fluent validators for WorkflowStep
            builder.Services.AddTransient<FluentValidation.IValidator<RWA.Web.Application.Models.WorkflowStep>, RWA.Web.Application.Services.Validation.Fluent.UploadTemplateFluentValidator>();
            builder.Services.AddTransient<FluentValidation.IValidator<RWA.Web.Application.Models.WorkflowStep>, RWA.Web.Application.Services.Validation.Fluent.MandatoryColumnsFluentValidator>();
            builder.Services.AddTransient<FluentValidation.IValidator<RWA.Web.Application.Models.WorkflowStep>, RWA.Web.Application.Services.Validation.Fluent.RwaCategoryMappingFluentValidator>();

            builder.Services.AddSingleton<Services.Validation.IValidatorsFactory, Services.Validation.ValidatorsFactory>();

            builder.Services.AddAuthentication("MyCookieAuth")
                .AddCookie("MyCookieAuth", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = "MyAuthCookie";
                    options.ExpireTimeSpan = TimeSpan.FromDays(2);
                });

            builder.Services.AddAuthorization();

            // Add session support for workflow orchestrator persistence
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(8); // Session timeout - adjust as needed
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; // Required for workflow state persistence
                options.Cookie.Name = "RWA.WorkflowSession";
            });

            // Add HttpContextAccessor for session access
            builder.Services.AddHttpContextAccessor();

            // Inventory import service (saves uploaded files to wwwroot/uploads and parses Excel)
            builder.Services.AddSingleton<Services.Workflow.IInventoryImportService, Services.Workflow.InventoryImportService>();
            // Default mapper that persists parsed rows into HecateInventaireenrichie
            builder.Services.AddSingleton<Services.Workflow.IInventoryMapper, Services.Workflow.InventoryMapper>();
            
            // Event-driven workflow architecture:
            // - Singleton state machine (holds state + delegates)
            // - Singleton actions (will use IServiceScopeFactory for EF Core operations)
            // - Singleton orchestrator: single instance manages workflow state consistently
            //   This prevents multiple orchestrator instances and ensures consistent workflow behavior.
            builder.Services.AddSingleton<Services.Workflow.WorkflowStateMachine>();
            builder.Services.AddSingleton<Services.Workflow.IWorkflowStateActions, Services.Workflow.WorkflowStateActions>();
            builder.Services.AddSingleton<Services.Workflow.IWorkflowOrchestrator, Services.Workflow.WorkflowOrchestrator>();
            builder.Services.AddSingleton<Services.Workflow.IWorkflowEventPublisher, Services.Workflow.WorkflowEventPublisher>();
            // Configure DbContext: use InMemory in Development for local work, otherwise use configured SQL Server
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDbContext<RwaContext>(options => options.UseInMemoryDatabase("RwaDevInMemory"));
            }
            else
            {
                builder.Services.AddDbContext<RwaContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            // DB provider abstraction - should have same scope as RWAContext to avoid null context when updating
            builder.Services.AddSingleton<RWA.Web.Application.Services.Workflow.IWorkflowDbProvider, RWA.Web.Application.Services.Workflow.EfWorkflowDbProvider>();
            // Note: IInventoryMapper is optional and can be registered by the caller to provide domain mapping. By default none is provided.

            var app = builder.Build();

            // Seed workflow steps and sample data into the in-memory DB (development) or SQL DB if empty
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<RwaContext>();
                    
                    // Seed workflow steps
                    if (!context.WorkflowSteps.Any())
                    {
                        var steps = new List<WorkflowStep>
                        {
                            new WorkflowStep { StepName = "Upload inventory", Status = "Current", DataPayload = "{}" },
                            new WorkflowStep { StepName = "RWA Category Manager", Status = "Open", DataPayload = "{}" },
                            new WorkflowStep { StepName = "BDD Manager", Status = "Open", DataPayload = "{}" },
                            new WorkflowStep { StepName = "RAF Manager", Status = "Open", DataPayload = "{}" },
                            new WorkflowStep { StepName = "Fichier Enrichie Generation", Status = "Open", DataPayload = "{}" }
                        };

                        context.WorkflowSteps.AddRange(steps);
                        context.SaveChanges();
                    }

                    // Seed HecateCategorieRwa sample data
                    if (!context.HecateCategorieRwas.Any())
                    {
                        var categorieRwaData = new List<HecateCategorieRwa>
                        {
                            new HecateCategorieRwa { Libelle = "ACT", ValeurMobiliere = "Y" },
                            new HecateCategorieRwa { Libelle = "OBL", ValeurMobiliere = "N" },
                            new HecateCategorieRwa { Libelle = "EQD", ValeurMobiliere = "Y" },
                            new HecateCategorieRwa { Libelle = "REP", ValeurMobiliere = "Y" }
                        };

                        context.HecateCategorieRwas.AddRange(categorieRwaData);
                        context.SaveChanges();
                    }

                    // Seed HecateTypeBloomberg sample data
                    if (!context.HecateTypeBloombergs.Any())
                    {
                        var typeBloombergData = new List<HecateTypeBloomberg>
                        {
                            new HecateTypeBloomberg { Libelle = "EQUITY" },
                            new HecateTypeBloomberg { Libelle = "CORP" },
                            new HecateTypeBloomberg { Libelle = "GOVT" },
                            new HecateTypeBloomberg { Libelle = "COMDTY" }
                        };

                        context.HecateTypeBloombergs.AddRange(typeBloombergData);
                        context.SaveChanges();
                    }

                    // Seed HecateCatDepositaire1 sample data
                    if (!context.HecateCatDepositaire1s.Any())
                    {
                        var catDepositaire1Data = new List<HecateCatDepositaire1>
                        {
                            new HecateCatDepositaire1 { LibelleDepositaire1 = "Open ended - Listed common investment funds EU" },
                            new HecateCatDepositaire1 { LibelleDepositaire1 = "Listed convertible  redeemable fixed rate bond" },
                            new HecateCatDepositaire1 { LibelleDepositaire1 = "Listed fixed rate bond / note" }
                        };

                        context.HecateCatDepositaire1s.AddRange(catDepositaire1Data);
                        context.SaveChanges();
                    }

                    // Seed HecateCatDepositaire2 sample data
                    if (!context.HecateCatDepositaire2s.Any())
                    {
                        var catDepositaire2Data = new List<HecateCatDepositaire2>
                        {
                            new HecateCatDepositaire2 { LibelleDepositaire2 = "TestX" },
                            new HecateCatDepositaire2 { LibelleDepositaire2 = "TestY" }
                        };

                        context.HecateCatDepositaire2s.AddRange(catDepositaire2Data);
                        context.SaveChanges();
                    }

                    // Seed HecateEquivalenceCatRwa sample data
                    if (!context.HecateEquivalenceCatRwas.Any())
                    {
                        // Get the seeded data for foreign key references
                        var actCategory = context.HecateCategorieRwas.First(c => c.Libelle == "ACT");
                        var eqdCategory = context.HecateCategorieRwas.First(c => c.Libelle == "EQD");
                        var openEndedCat1 = context.HecateCatDepositaire1s.First(c => c.LibelleDepositaire1 == "Open ended - Listed common investment funds EU");
                        var convertibleCat1 = context.HecateCatDepositaire1s.First(c => c.LibelleDepositaire1 == "Listed convertible  redeemable fixed rate bond");
                        var testXCat2 = context.HecateCatDepositaire2s.First(c => c.LibelleDepositaire2 == "TestX");

                        var equivalenceData = new List<HecateEquivalenceCatRwa>
                        {
                            new HecateEquivalenceCatRwa 
                            { 
                                Source = "090", 
                                RefCatDepositaire1 = openEndedCat1.IdDepositaire1, 
                                RefCatDepositaire2 = 0, // Empty as specified
                                RefCategorieRwa = actCategory.IdCatRwa 
                            },
                            new HecateEquivalenceCatRwa 
                            { 
                                Source = "090", 
                                RefCatDepositaire1 = convertibleCat1.IdDepositaire1, 
                                RefCatDepositaire2 = testXCat2.IdDepositaire2, 
                                RefCategorieRwa = eqdCategory.IdCatRwa 
                            }
                        };

                        context.HecateEquivalenceCatRwas.AddRange(equivalenceData);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    // If seeding fails, continue; dev environment may not have DB connectivity for non-development.
                    Console.WriteLine($"Warning: seeding sample data failed: {ex.Message}");
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            // Add response caching middleware
            app.UseResponseCaching();

            // Add session middleware for workflow orchestrator persistence
            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<WorkflowHub>("/workflowHub");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            if (app.Environment.IsDevelopment())
            {
                app.UseWebSockets();
                // Use Vite Dev Server as middleware.
                app.UseViteDevelopmentServer(true);
            }

            app.Run();
        }
    }
}
