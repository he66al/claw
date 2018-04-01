using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Web.Services;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace Crawl_WebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class WebService1 : System.Web.Services.WebService
    {
        SqlConnection con = new SqlConnection("Data Source=BTDEV07\\SQLEXPRESS;Initial Catalog=HyperClaw_DB;Integrated Security=False;User ID=blinklc;Password=Blinktech@123;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        SqlCommand cm;
        SqlDataAdapter da;

        [WebMethod]
        public string HyperClawOperations(string json)
        {
            if (json != "")
            {
                string response = "";
                var jss = new JavaScriptSerializer();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                string _Type = sData["type"].ToString();

                switch (_Type)
                {
                    case "registration":
                        response = Registration(json);
                        break;
                    case "login":
                        response = Login(json);
                        break;
                    case "forgot_pwd":
                        response = Forgot_pwd(json);
                        break;
                    case "reset_pwd":
                        response = Reset_pwd(json);
                        break;
                    case "payment":
                        response = Payment(json);
                        break;
                    case "profile_pic":
                        response = Profile_pic(json);
                        break;
                    case "ProductInfoSave":
                        response = ProductInfoSave(json);
                        break;
                    case "ProductInfoGet":
                        response = ProductInfoGet(json);
                        break;
                    case "CMSlogin":
                        response = cms_Login(json);
                        break;
                    case "game_play":
                        response = game_play(json);
                        break;
                    case "end_session":
                        response = end_session(json);
                        break;
                }
                return response;
            }
            else
                return "JSON is blank";
        }

        public string Registration(string json)
        {
            string response = "";
            Response Res = new Response();
            try
            {
                var jss = new JavaScriptSerializer();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                con.Open();
                cm = new SqlCommand("UserRegistration", con);
                cm.CommandType = CommandType.StoredProcedure;
                cm.Parameters.AddWithValue("@UserName", sData["UserName"].ToString());
                cm.Parameters.AddWithValue("@Email", sData["Email"].ToString());
                cm.Parameters.AddWithValue("@Password", sData["Password"].ToString());
                cm.Parameters.AddWithValue("@DateOfBirth", sData["DateOfBirth"].ToString());
                cm.Parameters.AddWithValue("@Gender", sData["Gender"].ToString());
                cm.Parameters.AddWithValue("@AddressLine1", sData["AddressLine1"].ToString());
                cm.Parameters.AddWithValue("@AddressLine2", sData["AddressLine2"].ToString());
                cm.Parameters.AddWithValue("@City", sData["City"].ToString());
                cm.Parameters.AddWithValue("@State", sData["State"].ToString());
                cm.Parameters.AddWithValue("@Country", sData["Country"].ToString());
                cm.Parameters.AddWithValue("@Zipcode", sData["Zipcode"].ToString());
                cm.Parameters.AddWithValue("@CountryCode", sData["CountryCode"].ToString());
                cm.Parameters.AddWithValue("@MobileNo", sData["MobileNo"].ToString());
                cm.Parameters.AddWithValue("@EntityKey", 0);
                int a = cm.ExecuteNonQuery();
                con.Close();
                if (a > 0)
                {
                    Res.status = "1";
                    Res.msg = "success";
                }
                else if (a == -2)
                {
                    Res.status = "0";
                    Res.msg = "Email already exists";
                }
                else
                {
                    Res.status = "0";
                    Res.msg = "Failed";
                }
                response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
                return response;
            }
            catch (Exception ex)
            {
                con.Close();
                Res.status = "0";
                Res.msg = "Failed";
                response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
                return response;
            }
        }

        public string Login(string json)
        {
            string response = "";
            Response Res = new Response();
            Login resLog = new Login();
            try
            {
                var jss = new JavaScriptSerializer();
                Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
                con.Open();
                cm = new SqlCommand("UserLogin", con);
                cm.CommandType = CommandType.StoredProcedure;
                cm.Parameters.AddWithValue("@Email", sData["type"].ToString());
                cm.Parameters.AddWithValue("@Password", sData["type"].ToString());
                da = new SqlDataAdapter(cm);
                DataTable ds = new DataTable();
                da.Fill(ds);
                con.Close();
                if (ds.Rows.Count > 0)
                {
                    if (ds.Columns.Count == 1)
                    {
                        if (ds.Rows[0][0].ToString() == "-3")
                        {
                            Res.status = "0";
                            Res.msg = "Email not exists";
                        }
                        if (ds.Rows[0][0].ToString() == "-4")
                        {
                            Res.status = "0";
                            Res.msg = "Wrong Password";
                        }
                        response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
                    }
                    else if (ds.Columns.Count > 1)
                    {
                        resLog.status = "1";
                        resLog._username = ds.Rows[0]["UserName"].ToString();
                        resLog._email = ds.Rows[0]["Email"].ToString();
                        resLog._DOB = Convert.ToDateTime(ds.Rows[0]["DateOfBirth"]);
                        resLog._Gender = ds.Rows[0]["Gender"].ToString();
                        byte[] profilePic = (byte[])ds.Rows[0]["ProfilePicture"];
                        resLog._profilepic = Convert.ToBase64String(profilePic);
                        resLog._AddressLine1 = ds.Rows[0]["AddressLine1"].ToString();
                        resLog._AddressLine2 = ds.Rows[0]["AddressLine2"].ToString();
                        resLog._City = ds.Rows[0]["City"].ToString();
                        resLog._State = ds.Rows[0]["State"].ToString();
                        resLog._Country = ds.Rows[0]["Country"].ToString();
                        resLog._Zipcode = ds.Rows[0]["Zipcode"].ToString();
                        resLog._2nd_AddressLine1 = ds.Rows[0]["2nd_AddressLine1"].ToString();
                        resLog._2nd_AddressLine2 = ds.Rows[0]["2nd_AddressLine2"].ToString();
                        resLog._2nd_City = ds.Rows[0]["2nd_City"].ToString();
                        resLog._2nd_State = ds.Rows[0]["2nd_State"].ToString();
                        resLog._2nd_Country = ds.Rows[0]["2nd_Country"].ToString();
                        resLog._2nd_Zipcode = ds.Rows[0]["2nd_Zipcode"].ToString();
                        resLog._3rd_AddressLine1 = ds.Rows[0]["3rd_AddressLine1"].ToString();
                        resLog._3rd_AddressLine2 = ds.Rows[0]["3rd_AddressLine2"].ToString();
                        resLog._3rd_City = ds.Rows[0]["3rd_City"].ToString();
                        resLog._3rd_State = ds.Rows[0]["3rd_State"].ToString();
                        resLog._3rd_Country = ds.Rows[0]["3rd_Country"].ToString();
                        resLog._3rd_Zipcode = ds.Rows[0]["3rd_Zipcode"].ToString();
                        response = JsonConvert.SerializeObject(resLog, Newtonsoft.Json.Formatting.Indented);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                con.Close();
                return response;
            }
        }

        public string Forgot_pwd(string json)
        {
            Response Res = new Response();
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            con.Open();
            cm = new SqlCommand("ForgotPassword", con);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@Email", sData["email"].ToString());
            da = new SqlDataAdapter(cm);
            DataTable ds = new DataTable();
            da.Fill(ds);
            con.Close();
            if (ds.Rows[0][0].ToString() == "0")
            {
                Res.status = "0";
                Res.msg = "Email not exists";
            }
            else if (ds.Rows[0][0].ToString() == "1")
            {
                Res.status = "1";
                Res.msg = "Email exists";
            }

            response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string Reset_pwd(string json)
        {
            Response Res = new Response();
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            con.Open();
            cm = new SqlCommand("ResetPassword", con);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@Email", sData["email"].ToString());
            cm.Parameters.AddWithValue("@Password", sData["Password"].ToString());
            cm.Parameters.AddWithValue("@EntityKey", 0);
            int a = cm.ExecuteNonQuery();
            con.Close();
            if (a == 1)
            {
                Res.status = "1";
                Res.msg = "Reset Successfully";
            }
            else
            {
                Res.status = "0";
                Res.msg = "No Reset";
            }
            response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string Payment(string json)
        {
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            con.Open();
            cm = new SqlCommand("PaymentInfoInsert", con);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@UserName", sData["UserName"].ToString());
            cm.Parameters.AddWithValue("@Email", sData["Email"].ToString());
            cm.Parameters.AddWithValue("@Mobile", sData["Mobile"].ToString());
            cm.Parameters.AddWithValue("@Amount", sData["Amount"].ToString());
            cm.Parameters.AddWithValue("@CoinsPurchased", sData["CoinsPurchased"].ToString());
            cm.Parameters.AddWithValue("@EntityKey", 0);
            int a = cm.ExecuteNonQuery();
            con.Close();

            response = JsonConvert.SerializeObject(sData, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string Profile_pic(string json)
        {
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            string _Type = sData["type"].ToString();

            response = JsonConvert.SerializeObject(sData, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string ProductInfoSave(string json)
        {
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            con.Open();
            cm = new SqlCommand("ProductDetailsInsert", con);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@ProductId", sData["ProductId"].ToString());
            cm.Parameters.AddWithValue("@ProductName", sData["ProductName"].ToString());
            cm.Parameters.AddWithValue("@ProductInfo", sData["ProductInfo"].ToString()); // ADD IMAGE PARAMETER TO ADD THE IMAGES OF PRODUCT
            cm.Parameters.AddWithValue("@ProductQty", sData["ProductQty"].ToString());
            cm.Parameters.AddWithValue("@ExistingProd", sData["ExistingProd"].ToString());
            cm.Parameters.AddWithValue("@EntityKey", 0);
            int a = cm.ExecuteNonQuery();
            con.Close();
            response = JsonConvert.SerializeObject(sData, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string ProductInfoGet(string json)
        {
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            con.Open();
            cm = new SqlCommand("ProductDetailsInsert", con);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@ProductId", sData["ProductId"].ToString());
            cm.Parameters.AddWithValue("@ProductName", sData["ProductName"].ToString());
            da = new SqlDataAdapter(cm);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            response = JsonConvert.SerializeObject(sData, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string cms_Login(string json)
        {
            Response Res = new Response();
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            con.Open();
            cm = new SqlCommand("UserLogin_CMS", con);
            cm.CommandType = CommandType.StoredProcedure;
            cm.Parameters.AddWithValue("@Email", sData["Email"].ToString());
            cm.Parameters.AddWithValue("@Password", sData["Password"].ToString());
            cm.Parameters.AddWithValue("@UserRole", sData["UserRole"].ToString());
            da = new SqlDataAdapter(cm);
            DataTable ds = new DataTable();
            da.Fill(ds);
            con.Close();
            if (ds.Rows.Count > 0)
            {
                if (ds.Columns.Count > 1)
                {
                    Res.status = ds.Rows[0]["UserName"].ToString();
                    Res.msg = ds.Rows[0]["Email"].ToString();
                }
                else
                {
                    Res.status = "1";
                    Res.msg = "199191";
                }
            }
            else
            {
                Res.status = "0";
                Res.msg = "Either email or password is incorrect!";
            }
            response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string game_play(string json)
        {
            Response Res = new Response();
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            //string _Type = sData["type"].ToString();
            Res.status = "1";
            Res.url = "199191";

            response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
            return response;
        }

        public string end_session(string json)
        {
            Response Res = new Response();
            string response = "";
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(json);
            //string _Type = sData["type"].ToString();
            Res.status = "1";
            //Res.url = "199191";

            response = JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.Indented);
            return response;
        }
    }

    public class Login
    {
        public string status { get; set; }
        public string _username { get; set; }
        public string _email { get; set; }
        public DateTime _DOB { get; set; }
        public string _Gender { get; set; }
        public string _profilepic { get; set; }

        public string _AddressLine1 { get; set; }
        public string _AddressLine2 { get; set; }
        public string _City { get; set; }
        public string _State { get; set; }
        public string _Country { get; set; }
        public string _Zipcode { get; set; }

        public string _2nd_AddressLine1 { get; set; }
        public string _2nd_AddressLine2 { get; set; }
        public string _2nd_City { get; set; }
        public string _2nd_State { get; set; }
        public string _2nd_Country { get; set; }
        public string _2nd_Zipcode { get; set; }

        public string _3rd_AddressLine1 { get; set; }
        public string _3rd_AddressLine2 { get; set; }
        public string _3rd_City { get; set; }
        public string _3rd_State { get; set; }
        public string _3rd_Country { get; set; }
        public string _3rd_Zipcode { get; set; }
    }

    public class Response
    {
        public string status { get; set; }
        public string msg { get; set; }
        public string url { get; set; }
    }

}
