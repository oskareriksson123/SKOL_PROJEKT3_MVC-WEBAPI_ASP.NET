using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DejtApplication10._0.Models
{
    public static class AnvändareInloggadVerifeierare
    {
        public static bool isInloogad { get; set; } = false;

        public static AnvändareModel användaren { get; set; } = null;

    }
}