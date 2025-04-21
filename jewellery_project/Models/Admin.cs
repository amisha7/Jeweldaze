
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;




namespace jewellery_project.Models
{
    public class Admin
    {
        //state
        //public int s_id { get; set; }
        //[Display(Name="sname")]
        //[Required(ErrorMessage = "Please Enter Name...!")]
        //public string sname { get; set; }
        //public System.DateTime sDOC { get; set; }
        //public System.DateTime sDOM { get; set; }
        public int s_id { get; set; }

        [Required(ErrorMessage="please enter name..!")]
        [Display(Name="state name")][DataType(DataType.Text)]
        public string name { get; set; }
        public System.DateTime DOC { get; set; }
        public System.DateTime DOM { get; set; }

        public List<Admin> rj { get; set; }

        //city
        public int c_id { get; set; }
        [Required(ErrorMessage = "Please Select State...!")]
        public int st_id { get; set; }
        [Required(ErrorMessage = "Please Select City...!")]
        public string cname { get; set; }
        public System.DateTime cDOC { get; set; }
        public Nullable<System.DateTime> cDOM { get; set; }
        public List<SelectListItem> states { get; set; }


        // category
        public int cat_id { get; set; }
        [Required(ErrorMessage = "Please Enter Category Name...!")]
        public string cat_name { get; set; }
        public System.DateTime cat_DOC { get; set; }
        public System.DateTime cat_DOM { get; set; }

        //collection
        public int col_id { get; set; }
        [Required(ErrorMessage = "Please Enter Collection Name...!")]
        public string col_name { get; set; }
        public System.DateTime col_DOC { get; set; }
        public System.DateTime col_DOM { get; set; }

        //product
        public int p_id { get; set; }
        [Required(ErrorMessage = "Please Select Category ...!")]
        public int category_id { get; set; }
        [Required(ErrorMessage = "Please Select Collection ...!")]
        public int collection_id { get; set; }
        [Required(ErrorMessage = "Please Enter Product Name...!")]
        public string pname { get; set; }
        [Required(ErrorMessage = "Please Enter Product Price...!")]
        
        public string price { get; set; }
        [Required(ErrorMessage = "Please Enter Product Color...!")]
        public int color { get; set; }
        [Required(ErrorMessage = "Please Enter Product Size...!")]
        public string size { get; set; }
        [Required(ErrorMessage = "Please Enter Product Description...!")]
        public string description { get; set; }
        [Required(ErrorMessage = "Please Enter Product Price...!")]
        public string offer { get; set; }
        //[Required(ErrorMessage = "Please Enter Product Photo...!")]
        public string pimage { get; set; }

        public System.DateTime pDOC { get; set; }
        public System.DateTime pDOM { get; set; }
        public List<SelectListItem> cat { get; set; }
        public List<SelectListItem> col { get; set; }

        //stock

        public int stock_id { get; set; }
        public string product_nm { get; set; }
        public int stock { get; set; }
        public List<Admin> rj1 { get; set; }


        //admin login
        public int a_id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        [Required(ErrorMessage = "Please Enter E-mail Id...!")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Email Id is not Valid !")]
        public string email { get; set; }
        [Required(ErrorMessage = "Please Enter Password...!")]
        [DataType(DataType.Password)]
        public string password { get; set; }


        //user details
        public int user_id { get; set; }
        public string mname { get; set; }
        public string gender { get; set; }
        public int state_id { get; set; }
        public int city_id { get; set; }
        public string address { get; set; }
        public string mobile { get; set; }
       
        public int isActive { get; set; }

        public List<Admin> rjd { get; set; }

        public string status { get; set; }

        //---------------------feedback
        public int f_id { get; set; }
        public string comment { get; set; }
        public List<Admin> rjs { get; set; }


        //-------------------------custom made-------------
          public int custom_id { get; set; }
      
        public string subject { get; set; }
        public string image { get; set; }
        public string message { get; set; }

        //-----------------------order----------------
        public int order_id { get; set; }
       
        public System.DateTime deliverydate { get; set; }
        public string paymentmode { get; set; }
       
        public decimal grandtotal { get; set; }

        //---------------------------------notification---------------------
        public int temp_id { get; set; }
        public string date { get; set; }
        public List<Admin> ns { get; set; }
        public List<Admin> ns1 { get; set; }
        public List<Admin> ons { get; set; }
        public List<Admin> ons1 { get; set; }
        public List<Admin> ons2 { get; set; }
        public int quantity { get; set; }
        public double total { get; set; }
        public int a { get; set; }
      

    }

    }

   

