﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataLib.Models
{
    public static class ColorsManager
    {
        public static RGB[] trendColors = {
            new RGB(255, 0, 0), 
            new RGB(0, 255, 0), 
            new RGB(0, 0, 255), 
            new RGB(255, 165, 0),
            new RGB(128, 0, 128), 
            new RGB(255, 255, 0), 
            new RGB(0, 255, 255), 
            new RGB(255, 0, 255),
            new RGB(0, 255, 0), 
            new RGB(0, 128, 128), 
            new RGB(255, 192, 203), 
            new RGB(165, 42, 42),
            new RGB(128, 0, 0), 
            new RGB(0, 0, 128), 
            new RGB(128, 128, 0), 
            new RGB(255, 140, 0),
            new RGB(0, 139, 139), 
            new RGB(139, 0, 139), 
            new RGB(0, 100, 0), 
            new RGB(0, 0, 128),
            new RGB(139, 0, 0), 
            new RGB(255, 255, 0), new RGB(169, 169, 169)
        };
    }

    public struct RGB
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public RGB(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return $"RGB({Red}, {Green}, {Blue})";
        }
    }
}
