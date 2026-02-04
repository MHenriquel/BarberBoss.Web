using BarberBoss.Application.UseCases.Appointments.CompleteAppointment;
using BarberBoss.Application.UseCases.Appointments.ScheduleAppointment;
using BarberBoss.Application.UseCases.Finance.ConfirmPayment;
using BarberBoss.Application.UseCases.Finance.CreatePaymentIntent;
using BarberBoss.Application.UseCases.Finance.RegisterExpense;
using BarberBoss.Infrastructure;
using BarberBoss.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using BarberBoss.Domain.Entities;
using BarberBoss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BarberBossDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


//builder.Services.AddRazorPages();

builder.Services.AddRazorPages(options =>
{
    // Protege a área Admin
    options.Conventions.AuthorizePage("/Admin");

    // Permite login sem autenticação
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Account/AccessDenied");
});


builder.Services.AddInfrastructure(builder.Configuration);


builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = false;

        // Login simples (para PoC). Em produção: reforçar.
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<BarberBossDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});


builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ScheduleAppointmentHandler).Assembly));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ScheduleAppointmentValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// --- API (prefix /api) ---
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/appointments", async (ScheduleAppointmentCommand cmd, IMediator mediator, IValidator<ScheduleAppointmentCommand> validator, CancellationToken ct) =>
{
    var val = await validator.ValidateAsync(cmd, ct);
    if (!val.IsValid) return Results.ValidationProblem(val.ToDictionary());
    var id = await mediator.Send(cmd, ct);
    return Results.Created($"/api/appointments/{id}", new { id });
});

app.MapPost("/api/appointments/complete", async (CompleteAppointmentCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var invoiceId = await mediator.Send(cmd, ct);
    return Results.Ok(new { invoiceId });
});

app.MapPost("/api/finance/expenses", async (RegisterExpenseCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var id = await mediator.Send(cmd, ct);
    return Results.Created($"/api/finance/expenses/{id}", new { id });
});

app.MapPost("/api/finance/payments/intent", async (CreatePaymentIntentCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(cmd, ct);
    return Results.Ok(result);
});

app.MapPost("/api/finance/payments/confirm", async (ConfirmPaymentCommand cmd, IMediator mediator, CancellationToken ct) =>
{
    var ok = await mediator.Send(cmd, ct);
    return ok ? Results.Ok(new { ok = true }) : Results.BadRequest(new { ok = false });
});


// Lista de profissionais por filial
app.MapGet("/api/barbers", async (
    Guid companyId,
    Guid branchId,
    BarberBossDbContext db,
    CancellationToken ct) =>
{
    var barbers = await db.Barbers
        .Where(b => b.CompanyId == companyId && b.BranchId == branchId && b.Active)
        .OrderBy(b => b.FullName)
        .Select(b => new { b.Id, b.FullName })
        .ToListAsync(ct);

    return Results.Ok(barbers);
});

//record AvailabilitySlotDto(string Time, bool Available);

// Disponibilidade (slots livres) por dia + barbeiro
app.MapGet("/api/availability", async (
    Guid companyId,
    Guid branchId,
    Guid barberId,
    string date, // YYYY-MM-DD
    BarberBossDbContext db,
    CancellationToken ct) =>
{
    if (!DateOnly.TryParse(date, out var day))
        return Results.BadRequest(new { error = "Parâmetro date inválido. Use YYYY-MM-DD." });

    var dayStart = day.ToDateTime(new TimeOnly(0, 0));
    var dayEnd = day.ToDateTime(new TimeOnly(23, 59, 59));

    // Horário de trabalho (depois a gente deixa configurável por filial/profissional)
    var workStart = day.ToDateTime(new TimeOnly(9, 0));
    var workEnd = day.ToDateTime(new TimeOnly(18, 0));
    var slotMinutes = 40;

    var appointments = await db.Appointments
        .Where(a =>
            a.CompanyId == companyId &&
            a.BranchId == branchId &&
            a.BarberId == barberId &&
            a.Status != BarberBoss.Domain.Enums.AppointmentStatus.Cancelled &&
            a.StartsAt <= dayEnd &&
            a.EndsAt >= dayStart)
        .Select(a => new { a.StartsAt, a.EndsAt })
        .ToListAsync(ct);

    var availableTimes = new List<string>();

    for (var t = workStart; t < workEnd; t = t.AddMinutes(slotMinutes))
    {
        var slotStart = t;
        var slotEnd = t.AddMinutes(slotMinutes);

        var busy = appointments.Any(a => slotStart < a.EndsAt && slotEnd > a.StartsAt);
        if (!busy)
            availableTimes.Add(slotStart.ToString("HH:mm"));
    }

    return Results.Ok(new
    {
        date = day.ToString("yyyy-MM-dd"),
        barberId,
        slots = availableTimes
    });
});


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BarberBossDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Companies.AnyAsync())
    {
        var company = new Company("BarberBoss Demo");
        db.Companies.Add(company);

        var branch = company.AddBranch("Matriz");
        db.Branches.Add(branch);

        var barber1 = new Barber(company.Id, branch.Id, "Keila Barber", new BarberBoss.Domain.Entities.CommissionRule(0.35m, 5m));
        var barber2 = new Barber(company.Id, branch.Id, "Carlos Barber", new BarberBoss.Domain.Entities.CommissionRule(0.30m, 8m));

        db.Barbers.AddRange(barber1, barber2);

        db.Services.Add(new Service(company.Id, branch.Id, "Corte", 30, 50m, null));
        db.Services.Add(new Service(company.Id, branch.Id, "Barba", 30, 40m, null));
        db.Services.Add(new Service(company.Id, branch.Id, "Corte + Barba", 60, 85m, null));

        await db.SaveChangesAsync();
    }
}


if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BarberBossDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    const string adminUser = "admin";
    const string adminPass = "Admin@123";

    var existing = await userManager.FindByNameAsync(adminUser);
    if (existing is null)
    {
        var u = new ApplicationUser
        {
            UserName = adminUser,
            FullName = "Administrador",
            Email = "admin@barberboss.local"
        };

        var created = await userManager.CreateAsync(u, adminPass);
        if (!created.Succeeded)
        {
            Console.WriteLine("Falha ao criar admin: " +
                string.Join(", ", created.Errors.Select(e => e.Description)));
        }
        else
        {
            Console.WriteLine($"Usuário admin criado: {adminUser} / {adminPass}");
        }
    }
}




app.Run();




