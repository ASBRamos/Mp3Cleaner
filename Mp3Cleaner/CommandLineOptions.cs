using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Mp3Cleaner
{
    internal class CommandLineOptions
    {
        [Option('d', "directory", Required = false, HelpText = "Target directory containing all the folders copied from phone.")]
        public string TargetDirectory { get; set; }
    }
}
