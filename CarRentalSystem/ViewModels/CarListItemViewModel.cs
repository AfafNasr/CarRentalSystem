using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;

public class CarListItemViewModel
{
    public Car Car { get; set; } = null!;

    public bool IsAvailableNow { get; set; }

    public DateTime? AvailableFrom { get; set; }

    public List<ReservationPeriodViewModel> ReservedPeriods { get; set; }
        = new();
}