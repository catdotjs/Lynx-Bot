using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

// I'm sure somewhere, someone knows how to make this 0.002391802138% better
// I'm glad you do but I wrote this with 4 hours of sleep so y'know leave me alone <3

namespace Lynx_Bot.Processing.UnitConversion {
    // Just makes stuff more tidy
    using UnitType = ConversionUnit.Type;
    static class ConversionUnit {
        public enum Type {
            // ---  Angle[0 - 2]  ---
            Degree,
            Radian,
            Gradian,

            // ---  Temperature[3 - 5]  ---
            Celsius,
            Kelvin,
            Fahrenheit,

            // ---  Mass[6 - 12,21]  ---
            // Metric
            Gram,
            Kilogram,
            Metric_Tonne,
            Dalton=21,

            // Imperial
            Ounce=9,
            Pound,
            US_Short_Ton,
            UK_Long_Ton,

            // ---  Length[13 - 20]  ---
            // Metric mwah
            Millimetre,
            Centimetre,
            Metre,
            Kilometre,

            //Imperial yucky stinky
            Inch,
            Feet,
            Yard,
            Mile,
        }
        public static Dictionary<UnitType, string> Symbols = new Dictionary<UnitType, string>(){
            // Angles - 3
            { Type.Degree,"°"},
            { Type.Radian,"rad"},
            { Type.Gradian,"gon"},

            // Temperature - 3
            { Type.Celsius,"°C"},
            { Type.Kelvin,"°K"},
            { Type.Fahrenheit,"°F"},

            // Mass - 7
            { Type.Gram,"g"},
            { Type.Kilogram,"Kg"},
            { Type.Metric_Tonne,"ton(Metric)"},
            { Type.Dalton,"u"},

            { Type.Ounce,"oz"},
            { Type.Pound,"lb"},
            { Type.US_Short_Ton,"ton(US/Short)"},
            { Type.UK_Long_Ton,"ton(UK/Long)"},

            // Length - 8
            { Type.Millimetre,"mm"},
            { Type.Centimetre,"cm"},
            { Type.Metre,"m"},
            { Type.Kilometre,"km"},

            { Type.Inch,"in"},
            { Type.Feet,"ft"},
            { Type.Yard,"yd"},
            { Type.Mile,"mile"},
        };
    }
    struct Angle {
        public List<(UnitType, double)> Data;
        public Angle(UnitType type, double value) {
            switch(type) {
                case UnitType.Degree:
                    Data=new List<(UnitType, double)>(){
                        (UnitType.Degree,value),
                        (UnitType.Radian,value*Math.PI/180),
                        (UnitType.Gradian,value*10/9),
                    };
                break;

                case UnitType.Radian:
                    Data=new List<(UnitType, double)>(){
                        (UnitType.Degree,value*180/Math.PI),
                        (UnitType.Radian,value),
                        (UnitType.Gradian,value*200/Math.PI),
                    };
                break;

                case UnitType.Gradian:
                    Data=new List<(UnitType, double)>(){
                        (UnitType.Degree,value*9/10),
                        (UnitType.Radian,value*Math.PI/200),
                        (UnitType.Gradian,value),
                    };
                break;

                default:
                throw new ArgumentException("Angles only allow Degree, Radian, and Gradian!");
            }
        }
    }
    struct Temperature {
        public List<(UnitType, double)> Data;
        public Temperature(UnitType type, double value) {
            switch(type) {
                case UnitType.Celsius:
                    Data=new List<(UnitType, double)>(){
                        (UnitType.Celsius,value),
                        (UnitType.Kelvin,value+273.15),
                        (UnitType.Fahrenheit,(value*9/5)+32),
                    };
                break;

                case UnitType.Kelvin:
                    Data=new List<(UnitType, double)>(){
                        (UnitType.Celsius,value-273.15),
                        (UnitType.Kelvin,value),
                        (UnitType.Fahrenheit,((value-273.15)*9/5)+32),
                    };
                break;

                case UnitType.Fahrenheit:
                    Data=new List<(UnitType, double)>(){
                        (UnitType.Celsius,(value-32)*5/9),
                        (UnitType.Kelvin,(value-32)*5/9+273.15),
                        (UnitType.Fahrenheit,value),
                    };
                break;

                default:
                throw new ArgumentException("Temperature only allow Celsius, Kelvin, and Fahrenheit!");
            }
        }
    }
    struct Mass {
        public List<(UnitType, double)> Data;
        public Mass(UnitType type, double value) {
            double gram = 0;
            double U = 1.66*Math.Pow(10, -24);
            // Convert all to gram
            switch(type) {
                // Metric perfection
                case UnitType.Gram:
                gram=value;
                break;

                case UnitType.Kilogram:
                gram=value*1_000;
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

                case UnitType.US_Short_Ton:
                gram=32000*(value/28.35);
                break;

                case UnitType.UK_Long_Ton:
                gram=35840*(value/28.35);
                break;

                case UnitType.Dalton:
                gram=value*U;
                break;

                default:
                throw new ArgumentException("Non Mass unit has been given. Mass units are");
            }

            Data = new List<(UnitType, double)>(){
                (UnitType.Gram,gram),
                (UnitType.Kilogram,gram/1_000),
                (UnitType.Metric_Tonne,gram/1_000_000),

                (UnitType.Ounce,gram/28.35),
                (UnitType.Pound,(gram/28.35)/16),
                (UnitType.US_Short_Ton,(gram/28.35)/32000),
                (UnitType.UK_Long_Ton,(gram/28.35)/35840),

                (UnitType.Dalton,gram/U),
            };
        }
    }
    struct Length {
        // Epic
        public List<(UnitType, double)> Data;
        public Length(UnitType type, double value) {
            double mm = 0;
            // Convert all to gram
            switch(type) {
                // Metric perfection
                case UnitType.Millimetre:
                    mm=value;
                break;

                case UnitType.Centimetre:
                    mm=value*10;
                break;

                case UnitType.Metre:
                    mm=value*1_000;
                break;

                case UnitType.Kilometre:
                    mm=value*1_000_000;
                break;

                // Imperial bullshit
                case UnitType.Inch:
                    mm=value*25.4;
                break;

                case UnitType.Feet:
                    mm=value*304.8;
                break;

                case UnitType.Yard:
                    mm=value*914.4;
                break;

                case UnitType.Mile:
                    mm=value*1_609_344;
                break;

                default:
                throw new ArgumentException("Non Length unit has been given. Length units are");
            }

            Data=new List<(UnitType, double)>(){
                (UnitType.Millimetre,mm),
                (UnitType.Centimetre,mm/10),
                (UnitType.Metre,mm/1_000),
                (UnitType.Kilometre,mm/1_000_000),
                (UnitType.Inch,mm/25.4),
                (UnitType.Feet,mm/304.8),
                (UnitType.Yard,mm/914.4),
                (UnitType.Mile,mm/1_609_344),
            };
        }
    }
}