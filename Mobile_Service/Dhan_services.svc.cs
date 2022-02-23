using Mobile_Service.get_peram;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Mobile_Service.Response;
using System.ServiceModel.Activation;
using System.Web;
using System.Net;
using System.IO;

namespace Mobile_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Dhan_services" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Dhan_services.svc or Dhan_services.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Dhan_services : IDhan_services
    {
        sqlhelper objsh = new sqlhelper();

        public string Translate(string sourceText)
        {
            // Initialize
            //this.Error = null;
            //this.TranslationSpeechUrl = null;
            //this.TranslationTime = TimeSpan.Zero;
            DateTime tmStart = DateTime.Now;
            string translation = string.Empty;

            try
            {
                // Download translation
                string url = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                                            "en", "gu", HttpUtility.UrlEncode(sourceText));

                //string outputFile = Path.GetTempFileName();
                string filepath = "/Temp/nm.tmp";
                string outputFile = System.Web.Hosting.HostingEnvironment.MapPath("~/" + filepath);
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                    wc.DownloadFile(url, outputFile);
                }

                // Get translated text
                if (File.Exists(outputFile))
                {
                    // Get phrase collection
                    string text = File.ReadAllText(outputFile);
                    int index = text.IndexOf(string.Format(",,\"{0}\"", "en"));
                    if (index == -1)
                    {
                        // Translation of single word
                        int startQuote = text.IndexOf('\"');
                        if (startQuote != -1)
                        {
                            int endQuote = text.IndexOf('\"', startQuote + 1);
                            if (endQuote != -1)
                            {
                                translation = text.Substring(startQuote + 1, endQuote - startQuote - 1);
                            }
                        }
                    }
                    else
                    {
                        // Translation of phrase
                        text = text.Substring(0, index);
                        text = text.Replace("],[", ",");
                        text = text.Replace("]", string.Empty);
                        text = text.Replace("[", string.Empty);
                        text = text.Replace("\",\"", "\"");

                        // Get translated phrases
                        string[] phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; (i < phrases.Count()); i += 2)
                        {
                            string translatedPhrase = phrases[i];
                            if (translatedPhrase.StartsWith(",,"))
                            {
                                i--;
                                continue;
                            }
                            translation += translatedPhrase + "  ";
                        }
                    }

                    // Fix up translation
                    translation = translation.Trim();
                    translation = translation.Replace(" ?", "?");
                    translation = translation.Replace(" !", "!");
                    translation = translation.Replace(" ,", ",");
                    translation = translation.Replace(" .", ".");
                    translation = translation.Replace(" ;", ";");

                    // And translation speech URL
                    //this.TranslationSpeechUrl = string.Format("https://translate.googleapis.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}&client=gtx",
                    //HttpUtility.UrlEncode(translation), Translator.LanguageEnumToIdentifier(targetLanguage), translation.Length);
                    File.Delete(outputFile);

                }
            }
            catch
            {
            }
            return translation;
        }

        public company_response chk_login(login_peram peram)
        {
            company_response r = new company_response();

            try
            {
                SqlParameter[] para = new SqlParameter[]
           {
               new SqlParameter("@user",peram.username),
               new SqlParameter("@pass",peram.password)
           };

                DataSet ds = objsh.GetDataTable(CommandType.StoredProcedure, "SPCheckLogin", para);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null)
                    {

                        log[] l = new log[ds.Tables[0].Rows.Count];
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            l[i] = new log();
                            l[i].name = Convert.ToString(ds.Tables[0].Rows[i][0]);
                            l[i].type = Convert.ToString(ds.Tables[0].Rows[i][1]);
                        }
                        //r.data = l;
                        //r.message = "Success";
                        //r.status = (int)responsestatuscode.success;
                        //return r;

                        DataTable dt = objsh.GetDataTable("select [CompanyName],[Shortcut] from CompanyMaster");
                        if (dt != null)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                comp[] e = new comp[dt.Rows.Count];
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    e[i] = new comp();
                                    e[i].cname = Convert.ToString(dt.Rows[i][0]);
                                }

                                r.data = e;
                                r.message = "Success";
                                r.status = (int)responsestatuscode.success;
                                return r;

                            }
                            else
                            {
                                r.message = "No data found";
                                r.status = (int)responsestatuscode.nodatafound;
                                return r;
                            }
                        }
                    }
                }
                else
                {
                    r.message = "UserName And Password not match";
                    r.status = (int)responsestatuscode.nodatafound;
                    return r;
                }

            }
            catch (Exception ex)
            {
                r.message = ex.Message;
                r.status = (int)responsestatuscode.failure;
                return r;
            }
            r.message = "No Data Found";
            r.status = (int)responsestatuscode.nodatafound;
            return r;
        }
        public changepass_response change_password(changepass_peram peram)
        {
            changepass_response r = new changepass_response();
            try
            {

                SqlParameter[] para = new SqlParameter[]
                    {
                        new SqlParameter("@user",peram.username),
                        new SqlParameter("@oldpass",peram.oldpassword),
                        new SqlParameter("@newpass",peram.newpassword),
                        new SqlParameter("@res",SqlDbType.Int)
                    };
                para[3].Direction = ParameterDirection.Output;
                objsh.ExecuteNonQuery(CommandType.StoredProcedure, "SPChangePassword", para);
                int res = Convert.ToInt32(para[3].Value);
                if (res > 0)
                {

                    r.data = 0;
                    r.message = "Success";
                    r.status = (int)responsestatuscode.success;
                    return r;

                }
                else
                {
                    r.message = "UserName And Password not match";
                    r.status = (int)responsestatuscode.nodatafound;
                    return r;
                }
            }
            catch (Exception ex)
            {
                r.message = ex.Message;
                r.status = (int)responsestatuscode.failure;
                return r;
            }
        }

        public booking_response booking(booking_peram[] peram)
        {
            booking_response r = new booking_response();

            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Clear();
                dt.Rows.Clear();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("AgentCode", typeof(string));
                dt.Columns.Add("C_Name", typeof(string));
                dt.Columns.Add("C_Guj", typeof(string));
                dt.Columns.Add("GameName", typeof(string));
                dt.Columns.Add("J_Session", typeof(bool));
                dt.Columns.Add("J_Time", typeof(bool));
                dt.Columns.Add("Barcode", typeof(string));
                dt.Columns.Add("Ank", typeof(string));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("Code", typeof(int));
                for (int i = 0; i < peram.Count(); i++)
                {
                    peram[i].C_Guj = Translate(peram[i].C_Name);
                    dt.Rows.Add(new Object[] { i + 1, peram[i].AgentCode, peram[i].C_Name, peram[i].C_Guj, peram[i].GameName, peram[i].J_Session, peram[i].J_Time, peram[i].Barcode, peram[i].Ank, peram[i].Amount, peram[i].Code });
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataTable dtValidateCode = objsh.GetDataTable("select top 1 CodeId,Code,CodeMaster.Agent_Id,IsUsed from CodeMaster inner join AgentMaster on AgentMaster.Agent_Id=CodeMaster.Agent_Id where AgentMaster.AgentCode='" + peram[0].AgentCode + "' and CodeMaster.Code=" + peram[0].Code);
                    if (dtValidateCode != null && dtValidateCode.Rows.Count > 0)
                    {
                        Int64 _AgentId = string.IsNullOrEmpty(Convert.ToString(dtValidateCode.Rows[0]["Agent_Id"])) ? 0 : Convert.ToInt64(dtValidateCode.Rows[0]["Agent_Id"]);
                        if (_AgentId == 0)
                        {
                            r.message = "Invalid either AgentCode or Code, please check and try again.";
                            r.status = (int)responsestatuscode.failure;
                            return r;
                        }
                    }
                    else
                    {
                        r.message = "Invalid either AgentCode or Code, please check and try again.";
                        r.status = (int)responsestatuscode.failure;
                        return r;
                    }

                    SqlParameter[] para = new SqlParameter[]
                        {
                            new SqlParameter("@citems",dt),
                            new SqlParameter("@res",SqlDbType.Int)
                        };
                    para[1].Direction = ParameterDirection.Output;
                    objsh.ExecuteNonQuery(CommandType.StoredProcedure, "SPInsertAgentCollection", para);
                    int res = Convert.ToInt32(para[1].Value);
                    if (res == 1)
                    {
                        para = new SqlParameter[]
                        {
                            new SqlParameter("@bcode",peram[0].Barcode),
                            new SqlParameter("@agent",peram[0].AgentCode)
                        };
                        DataSet ds = objsh.GetDataTable(CommandType.StoredProcedure, "SPSelectCollection", para);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                peram[i].AgentCode = Convert.ToString(ds.Tables[0].Rows[i]["AgentCode"]);
                                peram[i].C_Name = Convert.ToString(ds.Tables[0].Rows[i]["C_Name"]);
                                peram[i].C_Guj = Convert.ToString(ds.Tables[0].Rows[i]["C_Guj"]);
                                peram[i].GameName = Convert.ToString(ds.Tables[0].Rows[i]["GameName"]);
                                peram[i].J_Session = Convert.ToBoolean(ds.Tables[0].Rows[i]["J_Session"]);
                                peram[i].J_Time = Convert.ToBoolean(ds.Tables[0].Rows[i]["J_Time"]);
                                peram[i].Barcode = Convert.ToString(ds.Tables[0].Rows[i]["Barcode"]);
                                peram[i].Ank = Convert.ToString(ds.Tables[0].Rows[i]["Ank"]);
                                peram[i].Question = Convert.ToString(ds.Tables[0].Rows[i]["Question"]);
                                peram[i].Answer = Convert.ToString(ds.Tables[0].Rows[i]["Answer"]);
                                peram[i].Amount = Convert.ToString(ds.Tables[0].Rows[i]["Amount"]);
                            }
                        }
                        r.data = peram;
                        r.message = "Success";
                        r.status = (int)responsestatuscode.success;
                        return r;

                    }
                    else if (res == 2)
                    {
                        r.message = "Please enter right number";
                        r.status = (int)responsestatuscode.failure;
                        return r;
                    }
                    else if (res == 3)
                    {
                        r.message = "Your balance is exced than your limit";
                        r.status = (int)responsestatuscode.failure;
                        return r;
                    }
                    else if (res == 4)
                    {
                        r.message = "Booking is closed for this session";
                        r.status = (int)responsestatuscode.failure;
                        return r;
                    }
                    else if (res == 5)
                    {
                        r.message = "Limit has been crossed";
                        r.status = (int)responsestatuscode.failure;
                        return r;
                    }
                    else
                    {
                        r.message = "Something wrong in insert";
                        r.status = (int)responsestatuscode.failure;
                        return r;
                    }
                }
                else
                {
                    r.message = "Empty parameter";
                    r.status = (int)responsestatuscode.failure;
                    return r;
                }

            }
            catch (Exception ex)
            {
                r.message = ex.Message;
                r.status = (int)responsestatuscode.failure;
                return r;
            }
        }
        /*   -----------------------        */
        public report_response get_agent_data(report_peram reportpera)
        {
            report_response res = new report_response();
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@agent",reportpera.agent),
                    new SqlParameter("@frmdt",Convert.ToDateTime(reportpera.from)),
                    new SqlParameter("@todt",Convert.ToDateTime(reportpera.to))
                };
                DataSet ds = new DataSet();
                ds = objsh.GetDataTable(CommandType.StoredProcedure, "SPGetAgentDetail", para);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    report[] r = new report[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        r[i] = new report();
                        r[i].collectdate = Convert.ToString(ds.Tables[0].Rows[i]["Collect_Date"]);
                        r[i].totalcollect = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalCollect"]);
                        r[i].commission = Convert.ToDecimal(ds.Tables[0].Rows[i]["Commission"]);
                        r[i].commamt = Convert.ToDecimal(ds.Tables[0].Rows[i]["CommAmt"]);
                        r[i].winamt = Convert.ToDecimal(ds.Tables[0].Rows[i]["WinAmt"]);
                        r[i].finalbalance = Convert.ToDecimal(ds.Tables[0].Rows[i]["FinalBalance"]);
                    }
                    res.data = r;
                    res.message = "Success";
                    res.status = (int)responsestatuscode.success;
                    return res;
                }
                else
                {
                    res.message = "No data found";
                    res.status = (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }

            //res.message = "No Data Found";
            //res.status = (int)responsestatuscode.nodatafound;
            //return res;

        }

        public winnerpay_response pay_winamt(winnerpay_peram peram)
        {
            winnerpay_response res = new winnerpay_response();
            try
            {
                DataTable dtWinAnk = new DataTable();
                dtWinAnk.Columns.Add("Win_Id", typeof(long));
                for (int i = 0; i < peram.win_ank_list.Count(); i++)
                {
                    dtWinAnk.Rows.Add();
                    dtWinAnk.Rows[i]["Win_Id"] = peram.win_ank_list[i].ToString();
                }

                DataTable dt = objsh.GetDataTable("select Barcode from AgentCollectionMaster where AgentCode='" + peram.agent + "' and Code=" + peram.code);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<winank> mr = new List<winank>();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[j]["Barcode"])))
                        {
                            string _barcode = Convert.ToString(dt.Rows[j]["Barcode"]);

                            SqlParameter[] para = new SqlParameter[]
                            {
                                new SqlParameter("@barcode",_barcode),
                                new SqlParameter("@agent",peram.agent),
                                new SqlParameter("@winank",dtWinAnk),
                                new SqlParameter("@res",SqlDbType.Int)
                            };

                            para[3].Direction = ParameterDirection.Output;
                            objsh.ExecuteNonQuery(CommandType.StoredProcedure, "SPUpdateWinner", para);
                            int results = Convert.ToInt32(para[3].Value);
                            if (results == 1)
                            {
                                SqlParameter[] param = new SqlParameter[]
                                {
                                    new SqlParameter("@barcode",_barcode),
                                    new SqlParameter("@agent",peram.agent),
                                    new SqlParameter("@winank",dtWinAnk)
                                };
                                DataSet ds = objsh.GetDataTable(CommandType.StoredProcedure, "SPGetPayableWinnerAmt", param);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    winank[] r = new winank[ds.Tables[0].Rows.Count];
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        r[i] = new winank();
                                        r[i].company = Convert.ToString(ds.Tables[0].Rows[i]["Company"]);
                                        r[i].session = Convert.ToString(ds.Tables[0].Rows[i]["J_Session"]);
                                        r[i].ank = Convert.ToString(ds.Tables[0].Rows[i]["Ank"]);
                                        r[i].amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"]);
                                        r[i].rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]);
                                        r[i].winamt = Convert.ToDecimal(ds.Tables[0].Rows[i]["WinAmt"]);
                                    }
                                    mr.AddRange(r);
                                }
                            }
                            //else if (results == 2)
                            //{
                            //    res.message = "You have already paid winning amount";
                            //    res.status = (int)responsestatuscode.failure;
                            //    res.result = 0;
                            //    return res;
                            //}
                            //else
                            //{
                            //    res.message = "Something goes wrong";
                            //    res.status = (int)responsestatuscode.failure;
                            //    res.result = 0;
                            //    return res;
                            //}
                        }
                    }

                    if (mr.Count > 0)
                    {
                        res.data = mr;
                        res.message = "Winner amount paid successfully";
                        res.status = (int)responsestatuscode.success;
                        res.result = 1;
                        return res;
                    }
                }
                else
                {
                    res.message = "Sorry! You have not win any amount";
                    res.status = (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                res.result = 0;
                return res;
            }

            res.message = "Sorry! You have not win any amount";
            res.status = (int)responsestatuscode.nodatafound;
            return res;
        }

        public winner_response check_winner(winnerpay_peram peram)
        {
            winner_response res = new winner_response();
            try
            {
                DataTable dt = objsh.GetDataTable("select Barcode from AgentCollectionMaster where AgentCode='" + peram.agent + "' and Code=" + peram.code);

                if (dt != null && dt.Rows.Count > 0)
                {
                    decimal totalWinAmt = 0;
                    List<winank> mr = new List<winank>();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[j]["Barcode"])))
                        {
                            string _barcode = Convert.ToString(dt.Rows[j]["Barcode"]);

                            SqlParameter[] para = new SqlParameter[]
                            {
                                new SqlParameter("@barcode",_barcode),
                                new SqlParameter("@agent",peram.agent)
                            };

                            DataSet ds = objsh.GetDataTable(CommandType.StoredProcedure, "SPGetWinnerDetail", para);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                winank[] r = new winank[ds.Tables[0].Rows.Count];
                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    r[i] = new winank();
                                    r[i].win_id = Convert.ToInt64(ds.Tables[0].Rows[i]["Win_Id"]);
                                    r[i].company = Convert.ToString(ds.Tables[0].Rows[i]["Company"]);
                                    r[i].session = Convert.ToString(ds.Tables[0].Rows[i]["J_Session"]);
                                    r[i].ank = Convert.ToString(ds.Tables[0].Rows[i]["Ank"]);
                                    r[i].amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"]);
                                    r[i].rate = Convert.ToDecimal(ds.Tables[0].Rows[i]["Rate"]);
                                    r[i].winamt = string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i]["WinAmt"])) ? 0 : Convert.ToDecimal(ds.Tables[0].Rows[i]["WinAmt"]);
                                    totalWinAmt += string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[i]["WinAmt"])) ? 0 : Convert.ToDecimal(ds.Tables[0].Rows[i]["WinAmt"]);
                                }
                                mr.AddRange(r);
                            }
                            //else
                            //{
                            //    res.message = "Sorry! You have not win any amount";
                            //    res.status = (int)responsestatuscode.nodatafound;
                            //    return res;
                            //}
                        }
                    }

                    if (mr.Count > 0)
                    {
                        winank wi = new winank();
                        wi.winamt = totalWinAmt;
                        mr.Add(wi);

                        res.data = mr;
                        res.message = "Success";
                        res.status = (int)responsestatuscode.success;
                        return res;
                    }
                    else
                    {
                        res.message = "Sorry! You have not win any amount";
                        res.status = (int)responsestatuscode.nodatafound;
                        return res;
                    }
                }
                else
                {
                    res.message = "Invalid either AgentCode or Code, please check and try again.";
                    res.status = (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        public jackpotresult_response get_jackpot_ank()
        {
            jackpotresult_response res = new jackpotresult_response();
            try
            {
                DataTable dt = new DataTable();
                dt = objsh.GetDataTable("select cm.CompanyName,rm.Day_Open,rm.Day_Close,rm.Night_Open,rm.Night_Close from RasultMaster rm join CompanyMaster cm on rm.Comp_Id=cm.Id where CONVERT(date,Result_Date)=CONVERT(date,GETDATE())union all select 'CN' as CompanyName,GameAnk as Day_Open,'' as Day_Close,'' as Night_Open,'' as Night_Close from Cnmaster where cn_Id=(select Top(1) cn_Id from Cnmaster where CONVERT(date,J_Date)=CONVERT(date,GETDATE()) order by cn_Id desc)");
                if (dt != null && dt.Rows.Count > 0)
                {
                    jackpotank[] r = new jackpotank[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        r[i] = new jackpotank();
                        r[i].company = Convert.ToString(dt.Rows[i]["CompanyName"]);
                        r[i].dayopen = Convert.ToString(dt.Rows[i]["Day_Open"]);
                        r[i].dayclose = Convert.ToString(dt.Rows[i]["Day_Close"]);
                        r[i].nightopen = Convert.ToString(dt.Rows[i]["Night_Open"]);
                        r[i].nightclose = Convert.ToString(dt.Rows[i]["Night_Close"]);
                        //r[i].winamt = Convert.ToDecimal(ds.Tables[0].Rows[i]["WinAmt"]);
                    }
                    res.data = r;
                    res.message = "Success";
                    res.status = (int)responsestatuscode.success;
                    return res;
                }
                else
                {
                    res.message = "No data found";
                    res.status = (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        public balance_response get_balance(balance_peram peram)
        {
            balance_response res = new balance_response();
            try
            {
                SqlParameter[] para = new SqlParameter[]
                {
                    new SqlParameter("@agent",peram.agentcode),
                };

                DataSet ds = objsh.GetDataTable(CommandType.StoredProcedure, "SPAgentTotal", para);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    balance[] r = new balance[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        r[i] = new balance();

                        r[i].amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"]);
                        r[i].limit = Convert.ToDecimal(ds.Tables[0].Rows[i]["Limit"]);
                    }
                    res.data = r;
                    res.message = "Success";
                    res.status = (int)responsestatuscode.success;
                    return res;
                }
                else
                {
                    res.message = "No data found";
                    res.status = (int)responsestatuscode.nodatafound;
                    return res;
                }

            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        public cancle_response cancle_last_booking(cancle peram)
        {
            cancle_response response = new cancle_response();
            try
            {
                SqlParameter[] para = new SqlParameter[]
                        {
                            new SqlParameter("@type","agent"),
                            new SqlParameter("@user",peram.AgentCode),
                            new SqlParameter("@res",SqlDbType.Int)
                        };
                para[2].Direction = ParameterDirection.Output;
                objsh.ExecuteNonQuery(CommandType.StoredProcedure, "SPCancelTicket", para);
                int res = Convert.ToInt32(para[2].Value);
                if (res == 1)
                {
                    response.data = "Ticket cancelation requested successfully";
                    response.message = "success";
                    response.status = (int)responsestatuscode.success;
                    return response;
                }
                else if (res == 2)
                {
                    response.data = "You cannot cancel Ticket more than once";
                    response.message = "Fail";
                    response.status = (int)responsestatuscode.success;
                    return response;
                }
                else if (res == 3)
                {
                    response.data = "You cannot cancel last Ticket";
                    response.message = "Fail";
                    response.status = (int)responsestatuscode.success;
                    return response;
                }
                else if (res == 4)
                {
                    response.data = "You have already requested to cancel last Ticket";
                    response.message = "Fail";
                    response.status = (int)responsestatuscode.success;
                    return response;
                }
                else
                {
                    response.data = "Something wrong in cancel Ticket";
                    response.message = "Fail";
                    response.status = (int)responsestatuscode.failure;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.message = ex.Message;
                response.status = (int)responsestatuscode.failure;
                return response;
            }
        }

        public common_response buy_api(code_peram pera)
        {
            common_response res = new common_response();
            try
            {
                DataTable dt = objsh.GetDataTable("select top 1 CodeId,Code,Agent_Id,IsUsed from CodeMaster where Code=" + pera.code);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Int64 _AgentId = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["Agent_Id"])) ? 0 : Convert.ToInt64(dt.Rows[0]["Agent_Id"]);
                    if (_AgentId > 0)
                    {
                        res.message = "Success";
                        res.status = (int)responsestatuscode.success;
                        return res;
                    }
                    else
                    {
                        res.message = "Wrong Code";
                        res.status = (int)responsestatuscode.failure;
                        return res;
                    }
                }
                else
                {
                    res.message = "Wrong Code";
                    res.status = (int)responsestatuscode.failure;
                    return res;
                }

            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        public code_response get_code_list(code_peram pera)
        {
            code_response res = new code_response();
            try
            {
                DataTable dt = objsh.GetDataTable(@"select CodeId,Code,CodeMaster.Agent_Id,IsUsed,CodeMaster.Pin from CodeMaster
                                                    inner join AgentMaster on AgentMaster.Agent_Id=CodeMaster.Agent_Id where AgentMaster.AgentCode='" + pera.agentcode + "' order by IsUsed ");

                if (dt != null && dt.Rows.Count > 0)
                {
                    codes[] cds = new codes[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        cds[i] = new codes();
                        cds[i].code = Convert.ToInt32(dt.Rows[i]["Code"]);
                        cds[i].is_used = Convert.ToBoolean(dt.Rows[i]["IsUsed"]);
                        cds[i].pin = Convert.ToInt32(dt.Rows[i]["Pin"]);
                    }

                    res.data = cds;
                    res.message = "Success";
                    res.status = (int)responsestatuscode.success;
                    return res;
                }
                else
                {
                    res.message = "No data found";
                    res.status = (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }



        #region New customer app APIs

        public company_response company()
        {
            company_response r = new company_response();
            try
            {

                DataTable dt = objsh.GetDataTable("select [Id],[CompanyName] from CompanyMaster");
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        comp[] e = new comp[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            e[i] = new comp();
                            e[i].cid = Convert.ToInt32(dt.Rows[i]["Id"]);
                            e[i].cname = Convert.ToString(dt.Rows[i]["CompanyName"]);
                            //e[i].C_Guj = Convert.ToString(dt.Rows[i]["Comp_Guj"]);
                        }

                        r.data = e;
                        r.message = "Success";
                        r.status = 1; // (int)responsestatuscode.success;
                        return r;

                    }
                    else
                    {
                        r.message = "No data found";
                        r.status = 0; // (int)responsestatuscode.nodatafound;
                        return r;
                    }
                }
            }
            catch (Exception ex)
            {
                r.message = ex.Message;
                r.status = (int)responsestatuscode.failure;
                return r;
            }
            r.message = "No data found";
            r.status = (int)responsestatuscode.nodatafound;
            return r;
        }

        public code_response verify_code(code_peram pera)
        {
            code_response res = new code_response();
            try
            {
                DataTable dt = objsh.GetDataTable(@"select CodeId,Code from CodeMaster where Code=" + pera.code);

                if (dt != null && dt.Rows.Count > 0)
                {
                    res.message = "Success";
                    res.status = 1; // (int)responsestatuscode.success;
                    return res;
                }
                else
                {
                    res.message = "Invalid code, please check and enter again";
                    res.status = 0; // (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        public code_response verify_pin(code_peram pera)
        {
            code_response res = new code_response();
            try
            {
                DataTable dt = objsh.GetDataTable(@"select Code from CodeMaster where Code=" + pera.code + " and Pin=" + pera.pin);

                if (dt != null && dt.Rows.Count > 0)
                {
                    res.message = "Natraj";
                    res.status = 1;
                    return res;
                }
                else
                {
                    sqlhelperAlt objshAlt = new sqlhelperAlt();
                    DataTable dtAlt = objshAlt.GetDataTable(@"select Code from CodeMaster where Code=" + pera.code + " and Pin=" + pera.pin);

                    if (dtAlt != null && dtAlt.Rows.Count > 0)
                    {
                        res.message = "Mantra";
                        res.status = 1;
                        return res;
                    }

                    res.message = "Invalid pin, please check and enter again";
                    res.status = 0; // (int)responsestatuscode.nodatafound;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        public bookschapter_response book_chapter_list(code_peram pera)
        {
            bookschapter_response res = new bookschapter_response();
            try
            {                
                if (pera.lookfor == "Natraj")
                {
                    DataTable dt = objsh.GetDataTable(@"select Barcode,AgentCode from AgentCollectionMaster where Code=" + pera.code + " and Comp_Id=" + pera.cid);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //Get Winner details
                        res.data_win = new List<data_win_bought>();
                        res.data_bought = new List<data_win_bought>();

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[j]["Barcode"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[j]["AgentCode"])))
                            {
                                string _barcode = Convert.ToString(dt.Rows[j]["Barcode"]);
                                string _agentCode = Convert.ToString(dt.Rows[j]["AgentCode"]);
                                
                                DataTable dtWin = objsh.GetDataTable(@"select wm.Win_Id,b.CompanyName as Company,wm.Ank,wm.Amount from WinnerMaster wm 
                                                                       join CompanyMaster b on wm.Comp_Id=b.Id where wm.Barcode='" + _barcode + "' and wm.AgentCode='" + _agentCode + "' and wm.IsPaid=0 ");

                                if (dtWin != null && dtWin.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dtWin.Rows.Count; i++)
                                    {
                                        data_win_bought _data_win_bought = new data_win_bought();
                                        _data_win_bought.win_id = Convert.ToInt64(dtWin.Rows[i]["Win_Id"]);
                                        _data_win_bought.company = Convert.ToString(dtWin.Rows[i]["Company"]);                                        
                                        _data_win_bought.ank = Convert.ToString(dtWin.Rows[i]["Ank"]);
                                        _data_win_bought.amount = Convert.ToDecimal(dtWin.Rows[i]["Amount"]);

                                        res.data_win.Add(_data_win_bought);
                                    }
                                }
                                else
                                {
                                    DateTime currentDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");

                                    DataTable dtBought = objsh.GetDataTable(@"select b.CompanyName as Company,ag.Ank,ag.Amount from AgentCollectionMaster ag 
                                                                              join CompanyMaster b on ag.Comp_Id=b.Id where Barcode='" + _barcode + "' and AgentCode='" + _agentCode + "' and CONVERT(date,Coll_Date)=" + currentDate.Date);

                                    if (dtBought != null && dtBought.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtBought.Rows.Count; i++)
                                        {
                                            data_win_bought _data_win_bought = new data_win_bought();
                                            _data_win_bought.company = Convert.ToString(dtBought.Rows[i]["Company"]);                                            
                                            _data_win_bought.ank = Convert.ToString(dtBought.Rows[i]["Ank"]);
                                            _data_win_bought.amount = Convert.ToDecimal(dtBought.Rows[i]["Amount"]);

                                            res.data_bought.Add(_data_win_bought);
                                        }
                                    }
                                }
                            }
                        }

                        if (res.data_bought.Count == 0 && res.data_win.Count == 0)
                        {
                            res.message = "No volume buy, No free amount";
                        }
                        else
                        {
                            res.message = "Success";
                        }

                        res.status = 1;
                        return res;
                    }
                    else
                    {
                        res.message = "No volume buy, No free amount";
                        res.status = 1;
                        return res;
                    }
                }
                else if (pera.lookfor == "Mantra")
                {
                    sqlhelperAlt objshAlt = new sqlhelperAlt();
                    DataTable dt = objshAlt.GetDataTable(@"select Barcode,AgentCode from AgentCollectionMaster where Code=" + pera.code + " and Comp_Id=" + pera.cid);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        //Get Winner details
                        res.data_win = new List<data_win_bought>();
                        res.data_bought = new List<data_win_bought>();

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[j]["Barcode"])) && !string.IsNullOrEmpty(Convert.ToString(dt.Rows[j]["AgentCode"])))
                            {
                                string _barcode = Convert.ToString(dt.Rows[j]["Barcode"]);
                                string _agentCode = Convert.ToString(dt.Rows[j]["AgentCode"]);

                                SqlParameter[] para = new SqlParameter[]
                                {
                                new SqlParameter("@barcode",_barcode),
                                new SqlParameter("@agent",_agentCode)
                                };

                                DataSet ds = objshAlt.GetDataTable(CommandType.StoredProcedure, "SPGetWinnerDetail", para);
                                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        data_win_bought _data_win_bought = new data_win_bought();
                                        _data_win_bought.win_id = Convert.ToInt64(ds.Tables[0].Rows[i]["Win_Id"]);
                                        _data_win_bought.company = Convert.ToString(ds.Tables[0].Rows[i]["Company"]);
                                        _data_win_bought.session = Convert.ToString(ds.Tables[0].Rows[i]["J_Session"]);
                                        _data_win_bought.ank = Convert.ToString(ds.Tables[0].Rows[i]["Ank"]);
                                        _data_win_bought.amount = Convert.ToDecimal(ds.Tables[0].Rows[i]["Amount"]);

                                        res.data_win.Add(_data_win_bought);
                                    }
                                }
                                else
                                {
                                    DateTime currentDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "India Standard Time");

                                    DataTable dtBought = objshAlt.GetDataTable(@"select b.CompanyName as Company,(case when(ag.JackpotSession=1 and ag.JackpotTime=1) then 'Day_Open' 
                                                                          when (ag.JackpotSession=1 and ag.JackpotTime=0) then 'Day_Close'
                                                                          when (ag.JackpotSession=0 and ag.JackpotTime=1) then 'Night_Open'
                                                                          when (ag.JackpotSession=0 and ag.JackpotTime=0) then 'Night_Close' end) as J_Session,ag.Ank,ag.Amount from AgentCollectionMaster ag 
                                                                          join CompanyMaster b on ag.Comp_Id=b.Id where Barcode='" + _barcode + "' and AgentCode='" + _agentCode + "' and CONVERT(date,Coll_Date)=" + currentDate.Date);

                                    if (dtBought != null && dtBought.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtBought.Rows.Count; i++)
                                        {
                                            data_win_bought _data_win_bought = new data_win_bought();
                                            _data_win_bought.company = Convert.ToString(dtBought.Rows[i]["Company"]);
                                            _data_win_bought.session = Convert.ToString(dtBought.Rows[i]["J_Session"]);
                                            _data_win_bought.ank = Convert.ToString(dtBought.Rows[i]["Ank"]);
                                            _data_win_bought.amount = Convert.ToDecimal(dtBought.Rows[i]["Amount"]);

                                            res.data_bought.Add(_data_win_bought);
                                        }
                                    }
                                }
                            }
                        }

                        if (res.data_bought.Count == 0 && res.data_win.Count == 0)
                        {
                            res.message = "No volume buy, No free amount";
                        }
                        else
                        {
                            res.message = "Success";
                        }

                        res.status = 1;
                        return res;
                    }
                    else
                    {
                        res.message = "No volume buy, No free amount";
                        res.status = 1;
                        return res;
                    }
                }

                res.message = "No volume buy, No free amount";
                res.status = 1;
                return res;
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.status = (int)responsestatuscode.failure;
                return res;
            }
        }

        #endregion
    }

}
