using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace tiket_reservation.Models
{
    public class Statistik
    {
        public int total_user { get; set; }
        public int user_belum_lunas { get; set; }
        public int user_lunas { get; set; }
        public int user_validasi { get; set; }
        public string uang_diterima { get; set; }
        public string uang_estimasi { get; set; }
        public string selisiPendapatan { get; set; }
    }
}
