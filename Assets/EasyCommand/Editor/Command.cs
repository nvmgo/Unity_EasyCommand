
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace EasyCommand
{
    public class Command
    {
        // 请求的数据，目前只支持POST请求
        public Dictionary<string, object> RequestData { get; private set; }
        // 请求的context
        private HttpListenerContext Context;

        /// <summary>
        /// 内部对请求参数进行解析
        /// </summary>
        /// <param name="context">请求的contex</param>
        public Command(HttpListenerContext context)
        {
            try
            {
                StreamReader sr = new StreamReader(context.Request.InputStream);
                string postData = sr.ReadToEnd();
                // Debug.Log("获得POST消息：" + postData);
                sr.Close();
                RequestData = MiniJSON.Json.Deserialize(postData) as Dictionary<string, object>;
                // 只有数据解析正确的时候才会设置Context
                Context = context;
            }
            catch (Exception e)
            {
                Debug.LogError("解析消息出错：" + e.StackTrace);
            }
        }

        /// <summary>
        /// 命令执行完毕，相应Http请求
        /// </summary>
        /// <param name="data">返回的参数</param>
        /// <returns>返回是否成功，只能调用一次，后续再调用会失败</returns>
        public bool Response(Dictionary<string, object> data)
        {
            if (!IsLive())
            {
                Debug.LogError("Response函数只能调用一次");
                return false;
            }

            try
            {
                string respJson = MiniJSON.Json.Serialize(data);
                Debug.Log("返回消息：" + respJson);
                StreamWriter sw = new StreamWriter(Context.Response.OutputStream);
                sw.Write(respJson);
                sw.Close();
                Context.Response.Close();
                // 相应完成后，情况Context，以后不能再调用
                Context = null;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("相应Http请求失败：" + e.StackTrace);
                Context = null;
                return false;
            }
        }

        /// <summary>
        /// 是否可用，request参数解析失败，或者已经响应过Http请求，就标为不可用了
        /// </summary>
        /// <returns></returns>
        public bool IsLive()
        {
            return Context != null;
        }

        //private string GetPostData()
        //{
        //    ////HttpListenerContext httpListenerContext = listener.GetContext();
        //    ////httpListenerContext.Response.StatusCode = 200;
        //    ////String method = httpListenerContext.Request.HttpMethod;
        //    ////if (method.Trim().ToUpper().Equals("POST"))
        //    ////{
        //    ////    doPost(httpListenerContext);
        //    ////}
        //    ////else if (method.Trim().ToUpper().Equals("GET"))
        //    ////{
        //    ////    doGet(httpListenerContext);
        //    ////}


        //}
    }
}
