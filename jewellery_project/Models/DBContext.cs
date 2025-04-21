using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace jewellery_project.Models
{
    public class DBContext
    {
        static SqlConnection con = new SqlConnection (ConfigurationManager.ConnectionStrings["connection"].ConnectionString);
        static DataTable dt;
        static SqlDataAdapter sdp;

        public static List<User> getcat()
        {
            List<User> ll = new List<User>();
            SqlCommand cmd = new SqlCommand("select * from tbl_category", con);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    User dd = new User();
                    dd.cat_id = Convert.ToInt32(dr["cat_id"]);
                    dd.name = dr["name"].ToString();
                    ll.Add(dd);
                }
            }
            con.Close();
            return ll;
        
        }
        public static List<User> getcol()
        {
            List<User> ll = new List<User>();
            SqlCommand cmd = new SqlCommand("select * from tbl_collection", con);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    User dd = new User();
                    dd.col_id = Convert.ToInt32(dr["col_id"]);
                    dd.name = dr["name"].ToString();
                    ll.Add(dd);
                }
            }
            con.Close();
            return ll;

        }
        public static List<User> getprocat(int catid)
        {
            List<User> lc = new List<User>();
            SqlCommand cmd = new SqlCommand("select * from tbl_product where cat_id='"+catid+"'", con);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    User dd = new User();
                    dd.p_id = Convert.ToInt32(rdr["p_id"]);
                    dd.name = rdr["name"].ToString();
                    dd.price = rdr["price"].ToString();
                    dd.color = Convert.ToInt32(rdr["color"]);
                    dd.image = rdr["image"].ToString();
                    dd.description = rdr["description"].ToString();
                    dd.offer = rdr["offer"].ToString();
      
                    lc.Add(dd);
                }
            }
            con.Close();
            return lc;

        }
        public static List<User> getprocol(int colid)
        {
            List<User> lc = new List<User>();
            SqlCommand cmd = new SqlCommand("select * from tbl_product where col_id='" + colid + "'", con);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    User dd = new User();
                    dd.p_id = Convert.ToInt32(rdr["p_id"]);
                    dd.name = rdr["name"].ToString();
                    dd.price = rdr["price"].ToString();
                    dd.color = Convert.ToInt32(rdr["color"]);
                    dd.image = rdr["image"].ToString();
                    dd.description = rdr["description"].ToString();
                    dd.offer = rdr["offer"].ToString();

                    lc.Add(dd);
                }
            }
            con.Close();
            return lc;

        }

        public static List<User> getprodetails(int p_id)
        {
            List<User> lc = new List<User>();
            SqlCommand cmd = new SqlCommand("select * from tbl_product where p_id='" + p_id + "'", con);
            if (con.State != ConnectionState.Open)
            {
                con.Open();
            }

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    User dd = new User();
                    dd.p_id = Convert.ToInt32(rdr["p_id"]);
                    dd.name = rdr["name"].ToString();
                    dd.price = rdr["price"].ToString();
                    dd.color = Convert.ToInt32(rdr["color"]);
                    dd.image = rdr["image"].ToString();
                    dd.description = rdr["description"].ToString();
                    dd.offer = rdr["offer"].ToString();

                    lc.Add(dd);
                }
            }
            con.Close();
            return lc;

        }
        public static DataTable getinvoice(int id)
        {
            
            DataTable dt = new DataTable();
            SqlDataAdapter cmd = new SqlDataAdapter("select * from invoicereport where order_id='" + id + "'", con);
            

            cmd.Fill(dt);
            
            return dt;

        }

 
    }
}