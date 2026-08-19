using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avae.Blazor.Essentials;

internal class BlazorMagnetometer : IMagnetometer
{
    BlazorSensors.Magnetometer magnetometer;
    public BlazorMagnetometer(BlazorSensors.Magnetometer magnetometer)
    {
        this.magnetometer = magnetometer;
        this.magnetometer.OnReading += Accelerometer_OnReading;
    }

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        ReadingChanged?.Invoke(sender, new MagnetometerChangedEventArgs(
            new MagnetometerData(magnetometer.X, magnetometer.Y, magnetometer.Z)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => magnetometer.Activated;

    public event EventHandler<MagnetometerChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        magnetometer.Frequency = sensorSpeed.ToPlatform();
        magnetometer.Start();
    }

    public void Stop()
    {
        magnetometer.Stop();
    }
}
