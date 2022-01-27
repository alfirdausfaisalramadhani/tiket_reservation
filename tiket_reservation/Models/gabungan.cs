using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace tiket_reservation.Models
{
    public class Gabungan
    {
        public pembeli tblPembeli { get; set; }
        public pembeli_validasi tblValidasi { get; set; }
        public detil_pesan_tiket tblDetailTiket { get; set; }
        public tgl_pesan tblTglPesan { get; set; }
        public string password_conf { get; set; }
        public string rp_bandara_berangkat { get; set; }
        public string rp_bandara_tujuan { get; set; }
        public string nm_bandara_berangkat { get; set; }
        public string nm_bandara_tujuan { get; set; }
        public string rp_harga_tiket { get; set; }
        public string rp_total_transfer { get; set; }
    }
}

