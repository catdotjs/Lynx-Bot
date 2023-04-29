using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord.WebSocket;
using Lynx_Bot.Commands.Units;

namespace Lynx_Bot.Commands {
    static class Conversion {
        public static async Task ConvertAngle(SocketSlashCommand Context) {

        }
        public static async Task ConvertTemperature(SocketSlashCommand Context) {

        }
        public static async Task ConvertMass(SocketSlashCommand Context) {

        }
    }
}

namespace Lynx_Bot.Commands.Units {
    enum UnitType {
        // ---  Angle  ---
        Degree,
        Radian,
        Gradian,

        // ---  Temperature  ---
        Celsius,
        Kelvin,
        Fahrenheit,

        // ---  Mass  ---
        // Metric
        Gram,
        Kilogram,
        Metric_Tonne,

        // Imperial
        Ounce,
        Pound,
        US_Tonne,
        UK_Tonne,

    }
    struct Angle {
        public double Degree;
        public double Radian;
        public double Gradian;
        public Angle(UnitType type,double value) {
            switch(type) {

                case UnitType.Degree:
                    Degree = value;
                    Radian = value*Math.PI/180;
                    Gradian = value*10/9;
                break;

                case UnitType.Radian:
                    Degree = value*180/Math.PI;
                    Radian = value;
                    Gradian = value*200/Math.PI;
                break;

                case UnitType.Gradian:
                    Degree = value*9/10;
                    Radian = value*Math.PI/200;
                    Gradian = value;
                break;

                default:
                    throw new ArgumentException("Angles only allow Degree, Radian, and Gradian!");
            }
        }
    }
    struct Temperature {
        public double Celsius;
        public double Kelvin;
        public double Fahrenheit;
        public Temperature(UnitType type, double value) {
            switch(type) {

                case UnitType.Celsius:
                Celsius=value;
                Kelvin=value+273.15;
                Fahrenheit=(value*9/5)+32;
                break;

                case UnitType.Kelvin:
                Celsius=value-273.15;
                Kelvin=value;
                Fahrenheit=((value-273.15)*9/5)+32;
                break;

                case UnitType.Fahrenheit:
                Celsius=(value-32)*5/9;
                Kelvin=((value-32)*5/9)+273.15;
                Fahrenheit=value;
                break;

                default:
                throw new ArgumentException("Temperature only allow Celsius, Kelvin, and Fahrenheit!");
            }
        }
    }
    struct Mass {
        // Epic
        public double Gram;
        public double Kilogram;
        public double Metric_Tonne;

        // Bullshit
        public double Ounce;
        public double Pound;
        public double US_Tonne;
        public double UK_Tonne;
        public Mass(UnitType type, double value) {
            double gram=0;
            // Convert all to gram
            switch(type) {
                // Metric perfection
                case UnitType.Gram:
                    gram=value;
                break;

                case UnitType.Kilogram:
                    gram=value * 1_000;
                break;

                case UnitType.Metric_Tonne:
                    gram=value*1_000_000;
                break;

                // Imperial bullshit
                case UnitType.Ounce:
                    gram=value/28.35;
                break;

                case UnitType.Pound:
                    gram=16*(value/28.35);
                break;

                case UnitType.US_Tonne:
                    gram=32000*(value/28.35);
                break;

                case UnitType.UK_Tonne:
                    gram=35840*(value/28.35);
                break;

                default:
                throw new ArgumentException("Non Mass unit has been given. Mass units are");
            }
        
            Gram=gram;
            Kilogram=gram/1_000;
            Metric_Tonne=gram/1_000_000;

            Ounce=gram*28.35;
            Pound=(gram*28.35)/16;
            US_Tonne=(gram*28.35)/32000;
            UK_Tonne=(gram*28.35)/35840;
        }
    }
}