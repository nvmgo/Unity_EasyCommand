using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using EasyCommand;

namespace EasyCommand
{
    public class CommandWindow : EditorWindow
    {
        [MenuItem("EasyCommand/Run")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CommandWindow)).titleContent.text = "EasyCommand";
        }

        private CommandListener commandListener = new CommandListener();
        private DateTime previousTimeSinceStartup;

        private void OnEnable()
        {
            previousTimeSinceStartup = DateTime.Now;
        }

        void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.Label("当前运行状态为：" + (commandListener.IsWorking() ? "运行中" : "停止"));
            GUILayout.Space(5);

            if (GUILayout.Button("开始运行", GUILayout.Height(50)))
            {
                commandListener.Start();
                Debug.Log("脚本启动了!!!");
            }

            if (GUILayout.Button("停止运行", GUILayout.Height(50)))
            {
                commandListener.Stop();
                Debug.Log("脚本停止了!!!");
            }

            GUILayout.Space(5);
            if (GUILayout.Button("测试", GUILayout.Height(30), GUILayout.Width(80)))
            {
                commandListener.TestCommand();
            }
        }

        void Update()
        {
            float deltaTime = (float)(DateTime.Now.Subtract(previousTimeSinceStartup).TotalMilliseconds / 1000.0f);
            if (deltaTime >= 0.1)
            {
                previousTimeSinceStartup = DateTime.Now;

                Command cmd = commandListener.GetCommand();
                if (cmd != null)
                {
                    CommandConfig.OnCommandCallback.Invoke(cmd);
                }
            }
        }

    }
}
