using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace jewellery_project.Models
{
    public class User
    {
        //--------------------------------------------------------reg------------------------------------------
        public int stock { get; set; }
        public int user_id { get; set; }
        public string fname { get; set; }
        public string mname { get; set; }
        public string lname { get; set; }
        public string gender { get; set; }
        public int state_id { get; set; }
        public int city_id { get; set; }
                [Required(ErrorMessage = "Please enter the Address")]

        public string address { get; set; }
        public string mobile { get; set; }
        [Required(ErrorMessage = "Please enter the email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Email is not valid")]
        public string email { get; set; }
         [Required(ErrorMessage = "Please enter the password")]
        public string password { get; set; }
        public string confirm_password { get; set; }
       public List<User> ols { get; set; }
        public int isActive { get; set; }
        public System.DateTime DOC { get; set; }
        public System.DateTime DOM { get; set; }
        public List<SelectListItem> states { get; set; }
        public List<SelectListItem> city { get; set; }
        public List<SelectListItem> rj { get; set; }


        //  --------------------------------category bind in menu
        public int cat_id { get; set; }
        public string name { get; set; }
        public List<User> catlst { get; set; }

        //--------------------------------------collection bind in menu------
        public int col_id { get; set; }
        public List<User> collst { get; set; }

        //--------------------------------------product-------------------
        public int p_id { get; set; }
        //public int cat_id { get; set; }
        //public int col_id { get; set; }
       
        public string price { get; set; }
        public int color { get; set; }
        public string size { get; set; }
        public string description { get; set; }
        public string offer { get; set; }
        public string image { get; set; }
        public List<User> products { get; set; }
        public List<User> productdetail { get; set; }



        public int quantity { get; set; }

        //----------------------------------------custom made--------------------
        public int custom_id { get; set; }
        public string subject { get; set; }
        public string message { get; set; }


        //--------------------------------------------cart---------------------
        public int cart_id { get; set; }
        public int c_id { get; set; }
        public decimal total { get; set; }
        public List<User> cartitems { get; set; }


        //-------------------------------------------feedback-------------------
        public int f_id { get; set; }
        public string comment { get; set; }
        public List<User> rjs { get; set; }

        //----------------------------------------order details-----------
        public List<User> userdetail { get; set; }

        public int order_id { get; set; }
        //---------------------------------------checkout------------
        public int otpcode { get; set; }

        public int card_id { get; set; }
        public string name_on_card { get; set; }
        public string card_no { get; set; }
        public string card_exp { get; set; }
        public int cvv { get; set; }
        public decimal amount { get; set; }
        public string confirmation { get; set; }

    }
}