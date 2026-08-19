using Microsoft.Maui.Devices.Sensors;

namespace Avae.Blazor.Essentials;

internal class BlazorOrientationSensor : IOrientationSensor
{
    BlazorSensors.AbsoluteOrientationSensor sensor;
    public BlazorOrientationSensor(BlazorSensors.AbsoluteOrientationSensor sensor)
    {
        this.sensor = sensor;
        this.sensor.OnReading += Accelerometer_OnReading;
    }

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        ReadingChanged?.Invoke(sender, new OrientationSensorChangedEventArgs(
            new OrientationSensorData(sensor.X, sensor.Y, sensor.Z, sensor.W)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => sensor.Activated;

    public event EventHandler<OrientationSensorChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        sensor.Frequency = sensorSpeed.ToPlatform();
        sensor.Start();
    }

    public void Stop()
    {
        sensor.Stop();
    }
}
