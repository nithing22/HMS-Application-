
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using HMS_Api;
using System.Text;
using HMS_Data_Layer.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using HMS_Data_Layer.HMS_Data;
using HMS_Data_Layer.HMS_IData;
using Microsoft.Extensions.Caching.Memory;
using HMS_Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Converters.Add(new AllowEmptyStringJsonConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HMS_Api_DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null)
    ));

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 200; // 200MB cache limit
    options.CompactionPercentage = 0.2; // 20% compaction
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 64 * 1024 * 1024; // 64MB
    options.UseCaseSensitivePaths = false;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();



builder.Services.AddScoped<IAcknowledgeReturn, AcknowledgeReturn>();
builder.Services.AddScoped<IAdditionalCharge, AdditionalCharge>();
builder.Services.AddScoped<IAdditionalChargeAssociation, AdditionalChargeAssociation>();
builder.Services.AddScoped<IAdditionalChargeRule, AdditionalChargeRule>();
builder.Services.AddScoped<IAdditionalChargeSurgery, AdditionalChargeSurgery>();
builder.Services.AddScoped<IAppointmentConfiguration, AppointmentConfiguration>();
builder.Services.AddScoped<IArea, Area>();
builder.Services.AddScoped<IAssignedPlan, AssignedPlan>();
builder.Services.AddScoped<IAutoCharge, AutoCharge>();
builder.Services.AddScoped<IBed, Bed>();
builder.Services.AddScoped<IBillAggrementLine, BillAggrementLine>();
builder.Services.AddScoped<IBillagrement, Billagrement>();
builder.Services.AddScoped<IBilling, Billing>();
builder.Services.AddScoped<IClinicalChart, ClinicalChart>();
builder.Services.AddScoped<IClinicalPrecedure, ClinicalPrecedure>();
builder.Services.AddScoped<IClinicalSetup, ClinicalSetup>();
builder.Services.AddScoped<IConfigurationm, Configuration>();
builder.Services.AddScoped<IDefaultTariffPlan, DefaultTariffPlan>();
builder.Services.AddScoped<IDischargeClearance, DischargeClearance>();
builder.Services.AddScoped<IDischargeClearanceSetup, DischargeClearanceSetup>();
builder.Services.AddScoped<IEncounter, Encounter>();
builder.Services.AddScoped<IFacility, Facility>();
builder.Services.AddScoped<IFacilityDepartment, FacilityDepartment>();
builder.Services.AddScoped<IFacilityDepartmentPatientType, FacilityDepartmentPatientType>();
builder.Services.AddScoped<IFacilityDepartmentProvider, FacilityDepartmentProvider>();
builder.Services.AddScoped<IFacilityDepartmentServiceLocation, FacilityDepartmentServiceLocation>();
builder.Services.AddScoped<IGRNAgainstPO, GRNAgainstPO>();
builder.Services.AddScoped<IGRNDirect, GRNDirect>();
builder.Services.AddScoped<IHoliday, Holiday>();
builder.Services.AddScoped<IIndent, Indent>();
builder.Services.AddScoped<IIndentIssue, IndentIssue>();
builder.Services.AddScoped<ILaboratory, Laboratory>();
builder.Services.AddScoped<ILabTestMaster, LabTestMaster>();
builder.Services.AddScoped<IMaster, Master>();
builder.Services.AddScoped<IPatient, Patient>();
builder.Services.AddScoped<IPatientAccountBill, PatientAccountBill>();
builder.Services.AddScoped<IPatientAccountCharge, PatientAccountCharge>();
builder.Services.AddScoped<IPatientAccountOrders, PatientAccountOrders>();
builder.Services.AddScoped<IPatientIndent, PatientIndent>();
builder.Services.AddScoped<IPayer, Payer>();
builder.Services.AddScoped<IPharmacy, Pharmacy>();
builder.Services.AddScoped<IPharmacyReturn, PharmacyReturn>();
builder.Services.AddScoped<IPlace, Place>();
builder.Services.AddScoped<IPriceDefinition, PriceDefinition>();
builder.Services.AddScoped<IPriceTariff, PriceTariff>();
builder.Services.AddScoped<IPriceTariffLine, PriceTariffLine>();
builder.Services.AddScoped<IProductClassification, ProductClassification>();
builder.Services.AddScoped<IProductDefinition, ProductDefinition>();
builder.Services.AddScoped<IProvider, Provider>();
builder.Services.AddScoped<IProviderAbsence, ProviderAbsence>();
builder.Services.AddScoped<IPurchaseOrder, PurchaseOrder>();
builder.Services.AddScoped<IQueue, Queue>();
builder.Services.AddScoped<IReceipt, Receipt>();
builder.Services.AddScoped<IReceiptAllocation, ReceiptAllocation>();
builder.Services.AddScoped<IReceiptInstrument, ReceiptInstrument>();
builder.Services.AddScoped<IRecuringCharge, RecuringCharge>();
builder.Services.AddScoped<IReferral, Referral>();
builder.Services.AddScoped<IRefund, Refund>();
builder.Services.AddScoped<IScheduleAvailabilityCalender, ScheduleAvailabilityCalender>();
builder.Services.AddScoped<IScheduleProvider, ScheduleProvider>();
builder.Services.AddScoped<IScheduleProviderAppointment, ScheduleProviderAppointment>();
builder.Services.AddScoped<IScheduleTemplate, ScheduleTemplate>();
builder.Services.AddScoped<IService, Service>();
builder.Services.AddScoped<IServiceClassification, ServiceClassification>();
builder.Services.AddScoped<IServiceLocation, ServiceLocation>();
builder.Services.AddScoped<IServicePriceDefinition, ServicePriceDefinition>();
builder.Services.AddScoped<ISpecialEvents, SpecialEvent>();
builder.Services.AddScoped<IState, State>();
builder.Services.AddScoped<IStore, Store>();
builder.Services.AddScoped<IStoreReturn, StoreReturn>();
builder.Services.AddScoped<ITemplate, Template>();
builder.Services.AddScoped<IUom, Uom>();
builder.Services.AddScoped<IUser, User>();
builder.Services.AddScoped<IVendor, Vendor>();
builder.Services.AddScoped<IVendorReturn, VendorReturn>();
builder.Services.AddScoped<Iward, Ward>();
builder.Services.AddScoped<IWardManagement, WardManagement>();
builder.Services.AddScoped<IPatientClaim, PatientClaim>();



var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new string[0];
builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCorsPolicy", builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCorsPolicy", policyBuilder =>
    {
        policyBuilder
            .SetIsOriginAllowed(origin =>
            {
                // Log the origin being checked
                Console.WriteLine($"Checking CORS origin: {origin}");

                // Development environments
                if (origin.StartsWith("http://localhost:") ||
                    origin.StartsWith("https://localhost:"))
                {
                    Console.WriteLine("Allowing localhost origin");
                    return true;
                }

                // Allow any IP in local network
                if (origin.StartsWith("http://192.168.") ||
                    origin.StartsWith("https://192.168.") ||
                    origin.StartsWith("http://172.") ||
                    origin.StartsWith("https://172.") ||
                    origin.StartsWith("http://10.") ||
                    origin.StartsWith("https://10."))
                {
                    Console.WriteLine("Allowing local network origin");
                    return true;
                }

                // Check against configured allowed origins
                var isAllowed = builder.Configuration.GetSection("AllowedOrigins")
                    .Get<string[]>()?.Contains(origin) ?? false;
                Console.WriteLine($"Origin {origin} allowed by configuration: {isAllowed}");
                return isAllowed;
            })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Content-Disposition", "Access-Control-Allow-Origin")
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = ".YourApp.Session";  // Add this line
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.SameSite = SameSiteMode.None;  // Adjust this based on your authentication flow
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();



app.UseCors("DynamicCorsPolicy");

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
