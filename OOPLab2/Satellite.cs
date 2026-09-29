namespace OOPLab2;

public class Satellite
{
    private string _name;
    private SType _type;
    private double _orbitAltitude;
    private DateTime _launchDate;

    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            if (value == null || value.Length < 3 || value.Length > 15)
                throw new Exception("Назва має містити від 3 до 15 символів");
            _name = value;
        }
    }

    public SType Type
    {
        get
        {
            return _type;
        }
        set
        {
            if (!Enum.IsDefined(typeof(SType), value))
                throw new Exception("Вказано неіснуючий тип супутника");
            _type = value;
        }
    }

    public double OrbitAltitude
    {
        get
        {
            return _orbitAltitude;
        }
        set
        {
            if (value < 160 || value > 40000)
                throw new Exception("Висота орбіти має бути в межах 160 - 40000 км");
            _orbitAltitude = value;
        }
    }

    public DateTime LaunchDate
    {
        get
        {
            return _launchDate;
        }
        set
        {
            DateTime minDate = new DateTime(1957, 10, 4);
            if (value < minDate || value > DateTime.Now)
                throw new Exception($"Дата запуску має бути в межах {minDate:dd.MM.yyyy} - {DateTime.Now:dd.MM.yyyy}");
            _launchDate = value;
        }
    }

    public bool IsActive { get; private set; } = true;

    public double FuelLevel { get; private set; } = 100;

    public int TransmissionPower { get; set; } = 20;

    public string OrbitCategory
    {
        get
        {
            if (OrbitAltitude < 2000)
                return "LEO (низька)";
            if (OrbitAltitude < 35786)
                return "MEO (середня)";
            return "GEO (геостаціонарна)";
        }
    }

    public string GetData()
    {
        EnsureActive();
        return $"Супутник '{Name}' успішно передав дані з висоти {OrbitAltitude} км " +
               $"(потужність передавача {TransmissionPower} Вт)";
    }

    public string SwitchPower()
    {
        EnsureConnection();
        IsActive = !IsActive;
        return $"Живлення супутника '{Name}' переключено на {(IsActive ? "On" : "Off")}";
    }

    public string AdjustAltitude(double delta)
    {
        EnsureActive();

        double requiredFuel = CalculateRequiredFuel(delta);
        EnsureEnoughFuel(requiredFuel);

        OrbitAltitude += delta;
        FuelLevel -= requiredFuel;

        return $"Нова висота супутника '{Name}': {OrbitAltitude} км ({OrbitCategory})";
    }

    public string SimulateTimePassing(int days)
    {
        EnsureActive();

        if (OrbitAltitude >= 600)
            return $"Пройшло днів: {days}. Висота не змінилася (опір атмосфери відсутній)";

        double totalDrop = CalculateDailyDrop() * days;
        double newAltitude = OrbitAltitude - totalDrop;

        if (newAltitude < 160)
        {
            IsActive = false;
            _orbitAltitude = 0;
            return $"Супутник '{Name}' зійшов з орбіти та згорів у щільних шарах атмосфери";
        }

        _orbitAltitude = newAltitude;
        return $"Пройшло днів: {days}. Супутник '{Name}' втратив {totalDrop} км висоти. " +
               $"Поточна висота: {OrbitAltitude} км";
    }

    private void EnsureActive()
    {
        if (!IsActive)
            throw new Exception($"Супутник '{Name}' неактивний");
    }

    private void EnsureConnection()
    {
        if (OrbitAltitude < 160)
            throw new Exception($"Немає зв'язку із супутником '{Name}'");
    }

    private double CalculateRequiredFuel(double delta)
    {
        return Math.Abs(delta) * 0.1;
    }

    private void EnsureEnoughFuel(double required)
    {
        if (FuelLevel < required)
            throw new Exception(
                $"Недостатньо палива. Потрібно {required}, є {FuelLevel}");
    }

    private double CalculateDailyDrop()
    {
        if (OrbitAltitude < 300)
            return 0.8;
        return 0.2;
    }
}