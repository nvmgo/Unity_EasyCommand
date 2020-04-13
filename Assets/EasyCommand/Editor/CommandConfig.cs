using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

namespace EasyCommand
{
    public class CommandConfig
    {
        /// <summary>
        /// 侦听的地址
        /// </summary>
        public static string LintenAddress = "http://127.0.0.1:8080/";

        /// <summary>
        /// 收到命令后的回调
        /// 修改此处执行自己的脚本
        /// </summary>
        public static Action<Command> OnCommandCallback = (cmd) =>
        {
            Debug.Log("收到命令，参数如下：");
            foreach (var item in cmd.RequestData)
            {
                Debug.Log(string.Format("[{0}]={1}", item.Key, item.Value.ToString()));
            }

            // 在此调用自己的脚本
            Debug.Log("调用脚本1");
            Debug.Log("调用脚本2");
            Debug.Log("调用脚本3");
            Debug.Log("调用脚本...");

            // 调用成功返回数据
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("code", 0);
            data.Add("message", "success");
            data.Add("data", new Dictionary<string, object>());
            cmd.Response(data);
        };

    }
}
