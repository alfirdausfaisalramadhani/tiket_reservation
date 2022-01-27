using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tiket_reservation.Models;
using tiket_reservation.Helper;
using tiket_reservation.Security;

namespace tiket_reservation.Controllers
{
    [AuthorizationFilterAdmin]
    public class AdminController : Controller
    {
        static tiket_reservationEntities1 db = new tiket_reservationEntities1();
        // GET: Admin

        public ActionResult dashboard_admin()
        {
            Statistik statistik = new Statistik();
            statistik.total_user = db.detil_pesan_tiket.Count();
            statistik.user_lunas = db.detil_pesan_tiket.Where(u
            => u.total_transfer != 0).Count();
            statistik.user_belum_lunas = db.detil_pesan_tiket.Where(u
            => u.total_transfer == 0).Count();
            var checkPembeli = db.detil_pesan_tiket;
            // cek pembeli ada atau engga
            if (checkPembeli.Count() == 0)
            {
                // biarkan kosong.
            }
            else
            {
                statistik.uang_estimasi = ConvertCurrency.
                ToRupiah(db.detil_pesan_tiket.Select(u
                => u.harga_tiket).Sum());
                statistik.uang_diterima = ConvertCurrency.
                ToRupiah(db.detil_pesan_tiket.Select(u
                => u.total_transfer).Sum());
                decimal estimasi = db.detil_pesan_tiket.Select(u
                => u.harga_tiket).Sum();
                decimal uangDiterima = db.detil_pesan_tiket.Select(u
                => u.total_transfer).Sum();
                statistik.selisiPendapatan = ConvertCurrency.
                ToRupiah(estimasi - uangDiterima);
            }
            statistik.user_validasi = db.pembeli_validasi.Where(u
            => u.uang_transfer_validasi != null).Count();
            return View(statistik);
        }
        public ActionResult semua_pembeli()
        {
            var joinData = from p in db.pembeli
                           from d in db.detil_pesan_tiket
                           where p.id_pembeli == d.id_pembeli
                           from v in db.pembeli_validasi
                           where d.id_pembeli == v.id_pembeli
                           select new Gabungan
                           { tblPembeli = p, tblDetailTiket = d, tblValidasi = v };
            return View(joinData);
        }
        public ActionResult log_out()
        {
            Session.Remove("admin");
            Session.Remove("email");
            return RedirectToAction("index", "Home");
        }
        public ActionResult user_detail(int id)
        {
            Gabungan gabungan = new Gabungan();
            gabungan.tblPembeli = db.pembeli.Find(id);
            gabungan.tblDetailTiket = db.detil_pesan_tiket.Find(id);
            gabungan.tblValidasi = db.pembeli_validasi.Find(id);
            int pajak_berangkatId =
            gabungan.tblDetailTiket.bandara_berangkat;
            int pajak_tujuanId =
            gabungan.tblDetailTiket.bandara_tujuan;
            var hargaBerangkat =
            db.pajak_bandara.Find(pajak_berangkatId);
            var hargaTujuan = db.pajak_bandara.Find(pajak_tujuanId);
            gabungan.rp_bandara_berangkat = ConvertCurrency.
            ToRupiah(hargaBerangkat.pajak);
            gabungan.rp_bandara_tujuan = ConvertCurrency.
            ToRupiah(hargaTujuan.pajak);
            gabungan.rp_harga_tiket = ConvertCurrency.
            ToRupiah(gabungan.tblDetailTiket.harga_tiket);
            gabungan.rp_total_transfer = ConvertCurrency.ToRupiah(gabungan.tblDetailTiket.total_transfer);
            gabungan.nm_bandara_berangkat =
            hargaBerangkat.nm_bandara;
            gabungan.nm_bandara_tujuan = hargaTujuan.nm_bandara;
            return View(gabungan);
        }

        [HttpPost]
        public ActionResult user_detail(int id, Gabungan gabungan)
        {
            var user = db.pembeli.FirstOrDefault(u
            => u.id_pembeli == id);
            user.nm_pembeli = gabungan.tblPembeli.nm_pembeli;
            user.email_pembeli = gabungan.tblPembeli.email_pembeli;
            user.hp_pembeli = gabungan.tblPembeli.hp_pembeli;
            user.password = gabungan.tblPembeli.password;
            db.SaveChanges();
            decimal UnformatRpTotalTf = ConvertCurrency
            .ToAngka(gabungan.rp_total_transfer);
            decimal TotalTf =
            gabungan.tblDetailTiket.total_transfer;
            if (UnformatRpTotalTf == TotalTf)
            {
                var userDetail =
                db.detil_pesan_tiket.FirstOrDefault(u
                => u.id_pembeli == id);
                userDetail.total_transfer = 0;
                // it's means, number 1 has been paid, so 0 is otherwise
                userDetail.status = 0;
            }
            else
            {
                var userDetail =
            db.detil_pesan_tiket.FirstOrDefault(u
            => u.id_pembeli == id);
                userDetail.total_transfer = UnformatRpTotalTf;
                // it's means, number 1 has been paid, so 0 is otherwise
                userDetail.status = 1;
            }
            db.SaveChanges();
            return RedirectToAction("semua_pembeli", "Admin");
        }

        [HttpPost]
        public ActionResult delete_pembeli(int id, Gabungan gabungan)
        {
            var getUser = db.pembeli.SingleOrDefault(u => u.id_pembeli == id);
            if (getUser != null)
            {
                db.pembeli.Remove(getUser);
                db.SaveChanges();
            }
            return RedirectToAction("semua_pembeli", "Admin");
        }
        public ActionResult pembeli_lunas()
        {
            var joinData = from p in db.pembeli
                           from d in db.detil_pesan_tiket
                           where p.id_pembeli ==
                           d.id_pembeli
                           from v in db.pembeli_validasi
                           where d.id_pembeli == v.id_pembeli
                           where d.total_transfer != 0
                           select new Gabungan
                           { tblPembeli = p, tblDetailTiket = d, tblValidasi = v };
            return View(joinData);
        }
        public ActionResult pembeli_belum_lunas()
        {
            var joinData = from p in db.pembeli
                           from d in db.detil_pesan_tiket
                           where p.id_pembeli ==
                           d.id_pembeli
                           from v in db.pembeli_validasi
                           where d.id_pembeli == v.id_pembeli
                           where d.total_transfer == 0
                           select new Gabungan
                           { tblPembeli = p, tblDetailTiket = d, tblValidasi = v };
            return View(joinData);
        }
        public ActionResult kotak_validasi()
        {
            var joinData = from p in db.pembeli
                           from d in db.detil_pesan_tiket
                           where p.id_pembeli == d.id_pembeli
                           from v in db.pembeli_validasi
                           where d.id_pembeli == v.id_pembeli
                           where v.pilihan_bank != null
                           select new Gabungan
                           { tblPembeli = p, tblDetailTiket = d, tblValidasi = v };
            return View(joinData);
        }

    }
}
