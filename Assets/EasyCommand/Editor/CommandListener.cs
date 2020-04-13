using UnityEngine;
using UnityEditor;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Threading;
using System;

namespace EasyCommand
{
    public class CommandListener
    {
        private HttpListener listener;
        private Thread linstenThread;
        private Thread handleThread;
        private bool isWorking = false;
        private object lockObject = new object();
        /// <summary>
        /// 保存未执行的请求队列，执行时就放入HandleList中
        /// </summary>
        private Queue<Command> requestQueue = new Queue<Command>();
        /// <summary>
        /// 执行中，执行完毕的请求队列，用于记录状态，以后查询用
        /// </summary>
        private List<Command> handleList = new List<Command>();
        
        /// <summary>
        /// 开始侦听，一个线程用来侦听请求，一个线程用来清理完成的请求
        /// </summary>
        public void Start()
        {
            Stop();

            linstenThread = new Thread(RequestLoop);
            linstenThread.Start();
            handleThread = new Thread(HandleLoop);
            handleThread.Start();

            // Debug.Log("Listening...");
            isWorking = true;
        }

        /// <summary>
        /// 停止侦听，销毁所有
        /// </summary>
        public void Stop()
        {
            listener?.Stop();
            listener = null;
            linstenThread?.Abort();
            linstenThread = null;
            handleThread?.Abort();
            handleThread = null;

            isWorking = false;

            lock (lockObject)
            {
                requestQueue.Clear();
                handleList.Clear();
            }
        }

        /// <summary>
        /// 是否在工作中
        /// </summary>
        /// <returns>true/false</returns>
        public bool IsWorking()
        {
            return isWorking;
        }

        /// <summary>
        /// 获取一个命令请求，获取后就从队列中移除
        /// </summary>
        /// <returns>Command</returns>
        public Command GetCommand()
        {
            lock (lockObject)
            {
                if (requestQueue.Count > 0)
                {
                    Command cmdData = requestQueue.Dequeue();
                    // 获得的请求，就认为在执行中了，放入handle队列
                    handleList.Add(cmdData);
                    return cmdData;
                }
            }
            return null;
        }

        /// <summary>
        /// 循环获取Http请求
        /// </summary>
        private async void RequestLoop()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(CommandConfig.LintenAddress);
            listener.Start();

            while (true)
            {
                var context = await listener.GetContextAsync();

                // TODO 判断GET为监控以及查询，POST为命令
                try
                {
                    Command cmdData = new Command(context);
                    if (cmdData.IsLive())
                    {
                        lock (lockObject)
                        {
                            requestQueue.Enqueue(cmdData);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("获得消息出错：" + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// 循环清理执行完成的命令
        /// </summary>
        private void HandleLoop()
        {
            while (true)
            {
                lock (lockObject)
                {
                    for (int index = handleList.Count - 1; index >= 0; index--)
                    {
                        Command cmdData = handleList[index];
                        // 只要是不可用的都移除
                        if (!cmdData.IsLive())
                        {
                            handleList.RemoveAt(index);
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 测试命令
        /// </summary>
        public void TestCommand()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            data.Add("orderId", "10000001");
            data.Add("type", "compile");
            data.Add("logFile", "xxxxxxx");

            string jsonParam = MiniJSON.Json.Serialize(data);
            Debug.Log("[发出测试命令]=" + jsonParam);
            byte[] body = Encoding.UTF8.GetBytes(jsonParam);
            UnityWebRequest request = new UnityWebRequest(CommandConfig.LintenAddress, "POST");
            request.uploadHandler = new UploadHandlerRaw(body);
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            request.downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequestAsyncOperation requestAsync = request.SendWebRequest();
            requestAsync.completed += (option) =>
            {
                Debug.Log("[收到测试命令]=" + request.downloadHandler.text);
            };
        }
    }
}
