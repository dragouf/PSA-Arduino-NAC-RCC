using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace arduino_psa_diag
{
    public class MainForm : Form
    {
        private IContainer components = null;

        private string Version = "1.2.2";

        // private string AppPath = Application.StartupPath; 

        private string NacFile = "default.nac";

        // private string FullParamFilename = "full_param.json";

        private string JsonPath = "nac.json";

        private string NacKey = "D91C";

        private string RccKey = "F107";

        private string EcuCurrentKey = "";

        private string nacSerialHex = "";

        private string EcuCurrentType = "";

        private string NacCalibration = "FFFFFF";

        /// <summary>
        /// 1003 Open Diagnostic session
        /// </summary>
        private string OpenDiagCode = "1003";

        /// <summary>
        /// 1001	End of communication
        /// </summary>
        private string EndComCode = "1001";

        /// <summary>
        /// 2703	Unlocking service for configuration (Diagnostic session must be enabled first) - SEED
        /// </summary>
        private string UnlockingConfCode = "2703";

        private string CalibrationFilename = "";

        private string CalibrationSecondLine = "";

        private string LogFilePath = Application.StartupPath + "\\psa-arduino-nac.log";

        private string ComPortName;

        private Thread ThreadRebootCheck;

        private SerialPort SerialPortArduino;

        private bool IsNacReading = false;

        private bool isConnected = false;

        private bool isNacUnlocked = false;

        private bool needUnlocking = false;

        private bool IsConfLocked = false;

        private bool IsCalUploading = false;

        private bool IsZiErased = false;

        private bool isCalReceived = false;

        private string nextCommand = "";

        private int CurrentZoneIndex = 0;

        private int retryCount = 0;

        private int SentCalLineCount = 0;

        private string CurrentZoneKey = "";

        public Hashtable NacZoneValueHash = new Hashtable();

        public JObject JsonObject;

        private List<string> ZonesKeyDescriptionFlatList = new List<string>();

        private Hashtable NewZoneValueHash = new Hashtable();

        private ushort[] CalibrationData = new ushort[256] // m__00A0
		{
        0,
        4489,
        8978,
        12955,
        17956,
        22445,
        25910,
        29887,
        35912,
        40385,
        44890,
        48851,
        51820,
        56293,
        59774,
        63735,
        4225,
        264,
        13203,
        8730,
        22181,
        18220,
        30135,
        25662,
        40137,
        36160,
        49115,
        44626,
        56045,
        52068,
        63999,
        59510,
        8450,
        12427,
        528,
        5017,
        26406,
        30383,
        17460,
        21949,
        44362,
        48323,
        36440,
        40913,
        60270,
        64231,
        51324,
        55797,
        12675,
        8202,
        4753,
        792,
        30631,
        26158,
        21685,
        17724,
        48587,
        44098,
        40665,
        36688,
        64495,
        60006,
        55549,
        51572,
        16900,
        21389,
        24854,
        28831,
        1056,
        5545,
        10034,
        14011,
        52812,
        57285,
        60766,
        64727,
        34920,
        39393,
        43898,
        47859,
        21125,
        17164,
        29079,
        24606,
        5281,
        1320,
        14259,
        9786,
        57037,
        53060,
        64991,
        60502,
        39145,
        35168,
        48123,
        43634,
        25350,
        29327,
        16404,
        20893,
        9506,
        13483,
        1584,
        6073,
        61262,
        65223,
        52316,
        56789,
        43370,
        47331,
        35448,
        39921,
        29575,
        25102,
        20629,
        16668,
        13731,
        9258,
        5809,
        1848,
        65487,
        60998,
        56541,
        52564,
        47595,
        43106,
        39673,
        35696,
        33800,
        38273,
        42778,
        46739,
        49708,
        54181,
        57662,
        61623,
        2112,
        6601,
        11090,
        15067,
        20068,
        24557,
        28022,
        31999,
        38025,
        34048,
        47003,
        42514,
        53933,
        49956,
        61887,
        57398,
        6337,
        2376,
        15315,
        10842,
        24293,
        20332,
        32247,
        27774,
        42250,
        46211,
        34328,
        38801,
        58158,
        62119,
        49212,
        53685,
        10562,
        14539,
        2640,
        7129,
        28518,
        32495,
        19572,
        24061,
        46475,
        41986,
        38553,
        34576,
        62383,
        57894,
        53437,
        49460,
        14787,
        10314,
        6865,
        2904,
        32743,
        28270,
        23797,
        19836,
        50700,
        55173,
        58654,
        62615,
        32808,
        37281,
        41786,
        45747,
        19012,
        23501,
        26966,
        30943,
        3168,
        7657,
        12146,
        16123,
        54925,
        50948,
        62879,
        58390,
        37033,
        33056,
        46011,
        41522,
        23237,
        19276,
        31191,
        26718,
        7393,
        3432,
        16371,
        11898,
        59150,
        63111,
        50204,
        54677,
        41258,
        45219,
        33336,
        37809,
        27462,
        31439,
        18516,
        23005,
        11618,
        15595,
        3696,
        8185,
        63375,
        58886,
        54429,
        50452,
        45483,
        40994,
        37561,
        33584,
        31687,
        27214,
        22741,
        18780,
        15843,
        11370,
        7921,
        3960
        };

        private Label VersionLabel;

        private Label LabelCalibrationFileName;

        private Button ButtonCalibration;

        private TextBox TextBoxLog;

        private Button ButtonSend;

        private TextBox TextBoxSend;

        private Button ButtonSerial;

        private Button ButtonNac;

        private Button ButtonReadNac;

        private Label LabelStatus;

        private ProgressBar SendProgressBar;

        private Label LabelNac;

        private Button ButtonParams;

        private Button ButtonOfflineEditor;

        private Button ButtonCancel;

        private ComboBox ComboBoxCom;

        private Button ButtonBackup;

        private Button ButtonRestore;

        private Label LabelCalibrationId;

        public MainForm()
        {
            InitializeComponent();

            this.VersionLabel.Text = this.VersionLabel.Text + this.Version;

            this.Log("Opening Application..." + Environment.NewLine);
            /*try
            {
                if (File.Exists(this.LogFilePath))
                {
                    File.Delete(this.LogFilePath);
                }
            }
            catch (IOException)
            {
                MessageBox.Show(this, "Log file is not accessible (probably locked by another process)", "Error");
                // Application.Exit();
            }*/

            string[] portNames = SerialPort.GetPortNames();

            foreach (string item in portNames)
            {
                this.ComboBoxCom.Items.Add(item);
            }

            this.ComboBoxCom.SelectedIndex = checked(this.ComboBoxCom.Items.Count - 1);

            if (this.ComboBoxCom.Items.Count == 0)
            {
                // MessageBox.Show(this, "Error: No COM port found", "Error");
                this.ButtonSerial.Enabled = false;
                // Application.Exit();
            }

            string jsonText = File.ReadAllText(this.JsonPath);

            try
            {
                this.JsonObject = JsonConvert.DeserializeObject<JObject>(jsonText);
            }
            catch
            {
                MessageBox.Show(this, "Incorrect JSON Format", "Error");
                Application.Exit();
            }

            if (!File.Exists(Application.StartupPath + "\\" + this.NacFile))
            {
                return;
            }

            string zonesInline = "";

            foreach (JProperty zonesNode in this.JsonObject.Root["NAC"]["zones"])
            {
                zonesInline += zonesNode.Name + ";";
            }

            using (var streamReader = new StreamReader(Application.StartupPath + "\\" + this.NacFile))
            {
                string nacFileLine;
                while ((nacFileLine = streamReader.ReadLine()) != null)
                {
                    string[] keyvalue = nacFileLine.Split('=');
                    string defaultConfigKey = keyvalue[0];
                    string defaultConfigValue = keyvalue[1];
                    if (defaultConfigValue != "" && zonesInline.Contains(defaultConfigKey))
                    {
                        this.NacZoneValueHash.Add(defaultConfigKey, defaultConfigValue);
                    }
                }
            }

            if (this.NacZoneValueHash.Count > 0)
            {
                this.ButtonParams.Enabled = true;
                this.ButtonReadNac.Enabled = false;
            }
        }

        private void OnButtonSendClick(object sender, EventArgs args)
        {
            Invoke((Action)delegate
            {
                Send(this.TextBoxSend.Text);
            });
        }

        private void OnFormClosing(object sender, FormClosingEventArgs args)
        {
            if (this.SerialPortArduino != null && this.SerialPortArduino.IsOpen)
            {
                this.SerialPortArduino.Close();
            }

            this.Log("Closing Application..." + Environment.NewLine);

            // Application.Exit();
        }

        private void OnButtonSerialClick(object sender, EventArgs args)
        {
            this.ComPortName = this.ComboBoxCom.SelectedItem.ToString();
            if (this.ButtonSerial.Text == "Arduino Connect")
            {
                this.SerialPortArduino = new SerialPort(this.ComPortName, 115200, Parity.None, 8, StopBits.One)
                {
                    Handshake = Handshake.None,
                    ReadTimeout = 5000
                };

                this.SerialPortArduino.DataReceived += OnDataReceived;

                try
                {
                    this.SerialPortArduino.Open();
                    if (this.SerialPortArduino.IsOpen)
                    {
                        this.ButtonSerial.Text = "Arduino Disconnect";
                        this.Log("COM Port opened successfully" + Environment.NewLine);
                        this.ButtonNac.Enabled = true;
                        this.IsConfLocked = (this.IsCalUploading = (this.isNacUnlocked = (this.needUnlocking = false)));
                        Send("00");
                    }
                    else
                    {
                        MessageBox.Show(this, "Error: COM Port could not be opened", "Error", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error: COM port " + ex.Message, "Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                this.ButtonNac.Enabled = false;
                this.ButtonReadNac.Enabled = false;
                this.ButtonCalibration.Enabled = false;
                this.ButtonParams.Enabled = false;
                this.ButtonSend.Enabled = false;
                this.ButtonBackup.Enabled = false;
                this.ButtonRestore.Enabled = false;
                this.IsConfLocked = (this.IsCalUploading = (this.isNacUnlocked = (this.needUnlocking = false)));
                if (this.SerialPortArduino != null && this.SerialPortArduino.IsOpen)
                {
                    this.SerialPortArduino.Close();
                    this.ButtonSerial.Text = "Arduino Connect";
                    this.Log("COM port close" + Environment.NewLine);
                }
            }
        }

        private void OnButtonReadNacClick(object sender, EventArgs args)
        {
            this.ButtonNac.Enabled = false;
            this.CurrentZoneIndex = 0;

            Send(">" + "764" + ":" + "664");
            Send(this.OpenDiagCode);
        }

        private void OnButtonCalibrationUploadClick(object sender, EventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Calibration File (.cal)|*.cal",
                DefaultExt = ".cal",
                Multiselect = false
            };

            bool flag = false;
            int lineIndex = 0;
            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.IsCalUploading = true;
            this.IsZiErased = false;
            this.CalibrationFilename = openFileDialog.FileName;

            using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
            {
                string calibrationLine;
                while ((calibrationLine = streamReader.ReadLine()) != null)
                {
                    if (lineIndex == 1 && calibrationLine.Trim().StartsWith("S1100000") && calibrationLine.Trim().Contains(this.EcuCurrentKey))
                    {
                        this.CalibrationSecondLine = calibrationLine.Trim();
                        flag = true;
                    }
                    lineIndex++;
                }
            }

            if (!flag)
            {
                MessageBox.Show(this, "Calibration file invalid", "Error", MessageBoxButtons.OK);
                return;
            }

            FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

            if (MessageBox.Show(this, "Are you sure you want to flash the unit with " + fileInfo.Name + " ?", "", MessageBoxButtons.YesNo) != DialogResult.No)
            {
                this.LabelCalibrationFileName.Text = fileInfo.Name;
                this.SendProgressBar.Value = 0;
                this.SendProgressBar.Maximum = lineIndex;
                this.ButtonReadNac.Enabled = false;
                this.ButtonCalibration.Enabled = false;
                this.ButtonCancel.Enabled = false;
                this.ButtonNac.Enabled = false;

                Send(this.EndComCode);

                // 3101FF0081F05A	Empty flash memory (Unit must be unlocked first)
                this.nextCommand = "3101FF0081F05A";
            }
        }

        private void YieldSendCalibration()
        {
            string calibrationLineData = "";
            string calData = "";
            string calDataHex = "";
            int calibrationLineNumber = 0;

            if (this.SentCalLineCount <= 0)
            {
                return;
            }

            using (StreamReader streamReader = new StreamReader(this.CalibrationFilename))
            {
                checked
                {
                    while ((calibrationLineData = streamReader.ReadLine()) != null)
                    {
                        calibrationLineData = calibrationLineData.Trim();
                        if (calibrationLineNumber > 1)
                        {
                            if (calibrationLineData.StartsWith("S2"))
                            { // send
                                if (this.SentCalLineCount == calibrationLineNumber - 1)
                                {
                                    calData = calibrationLineData.Substring(10, calibrationLineData.Length - 10 - 2);
                                    calDataHex = unchecked("36" + (this.SentCalLineCount % 256).ToString("X2") + calibrationLineData.Substring(4, 6) + (calData.Length / 2).ToString("X2")) + calData;

                                    Send(calDataHex + this.CalibrationHash(calDataHex));
                                    break;
                                }
                            }
                            else if (calibrationLineData.StartsWith("S3"))
                            {
                                if (this.SentCalLineCount == calibrationLineNumber - 1)
                                {
                                    calData = calibrationLineData.Substring(12, calibrationLineData.Length - 12 - 2);
                                    calDataHex = unchecked("36" + (this.SentCalLineCount % 256).ToString("X2") + calibrationLineData.Substring(4, 8) + (calData.Length / 2).ToString("X2")) + calData;
                                    Send(calDataHex + this.CalibrationHash(calDataHex));
                                    break;
                                }
                            }
                            else
                            { // start
                                if (!calibrationLineData.StartsWith("S8") && !calibrationLineData.StartsWith("S7"))
                                {
                                    break;
                                }

                                this.SentCalLineCount = 0;

                                Invoke((Action)delegate
                                {
                                    this.SendProgressBar.Maximum = 10;
                                    this.SendProgressBar.Value = 0;
                                    this.LabelCalibrationFileName.Text = "";
                                });

                                // 37	Flash autocontrol (Unit must be unlocked first)
                                Send("37");
                            }
                        }
                        calibrationLineNumber++;
                    }
                }
            }
        }

        private void OnReadNacButtonClick(object sender, EventArgs args)
        {
            this.ButtonReadNac.Enabled = false;
            this.ButtonCalibration.Enabled = false;
            this.ButtonParams.Enabled = false;
            this.ButtonBackup.Enabled = false;
            this.ButtonRestore.Enabled = false;
            this.ButtonCancel.Enabled = true;
            int nbZones = ((JContainer)this.JsonObject.Root["NAC"]["zones"]).Count;
            this.SendProgressBar.Value = 0;
            this.SendProgressBar.Maximum = nbZones;
            this.LabelNac.Text = "0/" + nbZones;
            this.IsNacReading = true;
            this.CurrentZoneIndex = 0;
            this.NacZoneValueHash = new Hashtable();
            this.ZonesKeyDescriptionFlatList = new List<string>();

            // flatten zone
            foreach (JProperty zone in this.JsonObject["NAC"]["zones"].Children())
            {
                this.ZonesKeyDescriptionFlatList.Add(zone.Name + ";" + zone.Value["name"].ToString());
            }

            // start with first zone
            this.LabelStatus.Text = this.ZonesKeyDescriptionFlatList[0].ToString().Split(';')[1];
            this.CurrentZoneKey = this.ZonesKeyDescriptionFlatList[0].ToString().Split(';')[0];

            // 22XXXX	Read Zone XXXX (2 bytes)
            Send("22" + this.CurrentZoneKey);
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            string unprocessedData = "";
            string rcvData = this.SerialPortArduino.ReadLine();

            if (rcvData == null)
            {
                this.Log("Arduino Connection Failed" + Environment.NewLine);
                return;
            }

            unprocessedData += rcvData;

            // Empty response
            if (!rcvData.Contains("\r") || rcvData.Length == 0)
            {
                return;
            }

            rcvData = rcvData.Trim();

            // Log received data
            this.Log("< " + unprocessedData + Environment.NewLine + Environment.NewLine);            

            checked
            {
                if (rcvData.StartsWith("000000")) // connected to nac
                {
                    unprocessedData = "";
                    Invoke((Action)delegate
                    {
                        this.ButtonNac.Enabled = true;
                    });
                }
                else if (rcvData.StartsWith("5003") && this.IsConfLocked) // 5003XXXXXXXX	UDS Diagnostic session opened
                {
                    unprocessedData = "";
                    Send(this.UnlockingConfCode);
                    this.IsConfLocked = false;
                }
                else if (rcvData.StartsWith("50C0") && this.IsConfLocked) // KWP2000 50C0	Diagnostic session opened
                {
                    unprocessedData = "";
                    // 2783	Unlocking service for configuration (Diagnostic session must be enabled first) - SEED
                    Send("2783");
                    this.IsConfLocked = false;
                }
                else if (rcvData.StartsWith("5002")) // 5002XXXXXXXX	Download session opened
                {
                    unprocessedData = "";
                    // 2701	Unlocking service for download (Diagnostic session must be enabled first) - SEED
                    Send("2701");
                }
                else if ((rcvData.StartsWith("5001") || rcvData.StartsWith("7F277F")) && this.IsCalUploading) // 5001XXXXXXXX	Communication closed
                {
                    unprocessedData = "";

                    // 1002	Open Download session
                    Send("1002");
                    this.needUnlocking = true;
                }
                else if (rcvData.StartsWith("5001") || rcvData.StartsWith("7F277F")) // 5001XXXXXXXX	Communication closed // 7FXXYY	Error - XX = Service / YY = Error Number
                {
                    unprocessedData = "";
                    Send(this.OpenDiagCode);
                    this.needUnlocking = true;
                }
                else if (rcvData.StartsWith("7103FF0401"))
                {
                    unprocessedData = "";
                    Thread.Sleep(100);

                    // 3103FF04	Empty ZI Zone (Unit must be unlocked first)
                    Send("3103FF04");
                }
                else if (rcvData.StartsWith("7601") && this.IsZiErased)
                {
                    unprocessedData = "";
                    // 37	Flash autocontrol (Unit must be unlocked first)
                    Send("37");
                }
                else if (rcvData.StartsWith("76") && rcvData.EndsWith("02")) // 76XX02	Download frame XX injected with success
                {
                    unprocessedData = "";
                    unchecked
                    {
                        Invoke((Action)delegate
                        {
                            this.SendProgressBar.PerformStep();
                            int num = checked(this.SendProgressBar.Value * 100) / this.SendProgressBar.Maximum;
                            this.LabelNac.Text = num + "/100";
                            this.LabelStatus.Text = "Download in progress... " + num;
                        });
                    }

                    this.SentCalLineCount++;
                    this.YieldSendCalibration();
                }
                else if (rcvData.StartsWith("7101FF0001")) // 7101FF0001	Flash erased successfully
                {
                    unprocessedData = "";

                    // 3103FF00	Empty flash memory (Unit must be unlocked first)
                    Send("3103FF00");
                }
                else if (rcvData.StartsWith("7103FF0001"))
                {
                    unprocessedData = "";

                    // 3103FF00	Empty flash memory (Unit must be unlocked first)
                    Send("3103FF00");
                }
                else if (rcvData.StartsWith("7103FF0002")) // 7103FF0002	Flash erased successfully
                {
                    unprocessedData = "";

                    // 3481110000	Prepare flash writing (Unit must be unlocked first)
                    Send("3481110000");
                }
                else if (rcvData.StartsWith("741000") && !this.IsZiErased) // 741000	Download Writing ready
                {
                    unprocessedData = "";

                    this.SentCalLineCount = 1;
                    this.YieldSendCalibration();
                }
                else if (rcvData.StartsWith("7101FF0401")) // 7101FF0401	ZI erased successfully
                {
                    unprocessedData = "";
                    this.IsZiErased = true;

                    // 3103FF04	Empty ZI Zone (Unit must be unlocked first)
                    Send("3103FF04");
                }
                else if (rcvData.StartsWith("7103FF0402")) // 7103FF0402	ZI erased successfully
                {
                    unprocessedData = "";
                    // Prepare ZI zone writing (Unit must be unlocked first)
                    Send("3483110000");
                }
                else if (rcvData.StartsWith("741000") && this.IsZiErased) // 741000	Download Writing ready
                {
                    unprocessedData = "";

                    string calibrationResponse = this.CalibrationSecondLine.Substring(8, 4) +
                        "0000" +
                        this.CalibrationSecondLine.Substring(16, 4) +
                        "FFFFFF" +
                        this.CalibrationSecondLine.Substring(20, 8) +
                        "000000FDC7B7E301" +
                        this.CalibrationSecondLine.Substring(28, 6) +
                        "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF5C";

                    calibrationResponse = "3601" + calibrationResponse + this.CalibrationHash(calibrationResponse);

                    Send(calibrationResponse + this.CalibrationHash(calibrationResponse));
                }
                else if (rcvData.StartsWith("5003") && !this.needUnlocking) // 5003XXXXXXXX	Diagnostic session opened
                {
                    unprocessedData = "";
                    Invoke((Action)delegate
                    {
                        this.ButtonSerial.Text = "Arduino Disconnect";
                    });

                    // 22XXXX Read Zone XXXX(2 bytes)
                    // F0FE	ZI Zone (Last 6 characters: current calibration)
                    Send("22F0FE");
                }
                else if (rcvData.StartsWith("50C0") && !this.needUnlocking) // KWP2000  50C0	Diagnostic session opened
                {
                    unprocessedData = "";
                    Invoke((Action)delegate
                    {
                        this.ButtonSerial.Text = "Arduino Disconnect";
                    });

                    // 21XX	Read Zone XX (1 byte)
                    // FE	ZI Zone (Last 6 characters: current calibration)
                    Send("21FE");
                }
                else if (rcvData.StartsWith("5003") && this.needUnlocking) // Diagnostic session opened
                {
                    unprocessedData = "";
                    this.needUnlocking = false;

                    Send(this.UnlockingConfCode);
                }
                else if (rcvData.StartsWith("50C0") && this.needUnlocking) // KWP2000  50C0	Diagnostic session opened
                {
                    unprocessedData = "";
                    this.needUnlocking = false;
                    // 2783	Unlocking service for configuration (Diagnostic session must be enabled first) - SEED
                    Send("2783");
                }
                else if (rcvData.StartsWith("62F18C") && this.isCalReceived) // F18C	Serial number
                { // 62XXXXYYYYYYYYYYYY	Successfull read of Zone XXXX - YYYYYYYYYYYY = DATA
                    string rcvDataValue = rcvData.Substring(6);

                    for (int i = 0; i < rcvDataValue.Length; i += 2)
                    {
                        this.nacSerialHex += Convert.ToChar(byte.Parse(rcvDataValue.Substring(i, 2), NumberStyles.HexNumber));
                    }

                    if (this.nacSerialHex.Substring(0, 2) == "2D")
                    {
                        this.EcuCurrentType = "rcc";
                        this.EcuCurrentKey = this.RccKey;
                    }
                    else
                    {
                        this.EcuCurrentType = "nac";
                        this.EcuCurrentKey = this.NacKey;
                    }

                    this.isCalReceived = false;

                    Invoke((Action)delegate
                    {
                        this.ButtonCalibration.Enabled = true;
                        this.ButtonReadNac.Enabled = true;
                        this.ButtonSend.Enabled = true;
                        this.ButtonRestore.Enabled = true;
                    });
                }
                else if (rcvData.StartsWith("62F0FE")) // calibration received
                {
                    this.NacCalibration = rcvData.Substring(rcvData.Length - 6);

                    Invoke((Action)delegate
                    {
                        this.LabelCalibrationId.Text = "current Calibration: 96" + this.NacCalibration + "80";
                    });

                    this.isCalReceived = true;

                    //Read Zone XXXX (2 bytes)
                    Send("22F080");
                }
                else if (rcvData.StartsWith("62F080")) // 62XXXXYYYYYYYYYYYY	Successfull read of Zone XXXX - YYYYYYYYYYYY = DATA
                { // F080	ZA Zone
                    this.NacCalibration = rcvData.Substring(20, 10);

                    // 22XXXX	Read Zone XXXX (2 bytes)
                    // F18C Telecoding_Serial (SN)
                    Send("22F18C");
                }
                else if (rcvData.StartsWith("61FE")) // 61XXYYYYYYYYYYYY Successfull read of Zone XX -YYYYYYYYYYYY = DATA
                { // calibration received
                  // FE	ZI Zone (Last 6 characters: current calibration)
                    this.NacCalibration = rcvData.Substring(rcvData.Length - 6);
                    Invoke((Action)delegate
                    {
                        this.LabelCalibrationId.Text = "current Calibration: 96" + this.NacCalibration + "80";
                    });
                }
                else
                {
                    this.Log(rcvData + Environment.NewLine);

                    ProcessResponse(unprocessedData);

                    unprocessedData = "";

                    this.Log("-------------------------------------------------------------" + Environment.NewLine);
                }
            }
        }

        private void ProcessResponse(string rcvData)
        {
            if (this.CurrentZoneIndex == 1000)
            {
                this.IsConfLocked = (this.IsCalUploading = (this.isNacUnlocked = (this.needUnlocking = false)));

                this.Log("OPERATION CANCELED" + Environment.NewLine);

                return;
            }
            int currentZoneValueStart = 0;
            checked
            {
                if (rcvData.StartsWith("6703")) // 6703XXXXXXXX Seed generated for configuration - XXXXXXXX = SEED
                {
                    string ecuSeed = rcvData.Substring(4).Trim('\r');
                    string ecuKey = SeedKeyGenerator.getKey(ecuSeed, this.EcuCurrentKey);

                    if (ecuKey != "")
                    {
                        this.Log("Seed: " + ecuSeed + Environment.NewLine);

                        // 2704XXXXXXXX	Unlocking response for configuration - XXXXXXXX = KEY - Must be given within 5 seconds after seed generation
                        Send("2704" + ecuKey);

                        this.Log("Key: " + ecuKey + Environment.NewLine);
                    }
                    else
                    {
                        Invoke((Action)delegate
                        {
                            this.LabelNac.Text = "";
                            this.LabelStatus.Text = "NAC/RCC Unlocking failed !";
                            this.Log("Tracability OK" + Environment.NewLine);
                        });
                        return;
                    }
                }
                else if (rcvData.StartsWith("6701")) // 6701XXXXXXXX	Seed generated for download - XXXXXXXX = SEED
                {
                    string ecuSeed = rcvData.Substring(4).Trim('\r');
                    string ecuKey = SeedKeyGenerator.getKey(ecuSeed, this.EcuCurrentKey);

                    if (ecuKey != "")
                    {
                        this.Log("Seed: " + ecuSeed + Environment.NewLine);

                        // 2702XXXXXXXX	Unlocking response for download - XXXXXXXX = KEY - Must be given within 5 seconds after seed generation
                        Send("2702" + ecuKey);

                        this.Log("Key: " + ecuKey + Environment.NewLine);
                    }
                    else
                    {
                        Invoke((Action)delegate
                        {
                            this.LabelNac.Text = "";
                            this.LabelStatus.Text = "NAC/RCC Unlocking failed !";
                            this.Log("Tracability OK" + Environment.NewLine);
                        });

                        return;
                    }
                }
                else if (rcvData.StartsWith("6E2901") || rcvData.StartsWith("7F2E24")) // 6EXXXX	Successfull Configuration Write of Zone XXXX // 7F2EXX	Failed Configuration Write
                {
                    this.Log("Tracability OK" + Environment.NewLine);

                    // 1103	Reboot
                    Send("1103");

                    this.ThreadRebootCheck = new Thread(this.NacRebootCheck);
                    this.ThreadRebootCheck.Start();
                }
                else if (rcvData.StartsWith("54")) // 54	Faults cleared
                {
                    this.Log("Clearing faults OK" + Environment.NewLine);
                }
                else if (rcvData.StartsWith("77") && this.IsZiErased) // 77	Flash autocontrol OK
                {
                    this.Log("Flash autocontrol OK" + Environment.NewLine);

                    // 1103	Reboot
                    Send("1103");

                    this.ThreadRebootCheck = new Thread(this.NacRebootCheck);
                    this.ThreadRebootCheck.Start();
                }
                else if (rcvData.StartsWith("77")) // 77	Flash autocontrol OK
                {
                    Invoke((Action)delegate
                    {
                        this.LabelNac.Text = "";
                        this.LabelStatus.Text = "Flash autocontrol OK";
                        this.Log("Flash autocontrol OK" + Environment.NewLine);
                    });
                    Thread.Sleep(100);

                    // 3101FF04	Empty ZI Zone (Unit must be unlocked first)
                    Send("3101FF04");
                }
                else if (rcvData.StartsWith("7F2E78")) // 7F2E78	Configuration Write in progress
                {
                    this.Log("Writing in progress" + Environment.NewLine);
                }
                else if (rcvData.StartsWith("7F1012")) // 7FXXYY	Error - XX = Service / YY = Error Number
                {
                    this.OpenDiagCode = "10C0"; // OPEN DIAG SMEG CODE

                    Send(this.OpenDiagCode);

                    this.Log("Changing Diagnostic Session message" + Environment.NewLine);
                }
                else if (rcvData.StartsWith("7F3422")) // 7FXXYY	Error - XX = Service / YY = Error Number
                {
                    Invoke((Action)delegate
                    {
                        this.LabelStatus.Text = "ZI zone not writable... ";
                    });
                }
                else if (rcvData.StartsWith("7F3478")) // 7F3478	Download Writing in progress
                {
                    Invoke((Action)delegate
                    {
                        this.LabelStatus.Text = "Download in progress... ";
                    });
                }
                else if (rcvData.StartsWith("7F3778")) // 7F3778	Flash autocontrol in progress
                {
                    Invoke((Action)delegate
                    {
                        this.SendProgressBar.PerformStep();
                        this.LabelNac.Text = this.SendProgressBar.Value * 10 + "/" + this.SendProgressBar.Maximum * 10;
                        this.LabelStatus.Text = "Flash autocontrol in progress... " + this.SendProgressBar.Value * 10 + "%";
                        this.Log("Flash autocontrol in progress" + Environment.NewLine);
                    });
                }
                else if (rcvData.StartsWith("5103")) // 5103	Reboot OK
                {
                    this.Log("Reboot in progress\n" + Environment.NewLine);
                }
                else if (rcvData.StartsWith("7F2E7E") || rcvData.StartsWith("7F2E24")) // 7F2E7E	Failed Configuration Write - Unit is locked
                {
                    this.isNacUnlocked = false;
                }
                else if (rcvData.StartsWith("6704") || rcvData.StartsWith("6702"))
                // 6704	Unlocked successfully for configuration - Unit will be locked again if no command is issued within 5 seconds
                // 6702 Unlocked successfully for download - Unit will be locked again if no command is issued within 5 seconds
                {
                    this.isNacUnlocked = true;

                    this.Log("NAC/RCC Unlocked successfully" + Environment.NewLine);

                    this.CurrentZoneIndex = 0;

                    Send(this.nextCommand);

                    this.nextCommand = "";
                }
                else if (this.isConnected && this.isNacUnlocked && this.CurrentZoneIndex < this.NewZoneValueHash.Count)
                {
                    JContainer zonesJson = (JContainer)this.JsonObject["NAC"]["zones"];

                    // 6EXXXX	Successfull Configuration Write of Zone XXXX
                    if (!rcvData.StartsWith("6E"))
                    {
                        Invoke((Action)delegate
                        {
                            this.Log(this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString() + " : " + zonesJson[this.CurrentZoneKey]["name"].ToString() + ": Writing error" + Environment.NewLine);
                            this.SendProgressBar.Value = 0;
                            this.LabelStatus.Text = "Writing Error ! Invalid value on " + this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString();
                            this.ButtonReadNac.Enabled = false;
                            this.ButtonNac.Enabled = true;
                            this.ButtonCalibration.Enabled = false;
                            this.ButtonParams.Enabled = false;
                            this.ButtonCancel.Enabled = false;
                            this.ButtonBackup.Enabled = false;
                            this.ButtonRestore.Enabled = false;
                            this.SendProgressBar.Value = 0;
                            this.SendProgressBar.Maximum = 30;
                            this.LabelNac.Text = (this.LabelCalibrationId.Text = "");
                        });
                        this.CurrentZoneIndex = 0;
                        this.IsConfLocked = (this.IsCalUploading = (this.isNacUnlocked = (this.needUnlocking = false)));
                        return;
                    }

                    this.Log("Zone " + zonesJson[this.CurrentZoneKey]["name"].ToString() + " written successfully" + Environment.NewLine);

                    Invoke((Action)delegate
                    {
                        this.SendProgressBar.PerformStep();
                        this.LabelNac.Text = this.SendProgressBar.Value + "/" + this.SendProgressBar.Maximum;
                    });

                    this.CurrentZoneIndex++;
                    if (this.CurrentZoneIndex < this.NewZoneValueHash.Count)
                    {
                        this.CurrentZoneKey = this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString();
                        Invoke((Action)delegate
                        {
                            this.LabelStatus.Text = this.CurrentZoneKey + ": " + zonesJson[this.CurrentZoneKey]["name"].ToString();
                            // 2EXXXXYYYYYYYYYYYY	Write Zone XXXX with data YYYYYYYYYYYY (Unit must be unlocked first)
                            Send("2E" + this.CurrentZoneKey + this.NewZoneValueHash[this.CurrentZoneKey].ToString());
                        });
                        return;
                    }

                    // 2EXXXXYYYYYYYYYYYY	Write Zone XXXX with data YYYYYYYYYYYY (Unit must be unlocked first)
                    Send("2E2901FD000000010101");
                }

                if (!this.IsNacReading || this.CurrentZoneIndex >= ((JContainer)this.JsonObject["NAC"]["zones"]).Count)
                {
                    return;
                }

                string currentZoneKey = this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString().Split(';')[0];

                // 7F22XX	Failed Configuration Read
                if (rcvData.StartsWith("7F22"))
                { // retry
                  // 2133 Telecoding_Fct_WAVE3
                  // 2145 Telecoding_Fct_AIO
                    if (this.retryCount <= 3 && currentZoneKey != "2133" && currentZoneKey != "2145")
                    {
                        this.retryCount++;
                        this.CurrentZoneIndex--;
                        Thread.Sleep(500);

                        YieldZoneReading();
                        return;
                    }

                    this.retryCount = 0;

                    Invoke((Action)delegate
                    {
                        this.Log("Error reading zone " + currentZoneKey + " IGNORING" + Environment.NewLine);
                        this.SendProgressBar.PerformStep();
                        this.LabelNac.Text = this.SendProgressBar.Value + "/" + this.SendProgressBar.Maximum;
                        YieldZoneReading();
                    });
                }
                else
                {
                    this.retryCount = 0;
                    currentZoneValueStart = rcvData.IndexOf(currentZoneKey) + 4;
                    int currentZoneValueEnd = rcvData.IndexOf("\r", currentZoneValueStart);
                    string currentZoneValue = rcvData.Substring(currentZoneValueStart, currentZoneValueEnd - currentZoneValueStart).Trim();

                    Invoke((Action)delegate
                    {
                        this.Log("Zone Data: " + currentZoneValue + Environment.NewLine);
                        this.SendProgressBar.PerformStep();
                        this.LabelNac.Text = this.SendProgressBar.Value + "/" + this.SendProgressBar.Maximum;
                    });

                    // write value current nac config
                    this.NacZoneValueHash[currentZoneKey] = currentZoneValue;

                    YieldZoneReading();
                }
            }
        }

        private void YieldZoneReading()
        {
            checked
            {
                this.CurrentZoneIndex++;

                // zone reading continue
                if (this.CurrentZoneIndex < ((JContainer)this.JsonObject["NAC"]["zones"]).Count)
                {
                    Invoke((Action)delegate
                    {
                        this.LabelStatus.Text = this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString().Split(';')[1];
                        Thread.Sleep(100);

                        // 22XXXX	Read Zone XXXX (2 bytes)
                        Send("22" + this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString().Split(';')[0]);
                    });
                    return;
                }

                // zone reading end
                Invoke((Action)delegate
                {
                    this.LabelStatus.Text = "";
                    this.ButtonCalibration.Enabled = true;
                    this.ButtonReadNac.Enabled = true;
                    this.ButtonParams.Enabled = true;
                    this.ButtonBackup.Enabled = true;
                    this.ButtonRestore.Enabled = true;
                });

                this.IsNacReading = false;
                this.CurrentZoneIndex = 0;

                // write everything to log when ended
                this.Log("------------------------------------------------------" + Environment.NewLine);                
                foreach (DictionaryEntry item in this.NacZoneValueHash)
                {
                    this.Log(string.Concat(item.Key, "=", item.Value, Environment.NewLine));
                }
            }
        }

        private void NacRebootCheck()
        {
            int waitTime = 30;

            Invoke((Action)delegate
            {
                this.LabelStatus.Text = "NAC/RCC rebooting...";
                this.ButtonReadNac.Enabled = false;
                this.ButtonNac.Enabled = false;
                this.ButtonCalibration.Enabled = false;
                this.ButtonParams.Enabled = false;
                this.ButtonCancel.Enabled = false;
                this.ButtonBackup.Enabled = false;
                this.ButtonRestore.Enabled = false;
                this.SendProgressBar.Value = 0;
                this.SendProgressBar.Maximum = waitTime;
                string text3 = (this.LabelNac.Text = (this.LabelCalibrationId.Text = ""));
            });

            checked
            {
                int seconds;
                for (seconds = waitTime; seconds >= 0; seconds--)
                {
                    Invoke((Action)delegate
                    {
                        this.LabelStatus.Text = "NAC/RCC rebooting....  " + seconds + "s";
                        this.SendProgressBar.PerformStep();
                    });
                    Thread.Sleep(1000);
                }

                Send("14FFFFFF"); // 14FFFFFF	Clear faults

                Invoke((Action)delegate
                {
                    this.LabelStatus.Text = "NAC/RCC Up !";
                    this.ButtonNac.Enabled = true;
                });

                this.CurrentZoneIndex = 0;

                this.IsConfLocked = (this.IsCalUploading = (this.isNacUnlocked = (this.needUnlocking = false)));
            }
        }

        private void OnButtonOfflineEditorClick(object sender, EventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".nac",
                Multiselect = false,
                Filter = "PSA-Arduino-NAC sav (*.nac) | *.nac"
            };

            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            string zonesInline = "";

            foreach (JProperty zonesNode in this.JsonObject.Root["NAC"]["zones"])
            {
                zonesInline += zonesNode.Name + ";";
            }

            var keyValueHash = new Hashtable();

            using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
            {
                string backupFileLine;
                while ((backupFileLine = streamReader.ReadLine()) != null)
                {
                    string[] zoneKeyValue = backupFileLine.Trim().Split('=');
                    string zoneKey = zoneKeyValue[0];
                    string zoneValue = zoneKeyValue[1];

                    // F18C	Serial number
                    if (zoneValue != "" && zonesInline.Contains(zoneKey))
                    {
                        keyValueHash.Add(zoneKey, zoneValue);
                    }
                }
            }

            // open params form
            var form = new ParamsForm();
            form.InitForm(keyValueHash, this.JsonObject, true);
            DialogResult dialogResult = form.ShowDialog(this);

            var newKeyValueHash = form.UserKeyValueHash();

            // params form closed
            foreach (DictionaryEntry zoneKeyValue in newKeyValueHash)
            {
                keyValueHash[zoneKeyValue.Key] = zoneKeyValue.Value;
            }

            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            string zoneValueFlatList = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".nac",
                Filter = "PSA-Arduino-NAC sav (*.nac) | *.nac"
            };

            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false))
            {
                foreach (DictionaryEntry zone in keyValueHash)
                {
                    zoneValueFlatList = string.Concat(zoneValueFlatList, zone.Key, "=", zone.Value, ";");
                    streamWriter.WriteLine(string.Concat(zone.Key, "=", zone.Value));
                }
            }
        }

        private void OnButtonParamsClick(object sender, EventArgs args)
        {
            // open params form
            var form = new ParamsForm();
            form.InitForm(this.NacZoneValueHash, this.JsonObject, false);
            DialogResult dialogResult = form.ShowDialog(this);

            // params form closed
            this.NewZoneValueHash = form.UserKeyValueHash();

            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            // Write modified values to log
            this.Log(Environment.NewLine + "Modified values:" + Environment.NewLine);
            foreach (DictionaryEntry item in this.NacZoneValueHash)
            {
                if (this.NewZoneValueHash.ContainsKey(item.Key) && item.Value.ToString() != this.NewZoneValueHash[item.Key].ToString())
                {
                    this.Log(item.Key.ToString() + " : " + item.Value.ToString() + " => " + this.NewZoneValueHash[item.Key].ToString() + Environment.NewLine);
                }
            }
            this.ZonesKeyDescriptionFlatList.Clear();

            foreach (DictionaryEntry zone in this.NewZoneValueHash)
            {
                this.ZonesKeyDescriptionFlatList.Add(zone.Key as string);
            }

            this.isConnected = true;
            this.CurrentZoneIndex = 0;
            JContainer zonesJson = (JContainer)this.JsonObject["NAC"]["zones"];
            this.SendProgressBar.Maximum = this.NewZoneValueHash.Count;
            this.SendProgressBar.Value = 0;
            this.LabelNac.Text = "0/" + this.SendProgressBar.Maximum;
            this.CurrentZoneKey = this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString();
            this.LabelStatus.Text = this.CurrentZoneKey + ": " + zonesJson[this.CurrentZoneKey]["name"].ToString();
            this.ButtonCancel.Enabled = true;
            this.ButtonBackup.Enabled = false;
            this.ButtonRestore.Enabled = false;

            Send(this.OpenDiagCode);

            this.IsConfLocked = true;

            // 2EXXXXYYYYYYYYYYYY	Write Zone XXXX with data YYYYYYYYYYYY (Unit must be unlocked first)
            this.nextCommand = "2E" + this.CurrentZoneKey + this.NewZoneValueHash[this.CurrentZoneKey].ToString();
        }

        private void OnCancelButtonClick(object sender, EventArgs args)
        {
            this.CurrentZoneIndex = 1000;
            this.ButtonReadNac.Enabled = true;
            this.ButtonCalibration.Enabled = true;
            this.SendProgressBar.Value = 0;
            this.LabelNac.Text = this.LabelStatus.Text = "";
            this.IsConfLocked = (this.IsCalUploading = (this.isNacUnlocked = (this.needUnlocking = false)));
        }

        private void OnButtonBackupClick(object sender, EventArgs args)
        {
            string zoneValueFlatList = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".nac",
                Filter = "PSA-Arduino-NAC sav (*.nac) | *.nac"
            };

            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false))
            {
                foreach (DictionaryEntry zone in this.NacZoneValueHash)
                {
                    zoneValueFlatList = string.Concat(zoneValueFlatList, zone.Key, "=", zone.Value, ";");
                    streamWriter.WriteLine(string.Concat(zone.Key, "=", zone.Value));
                }
            }

            if (MessageBox.Show(this, "Do you want to send your backup to VLud for analysis and improvement ?", "", MessageBoxButtons.YesNo) != DialogResult.No)
            {
                // Telecoding_Fct_VIN
                string vinValue = this.NacZoneValueHash["F190"].ToString();
                string vinValueHex = "";

                for (int i = 0; i < vinValue.Length; i = checked(i + 2))
                {
                    vinValueHex += Convert.ToChar(byte.Parse(vinValue.Substring(i, 2), NumberStyles.HexNumber));
                }

                var zoneB64Encoded = WebUtility.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(zoneValueFlatList)));

                try
                {
                    this.PostUrl(
                        "https://vlud.net/backupAPI.php",
                        $"o=drgf&sn={this.nacSerialHex}&ut={this.EcuCurrentType}&uh={this.NacCalibration}&vin={vinValueHex}&cal={this.NacCalibration}&zones={zoneB64Encoded}",
                        "application/x-www-form-urlencoded",
                        "POST");
                }
                catch
                {
                }
            }
        }

        private void OnRestoreButtonClick(object sender, EventArgs args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".nac";
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "PSA-Arduino-NAC sav (*.nac) | *.nac";

            if (openFileDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            this.NewZoneValueHash.Clear();
            this.ZonesKeyDescriptionFlatList.Clear();

            using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
            {
                string backupFileLine;
                while ((backupFileLine = streamReader.ReadLine()) != null)
                {
                    string[] zoneKeyValue = backupFileLine.Trim().Split('=');
                    string zoneKey = zoneKeyValue[0];
                    string zoneValue = zoneKeyValue[1];

                    // F18C	Serial number
                    if (zoneKey != "F18C" && zoneValue != "")
                    {
                        this.ZonesKeyDescriptionFlatList.Add(zoneKey);
                        this.NewZoneValueHash.Add(zoneKey, zoneValue);
                    }
                }
            }

            this.isConnected = true;
            this.CurrentZoneIndex = 0;
            JContainer zonesJson = (JContainer)this.JsonObject["NAC"]["zones"];
            this.SendProgressBar.Maximum = this.NewZoneValueHash.Count;
            this.SendProgressBar.Value = 0;
            this.LabelNac.Text = "0/" + this.SendProgressBar.Maximum;
            this.CurrentZoneKey = this.ZonesKeyDescriptionFlatList[this.CurrentZoneIndex].ToString();
            this.LabelStatus.Text = this.CurrentZoneKey + ": " + zonesJson[this.CurrentZoneKey]["name"].ToString();
            this.ButtonCancel.Enabled = true;
            this.ButtonBackup.Enabled = false;
            this.ButtonRestore.Enabled = false;

            Send(this.OpenDiagCode);

            this.IsConfLocked = true;

            // 2EXXXXYYYYYYYYYYYY	Write Zone XXXX with data YYYYYYYYYYYY (Unit must be unlocked first)
            this.nextCommand = "2E" + this.CurrentZoneKey + this.NewZoneValueHash[this.CurrentZoneKey].ToString();
        }

        private void Send(string data)
        {
            this.SerialPortArduino.WriteLine(data);

            this.Log("> " + data + Environment.NewLine);
        }

        private byte[] CalibrationByteCalc(string P_0)
        {
            int length = P_0.Length;
            byte[] array = new byte[length / 2];
            for (int i = 0; i < length; i = checked(i + 2))
            {
                array[i / 2] = Convert.ToByte(P_0.Substring(i, 2), 16);
            }
            return array;
        }

        private string CalibrationHash(string P_0)
        {
            byte[] array = this.CalibrationByteCalc(P_0);
            int num = 65535;
            for (int i = 0; i < array.Length; i = checked(i + 1))
            {
                num = this.CalibrationData[(array[i] ^ num) & 0xFF] ^ ((num >> 8) & 0xFF);
            }
            num ^= 0xFFFF;
            string text = (num & 0xFFFF).ToString("X4");
            return text.Substring(2, 2) + text.Substring(0, 2);
        }

        private void OnLinkLabelClick(object P_0, LinkLabelLinkClickedEventArgs P_1)
        {
            Process.Start("https://www.forum-peugeot.com/Forum/threads/tuto-t%C3%A9l%C3%A9codage-et-calibration-dun-nac-ou-rcc-sans-diagbox-via-arduino.121767/");
        }

        public string FetchUrl(string Url)
        {
            ServicePointManager.ServerCertificateValidationCallback = (object P_0, X509Certificate P_1, X509Chain P_2, SslPolicyErrors P_3) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Proxy = null;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.UserAgent = "PSA-Arduino-NAC";
            try
            {
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                Invoke((Action)delegate
                {
                    MessageBox.Show(this, "Could not connect to remote server, please ensure you are connected to Internet");
                });
                return "";
            }
        }

        public string PostUrl(string url, string querystring, string encoding = "application/x-www-form-urlencoded", string method = "POST")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(querystring);
            ServicePointManager.ServerCertificateValidationCallback = (object P_0, X509Certificate P_1, X509Chain P_2, SslPolicyErrors P_3) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Proxy = null;
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.UserAgent = "PSA-Arduino-NAC";
            httpWebRequest.ContentLength = bytes.Length;
            httpWebRequest.ContentType = encoding;
            httpWebRequest.Method = method;
            try
            {
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (Stream stream2 = httpWebResponse.GetResponseStream())
                    {
                        using (StreamReader streamReader = new StreamReader(stream2))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                Invoke((Action)delegate
                {
                    MessageBox.Show(this, "Could not connect to remote server, please ensure you are connected to Internet");
                });
                return "";
            }
        }

        private void Log(string text, bool writeToFile = true)
        {
            if(writeToFile)
            {
                try
                {
                    File.AppendAllText(this.LogFilePath, text);
                }
                catch (IOException)
                {
                    this.Log("Log file is not accessible (probably locked by another process)", false);
                    // MessageBox.Show(this, "Log file is not accessible (probably locked by another process)", "Error");
                    // Application.Exit();
                }
            }

            if(this.TextBoxLog.InvokeRequired)
            {
                this.TextBoxLog.Invoke((MethodInvoker)delegate
                {
                    this.TextBoxLog.AppendText(text);
                });
            } 
            else
            {
                this.TextBoxLog.AppendText(text);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.TextBoxSend = new System.Windows.Forms.TextBox();
            this.ButtonSend = new System.Windows.Forms.Button();
            this.TextBoxLog = new System.Windows.Forms.TextBox();
            this.ButtonSerial = new System.Windows.Forms.Button();
            this.ButtonNac = new System.Windows.Forms.Button();
            this.ButtonReadNac = new System.Windows.Forms.Button();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.SendProgressBar = new System.Windows.Forms.ProgressBar();
            this.LabelNac = new System.Windows.Forms.Label();
            this.ButtonParams = new System.Windows.Forms.Button();
            this.ButtonOfflineEditor = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.ButtonCalibration = new System.Windows.Forms.Button();
            this.LabelCalibrationFileName = new System.Windows.Forms.Label();
            this.ComboBoxCom = new System.Windows.Forms.ComboBox();
            this.ButtonBackup = new System.Windows.Forms.Button();
            this.ButtonRestore = new System.Windows.Forms.Button();
            this.LabelCalibrationId = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TextBoxSend
            // 
            this.TextBoxSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxSend.Enabled = false;
            this.TextBoxSend.Location = new System.Drawing.Point(13, 305);
            this.TextBoxSend.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxSend.Name = "TextBoxSend";
            this.TextBoxSend.Size = new System.Drawing.Size(594, 23);
            this.TextBoxSend.TabIndex = 0;
            // 
            // ButtonSend
            // 
            this.ButtonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonSend.Enabled = false;
            this.ButtonSend.Location = new System.Drawing.Point(615, 305);
            this.ButtonSend.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonSend.Name = "ButtonSend";
            this.ButtonSend.Size = new System.Drawing.Size(88, 26);
            this.ButtonSend.TabIndex = 1;
            this.ButtonSend.Text = "Send";
            this.ButtonSend.UseVisualStyleBackColor = true;
            this.ButtonSend.Click += new System.EventHandler(this.OnButtonSendClick);
            // 
            // TextBoxLog
            // 
            this.TextBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxLog.Location = new System.Drawing.Point(13, 339);
            this.TextBoxLog.Margin = new System.Windows.Forms.Padding(4);
            this.TextBoxLog.Multiline = true;
            this.TextBoxLog.Name = "TextBoxLog";
            this.TextBoxLog.ReadOnly = true;
            this.TextBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxLog.Size = new System.Drawing.Size(690, 158);
            this.TextBoxLog.TabIndex = 2;
            // 
            // ButtonSerial
            // 
            this.ButtonSerial.Location = new System.Drawing.Point(144, 20);
            this.ButtonSerial.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonSerial.Name = "ButtonSerial";
            this.ButtonSerial.Size = new System.Drawing.Size(175, 26);
            this.ButtonSerial.TabIndex = 3;
            this.ButtonSerial.Text = "Arduino Connect";
            this.ButtonSerial.UseVisualStyleBackColor = true;
            this.ButtonSerial.Click += new System.EventHandler(this.OnButtonSerialClick);
            // 
            // ButtonNac
            // 
            this.ButtonNac.Enabled = false;
            this.ButtonNac.Location = new System.Drawing.Point(17, 53);
            this.ButtonNac.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonNac.Name = "ButtonNac";
            this.ButtonNac.Size = new System.Drawing.Size(175, 26);
            this.ButtonNac.TabIndex = 4;
            this.ButtonNac.Text = "NAC/RCC Access";
            this.ButtonNac.UseVisualStyleBackColor = true;
            this.ButtonNac.Click += new System.EventHandler(this.OnButtonReadNacClick);
            // 
            // ButtonReadNac
            // 
            this.ButtonReadNac.Enabled = false;
            this.ButtonReadNac.Location = new System.Drawing.Point(17, 119);
            this.ButtonReadNac.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonReadNac.Name = "ButtonReadNac";
            this.ButtonReadNac.Size = new System.Drawing.Size(175, 26);
            this.ButtonReadNac.TabIndex = 5;
            this.ButtonReadNac.Text = "Read Parameters";
            this.ButtonReadNac.UseVisualStyleBackColor = true;
            this.ButtonReadNac.Click += new System.EventHandler(this.OnReadNacButtonClick);
            // 
            // LabelStatus
            // 
            this.LabelStatus.Location = new System.Drawing.Point(17, 204);
            this.LabelStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.Size = new System.Drawing.Size(427, 26);
            this.LabelStatus.TabIndex = 6;
            // 
            // SendProgressBar
            // 
            this.SendProgressBar.Location = new System.Drawing.Point(17, 234);
            this.SendProgressBar.Margin = new System.Windows.Forms.Padding(4);
            this.SendProgressBar.Name = "SendProgressBar";
            this.SendProgressBar.Size = new System.Drawing.Size(585, 26);
            this.SendProgressBar.Step = 1;
            this.SendProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.SendProgressBar.TabIndex = 7;
            // 
            // LabelNac
            // 
            this.LabelNac.Location = new System.Drawing.Point(606, 234);
            this.LabelNac.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LabelNac.Name = "LabelNac";
            this.LabelNac.Size = new System.Drawing.Size(94, 26);
            this.LabelNac.TabIndex = 8;
            this.LabelNac.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButtonParams
            // 
            this.ButtonParams.Enabled = false;
            this.ButtonParams.Location = new System.Drawing.Point(524, 20);
            this.ButtonParams.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonParams.Name = "ButtonParams";
            this.ButtonParams.Size = new System.Drawing.Size(175, 26);
            this.ButtonParams.TabIndex = 9;
            this.ButtonParams.Text = "Parameters";
            this.ButtonParams.UseVisualStyleBackColor = true;
            this.ButtonParams.Click += new System.EventHandler(this.OnButtonParamsClick);
            // 
            // ButtonOfflineEditor
            // 
            this.ButtonOfflineEditor.Location = new System.Drawing.Point(524, 173);
            this.ButtonOfflineEditor.Margin = new System.Windows.Forms.Padding(4);
            this.ButtonOfflineEditor.Name = "ButtonOfflineEditor";
            this.ButtonOfflineEditor.Size = new System.Drawing.Size(175, 26);
            this.ButtonOfflineEditor.TabIndex = 14;
            this.ButtonOfflineEditor.Text = "Backup file Editor";
            this.ButtonOfflineEditor.UseVisualStyleBackColor = true;
            this.ButtonOfflineEditor.Click += new System.EventHandler(this.OnButtonOfflineEditorClick);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Enabled = false;
            this.ButtonCancel.Location = new System.Drawing.Point(524, 53);
            this.ButtonCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(175, 26);
            this.ButtonCancel.TabIndex = 10;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.OnCancelButtonClick);
            // 
            // ButtonCalibration
            // 
            this.ButtonCalibration.Enabled = false;
            this.ButtonCalibration.Location = new System.Drawing.Point(17, 86);
            this.ButtonCalibration.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ButtonCalibration.Name = "ButtonCalibration";
            this.ButtonCalibration.Size = new System.Drawing.Size(175, 26);
            this.ButtonCalibration.TabIndex = 11;
            this.ButtonCalibration.Text = "Calibration Upload";
            this.ButtonCalibration.UseVisualStyleBackColor = true;
            this.ButtonCalibration.Click += new System.EventHandler(this.OnButtonCalibrationUploadClick);
            // 
            // LabelCalibrationFileName
            // 
            this.LabelCalibrationFileName.Location = new System.Drawing.Point(207, 91);
            this.LabelCalibrationFileName.Name = "LabelCalibrationFileName";
            this.LabelCalibrationFileName.Size = new System.Drawing.Size(256, 22);
            this.LabelCalibrationFileName.TabIndex = 12;
            // 
            // ComboBoxCom
            // 
            this.ComboBoxCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxCom.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ComboBoxCom.FormattingEnabled = true;
            this.ComboBoxCom.Location = new System.Drawing.Point(17, 20);
            this.ComboBoxCom.Margin = new System.Windows.Forms.Padding(4);
            this.ComboBoxCom.Name = "ComboBoxCom";
            this.ComboBoxCom.Size = new System.Drawing.Size(120, 23);
            this.ComboBoxCom.TabIndex = 13;
            // 
            // ButtonBackup
            // 
            this.ButtonBackup.Enabled = false;
            this.ButtonBackup.Location = new System.Drawing.Point(524, 86);
            this.ButtonBackup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ButtonBackup.Name = "ButtonBackup";
            this.ButtonBackup.Size = new System.Drawing.Size(175, 26);
            this.ButtonBackup.TabIndex = 14;
            this.ButtonBackup.Text = "Backup Parameters";
            this.ButtonBackup.UseVisualStyleBackColor = true;
            this.ButtonBackup.Click += new System.EventHandler(this.OnButtonBackupClick);
            // 
            // ButtonRestore
            // 
            this.ButtonRestore.Enabled = false;
            this.ButtonRestore.Location = new System.Drawing.Point(524, 118);
            this.ButtonRestore.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ButtonRestore.Name = "ButtonRestore";
            this.ButtonRestore.Size = new System.Drawing.Size(175, 26);
            this.ButtonRestore.TabIndex = 15;
            this.ButtonRestore.Text = "Restore Parameters";
            this.ButtonRestore.UseVisualStyleBackColor = true;
            this.ButtonRestore.Click += new System.EventHandler(this.OnRestoreButtonClick);
            // 
            // LabelCalibrationId
            // 
            this.LabelCalibrationId.Location = new System.Drawing.Point(207, 122);
            this.LabelCalibrationId.Name = "LabelCalibrationId";
            this.LabelCalibrationId.Size = new System.Drawing.Size(256, 22);
            this.LabelCalibrationId.TabIndex = 16;
            this.LabelCalibrationId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // VersionLabel
            // 
            this.VersionLabel.Location = new System.Drawing.Point(17, 279);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(228, 22);
            this.VersionLabel.TabIndex = 18;
            this.VersionLabel.Text = "Version: ";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 510);
            this.Controls.Add(this.ButtonCalibration);
            this.Controls.Add(this.SendProgressBar);
            this.Controls.Add(this.ButtonReadNac);
            this.Controls.Add(this.ButtonRestore);
            this.Controls.Add(this.ButtonBackup);
            this.Controls.Add(this.TextBoxLog);
            this.Controls.Add(this.TextBoxSend);
            this.Controls.Add(this.ComboBoxCom);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonParams);
            this.Controls.Add(this.ButtonOfflineEditor);
            this.Controls.Add(this.ButtonNac);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.ButtonSerial);
            this.Controls.Add(this.LabelCalibrationId);
            this.Controls.Add(this.LabelNac);
            this.Controls.Add(this.LabelStatus);
            this.Controls.Add(this.LabelCalibrationFileName);
            this.Controls.Add(this.ButtonSend);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "PSA-Arduino-NAC/RCC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}