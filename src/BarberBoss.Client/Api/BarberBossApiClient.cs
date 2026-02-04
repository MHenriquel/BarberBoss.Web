using System.Net.Http.Json;

namespace BarberBoss.Client.Api;

public class BarberBossApiClient(HttpClient http)
{
    public async Task<Guid> ScheduleAppointmentAsync(object payload, CancellationToken ct = default)
    {
        var res = await http.PostAsJsonAsync("/api/appointments", payload, ct);
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<Dictionary<string, string>>(cancellationToken: ct);
        return Guid.Parse(body!["id"]);
    }
}
