using NewTemplate.Context;

namespace NewTemplate;

public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary { get; set; }
}

public class RSeat
{
    public int SeatId { get; set; }
    public int BusId { get; set; }
    public int Price { get; set; }
    public string Name { get; set; }
    public static explicit operator RSeat(Seat seat)
    {
        return new RSeat
        {
            BusId = seat.BusId,
            Name = seat.Name,
            Price = seat.Price,
            SeatId = seat.SeatId
        };
    }
}