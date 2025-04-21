using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using jewellery_project.Models;
using System.IO;

namespace jewellery_project.Controllers
{
    public class adminController : Controller
    {

        jewellary_dbEntities1 db = new jewellary_dbEntities1();
        //
        // GET: /admin/
        public ActionResult Dashboard()
        {
            if (Session["fname"] != null)
            {

                var dt = (from j in db.tbl_user select j).ToList();
                int scount = 0;
                Session["ucount"] = dt.Count();
                var dt2 = (from j in db.tbl_order select j).ToList();
                foreach (var item in dt2)
                {
                    scount = scount + Convert.ToInt32(item.grandtotal);
                }
                Session["scount"] = scount;
                var dt3 = (from j in db.tbl_order select j).ToList();
                Session["ocount"] = dt3.Count();
                var dt4 = (from j in db.tbl_product select j).ToList();
                Session["stockcount"] = dt4.Count();
                Admin ss = new Admin();
                binddata_temp(ss);
                return View(ss);

            }
            else
            {
                return RedirectToAction("Login");

            }
        }
        //-------------------------login------------------------------
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Admin ss)
        {
            var obj = db.tbl_admin.Where(a => a.email.Equals(ss.email) && a.password.Equals(ss.password)).FirstOrDefault();
            if (obj != null)
            {
                Session["fname"] = obj.fname;
                Session["lname"] = obj.lname;
                Session["email"] = obj.email;
                return RedirectToAction("Dashboard");
            }
            else
            {
                Response.Write("<script>alert('Email Or Password Not Valid')</script>");

            }
            return View(ss);

        }
       
        public ActionResult Logout()
        {

            Session.Clear();
            return View("Login");
        }
        //---------------------------state---------------------
        [HttpGet]
        public ActionResult State()
        {
            if (Session["fname"] != null)
            {

                Admin ss = new Admin();
                binddata_state(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult State(Admin ss)
        {

            if (ss.s_id == 0)
            {
                tbl_state dt = new tbl_state();
                dt.name = ss.name;
                dt.DOC = System.DateTime.Now;
                dt.DOM = System.DateTime.Now;
                db.tbl_state.Add(dt);

                db.SaveChanges();


            }
            else
            {
                var dt = (from j in db.tbl_state where j.state_id == ss.s_id select j).FirstOrDefault();
                dt.name = ss.name;

                dt.DOM = System.DateTime.Now;

                db.SaveChanges();
            }

            binddata_state(ss);
            binddata_temp(ss);

            return RedirectToAction("State", ss);




        }
        private void binddata_state(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_state> ll = db.tbl_state.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.s_id = item.state_id;
                cc.name = item.name;
                cc.DOC = item.DOC;
                cc.DOM = item.DOM;

                lc.Add(cc);
            }
            ss.rj = lc;
        }
        [HttpGet]
        public ActionResult Edit_state(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_state where j.state_id == id select j).FirstOrDefault();
            ss.s_id = dt.state_id;
            ss.name = dt.name;

            binddata_state(ss);
            binddata_temp(ss);

            return View("State", ss);
        }
        [HttpGet]
        public ActionResult Delete_state(int id)
        {
            Admin ss = new Admin();
            var dt1 = db.tbl_state.Where(x => x.state_id == id).FirstOrDefault();
            db.tbl_state.Remove(dt1);
            db.SaveChanges();
            binddata_temp(ss);

            return RedirectToAction("State", ss);
        }
        /*------------------city Details-------------*/
        private void stateBind(Admin ss)
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
        public ActionResult City()
        {
            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                stateBind(ss);

                binddata_city(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }


        }


        [HttpPost]
        public ActionResult City(Admin ss)
        {

            if (ss.c_id == 0)
            {
                tbl_city dt = new tbl_city();
                dt.state_id = ss.st_id;
                dt.name = ss.cname;
                dt.DOC = System.DateTime.Now;
                dt.DOM = System.DateTime.Now;
                db.tbl_city.Add(dt);
                db.SaveChanges();
            }
            else
            {
                var dt = (from j in db.tbl_city where j.city_id == ss.c_id select j).FirstOrDefault();
                dt.state_id = ss.st_id;
                dt.name = ss.cname;

                dt.DOM = System.DateTime.Now;
                db.SaveChanges();
            }

            stateBind(ss);
            binddata_city(ss);
            binddata_temp(ss);

            return RedirectToAction("City", ss);
            // return View(ss);


        }

        private void binddata_city(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            // List<tbl_city> ll = db.tbl_city.ToList();
            var dt = (from j in db.tbl_city
                      join
                          k in db.tbl_state on j.state_id equals k.state_id

                      select new
                      {
                          c_id = j.city_id,
                          id = j.state_id,
                          state = k.name,
                          city = j.name,
                          DOC = j.DOC,
                          DOM = j.DOM

                      }).ToList();
            foreach (var item in dt)
            {
                Admin cc = new Admin();

                cc.name = item.state;
                cc.c_id = item.c_id;
                cc.cname = item.city;
                cc.cDOC = item.DOC;
                cc.cDOM = item.DOM;

                lc.Add(cc);
            }
            ss.rj = lc;
        }


        [HttpGet]
        public ActionResult Edit_city(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_city where j.city_id == id select j).FirstOrDefault();
            ss.c_id = dt.city_id;
            ss.st_id = dt.state_id;
            ss.cname = dt.name;

            binddata_city(ss);
            stateBind(ss);
            binddata_temp(ss);

            return View("City", ss);
        }

        [HttpGet]
        public ActionResult Delete_city(int id)
        {
            Admin ss = new Admin();
            var dt1 = db.tbl_city.Where(x => x.city_id == id).FirstOrDefault();
            db.tbl_city.Remove(dt1);
            db.SaveChanges();
            binddata_temp(ss);

            return RedirectToAction("City", ss);
        }


        //           /*-------------------------Category Details -------------------*/
        public ActionResult Category()
        {

            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                binddata_category(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }


        }

        [HttpPost]
        public ActionResult Category(Admin sa)
        {

            if (sa.cat_id == 0)
            {
                tbl_category dt = new tbl_category();
                dt.name = sa.cat_name;
                dt.DOC = System.DateTime.Now;
                dt.DOM = System.DateTime.Now;
                db.tbl_category.Add(dt);
                db.SaveChanges();
                ModelState.Clear();
            }
            else
            {
                var dt = (from j in db.tbl_category where j.cat_id == sa.cat_id select j).FirstOrDefault();
                dt.name = sa.cat_name;
                dt.DOM = System.DateTime.Now;
                db.SaveChanges();
            }


            Admin ss = new Admin();
            binddata_category(ss);
            binddata_temp(ss);

            return View("Category", ss);
            // return View(ss);
        }
        private void binddata_category(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_category> ll = db.tbl_category.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.cat_id = item.cat_id;
                cc.cat_name = item.name;
                cc.cat_DOC = item.DOC;
                cc.cat_DOM = item.DOM;

                lc.Add(cc);
            }
            ss.rj = lc;
        }
        [HttpGet]
        public ActionResult Edit_category(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_category where j.cat_id == id select j).FirstOrDefault();
            ss.cat_id = dt.cat_id;
            ss.cat_name = dt.name;

            binddata_category(ss);
            binddata_temp(ss);

            return View("Category", ss);
        }
        [HttpGet]
        public ActionResult Delete_category(int id)
        {
            Admin ss = new Admin();
            var dt1 = db.tbl_category.Where(x => x.cat_id == id).FirstOrDefault();
            db.tbl_category.Remove(dt1);
            db.SaveChanges();
            binddata_temp(ss);

            return RedirectToAction("Category", ss);
        }



        //        /*-----------------------Collection-------------------*/
        public ActionResult Collection()
        {
            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                binddata_col(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }

        }
        [HttpPost]
        public ActionResult Collection(Admin ss)
        {

            if (ss.col_id == 0)
            {
                tbl_collection dt = new tbl_collection();
                dt.name = ss.col_name;
                dt.DOC = System.DateTime.Now;
                dt.DOM = System.DateTime.Now;
                db.tbl_collection.Add(dt);
                db.SaveChanges();
            }
            else
            {
                var dt = (from j in db.tbl_collection where j.col_id == ss.col_id select j).FirstOrDefault();
                dt.name = ss.col_name;

                dt.DOM = System.DateTime.Now;
                db.SaveChanges();
            }

            binddata_col(ss);
            binddata_temp(ss);

            return RedirectToAction("Collection", ss);


        }
        private void binddata_col(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_collection> ll = db.tbl_collection.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.col_id = item.col_id;
                cc.col_name = item.name;
                cc.col_DOC = item.DOC;
                cc.col_DOM = item.DOM;

                lc.Add(cc);
            }
            ss.rj = lc;
        }
        [HttpGet]
        public ActionResult Edit_col(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_collection where j.col_id == id select j).FirstOrDefault();
            ss.col_id = dt.col_id;
            ss.col_name = dt.name;

            binddata_col(ss);
            binddata_temp(ss);

            return View("Collection", ss);
        }
        [HttpGet]
        public ActionResult Delete_col(int id)
        {
            Admin ss = new Admin();
            var dt1 = db.tbl_collection.Where(x => x.col_id == id).FirstOrDefault();
            db.tbl_collection.Remove(dt1);
            db.SaveChanges();
            binddata_temp(ss);

            return RedirectToAction("Collection", ss);
        }

        /*-------------------Product--------------*/
        [HttpGet]
        public ActionResult Product()
        {
            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                cat_bind(ss);
                col_bind(ss);
                color_bind();
                binddata_pro(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }


        }
        private void color_bind()
        {
            List<SelectListItem> ls = new List<SelectListItem>();
            ls.Add(new SelectListItem { Text = "Black", Value = "1" });
            ls.Add(new SelectListItem { Text = "White", Value = "2" });
            ls.Add(new SelectListItem { Text = "Red", Value = "3" });
            ls.Add(new SelectListItem { Text = "Green", Value = "4" });
            ls.Add(new SelectListItem { Text = "Blue", Value = "5" });
            ls.Add(new SelectListItem { Text = "Yellow", Value = "6" });
            ViewBag.clr = ls;
        }
        private void cat_bind(Admin ss)
        {
            ss.cat = new List<SelectListItem>();

            List<tbl_category> lc = db.tbl_category.ToList();
            foreach (var item in lc)
            {
                SelectListItem l = new SelectListItem();
                l.Text = item.name;
                l.Value = item.cat_id.ToString();
                ss.cat.Add(l);
            }
        }
        private void col_bind(Admin ss)
        {
            ss.col = new List<SelectListItem>();

            List<tbl_collection> lc = db.tbl_collection.ToList();
            foreach (var item in lc)
            {
                SelectListItem l = new SelectListItem();
                l.Text = item.name;
                l.Value = item.col_id.ToString();
                ss.col.Add(l);
            }
        }

        [HttpPost]
        public ActionResult Product(Admin ss, HttpPostedFileBase ffl)
        {

            if (ss.p_id == 0)
            {
                string img = "";
                if (ffl != null)
                {
                    ffl.SaveAs(Server.MapPath("~/Photos/") + ffl.FileName);
                    img = "~/Photos/" + ffl.FileName;
                }
                tbl_product dt = new tbl_product();
                tbl_stock dt1 = new tbl_stock();
                dt.cat_id = ss.category_id;
                dt.col_id = ss.collection_id;
                dt.name = ss.pname;
                dt.price = ss.price;
                dt.color = ss.color;
                dt.size = ss.size;
                dt.description = ss.description;
                dt.offer = ss.offer;
                dt.image = img;
                dt.DOC = System.DateTime.Now;
                dt.DOM = System.DateTime.Now;
                dt1.product_nm = ss.pname;
                dt1.stock = 0;
                dt1.DOC = System.DateTime.Now;
                dt1.DOM = System.DateTime.Now;
                db.tbl_product.Add(dt);
                db.tbl_stock.Add(dt1);
                db.SaveChanges();
            }
            else
            {
                var dt = (from j in db.tbl_product where j.p_id == ss.p_id select j).FirstOrDefault();
                if (ffl != null)
                {
                    ffl.SaveAs(Server.MapPath("~/Photos/") + ffl.FileName);
                    dt.image = "~/Photos/" + ffl.FileName;
                }
                dt.cat_id = ss.category_id;
                dt.col_id = ss.collection_id;
                dt.name = ss.pname;
                dt.price = ss.price;
                dt.color = ss.color;
                dt.size = ss.size;
                dt.description = ss.description;
                dt.offer = ss.offer;
                dt.image = dt.image;

                dt.DOM = System.DateTime.Now;
                db.SaveChanges();
            }

            cat_bind(ss);
            col_bind(ss);
            color_bind();
            binddata_pro(ss);
            binddata_temp(ss);

            return RedirectToAction("Product", ss);


        }
        private void binddata_pro(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            // List<tbl_product> ll = db.tbl_product.ToList();
            var dt = (from j in db.tbl_product
                      join
                          k in db.tbl_category on j.cat_id equals k.cat_id
                      join
                        l in db.tbl_collection on j.col_id equals l.col_id
                      select new
                      {
                          p_id = j.p_id,
                          cat_id = k.name,
                          col_id = l.name,
                          name = j.name,
                          price = j.price,
                          color = j.color,
                          size = j.size,
                          description = j.description,
                          offer = j.offer,
                          image = j.image,
                          DOC = j.DOC,
                          DOM = j.DOM

                      }).ToList();
            foreach (var item in dt)
            {
                Admin cc = new Admin();
                cc.p_id = item.p_id;
                cc.cat_name = item.cat_id;
                cc.col_name = item.col_id;
                cc.pname = item.name;
                cc.price = item.price;
                cc.color = item.color;
                cc.size = item.size;
                cc.description = item.description;
                cc.offer = item.offer;
                cc.pimage = item.image;
                cc.pDOC = item.DOC;
                cc.pDOM = item.DOM;

                lc.Add(cc);
            }
            ss.rj = lc;
        }
        [HttpGet]
        public ActionResult Edit_pro(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_product where j.p_id == id select j).FirstOrDefault();
            ss.p_id = dt.p_id;
            ss.pname = dt.name;
            ss.category_id = dt.cat_id;
            ss.collection_id = dt.col_id;
            ss.price = dt.price;
            ss.size = dt.size;
            ss.color = dt.color;
            ss.description = dt.description;
            ss.offer = dt.offer;
            ss.pimage = dt.image;
            ss.pDOM = dt.DOM;
            cat_bind(ss);
            col_bind(ss);
            color_bind();
            binddata_pro(ss);
            binddata_temp(ss);

            return View("Product", ss);
        }
        [HttpGet]
        public ActionResult Delete_pro(int id, string nm)
        {
            Admin ss = new Admin();
            var dt1 = db.tbl_product.Where(x => x.p_id == id).FirstOrDefault();
            var dt2 = db.tbl_stock.Where(x => x.product_nm == nm).FirstOrDefault();
            db.tbl_product.Remove(dt1);
            db.tbl_stock.Remove(dt2);
            db.SaveChanges();
            binddata_temp(ss);

            return RedirectToAction("Product", ss);
        }


        /*----------------custom----------------*/
        public ActionResult custom()
        {
            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                binddata_custom(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }

        }
        private void binddata_custom(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_custom> ll = db.tbl_custom.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.custom_id = item.custom_id;
                cc.name = item.name;
                cc.email = item.email;
                cc.mobile = item.mobile;
                cc.subject = item.subject;
                cc.message = item.message;
                cc.image = item.image;
                cc.DOC = item.DOC;

                lc.Add(cc);
            }
            ss.rjs = lc;
        }

        /*---------------------------feedback--------------------*/
        public ActionResult Feedback()
        {
            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                binddata_feedback(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }
        }

        private void binddata_feedback(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_feedback> ll = db.tbl_feedback.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.f_id = item.f_id;
                cc.name = item.name;
                cc.email = item.email;
                cc.comment = item.comment;


                cc.DOC = item.DOC;

                lc.Add(cc);
            }
            ss.rjs = lc;
        }

        //---------------------------------stock-------------------------------
        public ActionResult Stock()
        {
            Admin ss = new Admin();
            if (Session["fname"] != null)
            {
                binddata_temp(ss);

                binddata_stock(ss);
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View(ss);
        }
        private void binddata_stock(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_stock> ll = db.tbl_stock.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.stock_id = item.stock_id;
                cc.product_nm = item.product_nm;
                cc.stock = Convert.ToInt16(item.stock);

                cc.DOC = item.DOC;
                cc.pDOM = item.DOM;

                lc.Add(cc);
            }
            ss.rj1 = lc;
        }

        [HttpGet]
        public ActionResult Editstock(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_stock where j.stock_id == id select j).FirstOrDefault();
            ss.stock_id = dt.stock_id;
            ss.product_nm = dt.product_nm;
            ss.stock = Convert.ToInt16(dt.stock);


            binddata_stock(ss);
            binddata_temp(ss);

            return PartialView("stockedit", ss);
        }

        [HttpPost]
        public ActionResult Stock(Admin ss)
        {
            if (ss.stock_id != 0)
            {
                var dt = (from j in db.tbl_stock where j.stock_id == ss.stock_id select j).FirstOrDefault();
                dt.stock = ss.stock;
                db.SaveChanges();
                binddata_stock(ss);
                binddata_temp(ss);

            }
            return View("Stock", ss);
        }
        //--------------------------------------------------------user details--------
        public ActionResult User_details()
        {
            if (Session["fname"] != null)
            {
                Admin ss = new Admin();
                binddata_user(ss);
                binddata_temp(ss);

                return View(ss);
            }
            else
            {
                return RedirectToAction("Login");

            }
        }
        public void binddata_user(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_user> ll = db.tbl_user.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.user_id = item.user_id;
                cc.fname = item.fname;
                cc.mname = item.mname;
                cc.lname = item.lname;
                cc.gender = item.gender;
                cc.state_id = item.state_id;
                cc.city_id = item.city_id;
                cc.address = item.address;
                cc.mobile = item.mobile;
                cc.email = item.email;
                cc.isActive = item.isActive;
                if (cc.isActive == 1)
                {
                    cc.status = "Active";
                }
                else
                {
                    cc.status = "Blocked";
                }

                cc.DOC = item.DOC;
                cc.DOM = item.DOM;
                lc.Add(cc);

            }
            ss.rjd = lc;

        }
        public ActionResult statustrue(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_user where j.user_id == id select j).FirstOrDefault();

            dt.isActive = 1;
            db.SaveChanges();

            binddata_user(ss);
            binddata_temp(ss);

            return RedirectToAction("User_details", ss);
        }
        public ActionResult statusfalse(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_user where j.user_id == id select j).FirstOrDefault();

            dt.isActive = 0;
            db.SaveChanges();

            binddata_user(ss);
            binddata_temp(ss);

            return RedirectToAction("User_details", ss);
        }
        //----------------------------------------------------order----------------------
        public ActionResult Order()
        {
            Admin ss = new Admin();
            if (Session["fname"] != null)
            {
                binddata_order(ss);
                binddata_temp(ss);

            }
            else
            {
                return RedirectToAction("Login");
            }
            return View(ss);
        }
        [HttpPost]
        public ActionResult Order(Admin ss)
        {
            if (ss.order_id != 0)
            {
                var dt = (from j in db.tbl_order where j.order_id == ss.order_id select j).FirstOrDefault();
                dt.status = ss.status;
                db.SaveChanges();
                binddata_order(ss);
                binddata_temp(ss);

            }
            return View("Order", ss);
        }
        public void binddata_order(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tbl_order> ll = db.tbl_order.ToList();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.order_id = item.order_id;
                cc.user_id = item.user_id;
                cc.status = item.status;
                cc.deliverydate = item.deliverydate;
                cc.grandtotal = item.grandtotal;
                cc.address = item.address;
                cc.paymentmode = item.paymentmode;


                cc.DOC = item.DOC;
                cc.DOM = item.DOM;
                lc.Add(cc);

            }
            ss.rjd = lc;

        }
        [HttpGet]
        public ActionResult cancelorder()
        {
            Admin ss = new Admin();
            binddata_order(ss);
            binddata_temp(ss);

            return RedirectToAction("Order");
        }
        [HttpGet]
        public ActionResult editorder(int id)
        {
            Admin ss = new Admin();
            var dt = (from j in db.tbl_order where j.order_id == id select j).FirstOrDefault();
            ss.order_id = id;
            ss.grandtotal = dt.grandtotal;
            ss.deliverydate = dt.deliverydate;
            ss.address = dt.address;
            ss.paymentmode = dt.paymentmode;
            ss.status = dt.status;
            binddata_order(ss);
            binddata_temp(ss);

            return PartialView("orderedit", ss);
        }


        //-------------------------------------------notification--------------------------------

        public void binddata_temp(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<tempdata> ll = db.tempdatas.ToList();
            List<custom_temp> ll1 = db.custom_temp.ToList();
            Session["nt"] = ll.Count();
            Session["cnt"] = ll1.Count();
            foreach (var item in ll)
            {
                Admin cc = new Admin();
                cc.order_id = item.order_id;
                cc.user_id = item.user_id;
                cc.date = item.date;
                lc.Add(cc);

            }
            ss.ns = lc;
            foreach (var item in ll1)
            {
                Admin cc = new Admin();
                cc.custom_id = item.custom_id;
                cc.date = (item.date).ToString() ;
              
                lc.Add(cc);

            }
            ss.ns1 = lc;

        }

        //---------------------------------order notification--------------------------------
        public ActionResult Order_nt()
        {
            Admin ss = new Admin();
            if (Session["fname"] != null)
            {
                bind_order_nt(ss);
                binddata_temp(ss);
                var dt = (from j in db.tempdatas select j).ToList();
                foreach(var item in dt)
                { 
                db.tempdatas.Remove(item);
                }
                db.SaveChanges();
                binddata_temp(ss);


            }
            else
            {
                return RedirectToAction("Login");
            }
            return View(ss);
        }
        public void bind_order_nt(Admin ss)
        {
            List<Admin> lc = new List<Admin>();
            List<Admin> lc1 = new List<Admin>();
            int a = 0;
            // List<tbl_city> ll = db.tbl_city.ToList();
            var dt1 = (from j in db.tempdatas select j).ToList();
            foreach (var item in dt1)
            {
                Admin cc1 = new Admin();
                var dt = (from j in db.tbl_orderdetails
                          where j.order_id == item.order_id
                          join
                              k in db.tbl_product on j.p_id equals k.p_id

                          select new
                          {
                              name = k.name,
                              image = k.image,
                              quantity = j.quantity,
                              price = k.price,
                              total = j.total,
                              id=j.order_id

                          }).ToList();
                foreach (var item1 in dt)
                {
                    Admin cc = new Admin();
                    
                    cc.name = item1.name;
                    cc.image = item1.image;
                    cc.quantity = item1.quantity;
                    cc.price = item1.price;
                    cc.total = Convert.ToDouble(item1.total);
                    cc.order_id = item1.id;
                   
                    lc.Add(cc);
                }
                ss.ons = lc;
                cc1.user_id = item.user_id;
                cc1.order_id = item.order_id;
                cc1.date = item.date;
                cc1.a = dt.Count();
                Session["a"] = 1;
                lc1.Add(cc1);


            }
            ss.ons1 = lc1;
            

        }


        //-----------------------------------------------------------------custom notification----------------

        public ActionResult custom_nt()
        {
            Admin ss = new Admin();
            if (Session["fname"] != null)
            {
                bind_custom_nt(ss);
                binddata_temp(ss);
                var dt = (from j in db.custom_temp select j).ToList();
                foreach (var item in dt)
                {
                    db.custom_temp.Remove(item);
                }
                db.SaveChanges();
                binddata_temp(ss);


            }
            else
            {
                return RedirectToAction("Login");
            }
            return View(ss);
        }

        public void bind_custom_nt(Admin ss)
        {
            List<Admin> lc = new List<Admin>();

            var dt = (from j in db.custom_temp
                     
                      join
                          k in db.tbl_custom on j.custom_id equals k.custom_id

                      select new
                      {
                          customid = k.custom_id,
                          name = k.name,
                          email = k.email,
                          mobile = k.mobile,
                          subject = k.subject,
                          image = k.image,
                          message=k.message,
                          date=k.DOC

                      }).ToList(); 
            foreach (var item in dt)
            {
                Admin cc1 = new Admin();
               
                cc1.custom_id = item.customid;
                cc1.name = item.name;
                cc1.email = item.email;
                cc1.mobile = item.mobile;
                cc1.subject = item.subject;
                cc1.image = item.image;
                cc1.message = item.message;
                cc1.date = (item.date).ToString();
                lc.Add(cc1);


            }
            ss.ons2 = lc;


        }


    }

}