using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jewellery_project.Models;
using CaptchaMvc.HtmlHelpers;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;


namespace jewellery_project.Controllers
{
    public class User_newController : Controller
    {
        jewellary_dbEntities1 db = new jewellary_dbEntities1();
        // GET: /User_new/
        //-------------------------------------search---------------------
        [HttpPost]
        public ActionResult search(User ss)
        {
            return View();
        }
        //--------------------------------------------home------------------
        public ActionResult Home()
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);
        }
        //------------------------------------------reg----------------------
        public ActionResult Reg()
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            stateBind(df);
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);
        }
        private void stateBind(User ss)
        {
            ss.states = new List<SelectListItem>();
            List<tbl_state> lc = db.tbl_state.ToList();
            foreach (var item in lc)
            {
                SelectListItem l = new SelectListItem();
                l.Text = item.name;
                l.Value = item.state_id.ToString();
                ss.states.Add(l);
            }
        }
        private static void cityBind(List<tbl_city> lc, List<SelectListItem> ls)
        {
            foreach (var item in lc)
            {
                SelectListItem l = new SelectListItem();
                l.Text = item.name;
                l.Value = item.city_id.ToString();
                ls.Add(l);
            }
        }
        [HttpPost]
        public ActionResult getcity(int stid)
        {
            List<tbl_city> lc = (from j in db.tbl_city where j.state_id == stid select j).ToList();
            List<SelectListItem> ls = new List<SelectListItem>();
            cityBind(lc, ls);
            return Json(ls, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Reg(User ss)
        {
            var email=ss.email;
            var mobile = ss.mobile;          
            int res;
            if ((int.TryParse(mobile, out res)) && (mobile.Length == 10))
            {
                if (email.Contains("@") && email.Contains(".com"))
                {
                    if (ss.password == ss.confirm_password)
                    {
                        tbl_user dt = new tbl_user();
                        dt.fname = ss.fname;
                        dt.mname = ss.mname;
                        dt.lname = ss.lname;
                        dt.gender = ss.gender;
                        dt.state_id = ss.state_id;
                        dt.city_id = ss.city_id;
                        dt.address = ss.address;
                        dt.mobile = ss.mobile;
                        dt.email = ss.email;
                        dt.password = ss.password;
                        dt.DOC = System.DateTime.Now;
                        dt.DOM = System.DateTime.Now;
                        db.tbl_user.Add(dt);
                        db.SaveChanges();
                        return RedirectToAction("Home");
                    }
                    else
                    {
                        ViewBag.conpass = "Password Doesn't Match";
                        User df = new User();
                        df.catlst = DBContext.getcat();
                        df.collst = DBContext.getcol();
                        stateBind(df);
                        df.city = new List<SelectListItem>();
                        List<tbl_city> lc = (from j in db.tbl_city where j.state_id == ss.state_id select j).ToList();
                        cityBind(lc, df.city);
                        int count_id = Convert.ToInt16(Session["u_id"]);
                        var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                        Session["count"] = count_dt.Count();
                        return View("Reg", df);
                    }
                }
                else
                {
                    ViewBag.email = "Email Is Not Valid";
                    User df = new User();
                    df.catlst = DBContext.getcat();
                    df.collst = DBContext.getcol();
                    stateBind(df);
                    df.city = new List<SelectListItem>();
                    List<tbl_city> lc = (from j in db.tbl_city where j.state_id == ss.state_id select j).ToList();
                    cityBind(lc, df.city);
                    int count_id = Convert.ToInt16(Session["u_id"]);
                    var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                    Session["count"] = count_dt.Count();
                    return View("Reg", df);
                }
            }
            else
            {
                ViewBag.mobile = "Mobile Number Is Not Valid";
                User df = new User();
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                stateBind(df);
                df.city = new List<SelectListItem>();
                List<tbl_city> lc = (from j in db.tbl_city where j.state_id == ss.state_id select j).ToList();
                cityBind(lc, df.city);
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                return View("Reg", df);
            }           
        }
        //--------------------------------------------login-------------------------
        public ActionResult Login()
        {
            User df = new User();
            Session["forget"] = 0;
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            return View(df);
        }
        [HttpPost]
        public ActionResult Login(User ss)
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
           var email=ss.email;
           if (Convert.ToInt16(Session["forget"]) == 0)
           {
               if (email.Contains("@") && email.Contains(".com"))
               {
                   var obj = db.tbl_user.Where(a => a.email.Equals(ss.email) && a.password.Equals(ss.password)).FirstOrDefault();
                   if (obj != null)
                   {
                       if (obj.isActive == 1)
                       {
                           if (this.IsCaptchaValid("Captcha is not valid"))
                           {
                               Session["details"] = obj;
                               Session["fname"] = obj.fname;
                               Session["lname"] = obj.lname;
                               Session["u_id"] = obj.user_id;
                               Session["email"] = obj.email;
                               var obj1 = (from j in db.tbl_user
                                           join
                                               k in db.tbl_city on obj.city_id equals k.city_id
                                           join
                                                   l in db.tbl_state on obj.state_id equals l.state_id
                                           select new
                                           {
                                               city = k.name,
                                               state = l.name
                                           }).FirstOrDefault();
                               Session["address"] = obj.address + "," + obj1.city + "," + obj1.state;
                               if (Session["prev"] != null)
                               { 
                                   string prevurl=Session["prev"].ToString();
                                   return Redirect(prevurl);
                               }
                               else
                               {
                                   return RedirectToAction("Home");
                               }                             
                           }
                           else
                           {
                               ViewBag.ErrMessage = "Captcha is not valid.";
                               return View(df);
                           }
                       }
                       else
                       {
                           Response.Write("<script>alert('Unauthorized access')</script>");
                           return View(df);
                       }
                   }
                   else
                   {
                       Response.Write("<script>alert('Email Or Password Not Valid')</script>");
                       return View(df);
                   }
               }
               else
               {
                   ViewBag.emaillog = "Email Is Not Valid";
                   return View(df);
               }
           }
           else
           {
               var obj = db.tbl_user.Where(a => a.email.Equals(ss.email)).FirstOrDefault();
               if (obj != null)
               {
                   if (obj.isActive == 1)
                   {
                       MailMessage mail = new MailMessage();
                       SmtpClient smtpserver = new SmtpClient("smtp.gmail.com");
                       string st = obj.password;
                       mail.From = new MailAddress("deekshatiwari965@gmail.com");
                       mail.To.Add(obj.email);
                       mail.Subject = "Test Email";
                       mail.Body = "Password is:" + "\n" + st + "\t" + "Thank u";
                       smtpserver.Port = 587;
                       smtpserver.Credentials = new System.Net.NetworkCredential("deekshatiwari965@gmail.com", "9807939808");
                       smtpserver.EnableSsl = true;
                       smtpserver.Send(mail);
                       string script = "<script>alert('Send password on your email..!!')</script>";
                       Response.Write(script);
                       Session["forget"] = 0;
                       return View("Login", df);
                   }
                   else
                   {
                       Response.Write("<script>alert('Unauthorized access')</script>");
                       return View(df);
                   }
               }
               else
               {
                   Response.Write("<script>alert('Email is Not Valid')</script>");
                   return View(df);
               }
           }
        }
        public ActionResult forgetpassword()
        {
            Session["forget"] = 1;
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            return View("Login", df);
        }
        //---------------------------------------------------cat bind in menu-------------------------
        public ActionResult viewcatwiseprod(int id)
        {
            Session["cat_id"] = id;
            return RedirectToAction("Product");
        }
        public ActionResult viewcolwiseprod(int id)
        {
            Session["col_id"] = id;
            return RedirectToAction("Product");
        }
        //-------------------------------------product-----------------------------------
        public ActionResult Product()
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            if (Session["cat_id"] != null)
            {
                int catid = Convert.ToInt16(Session["cat_id"]);
                df.products = DBContext.getprocat(catid);
                Session.Remove("cat_id");
            }
            if (Session["col_id"] != null)
            {
                int colid = Convert.ToInt16(Session["col_id"]);
                df.products = DBContext.getprocol(colid);
                Session.Remove("col_id");
            }
            return View("Product",df);

        }
        //--------------------------------------------------product details
        public ActionResult viewproductdetails(int id)
        {
            Session["p_id"] = id;
            return RedirectToAction("Productdetails");
        }
        [HttpGet]
        public ActionResult Productdetails()
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            if (Session["p_id"] != null)
            {
                int pid = Convert.ToInt16(Session["p_id"]);
                df.productdetail = DBContext.getprodetails(pid);
            }
            ViewBag.returnUrl = Request.Url.AbsoluteUri;
            return View(df);
        }
        //-------------------------------------------------------------custome_made
        public ActionResult Custom_made()
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);
        }
        [HttpPost]
        public ActionResult Custom_made(User ss, HttpPostedFileBase ffl)
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            int res;           
            if (Session["u_id"] != null)
            {
                if (ss.email.Contains("@") && ss.email.Contains(".com"))
                {
                    if ((int.TryParse(ss.mobile, out res)) && (ss.mobile.Length == 10))
                    {
                        string img = "";
                        if (ffl != null)
                        {
                            ffl.SaveAs(Server.MapPath("~/Photos/") + ffl.FileName);
                            img = "~/Photos/" + ffl.FileName;
                        }
                        tbl_custom dt = new tbl_custom();
                        custom_temp dt1 = new custom_temp();
                        dt.name = ss.name;
                        dt.email = ss.email;
                        dt.mobile = ss.mobile;
                        dt.subject = ss.subject;
                        dt.image = img;
                        dt.message = ss.message;
                        dt.DOC = System.DateTime.Now;
                        db.tbl_custom.Add(dt);
                        dt1.custom_id = (from j in db.tbl_custom select j.custom_id).Max() + 1;
                        dt1.date = System.DateTime.Now;
                        db.custom_temp.Add(dt1);
                        db.SaveChanges();

                        Response.Write("<script>alert('Your message has been successfully send')</script>");
                        ModelState.Clear();

                        return View(df);
                    }
                    else
                    {
                        ViewBag.mobile = "Mobile Is Not Valid";
                        return View(df);
                    }
                  }

                else
                {
                    ViewBag.email = "Email Is Not Valid";
                    return View(df);
                }
            }
            else
            {
                Response.Write("<script>alert('Please Logged in Yourself')</script>");
                return View(df);

            }


        }
        //---------------------------------------------------------cart---------------
        public ActionResult Cart()
        {
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            binddata_cart(df);
            return View(df);
        }
        [HttpPost]
        public ActionResult Productdetails(User ss, string returnUrl)
        {
            User df = new User();
            int pid = Convert.ToInt16(Session["p_id"]);
            string nm = (from j in db.tbl_product where j.p_id == pid select j.name).FirstOrDefault();
            int stock = Convert.ToInt32((from j in db.tbl_stock where j.product_nm == nm select j.stock).FirstOrDefault());
            if (Session["u_id"] != null)
            {
                if (stock >= ss.quantity)
                {
                    int id = Convert.ToInt16(Session["p_id"]);
                    var dt1 = (from j in db.tbl_product where j.p_id == id select j).FirstOrDefault();
                    double ans = 0;
                    tbl_cart dt = new tbl_cart();
                    dt.c_id = Convert.ToInt16(Session["u_id"]);
                    dt.p_id = id;
                    dt.quantity = ss.quantity;
                    ans = dt.quantity * Convert.ToDouble(dt1.price);
                    dt.total = Convert.ToDecimal(ans);

                    db.tbl_cart.Add(dt);
                    db.SaveChanges();
                    df.catlst = DBContext.getcat();
                    df.collst = DBContext.getcol();
                    df.productdetail = DBContext.getprodetails(pid);
                    int count_id = Convert.ToInt16(Session["u_id"]);
                    var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                    Session["count"] = count_dt.Count();
                  
                    return View("Productdetails", df);
                }
                else
                {
                    Response.Write("<script>alert('Out of Stock')</script>");
                    df.catlst = DBContext.getcat();
                    df.collst = DBContext.getcol();
                    df.productdetail = DBContext.getprodetails(pid);
                    int count_id = Convert.ToInt16(Session["u_id"]);
                    var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                    Session["count"] = count_dt.Count();
                  
                    return View("Productdetails", df);
                }

            }
            else
            {
                Response.Write("<script>alert('Please Login Yourself')</script>");
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                Session["prev"] = returnUrl;
                return View("Login", df);

            }


        }
        public ActionResult addtocart(int id, string returnUrl)
        {
            User df = new User();
            int catid = Convert.ToInt16(Session["cat_id"]);
            string nm = (from j in db.tbl_product where j.p_id == id select j.name).FirstOrDefault();
            int stock = Convert.ToInt32((from j in db.tbl_stock where j.product_nm == nm select j.stock).FirstOrDefault());

            if (Session["u_id"] != null)
            {
                if (stock != 0)
                {
                    var dt1 = (from j in db.tbl_product where j.p_id == id select j).FirstOrDefault();
                    double ans = 0;
                    tbl_cart dt = new tbl_cart();
                    dt.c_id = Convert.ToInt16(Session["u_id"]);
                    dt.p_id = id;
                    dt.quantity = 1;
                    ans = dt.quantity * Convert.ToDouble(dt1.price);
                    dt.total = Convert.ToDecimal(ans);

                    db.tbl_cart.Add(dt);
                    db.SaveChanges();
                    df.catlst = DBContext.getcat();
                    df.collst = DBContext.getcol();
                    df.products = DBContext.getprocat(catid);
                    int count_id = Convert.ToInt16(Session["u_id"]);
                    var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                    Session["count"] = count_dt.Count();
                    return View("Product", df);
                }
                else
                {
                    Response.Write("<script>alert('Out of Stock')</script>");
                    df.catlst = DBContext.getcat();
                    df.collst = DBContext.getcol();
                    df.products = DBContext.getprocat(catid);
                    int count_id = Convert.ToInt16(Session["u_id"]);
                    var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                    Session["count"] = count_dt.Count();
                    return View("Product", df);
                }
            }
            else
            {
                Response.Write("<script>alert('Please Login Yourself')</script>");
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                Session["prev"] = returnUrl;
                return RedirectToAction("Login", df);

            }
        }

        private void binddata_cart(User ss)
        {
            int id = Convert.ToInt32(Session["u_id"]);
            List<User> lc = new List<User>();
            // List<tbl_cart> ll = db.tbl_cart.ToList();
            var dt = (from j in db.tbl_cart
                      where j.c_id == id
                      join
                          k in db.tbl_product on j.p_id equals k.p_id

                      select new
                      {
                          p_id = j.p_id,
                          name = k.name,
                          price = k.price,
                          cart_id = j.cart_id,
                          c_id = j.c_id,
                          quantity = j.quantity,
                          total = j.total,
                          image = k.image,


                      }).ToList();
            foreach (var item in dt)
            {
                User cc = new User();
                cc.cart_id = item.cart_id;
                cc.c_id = item.c_id;
                cc.name = item.name;
                cc.p_id = item.p_id;
                cc.price = item.price;
                cc.quantity = item.quantity;
                cc.total = item.total;
                cc.image = item.image;

                lc.Add(cc);
            }
            ss.cartitems = lc;
        }

        public ActionResult removecartitem(int id)
        {
            User df = new User();
            var dt = (from j in db.tbl_cart where j.cart_id == id select j).FirstOrDefault();
            db.tbl_cart.Remove(dt);
            db.SaveChanges();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return RedirectToAction("Cart");
        }
        public ActionResult plusquantity(int id, int q, string p)
        {
            User df = new User();
            double ans = 0;
            var dt = (from j in db.tbl_cart where j.cart_id == id select j).FirstOrDefault();

            string nm = (from j in db.tbl_product where j.p_id == dt.p_id select j.name).FirstOrDefault();
            int stock = Convert.ToInt32((from j in db.tbl_stock where j.product_nm == nm select j.stock).FirstOrDefault());
            if (stock >= q + 1)
            {
                dt.quantity = q + 1;
                ans = dt.quantity * Convert.ToDouble(p);
                dt.total = Convert.ToDecimal(ans);
                db.SaveChanges();
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                binddata_cart(df);
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                return View("Cart", df);
            }
            else
            {
                Response.Write("<script>alert('Out of Stock')</script>");

                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                binddata_cart(df);

                Session["count"] = count_dt.Count();
                return View("Cart", df);
            }
        }
        public ActionResult minusquantity(int id, int q, string p)
        {
            User df = new User();
            double ans = 0;
            var dt = (from j in db.tbl_cart where j.cart_id == id select j).FirstOrDefault();
            if (q > 1)
            {
                dt.quantity = q - 1;
                ans = dt.quantity * Convert.ToDouble(p);
                dt.total = Convert.ToDecimal(ans);
                db.SaveChanges();
            }
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public ActionResult cartchecking()
        {
            int id = Convert.ToInt16(Session["u_id"]);
            var dt = (from j in db.tbl_cart where j.c_id == id select j).ToList();
            if (dt.Count != 0)
            {
                User df = new User();

                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();

                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                binddata_user(df);
                binddata_orderdetails(df);

                return RedirectToAction("Orderdetails", df);

            }
            else
            {
                Response.Write("<script>alert('Cart is empty')</script>");
                User df = new User();
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                binddata_cart(df);
                return RedirectToAction("Cart", df);

            }
        }

        //-------------------------------------------FEEDBACK DETAILS-----------------
        public ActionResult feedback()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);

        }
        [HttpPost]
        public ActionResult feedback(User ss)
        {
            User df = new User();
            if (ss.email.Contains("@") && ss.email.Contains(".com"))
            {
                tbl_feedback dt = new tbl_feedback();

                dt.name = ss.name;
                dt.email = ss.email;
                dt.comment = ss.comment;
                dt.DOC = System.DateTime.Now;


                db.tbl_feedback.Add(dt);
                db.SaveChanges();
            }
            else
            {
                ViewBag.email = "Email Is Not Valid";
               
            }
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);

        }
        //-------------------------------------------contact us------------------------------
        public ActionResult contact_us()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);
        }

        //----------------------------------------my account--------------------------
        public ActionResult Myaccount()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            Session["abc"] = 0;
            stateBind(df);
            bind_myaccountorder(df);
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);


        }
        [HttpPost]
        public ActionResult Myaccount(User ss)
        {
            int id = Convert.ToInt16(Session["u_id"]);
            var dt = (from j in db.tbl_user where j.user_id == id select j).FirstOrDefault();
            dt.fname = ss.fname;
            dt.mname = ss.mname;
            dt.lname = ss.lname;
            dt.gender = ss.gender;
            dt.state_id = ss.state_id;
            dt.city_id = ss.city_id;
            dt.address = ss.address;
            dt.mobile = ss.mobile;
            dt.email = ss.email;

            dt.DOM = System.DateTime.Now;

            db.SaveChanges();
            Session["fname"] = dt.fname;
            Session["lname"] = dt.lname;

            Session["email"] = dt.email;
            var obj1 = (from j in db.tbl_user
                        join
                            k in db.tbl_city on dt.city_id equals k.city_id
                        join
                                l in db.tbl_state on dt.state_id equals l.state_id

                        select new
                        {
                            city = k.name,
                            state = l.name

                        }).FirstOrDefault();
            Session["address"] = dt.address + "," + obj1.city + "," + obj1.state;
            Session["abc"] = 0;
            ss.catlst = DBContext.getcat();
            ss.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(ss);
        }
        public ActionResult logoutaccount()
        {
            Session.Clear();
            User df = new User();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return RedirectToAction("Home");
        }
        [HttpGet]
        public ActionResult editprofile(int id)
        {
            User ss = new User();
            var dt = (from j in db.tbl_user where j.user_id == id select j).FirstOrDefault();
            ss.fname = dt.fname;
            ss.mname = dt.mname;
            ss.lname = dt.lname;
            ss.gender = dt.gender;
            ss.state_id = dt.state_id;
            ss.city_id = dt.city_id;

            stateBind(ss);
            ss.city = new List<SelectListItem>();
            List<tbl_city> lc = (from j in db.tbl_city where j.state_id == ss.state_id select j).ToList();
            cityBind(lc, ss.city);

            ss.address = dt.address;
            ss.email = dt.email;
            ss.mobile = dt.mobile;
            ss.catlst = DBContext.getcat();
            ss.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            Session["abc"] = 1;
            return View("Myaccount", ss);
        }

        private void bind_myaccountorder(User ss)
        {
            List<User> lc = new List<User>();

            int id = Convert.ToInt16(Session["u_id"]);
            var dt = (from j in db.tbl_order
                      where j.user_id == id

                      join
                       k in db.tbl_orderdetails on j.order_id equals k.order_id
                      join

                      z in db.tbl_product on k.p_id equals z.p_id

                      select new
                      {
                          date = j.DOC,
                          item = z.name,
                          quantity = k.quantity,
                          total = k.total,
                          price = z.price,
                          image = z.image

                      }).ToList();
            foreach (var item in dt)
            {
                User cc = new User();
                cc.DOC = item.date;
                cc.name = item.item;
                cc.quantity = item.quantity;
                cc.total = item.total;
                cc.price = item.price;
                cc.image = item.image;
                lc.Add(cc);
            }


            ss.ols = lc;
        }


        //---------------------------------------------checkout------------------------
        public ActionResult checkout()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();

            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            Session["verification"] = 0;
            binddata_user(df);
            return View(df);


        }
        public void removecart()
        {
            int id = Convert.ToInt16(Session["u_id"]);

            var dt = (from j in db.tbl_cart where j.c_id == id select j).ToList();
            foreach (var item in dt)
            {
                db.tbl_cart.Remove(item);
            }
            db.SaveChanges();


        }
        [HttpGet]
        public ActionResult placeordercredit(int id)
        {
            User df = new User();
            Session["check"] = 1;

            MailMessage mail = new MailMessage();
            SmtpClient smtpserver = new SmtpClient("smtp.gmail.com");
            int st = random();
            Session["otp"] = st;
            mail.From = new MailAddress("deekshatiwari965@gmail.com");
            mail.To.Add(Session["email"].ToString());
            mail.Subject = "Test Email";
            mail.Body = "Confirmation Code" + "\n" + st + "\t" + "Thank u";

            smtpserver.Port = 587;
            smtpserver.Credentials = new System.Net.NetworkCredential("deekshatiwari965@gmail.com", "9807939808");
            smtpserver.EnableSsl = true;
            smtpserver.Send(mail);
            string script = "<script>alert('Send confirmation code on your email..!!')</script>";
            Response.Write(script);

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();

            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            Session["verification"] = 1;
            binddata_user(df);

            return View("checkout", df);
        }
        public static int random()
        {
            Random r = new Random();
            int i = r.Next(000000, 999999);
            return i;
        }
        public ActionResult placeorderdebit(int id)
        {
            User df = new User();
            Session["check"] = 2;
            MailMessage mail = new MailMessage();
            SmtpClient smtpserver = new SmtpClient("smtp.gmail.com");
            int st = random();
            Session["otp"] = st;
            mail.From = new MailAddress("deekshatiwari965@gmail.com");
            mail.To.Add(Session["email"].ToString());
            mail.Subject = "Test Email";
            mail.Body = "Confirmation Code" + "\n" + st + "\t" + "Thank u";

            smtpserver.Port = 587;
            smtpserver.Credentials = new System.Net.NetworkCredential("deekshatiwari965@gmail.com", "9807939808");
            smtpserver.EnableSsl = true;
            smtpserver.Send(mail);
            string script = "<script>alert('Send confirmation code on your email..!!')</script>";
            Response.Write(script);
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();

            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            Session["verification"] = 1;
            binddata_user(df);

            return View("checkout", df);
        }
        public ActionResult placeordercod(int id)
        {
            var dt = (from j in db.tbl_cart where j.c_id == id select j).ToList();
            tbl_order dt1 = new tbl_order();
            string nm = null;
            int stock = 0;
            dt1.user_id = id;
            dt1.status = "Pending";
            dt1.deliverydate = DateTime.Now.AddDays(7);
            dt1.paymentmode = "COD";
            if (Session["delivery_address"] != null)
            {
                dt1.address = Session["delivery_address"].ToString();
            }
            else
            {
                dt1.address = Session["address"].ToString();
            }
            dt1.grandtotal = Convert.ToDecimal(Session["grand_total"]);
            dt1.DOC = System.DateTime.Now;
            dt1.DOM = System.DateTime.Now;
            db.tbl_order.Add(dt1);
            db.SaveChanges();
            foreach (var item in dt)
            {
                nm = (from j in db.tbl_product where j.p_id == item.p_id select j.name).FirstOrDefault();
                stock = Convert.ToInt32((from j in db.tbl_stock where j.product_nm == nm select j.stock).FirstOrDefault());
                var dt3 = (from j in db.tbl_stock where j.product_nm == nm select j).FirstOrDefault();

                tbl_orderdetails dt2 = new tbl_orderdetails();
                int maxid = (from c in db.tbl_order select c.order_id).Max();
                Session["Order_id"] = maxid;
                dt2.order_id = maxid;
                dt2.p_id = item.p_id;
                dt2.quantity = item.quantity;
                dt2.total = item.total;
                db.tbl_orderdetails.Add(dt2);
                dt3.stock = stock - item.quantity;

            }
            removecart();
            db.SaveChanges();
            tempdata tr = new tempdata();
            tr.user_id = id;
            int maxid1 = (from c in db.tbl_order select c.order_id).Max();
            tr.order_id = maxid1;
            tr.date = Convert.ToString(System.DateTime.Now);
            db.tempdatas.Add(tr);
            db.SaveChanges();
            Session["verification"] = 0;
            return RedirectToAction("Orderconfirm");
        }
        [HttpPost]
        public ActionResult checkout(User ss)
        {
            User df = new User();
            string otp = Session["otp"].ToString();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            if (otp == ss.confirmation)
            {
                string nm = null;
                int stock = 0;
                int id = Convert.ToInt32(Session["u_id"]);
                var dt = (from j in db.tbl_cart where j.c_id == id select j).ToList();

                tbl_order dt1 = new tbl_order();
                dt1.user_id = id;
                dt1.status = "Pending";
                dt1.deliverydate = DateTime.Now.AddDays(7);
                if (Convert.ToInt32(Session["check"]) == 1)
                {
                    dt1.paymentmode = "Credit Card";
                }
                else
                {
                    dt1.paymentmode = "Debit Card";
                }

                if (Session["delivery_address"] != null)
                {
                    dt1.address = Session["delivery_address"].ToString();
                }
                else
                {
                    dt1.address = Session["address"].ToString();
                }
                dt1.grandtotal = Convert.ToDecimal(Session["grand_total"]);
                dt1.DOC = System.DateTime.Now;
                dt1.DOM = System.DateTime.Now;
                db.tbl_order.Add(dt1);
                db.SaveChanges();
                foreach (var item in dt)
                {
                    nm = (from j in db.tbl_product where j.p_id == item.p_id select j.name).FirstOrDefault();
                    stock = Convert.ToInt32((from j in db.tbl_stock where j.product_nm == nm select j.stock).FirstOrDefault());

                    tbl_orderdetails dt2 = new tbl_orderdetails();
                    var dt3 = (from j in db.tbl_stock where j.product_nm == nm select j).FirstOrDefault();
                    int maxid = (from c in db.tbl_order select c.order_id).Max();
                    Session["Order_id"] = maxid;
                    dt2.order_id = maxid;
                    dt2.p_id = item.p_id;
                    dt2.quantity = item.quantity;
                    dt2.total = item.total;
                    db.tbl_orderdetails.Add(dt2);
                    dt3.stock = stock - item.quantity;

                }
                removecart();
                db.SaveChanges();

                tempdata tt = new tempdata();
                tt.user_id = Convert.ToInt32(Session["u_id"]);
                int maxid1 = (from c in db.tbl_order select c.order_id).Max();
                tt.order_id = maxid1;
                tt.date = Convert.ToString(System.DateTime.Now);
                db.tempdatas.Add(tt);
                db.SaveChanges();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                binddata_user(df);


                tbl_payment dt4 = new tbl_payment();
                dt4.order_id = Convert.ToInt16(Session["Order_id"]);
                dt4.name_on_card = ss.name_on_card;
                dt4.card_no = ss.card_no;
                dt4.card_exp = ss.card_exp;
                dt4.cvv = ss.cvv;

                dt4.amount = Convert.ToInt16(Session["grand_total"]);
                dt4.confirmation = Session["verification"].ToString();
                dt4.DOC = System.DateTime.Now;
                db.tbl_payment.Add(dt4);
                db.SaveChanges();
                return View("Orderconfirm", df);
            }
            else
            {
                Session["verification"] = 0;
                Response.Write("<script>alert('Wrong OTP')</script>");
                return View("checkout", df);

            }


        }


        //----------------------------------------------------order details-----------------------------
        public ActionResult Orderdetails()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();

            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            binddata_user(df);
            binddata_orderdetails(df);

            return View(df);


        }
        private void binddata_user(User ss)
        {
            List<User> lc = new List<User>();

            int id = Convert.ToInt16(Session["u_id"]);
            var dt = (from j in db.tbl_user where j.user_id == id select j).ToList();
            foreach (var item in dt)
            {
                User cc = new User();
                cc.fname = item.fname;
                cc.mname = item.mname;
                cc.lname = item.lname;
                cc.address = item.address;
                cc.mobile = item.mobile;
                cc.email = item.email;
                lc.Add(cc);
            }


            ss.userdetail = lc;
        }

        private void binddata_orderdetails(User ss)
        {
            List<User> lc = new List<User>();

            int id = Convert.ToInt16(Session["u_id"]);

            double grandtotal = 0;
            var dt = (from j in db.tbl_cart

                      join
                       k in db.tbl_product on j.p_id equals k.p_id


                      select new
                      {
                          p_id = j.p_id,
                          name = k.name,
                          price = k.price,
                          cart_id = j.cart_id,
                          c_id = j.c_id,
                          quantity = j.quantity,
                          total = j.total,
                          image = k.image,
                      }).ToList();
            foreach (var item in dt)
            {
                User cc = new User();
                cc.cart_id = item.cart_id;
                cc.c_id = item.c_id;
                cc.name = item.name;
                cc.p_id = item.p_id;
                cc.price = item.price;
                cc.quantity = item.quantity;
                cc.total = item.total;
                cc.image = item.image;
                grandtotal = grandtotal + Convert.ToDouble(item.total);
                lc.Add(cc);
            }
            Session["grand_total"] = grandtotal;
            ss.cartitems = lc;
        }

        public ActionResult removeorderitem(int id)
        {
            User df = new User();
            var dt = (from j in db.tbl_cart where j.cart_id == id select j).FirstOrDefault();
            db.tbl_cart.Remove(dt);
            db.SaveChanges();
            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return RedirectToAction("Orderdetails");
        }

        [HttpPost]
        public ActionResult Orderdetails(User ss)
        {
            
                User df = new User();
                Session["delivery_address"] = ss.address;
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                binddata_user(df);
                binddata_orderdetails(df);
                return View(df);
            

          
        }
        public ActionResult cartcheck2()
        {
            int id = Convert.ToInt16(Session["u_id"]);
            var dt = (from j in db.tbl_cart where j.c_id == id select j).ToList();
            if (dt.Count != 0)
            {
                User df = new User();

                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();

                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                binddata_user(df);
                binddata_orderdetails(df);
                binddata_user(df);
                return RedirectToAction("checkout", df);

            }
            else
            {
                Response.Write("<script>alert('Cart is empty')</script>");
                User df = new User();
                df.catlst = DBContext.getcat();
                df.collst = DBContext.getcol();
                int count_id = Convert.ToInt16(Session["u_id"]);
                var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
                Session["count"] = count_dt.Count();
                binddata_user(df);
                binddata_orderdetails(df);
                return RedirectToAction("Orderdetails", df);

            }
        }


        //--------------------------------------------------------------------conform order---------------------
        public ActionResult Orderconfirm()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);
        }


        public ActionResult Invoice()
        {

            if (Session["Order_id"] != null)
            {
                int id = Convert.ToInt32(Session["Order_id"]);
                User jj = new User();
                jj.order_id = id;

                DataTable dt = DBContext.getinvoice(id);
                ReportDocument rt = new ReportDocument();
                ReportClass th = new ReportClass();
                rt.Load(@"D:\jewellery_project\jewellery_project\Report.rpt");
                rt.SetDataSource(dt);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream strm = rt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                return File(strm, "application/pdf");
            }
            return View();
        }

        //---------------------------------------------------------our store-----------------------------------
        public ActionResult Our_store()
        {
            User df = new User();

            df.catlst = DBContext.getcat();
            df.collst = DBContext.getcol();
            int count_id = Convert.ToInt16(Session["u_id"]);
            var count_dt = (from j in db.tbl_cart where j.c_id == count_id select j).ToList();
            Session["count"] = count_dt.Count();
            return View(df);

        }
    }
}