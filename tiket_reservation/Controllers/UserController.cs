using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tiket_reservation.Security;
using tiket_reservation.Helper;
using tiket_reservation.Models;


namespace tiket_reservation.Controllers
{
    [AuthorizationFilterUser]
    public class UserController : Controller
    {
        tiket_reservationEntities1 db = new tiket_reservationEntities1();

        // GET: User
        public ActionResult dashboard()
        {
            return View();
        }
        public ActionResult log_out()
        {
            Session.Remove("user");
            Session.Remove("email");
            Session.Remove("id");
            return RedirectToAction("index", "Home");
        }

        public ActionResult data_pembeli()
        {
            Gabungan gabungan = new Gabungan();
            int idUser = (int)Session["id"];
            //Gabungan. student = db.Students.Find(id); 
            gabungan.tblPembeli = db.pembeli.Find(idUser);
            gabungan.tblDetailTiket =
            db.detil_pesan_tiket.Find(idUser);
            int pajak_berangkatId = 
            gabungan.tblDetailTiket.bandara_berangkat;
            int pajak_tujuanId =
            gabungan.tblDetailTiket.bandara_tujuan;
            var hargaBerangkat =
            db.pajak_bandara.Find(pajak_berangkatId);
            var hargaTujuan = db.pajak_bandara.Find(pajak_tujuanId);
            gabungan.rp_bandara_berangkat =
            ConvertCurrency.ToRupiah(hargaBerangkat.pajak);
            gabungan.rp_bandara_tujuan =
            ConvertCurrency.ToRupiah(hargaTujuan.pajak);
            gabungan.rp_harga_tiket =
            ConvertCurrency.ToRupiah(gabungan.tblDetailTiket.harga_tiket);
            gabungan.nm_bandara_berangkat =
            hargaBerangkat.nm_bandara;
            gabungan.nm_bandara_tujuan = hargaTujuan.nm_bandara;
            return View(gabungan);
        }


        [HttpPost]
        public ActionResult data_pembeli(int id, Gabungan gabungan)
        {
            var user = db.pembeli.FirstOrDefault(u => u.id_pembeli == id);
            user.nm_pembeli = gabungan.tblPembeli.nm_pembeli;
            user.email_pembeli = gabungan.tblPembeli.email_pembeli;
            user.hp_pembeli = gabungan.tblPembeli.hp_pembeli;
            db.SaveChanges();
            return RedirectToAction("data_pembeli", "User");
        }

        public ActionResult ketentuan()
        {
            return View();
        }

        public ActionResult validasi_tiket()
        {
            Gabungan gabungan = new Gabungan();
            int idUser = (int)Session["id"];
            //Gabungan. student = db.Students.Find(id); 
            gabungan.tblPembeli = db.pembeli.Find(idUser);
            gabungan.tblDetailTiket =
            db.detil_pesan_tiket.Find(idUser);
            gabungan.tblValidasi = db.pembeli_validasi.Find(idUser);
            gabungan.rp_harga_tiket = ConvertCurrency.ToRupiah
            (gabungan.tblDetailTiket.harga_tiket);
            int pajak_berangkatId =
            gabungan.tblDetailTiket.bandara_berangkat;
            int pajak_tujuanId =
            gabungan.tblDetailTiket.bandara_tujuan;
            var hargaBerangkat =
            db.pajak_bandara.Find(pajak_berangkatId);
            var hargaTujuan = db.pajak_bandara.Find(pajak_tujuanId);
            gabungan.rp_bandara_berangkat =
            ConvertCurrency.ToRupiah(hargaBerangkat.pajak);
            gabungan.rp_bandara_tujuan =
            ConvertCurrency.ToRupiah(hargaTujuan.pajak);
            gabungan.nm_bandara_berangkat = hargaBerangkat.nm_bandara;
            gabungan.nm_bandara_tujuan = hargaBerangkat.nm_bandara;
            return View(gabungan);
        }

        [HttpPost]
        public ActionResult validasi_tiket(int id, Gabungan gabungan)
        {
            var userValidasi = db.pembeli_validasi.FirstOrDefault(u
            => u.id_pembeli == id);
            userValidasi.pilihan_bank =
            gabungan.tblDetailTiket.pilihan_bank;
            userValidasi.uang_transfer_validasi =
            gabungan.tblDetailTiket.harga_tiket;
            db.SaveChanges();
            return RedirectToAction("validasi_tiket", "User");
        }
    }
}
