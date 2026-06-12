using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConversion.Models
{
    public class ConvertRequest
    {
        public string InputPath { get; set; }
        public string TargetExtension { get; set; }
    }
}
