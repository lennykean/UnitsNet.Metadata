﻿using System;
using System.Collections.Generic;
using System.IO;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Annotations;
using HondataDotNet.Datalog.Core.Quantities;
using HondataDotNet.Datalog.Core.Units;
using HondataDotNet.Datalog.Core.Utils;
using HondataDotNet.Datalog.OBDII;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Units;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed partial class FlashProDatalogFrame : IOBDIIDatalogFrame<FlashProFaultCode, FlashProReadinessTests, FlashProReadinessCode>
    {
        private DatalogFrame _frame;

        public FlashProDatalog? Datalog { get; internal set; }
        public TimeSpan Offset => TimeSpan.FromMilliseconds(_frame.Offset);

        /// <summary>
        /// Engine speed in rpm.
        /// </summary>
        [Sensor("RPM", RotationalSpeedUnit.RevolutionPerMinute, Description = "Engine speed")]
        public double RPM => _frame.RPM;
        int IDatalogFrame.RPM => (int)RPM;
        /// <summary>
        /// Vehicle speed in KilometerPerHour.
        /// </summary>
        [Sensor("VSS", SpeedUnit.KilometerPerHour, Description = "Vehicle speed")]
        public double VSS => _frame.VSS;
        /// <summary>
        /// Gear.
        /// </summary>
        [Sensor("Gear", ScalarUnit.Amount, Description = "Gear")]
        public int Gear => (int)_frame.Gear;
        /// <summary>
        /// Manifold pressure in Bar.
        /// </summary>
        [Sensor("MAP", PressureUnit.Bar, Description = "Manifold pressure")]
        public double MAP => _frame.MAP;
        /// <summary>
        /// Throttle pedal in percent.
        /// </summary>
        [Sensor("TPedal", RatioUnit.DecimalFraction, Description = "Throttle pedal")]
        public double TPedal => _frame.TPedal;
        /// <summary>
        /// Throttle plate in percent
        /// </summary>
        [Sensor("TPlate", RatioUnit.DecimalFraction, Description = "Throttle plate")]
        public double TPlate => _frame.TPlate;
        /// <summary>
        /// Air flow meter in volts.
        /// </summary>
        [Sensor("AFM.v", ElectricPotentialUnit.Volt, Description = "Air flow meter")]
        public double AFMv => _frame.AFMv;
        /// <summary>
        /// Air flow meter in g/s.
        /// </summary>
        [Sensor("AFM", MassFlowUnit.GramPerSecond, Description = "Air flow meter")]
        public double AFM => _frame.AFM;
        /// <summary>
        /// Injector pulse width in ms.
        /// </summary>
        [Sensor("INJ", DurationUnit.Millisecond, Description = "Injector pulse width")]
        public double INJ => _frame.INJ;
        /// <summary>
        /// Ignition advance in degrees.
        /// </summary>
        [Sensor("IGN", AngleUnit.Degree, Description = "Ignition advance")]
        public double IGN => _frame.IGN;
        /// <summary>
        /// Intake air temperature in DegreeCelsius.
        /// </summary>
        [Sensor("IAT", TemperatureUnit.DegreeCelsius, Description = "Intake air temperature")]
        public double IAT => _frame.IAT;
        /// <summary>
        /// Engine coolant temperature in DegreeCelsius.
        /// </summary>
        [Sensor("ECT", TemperatureUnit.DegreeCelsius, Description = "Engine coolant temperature")]
        public double ECT => _frame.ECT;
        /// <summary>
        /// Actual VTC cam angle in degrees.
        /// </summary>
        [Sensor("CAM", AngleUnit.Degree, Description = "Actual VTC cam angle")]
        public double CAM => _frame.CAM;
        /// <summary>
        /// Commanded VTC cam angle in degrees.
        /// </summary>
        [Sensor("CAMCMD", AngleUnit.Degree, Description = "Commanded VTC cam angle")]
        public double CAMCMD => _frame.CAMCMD;
        /// <summary>
        /// Air / fuel ratio lambda.
        /// </summary>
        [Sensor("A / F", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Air / fuel ratio")]
        [AllowUnitConversion(AirFuelRatioUnit.GasolineAirFuelRatio)]
        public double AF => _frame.AF;
        /// <summary>
        /// Target air / fuel ratio lambda.
        /// </summary>
        [Sensor("A / F.CMD", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Target air / fuel ratio")]
        public double AFCMD => _frame.AFCMD;
        /// <summary>
        /// Short term fuel trim lambda.
        /// </summary>
        [Sensor("S.TRIM", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Short term fuel trim")]
        public double STRIM => _frame.STRIM;
        /// <summary>
        /// Long term fuel trim lambda.
        /// </summary>
        [Sensor("L.TRIM", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Long term fuel trim")]
        public double LTRIM => _frame.LTRIM;
        /// <summary>
        /// Fuel system status.
        /// </summary>
        [Sensor("Fuel Status", ScalarUnit.Amount, Description = "Fuel system status")]
        public int FuelStatus => (int)_frame.FuelStatus;
        /// <summary>
        /// Knock level in percent.
        /// </summary>
        [Sensor("Knock Level", RatioUnit.DecimalFraction, Description = "Knock level")]
        public double KLevel => _frame.KLevel;
        /// <summary>
        /// Knock retard in degrees.
        /// </summary>
        [Sensor("Knock Retard", AngleUnit.Degree, Description = "Knock retard")]
        public double KRetard => _frame.KRetard;
        /// <summary>
        /// Knock control in percent.
        /// </summary>
        [Sensor("Knock Control", RatioUnit.DecimalFraction, Description = "Knock control")]
        public double KControl => _frame.KControl;
        /// <summary>
        /// Atmospheric air pressure in Bar.
        /// </summary>
        [Sensor("PA", PressureUnit.Bar, Description = "Atmospheric air pressure")]
        public double PA => _frame.PA;
        /// <summary>
        /// Battery voltage in volts.
        /// </summary>
        [Sensor("BAT", ElectricPotentialUnit.Volt, Description = "Battery voltage")]
        public double BAT => _frame.BAT;
        /// <summary>
        /// Air conditioning clutch.
        /// </summary>
        [Sensor("ACCL", Description = "Air conditioning clutch")]
        public bool ACCL => _frame.ACCL > 0;
        /// <summary>
        /// VTEC spool.
        /// </summary>
        [Sensor("VTS", Description = "VTEC spool")]
        public bool VTS => _frame.VTS > 0;
        /// <summary>
        /// Fuel Economy in L/100km.
        /// </summary>
        [Sensor("Fuel Eco", FuelEfficiencyUnit.LiterPer100Kilometers, Description = "Fuel Economy")]
        public double Eco => _frame.Eco;
        /// <summary>
        /// Fuel Used in cm³.
        /// </summary>
        [Sensor("Fuel Used", VolumeUnit.CubicCentimeter, Description = "Fuel Used")]
        public double FuelUsed => _frame.FuelUsed;
        /// <summary>
        /// Wideband input lambda.
        /// </summary>
        [Sensor("Wideband", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Wideband input")]
        public double Wide => _frame.Wide;
        /// <summary>
        /// Wideband voltage in volts.
        /// </summary>
        [Sensor("Wideband voltage", ElectricPotentialUnit.Volt, Description = "Wideband voltage")]
        public double WideV => _frame.WideV;
        /// <summary>
        /// Knock count #1 cylinder.
        /// </summary>
        [Sensor("Knock count #1 cylinder", ScalarUnit.Amount, Description = "Knock count #1 cylinder")]
        public double KCount1 => _frame.KCount1;
        /// <summary>
        /// Knock count #2 cylinder.
        /// </summary>
        [Sensor("Knock count #2 cylinder", ScalarUnit.Amount, Description = "Knock count #2 cylinder")]
        public double KCount2 => _frame.KCount2;
        /// <summary>
        /// Knock count #3 cylinder.
        /// </summary>
        [Sensor("Knock count #3 cylinder", ScalarUnit.Amount, Description = "Knock count #3 cylinder")]
        public double KCount3 => _frame.KCount3;
        /// <summary>
        /// Knock count #4 cylinder.
        /// </summary>
        [Sensor("Knock count #4 cylinder", ScalarUnit.Amount, Description = "Knock count #4 cylinder")]
        public double KCount4 => _frame.KCount4;
        /// <summary>
        /// Knock count #5 cylinder.
        /// </summary>
        [Sensor("Knock count #5 cylinder", ScalarUnit.Amount, Description = "Knock count #5 cylinder")]
        public double KCount5 => _frame.KCount5;
        /// <summary>
        /// Knock count #6 cylinder.
        /// </summary>
        [Sensor("Knock count #6 cylinder", ScalarUnit.Amount, Description = "Knock count #6 cylinder")]
        public double KCount6 => _frame.KCount6;
        /// <summary>
        /// Boost Controller Duty Cycle in percent.
        /// </summary>
        [Sensor("Boost Control Duty", RatioUnit.DecimalFraction, Description = "Boost Controller Duty Cycle")]
        public double BCDuty => _frame.BCDuty;
        /// <summary>
        /// Knock ignition limit in degrees.
        /// </summary>
        [Sensor("Ign.Limit", AngleUnit.Degree, Description = "Knock ignition limit")]
        public double IgnLimit => _frame.IgnLimit;
        /// <summary>
        /// Engine coolant temperature 2 in DegreeCelsius.
        /// </summary>
        [Sensor("ECT2", TemperatureUnit.DegreeCelsius, Description = "Engine coolant temperature 2")]
        public double ECT2 => _frame.ECT2;
        /// <summary>
        /// Traction control voltage(from ECU) in volts.
        /// </summary>
        [Sensor("TC.Volt", ElectricPotentialUnit.Volt, Description = "Traction control voltage(from ECU)")]
        public double TCV => _frame.TCV;
        /// <summary>
        /// Traction control over slip (from ECU) in percent.
        /// </summary>
        [Sensor("TC.ECUSlip", RatioUnit.DecimalFraction, Description = "Traction control over slip (from ECU)")]
        public double TCECUSlip => _frame.TCECUSlip;
        /// <summary>
        /// Traction control ign retard in degrees.
        /// </summary>
        [Sensor("TC.Retard", AngleUnit.Degree, Description = "Traction control ign retard")]
        public double TCR => _frame.TCR;
        /// <summary>
        /// Traction control left front wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("TC.Left Front", SpeedUnit.KilometerPerHour, Description = "Traction control left front wheel speed")]
        public double TCLF => _frame.TCLF;
        /// <summary>
        /// Traction control right front wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("TC.Right Front", SpeedUnit.KilometerPerHour, Description = "Traction control right front wheel speed")]
        public double TCRF => _frame.TCRF;
        /// <summary>
        /// Traction control left rear wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("TC.Left Rear", SpeedUnit.KilometerPerHour, Description = "Traction control left rear wheel speed")]
        public double TCLR => _frame.TCLR;
        /// <summary>
        /// Traction control right rear wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("TC.Right Rear", SpeedUnit.KilometerPerHour, Description = "Traction control right rear wheel speed")]
        public double TCRR => _frame.TCRR;
        /// <summary>
        /// Traction control slip in percent.
        /// </summary>
        [Sensor("TC.Slip", RatioUnit.DecimalFraction, Description = "Traction control slip")]
        public double TCSlip => _frame.TCSlip;
        /// <summary>
        /// Traction control turning factor in percent.
        /// </summary>
        [Sensor("TC.Turning", RatioUnit.DecimalFraction, Description = "Traction control turning factor")]
        public double TCTurn => _frame.TCTurn;
        /// <summary>
        /// Traction control over slip (from TC) in percent.
        /// </summary>
        [Sensor("TC.OverSlip", RatioUnit.DecimalFraction, Description = "Traction control over slip (from TC)")]
        public double TCOverSlip => _frame.TCOverSlip;
        /// <summary>
        /// Traction control output voltage in volts.
        /// </summary>
        [Sensor("TC.Output", ElectricPotentialUnit.Volt, Description = "Traction control output voltage")]
        public double TCOut => _frame.TCOut;
        /// <summary>
        /// Secondary intake runners.
        /// </summary>
        [Sensor("SVS", Description = "Secondary intake runners")]
        public bool SVS => _frame.SVS > 0;
        /// <summary>
        /// Fuel tank pressure in Bar.
        /// </summary>
        [Sensor("PTANK", PressureUnit.Bar, Description = "Fuel tank pressure")]
        public double PTANK => _frame.PTANK;
        /// <summary>
        /// Purge duty cycle in percent.
        /// </summary>
        [Sensor("Purge", RatioUnit.DecimalFraction, Description = "Purge duty cycle")]
        public double Purge => _frame.Purge;
        /// <summary>
        /// Air / fuel ratio lambda.
        /// </summary>
        [Sensor("A / F #2", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Air / fuel ratio")]
        public double AFBank2 => _frame.AFBank2;
        /// <summary>
        /// Target air / fuel ratio lambda.
        /// </summary>
        [Sensor("A / F.CMD #2", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Target air / fuel ratio")]
        public double AFCMDBank2 => _frame.AFCMDBank2;
        /// <summary>
        /// Short term fuel trim lambda.
        /// </summary>
        [Sensor("S.TRIM #2", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Short term fuel trim")]
        public double STRIMBank2 => _frame.STRIMBank2;
        /// <summary>
        /// Long term fuel trim lambda.
        /// </summary>
        [Sensor("L.TRIM #2", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Long term fuel trim")]
        public double LTRIMBank2 => _frame.LTRIMBank2;
        /// <summary>
        /// Fuel system status in .
        /// </summary>
        [Sensor("Fuel Status #2", ScalarUnit.Amount, Description = "Fuel system status")]
        public int FuelStatusBank2 => (int)_frame.FuelStatusBank2;
        /// <summary>
        /// Injector pulse width in ms.
        /// </summary>
        [Sensor("INJ #2", DurationUnit.Millisecond, Description = "Injector pulse width")]
        public double INJBank2 => _frame.INJBank2;
        /// <summary>
        /// Intake air temperature 2 in DegreeCelsius.
        /// </summary>
        [Sensor("IAT2", TemperatureUnit.DegreeCelsius, Description = "Intake air temperature 2")]
        public double IAT2 => _frame.IAT2;
        /// <summary>
        /// Boost pressure in Bar.
        /// </summary>
        [Sensor("BP", PressureUnit.Bar, Description = "Boost pressure")]
        public double BP => _frame.BP;
        /// <summary>
        /// Actual exhaust cam angle in degrees.
        /// </summary>
        [Sensor("EXCAM", AngleUnit.Degree, Description = "Actual exhaust cam angle")]
        public double EXCAM => _frame.EXCAM;
        /// <summary>
        /// Commanded exhaust cam angle in degrees.
        /// </summary>
        [Sensor("EXCAMCMD", AngleUnit.Degree, Description = "Commanded exhaust cam angle")]
        public double EXCAMCMD => _frame.EXCAMCMD;
        /// <summary>
        /// Direct Injection Fuel Pressure in Bar.
        /// </summary>
        [Sensor("DIFP", PressureUnit.Bar, Description = "Direct Injection Fuel Pressure")]
        public double DIFP => _frame.DIFP;
        /// <summary>
        /// Waste gate opening in mm.
        /// </summary>
        [Sensor("Waste Gate", LengthUnit.Millimeter, Description = "Waste gate opening")]
        public double WG => _frame.WG;
        /// <summary>
        /// Waste gate opening in mm.
        /// </summary>
        [Sensor("WGCMD", LengthUnit.Millimeter, Description = "Waste gate opening")]
        public double WGCMD => _frame.WGCMD;
        /// <summary>
        /// Boost pressure CMD in Bar.
        /// </summary>
        [Sensor("BP CMD", PressureUnit.Bar, Description = "Boost pressure CMD")]
        public double BPCMD => _frame.BPCMD;
        /// <summary>
        /// Direct Injection Fuel Pressure CMD in Bar.
        /// </summary>
        [Sensor("DIFPCMD", PressureUnit.Bar, Description = "Direct Injection Fuel Pressure CMD")]
        public double DIFPCMD => _frame.DIFPCMD;
        /// <summary>
        /// Air Charge in percent.
        /// </summary>
        [Sensor("AIRC", RatioUnit.DecimalFraction, Description = "Air Charge")]
        public double AIRC => _frame.AIRC;
        /// <summary>
        /// EGR rate in percent.
        /// </summary>
        [Sensor("EGR", RatioUnit.DecimalFraction, Description = "EGR rate")]
        public double EGR => _frame.EGR;
        /// <summary>
        /// Oil pressure in Bar.
        /// </summary>
        [Sensor("Oil Press", PressureUnit.Bar, Description = "Oil pressure")]
        public double OilPress => _frame.OilPress;
        /// <summary>
        /// Catalyst temperature in DegreeCelsius.
        /// </summary>
        [Sensor("Cat.T", TemperatureUnit.DegreeCelsius, Description = "Catalyst temperature")]
        public double CatT => _frame.CatT;
        /// <summary>
        /// Knock retard #1 cylinder in degrees.
        /// </summary>
        [Sensor("Knock Retard 1", AngleUnit.Degree, Description = "Knock retard #1 cylinder")]
        public double KRetard1 => _frame.KRetard1;
        /// <summary>
        /// Knock retard #2 cylinder in degrees.
        /// </summary>
        [Sensor("Knock Retard 2", AngleUnit.Degree, Description = "Knock retard #2 cylinder")]
        public double KRetard2 => _frame.KRetard2;
        /// <summary>
        /// Knock retard #3 cylinder in degrees.
        /// </summary>
        [Sensor("Knock Retard 3", AngleUnit.Degree, Description = "Knock retard #3 cylinder")]
        public double KRetard3 => _frame.KRetard3;
        /// <summary>
        /// Knock retard #4 cylinder in degrees.
        /// </summary>
        [Sensor("Knock Retard 4", AngleUnit.Degree, Description = "Knock retard #4 cylinder")]
        public double KRetard4 => _frame.KRetard4;
        /// <summary>
        /// G Sensor Lateral in g.
        /// </summary>
        [Sensor("G Lat", AccelerationUnit.StandardGravity, Description = "G Sensor Lateral")]
        public double GLat => _frame.GLat;
        /// <summary>
        /// G Sensor Long in g.
        /// </summary>
        [Sensor("G Long", AccelerationUnit.StandardGravity, Description = "G Sensor Long")]
        public double GLong => _frame.GLong;
        /// <summary>
        /// Yaw Sensor in Degree/s.
        /// </summary>
        [Sensor("Yaw", RotationalSpeedUnit.DegreePerSecond, Description = "Yaw Sensor")]
        public double Yaw => _frame.Yaw;
        /// <summary>
        /// G Sensor Z axis in g.
        /// </summary>
        [Sensor("G Zaxis", AccelerationUnit.StandardGravity, Description = "G Sensor Z axis")]
        public double GZ => _frame.GZ;
        /// <summary>
        /// Ethanol content in percent.
        /// </summary>
        [Sensor("Ethanol", RatioUnit.DecimalFraction, Description = "Ethanol content")]
        public double Ethanol => _frame.Ethanol;
        /// <summary>
        /// CVT temperature in DegreeCelsius.
        /// </summary>
        [Sensor("CVT Temp", TemperatureUnit.DegreeCelsius, Description = "CVT temperature")]
        public double CVTTemp => _frame.CVTTemp;
        /// <summary>
        /// ABS left front wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("ABS.Left Front", SpeedUnit.KilometerPerHour, Description = "ABS left front wheel speed")]
        public double ABSLF => _frame.ABSLF;
        /// <summary>
        /// ABS right front wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("ABS.Right Front", SpeedUnit.KilometerPerHour, Description = "ABS right front wheel speed")]
        public double ABSRF => _frame.ABSRF;
        /// <summary>
        /// ABS left rear wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("ABS.Left Rear", SpeedUnit.KilometerPerHour, Description = "ABS left rear wheel speed")]
        public double ABSLR => _frame.ABSLR;
        /// <summary>
        /// ABS right rear wheel speed in KilometerPerHour.
        /// </summary>
        [Sensor("ABS.Right Rear", SpeedUnit.KilometerPerHour, Description = "ABS right rear wheel speed")]
        public double ABSRR => _frame.ABSRR;
        /// <summary>
        /// Clutch pedal position in percent.
        /// </summary>
        [Sensor("Clutch Pos", RatioUnit.DecimalFraction, Description = "Clutch pedal position")]
        public double ClutchPos => _frame.ClutchPos;
        /// <summary>
        /// Brake pressure in Bar.
        /// </summary>
        [Sensor("Brake Press", PressureUnit.Bar, Description = "Brake pressure")]
        [AllowUnitConversion(PressureUnit.Millibar)]
        [AllowUnitConversion(PressureUnit.PoundForcePerSquareInch)]
        public double BrakePress => _frame.BrakePress;
        /// <summary>
        /// Steering wheel angle in degrees.
        /// </summary>
        [Sensor("Steer Ang", AngleUnit.Degree, Description = "Steering wheel angle")]
        [AllowUnitConversion(AngleUnit.Degree)]
        public double SteerAng => _frame.SteerAng;
        /// <summary>
        /// Steering wheel torque in N·m.
        /// </summary>
        [Sensor("Steer Trq", TorqueUnit.NewtonMeter, Description = "Steering wheel torque")]
        [AllowUnitConversion(TorqueUnit.NewtonMeter)]
        public double SteerTrq => _frame.SteerTrq;
        /// <summary>
        /// Fuel pump duty cycle in percent.
        /// </summary>
        [Sensor("Fuel Pump Duty", RatioUnit.DecimalFraction, Description = "Fuel pump duty cycle")]
        [AllowUnitConversion(RatioUnit.Percent)]
        public double FuelP => _frame.FuelP;
        /// <summary>
        /// Air flow meter frequency in Hz.
        /// </summary>
        [Sensor("AFM Hz", FrequencyUnit.Hertz, Description = "Air flow meter frequency")]
        [AllowUnitConversion(FrequencyUnit.Hertz)]
        public double AFMHz => _frame.AFMHz;
        /// <summary>
        /// Injector duty cycle in percent.
        /// </summary>
        [Sensor("DUTY", RatioUnit.DecimalFraction, Description = "Injector duty cycle")]
        [AllowUnitConversion(RatioUnit.Percent)]
        public double DUTY => RPM * INJ / 1200;
        /// <summary>
        /// Knock count.
        /// </summary>
        [Sensor("Knock count", ScalarUnit.Amount, Description = "Knock count")]
        public double KCount => KCount1 + KCount2 + KCount3 + KCount4 + KCount5 + KCount6;
        /// <summary>
        /// Total fuel trim in lambda.
        /// </summary>
        [Sensor("Trim", AirFuelRatioUnit.Lambda, typeof(AirFuelRatio), Description = "Total fuel trim")]
        [AllowUnitConversion(AirFuelRatioUnit.GasolineAirFuelRatio)]
        public double Trim => LTRIM + STRIM;

        public IReadOnlyDictionary<FlashProReadinessTests, FlashProReadinessCode> ReadinessCodes => throw new NotImplementedException();
        public IReadOnlyCollection<FlashProFaultCode> FaultCodes => throw new NotImplementedException();

        IReadOnlyCollection<IFaultCode> IDatalogFrame.FaultCodes => throw new NotImplementedException();

        internal static FlashProDatalogFrame ReadFromStream(Stream stream, int frameSize)
        {
            return new()
            {
                _frame = stream.ReadStruct<DatalogFrame>(0, frameSize)
            };
        }

        internal int Save(Stream stream, int frameNumber, int frameSize)
        {
            _frame.FrameNumber = frameNumber;

            return stream.WriteStruct(_frame, offset: 0, length: frameSize);
        }
    }
}
