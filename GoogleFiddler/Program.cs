using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GoogleParse;
using System.Data;
namespace GoogleFiddler
{
    class Program
    {
        static Proxy oSecureEndpoint;
        static string sSecureEndpointHostname = "localhost";
        static int iSecureEndpointPort = 8888;
        static Thread writeThread = new Thread(WriteToDB);
        static Queue<Fiddler.Session> QueueSessions = new Queue<Session>();
        private static void WriteToDB()
        {
            while (true)
            {
                try
                {
                    //if (DataReceivedEventArgs_Kiwi.Instance.Count > 0)
                    //{

                    //    Session dataReceived = DataReceivedEventArgs_Kiwi.Instance.DeQueue();
                    //    if (!String.IsNullOrEmpty(dataReceived.GetResponseBodyAsString()))
                    //    {
                    //        WriteToFiles(dataReceived);
                    //        //20151222-->Kiwi:在这里扩展图片下载的功能
                    //    }
                    //}               
                    if (QueueSessions.Count > 0)
                    {
                        Session oSession = QueueSessions.Dequeue();
                        WriteToFiles(oSession);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        private static void WriteToFiles(Session dataReceived)
        {
            //oS.utilDecodeResponse();       //针对js可解析         
            if (dataReceived.oResponse.MIMEType == "application/json")
            {
                List<Session> list = new List<Session>();
                for (int i = 0; i < 5; i++)
                {
                    Session iSeesion = new Session(new SessionData(dataReceived));
                    list.Add(iSeesion);
                }
                #region 失败的测试
                //list[0].utilDecodeResponse();
                //Console.WriteLine(list[0].GetResponseBodyAsString());
                //list[1].utilGZIPResponse();
                //Console.WriteLine(list[1].GetResponseBodyAsString());
                //string str = "";
                //list[2].utilSetResponseBody(str);
                //Console.WriteLine(str);
                //list[3].utilGZIPResponse();
                //list[3].utilDecodeResponse();
                //Console.WriteLine(list[3].GetResponseBodyAsString());
                //Console.WriteLine(list[4].GetRequestBodyAsString());
                //list[0].utilGZIPResponse();
                //Console.WriteLine(list[0].GetResponseBodyAsString());
                //list[1].utilDeflateResponse();
                //Console.WriteLine(list[1].GetResponseBodyAsString());
                //list[2].utilGZIPResponse();
                //list[2].utilDeflateResponse();
                //Console.WriteLine(list[2].GetResponseBodyAsString()); 
                #endregion

                while ((list[3].state<=SessionStates.ReadingResponse))
                {
                    continue;
                }
                if (dataReceived.fullUrl.Contains("search"))
                {
                    //Console.WriteLine(list[3].GetResponseBodyEncoding());
                    //Console.WriteLine(list[3].);
                    list[3].utilDecodeResponse();
                    string str = list[3].GetResponseBodyAsString();
                    List<googlepoi> pois = GoogleParse.googlepoi_parser.GetItems(str);

                  
                    SavePoiToDB(pois);
                    

                    File.AppendAllText("c://fiddler//KiwiE.txt", list[3].GetResponseBodyAsString(), Encoding.UTF8);    
                }
                
            }
        }

       static private void SavePoiToDB(List<googlepoi> pois) {

            foreach (googlepoi poi in pois)
            {
                if (!IsExist(poi.oid)) {
                    AppendPoi(poi);
                }

                System.Diagnostics.Debug.Write(poi.name + ",");
            }
        }

        //判断是否存储相同的poi点
        static private bool IsExist(string oid) {
            bool rv = false;
            string sql = "select count(id) from pois where oid='"+oid+"'";
            double d=Permanence.GetPages(sql);
            if (d > 0) rv = true;
            return rv;
        }
        //追加
        static private void AppendPoi(googlepoi poi) {

            string sql = "insert into pois (oid,name,fname,address,classaddress,category,phone,website,lon,lat,ctime,closed) values ('" + poi.oid+"','"+poi.name+ "','" + poi.fname + "','" + poi.address + "','" + poi.classaddress + "','" + poi.category + "','" + poi.phone + "','" + poi.website + "','" + poi.lon + "','" + poi.lat + "',now(),"+poi.closed+") ";
            Permanence.sqlCreate(sql);
        }
        

        //单词：tampering（干扰），permit（允许）， modification（修改），echo（重复，模仿），（vast majority of）【绝大多数的】
        //You almost certainly don't want to add a handler for this event
        //你几乎可以肯定不想添加此事件处理程序
        static void Main(string[] args)
        {

            DataTable dt= Permanence.getDataTable("select * from pois");

            System.Diagnostics.Debug.WriteLine( "test line"+ ",");

            List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();

            Fiddler.FiddlerApplication.SetAppDisplayName("FiddlerKiwi");

            #region AttachEventListeners

            Fiddler.FiddlerApplication.OnNotification += delegate(object sender, NotificationEventArgs oNEA)
            {
                Console.WriteLine("**通知: " + oNEA.NotifyString);
            };
            Fiddler.FiddlerApplication.Log.OnLogString += delegate(object sender, LogEventArgs oLEA)
            {
                Console.WriteLine("**日志: " + oLEA.LogString);
            };


            Fiddler.FiddlerApplication.BeforeRequest += delegate(Fiddler.Session oS)
            {
                oS.bBufferResponse = false;
                Monitor.Enter(oAllSessions);//添加session时必须加排他锁
                oAllSessions.Add(oS);
                Monitor.Exit(oAllSessions);
                oS["X-AutoAuth"] = "(default)";
                oS.RequestHeaders["Accept-Encoding"] = "gzip, deflate";
                if ((oS.oRequest.pipeClient.LocalPort == iSecureEndpointPort) && (oS.hostname == sSecureEndpointHostname))
                {
                    oS.utilCreateResponseAndBypassServer();
                    oS.oResponse.headers.SetStatus(200, "Ok");
                    oS.oResponse["Content-Type"] = "text/html; charset=UTF-8";
                    oS.oResponse["Cache-Control"] = "private, max-age=0";
                    oS.utilSetResponseBody("<html><body>Request for httpS://" + sSecureEndpointHostname + ":" + iSecureEndpointPort.ToString() + " received. Your request was:<br /><plaintext>" + oS.oRequest.headers.ToString());
                }
            };
            Fiddler.FiddlerApplication.BeforeResponse += (oSession) =>
            {
               
            };
            //Kiwi：raw原生的，获得原生数据参数的事件。decompressed（解压缩）chunk（块），gracefully（优雅的地），invalid（无效的），EXACTLY（完全正确）,compatible（兼容的）,Decryption（解码）, E.g.例如，masquerading（伪装）
            Fiddler.FiddlerApplication.AfterSessionComplete += delegate(Fiddler.Session oS)
            {
                DataReceivedEventArgs_Kiwi.Instance.EnQueue(oS);
                if (oS != null)
                {
                    QueueSessions.Enqueue(oS);
                }
                //Console.WriteLine(oS.GetResponseBodyAsString());
                //if (oS.url.Contains("/search?"))
                {
                    //oS.utilGZIPResponse();
                    //oS.utilDecodeResponse();       //针对js可解析         
                    //oS.utilBZIP2Response();可能用于中文
                    // Console.WriteLine(oS.GetResponseBodyAsString());
                }
            };
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
            #endregion AttachEventListeners

            Fiddler.CONFIG.bHookAllConnections = true;
            Fiddler.CONFIG.IgnoreServerCertErrors = true;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;
            CreateAndTrustRoot();
            int iPort = 8877;//设置为0，程序自动选择可用端口
            writeThread.Start();
            Fiddler.FiddlerApplication.Startup(iPort, oFCSF);
            #region 日志系统
            FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", iPort);
            FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
            FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());
            #endregion

            Console.WriteLine("Hit CTRL+C to end session.");

            // We'll also create a HTTPS listener, useful for when FiddlerCore is masquerading（伪装） as a HTTPS server
            // instead of acting as a normal CERN-style proxy server.
            oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);
            if (null != oSecureEndpoint)
            {
                FiddlerApplication.Log.LogFormat("Created secure endpoint listening on port {0}, using a HTTPS certificate for '{1}'", iSecureEndpointPort, sSecureEndpointHostname);
            }

            bool bDone = false;
            do//使用的是do while
            {
                Console.WriteLine("\nEnter a command [C=Clear; L=List; G=Collect Garbage; R=read SAZ;\n\tS=Toggle Forgetful Streaming; T=Trust Root Certificate; Q=Quit]:");
                Console.Write(">");
                ConsoleKeyInfo cki = Console.ReadKey();
                Console.WriteLine();
                switch (Char.ToLower(cki.KeyChar))
                {
                    case 'c':
                        Monitor.Enter(oAllSessions);
                        oAllSessions.Clear();
                        Monitor.Exit(oAllSessions);
                        WriteCommandResponse("Clear...");
                        FiddlerApplication.Log.LogString("Cleared session list.");
                        break;

                    case 'd':
                        FiddlerApplication.Log.LogString("FiddlerApplication::Shutdown.");
                        FiddlerApplication.Shutdown();
                        break;

                    case 'l':
                        WriteSessionList(oAllSessions);//【Kiwi】
                        break;

                    case 'g':
                        Console.WriteLine("Working Set:\t" + Environment.WorkingSet.ToString("n0"));
                        Console.WriteLine("Begin GC...");
                        GC.Collect();
                        Console.WriteLine("GC Done.\nWorking Set:\t" + Environment.WorkingSet.ToString("n0"));
                        break;

                    case 'q':
                        bDone = true;
                        DoQuit();
                        break;

                    case 'r':
#if SAZ_SUPPORT
                        ReadSessions(oAllSessions);
#else
                        WriteCommandResponse("This demo was compiled without SAZ_SUPPORT defined");
#endif
                        break;

                    case 't':
                        try
                        {
                            WriteCommandResponse("Result: " + Fiddler.CertMaker.trustRootCert().ToString());
                        }
                        catch (Exception eX)
                        {
                            WriteCommandResponse("Failed: " + eX.ToString());
                        }
                        break;

                    // Forgetful streaming
                    case 's':
                        bool bForgetful = !FiddlerApplication.Prefs.GetBoolPref("fiddler.network.streaming.ForgetStreamedData", false);
                        FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.ForgetStreamedData", bForgetful);
                        Console.WriteLine(bForgetful ? "FiddlerCore will immediately dump streaming response data." : "FiddlerCore will keep a copy of streamed response data.");
                        break;

                }
            } while (!bDone);
        }

        private static bool CreateAndTrustRoot()
        {
            if (!Fiddler.CertMaker.rootCertExists())
            {
                var bCreatedRootCertificate = Fiddler.CertMaker.createRootCert();
                if (!bCreatedRootCertificate)
                {
                    return false;
                }
            }
            if (!Fiddler.CertMaker.rootCertIsTrusted())
            {
                var bTrustedRootCertificate = Fiddler.CertMaker.trustRootCert();
                if (!bTrustedRootCertificate)
                {
                    return false;
                }
            }
            return true;
        }
        private static void WriteSessionList(List<Fiddler.Session> oAllSessions)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Session list contains...");
            //放到一个try块中
            try
            {
                Monitor.Enter(oAllSessions);//Monitor Kiwi-?
                foreach (Session oS in oAllSessions)
                {
                    //MIME Type，资源的媒体类型
                    Console.Write(String.Format("{0} {1} {2}\n{3} {4}\n\n", oS.id, oS.oRequest.headers.HTTPMethod, Ellipsize(oS.fullUrl, 60), oS.responseCode, oS.oResponse.MIMEType));
                }
            }
            finally
            {
                Monitor.Exit(oAllSessions);
            }
            Console.WriteLine();
            Console.ForegroundColor = oldColor;
        }
        /// <summary>
        /// 超过长度时的显示方式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="iLen"></param>
        /// <returns></returns>
        private static string Ellipsize(string s, int iLen)
        {
            if (s.Length <= iLen) return s;
            return s.Substring(0, iLen - 3) + "...";
        }
        public static void WriteCommandResponse(string s)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(s);
            Console.ForegroundColor = oldColor;
        }

        #region 退出
        /// <summary>
        /// 退出程序
        /// </summary>
        public static void DoQuit()
        {
            WriteCommandResponse("Shutting down...");
            if (null != oSecureEndpoint) oSecureEndpoint.Dispose();
            Fiddler.FiddlerApplication.Shutdown();
            Thread.Sleep(500);
        }
        /// <summary>
        /// When the user hits CTRL+C, this event fires.  We use this to shut down and unregister our FiddlerCore.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            DoQuit();
        }
        #endregion

    }
}
