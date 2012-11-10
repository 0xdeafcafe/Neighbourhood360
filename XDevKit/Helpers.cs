using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDevKit
{
    public class Helpers
    {
        private const ulong KB_IN_BYTES = 1024;
        private const ulong MB_IN_BYTES = 1048576;
        private const ulong GB_IN_BYTES = 1073741824;
        private const ulong TB_IN_BYTES = 1099511627776;

        public static string GetFriendlySizeName(ulong bytesNumber)
        {
            string friendlySizeName = "";

            if (bytesNumber > TB_IN_BYTES)
            {
                // It's in TB
                friendlySizeName = (Convert.ToSingle(bytesNumber) / 1024 / 1024 / 1024 / 1024).ToString("N2") + " TB";
            }
            else if (bytesNumber > GB_IN_BYTES)
            {
                // It's in GB
                friendlySizeName = (Convert.ToSingle(bytesNumber) / 1024 / 1024 / 1024).ToString("N2") + " GB";
            }
            else if (bytesNumber > MB_IN_BYTES)
            {
                // It's in MB
                friendlySizeName = (Convert.ToSingle(bytesNumber) / 1024 / 1024).ToString("N2") + " MB";
            }
            else if (bytesNumber > KB_IN_BYTES)
            {
                // It's in KB... Fucking hell.
                friendlySizeName = (Convert.ToSingle(bytesNumber) / 1024).ToString("N2") + " KB";
            }
            else
                // The number is in bytes, ugh.
                friendlySizeName = bytesNumber.ToString() + " bytes";

            return friendlySizeName;
        }

        public static string GetFriendlyNameFromPath(string path)
        {
            switch (path)
            {
                case "DEVKIT": return "Game Development Volume";
                case "E": return "Game Development Volume";
                case "FLASH": return "Volume";
                case "HDD": return "Retail Hard Drive Emulation";
                case "Y": return "Xbox 360 Dashboard Volume";
                case "Z": return "Devkit Drive";
                case "GAME": return "Active Title Media";

                default: return path;
            }
        }
    }
}
