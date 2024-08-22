# CGpu OSC: CPU と GPU の温度を取得し VirtualCast へ送信するプログラム
このプログラムは、CPU と GPU の温度を取得し、Open Sound Control (OSC) プロトコルを使用して VirtualCast へ送信します。また、別途公開中の「CGpu 温度 (OSC) CGpu Temperatures」は、受信したデータを表示する VCI です。これらを連携させることにより、VirtualCast 内で CPU と GPU の温度が確認できるようになります。  

## 動作環境
- VirtualCast との連携を前提に作成されています。  
- [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) の DLL ファイルを利用しています。  
   - ※再頒布が認められている MPL 2.0 ライセンスのもとで、配布用 zip ファイルに DLL ファイルを同梱します（2024年8月22日時点 v0.9.3）。  

   LibreHardwareMonitorLib.dll で CPU の「Core Max」と GPU の「Hot Spot」の値が取得できることが前提です。ハードウェア構成によってはこれらの値が取得できない場合があります。  

こちらで確認できたハードウェアは以下の通りです。  

   - CPU: 13th Gen Intel(R) Core(TM) i9-13900HX  
     GPU: NVIDIA GeForce RTX 4070 Laptop GPU  

   - CPU: 8th Gen Intel(R) Core(TM) i7-8750H  
     GPU: NVIDIA GeForce GTX 1070 with Max-Q Design  


## インストール
1. 「CGpuOSC.zip」 をダウンロードし、解凍します（またはソースコードから自分でコンパイルしてください）。  
2. ウィルス対策ソフト等でファイルをスキャンし、問題がないことを確認します。  
3. 「CGpuOSC.exe」「LibreHardwareMonitorLib.dll」を右クリックし、プロパティのセキュリティ項目で「許可する」にチェックを入れます。  
   - 右クリック→プロパティ→セキュリティ:このファイルは…☑許可する(K)  
4. 「CGpuOSC.exe」を**管理者権限**で実行します（設定ファイル等はありません）。  

## アンインストール
1. 「CGpuOSC.exe」「LibreHardwareMonitorLib.dll」を削除します。   

## VirtualCast との連携
VCI と連携させるには、VirtualCast のタイトル画面で「VCI」メニュー内の「OSC受信機能」を「creator-only」または「enabled」に設定してください。  

## VCIの取得
「CGpu 温度 (OSC) CGpu temperatures」は [VirtualCastで公開中の商品ページ](https://virtualcast.jp/users/100215#products) から取得できます。

## 使用方法
1. VirtualCast 内で VCI「CGpu 温度 (OSC) CGpu temperatures」を出しておきます。  
2. CGpuOSC を起動し、ドロップダウンリストから更新間隔（秒）を選択します。  
3. 停止する時は「0」を選択します。  

## プログラムの挙動
1. ドロップダウンリストから更新間隔（秒）を選択すると指定間隔で CPU と GPU の温度を整数値で取得します。  
   - 対象は「Core Max」と「GPU Hot Spot」です。  

   ```C#:Form1.cs
   if (sensor.Name == "Core Max")
   {
       var value = sensor.Value;
       if (value.HasValue)
       {
           cpuTemperature = Convert.ToInt32(value.Value);
       }
   }
   else if (sensor.Name == "GPU Hot Spot")
   {
       var value = sensor.Value;
       if (value.HasValue)
       {
           gpuTemperature = Convert.ToInt32(value.Value);
       }
   }
   ```

2. 指定秒間隔で、以下のように OSC でデータを送信します。  
   2-1. OSC でバージョン情報を送信    

   ```C#:Form1.cs
   UDPSender("127.0.0.1", 19100);
   OscMessage("/Taki/CGpuTemp/version", blob_version);
   ```
   - 間隔指定時に一度だけ送信します。  

   2-2. OSC で CPU と GPU の温度を送信  

   ```C#:Form1.cs
   UDPSender("127.0.0.1", 19100);
   OscMessage("/Taki/CGpuTemp/temperatures", int_cpuTemperature, int_gpuTemperature);
   ```
   - 指定秒毎に送信します。  

## ライセンス
このプログラムは、Mozilla Public License 2.0 (MPL 2.0) でライセンスされた DLLファイルを利用しています。  
プログラム自体も、Mozilla Public License 2.0 (MPL 2.0) ライセンスのもとで公開します。  

DLL のソースコードは、以下の GitHub リポジトリで入手可能です:  
[LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)  

DLL に変更はありません。 MPL 2.0 の条件に従い、配布用 ZIP ファイルにライセンスのコピーを同梱しています。  

# CGpu OSC: A Program to Retrieve CPU and GPU Temperatures and Send Them to VirtualCast
This program retrieves CPU and GPU temperatures and sends them to VirtualCast using the Open Sound Control (OSC) protocol. Additionally, the separately released "CGpu Temperatures" is a VCI that displays the received data. By linking these, you can monitor CPU and GPU temperatures within VirtualCast. 

## System Requirements
- Designed for integration with VirtualCast.  
- Utilizes DLL files from [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor).  
   - Note: The DLL files included in the distribution ZIP are licensed under MPL 2.0, which permits redistribution (as of version 0.9.3, dated August 22, 2024).  

   The program assumes that LibreHardwareMonitorLib.dll can retrieve CPU's "Core Max" and GPU's "Hot Spot" values. Depending on your hardware configuration, these values may not be available.  

The hardware tested includes:  

   - CPU: 13th Gen Intel(R) Core(TM) i9-13900HX  
     GPU: NVIDIA GeForce RTX 4070 Laptop GPU  

   - CPU: 8th Gen Intel(R) Core(TM) i7-8750H  
     GPU: NVIDIA GeForce GTX 1070 with Max-Q Design  


## Installation
1. Download and extract the "CGpuOSC.zip" file (or compile from source code yourself).  
2. Scan the files with antivirus software to ensure they are safe.  
3. Right-click "CGpuOSC.exe" and "LibreHardwareMonitorLib.dll", and check the "Allow" box in the properties' security tab.  
   - Right-click → Properties → Security: This file is… ☑ Allow (K)  
4. Run "CGpuOSC.exe" with **administrator privileges** (no configuration files are needed).  

## Uninstallation
1. Delete "CGpuOSC.exe" and "LibreHardwareMonitorLib.dll".   

## Integration with VirtualCast
To link with a VCI, set the "OSC Receiver Function" to "creator-only" or "enabled" in the "VCI" menu on the VirtualCast title screen.  

## Obtaining VCI
The "(OSC) CGpu temperatures" can be obtained from the [products page on VirtualCast](https://virtualcast.jp/users/100215#products).  

## Usage
1. Make the VCI "(OSC) CGpu temperatures" appear within VirtualCast.  
2. Launch CGpuOSC and select the update interval (in seconds) from the dropdown list.  
3. To stop, select "0".  

## Program Behavior
1. Selecting an update interval from the dropdown list causes the program to retrieve CPU and GPU temperatures as integer values at the specified interval.  
   - Targets are "Core Max" and "GPU Hot Spot".  

   ```C#:Form1.cs
   if (sensor.Name == "Core Max")
   {
       var value = sensor.Value;
       if (value.HasValue)
       {
           cpuTemperature = Convert.ToInt32(value.Value);
       }
   }
   else if (sensor.Name == "GPU Hot Spot")
   {
       var value = sensor.Value;
       if (value.HasValue)
       {
           gpuTemperature = Convert.ToInt32(value.Value);
       }
   }
   ```

2. This program sends data via OSC at the specified interval.  
   2-1. Send version information via OSC    

   ```C#:Form1.cs
   UDPSender("127.0.0.1", 19100);
   OscMessage("/Taki/CGpuTemp/version", blob_version);
   ```
   - Send only once when the interval is set.  

   2-2. Send CPU and GPU temperatures via OSC  

   ```C#:Form1.cs
   UDPSender("127.0.0.1", 19100);
   OscMessage("/Taki/CGpuTemp/temperatures", int_cpuTemperature, int_gpuTemperature);
   ```
   - Send at the specified interval.  

## License
This program uses DLL files licensed under the Mozilla Public License 2.0 (MPL 2.0). The program itself is also released under the Mozilla Public License 2.0 (MPL 2.0).  

The source code for the DLL can be obtained from the following GitHub repository:  
[LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)  

There are no changes to the DLL. In accordance with MPL 2.0, a copy of the license will be included in the distribution ZIP file.  
