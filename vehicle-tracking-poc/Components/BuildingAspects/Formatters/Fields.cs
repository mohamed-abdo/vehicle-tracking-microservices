using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Formatters
{
    public static class Fields
    {
        public static string Vehicle(string value) => $"vehicle:{value}";
        public static string Customer(string value) => $"customer:{value}";
        public static string TimeStamp(string value) => $"timestamp:{value}";
        public static string Status(string value) => $"status:{value}";
        public static string Message(string value) => $"message:{value}";
        public static string Brand(string value) => $"brand:{value}";
        public static string Model(string value) => $"model:{value}";
    }
}
