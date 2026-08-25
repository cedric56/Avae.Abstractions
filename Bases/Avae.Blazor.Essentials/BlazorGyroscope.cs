using Avae.Core;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Blazor.Essentials;

internal class BlazorGyroscope : IGyroscope
{
    BlazorSensors.Gyroscope? gyroscope;

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        if(gyroscope != null) 
        ReadingChanged?.Invoke(sender, new GyroscopeChangedEventArgs(
            new GyroscopeData(gyroscope.X, gyroscope.Y, gyroscope.Z)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => gyroscope?.Activated ?? false;

    public event EventHandler<GyroscopeChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        if (gyroscope == null)
        {
            gyroscope = ServiceLocator.GetScopedRequiredService<BlazorSensors.Gyroscope>();
            gyroscope.OnReading += Accelerometer_OnReading;
        }

        gyroscope.Frequency = sensorSpeed.ToPlatform();
        gyroscope.Start();
    }

    public void Stop()
    {
        gyroscope?.Stop();
    }
}
