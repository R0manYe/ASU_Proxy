using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;

namespace ASU_Proxy
{
    /// <summary>
    /// Сводное описание для Proxy
    /// </summary>
    [WebService(Namespace = "http://192.168.1.125/Asu_proxy/Proxy.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Чтобы разрешить вызывать веб-службу из скрипта с помощью ASP.NET AJAX, раскомментируйте следующую строку. 
    // [System.Web.Script.Services.ScriptService]
    public class Proxy : System.Web.Services.WebService
    { 

        [WebMethod]    
        public string Zapros(string Perem)
        {        

        var _url = "http://10.248.35.14:8092/AppServer/IEtranSysservice";
        var _action = "http://10.248.35.14:8092/AppServer/IEtranSysservice?op=HelloWorld";
            XmlDocument CreateSoap()
            {
                
                    XmlDocument soapEnvelopeDocument = new XmlDocument();
                    soapEnvelopeDocument.LoadXml(
                     "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:sys='SysEtranInt'>" +
                      "<soapenv:Body>" +
                     "<sys:GetBlock>" +
                     "<Login>ДУРАКОВ_ВА</Login>" +
                     "<Password>RO238533ye</Password>" +
                     "<Text>" +                
                      Perem+
                    "</Text>" +
                     "</sys:GetBlock>" +
                     "</soapenv:Body>" +
                     "</soapenv:Envelope> ");
                   
                    return soapEnvelopeDocument;
                }
            

            XmlDocument soapEnvelopeXml = CreateSoap();
            HttpWebRequest webRequest = CreateWebRequest(_url, _action);
            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;          
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
 
                string Otvet = soapResult;
 
                return Otvet;  
            }
            
        }
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 30000;
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";


            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }



    }

}

