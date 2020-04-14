# Unity_EasyCommand
 
通过Http请求调用Unity写的脚本，不重启Unity，只需要一个Http POST请求就可以

代替命令行方式调用Unity方法：`C:\program files\Unity\Editor\Unity.exe -quit -batchmode -executeMethod MyEditorScript.MyMethod`

### 使用

1. 将EasyCommand文件夹复制到项目工程Assets文件夹下
2. 修改`CommandConfig.OnCommandCallback`，调用自己的脚本
3. 通过Unity菜单EasyCommand->Run打开EasyCommand窗口，点击按钮[开始运行]
4. 然后就可以通过Http发送命令了，目前只接收POST请求，格式为JSON

### 示例

1. 启动EasyCommand
2. 点击窗口中的按钮[测试]，或者运行运行test/test.py