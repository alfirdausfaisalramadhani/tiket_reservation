using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
namespace tiket_reservation.Helper
{
    public static class ConvertCurrency
    {
        public static string ToRupiah(this decimal angka)
        {
            return
            String.Format(CultureInfo.CreateSpecificCulture("id-id"), "Rp. {0:N} ", angka);
        }
        public static int ToAngka(this string rupiah)
        {
            return int.Parse(Regex.Replace(rupiah, @",.*|\D", ""));
        }
    }
}


