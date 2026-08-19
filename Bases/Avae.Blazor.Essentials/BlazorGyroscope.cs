using Microsoft.Maui.Devices.Sensors;

namespace Avae.Blazor.Essentials;

internal class BlazorGyroscope : IGyroscope
{
    BlazorSensors.Gyroscope gyroscope;
    public BlazorGyroscope(BlazorSensors.Gyroscope gyroscope)
    {
        this.gyroscope = gyroscope;
        this.gyroscope.OnReading += Accelerometer_OnReading;
    }

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        ReadingChanged?.Invoke(sender, new GyroscopeChangedEventArgs(
            new GyroscopeData(gyroscope.X, gyroscope.Y, gyroscope.Z)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => gyroscope.Activated;

    public event EventHandler<GyroscopeChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        gyroscope.Frequency = sensorSpeed.ToPlatform();
        gyroscope.Start();
    }

    public void Stop()
    {
        gyroscope.Stop();
    }
}
