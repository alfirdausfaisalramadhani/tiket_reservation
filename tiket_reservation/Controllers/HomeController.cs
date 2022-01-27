using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tiket_reservation.Helper;
using tiket_reservation.Models;


namespace tiket_reservation.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        static tiket_reservationEntities1 db = new tiket_reservationEntities1();
        public void RefreshAllTable()
        {
            foreach (var entity in db.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }
        public ActionResult Login_user()
        {
            if (Session["user"] != null)
            {
                return RedirectToAction("dashboard", "User");
            }
            return View();
        }

        public ActionResult Login_admin()
        {
            if (Session["admin"] != null)
            {
                return RedirectToAction("dashboard_admin", "Admin");
            }
            return View();
        }

        public ActionResult daftar()
        {
            return View();
        }

        [HttpGet]
        public JsonResult HargaBandara(int id)
        {
            var dataPajak = db.pajak_bandara.SingleOrDefault(u =>
            u.id_bandara == id);
            string harga =
            ConvertCurrency.ToRupiah(dataPajak.pajak);
            return Json(new { harga = harga },
            JsonRequestBehavior.AllowGet);
        }
        public static IEnumerable<SelectListItem> getBandara()
        {
            IEnumerable<SelectListItem> items =
            db.pajak_bandara.ToList().Select(c => new SelectListItem
            {
                Value = c.id_bandara.ToString(),
                Text = c.nm_bandara
            });
            return items;
        }

        [HttpPost]
        public ActionResult daftar(Gabungan gabungan)
        {
            if (gabungan.tblPembeli.password != gabungan.password_conf)
            {
                ViewBag.passTidakSama = "has-error";
                ViewBag.errorMessage = "Password Konfirmasi Tidak Sama.";
                return View();
            }
            string hashPass =
            PBKDF2Encription.HashPassword
            (gabungan.tblPembeli.password);
            // table Pembeli
            var dbPembeli = new pembeli
            {
                nm_pembeli = gabungan.tblPembeli.nm_pembeli,
                email_pembeli = gabungan.tblPembeli.email_pembeli,
                password = hashPass,
                hp_pembeli = gabungan.tblPembeli.hp_pembeli,
                gd_pembeli = gabungan.tblPembeli.gd_pembeli
            };
            db.pembeli.Add(dbPembeli);
            db.SaveChanges();
            //table tgl Order
            tgl_pesan tgl_table = new tgl_pesan();
            tgl_table.tgl_order =
            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var dbTglPesan = new tgl_pesan
            {
                nm_pembeli = gabungan.tblPembeli.nm_pembeli,
                tgl_order = tgl_table.tgl_order
            };
            db.tgl_pesan.Add(dbTglPesan);
            db.SaveChanges();
            // table Detail Pembeli
            var dbPembeliDetail = new detil_pesan_tiket
            {
                nm_pembeli = gabungan.tblPembeli.nm_pembeli,
                harga_tiket =
            ConvertCurrency.ToAngka(gabungan.rp_harga_tiket),
                total_transfer = gabungan.tblDetailTiket.total_transfer,
                pilihan_bank = gabungan.tblDetailTiket.pilihan_bank,
                bandara_berangkat =
            gabungan.tblDetailTiket.bandara_berangkat,
                bandara_tujuan = gabungan.tblDetailTiket.bandara_tujuan,
                status = gabungan.tblDetailTiket.status
            };
            db.detil_pesan_tiket.Add(dbPembeliDetail);
            db.SaveChanges();
            // table Validasi Pembeli
            var dbValidasi = new pembeli_validasi
            {
                nm_pembeli = gabungan.tblPembeli.nm_pembeli,
                email_pembeli = gabungan.tblPembeli.email_pembeli,
                hp_pembeli = gabungan.tblPembeli.hp_pembeli,
                uang_transfer_validasi = null,
                pilihan_bank = null
            };
            db.pembeli_validasi.Add(dbValidasi);
            db.SaveChanges();
            return RedirectToAction("login_user", "Home");
        }

        [HttpPost]
        public ActionResult Login_user(pembeli postPembeli)
        {
            pembeli pb = db.pembeli.SingleOrDefault(u => u.email_pembeli ==
            postPembeli.email_pembeli);
            if (pb == null)
            {
                ViewBag.htmlError = "has-error";
                ViewBag.errorMessage = "Username atau password anda salah.";
                return View();
            }
            bool comparePassword =
            PBKDF2Encription.VerifyHashedPassword(pb.password, postPembeli.password);
            if (postPembeli.email_pembeli == pb.email_pembeli && comparePassword)
            {
                Session["user"] = pb.nm_pembeli;
                Session["email"] = pb.email_pembeli;
                Session["id"] = pb.id_pembeli;
                return RedirectToAction("dashboard", "User");
            }
            else
            {
                ViewBag.htmlError = "has-error";
                ViewBag.errorMessage = "Username atau password anda salah";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login_admin(admin postAdmin)
        {
            RefreshAllTable();
            admin ad = db.admin.SingleOrDefault(u
            => u.email_admin == postAdmin.email_admin);
            if (ad == null)
            {
                ViewBag.htmlError = "has-error";
                ViewBag.errorMessage =
                "Username atau password anda salah.";
                return View();
            }
            bool comparePassword =
            PBKDF2Encription.VerifyHashedPassword
            (ad.pass_admin, postAdmin.pass_admin);
            if (postAdmin.email_admin == ad.email_admin
                && comparePassword)
            {
                Session["admin"] = ad.nm_admin;
                Session["email"] = ad.email_admin;
                return RedirectToAction("dashboard_admin", "Admin");
            }
            else
            {
                ViewBag.htmlError = "has-error";
                ViewBag.errorMessage =
                "Username atau password anda salah";
            }
            return View();
        }
    }
}