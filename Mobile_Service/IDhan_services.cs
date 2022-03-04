using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Mobile_Service.get_peram;
using Mobile_Service.Response;
using System.ServiceModel.Channels;

namespace Mobile_Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IDhan_services" in both code and config file together.
    [ServiceContract]
    public interface IDhan_services
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/chk_login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        company_response chk_login(login_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/change_password", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        changepass_response change_password(changepass_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/company", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        company_response company();
        [OperationContract]
        [WebInvoke(UriTemplate = "/booking", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        booking_response booking(booking_peram[] pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/get_agent_data", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        report_response get_agent_data(report_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/pay_winamt", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        winnerpay_response pay_winamt(winnerpay_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/check_winner", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        winner_response check_winner(winnerpay_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/get_jackpot_ank", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        jackpotresult_response get_jackpot_ank();
        [OperationContract]
        [WebInvoke(UriTemplate = "/get_balance", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        balance_response get_balance(balance_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/cancle_last_booking", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        cancle_response cancle_last_booking(cancle pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/buy_api", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        common_response buy_api(code_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/get_code_list", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        code_response get_code_list(code_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/verify_code", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        code_response verify_code(code_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/verify_pin", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        code_response verify_pin(code_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/book_chapter_list", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        bookschapter_response book_chapter_list(code_peram pera);

        [OperationContract]
        [WebInvoke(UriTemplate = "/cust_login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        cust_login_response cust_login(code_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/cust_booking", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        booking_response cust_booking(booking_peram[] pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/ag_get_code_list", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        ag_code_response ag_get_code_list(code_peram pera);
        [OperationContract]
        [WebInvoke(UriTemplate = "/cust_code_bal", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        common_response cust_code_bal(cust_code_bal_peram pera);

    }
}
