using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;

using hvppleDotNet;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;


namespace dhVision
{

    #region public struct PointA

    public struct PointA
    {
        private float x;
        private float y;
        private float angle;

        public PointA(float value_x, float value_y, float value_angle)
        {
            this.x = value_x;
            this.y = value_y;
            this.angle = value_angle;
        }
        public PointA(PointF point, float value_angle)
        {
            this.x = point.X;
            this.y = point.Y;
            this.angle = value_angle;
        }
        public float X
        {
            get { return this.x; }
            set { this.x = value; }
        }
        public float Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        /// <summary>
        /// 角度，单位是角度
        /// </summary>
        public float Angle
        {
            get { return this.angle; }
            set { this.angle = value; }
        }

        /// <summary>
        /// 角度，单位是弧度
        /// </summary>
        public float Angle_rad
        {
            get { return (float)(this.angle / 180d * Math.PI); }
            set { this.angle = (float)(value / Math.PI * 180d); }
        }

        public PointF PointF
        {
            get { return new PointF(x,y); }
            set { this.x = value.X; this.y = value.Y; }
        }
    }
    #endregion

    #region *** 根据点的坐标对PointF[]排序 ***

    public class comparisonPointF
    {
        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>System.Int32.</returns>
        public static int CompareToX(PointF Index, PointF other)
        {
            if (other == null) return -1;
            if (Index.X > other.X)
            {
                return 1;
            }
            if (Index.X == other.X)
            {
                return 0;
            }
            return -1;
        }

        public static int CompareToY(PointF Index, PointF other)
        {
            if (other == null) return -1;
            if (Index.Y > other.Y)
            {
                return 1;
            }
            if (Index.Y == other.Y)
            {
                return 0;
            }
            return -1;
        }

    }

    #endregion

    #region *** 根据点的坐标对RectangleF[]排序 ***

    public class comparisonRectangleF
    {
        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>System.Int32.</returns>
        public static int CompareToX(RectangleF Index, RectangleF other)
        {
            if (other == null) return -1;
            if (Index.X > other.X)
            {
                return 1;
            }
            if (Index.X == other.X)
            {
                return 0;
            }
            return -1;
        }

        public static int CompareToY(RectangleF Index, RectangleF other)
        {
            if (other == null) return -1;
            if (Index.Y > other.Y)
            {
                return 1;
            }
            if (Index.Y == other.Y)
            {
                return 0;
            }
            return -1;
        }

    }

    #endregion




    class ParamHvpple
    {
        //**************************************************************************************************
        
        #region  *** 2017.7.14 改版后所有的句柄都放在一个数组 ***

        public static HTuple[] hvAllHandles = new HTuple[150];

        public static int syGetNullHandleIndex()
        {
            int iHtupleIndex = -1;
            for (int ihh = 0; ihh < ParamHvpple.hvAllHandles.Length; ihh++)
            {
                if (ParamHvpple.hvAllHandles[ihh] == null)
                {
                    iHtupleIndex = ihh;
                    break;
                }
            }

            return iHtupleIndex;
        }

        #endregion


        #region  ****** 初始化定位参数 和 标定参数 和 一维码 二维码 彩色图片的神经网络读取 ******


        public static paramLibGlobal configLibGlobal = new paramLibGlobal();

        [Serializable()]
        public class paramLibGlobal
        {
            #region  ***  2017.11.28 改过之后的全局参数类  ***


            public Dictionary<string, List<string>> dictTCP = new Dictionary<string, List<string>>();

            public Dictionary<string, List<string>> dictRS232 = new Dictionary<string, List<string>>();


         
            public Dictionary<string, List<string>> dictIO = new Dictionary<string, List<string>>();



            public Dictionary<string, List<string>> dictFirstPiece = new Dictionary<string, List<string>>();

   
            public Dictionary<string, List<string>> dictPLC = new Dictionary<string, List<string>>();

       
            public List<string> lstrLaser = new List<string>();

            public List<string> lstrServer = new List<string>();


            public List<string> lstrAdvanced = new List<string>();

 
            public List<string> lstrStatistic = new List<string>();

            public List<string[]> lstrWholemap = new List<string[]>();

   
            public Dictionary<string, List<string>> dictDrawing = new Dictionary<string, List<string>>();

            public List<string> lstrModbus = new List<string>();

            public List<string> lstrDetails = new List<string>();

            /// <summary>
            /// 机械手参数
            /// </summary>
            public List<string> lstrMachineHand = new List<string>();

            /// <summary>
            /// EXCEL文件保存设定
            /// </summary>
            public List<string> lstrExcel = new List<string>();

            /// <summary>
            /// 检查DLL的版本号
            /// </summary>
            public List<string> lstrCheckDll = new List<string>();

            /// <summary>
            /// AI库
            /// </summary>
            public Dictionary<string, IntPtr> dictAI = new Dictionary<string, IntPtr>();



            public Dictionary<string, List<string>> dictLocPosition = new Dictionary<string, List<string>>();

            public Dictionary<string, List<string>> dictCalibration = new Dictionary<string, List<string>>();

  
            public Dictionary<string, List<string>> dictXXY = new Dictionary<string, List<string>>();

            public Dictionary<string, List<string>> dictClassification = new Dictionary<string, List<string>>();

  
            public Dictionary<string, List<string>> dictFindCircle = new Dictionary<string, List<string>>();


 
            public Dictionary<string, List<string>> dictFindLines = new Dictionary<string, List<string>>();


            public Dictionary<string, List<string>> dictBarcode = new Dictionary<string, List<string>>();

            public Dictionary<string, List<string>> dictDatacode = new Dictionary<string, List<string>>();


    
            public Dictionary<string, List<string>> dictOCR = new Dictionary<string, List<string>>();

            public Dictionary<string, string> dictOcrRegulation = new Dictionary<string, string>();

            public Dictionary<string, List<string>> dictOCRDetect = new Dictionary<string, List<string>>();


    
            public Dictionary<string, HObject> dictRegion = new Dictionary<string, HObject>();


            public Dictionary<string, List<string>> dictProgram = new Dictionary<string, List<string>>();

            public List<List<string>> listPubHalconCalib = new List<List<string>>();

            public paramLibGlobal Clone()
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Binder = new UBinder();

                formatter.Serialize(stream, this);
                stream.Position = 0;
                return formatter.Deserialize(stream) as paramLibGlobal;
            }
            #endregion
        }

        public class UBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return ass.GetType(typeName);
            }
        }


        public static void ConfigReadLib()//根据路径声明
        {
            #region  *** 读取参数  ***

            try
            {
                XmlDocument xmlDoc = new XmlDocument();


                #region *** OCR Detect  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\OCRDetect";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictOCRDetect.Add(strName, lShip);
                                dhDll.frmMsg.Log("读取参数文件OCR检测" + strName, "Reading global oct detect parameters " + strName, null, dhDll.logDiskMode.Normal, 0);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件OCR检测出错:", "Error occured when reading global barcode parameters", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region *** Region   ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\Regions";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.tif");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            HObject hoRegion;
                            //HOperatorSet.GenEmptyRegion(out hoRegion);
                            HOperatorSet.ReadRegion(out hoRegion, strFiles[igg]);

                            int iIndex = strFiles[igg].LastIndexOf("\\");
                            string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                            configLibGlobal.dictRegion.Add(strName, hoRegion);
                            //HOperatorSet.WriteRegion(configLibGlobal.dictRegion[strName], "D:\\1.tif");


                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件Region出错:", "Error occured when reading global barcode parameters", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** 定位 形状定位  ***

                string strDirectory11 = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\LocPosition";
                if (System.IO.Directory.Exists(strDirectory11))
                {
                    string[] strFiles = System.IO.Directory.GetFiles(strDirectory11, "*.xml");

                    for (int igg = 0; igg < strFiles.Length; igg++)
                    {
                        try
                        {
                            int iIndex = strFiles[igg].LastIndexOf("\\");
                            string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                            //初始化handle
                            xslReadShapeModel(strName);
                        }
                        catch (Exception exc)
                        {
                            dhDll.frmMsg.Log("读取全局通用参数文件定位出错:", "Error occured when reading global shape model parameters", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                        }
                    }
                }

                #endregion

                #region  *** RS232全局参数  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\RS232";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictRS232.Add(strName, lShip);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件串口出错:", "Error occured when reading global rs232 parameters", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** TCP全局参数  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\TCP";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictTCP.Add(strName, lShip);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件TCP出错:", "Error occured when reading global tcp socket settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** IO全局参数  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\IO";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictIO.Add(strName, lShip);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件IO出错:", "Error occured when reading global rs232 settings", null, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** 首件全局参数  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\PrintCheck";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictFirstPiece.Add(strName, lShip);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件FirstPiece出错:", "Error occured when reading global FirstPiece settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** PLC全局参数  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\PLC";
                    if (System.IO.Directory.Exists(strDirectory))
                    {
                        string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");

                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictPLC.Add(strName, lShip);
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件PLC出错:", "Error occured when reading global plc settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion


                #region  *** 高级参数 抽风机冷水机工单确认界面等 ***

                try
                {
                    string strFile = System.Windows.Forms.Application.StartupPath + "\\Global\\Advanced\\Advanced.xml";
                    if (System.IO.File.Exists(strFile))
                    {
                        xmlDoc = new XmlDocument(); xmlDoc.Load(strFile);
                        XmlNode root = xmlDoc.SelectSingleNode("Config");
                        XmlNode xnItem = root.SelectSingleNode("Params");

                        if (xnItem != null)
                        {
                            string strAdvanced = xnItem.InnerText.Split('=')[1];
                            configLibGlobal.lstrAdvanced = new List<string>(strAdvanced.Split('#'));
                        }
                        else
                        {
                            for (int ihh = 0; ihh < 10; ihh++) configLibGlobal.lstrAdvanced.Add("0");
                        }
                    }
                    else
                    {
                        MetroFramework.MetroMessageBox.Show(null, "无法读取Advanced参数", "Advanced");
                        configLibGlobal.lstrAdvanced = new List<string>();
                        for (int ihh = 0; ihh < 10; ihh++) configLibGlobal.lstrLaser.Add("0");
                    }

                }
                catch (Exception exc)
                {
                    MetroFramework.MetroMessageBox.Show(null, "读取ADVANCED出错", "读取出错", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    //dhDll.frmMsg.Log("读取全局通用参数文件高级出错:" + exc.Message, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** 缺陷细节显示参数等 ***

                try
                {
                    string strFile = System.Windows.Forms.Application.StartupPath + "\\Global\\Advanced\\Details.xml";
                    if (System.IO.File.Exists(strFile))
                    {
                        xmlDoc = new XmlDocument(); xmlDoc.Load(strFile);
                        XmlNode root = xmlDoc.SelectSingleNode("Config");
                        XmlNode xnItem = root.SelectSingleNode("Params");

                        if (xnItem != null)
                        {
                            string strDetails = xnItem.InnerText.Split('=')[1];
                            configLibGlobal.lstrDetails = new List<string>(strDetails.Split('#'));
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件细节显示出错:", "Error occured when reading error showing details", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** 读取要Check的DLL等 ***

                try
                {
                    string strFile = System.Windows.Forms.Application.StartupPath + "\\Global\\Advanced\\CheckDll.xml";
                    if (System.IO.File.Exists(strFile))
                    {
                        xmlDoc = new XmlDocument(); xmlDoc.Load(strFile);
                        XmlNode root = xmlDoc.SelectSingleNode("Config");
                        XmlNode xnItem = root.SelectSingleNode("Params");

                        if (xnItem != null)
                        {
                            string strCheckdll = xnItem.InnerText.Split('=')[1];
                            configLibGlobal.lstrCheckDll = new List<string>(strCheckdll.Split('#'));
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        MetroFramework.MetroMessageBox.Show(null, "无法读取要校验的DLL的版本参数", "CHECKDLL");
                    }
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数DLL版本监控出错:", "Error occured when reading global dll monitor settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** 统计信息参数 ***

                try
                {
                    string strFile = System.Windows.Forms.Application.StartupPath + "\\Global\\Statistics\\Statistics.xml";
                    if (System.IO.File.Exists(strFile))
                    {
                        xmlDoc = new XmlDocument(); xmlDoc.Load(strFile);
                        XmlNode root = xmlDoc.SelectSingleNode("Config");
                        XmlNode xnItem = root.SelectSingleNode("Params");

                        if (xnItem != null)
                        {
                            string strAdvanced = xnItem.InnerText.Split('=')[1];
                            configLibGlobal.lstrStatistic = new List<string>(strAdvanced.Split('#'));

                        }
                        else
                        {
                            for (int ihh = 0; ihh < 50; ihh++) configLibGlobal.lstrStatistic.Add("0");
                        }
                    }
                    else
                    {
                        for (int ihh = 0; ihh < 50; ihh++) configLibGlobal.lstrStatistic.Add("0");
                    }

                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件统计信息出错:", "Error occured when reading statistics settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion

                #region  *** drawing 绘图参数  ***

                try
                {
                    string strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\Drawing";
                    if (!System.IO.Directory.Exists(strDirectory)) System.IO.Directory.CreateDirectory(strDirectory);

                    string[] strFiles = System.IO.Directory.GetFiles(strDirectory, "*.xml");
                    if (strFiles.Length == 0)
                    {
                        string strkpdraw = "0#宋体#26#Bold#100#100#0#255#0#OK#255#0#0#NG#0#255#255#1#0#0#255#1#宋体#16#Bold#255#0#0#1#宋#16#Bold#0#0#0#0#0#255#255#0#0#0#0#255#1#255#0#0#0#0#255#1#255#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0#0";
                        List<string> ldrawparam = new List<string>(strkpdraw.Split('#'));
                        configLibGlobal.dictDrawing.Add("drawing", ldrawparam);

                        ConfigSaveLib(24, "drawing");
                    }
                    else
                    {
                        for (int igg = 0; igg < strFiles.Length; igg++)
                        {
                            xmlDoc = new XmlDocument(); xmlDoc.Load(strFiles[igg]);
                            XmlNode root = xmlDoc.SelectSingleNode("Config");
                            XmlNode xnItem = root.SelectSingleNode("Params");

                            if (xnItem != null)
                            {
                                string[] strShip = xnItem.InnerText.Split(',');
                                List<string> lShip = new List<string>();
                                for (int ikk = 0; ikk < strShip.Length; ikk++)
                                {
                                    lShip.Add(strShip[ikk].Split('=')[1]);
                                }
                                int iIndex = strFiles[igg].LastIndexOf("\\");
                                string strName = strFiles[igg].Substring(iIndex + 1, strFiles[igg].Length - iIndex - 5);

                                configLibGlobal.dictDrawing.Add(strName, lShip);
                            }
                        }
                    }

                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("读取全局通用参数文件drawing出错:", "Error occured when reading global drawing settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
                }
                #endregion






            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("读取全局通用参数文件出错:", "Error occured when reading global settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
            }

            #endregion
        }


        private static Mutex mutexConfigSave = new Mutex();


        public static void ConfigSaveLib(int iLibStyle, string strLibName)//根据路径声明
        {
            #region  *** 保存库参数  ***

            try
            {
                mutexConfigSave.WaitOne();

                XmlDocument xmlDoc = new XmlDocument();
                List<string> lstrParams = new List<string>();

                string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL";
                if (!System.IO.Directory.Exists(strDirectory)) System.IO.Directory.CreateDirectory(strDirectory);

                if (iLibStyle == 1)
                {
                    strDirectory += "\\Barcode";
                    if (configLibGlobal.dictBarcode.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictBarcode[strLibName];
                    }
                }
                else if (iLibStyle == 2)
                {
                    strDirectory += "\\Calibration";
                    if (configLibGlobal.dictCalibration.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictCalibration[strLibName];
                    }
                }
                else if (iLibStyle == 3)
                {
                    strDirectory += "\\Circle";
                    if (configLibGlobal.dictFindCircle.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictFindCircle[strLibName];
                    }
                }
                else if (iLibStyle == 4)
                {
                    strDirectory += "\\Classification";
                    if (configLibGlobal.dictClassification.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictClassification[strLibName];
                    }
                }
                else if (iLibStyle == 5)
                {
                    strDirectory += "\\Datacode";
                    if (configLibGlobal.dictDatacode.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictDatacode[strLibName];
                    }
                }
                else if (iLibStyle == 6)
                {
                    strDirectory += "\\LocPosition";
                    if (configLibGlobal.dictLocPosition.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictLocPosition[strLibName];
                    }
                }
                else if (iLibStyle == 7)
                {
                    strDirectory += "\\OCR";
                    if (configLibGlobal.dictOCR.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictOCR[strLibName];
                    }

                }
                else if (iLibStyle == 8)
                {
                    strDirectory += "\\PLC";
                    if (configLibGlobal.dictPLC.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictPLC[strLibName];
                    }
                }
                else if (iLibStyle == 9)
                {
                    strDirectory += "\\RS232";
                    if (configLibGlobal.dictRS232.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictRS232[strLibName];
                    }

                }
                else if (iLibStyle == 10)
                {
                    strDirectory += "\\1";
                }
                else if (iLibStyle == 11)
                {
                    strDirectory += "\\TCP";
                    if (configLibGlobal.dictTCP.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictTCP[strLibName];
                    }
                }
                else if (iLibStyle == 12)
                {
                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\Laser";
                    string str2laser = dhDll.clsFunction.transFromStringArray2String(configLibGlobal.lstrLaser.ToArray());
                    lstrParams = new List<string>();
                    lstrParams.Add(str2laser);
                }
                else if (iLibStyle == 13)
                {
                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\Advanced";
                    string str2Advanced = dhDll.clsFunction.transFromStringArray2String(configLibGlobal.lstrAdvanced.ToArray());
                    lstrParams = new List<string>();
                    lstrParams.Add(str2Advanced);
                }
                else if (iLibStyle == 14)
                {
                    strDirectory += "\\IO";
                    if (configLibGlobal.dictIO.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictIO[strLibName];
                    }
                }
                else if (iLibStyle == 15)
                {
                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\Wholemap";
                    lstrParams = new List<string>();
                    for (int ikk = 0; ikk < configLibGlobal.lstrWholemap.Count; ikk++)
                    {
                        string str2Advanced = dhDll.clsFunction.transFromStringArray2String(configLibGlobal.lstrWholemap[ikk].ToArray());

                        lstrParams.Add(str2Advanced);
                    }
                }
                else if (iLibStyle == 16)
                {
                    strDirectory += "\\StageXXY";
                    if (configLibGlobal.dictXXY.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictXXY[strLibName];
                    }
                }
                else if (iLibStyle == 17)
                {
                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\Advanced";
                    string str2Details = dhDll.clsFunction.transFromStringArray2String(configLibGlobal.lstrDetails.ToArray());
                    lstrParams = new List<string>();
                    lstrParams.Add(str2Details);
                }
                else if (iLibStyle == 18)
                {
                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\Advanced";
                    string strCheckdlls = dhDll.clsFunction.transFromStringArray2String(configLibGlobal.lstrCheckDll.ToArray());
                    lstrParams = new List<string>();
                    lstrParams.Add(strCheckdlls);
                }
                else if (iLibStyle == 19)
                {
                    strDirectory += "\\Line";
                    if (configLibGlobal.dictFindLines.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictFindLines[strLibName];
                    }
                }
                else if (iLibStyle == 20)
                {
                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\Global\\Statistics";
                    string str2Statistic = dhDll.clsFunction.transFromStringArray2String(configLibGlobal.lstrStatistic.ToArray());
                    lstrParams = new List<string>();
                    lstrParams.Add(str2Statistic);
                }
                else if (iLibStyle == 21)
                {
                    strDirectory += "\\OCRDetect";
                    if (configLibGlobal.dictOCRDetect.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictOCRDetect[strLibName];
                    }
                }
                else if (iLibStyle == 22)
                {
                    strDirectory += "\\Program";
                    if (configLibGlobal.dictProgram.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictProgram[strLibName];
                    }
                }
                else if (iLibStyle == 23)//首件
                {
                    strDirectory += "\\PrintCheck";
                    if (configLibGlobal.dictFirstPiece.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictFirstPiece[strLibName];
                    }
                }
                else if (iLibStyle == 24)//drawing
                {
                    strDirectory += "\\Drawing";
                    if (configLibGlobal.dictDrawing.ContainsKey(strLibName))
                    {
                        lstrParams = configLibGlobal.dictDrawing[strLibName];
                    }
                }
                else
                {
                  
                }
                if (!System.IO.Directory.Exists(strDirectory)) System.IO.Directory.CreateDirectory(strDirectory);

                string xmlpath = strDirectory + "\\" + strLibName + ".xml";
                if (System.IO.File.Exists(xmlpath))
                {
                    xmlDoc.Load(xmlpath);
                    XmlNode root = xmlDoc.SelectSingleNode("Config");
                    XmlNode xnItem = root.SelectSingleNode("Params");
                    if (xnItem != null && lstrParams.Count > 0)
                    {
                        xnItem.InnerText = dhDll.clsFunction.transFormList2String(lstrParams, "Parameter");
                    }
                }
                else //如果name.xml不存在，那么先创建 此时barcode参数应该是有的
                {
                    XmlDeclaration xmlDec = xmlDoc.CreateXmlDeclaration("1.0", "GB2312", null);
                    xmlDoc.AppendChild(xmlDec);

                    XmlNode root = xmlDoc.CreateElement("Config");
                    xmlDoc.AppendChild(root);

                    XmlNode xnItem = xmlDoc.CreateElement("Params");
                    if (xnItem != null && lstrParams.Count > 0)
                    {
                        xnItem.InnerText = dhDll.clsFunction.transFormList2String(lstrParams, "Parameter");
                    }
                    root.AppendChild(xnItem);
                }

                xmlDoc.Save(xmlpath);

            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("保存全局通用参数文件出错:", "Error occured when saving global settings", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                mutexConfigSave.ReleaseMutex();
            }

            #endregion
        }

        public static void destroyAIHandle()
        {
          
        }

        #endregion


        #region  ****** 2017.7.24 新版后的处理函数 功能添加式  ******



        public static Dictionary<string, string[]> syVisionProcess(HObject hoImage, dhDll.KickEnResults dhKickERJah, HObject objHeight, Image<Gray, byte> bmpEmgu,
            Image<Bgr, byte> bmpEmguRGB, int iThreadNumber, ref string strSetupClick, out List<object> Obj2Draw, ref bool bResultOKNG, int iStepSelect)
        {
            //muMainProcess.WaitOne();

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start(); string strTimeEllapse = "";

            List<object> listObj2Draw = new List<object>();
            List<string> listInfo2Draw = new List<string>();
            Dictionary<string, string[]> dicResult = new Dictionary<string, string[]>();

            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            

            if (hoImage == null)
            {
                listObj2Draw[0] = 0; listObj2Draw[1] = "NG"; listObj2Draw[2] = 888;
                Obj2Draw = listObj2Draw;
                return dicResult;
            }


            try
            {
                Parameter.paramClass configTemple = new Parameter.paramClass();

                #region  *** 复制参数类，用浅复制，复制指针  ***

                if (iThreadNumber == 1) configTemple = Parameter.configStructThread01;
                else if (iThreadNumber == 2) configTemple = Parameter.configStructThread02;
                else if (iThreadNumber == 3) configTemple = Parameter.configStructThread03;
                else if (iThreadNumber == 4) configTemple = Parameter.configStructThread04;
                else if (iThreadNumber == 5) configTemple = Parameter.configStructThread05;
                else if (iThreadNumber == 6) configTemple = Parameter.configStructThread06;
                else if (iThreadNumber == 7) configTemple = Parameter.configStructThread07;
                else if (iThreadNumber == 8) configTemple = Parameter.configStructThread08;
                else if (iThreadNumber == 9) configTemple = Parameter.configStructThread09;
                else if (iThreadNumber == 10) configTemple = Parameter.configStructThread10;
                else if (iThreadNumber == 11) configTemple = Parameter.configStructThread11;
                else if (iThreadNumber == 12) configTemple = Parameter.configStructThread12;
                else if (iThreadNumber == 13) configTemple = Parameter.configStructThread13;
                else if (iThreadNumber == 14) configTemple = Parameter.configStructThread14;
                else if (iThreadNumber == 15) configTemple = Parameter.configStructThread15;
                else if (iThreadNumber == 16) configTemple = Parameter.configStructThread16;
                else if (iThreadNumber == 17) configTemple = Parameter.configStructThread17;
                else if (iThreadNumber == 18) configTemple = Parameter.configStructThread18;

                #endregion



                PointA paCenter_model = new PointA();
                PointA paCenter_identify_image = new PointA();
                PointA paCenter_identify_world = new PointA();
                PointF paGrabPosition = new PointF();
                PointF paGrabCenter = new PointF();

                HTuple hv_calibMatrix = null;//标定仿射矩阵,将得到的匹配中心转换为世界坐标
                HTuple hv_movementMatrix = null;//仿射矩阵,将检测区域的ROI根据匹配中心平移和旋转

                HTuple hvCalibx, hvCaliby, hvCalibz, hv_OutCol, hv_OutRow;

                List<double> ldCenterMatchResult = new List<double>();
                List<double> ldCenterAffined = new List<double>();
                bool bisOverlap = false;
                List<object> ldrawItems = new List<object>();

                bool bFindLocation = false;
                bool bCalibrationExed = false;

                //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>  耗时监控  >>>>>>>>>>>

                if (configTemple.CalibCommon.StartsWith("calibrationMultiImages") && configTemple.calibMultiImages != "")
                { 
                    #region  *** 根据标定关系，得到标定矩阵  ***

                    string[] strcalibparam = configTemple.calibMultiImages.Split('#');

                    if (!configLibGlobal.dictCalibration.ContainsKey(strcalibparam[0]))
                    {
                        listObj2Draw[0] = 0; listObj2Draw[1] = "NG-无标定库"; listObj2Draw[2] = 888;
                        Obj2Draw = listObj2Draw;
                        return dicResult;
                    }

                    List<string> strCalibration = configLibGlobal.dictCalibration[strcalibparam[0]];

                    strcalibparam = strCalibration[0].Split('#');

                    //多点标定和系数标定
                    hv_calibMatrix = hvAllHandles[int.Parse(strcalibparam[0])];


                    //镜头畸变校准


                    //XXY平台


                    #endregion
                }

                //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>  耗时监控  >>>>>>>>>>>
                bool bSkip = false;
                bool bThreadSave = false;
                if (configTemple.listUserSet.Count > 0)
                {
                    bSkip = true;

                    #region  *** 用户自定义程序  ***

                    for (int igg = 0; igg < configTemple.listUserSet.Count; igg++)
                    {
                        string strResults = "";
                        string strUserParam = strSetupClick.Contains("#") ? strSetupClick : configTemple.listUserSet[igg].listParameters[0];//拉动滚动条显示处理效果，时候传进来 参数集

                        if (strUserParam.StartsWith("5#") || strUserParam.StartsWith("15#")) bThreadSave = true;

                        List<object> looResult = syUserSetProgram(iThreadNumber, hoImage, objHeight, ref strSetupClick, strUserParam,
                            configTemple.listUserSet[igg].listPolygonFind.ToArray(), ref strResults);

                        //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>  耗时监控  >>>>>>>>>>>

                        //将 Object 复制到 绘图
                        if (looResult.Count > 1) listObj2Draw[1] = looResult[1];
                        if (looResult.Count > 2) listObj2Draw[2] = looResult[2];
                        for (int ikk = 3; ikk < looResult.Count; ikk++)
                        {
                            listObj2Draw.Add(looResult[ikk]);
                        }

                        //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>  耗时监控  >>>>>>>>>>>

                        if ((string)listObj2Draw[1] != "OK") break;

                        string[] lmmResult = strResults.Split('#');
                        if (lmmResult.Length >= 4 && lmmResult[0] == "RANDS")
                        {
                            //全自动印刷机对网板角度和偏差

                            //HOperatorSet.ProjectiveTransPoint2d(hv_calibMatrix, 120, 2800, new HTuple(1), out hvCaliby, out hvCalibx, out hvCalibz);
                            //hvCalibx = hvCalibx / hvCalibz;
                            //hvCaliby = hvCaliby / hvCalibz;

                            //double dcalibx1 = hvCalibx.D;
                            //double dcaliby1 = hvCaliby.D;

                            //HOperatorSet.ProjectiveTransPoint2d(hv_calibMatrix, 121, 2800, new HTuple(1), out hvCaliby, out hvCalibx, out hvCalibz);
                            //hvCaliby = hvCaliby / hvCalibz;
                            //double dcaliby2 = hvCaliby.D;

                            //HOperatorSet.ProjectiveTransPoint2d(hv_calibMatrix, 120, 2801, new HTuple(1), out hvCaliby, out hvCalibx, out hvCalibz);
                            //hvCalibx = hvCalibx / hvCalibz;

                            //double dcalibx2 = hvCalibx.D;

                            double deltaZ1 = double.Parse(lmmResult[1]) * 1 / 1d;
                            double deltaZ2 = double.Parse(lmmResult[2]) * 1 / 1d;
                            double deltaZ3 = double.Parse(lmmResult[3]) * 1 / 1d;
                            //double deltaZ4 = double.Parse(lmmResult[4]) * 1 / 1d;
                            //double deltaZ3 = double.Parse(lmmResult[4]) * dPixelY;
                            //double deltaZ4 = double.Parse(lmmResult[5]) * dPixelY;
                            lmmResult[1] = deltaZ1.ToString();
                            lmmResult[2] = deltaZ2.ToString();
                            lmmResult[3] = deltaZ3.ToString();
                            //lmmResult[4] = deltaZ4.ToString();
                            //lmmResult[4] = deltaZ3.ToString();
                            //lmmResult[5] = deltaZ4.ToString();
                            dicResult.Add("RANDS", lmmResult);
                        }
                        if (lmmResult.Length >= 12 && lmmResult[1] != "RANDS")
                        {
                            //找到的MARK
                            paCenter_identify_image.X = float.Parse((string)lmmResult[0]);
                            paCenter_identify_image.Y = float.Parse((string)lmmResult[1]);

                            //添加绘制信息
                            listInfo2Draw.Add("中心:X=" + paCenter_identify_image.X.ToString("0.00") + ",Y=" + paCenter_identify_image.Y.ToString("0.00") + ",R=" + paCenter_identify_image.Angle.ToString("0.000"));
                            listInfo2Draw.Add("OK");

                            //添加绘制图形
                            listObj2Draw.Add("十字");
                            listObj2Draw.Add(paCenter_identify_image.PointF);
                            listObj2Draw.Add("OK");

                            listObj2Draw.Add("矩形");
                            listObj2Draw.Add(new RectangleF(paCenter_identify_image.X - 160, paCenter_identify_image.Y - 160, 320, 320));
                            listObj2Draw.Add("OK");

                            dicResult.Add("LOCATION", new string[] { paCenter_model.X.ToString(),paCenter_model.Y.ToString(),paCenter_model.Angle.ToString(), 
                                                        paCenter_identify_image.X.ToString(), paCenter_identify_image.Y.ToString(), paCenter_identify_image.Angle.ToString(),
                                                        paCenter_identify_world.X.ToString(), paCenter_identify_world.Y.ToString(), paCenter_identify_world.Angle.ToString() });
                        }
                    }

                    #endregion
                }

                List<RectangleF> recfLocResult = new List<RectangleF>();
                bool[] bLocOK = new bool[configTemple.LocPosNodes.Count];
                List<RectangleF> recfLocYnnn = new List<RectangleF>();

                if (configTemple.LocPosNodes.Count > 0 && !bSkip)
                {
                    #region  *** 如果存在定位方式，需要先求出定位矩阵，后面的区域进行仿射变换  ***

                    for (int izod = 0; izod < configTemple.LocPosNodes.Count; izod++)
                    {
                        //暂时只支持 圆定位，形状定位，后续添加

                        string strloccommon = configTemple.LocPosNodes[izod].strUsed; ;
                        bLocOK[izod] = false;

                        if (strloccommon.Contains("形状定位"))//单MARK形状定位
                        {
                            List<RectangleF> lkkResult = dhLocPositionAlk(hoImage, configTemple.LocPosNodes[izod], -1, -1, out bisOverlap, out ldrawItems, 888);
                            recfLocResult.AddRange(lkkResult);

                            if (lkkResult.Count > 0)
                            {
                                bLocOK[izod] = true;
                                recfLocYnnn.Add(lkkResult[0]);
                            }
                            else
                            {
                                recfLocYnnn.Add(RectangleF.Empty);
                            }

                            if (izod == 0)
                            {
                                string[] strParam = configTemple.LocPosNodes[0].listParameters[2].Split('#');

                                //模板中心
                                paCenter_model.X = float.Parse(strParam[2]);
                                paCenter_model.Y = float.Parse(strParam[3]);

                                paGrabPosition.X = float.Parse(strParam[5]);
                                paGrabPosition.Y = float.Parse(strParam[6]);

                                if (strParam.Length > 7) paGrabCenter.X = float.Parse(strParam[7]);
                                if (strParam.Length > 8) paGrabCenter.Y = float.Parse(strParam[8]);
                            }
                        }
                        //else if (strloccommon[0] == "locationmethod2" || strloccommon[0] == "locationmethod3")//多MARK形状定位
                        //{
                        //    //dhLocPositionAlk(hoImage, iThreadNumber, out ldCenterMatchResult, out hv_movementOfObject, -1, -1, out ldCenterAffined, out bisOverlap, out ldrawItems, 888);
                        //}
                        else if (strloccommon.Contains("定位圆"))
                        {
                            #region  *** 单圆定位 无旋转 ***

                            //List<PointF> pfsRegions = null;
                            ////float fScale = float.Parse(Parameter.configPublic.lstrWholeMap[7]);
                            //recfLocResult = dhFindCircleInROI(hoImage, configTemple.LocPosNodes[izod], ref pfsRegions);

                            //if (izod == 0)
                            //{
                            //    string[] strParam = configTemple.LocPosNodes[0].listParameters[1].Split('#');

                            //    //模板中心
                            //    paCenter_model.X = float.Parse(strParam[2]);
                            //    paCenter_model.Y = float.Parse(strParam[3]);

                            //    paGrabPosition.X = float.Parse(strParam[5]);
                            //    paGrabPosition.Y = float.Parse(strParam[6]);

                            //    if (strParam.Length > 7) paGrabCenter.X = float.Parse(strParam[7]);
                            //    if (strParam.Length > 8) paGrabCenter.Y = float.Parse(strParam[8]);

                            //}

                            #endregion
                        }

                    
                    }

                    if (recfLocResult.Count > 0 && recfLocResult[0].X != -1 && recfLocResult[0].Y != -1 && recfLocResult[0].X != -2 && recfLocResult[0].Y != -2)
                    {
                        bFindLocation = true;

                        //图片mark中心
                        paCenter_identify_image.X = recfLocResult[0].X;
                        paCenter_identify_image.Y = recfLocResult[0].Y;
                        paCenter_identify_image.Angle = recfLocResult[0].Width;

                        //添加绘制信息
                        listInfo2Draw.Add("中心:X=" + paCenter_identify_image.X.ToString("0.00") + ",Y=" + paCenter_identify_image.Y.ToString("0.00") + ",R=" + paCenter_identify_image.Angle.ToString("0.000"));
                        listInfo2Draw.Add("OK");

                        //添加绘制图形
                        listObj2Draw.Add("定位中心");
                        listObj2Draw.Add(paCenter_identify_image.PointF);
                        listObj2Draw.Add("OK");

                        if (recfLocResult.Count > 1)
                        {
                            for (int imm = 1; imm < recfLocResult.Count; imm += 1)
                            {
                                listInfo2Draw.Add("中心:X=" + recfLocResult[imm].X.ToString("0.00") + ",Y=" + recfLocResult[imm].Y.ToString("0.00") + ",R=" + recfLocResult[imm].Width.ToString("0.000"));
                                listInfo2Draw.Add("OK");

                                listObj2Draw.Add("十字");
                                listObj2Draw.Add(recfLocResult[imm].Location);
                                listObj2Draw.Add("OK");
                            }

                        }

                        listObj2Draw.Add("矩形");
                        listObj2Draw.Add(new RectangleF(paCenter_identify_image.X - 160, paCenter_identify_image.Y - 160, 320, 320));
                        listObj2Draw.Add("OK");

                        //求出定位仿射矩阵
                        HOperatorSet.VectorAngleToRigid(paCenter_model.Y, paCenter_model.X, paCenter_model.Angle_rad,
                            paCenter_identify_image.Y, paCenter_identify_image.X, paCenter_identify_image.Angle_rad, out hv_movementMatrix);

                        //转换为世界坐标
                        if (hv_calibMatrix != null)
                        {
                            HOperatorSet.ProjectiveTransPoint2d(hv_calibMatrix, paCenter_identify_image.Y, paCenter_identify_image.X, new HTuple(1), out hvCaliby, out hvCalibx, out hvCalibz);

                            hv_OutCol = hvCalibx / hvCalibz;
                            hv_OutRow = hvCaliby / hvCalibz;

                            paCenter_identify_world.X = (float)hv_OutCol.DArr[0];
                            paCenter_identify_world.Y = (float)hv_OutRow.DArr[0];

                            bCalibrationExed = true;
                        }

                        
                    }


                    #endregion
                }

    

                RectangleF[] recFindLines = new RectangleF[configTemple.listFindLine.Count+1];
           


                if (iStepSelect < 800)
                {
                    //listObj2Draw[0] = 0; listObj2Draw[1] = "NG-标定无法执行"; listObj2Draw[2] = 888;
                    Obj2Draw = listObj2Draw; //bResultOKNG = false;
                    return dicResult;
                }

             

                //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>耗时监控

                List<string> lstrOutput = new List<string>();
                if (configTemple.listExpression.Count > 0)
                {            
                    #region  *** OUTPUT  ***

                    for (int igg = 0; igg < configTemple.listExpression.Count; igg++)
                    {
                        for (int ihh = 0; ihh < configTemple.listExpression[igg].listParameters.Count; ihh++)
                        {
                            lstrOutput.Add(configTemple.listExpression[igg].listParameters[ihh]);
                        }
                    }
                    #endregion
                }

                dicResult.Add("Output", lstrOutput.ToArray());

                //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>耗时监控

                dicResult.Add("GRABPOSITION", new string[] { paGrabPosition.X.ToString(), paGrabPosition.Y.ToString(), paGrabCenter.X.ToString(), paGrabCenter.Y.ToString() });

                if (!bFindLocation && !bSkip && configTemple.LocPosNodes.Count > 0)
                {
                    listObj2Draw[0] = 0; listObj2Draw[1] = "NG-无定位"; listObj2Draw[2] = 888;
                    Obj2Draw = listObj2Draw; bResultOKNG = false;
                    return dicResult;
                }

                if (hv_calibMatrix != null && configTemple.LocPosNodes.Count > 0 && !bCalibrationExed)
                {
                    listObj2Draw[0] = 0; listObj2Draw[1] = "NG-标定无法执行"; listObj2Draw[2] = 888;
                    Obj2Draw = listObj2Draw; bResultOKNG = false;
                    return dicResult;
                }

                if (bFindLocation && configTemple.LocPosNodes.Count > 0)
                {
                    dicResult.Add("LOCATION", new string[] { paCenter_model.X.ToString(),paCenter_model.Y.ToString(),paCenter_model.Angle.ToString(), 
                                                        paCenter_identify_image.X.ToString(), paCenter_identify_image.Y.ToString(), paCenter_identify_image.Angle.ToString(),
                                                        paCenter_identify_world.X.ToString(), paCenter_identify_world.Y.ToString(), paCenter_identify_world.Angle.ToString() });
                }
                if (bFindLocation && configTemple.LocPosNodes.Count > 0)
                {
                    List<string> lstrFind = new List<string>();
                    for (int igg = 0; igg < recfLocYnnn.Count; igg++)
                    {
                        lstrFind.Add(bLocOK[igg].ToString());
                        lstrFind.Add(recfLocYnnn[igg].X.ToString());
                        lstrFind.Add(recfLocYnnn[igg].Y.ToString());
                        lstrFind.Add(recfLocYnnn[igg].Width.ToString());
                    }
                    dicResult.Add("FIND", lstrFind.ToArray());
                }
                if (bFindLocation && configTemple.LocPosNodes.Count > 0 && configTemple.CalibCommon.StartsWith("calibrationMultiImages") && configTemple.calibMultiImages != "")
                {
                    //转换为世界坐标
                    if (hv_calibMatrix != null)
                    {
                        List<string> lstrCalib = new List<string>();
                        for (int igg = 0; igg < recfLocResult.Count; igg++)
                        {
                            HOperatorSet.ProjectiveTransPoint2d(hv_calibMatrix, recfLocResult[igg].Y, recfLocResult[igg].X, new HTuple(1), out hvCaliby, out hvCalibx, out hvCalibz);
                          
                            hv_OutCol = hvCalibx / hvCalibz;
                            hv_OutRow = hvCaliby / hvCalibz;

                            lstrCalib.Add(hv_OutCol.DArr[0].ToString());
                            lstrCalib.Add(hv_OutRow.DArr[0].ToString());
                        }

                        dicResult.Add("CACLIBRATION", lstrCalib.ToArray());

                    }


                }

                //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>耗时监控

                listObj2Draw.Add("字符串");
                listObj2Draw.Add(listInfo2Draw);
                listObj2Draw.Add(new PointF(1800, 100));

                string strItemOkNg = listObj2Draw.Count > 1 ? (string)listObj2Draw[1] : "NG";

                if (strItemOkNg.Contains("OK")) bResultOKNG = true;
                else if (strItemOkNg.Contains("NG")) bResultOKNG = false;

                //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>耗时监控

                #region **** 首先将listObj2Draw排序，OK的放前面，NG的放后面以免被覆盖 ****

                bool bReformList = false;
                if (bmpEmgu != null && bmpEmgu.Width > 2000 && bmpEmgu.Height > 900) bReformList = true;
                if (bmpEmguRGB != null && bmpEmguRGB.Width > 2000 && bmpEmguRGB.Height > 900) bReformList = true;
                if (bReformList && true)
                {
                    List<object> listmm2draw = new List<object>();
                    listmm2draw.Add(listObj2Draw[0]);
                    listmm2draw.Add(listObj2Draw[1]);
                    listmm2draw.Add(listObj2Draw[2]);
                    for (int igg = 3; igg < listObj2Draw.Count - 2; igg += 3)
                    {
                        if (listObj2Draw[igg + 2] is string)
                        {
                            string strOkNgMsg = (string)listObj2Draw[igg + 2];
                            if (strOkNgMsg.Contains("OK"))
                            {
                                listmm2draw.Add(listObj2Draw[igg]);
                                listmm2draw.Add(listObj2Draw[igg + 1]);
                                listmm2draw.Add(listObj2Draw[igg + 2]);
                            }
                        }
                    }
                    for (int igg = 3; igg < listObj2Draw.Count - 2; igg += 3)
                    {
                        if (listObj2Draw[igg + 2] is string)
                        {
                            string strOkNgMsg = (string)listObj2Draw[igg + 2];
                            if (strOkNgMsg.Contains("NG") || strOkNgMsg.Contains("JUMP") || strOkNgMsg.Contains("SET "))
                            {
                                listmm2draw.Add(listObj2Draw[igg]);
                                listmm2draw.Add(listObj2Draw[igg + 1]);
                                listmm2draw.Add(listObj2Draw[igg + 2]);
                            }
                        }
                        else
                        {
                            listmm2draw.Add(listObj2Draw[igg]);
                            listmm2draw.Add(listObj2Draw[igg + 1]);
                            listmm2draw.Add(listObj2Draw[igg + 2]);
                        }
                    }

                    listObj2Draw = listmm2draw;
                }
                #endregion


                //之前版本，保存所有图片，新版本将OKNG分开保存
                if (strSetupClick == "0")
                {
                    if (!bThreadSave)
                    {
                        List<object> lship2Draw = new List<object>(listObj2Draw.ToArray());//重新做一个list绘图



                        savePicThread(hoImage, bmpEmgu, bmpEmguRGB, iThreadNumber, 1, strItemOkNg, lship2Draw, dhKickERJah);
                    }
                    else   //后端CCD 改成线程存图2019.6.3
                    {
                        

                        List<object> lship2Draw = new List<object>(listObj2Draw.ToArray());//重新做一个list绘图
                        savePicsShip(hoImage, bmpEmgu, bmpEmguRGB, iThreadNumber, 1, strItemOkNg, lship2Draw, dhKickERJah);
                    }
                }

                Obj2Draw = listObj2Draw;

      

                return dicResult;
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("程序出错:","Vision process failed",exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.None);

                listObj2Draw[0] = 0; listObj2Draw[1] = "NG-程序出错"; listObj2Draw[2] = 888;
                Obj2Draw = listObj2Draw;
                return dicResult;
            }
            finally
            {
                //hoImage.Dispose();
            }
        }

        public static T objClone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }


        #endregion


        #region  以一个中心点 将某个点旋转Angle角度 得到新的点的坐标

        /// <summary>  
        /// 以一个中心点 将某个点旋转Angle角度 得到新的点的坐标 逆时针角度为正
        /// </summary>  
        /// <param name="center">中心点</param>  
        /// <param name="p1">待旋转的点</param>  
        /// <param name="angle">旋转角度（角度）</param>  
        public static PointF dhPointRotate(PointF pCenter, PointF p2Rotate, double dAngle)
        {
            try
            {
                //if (dAngle > 90)
                //    dAngle -= 90;
                double x1 = (p2Rotate.X - pCenter.X) * Math.Cos(dAngle * Math.PI / 180) +
                    (p2Rotate.Y - pCenter.Y) * Math.Sin(dAngle * Math.PI / 180) + pCenter.X;
                double y1 = -(p2Rotate.X - pCenter.X) * Math.Sin(dAngle * Math.PI / 180) +
                    (p2Rotate.Y - pCenter.Y) * Math.Cos(dAngle * Math.PI / 180) + pCenter.Y;
                return new PointF((float)x1, (float)y1);
            }
            catch (Exception exc)
            {
                return new PointF(-1, -1);
            }
        }

        #endregion


        #region ******* 输入rectangle2 输出4个边缘交点坐标函数  ******

        public static List<PointF> dhFindVerticesOfRectangle2D(double hvCenterRow, double hvCenterColumn, double hvAngle, float hvLen1, double hvLen2)
        {
            return dhFindVerticesOfRectangle2(new HTuple(hvCenterRow), new HTuple(hvCenterColumn), new HTuple(hvAngle), new HTuple(hvLen1), new HTuple(hvLen2));
        }
        /// <summary>
        /// 输入rectangle2 角度为弧度 输出4个边缘交点坐标函数 ,按照 右上，左上，左下，右下的顺序
        /// </summary>
        /// <param name="hvCenterRow"></param>
        /// <param name="hvCenterColumn"></param>
        /// <param name="hvAngle"></param>
        /// <param name="hvLen1"></param>
        /// <param name="hvLen2"></param>
        /// <returns></returns>
        public static List<PointF> dhFindVerticesOfRectangle2(HTuple hvCenterRow, HTuple hvCenterColumn, HTuple hvAngle, HTuple hvLen1, HTuple hvLen2)
        {
            HTuple Cos = 0, Sin = 0;
            HOperatorSet.TupleCos(hvAngle, out Cos);
            HOperatorSet.TupleSin(hvAngle, out Sin);

            PointF pRecCenter = new PointF((float)hvCenterColumn.DArr[0], (float)hvCenterRow.DArr[0]);
            float fLenth1 = (float)hvLen1.DArr[0];
            float fLenth2 = (float)hvLen2.DArr[0];

            double RB_X = fLenth1 * Cos - fLenth2 * Sin;
            double RB_Y = fLenth1 * Sin + fLenth2 * Cos;
            PointF pTopRight = new PointF(pRecCenter.X + (float)RB_X, pRecCenter.Y - (float)RB_Y);

            double RT_X = -fLenth1 * Cos - fLenth2 * Sin;
            double RT_Y = -fLenth1 * Sin + fLenth2 * Cos;
            PointF pTopLeft = new PointF(pRecCenter.X + (float)RT_X, pRecCenter.Y - (float)RT_Y);

            double LT_X = -fLenth1 * Cos + fLenth2 * Sin;
            double LT_Y = -fLenth1 * Sin - fLenth2 * Cos;
            PointF pBtmLeft = new PointF(pRecCenter.X + (float)LT_X, pRecCenter.Y - (float)LT_Y);

            double LB_X = fLenth1 * Cos + fLenth2 * Sin;
            double LB_Y = fLenth1 * Sin - fLenth2 * Cos;

            PointF pBtmRight = new PointF(pRecCenter.X + (float)LB_X, pRecCenter.Y - (float)LB_Y);

            List<PointF> lmmResult = new List<PointF>();

            lmmResult.Add(pTopRight); lmmResult.Add(pTopLeft);
            lmmResult.Add(pBtmLeft); lmmResult.Add(pBtmRight);

            return lmmResult;

        }

        #endregion




        #region   *** 读取 设置保存 形状模板 NCC  ***

        /// <summary>
        /// 自定义命名17#9#0#0#0#0#0#0#0#0#0#0 查找字典 设置句柄参数
        /// </summary>
        /// <param name="strUsed"></param>
        /// <returns></returns>
        public static bool xslWriteShapeModel(int ishapeType, HTuple hvTempHandle, string strUsed)
        {
            try
            {
                string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\LocPosition";
                if (!System.IO.Directory.Exists(strDirectory)) System.IO.Directory.CreateDirectory(strDirectory);

                string[] strDict = strUsed.Split('#');
                if (strDict.Length < 1) return false;

                string strFilePath = strDirectory + "\\" + strDict[0] + ".shm";

                if (ishapeType == 1) HOperatorSet.WriteShapeModel(hvTempHandle, strFilePath);
                else if (ishapeType == 3) HOperatorSet.WriteDeformableModel(hvTempHandle, strFilePath);
                else if (ishapeType == 4) HOperatorSet.WriteNccModel(hvTempHandle, strFilePath);

                return true;

                //if (!configLibGlobal.dictBarcode.ContainsKey(strDict[0])) return false;

                //return xslSetBarcodeParam(configLibGlobal.dictBarcode[strDict[0]]);
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("写入形状定位句柄错误:", "Error occured when writing shape model", exc, dhDll.logDiskMode.Error, 0);
                return false;
            }
        }

        /// <summary>
        /// 自定义命名17#9#0#0#0#0#0#0#0#0#0#0 查找字典 设置句柄参数
        /// </summary>
        /// <param name="strUsed"></param>
        /// <returns></returns>
        public static bool xslReadShapeModel(string strUsed)
        {
            #region  ***  读取形状定位句柄  ***
            try
            {
                //自定义命名17#9#0#0#0#0#0#0#0#0#0#0 查找字典 设置句柄参数

                string[] strDict = strUsed.Split('#');
                if (strDict.Length < 1) return false;

                string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\LocPosition";
                string strFilePath = strDirectory + "\\" + strDict[0] + ".xml";

                if (!System.IO.File.Exists(strFilePath)) return false;

                XmlDocument xmlDoc = new XmlDocument(); xmlDoc.Load(strFilePath);
                XmlNode root = xmlDoc.SelectSingleNode("Config");
                XmlNode xnItem = root.SelectSingleNode("Params");

                if (xnItem == null) return false;

                string[] strShip = xnItem.InnerText.Split(',');
                List<string> lShip = new List<string>();
                for (int ikk = 0; ikk < strShip.Length; ikk++)
                {
                    lShip.Add(strShip[ikk].Split('=')[1]);
                }

                strShip = lShip[0].Split('#');
                int iIndex = int.Parse(strShip[0]);
                strFilePath = strFilePath.Replace(".xml", ".shm");
                if (System.IO.File.Exists(strFilePath))
                {
                    if (strShip[1] == "shapemodel0" || strShip[1] == "shapemodel1" || strShip[1] == "shapemodel2")
                    {
                        HOperatorSet.ReadShapeModel(strFilePath, out hvAllHandles[iIndex]);
                    }
                    else if (strShip[1] == "shapemodel3")//局部可变性模板匹配
                    {
                        HOperatorSet.ReadDeformableModel(strFilePath, out hvAllHandles[iIndex]);
                    }
                    else if (strShip[1] == "shapemodel4")//NCC
                    {
                        HOperatorSet.ReadNccModel(strFilePath, out hvAllHandles[iIndex]);
                    }
                }

                if (!configLibGlobal.dictLocPosition.ContainsKey(strDict[0]))
                    configLibGlobal.dictLocPosition.Add(strDict[0], lShip);




                return true;
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("读取形状定位句柄错误:", "Error occured when reading shape model", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.None);
                return false;
            }
            #endregion
        }


        #endregion


    

        #region ****** 形状 定位的函数，输出中心点及旋转角度  ******


        public static List<RectangleF> locPositionShape(HObject hoImage, Parameter.paramNode nNodeParam, out List<object> lmm2Draw, int ifindmode)
        {
            List<RectangleF> lblobResult = new List<RectangleF>();
            lmm2Draw = new List<object>();
            if (hoImage == null) return lblobResult;

            HTuple hvCount = null;
            HTuple hvRow = null, hvColumn = null, hvAngle = null, hvScore = null;
            HTuple hvScaleRow, hvScaleCol;
            HObject hoRegion, hoReduced, hoUnion;

            HOperatorSet.GenEmptyObj(out hoRegion);
            HOperatorSet.GenEmptyObj(out hoUnion);

            #region ****** 生成区域ROI  ******

            bool bfindWholePic = true;
            if (nNodeParam.listPolygonFind.Count > 0 && nNodeParam.listPolygonFind[0].Contains("#"))
            {
                bfindWholePic = false;
                List<PointF[]> lkkPolygon = dhDll.clsFunction.transFormString2Polygon(nNodeParam.listPolygonFind, 1);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = pgg2.X;

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.Union2(hoUnion, hoRegion, out hoUnion);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.Union2(hoUnion, hoRegion, out hoUnion);
                    }
                    else if (lkkPolygon[igg][0].X == 1)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.Union2(hoUnion, hoRegion, out hoUnion);
                    }
                }
            }
         

            #endregion

            HTuple hvWidth, hvHeight;
            HOperatorSet.GetImageSize(hoImage, out hvWidth, out hvHeight);

            string[] strUsed = nNodeParam.strUsed.Split('#');
            if (!configLibGlobal.dictLocPosition.ContainsKey(strUsed[0])) return lblobResult;

            List<string> lcodeParam = configLibGlobal.dictLocPosition[strUsed[0]];

            if (lcodeParam.Count < 2) return lblobResult;

            string[] strShapeParams = lcodeParam[0].Split('#');
            int iIndex = int.Parse(strShapeParams[0]);

            strShapeParams = lcodeParam[1].Split('#');
            string strType = strShapeParams[0];

            strShapeParams = lcodeParam[2].Split('#');

            if (strShapeParams[1] == "0" && ifindmode == 0) HOperatorSet.CopyImage(hoImage, out hoReduced);
            else HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

            //HOperatorSet.WriteImage(hoReduced, "jpg", 0, "C:\\bbm.jpg");

            HTuple hvAngleStart = 0;// double.Parse(strShapeParams[2]) / 180 * Math.PI;
            HTuple hvAngleExtent = 2 * Math.PI;// double.Parse(strShapeParams[3]) / 180 * Math.PI;
            double dminScore = double.Parse(strShapeParams[4]);
            int imatchCount = int.Parse(strShapeParams[5]);
            double dmaxOverlap = double.Parse(strShapeParams[6]);
            //int iNumLevels = 0;// int.Parse(strShapeParams[7]);
            HTuple iNumLevels = (new HTuple(15)).TupleConcat(1);//new int[] { 10, 1 };
            string[] strSubPixel = new string[] { "least_squares", "max_deformation " + strShapeParams[9] };
            double dgreediness = double.Parse(strShapeParams[10]);

            double dScaleColmin = double.Parse(strShapeParams[11]);
            double dScaleColmax = double.Parse(strShapeParams[12]);

            double dScaleRowmin = double.Parse(strShapeParams[13]);
            double dScaleRowmax = double.Parse(strShapeParams[14]);

//            HTuple hv_Row, hv_Column, hv_Angle, hv_Score;
//            HOperatorSet.FindShapeModel(hoReduced, ParamHvpple.hvAllHandles[26], 0
//,6.28, 0.5, 1, 0.5, "least_squares", (new HTuple(5)).TupleConcat(
//1), 0.75, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);

            if (strType == "0")//shape model
            {
                try
                {
                    HOperatorSet.FindShapeModel(hoReduced,
                        hvAllHandles[iIndex],
                        hvAngleStart,
                        hvAngleExtent,
                        dminScore,
                        imatchCount,
                        dmaxOverlap,
                        strSubPixel,
                        iNumLevels,
                        dgreediness,
                        out hvRow,
                        out hvColumn,
                        out hvAngle,
                        out hvScore);

                    if (hvWidth.I > 2000 && hvHeight.I > 1000) GC.Collect();

          
                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("形状定位0:","shapemodel 0 ",exc, dhDll.logDiskMode.Error, 0);
                }

                if (hvScore != null && hvScore.Length > 0)
                {
                    #region ****** 获取 模板的 轮廓 并绘制 ******

                    HObject hoModelContour, hvTransContours, hoIndex;
                    HTuple hvContourRow, hvContourCol, hvHom2d;

                    HOperatorSet.GenEmptyObj(out hvTransContours);
                    HOperatorSet.GenEmptyObj(out hoModelContour);

                    HOperatorSet.GetShapeModelContours(out hoModelContour, hvAllHandles[iIndex], 1);

                    for (int iMatIdx = 0; iMatIdx <= hvScore.Length - 1; iMatIdx++)
                    {
                        HOperatorSet.VectorAngleToRigid(0, 0, 0, hvRow[iMatIdx], hvColumn[iMatIdx], hvAngle[0], out hvHom2d);
                        HOperatorSet.AffineTransContourXld(hoModelContour, out hvTransContours, hvHom2d);

                        HOperatorSet.CountObj(hvTransContours, out hvCount);

                        if (hvCount.I > 0)
                        {
                            for (int igg = 1; igg <= hvCount.I; igg++)
                            {
                                HOperatorSet.SelectObj(hvTransContours, out hoIndex, igg);
                                HOperatorSet.GetContourXld(hoIndex, out hvContourRow, out hvContourCol);

                                List<PointF> listPointsContour = new List<PointF>();
                                for (int ihh = 0; ihh < hvContourRow.Length; ihh++)
                                {
                                    listPointsContour.Add(new PointF((float)hvContourCol.DArr[ihh] / 1, (float)hvContourRow.DArr[ihh] / 1));
                                }

                                lmm2Draw.Add("轮廓");
                                lmm2Draw.Add(listPointsContour.ToArray());
                                lmm2Draw.Add("OK");
                            }
                        }
                    }

                    #endregion
                }
            }
            else if (strType == "1")//aniso
            {
                try
                {
                    HOperatorSet.FindAnisoShapeModel(hoReduced,
                        hvAllHandles[iIndex],
                        hvAngleStart,
                        hvAngleExtent,
                        dScaleRowmin,
                        dScaleRowmax,
                        dScaleColmin,
                        dScaleColmax,
                        dminScore,
                        imatchCount,
                        dmaxOverlap,
                        strSubPixel,
                        iNumLevels,
                        dgreediness,
                        out hvRow,
                        out hvColumn,
                        out hvAngle,
                        out hvScaleRow,
                        out hvScaleCol,
                        out hvScore);

                }
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("形状定位1:","shapemodel 1 ",exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.None);
                }
            }
            else if (strType == "2")//scale
            {
               
            }
            else if (strType == "3")//deformable
            {
               
            }
            else if (strType == "4")//ncc
            {
              
            }

            if (hvScore != null && hvScore.Length > 0)//匹配到
            {
                for (int iMatIdx = 0; iMatIdx < hvScore.Length; iMatIdx++)
                {
                    float fx = (float)hvColumn.DArr[iMatIdx];
                    float fy = (float)hvRow.DArr[iMatIdx];
                    float fa = (float)hvAngle.DArr[iMatIdx];
                    float fs = (float)hvScore.DArr[iMatIdx];

                    lblobResult.Add(new RectangleF(fx, fy, (strType == "3" ? 0f : fa), fs));

                }
            }

            return lblobResult;
        }

        #endregion


  

        #region ****** 定位函数 ******


        public static List<RectangleF> dhLocPositionAlk(HObject hoImage, Parameter.paramNode nNodeParam, int iSortMethod, int iAngleSelect, out bool bisOverlap, out List<object> ldrawItmes, int izodSelectIndex)
        {
            List<RectangleF> locResult = new List<RectangleF>();

            bisOverlap = false;
            ldrawItmes = new List<object>();

            string[] strUsed = nNodeParam.strUsed.Split('#');
            if (!configLibGlobal.dictLocPosition.ContainsKey(strUsed[0])) return locResult;

            List<string> lcodeParam = configLibGlobal.dictLocPosition[strUsed[0]];
            if (lcodeParam.Count < 3) return locResult;

            string[] strcodeParams = lcodeParam[0].Split('#');
            if (strcodeParams.Length < 1) return locResult;
            string  strShapeType = strcodeParams[1];

            List<object> list2DrawTemple = new List<object>();

            if (strShapeType.Contains("shapemodel") || strShapeType.Contains("graymodel"))
            {
                #region  ****** 得到定位区域  ******

                List<PointF[]> lblobJah = new List<PointF[]>();

                if (nNodeParam.listPolygonFind.Count > 0 && nNodeParam.listPolygonFind[0].Contains("#"))
                {
                    if (izodSelectIndex == 888)
                    {
                        lblobJah = dhDll.clsFunction.transFormString2Polygon(nNodeParam.listPolygonFind, 1);
                    }
                    else if (izodSelectIndex < 50)
                    {
                        lblobJah = dhDll.clsFunction.transFormString2Polygon(nNodeParam.listPolygonFind[izodSelectIndex - 1], 1);
                    }
                }

                #endregion

                #region  ******  定位本张图片的中心和角度  ******

                if (strShapeType.Contains("shapemodel"))
                {
                    locResult = ParamHvpple.locPositionShape(hoImage, nNodeParam, out list2DrawTemple, 0);
                }
            


                #endregion

                #region  ******  判断产品有无定位，如果定位到，记录中心及角度，计算仿射矩阵，添加绘图 ******

                if (locResult.Count < 1)
                {
                    return locResult;
                }
                else
                {
                    ldrawItmes = list2DrawTemple;
                }


                #endregion

            }
     
            else
            {

            }


            #region ****** 根据设置的获取方法，得到想要的产品中心点 ******

            if (locResult.Count > 0 && iSortMethod == 1)//X max
            {
                for (int igg = locResult.Count - 1; igg >= 1; igg--)
                {
                    if (locResult[igg].X > locResult[0].X)
                    {
                        locResult[0] = locResult[igg];

                        locResult.RemoveAt(igg);
                    }
                }
            }
            if (locResult.Count > 0 && iSortMethod == 2)//X min
            {
                for (int igg = locResult.Count - 1; igg >= 1; igg--)
                {
                    if (locResult[igg].X < locResult[0].X)
                    {
                        locResult[0] = locResult[igg];

                        locResult.RemoveAt(igg);
                    }
                }
            }
            if (locResult.Count > 0 && iSortMethod == 3)//Y max
            {
                for (int igg = locResult.Count - 1; igg >= 1; igg--)
                {
                    if (locResult[igg].Y > locResult[0].Y)
                    {
                        locResult[0] = locResult[igg];

                        locResult.RemoveAt(igg);
                    }
                }
            }
            if (locResult.Count > 0 && iSortMethod == 4)//Y min
            {
                for (int igg = locResult.Count - 1; igg >= 1; igg--)
                {
                    if (locResult[igg].Y < locResult[0].Y)
                    {
                        locResult[0] = locResult[igg];

                        locResult.RemoveAt(igg);
                    }
                }
            }

            #endregion


      
            return locResult;

        }

        #endregion




        #region  ****** 2018.2.28 用户自定义处理程序 ******

        public static PrintingDetection.Process syGuiJieProcess = new PrintingDetection.Process();
        public static PrintingDetection_SY.Process syGuiJieNew = new PrintingDetection_SY.Process();
        public static HTuple hv_Result_bac = new HTuple(), hv_ShowCircle_bac = new HTuple(), hv_strResult_bac = new HTuple();
        public static ccGeneral.HDevelopExport ccProcess = new ccGeneral.HDevelopExport();

        /// <summary>
        /// =1表示正在制作首件
        /// </summary>
        public static long lIsFirstPiece = 0;
        private static HTuple hvParameters_old_print15 = new HTuple();

        public static List<object> syUserSetProgram(int ithreadCheck, HObject hoImage, HObject hoModelX, ref string strSetupTest, string nNodeParam, string[] strPolygon, ref string strResults)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start(); string strTimeEllapse = "";

            List<object> listObj2Draw = new List<object>();

            List<string> strUserParam = new List<string>(nNodeParam.Split('#'));
            if (strUserParam[0] == "0")
            {
                listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
                return listObj2Draw;
            }

            if (syGuiJieProcess == null) syGuiJieProcess = new PrintingDetection.Process(); 


            List<string> listInfo2Draw = new List<string>();

            HTuple hvRow1 = 0, hvRow2 = 0, hvColumn1 = 0, hvColumn2 = 0, hvSort = 0, hvThresh,hvNr, hvNc, hvDist;
            HTuple hvRow = 0, hvColumn = 0, hvAngle = 0, hvScore = 0, hvArea, hvResult, hv_ShowPoint = null, hv_ShowLine = null, hv_ShowCircle = null, hv_ERR=null;
            HObject hoRegion;//, hoReduced, hoUnion, hoConcate, hoConnection, hoSelect, hoContour;
            HOperatorSet.GenEmptyObj(out hoRegion);
            //HOperatorSet.GenEmptyObj(out hoConcate);

            List<PointF[]> lkkPolygon = dhDll.clsFunction.transFormString2Polygon(new List<string>(strPolygon), 1);
            
            //strTimeEllapse += " " + sw.ElapsedMilliseconds.ToString();//  >>>>>>>>>>>耗时监控

            try
            {
                if (strUserParam[0] == "5")
                {
                    #region  *** 印刷机后端  ***

                    listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

                    strUserParam.RemoveAt(0);
                    int[] iParams = new int[strUserParam.Count];
                    for (int igg = 0; igg < 9; igg++)
                    {
                        iParams[igg] = int.Parse(strUserParam[igg]);
                    }

                    iParams[9] = int.Parse(strUserParam[20]);
                    iParams[10] = int.Parse(strUserParam[21]);
                    iParams[11] = int.Parse(strUserParam[22]);

                    iParams[12] = 1; if (lIsFirstPiece == 1) iParams[12] = 2;//2019.6.24Param[12]中为区分首件还是正常检测的参数，1是正常检测，2是首件模式
                    iParams[13] = 1;//2019.11.21这个函数中的参数Param中的Param[12]放检测模式，1是尺寸和塞网都检测，2是只加测塞网


                    if (configLibGlobal.lstrAdvanced[32] == "1")
                    {
                        int iInterval = int.Parse(configLibGlobal.lstrAdvanced[33]);

                        if (iInterval >= 2 && dhDll.clsss_Status.lNumberThreads[1][0] % iInterval == 1)
                        {
                            iParams[13] = 1;
                        }
                        else
                        {
                            iParams[13] = 2;
                        }
                    }

                    HTuple hvParams = new HTuple(iParams);

                    int icheckMode = int.Parse(strUserParam[19]);

                    if (strUserParam[1] == "4")
                    {
                        //G1工序，选择统计信息的高阻值还是低阻值

                        int.TryParse(configLibGlobal.lstrStatistic[9], out icheckMode);
                        //icheckMode = int.Parse(strUserParam[19]);
                    }



                    //HOperatorSet.ReadImage(out hoImage, @"C:\Users\dell\Desktop\0402G2test\1\1.bmp");
                    //HOperatorSet.ReadImage(out classApp.hoProcessThs[2], @"C:\Users\dell\Desktop\0402G2test\2\1.bmp");

                    Parameter.paramClass configTemple = Parameter.configClone(ithreadCheck);

                    if (configTemple.listOCRDetect.Count > 0)
                    {
                        #region  *** 带字符的检测  ***

                        string[] strLib = configTemple.listOCRDetect[0].strUsed.Split('#');
                        List<string> strParams = configLibGlobal.dictOCRDetect[strLib[0]];
                        strLib = strParams[0].Split('#');

                        string strModel = strLib[1]; 
                        strParams = configLibGlobal.dictLocPosition[strModel];
                        string[] strTemp = strParams[0].Split('#');

                        string[] strFindModel = strParams[2].Split('#');

                        string strRegion = strLib[2];

                        HTuple hvParamsData = new HTuple();
                        hvParamsData[0] = double.Parse(strTemp[5]);//模板中心X
                        hvParamsData[1] = double.Parse(strTemp[4]);//模板Y
                        hvParamsData[2] = double.Parse(strTemp[6]);//模板角度
                        hvParamsData[3] = double.Parse(strFindModel[4]);//匹配分数

                        string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\Regions";
                        strDirectory += "\\" + strRegion + ".tif";
                        HOperatorSet.ReadRegion(out hoRegion, strDirectory);

                        //HOperatorSet.WriteRegion((HObject)configLibGlobal.dictRegion[strRegion], "D:\\1.tif");
                        //HOperatorSet.WriteRegion(hoRegion, "D:\\1.tif");

                        syGuiJieProcess.Mainpro_MK(hoImage, hoRegion, hvParams, icheckMode, hvAllHandles[int.Parse(strTemp[0])],
                            hvParamsData, out hvResult, out hv_ShowPoint, out  hv_ShowLine, out hv_ShowCircle, out hv_ERR);

                        #endregion
                    }
                    else
                    {
                        syGuiJieProcess.Mainpro_800W_Old(hoImage, classApp.hoProcessThs[2] == null ? null : classApp.hoProcessThs[2].Clone(), hvParams, icheckMode, out hvResult,
                            out hv_ShowPoint, out  hv_ShowLine, out hv_ShowCircle, out hv_ERR);
                    }
                    //savePicThread(classApp.hoProcessThs[2], null, null, 2, -1, "NG", null);

                    if (hv_ERR.D == 1 && configLibGlobal.lstrAdvanced[36] == "1")
                    {
                        hv_ERR = 0d;
                    }

                    bool bcalcKeLi = false;
                    if (frm_StatMultiply.lstrParamsKeli.Count > 0)
                    {
                        string[] strmkLkp = frm_StatMultiply.lstrParamsKeli[0];
                        if (strmkLkp != null && strmkLkp.Length > 0 && strmkLkp[0] == "1")
                        {
                            bcalcKeLi = true;
                        }
                    }

                    List<RectangleF> lcheckCircle = new List<RectangleF>();
                    int iierror1 = 0;
                    int iierror2 = 0;

                    if (hv_ERR.D == 0 )
                    {
                        #region  *** 数据处理  ***

                        HTuple hvParamsData = new HTuple();
                        hvParamsData[0] = double.Parse(strUserParam[11]);//左右范围小
                        hvParamsData[1] = double.Parse(strUserParam[12]);//左右范围大
                        hvParamsData[2] = double.Parse(strUserParam[13]);//上下范围小
                        hvParamsData[3] = double.Parse(strUserParam[14]);//上下范围大
                        hvParamsData[4] = int.Parse(strUserParam[15]);
                        hvParamsData[5] = int.Parse(strUserParam[16]);
                        hvParamsData[6] = double.Parse(strUserParam[10]);
                        hvParamsData[7] = double.Parse(strUserParam[17]);
                        hvParamsData[8] = double.Parse(strUserParam[18]);
                        hvParamsData[9] = 1;
                        hvParamsData[10] = iParams[13];

                        hvParamsData[11] = double.Parse(strUserParam[23]);//2020.1.8修改
                        hvParamsData[12] = double.Parse(strUserParam[24]);
                        hvParamsData[13] = double.Parse(strUserParam[25]);
                        hvParamsData[14] = double.Parse(strUserParam[26]);
                 
                        if (lIsFirstPiece == 1)
                        {
                            #region  2019.6.24Param[12]中为区分首件还是正常检测的参数，1是正常检测，2是首件模式

                            hvParamsData[9] = 2;

                            if (strUserParam[1] == "1")//C2
                            {
                                #region  制程选择
                                if (strUserParam[0] == "0")//0201
                                {

                                }
                                else if (strUserParam[0] == "1")//0402
                                {

                                }
                                else if (strUserParam[0] == "2")//0603
                                {

                                }
                                else if (strUserParam[0] == "3")//0805
                                {

                                }
                                else if (strUserParam[0] == "4")//1206
                                {

                                }
                                #endregion

                                if (frmPrintProcesses.lstrPrintC2.Count < 9)
                                {
                                    dhDll.frmMsg.Log("制作C2首件的参数配置不正确", "Parameter invalid in making C2 first piece", null, dhDll.logDiskMode.Error, 0);
                                }
                                else
                                {
                                    hvParamsData[0] = double.Parse(frmPrintProcesses.lstrPrintC2[3]);
                                    hvParamsData[1] = double.Parse(frmPrintProcesses.lstrPrintC2[4]);
                                    hvParamsData[2] = double.Parse(frmPrintProcesses.lstrPrintC2[7]);
                                    hvParamsData[3] = double.Parse(frmPrintProcesses.lstrPrintC2[8]);
                                }
                            }
                            else if (strUserParam[1] == "2")//C1
                            {
                                if (frmPrintProcesses.lstrPrintC1.Count < 9)
                                {
                                    dhDll.frmMsg.Log("制作C1首件的参数配置不正确", "Parameter invalid in making C1 first piece", null, dhDll.logDiskMode.Error, 0);
                                }
                                else
                                {
                                    hvParamsData[0] = double.Parse(frmPrintProcesses.lstrPrintC1[3]);
                                    hvParamsData[1] = double.Parse(frmPrintProcesses.lstrPrintC1[4]);
                                    hvParamsData[2] = double.Parse(frmPrintProcesses.lstrPrintC1[7]);
                                    hvParamsData[3] = double.Parse(frmPrintProcesses.lstrPrintC1[8]);
                                }
                            }
                            else if (strUserParam[1] == "3")//R
                            {
                                if (frmPrintProcesses.lstrPrintR.Count < 9)
                                {
                                    dhDll.frmMsg.Log("制作R首件的参数配置不正确", "Parameter invalid in making R first piece", null, dhDll.logDiskMode.Error, 0);
                                }
                                else
                                {
                                    hvParamsData[0] = double.Parse(frmPrintProcesses.lstrPrintR[3]);
                                    hvParamsData[1] = double.Parse(frmPrintProcesses.lstrPrintR[4]);
                                    hvParamsData[2] = double.Parse(frmPrintProcesses.lstrPrintR[7]);
                                    hvParamsData[3] = double.Parse(frmPrintProcesses.lstrPrintR[8]);
                                }
                            }
                            else if (strUserParam[1] == "4")//G1
                            {
                                if (frmPrintProcesses.lstrPrintG1.Count < 9)
                                {
                                    dhDll.frmMsg.Log("制作G1首件的参数配置不正确", "Parameter invalid in making G1 first piece", null, dhDll.logDiskMode.Error, 0);
                                }
                                else
                                {
                                    hvParamsData[0] = double.Parse(frmPrintProcesses.lstrPrintG1[3]);
                                    hvParamsData[1] = double.Parse(frmPrintProcesses.lstrPrintG1[4]);
                                    hvParamsData[2] = double.Parse(frmPrintProcesses.lstrPrintG1[7]);
                                    hvParamsData[3] = double.Parse(frmPrintProcesses.lstrPrintG1[8]);
                                }
                            }
                            else if (strUserParam[1] == "5")//G2
                            {
                                if (frmPrintProcesses.lstrPrintG2.Count < 9)
                                {
                                    dhDll.frmMsg.Log("制作G2首件的参数配置不正确", "Parameter invalid in making G2 first piece", null, dhDll.logDiskMode.Error, 0);
                                }
                                else
                                {
                                    hvParamsData[0] = double.Parse(frmPrintProcesses.lstrPrintG2[3]);
                                    hvParamsData[1] = double.Parse(frmPrintProcesses.lstrPrintG2[4]);
                                    hvParamsData[2] = double.Parse(frmPrintProcesses.lstrPrintG2[7]);
                                    hvParamsData[3] = double.Parse(frmPrintProcesses.lstrPrintG2[8]);
                                }
                            }
                            else if (strUserParam[1] == "6")//MK
                            {
                                if (frmPrintProcesses.lstrPrintMK.Count < 9)
                                {
                                    dhDll.frmMsg.Log("制作MK首件的参数配置不正确", "Parameter invalid in making MK first piece", null, dhDll.logDiskMode.Error, 0);
                                }
                                else
                                {
                                    hvParamsData[0] = double.Parse(frmPrintProcesses.lstrPrintMK[3]);
                                    hvParamsData[1] = double.Parse(frmPrintProcesses.lstrPrintMK[4]);
                                    hvParamsData[2] = double.Parse(frmPrintProcesses.lstrPrintMK[7]);
                                    hvParamsData[3] = double.Parse(frmPrintProcesses.lstrPrintMK[8]);
                                }
                            }

                            #endregion
                        }

                        HTuple hv_strResult = null, hv_bResult = null, hv_error = null;
                        syGuiJieProcess.DataProcessing(iParams[0], iParams[1], iParams[2], hvParamsData, hvResult, icheckMode, hv_Result_bac, hv_ShowCircle_bac, hv_ShowCircle, out hv_strResult, out hv_bResult, out hv_error);
                       
                        hv_Result_bac = hvResult;
                        hv_ShowCircle_bac = hv_ShowCircle;
                        hv_strResult_bac = hv_strResult;

                        bool bOkNgResult = true;
                        if (hv_strResult.Length == hv_bResult.Length)
                        {

                            string[] strMsg = hv_strResult.SArr;

                            List<string> lstrTemp = new List<string>(strMsg);
                            for (int ihh = lstrTemp.Count - 1; ihh >= 0; ihh--)
                            {
                                if (lstrTemp[ihh] == "" || lstrTemp[ihh].Length < 2)
                                    lstrTemp.RemoveAt(ihh);
                            }
                            strMsg = lstrTemp.ToArray();

                            int[] iMsg = null;
                            try
                            {
                                iMsg = hv_bResult.IArr;
                            }
                            catch
                            {
                                long[] lmxxMsg = hv_bResult.LArr;
                                iMsg = new int[lmxxMsg.Length];

                                for (int ihh = 0; ihh < iMsg.Length; ihh++)
                                {
                                    iMsg[ihh] = (int)lmxxMsg[ihh];
                                }
                            }

                            for (int igg = 0; igg < strMsg.Length; igg++)
                            {
                                listInfo2Draw.Add(strMsg[igg]);
                                int iokng = iMsg[igg];
                                if (iokng == 0) bOkNgResult = false;
                                listInfo2Draw.Add(iMsg[igg] == 1 ? "OK" : "NG");
                            }

                            if(strMsg.Length>=2)
                            {
                                string[] strerror1 = strMsg[strMsg.Length - 2].Split(':');
                                string[] strerror2 = strMsg[strMsg.Length - 1].Split(':');

                                if (strerror1.Length == 2) iierror1 = int.Parse(strerror1[1].Trim());
                                if (strerror2.Length == 2) iierror2 = int.Parse(strerror2[1].Trim());
                            }
                            
                        }

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(listInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        if (hv_ShowPoint.Length > 0)
                        {
                            double[] dPoint = new double[hv_ShowPoint.Length];
                            for (int ihh = 0; ihh < hv_ShowPoint.Length; ihh++)
                            {
                                dPoint[ihh] = (double)hv_ShowPoint[ihh];
                            }
                            for (int igg = 0; igg < dPoint.Length - 1; igg += 2)
                            {
                                listObj2Draw.Add("小十字");
                                listObj2Draw.Add(new PointF((float)dPoint[igg + 1], (float)dPoint[igg]));
                                listObj2Draw.Add("OK");
                            }
                        }
                        if (hv_ShowLine.Length > 0)
                        {
                            double[] dLine = new double[hv_ShowLine.Length];
                            for (int ihh = 0; ihh < hv_ShowLine.Length; ihh++)
                            {
                                dLine[ihh] = (double)hv_ShowLine[ihh];
                            }
                            //double[] dLine = hv_ShowLine.DArr;
                            for (int igg = 0; igg < dLine.Length - 3; igg += 4)
                            {
                                listObj2Draw.Add("线");
                                RectangleF recLine = new RectangleF((float)dLine[igg + 1], (float)dLine[igg], (float)dLine[igg + 3], (float)dLine[igg + 2]);
                                listObj2Draw.Add(recLine);
                                listObj2Draw.Add("OK");
                            }

                        }

    
                        if (hv_ShowCircle.Length > 0)
                        {
                            //listObj2Draw[1] = "NG";

                            double[] dCircle = new double[hv_ShowCircle.Length];
                            for (int ihh = 0; ihh < hv_ShowCircle.Length; ihh++)
                            {
                                dCircle[ihh] = (double)hv_ShowCircle[ihh];
                            }
                            //double[] dCircle = hv_ShowCircle.DArr;
                            for (int igg = 0; igg < dCircle.Length - 2; igg += 3)
                            {
                                listObj2Draw.Add("圆");
                                RectangleF recCircle = new RectangleF((float)dCircle[igg + 1], (float)dCircle[igg], (float)dCircle[igg + 2], (float)dCircle[igg + 2]);
                                listObj2Draw.Add(recCircle);
                                listObj2Draw.Add("NG");

                                lcheckCircle.Add(recCircle);
                            }

                        }

                



                        if (hv_error.D != 0)
                        {
                            #region  *** 显示 NG-内容  ***

                            if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                            {
                                if (frmErrorType.lstrErrorType.Count > 23)
                                {
                                    listObj2Draw[1] = frmErrorType.lstrErrorType[(int)hv_error.D];
                                }
                                else
                                {
                                    if (hv_error.D == 1) listObj2Draw[1] = "NG-尺寸超标并塞网";
                                    if (hv_error.D == 2) listObj2Draw[1] = "NG-网板尺寸超标";
                                    if (hv_error.D == 3) listObj2Draw[1] = "NG-塞网数超标";
                                    if (hv_error.D == 4) listObj2Draw[1] = "NG-塞网";
                                    //if (hv_error.D == 5) listObj2Draw[1] = "NG-塞网";
                                    if (hv_error.D == 6) listObj2Draw[1] = "NG-上下尺寸超标";
                                    if (hv_error.D == 7) listObj2Draw[1] = "NG-左右尺寸超标";
                                    if (hv_error.D == 8) listObj2Draw[1] = "NG-网版尺寸超标";
                                    if (hv_error.D == 9) listObj2Draw[1] = "NG-上下尺寸超标+左右尺寸超标";
                                    if (hv_error.D == 10) listObj2Draw[1] = "NG-上下尺寸超标+网版尺寸超标";
                                    if (hv_error.D == 11) listObj2Draw[1] = "NG-左右尺寸超标+网版尺寸超标";
                                    if (hv_error.D == 12) listObj2Draw[1] = "NG-上下尺寸超标+左右尺寸超标+网版尺寸超标";

                                    if (hv_error.D == 13) listObj2Draw[1] = "NG-上下尺寸超标+塞网 ";
                                    if (hv_error.D == 14) listObj2Draw[1] = "NG-左右尺寸超标+塞网";
                                    if (hv_error.D == 15) listObj2Draw[1] = "NG-网版尺寸超标+塞网";
                                    if (hv_error.D == 16) listObj2Draw[1] = "NG-上下尺寸超标+左右尺寸超标+塞网";
                                    if (hv_error.D == 17) listObj2Draw[1] = "NG-上下尺寸超标+网版尺寸超标+塞网";
                                    if (hv_error.D == 18) listObj2Draw[1] = "NG-左右尺寸超标+网版尺寸超标+塞网";
                                    if (hv_error.D == 19) listObj2Draw[1] = "NG-上下尺寸超标+左右尺寸超标+网版尺寸超标+塞网";

                                    //2019.5.21
                                    if (hv_error.D == 20) listObj2Draw[1] = "NG-尺寸不良+漏浆";
                                    if (hv_error.D == 21) listObj2Draw[1] = "NG-尺寸不良+脏污";
                                    if (hv_error.D == 22) listObj2Draw[1] = "NG-漏浆";
                                    if (hv_error.D == 23) listObj2Draw[1] = "NG-脏污";
                                }
                            

                            }
                            else
                            {
                                if (frmErrorType.lstrErrorType.Count > 23)
                                {
                                    listObj2Draw[1] = frmErrorType.lstrErrorType[(int)hv_error.D];
                                }
                                else
                                {
                                    if (hv_error.D == 1) listObj2Draw[1] = "NG-Defect and size error";
                                    if (hv_error.D == 2) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 3) listObj2Draw[1] = "NG-Defect";
                                    if (hv_error.D == 4) listObj2Draw[1] = "NG-Defect";
                                    //if (hv_error.D == 5) listObj2Draw[1] = "NG-塞网";
                                    if (hv_error.D == 6) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 7) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 8) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 9) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 10) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 11) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 12) listObj2Draw[1] = "NG-Size error";

                                    if (hv_error.D == 13) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 14) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 15) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 16) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 17) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 18) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 19) listObj2Draw[1] = "NG-Size error";

                                    //2019.5.21
                                    if (hv_error.D == 20) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 21) listObj2Draw[1] = "NG-Size error";
                                    if (hv_error.D == 22) listObj2Draw[1] = "NG-Dirty";
                                    if (hv_error.D == 23) listObj2Draw[1] = "NG-Dirty";

                                    //if (hv_error.D == 1) listObj2Draw[1] = "NG-Defect and size error";
                                    //if (hv_error.D == 2) listObj2Draw[1] = "NG-Size error";
                                    //if (hv_error.D == 3) listObj2Draw[1] = "NG-Counts of Defect";
                                    //if (hv_error.D == 4) listObj2Draw[1] = "NG-Defect";
                                    ////if (hv_error.D == 5) listObj2Draw[1] = "NG-塞网";
                                    //if (hv_error.D == 6) listObj2Draw[1] = "NG-Updown size error";
                                    //if (hv_error.D == 7) listObj2Draw[1] = "NG-LeftR size error";
                                    //if (hv_error.D == 8) listObj2Draw[1] = "NG-Size error";
                                    //if (hv_error.D == 9) listObj2Draw[1] = "NG-Updown and LeftR  size error";
                                    //if (hv_error.D == 10) listObj2Draw[1] = "NG-Updown and size error";
                                    //if (hv_error.D == 11) listObj2Draw[1] = "NG-LeftR and size error";
                                    //if (hv_error.D == 12) listObj2Draw[1] = "NG-Updown and LeftR  and size error";

                                    //if (hv_error.D == 13) listObj2Draw[1] = "NG-Updown size and defect";
                                    //if (hv_error.D == 14) listObj2Draw[1] = "NG-LeftR size and defect";
                                    //if (hv_error.D == 15) listObj2Draw[1] = "NG-Size and defect";
                                    //if (hv_error.D == 16) listObj2Draw[1] = "NG-Updown and LeftR  size and defect";
                                    //if (hv_error.D == 17) listObj2Draw[1] = "NG-Updown and size and defect";
                                    //if (hv_error.D == 18) listObj2Draw[1] = "NG-LeftR and size and defect";
                                    //if (hv_error.D == 19) listObj2Draw[1] = "NG-Updown and LeftR  and size and defect";

                                    ////2019.5.21
                                    //if (hv_error.D == 20) listObj2Draw[1] = "NG-Size error + Paste adhesion";
                                    //if (hv_error.D == 21) listObj2Draw[1] = "NG-Size error + Dirty";
                                    //if (hv_error.D == 22) listObj2Draw[1] = "NG-Paste adhesion";
                                    //if (hv_error.D == 23) listObj2Draw[1] = "NG-Dirty";
                                }


                            }
                            #endregion

                            if (hv_error.D == 4)
                            {
                                #region ****** 2018.9.6,如果塞网，可以设置一个区域，区域内的塞网不检测 ******

                                bool bContains = false;
                                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                                {
                                    if (lkkPolygon[igg][0].X == 8)
                                    {
                                        PointF pgg1 = lkkPolygon[igg][1];
                                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                                        RectangleF recTemp = new RectangleF(pgg1.X, pgg1.Y, pgg2.X, pgg2.Y);

                                        for (int ihh = 0; ihh < lcheckCircle.Count; ihh++)
                                        {
                                            if (recTemp.Contains(lcheckCircle[ihh].Location))
                                            {
                                                bContains = true;
                                            }
                                        }


                                    }
                                    if (lkkPolygon[igg][0].X == 3)
                                    {
                                        PointF pgg1 = lkkPolygon[igg][1];
                                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                                        //HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                                        //HOperatorSet.Union2(hoUnion, hoRegion, out hoUnion);
                                    }


                                }

                                if (bContains)
                                {
                                    if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                                    {
                                        listObj2Draw[1] = "OK-忽略";
                                    }
                                    else
                                    {
                                        listObj2Draw[1] = "OK-Ignored";
                                    }

                                }


                                #endregion
                            }

                            if (configTemple.listOCRDetect.Count == 0)
                            {
                                savePicsShip(classApp.hoProcessThs[2], null, null, 2, -1, "NG", null,new dhDll.KickEnResults());
                                Thread.Sleep(10);
                            }
                        }

                        #endregion
                    }
                    else if (hv_ERR.D == 1)
                    {
                        if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                        {
                            listObj2Draw[1] = "OK-无判定";
                        }
                        else
                        {
                            listObj2Draw[1] = "OK-No decision";
                        }
                    }
                    else if (hv_ERR.D == 2)
                    {
                        if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                        {
                            listObj2Draw[1] = "OK-无基板";
                        }
                        else
                        {
                            listObj2Draw[1] = "OK-None Substrate";
                        }

                        //统计数值要减去这个值2019.4.25
                        //if (!bcalcKeLi)
                        //{
                            Interlocked.Decrement(ref dhDll.clsss_Status.lNumberThreads[1][0]);
                            Interlocked.Decrement(ref dhDll.clsss_Status.lNumberThreads[1][1]);
                       // }

                        Interlocked.Decrement(ref frmOEE.lOeeCounts[0]);
                        Interlocked.Decrement(ref frmOEE.lOeeCounts[1]);
                    }
                    else if (hv_ERR.D == 3)
                    {
                        if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                        {
                            listObj2Draw[1] = "NG-未印刷";
                        }
                        else
                        {
                            listObj2Draw[1] = "NG-Not Printed";
                        }


                        if (configTemple.listOCRDetect.Count == 0)
                        {
                            savePicsShip(classApp.hoProcessThs[2], null, null, 2, 1, "NG", null, new dhDll.KickEnResults());
                            Thread.Sleep(10);
                        }
                    }
                    else if (hv_ERR.D == 4)
                    {
                        if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                        {
                            listObj2Draw[1] = "NG-混料";
                        }
                        else
                        {
                            listObj2Draw[1] = "NG-Mixed Substrate";
                        }


                        if (configTemple.listOCRDetect.Count == 0)
                        {
                            savePicsShip(classApp.hoProcessThs[2], null, null, 2, -1, "NG", null, new dhDll.KickEnResults());
                            Thread.Sleep(10);
                        }
                    }
                    else if (hv_ERR.D == 8)
                    {
                        if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                        {
                            listObj2Draw[1] = "NG-方向出错";
                        }
                        else
                        {
                            listObj2Draw[1] = "NG-Orientation Error";
                        }


                        if (configTemple.listOCRDetect.Count == 0)
                        {
                            savePicsShip(classApp.hoProcessThs[2], null, null, 2, -1, "NG", null, new dhDll.KickEnResults());
                            Thread.Sleep(10);
                        }
                    }


                    if (bcalcKeLi && hv_ERR.D != 2)
                    {
                        #region   ***  计算颗粒  ***

                        string strProduct = dhDll.frmVarietyMg.strCurrentProduct;
                        bool bfindXingBie = false;
                        if (strProduct.Length < 5)
                        {
                            dhDll.frmMsg.Log("产品名称长度小于6，无法获取型别，无法进行颗粒计算。", "", null, dhDll.logDiskMode.Error, 0);
                        }
                        else
                        {
                            if (strProduct.Length == 5) strProduct = strProduct.Substring(0, 5);
                            else strProduct = strProduct.Substring(0, 6);
                            for (int igg = 2; igg < frm_StatMultiply.lstrParamsKeli.Count; igg++)
                            {
                                string[] strmkLkp = frm_StatMultiply.lstrParamsKeli[igg];
                                if (strmkLkp != null && strmkLkp.Length > 3)
                                {
                                    if (strProduct.Contains(strmkLkp[0]))
                                    {
                                        bfindXingBie = true;

                                        //Interlocked.Decrement(ref dhDll.clsss_Status.lNumberThreads[1][0]);
                                        string strresult = listObj2Draw[1] as string;

                                        int isum = int.Parse(strmkLkp[3]);
                                        dhDll.clsss_Status.lNumberThreads[1][30] += isum;

                                        if (strresult.Contains("OK"))
                                        {
                                            //Interlocked.Decrement(ref dhDll.clsss_Status.lNumberThreads[1][1]);

                                            dhDll.clsss_Status.lNumberThreads[1][31] += isum;
                                        }
                                        else
                                        {
                                            //Interlocked.Decrement(ref dhDll.clsss_Status.lNumberThreads[1][2]);

                                            dhDll.clsss_Status.lNumberThreads[1][32] += lcheckCircle.Count;
                                            dhDll.clsss_Status.lNumberThreads[1][31] += isum - lcheckCircle.Count;

                                            dhDll.clsss_Status.lNumberThreads[1][33] += iierror1;
                                            dhDll.clsss_Status.lNumberThreads[1][34] += iierror2;
                                        }

                                        //dhDll.frmMsg.Log(isum.ToString() + "," + lcheckCircle.Count.ToString(), "", null, dhDll.logDiskMode.Error, 0);



                                        break;
                                    }
                                }
                            }
                            if (!bfindXingBie)
                            {
                                dhDll.frmMsg.Log("无法获取型别类型，无法进行颗粒计算。", "", null, dhDll.logDiskMode.Error, 0);
                            }
                        }

                        #endregion
                    }

                    #region  ******  定时清理内存  *****

                    GC.Collect();

                    lIsFirstPiece = 0;

                    //GC.WaitForPendingFinalizers();

                    //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    //{
                    //    class_Win32.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                    //}

                    #endregion


                    #endregion
                }
                else if (strUserParam[0] == "15")
                {
                    #region  *** 印刷机后端 2019.4.16更改 ***

                    listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

                    //HOperatorSet.ReadImage(out hoImage, @"D:\11.bmp");
                    //HOperatorSet.ReadImage(out classApp.hoProcessThs[2], @"D:\12.bmp");

                    #region  *** 参数设置  ***

                    Parameter.paramClass configTemple = Parameter.configClone(ithreadCheck);

                    HTuple hvShowSY = new HTuple();
                    HTuple hv_strResult = null;
                    HTuple hvParameters = new HTuple();

                    //左右范围(0,1)，上下范围(2,3)，到剥离线范围(4,5),塞网颗数(6),脏污颗数(7),标定值(8),是否检测尺寸(9),是否检测塞网(10),是否检测脏污(11)

                    hvParameters[0] = double.Parse(strUserParam[12]);//-0.1d;//左右范围最小
                    hvParameters[1] = double.Parse(strUserParam[13]);//0.1d;//左右范围最大
                    hvParameters[2] = double.Parse(strUserParam[14]);//-0.1d;
                    hvParameters[3] = double.Parse(strUserParam[15]);//0.11d;
                    hvParameters[4] = double.Parse(strUserParam[18]);//1d;//到剥离线范围
                    hvParameters[5] = double.Parse(strUserParam[19]);//1d;//到剥离线范围
                    hvParameters[6] = double.Parse(strUserParam[16]);//5d;//塞网颗数 塞网数超标
                    hvParameters[7] = double.Parse(strUserParam[17]);//5d;//脏污颗数 
                    hvParameters[8] = double.Parse(strUserParam[11]);// 0.0183d;//标定值
                    hvParameters[9] = 1d;//是否检测尺寸  1 or 0

                    hvParameters[10] = double.Parse(strUserParam[7]);//1d;//是否检测塞网  1 or 0
                    hvParameters[11] = double.Parse(strUserParam[10]);//1d;//是否检测脏污  1 or 0
                    hvParameters[12] = 1d;//CustomerCode  不同厂家显示不同的信息
                    hvParameters[13] = 1d;
                    hvParameters[14] = 1d;
                    hvParameters[15] = 1d;
                    hvParameters[16] = 1d;
                    hvParameters[17] = 1d;
                    hvParameters[18] = 1d;
                    hvParameters[19] = 1d;

                    hvParameters[20] = double.Parse(strUserParam[1]);//4d;//型别 =4   0201=0  0402=1  0603=2  0805=3  1206=4
                    hvParameters[21] = double.Parse(strUserParam[2]);//1d;//工序 =1  K=0  C2=1  C1=2  R=3  G1=4  G2=5  MK=6
                    hvParameters[22] = double.Parse(strUserParam[3]);//2d;//类型 =2  1是颗粒，2是条形
                    hvParameters[23] = double.Parse(strUserParam[31]);//155d;//提取基板二值化,0-255
                    hvParameters[24] = double.Parse(strUserParam[34]);//3000d;//MinWidth:=3000    *筛选基板宽度最小值0-6000
                    hvParameters[25] = double.Parse(strUserParam[35]);//1500d;//MinHeight:=1500   *筛选基板高度最小值0-6000
                    hvParameters[26] = double.Parse(strUserParam[37]);//150d;//GrayValue:=150    *正光提取印刷图案二值化0-255
                    hvParameters[27] = double.Parse(strUserParam[40]);//15d;//MinWidth:=15      *筛选图案宽度最小值,0-6000
                    hvParameters[28] = double.Parse(strUserParam[41]);//200d;//MaxWidth:=200     *筛选图案宽度最大值,0-6000
                    hvParameters[29] = double.Parse(strUserParam[42]);//10d;//MinHeight:=10     *筛选图案高度最小值,0-6000

                    hvParameters[30] = double.Parse(strUserParam[43]);//10000d;//MaxHeight:=10000  *筛选图案高度最大值,0-6000
                    hvParameters[31] = double.Parse(strUserParam[44]);//1.5d;//Closing:=1.5      *将所有印刷图案整合到一起以便筛选,0-25.5
                    hvParameters[32] = double.Parse(strUserParam[46]);//1d;//RowNumber:=1      *行数
                    hvParameters[33] = double.Parse(strUserParam[47]);//1d;//ColNumber:=1      *列数
                    hvParameters[34] = double.Parse(strUserParam[48]);//1d;//column:=1         *找第几列的图案
                    hvParameters[35] = double.Parse(strUserParam[49]);//1d;//row:=1            *找第几个图案
                    hvParameters[36] = double.Parse(strUserParam[50]);//131d;//dilation:=131     *通过膨胀将图案合成几列的膨胀系数
                    hvParameters[37] = double.Parse(strUserParam[52]);//350d;//OffsetX1:=350     *水平线段偏移量1(-500-500)
                    hvParameters[38] = double.Parse(strUserParam[55]);//0d;//OffsetX2:=0       *水平线段偏移量2(-500-500)
                    hvParameters[39] = double.Parse(strUserParam[58]);//300d;//OffsetY1:=300     *竖直方向偏移量1(-500-500)

                    hvParameters[40] = double.Parse(strUserParam[61]);//0d;//OffsetY2:=0       *竖直方向偏移量2(-500-500)
                    hvParameters[41] = double.Parse(strUserParam[64]);//400d;//OffsetLineX:=400  *水平整体线段偏移量（-500-500）
                    hvParameters[42] = double.Parse(strUserParam[67]);//0d;//OffsetLineY:=0    *竖直整体线段偏移量（-500-500）
                    hvParameters[43] = double.Parse(strUserParam[70]);//2d;//Mode:=2           *图形格式，1位方形，2位条形
                    hvParameters[44] = double.Parse(strUserParam[71]);//10d;//AmplitudeThreshold:=10
                    hvParameters[45] = double.Parse(strUserParam[72]);//3d;//Sigma:=3
                    hvParameters[46] = double.Parse(strUserParam[73]);//10d;//ROIWidth:=10    
                    hvParameters[47] = double.Parse(strUserParam[74]);//2d;//FindMode:=2       *0表示第一点和最后一点，1表示第一点和第二点，2表示倒数第二点和倒数
                    hvParameters[48] = double.Parse(strUserParam[80]);//10d;//MinArea:=10       *缺失面积
                    hvParameters[49] = double.Parse(strUserParam[81]);// 140d;//NGGray:=140       *缺失二值化(1-255)

                    hvParameters[50] = double.Parse(strUserParam[84]);// 10d;//DirtyArea:=10     *脏污面积
                    hvParameters[51] = double.Parse(strUserParam[85]);//130d;//DirtyGray:=100    *脏污二值化(0-254)
                    hvParameters[52] = double.Parse(strUserParam[88]);//3d;//DilationValue:=3  *膨胀系数（对提取缺陷的区域进行膨胀）
                    hvParameters[53] = double.Parse(strUserParam[89]);//3d;//ErosionValue:=3   *腐蚀系数（对提取缺陷的区域进行腐蚀）
                    hvParameters[54] = 1d;
                    hvParameters[55] = 1d;
                    hvParameters[56] = 1d;
                    hvParameters[57] = 1d;
                    hvParameters[58] = 1d;
                    hvParameters[59] = 1d;

                    #endregion

                    #region  *** 主处理函数  ***

                    if (hvParameters_old_print15.Length < 10) hvParameters_old_print15 = hvParameters.Clone();

                    //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                    //sw.Start();

                    syGuiJieNew.MainPro(hoImage, classApp.hoProcessThs[2] == null ? null : classApp.hoProcessThs[2].Clone(), new HObject(), out hoRegion, 0, 0, hvParameters, hvParameters_old_print15, 0, 0, out hvShowSY, out hv_strResult, out hvResult, out hv_ERR);

                    //sw.Stop();
                    //System.Windows.Forms.MessageBox.Show(sw.ElapsedMilliseconds.ToString());


                    hvParameters_old_print15 = hvParameters.Clone();

                    #endregion

                    #region  *** 报错类型分类  ***

                    List<RectangleF> lcheckCircle = new List<RectangleF>();

                    if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                    {
                        if (hv_ERR.D == 1) listObj2Draw[1] = "NG-印刷偏移并塞网";
                        if (hv_ERR.D == 2) listObj2Draw[1] = "NG-印刷偏移";
                        if (hv_ERR.D == 3) listObj2Draw[1] = "NG-塞网数超标";
                        if (hv_ERR.D == 4) listObj2Draw[1] = "NG-塞网";
                        //if (hv_error.D == 5) listObj2Draw[1] = "NG-塞网";
                        if (hv_ERR.D == 6) listObj2Draw[1] = "NG-上下印刷偏移";
                        if (hv_ERR.D == 7) listObj2Draw[1] = "NG-左右印刷偏移";
                        if (hv_ERR.D == 8) listObj2Draw[1] = "NG-印刷偏移";
                        if (hv_ERR.D == 9) listObj2Draw[1] = "NG-上下印刷偏移+左右印刷偏移";
                        if (hv_ERR.D == 10) listObj2Draw[1] = "NG-上下印刷偏移+印刷偏移";
                        if (hv_ERR.D == 11) listObj2Draw[1] = "NG-左右印刷偏移+印刷偏移";
                        if (hv_ERR.D == 12) listObj2Draw[1] = "NG-上下印刷偏移+左右印刷偏移+印刷偏移";

                        if (hv_ERR.D == 13) listObj2Draw[1] = "NG-上下印刷偏移+塞网 ";
                        if (hv_ERR.D == 14) listObj2Draw[1] = "NG-左右印刷偏移+塞网";
                        if (hv_ERR.D == 15) listObj2Draw[1] = "NG-印刷偏移+塞网";
                        if (hv_ERR.D == 16) listObj2Draw[1] = "NG-上下印刷偏移+左右印刷偏移+塞网";
                        if (hv_ERR.D == 17) listObj2Draw[1] = "NG-上下印刷偏移+网版印刷偏移+塞网";
                        if (hv_ERR.D == 18) listObj2Draw[1] = "NG-左右印刷偏移+网版印刷偏移+塞网";
                        if (hv_ERR.D == 19) listObj2Draw[1] = "NG-上下印刷偏移+左右印刷偏移+网印刷偏移+塞网";

                        if (hv_ERR.D == 100) listObj2Draw[1] = "OK-无基板";
                        if (hv_ERR.D == 101) listObj2Draw[1] = "NG-未印刷";
                        if (hv_ERR.D == 102) listObj2Draw[1] = "NG-混料";
                        if (hv_ERR.D == 103) listObj2Draw[1] = "NG-方向出错";


                        //ERR=6NG-上下尺寸超标
                        //ERR=7NG-左右尺寸超标
                        //ERR=8NG-网版尺寸超标
                        //ERR=9NG-上下尺寸超标+左右尺寸超标
                        //ERR=10NG-上下尺寸超标+网版尺寸超标
                        //ERR=11NG-左右尺寸超标+网版尺寸超标
                        //ERR=12NG-上下尺寸超标+左右尺寸超标+网版尺寸超标

                        //ERR:=13*上下尺寸超标+塞网        
                        //ERR:=14*左右尺寸超标+塞网               
                        //ERR:=15*网版尺寸超标+塞网             
                        //ERR:=16*上下尺寸超标+左右尺寸超标+塞网                
                        //ERR:=17*上下尺寸超标+网版尺寸超标+塞网                  
                        //ERR:=18*左右尺寸超标+网版尺寸超标+塞网                 
                        //ERR:=19*上下尺寸超标+左右尺寸超标+网版尺寸超标+塞网
                    }
                    else
                    {
                        if (hv_ERR.D == 1) listObj2Draw[1] = "NG-Defect and size error";
                        if (hv_ERR.D == 2) listObj2Draw[1] = "NG-Size error";
                        if (hv_ERR.D == 3) listObj2Draw[1] = "NG-Counts of Defect";
                        if (hv_ERR.D == 4) listObj2Draw[1] = "NG-Defect";
                        //if (hv_error.D == 5) listObj2Draw[1] = "NG-塞网";
                        if (hv_ERR.D == 6) listObj2Draw[1] = "NG-Updown size error";
                        if (hv_ERR.D == 7) listObj2Draw[1] = "NG-LeftR size error";
                        if (hv_ERR.D == 8) listObj2Draw[1] = "NG-Size error";
                        if (hv_ERR.D == 9) listObj2Draw[1] = "NG-Updown and LeftR  size error";
                        if (hv_ERR.D == 10) listObj2Draw[1] = "NG-Updown and size error";
                        if (hv_ERR.D == 11) listObj2Draw[1] = "NG-LeftR and size error";
                        if (hv_ERR.D == 12) listObj2Draw[1] = "NG-Updown and LeftR  and size error";

                        if (hv_ERR.D == 13) listObj2Draw[1] = "NG-Updown size and defect";
                        if (hv_ERR.D == 14) listObj2Draw[1] = "NG-LeftR size and defect";
                        if (hv_ERR.D == 15) listObj2Draw[1] = "NG-Size and defect";
                        if (hv_ERR.D == 16) listObj2Draw[1] = "NG-Updown and LeftR  size and defect";
                        if (hv_ERR.D == 17) listObj2Draw[1] = "NG-Updown and size and defect";
                        if (hv_ERR.D == 18) listObj2Draw[1] = "NG-LeftR and size and defect";
                        if (hv_ERR.D == 19) listObj2Draw[1] = "NG-Updown and LeftR  and size and defect";

                        if (hv_ERR.D == 100) listObj2Draw[1] = "OK-None Substrate";
                        if (hv_ERR.D == 101) listObj2Draw[1] = "NG-Not Printed";
                        if (hv_ERR.D == 102) listObj2Draw[1] = "NG-Mixed Substrate";
                        if (hv_ERR.D == 103) listObj2Draw[1] = "NG-Orientation Error";
                        //if (hv_ERR.D == 104) listObj2Draw[1] = "OK-None Substrate";
                    }

                    #endregion

                    #region  *** 绘制图形和信息  ***

                    if (hv_strResult.Length == hvResult.Length)
                    {
                        string[] strMsg = hv_strResult.SArr;
                        int[] iMsg = hvResult.IArr;

                        for (int igg = 0; igg < strMsg.Length; igg++)
                        {
                            listInfo2Draw.Add(strMsg[igg]);
                            int iokng = iMsg[igg];
                            listInfo2Draw.Add(iMsg[igg] == 1 ? "OK" : "NG");
                        }
                    }

                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(listInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    HObject hoConnection,hoSelect;
                    HTuple hvCount;
                    HOperatorSet.Connection(hoRegion, out hoConnection);
                    HOperatorSet.CountObj(hoConnection, out hvCount);
                    for (int igg = 1; igg <= hvCount.I; igg++)
                    {
                        HOperatorSet.SelectObj(hoConnection, out hoSelect, igg);
                        HOperatorSet.GetRegionContour(hoSelect, out hvRow, out hvColumn);
                        List<PointF> lpointf = new List<PointF>();

                        for (int ihh = 0; ihh < hvRow.Length; ihh++)
                        {
                            lpointf.Add(new PointF((float)hvColumn.LArr[ihh], (float)hvRow.LArr[ihh]));
                        }
                        listObj2Draw.Add("轮廓");
                        listObj2Draw.Add(lpointf.ToArray());
                        listObj2Draw.Add("OK");
                    }


                    //List<PointF> lpointf = new List<PointF>();
                    //for (int igg = 0; igg < hvRow.Length; igg++)
                    //{
                    //    if (hvRow.LArr[igg] == -999)
                    //    {
                    //        listObj2Draw.Add("轮廓");
                    //        listObj2Draw.Add(lpointf.ToArray());
                    //        listObj2Draw.Add("OK");

                    //        lpointf.Clear();
                    //    }
                    //    else
                    //    {
                    //        lpointf.Add(new PointF((float)hvColumn.LArr[igg], (float)hvRow.LArr[igg]));
                    //    }
                    //}


                    if (hvShowSY.Length > 0)
                    {
                        string[] strDrawing = hvShowSY.SArr;
                        for (int ihh = 0; ihh < strDrawing.Length - 2; ihh += 3)
                        {
                            string strResult = strDrawing[ihh + 2];
                            if (strDrawing[ihh] == "轮廓")
                            {
                                string[] strContour = strDrawing[ihh + 1].Split(',');
                                List<PointF> lcontour = new List<PointF>();
                                for (int ikk = 1; ikk < strContour.Length - 1; ikk += 2)
                                {
                                    lcontour.Add(new PointF(float.Parse(strContour[ikk + 1]), float.Parse(strContour[ikk])));
                                }

                                listObj2Draw.Add("轮廓");
                                listObj2Draw.Add(lcontour.ToArray());
                                listObj2Draw.Add(strResult);


                            }
                            else if (strDrawing[ihh] == "点")
                            {
                                string[] strPoints = strDrawing[ihh + 1].Split(',');
                                for (int ikk = 1; ikk < strPoints.Length - 1; ikk += 2)
                                {
                                    listObj2Draw.Add("小十字");
                                    listObj2Draw.Add(new PointF(float.Parse(strPoints[ikk + 1]), float.Parse(strPoints[ikk])));
                                    listObj2Draw.Add(strResult);
                                }

                            }
                            else if (strDrawing[ihh] == "线")
                            {
                                string[] strLines = strDrawing[ihh + 1].Split(',');
                                for (int ikk = 1; ikk < strLines.Length - 3; ikk += 4)
                                {
                                    listObj2Draw.Add("线段");
                                    listObj2Draw.Add(new RectangleF(float.Parse(strLines[ikk + 1]), float.Parse(strLines[ikk]), float.Parse(strLines[ikk + 3]), float.Parse(strLines[ikk + 2])));
                                    listObj2Draw.Add(strResult);
                                }
                            }
                            else if (strDrawing[ihh] == "圆")
                            {
                                string[] strCircles = strDrawing[ihh + 1].Split(',');
                                for (int ikk = 1; ikk < strCircles.Length - 2; ikk += 3)
                                {
                                    listObj2Draw.Add("圆");
                                    listObj2Draw.Add(new RectangleF(float.Parse(strCircles[ikk + 1]), float.Parse(strCircles[ikk]), float.Parse(strCircles[ikk + 2]), float.Parse(strCircles[ikk + 2])));
                                    listObj2Draw.Add(strResult);
                                }

                                //    listObj2Draw.Add("圆");
                                //    RectangleF recCircle = new RectangleF((float)dCircle[igg + 1], (float)dCircle[igg], (float)dCircle[igg + 2], (float)dCircle[igg + 2]);
                                //    listObj2Draw.Add(recCircle);
                                //    listObj2Draw.Add("NG");

                                //    lcheckCircle.Add(recCircle);
                            }


                        }
                    }

                    #endregion

                    if (hv_ERR.D == 4)
                    {
                        #region ****** 2018.9.6,如果塞网，可以设置一个区域，区域内的塞网不检测 ******

                        bool bContains = false;
                        for (int igg = 0; igg < lkkPolygon.Count; igg++)
                        {
                            if (lkkPolygon[igg][0].X == 8)
                            {
                                PointF pgg1 = lkkPolygon[igg][1];
                                PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                                RectangleF recTemp = new RectangleF(pgg1.X, pgg1.Y, pgg2.X, pgg2.Y);

                                for (int ihh = 0; ihh < lcheckCircle.Count; ihh++)
                                {
                                    if (recTemp.Contains(lcheckCircle[ihh].Location))
                                    {
                                        bContains = true;
                                    }
                                }


                            }
                            if (lkkPolygon[igg][0].X == 3)
                            {
                                PointF pgg1 = lkkPolygon[igg][1];
                                PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                                double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                                //HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                                //HOperatorSet.Union2(hoUnion, hoRegion, out hoUnion);
                            }


                        }

                        if (bContains)
                        {
                            if (dhDll.frmVarietyMg.strLanguage.ToUpper().Contains("CHINESE"))
                            {
                                listObj2Draw[1] = "OK-Defect ignored";
                            }
                            else
                            {
                            }

                        }


                        #endregion
                    }

                    if (classApp.hoProcessThs[2] != null && ((string)listObj2Draw[1]).Contains("NG"))
                    {
                        savePicsShip(classApp.hoProcessThs[2].Clone(), null, null, 2, -1, "NG", null, new dhDll.KickEnResults());
                        Thread.Sleep(10);
                    }

                    #region  ******  定时清理内存  *****

                    //GC.Collect();
                    ////GC.WaitForPendingFinalizers();
                    //if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    //{
                    //    class_Win32.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                    //}

                    #endregion

                    #endregion
                }
                else if (strUserParam[0] == "6")
                {
                    // ***  六面机 0402R 侧面12号相机***
                    listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0402R_Camera12(hoImage, lkkPolygon, nNodeParam);

                    //if (ithreadCheck == 1) listObj2Draw = syDetectJah1.sySixSideDetect12(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 2) listObj2Draw = syDetectJah2.sySixSideDetect12(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 3) listObj2Draw = syDetectJah3.sySixSideDetect12(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 4) listObj2Draw = syDetectJah4.sySixSideDetect12(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 5) listObj2Draw = syDetectJah5.sySixSideDetect12(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 6) listObj2Draw = syDetectJah6.sySixSideDetect12(hoImage, lkkPolygon, nNodeParam);

                }
                else if (strUserParam[0] == "7")
                {
                    // ***  六面机 0402R 侧面34号相机***
                    string strMessage = strSetupTest;//=="0"表示正常检测， =="1"表示设置参数时候的测试模式，返回要显示的内容
                    listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0402R_Camera34(hoImage, lkkPolygon, nNodeParam);                   

                    //if (ithreadCheck == 1) listObj2Draw = syDetectJah1.sySixSideDetect56(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 2) listObj2Draw = syDetectJah2.sySixSideDetect56(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 3) listObj2Draw = syDetectJah3.sySixSideDetect56(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 4) listObj2Draw = syDetectJah4.sySixSideDetect56(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 5) listObj2Draw = syDetectJah5.sySixSideDetect56(hoImage, lkkPolygon, nNodeParam);
                    //else if (ithreadCheck == 6) listObj2Draw = syDetectJah6.sySixSideDetect65(hoImage, lkkPolygon, nNodeParam);


                    #endregion
                }
                else if (strUserParam[0] == "8")
                {
                    // ***  六面机 0402R 正反面 56号相机***

                    Parameter.paramClass configTemple = new Parameter.paramClass();

                    #region  *** 复制参数类，用浅复制，复制指针  ***

                    if (ithreadCheck == 1) configTemple = Parameter.configStructThread01;
                    else if (ithreadCheck == 2) configTemple = Parameter.configStructThread02;
                    else if (ithreadCheck == 3) configTemple = Parameter.configStructThread03;
                    else if (ithreadCheck == 4) configTemple = Parameter.configStructThread04;
                    else if (ithreadCheck == 5) configTemple = Parameter.configStructThread05;
                    else if (ithreadCheck == 6) configTemple = Parameter.configStructThread06;
                    else if (ithreadCheck == 7) configTemple = Parameter.configStructThread07;
                    else if (ithreadCheck == 8) configTemple = Parameter.configStructThread08;
                    else if (ithreadCheck == 9) configTemple = Parameter.configStructThread09;
                    else if (ithreadCheck == 10) configTemple = Parameter.configStructThread10;
                    else if (ithreadCheck == 11) configTemple = Parameter.configStructThread11;
                    else if (ithreadCheck == 12) configTemple = Parameter.configStructThread12;
                    else if (ithreadCheck == 13) configTemple = Parameter.configStructThread13;
                    else if (ithreadCheck == 14) configTemple = Parameter.configStructThread14;
                    else if (ithreadCheck == 15) configTemple = Parameter.configStructThread15;
                    else if (ithreadCheck == 16) configTemple = Parameter.configStructThread16;
                    else if (ithreadCheck == 17) configTemple = Parameter.configStructThread17;
                    else if (ithreadCheck == 18) configTemple = Parameter.configStructThread18;

                    #endregion

                    if (configTemple.listOCRDetect.Count > 0)
                    {
                        #region  *** 带字符的检测  ***

                        string[] strLib = configTemple.listOCRDetect[0].strUsed.Split('#');
                        List<string> strParams = configLibGlobal.dictOCRDetect[strLib[0]];
                        strLib = strParams[0].Split('#');

                        string strModel = strLib[1];
                        strParams = configLibGlobal.dictLocPosition[strModel];
                        string[] strTemp = strParams[0].Split('#');

                        string[] strFindModel = strParams[2].Split('#');

                        string strRegion = strLib[2];

                        HTuple hvParamsData = new HTuple();
                        hvParamsData[0] = double.Parse(strTemp[5]);//模板中心X
                        hvParamsData[1] = double.Parse(strTemp[4]);//模板Y
                        hvParamsData[2] = double.Parse(strTemp[6]);//模板角度
                        hvParamsData[3] = double.Parse(strFindModel[4]);//匹配分数

                        //string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\Regions";
                        //strDirectory += "\\" + strRegion + ".tif";
                        //HOperatorSet.ReadRegion(out hoRegion, strDirectory);

                        //HOperatorSet.WriteRegion((HObject)configLibGlobal.dictRegion[strRegion], "D:\\1.tif");
                        //HOperatorSet.WriteRegion(hoRegion, "D:\\1.tif");

                        listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0402R_Camera56_OCR(hoImage, configLibGlobal.dictRegion[strRegion].Clone(), hvAllHandles[int.Parse(strTemp[0])], hvParamsData, lkkPolygon, nNodeParam);
                        #endregion
                    }
                    else
                    {
                        listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0402R_Camera56(hoImage, lkkPolygon, nNodeParam);

                    }
                    //listObj2Draw[2] = "8.199999988#-8.9#10.1#";

                    //float f1 = 1.111f, f2 = 2.222f, f3 = 43.333f;

                    //string str1 = f1.ToString("0.0000") + "#" + f2.ToString("0.0000") + "#" + f3.ToString("0.0000");

                }         
                else if (strUserParam[0] == "11")
                {
                    #region  ***  六面机  0201电容两个小端***

                    string strMessage = strSetupTest;//=="0"表示正常检测， =="1"表示设置参数时候的测试模式，返回要显示的内容
                    listObj2Draw = SiyarSixsDetect.classSiyarDetect_C.sySixSideDetect21(hoImage, lkkPolygon, nNodeParam, ref strMessage);
                    strSetupTest = strMessage;

                    #endregion
                }
                else if (strUserParam[0] == "12")
                {
                    #region  ***  六面机 检测0201电容其余四个面***

                    string strMessage = strSetupTest;//=="0"表示正常检测， =="1"表示设置参数时候的测试模式，返回要显示的内容
                    listObj2Draw = SiyarSixsDetect.classSiyarDetect_C.sySixSideDetect22(hoImage, lkkPolygon, nNodeParam, ref strMessage);
                    strSetupTest = strMessage;
                    #endregion
                }
                else if (strUserParam[0] == "13")
                {
                    // ***  六面机 检测0603电阻 相机12
                    string strMessage = strSetupTest;//=="0"表示正常检测， =="1"表示设置参数时候的测试模式，返回要显示的内容
                    listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0603R_Camera12(hoImage, lkkPolygon, nNodeParam, ref strMessage);
                    strSetupTest = strMessage;
                }
                else if (strUserParam[0] == "14")
                {
                    // ***  六面机 检测0603电阻 相机34 

                    string strMessage = strSetupTest;//=="0"表示正常检测， =="1"表示设置参数时候的测试模式，返回要显示的内容
                    listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0603R_Camera34(hoImage, lkkPolygon, nNodeParam, ref strMessage);
                    strSetupTest = strMessage;
                }
                else if (strUserParam[0] == "16")
                {
                    // ***  六面机 检测0603电阻 相机56

                    Parameter.paramClass configTemple = new Parameter.paramClass();

                    #region  *** 复制参数类，用浅复制，复制指针  ***

                    if (ithreadCheck == 1) configTemple = Parameter.configStructThread01;
                    else if (ithreadCheck == 2) configTemple = Parameter.configStructThread02;
                    else if (ithreadCheck == 3) configTemple = Parameter.configStructThread03;
                    else if (ithreadCheck == 4) configTemple = Parameter.configStructThread04;
                    else if (ithreadCheck == 5) configTemple = Parameter.configStructThread05;
                    else if (ithreadCheck == 6) configTemple = Parameter.configStructThread06;
                    else if (ithreadCheck == 7) configTemple = Parameter.configStructThread07;
                    else if (ithreadCheck == 8) configTemple = Parameter.configStructThread08;
                    else if (ithreadCheck == 9) configTemple = Parameter.configStructThread09;
                    else if (ithreadCheck == 10) configTemple = Parameter.configStructThread10;
                    else if (ithreadCheck == 11) configTemple = Parameter.configStructThread11;
                    else if (ithreadCheck == 12) configTemple = Parameter.configStructThread12;
                    else if (ithreadCheck == 13) configTemple = Parameter.configStructThread13;
                    else if (ithreadCheck == 14) configTemple = Parameter.configStructThread14;
                    else if (ithreadCheck == 15) configTemple = Parameter.configStructThread15;
                    else if (ithreadCheck == 16) configTemple = Parameter.configStructThread16;
                    else if (ithreadCheck == 17) configTemple = Parameter.configStructThread17;
                    else if (ithreadCheck == 18) configTemple = Parameter.configStructThread18;

                    #endregion

                    if (configTemple.listOCRDetect.Count > 0)
                    {
                        //  *** 带字符的检测  ***
                        string[] strLib = configTemple.listOCRDetect[0].strUsed.Split('#');
                        List<string> strParams = configLibGlobal.dictOCRDetect[strLib[0]];
                        strLib = strParams[0].Split('#');

                        string strModel = strLib[1];
                        strParams = configLibGlobal.dictLocPosition[strModel];
                        string[] strTemp = strParams[0].Split('#');

                        string[] strFindModel = strParams[2].Split('#');

                        string strRegion = strLib[2];

                        HTuple hvParamsData = new HTuple();
                        hvParamsData[0] = double.Parse(strTemp[5]);//模板中心X
                        hvParamsData[1] = double.Parse(strTemp[4]);//模板Y
                        hvParamsData[2] = double.Parse(strTemp[6]);//模板角度
                        hvParamsData[3] = double.Parse(strFindModel[4]);//匹配分数

                        //string strDirectory = System.Windows.Forms.Application.StartupPath + "\\GLOBAL\\Regions";
                        //strDirectory += "\\" + strRegion + ".tif";
                        //HOperatorSet.ReadRegion(out hoRegion, strDirectory);

                        //HOperatorSet.WriteRegion((HObject)configLibGlobal.dictRegion[strRegion], "D:\\1.tif");
                        //HOperatorSet.WriteRegion(hoRegion, "D:\\1.tif");

                        string strMessage = strSetupTest;//=="0"表示正常检测， =="1"表示设置参数时候的测试模式，返回要显示的内容

                        listObj2Draw = SiyarSixsDetect.classSiyarDetect_R.sySixSideDetect_0603R_Camera56_OCR(hoImage, configLibGlobal.dictRegion[strRegion].Clone(), hvAllHandles[int.Parse(strTemp[0])], hvParamsData, lkkPolygon, nNodeParam, ref strMessage);
                        strSetupTest = strMessage;
                    }
                }
             
                return listObj2Draw;

            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("用户自定义程序出错(1035):", "Error occured when proceeding userset function", exc, dhDll.logDiskMode.Error, System.Windows.Forms.MessageBoxIcon.None);

                listObj2Draw[0] = 0; listObj2Draw[1] = "NG-程序出错"; listObj2Draw[2] = 888;
                return listObj2Draw;
            }
            finally
            {

            }
        }


        public static void syShowRegionBorder(HObject hoImage, ref List<object> lobjdrawing, string strOKNG)
        {
            #region  *** 将一个区域轮廓添加到绘图  ***
            try
            {
                HObject hoSixIndex, hoSixContour;
                HTuple hvCount, hvRow, hvColumn;
                HOperatorSet.CountObj(hoImage, out hvCount);
                if (hvCount.I > 0)
                {
                    for (int igg = 1; igg <= hvCount.I; igg++)
                    {
                        HOperatorSet.SelectObj(hoImage, out hoSixIndex, igg);

                        HOperatorSet.GenContourRegionXld(hoSixIndex, out hoSixContour, "border");
                        HOperatorSet.GetContourXld(hoSixContour, out hvRow, out hvColumn);
                        List<PointF> lpsTemp = new List<PointF>();
                        for (int ikk = 0; ikk < hvRow.Length; ikk++)
                        {
                            lpsTemp.Add(new PointF((float)hvColumn.DArr[ikk], (float)hvRow.DArr[ikk]));
                        }

                        lobjdrawing.Add("轮廓");
                        lobjdrawing.Add(lpsTemp.ToArray());
                        lobjdrawing.Add(strOKNG);

                    }
                }
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("将一个区域轮廓添加到绘图出错", "", exc, dhDll.logDiskMode.Error, 0);
            }
            #endregion
        }

        #region *** 根据第几线程的第几个定位工具，得到定位的结果点坐标和角度  ***

        public static List<RectangleF> ccFindPostion(HObject hoImageX, int ithreadselect, int iPosIndex, List<PointF[]> lpfPolygon, ref List<object> lShDraw)
        {
            #region  *** 根据第几个定位工具 找到匹配点  ***

            List<object> listShip2Draw = new List<object>();
            listShip2Draw.Add(0); listShip2Draw.Add("OK"); listShip2Draw.Add(888);

            List<object> lShip2Draw = new List<object>();

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();

            Parameter.paramClass configTemple = Parameter.configClone(ithreadselect);

            Parameter.paramNode nNode = configTemple.LocPosNodes[iPosIndex];
            if (lpfPolygon != null) nNode.listPolygonFind = dhDll.clsFunction.transFormPolygon2String(lpfPolygon, 1);

            List<RectangleF> locposResult = new List<RectangleF>();
            bool boverlap = false;
            locposResult = ParamHvpple.dhLocPositionAlk(hoImageX, nNode, -1, -1, out boverlap, out lShip2Draw, 888);

            if (locposResult.Count >= 1)
            {
                listShip2Draw.Add("定位中心");
                listShip2Draw.Add(new PointF(locposResult[0].X, locposResult[0].Y));
                listShip2Draw.Add("OK");

                if (locposResult.Count > 1)
                {
                    for (int imm = 1; imm < locposResult.Count - 1; imm += 1)
                    {
                        listShip2Draw.Add("小十字");
                        listShip2Draw.Add(locposResult[imm].Location);
                        listShip2Draw.Add("OK");
                    }

                }

                lShDraw = listShip2Draw;

                sw.Stop();

                return locposResult;
                //imgdocMsgShow(iThreadNumber, true, "找到匹配点:X=" + locposResult[0].X.ToString() + ",Y=" + locposResult[0].Y.ToString() + ",耗时" + sw.ElapsedMilliseconds.ToString() + "ms");
            }
            else
            {
                return locposResult;
                //imgdocMsgShow(iThreadNumber, false, "没找到匹配点");
            }

            #endregion
        }

        #endregion
  



        #region ****** 开启线程保存对应的图片 ******


        /// <summary>
        /// 开启线程保存对应的图片
        /// bisokItem=1-保存所有图片(OK NG) bisokItem=3 保存当前图片  =3时候strItemOkNg表示存放路径
        /// </summary>
        /// <param name="hoImage"></param>
        /// <param name="iThreadNumber"></param>
        /// <param name="bisokItem"></param>
        public static void savePicsShip(HObject hoImage, Image<Gray, byte> bmpEmgu, Image<Bgr, byte> bmpEmguRGB, int iThreadNumber, int iisokItem, string strItemOkNg, List<object> liObj2Draw, dhDll.KickEnResults dhKickDetail)
        {
            Task t = Task.Factory.StartNew(() =>
            {
                
                // Parallel.Invoke(() =>
                //{
               
                savePicThread(hoImage, bmpEmgu, bmpEmguRGB, iThreadNumber, iisokItem, strItemOkNg, liObj2Draw, dhKickDetail);

                //});
            
            });  

     
        }



        /// <summary>
        /// 直接的保存图片函数  bSameName=true表示下一张图片的后缀名称和当前一张一样
        /// </summary>
        /// <param name="hoImage"></param>
        /// <param name="iThreadNumber"></param>
        /// <param name="iisokItem"></param>
        /// <param name="strItemOkNg"></param>
        private static void savePicThread(HObject hoImage, Image<Gray, byte> bmpEmgu, Image<Bgr, byte> bmpEmguRGB, int iThreadNumber, int iisokItem, string strItemOkNg, List<object> liObj2Draw, dhDll.KickEnResults dhKickDetail, bool bSameName = false)
        {
            //muSavePics.WaitOne();
            try
            {
                if (classApp.i_plc_loopexe[8] == 0 && iisokItem != 3) return;

                if (hoImage == null) return;

                HTuple hvNumber;
                HOperatorSet.CountObj(hoImage, out hvNumber);
                if (hvNumber.I < 1) return;

                long lksave = 1;
                long lSaveNumber = 0;

                #region 根据线程号 拷贝保存状态，保存 数量后缀

                //保存标识 0-不保存 1-保存所有 2-保存报错
                lksave = Interlocked.Read(ref dhDll.frmSaveImages.lsavePicThreads[iThreadNumber]);

                lSaveNumber = Interlocked.Read(ref dhDll.clsss_Status.lNumberThreads[iThreadNumber][0]);
               
                #endregion

                string strDirectory = "C:\\VisionImages";

                #region  *** 指定路径 或者 默认路径  ***

                if (Parameter.configPublic.lstrSaveImages[2].Contains(":\\"))
                {
                    strDirectory = Parameter.configPublic.lstrSaveImages[2];
                    if (!System.IO.Directory.Exists(strDirectory)) System.IO.Directory.CreateDirectory(strDirectory);
                }
                else //如果是默认路径
                {
                    if (!System.IO.Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\" + "Images"))
                    {
                        System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\" + "Images");
                    }

                    strDirectory = System.Windows.Forms.Application.StartupPath + "\\" + "Images" + "\\" +
                        ParamHvpple.configLibGlobal.lstrAdvanced[2].Trim() + dhDll.frmMsg.GetTimeNow(1) + "-" + dhDll.frmVarietyMg.strCurrentProduct;

                    if (!System.IO.Directory.Exists(strDirectory)) System.IO.Directory.CreateDirectory(strDirectory);
                }
                #endregion

                string strSavePath = strDirectory + "\\" + iThreadNumber.ToString() + "号工位";

                float fzoomed = 1;

                #region  *** 计算缩放比例
                if (float.TryParse(Parameter.configPublic.lstrSaveImages[3], out fzoomed))
                {
                    //if (fzoomed < 1) HOperatorSet.ZoomImageFactor(hoImage, out hozoomed, fzoomed, fzoomed, "constant");
                    //else
                    //{
                    //    HOperatorSet.CopyImage(hoImage, out hozoomed);
                    //}
                }
                else
                {
                    //HOperatorSet.CopyImage(hoImage, out hozoomed);
                }
                #endregion


                #region  *** 是否用上一张图片的名字，命名本张图片  ***

                bool bsaveimage = false;

                string strFileName = dhDll.frmVarietyMg.strCurrentProduct + "-" + dhDll.frmMsg.GetTimeNow(2) + "-" + DateTime.Now.Millisecond.ToString() + "-" + strItemOkNg
                     + "-EN_" + dhKickDetail.lEncoder.ToString();

                //if (bsaveimage) strSameName = strFileName;
                //else
                //{
                //    if (strSameName != "")
                //    {
                //        strFileName = strSameName;
                //        strSameName = "";
                //    }
                //}
                #endregion

                string sPath2SaveImg = "";

                if (classApp.i_plc_loopexe[8] == 2 && strItemOkNg.Contains("NG") && (iisokItem == 1 || iisokItem == -1))
                {
                    #region 保存报错图片

                    dhDll.frmSaveImages.iCountOfSaved++;

                    strSavePath += "-NG";
                    if (!System.IO.Directory.Exists(strSavePath)) System.IO.Directory.CreateDirectory(strSavePath);

                    sPath2SaveImg = strSavePath + "\\" + strFileName;

                    bsaveimage = true;

                    #endregion
                }

                if (classApp.i_plc_loopexe[8] == 1 && (iisokItem == 1 || iisokItem == -1))
                {
                    #region 保存所有图片 OK NG 分开保存

                    dhDll.frmSaveImages.iCountOfSaved++;

                    string strSavePath_NG = strSavePath + "-NG";
                    if (!System.IO.Directory.Exists(strSavePath_NG)) System.IO.Directory.CreateDirectory(strSavePath_NG);

                    string strSavePath_OK = strSavePath + "-OK";
                    if (!System.IO.Directory.Exists(strSavePath_OK)) System.IO.Directory.CreateDirectory(strSavePath_OK);

                    if (strItemOkNg.Contains("NG"))
                    {
                        sPath2SaveImg = strSavePath_NG + "\\" + strFileName;
                    }
                    else
                    {
                        sPath2SaveImg = strSavePath_OK + "\\" + strFileName;
                    }

                    bsaveimage = true;

                    #endregion
                }

                if ((Parameter.configPublic.lstrSaveImages[6] == "1" && strItemOkNg.Contains("OK")) ||
                    (Parameter.configPublic.lstrSaveImages[7] == "1" && strItemOkNg.Contains("NG") && iisokItem!=-1))
                {
                    #region  *** 保存处理效果图  ***

                    Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> bmpRGB = null;

                    HTuple hvCount = 0, hvwidth, hvheight, hvtype,hvPointer;
                    HOperatorSet.CountChannels(hoImage, out hvCount);

                    if (hvCount.I == 1)
                    {
                        HOperatorSet.GetImagePointer1(hoImage, out hvPointer, out hvtype, out hvwidth, out hvheight);
                        Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> bmpLoad = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(hvwidth.I, hvheight.I, hvwidth.I, new IntPtr(hvPointer.L));
                        bmpRGB = new Image<Bgr, byte>(bmpLoad.Bitmap);
                    }
                    else if (hvCount.I == 3)
                    {
                        HTuple hvRed, hvBlue, hvGreen;
                        HOperatorSet.GetImagePointer3(hoImage, out hvRed, out hvGreen, out hvBlue, out hvtype, out hvwidth, out hvheight);
                        Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> bmpRed = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(hvwidth.I, hvheight.I, hvwidth.I, new IntPtr(hvRed.L));
                        Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> bmpGreen = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(hvwidth.I, hvheight.I, hvwidth.I, new IntPtr(hvGreen.L));
                        Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> bmpBlue = new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>(hvwidth.I, hvheight.I, hvwidth.I, new IntPtr(hvBlue.L));

                        bmpRGB = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(new Emgu.CV.Image<Emgu.CV.Structure.Gray, byte>[] { bmpBlue, bmpGreen, bmpRed });

                    }


                    //bmpRGB.Save("D:\\mmm2.jpg");


                    //if (bmpEmgu != null)
                    //{
                    //    bmpRGB = new Image<Emgu.CV.Structure.Bgr, byte>(bmpEmgu.Bitmap);

                    //}
                    //else if (bmpEmguRGB != null)
                    //{
                    //    bmpRGB = bmpEmguRGB;
                    //}

                    if (bmpRGB != null)
                    {
                        //bmpRGB = new Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>(5496, 3672);

                        //Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> bmKKKK = new Image<Bgr,byte>(bmpRGB.Width,bmpRGB.Height);
                        //Emgu.CV.CvInvoke.cvCopy(bmpRGB, bmKKKK, new IntPtr());

                        //Bitmap bmXXXX = bmpRGB.ToBitmap();
                        Graphics g = Graphics.FromImage(bmpRGB.Bitmap);

                        dhDll.frmDrawPics.drawListObj(liObj2Draw, g, new Rectangle(0, 0, bmpRGB.Width, bmpRGB.Height), 1, true);




                        //if (g.SmoothingMode != System.Drawing.Drawing2D.SmoothingMode.AntiAlias)
                        //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        //strItemOkNg = (string)liObj2Draw[1];

                        //float fscaleXy = 1.0f;
                        //Rectangle recPicboxBounds = new Rectangle(0, 0, bmpRGB.Width, bmpRGB.Height);

                        //if (strItemOkNg.Contains("OK"))
                        //{
                        //    g.DrawString(strItemOkNg, dhDll.frmDrawPics.dhDrawParams.ResultFont, new SolidBrush(dhDll.frmDrawPics.dhDrawParams.ResultColorOK),
                        //        dhDll.frmDrawPics.dhDrawParams.ResultLocation.X / fscaleXy + recPicboxBounds.X, dhDll.frmDrawPics.dhDrawParams.ResultLocation.Y / fscaleXy + recPicboxBounds.Y);
                        //}
                        //else if (strItemOkNg.Contains("JUMP"))
                        //{
                        //    //不绘制结果
                        //}
                        //else
                        //{
                        //    g.DrawString(strItemOkNg, dhDll.frmDrawPics.dhDrawParams.ResultFont, new SolidBrush(dhDll.frmDrawPics.dhDrawParams.ResultColorNG),
                        //            dhDll.frmDrawPics.dhDrawParams.ResultLocation.X / fscaleXy + recPicboxBounds.X, dhDll.frmDrawPics.dhDrawParams.ResultLocation.Y / fscaleXy + recPicboxBounds.Y);
                        //}

                        //PointF p2DrawInfo = dhDll.frmDrawPics.dhDrawParams.StringLocation;
                        //p2DrawInfo.X /= fscaleXy; p2DrawInfo.Y /= fscaleXy;
                        //p2DrawInfo.X += (int)(recPicboxBounds.X / 1);
                        //p2DrawInfo.Y += (int)(recPicboxBounds.Y / 1);


                        //for (int igg = 0; igg < liObj2Draw.Count - 2; igg += 3)
                        //{
                        //    string strDrawStyle = liObj2Draw[igg].ToString();

                        //    if (strDrawStyle == "小十字")
                        //    {
                        //        #region  ****** 绘制小十字 ******

                        //        PointF p2DrawCenter = (PointF)liObj2Draw[igg + 1];
                        //        p2DrawCenter.X /= fscaleXy; p2DrawCenter.Y /= fscaleXy;
                        //        p2DrawCenter.X += (int)(recPicboxBounds.X / 1);
                        //        p2DrawCenter.Y += (int)(recPicboxBounds.Y / 1);

                        //        string strOkNgMsg = (string)liObj2Draw[igg + 2];
                        //        if (strOkNgMsg.Contains("OK"))
                        //        {
                        //            g.DrawLine(dhDll.frmDrawPics.dhDrawParams.LinePenOK, p2DrawCenter.X - 8, p2DrawCenter.Y, p2DrawCenter.X + 8, p2DrawCenter.Y);
                        //            g.DrawLine(dhDll.frmDrawPics.dhDrawParams.LinePenOK, p2DrawCenter.X, p2DrawCenter.Y - 8, p2DrawCenter.X, p2DrawCenter.Y + 8);
                        //        }
                        //        else if (strOkNgMsg.Contains("NG"))
                        //        {
                        //            g.DrawLine(dhDll.frmDrawPics.dhDrawParams.LinePenNG, p2DrawCenter.X - 8, p2DrawCenter.Y, p2DrawCenter.X + 8, p2DrawCenter.Y);
                        //            g.DrawLine(dhDll.frmDrawPics.dhDrawParams.LinePenNG, p2DrawCenter.X, p2DrawCenter.Y - 8, p2DrawCenter.X, p2DrawCenter.Y + 8);
                        //        }
                        //        #endregion
                        //    }
                        //    else if (strDrawStyle == "线" || strDrawStyle == "直线" || strDrawStyle == "两点定线" || strDrawStyle == "点线定线")
                        //    {
                        //        #region  ****** 绘制线 ******

                        //        RectangleF rec2Draw = (RectangleF)liObj2Draw[igg + 1];
                        //        rec2Draw.X /= fscaleXy; rec2Draw.Y /= fscaleXy;
                        //        rec2Draw.X += recPicboxBounds.X;
                        //        rec2Draw.Y += recPicboxBounds.Y;

                        //        rec2Draw.Width /= fscaleXy; rec2Draw.Height /= fscaleXy;
                        //        rec2Draw.Width += recPicboxBounds.X;
                        //        rec2Draw.Height += recPicboxBounds.Y;

                        //        PointF pfInters1 = PointF.Empty; PointF pfInters2 = PointF.Empty;

                        //        int iDeltaX = 0; int iDeltaY = 0;

                        //        iDeltaX = (int)Math.Abs(rec2Draw.Width - rec2Draw.X);
                        //        iDeltaY = (int)Math.Abs(rec2Draw.Height - rec2Draw.Y);

                        //        if (iDeltaX > iDeltaY)
                        //        {
                        //            pfInters1 = dhDll.clsFunction.TwoLineInteraction((PointF)rec2Draw.Size, (PointF)rec2Draw.Location,
                        //                new PointF(recPicboxBounds.X, recPicboxBounds.Y),
                        //                new PointF(recPicboxBounds.X, recPicboxBounds.Bottom));
                        //            pfInters2 = dhDll.clsFunction.TwoLineInteraction((PointF)rec2Draw.Size, (PointF)rec2Draw.Location,
                        //                new PointF(recPicboxBounds.Right, recPicboxBounds.Y),
                        //                new PointF(recPicboxBounds.Right, recPicboxBounds.Bottom));
                        //        }
                        //        else
                        //        {
                        //            pfInters1 = dhDll.clsFunction.TwoLineInteraction((PointF)rec2Draw.Size, (PointF)rec2Draw.Location,
                        //                new PointF(recPicboxBounds.X, recPicboxBounds.Y),
                        //                new PointF(recPicboxBounds.Right, recPicboxBounds.Y));
                        //            pfInters2 = dhDll.clsFunction.TwoLineInteraction((PointF)rec2Draw.Size, (PointF)rec2Draw.Location,
                        //                new PointF(recPicboxBounds.X, recPicboxBounds.Bottom),
                        //                new PointF(recPicboxBounds.Right, recPicboxBounds.Bottom));
                        //        }

                        //        string strOkNgMsg = (string)liObj2Draw[igg + 2];

                        //        if (strOkNgMsg.Contains("OK"))
                        //        {
                        //            g.DrawLine(dhDll.frmDrawPics.dhDrawParams.LinePenOK, pfInters1, pfInters2);
                        //        }
                        //        else if (strOkNgMsg.Contains("NG"))
                        //        {
                        //            g.DrawLine(dhDll.frmDrawPics.dhDrawParams.LinePenNG, pfInters1, pfInters2);
                        //        }

                        //        #endregion
                        //    }
                        //    else if (strDrawStyle == "圆")
                        //    {
                        //        #region  ****** 绘制圆 ******

                        //        //rec2Draw.X rec2Draw.Y表示圆心坐标 rec2Draw.Width表示半径
                        //        RectangleF rec2Draw = (RectangleF)liObj2Draw[igg + 1];
                        //        rec2Draw.Height = rec2Draw.Width;
                        //        rec2Draw.X /= fscaleXy; rec2Draw.Y /= fscaleXy;
                        //        rec2Draw.X += (int)(recPicboxBounds.X / 1);
                        //        rec2Draw.Y += (int)(recPicboxBounds.Y / 1);
                        //        rec2Draw.Width /= fscaleXy; rec2Draw.Height /= fscaleXy;

                        //        //g.FillEllipse(Brushes.Blue, rec2Draw.X - 4, rec2Draw.Y - 4, 8, 8);

                        //        rec2Draw.X -= rec2Draw.Width; rec2Draw.Y -= rec2Draw.Width;
                        //        rec2Draw.Width *= 2; rec2Draw.Height *= 2;

                        //        string strOkNgMsg = (string)liObj2Draw[igg + 2];
                        //        if (strOkNgMsg.Contains("OK"))
                        //        {
                        //            g.DrawEllipse(dhDll.frmDrawPics.dhDrawParams.CirclePenOK, rec2Draw);
                        //        }
                        //        else if (strOkNgMsg.Contains("NG"))
                        //        {
                        //            g.DrawEllipse(dhDll.frmDrawPics.dhDrawParams.CirclePenNG, rec2Draw);
                        //        }

                        //        #endregion
                        //    }
                        //}
                        //for (int igg = 0; igg < liObj2Draw.Count - 2; igg += 3)
                        //{
                        //    string strDrawStyle = liObj2Draw[igg].ToString();

                        //    if (strDrawStyle == "字符串")
                        //    {
                        //        #region ****** 绘制 字符串 ******

                        //        List<string> strInfo = (List<string>)liObj2Draw[igg + 1];

                        //        for (int ikm = 0; ikm < strInfo.Count - 1; ikm += 2)
                        //        {
                        //            string strMok = strInfo[ikm + 1];

                        //            if (strMok != "")
                        //            {
                        //                Size textSize = new Size(30, 30);// System.Windows.Forms.TextRenderer.MeasureText(strMok, dhDll.frmDrawPics.dhDrawParams.StringFont);

                        //                g.DrawString(strInfo[ikm], dhDll.frmDrawPics.dhDrawParams.StringFont, (strMok.Contains("NG") ?
                        //                    new SolidBrush(dhDll.frmDrawPics.dhDrawParams.StringColorNG) : new SolidBrush(dhDll.frmDrawPics.dhDrawParams.StringColorOK)), p2DrawInfo);

                        //                //g.DrawString(strInfo[ikm], new Font("宋体", 32), Brushes.Red, p2DrawInfo);
                        //                p2DrawInfo.Y += textSize.Height + 1;
                        //            }
                        //        }

                        //        #endregion
                        //    }
                        //}





                        //for (int ipp = 0; ipp < 10000; ipp++)
                        //{
                        //    g.DrawLine(Pens.Lime, new Point(0, ipp), new Point(ipp + 100, ipp + 200));
                        //}

                        //string strDrawing = "";
                        //for (int icc = 0; icc < liObj2Draw.Count; icc++)
                        //{
                        //    if (liObj2Draw[icc] is string) strDrawing += liObj2Draw[icc];
                        //    else strDrawing += liObj2Draw[icc].ToString();

                        //    strDrawing += " ";
                        //}

                        //dhDll.frmMsg.Log("绘制图片元素个数=" + liObj2Draw.Count.ToString(), "Drawing element count=" + liObj2Draw.Count.ToString(), null, dhDll.logDiskMode.None, 0, true);
                        //dhDll.frmMsg.Log("绘制图片元素=" + strDrawing, "Drawing element=" + strDrawing, null, dhDll.logDiskMode.None, 0, true);

                        g.Dispose();

                        if (sPath2SaveImg == "")
                        {
                            string strSavePath_OK = strSavePath + "-OK";
                            if (!System.IO.Directory.Exists(strSavePath_OK)) System.IO.Directory.CreateDirectory(strSavePath_OK);
                            sPath2SaveImg = strSavePath_OK + "\\" + strFileName;
                        }

                        string strResults = sPath2SaveImg + "-results.jpeg";

                        bmpRGB.Save(strResults);

                        Task t = Task.Factory.StartNew(() =>
                        {
                            frmFTP.syUploadFileFtp(1, strResults);
                        }); 
                    }


                    if (Parameter.configPublic.lstrSaveImages[8] != "1") bsaveimage = false;

                    #endregion
                }



                if (iisokItem == 3)
                {
                    #region 保存当前图片

                    if (strItemOkNg.Contains("\\"))
                    {
                        sPath2SaveImg = strItemOkNg;
                    }
                    else
                    {
                        strSavePath += "-Current";
                        if (!System.IO.Directory.Exists(strSavePath)) System.IO.Directory.CreateDirectory(strSavePath);

                        sPath2SaveImg = strSavePath + "\\" + dhDll.frmVarietyMg.strCurrentProduct + "-" + dhDll.frmMsg.GetTimeNow(2) + "-Current-" + lSaveNumber.ToString();
                    }

                    bsaveimage = true;

                    #endregion
                }

                if (bsaveimage)
                {
                    #region  *** 写入图片到硬盘  ***

                    if (Parameter.configPublic.lstrSaveImages[4].Contains("jpeg"))
                    {
                        int iRename = 1;
                        string strRename = sPath2SaveImg;

                        if (File.Exists(sPath2SaveImg + ".jpg"))
                        {
                            while (true)
                            {
                                strRename = sPath2SaveImg + "(" + iRename.ToString() + ")";
                                if (!File.Exists(strRename + ".jpg"))
                                {
                                    sPath2SaveImg = strRename;
                                    break;
                                }

                                iRename++;
                                if (iRename >= 1000) break;
                            }
                        }

                        HOperatorSet.WriteImage(hoImage, Parameter.configPublic.lstrSaveImages[4], 0, sPath2SaveImg + ".jpg");
                    }
                    else if (Parameter.configPublic.lstrSaveImages[4].Contains("png"))
                    {
                        int iRename = 1;
                        string strRename = sPath2SaveImg;

                        if (File.Exists(sPath2SaveImg + ".png"))
                        {
                            while (true)
                            {
                                strRename = sPath2SaveImg + "(" + iRename.ToString() + ")";
                                if (!File.Exists(strRename + ".png"))
                                {
                                    sPath2SaveImg = strRename;
                                    break;
                                }

                                iRename++;
                                if (iRename >= 1000) break;
                            }
                        }
                        HOperatorSet.WriteImage(hoImage, Parameter.configPublic.lstrSaveImages[4], 0, sPath2SaveImg + ".png");
                    }
                    else if (Parameter.configPublic.lstrSaveImages[4].Contains("tif"))
                    {
                        int iRename = 1;
                        string strRename = sPath2SaveImg;

                        if (File.Exists(sPath2SaveImg + ".tif"))
                        {
                            while (true)
                            {
                                strRename = sPath2SaveImg + "(" + iRename.ToString() + ")";
                                if (!File.Exists(strRename + ".tif"))
                                {
                                    sPath2SaveImg = strRename;
                                    break;
                                }

                                iRename++;
                                if (iRename >= 1000) break;
                            }
                        }
                        HOperatorSet.WriteImage(hoImage, Parameter.configPublic.lstrSaveImages[4], 0, sPath2SaveImg + ".tif");
                    }
                    else if (Parameter.configPublic.lstrSaveImages[4].Contains("bmp"))
                    {
                        int iRename = 1;
                        string strRename = sPath2SaveImg;

                        if (File.Exists(sPath2SaveImg + ".bmp"))
                        {
                            while (true)
                            {
                                strRename = sPath2SaveImg + "(" + iRename.ToString() + ")";
                                if (!File.Exists(strRename + ".bmp"))
                                {
                                    sPath2SaveImg = strRename;
                                    break;
                                }

                                iRename++;
                                if (iRename >= 1000) break;
                            }
                        }
                        HOperatorSet.WriteImage(hoImage, Parameter.configPublic.lstrSaveImages[4], 0, sPath2SaveImg + ".bmp");
                    }
                    else
                    {
                        int iRename = 1;
                        string strRename = sPath2SaveImg;

                        if (File.Exists(sPath2SaveImg + ".jpg"))
                        {
                            while (true)
                            {
                                strRename = sPath2SaveImg + "(" + iRename.ToString() + ")";
                                if (!File.Exists(strRename + ".jpg"))
                                {
                                    sPath2SaveImg = strRename;
                                    break;
                                }

                                iRename++;
                                if (iRename >= 1000) break;
                            }
                        }
                        HOperatorSet.WriteImage(hoImage, "jpeg", 0, sPath2SaveImg + ".jpg");
                    }

                    #endregion
                }

                if (Parameter.configPublic.lstrSaveImages[5] == "6")
                {
                    #region  *** 保存在固定路径的固定名字图片，下一张覆盖上一张  ***

                    sPath2SaveImg = strDirectory + "\\" + iThreadNumber.ToString() + "vision";

                    //if (Parameter.strParamGlobal[12].Contains("jpeg"))
                    //    HOperatorSet.WriteImage(hoImage, Parameter.strParamGlobal[12], 0, sPath + ".jpg");
                    //else if (Parameter.strParamGlobal[12].Contains("png"))
                    //    HOperatorSet.WriteImage(hoImage, Parameter.strParamGlobal[12], 0, sPath + ".png");
                    //else if (Parameter.strParamGlobal[12].Contains("tif"))
                    //    HOperatorSet.WriteImage(hoImage, Parameter.strParamGlobal[12], 0, sPath + ".tif");
                    //else if (Parameter.strParamGlobal[12].Contains("bmp"))
                    //    HOperatorSet.WriteImage(hoImage, Parameter.strParamGlobal[12], 0, sPath + ".bmp");
                    //else
                    //    HOperatorSet.WriteImage(hoImage, "jpeg", 0, sPath + ".jpg");

                    #endregion
                }

                #region  *** 计算当前保存总数，是否需要清零  ***

                long lsavepicCount = 0;
                bool bsaveok = long.TryParse(dhDll.frmSaveImages.strSaveParams[1], out lsavepicCount);
                if (!bsaveok) lsavepicCount = 0;

                if (bsaveok && lsavepicCount > 0 && dhDll.frmSaveImages.iCountOfSaved >= lsavepicCount)
                {
                    dhDll.frmSaveImages.iCountOfSaved = 0;
                    dhDll.frmSaveImages.setSaveStatus(0, true, false);
                }
                #endregion

            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("线程保存图片出错:" + iThreadNumber.ToString() + ",", "Error occured when saving images to local hardware", exc, dhDll.logDiskMode.Error, 0);
            }
            finally
            {
                //muSavePics.ReleaseMutex();
            }
        }

        #endregion


    }


}
