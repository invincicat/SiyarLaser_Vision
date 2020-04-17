using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using hvppleDotNet;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace SiyarSixsDetect
{
    public class classSiyarDetect_C
    {
        private static int iTimeout = 80;
        private static bool bUseMutex = false;

        private static bool bUseSaveMutex = false;

        private static Mutex muDetect12 = new Mutex();
        private static Mutex muDetect56 = new Mutex();
        private static Mutex muDetect8 = new Mutex();
        private static Mutex muDetect21 = new Mutex();
        private static Mutex muDetect22 = new Mutex();

        private static int DDDDD;

        //创建字码模板
        public static void CreateModelMK(HObject ho_Image, out HObject ho_ModelRegion, out HTuple hv_ModelID, out HTuple hv_ModelParam, out HTuple hv_ERR)
        {

            HObject ho_ImageMean = null;

            HTuple hv_Row111 = new HTuple(), hv_Column111 = new HTuple();
            HTuple hv_Angle111 = new HTuple(), hv_Score111 = new HTuple();
            HTuple hv_Exception = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ModelRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            hv_ModelID = new HTuple();
            hv_ModelParam = new HTuple();
            hv_ERR = new HTuple();
            try
            {
                try
                {
                    //gen_rectangle1 (Rectangle, RtgParam[0], RtgParam[1], RtgParam[2], RtgParam[3])
                    //reduce_domain (Image, Rectangle, ImageReduced)
                    //create_shape_model (Image, 'auto', -0.39, 0.79, 'auto', 'auto', 'use_polarity', 'auto', 'auto', ModelID)
                    HOperatorSet.CreateShapeModel(ho_Image, "auto", (new HTuple(0)).TupleRad()
                        , (new HTuple(360)).TupleRad(), "auto", "auto", "use_polarity", "auto",
                        "auto", out hv_ModelID);
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_Image, out ho_ImageMean, 40, 40);
                    //median_image (Image, ImageMedian3, 'circle', 19.5, 'mirrored')
                    ho_ModelRegion.Dispose();
                    HOperatorSet.DynThreshold(ho_Image, ho_ImageMean, out ho_ModelRegion, 10,
                        "light");
                    //shape_trans (RegionsStd, ModelRegion, 'rectangle1')
                    HOperatorSet.FindShapeModel(ho_Image, hv_ModelID, -((new HTuple(10)).TupleRad()
                        ), (new HTuple(20)).TupleRad(), 0.3, 1, 0.1, "least_squares", 0, 0.9,
                        out hv_Row111, out hv_Column111, out hv_Angle111, out hv_Score111);
                    hv_ModelParam = new HTuple();
                    hv_ModelParam = hv_ModelParam.TupleConcat(hv_Row111);
                    hv_ModelParam = hv_ModelParam.TupleConcat(hv_Column111);
                    hv_ModelParam = hv_ModelParam.TupleConcat(hv_Angle111);
                    hv_ERR = 0;
                    //write_region (ModelRegion, 'E:/CreateModel.reg')
                }
                // catch (Exception) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ERR = 1;
                }
                ho_ImageMean.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_ImageMean.Dispose();

                throw HDevExpDefaultException;
            }
        }

        //字码检测，引用文件 8
        public static List<object> sySixSideDetectMK(HObject hoImage, HObject ho_ModelRegion, HTuple hv_ModelID, HTuple hv_ModelParam, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 1206 字码检测  ***

            if (bUseMutex) muDetect56.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {
                HObject hoRegion = null, hoReduced = null, hoConcate = null, hoClosing = null, hoConnection = null, hoSelect = null, hoContour = null, hoUnion = null, hoIndex = null, hoFillup = null, hoTrans = null, hoErosion = null, hoIntersection = null, hoDiffer = null, hoOpening = null;
                HTuple hvRow, hvColumn, hvDist, hvPhi, hvLength1, hvLength2, hvCount, hvUsedThreshold, hvSigma, hvRound, hvSides;
                HTuple AreaErrs, RowErrs, ColErrs;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion
                HObject ImageGray, EroStruct, RegionMK, WhiteRegion, UnionMK, SelectedRegions1, RegionDynThresh, ImageMean, ImageMK, ConnectedRegionsaaa, ConnectedRegions1, Rectangleaaa, RegionUnion, SelectedRegionsaaa, RegionOpening, ConnectedRegions, SelectedRegions;
                HTuple Areasss, Row1sss, Column1sss, Area, Row, Column, Rowaaa, Columnaaa, Phiaaa, Length1aaa, Length2aaa;
                HOperatorSet.Rgb1ToGray(hoImage, out ImageGray);
                HOperatorSet.Threshold(ImageGray, out WhiteRegion, 40, 255);
                HOperatorSet.OpeningCircle(WhiteRegion, out RegionOpening, 5);
                HOperatorSet.Connection(RegionOpening, out ConnectedRegions);
                HOperatorSet.SelectShapeStd(ConnectedRegions, out SelectedRegions, "max_area", 70);
                HOperatorSet.AreaCenter(SelectedRegions, out Area, out Row, out Column);
                //判断正反
                if (Area < 90000)
                {
                    //字码朝上
                    //提取字码区域
                    HOperatorSet.Union1(RegionOpening, out RegionUnion);
                    HOperatorSet.SmallestRectangle2(RegionUnion, out Rowaaa, out Columnaaa, out Phiaaa, out Length1aaa, out Length2aaa);
                    HOperatorSet.GenRectangle2(out Rectangleaaa, Rowaaa, Columnaaa, Phiaaa, Length1aaa, Length2aaa);
                    HOperatorSet.GenRectangle2(out EroStruct, Rowaaa, Columnaaa, Phiaaa, 80, 25);
                    HOperatorSet.Erosion1(Rectangleaaa, EroStruct, out RegionMK, 1);
                    HOperatorSet.ReduceDomain(ImageGray, RegionMK, out ImageMK);
                    HOperatorSet.MeanImage(ImageMK, out ImageMean, 40, 40);
                    HOperatorSet.DynThreshold(ImageMK, ImageMean, out RegionDynThresh, 10, "light");
                    HOperatorSet.Connection(RegionDynThresh, out ConnectedRegions1);
                    HOperatorSet.SelectShape(ConnectedRegions1, out SelectedRegions1, "area", "and", 150, 99999);
                    HOperatorSet.Union1(SelectedRegions1, out UnionMK);  //UnionMK 字符提取区域

                    HTuple hv_Num, Areajjj, Rowjjj, Coljjj, Arealll, Rowlll, Collll, FoundRow, FoundColumn, FoundAngle, Score, HomMat2D;
                    HObject ho_RegionSel, ErrConn, ErrUnion, RegionMkErr1, RegionMkErr2, RegionAffineTrans1, RegionMkErr1Open, RegionMkErr2Open;
                    HOperatorSet.FindShapeModel(ImageMK, hv_ModelID, -((new HTuple(0)).TupleRad()), (new HTuple(360)).TupleRad(), 0.3, 1, 0, "least_squares", 0, 0.9, out FoundRow, out FoundColumn, out FoundAngle, out Score);
                    if ((int)(new HTuple((new HTuple(Score.TupleLength())).TupleGreater(0))) != 0)
                    {
                        HOperatorSet.VectorAngleToRigid(hv_ModelParam.TupleSelect(0), hv_ModelParam.TupleSelect(1), hv_ModelParam.TupleSelect(2), FoundRow, FoundColumn, FoundAngle, out HomMat2D);
                        HOperatorSet.AffineTransRegion(ho_ModelRegion, out RegionAffineTrans1, HomMat2D, "true");

                        //求缺失
                        HOperatorSet.Difference(RegionAffineTrans1, UnionMK, out RegionMkErr1);
                        HOperatorSet.OpeningCircle(RegionMkErr1, out RegionMkErr1Open, 2);
                        //求多于
                        HOperatorSet.Difference(UnionMK, RegionAffineTrans1, out RegionMkErr2);
                        HOperatorSet.OpeningCircle(RegionMkErr2, out RegionMkErr2Open, 2);

                        HOperatorSet.AreaCenter(RegionMkErr1Open, out Arealll, out Rowlll, out Collll);
                        HOperatorSet.AreaCenter(RegionMkErr2Open, out Areajjj, out Rowjjj, out Coljjj);
                        if (Arealll > 500 || Areajjj > 500)
                        {
                            listObj2Draw[1] = "NG-字码";
                            HOperatorSet.Union2(RegionMkErr1Open, RegionMkErr2Open, out ErrUnion);
                            HOperatorSet.Connection(ErrUnion, out ErrConn);

                            hv_Num = 0;
                            HOperatorSet.CountObj(ErrConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ErrConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            return listObj2Draw;
                        }

                        //执行到这里，OK  绘制字码轮廓
                        listObj2Draw[1] = "OK";
                        hv_Num = 0;
                        HObject RegionMKConn;
                        HOperatorSet.Connection(UnionMK, out RegionMKConn);
                        HOperatorSet.CountObj(RegionMKConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(RegionMKConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
                        }
                        return listObj2Draw;
                    }
                    else
                    {

                        listObj2Draw[1] = "NG-无定位";
                        return listObj2Draw;
                    }
                }
                else
                {

                }
                //执行到这里，OK  
                listObj2Draw[1] = "OK";
                return listObj2Draw;
            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetectMK", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect56.ReleaseMutex();
            }

            #endregion
        }

        //昶龙六面机56相机  引用7
        public static List<object> sySixSideDetect56(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 六面机 侧面  ***

            if (bUseMutex) muDetect56.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {
                HObject hoRegion = null, hoReduced = null, hoConcate = null, hoClosing = null, hoConnection = null, hoSelect = null, hoContour = null, hoUnion = null, hoIndex = null, hoFillup = null, hoTrans = null, hoErosion = null, hoIntersection = null, hoDiffer = null, hoOpening = null;
                HTuple hvRow, hvColumn, hvDist, hvPhi, hvLength1, hvLength2, hvCount, hvUsedThreshold, hvSigma, hvRound, hvSides;
                HTuple AreaErrs, RowErrs, ColErrs;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

#if true
                #region  ******参数读取*******
                string[] strUserParam = strParams.Split('#');

                //int iGrayClosing = int.Parse(strUserParam[4]);     //掩膜边长   10

                int ithreshmax = int.Parse(strUserParam[5]);       //总定位阈值 1工位100 2工位100
                int iLength1 = int.Parse(strUserParam[6]);       //半长 110
                int iLength1Scale = int.Parse(strUserParam[7]);       //半长变化15
                int iLength2 = int.Parse(strUserParam[8]);       //半宽 35
                int iLength2Scale = int.Parse(strUserParam[9]);       //半宽变化 5
                int iRectEro = int.Parse(strUserParam[10]);      //矩形腐蚀半径 5
                int iDrakThres = int.Parse(strUserParam[11]);      //黑点或缺损阈值 30
                int iDrakArea = int.Parse(strUserParam[12]);      //黑点或缺损最小面积 100
                int iAmpSize = int.Parse(strUserParam[13]);      //梯度大小  5
                int iAmpThres = int.Parse(strUserParam[14]);      //梯度阈值  20
                int iClosing = int.Parse(strUserParam[15]);      //梯度闭运算8
                int iOpening = int.Parse(strUserParam[16]);      //梯度开运算6
                int iDiffradis = int.Parse(strUserParam[17]);      //破损区域开运算 2
                int iBrokenArea = int.Parse(strUserParam[18]);      //破损区域面积 200

                #endregion

                #region ******总定位*******
                HObject ReduceRect, ho_ImageClosing, ho_RegionMax, ho_RegionOpen, ho_Conns, ho_RegionMaxLight, ho_ImageReduced;
                HTuple NChannel;


                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    //HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                    HOperatorSet.Rgb1ToGray(hoReduced, out ho_ImageReduced);
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }




                // HOperatorSet.GrayClosingRect(ho_ImageReduced, out ho_ImageClosing, iGrayClosing, iGrayClosing); //10
                HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionMax, ithreshmax, 255); //100
                HOperatorSet.OpeningRectangle1(ho_RegionMax, out ho_RegionOpen, 10, 5);
                HOperatorSet.Connection(ho_RegionOpen, out ho_Conns);
                HOperatorSet.SelectShapeStd(ho_Conns, out ho_RegionMaxLight, "max_area", 70);
                HTuple A1, R1, C1;
                HOperatorSet.AreaCenter(ho_RegionMaxLight, out A1, out R1, out C1);
                if (A1 < 10000)
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }
                #endregion

                #region*******判断尺寸******
                HTuple hv_Row1, hv_Col1, hv_Phi1, hv_Length111, hv_Length222;
                HObject ho_RectPu;
                HOperatorSet.SmallestRectangle2(ho_RegionMaxLight, out hv_Row1, out hv_Col1, out hv_Phi1, out hv_Length111, out hv_Length222);
                HOperatorSet.GenRectangle2(out ho_RectPu, hv_Row1, hv_Col1, hv_Phi1, hv_Length111, hv_Length222);
                //绘制矩形
                List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row1, hv_Col1, hv_Phi1, hv_Length111, hv_Length222);
                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcode.ToArray());
                if ((hv_Length111 > (iLength1 + iLength1Scale)) || (hv_Length111 < (iLength1 - iLength1Scale)) || (hv_Length222 > (iLength2 + iLength2Scale)) || (hv_Length222 < (iLength2 - iLength2Scale)))
                {
                    listObj2Draw.Add("NG");
                    listObj2Draw[1] = "NG-尺寸不符";
                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length111.D.ToString("0.0") + " pix * " + hv_Length222.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }
                listObj2Draw.Add("OK");
                #endregion

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时216," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                #region*******判断外轮廓缺损或内部黑点********
                HObject ho_RegionSel, ho_RectPuEro, ho_ImageRectPuEro, ho_RegionDark, ho_Err_RegionConn;
                HTuple hv_AreaDrak, hv_RowDark, hv_ColDark, hv_Num;
                HOperatorSet.ErosionCircle(ho_RectPu, out ho_RectPuEro, iRectEro); //10
                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RectPuEro, out ho_ImageRectPuEro);
                HOperatorSet.Threshold(ho_ImageRectPuEro, out ho_RegionDark, 0, iDrakThres); // 30
                HOperatorSet.AreaCenter(ho_RegionDark, out hv_AreaDrak, out hv_RowDark, out hv_ColDark);
                if (hv_AreaDrak > iDrakArea) //100
                {
                    listObj2Draw[1] = "NG-区域缺损";
                    HOperatorSet.Connection(ho_RegionDark, out ho_Err_RegionConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iDrakArea.ToString("0.0") + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_AreaDrak.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }
                #endregion

                #region*******判断侧导露基板********
                HObject ho_DiffBig, ho_Amp, ho_RegionAmp, ho_DoffConn, ho_Closing, ho_Opening, ho_DiffRegion, ho_DiffRegionOpen;
                HTuple AreaDiffBig, RowDiffBig, ColDiffBig;
                HOperatorSet.SobelAmp(ho_ImageRectPuEro, out ho_Amp, "sum_abs", iAmpSize); //5
                HOperatorSet.Threshold(ho_Amp, out ho_RegionAmp, iAmpThres, 255); //30
                HOperatorSet.ClosingCircle(ho_RegionAmp, out ho_Closing, iClosing); //8
                HOperatorSet.OpeningCircle(ho_Closing, out ho_Opening, iOpening); //6
                HOperatorSet.Difference(ho_RectPuEro, ho_Opening, out ho_DiffRegion);
                HOperatorSet.OpeningCircle(ho_DiffRegion, out ho_DiffRegionOpen, iDiffradis); //2
                HOperatorSet.Connection(ho_DiffRegionOpen, out ho_DoffConn);
                HOperatorSet.SelectShapeStd(ho_DoffConn, out ho_DiffBig, "max_area", 70);
                HOperatorSet.AreaCenter(ho_DiffBig, out AreaDiffBig, out RowDiffBig, out ColDiffBig);
                if (AreaDiffBig > iBrokenArea)
                {
                    listObj2Draw[1] = "NG-端头破损";
                    HOperatorSet.Connection(ho_DiffBig, out ho_Err_RegionConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iBrokenArea.ToString("0.0") + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + AreaDiffBig.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }


                #endregion

#endif

                //执行到这里，OK  
                listObj2Draw[1] = "OK";
                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect8", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect56.ReleaseMutex();
            }

            #endregion
        }



















        //大毅原六面机0402算法
        public static List<object> sySixSideDetect1212(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)

        {
            #region  *** 六面机 侧面  ***
            if (bUseMutex) muDetect12.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoRegion, hoReduced, hoConcate, hoUnion;
                HObject hoEmphasize, ho_BlackReg, ho_DarkPix;
                HObject ho_RegionSelect, ho_RegionDiff, ho_ImageMean;
                HObject ho_RegionOpen, ho_RegionConnect, ho_SortedRegion, ho_RegionFill, ho_ImageMaxReduce, ho_RegionConn, ho_RegionSelect1, ho_ImageReduce, ho_ImageClosing, ho_Rectangle, ho_RectangleEro;
                HObject ho_RegionLight, ho_RegionLights, ho_BiggestRegion, ho_RegionEro, ho_ImageCheck, EdgeAmplitude, ho_Err_RegionConn, ho_RegionSel;

                HTuple hv_Area, hv_Col, hv_Phi, hv_Length1, hv_Length2, hv_Row1, hv_Col1, Areapppp, Rowpppp, Colpppp;
                HTuple hv_MaxIndex, hv_cmp, hv_I, hv_AreaSelect, hv_Row, hv_Column, Areakkk, Rowkkk, Colkkk, hv_Num;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);


                #endregion

                string[] strUserParam = strParams.Split('#');

                int iEmphasize = int.Parse(strUserParam[4]);   //iEmphasize   = 2  
                int ithresh1 = int.Parse(strUserParam[5]);   //ithresh1   = 70
                int iopening1 = int.Parse(strUserParam[6]);     //iopening1  = 1
                int iLength1 = int.Parse(strUserParam[7]);       //iLength1  = 110  
                int iLength1Scale = int.Parse(strUserParam[8]);  //iLength1Scale = 20
                int iLength2 = int.Parse(strUserParam[9]);       //iLength2  = 35
                int iLength2Scale = int.Parse(strUserParam[10]); //iLength2Scale = 15
                int ithreshErr = int.Parse(strUserParam[11]);    //ithreshErr = 15  缺陷阈值
                int iAreaErr = int.Parse(strUserParam[12]);      //iAreaErr   = 350  缺陷面积
                int ithresh3 = int.Parse(strUserParam[13]);      //ithresh3 = 200
                int iopening2 = int.Parse(strUserParam[14]);      //iopening2 = 3
                int iAreaLie = int.Parse(strUserParam[15]);      //iAreaLie = 30

                //int iEmphasize = 3;   //iEmphasize   = 2  
                //int ithresh1 = 80;   //ithresh1   = 70
                //int iopening1 = 1;     //iopening1  = 1
                //int iLength1 = 110;       //iLength1  = 110  
                //int iLength1Scale = 15;  //iLength1Scale = 20
                //int iLength2 = 35;       //iLength2  = 35
                //int iLength2Scale = 5; //iLength2Scale = 15
                //int ithreshErr = 15;    //ithreshErr = 15  缺陷阈值
                //int iAreaErr = 1000;      //iAreaErr   = 350  缺陷面积
                //int ithresh3 = 200;      //ithresh3 = 200
                //int iopening2 = 3;      //iopening2 = 3

                HObject ho_Image1, ho_ImageReduced, ho_Image3;
                HTuple NChannel;

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                HOperatorSet.Emphasize(ho_ImageReduced, out hoEmphasize, 5, 5, iEmphasize);
                HOperatorSet.Threshold(hoEmphasize, out hoRegion, ithresh1, 255);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //HOperatorSet.OpeningCircle(hoRegion, out ho_RegionOpen, iopening1);
                HOperatorSet.AreaCenter(hoRegion, out Areakkk, out Rowkkk, out Colkkk);
                if (Areakkk < 1000)
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }

                HOperatorSet.Connection(hoRegion, out ho_RegionConnect);
                HOperatorSet.SortRegion(ho_RegionConnect, out ho_SortedRegion, "upper_left", "true", "column");
                HOperatorSet.AreaCenter(ho_SortedRegion, out hv_Area, out hv_Row, out hv_Column);

                //*寻找面积最大区域
                hv_MaxIndex = 0;
                hv_cmp = 0;
                for (hv_I = 0; (int)hv_I <= (int)((hv_Area.TupleLength()) - 1); hv_I = (int)hv_I + 1)
                {
                    HOperatorSet.TupleSelect(hv_Area, hv_I, out hv_AreaSelect);
                    if ((int)(hv_AreaSelect.TupleGreater(hv_cmp)) != 0)
                    {
                        hv_MaxIndex = hv_I.Clone();
                        hv_cmp = hv_AreaSelect.Clone();
                    }
                }
                HOperatorSet.SelectObj(ho_SortedRegion, out hoRegion, hv_MaxIndex + 1);
                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时112," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //*确定需要检测的区域
                HOperatorSet.SmallestRectangle2(hoRegion, out hv_Row, out hv_Col, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.FillUp(hoRegion, out ho_RegionFill);
                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionFill, out ho_ImageMaxReduce);
                HOperatorSet.Threshold(ho_ImageMaxReduce, out hoRegion, 0, ithresh3); //ithresh3 = 200
                HOperatorSet.OpeningCircle(hoRegion, out ho_RegionOpen, iopening2);   //iopening2 = 3
                HOperatorSet.Connection(ho_RegionOpen, out ho_RegionConn);
                HOperatorSet.AreaCenter(ho_RegionConn, out hv_Area, out hv_Row1, out hv_Col1);
                HOperatorSet.SelectShape(ho_RegionConn, out ho_RegionSelect, "area",
    "and", 800, 2000);
                HOperatorSet.SelectShape(ho_RegionSelect, out ho_RegionSelect1, "column",
    "and", hv_Col - 20, hv_Col + 20);
                HOperatorSet.Difference(ho_RegionFill, ho_RegionSelect1, out ho_RegionDiff
    );
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegionConn);
                HOperatorSet.SelectShape(ho_RegionConn, out ho_RegionSelect, "area", "and",
    2000, 999999);

                //必须只能找到一个区域，找到多个或者未找到视为无定位
                hv_I = 0;
                HOperatorSet.CountObj(ho_RegionSelect, out hv_I);
                if (hv_I != 1)
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }

                HOperatorSet.SmallestRectangle2(ho_RegionSelect, out hv_Row, out hv_Col,
    out hv_Phi, out hv_Length1, out hv_Length2);

                //绘制矩形
                List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Col, hv_Phi, hv_Length1, hv_Length2);

                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcode.ToArray());
                listObj2Draw.Add("OK");

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时113," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                if (hv_Length1 < (iLength1 - iLength1Scale))
                {
                    listObj2Draw[1] = "NG-产品异常";

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }
                if (hv_Length1 > (iLength1 + iLength1Scale))
                {
                    listObj2Draw[1] = "NG-产品异常";

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }
                if (hv_Length2 < (iLength2 - iLength2Scale))
                {
                    listObj2Draw[1] = "NG-产品异常";
                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }
                if (hv_Length2 > (iLength2 + iLength2Scale))
                {
                    listObj2Draw[1] = "NG-产品异常";
                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }
                HOperatorSet.GenRectangle2(out hoRegion, hv_Row, hv_Col, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);

                #region ---- *** 检查中间陶瓷区域裂痕（使用动态阈值法）  *** ----
                HOperatorSet.Threshold(ho_ImageReduce, out ho_RegionLight, 50, 255);  //侧面阈值50-255
                HOperatorSet.Connection(ho_RegionLight, out ho_RegionLights);
                HOperatorSet.SelectShapeStd(ho_RegionLights, out ho_BiggestRegion, "max_area", 70);
                HOperatorSet.FillUp(ho_BiggestRegion, out ho_RegionFill);
                HOperatorSet.ErosionCircle(ho_RegionFill, out ho_RegionEro, 14);      //缩小14pix,减去边缘电极包裹区域干扰
                HOperatorSet.ReduceDomain(ho_ImageReduce, ho_RegionEro, out ho_ImageCheck); //获得检测区域
                HOperatorSet.MeanImage(ho_ImageCheck, out ho_ImageMean, 15, 15);  //动态阈值法检测裂痕 15 15
                HOperatorSet.DynThreshold(ho_ImageCheck, ho_ImageMean, out ho_DarkPix, 25, "dark"); //offset=25
                //HOperatorSet.SobelAmp(ho_ImageCheck, out EdgeAmplitude,"sum_abs", 5);  //使用sobel显示梯度
                //HOperatorSet.Threshold(EdgeAmplitude, out ho_RegionLight,100,255);  //检测裂痕100-255
                HOperatorSet.Connection(ho_DarkPix, out ho_RegionLights);
                HOperatorSet.SelectShapeStd(ho_RegionLights, out ho_BiggestRegion, "max_area", 70);
                HOperatorSet.AreaCenter(ho_BiggestRegion, out Areapppp, out Rowpppp, out Colpppp);
                if (Areapppp > iAreaLie)                                                  //开裂面积大于30为异常
                {
                    listObj2Draw[1] = "NG-产品异常";
                    HOperatorSet.Connection(ho_BiggestRegion, out ho_Err_RegionConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iAreaLie.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + Areapppp.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                }
                #endregion

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时114," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                #region ---- *** 检查内部缺陷  *** ----
                HOperatorSet.Emphasize(ho_ImageReduce, out hoEmphasize, 5, 5, 2);
                HOperatorSet.GrayClosingRect(hoEmphasize, out ho_ImageClosing, 10, 10);
                HOperatorSet.Threshold(ho_ImageClosing, out hoRegion, 100, 255); //ithresh2 = 100
                HOperatorSet.SmallestRectangle2(hoRegion, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.ErosionCircle(ho_Rectangle, out ho_RectangleEro, 6); //ieroi = 6

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时115," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion
                //HOperatorSet.ReduceDomain(ho_ImageProductClosing, ho_RectangleProductEro, out ho_ImageProductEro);
                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RectangleEro, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_BlackReg, 0, ithreshErr); //  ithreshErr = 15 缺陷阈值
                HOperatorSet.Connection(ho_BlackReg, out ho_RegionConnect);
                HOperatorSet.SortRegion(ho_RegionConnect, out ho_SortedRegion, "upper_left", "true", "column");
                HOperatorSet.AreaCenter(ho_SortedRegion, out hv_Area, out hv_Row, out hv_Column);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时116," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion
                //*寻找面积最大区域
                hv_MaxIndex = 0;
                hv_cmp = 0;
                for (hv_I = 0; (int)hv_I <= (int)(((hv_Area.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    HOperatorSet.TupleSelect(hv_Area, hv_I, out hv_AreaSelect);
                    if ((int)((hv_AreaSelect.TupleGreater(hv_cmp))) != 0)
                    {
                        hv_MaxIndex = hv_I.Clone();
                        hv_cmp = hv_AreaSelect.Clone();
                    }
                }
                HOperatorSet.SelectObj(ho_SortedRegion, out hoRegion, hv_MaxIndex + 1);
                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时117," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion
                if ((int)((hv_cmp.TupleGreater(iAreaErr))) != 0)
                {

                    listObj2Draw[1] = "NG-产品异常";
                    return listObj2Draw;
                }

                #endregion

                listObj2Draw[1] = "OK";
                return listObj2Draw;
            }

            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect12", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect12.ReleaseMutex();
            }
            #endregion
        }

        public static List<object> sySixSideDetect5656(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 六面机 侧面  ***

            if (bUseMutex) muDetect56.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {
                HObject hoRegion = null, hoReduced = null, hoConcate = null, hoClosing = null, hoConnection = null, hoSelect = null, hoContour = null, hoUnion = null, hoIndex = null, hoFillup = null, hoTrans = null, hoErosion = null, hoIntersection = null, hoDiffer = null, hoOpening = null;
                HTuple hvRow, hvColumn, hvDist, hvPhi, hvLength1, hvLength2, hvCount, hvUsedThreshold, hvSigma, hvRound, hvSides;
                HTuple AreaErrs, RowErrs, ColErrs;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                string[] strUserParam = strParams.Split('#');

                int ithreshmin = int.Parse(strUserParam[4]);
                int ithreshmax = int.Parse(strUserParam[5]);
                int iclosing = int.Parse(strUserParam[6]);
                bool bautoThresh = strUserParam[7] == "1" ? true : false;

                int iareamin = int.Parse(strUserParam[8]);
                int iareamax = int.Parse(strUserParam[9]);
                int iwidthmin = int.Parse(strUserParam[10]);
                int iwidthmax = int.Parse(strUserParam[11]);
                int iheightmin = int.Parse(strUserParam[12]);
                int iheightmax = int.Parse(strUserParam[13]);
                int iEdgeReserve = int.Parse(strUserParam[14]);
                int iedgeErosion = int.Parse(strUserParam[15]);
                int iedgeArea = int.Parse(strUserParam[16]);

                int iselMinarea = int.Parse(strUserParam[17]);
                float fSigmaMin = float.Parse(strUserParam[18]);
                float fSigmaMax = float.Parse(strUserParam[19]);

                HObject ho_Image1, ho_ImageReduced, ho_Image3;
                HTuple NChannel;

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                if (bautoThresh)
                {
                    HOperatorSet.BinaryThreshold(ho_ImageReduced, out hoRegion, "max_separability", "light", out hvUsedThreshold);
                }
                else
                {
                    //HOperatorSet.ExpImage(hoReduced, out hoRegion, float.Parse(strUserParam[7]));
                    //HOperatorSet.ScaleImageMax(hoRegion, out hoSelect);
                    //HOperatorSet.Threshold(hoSelect, out hoRegion, 128, 256);

                    HOperatorSet.Threshold(ho_ImageReduced, out hoRegion, ithreshmin, ithreshmax);
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时211," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HOperatorSet.ClosingCircle(hoRegion, out hoClosing, 1.5);
                HOperatorSet.Connection(hoRegion, out hoConnection);

                HTuple hvfeature = new HTuple();
                hvfeature[0] = "area";
                hvfeature[1] = "width";
                hvfeature[2] = "height";

                HTuple hvMin = new HTuple();
                hvMin[0] = iareamin;
                hvMin[1] = iwidthmin;
                hvMin[2] = iheightmin;

                HTuple hvMax = new HTuple();
                hvMax[0] = iareamax;
                hvMax[1] = iwidthmax;
                hvMax[2] = iheightmax;

                HOperatorSet.SelectShape(hoConnection, out hoRegion, hvfeature, "and", hvMin, hvMax);
                HOperatorSet.SelectShapeStd(hoRegion, out hoSelect, "max_area", 80);

                HOperatorSet.CountObj(hoSelect, out hvCount);
                if (hvCount.I == 0)
                {
                    listObj2Draw[1] = "NG-无定位";
                }
                else
                {
                    for (int ikk = 1; ikk <= hvCount.I; ikk++)
                    {
                        HOperatorSet.SelectObj(hoSelect, out hoIndex, ikk);
                        HOperatorSet.SmallestRectangle2(hoIndex, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);

                        #region ---- *** 超时处理  *** ----

                        if (sw.ElapsedMilliseconds > iTimeout)
                        {
                            sw.Stop();
                            listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时212," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                            return listObj2Draw;
                        }
                        #endregion

                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);

                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());

                        if (hvCount.I == 1)
                        {
                            listObj2Draw.Add("OK");
                        }
                        else
                        {
                            listObj2Draw.Add("NG");
                        }

                    }
                    if (hvCount.I == 1)
                    {

                    }
                    else
                    {
                        listObj2Draw[1] = "NG-区域不符";
                    }
                }

                if (hvCount.I != 1) return listObj2Draw;

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时213," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HOperatorSet.FillUp(hoSelect, out hoFillup);
                HOperatorSet.OpeningRectangle1(hoFillup, out hoOpening, 10, 10);
                HOperatorSet.Roundness(hoOpening, out hvDist, out hvSigma, out hvRound, out hvSides);

                if (hvSigma.D < fSigmaMin || hvSigma.D > fSigmaMax)
                {
                    listObj2Draw[1] = "NG-区域缺损"; listObj2Draw[5] = "NG";

                    //输出NG详情
                    lsInfo2Draw.Add("轮廓度下限：" + fSigmaMin.ToString("0.0") + " 上限：" + fSigmaMax.ToString("0.0"));
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前轮廓度：" + hvSigma.D.ToString("0.0"));
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));



                    return listObj2Draw;
                }

                HOperatorSet.ShapeTrans(hoSelect, out hoTrans, "rectangle2");
                HOperatorSet.OpeningRectangle1(hoTrans, out hoOpening, 20, 10);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时214," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //*找到区域的四个角
                HOperatorSet.SmallestRectangle2(hoOpening, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                HOperatorSet.GenRectangle2(out hoContour, hvRow, hvColumn, hvPhi, hvLength1 - iEdgeReserve, 1);
                HOperatorSet.Erosion1(hoOpening, hoContour, out hoErosion, 3);
                HOperatorSet.Difference(hoOpening, hoErosion, out hoContour);

                HOperatorSet.ErosionCircle(hoOpening, out hoErosion, iedgeErosion);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时215," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HOperatorSet.Intersection(hoContour, hoErosion, out hoIntersection);
                HOperatorSet.Difference(hoIntersection, hoFillup, out hoDiffer);

                HOperatorSet.OpeningCircle(hoDiffer, out hoOpening, 2.5);
                HOperatorSet.Connection(hoOpening, out hoConnection);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时216," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                hvfeature = new HTuple();
                hvfeature[0] = "area";
                hvfeature[1] = "anisometry";

                hvMin = new HTuple();
                hvMin[0] = iedgeArea;
                hvMin[1] = 1;

                hvMax = new HTuple();
                hvMax[0] = 9999;
                hvMax[1] = 3;

                HOperatorSet.SelectShape(hoConnection, out hoSelect, hvfeature, "and", hvMin, hvMax);
                //HOperatorSet.SelectShape(hoConnection, out hoSelect, "area", "and", iedgeArea, 99999);

                HOperatorSet.AreaCenter(hoSelect, out AreaErrs, out RowErrs, out ColErrs);

                HOperatorSet.CountObj(hoSelect, out hvCount);

                if (hvCount.I > 0)
                {
                    listObj2Draw[1] = "NG-轮廓缺损";

                    for (int ikk = 1; ikk <= hvCount.I; ikk++)
                    {
                        HOperatorSet.SelectObj(hoSelect, out hoIndex, ikk);
                        HOperatorSet.SmallestRectangle2(hoIndex, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);

                        #region ---- *** 超时处理  *** ----

                        if (sw.ElapsedMilliseconds > iTimeout)
                        {
                            sw.Stop();
                            listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时217," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                            return listObj2Draw;
                        }
                        #endregion

                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);

                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");
                    }

                    //输出NG详情
                    lsInfo2Draw.Add("缺损面积上限：" + hvMin.D.ToString("0.0") + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前缺损面积：" + AreaErrs.TupleSelect(0).D.ToString("0.0") + " pix  ");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                }
                return listObj2Draw;
            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect56", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect56.ReleaseMutex();
            }

            #endregion
        }

        public static List<object> sySixSideDetect8(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 六面机 正反面  ***

            string[] strUserParam = strParams.Split('#');
            int iWorkStation = int.Parse(strUserParam[4]); //iWorkStation
            if (iWorkStation == 40)  //4HAOGONGWEI  
            {
                //listObj2Draw[1] = "OK";
                //return listObj2Draw;
                return sySixSideDetect8_old(hoImage, lkkPolygon, strParams);
            }

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {
                HObject hoReduced = null, hoConcate = null, hoRegion = null, hoUnion = null, ho_RegionSel = null, hoRegionsConn = null, hoSelectedRegions = null, ho_Rectangle = null, ho_ImageReduce = null, ho_RectangleDia = null, ho_Edges = null, ho_ShortEdges = null;
                HObject ho_RegionLight = null, ho_Err_RegionConn = null, ho_RegionConn = null, Rectbbb = null, ho_EdgeAmp1 = null, ho_RegionLie = null, ho_RegionLies = null, ho_ImageMean = null, ho_DarkPix = null;
                HObject ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionUnion = null, ho_RegionClose = null, ho_Regionpen = null, ho_RegionTrans = null, ho_RegionDiff = null, ho_ImageReduce1 = null, ho_RegionDark = null, ho_ImageAmp = null;

                HTuple NChannel, hv_Num, hv_Length1, hv_Length2, Rowbbb, Colbbb, Phibbb, Length1bbb, Length2bbb;
                HTuple hv_Row, hv_Column, hv_Phi, hv_Area, hv_Mean, hv_Dev, hv_Row111, hv_Column111, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //读取参数
                //string[] strUserParam = strParams.Split('#');
                //int iWorkStation = int.Parse(strUserParam[4]) ; //iWorkStation


                int iProductCode = int.Parse(strUserParam[5]);  //产品类别：0--0402 ； 1--0402保护层带印刷
                int iFixThres = int.Parse(strUserParam[6]); //粗定位阈值65
                int iLengthScale = int.Parse(strUserParam[7]); //长宽变化范围15
                int iBlackArea = int.Parse(strUserParam[8]); //黑点最大面积300
                int iBlackThres = int.Parse(strUserParam[9]);  //黑点区域阈值30
                int iSmallestArea = int.Parse(strUserParam[10]); //焊锡最小面积2500
                int iBiggstArea = int.Parse(strUserParam[11]); //焊锡最大面积18000
                int iAreaDiff = int.Parse(strUserParam[12]);    //左右面积最大差异8000
                int iBaseResThres = int.Parse(strUserParam[13]); //基板相对缺陷阈值60
                int iBaseBlackArea = int.Parse(strUserParam[14]);   ////基板黑点面积最大300
                int iProtectBrokenResThres = int.Parse(strUserParam[15]); //保护层破损相对阈值 30
                int iProtectBrokenArea = int.Parse(strUserParam[16]); //保护层破损面积  300
                int iAreaLie = int.Parse(strUserParam[17]); //破裂面积150

                //int iProductCode = 0;  //产品类别：0--0402 ； 1--0402保护层带印刷
                //int iFixThres = 60; //粗定位阈值60
                //int iLengthScale = 15; //长宽变化范围15
                //int iBlackArea = 250; //黑点最大面积350
                //int iBlackThres = 30;  //黑点区域阈值30
                //int iSmallestArea = 3000; //焊锡最小面积3000
                //int iBiggstArea = 18000; //焊锡最大面积18000
                //int iAreaDiff = 8000;    //左右面积最大差异8000
                //int iBaseResThres = 60; //基板相对缺陷阈值60
                //int iBaseBlackArea = 300;   ////基板黑点面积最大300
                //int iProtectBrokenResThres = 30; //保护层破损相对阈值 30
                //int iProtectBrokenArea = 300; //保护层破损面积  250 ~ 300


                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                //开始检测 hoReduced
                HOperatorSet.Threshold(ho_ImageReduced, out hoRegion, iFixThres, 255);   //粗定位阈值75
                HOperatorSet.OpeningCircle(hoRegion, out ho_Regionpen, 3);          //开放半径3
                HOperatorSet.Connection(ho_Regionpen, out hoRegionsConn);
                HOperatorSet.SelectShape(hoRegionsConn, out hoSelectedRegions, "area", "and", 2500, 99999);  //2500
                hv_Num = 0;
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //判断正反面
                if (hv_Num == 1)
                {
                    #region ---- *** 背导朝上  *** ----

                    HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    HOperatorSet.SmallestRectangle2(hoRegion, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    //检查长宽尺寸
                    if ((hv_Length1 < (150 - iLengthScale)) || (hv_Length1 > (150 + iLengthScale)) || (hv_Length2 < (75 - iLengthScale)) || (hv_Length2 > (75 + iLengthScale)))  //长150 宽75 变化范围15
                    {
                        listObj2Draw[1] = "NG-尺寸异常";//"NG-长宽尺寸异常";
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("标准尺寸：150 pix * 75 pix  ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    //检查黑点
                    HOperatorSet.ErosionCircle(ho_Rectangle, out hoRegion, 10);    //检查黑点矩形腐蚀半径10
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);
                    HOperatorSet.Threshold(ho_ImageReduce, out hoRegion, 0, iBlackThres);               //0-30阈值 为黑点区域
                    HOperatorSet.AreaCenter(hoRegion, out hv_Area, out hv_Row, out hv_Column);
                    if (hv_Area > iBlackArea)                                                  //黑点最大面积250
                    {
                        listObj2Draw[1] = "NG-产品异常";
                        HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("缺陷最大面积：" + iBlackArea.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

#if true
                    //边缘提取检查焊锡区域
                    HOperatorSet.DilationCircle(ho_Rectangle, out ho_RectangleDia, 5);  //膨胀半径5
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_Rectangle, out ho_ImageReduce);

                    //方式1：使用sobel寻边
                    HOperatorSet.SobelAmp(ho_ImageReduce, out ho_ImageAmp, "sum_abs", 3);
                    HOperatorSet.Threshold(ho_ImageAmp, out hoRegion, 25, 255); //25
                    HOperatorSet.ClosingCircle(hoRegion, out ho_RegionClose, 8);  //闭合半径8
                    HOperatorSet.OpeningCircle(ho_RegionClose, out ho_Regionpen, 10);     //开放半径10

                    //方式2：使用edges_sub_pix寻边(耗费时间和内存,但效果稳定)
                    //HOperatorSet.GenEmptyObj(out ho_Edges);      //
                    //HOperatorSet.GenEmptyObj(out ho_ShortEdges); //
                    //ho_Edges.Dispose();                          //
                    //HOperatorSet.EdgesSubPix(ho_ImageReduce, out ho_Edges, "mderiche2", 0.7, 8, 20);
                    //ho_ShortEdges.Dispose();                     //
                    //HOperatorSet.SelectContoursXld(ho_Edges, out ho_ShortEdges, "contour_length", 0.5, 200, -0.5, 0.5);
                    //ho_Edges.Dispose();                          //
                    //HOperatorSet.GenRegionContourXld(ho_ShortEdges, out hoRegion, "margin");
                    //ho_ShortEdges.Dispose();                     //
                    //HOperatorSet.Union1(hoRegion, out ho_RegionUnion);
                    //HOperatorSet.ClosingCircle(ho_RegionUnion, out ho_RegionClose, 10);  //闭合半径10
                    //HOperatorSet.OpeningCircle(ho_RegionClose, out ho_Regionpen, 8);     //开放半径8

                    HOperatorSet.Connection(ho_Regionpen, out hoRegion);
                    HOperatorSet.SelectShape(hoRegion, out hoSelectedRegions, "area", "and", 2500, 99999);  //选取大于3000的区域作为焊锡区域
                    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    if (hv_Num != 2)  //判断区域个数是否为2
                    {
                        listObj2Draw[1] = "NG-电极提取异常";
                        //HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                    }

                    //检查导体面积、尺寸
                    HOperatorSet.AreaCenter(hoSelectedRegions, out hv_Area, out hv_Row, out hv_Column);

                    if ((hv_Area.TupleSelect(0) < iSmallestArea) || (hv_Area.TupleSelect(1) < iSmallestArea))    //面积小于2500
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积下限：" + iSmallestArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea) || (hv_Area.TupleSelect(1) > iBiggstArea))      //面积大于18000
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积上限：" + iBiggstArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于8000
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    HOperatorSet.SmallestRectangle2(hoSelectedRegions, out RowDDD, out ColDDD, out PhiDDD, out Length1DDD, out Length2DDD);
                    if ((Length1DDD.TupleSelect(0) < 0.75 * 75) || (Length1DDD.TupleSelect(1) < 0.75 * 75)) //电极长边不能小于0.75 *75
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("电极长度下限：" + "57" + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    //if (Math.Abs(PhiDDD.TupleSelect(0).D - PhiDDD.TupleSelect(1).D) > 0.300) //电极角度差不能大于0.3
                    //{
                    //    listObj2Draw[1] = "NG-电极异常";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("电极允许角度差：" + "0.3" );
                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前角度差：" + Math.Abs(PhiDDD.TupleSelect(0) - PhiDDD.TupleSelect(1)).ToString("0.0") );
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //}

                    //检查陶瓷基板区域
                    HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    HOperatorSet.Difference(ho_RegionTrans, hoRegion, out ho_RegionDiff);

                    //HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDiff);
                    //hv_Num = 0;
                    //HOperatorSet.Connection(ho_RegionDiff, out ho_RegionConn);
                    //HOperatorSet.CountObj(ho_RegionConn, out hv_Num); //提取保护层腐蚀后只能有一个区域，否则视为异常
                    //if (hv_Num != 1)
                    //{
                    //    listObj2Draw[1] = "NG-电极异常";
                    //    //HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    return listObj2Draw;
                    //}

                    HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 12);  //腐蚀半径12
                    HOperatorSet.ReduceDomain(ho_ImageReduce, hoRegion, out ho_ImageReduce1);
                    HOperatorSet.Intensity(hoRegion, ho_ImageReduce1, out hv_Mean, out hv_Dev);
                    if (hv_Mean < 75) //陶瓷基板平均灰度不应小于75
                    {
                        listObj2Draw[1] = "NG-保护层亮度异常";
                        HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                    }

                    HOperatorSet.Threshold(ho_ImageReduce1, out ho_RegionDark, 0, hv_Mean - iBaseResThres);  //基板黑点相对阈值iBaseResThres 60
                    HOperatorSet.AreaCenter(ho_RegionDark, out hv_Area, out hv_Row, out hv_Column);
                    if (hv_Area > iBaseBlackArea)  //基板黑点面积iBaseBlackArea大于300
                    {
                        listObj2Draw[1] = "NG-保护层异常";
                        HOperatorSet.Connection(ho_RegionDark, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("缺陷面积下限：" + iBaseBlackArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix  ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    //检查基板中间陶瓷裂纹
                    HOperatorSet.MeanImage(ho_ImageReduce1, out ho_ImageMean, 15, 15);  //动态阈值法检测裂痕 15 15
                    HOperatorSet.DynThreshold(ho_ImageReduce1, ho_ImageMean, out ho_DarkPix, 12, "dark"); //offset=12
                    //HOperatorSet.SobelAmp(ho_ImageReduce1,out ho_EdgeAmp1,"sum_abs",3);
                    //HOperatorSet.Threshold(ho_EdgeAmp1,out ho_RegionLie,40,255);   //裂纹阈值40-255
                    HOperatorSet.Connection(ho_DarkPix, out ho_RegionLies);
                    HOperatorSet.SelectShapeStd(ho_RegionLies, out ho_RegionLie, "max_area", 70);
                    HOperatorSet.AreaCenter(ho_RegionLie, out hv_Area, out hv_Row, out hv_Column);

                    if (hv_Area > iAreaLie)  //裂纹面积大于150即开裂
                    {
                        listObj2Draw[1] = "NG-保护层异常";
                        HOperatorSet.Connection(ho_RegionLie, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("裂纹面积下限：" + iAreaLie.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix  ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

#endif
                    #endregion

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion
                }
                else if (hv_Num == 2)
                {
                    #region ---- *** 正导朝上  *** ----
                    HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row111, out hv_Column111, out hv_Phi, out hv_Length1, out hv_Length2);

                    //检查长宽尺寸
                    int ErrNum = 0;
                    if (hv_Length1 < (150 - iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (hv_Length1 > (150 + iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (hv_Length2 < (75 - iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (hv_Length2 > (75 + iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (ErrNum != 0)
                    {
                        listObj2Draw[1] = "NG-尺寸异常"; //"NG-长宽尺寸异常"
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row111, hv_Column111, hv_Phi, hv_Length1, hv_Length2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("标准尺寸：150 pix * 75 pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, hv_Length1, hv_Length2);
                    HOperatorSet.AreaCenter(hoSelectedRegions, out hv_Area, out hv_Row, out hv_Column);
                    ErrNum = 0;
                    //检查导体面积、尺寸
                    if ((hv_Area.TupleSelect(0) < iSmallestArea) || (hv_Area.TupleSelect(1) < iSmallestArea))    //面积小于2500
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积最小值：" + iSmallestArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea) || (hv_Area.TupleSelect(1) > iBiggstArea))      //面积大于18000
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积最大值：" + iBiggstArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于8000
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    //导体长边不能小于0.75 *75
                    HOperatorSet.SmallestRectangle2(hoSelectedRegions, out RowDDD, out ColDDD, out PhiDDD, out Length1DDD, out Length2DDD);
                    if ((Length1DDD.TupleSelect(0) < 0.75 * 75) || (Length1DDD.TupleSelect(1) < 0.75 * 75))
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("电极长度最小值：" + "57" + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    //if (Math.Abs(PhiDDD.TupleSelect(0).D - PhiDDD.TupleSelect(1).D) > 0.300) //电极角度差不能大于0.3
                    //{
                    //    listObj2Draw[1] = "NG-电极异常";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("电极允许角度差：" + "0.3");
                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前角度差：" + Math.Abs(PhiDDD.TupleSelect(0) - PhiDDD.TupleSelect(1)).ToString("0.0"));
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //}

                    //取所有焊锡区域最小外接矩形,检查缺锡或缺角区域，要求焊锡区域定位准确
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out Rowbbb, out Colbbb, out Phibbb, out Length1bbb, out Length2bbb);
                    HOperatorSet.GenRectangle2(out Rectbbb, Rowbbb, Colbbb, Phibbb, Length1bbb, Length2bbb);
                    HOperatorSet.Difference(Rectbbb, ho_RegionTrans, out ho_RegionDiff);
                    HOperatorSet.OpeningCircle(ho_RegionDiff, out ho_Regionpen, 6);
                    HOperatorSet.AreaCenter(ho_Regionpen, out hv_Area, out hv_Row, out hv_Column);

                    if (hv_Area > 600)   //缺锡区域面积如果大于500并且平均灰度小于30,认为产品缺损
                    {
                        HOperatorSet.Intensity(ho_Regionpen, ho_ImageReduced, out hv_Mean, out hv_Dev);
                        if (hv_Mean < 30)
                        {
                            listObj2Draw[1] = "NG-电极异常";
                            HOperatorSet.Connection(ho_Regionpen, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("缺陷最大面积：" + "500" + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                    }

                    if (iProductCode == 1)
                    {
                        //如果是保护层印刷产品，定位中心区域(70,40)矩形
                        HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, 70, 45);  //70,40
                    }

                    //检查保护层破洞
                    HOperatorSet.Difference(ho_RegionTrans, hoRegion, out ho_RegionDiff);
                    HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 10);    //腐蚀半径10

                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);
                    HOperatorSet.Intensity(hoRegion, ho_ImageReduce, out hv_Mean, out hv_Dev);
                    if (hv_Mean > 30) //保护层平均灰度不应大于30
                    {
                        listObj2Draw[1] = "NG-保护层异常";
                        HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                    }

                    HOperatorSet.Threshold(ho_ImageReduce, out ho_RegionLight, hv_Mean + iProtectBrokenResThres, 255);  //相对亮阈值30

                    if (iProductCode == 0)  //普通0402
                    {
                        HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                        if (hv_Area > iProtectBrokenArea)
                        {   //相对亮面积大于300
                            listObj2Draw[1] = "NG-保护层破洞";
                            HOperatorSet.Connection(ho_RegionLight, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("缺陷面积下限：" + iProtectBrokenArea + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                    }
                    else if (iProductCode == 1)    //保护层印刷0402
                    {
                        HOperatorSet.Connection(ho_RegionLight, out hoRegionsConn);
                        HOperatorSet.SelectShape(hoRegionsConn, out ho_RegionLight, "area", "and", iProtectBrokenArea, 99999); //印刷面积iProtectBrokenArea>300
                        //HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_RegionLight, out hv_Num);

                        if (hv_Num != 1) //区域个数不为1则异常
                        {
                            listObj2Draw[1] = "NG-保护层异常";
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_RegionLight, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("个数：" + hv_Num.D.ToString());
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }

                        hv_Num = 0;
                        HOperatorSet.TestSubsetRegion(ho_RegionLight, ho_Rectangle, out hv_Num);
                        if (hv_Num != 1)  //印刷区域超出矩形区域
                        {
                            listObj2Draw[1] = "NG-保护层异常";
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_RegionLight, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_RegionLight, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("区域超出");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                    }

                    else
                    {

                    }
                    #endregion
                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion
                }
                else  //无定位
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }

                //执行到这里，OK  绘制 hoSelectedRegions
                listObj2Draw[1] = "OK";
                hv_Num = 0;
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                for (int i = 1; i <= hv_Num; i++)
                {
                    HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
                }

                #region ---- *** 释放  *** ----
                //hoReduced.Dispose(); 
                //hoConcate.Dispose();
                //hoRegion.Dispose();
                //hoUnion.Dispose();
                //ho_RegionSel.Dispose();
                //hoRegionsConn.Dispose();
                //hoSelectedRegions.Dispose();
                //ho_Rectangle.Dispose();
                //ho_ImageReduce.Dispose();
                //ho_RectangleDia.Dispose();
                //ho_Edges.Dispose();
                //ho_ShortEdges.Dispose();
                //ho_RegionLight.Dispose();
                //ho_Err_RegionConn.Dispose();
                //ho_RegionUnion.Dispose();
                //ho_RegionClose.Dispose();
                //ho_Regionpen.Dispose();
                //ho_RegionTrans.Dispose();
                //ho_RegionDiff.Dispose();
                //ho_ImageReduce1.Dispose();
                //ho_RegionDark.Dispose(); 
                #endregion

                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect8", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }
            #endregion
        }


        //新六面机0402算法    引用文件 6 7 8
        public static List<object> sySixSideDetect12(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 34相机 六面机前后面 ***
            if (bUseMutex) muDetect12.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoRegion, hoReduced, hoConcate, hoUnion;
                HObject hoEmphasize, ho_BlackReg, ho_DarkPix;
                HObject ho_RegionSelect, ho_RegionDiff, ho_ImageMean;
                HObject ho_RegionOpen, ho_RegionConnect, ho_SortedRegion, ho_RegionFill, ho_ImageMaxReduce, ho_RegionConn, ho_RegionSelect1, ho_ImageReduce, ho_ImageClosing, ho_Rectangle, ho_RectangleEro;
                HObject ho_RegionLight, ho_RegionLights, ho_BiggestRegion, ho_RegionEro, ho_ImageCheck, EdgeAmplitude, ho_Err_RegionConn, ho_RegionSel;

                HTuple hv_Area, hv_Col, hv_Phi, hv_Length1, hv_Length2, hv_Row1, hv_Col1, Areapppp, Rowpppp, Colpppp;
                HTuple hv_MaxIndex, hv_cmp, hv_I, hv_AreaSelect, hv_Row, hv_Column, Areakkk, Rowkkk, Colkkk, hv_Num;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);


                #endregion

                string[] strUserParam = strParams.Split('#');

                int iLength1 = int.Parse(strUserParam[4]);      //iLength1  = 145    半长
                int iLength1Scale = int.Parse(strUserParam[5]); //iLength1Scale = 15 半长变化值
                int iLength2 = int.Parse(strUserParam[6]);      //iLength2  = 45     半宽
                int iLength2Scale = int.Parse(strUserParam[7]); //iLength2Scale = 15 半宽变化值
                int iErrThres = int.Parse(strUserParam[8]);     //iErrThres  = 40    缺陷阈值40
                int iErrArea = int.Parse(strUserParam[9]);      //iErrArea = 50      缺陷面积50

                HObject ho_RegionErrDConn, ho_RegionErrD, ho_ConnectedRegionDark, ho_RegionDark, ho_ImageReduced2, ho_RegionErosion, ho_RegionOpening2, ho_RectPu, ho_RegionOpening, ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionBinary, ho_ConnectedRegions, ho_MaxRegion, ho_RegionFillUp;
                HTuple hv_Number, NChannel, hv_UsedThreshold;

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HOperatorSet.GrayClosingRect(ho_ImageReduced, out ho_ImageClosing, 11, 11);
                HOperatorSet.BinaryThreshold(ho_ImageClosing, out ho_RegionBinary, "max_separability",
                    "light", out hv_UsedThreshold);

                HOperatorSet.Connection(ho_RegionBinary, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_MaxRegion, "max_area",
                    70);

                HOperatorSet.AreaCenter(ho_MaxRegion, out Areakkk, out Rowkkk, out Colkkk);
                if (Areakkk < 800)
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }

                HOperatorSet.FillUp(ho_MaxRegion, out ho_RegionFillUp);
                HOperatorSet.OpeningRectangle1(ho_RegionFillUp, out ho_RegionOpening, 5, 5);

                HOperatorSet.SmallestRectangle2(ho_RegionOpening, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);

                HOperatorSet.GenRectangle2(out ho_RectPu, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);

                //*判断产品尺寸（145 * 45）
                HTuple hv_Length1Scale = iLength1Scale;
                HTuple hv_Length2Scale = iLength2Scale;

                if ((int)((new HTuple(hv_Length1.TupleLess(iLength1 - hv_Length1Scale))).TupleOr(new HTuple(hv_Length1.TupleGreater(
                    iLength1 + hv_Length1Scale)))) != 0)
                {
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG1 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG1.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-无定位";  //0508更改 NG-尺寸不符

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }

                if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                    iLength2 + hv_Length2Scale)))) != 0)
                {
                    //HDevelopStop();
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-无定位";  //0508更改 NG-尺寸不符

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }

                //OK绘制蓝色矩形
                List<PointF> lnBarcodeOK = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcodeOK.ToArray());
                listObj2Draw.Add("OK");

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //*检测内部缺陷
                HOperatorSet.OpeningCircle(ho_RegionOpening, out ho_RegionOpening2, 20);
                HOperatorSet.ErosionCircle(ho_RegionOpening2, out ho_RegionErosion, 10);
                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionErosion, out ho_ImageReduced2);

                HOperatorSet.Threshold(ho_ImageReduced2, out ho_RegionDark, 0, iErrThres);
                HOperatorSet.Connection(ho_RegionDark, out ho_ConnectedRegionDark);
                HOperatorSet.SelectShape(ho_ConnectedRegionDark, out ho_RegionErrD, "area",
                    "and", iErrArea, 9999999999);
                HOperatorSet.CountObj(ho_RegionErrD, out hv_Number);

                if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-导体沾污";
                    HOperatorSet.Connection(ho_RegionErrD, out ho_RegionErrDConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrDConn, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionErrDConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iErrArea.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + iErrArea.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时114," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                listObj2Draw[1] = "OK";
                return listObj2Draw;
            }

            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect12", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect12.ReleaseMutex();
            }
            #endregion
        }

        public static List<object> sySixSideDetect565656(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 12相机 六面机左右面 ***
            if (bUseMutex) muDetect12.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoRegion, hoReduced, hoConcate, hoUnion;
                HObject hoEmphasize, ho_BlackReg, ho_DarkPix;
                HObject ho_RegionSelect, ho_RegionDiff, ho_ImageMean;
                HObject ho_RegionOpen, ho_RegionConnect, ho_SortedRegion, ho_RegionFill, ho_ImageMaxReduce, ho_RegionConn, ho_RegionSelect1, ho_ImageReduce, ho_ImageClosing, ho_Rectangle, ho_RectangleEro;
                HObject ho_RegionLight, ho_RegionLights, ho_BiggestRegion, ho_RegionEro, ho_ImageCheck, EdgeAmplitude, ho_Err_RegionConn, ho_RegionSel;
                HTuple hv_Rectangularity; HObject ho_ConnectedRegionOpening;
                HTuple hv_Area, hv_Col, hv_Phi, hv_Length1, hv_Length2, hv_Row1, hv_Col1, Areapppp, Rowpppp, Colpppp;
                HTuple hv_MaxIndex, hv_cmp, hv_I, hv_AreaSelect, hv_Row, hv_Column, Areakkk, Rowkkk, Colkkk, hv_Num;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);


                #endregion

                string[] strUserParam = strParams.Split('#');

                //12相机参数
                int iLength1 = int.Parse(strUserParam[4]);      //iLength1  = 110    半长
                int iLength1Scale = int.Parse(strUserParam[5]); //iLength1Scale = 15 半长变化值
                int iLength2 = int.Parse(strUserParam[6]);      //iLength2  = 60     半宽
                int iLength2Scale = int.Parse(strUserParam[7]); //iLength2Scale = 10 半宽变化值
                int iErrThres = int.Parse(strUserParam[8]);     //iErrThres  = 40    缺陷阈值40
                int iErrArea = int.Parse(strUserParam[9]);      //iErrArea = 50      缺陷面积50
                float iRecty = float.Parse(strUserParam[10]);   //轮廓矩形度 0.85
                int iEroradis = int.Parse(strUserParam[11]);    //iEroradis=8 矩形腐蚀半径 8

                HObject ho_RegionErrDConn, ho_RegionErrD, ho_ConnectedRegionDark, ho_RegionDark, ho_ImageReduced2, ho_RegionErosion, ho_RegionOpening2, ho_RectPu, ho_RegionOpening, ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionBinary, ho_ConnectedRegions, ho_MaxRegion, ho_RegionFillUp;
                HTuple hv_Number, NChannel, hv_UsedThreshold;

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HOperatorSet.GrayClosingRect(ho_ImageReduced, out ho_ImageClosing, 11, 11);
                HOperatorSet.BinaryThreshold(ho_ImageClosing, out ho_RegionBinary, "max_separability",
                    "light", out hv_UsedThreshold);

                HOperatorSet.Connection(ho_RegionBinary, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_MaxRegion, "max_area",
                    70);

                HOperatorSet.AreaCenter(ho_MaxRegion, out Areakkk, out Rowkkk, out Colkkk);
                if (Areakkk < 800)
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }

                HOperatorSet.FillUp(ho_MaxRegion, out ho_RegionFillUp);
                HOperatorSet.OpeningRectangle1(ho_RegionFillUp, out ho_RegionOpening, 10, 5);

                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegionOpening);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegionOpening, out ho_RegionOpening, "max_area",
                    70);
                //*判断矩形度
                HOperatorSet.Rectangularity(ho_RegionOpening, out hv_Rectangularity);
                if (hv_Rectangularity < iRecty)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-无定位";
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionOpening, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionOpening, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    return listObj2Draw;
                }

                HOperatorSet.SmallestRectangle2(ho_RegionOpening, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);

                HOperatorSet.GenRectangle2(out ho_RectPu, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);

                //*判断产品尺寸（110 * 60）
                HTuple hv_Length1Scale = iLength1Scale;
                HTuple hv_Length2Scale = iLength2Scale;

                if ((int)((new HTuple(hv_Length1.TupleLess(iLength1 - hv_Length1Scale))).TupleOr(new HTuple(hv_Length1.TupleGreater(
                    iLength1 + hv_Length1Scale)))) != 0)
                {
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG1 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG1.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-无定位";

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }

                if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                    iLength2 + hv_Length2Scale)))) != 0)
                {
                    //HDevelopStop();
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-无定位";

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }

                //OK绘制蓝色矩形
                List<PointF> lnBarcodeOK = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcodeOK.ToArray());
                listObj2Draw.Add("OK");

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //*检测内部缺陷
                //HOperatorSet.OpeningCircle(ho_RegionOpening, out ho_RegionOpening2, 20);
                HOperatorSet.ErosionCircle(ho_RectPu, out ho_RegionErosion, iEroradis);  //腐蚀半径8
                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionErosion, out ho_ImageReduced2);

                HOperatorSet.Threshold(ho_ImageReduced2, out ho_RegionDark, 0, iErrThres);
                HOperatorSet.Connection(ho_RegionDark, out ho_ConnectedRegionDark);
                HOperatorSet.SelectShape(ho_ConnectedRegionDark, out ho_RegionErrD, "area",
                    "and", iErrArea, 99999);
                HOperatorSet.CountObj(ho_RegionErrD, out hv_Number);


                if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-端电极损伤";
                    HOperatorSet.Connection(ho_RegionErrD, out ho_RegionErrDConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrDConn, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionErrDConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    HObject ho_RegionErrDUnion;
                    HTuple Areajjj, Rowjjj, Coljjj;
                    HOperatorSet.Union1(ho_RegionErrD, out ho_RegionErrDUnion);
                    HOperatorSet.AreaCenter(ho_RegionErrDUnion, out Areajjj, out Rowjjj, out Coljjj);
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iErrArea.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + Areajjj.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时114," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                listObj2Draw[1] = "OK";
                return listObj2Draw;
            }

            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect56", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect12.ReleaseMutex();
            }
            #endregion
        }

        public static List<object> sySixSideDetect88888888(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 56相机 六面机 正反面  ***

            string[] strUserParam = strParams.Split('#');
            int iWorkStation = int.Parse(strUserParam[4]); //iWorkStation
            if (iWorkStation == 40)  //4HAOGONGWEI  
            {
                //listObj2Draw[1] = "OK";
                //return listObj2Draw;
                return sySixSideDetect8_old(hoImage, lkkPolygon, strParams);
            }

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {
                HObject hoReduced = null, hoConcate = null, hoRegion = null, hoUnion = null, ho_RegionSel = null, hoRegionsConn = null, hoSelectedRegions = null, ho_Rectangle = null, ho_ImageReduce = null, ho_RectangleDia = null, ho_Edges = null, ho_ShortEdges = null;
                HObject ho_RegionLight = null, ho_Err_RegionConn = null, ho_RegionConn = null, Rectbbb = null, ho_EdgeAmp1 = null, ho_RegionLie = null, ho_RegionLies = null, ho_ImageMean = null, ho_DarkPix = null;
                HObject ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionUnion = null, ho_RegionClose = null, ho_Regionpen = null, ho_RegionTrans = null, ho_RegionDiff = null, ho_ImageReduce1 = null, ho_RegionDark = null, ho_ImageAmp = null;

                HTuple NChannel, hv_Num, hv_Length1, hv_Length2, Rowbbb, Colbbb, Phibbb, Length1bbb, Length2bbb;
                HTuple hv_Row, hv_Column, hv_Phi, hv_Area, hv_Mean, hv_Dev, hv_Row111, hv_Column111, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //读取参数
                //string[] strUserParam = strParams.Split('#');
                //int iWorkStation = int.Parse(strUserParam[4]) ; //iWorkStation


                int iProductCode = int.Parse(strUserParam[5]);  //产品类别：0--0402 ； 1--0402RMS
                int iFixThres = int.Parse(strUserParam[6]);  //粗定位阈值30
                int iLengthScale = int.Parse(strUserParam[7]);  //长宽变化范围20
                int iBlackArea = int.Parse(strUserParam[8]);  //黑点最大面积500
                int iBlackThres = int.Parse(strUserParam[9]);  //黑点区域阈值30
                int iSmallestArea = int.Parse(strUserParam[10]); //焊锡最小面积6000
                int iBiggstArea = int.Parse(strUserParam[11]); //焊锡最大面积17000
                int iAreaDiff = int.Parse(strUserParam[12]); //左右面积最大差异8000
                int iBaseResThres = int.Parse(strUserParam[13]); //基板相对缺陷阈值60
                int iBaseBlackArea = int.Parse(strUserParam[14]); //基板黑点面积最大300
                int iProtectBrokenResThres = int.Parse(strUserParam[15]); //保护层破损相对阈值 30
                int iProtectBrokenArea = int.Parse(strUserParam[16]); //保护层破损面积  300
                int iAreaLie = int.Parse(strUserParam[17]); //破裂面积150
                int iAngleScale = int.Parse(strUserParam[18]); //歪斜角度正负极线 5


                //int iProductCode = 0;  //产品类别：0--0402 ； 1--0402保护层带印刷
                //int iFixThres = 60; //粗定位阈值60
                //int iLengthScale = 15; //长宽变化范围15
                //int iBlackArea = 250; //黑点最大面积350
                //int iBlackThres = 30;  //黑点区域阈值30
                //int iSmallestArea = 3000; //焊锡最小面积3000
                //int iBiggstArea = 18000; //焊锡最大面积18000
                //int iAreaDiff = 8000;    //左右面积最大差异8000
                //int iBaseResThres = 60; //基板相对缺陷阈值60
                //int iBaseBlackArea = 300;   ////基板黑点面积最大300
                //int iProtectBrokenResThres = 30; //保护层破损相对阈值 30
                //int iProtectBrokenArea = 300; //保护层破损面积  250 ~ 300


                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                //开始检测 hoReduced
                HOperatorSet.Threshold(ho_ImageReduced, out hoRegion, iFixThres, 255);   //粗定位阈值30
                //HOperatorSet.OpeningCircle(hoRegion, out ho_Regionpen, 3);          //开放半径3

                HOperatorSet.OpeningRectangle1(hoRegion, out ho_Regionpen, 2, 15);

                HOperatorSet.Connection(ho_Regionpen, out hoRegionsConn);
                HOperatorSet.SelectShape(hoRegionsConn, out hoSelectedRegions, "area", "and", 6000, 99999);  //6000
                hv_Num = 0;
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //判断正反面
                if (hv_Num == 1)
                {
                    #region ---- *** 背导朝上  *** ----

                    HOperatorSet.OpeningRectangle1(hoSelectedRegions, out hoSelectedRegions, 30, 5);

                    HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    HOperatorSet.SmallestRectangle2(hoRegion, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);


                    //0508更改，判断产品角度，歪斜过大直接无定位 正负5度
                    HTuple Deg;
                    HOperatorSet.TupleDeg(hv_Phi, out Deg);
                    if (Deg > iAngleScale || Deg < (0 - iAngleScale))
                    {
                        listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("歪斜角度:" + Deg.D.ToString("0.0") + " 度");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }


                    //检查长宽尺寸
                    if ((hv_Length1 < (160 - iLengthScale)) || (hv_Length1 > (160 + iLengthScale)) || (hv_Length2 < (85 - iLengthScale)) || (hv_Length2 > (85 + iLengthScale)))  //长160 宽85 变化范围20
                    {
                        listObj2Draw[1] = "NG-无定位";//0508更改 "NG-切割不良";
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("标准尺寸：160 pix * 85 pix  ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    //检查黑点
                    HOperatorSet.ErosionCircle(ho_Rectangle, out hoRegion, 10);    //检查黑点矩形腐蚀半径10
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);
                    HOperatorSet.Threshold(ho_ImageReduce, out hoRegion, 0, iBlackThres);               //黑点阈值iBlackThres = 30


                    //0508更改，选取最大黑点作为缺陷
                    HObject RegionCnn, ho_RegionMax;
                    HOperatorSet.Connection(hoRegion, out RegionCnn);
                    HOperatorSet.SelectShapeStd(RegionCnn, out ho_RegionMax, "max_area", 70);


                    HOperatorSet.AreaCenter(ho_RegionMax, out hv_Area, out hv_Row, out hv_Column);
                    if (hv_Area > iBlackArea)                                                  //黑点最大面积iBlackArea = 500
                    {
                        listObj2Draw[1] = "NG-导体沾污"; //"NG-产品异常";
                        HOperatorSet.Connection(ho_RegionMax, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("缺陷最大面积：" + iBlackArea.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

#if true
                    //边缘提取检查焊锡区域
                    HOperatorSet.DilationCircle(ho_Rectangle, out ho_RectangleDia, 7);  //膨胀半径7
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_Rectangle, out ho_ImageReduce);

                    //方式1：使用sobel寻边
                    HOperatorSet.SobelAmp(ho_ImageReduce, out ho_ImageAmp, "sum_abs", 3);
                    HOperatorSet.Threshold(ho_ImageAmp, out hoRegion, 20, 255); //20
                    HOperatorSet.ClosingCircle(hoRegion, out ho_RegionClose, 8);  //闭合半径8
                    HOperatorSet.OpeningCircle(ho_RegionClose, out ho_Regionpen, 8);     //开放半径8

                    HOperatorSet.Connection(ho_Regionpen, out hoRegion);
                    HOperatorSet.SelectShape(hoRegion, out hoSelectedRegions, "area", "and", 3500, 99999);  //选取大于3500的区域作为焊锡区域
                    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    if (hv_Num != 2)  //判断区域个数是否为2
                    {
                        listObj2Draw[1] = "NG-电极异常"; //"NG-电极提取异常";
                        //HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                    }

                    //检查导体面积、尺寸
                    HOperatorSet.AreaCenter(hoSelectedRegions, out hv_Area, out hv_Row, out hv_Column);

                    if ((hv_Area.TupleSelect(0) < iSmallestArea) || (hv_Area.TupleSelect(1) < iSmallestArea))    //面积小于iSmallestArea = 6000
                    {
                        listObj2Draw[1] = "NG-电极异常";//"NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积下限：" + iSmallestArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea) || (hv_Area.TupleSelect(1) > iBiggstArea))      //面积大于iBiggstArea = 17000
                    {
                        listObj2Draw[1] = "NG-电极异常"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积上限：" + iBiggstArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于iAreaDiff = 8000
                    {
                        listObj2Draw[1] = "NG-电极大小端";  //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    HOperatorSet.SmallestRectangle2(hoSelectedRegions, out RowDDD, out ColDDD, out PhiDDD, out Length1DDD, out Length2DDD);
                    if ((Length1DDD.TupleSelect(0) < 0.75 * 85) || (Length1DDD.TupleSelect(1) < 0.75 * 85)) //电极长边不能小于0.75 *85
                    {
                        listObj2Draw[1] = "NG-电极损伤";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("电极长度下限：" + "63" + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }


                    //检查陶瓷基板区域
                    HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    HOperatorSet.Difference(ho_RegionTrans, hoRegion, out ho_RegionDiff);

                    //HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDiff);
                    //hv_Num = 0;
                    //HOperatorSet.Connection(ho_RegionDiff, out ho_RegionConn);
                    //HOperatorSet.CountObj(ho_RegionConn, out hv_Num); //提取保护层腐蚀后只能有一个区域，否则视为异常
                    //if (hv_Num != 1)
                    //{
                    //    listObj2Draw[1] = "NG-电极异常";
                    //    //HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    return listObj2Draw;
                    //}

                    HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 14);  //腐蚀半径14
                    HOperatorSet.ReduceDomain(ho_ImageReduce, hoRegion, out ho_ImageReduce1);
                    HOperatorSet.Intensity(hoRegion, ho_ImageReduce1, out hv_Mean, out hv_Dev);
                    if (hv_Mean < 45) //陶瓷基板平均灰度不应小于45
                    {
                        listObj2Draw[1] = "NG-非导电性沾污"; //"NG-保护层亮度异常"
                        HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                    }

                    HOperatorSet.Threshold(ho_ImageReduce1, out ho_RegionDark, 0, hv_Mean - iBaseResThres);  //基板黑点相对阈值iBaseResThres = 60
                    HOperatorSet.AreaCenter(ho_RegionDark, out hv_Area, out hv_Row, out hv_Column);
                    if (hv_Area > iBaseBlackArea)  //基板黑点面积大于iBaseBlackArea = 300
                    {
                        listObj2Draw[1] = "NG-非导电性沾污";//"NG-保护层异常"
                        HOperatorSet.Connection(ho_RegionDark, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("缺陷面积下限：" + iBaseBlackArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix  ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    //检查基板中间陶瓷裂纹
                    HOperatorSet.MeanImage(ho_ImageReduce1, out ho_ImageMean, 15, 15);  //动态阈值法检测裂痕 15 15
                    HOperatorSet.DynThreshold(ho_ImageReduce1, ho_ImageMean, out ho_DarkPix, 12, "dark"); //offset=12
                    //HOperatorSet.SobelAmp(ho_ImageReduce1,out ho_EdgeAmp1,"sum_abs",3);
                    //HOperatorSet.Threshold(ho_EdgeAmp1,out ho_RegionLie,40,255);   //裂纹阈值40-255
                    HOperatorSet.Connection(ho_DarkPix, out ho_RegionLies);
                    HOperatorSet.SelectShapeStd(ho_RegionLies, out ho_RegionLie, "max_area", 70);
                    HOperatorSet.AreaCenter(ho_RegionLie, out hv_Area, out hv_Row, out hv_Column);

                    if (hv_Area > iAreaLie)  //裂纹面积大于iAreaLie = 150即开裂
                    {
                        listObj2Draw[1] = "NG-非导电性侧裂";//"NG-保护层异常"
                        HOperatorSet.Connection(ho_RegionLie, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("裂纹面积下限：" + iAreaLie.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix  ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

#endif
                    #endregion

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion
                }
                else if (hv_Num == 2)
                {
                    #region ---- *** 正导朝上  *** ----
                    HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row111, out hv_Column111, out hv_Phi, out hv_Length1, out hv_Length2);

                    //0508更改，判断产品角度，歪斜过大直接无定位 正负5度
                    HTuple Deg;
                    HOperatorSet.TupleDeg(hv_Phi, out Deg);
                    if (Deg > iAngleScale || Deg < (0 - iAngleScale))
                    {
                        listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row111, hv_Column111, hv_Phi, hv_Length1, hv_Length2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("歪斜角度:" + Deg.D.ToString("0.0") + " 度");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    //检查长宽尺寸
                    int ErrNum = 0;
                    if (hv_Length1 < (160 - iLengthScale)) //iLengthScale = 20
                    {
                        ErrNum += 1;
                    }
                    if (hv_Length1 > (160 + iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (hv_Length2 < (85 - iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (hv_Length2 > (85 + iLengthScale))
                    {
                        ErrNum += 1;
                    }
                    if (ErrNum != 0)
                    {
                        listObj2Draw[1] = "NG-无定位"; //0508更改"NG-切割不良"
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row111, hv_Column111, hv_Phi, hv_Length1, hv_Length2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("标准尺寸：150 pix * 75 pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                    }

                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, hv_Length1, hv_Length2);
                    HOperatorSet.AreaCenter(hoSelectedRegions, out hv_Area, out hv_Row, out hv_Column);
                    ErrNum = 0;
                    //检查导体面积、尺寸
                    if ((hv_Area.TupleSelect(0) < iSmallestArea) || (hv_Area.TupleSelect(1) < iSmallestArea))    //面积小于iSmallestArea = 6000
                    {
                        listObj2Draw[1] = "NG-电极异常"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积最小值：" + iSmallestArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea) || (hv_Area.TupleSelect(1) > iBiggstArea))      //面积大于iBiggstArea = 17000
                    {
                        listObj2Draw[1] = "NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积最大值：" + iBiggstArea.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "pix ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于iAreaDiff = 8000
                    {
                        listObj2Draw[1] = "NG-电极大小端";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    //导体长边不能小于0.75 *85
                    HOperatorSet.SmallestRectangle2(hoSelectedRegions, out RowDDD, out ColDDD, out PhiDDD, out Length1DDD, out Length2DDD);
                    if ((Length1DDD.TupleSelect(0) < 0.75 * 85) || (Length1DDD.TupleSelect(1) < 0.75 * 85))
                    {
                        listObj2Draw[1] = "NG-电极损伤"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("电极长度最小值：" + "63" + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                    }

                    //if (Math.Abs(PhiDDD.TupleSelect(0).D - PhiDDD.TupleSelect(1).D) > 0.300) //电极角度差不能大于0.3
                    //{
                    //    listObj2Draw[1] = "NG-电极异常";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("电极允许角度差：" + "0.3");
                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前角度差：" + Math.Abs(PhiDDD.TupleSelect(0) - PhiDDD.TupleSelect(1)).ToString("0.0"));
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //}

                    //取所有焊锡区域最小外接矩形,检查缺锡或缺角区域，要求焊锡区域定位准确
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out Rowbbb, out Colbbb, out Phibbb, out Length1bbb, out Length2bbb);
                    HOperatorSet.GenRectangle2(out Rectbbb, Rowbbb, Colbbb, Phibbb, Length1bbb, Length2bbb);
                    HOperatorSet.Difference(Rectbbb, ho_RegionTrans, out ho_RegionDiff);
                    HOperatorSet.OpeningCircle(ho_RegionDiff, out ho_Regionpen, 6);
                    HOperatorSet.AreaCenter(ho_Regionpen, out hv_Area, out hv_Row, out hv_Column);

                    if (hv_Area > 1000)   //缺锡区域面积如果大于1000并且平均灰度小于30,认为产品缺损
                    {
                        HOperatorSet.Intensity(ho_Regionpen, ho_ImageReduced, out hv_Mean, out hv_Dev);
                        if (hv_Mean < 30)
                        {
                            listObj2Draw[1] = "NG-电极沾污";
                            HOperatorSet.Connection(ho_Regionpen, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("缺陷最大面积：" + "1000" + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                    }

                    if (iProductCode == 1)
                    {
                        //如果是保护层印刷产品，定位中心区域(70,40)矩形
                        HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, 70, 45);  //70,40
                    }

                    //检查保护层破洞
                    HOperatorSet.Difference(ho_RegionTrans, hoRegion, out ho_RegionDiff);
                    //HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 12);    //腐蚀半径12
                    HOperatorSet.ErosionRectangle1(ho_RegionDiff, out hoRegion, 12, 25);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);
                    HOperatorSet.Intensity(hoRegion, ho_ImageReduce, out hv_Mean, out hv_Dev);
                    if (hv_Mean > 30) //保护层平均灰度不应大于30
                    {
                        listObj2Draw[1] = "NG-导体沾污";
                        HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                    }

                    HOperatorSet.Threshold(ho_ImageReduce, out ho_RegionLight, hv_Mean + iProtectBrokenResThres, 255);  //相对亮阈值iProtectBrokenResThres=30

                    if (iProductCode == 0)  //普通0402
                    {
                        HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                        if (hv_Area > iProtectBrokenArea)
                        {   //相对亮面积大于300
                            listObj2Draw[1] = "NG-保护层破洞";
                            HOperatorSet.Connection(ho_RegionLight, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("缺陷面积下限：" + iProtectBrokenArea + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                    }
                    else if (iProductCode == 1)    //保护层印刷0402
                    {
                        HOperatorSet.Connection(ho_RegionLight, out hoRegionsConn);
                        HOperatorSet.SelectShape(hoRegionsConn, out ho_RegionLight, "area", "and", iProtectBrokenArea, 99999); //印刷面积iProtectBrokenArea>300
                        //HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_RegionLight, out hv_Num);

                        if (hv_Num != 1) //区域个数不为1则异常
                        {
                            listObj2Draw[1] = "NG-保护层异常";
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_RegionLight, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("个数：" + hv_Num.D.ToString());
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }

                        hv_Num = 0;
                        HOperatorSet.TestSubsetRegion(ho_RegionLight, ho_Rectangle, out hv_Num);
                        if (hv_Num != 1)  //印刷区域超出矩形区域
                        {
                            listObj2Draw[1] = "NG-保护层异常";
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_RegionLight, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_RegionLight, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("区域超出");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                    }

                    else
                    {

                    }
                    #endregion
                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion
                }
                else  //无定位
                {
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }

                //执行到这里，OK  绘制 hoSelectedRegions
                listObj2Draw[1] = "OK";
                hv_Num = 0;
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                for (int i = 1; i <= hv_Num; i++)
                {
                    HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
                }


                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect8", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }
            #endregion
        }



        //调试输出内容
        public static string DebugPrint(string Str, bool isDebug)
        {
            if (isDebug)
                return Str;
            else
                return "0";
        }


        //六面机0201电容 3456 相机 算法  引用文件12
        public static List<object> sySixSideDetect22(HObject hoImage, List<PointF[]> lkkPolygon, string strParams, ref string strMessage)
        {
            #region  *** 六面机0201电容 3456 相机  ***

            if (bUseMutex) muDetect22.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            HObject hoConcate, hoRegion, hoUnion, hoReduced;

            try
            {
                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //读取参数
                string[] strUserParam = strParams.Split('#');
                int iStation = int.Parse(strUserParam[4]);     //工位号   34  56
                int iCode = int.Parse(strUserParam[21]);     //料代码 R料（黑料）=1 ； B料（黄料） = 2 ；   
                int iOnlyWuDingwei = 0;

                //开放所有参数
                //参数1 ：34工位整体定位阈值 20
                int iLocalThres34 = int.Parse(strUserParam[30]);
                //参数2 ：34工位整体定位矩形半长  150
                int iLocalLength2_34 = int.Parse(strUserParam[31]);
                //参数3 ：34工位分割倒影阈值  15
                int iLocalPairThres34 = int.Parse(strUserParam[32]);
                //参数4 ：34工位膨胀定位半长 500
                int iLocalDilaLength1_34 = int.Parse(strUserParam[33]);

                //参数5 ：56工位整体定位阈值 10
                int iLocalThres56 = int.Parse(strUserParam[34]);
                //参数6 ：56工位整体定位开运算 5
                int iLocalOpen56 = int.Parse(strUserParam[35]);
                //参数7 ：56工位整体定位分割左右两边半宽 150
                int iLocalRectLength2_56 = int.Parse(strUserParam[36]);
                //参数8 ：电极定位梯度大小 5
                int iElectSobelSize = int.Parse(strUserParam[37]);


                //参数9 ：电极定位梯度阈值 20
                int iElectSobelThres = int.Parse(strUserParam[38]);
                //参数10：电极定位梯度过滤面积 10
                int iElectSobelFilterArea = int.Parse(strUserParam[39]);
                //参数11：电极定位梯度闭运算  20
                int iElectSobelClosing = int.Parse(strUserParam[40]);
                //参数12：电极定位梯度开运算 10
                int iElectSobelOpening = int.Parse(strUserParam[41]);



                //参数13：电极定位电极过滤面积  6000
                int iElectFilterArea = int.Parse(strUserParam[42]);
                //参数14：电极定位电极闭运算  15
                int iElectClosing = int.Parse(strUserParam[43]);
                //参数15：电极定位开运算构造矩形半宽 40
                int iElectStructOpenLength2 = int.Parse(strUserParam[44]);

                //45 电极横向开运算  15
                int iElectHorizStructOpenLength2 = int.Parse(strUserParam[45]);













                //参数16：产品矩形度最小值 
                float iPuRecty = float.Parse(strUserParam[46]);
                //参数17：无定位角度  15
                int iWaiAngle = int.Parse(strUserParam[47]);
                //参数18：产品半长     
                int iPuLength1 = int.Parse(strUserParam[48]);
                //参数19：产品半长变化 
                int iPuLength1Scale = int.Parse(strUserParam[49]);


                //参数20：产品半宽     
                int iPuLength2 = int.Parse(strUserParam[50]);
                //参数21：产品半宽变化 
                int iPuLength2Scale = int.Parse(strUserParam[51]);
                //参数22：电极矩形度最小值 
                float iElectrodeRecty = float.Parse(strUserParam[52]);
                //参数23： 电极半长        
                int iElecLength1 = int.Parse(strUserParam[53]);


                //参数24： 电极半长变化值  
                int iElecLength1Scale = int.Parse(strUserParam[54]);
                //参数25： 电极半宽        
                int iElecLength2 = int.Parse(strUserParam[55]);
                //参数26： 电极半宽变化值  
                int iElecLength2Scale = int.Parse(strUserParam[56]);

                //参数57~61预留











                //参数27：电极上爬掩膜大小 20
                int iElectSPMeanSize = int.Parse(strUserParam[62]);
                //参数28 ：电极上爬动态阈值  10
                int iElectSPDynThres = int.Parse(strUserParam[63]);
                //参数29 ：电极上爬过滤灰度值 120
                int iElectSPFilterThres = int.Parse(strUserParam[64]);
                //参数30 ：电极上爬过滤面积 250
                int iElectSPFilterArea = int.Parse(strUserParam[65]);


                //参数31 ：电极上爬宽度相对瓷体宽度 0.2
                float iElectSPWideSacle = float.Parse(strUserParam[66]);
                //参数32 ：瓷体定位腐蚀长度 40
                int iCiTiEroWidth = int.Parse(strUserParam[67]);
                //参数33 ：瓷体定位腐蚀宽度 R:50 / B:60
                int iCiTiEroHeight = int.Parse(strUserParam[68]);


                //参数34 ：瓷体缺陷掩膜大小 

                string[] strUserParamMask = strUserParam[69].Split('+');
                int iZSMaskWidth = int.Parse(strUserParamMask[0]);
                int iZSMaskHeight = int.Parse(strUserParamMask[1]);

                //int iZSMask = int.Parse(strUserParam[69]);


                //参数35 ：5    黑白缺陷动态阈值 = 5
                int iZSThres = int.Parse(strUserParam[70]);
                //参数36 ：
                int iZSArea = int.Parse(strUserParam[71]);
                //参数37 ：瓷损漏电极发黑阈值  5
                int iCSLDJThres = int.Parse(strUserParam[72]);
                //参数38 ：瓷损漏电极发黑面积  100
                int iCSLDJArea = int.Parse(strUserParam[73]);


                //参数39 ：瓷体颜色灰度      R : 35 B : 70
                int iCurrentMean = int.Parse(strUserParam[74]);
                //参数40 ：瓷体颜色变化 
                int iCurrentMeanScale = int.Parse(strUserParam[75]);

                //参数76~77预留






                //参数76 ：瓷体检测区域变化宽度      //0 为保持不变  正数为变宽，负数为变窄
                int iRegionCiTiWidthScale = int.Parse(strUserParam[76]);
                //参数76 ：瓷体检测区域变化高度      //0 为保持不变  正数为变高，负数为变矮
                int iRegionCiTiHighScale = int.Parse(strUserParam[77]);











                //参数41 ：棱边瓷损高度设定值 20
                int iLBHigh = int.Parse(strUserParam[78]);
                //参数42 ：棱边定位矩形半宽 25
                int iLBStructLength2 = int.Parse(strUserParam[79]);
                //参数43 ：棱边瓷损检测梯度大小 7
                int iLBSobelSize = int.Parse(strUserParam[80]);
                //参数44 ：棱边瓷损检测梯度阈值 R:10 B:15
                int iLBSobelThres = int.Parse(strUserParam[81]);


                //参数45 ：电极黑缺陷检测电极腐蚀半径 5
                int iElectBlackErrEro = int.Parse(strUserParam[82]);
                //参数46 ：电极黑缺陷阈值  
                int iGSThres = int.Parse(strUserParam[83]);
                //参数47 ：电极黑缺陷面积  
                int iGSArea = int.Parse(strUserParam[84]);
                //参数48 ：漏铜阈值 50
                int iLTThres = int.Parse(strUserParam[85]);



                //参数49 ：漏铜面积 1000
                int iLTArea = int.Parse(strUserParam[86]);
                //参数50 ：锡瘤阈值 50
                int iXLThres = int.Parse(strUserParam[87]);
                //参数51 ：锡瘤面积 150
                int iXLArea = int.Parse(strUserParam[88]);
                //参数52 ：锡瘤面内接圆半径 4 
                int iXLRadis = int.Parse(strUserParam[89]);



                //参数53 ：棱边瓷损检测区域变化宽度  //0 为保持不变  正数为变宽，负数为变窄
                int iLBWidthScale = int.Parse(strUserParam[90]);

                //参数54 ：电极收缩区域半径
                int iDianjiShousuoRadis = int.Parse(strUserParam[91]);

                //参数55 ：凹坑指数
                int iT12Diff = int.Parse(strUserParam[92]);

                //参数90~93预留







                bool iCheckSelAll = false;   //是否全检还是依赖于外部选择
                //用户检测项可选项 strUserParam[94]开始  , 0：屏蔽 1：启用 ， 默认 1（打钩）
                int A_产品尺寸 = iCheckSelAll ? 1 : int.Parse(strUserParam[94]); //产品尺寸
                int A_电极尺寸 = iCheckSelAll ? 1 : int.Parse(strUserParam[95]); //电极尺寸
                int A_电极爬镀 = iCheckSelAll ? 1 : int.Parse(strUserParam[96]); //电极爬镀
                int A_瓷体损伤 = iCheckSelAll ? 1 : int.Parse(strUserParam[97]); //瓷体损伤
                int A_瓷损漏电极 = iCheckSelAll ? 1 : int.Parse(strUserParam[98]); //瓷损漏电极
                int A_瓷体颜色不一致 = iCheckSelAll ? 1 : int.Parse(strUserParam[99]); //瓷体颜色不一致
                int A_棱边瓷损 = iCheckSelAll ? 1 : int.Parse(strUserParam[100]); //棱边瓷损
                int A_电极刮伤 = iCheckSelAll ? 1 : int.Parse(strUserParam[101]); //电极刮伤
                int A_电极漏铜 = iCheckSelAll ? 1 : int.Parse(strUserParam[102]); //电极漏铜
                int A_锡瘤 = iCheckSelAll ? 1 : int.Parse(strUserParam[103]);     //锡瘤



                //判断是否调试输出
                bool is_Debug;
                if (strMessage == "1")
                    is_Debug = true;
                else
                    is_Debug = false;
                //初始化调试输出内容
                string strDebug = "";

                if (is_Debug)
                {
                    syShowRegionBorder(hoUnion, ref listObj2Draw, "NG");  //调试状态显示reduce区域
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HObject ho_Rectangleqqq, ho_Rectangleppp, ho_RegionTrans, ho_RegionLR, ho_RegionL, ho_RegionR, ho_ConnectedRegions2, ho_RegionsBig, ho_RegionEdge, ho_ImageReduced, ho_ImageReduced2, ho_Image1, ho_Image2, ho_Image3, ho_RegionLight, ho_RegionBig, ho_RegionOpening, ho_ConnectedRegions, ho_RegionFillUp;
                HObject ho_RegionErr, ho_ConnectedRegions3, ho_RegionBlack, ho_ImageReducekkkkk, ho_RegionErosion, ho_RegionUnionkkkkk, ho_RegionSel, ho_RegionFillUp1, ho_RegionOpening1, ho_EdgeAmplitude, ho_Regionk, ho_ConnectedRegions1, ho_SelectedRegions, ho_RegionUnion, ho_RegionClosing;
                HTuple NChannel, hv_Rowqqq, hv_Colqqq, hv_Phiqqq, hv_Length1qqq, hv_Length2qqq, hv_Recties, hv_Rowppp, hv_Colppp, hv_Phippp, hv_Length1ppp, hv_Length2ppp, hv_Recty, hv_N, hv_Num;

                //*一、产品定位
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    //HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                    HOperatorSet.Rgb1ToGray(hoReduced, out ho_ImageReduced);
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                //34工位整体定位
                if (iStation == 34)
                {
                    HObject RegionDiffkkk, ho_Dia, ho_Select, ho_EdgeAmplitude1, ho_Region, ho_Rect, ho_Cnn, ho_RegionCenter, ho_ImageCenter;
                    HTuple Areakkk, Rowkkk, Colkkk;
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, iLocalThres34, 255);    //参数1：34工位整体定位阈值 iLocalThres34 = 20
                    HOperatorSet.AreaCenter(ho_Region, out Areakkk, out Rowkkk, out Colkkk);
                    HOperatorSet.GenRectangle2(out ho_Rect, Rowkkk, Colkkk, 0, 20, iLocalLength2_34); //参数2：34工位整体定位矩形半长 iLocalLength2_34 = 150
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_Rect, out ho_ImageCenter);
                    HOperatorSet.Threshold(ho_ImageCenter, out ho_RegionCenter, iLocalPairThres34, 255);  //参数3：34工位分割倒影阈值  iLocalPairThres34 = 15      15 定位瓷体区域
                    HOperatorSet.Connection(ho_RegionCenter, out ho_Cnn);
                    HOperatorSet.SelectShapeStd(ho_Cnn, out ho_Select, "max_area", 70);



                    //通过ho_Select水平角度判断锡瘤




                    HOperatorSet.DilationRectangle1(ho_Select, out ho_Dia, iLocalDilaLength1_34, 10);       //参数4：34工位膨胀定位半长 iLocalDilaLength1_34 = 500
                    //HOperatorSet.ReduceDomain(ho_ImageReduced, ho_Dia, out ho_ImageReduced2);
                    HOperatorSet.Difference(ho_Dia, ho_Rect, out RegionDiffkkk);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, RegionDiffkkk, out ho_ImageReduced2);


                    if (is_Debug)  //调试状态输出信息
                    {
                        strDebug += "(1)总定位相关参数：\n";
                        strDebug += "当前工位：34\n";
                        strDebug += "总定位阈值:" + iLocalThres34.ToString() + "\n";
                        strDebug += "定位矩形半长:" + iLocalLength2_34.ToString() + "\n";
                        strDebug += "分割倒影阈值:" + iLocalPairThres34.ToString() + "\n";
                        strDebug += "膨胀定位半长:" + iLocalDilaLength1_34.ToString() + "\n";
                    }
                }
                else
                {
                    //56工位整体定位
                    HObject Rect, ho_RegionFillkkk, Cnnkkk, DiffKKK;
                    HTuple Rowkkk, Colkkk, Phikkk, L1kkk, L2kkk;
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionLight, iLocalThres56, 255); //参数5：56工位整体定位阈值 iLocalThres56 = 10     整体阈值10
                    HOperatorSet.OpeningCircle(ho_RegionLight, out ho_RegionOpening, iLocalOpen56);  //参数6：56工位整体定位开运算 iLocalOpen56 = 5     5
                    HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillkkk);
                    HOperatorSet.Connection(ho_RegionFillkkk, out Cnnkkk);
                    HOperatorSet.SelectShapeStd(Cnnkkk, out ho_RegionOpening, "max_area", 70);
                    HOperatorSet.SmallestRectangle2(ho_RegionOpening, out Rowkkk, out Colkkk, out Phikkk, out L1kkk, out L2kkk);




                    HOperatorSet.GenRectangle2(out Rect, Rowkkk, Colkkk, Phikkk, 20, iLocalRectLength2_56); //参数7：56工位整体定位分割左右两边半宽 iLocalRectLength2_56 = 150







                    HOperatorSet.Difference(ho_RegionOpening, Rect, out DiffKKK);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, DiffKKK, out ho_ImageReduced2);

                    if (is_Debug)  //调试状态输出信息
                    {
                        strDebug += "(1)总定位相关参数：\n";
                        strDebug += "当前工位：56\n";
                        strDebug += "总定位阈值:" + iLocalThres56.ToString() + "\n";
                        strDebug += "56工位整体定位开运算:" + iLocalOpen56.ToString() + "\n";
                        strDebug += "56工位整体定位分割左右两边半宽:" + iLocalRectLength2_56.ToString() + "\n";
                    }


                }

                strDebug += "\n";



                //使用mean将产品区域准确定位














                //定位电极区域
                //使用sobelamp
                HObject ho_Conn2, ho_RegionBig1, ho_Conn3, ho_DianjiOpen, ho_RegionsDianji2, ho_RegionsDianji1, SelectCnn, ho_RegionsDianji, EdgeAmplitude, ho_RegionCnn, ho_RegionFill, ho_RegionClosing2;










                HOperatorSet.SobelAmp(ho_ImageReduced2, out EdgeAmplitude, "sum_abs", iElectSobelSize);// 参数8：电极定位梯度大小iElectSobelSize = 5  
                HOperatorSet.Threshold(EdgeAmplitude, out ho_RegionEdge, iElectSobelThres, 255);       // 参数9：电极定位梯度阈值iElectSobelThres= 20        
                HOperatorSet.Connection(ho_RegionEdge, out ho_RegionCnn);
                HOperatorSet.SelectShape(ho_RegionCnn, out ho_RegionBig, "area", "and", iElectSobelFilterArea, 999999); //参数10：电极定位梯度过滤面积iElectSobelFilterArea = 10
                HOperatorSet.Union1(ho_RegionBig, out ho_RegionBig1);
                HOperatorSet.ClosingRectangle1(ho_RegionBig1, out ho_RegionClosing, iElectSobelClosing, iElectSobelClosing);      //参数11：电极定位梯度闭运算iElectSobelClosing = 20
                HOperatorSet.OpeningRectangle1(ho_RegionClosing, out ho_RegionOpening, iElectSobelOpening, iElectSobelOpening);   //参数12：电极定位梯度开运算iElectSobelOpening = 10
                HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFill);
                HOperatorSet.Connection(ho_RegionFill, out ho_Conn2);



                //HObject ImageMeankkk, RegionFillUpkkk, RegionDynThreshkkk,  RegionClosingkkk,RegionOpeningkkk;
                //HOperatorSet.MeanImage (ho_ImageReduced2, out ImageMeankkk, 200, 200);
                //HOperatorSet.DynThreshold(ho_ImageReduced2, ImageMeankkk, out RegionDynThreshkkk, 15, "light");
                //HOperatorSet.FillUp(RegionDynThreshkkk, out RegionFillUpkkk);
                //HOperatorSet.ClosingCircle(RegionFillUpkkk, out RegionClosingkkk, 12);
                //HOperatorSet.FillUp(RegionClosingkkk, out RegionFillUpkkk);
                //HOperatorSet.OpeningRectangle1(RegionFillUpkkk, out RegionOpeningkkk, 20, 20);
                //HOperatorSet.Connection(RegionOpeningkkk, out ho_Conn2);










                HOperatorSet.SelectShape(ho_Conn2, out SelectCnn, "area", "and", iElectFilterArea, 999999);     //参数13：电极定位电极过滤面积 iElectFilterArea = 6000
                HOperatorSet.ClosingCircle(SelectCnn, out ho_RegionClosing2, iElectClosing);                   //参数14：电极定位电极闭运算  iElectClosing = 15
                HOperatorSet.Connection(ho_RegionClosing2, out ho_RegionsDianji);
                HOperatorSet.FillUp(ho_RegionsDianji, out ho_RegionsDianji1);
                HOperatorSet.SelectShape(ho_RegionsDianji1, out ho_RegionsDianji2, "area", "and", iElectFilterArea, 999999); //同参数13：电极定位电极过滤面积 iElectFilterArea = 6000

                HOperatorSet.CountObj(ho_RegionsDianji2, out hv_N);

                strDebug += "  总定位识别电极个数:" + hv_N.D.ToString() + "\n";

                if (is_Debug)  //调试状态输出信息
                {
                    strDebug += "(2)电极定位相关参数：\n";
                    strDebug += "梯度大小:" + iElectSobelSize.ToString() + "\n";
                    strDebug += "梯度阈值:" + iElectSobelThres.ToString() + "\n";
                    strDebug += "梯度过滤面积:" + iElectSobelFilterArea.ToString() + "\n";
                    strDebug += "梯度闭运算:" + iElectSobelClosing.ToString() + "\n";
                    strDebug += "梯度开运算:" + iElectSobelOpening.ToString() + "\n";
                    strDebug += "电极闭运算:" + iElectClosing.ToString() + "\n";
                    strDebug += "电极过滤面积:" + iElectFilterArea.ToString() + "\n";

                    if (hv_N != 2)
                    {
                        strDebug += "电极定位失败!\n";
                    }
                    else
                    {
                        HTuple Rowtmp, Coltmp, AreaTmp;
                        HOperatorSet.AreaCenter(ho_RegionsDianji2, out AreaTmp, out Rowtmp, out Coltmp);
                        strDebug += "当前两个电极面积:" + AreaTmp.TupleSelect(0).D.ToString("0.0") + ";" + AreaTmp.TupleSelect(1).D.ToString("0.0") + "\n";
                    }
                }

                strDebug += "\n";

                if (hv_N > 2 || hv_N == 0)
                {
                    listObj2Draw[1] = "NG-无定位";
                    //绘制电极轮廓  ho_DianjiOpen
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionsDianji2, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionsDianji2, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    strMessage = DebugPrint(strDebug, is_Debug);
                    return listObj2Draw;
                }
                HObject RegionBinUnion, StructOpen, ho_DianjiOpen1;
                HTuple RowBin, ColumnBin, PhiBin, Length1Bin, Length2Bin;
                HOperatorSet.Union1(ho_RegionsDianji2, out RegionBinUnion);
                HOperatorSet.SmallestRectangle2(RegionBinUnion, out RowBin, out ColumnBin, out PhiBin, out Length1Bin, out Length2Bin);
                HOperatorSet.GenRectangle2(out StructOpen, 100, 100, PhiBin, 1, iElectStructOpenLength2);   //参数15：电极定位开运算构造矩形半宽 iElectStructOpenLength2 = 40
                HOperatorSet.Opening(RegionBinUnion, StructOpen, out ho_DianjiOpen1);
                // HOperatorSet.OpeningRectangle1(ho_RegionsDianji2,out ho_DianjiOpen,2,80); //2 60



                //参数电极横向开运算 iElectHorizStructOpenLength2 15
                if (iElectHorizStructOpenLength2 == 0)
                    iElectHorizStructOpenLength2 = 15;
                HObject StructOpen2, ho_DianjiOpen2;
                HOperatorSet.GenRectangle2(out StructOpen2, 100, 100, PhiBin, iElectHorizStructOpenLength2, 1);   //电极横向开运算参数
                HOperatorSet.Opening(ho_DianjiOpen1, StructOpen2, out ho_DianjiOpen2);



                HOperatorSet.Connection(ho_DianjiOpen2, out ho_Conn3);
                HOperatorSet.SelectShape(ho_Conn3, out ho_DianjiOpen, "area", "and", iElectFilterArea, 999999); //同参数13：电极定位电极过滤面积 iElectFilterArea = 6000
                HOperatorSet.CountObj(ho_DianjiOpen, out hv_N);

                if (is_Debug)  //调试状态输出信息
                {
                    strDebug += "(3)电极构造参数：\n";
                    strDebug += "电极定位开运算构造矩形半宽:" + iElectStructOpenLength2.ToString() + "\n";
                    strDebug += "定位电极个数:" + hv_N.D.ToString() + "\n";
                }
                strDebug += "\n";

                //判断电极个数是否为2
                if (hv_N > 2 || hv_N == 0)
                {
                    listObj2Draw[1] = "NG-无定位";
                    //绘制电极轮廓  ho_DianjiOpen
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    strMessage = DebugPrint(strDebug, is_Debug);
                    return listObj2Draw;
                }
                else if (hv_N == 1)
                {
                    listObj2Draw[1] = "NG-电极尺寸";
                    //绘制电极轮廓  ho_DianjiOpen
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    strMessage = DebugPrint(strDebug, is_Debug);
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionL, 1);
                HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionR, 2);
                HOperatorSet.Union1(ho_DianjiOpen, out ho_RegionLR);
                HOperatorSet.ShapeTrans(ho_RegionLR, out ho_RegionTrans, "convex");

                //*二、检查尺寸
                //*产品矩形度，大于0.9
                HOperatorSet.Rectangularity(ho_RegionTrans, out hv_Recty);  //参数16：产品矩形度最小值 iPuRecty

                if (is_Debug)  //调试状态输出信息
                {
                    strDebug += "(4)整体矩形度相关参数:\n";
                    strDebug += "设定矩形度:" + iPuRecty.ToString() + "\n";
                    strDebug += "当前整体矩形度:" + hv_Recty.D.ToString("0.00") + "\n";
                    strDebug += "\n";
                }

                if (A_产品尺寸 == 0) goto A_产品尺寸END1;
                if ((int)(new HTuple(hv_Recty.TupleLess(iPuRecty))) != 0)
                {
                    listObj2Draw[1] = "NG-产品尺寸";
                    //绘制产品轮廓  
                    syShowRegionBorder(ho_RegionTrans, ref listObj2Draw, "NG");

                    //输出NG详情
                    lsInfo2Draw.Add("当前矩形度:" + hv_Recty.D.ToString("0.0"));
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    strMessage = DebugPrint(strDebug, is_Debug);
                    return listObj2Draw;
                }
                A_产品尺寸END1:

                //*检查产品尺寸    165 * 80
                HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Rowppp, out hv_Colppp, out hv_Phippp, out hv_Length1ppp, out hv_Length2ppp);
                HOperatorSet.GenRectangle2(out ho_Rectangleppp, hv_Rowppp, hv_Colppp, hv_Phippp, hv_Length1ppp, hv_Length2ppp);

                //0508更改，判断产品角度，歪斜过大直接无定位 正负15度
                HTuple Deg;
                HOperatorSet.TupleDeg(hv_Phippp, out Deg);

                if (is_Debug)  //调试状态输出信息
                {
                    strDebug += "(5)整体无定位歪斜参数：\n";
                    strDebug += "设定歪斜角度:" + iWaiAngle.ToString() + "\n";
                    strDebug += "当前歪斜角度:" + Deg.D.ToString("0.0") + "\n";
                    strDebug += "\n";
                }

                if (Deg > iWaiAngle || Deg < (0 - iWaiAngle))    //参数17：无定位角度  iWaiAngle = 15
                {
                    listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                    //输出NG详情
                    lsInfo2Draw.Add("当前角度:" + Deg.D.ToString("0.0") + "度");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    strMessage = DebugPrint(strDebug, is_Debug);
                    return listObj2Draw;
                }

                if (iOnlyWuDingwei != 1)
                {
                    #region     尺寸矩形度检查

                    if (is_Debug)  //调试状态输出信息
                    {
                        strDebug += "(6)整体尺寸参数:\n";

                        strDebug += "长度值:" + iPuLength1.ToString() + "\n";
                        strDebug += "长度变化值:" + iPuLength1Scale.ToString() + "\n";
                        strDebug += "长度上限值:" + (iPuLength1 + iPuLength1Scale).ToString() + "\n";
                        strDebug += "长度下限值:" + (iPuLength1 - iPuLength1Scale).ToString() + "\n";
                        strDebug += "宽度值:" + iPuLength2.ToString() + "\n";
                        strDebug += "宽度变化值:" + iPuLength2Scale.ToString() + "\n";
                        strDebug += "宽度上限值:" + (iPuLength2 + iPuLength2Scale).ToString() + "\n";
                        strDebug += "宽度下限值:" + (iPuLength2 - iPuLength2Scale).ToString() + "\n";

                        strDebug += "当前长度:" + hv_Length1ppp.D.ToString() + "\n";
                        strDebug += "当前宽度:" + hv_Length2ppp.D.ToString() + "\n";
                        strDebug += "\n";
                    }



                    //绘制设定大小产品的尺寸框  hv_Rowppp, hv_Colppp, hv_Phippp, iPuLength1,iPuLength2
                    if (is_Debug)
                    {
                        //HTuple lll1, lll2;
                        //lll1 = iPuLength1; lll2 = iPuLength2;
                        //List<PointF> lnBarcodeeee = dhFindVerticesOfRectangle2(hv_Rowppp, hv_Colppp, hv_Phippp, lll1, lll2);
                        //listObj2Draw.Add("多边形");
                        //listObj2Draw.Add(lnBarcodeeee.ToArray());
                        //listObj2Draw.Add("OK");
                        HObject ho_RectangleDebug;
                        HOperatorSet.GenRectangle2(out ho_RectangleDebug, hv_Rowppp, hv_Colppp, hv_Phippp, iPuLength1, iPuLength2);
                        syShowRegionBorder(ho_RectangleDebug, ref listObj2Draw, "OK");
                    }

                    if (A_产品尺寸 == 0) goto A_产品尺寸END2;
                    //产品尺寸 
                    if ((int)((new HTuple(hv_Length1ppp.TupleLess(iPuLength1 - iPuLength1Scale))).TupleOr(new HTuple(hv_Length1ppp.TupleGreater(
                        iPuLength1 + iPuLength1Scale)))) != 0)  //参数18：产品半长     iPuLength1 
                                                                //参数19：产品半长变化 iPuLength1Scale
                    {
                        //绘制外接矩形
                        listObj2Draw[1] = "NG-产品尺寸";
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Rowppp, hv_Colppp, hv_Phippp, hv_Length1ppp, hv_Length2ppp);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1ppp.D.ToString("0.0") + "* " + hv_Length2ppp.D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("标准尺寸:" + iPuLength1.ToString("0.0") + "* " + iPuLength2.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    if ((int)((new HTuple(hv_Length2ppp.TupleLess(iPuLength2 - iPuLength2Scale))).TupleOr(new HTuple(hv_Length2ppp.TupleGreater(
                        iPuLength2 + iPuLength2Scale)))) != 0)   //参数20：产品半宽     iPuLength2
                                                                 //参数21：产品半宽变化 iPuLength2Scale
                    {
                        //绘制外接矩形
                        listObj2Draw[1] = "NG-产品尺寸";
                        List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Rowppp, hv_Colppp, hv_Phippp, hv_Length1ppp, hv_Length2ppp);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode.ToArray());
                        listObj2Draw.Add("NG");

                        //输出NG详情
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1ppp.D.ToString("0.0") + "* " + hv_Length2ppp.D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("标准尺寸:" + iPuLength1.ToString("0.0") + "* " + iPuLength2.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_产品尺寸END2:

                    //*电极矩形度，大于0.85
                    HOperatorSet.Rectangularity(ho_DianjiOpen, out hv_Recties);  //参数22：电极矩形度最小值 iElectrodeRecty

                    if (is_Debug)  //调试状态输出信息
                    {
                        strDebug += "(7)电极矩形度参数:\n";
                        strDebug += "设定最小矩形度:" + iElectrodeRecty.ToString() + "\n";
                        strDebug += "当前电极1矩形度:" + hv_Recties.TupleSelect(0).D.ToString("0.00") + "\n";
                        strDebug += "当前电极2矩形度:" + hv_Recties.TupleSelect(1).D.ToString("0.00") + "\n";
                        strDebug += "\n";
                    }

                    if (A_电极尺寸 == 0) goto A_电极尺寸END1;
                    if ((int)((new HTuple(((hv_Recties.TupleSelect(0))).TupleLess(iElectrodeRecty))).TupleOr(
                        new HTuple(((hv_Recties.TupleSelect(1))).TupleLess(iElectrodeRecty)))) != 0)
                    {
                        listObj2Draw[1] = "NG-电极尺寸";
                        //绘制电极轮廓  
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("电极矩形度:" + hv_Recties.TupleSelect(0).D.ToString("0.00") + "-" + hv_Recties.TupleSelect(1).D.ToString("0.00"));
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极尺寸END1:

                    //*检查电极尺寸
                    HOperatorSet.SmallestRectangle2(ho_DianjiOpen, out hv_Rowqqq, out hv_Colqqq,
                        out hv_Phiqqq, out hv_Length1qqq, out hv_Length2qqq);
                    HOperatorSet.GenRectangle2(out ho_Rectangleqqq, hv_Rowqqq, hv_Colqqq, hv_Phiqqq,
                        hv_Length1qqq, hv_Length2qqq);

                    if (is_Debug)  //调试状态输出信息
                    {
                        strDebug += "(8)电极尺寸参数:\n";

                        strDebug += "长度设定值:" + iElecLength1.ToString() + "\n";
                        strDebug += "长度设定变化值:" + iElecLength1Scale.ToString() + "\n";
                        strDebug += "长度设定上限值:" + (iElecLength1 + iElecLength1Scale).ToString() + "\n";
                        strDebug += "长度设定下限值:" + (iElecLength1 - iElecLength1Scale).ToString() + "\n";

                        strDebug += "宽度设定值:" + iElecLength2.ToString() + "\n";
                        strDebug += "宽度设定变化值:" + iElecLength2Scale.ToString() + "\n";
                        strDebug += "宽度设定上限值:" + (iElecLength2 + iElecLength2Scale).ToString() + "\n";
                        strDebug += "宽度设定下限值:" + (iElecLength2 - iElecLength2Scale).ToString() + "\n";

                        strDebug += "当前电极1长度:" + hv_Length1qqq.TupleSelect(0).D.ToString() + "\n";
                        strDebug += "当前电极1宽度:" + hv_Length2qqq.TupleSelect(0).D.ToString() + "\n";

                        strDebug += "当前电极2长度:" + hv_Length1qqq.TupleSelect(1).D.ToString() + "\n";
                        strDebug += "当前电极2宽度:" + hv_Length2qqq.TupleSelect(1).D.ToString() + "\n";
                        strDebug += "\n";
                    }




                    //绘制两个设定大小电极框
                    if (is_Debug)
                    {
                        HObject ho_RectangleDebug;
                        HOperatorSet.GenRectangle2(out ho_RectangleDebug, hv_Rowqqq.TupleSelect(0), hv_Colqqq.TupleSelect(0), hv_Phiqqq.TupleSelect(0), iElecLength1, iElecLength2);
                        syShowRegionBorder(ho_RectangleDebug, ref listObj2Draw, "OK");
                        HOperatorSet.GenRectangle2(out ho_RectangleDebug, hv_Rowqqq.TupleSelect(1), hv_Colqqq.TupleSelect(1), hv_Phiqqq.TupleSelect(1), iElecLength1, iElecLength2);
                        syShowRegionBorder(ho_RectangleDebug, ref listObj2Draw, "OK");
                    }



                    if (A_电极尺寸 == 0) goto A_电极尺寸END2;
                    if ((int)((new HTuple((new HTuple((new HTuple(((hv_Length1qqq.TupleSelect(0))).TupleLess(
                        iElecLength1 - iElecLength1Scale))).TupleOr(new HTuple(((hv_Length1qqq.TupleSelect(0))).TupleGreater(
                        iElecLength1 + iElecLength1Scale))))).TupleOr(new HTuple(((hv_Length1qqq.TupleSelect(1))).TupleLess(
                        iElecLength1 - iElecLength1Scale))))).TupleOr(new HTuple(((hv_Length1qqq.TupleSelect(1))).TupleGreater(
                        iElecLength1 + iElecLength1Scale)))) != 0) //参数23： 电极半长       iElecLength1
                                                                   //参数24： 电极半长变化值 iElecLength1Scale
                    {
                        listObj2Draw[1] = "NG-电极尺寸";
                        //绘制电极轮廓  
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1qqq.TupleSelect(0).D.ToString("0.0") + "* " + hv_Length2qqq.TupleSelect(0).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("标准尺寸:" + iElecLength1.ToString("0.0") + "* " + iElecLength2.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    if ((int)((new HTuple((new HTuple((new HTuple(((hv_Length2qqq.TupleSelect(0))).TupleLess(
                        iElecLength2 - iElecLength2Scale))).TupleOr(new HTuple(((hv_Length2qqq.TupleSelect(0))).TupleGreater(
                        iElecLength2 + iElecLength2Scale))))).TupleOr(new HTuple(((hv_Length2qqq.TupleSelect(1))).TupleLess(
                        iElecLength2 - iElecLength2Scale))))).TupleOr(new HTuple(((hv_Length2qqq.TupleSelect(1))).TupleGreater(
                        iElecLength2 + iElecLength2Scale)))) != 0)   //参数25： 电极半宽       iElecLength2
                                                                     //参数26： 电极半宽变化值 iElecLength2Scale
                    {
                        listObj2Draw[1] = "NG-电极尺寸";
                        //绘制电极轮廓  
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1qqq.TupleSelect(1).D.ToString("0.0") + "* " + hv_Length2qqq.TupleSelect(1).D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("标准尺寸:" + iElecLength1.ToString("0.0") + "* " + iElecLength2.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极尺寸END2:

#if true
                    //判断两个电极角度差 hv_Phiqqq.TupleSelect(0) ，hv_Colqqq.TupleSelect(1)
                    int SetAngleDiff = 5;
                    HTuple A, B;
                    HOperatorSet.TupleDeg(hv_Phiqqq.TupleSelect(0), out A);
                    HOperatorSet.TupleDeg(hv_Phiqqq.TupleSelect(1), out B);
                    int DaianjiAngleDiff = (int)(A.D - B.D);
                    int absangle = Math.Abs(DaianjiAngleDiff);
                    if (absangle > 90) absangle = 180 - absangle;

                    if (is_Debug)  //调试状态输出信息
                    {
                        strDebug += "(8)电极角度参数:\n";
                        strDebug += "当前角度差值:" + absangle.ToString("0.0") + "\n";
                        strDebug += "设定角度差值:" + SetAngleDiff.ToString("0.0") + "\n";
                    }

                    if (A_电极尺寸 == 0) goto A_电极尺寸END3;
                    if (absangle > SetAngleDiff)
                    {
                        listObj2Draw[1] = "NG-电极尺寸";
                        //绘制电极轮廓  
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("当前电极角度差:" + absangle.ToString("0.0") + "度");
                        lsInfo2Draw.Add("设定电极角度差:" + SetAngleDiff.ToString("0.0") + "度");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极尺寸END3:

#endif

#if true

                    //检查锡瘤导致的电极收缩  ho_DianjiOpen  ho_Rectangleqqq
                    HObject ho_DianjiOpenAAA, ho_RectangleqqqAAA, DiffAAA, UnionDianjiTransCnvex, DiffOpenAAA, DianjiTransCnvex, DiffCnnAAA, DiffSelectAAA;
                    HTuple ShouSuoNum;
                    HOperatorSet.ShapeTrans(ho_DianjiOpen, out DianjiTransCnvex, "convex");
                    HOperatorSet.Union1(DianjiTransCnvex, out UnionDianjiTransCnvex);
                    HOperatorSet.Union1(ho_DianjiOpen, out ho_DianjiOpenAAA);
                    HOperatorSet.Difference(UnionDianjiTransCnvex, ho_DianjiOpenAAA, out DiffAAA);
                    HOperatorSet.OpeningCircle(DiffAAA, out DiffOpenAAA, 5);  //收缩开运算 5
                    HOperatorSet.Connection(DiffOpenAAA, out DiffCnnAAA);
                    if (iDianjiShousuoRadis == 0) iDianjiShousuoRadis = 5;
                    HOperatorSet.SelectShape(DiffCnnAAA, out DiffSelectAAA, "inner_radius", "and", iDianjiShousuoRadis, 9999999); //电极收缩半径5
                    HOperatorSet.CountObj(DiffSelectAAA, out ShouSuoNum);
                    if (is_Debug)
                    {
                        strDebug += "(9)电极收缩参数:\n";
                        strDebug += "设定电极收缩半径:" + iDianjiShousuoRadis.ToString() + "\n";
                    }

                    if (A_电极尺寸 == 0) goto A_电极尺寸END4;
                    if (ShouSuoNum > 0)
                    {
                        listObj2Draw[1] = "NG-电极尺寸";
                        //绘制电极轮廓  
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("设定电极锡瘤收缩半径:" + iDianjiShousuoRadis.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极尺寸END4:



#endif







                    #endregion




                    #region       检查基板区域RegionBase电极上爬 ImageBase
                    HObject Mean, RegionSP3, RegionSP2, ho_Diff, DynCnn, ho_DiffOpen, Dyn, DiffCnn, RegionBase, ImageBase, RegionSP1;
                    HTuple L, Num, Row11, Col11, Row22, Col22, SPWide, R1, C1, R2, C2, R11, C11, R22, C22;
                    HOperatorSet.Difference(ho_RegionTrans, ho_RegionLR, out ho_Diff);
                    HOperatorSet.OpeningRectangle1(ho_Diff, out ho_DiffOpen, 4, 4);
                    HOperatorSet.Connection(ho_DiffOpen, out DiffCnn);
                    HOperatorSet.SelectShapeStd(DiffCnn, out RegionBase, "max_area", 70);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, RegionBase, out ImageBase);
                    HOperatorSet.MeanImage(ImageBase, out Mean, iElectSPMeanSize, iElectSPMeanSize);             //参数27：电极上爬掩膜大小 iElectSPMeanSize = 20
                    HOperatorSet.DynThreshold(ImageBase, Mean, out Dyn, iElectSPDynThres, "light");              //参数28：电极上爬动态阈值 iElectSPDynThres = 10
                    HOperatorSet.Connection(Dyn, out DynCnn);
                    HOperatorSet.SelectGray(DynCnn, ImageBase, out RegionSP1, "mean", "and", iElectSPFilterThres, 255); //参数29：电极上爬过滤灰度值 iElectSPFilterThres = 120
                    HOperatorSet.SelectShape(RegionSP1, out RegionSP2, "area", "and", iElectSPFilterArea, 9999999);   //参数30：电极上爬过滤面积 iElectSPFilterArea = 250
                    HOperatorSet.CountObj(RegionSP2, out Num);

                    //检查上爬，上爬宽度可设置
                    HOperatorSet.Union1(RegionSP2, out RegionSP3);
                    HOperatorSet.SmallestRectangle1(RegionSP3, out Row11, out Col11, out Row22, out Col22);
                    SPWide = Col22 - Col11; //上爬宽度
                    //计算L
                    HOperatorSet.SmallestRectangle1(ho_RegionTrans, out R1, out C1, out R2, out C2);
                    HOperatorSet.SmallestRectangle1(ho_DianjiOpen, out R11, out C11, out R22, out C22);
                    L = C2 - C1 - (C22.TupleSelect(0) - C11.TupleSelect(0)) - (C22.TupleSelect(1) - C11.TupleSelect(1));






                    if (is_Debug)
                    {
                        strDebug += "(9)电极爬镀相关参数:\n";
                        strDebug += "掩膜大小:" + iElectSPMeanSize.ToString() + "\n";
                        strDebug += "动态阈值:" + iElectSPDynThres.ToString() + "\n";
                        strDebug += "过滤灰度值:" + iElectSPFilterThres.ToString() + "\n";
                        strDebug += "过滤面积:" + iElectSPFilterArea.ToString() + "\n";
                        strDebug += "上爬系数:" + iElectSPWideSacle.ToString() + "\n";

                        strDebug += "当前计算L:" + L.D.ToString("0.0") + "\n";
                        strDebug += "当前允许爬镀宽度:" + (iElectSPWideSacle * L).D.ToString("0.0") + "\n";

                        //SPWide 可能有多个
                        for (int kkk = 0; kkk < SPWide.TupleLength(); kkk++)
                        {
                            strDebug += "当前爬镀宽度:" + SPWide.TupleSelect(kkk).D.ToString("0.0") + "\n";
                        }
                        strDebug += "\n";
                    }

                    if (A_电极爬镀 == 0) goto A_电极爬镀END;
                    if (SPWide > (iElectSPWideSacle * L))            //参数31 ：电极上爬宽度相对瓷体宽度iElectSPWideSacle = 0.2           0.2倍L
                    {
                        listObj2Draw[1] = "NG-电极爬镀";
                        //绘制电极轮廓  RegionSP2
                        hv_Num = 0;
                        HObject RegionSP2Cnnnnn;
                        HOperatorSet.Connection(RegionSP2, out RegionSP2Cnnnnn);
                        HOperatorSet.CountObj(RegionSP2Cnnnnn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(RegionSP2Cnnnnn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            lsInfo2Draw.Add("当前爬镀宽度: " + SPWide.D.ToString("0.0") + " pix" + " " + "L: " + L.D.ToString("0.0") + " pix");
                            //lsInfo2Draw.Add("L: " + L.D.ToString("0.0") + " pix");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极爬镀END:


                    //检查砸伤瓷体
                    HObject ImageCiTiZS, ZSMean, RegionDynThresh3, DynCnn3, DynZS, RegionCiTi;

                    if (iCode == 1)
                        HOperatorSet.ErosionRectangle1(RegionBase, out RegionCiTi, iCiTiEroWidth, iCiTiEroHeight);   //R代码 //参数32：瓷体定位腐蚀长度 iCiTiEroWidth  =  40                                                                           //参数33：瓷体定位腐蚀宽度 iCiTiEroHeight =  50 / 60
                    else
                        HOperatorSet.ErosionRectangle1(RegionBase, out RegionCiTi, iCiTiEroWidth, iCiTiEroHeight);   //B代码

                    HObject ImageCiTiZSCrop, RegionCiTiTrans;

                    HOperatorSet.GenEmptyObj(out RegionCiTiTrans);
                    if (false)  //原始算法
                    {
                        HOperatorSet.ReduceDomain(ho_ImageReduced, RegionCiTi, out ImageCiTiZS);

                        // HOperatorSet.MeanImage(ImageCiTiZS, out ZSMean, iZSMask, iZSMask); //25         
                        HOperatorSet.MeanImage(ImageCiTiZS, out ZSMean, iZSMaskWidth, iZSMaskHeight); //25    //参数34 ： 瓷体缺陷掩膜大小 iZSMask 

                        HOperatorSet.DynThreshold(ImageCiTiZS, ZSMean, out RegionDynThresh3, iZSThres, "not_equal"); //参数35 ：iZSThres  = 5    白缺陷动态阈值5
                        HOperatorSet.Connection(RegionDynThresh3, out DynCnn3);
                        HOperatorSet.SelectShape(DynCnn3, out DynZS, "area", "and", iZSArea, 999999);                //参数36 ：iZSArea        白缺陷面积300
                        HOperatorSet.CountObj(DynZS, out Num);
                    }
                    else       //Crop算法
                    {
                        //HObject ImageCiTiZSCrop, RegionCiTiTrans;

                        HOperatorSet.ShapeTrans(RegionCiTi, out RegionCiTiTrans, "inner_rectangle1");

                        HOperatorSet.DilationRectangle1(RegionCiTiTrans, out RegionCiTiTrans, 1, 12);

                        //MessageBox.Show("iRegionCiTiWidthScale = " + iRegionCiTiWidthScale.ToString() + "iRegionCiTiHighScale = " + iRegionCiTiHighScale.ToString());


                        //瓷体检测区域变宽、变高 iRegionCiTiWidthScale iRegionCiTiHighScale
                        if (iRegionCiTiWidthScale == 0)     //不变
                        {
                            HOperatorSet.ErosionRectangle1(RegionCiTiTrans, out RegionCiTiTrans, 1, 1);
                        }
                        else if (iRegionCiTiWidthScale > 0) //变宽
                        {
                            HOperatorSet.DilationRectangle1(RegionCiTiTrans, out RegionCiTiTrans, iRegionCiTiWidthScale, 1);
                        }
                        else                                // < 0 变窄
                        {
                            HOperatorSet.ErosionRectangle1(RegionCiTiTrans, out RegionCiTiTrans, (0 - iRegionCiTiWidthScale), 1);
                        }


                        if (iRegionCiTiHighScale == 0)    //不变
                        {
                            HOperatorSet.ErosionRectangle1(RegionCiTiTrans, out RegionCiTiTrans, 1, 1);
                        }
                        else if (iRegionCiTiHighScale > 0) //变高
                        {
                            HOperatorSet.DilationRectangle1(RegionCiTiTrans, out RegionCiTiTrans, 1, iRegionCiTiHighScale);
                        }
                        else // < 0 变矮
                        {
                            HOperatorSet.ErosionRectangle1(RegionCiTiTrans, out RegionCiTiTrans, 1, (0 - iRegionCiTiHighScale));
                        }




                        HOperatorSet.ReduceDomain(ho_ImageReduced, RegionCiTiTrans, out ImageCiTiZS);
                        HOperatorSet.CropDomain(ImageCiTiZS, out ImageCiTiZSCrop);




                        HOperatorSet.MeanImage(ImageCiTiZSCrop, out ZSMean, iZSMaskWidth, iZSMaskHeight); //25    //参数34 ： 瓷体缺陷掩膜大小 iZSMask 




                        HOperatorSet.DynThreshold(ImageCiTiZSCrop, ZSMean, out RegionDynThresh3, iZSThres, "not_equal"); //参数35 ：iZSThres  = 5    白缺陷动态阈值5

                        //把Crop后的缺陷区域移动到原来区域
                        HTuple Arearrr, Rowrrr, Colrrr, Areaggg, Rowggg, Colggg;
                        HObject RegionCrop, RegionDynThresh3Move;
                        HOperatorSet.AreaCenter(RegionCiTiTrans, out Arearrr, out Rowrrr, out Colrrr);
                        HOperatorSet.GetDomain(ImageCiTiZSCrop, out RegionCrop);
                        HOperatorSet.AreaCenter(RegionCrop, out Areaggg, out Rowggg, out Colggg);
                        HOperatorSet.MoveRegion(RegionDynThresh3, out RegionDynThresh3Move, Rowrrr - Rowggg, Colrrr - Colggg);

                        HOperatorSet.Connection(RegionDynThresh3Move, out DynCnn3);
                        HOperatorSet.SelectShape(DynCnn3, out DynZS, "area", "and", iZSArea, 999999);                   //参数36 ：iZSArea        白缺陷面积300
                        HOperatorSet.CountObj(DynZS, out Num);
                    }








                    //绘制瓷体检查区域
                    if (is_Debug)
                    {
                        syShowRegionBorder(RegionCiTiTrans, ref listObj2Draw, "OK");
                    }

                    if (is_Debug)
                    {
                        strDebug += "(10)瓷体损伤相关参数:\n";
                        strDebug += "掩膜大小:" + iZSMaskWidth.ToString() + "+" + iZSMaskHeight.ToString() + "\n";
                        strDebug += "动态阈值:" + iZSThres.ToString() + "\n";
                        strDebug += "设定面积:" + iZSArea.ToString() + "\n";
                        HTuple Rowtmp, Coltmp, Areatmp;
                        HOperatorSet.AreaCenter(DynCnn3, out Areatmp, out Rowtmp, out Coltmp);
                        if (Areatmp.TupleLength() > 0)
                        {
                            HObject DynCnn3Union;
                            HOperatorSet.Union1(DynCnn3, out DynCnn3Union);
                            syShowRegionBorder(DynCnn3Union, ref listObj2Draw, "OK"); //绘制当前瓷体缺陷区域：DynCnn3

                            strDebug += "当前缺陷面积:\n";
                            for (int fff = 0; fff < Areatmp.TupleLength(); fff++)
                                strDebug += Areatmp.TupleSelect(fff).D.ToString("0.0") + "\n";
                        }
                        strDebug += "\n";
                    }

                    if (A_瓷体损伤 == 0) { goto A_瓷体损伤_END1; }
                    if (Num != 0)  //瓷体砸伤
                    {
                        listObj2Draw[1] = "NG-瓷体损伤";
                        //绘制电极轮廓  RegionSP2
                        hv_Num = 0;
                        HOperatorSet.CountObj(DynZS, out hv_Num);

                        HTuple AAA, RRR, CCC;
                        AAA = 0;
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(DynZS, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");

                            //输出NG详情
                            HOperatorSet.AreaCenter(ho_RegionSel, out AAA, out RRR, out CCC);

                            lsInfo2Draw.Add("缺陷面积上限: " + iZSArea.ToString("0.0") + " pix" + "当前缺陷面积: " + AAA.D.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strDebug += "当前缺陷面积:" + AAA.D.ToString("0.0") + "\n";

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_瓷体损伤_END1:


                    //瓷体区域 RegionCiTiTrans sobel检查纹理,检查瓷体凹坑
                    int iSobelCT = int.Parse(strUserParam[58]); //5
                    int iThresSobelCT = int.Parse(strUserParam[59]); //10
                    int iSobelCTArea = int.Parse(strUserParam[60]); //200

                    HObject ImageCTSobel, RegionampLightCnn, AmpCT, SelectedRegions8, RegionampLight;

                    //方式1: 直接sobel
                    HOperatorSet.ReduceDomain(ho_ImageReduced, RegionCiTiTrans, out ImageCTSobel);

                    ////方式2：先scale再mean
                    //HObject RegionCiTiTransDia, Imageggggg, Scaleggg, ScalegggMean;
                    //HOperatorSet.DilationCircle(RegionCiTiTrans, out RegionCiTiTransDia,12);
                    //HOperatorSet.ReduceDomain(ho_ImageReduced, RegionCiTiTransDia, out Imageggggg);
                    //HOperatorSet.ScaleImage(Imageggggg,out Scaleggg,5,0);
                    //HOperatorSet.MeanImage(Scaleggg,out  ScalegggMean,10,10);
                    //HOperatorSet.ReduceDomain(ScalegggMean, RegionCiTiTrans, out ImageCTSobel);
                    //iThresSobelCT = 15;


                    HOperatorSet.SobelAmp(ImageCTSobel, out AmpCT, "sum_abs", iSobelCT); //梯度5
                    HOperatorSet.Threshold(AmpCT, out RegionampLight, iThresSobelCT, 255);   //阈值   10

                    //   HOperatorSet.ClosingCircle(RegionampLight, out RegionampLight,1);        //闭运算 1

                    HOperatorSet.Connection(RegionampLight, out RegionampLightCnn);
                    HOperatorSet.SelectShape(RegionampLightCnn, out SelectedRegions8, "area", "and", iSobelCTArea, 99999);  //sobel瓷损面积200
                    HOperatorSet.CountObj(SelectedRegions8, out Num);

                    if (A_瓷体损伤 == 0) { goto A_瓷体损伤_END2; }
                    if (Num != 0)  //瓷体砸伤sobel
                    {
                        //绘制电极轮廓  RegionSP2
                        listObj2Draw[1] = "NG-瓷体损伤";
                        hv_Num = 0;
                        HOperatorSet.CountObj(SelectedRegions8, out hv_Num);

                        HTuple AAA = 0, RRR, CCC;

                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(SelectedRegions8, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            HOperatorSet.AreaCenter(ho_RegionSel, out AAA, out RRR, out CCC);
                            lsInfo2Draw.Add("sobel缺陷面积上限: " + iSobelCTArea.ToString("0.0") + " pix" + "当前缺陷面积: " + AAA.D.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("缺陷阈值上限: " + iThresSobelCT.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strDebug += "sobel缺陷面积上限:" + iSobelCTArea.ToString("0.0") + "\n";
                        strDebug += "当前缺陷面积:" + AAA.D.ToString("0.0") + "\n";

                        strMessage = DebugPrint(strDebug, is_Debug);
                        return listObj2Draw;
                    }
                    A_瓷体损伤_END2:





                    //检查瓷体黑缺陷（瓷损漏电极）
                    //int iCSLDJThres = 5; //瓷损阈值            //参数37：瓷损漏电极发黑阈值  iCSLDJThres = 5
                    //int iCSLDJArea = 100;  //瓷损面积          //参数38：瓷损漏电极发黑面积  iCSLDJArea  = 100
                    HObject ImageCiTi, RegionBlackCT, RegionBlackCTCnn, SelectedRegions1, RegionWhiteCT;
                    HOperatorSet.ReduceDomain(ho_ImageReduced, RegionCiTi, out ImageCiTi);

                    HOperatorSet.GenEmptyObj(out RegionBlackCT);
                    HOperatorSet.GenEmptyObj(out RegionWhiteCT);

                    HOperatorSet.Threshold(ImageCiTi, out RegionBlackCT, 0, iCSLDJThres);                //瓷损黑阈值
                    HOperatorSet.Threshold(ImageCiTi, out RegionWhiteCT, iCurrentMean + 60, 255);          //瓷损白阈值,输入的平均值+60

                    HOperatorSet.ConcatObj(RegionWhiteCT, RegionBlackCT, out RegionBlackCT);             //合并黑白缺陷区域

                    HOperatorSet.Connection(RegionBlackCT, out RegionBlackCTCnn);
                    HOperatorSet.SelectShape(RegionBlackCTCnn, out SelectedRegions1, "area", "and", iCSLDJArea, 99999);  //瓷损面积
                    HOperatorSet.CountObj(SelectedRegions1, out Num);

                    if (A_瓷损漏电极 == 0) goto A_瓷损漏电极END;
                    if (Num != 0)  //瓷体砸伤
                    {
                        listObj2Draw[1] = "NG-瓷损露电极";
                        //绘制电极轮廓  RegionSP2
                        hv_Num = 0;
                        HOperatorSet.CountObj(SelectedRegions1, out hv_Num);
                        HTuple AAA, RRR, CCC;
                        AAA = 0;
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(SelectedRegions1, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            HOperatorSet.AreaCenter(ho_RegionSel, out AAA, out RRR, out CCC);
                            lsInfo2Draw.Add("缺陷面积上限: " + iCSLDJArea.ToString("0.0") + " pix" + "当前缺陷面积: " + AAA.D.ToString("0.0"));
                            lsInfo2Draw.Add("缺陷阈值上限: " + iCSLDJThres.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strDebug += "瓷体黑缺陷面积上限:" + iCSLDJArea.ToString("0.0") + "\n";
                        strDebug += "当前缺陷面积:" + AAA.D.ToString("0.0") + "\n";

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_瓷损漏电极END:


#if false
                    //检查凹坑  hoReduced   
                    HObject expImage, ImageScalemax, GrayImageggg, Regionbbb, RegionAok, RegionAokCnn;
                    HOperatorSet.ExpImage(hoReduced, out expImage, iAokExp);                                             //exp参数
                    HOperatorSet.ScaleImageMax(expImage,out ImageScalemax);
                    HOperatorSet.Rgb1ToGray(ImageScalemax, out GrayImageggg);
                    HOperatorSet.Threshold(GrayImageggg, out Regionbbb,10,255);
                    HOperatorSet.Intersection(Regionbbb, RegionCiTi,out RegionAok);
                    HOperatorSet.Connection(RegionAok, out RegionAokCnn);
                    HOperatorSet.SelectShape(RegionAokCnn, out SelectedRegions1, "area", "and", iZSArea, 99999);  //瓷损面积
                    HOperatorSet.CountObj(SelectedRegions1, out Num);
                    if (Num != 0)  //瓷体砸伤
                    {
                        listObj2Draw[1] = "NG-瓷体损伤";
                        //绘制电极轮廓  RegionSP2
                        hv_Num = 0;
                        HOperatorSet.CountObj(SelectedRegions1, out hv_Num);

                        HTuple AAA, RRR, CCC;
                        AAA = 0;
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(SelectedRegions1, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");

                            //输出NG详情
                            HOperatorSet.AreaCenter(ho_RegionSel, out AAA, out RRR, out CCC);

                            lsInfo2Draw.Add("凹坑面积上限: " + iZSArea.ToString("0.0") + " pix" + "当前缺陷面积: " + AAA.D.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strDebug += "当前缺陷面积:" + AAA.D.ToString("0.0") + "\n";

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
#endif













                    //检查瓷体颜色不一致
                    HTuple MeanCiTi, DevCiTi;
                    HOperatorSet.Intensity(RegionCiTi, ImageCiTi, out MeanCiTi, out DevCiTi);

                    if (is_Debug)
                    {
                        strDebug += "(11)瓷体颜色相关参数:\n";
                        strDebug += "平均灰度:" + iCurrentMean.ToString() + "\n";
                        strDebug += "灰度变化值:" + iCurrentMeanScale.ToString() + "\n";
                        strDebug += "灰度上限:" + (iCurrentMeanScale + iCurrentMean).ToString() + "\n";
                        strDebug += "灰度下限:" + (iCurrentMeanScale - iCurrentMean).ToString() + "\n";
                        strDebug += "当前灰度:" + MeanCiTi.D.ToString("0.0") + "\n";
                        strDebug += "\n";
                    }


                    if (A_瓷体颜色不一致 == 0) goto A_瓷体颜色不一致END;
                    if ((MeanCiTi > (iCurrentMean + iCurrentMeanScale)) || (MeanCiTi < (iCurrentMean - iCurrentMeanScale)))  //参数39：瓷体颜色灰度 iCurrentMean
                                                                                                                             //参数40：瓷体颜色变化 iCurrentMeanScale
                    {
                        listObj2Draw[1] = "NG-瓷体颜色不一致";
                        //绘制电极轮廓  RegionSP2
                        hv_Num = 0;
                        HOperatorSet.CountObj(RegionCiTi, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(RegionCiTi, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            lsInfo2Draw.Add("设定灰度: " + iCurrentMean.ToString("0.0") + "当前灰度: " + MeanCiTi.D.ToString("0.0"));
                            //lsInfo2Draw.Add("当前灰度: " + MeanCiTi.D.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_瓷体颜色不一致END:

#if true
                    //检查棱边瓷损
                    HObject LBBigAffine, CiTiDila1, StructLB1, CiTiEro1, RegionLB, LBOpening, ImageLB, LBMean, RegionDynThresh2, DynCnnLB, LBBig;
                    HTuple HomMat2D, R1111, C1111, R2222, C2222, LBHigh, PhiPPP, L1111111111, L2222222222;
                    //   int iLBHigh = 20; //瓷损高度20            //参数41 ： 棱边瓷损高度设定值 iLBHigh = 20

                    HOperatorSet.GenRectangle2(out StructLB1, 100, 100, PhiBin, 1, iLBStructLength2); //参数42 ： 棱边定位矩形半宽 iLBStructLength2 = 25
                    HOperatorSet.Dilation1(RegionCiTi, StructLB1, out CiTiDila1, 1);
                    HOperatorSet.Erosion1(RegionCiTi, StructLB1, out CiTiEro1, 1);



                    HOperatorSet.Difference(CiTiDila1, CiTiEro1, out RegionLB);
                    HOperatorSet.OpeningCircle(RegionLB, out LBOpening, 10);




                    HObject LBCnn, LBOpening1, LBOpening2;
                    HOperatorSet.Connection(LBOpening, out LBCnn);




                    HOperatorSet.SelectObj(LBCnn, out LBOpening1, 1);
                    HOperatorSet.SelectObj(LBCnn, out LBOpening2, 2);

                    //棱边瓷损检测区域宽度变化
                    //int iLBWidthScale = 12;
                    if (iLBWidthScale == 0)    //不变
                    {
                        HOperatorSet.ErosionRectangle1(LBOpening1, out LBOpening1, 1, 1);
                        HOperatorSet.ErosionRectangle1(LBOpening2, out LBOpening2, 1, 1);
                    }
                    else if (iLBWidthScale > 0) //变宽
                    {
                        HOperatorSet.DilationRectangle1(LBOpening1, out LBOpening1, iLBWidthScale, 1);
                        HOperatorSet.DilationRectangle1(LBOpening2, out LBOpening2, iLBWidthScale, 1);
                    }
                    else // < 0 变窄
                    {

                        HOperatorSet.ErosionRectangle1(LBOpening1, out LBOpening1, (0 - iLBWidthScale), 1);
                        HOperatorSet.ErosionRectangle1(LBOpening2, out LBOpening2, (0 - iLBWidthScale), 1);
                    }



                    HObject ImageLB1, ImageLB2;
                    HOperatorSet.ReduceDomain(ho_ImageReduced, LBOpening1, out ImageLB1);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, LBOpening2, out ImageLB2);



                    HObject EdgeAmplitude1, EdgeAmplitude2, RegionDynThresh21, RegionDynThresh22;
                    HOperatorSet.SobelAmp(ImageLB1, out EdgeAmplitude1, "sum_abs", iLBSobelSize); //参数43：棱边瓷损检测梯度大小 iLBSobelSize = 7
                    HOperatorSet.SobelAmp(ImageLB2, out EdgeAmplitude2, "sum_abs", iLBSobelSize);

                    if (iCode == 1)
                    {
                        HOperatorSet.Threshold(EdgeAmplitude1, out RegionDynThresh21, iLBSobelThres, 255); //R代码 10   //参数44：棱边瓷损检测梯度阈值 iLBSobelThres = 10、15
                        HOperatorSet.Threshold(EdgeAmplitude2, out RegionDynThresh22, iLBSobelThres, 255); //R代码 10
                    }
                    else
                    {
                        HOperatorSet.Threshold(EdgeAmplitude1, out RegionDynThresh21, iLBSobelThres, 255); //B代码 15
                        HOperatorSet.Threshold(EdgeAmplitude2, out RegionDynThresh22, iLBSobelThres, 255); //B代码 15
                    }


                    HObject DynCnnLB1, DynCnnLB2, LBBig1, LBBig2, LBBigAffine1, LBBigAffine2;
                    HOperatorSet.Connection(RegionDynThresh21, out DynCnnLB1);
                    HOperatorSet.Connection(RegionDynThresh22, out DynCnnLB2);

                    HOperatorSet.SelectShapeStd(DynCnnLB1, out LBBig1, "max_area", 70);
                    HOperatorSet.SelectShapeStd(DynCnnLB2, out LBBig2, "max_area", 70);

                    //矫正LBBig位置为水平
                    HOperatorSet.VectorAngleToRigid(100, 100, PhiBin, 100, 100, 0, out HomMat2D);
                    HOperatorSet.AffineTransRegion(LBBig1, out LBBigAffine1, HomMat2D, "constant");
                    HOperatorSet.AffineTransRegion(LBBig2, out LBBigAffine2, HomMat2D, "constant");

                    HTuple R1111Dn, R2222Dn;
                    HOperatorSet.SmallestRectangle1(LBBigAffine1, out R1111, out C1111, out R2222, out C2222);
                    HOperatorSet.SmallestRectangle1(LBBigAffine2, out R1111Dn, out C1111, out R2222Dn, out C2222);

                    HTuple LBHigh1 = R2222 - R1111;
                    HTuple LBHigh2 = R2222Dn - R1111Dn;


                    //判断上下棱边平行度  LBBigAffine1 ， LBBigAffine2  计算角度判断瓷体变形或者瓷损
                    HTuple hhh, bbb, jiao1, jiao2, L111, L222, T2, T1;
                    HOperatorSet.SmallestRectangle2(LBBigAffine1, out hhh, out bbb, out jiao1, out L111, out L222);
                    HOperatorSet.SmallestRectangle2(LBBigAffine2, out hhh, out bbb, out jiao2, out L111, out L222);
                    HOperatorSet.TupleDeg(jiao1, out T1);
                    HOperatorSet.TupleDeg(jiao2, out T2);
                    HTuple T12diff = T1 - T2;
                    HOperatorSet.TupleAbs(T12diff, out T12diff);  //计算角度

                    if (iT12Diff == 0) iT12Diff = 2;  //默认角度差2度

                    //绘制棱边瓷损检查区域，识别棱边区域
                    if (is_Debug)
                    {
                        HObject ho_RectangleDebug;
                        syShowRegionBorder(LBOpening1, ref listObj2Draw, "OK");    //棱边搜索区域
                        syShowRegionBorder(LBOpening2, ref listObj2Draw, "OK");
                        syShowRegionBorder(LBBigAffine1, ref listObj2Draw, "OK");  //棱边区域
                        syShowRegionBorder(LBBigAffine2, ref listObj2Draw, "OK");
                    }

                    if (is_Debug)
                    {
                        strDebug += "(12)棱边瓷损相关参数:\n";

                        strDebug += "棱边定位矩形半宽:" + iLBStructLength2.ToString() + "\n";
                        strDebug += "棱边定位宽度变化:" + iLBWidthScale.ToString() + "\n";
                        strDebug += "检测梯度大小:" + iLBSobelSize.ToString() + "\n";
                        strDebug += "检测梯度阈值:" + iLBSobelThres.ToString() + "\n";
                        strDebug += "设定高度:" + iLBHigh.ToString() + "\n";
                        strDebug += "设定棱边角度差:" + iT12Diff.ToString() + "\n";

                        strDebug += "当前上高度:" + LBHigh1.D.ToString("0.0") + "\n";
                        strDebug += "当前下高度:" + LBHigh2.D.ToString("0.0") + "\n";
                        strDebug += "当前棱边角度差:" + T12diff.D.ToString("0.0") + "\n";
                    }

                    strDebug += "\n";

                    if (A_棱边瓷损 == 0) goto A_棱边瓷损END;
                    if ((LBHigh1 > iLBHigh) || (LBHigh2 > iLBHigh) || T12diff > iT12Diff)  //棱边瓷损高度大于18 ，或角度差大于2
                    {
                        listObj2Draw[1] = "NG-棱边瓷损";
                        //绘制电极轮廓  RegionSP2
                        hv_Num = 0;

                        if (LBHigh1 > iLBHigh)
                            HOperatorSet.CountObj(LBBig1, out hv_Num);
                        else
                            HOperatorSet.CountObj(LBBig2, out hv_Num);

                        for (int i = 1; i <= hv_Num; i++)
                        {

                            if (LBHigh1 > iLBHigh)
                                HOperatorSet.SelectObj(LBBig1, out ho_RegionSel, i);
                            else
                                HOperatorSet.SelectObj(LBBig2, out ho_RegionSel, i);









                            HObject ho_RegionSelCnnnn;

                            HOperatorSet.Connection(ho_RegionSel, out ho_RegionSelCnnnn);

                            syShowRegionBorder(ho_RegionSelCnnnn, ref listObj2Draw, "NG");
                            //输出NG详情
                            lsInfo2Draw.Add("瓷损高度Up: " + LBHigh1.D.ToString("0.0") + " pix" + "瓷损高度Dn: " + LBHigh2.D.ToString("0.0") + " pix");
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("瓷损高度上限: " + iLBHigh.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("当前棱边角度差: " + T12diff.D.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("棱边角度差上限: " + iT12Diff.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }

                    A_棱边瓷损END:

#endif







                    //检测电极发黑（刮伤）
                    HObject ho_RegionTransEro, ImageTransEro, RegionGS, RegionDark, RegionCKDJ, ImageCKDJ, RegionDarkCnn;





                    //电极区域定位1
                    //      HOperatorSet.ErosionCircle(ho_RegionTrans,out ho_RegionTransEro,25);
                    //      HObject DianjiUnionEro;
                    //      HOperatorSet.ErosionCircle(ho_RegionLR, out  DianjiUnionEro, iElectBlackErrEro);  //参数45： 电极黑缺陷检测电极腐蚀半径 iElectBlackErrEro = 5
                    //      HOperatorSet.Intersection(ho_RegionTransEro, DianjiUnionEro, out RegionCKDJ);


                    //电极区域定位2
                    HObject ho_DianjiOpenUnion;
                    HOperatorSet.Union1(ho_DianjiOpen, out ho_DianjiOpenUnion);
                    HOperatorSet.ErosionCircle(ho_DianjiOpenUnion, out RegionCKDJ, iElectBlackErrEro);  //参数45： 电极黑缺陷检测电极腐蚀半径 iElectBlackErrEro = 5










                    HOperatorSet.ReduceDomain(ho_ImageReduced, RegionCKDJ, out ImageCKDJ);
                    HOperatorSet.Threshold(ImageCKDJ, out RegionDark, 0, iGSThres);    // 参数46 ：电极黑缺陷阈值  iGSThres      刮伤阈值30
                    HOperatorSet.Connection(RegionDark, out RegionDarkCnn);
                    HOperatorSet.SelectShape(RegionDarkCnn, out RegionGS, "area", "and", iGSArea, 999999999);  // 参数47 ：电极黑缺陷面积  iGSArea      刮伤面积 500
                    HOperatorSet.CountObj(RegionGS, out Num);

                    if (is_Debug)
                    {
                        HObject RegionCnnnnnnnnnn;
                        HOperatorSet.Connection(RegionCKDJ, out RegionCnnnnnnnnnn);
                        syShowRegionBorder(RegionCnnnnnnnnnn, ref listObj2Draw, "OK"); //绘制电极检测区域

                        strDebug += "(13)电极黑缺陷相关参数:\n";

                        strDebug += "电极腐蚀半径:" + iElectBlackErrEro.ToString() + "\n";
                        strDebug += "设定阈值:" + iGSThres.ToString() + "\n";
                        strDebug += "设定面积:" + iGSArea.ToString() + "\n";

                        HTuple Rowtmp, Coltmp, Areatmp;
                        HOperatorSet.AreaCenter(RegionDarkCnn, out Rowtmp, out Coltmp, out Areatmp);
                        if (Areatmp.TupleLength() > 0)
                        {
                            HObject DynCnn3Union;
                            HOperatorSet.Union1(RegionDarkCnn, out DynCnn3Union);
                            syShowRegionBorder(DynCnn3Union, ref listObj2Draw, "OK"); //绘制当前电极缺陷区域：DynCnn3

                            for (int fff = 0; fff < Areatmp.TupleLength(); fff++)
                                strDebug += "当前缺陷面积:" + Areatmp.TupleSelect(fff).D.ToString() + "\n";
                        }

                        strDebug += "NG找到个数:" + Num.ToString() + "\n";

                        strDebug += "\n";
                    }


                    if (A_电极刮伤 == 0) goto A_电极刮伤END;
                    if (Num > 0)
                    {
                        listObj2Draw[1] = "NG-电极刮伤";

                        //绘制电极轮廓  RegionGS
                        hv_Num = 0;
                        HOperatorSet.CountObj(RegionGS, out hv_Num);
                        HTuple AAA, RRR, CCC;
                        AAA = 0;
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(RegionGS, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            HOperatorSet.AreaCenter(ho_RegionSel, out AAA, out RRR, out CCC);
                            lsInfo2Draw.Add("刮伤面积上限: " + iGSArea.ToString("0.0") + " pix" + "当前面积: " + AAA.D.ToString("0.0"));
                            lsInfo2Draw.Add("刮伤阈值上限: " + iGSThres.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strDebug += "电极黑缺陷当前缺陷面积:" + AAA.D.ToString("0.0") + "\n";

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极刮伤END:




                    //检查电极漏铜
                    HObject DianjiEro, ImageCKDJColor, ImageR, ImageG, ImageB, RegionLT, RegionLTCnn, RegionLT1;
                    HOperatorSet.ErosionCircle(ho_RegionLR, out DianjiEro, 20);
                    HOperatorSet.ReduceDomain(hoReduced, DianjiEro, out ImageCKDJColor);
                    HOperatorSet.Decompose3(ImageCKDJColor, out ImageR, out ImageG, out ImageB);  //使用蓝色分量分析漏铜


                    HObject ImageResult1, ImageResult3;
                    HOperatorSet.TransFromRgb(ImageR, ImageG, ImageB, out ImageResult1, out ImageB, out ImageResult3, "hsi");





                    HOperatorSet.Threshold(ImageB, out RegionLT, 0, iLTThres);                                // 参数48 ：iLTThres 漏铜阈值 50
                    HOperatorSet.Connection(RegionLT, out RegionLTCnn);
                    HOperatorSet.SelectShape(RegionLTCnn, out RegionLT1, "area", "and", iLTArea, 999999999);  // 参数49 ：iLTArea  漏铜面积 1000
                    HOperatorSet.CountObj(RegionLT1, out Num);

                    if (A_电极漏铜 == 0) goto A_电极漏铜END;
                    if (Num > 0)
                    {
                        listObj2Draw[1] = "NG-电极漏铜";
                        //绘制电极轮廓  RegionLT1
                        hv_Num = 0;
                        HOperatorSet.CountObj(RegionLT1, out hv_Num);
                        HTuple AAA, RRR, CCC;
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(RegionLT1, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            HOperatorSet.AreaCenter(ho_RegionSel, out AAA, out RRR, out CCC);
                            lsInfo2Draw.Add("漏铜面积上限: " + iLTArea.ToString("0.0") + " pix" + "当前面积: " + AAA.D.ToString("0.0"));
                            lsInfo2Draw.Add("漏铜阈值上限: " + iLTThres.ToString("0.0"));

                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    A_电极漏铜END:



                    //检查锡瘤（侧面计算突出部分）
                    //  int iXLThres = 50;    //参数50：iXLThres 锡瘤阈值 50
                    //  int iXLArea = 150;    //参数51：iXLArea  锡瘤面积 150
                    //  int iXLRadis = 4;     //参数52：iXLRadis 锡瘤面内接圆半径 4 
                    HObject RegionTransDilaX, RegionTransDila, RegionDn, Diff2, SortDiffYOpen, DiffYOpenCnn, DiffYOpen, Diff, RegionTransDila2, DiffX, RegionTransDila1, DiffXCnn, RegionXL, RegionXLCnn, DiffUnion, ImageXL, RegionUp, RegionUpUp, RegionTransDilaY, DiffY, DiffYCnn, DiffYCnnUnion;
                    HOperatorSet.DilationCircle(ho_RegionTrans, out RegionTransDila1, 3);
                    HOperatorSet.DilationCircle(RegionTransDila1, out RegionTransDila2, 20);
                    HOperatorSet.Difference(RegionTransDila2, RegionTransDila1, out Diff);
                    if (iStation == 34) //34工位
                    {
                        HOperatorSet.DilationRectangle1(RegionTransDila1, out RegionTransDilaX, 40, 1);
                        HOperatorSet.Difference(RegionTransDila2, RegionTransDilaX, out DiffY);
                        HOperatorSet.OpeningCircle(DiffY, out DiffYOpen, 5);
                        HOperatorSet.Connection(DiffYOpen, out DiffYOpenCnn);
                        HOperatorSet.SortRegion(DiffYOpenCnn, out SortDiffYOpen, "upper_left", "true", "row");


                        //HOperatorSet.SelectObj(SortDiffYOpen, out RegionDn, 2);
                        //HOperatorSet.Difference(Diff, RegionDn, out Diff2);
                        //HOperatorSet.OpeningCircle(Diff2, out Diff,1);

                        HOperatorSet.SelectObj(SortDiffYOpen, out Diff, 1);  //34工位选取上半部分

                    }

                    HOperatorSet.ReduceDomain(ho_ImageReduced, Diff, out ImageXL);
                    HOperatorSet.Threshold(ImageXL, out RegionXL, iXLThres, 255);                            //锡瘤阈值 50
                    HOperatorSet.Connection(RegionXL, out RegionXLCnn);
                    //HOperatorSet.AreaCenter(RegionXLCnn, Area3, Row6, Column6) ;
                    //HOperatorSet.SelectShape(RegionXLCnn, out RegionXL, "area", "and", iXLArea, 9999999);    //锡瘤面积200
                    HOperatorSet.SelectShape(RegionXLCnn, out RegionXL, "inner_radius", "and", iXLRadis, 99999);  //锡瘤内接圆



                    //绘制锡瘤检查区域
                    if (is_Debug)
                    {
                        syShowRegionBorder(Diff, ref listObj2Draw, "OK");
                    }


                    if (A_锡瘤 == 0) goto 锡瘤END;
                    HOperatorSet.CountObj(RegionXL, out Num);
                    if (Num > 0)
                    {
                        listObj2Draw[1] = "NG-锡瘤";
                        //绘制电极轮廓  RegionLT1
                        hv_Num = 0;
                        HOperatorSet.CountObj(RegionXL, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(RegionXL, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            //输出NG详情
                            lsInfo2Draw.Add("锡瘤面积上限: " + iXLArea.ToString("0.0") + " pix");
                            lsInfo2Draw.Add("锡瘤阈值上限: " + iXLThres.ToString("0.0"));
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                        }

                        strMessage = DebugPrint(strDebug, is_Debug);

                        return listObj2Draw;
                    }
                    锡瘤END:;

                    #endregion

                }



























                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //执行到这里，OK  绘制电极轮廓
                listObj2Draw[1] = "OK";
                hv_Num = 0;
                HOperatorSet.CountObj(ho_DianjiOpen, out hv_Num);
                for (int i = 1; i <= hv_Num; i++)
                {
                    HOperatorSet.SelectObj(ho_DianjiOpen, out ho_RegionSel, i);
                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
                }

                //GC.Collect();

                strMessage = DebugPrint(strDebug, is_Debug);

                //MessageBox.Show(strMessage);

                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect22", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect22.ReleaseMutex();
            }
            #endregion
        }

        //六面机0201电容 12 相机 算法    引用文件11
        public static List<object> sySixSideDetect21(HObject hoImage, List<PointF[]> lkkPolygon, string strParams, ref string strMessage)
        {
            #region  *** 六面机0201电容 12 相机  ***

            if (bUseMutex) muDetect21.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            HObject hoConcate, hoRegion, hoUnion, hoReduced;

            try
            {
                List<string> lsInfo2Draw = new List<string>();
                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //读取参数
                string[] strUserParam = strParams.Split('#');

                int iCameraCode = 888;

                //参数1：
                int iMainThres = int.Parse(strUserParam[30]);     //电极定位阈值 40
                //参数2 ：总定位开运算iMainOpening = 5
                int iMainOpening = int.Parse(strUserParam[31]);   //5
                //参数3 ：总定位闭运算iMainClosing = 10
                int iMainClosing = int.Parse(strUserParam[32]);   //10
                //参数4:
                int iMainFilterArea = int.Parse(strUserParam[33]); //电极过滤面积 20000
                //参数5:
                float iRectyThres = float.Parse(strUserParam[34]); //电极矩形度阈值 0.85
                //参数6：
                int iSideLength = int.Parse(strUserParam[35]);     //电极边长95
                //参数7：
                int iSideLengthScale = int.Parse(strUserParam[36]); //电极边长变化量15
                //参数8 ：封端瓷损开运算iCisunOpening = 4
                int iCisunOpening = int.Parse(strUserParam[37]);    //4
                //参数9：
                int iMinCisunarea = int.Parse(strUserParam[38]);    //瓷损面积 1200
                //参数10 ：
                int iMinCisunradis = int.Parse(strUserParam[39]);  //瓷损半径 10
                //参数11 ： 黑缺陷检查腐蚀半径 iEroCheckGS = 15
                int iEroCheckGS = int.Parse(strUserParam[40]);     //15
                //参数12 ：刮伤阈值
                int iGSThres = int.Parse(strUserParam[41]);        //刮伤阈值 15 
                //参数13： 刮伤面积
                int iGSArea = int.Parse(strUserParam[42]);         //刮伤面积 200

                //封端脱落mean
                //int iFengduanMean = int.Parse(strUserParam[14]);      //端面平均灰度   60






                bool iCheckSelAll = false;   //是否全检还是依赖于外部选择
                //用户检测项可选项 strUserParam[94]开始  , 0：屏蔽 1：启用 ， 默认 1（打钩）
                int A_封端前瓷损 = iCheckSelAll ? 1 : int.Parse(strUserParam[94]); //封端前瓷损
                int A_封端刮伤 = iCheckSelAll ? 1 : int.Parse(strUserParam[95]); //封端刮伤







                //判断是否调试输出
                bool is_Debug;
                if (strMessage == "1")
                    is_Debug = true;
                else
                    is_Debug = false;
                //初始化调试输出内容
                string strDebug = "";

                if (is_Debug)
                {
                    syShowRegionBorder(hoUnion, ref listObj2Draw, "NG");  //调试状态显示reduce区域
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HObject RegionFillUp, RegionClosing, ho_Rectangle1, ho_Rectangleqqq, ho_Rectangleppp, ho_RegionTrans, ho_RegionLR, ho_RegionL, ho_RegionR, ho_ConnectedRegions2, ho_RegionsBig, ho_RegionEdge, ho_ImageReduced, ho_ImageReduced2, ho_Image1, ho_Image2, ho_Image3, ho_RegionLight, ho_RegionBig, ho_RegionOpening, ho_ConnectedRegions, ho_RegionFillUp;
                HObject ImageMean, RegionDynThresh, ho_RegionErr, ho_ConnectedRegions3, ho_RegionBlack, ho_ImageReducekkkkk, ho_RegionErosion, ho_RegionUnionkkkkk, ho_RegionSel, ho_RegionFillUp1, ho_RegionOpening1, ho_EdgeAmplitude, ho_Regionk, ho_ConnectedRegions1, ho_SelectedRegions, ho_RegionUnion, ho_RegionClosing;
                HTuple hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2, hv_Rectangularity, hv_Number, NChannel, hv_Rowqqq, hv_Colqqq, hv_Phiqqq, hv_Length1qqq, hv_Length2qqq, hv_Recties, hv_Rowppp, hv_Colppp, hv_Phippp, hv_Length1ppp, hv_Length2ppp, hv_Recty, hv_N, hv_Num;

                //*一、产品定位
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    //HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                    HOperatorSet.Rgb1ToGray(hoReduced, out ho_ImageReduced);
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);
                }

                HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionLight, iMainThres, 255);             //参数1 ：总定位阈值iMainThres   电极定位阈值40
                HOperatorSet.OpeningCircle(ho_RegionLight, out ho_RegionOpening, iMainOpening);           //参数2 ：总定位开运算iMainOpening = 5

                //    HOperatorSet.MeanImage(ho_ImageReduced, out ImageMean, 250, 250);
                //    HOperatorSet.DynThreshold(ho_ImageReduced, ImageMean, out RegionDynThresh, 3, "light");

                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);


                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_RegionBig, "max_area", 70);
                HOperatorSet.ClosingCircle(ho_RegionBig, out RegionClosing, iMainClosing);               //参数3 ：总定位闭运算iMainClosing = 10
                HOperatorSet.FillUp(RegionClosing, out RegionFillUp);


                HOperatorSet.SelectShape(RegionFillUp, out ho_SelectedRegions, "area",         //参数4 ：电极过滤面积iMainFilterArea 20000-50000
"and", iMainFilterArea, iMainFilterArea + 30000);

                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);

                if (is_Debug)
                {
                    strDebug += "(1)总定位相关参数:" + "\n";
                    strDebug += "总定位阈值:" + iMainThres.ToString() + "\n";
                    strDebug += "总定位开运算:" + iMainOpening.ToString() + "\n";
                    strDebug += "总定位闭运算:" + iMainClosing.ToString() + "\n";
                    strDebug += "电极过滤面积:" + iMainFilterArea.ToString() + "\n";

                    HTuple Rowtmp, Coltmp, Areatmp;
                    HOperatorSet.AreaCenter(ho_SelectedRegions, out Areatmp, out Rowtmp, out Coltmp);
                    strDebug += "当前识别电极面积:" + Areatmp.TupleSelect(0).D.ToString("0.0") + "\n";

                    strDebug += "当前识别电极个数:" + hv_Number.D.ToString() + "\n";
                    strDebug += "\n";
                }



                if ((int)(new HTuple(hv_Number.TupleNotEqual(1))) != 0)
                {
                    listObj2Draw[1] = "NG-无定位";
                    //绘制电极轮廓  ho_RegionFillUp1
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_SelectedRegions, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "D:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }

                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_SelectedRegions, out ho_ImageReduced2);

                //*二、检查尺寸
                HOperatorSet.Rectangularity(ho_SelectedRegions, out hv_Rectangularity);
                HTuple HRectyThres = iRectyThres;

                if (is_Debug)
                {
                    strDebug += "(2)矩形度相关参数:" + "\n";
                    strDebug += "当前矩形度:" + hv_Rectangularity.D.ToString("0.00") + "\n";
                    strDebug += "设置矩形度:" + HRectyThres.D.ToString("0.00") + "\n";
                    strDebug += "\n";
                }


                if (A_封端前瓷损 == 0) goto A_封端前瓷损END1;
                //参数5：矩形度 iRectyThres
                if ((int)(new HTuple(hv_Rectangularity.TupleLess(HRectyThres))) != 0)
                {
                    listObj2Draw[1] = "NG-封端前瓷损";
                    //绘制电极轮廓  ho_RegionFillUp1
                    syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                    //输出NG详情
                    lsInfo2Draw.Add("矩形度:" + hv_Rectangularity.D.ToString("0.0"));
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "D:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }
                A_封端前瓷损END1:

                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);

                //判断旋转角度
                HTuple Degggg;
                HOperatorSet.TupleDeg(hv_Phi, out Degggg);
                if (Degggg.D < 0) Degggg.D = Degggg.D + 90;
                if (Degggg.D > 45) Degggg.D = 90 - Degggg.D;
                int iDeg = 15;

                if (is_Debug)
                {
                    HObject RectChiCun;
                    HOperatorSet.GenRectangle2(out RectChiCun, hv_Row, hv_Column, hv_Phi, iSideLength, iSideLength);
                    syShowRegionBorder(RectChiCun, ref listObj2Draw, "OK");  //调试状态显示尺寸区域


                    strDebug += "(3)尺寸相关参数:" + "\n";

                    strDebug += "设定值:" + iSideLength.ToString() + "\n";
                    strDebug += "设定变化值:" + iSideLengthScale.ToString() + "\n";
                    strDebug += "上限:" + (iSideLength + iSideLengthScale).ToString() + "\n";
                    strDebug += "下限:" + (iSideLength - iSideLengthScale).ToString() + "\n";
                    strDebug += "角度:" + iDeg.ToString() + "\n";

                    strDebug += "当前值1:" + hv_Length1.D.ToString("0.00") + "\n";
                    strDebug += "当前值2:" + hv_Length2.D.ToString("0.00") + "\n";
                    strDebug += "当前值角度:" + Degggg.D.ToString("0.00") + "\n";

                    strDebug += "\n";
                }

                if (Degggg > iDeg || Degggg < (0 - iDeg))  //无定位
                {
                    listObj2Draw[1] = "NG-无定位";

                    //输出NG详情
                    lsInfo2Draw.Add("当前角度:" + Degggg.D.ToString("0.0") + "设定角度:" + iDeg.ToString());
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }


                if (A_封端前瓷损 == 0) goto A_封端前瓷损END2;
                if ((int)((new HTuple((new HTuple((new HTuple(hv_Length1.TupleLess(iSideLength - iSideLengthScale))).TupleOr(
                    new HTuple(hv_Length1.TupleGreater(iSideLength + iSideLengthScale))))).TupleOr(new HTuple(hv_Length2.TupleLess(
                    iSideLength - iSideLengthScale))))).TupleOr(new HTuple(hv_Length2.TupleGreater(iSideLength + iSideLengthScale)))) != 0)

                //参数6： 边长 iSideLength    
                //参数7： 边长变化iSideLengthScale
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-封端前瓷损";
                    //绘制电极轮廓  ho_RegionFillUp1
                    syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                    //输出NG详情
                    lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + "*" + hv_Length2.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));


                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "C:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }
                A_封端前瓷损END2:

                //检查封端瓷损
                HObject Cnnonnnnn, RegionDiff, DiffOpen, RegionCisun, RegionCisun1;
                HTuple CisunNum;
                HOperatorSet.Difference(ho_Rectangle1, ho_SelectedRegions, out RegionDiff);
                HOperatorSet.OpeningCircle(RegionDiff, out DiffOpen, iCisunOpening);  //4                                //参数8 ：封端瓷损开运算iCisunOpening = 4
                HOperatorSet.Connection(DiffOpen, out Cnnonnnnn);
                HOperatorSet.SelectShape(Cnnonnnnn, out RegionCisun, "area", "and", iMinCisunarea, 9999999);             //参数9：瓷损面积 iMinCisunarea    1200
                HOperatorSet.SelectShape(RegionCisun, out RegionCisun1, "inner_radius", "and", iMinCisunradis, 9999999); //参数10 ：瓷损半径iMinCisunradis  瓷损半径10
                HOperatorSet.CountObj(RegionCisun1, out CisunNum);

                if (is_Debug)
                {
                    strDebug += "(4)瓷损相关参数:" + "\n";

                    strDebug += "开运算:" + iCisunOpening.ToString() + "\n";
                    strDebug += "面积设定值:" + iMinCisunarea.ToString() + "\n";
                    strDebug += "瓷损半径设定值:" + iMinCisunradis.ToString() + "\n";

                    HObject CnnonnnnnMax;
                    HTuple Nummmm, Rowpppp, Colpppp, Areapppp, radissss;
                    HOperatorSet.SelectShapeStd(Cnnonnnnn, out CnnonnnnnMax, "max_area", 70);
                    HOperatorSet.CountObj(CnnonnnnnMax, out Nummmm);
                    if (Nummmm = 1)
                    {
                        HOperatorSet.AreaCenter(CnnonnnnnMax, out Areapppp, out Rowpppp, out Colpppp);
                        HOperatorSet.InnerCircle(CnnonnnnnMax, out Rowpppp, out Colpppp, out radissss);
                        strDebug += "当前瓷损面积:" + Areapppp.D.ToString("0.0") + "\n";
                        strDebug += "当前瓷损半径:" + radissss.D.ToString("0.0") + "\n";
                    }
                }
                strDebug += "\n";

                if (A_封端前瓷损 == 0) goto A_封端前瓷损END3;
                if (CisunNum > 0)
                {
                    listObj2Draw[1] = "NG-封端前瓷损";
                    //绘制电极轮廓  ho_RegionFillUp1
                    hv_Num = 0;
                    HOperatorSet.CountObj(RegionCisun1, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(RegionCisun1, out ho_RegionSel, i);
                        syShowRegionBorder(RegionCisun1, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    HObject UnionCisun;
                    HTuple AreaCisun, aaaaaa, bbbbbb;
                    HOperatorSet.Union1(RegionCisun1, out UnionCisun);
                    HOperatorSet.AreaCenter(UnionCisun, out AreaCisun, out aaaaaa, out bbbbbb);

                    lsInfo2Draw.Add("最小半径:" + iMinCisunradis.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "D:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }

                    strDebug += "当前检测出的瓷损面积:" + AreaCisun.D.ToString() + "\n";

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }
                A_封端前瓷损END3:

                //检查端头内部损伤(黑缺陷)
                HObject ImageDT, RegionDark, RegionDarkCnn, RegionGS, RectangleEro;
                HTuple Num;
                HOperatorSet.ErosionCircle(ho_Rectangle1, out RectangleEro, iEroCheckGS);                //参数11 ： 黑缺陷检查腐蚀半径 iEroCheckGS = 15
                HOperatorSet.ReduceDomain(ho_ImageReduced, RectangleEro, out ImageDT);
                HOperatorSet.Threshold(ImageDT, out RegionDark, 0, iGSThres);                            //参数12 ：刮伤阈值 iGSThres  20
                HOperatorSet.Connection(RegionDark, out RegionDarkCnn);
                HOperatorSet.SelectShape(RegionDarkCnn, out RegionGS, "area", "and", iGSArea, 9999999);  //参数13：刮伤面积iGSArea 200
                HOperatorSet.CountObj(RegionGS, out Num);

                if (is_Debug)
                {
                    syShowRegionBorder(RectangleEro, ref listObj2Draw, "OK");  //调试状态显示电极刮伤检查区域

                    strDebug += "(5)刮伤相关参数:" + "\n";

                    strDebug += "腐蚀半径:" + iEroCheckGS.ToString() + "\n";
                    strDebug += "刮伤阈值:" + iGSThres.ToString() + "\n";
                    strDebug += "刮伤设定面积:" + iGSArea.ToString() + "\n";

                    HObject CnnonnnnnMax;
                    HTuple Nummmm;
                    HOperatorSet.SelectShapeStd(RegionDarkCnn, out CnnonnnnnMax, "max_area", 70);
                    HOperatorSet.CountObj(CnnonnnnnMax, out Nummmm);
                    if (Nummmm = 1)
                    {
                        HTuple Rowpppp, Colpppp, Areapppp;
                        HOperatorSet.AreaCenter(CnnonnnnnMax, out Areapppp, out Rowpppp, out Colpppp);
                        strDebug += "当前刮伤面积:" + Areapppp.D.ToString("0.0") + "\n";
                    }
                    strDebug += "\n";
                }


                if (A_封端刮伤 == 0) goto A_封端刮伤END1;
                if (Num > 0)
                {
                    listObj2Draw[1] = "NG-封端刮伤";
                    //绘制电极轮廓  RegionGS
                    hv_Num = 0;
                    HOperatorSet.CountObj(RegionGS, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(RegionGS, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }


                    HObject RegionGSUnion;
                    HTuple Arealll, Rowlll, Collll;
                    HOperatorSet.Union1(RegionGS, out RegionGSUnion);
                    HOperatorSet.AreaCenter(RegionGSUnion, out Arealll, out Rowlll, out Collll);

                    lsInfo2Draw.Add("当前刮伤面积:" + Arealll.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("面积上限:" + iGSArea.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("阈值上限:" + iGSThres.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    strDebug += "当前刮伤面积:" + Arealll.D.ToString() + "\n";

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }
                A_封端刮伤END1:

#if false

                //检查封端漏瓷
                int iLCThres = 40;
                int iLCArea  = 1000;
                HObject LCCorImage, C1, C2, C3, RegionLC, RegionCnnLC, RegionLcCheck;
                HOperatorSet.ReduceDomain(hoReduced, RectangleEro, out LCCorImage);
                HOperatorSet.Decompose3(LCCorImage,out C1,out C2,out C3);
                HOperatorSet.Threshold(C1, out RegionLC, 0, iLCThres); //漏瓷阈值 40
                HOperatorSet.Connection(RegionLC,out RegionCnnLC);
                HOperatorSet.SelectShape(RegionCnnLC, out RegionLcCheck, "area", "and", iLCArea, 999999);    //面积大于1000
                HOperatorSet.CountObj(RegionLcCheck, out Num);
                if (is_Debug)
                {
                    syShowRegionBorder(RegionCnnLC, ref listObj2Draw, "OK");  //调试状态显示电极刮伤检查区域

                    strDebug += "(6)漏镍相关参数:" + "\n";

                    strDebug += "漏镍阈值:" + iLCThres.ToString() + "\n";
                    strDebug += "漏镍设定面积:" + iLCArea.ToString() + "\n";

                    HObject CnnonnnnnMax;
                    HTuple Nummmm;
                    HOperatorSet.SelectShapeStd(RegionCnnLC, out CnnonnnnnMax, "max_area", 70);
                    HOperatorSet.CountObj(CnnonnnnnMax, out Nummmm);
                    if (Nummmm = 1)
                    {
                        HTuple Rowpppp, Colpppp, Areapppp;
                        HOperatorSet.AreaCenter(CnnonnnnnMax, out Areapppp, out Rowpppp, out Colpppp);
                        strDebug += "当前漏镍面积:" + Areapppp.D.ToString("0.0") + "\n";
                    }
                    strDebug += "\n";
                }
                if (Num > 0)
                {
                    listObj2Draw[1] = "NG-封端刮伤";
                    //绘制电极轮廓  RegionGS
                    hv_Num = 0;
                    HOperatorSet.CountObj(RegionLcCheck, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(RegionLcCheck, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }


                    HObject RegionGSUnion;
                    HTuple Arealll, Rowlll, Collll;
                    HOperatorSet.Union1(RegionLcCheck, out RegionGSUnion);
                    HOperatorSet.AreaCenter(RegionGSUnion, out Arealll, out Rowlll, out Collll);

                    lsInfo2Draw.Add("当前漏镍面积:" + Arealll.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("面积上限:" + iLCArea.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("阈值上限:" + iLCThres.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    strDebug += "当前漏镍面积:" + Arealll.D.ToString() + "\n";

                    strMessage = DebugPrint(strDebug, is_Debug);

                    return listObj2Draw;
                }

#endif




















                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //执行到这里，OK  绘制外接矩形
                listObj2Draw[1] = "OK";
                List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcode.ToArray());
                listObj2Draw.Add("OK");

                //GC.Collect();

                strMessage = DebugPrint(strDebug, is_Debug);

                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect12", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect21.ReleaseMutex();
            }
            #endregion
        }









        //六面机0201电容 12 相机 算法    引用文件11
        public static List<object> sySixSideDetect2121(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 六面机0201电容 12 相机  ***

            if (bUseMutex) muDetect21.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            HObject hoConcate, hoRegion, hoUnion, hoReduced;

            try
            {
                List<string> lsInfo2Draw = new List<string>();
                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //读取参数
                string[] strUserParam = strParams.Split('#');

                //int iMainThres = int.Parse(strUserParam[4]);      //电极定位阈值 40
                //int iMainFilterArea = int.Parse(strUserParam[5]); //电极过滤面积 20000
                //float iRectyThres = float.Parse(strUserParam[6]); //电极矩形度阈值 0.85
                //int iSideLength = int.Parse(strUserParam[7]);     //电极边长95
                //int iSideLengthScale = int.Parse(strUserParam[8]); //电极边长变化量15
                //int iMinCisunarea = int.Parse(strUserParam[9]);    //瓷损面积 1200
                //int iMinCisunradis = int.Parse(strUserParam[10]);  //瓷损半径 10
                int iCameraCode = 888;    //工位号

                ////刮伤参数
                //int iGSThres = int.Parse(strUserParam[12]);      //刮伤阈值 15 
                //int iGSArea  = int.Parse(strUserParam[13]);      //刮伤面积 200

                ////封端脱落mean
                //int iFengduanMean = int.Parse(strUserParam[14]);      //端面平均灰度   60





                //开放所有参数
                //参数1 ：总定位阈值 
                int iMainThres = int.Parse(strUserParam[4]);                 //电极定位阈值 40
                //参数2 ：总定位开运算
                int iMainOpening = int.Parse(strUserParam[5]);                 // 5
                //参数3 ：总定位闭运算
                int iMainClosing = int.Parse(strUserParam[6]);                 //10
                //参数4 ：电极过滤面积
                int iMainFilterArea = int.Parse(strUserParam[7]);              //电极过滤面积 20000
                //参数5 ：矩形度 
                float iRectyThres = float.Parse(strUserParam[8]);
                //参数6 ：边长 
                int iSideLength = int.Parse(strUserParam[9]);
                //参数7 ：边长变化 
                int iSideLengthScale = int.Parse(strUserParam[10]);
                //参数8 ：封端瓷损开运算 
                int iCisunOpening = int.Parse(strUserParam[11]);               //4
                //参数9 ：瓷损面积 
                int iMinCisunarea = int.Parse(strUserParam[12]);
                //参数10：瓷损半径 
                int iMinCisunradis = int.Parse(strUserParam[13]);              //瓷损半径10
                //参数11：黑缺陷腐蚀半径 
                int iEroCheckGS = int.Parse(strUserParam[14]);                 //刮伤收缩半径10
                //参数12：刮伤阈值
                int iGSThres = int.Parse(strUserParam[15]);                    //刮伤阈值 15 
                //参数13：刮伤面积
                int iGSArea = int.Parse(strUserParam[16]);                     //刮伤面积 200








                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HObject RegionFillUp, RegionClosing, ho_Rectangle1, ho_Rectangleqqq, ho_Rectangleppp, ho_RegionTrans, ho_RegionLR, ho_RegionL, ho_RegionR, ho_ConnectedRegions2, ho_RegionsBig, ho_RegionEdge, ho_ImageReduced, ho_ImageReduced2, ho_Image1, ho_Image2, ho_Image3, ho_RegionLight, ho_RegionBig, ho_RegionOpening, ho_ConnectedRegions, ho_RegionFillUp;
                HObject ho_RegionErr, ho_ConnectedRegions3, ho_RegionBlack, ho_ImageReducekkkkk, ho_RegionErosion, ho_RegionUnionkkkkk, ho_RegionSel, ho_RegionFillUp1, ho_RegionOpening1, ho_EdgeAmplitude, ho_Regionk, ho_ConnectedRegions1, ho_SelectedRegions, ho_RegionUnion, ho_RegionClosing;
                HTuple hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2, hv_Rectangularity, hv_Number, NChannel, hv_Rowqqq, hv_Colqqq, hv_Phiqqq, hv_Length1qqq, hv_Length2qqq, hv_Recties, hv_Rowppp, hv_Colppp, hv_Phippp, hv_Length1ppp, hv_Length2ppp, hv_Recty, hv_N, hv_Num;

                //*一、产品定位
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    //HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                    HOperatorSet.Rgb1ToGray(hoReduced, out ho_ImageReduced);
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_ImageReduced, 1, 1);
                }

                HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionLight, iMainThres, 255);  //参数1 ：总定位阈值iMainThres   电极定位阈值40
                HOperatorSet.OpeningCircle(ho_RegionLight, out ho_RegionOpening, iMainOpening);           //参数2 ：总定位开运算iMainOpening = 5
                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_RegionBig, "max_area", 70);
                HOperatorSet.ClosingCircle(ho_RegionBig, out RegionClosing, iMainClosing);               //参数3 ：总定位闭运算iMainClosing = 10
                HOperatorSet.FillUp(RegionClosing, out RegionFillUp);

                HOperatorSet.SelectShape(RegionFillUp, out ho_SelectedRegions, "area",         //参数4 ：电极过滤面积iMainFilterArea 20000-50000
"and", iMainFilterArea, iMainFilterArea + 30000);

                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleNotEqual(1))) != 0)
                {
                    listObj2Draw[1] = "NG-无定位";
                    //绘制电极轮廓  ho_RegionFillUp1
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_SelectedRegions, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "D:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }


                    return listObj2Draw;
                }

                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionOpening, out ho_ImageReduced2);

                //*二、检查尺寸
                HOperatorSet.Rectangularity(ho_SelectedRegions, out hv_Rectangularity);

                HTuple HRectyThres = iRectyThres;                                       //参数5：矩形度 iRectyThres
                if ((int)(new HTuple(hv_Rectangularity.TupleLess(HRectyThres))) != 0)
                {
                    listObj2Draw[1] = "NG-封端前瓷损";
                    //绘制电极轮廓  ho_RegionFillUp1
                    syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                    //输出NG详情
                    lsInfo2Draw.Add("矩形度:" + hv_Rectangularity.D.ToString("0.0"));
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "D:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }

                    return listObj2Draw;
                }

                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);

                //80 * 80  + - 15
                if ((int)((new HTuple((new HTuple((new HTuple(hv_Length1.TupleLess(iSideLength - iSideLengthScale))).TupleOr(
                    new HTuple(hv_Length1.TupleGreater(iSideLength + iSideLengthScale))))).TupleOr(new HTuple(hv_Length2.TupleLess(
                    iSideLength - iSideLengthScale))))).TupleOr(new HTuple(hv_Length2.TupleGreater(iSideLength + iSideLengthScale)))) != 0)

                //参数6： 边长 iSideLength    
                //参数7： 边长变化iSideLengthScale

                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-封端前瓷损";
                    //绘制电极轮廓  ho_RegionFillUp1
                    syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                    //输出NG详情
                    lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + "*" + hv_Length2.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));


                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "C:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }

                    return listObj2Draw;
                }

                //检查封端瓷损
                HObject Cnnonnnnn, RegionDiff, DiffOpen, RegionCisun, RegionCisun1;
                HTuple CisunNum;
                HOperatorSet.Difference(ho_Rectangle1, ho_SelectedRegions, out RegionDiff);
                HOperatorSet.OpeningCircle(RegionDiff, out DiffOpen, iCisunOpening);  //4    //参数8 ：封端瓷损开运算iCisunOpening = 4
                HOperatorSet.Connection(DiffOpen, out Cnnonnnnn);
                HOperatorSet.SelectShape(Cnnonnnnn, out RegionCisun, "area", "and", iMinCisunarea, 9999999);             //参数9：瓷损面积 iMinCisunarea    1200
                HOperatorSet.SelectShape(RegionCisun, out RegionCisun1, "inner_radius", "and", iMinCisunradis, 9999999); //参数10 ：瓷损半径iMinCisunradis  瓷损半径10
                HOperatorSet.CountObj(RegionCisun1, out CisunNum);
                if (CisunNum > 0)
                {
                    listObj2Draw[1] = "NG-封端前瓷损";
                    //绘制电极轮廓  ho_RegionFillUp1
                    hv_Num = 0;
                    HOperatorSet.CountObj(RegionCisun1, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(RegionCisun1, out ho_RegionSel, i);
                        syShowRegionBorder(RegionCisun1, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    HObject UnionCisun;
                    HTuple AreaCisun, aaaaaa, bbbbbb;
                    HOperatorSet.Union1(RegionCisun1, out UnionCisun);
                    HOperatorSet.AreaCenter(UnionCisun, out AreaCisun, out aaaaaa, out bbbbbb);

                    lsInfo2Draw.Add("最小半径:" + iMinCisunradis.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    //向硬盘写入NG图片
                    if (iCameraCode == 1)
                    {
                        DDDDD++;
                        if (bUseSaveMutex) muDetect21.WaitOne();
                        HOperatorSet.WriteImage(hoImage, "jpeg", -1, "D:/imagesave/" + DDDDD.ToString() + ".jpg");
                    }
                    return listObj2Draw;
                }

                //检查端头内部损伤(黑缺陷)
                HObject ImageDT, RegionDark, RegionDarkCnn, RegionGS, RectangleEro;
                HTuple Num;
                HOperatorSet.ErosionCircle(ho_Rectangle1, out RectangleEro, iEroCheckGS);  //参数11 ： 黑缺陷检查腐蚀半径 iEroCheckGS = 15
                HOperatorSet.ReduceDomain(ho_ImageReduced, RectangleEro, out ImageDT);
                HOperatorSet.Threshold(ImageDT, out RegionDark, 0, iGSThres);              // 参数12 ：刮伤阈值 iGSThres  20
                HOperatorSet.Connection(RegionDark, out RegionDarkCnn);
                HOperatorSet.SelectShape(RegionDarkCnn, out RegionGS, "area", "and", iGSArea, 9999999);  //参数13：刮伤面积iGSArea 200
                HOperatorSet.CountObj(RegionGS, out Num);
                if (Num > 0)
                {
                    listObj2Draw[1] = "NG-封端刮伤";
                    //绘制电极轮廓  RegionGS
                    hv_Num = 0;
                    HOperatorSet.CountObj(RegionGS, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(RegionGS, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    lsInfo2Draw.Add("面积上限:" + iGSArea.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("阈值上限:" + iGSThres.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                }



                //HTuple Mean, Deviation;
                //HOperatorSet.Intensity(RectangleEro, ImageDT, out Mean, out Deviation);  //mean 90
                //if (Mean < iFengduanMean)
                //{
                //    listObj2Draw[1] = "NG-封端刮伤";
                //    lsInfo2Draw.Add("平均灰度:" + Mean.D.ToString("0.0"));
                //    lsInfo2Draw.Add("NG");
                //    listObj2Draw.Add("字符串");
                //    listObj2Draw.Add(lsInfo2Draw);
                //    listObj2Draw.Add(new PointF(1800, 100));
                //    return listObj2Draw;
                //}













                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                //执行到这里，OK  绘制外接矩形
                listObj2Draw[1] = "OK";
                List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcode.ToArray());
                listObj2Draw.Add("OK");

                //GC.Collect();

                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect12", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect21.ReleaseMutex();
            }
            #endregion
        }



        ////白基板算法
        //public static List<object> sySixSideDetect888(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        //{

        //    #region  *** 六面机 正反面  ***

        //    string[] strUserParam = strParams.Split('#');
        //    int iWorkStation = int.Parse(strUserParam[4]); //iWorkStation
        //    if (iWorkStation == 40)  //4HAOGONGWEI  
        //    {
        //        //listObj2Draw[1] = "OK";
        //        //return listObj2Draw;
        //        //return sySixSideDetect8_old(hoImage, lkkPolygon, strParams);
        //    }

        //    if (bUseMutex) muDetect8.WaitOne();

        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    List<object> listObj2Draw = new List<object>();
        //    //添加元素
        //    listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

        //    try
        //    {
        //        HObject hoReduced = null, hoConcate = null, hoRegion = null, hoUnion = null, ho_RegionSel = null, hoRegionsConn = null, hoSelectedRegions = null, ho_Rectangle = null, ho_ImageReduce = null, ho_RectangleDia = null, ho_Edges = null, ho_ShortEdges = null;
        //        HObject ho_RegionLight = null, ho_Err_RegionConn = null, ho_RegionConn = null, Rectbbb = null, ho_EdgeAmp1 = null, ho_RegionLie = null, ho_RegionLies = null, ho_ImageMean = null, ho_DarkPix = null;
        //        HObject ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionUnion = null, ho_RegionClose = null, ho_Regionpen = null, ho_RegionTrans = null, ho_RegionDiff = null, ho_ImageReduce1 = null, ho_RegionDark = null, ho_ImageAmp = null;

        //        HTuple NChannel, hv_Num, hv_Length1, hv_Length2, Rowbbb, Colbbb, Phibbb, Length1bbb, Length2bbb;
        //        HTuple hv_Row, hv_Column, hv_Phi, hv_Area, hv_Mean, hv_Dev, hv_Row111, hv_Column111, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD;

        //        List<string> lsInfo2Draw = new List<string>();

        //        #region ****** 生成区域ROI  ******

        //        HOperatorSet.GenEmptyObj(out hoConcate);
        //        for (int igg = 0; igg < lkkPolygon.Count; igg++)
        //        {
        //            if (lkkPolygon[igg][0].X == 3)
        //            {
        //                PointF pgg1 = lkkPolygon[igg][1];
        //                PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
        //                double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

        //                HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
        //                HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
        //            }
        //            else if (lkkPolygon[igg][0].X == 8)
        //            {
        //                PointF pgg1 = lkkPolygon[igg][1];
        //                PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

        //                HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
        //                HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
        //            }
        //            else
        //            {
        //                PointF pgg1 = lkkPolygon[igg][1];
        //                PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

        //                HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
        //                HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
        //            }
        //        }

        //        HOperatorSet.Union1(hoConcate, out hoUnion);
        //        HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

        //        #endregion

        //        //读取参数
        //        //string[] strUserParam = strParams.Split('#');
        //        //int iWorkStation = int.Parse(strUserParam[4]) ; //iWorkStation




        //        HTuple hv_Number = 0;
        //        HTuple hv_Number1 = 0;
        //        HTuple hv_Number2 = 0;

        //        int hv_X = int.Parse(strUserParam[5]);  
        //        int hv_thr1 = int.Parse(strUserParam[6]);  
        //        int hv_thr2 = int.Parse(strUserParam[7]);  
        //        int hv_area1 = int.Parse(strUserParam[8]);  
        //        int hv_area2 = int.Parse(strUserParam[9]);  
        //        int hv_mean1 = int.Parse(strUserParam[10]); 
        //        int hv_mean2 = int.Parse(strUserParam[11]); 
        //        int hv_thr3 = int.Parse(strUserParam[12]); 
        //        int hv_thr4 = int.Parse(strUserParam[13]); 
        //        int hv_area3 = int.Parse(strUserParam[14]); 
        //        int hv_area4 = int.Parse(strUserParam[15]); 
        //        int hv_mean3 = int.Parse(strUserParam[16]); 
        //        int hv_mean4 = int.Parse(strUserParam[17]); 
        //        int hv_widthmax = int.Parse(strUserParam[18]);
        //        int hv_conture = int.Parse(strUserParam[19]);
        //        int hv_lenthmin = int.Parse(strUserParam[20]);

        //        HObject ho_F01 = null, ho_Rectangle8 = null, ho_ImageReduced2 = null;
        //        HObject ho_Image = null, ho_Regions0 = null, ho_ConnectedRegions0 = null;
        //        HObject ho_SelectedRegions0 = null, ho_SelectedRegions01 = null;
        //        HObject ho_Regions1 = null, ho_ConnectedRegions1 = null;
        //        HObject ho_SelectedRegions1 = null, ho_SelectedRegions11 = null;
        //        HObject ho_Image2 = null, ho_Lines = null, ho_Polygons = null;
        //        HObject ho_SplitContours = null, ho_SelectedContours = null;
        //        HObject ho_SelectedContours2 = null, ho_UnionContours = null;
        //        HObject ho_SelectedContours1 = null, ho_MinLine1 = null, ho_MaxLine1 = null, ho_XLDSel;

        //        HTuple hv_Sigma1 , hv_Low1;






        //        //判断正反面
        //        if (hv_X == 1)
        //        {
        //            #region ---- *** 正面处理  *** ----


        //            HOperatorSet.Threshold(hoReduced, out ho_Regions0, hv_thr1, hv_thr2);

        //            HOperatorSet.Connection(ho_Regions0, out ho_ConnectedRegions0);

        //            HOperatorSet.SelectShape(ho_ConnectedRegions0, out ho_SelectedRegions0, "area",
        //                "and", hv_area1, hv_area2);

        //            HOperatorSet.SelectGray(ho_SelectedRegions0, hoReduced, out ho_SelectedRegions01,
        //                "mean", "and", hv_mean1, hv_mean2);
        //            HOperatorSet.CountObj(ho_SelectedRegions01, out hv_Number2);

        //            //area_center (SelectedRegions01, Area, Row, Column)
        //            if (hv_Number2 == 0)
        //            {


        //                ////孔洞检测   （正面检）

        //                ho_Image1 = hoReduced.CopyObj(1, -1);

        //                HOperatorSet.Threshold(ho_Image1, out ho_Regions1, hv_thr3, hv_thr4);

        //                HOperatorSet.Connection(ho_Regions1, out ho_ConnectedRegions1);

        //                HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1,
        //                    "area", "and", hv_area3, hv_area4);

        //                HOperatorSet.SelectGray(ho_SelectedRegions1, hoImage, out ho_SelectedRegions11,
        //                    "mean", "and", hv_mean3, hv_mean4);
        //                HOperatorSet.CountObj(ho_SelectedRegions11, out hv_Number1);

        //            }
        //            #endregion


        //        }
        //        else if (hv_X == 2)
        //        {
        //            #region ---- *** 反面处理  *** ----

        //            HOperatorSet.Threshold(hoReduced, out ho_Regions0, hv_thr1, hv_thr2);

        //            HOperatorSet.Connection(ho_Regions0, out ho_ConnectedRegions0);

        //            HOperatorSet.SelectShape(ho_ConnectedRegions0, out ho_SelectedRegions0, "area",
        //                "and", hv_area1, hv_area2);

        //            HOperatorSet.SelectGray(ho_SelectedRegions0, hoImage, out ho_SelectedRegions01,
        //                "mean", "and", hv_mean1, hv_mean2);
        //            HOperatorSet.CountObj(ho_SelectedRegions01, out hv_Number2);

        //           // if (hv_Number2 == 0)
        //           // {

        //            dhDll.frmMsg.Log("111111111111111111111111111111111111" + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0 );

        //                HTuple hv_High1, hv_pi, hv_Eps;

        //                calculate_lines_gauss_parameters(hv_widthmax, hv_conture, out hv_Sigma1,
        //                    out hv_Low1, out hv_High1);

        //                HOperatorSet.LinesGauss(hoReduced, out ho_Lines, hv_Sigma1, hv_Low1, hv_High1,
        //                    "dark", "true", "bar-shaped", "true");

        //                HOperatorSet.GenPolygonsXld(ho_Lines, out ho_Polygons, "ramer", 1);

        //                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
        //                    1, 5);
        //                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
        //                hv_Eps = hv_pi / 36;

        //                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
        //                    "direction", 0 - hv_Eps, 0 + hv_Eps, -0.5, 0.5);
        //                //选择特定长度的线段
        //                //select_contours_xld (SelectedContours, SelectedContours2, 'contour_length', 100, 99900, -0.5, 0.5)

        //                HOperatorSet.UnionCollinearContoursXld(ho_SelectedContours, out ho_UnionContours,
        //                    30, 1, 2, 0.1, "attr_keep");

        //                HOperatorSet.SelectContoursXld(ho_UnionContours, out ho_SelectedContours1,
        //                    "direction", 0 - hv_Eps, 0 + hv_Eps, -0.5, 0.5);

        //                select_min_max_length_contour(ho_SelectedContours1, out ho_MinLine1, out ho_MaxLine1     );


        //                HOperatorSet.SelectContoursXld(ho_MaxLine1, out ho_SelectedContours2, "contour_length",
        //                    hv_lenthmin, 99900, -0.5, 0.5);
        //                HOperatorSet.CountObj(ho_SelectedContours2, out hv_Number);

        //                dhDll.frmMsg.Log("kkkkkkkkkkkkkkkkkkkkkkkkkkkk" + sw.ElapsedMilliseconds.ToString() + "hv_Number = " + hv_Number, "", null, dhDll.logDiskMode.Error, 0);


        //            //length_xld (SelectedContours2, Length)
        //           // }

        //            #endregion



        //        }

        //        else
        //        {

        //        }




        //        //斑点缺陷
        //        if (hv_Number2 != 0)
        //        {
        //            listObj2Draw[1] = "NG";
        //            //hv_Num = hv_Number2;
        //            HOperatorSet.CountObj(ho_SelectedRegions01, out hv_Num);
        //            for (int i = 1; i <= hv_Num; i++)
        //            {
        //                HOperatorSet.SelectObj(ho_SelectedRegions01, out ho_RegionSel, i);
        //                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
        //            }
        //            return listObj2Draw;
        //        }
        //        //孔洞
        //        if (hv_Number2 == 0 && hv_Number1 != 0)
        //        {
        //            listObj2Draw[1] = "NG";
        //            //hv_Num = hv_Number1;
        //            HOperatorSet.CountObj(ho_SelectedRegions11, out hv_Num);
        //            for (int i = 1; i <= hv_Num; i++)
        //            {
        //                HOperatorSet.SelectObj(ho_SelectedRegions11, out ho_RegionSel, i);
        //                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
        //            }
        //            return listObj2Draw;
        //        }

        //        //微裂X
        //        if ( hv_Number != 0)
        //        {
        //            listObj2Draw[1] = "NG";
        //            //hv_Num = hv_Number1;
        //            HOperatorSet.CountObj(ho_SelectedContours2, out hv_Num);
        //            for (int i = 1; i <= hv_Num; i++)
        //            {
        //                HOperatorSet.SelectObj(hoSelectedRegions, out ho_XLDSel, i);
        //                syShowXLD(ho_XLDSel, ref listObj2Draw, "NG");
        //                //syShowXLD();
        //            }
        //            return listObj2Draw;
        //        }


        //        listObj2Draw[1] = "OK";
        //        //hv_Num = 0;
        //        //HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
        //        //for (int i = 1; i <= hv_Num; i++)
        //        //{
        //         //   HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
        //         //   syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
        //      //  }


        //        return listObj2Draw;




        //    }
        //    catch (Exception exc)
        //    {
        //        listObj2Draw[1] = "NG-程序出错";
        //        dhDll.frmMsg.Log("sySixSideDetect8", "", exc, dhDll.logDiskMode.Error, 0);
        //        return listObj2Draw;
        //    }
        //    finally
        //    {
        //        sw.Stop();
        //        if (bUseMutex) muDetect8.ReleaseMutex();
        //    }
        //    #endregion
        //}


        //白基板算法
        public static List<object> sySixSideDetect888888888888888888(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 白基板正背面 ***
            if (bUseMutex) muDetect12.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoRegion, hoReduced, hoConcate, hoUnion;
                HObject hoEmphasize, ho_BlackReg, ho_DarkPix;
                HObject ho_RegionSelect, ho_RegionDiff, ho_ImageMean;
                HObject ho_RegionOpen, ho_RegionConnect, ho_SortedRegion, ho_RegionFill, ho_ImageMaxReduce, ho_RegionConn, ho_RegionSelect1, ho_ImageReduce, ho_ImageClosing, ho_Rectangle, ho_RectangleEro;
                HObject ho_RegionLight, ho_RegionLights, ho_BiggestRegion, ho_RegionEro, ho_ImageCheck, EdgeAmplitude, ho_Err_RegionConn, ho_RegionSel;
                HTuple hv_Rectangularity; HObject ho_ConnectedRegionOpening;
                //HTuple hv_Area, hv_Col, hv_Phi, hv_Length1, hv_Length2, hv_Row1, hv_Col1, Areapppp, Rowpppp, Colpppp;
                HTuple hv_MaxIndex, hv_cmp, hv_I, hv_AreaSelect, hv_Row, hv_Column, Areakkk, Rowkkk, Colkkk, hv_Num;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);


                #endregion

                string[] strUserParam = strParams.Split('#');

                int CornerErrArea = int.Parse(strUserParam[4]);                //缺角面积 2000
                int hv_is_mark = int.Parse(strUserParam[5]);                   //是否有边缘mark 0：没有，1：有
                int hv_mark_area = int.Parse(strUserParam[6]);                 //边缘mark最大面积 1200
                int hv_edge_err_area = int.Parse(strUserParam[7]);             //边缘缺损最大面积 200


                HTuple hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                HTuple hv_Eps = hv_pi / 36;


                HObject ho_ImageFFTY, ho_Rectangle3, ho_Rectangle4, ho_ImageFFTX, ho_ImageResult, ho_ImageFFT2, ho_RegionUnion, ho_Rectangle1, ho_Rectangle2, ho_Image, ho_Rectangle8, ho_Imagerota, ho_Cross1, ho_Cross2, ho_MaxLine1, ho_MinLine1, ho_Edges, ho_ImageReduced11111111, ho_Rectangle1111111, ho_RegionErrDConn, ho_RegionErrD, ho_ConnectedRegionDark, ho_RegionDark, ho_ImageReduced2, ho_RegionErosion, ho_RegionOpening2, ho_RectPu, ho_RegionOpening, ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionBinary, ho_ConnectedRegions, ho_MaxRegion, ho_RegionFillUp;
                HTuple hv_Width1, hv_Height1, hv_Number, NChannel, hv_UsedThreshold, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist;

                //判断彩色还是黑白
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_Image, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_Image, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                HObject ho_ContourD, ho_CrossD, ho_CrossU, ho_ContourL, ho_CrossL, ho_ContourR, ho_CrossR, ho_ContourU;

                #region######## 1、寻边定位########



                HTuple hv_shapeParam = new HTuple();
                hv_shapeParam[0] = 1225;
                hv_shapeParam[1] = 956;
                hv_shapeParam[2] = 2330;
                hv_shapeParam[3] = 956;
                caliper_single_edge(ho_Image, out ho_ContourL, out ho_CrossL, hv_shapeParam, 100, 1, 300, 1, 100, "first", 0.2);

                hv_shapeParam[0] = 1060;
                hv_shapeParam[1] = 4587;
                hv_shapeParam[2] = 2636;
                hv_shapeParam[3] = 4587;
                caliper_single_edge(ho_Image, out ho_ContourR, out ho_CrossR, hv_shapeParam, 100, 1, 300, 1, 100, "last", 0.2);

                hv_shapeParam[0] = 215;
                hv_shapeParam[1] = 1656;
                hv_shapeParam[2] = 215;
                hv_shapeParam[3] = 3933;
                caliper_single_edge(ho_Image, out ho_ContourU, out ho_CrossU, hv_shapeParam, 100, 1, 300, 1, 100, "last", 0.2);

                hv_shapeParam[0] = 3317;
                hv_shapeParam[1] = 1643;
                hv_shapeParam[2] = 3317;
                hv_shapeParam[3] = 3900;
                caliper_single_edge(ho_Image, out ho_ContourD, out ho_CrossD, hv_shapeParam, 100, 1, 300, 1, 100, "first", 0.2);

                HTuple hv_RowBeginD, hv_ColBeginD, hv_RowEndD, hv_ColEndD, hv_RowBeginU, hv_ColBeginU, hv_RowEndU, hv_ColEndU, hv_RowBeginL, hv_ColBeginL, hv_RowEndL, hv_ColEndL, hv_RowBeginR, hv_ColBeginR, hv_RowEndR, hv_ColEndR;

                HOperatorSet.FitLineContourXld(ho_ContourL, "tukey", -1, 0, 5, 2, out hv_RowBeginL,
                    out hv_ColBeginL, out hv_RowEndL, out hv_ColEndL, out hv_Nr, out hv_Nc,
                    out hv_Dist);

                //绘制左边缘线
                RectangleF rectLine1 = new RectangleF((float)hv_ColBeginL.D, (float)hv_RowBeginL.D, (float)hv_ColEndL.D, (float)hv_RowEndL.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                HOperatorSet.FitLineContourXld(ho_ContourR, "tukey", -1, 0, 5, 2, out hv_RowBeginR,
                    out hv_ColBeginR, out hv_RowEndR, out hv_ColEndR, out hv_Nr, out hv_Nc,
                    out hv_Dist);

                //绘制右边缘线
                RectangleF rectLine2 = new RectangleF((float)hv_ColBeginR.D, (float)hv_RowBeginR.D, (float)hv_ColEndR.D, (float)hv_RowEndR.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");

                HOperatorSet.FitLineContourXld(ho_ContourU, "tukey", -1, 0, 5, 2, out hv_RowBeginU,
                    out hv_ColBeginU, out hv_RowEndU, out hv_ColEndU, out hv_Nr, out hv_Nc,
                    out hv_Dist);

                //绘制上边缘线
                RectangleF rectLine3 = new RectangleF((float)hv_ColBeginU.D, (float)hv_RowBeginU.D, (float)hv_ColEndU.D, (float)hv_RowEndU.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");

                HOperatorSet.FitLineContourXld(ho_ContourD, "tukey", -1, 0, 5, 2, out hv_RowBeginD,
                    out hv_ColBeginD, out hv_RowEndD, out hv_ColEndD, out hv_Nr, out hv_Nc,
                    out hv_Dist);

                //绘制下边缘线
                RectangleF rectLine4 = new RectangleF((float)hv_ColBeginD.D, (float)hv_RowBeginD.D, (float)hv_ColEndD.D, (float)hv_RowEndD.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");

                HObject ho_LineL, ho_LineR, ho_LineU, ho_LineD;


                HOperatorSet.GenContourPolygonXld(out ho_LineL, hv_RowBeginL.TupleConcat(
                    hv_RowEndL), hv_ColBeginL.TupleConcat(hv_ColEndL));
                HOperatorSet.GenContourPolygonXld(out ho_LineR, hv_RowBeginR.TupleConcat(
                    hv_RowEndR), hv_ColBeginR.TupleConcat(hv_ColEndR));
                HOperatorSet.GenContourPolygonXld(out ho_LineU, hv_RowBeginU.TupleConcat(
                    hv_RowEndU), hv_ColBeginU.TupleConcat(hv_ColEndU));
                HOperatorSet.GenContourPolygonXld(out ho_LineD, hv_RowBeginD.TupleConcat(
                    hv_RowEndD), hv_ColBeginD.TupleConcat(hv_ColEndD));

                HTuple hv_RowLU, hv_ColLU, hv_IsOverlapping, hv_RowLD, hv_ColLD, hv_RowRD, hv_ColRD, hv_RowRU, hv_ColRU;

                HOperatorSet.IntersectionLines(hv_RowBeginL, hv_ColBeginL, hv_RowEndL, hv_ColEndL,
                    hv_RowBeginU, hv_ColBeginU, hv_RowEndU, hv_ColEndU, out hv_RowLU, out hv_ColLU,
                    out hv_IsOverlapping);
                HOperatorSet.IntersectionLines(hv_RowBeginL, hv_ColBeginL, hv_RowEndL, hv_ColEndL,
                    hv_RowBeginD, hv_ColBeginD, hv_RowEndD, hv_ColEndD, out hv_RowLD, out hv_ColLD,
                    out hv_IsOverlapping);
                HOperatorSet.IntersectionLines(hv_RowBeginR, hv_ColBeginR, hv_RowEndR, hv_ColEndR,
                    hv_RowBeginD, hv_ColBeginD, hv_RowEndD, hv_ColEndD, out hv_RowRD, out hv_ColRD,
                    out hv_IsOverlapping);
                HOperatorSet.IntersectionLines(hv_RowBeginR, hv_ColBeginR, hv_RowEndR, hv_ColEndR,
                    hv_RowBeginU, hv_ColBeginU, hv_RowEndU, hv_ColEndU, out hv_RowRU, out hv_ColRU,
                    out hv_IsOverlapping);

                HObject ho_CrossLU, ho_CrossLD, ho_CrossRD, ho_CrossRU;

                HOperatorSet.GenCrossContourXld(out ho_CrossLU, hv_RowLU, hv_ColLU, 100,
                    0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossLD, hv_RowLD, hv_ColLD, 100,
                    0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossRD, hv_RowRD, hv_ColRD, 100,
                    0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossRU, hv_RowRU, hv_ColRU, 100,
                    0.785398);

                HObject ho_RegionPu, ho_RegionPuEro, ho_ImagePu, ho_ContourPu, ho_Polygons;

                HTuple hv_ero_radis = 5;

                HOperatorSet.GenRegionPolygonFilled(out ho_RegionPu, ((((hv_RowLU.TupleConcat(
                    hv_RowLD))).TupleConcat(hv_RowRD))).TupleConcat(hv_RowRU), ((((hv_ColLU.TupleConcat(
                    hv_ColLD))).TupleConcat(hv_ColRD))).TupleConcat(hv_ColRU));


                HOperatorSet.ErosionRectangle1(ho_RegionPu, out ho_RegionPuEro, hv_ero_radis, hv_ero_radis);
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionPuEro, out ho_ImagePu);

                HTuple hv_RowKKK, hv_ColKKK, hv_LengthKKK, hv_PhiKKK;

                HOperatorSet.GenContourRegionXld(ho_RegionPuEro, out ho_ContourPu, "border");
                HOperatorSet.GenPolygonsXld(ho_ContourPu, out ho_Polygons, "ramer", 2);
                HOperatorSet.GetPolygonXld(ho_Polygons, out hv_RowKKK, out hv_ColKKK, out hv_LengthKKK, out hv_PhiKKK);

                #endregion

                HTuple hv_Column1, hv_Column2, hv_Column3, hv_Column4, hv_Row1, hv_Row2, hv_Row3, hv_Row4, hv_Area1, hv_Area2, hv_Area3, hv_Area4;
                HObject ho_RegionFillUp1, ho_RegionFillUp2, ho_RegionFillUp3, ho_RegionFillUp4, ho_RegionLight1, ho_RegionLight2, ho_RegionLight3, ho_RegionLight4, ho_ImageReduced1, ho_ImageReduced3, ho_ImageReduced4, ho_Circle1, ho_Circle2, ho_Circle3, ho_Circle4, ho_RegionIntersection1, ho_RegionIntersection2, ho_RegionIntersection3, ho_RegionIntersection4;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2;

                //int CornerErrCode = 0;
                int CornerErrCode1 = 0;
                int CornerErrCode2 = 0;
                int CornerErrCode3 = 0;
                int CornerErrCode4 = 0;
                int EdgeErrCode = 0;

                HTuple hv_circle_radis = 150; //缺角检测半径




                #region ######## 2、检查缺角########a
                //1、检查缺角
                HOperatorSet.GenCircle(out ho_Circle1, hv_RowLU, hv_ColLU, hv_circle_radis);
                HOperatorSet.Intersection(ho_Circle1, ho_RegionPuEro, out ho_RegionIntersection1);
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionIntersection1, out ho_ImageReduced1);
                HOperatorSet.Threshold(ho_ImageReduced1, out ho_RegionLight1, 250, 255);
                HOperatorSet.FillUp(ho_RegionLight1, out ho_RegionFillUp1);
                HOperatorSet.AreaCenter(ho_RegionFillUp1, out hv_Area1, out hv_Row1, out hv_Column1);
                if ((int)(new HTuple(hv_Area1.TupleGreater(CornerErrArea))) != 0)
                {
                    //把缺角区域框出来
                    HOperatorSet.SmallestRectangle2(ho_RegionFillUp1, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                    List<PointF> lnBarcode1 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcode1.ToArray());
                    listObj2Draw.Add("NG");

                    CornerErrCode1++;
                }

                HOperatorSet.GenCircle(out ho_Circle2, hv_RowRU, hv_ColRU, hv_circle_radis);
                HOperatorSet.Intersection(ho_Circle2, ho_RegionPuEro, out ho_RegionIntersection2);
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionIntersection2, out ho_ImageReduced2);
                HOperatorSet.Threshold(ho_ImageReduced2, out ho_RegionLight2, 250, 255);
                HOperatorSet.FillUp(ho_RegionLight2, out ho_RegionFillUp2);
                HOperatorSet.AreaCenter(ho_RegionFillUp2, out hv_Area2, out hv_Row2, out hv_Column2);
                if ((int)(new HTuple(hv_Area2.TupleGreater(CornerErrArea))) != 0)
                {
                    //把缺角区域框出来
                    HOperatorSet.SmallestRectangle2(ho_RegionFillUp2, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                    List<PointF> lnBarcode2 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcode2.ToArray());
                    listObj2Draw.Add("NG");
                    CornerErrCode2++;
                }

                HOperatorSet.GenCircle(out ho_Circle3, hv_RowRD, hv_ColRD, hv_circle_radis);
                HOperatorSet.Intersection(ho_Circle3, ho_RegionPuEro, out ho_RegionIntersection3);
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionIntersection3, out ho_ImageReduced3);
                HOperatorSet.Threshold(ho_ImageReduced3, out ho_RegionLight3, 250, 255);
                HOperatorSet.FillUp(ho_RegionLight3, out ho_RegionFillUp3);
                HOperatorSet.AreaCenter(ho_RegionFillUp3, out hv_Area3, out hv_Row3, out hv_Column3);
                if ((int)(new HTuple(hv_Area3.TupleGreater(CornerErrArea))) != 0)
                {
                    //把缺角区域框出来
                    HOperatorSet.SmallestRectangle2(ho_RegionFillUp3, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                    List<PointF> lnBarcode3 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcode3.ToArray());
                    listObj2Draw.Add("NG");
                    CornerErrCode3++;
                }

                HOperatorSet.GenCircle(out ho_Circle4, hv_RowLD, hv_ColLD, hv_circle_radis);
                HOperatorSet.Intersection(ho_Circle4, ho_RegionPuEro, out ho_RegionIntersection4);
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionIntersection4, out ho_ImageReduced4);
                HOperatorSet.Threshold(ho_ImageReduced4, out ho_RegionLight4, 250, 255);
                HOperatorSet.FillUp(ho_RegionLight4, out ho_RegionFillUp4);
                HOperatorSet.AreaCenter(ho_RegionFillUp4, out hv_Area4, out hv_Row4, out hv_Column4);
                if ((int)(new HTuple(hv_Area4.TupleGreater(CornerErrArea))) != 0)
                {
                    //把缺角区域框出来
                    HOperatorSet.SmallestRectangle2(ho_RegionFillUp4, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                    List<PointF> lnBarcode4 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcode4.ToArray());
                    listObj2Draw.Add("NG");
                    CornerErrCode4++;
                }

                #endregion

                HObject ho_RegionEdgeErr, ho_ImageUnionEdge, ho_RegionUnionEdge, ho_RegionInterL, ho_RegionInterR, ho_RegionInterU, ho_RegionInterD, ho_RegionErosionLEdge, ho_RegionErosionREdge, ho_RegionErosionUEdge, ho_RegionErosionDEdge, ho_RegionDilationLEdge, ho_RegionDilationREdge, ho_RegionDilationUEdge, ho_RegionDilationDEdge, ho_LEdge, ho_REdge, ho_UEdge, ho_DEdge, ho_RegionLEdge, ho_RegionREdge, ho_RegionUEdge, ho_RegionDEdge;
                HTuple hv_AreaEdgeErr, hv_RowEdgeErr, hv_ColumnEdgeErr;

                #region ######## 3、检查崩边########
                //2、检查崩边
                HOperatorSet.GenContourPolygonXld(out ho_LEdge, hv_RowLU.TupleConcat(hv_RowLD),
                    hv_ColLU.TupleConcat(hv_ColLD));
                HOperatorSet.GenRegionContourXld(ho_LEdge, out ho_RegionLEdge, "filled");
                HOperatorSet.DilationRectangle1(ho_RegionLEdge, out ho_RegionDilationLEdge,
                    50, 1);
                HOperatorSet.ErosionRectangle1(ho_RegionDilationLEdge, out ho_RegionErosionLEdge,
                    1, hv_circle_radis * 2);
                HOperatorSet.Intersection(ho_RegionErosionLEdge, ho_RegionPuEro, out ho_RegionInterL);
                //*�ұ�
                HOperatorSet.GenContourPolygonXld(out ho_REdge, hv_RowRU.TupleConcat(hv_RowRD),
                    hv_ColRU.TupleConcat(hv_ColRD));
                HOperatorSet.GenRegionContourXld(ho_REdge, out ho_RegionREdge, "filled");
                HOperatorSet.DilationRectangle1(ho_RegionREdge, out ho_RegionDilationREdge,
                    50, 1);
                HOperatorSet.ErosionRectangle1(ho_RegionDilationREdge, out ho_RegionErosionREdge,
                    1, hv_circle_radis * 2);
                HOperatorSet.Intersection(ho_RegionErosionREdge, ho_RegionPuEro, out ho_RegionInterR
                    );
                //*�ϱ�
                HOperatorSet.GenContourPolygonXld(out ho_UEdge, hv_RowLU.TupleConcat(hv_RowRU),
                    hv_ColLU.TupleConcat(hv_ColRU));
                HOperatorSet.GenRegionContourXld(ho_UEdge, out ho_RegionUEdge, "filled");
                HOperatorSet.DilationRectangle1(ho_RegionUEdge, out ho_RegionDilationUEdge,
                    1, 50);
                HOperatorSet.ErosionRectangle1(ho_RegionDilationUEdge, out ho_RegionErosionUEdge,
                    hv_circle_radis * 2, 1);
                HOperatorSet.Intersection(ho_RegionErosionUEdge, ho_RegionPuEro, out ho_RegionInterU
                    );
                //*�±�
                HOperatorSet.GenContourPolygonXld(out ho_DEdge, hv_RowLD.TupleConcat(hv_RowRD),
                    hv_ColLD.TupleConcat(hv_ColRD));
                HOperatorSet.GenRegionContourXld(ho_DEdge, out ho_RegionDEdge, "filled");
                HOperatorSet.DilationRectangle1(ho_RegionDEdge, out ho_RegionDilationDEdge,
                    1, 50);
                HOperatorSet.ErosionRectangle1(ho_RegionDilationDEdge, out ho_RegionErosionDEdge,
                    hv_circle_radis * 2, 1);
                HOperatorSet.Intersection(ho_RegionErosionDEdge, ho_RegionPuEro, out ho_RegionInterD
                    );

                HOperatorSet.Union2(ho_RegionInterL, ho_RegionInterR, out ho_RegionUnionEdge);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Union2(ho_RegionUnionEdge, ho_RegionInterU, out ExpTmpOutVar_0);
                    ho_RegionUnionEdge = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Union2(ho_RegionUnionEdge, ho_RegionInterD, out ExpTmpOutVar_0);
                    ho_RegionUnionEdge = ExpTmpOutVar_0;
                }
                HOperatorSet.ReduceDomain(ho_ImagePu, ho_RegionUnionEdge, out ho_ImageUnionEdge);


                HObject ho_SelectedRegions2, ho_ObjectsDiff, ho_ConnectedRegionsErr, ho_SelectedRegions1;
                HTuple hv_Area6, hv_Row6, hv_Column6, hv_Area7, hv_Row7, hv_Column7;

                HOperatorSet.Threshold(ho_ImageUnionEdge, out ho_RegionEdgeErr, 250, 255);
                HOperatorSet.Connection(ho_RegionEdgeErr, out ho_ConnectedRegionsErr);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegionsErr, out ho_SelectedRegions1,
                    "max_area", 70);
                HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area6, out hv_Row6, out hv_Column6);

                if (hv_is_mark == 0) //无mark
                {
                    if ((int)(new HTuple(hv_Area6.TupleGreater(hv_edge_err_area))) != 0)
                    {
                        //把边缘缺损区域框出来
                        HOperatorSet.SmallestRectangle2(ho_SelectedRegions1, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                        List<PointF> lnBarcode5 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode5.ToArray());
                        listObj2Draw.Add("NG");
                        EdgeErrCode++;
                    }

                    hv_Area7 = 0;
                }

                else if (hv_is_mark == 1) //有mark
                {
                    if ((int)(new HTuple(hv_Area6.TupleGreater(hv_mark_area))) != 0)
                    {
                        //把边缘缺损区域框出来
                        HOperatorSet.SmallestRectangle2(ho_SelectedRegions1, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                        List<PointF> lnBarcode6 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode6.ToArray());
                        listObj2Draw.Add("NG");
                        EdgeErrCode++;
                    }
                    HOperatorSet.ObjDiff(ho_ConnectedRegionsErr, ho_SelectedRegions1, out ho_ObjectsDiff);
                    HOperatorSet.SelectShapeStd(ho_ObjectsDiff, out ho_SelectedRegions2, "max_area", 70);
                    HOperatorSet.AreaCenter(ho_SelectedRegions2, out hv_Area7, out hv_Row7, out hv_Column7);
                    if ((int)(new HTuple(hv_Area7.TupleGreater(hv_edge_err_area))) != 0)
                    {
                        //把边缘缺损区域框出来
                        HOperatorSet.SmallestRectangle2(ho_SelectedRegions2, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);
                        List<PointF> lnBarcode7 = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);
                        listObj2Draw.Add("多边形");
                        listObj2Draw.Add(lnBarcode7.ToArray());
                        listObj2Draw.Add("NG");
                        EdgeErrCode++;
                    }
                }
                else
                {

                    hv_Area7 = 0;

                }

                #endregion

                int CornerErrCode = CornerErrCode1 + CornerErrCode2 + CornerErrCode3 + CornerErrCode4;
                if (CornerErrCode + EdgeErrCode != 0)
                {
                    listObj2Draw[1] = "NG-缺损";

                    //输出NG详情
                    if (CornerErrCode != 0)
                    {
                        lsInfo2Draw.Add("4个缺角值:" + hv_Area1.D.ToString("0.0") + "#" + hv_Area2.D.ToString("0.0") + "#" + hv_Area3.D.ToString("0.0") + "#" + hv_Area4.D.ToString("0.0") + " " + " pix ");
                        lsInfo2Draw.Add("NG");
                        lsInfo2Draw.Add("缺角设定值 ：" + CornerErrArea.ToString("0.0") + " pix ");
                        lsInfo2Draw.Add("OK");
                    }

                    if (EdgeErrCode != 0)
                    {
                        if (hv_is_mark == 0) //无mark
                        {
                            lsInfo2Draw.Add("边缘缺损值:" + hv_Area6.D.ToString("0.0") + " pix ");
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("边缘缺损设定值 ：" + hv_edge_err_area.ToString("0.0") + " pix ");
                            lsInfo2Draw.Add("OK");
                        }
                        if (hv_is_mark == 1) //有mark
                        {
                            lsInfo2Draw.Add("mark缺损值:" + hv_Area6.D.ToString("0.0") + " pix ");
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("mark缺损设定值 ：" + hv_mark_area.ToString("0.0") + " pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("边缘缺损值:" + hv_Area7.D.ToString("0.0") + " pix ");
                            lsInfo2Draw.Add("NG");
                            lsInfo2Draw.Add("边缘缺损设定值 ：" + hv_edge_err_area.ToString("0.0") + " pix ");
                            lsInfo2Draw.Add("OK");
                        }
                    }
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                }

#if true

                HOperatorSet.ErosionRectangle1(ho_RegionPuEro, out ho_RegionPuEro, hv_ero_radis * 10, hv_ero_radis * 10);
                HOperatorSet.ReduceDomain(ho_Image, ho_RegionPuEro, out ho_ImagePu);

                //HTuple a, r, c;
                //HOperatorSet.AreaCenter(ho_RegionPuEro,out a, out r , out c );
                //listObj2Draw[1] = "OK" + a.D.ToString("0.0");
                //return listObj2Draw;

                //*滤除剥离线后可能同时滤除裂痕

                HObject ho_SelectedContours3, ho_XLDSel, hv_Length, ho_SelectedContours2, ho_SelectedContours1, ho_UnionContours, ho_SelectedContours, ho_UnionContours3, ho_SelectedXLD3, ho_SelectedXLD4, ho_ImageMean1, ho_RegionDynThresh, ho_RegionClosing, ho_Skeleton1, ho_Contours1;
                //HTuple hv_Length;
                //*检测横向裂痕，使用滤除纵向干扰的图片ImageFFTY
                HOperatorSet.MeanImage(ho_ImagePu, out ho_ImageMean, 50, 1);
                HOperatorSet.MeanImage(ho_ImagePu, out ho_ImageMean1, 50, 20);
                HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageMean1, out ho_RegionDynThresh,
                    2, "dark");
                HOperatorSet.ClosingRectangle1(ho_RegionDynThresh, out ho_RegionClosing,
                    40, 1);
                HOperatorSet.Skeleton(ho_RegionClosing, out ho_Skeleton1);
                HOperatorSet.GenContoursSkeletonXld(ho_Skeleton1, out ho_Contours1, 1, "filter");
                HOperatorSet.SelectShapeXld(ho_Contours1, out ho_SelectedXLD3, "height",
                    "and", 0, 100);
                HOperatorSet.SelectShapeXld(ho_SelectedXLD3, out ho_SelectedXLD4, "width",
                    "and", 20, 99999);
                HOperatorSet.UnionCollinearContoursXld(ho_SelectedXLD4, out ho_UnionContours3,
                    300, 10, 5, 0.1, "attr_keep");
                HOperatorSet.SelectContoursXld(ho_UnionContours3, out ho_SelectedContours,
                    "direction", 0 - hv_Eps, 0 + hv_Eps, -0.5, 0.5);
                //选择特定长度的线段
                //select_contours_xld (SelectedContours, SelectedContours2, 'contour_length', 100, 99900, -0.5, 0.5)
                HOperatorSet.UnionCollinearContoursXld(ho_SelectedContours, out ho_UnionContours,
                    30, 1, 2, 0.1, "attr_keep");
                HOperatorSet.SelectContoursXld(ho_UnionContours, out ho_SelectedContours1,
                    "direction", 0 - hv_Eps, 0 + hv_Eps, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours1, out ho_MinLine1, out ho_MaxLine1
                    );
                HOperatorSet.SelectContoursXld(ho_MaxLine1, out ho_SelectedContours2, "contour_length",
                    200, 9999999, -0.5, 0.5);

                //*检测竖向裂痕，使用滤除纵向干扰的图片ImageFFTY
                HOperatorSet.MeanImage(ho_ImagePu, out ho_ImageMean, 1, 50);
                HOperatorSet.MeanImage(ho_ImagePu, out ho_ImageMean1, 20, 50);
                HOperatorSet.DynThreshold(ho_ImageMean, ho_ImageMean1, out ho_RegionDynThresh,
                    2, "dark");
                HOperatorSet.ClosingRectangle1(ho_RegionDynThresh, out ho_RegionClosing,
                    1, 40);
                HOperatorSet.Skeleton(ho_RegionClosing, out ho_Skeleton1);
                HOperatorSet.GenContoursSkeletonXld(ho_Skeleton1, out ho_Contours1, 1, "filter");
                HOperatorSet.SelectShapeXld(ho_Contours1, out ho_SelectedXLD3, "width",
                    "and", 0, 100);
                HOperatorSet.SelectShapeXld(ho_SelectedXLD3, out ho_SelectedXLD4, "height",
                    "and", 20, 99999);
                HOperatorSet.UnionCollinearContoursXld(ho_SelectedXLD4, out ho_UnionContours3,
                    300, 10, 5, 0.1, "attr_keep");
                HOperatorSet.SelectContoursXld(ho_UnionContours3, out ho_SelectedContours,
                    "direction", 0.5 * hv_pi - hv_Eps, 0.5 * hv_pi + hv_Eps, -0.5, 0.5);
                //选择特定长度的线段
                //select_contours_xld (SelectedContours, SelectedContours2, 'contour_length', 100, 99900, -0.5, 0.5)
                HOperatorSet.UnionCollinearContoursXld(ho_SelectedContours, out ho_UnionContours,
                    30, 1, 2, 0.1, "attr_keep");
                HOperatorSet.SelectContoursXld(ho_UnionContours, out ho_SelectedContours1,
                    "direction", 0.5 * hv_pi - hv_Eps, 0.5 * hv_pi + hv_Eps, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours1, out ho_MinLine1, out ho_MaxLine1
                    );
                HOperatorSet.SelectContoursXld(ho_MaxLine1, out ho_SelectedContours3, "contour_length",
                    200, 9999999, -0.5, 0.5);

                HTuple hv_Number2, hv_Number3;
                //统计裂痕XY
                HOperatorSet.CountObj(ho_SelectedContours2, out hv_Number2);
                HOperatorSet.CountObj(ho_SelectedContours3, out hv_Number3);



                //listObj2Draw[1] = "OK" + hv_Number2.D.ToString("0.0" )+ hv_Number3.D.ToString("0.0" );
                //return listObj2Draw;



                if (hv_Number2 > 0 || hv_Number3 > 0)
                {
                    listObj2Draw[1] = "NG-裂痕XY";
                    hv_Num = 0;

                    HTuple Length;

                    if (hv_Number2 > 0)
                    {
                        HOperatorSet.LengthXld(ho_SelectedContours2, out Length);

                    }
                    else
                    {

                        HOperatorSet.LengthXld(ho_SelectedContours3, out Length);

                    }


                    if (Length > 2500)
                        goto CC;

                    if (hv_Number2 > 0 && hv_Number3 > 0)
                    {
                        HOperatorSet.ConcatObj(ho_SelectedContours2, ho_SelectedContours3, out ho_SelectedContours2);
                    }

                    if (hv_Number2 == 0 && hv_Number3 > 0)
                    {
                        HOperatorSet.CopyObj(ho_SelectedContours3, out ho_SelectedContours2, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                    }


                    HOperatorSet.CountObj(ho_SelectedContours2, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_SelectedContours2, out ho_XLDSel, i);
                        syShowXLD(ho_XLDSel, ref listObj2Draw, "NG");
                    }

                    //输出NG详情
                    lsInfo2Draw.Add("裂痕长度:" + Length.D.ToString("0.0") + " pix ");
                    lsInfo2Draw.Add("NG");

                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                }

#endif
                CC:
                listObj2Draw[1] = "OK";
                return listObj2Draw;

            }

            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect8", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect12.ReleaseMutex();
            }
            #endregion
        }












        // Local procedures 
        public static void caliper_single_edge(HObject ho_Image, out HObject ho_Contour, out HObject ho_Cross,
            HTuple hv_shapeParam, HTuple hv_num_measures, HTuple hv_measure_sigma, HTuple hv_measure_length1,
            HTuple hv_measure_length2, HTuple hv_measure_threshold, HTuple hv_measure_select,
            HTuple hv_min_score)
        {

            // Local iconic variables 

            HObject ho_Contours;

            // Local control variables 

            HTuple hv_MetrologyHandle = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Parameter = new HTuple(), hv_N = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Contour);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            try
            {

                //hv_MetrologyHandle.Dispose();
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                //hv_Index.Dispose();
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    hv_measure_sigma);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    hv_measure_length1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    hv_measure_length2);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    hv_measure_select);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    hv_min_score);
                HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);
                //ho_Contours.Dispose(); //hv_Row.Dispose(); hv_Column.Dispose();
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                //ho_Cross.Dispose();
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                //hv_Parameter.Dispose();
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                //ho_Contour.Dispose();
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                //hv_N.Dispose();
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                }
                HObject a;

                select_min_max_length_contour(ho_Contour, out a, out ho_Contour);

                //ho_Contours.Dispose();

                //hv_MetrologyHandle.Dispose();
                //hv_Width.Dispose();
                //hv_Height.Dispose();
                //hv_Index.Dispose();
                //hv_Row.Dispose();
                //hv_Column.Dispose();
                //hv_Parameter.Dispose();
                //hv_N.Dispose();

                return;
            }
            catch (Exception exc)
            {
                //ho_Contours.Dispose();

                //hv_MetrologyHandle.Dispose();
                //hv_Width.Dispose();
                //hv_Height.Dispose();
                //hv_Index.Dispose();
                //hv_Row.Dispose();
                //hv_Column.Dispose();
                //hv_Parameter.Dispose();
                //hv_N.Dispose();

                dhDll.frmMsg.Log("寻边出错" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);

                throw exc;
            }
        }

        public static void calculate_lines_gauss_parameters(HTuple hv_MaxLineWidth, HTuple hv_Contrast, out HTuple hv_Sigma, out HTuple hv_Low, out HTuple hv_High)
        {
            HTuple hv_ContrastHigh = null, hv_ContrastLow = new HTuple();
            HTuple hv_HalfWidth = null, hv_Help = null;
            HTuple hv_MaxLineWidth_COPY_INP_TMP = hv_MaxLineWidth.Clone();

            // Initialize local and output iconic variables 
            //Check control parameters
            if ((int)(new HTuple((new HTuple(hv_MaxLineWidth_COPY_INP_TMP.TupleLength())).TupleNotEqual(
                1))) != 0)
            {
                //throw new HalconException("Wrong number of values of control parameter: 1");
            }
            if ((int)(((hv_MaxLineWidth_COPY_INP_TMP.TupleIsNumber())).TupleNot()) != 0)
            {
                //throw new HalconException("Wrong type of control parameter: 1");
            }
            if ((int)(new HTuple(hv_MaxLineWidth_COPY_INP_TMP.TupleLessEqual(0))) != 0)
            {
                //throw new HalconException("Wrong value of control parameter: 1");
            }
            if ((int)((new HTuple((new HTuple(hv_Contrast.TupleLength())).TupleNotEqual(1))).TupleAnd(
                new HTuple((new HTuple(hv_Contrast.TupleLength())).TupleNotEqual(2)))) != 0)
            {
                //throw new HalconException("Wrong number of values of control parameter: 2");
            }
            if ((int)(new HTuple(((((hv_Contrast.TupleIsNumber())).TupleMin())).TupleEqual(
                0))) != 0)
            {
                //throw new HalconException("Wrong type of control parameter: 2");
            }
            //Set and check ContrastHigh
            hv_ContrastHigh = hv_Contrast.TupleSelect(0);
            if ((int)(new HTuple(hv_ContrastHigh.TupleLess(0))) != 0)
            {
                //throw new HalconException("Wrong value of control parameter: 2");
            }
            //Set or derive ContrastLow
            if ((int)(new HTuple((new HTuple(hv_Contrast.TupleLength())).TupleEqual(2))) != 0)
            {
                hv_ContrastLow = hv_Contrast.TupleSelect(1);
            }
            else
            {
                hv_ContrastLow = hv_ContrastHigh / 3.0;
            }
            //Check ContrastLow
            if ((int)(new HTuple(hv_ContrastLow.TupleLess(0))) != 0)
            {
                //throw new HalconException("Wrong value of control parameter: 2");
            }
            if ((int)(new HTuple(hv_ContrastLow.TupleGreater(hv_ContrastHigh))) != 0)
            {
                //throw new HalconException("Wrong value of control parameter: 2");
            }
            //
            //Calculate the parameters Sigma, Low, and High for lines_gauss
            if ((int)(new HTuple(hv_MaxLineWidth_COPY_INP_TMP.TupleLess((new HTuple(3.0)).TupleSqrt()
                ))) != 0)
            {
                //Note that LineWidthMax < sqrt(3.0) would result in a Sigma < 0.5,
                //which does not make any sense, because the corresponding smoothing
                //filter mask would be of size 1x1.
                //To avoid this, LineWidthMax is restricted to values greater or equal
                //to sqrt(3.0) and the contrast values are adapted to reflect the fact
                //that lines that are thinner than sqrt(3.0) pixels have a lower contrast
                //in the smoothed image (compared to lines that are sqrt(3.0) pixels wide).
                hv_ContrastLow = (hv_ContrastLow * hv_MaxLineWidth_COPY_INP_TMP) / ((new HTuple(3.0)).TupleSqrt()
                    );
                hv_ContrastHigh = (hv_ContrastHigh * hv_MaxLineWidth_COPY_INP_TMP) / ((new HTuple(3.0)).TupleSqrt()
                    );
                hv_MaxLineWidth_COPY_INP_TMP = (new HTuple(3.0)).TupleSqrt();
            }
            //Convert LineWidthMax and the given contrast values into the input parameters
            //Sigma, Low, and High required by lines_gauss
            hv_HalfWidth = hv_MaxLineWidth_COPY_INP_TMP / 2.0;
            hv_Sigma = hv_HalfWidth / ((new HTuple(3.0)).TupleSqrt());
            hv_Help = ((-2.0 * hv_HalfWidth) / (((new HTuple(6.283185307178)).TupleSqrt()) * (hv_Sigma.TuplePow(
                3.0)))) * (((-0.5 * (((hv_HalfWidth / hv_Sigma)).TuplePow(2.0)))).TupleExp());
            hv_High = ((hv_ContrastHigh * hv_Help)).TupleFabs();
            hv_Low = ((hv_ContrastLow * hv_Help)).TupleFabs();

            return;
        }

        public static void select_min_max_length_contour(HObject ho_Contours, out HObject ho_MinLengthContour, out HObject ho_MaxLengthContour)
        {

            // Local iconic variables 

            HObject ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Number = new HTuple(), hv_Exception = null;
            HTuple hv_Max_Length = null, hv_Max_Index = null, hv_i = null;
            HTuple hv_Length = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_MinLengthContour);
            HOperatorSet.GenEmptyObj(out ho_MaxLengthContour);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            try
            {
                //打开错误信息
                // (dev_)set_check ("give_error")
                //最长轮廓变量初始化
                ho_MaxLengthContour.Dispose();
                HOperatorSet.GenEmptyObj(out ho_MaxLengthContour);
                //开启捕获异常
                try
                {
                    //统计轮廓集合的数量
                    HOperatorSet.CountObj(ho_Contours, out hv_Number);
                    //捕获异常
                }

                // catch (Exception) 
                catch (Exception exc)
                {
                    dhDll.frmMsg.Log("寻找最长XLD出错" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                    return;
                }
                //如果轮廓数量无效，返回
                if ((int)(new HTuple(hv_Number.TupleLess(1))) != 0)
                {
                    ho_ObjectSelected.Dispose();

                    return;
                }
                //最长长度、最长长度索引初始化
                hv_Max_Length = 0;
                hv_Max_Index = 0;
                //遍历每个轮廓的长度
                HTuple end_val21 = hv_Number;
                HTuple step_val21 = 1;
                for (hv_i = 1; hv_i.Continue(end_val21, step_val21); hv_i = hv_i.TupleAdd(step_val21))
                {
                    //选择轮廓
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_Contours, out ho_ObjectSelected, hv_i);
                    //求轮廓长度
                    HOperatorSet.LengthXld(ho_ObjectSelected, out hv_Length);
                    //保存最长轮廓的长度和索引
                    if ((int)(new HTuple(hv_Max_Length.TupleLess(hv_Length))) != 0)
                    {
                        hv_Max_Length = hv_Length.Clone();
                        hv_Max_Index = hv_i.Clone();
                    }
                }
                //选择最长轮廓
                ho_MaxLengthContour.Dispose();
                HOperatorSet.SelectObj(ho_Contours, out ho_MaxLengthContour, hv_Max_Index);
                //返回
                ho_ObjectSelected.Dispose();

                return;
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("寻找最长XLD出错" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
            }
        }

        public static void f_get_conner_rectangle2(HTuple hv_Length1, HTuple hv_Length2, HTuple hv_CenterRow,
    HTuple hv_CenterCol, HTuple hv_Phi, out HTuple hv_CornerRow, out HTuple hv_CornerCol)
        {
            // Local iconic variables 
            // Local control variables 
            HTuple hv_RowTem = null, hv_ColTem = null;
            HTuple hv_Cos = null, hv_Sin = null;
            // Initialize local and output iconic variables 
            //Initialize the variable for coordinate of vertexes of rectangle2
            hv_CornerRow = new HTuple();
            hv_CornerCol = new HTuple();
            //Initialize the temperary variables
            hv_RowTem = 0;
            hv_ColTem = 0;
            //Judge the rectangle if it is available
            if ((int)((new HTuple(hv_Length1.TupleLess(0))).TupleOr(new HTuple(hv_Length2.TupleLess(
                0)))) != 0)
            {
                return;
            }
            //Compute the sine and cosine of tuple Phi
            HOperatorSet.TupleCos(hv_Phi, out hv_Cos);
            HOperatorSet.TupleSin(hv_Phi, out hv_Sin);
            //Compute the coordinate of the upper-right vertex of rectangle
            hv_RowTem = (hv_CenterRow - (hv_Length1 * hv_Sin)) - (hv_Length2 * hv_Cos);
            hv_ColTem = (hv_CenterCol + (hv_Length1 * hv_Cos)) - (hv_Length2 * hv_Sin);
            hv_CornerRow = hv_CornerRow.TupleConcat(hv_RowTem);
            hv_CornerCol = hv_CornerCol.TupleConcat(hv_ColTem);

            //Compute the coordinate of the upper-left vertex of rectangle
            hv_RowTem = (hv_CenterRow + (hv_Length1 * hv_Sin)) - (hv_Length2 * hv_Cos);
            hv_ColTem = (hv_CenterCol - (hv_Length1 * hv_Cos)) - (hv_Length2 * hv_Sin);
            hv_CornerRow = hv_CornerRow.TupleConcat(hv_RowTem);
            hv_CornerCol = hv_CornerCol.TupleConcat(hv_ColTem);

            //Compute the coordinate of the bottom-left vertex of rectangle
            hv_RowTem = (hv_CenterRow + (hv_Length1 * hv_Sin)) + (hv_Length2 * hv_Cos);
            hv_ColTem = (hv_CenterCol - (hv_Length1 * hv_Cos)) + (hv_Length2 * hv_Sin);
            hv_CornerRow = hv_CornerRow.TupleConcat(hv_RowTem);
            hv_CornerCol = hv_CornerCol.TupleConcat(hv_ColTem);

            //Compute the coordinate of the bottom-right vertex of rectangle
            hv_RowTem = (hv_CenterRow - (hv_Length1 * hv_Sin)) + (hv_Length2 * hv_Cos);
            hv_ColTem = (hv_CenterCol + (hv_Length1 * hv_Cos)) + (hv_Length2 * hv_Sin);
            hv_CornerRow = hv_CornerRow.TupleConcat(hv_RowTem);
            hv_CornerCol = hv_CornerCol.TupleConcat(hv_ColTem);

            return;
        }

        public static void f_find_check_line(HObject ho_ImageReduce, out HObject ho_Line, HTuple hv_MaxLineWidth,
    HTuple hv_Contrast, HTuple hv_Phi, HTuple hv_Eps, HTuple hv_HOrV, HTuple hv_LightDark,
    HTuple hv_pi, out HTuple hv_Success)
        {
            // Local iconic variables 

            HObject ho_Line1, ho_Polygons, ho_SplitContours;
            HObject ho_SelectedContours = null, ho_MinLine1;

            // Local control variables 

            HTuple hv_Sigma = null, hv_Low = null, hv_High = null;
            HTuple hv_Num1 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_Line1);
            HOperatorSet.GenEmptyObj(out ho_Polygons);
            HOperatorSet.GenEmptyObj(out ho_SplitContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            HOperatorSet.GenEmptyObj(out ho_MinLine1);
            try
            {
                hv_Success = 0;
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                ho_Line1.Dispose();
                HOperatorSet.LinesGauss(ho_ImageReduce, out ho_Line1, hv_Sigma, hv_Low, hv_High,
                    hv_LightDark, "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line1, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    hv_Success = 0;
                    ho_Line1.Dispose();
                    ho_Polygons.Dispose();
                    ho_SplitContours.Dispose();
                    ho_SelectedContours.Dispose();
                    ho_MinLine1.Dispose();

                    return;
                }
                ho_Polygons.Dispose();
                HOperatorSet.GenPolygonsXld(ho_Line1, out ho_Polygons, "ramer", 1);
                ho_SplitContours.Dispose();
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                if ((int)(new HTuple(hv_HOrV.TupleEqual("H"))) != 0)
                {
                    ho_SelectedContours.Dispose();
                    HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                        "direction", hv_Phi - hv_Eps, hv_Phi + hv_Eps, -0.5, 0.5);
                }
                else
                {
                    ho_SelectedContours.Dispose();
                    HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                        "direction", (hv_Phi + (hv_pi / 2)) - hv_Eps, (hv_Phi + (hv_pi / 2)) + hv_Eps, -0.5,
                        0.5);
                }
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    hv_Success = 0;
                    ho_Line1.Dispose();
                    ho_Polygons.Dispose();
                    ho_SplitContours.Dispose();
                    ho_SelectedContours.Dispose();
                    ho_MinLine1.Dispose();

                    return;
                }
                ho_MinLine1.Dispose(); ho_Line.Dispose();
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_Line);
                hv_Success = 1;
                ho_Line1.Dispose();
                ho_Polygons.Dispose();
                ho_SplitContours.Dispose();
                ho_SelectedContours.Dispose();
                ho_MinLine1.Dispose();

                return;
            }
            catch (Exception exc)
            {
                hv_Success = 0;
                ho_Line1.Dispose();
                ho_Polygons.Dispose();
                ho_SplitContours.Dispose();
                ho_SelectedContours.Dispose();
                ho_MinLine1.Dispose();

                dhDll.frmMsg.Log("寻找剥离线出错" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
            }
        }

















        //印刷机找剥离线程序入口
        public static List<object> syPrintCheck(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 印刷机找剥离线程序入口  ***

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            string[] strUserParam = strParams.Split('#');
            int iProductModel = int.Parse(strUserParam[1]);  //产品型别 0402 0603 0805 1206 ...
            int iProductProgram = int.Parse(strUserParam[2]);//制作工序 0：G2前端 1：G2后端
            int iProductClass = int.Parse(strUserParam[3]);  //产品形状 0：条状   1：粒状 

            //dhDll.frmMsg.Log("sySixSideDetect8", "iProductModel = " + iProductModel + "iProductClass = " + iProductClass, null, dhDll.logDiskMode.Error, 0);

            if (iProductModel == 1 && iProductProgram == 0 && iProductClass == 0)                //0402_0    前端         
            {
                return syPrintCheck0402_0(hoImage, lkkPolygon, strParams);
            }
            else if (iProductModel == 1 && iProductProgram == 1 && iProductClass == 0)            //0402_0_1  后端         
            {
                return syPrintCheck0402_0_1(hoImage, lkkPolygon, strParams);
            }

            else if (iProductModel == 2 && iProductProgram == 0 && iProductClass == 0)            //0603_0    前端    
            {
                return syPrintCheck0603_0(hoImage, lkkPolygon, strParams);
            }
            else if (iProductModel == 2 && iProductProgram == 1 && iProductClass == 0)            //0603_0_1  后端
            {
                return syPrintCheck0603_0_1(hoImage, lkkPolygon, strParams);
            }

            else if (iProductModel == 4 && iProductProgram == 0 && iProductClass == 0)            //1206_0    前端
            {
                return syPrintCheck1206_0(hoImage, lkkPolygon, strParams);
            }
            else if (iProductModel == 4 && iProductProgram == 1 && iProductClass == 0)            //1206_0_1  后端
            {
                return syPrintCheck1206_0_1(hoImage, lkkPolygon, strParams);
            }

            else if (iProductModel == 3)  //0805_0,先不管产品形状
            {
                return syPrintCheck0805_0(hoImage, lkkPolygon, strParams);
            }

            else
            {
                listObj2Draw[1] = "NG-型别不符";
                return listObj2Draw;
            }

            #endregion
        }

        public static List<object> syPrintCheck0402_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 0402前端对位  ***

            if (bUseMutex) muDetect8.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);                 //粗定位阈值  25
                //int ithresCorner = int.Parse(strUserParam[5]);              //定位角阈值  20

                int hv_MaxLineWidth = int.Parse(strUserParam[8]);           //高斯线宽    5
                int hv_Contrast = int.Parse(strUserParam[9]);               //高斯对比度  5  

                int iLength1 = int.Parse(strUserParam[10]);                  //横向剥离线区域半宽  15
                int iLength2 = int.Parse(strUserParam[11]);                  //横向剥离线区域半高  20

                float iMinScore = float.Parse(strUserParam[12]);            //边缘最小得分 0.2
                int iMeasureTthreshold = int.Parse(strUserParam[13]);       //边缘阈值     30
                int iNumMeasures = int.Parse(strUserParam[14]);             //边缘卡尺个数 200

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_RegInters, ho_ImageCenter, ho_CenterReg, ho_CenterRegs, ho_BiggerRegs, ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_N, hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************
                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);  //粗定位阈值  25
                HOperatorSet.OpeningRectangle1(ho_RegionBlack, out ho_RegionBlackOpen, 8, 4);
                HOperatorSet.Connection(ho_RegionBlackOpen, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_BiggerRegs, "area", "and", 10000, 9999999999999);
                HOperatorSet.SelectShape(ho_BiggerRegs, out ho_CenterRegs, "width", "and", (4535 - 893) - 500, (4535 - 893) + 500);
                HOperatorSet.CountObj(ho_CenterRegs, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.Union1(ho_CenterRegs, out ho_CenterReg);
                HOperatorSet.ReduceDomain(hoImage, ho_CenterReg, out ho_ImageCenter);
                #endregion

                //找两次剥离线
                #region ****** 寻找左上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1538)).TupleConcat(649)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1337)).TupleConcat(1460)).TupleConcat(660));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect1, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1,
                    out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 120, hv_LUPCol + 10, hv_Phi1, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow - 38, hv_LUPCol - 10, hv_Phi1, iLength1, iLength2);
                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(2760)).TupleConcat(
                    3534)).TupleConcat(3534)).TupleConcat(2952)).TupleConcat(2760), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(2100)).TupleConcat(1506)).TupleConcat(665));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect2, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 120, hv_LDPCol + 10, hv_Phi2, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow + 38, hv_LDPCol - 10, hv_Phi2, iLength1, iLength2);
                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3610)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3610));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect3, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 120, hv_RUPCol - 10, hv_Phi3, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow - 38, hv_RUPCol + 10, hv_Phi3, iLength1, iLength2);
                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2791)).TupleConcat(3013)).TupleConcat(3546), ((((new HTuple(3412)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4062)).TupleConcat(3412));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect4, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 120, hv_RDPCol - 10, hv_Phi4, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow + 38, hv_RDPCol + 10, hv_Phi4, iLength1, iLength2);
                #endregion


                HTuple hv_LightDark = "light";
                HTuple hv_Success;

                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);

                f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast,
                 hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);

                f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);

                f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);

                f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);

                f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);

                f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_UHPoint1Row = new HTuple();
                HTuple hv_UHPoint1Col = new HTuple();
                HTuple hv_UHPoint2Row = new HTuple();
                HTuple hv_UHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_UHPoint1Row.TupleConcat(hv_UHPoint2Row), hv_UHPoint1Col.TupleConcat(hv_UHPoint2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_UHPoint1Col.D, (float)hv_UHPoint1Row.D, (float)hv_UHPoint2Col.D, (float)hv_UHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_DHPoint1Row = new HTuple();
                HTuple hv_DHPoint1Col = new HTuple();
                HTuple hv_DHPoint2Row = new HTuple();
                HTuple hv_DHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_DHPoint1Row.TupleConcat(hv_DHPoint2Row), hv_DHPoint1Col.TupleConcat(hv_DHPoint2Col));
                RectangleF rectLine2 = new RectangleF((float)hv_DHPoint1Col.D, (float)hv_DHPoint1Row.D, (float)hv_DHPoint2Col.D, (float)hv_DHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_LVPoint1Row = new HTuple();
                HTuple hv_LVPoint1Col = new HTuple();
                HTuple hv_LVPoint2Row = new HTuple();
                HTuple hv_LVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_LVPoint1Row.TupleConcat(hv_LVPoint2Row), hv_LVPoint1Col.TupleConcat(hv_LVPoint2Col));
                RectangleF rectLine3 = new RectangleF((float)hv_LVPoint1Col.D, (float)hv_LVPoint1Row.D, (float)hv_LVPoint2Col.D, (float)hv_LVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_RVPoint1Row = new HTuple();
                HTuple hv_RVPoint1Col = new HTuple();
                HTuple hv_RVPoint2Row = new HTuple();
                HTuple hv_RVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_RVPoint1Row.TupleConcat(hv_RVPoint2Row), hv_RVPoint1Col.TupleConcat(hv_RVPoint2Col));
                RectangleF rectLine4 = new RectangleF((float)hv_RVPoint1Col.D, (float)hv_RVPoint1Row.D, (float)hv_RVPoint2Col.D, (float)hv_RVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion


                HTuple hv_IsOverlapping, hv_Columnaaa, hv_Rowaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;
                #region *************计算四个交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);
                #endregion

                //Metrology 参数
                HTuple hv_min_score = iMinScore;
                HTuple hv_measure_threshold = iMeasureTthreshold;
                HTuple hv_num_measures = iNumMeasures;
                HTuple hv_PRt2Row, hv_PRt2Col, hv_PRt1Row, hv_PRt1Col, hv_PLf2Row, hv_PLf2Col, hv_NumDn, hv_RowMax, hv_PLf1Col, hv_PLf1Row, hv_NumUp, hv_RowMin, hv_PDn2Row, hv_PDn2Col, hv_PDn1Row, hv_PDn1Col, hv_PUp2Col, hv_PUp2Row, hv_Numright, hv_ColMax, hv_PUp1Row, hv_PUp1Col, hv_Numleft, hv_ColMin, hv_RowTmp, hv_ColTmp, hv_I, hv_Parameter, hv_Column, hv_Row, hv_MetrologyHandle, hv_Index;
                HObject ho_LineRT, ho_LineLF, ho_LineDN, ho_CrossLineUPddd, ho_CrossLineUPbbb, ho_LineUP, ho_CrossL, ho_CrossR, ho_ContourTmp, ho_Contour, ho_Cross, ho_Contours, ho_CrossLineUPaaa, ho_CrossLineUPccc;

                #region *************Metrology找基板上边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa - 125, hv_Columnaaa, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc - 125, hv_Columnccc, 60, 0.5);
                HTuple hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa - 125);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc - 125);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板上边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                HTuple hv_RowAll = new HTuple();
                HTuple hv_ColAll = new HTuple();
                HTuple end_val430 = hv_N;
                HTuple step_val430 = 1;
                for (hv_I = 1; hv_I.Continue(end_val430, step_val430); hv_I = hv_I.TupleAdd(step_val430))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PUp1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PUp1Col = hv_ColAll.TupleSelect(hv_Numleft);
                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PUp2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PUp2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PUp1Row, hv_PUp1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PUp2Row, hv_PUp2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineUP, hv_PUp1Row.TupleConcat(hv_PUp2Row), hv_PUp1Col.TupleConcat(hv_PUp2Col));
                RectangleF rectLineUP = new RectangleF((float)hv_PUp1Col.D, (float)hv_PUp1Row.D, (float)hv_PUp2Col.D, (float)hv_PUp2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineUP);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板下边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb + 125, hv_Columnbbb, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd + 125, hv_Columnddd, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb + 125);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd + 125);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板下边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val486 = hv_N;
                HTuple step_val486 = 1;
                for (hv_I = 1; hv_I.Continue(end_val486, step_val486); hv_I = hv_I.TupleAdd(step_val486))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PDn1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PDn1Col = hv_ColAll.TupleSelect(hv_Numleft);
                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PDn2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PDn2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PDn1Row, hv_PDn1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PDn2Row, hv_PDn2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineDN, hv_PDn1Row.TupleConcat(hv_PDn2Row), hv_PDn1Col.TupleConcat(hv_PDn2Col));
                RectangleF rectLineDN = new RectangleF((float)hv_PDn1Col.D, (float)hv_PDn1Row.D, (float)hv_PDn2Col.D, (float)hv_PDn2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineDN);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板左边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa, hv_Columnaaa - 95, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb, hv_Columnbbb - 95, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa - 95);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb - 95);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板左边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val542 = hv_N;
                HTuple step_val542 = 1;
                for (hv_I = 1; hv_I.Continue(end_val542, step_val542); hv_I = hv_I.TupleAdd(step_val542))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PLf1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PLf1Col = hv_ColAll.TupleSelect(hv_NumUp);

                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PLf2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PLf2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PLf1Row, hv_PLf1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PLf2Row, hv_PLf2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineLF, hv_PLf1Row.TupleConcat(hv_PLf2Row), hv_PLf1Col.TupleConcat(hv_PLf2Col));
                RectangleF rectLineLF = new RectangleF((float)hv_PLf1Col.D, (float)hv_PLf1Row.D, (float)hv_PLf2Col.D, (float)hv_PLf2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineLF);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板右边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc, hv_Columnccc + 95, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd, hv_Columnddd + 95, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc + 95);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd + 95);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板右边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val599 = hv_N;
                HTuple step_val599 = 1;
                for (hv_I = 1; hv_I.Continue(end_val599, step_val599); hv_I = hv_I.TupleAdd(step_val599))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PRt1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PRt1Col = hv_ColAll.TupleSelect(hv_NumUp);
                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PRt2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PRt2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PRt1Row, hv_PRt1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PRt2Row, hv_PRt2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineRT, hv_PRt1Row.TupleConcat(hv_PRt2Row), hv_PRt1Col.TupleConcat(hv_PRt2Col));
                RectangleF rectLineRT = new RectangleF((float)hv_PRt1Col.D, (float)hv_PRt1Row.D, (float)hv_PRt2Col.D, (float)hv_PRt2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineRT);
                listObj2Draw.Add("OK");
                #endregion

                HTuple hv_RowRD, hv_ColumnRD, hv_RowRU, hv_ColumnRU, hv_ColumnLD, hv_RowLD, hv_ColumnLU, hv_RowLU;
                #region *************计算四个基板边缘线交点*************
                //*左上交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowLU, out hv_ColumnLU,
                out hv_IsOverlapping);
                //*左下交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowLD, out hv_ColumnLD,
                out hv_IsOverlapping);
                //*右上交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowRU, out hv_ColumnRU,
                out hv_IsOverlapping);
                //*右下交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowRD, out hv_ColumnRD,
                out hv_IsOverlapping);
                #endregion

                HTuple hv_Dist9, hv_Dist10, hv_Dist11, hv_Dist12;
                #region *************计算边缘线边长*************
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowRU, hv_ColumnRU, out hv_Dist9);   //上边长
                HOperatorSet.DistancePp(hv_RowLD, hv_ColumnLD, hv_RowRD, hv_ColumnRD, out hv_Dist10);  //下边长
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, out hv_Dist11);  //左边长
                HOperatorSet.DistancePp(hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD, out hv_Dist12);  //右边长
                #endregion

                HTuple hv_ColumnKKK1, hv_RowKKK1, hv_Dist1, hv_ColumnKKK2, hv_RowKKK2, hv_Dist2, hv_ColumnKKK3, hv_RowKKK3, hv_Dist3, hv_ColumnKKK4, hv_RowKKK4, hv_Dist4, hv_ColumnKKK5, hv_RowKKK5, hv_Dist5, hv_ColumnKKK6, hv_RowKKK6, hv_Dist6, hv_ColumnKKK7, hv_RowKKK7, hv_Dist7, hv_ColumnKKK8, hv_RowKKK8, hv_Dist8;
                HObject ho_CrossKKK1, ho_CrossKKK2, ho_CrossKKK3, ho_CrossKKK4, ho_CrossKKK5, ho_CrossKKK6, ho_CrossKKK7, ho_CrossKKK8;
                #region *************计算剥离线交点到基板边缘距离*************
                //*计算左上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK1,
                out hv_ColumnKKK1, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK1, hv_RowKKK1, hv_ColumnKKK1,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK1, hv_ColumnKKK1,
                out hv_Dist1);
                //*计算左上剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK2,
                out hv_ColumnKKK2, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK2, hv_RowKKK2, hv_ColumnKKK2,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK2, hv_ColumnKKK2,
                out hv_Dist2);
                //*计算左下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK3,
                out hv_ColumnKKK3, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK3, hv_RowKKK3, hv_ColumnKKK3,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK3, hv_ColumnKKK3,
                out hv_Dist3);
                //*计算左下剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK4,
                out hv_ColumnKKK4, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK4, hv_RowKKK4, hv_ColumnKKK4,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK4, hv_ColumnKKK4,
                out hv_Dist4);
                //*计算右上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK5,
                out hv_ColumnKKK5, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK5, hv_RowKKK5, hv_ColumnKKK5,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK5, hv_ColumnKKK5,
                out hv_Dist5);
                //*计算右上剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK6,
                out hv_ColumnKKK6, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK6, hv_RowKKK6, hv_ColumnKKK6,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK6, hv_ColumnKKK6,
                out hv_Dist6);
                //*计算右下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK7,
                out hv_ColumnKKK7, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK7, hv_RowKKK7, hv_ColumnKKK7,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK7, hv_ColumnKKK7,
                out hv_Dist7);
                //*计算右下剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK8,
                out hv_ColumnKKK8, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK8, hv_RowKKK8, hv_ColumnKKK8,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK8, hv_ColumnKKK8,
                out hv_Dist8);
                #endregion

                HTuple hv_DistF1Sum, hv_DistF1, hv_DistF2Sum, hv_DistF2;
                #region *************计算F1 F2 *************
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_Rowccc, hv_Columnccc, out hv_DistF1Sum);
                hv_DistF1 = hv_DistF1Sum / 64;
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_Rowddd, hv_Columnddd, out hv_DistF2Sum);
                hv_DistF2 = hv_DistF2Sum / 64;
                #endregion

                //0402前端返回距离值
                string RetStr = "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +      // L R 
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#"

                                  + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"  //剥离线交点坐标 X Y
                                  + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                                  + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                                  + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000") + "#"

                                  + hv_Dist1.D.ToString("0.0000") + "#" + hv_Dist2.D.ToString("0.0000") + "#"   // D 
                                  + hv_Dist3.D.ToString("0.0000") + "#" + hv_Dist4.D.ToString("0.0000") + "#"
                                  + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#"
                                  + hv_Dist7.D.ToString("0.0000") + "#" + hv_Dist8.D.ToString("0.0000") + "#"

                                  + hv_Dist9.D.ToString("0.0000") + "#" + hv_Dist10.D.ToString("0.0000") + "#"  //W H
                                  + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"

                                  + hv_DistF1.D.ToString("0.0000") + "#" + hv_DistF2.D.ToString("0.0000");      //F1 F2

                //dhDll.frmMsg.Log("syPrintCheck0402_0", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;
            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck0402_0" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }
            #endregion  
        }
        public static List<object> syPrintCheck0402_0_1(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 0402后端对位  ***
            if (bUseMutex) muDetect8.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******
                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);                 //粗定位阈值  25
                //int ithresCorner = int.Parse(strUserParam[5]);              //定位角阈值  20

                int hv_MaxLineWidth = int.Parse(strUserParam[8]);           //高斯线宽    5
                int hv_Contrast = int.Parse(strUserParam[9]);               //高斯对比度  5  

                int iLength1 = int.Parse(strUserParam[10]);                  //横向剥离线区域半宽  20
                int iLength2 = int.Parse(strUserParam[11]);                  //横向剥离线区域半高  15

                float iMinScore = float.Parse(strUserParam[12]);            //边缘最小得分 0.1
                int iMeasureTthreshold = int.Parse(strUserParam[13]);       //边缘阈值     30
                int iNumMeasures = int.Parse(strUserParam[14]);             //边缘卡尺个数 200

                int iParamNum = int.Parse(strUserParam[16]);                //横向距离检测平行线偏移像素值 1
                int iHengThres = int.Parse(strUserParam[17]);               //横向距离检测阈值 40

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_RegInters, ho_ImageCenter, ho_CenterReg, ho_CenterRegs, ho_BiggerRegs, ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_N, hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************
                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);    //粗定位阈值  25
                HOperatorSet.OpeningCircle(ho_RegionBlack, out ho_RegionBlackOpen, 2);
                HOperatorSet.Connection(ho_RegionBlackOpen, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_SelectedRegionsholes, "holes_num", "and", 50, 99999);
                HOperatorSet.CountObj(ho_SelectedRegionsholes, out hv_Num);
                if ((int)(new HTuple(hv_Num.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.SelectShapeStd(ho_SelectedRegionsholes, out ho_RegionR, "max_area", 70);
                HOperatorSet.FillUp(ho_RegionR, out ho_RegionFillRRR);
                HOperatorSet.AreaCenter(ho_RegionFillRRR, out hv_areaRRR, out hv_rowRRR, out hv_colRRR);
                if ((int)(new HTuple(hv_areaRRR.TupleLess(10000000))) != 0)
                {
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.ReduceDomain(hoImage, ho_RegionFillRRR, out ho_ImageRRR);

                #endregion

                #region ****** 寻找左上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1238)).TupleConcat(790)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1070)).TupleConcat(1160)).TupleConcat(660));
                HOperatorSet.Intersection(ho_RegionFillRRR, ho_Rect1, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1, out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 40, hv_LUPCol + 170, hv_Phi1, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow + 8, hv_LUPCol - 20, hv_Phi1, iLength1, iLength2);
                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(3060)).TupleConcat(
                    3534)).TupleConcat(3550)).TupleConcat(3210)).TupleConcat(3060), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(1600)).TupleConcat(1172)).TupleConcat(665));
                HOperatorSet.Intersection(ho_RegionFillRRR, ho_Rect2, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 40, hv_LDPCol + 170, hv_Phi2, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow - 8, hv_LDPCol - 20, hv_Phi2, iLength1, iLength2);
                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3810)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3810));
                HOperatorSet.Intersection(ho_RegionFillRRR, ho_Rect3, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 40, hv_RUPCol - 170, hv_Phi3, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow + 8, hv_RUPCol + 20, hv_Phi3, iLength1, iLength2);
                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2991)).TupleConcat(3159)).TupleConcat(3546), ((((new HTuple(3912)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4224)).TupleConcat(3912));
                HOperatorSet.Intersection(ho_RegionFillRRR, ho_Rect4, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 40, hv_RDPCol - 170, hv_Phi4, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow - 8, hv_RDPCol + 20, hv_Phi4, iLength1, iLength2);
                #endregion

                HTuple hv_LightDark = "light";
                HTuple hv_Success;

                //找两次剥离线
                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);

                f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast,
                 hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);

                f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);

                f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);

                f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);

                f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);

                f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_UHPoint1Row = new HTuple();
                HTuple hv_UHPoint1Col = new HTuple();
                HTuple hv_UHPoint2Row = new HTuple();
                HTuple hv_UHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_UHPoint1Row.TupleConcat(hv_UHPoint2Row), hv_UHPoint1Col.TupleConcat(hv_UHPoint2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_UHPoint1Col.D, (float)hv_UHPoint1Row.D, (float)hv_UHPoint2Col.D, (float)hv_UHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_DHPoint1Row = new HTuple();
                HTuple hv_DHPoint1Col = new HTuple();
                HTuple hv_DHPoint2Row = new HTuple();
                HTuple hv_DHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_DHPoint1Row.TupleConcat(hv_DHPoint2Row), hv_DHPoint1Col.TupleConcat(hv_DHPoint2Col));
                RectangleF rectLine2 = new RectangleF((float)hv_DHPoint1Col.D, (float)hv_DHPoint1Row.D, (float)hv_DHPoint2Col.D, (float)hv_DHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_LVPoint1Row = new HTuple();
                HTuple hv_LVPoint1Col = new HTuple();
                HTuple hv_LVPoint2Row = new HTuple();
                HTuple hv_LVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_LVPoint1Row.TupleConcat(hv_LVPoint2Row), hv_LVPoint1Col.TupleConcat(hv_LVPoint2Col));
                RectangleF rectLine3 = new RectangleF((float)hv_LVPoint1Col.D, (float)hv_LVPoint1Row.D, (float)hv_LVPoint2Col.D, (float)hv_LVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_RVPoint1Row = new HTuple();
                HTuple hv_RVPoint1Col = new HTuple();
                HTuple hv_RVPoint2Row = new HTuple();
                HTuple hv_RVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_RVPoint1Row.TupleConcat(hv_RVPoint2Row), hv_RVPoint1Col.TupleConcat(hv_RVPoint2Col));
                RectangleF rectLine4 = new RectangleF((float)hv_RVPoint1Col.D, (float)hv_RVPoint1Row.D, (float)hv_RVPoint2Col.D, (float)hv_RVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion

                HObject ExpTmpOutVar_0;
                HOperatorSet.Emphasize(hoImage, out ExpTmpOutVar_0, 4, 4, 2);
                hoImage = ExpTmpOutVar_0;
                PointF[] ptsLeft = new PointF[12];

                HTuple hv_ParamNum = iParamNum;    //*平行线偏移像素值 1  2  3
                HTuple hv_Heng_thres = iHengThres; //*横向偏移分割阈值 40

                HTuple hv_RowP, hv_ColumnP, hv_DistanceMax, hv_RowCross, hv_ColumnCross, hv_MetrologyHandle, hv_RowQQQ, hv_ColumnQQQ, hv_A, hv_B, hv_x, hv_Col2, hv_Col1, hv_Index;
                HObject ho_BigLine, ho_RegionL, ho_SortedRegions, ho_Rect;
                HObject ho_RVLinePara1, ho_RVLinePara2, ho_Contour, ho_Contours, ho_LVLinePara1, ho_LVLinePara2, ho_Cross12, ho_Cross11, ho_Cross10, ho_Line10, ho_Cross8, ho_Cross9, ho_Cross7, ho_RVLinePara, ho_Cross6, ho_Cross5, ho_DHLinePara, ho_Cross4, ho_Cross3, ho_Cross2, ho_UHLinePara, ho_ImageEmph, ho_LVLinePara, ho_Cross, ho_Edges, ho_XXX, ho_LongEdge, ho_Cross1;
                HTuple hv_Select, hv_Dist11, hv_Transition, hv_Dist12, hv_Dist10, hv_Dist8, hv_Dist9, hv_Dist7, hv_Dist5, hv_Dist6, hv_Dist4, hv_Dist2, hv_Dist3, hv_RowEdge, hv_ColumnEdge, hv_Amplitude, hv_Distance, hv_MeasureHandle, hv_Row, hv_Column, hv_IsOverlapping, hv_Row1, hv_Column1, hv_Dist1;
                HTuple hv_Parameter, hv_RowEdgeFirst, hv_ColumnEdgeFirst, hv_AmplitudeFirst, hv_RowEdgeSecond, hv_ColumnEdgeSecond, hv_AmplitudeSecond, hv_IntraDistance, hv_InterDistance;

                HTuple hv_Eps2 = hv_Eps / 2;

                #region*********************************计算左上三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara1, "regression_normal", 100);
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara2, "regression_normal", -100);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                HTuple hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左上竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi1 - hv_Eps2, hv_Phi1 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist1, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_LVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[0] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[0]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleLess(hv_RowP))) != 0)
                {
                    hv_Dist1 = 0 - hv_Dist1;
                }

                HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", (-hv_Dist1) + hv_ParamNum);
                HOperatorSet.IntersectionContoursXld(ho_LVLine, ho_UHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi1, 30, 2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, hv_Heng_thres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad());
                ptsLeft[1] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[1]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = (hv_RowQQQ + 1) + (0.3 * hv_ParamNum);
                hv_Col2 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad());
                ptsLeft[2] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[2]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_LVLine, hv_Row1, hv_Col1, out hv_Dist2, out hv_x);
                HOperatorSet.DistancePc(ho_LVLine, hv_Row2, hv_Col2, out hv_Dist3, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist2 = 0 - hv_Dist2;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist3 = 0 - hv_Dist3;
                }
                #endregion

                #region*********************************计算左下三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara1, "regression_normal", 100);
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara2, "regression_normal", -100);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左下竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi2 - hv_Eps2, hv_Phi2 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist4, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_LVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[3] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[3]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleGreater(hv_RowP))) != 0)
                {
                    hv_Dist4 = 0 - hv_Dist4;
                }

                HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist4 - hv_ParamNum);
                HOperatorSet.IntersectionContoursXld(ho_LVLine, ho_DHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi2, 30, 2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, hv_Heng_thres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad());
                ptsLeft[4] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[4]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = (hv_RowQQQ + 1) + (0.3 * hv_ParamNum);
                hv_Col2 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad());
                ptsLeft[5] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[5]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_LVLine, hv_Row1, hv_Col1, out hv_Dist5, out hv_x);
                HOperatorSet.DistancePc(ho_LVLine, hv_Row2, hv_Col2, out hv_Dist6, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist5 = 0 - hv_Dist5;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist6 = 0 - hv_Dist6;
                }
                #endregion

                #region*********************************计算右上三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara1, "regression_normal", 100);
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara2, "regression_normal", -100);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算右上竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi3 - hv_Eps2, hv_Phi3 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist7, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_RVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[6] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[6]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleLess(hv_RowP))) != 0)
                {
                    hv_Dist7 = 0 - hv_Dist7;
                }

                HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", (-hv_Dist7) + hv_ParamNum);
                HOperatorSet.IntersectionContoursXld(ho_RVLine, ho_UHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi3, 30, 2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, hv_Heng_thres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    listObj2Draw[1] = "NG-计算右上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross8, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad());
                ptsLeft[7] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[7]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = (hv_RowQQQ + 1) + (0.3 * hv_ParamNum);
                hv_Col2 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross9, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad());
                ptsLeft[8] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[8]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_RVLine, hv_Row1, hv_Col1, out hv_Dist8, out hv_x);
                HOperatorSet.DistancePc(ho_RVLine, hv_Row2, hv_Col2, out hv_Dist9, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist8 = 0 - hv_Dist8;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist9 = 0 - hv_Dist9;
                }
                #endregion

                #region*********************************计算右下三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara1, "regression_normal", 100);
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara2, "regression_normal", -100);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算右下竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi4 - hv_Eps2, hv_Phi4 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist10, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_RVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross10, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[9] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[9]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleGreater(hv_RowP))) != 0)
                {
                    hv_Dist10 = 0 - hv_Dist10;
                }

                HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist10 - hv_ParamNum);
                HOperatorSet.IntersectionContoursXld(ho_RVLine, ho_DHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi4, 30, 2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, hv_Heng_thres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    listObj2Draw[1] = "NG-计算右下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad());
                ptsLeft[10] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[10]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = (hv_RowQQQ + 1) + (0.3 * hv_ParamNum);
                hv_Col2 = hv_ColumnQQQ.Clone();
                HOperatorSet.GenCrossContourXld(out ho_Cross12, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad());
                ptsLeft[11] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[11]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_RVLine, hv_Row1, hv_Col1, out hv_Dist11, out hv_x);
                HOperatorSet.DistancePc(ho_RVLine, hv_Row2, hv_Col2, out hv_Dist12, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist11 = 0 - hv_Dist11;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist12 = 0 - hv_Dist12;
                }
                #endregion

                HTuple hv_Rowaaa, hv_Columnaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;
                #region *************计算四个交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);
                #endregion

                //Metrology 参数
                HTuple hv_min_score = iMinScore;                  //0.1
                HTuple hv_measure_threshold = iMeasureTthreshold; //30
                HTuple hv_num_measures = iNumMeasures;            //200
                HTuple hv_PRt2Row, hv_PRt2Col, hv_PRt1Row, hv_PRt1Col, hv_PLf2Row, hv_PLf2Col, hv_NumDn, hv_RowMax, hv_PLf1Col, hv_PLf1Row, hv_NumUp, hv_RowMin, hv_PDn2Row, hv_PDn2Col, hv_PDn1Row, hv_PDn1Col, hv_PUp2Col, hv_PUp2Row, hv_Numright, hv_ColMax, hv_PUp1Row, hv_PUp1Col, hv_Numleft, hv_ColMin, hv_RowTmp, hv_ColTmp, hv_I;
                HObject ho_LineRT, ho_LineLF, ho_LineDN, ho_CrossLineUPddd, ho_CrossLineUPbbb, ho_LineUP, ho_CrossL, ho_CrossR, ho_ContourTmp, ho_CrossLineUPaaa, ho_CrossLineUPccc;

                #region *************Metrology找基板上边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa - 160, hv_Columnaaa, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc - 160, hv_Columnccc, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa - 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc - 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板上边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                HTuple hv_RowAll = new HTuple();
                HTuple hv_ColAll = new HTuple();
                HTuple end_val592 = hv_N;
                HTuple step_val592 = 1;
                for (hv_I = 1; hv_I.Continue(end_val592, step_val592); hv_I = hv_I.TupleAdd(step_val592))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PUp1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PUp1Col = hv_ColAll.TupleSelect(hv_Numleft);

                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PUp2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PUp2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PUp1Row, hv_PUp1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PUp2Row, hv_PUp2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineUP, hv_PUp1Row.TupleConcat(hv_PUp2Row), hv_PUp1Col.TupleConcat(hv_PUp2Col));
                RectangleF rectLineUP = new RectangleF((float)hv_PUp1Col.D, (float)hv_PUp1Row.D, (float)hv_PUp2Col.D, (float)hv_PUp2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineUP);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板下边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb + 160, hv_Columnbbb, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd + 160, hv_Columnddd, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb + 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd + 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板下边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val649 = hv_N;
                HTuple step_val649 = 1;
                for (hv_I = 1; hv_I.Continue(end_val649, step_val649); hv_I = hv_I.TupleAdd(step_val649))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PDn1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PDn1Col = hv_ColAll.TupleSelect(hv_Numleft);

                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PDn2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PDn2Col = hv_ColAll.TupleSelect(hv_Numright);

                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PDn1Row, hv_PDn1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PDn2Row, hv_PDn2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineDN, hv_PDn1Row.TupleConcat(hv_PDn2Row), hv_PDn1Col.TupleConcat(hv_PDn2Col));
                RectangleF rectLineDN = new RectangleF((float)hv_PDn1Col.D, (float)hv_PDn1Row.D, (float)hv_PDn2Col.D, (float)hv_PDn2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineDN);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板左边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa, hv_Columnaaa - 260, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb, hv_Columnbbb - 260, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa - 260);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb - 260);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板左边缘失败";
                    return listObj2Draw;
                }

                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val706 = hv_N;
                HTuple step_val706 = 1;
                for (hv_I = 1; hv_I.Continue(end_val706, step_val706); hv_I = hv_I.TupleAdd(step_val706))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PLf1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PLf1Col = hv_ColAll.TupleSelect(hv_NumUp);

                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PLf2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PLf2Col = hv_ColAll.TupleSelect(hv_NumDn);

                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PLf1Row, hv_PLf1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PLf2Row, hv_PLf2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineLF, hv_PLf1Row.TupleConcat(hv_PLf2Row), hv_PLf1Col.TupleConcat(hv_PLf2Col));
                RectangleF rectLineLF = new RectangleF((float)hv_PLf1Col.D, (float)hv_PLf1Row.D, (float)hv_PLf2Col.D, (float)hv_PLf2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineLF);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板右边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc, hv_Columnccc + 260, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd, hv_Columnddd + 260, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc + 260);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd + 260);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板右边缘失败";
                    return listObj2Draw;
                }

                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val763 = hv_N;
                HTuple step_val763 = 1;
                for (hv_I = 1; hv_I.Continue(end_val763, step_val763); hv_I = hv_I.TupleAdd(step_val763))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PRt1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PRt1Col = hv_ColAll.TupleSelect(hv_NumUp);
                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PRt2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PRt2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PRt1Row, hv_PRt1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PRt2Row, hv_PRt2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineRT, hv_PRt1Row.TupleConcat(hv_PRt2Row), hv_PRt1Col.TupleConcat(hv_PRt2Col));
                RectangleF rectLineRT = new RectangleF((float)hv_PRt1Col.D, (float)hv_PRt1Row.D, (float)hv_PRt2Col.D, (float)hv_PRt2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineRT);
                listObj2Draw.Add("OK");
                #endregion

                HTuple hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD;

                #region *************计算四个基板边缘线交点*************
                //*左上交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                    hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowLU, out hv_ColumnLU,
                    out hv_IsOverlapping);
                //*左下交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                    hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowLD, out hv_ColumnLD,
                    out hv_IsOverlapping);
                //*右上交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                    hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowRU, out hv_ColumnRU,
                    out hv_IsOverlapping);
                //*右下交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                    hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowRD, out hv_ColumnRD,
                    out hv_IsOverlapping);
                #endregion

                HTuple hv_Dist13, hv_Dist14, hv_Dist15, hv_Dist16, hv_Dist21, hv_Dist22, hv_Dist23, hv_Dist24;
                #region *************计算边缘线边长*************
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowRU, hv_ColumnRU, out hv_Dist21);   //上边长
                HOperatorSet.DistancePp(hv_RowLD, hv_ColumnLD, hv_RowRD, hv_ColumnRD, out hv_Dist22);   //下边长
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, out hv_Dist23);   //左边长
                HOperatorSet.DistancePp(hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD, out hv_Dist24);   //右边长
                #endregion

                HTuple hv_ColumnKKK1, hv_RowKKK1, hv_Dist17, hv_ColumnKKK2, hv_RowKKK2, hv_Dist18, hv_ColumnKKK3, hv_RowKKK3, hv_Dist19, hv_ColumnKKK4, hv_RowKKK4, hv_Dist20, hv_ColumnKKK5, hv_RowKKK5, hv_ColumnKKK6, hv_RowKKK6, hv_ColumnKKK7, hv_RowKKK7, hv_ColumnKKK8, hv_RowKKK8;
                HObject ho_CrossKKK1, ho_CrossKKK2, ho_CrossKKK3, ho_CrossKKK4, ho_CrossKKK5, ho_CrossKKK6, ho_CrossKKK7, ho_CrossKKK8;
                #region *************计算剥离线交点到基板边缘距离*************
                //*计算左上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK1,
                out hv_ColumnKKK1, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK1, hv_RowKKK1, hv_ColumnKKK1,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK1, hv_ColumnKKK1,
                out hv_Dist13);
                //*计算左上剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK2,
                out hv_ColumnKKK2, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK2, hv_RowKKK2, hv_ColumnKKK2,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK2, hv_ColumnKKK2,
                out hv_Dist14);
                //*计算左下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK3,
                out hv_ColumnKKK3, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK3, hv_RowKKK3, hv_ColumnKKK3,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK3, hv_ColumnKKK3,
                out hv_Dist15);
                //*计算左下剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK4,
                out hv_ColumnKKK4, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK4, hv_RowKKK4, hv_ColumnKKK4,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK4, hv_ColumnKKK4,
                out hv_Dist16);
                //*计算右上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK5,
                out hv_ColumnKKK5, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK5, hv_RowKKK5, hv_ColumnKKK5,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK5, hv_ColumnKKK5,
                out hv_Dist17);
                //*计算右上剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK6,
                out hv_ColumnKKK6, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK6, hv_RowKKK6, hv_ColumnKKK6,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK6, hv_ColumnKKK6,
                out hv_Dist18);
                //*计算右下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK7,
                out hv_ColumnKKK7, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK7, hv_RowKKK7, hv_ColumnKKK7,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK7, hv_ColumnKKK7,
                out hv_Dist19);
                //*计算右下剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK8,
                out hv_ColumnKKK8, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK8, hv_RowKKK8, hv_ColumnKKK8,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK8, hv_ColumnKKK8,
                out hv_Dist20);
                #endregion

                HTuple hv_DistF1Sum, hv_DistF1, hv_DistF2Sum, hv_DistF2;
                #region *************计算F1 F2 *************
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_Rowccc, hv_Columnccc, out hv_DistF1Sum);
                hv_DistF1 = hv_DistF1Sum / 64;
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_Rowddd, hv_Columnddd, out hv_DistF2Sum);
                hv_DistF2 = hv_DistF2Sum / 64;
                #endregion

                //返回距离值
                string RetStr = hv_Dist1.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist2.D.ToString("0.0000") + "#" + hv_Dist3.D.ToString("0.0000") + "#"
                              + hv_Dist4.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#"
                              + hv_Dist7.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist8.D.ToString("0.0000") + "#" + hv_Dist9.D.ToString("0.0000") + "#"
                              + hv_Dist10.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"  //LR

                              + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"  //剥离线交点坐标
                              + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                              + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                              + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000") + "#"

                              + hv_Dist13.D.ToString("0.0000") + "#" + hv_Dist14.D.ToString("0.0000") + "#"     // D
                              + hv_Dist15.D.ToString("0.0000") + "#" + hv_Dist16.D.ToString("0.0000") + "#"
                              + hv_Dist17.D.ToString("0.0000") + "#" + hv_Dist18.D.ToString("0.0000") + "#"
                              + hv_Dist19.D.ToString("0.0000") + "#" + hv_Dist20.D.ToString("0.0000") + "#"

                              + hv_Dist21.D.ToString("0.0000") + "#" + hv_Dist22.D.ToString("0.0000") + "#"     // W H
                              + hv_Dist23.D.ToString("0.0000") + "#" + hv_Dist24.D.ToString("0.0000") + "#"
                              + hv_DistF1.D.ToString("0.0000") + "#" + hv_DistF2.D.ToString("0.0000");          // F1 F2

                dhDll.frmMsg.Log("syPrintCheck0402_0_1", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck0402_0_1" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }

            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }

            #endregion
        }

        public static List<object> syPrintCheck0603_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 0603前端对位  ***

            if (bUseMutex) muDetect8.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);                 //粗定位阈值  20
                int hv_MaxLineWidth = int.Parse(strUserParam[8]);           //高斯线宽    5
                int hv_Contrast = int.Parse(strUserParam[9]);               //高斯对比度  5  
                int iLength1 = int.Parse(strUserParam[10]);                  //横向剥离线区域半宽  15
                int iLength2 = int.Parse(strUserParam[11]);                  //横向剥离线区域半高  20
                float iMinScore = float.Parse(strUserParam[12]);            //边缘最小得分 0.2
                int iMeasureTthreshold = int.Parse(strUserParam[13]);       //边缘阈值     30
                int iNumMeasures = int.Parse(strUserParam[14]);             //边缘卡尺个数 200

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_RegInters, ho_ImageCenter, ho_CenterReg, ho_CenterRegs, ho_BiggerRegs, ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_N, hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************
                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);  //粗定位阈值  20
                HOperatorSet.OpeningRectangle1(ho_RegionBlack, out ho_RegionBlackOpen, 8, 4);
                HOperatorSet.Connection(ho_RegionBlackOpen, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_BiggerRegs, "area", "and", 10000, 9999999999999);
                HOperatorSet.SelectShape(ho_BiggerRegs, out ho_CenterRegs, "width", "and", (4535 - 893) - 500, (4535 - 893) + 500);
                HOperatorSet.CountObj(ho_CenterRegs, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.Union1(ho_CenterRegs, out ho_CenterReg);
                HOperatorSet.ReduceDomain(hoImage, ho_CenterReg, out ho_ImageCenter);
                #endregion

                //0402两次寻找剥离线

                #region ****** 寻找左上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1538)).TupleConcat(649)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1337)).TupleConcat(1460)).TupleConcat(660));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect1, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1,
                    out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 50, hv_LUPCol + 10, hv_Phi1, 30, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow - 5, hv_LUPCol - 15, hv_Phi1, iLength1, iLength2);
                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(2760)).TupleConcat(
                    3534)).TupleConcat(3534)).TupleConcat(2952)).TupleConcat(2760), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(2100)).TupleConcat(1506)).TupleConcat(665));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect2, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 50, hv_LDPCol + 10, hv_Phi2, 30, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow + 5, hv_LDPCol - 15, hv_Phi2, iLength1, iLength2);
                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3610)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3610));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect3, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 50, hv_RUPCol - 10, hv_Phi3, 30, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow - 5, hv_RUPCol + 15, hv_Phi3, iLength1, iLength2);
                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2791)).TupleConcat(3013)).TupleConcat(3546), ((((new HTuple(3412)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4062)).TupleConcat(3412));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect4, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 50, hv_RDPCol - 10, hv_Phi4, 30, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow + 5, hv_RDPCol + 15, hv_Phi4, iLength1, iLength2);
                #endregion


                HTuple hv_LightDark = "light";
                HTuple hv_Success;

                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);

                f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast,
                 hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);

                f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);

                f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);

                f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);

                f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);

                f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_UHPoint1Row = new HTuple();
                HTuple hv_UHPoint1Col = new HTuple();
                HTuple hv_UHPoint2Row = new HTuple();
                HTuple hv_UHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_UHPoint1Row.TupleConcat(hv_UHPoint2Row), hv_UHPoint1Col.TupleConcat(hv_UHPoint2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_UHPoint1Col.D, (float)hv_UHPoint1Row.D, (float)hv_UHPoint2Col.D, (float)hv_UHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_DHPoint1Row = new HTuple();
                HTuple hv_DHPoint1Col = new HTuple();
                HTuple hv_DHPoint2Row = new HTuple();
                HTuple hv_DHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_DHPoint1Row.TupleConcat(hv_DHPoint2Row), hv_DHPoint1Col.TupleConcat(hv_DHPoint2Col));
                RectangleF rectLine2 = new RectangleF((float)hv_DHPoint1Col.D, (float)hv_DHPoint1Row.D, (float)hv_DHPoint2Col.D, (float)hv_DHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_LVPoint1Row = new HTuple();
                HTuple hv_LVPoint1Col = new HTuple();
                HTuple hv_LVPoint2Row = new HTuple();
                HTuple hv_LVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_LVPoint1Row.TupleConcat(hv_LVPoint2Row), hv_LVPoint1Col.TupleConcat(hv_LVPoint2Col));
                RectangleF rectLine3 = new RectangleF((float)hv_LVPoint1Col.D, (float)hv_LVPoint1Row.D, (float)hv_LVPoint2Col.D, (float)hv_LVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_RVPoint1Row = new HTuple();
                HTuple hv_RVPoint1Col = new HTuple();
                HTuple hv_RVPoint2Row = new HTuple();
                HTuple hv_RVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_RVPoint1Row.TupleConcat(hv_RVPoint2Row), hv_RVPoint1Col.TupleConcat(hv_RVPoint2Col));
                RectangleF rectLine4 = new RectangleF((float)hv_RVPoint1Col.D, (float)hv_RVPoint1Row.D, (float)hv_RVPoint2Col.D, (float)hv_RVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion


                HTuple hv_IsOverlapping, hv_Columnaaa, hv_Rowaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;
                #region *************计算四个交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);
                #endregion

                //Metrology 参数
                HTuple hv_min_score = iMinScore;
                HTuple hv_measure_threshold = iMeasureTthreshold;
                HTuple hv_num_measures = iNumMeasures;
                HTuple hv_PRt2Row, hv_PRt2Col, hv_PRt1Row, hv_PRt1Col, hv_PLf2Row, hv_PLf2Col, hv_NumDn, hv_RowMax, hv_PLf1Col, hv_PLf1Row, hv_NumUp, hv_RowMin, hv_PDn2Row, hv_PDn2Col, hv_PDn1Row, hv_PDn1Col, hv_PUp2Col, hv_PUp2Row, hv_Numright, hv_ColMax, hv_PUp1Row, hv_PUp1Col, hv_Numleft, hv_ColMin, hv_RowTmp, hv_ColTmp, hv_I, hv_Parameter, hv_Column, hv_Row, hv_MetrologyHandle, hv_Index;
                HObject ho_LineRT, ho_LineLF, ho_LineDN, ho_CrossLineUPddd, ho_CrossLineUPbbb, ho_LineUP, ho_CrossL, ho_CrossR, ho_ContourTmp, ho_Contour, ho_Cross, ho_Contours, ho_CrossLineUPaaa, ho_CrossLineUPccc;

                #region *************Metrology找基板上边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa - 110, hv_Columnaaa, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc - 110, hv_Columnccc, 60, 0.5);
                HTuple hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa - 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc - 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板上边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                HTuple hv_RowAll = new HTuple();
                HTuple hv_ColAll = new HTuple();
                HTuple end_val430 = hv_N;
                HTuple step_val430 = 1;
                for (hv_I = 1; hv_I.Continue(end_val430, step_val430); hv_I = hv_I.TupleAdd(step_val430))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PUp1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PUp1Col = hv_ColAll.TupleSelect(hv_Numleft);
                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PUp2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PUp2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PUp1Row, hv_PUp1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PUp2Row, hv_PUp2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineUP, hv_PUp1Row.TupleConcat(hv_PUp2Row), hv_PUp1Col.TupleConcat(hv_PUp2Col));
                RectangleF rectLineUP = new RectangleF((float)hv_PUp1Col.D, (float)hv_PUp1Row.D, (float)hv_PUp2Col.D, (float)hv_PUp2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineUP);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板下边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb + 110, hv_Columnbbb, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd + 110, hv_Columnddd, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb + 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd + 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板下边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val486 = hv_N;
                HTuple step_val486 = 1;
                for (hv_I = 1; hv_I.Continue(end_val486, step_val486); hv_I = hv_I.TupleAdd(step_val486))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PDn1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PDn1Col = hv_ColAll.TupleSelect(hv_Numleft);
                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PDn2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PDn2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PDn1Row, hv_PDn1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PDn2Row, hv_PDn2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineDN, hv_PDn1Row.TupleConcat(hv_PDn2Row), hv_PDn1Col.TupleConcat(hv_PDn2Col));
                RectangleF rectLineDN = new RectangleF((float)hv_PDn1Col.D, (float)hv_PDn1Row.D, (float)hv_PDn2Col.D, (float)hv_PDn2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineDN);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板左边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa, hv_Columnaaa - 110, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb, hv_Columnbbb - 110, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa - 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb - 110);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板左边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val542 = hv_N;
                HTuple step_val542 = 1;
                for (hv_I = 1; hv_I.Continue(end_val542, step_val542); hv_I = hv_I.TupleAdd(step_val542))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PLf1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PLf1Col = hv_ColAll.TupleSelect(hv_NumUp);

                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PLf2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PLf2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PLf1Row, hv_PLf1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PLf2Row, hv_PLf2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineLF, hv_PLf1Row.TupleConcat(hv_PLf2Row), hv_PLf1Col.TupleConcat(hv_PLf2Col));
                RectangleF rectLineLF = new RectangleF((float)hv_PLf1Col.D, (float)hv_PLf1Row.D, (float)hv_PLf2Col.D, (float)hv_PLf2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineLF);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板右边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc, hv_Columnccc + 110, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd, hv_Columnddd + 110, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc + 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd + 110);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板右边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val599 = hv_N;
                HTuple step_val599 = 1;
                for (hv_I = 1; hv_I.Continue(end_val599, step_val599); hv_I = hv_I.TupleAdd(step_val599))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PRt1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PRt1Col = hv_ColAll.TupleSelect(hv_NumUp);
                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PRt2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PRt2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PRt1Row, hv_PRt1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PRt2Row, hv_PRt2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineRT, hv_PRt1Row.TupleConcat(hv_PRt2Row), hv_PRt1Col.TupleConcat(hv_PRt2Col));
                RectangleF rectLineRT = new RectangleF((float)hv_PRt1Col.D, (float)hv_PRt1Row.D, (float)hv_PRt2Col.D, (float)hv_PRt2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineRT);
                listObj2Draw.Add("OK");
                #endregion

                HTuple hv_RowRD, hv_ColumnRD, hv_RowRU, hv_ColumnRU, hv_ColumnLD, hv_RowLD, hv_ColumnLU, hv_RowLU;
                #region *************计算四个基板边缘线交点*************
                //*左上交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowLU, out hv_ColumnLU,
                out hv_IsOverlapping);
                //*左下交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowLD, out hv_ColumnLD,
                out hv_IsOverlapping);
                //*右上交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowRU, out hv_ColumnRU,
                out hv_IsOverlapping);
                //*右下交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowRD, out hv_ColumnRD,
                out hv_IsOverlapping);
                #endregion

                HTuple hv_Dist9, hv_Dist10, hv_Dist11, hv_Dist12;
                #region *************计算边缘线边长*************
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowRU, hv_ColumnRU, out hv_Dist9);   //上边长
                HOperatorSet.DistancePp(hv_RowLD, hv_ColumnLD, hv_RowRD, hv_ColumnRD, out hv_Dist10);  //下边长
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, out hv_Dist11);  //左边长
                HOperatorSet.DistancePp(hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD, out hv_Dist12);  //右边长
                #endregion

                HTuple hv_ColumnKKK1, hv_RowKKK1, hv_Dist1, hv_ColumnKKK2, hv_RowKKK2, hv_Dist2, hv_ColumnKKK3, hv_RowKKK3, hv_Dist3, hv_ColumnKKK4, hv_RowKKK4, hv_Dist4, hv_ColumnKKK5, hv_RowKKK5, hv_Dist5, hv_ColumnKKK6, hv_RowKKK6, hv_Dist6, hv_ColumnKKK7, hv_RowKKK7, hv_Dist7, hv_ColumnKKK8, hv_RowKKK8, hv_Dist8;
                HObject ho_CrossKKK1, ho_CrossKKK2, ho_CrossKKK3, ho_CrossKKK4, ho_CrossKKK5, ho_CrossKKK6, ho_CrossKKK7, ho_CrossKKK8;
                #region *************计算剥离线交点到基板边缘距离*************
                //*计算左上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK1,
                out hv_ColumnKKK1, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK1, hv_RowKKK1, hv_ColumnKKK1,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK1, hv_ColumnKKK1,
                out hv_Dist1);
                //*计算左上剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK2,
                out hv_ColumnKKK2, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK2, hv_RowKKK2, hv_ColumnKKK2,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK2, hv_ColumnKKK2,
                out hv_Dist2);
                //*计算左下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK3,
                out hv_ColumnKKK3, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK3, hv_RowKKK3, hv_ColumnKKK3,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK3, hv_ColumnKKK3,
                out hv_Dist3);
                //*计算左下剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK4,
                out hv_ColumnKKK4, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK4, hv_RowKKK4, hv_ColumnKKK4,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK4, hv_ColumnKKK4,
                out hv_Dist4);
                //*计算右上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK5,
                out hv_ColumnKKK5, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK5, hv_RowKKK5, hv_ColumnKKK5,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK5, hv_ColumnKKK5,
                out hv_Dist5);
                //*计算右上剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK6,
                out hv_ColumnKKK6, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK6, hv_RowKKK6, hv_ColumnKKK6,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK6, hv_ColumnKKK6,
                out hv_Dist6);
                //*计算右下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK7,
                out hv_ColumnKKK7, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK7, hv_RowKKK7, hv_ColumnKKK7,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK7, hv_ColumnKKK7,
                out hv_Dist7);
                //*计算右下剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK8,
                out hv_ColumnKKK8, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK8, hv_RowKKK8, hv_ColumnKKK8,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK8, hv_ColumnKKK8,
                out hv_Dist8);
                #endregion

                HTuple hv_DistF1Sum, hv_DistF1, hv_DistF2Sum, hv_DistF2;
                #region *************计算F1 F2 *************
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_Rowccc, hv_Columnccc, out hv_DistF1Sum);
                hv_DistF1 = hv_DistF1Sum / 44;
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_Rowddd, hv_Columnddd, out hv_DistF2Sum);
                hv_DistF2 = hv_DistF2Sum / 44;
                #endregion


                //返回距离值
                string RetStr = "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +      // L R 
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#"

                                + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"  //剥离线交点坐标 X Y
                                + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                                + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                                + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000") + "#"

                              + hv_Dist1.D.ToString("0.0000") + "#" + hv_Dist2.D.ToString("0.0000") + "#"  //D
                              + hv_Dist3.D.ToString("0.0000") + "#" + hv_Dist4.D.ToString("0.0000") + "#"
                              + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#"
                              + hv_Dist7.D.ToString("0.0000") + "#" + hv_Dist8.D.ToString("0.0000") + "#"

                              + hv_Dist9.D.ToString("0.0000") + "#" + hv_Dist10.D.ToString("0.0000") + "#"  //W H 
                              + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"

                              + hv_DistF1.D.ToString("0.0000") + "#" + hv_DistF2.D.ToString("0.0000");      //F1 F2

                //dhDll.frmMsg.Log("syPrintCheck0603_0", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;
            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck0603_0" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }
            #endregion
        }
        public static List<object> syPrintCheck0603_0_1(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 0603后端对位  ***

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {

                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);                 //粗定位阈值  25
                int ithresCorner = int.Parse(strUserParam[5]);              //定位角阈值  20


                int hv_MaxLineWidth = int.Parse(strUserParam[8]);           //高斯线宽    5
                int hv_Contrast = int.Parse(strUserParam[9]);               //高斯对比度  5

                int iHengRectLength1 = int.Parse(strUserParam[10]);         //横向距离矩形半宽 40
                int iHengRectLength2 = int.Parse(strUserParam[11]);         //横向距离矩形半高 2

                int iHengRectThres = int.Parse(strUserParam[12]);           //横向距离矩形阈值 25
                int iStandHengSum = int.Parse(strUserParam[13]);            //横向距离标准值   20    16~24   不使用
                int iParallNum = int.Parse(strUserParam[14]);               //横向距离平行线偏移值   1      

                float iMinScore = float.Parse(strUserParam[16]);               //边缘最小得分 0.2
                int iMeasureThreshold = int.Parse(strUserParam[17]);           //边缘阈值20
                int iNumMeasures = int.Parse(strUserParam[18]);                //边缘卡尺个数 200

                int iLength1 = int.Parse(strUserParam[20]);                  //横向剥离线区域半宽  20
                int iLength2 = int.Parse(strUserParam[21]);                  //横向剥离线区域半高  25

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************
                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);  //粗定位阈值
                HOperatorSet.OpeningCircle(ho_RegionBlack, out ho_RegionBlackOpen, 2);
                HOperatorSet.Connection(ho_RegionBlackOpen, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_SelectedRegionsholes, "holes_num", "and", 50, 99999);
                HOperatorSet.CountObj(ho_SelectedRegionsholes, out hv_Num);
                if ((int)(new HTuple(hv_Num.TupleEqual(0))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.SelectShapeStd(ho_SelectedRegionsholes, out ho_RegionR, "max_area", 70);
                HOperatorSet.FillUp(ho_RegionR, out ho_RegionFillRRR);
                HOperatorSet.AreaCenter(ho_RegionFillRRR, out hv_areaRRR, out hv_rowRRR, out hv_colRRR);
                if ((int)(new HTuple(hv_areaRRR.TupleLess(10000000))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.ReduceDomain(hoImage, ho_RegionFillRRR, out ho_ImageRRR);
                #endregion

                #region ****** 寻找左上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1238)).TupleConcat(568)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1161)).TupleConcat(1260)).TupleConcat(660));
                HOperatorSet.ReduceDomain(ho_ImageRRR, ho_Rect1, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, ithresCorner); //定位角阈值
                HOperatorSet.OpeningRectangle1(ho_Region, out ho_RegionOpening1, 20, 3);
                HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions);
                HOperatorSet.FillUp(ho_ConnectedRegions, out ho_RegionFills);
                HOperatorSet.SelectShapeStd(ho_RegionFills, out ho_SelectedRegions, "max_area", 70);
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1,
                    out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 40, hv_LUPCol + 100, hv_Phi1, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow + 15, hv_LUPCol - 20, hv_Phi1, iLength1, iLength2);
                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(3060)).TupleConcat(
                    3534)).TupleConcat(3550)).TupleConcat(3210)).TupleConcat(3060), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(1600)).TupleConcat(1172)).TupleConcat(665));
                HOperatorSet.ReduceDomain(ho_ImageRRR, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, ithresCorner);   //定位角阈值
                HOperatorSet.OpeningRectangle1(ho_Region, out ho_RegionOpening1, 20, 3);
                HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions);
                HOperatorSet.FillUp(ho_ConnectedRegions, out ho_RegionFills);
                HOperatorSet.SelectShapeStd(ho_RegionFills, out ho_SelectedRegions, "max_area", 70);
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row2, out hv_Column2,
                    out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2,
                    hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2,
                    out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30,
                    0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 40, hv_LDPCol + 100,
                    hv_Phi2, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow - 15, hv_LDPCol - 25,
                    hv_Phi2, iLength1, iLength2);

                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3810)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3810));
                HOperatorSet.ReduceDomain(ho_ImageRRR, ho_Rect3, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, ithresCorner); //定位角阈值
                HOperatorSet.OpeningRectangle1(ho_Region, out ho_RegionOpening1, 20, 3);
                HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions);
                HOperatorSet.FillUp(ho_ConnectedRegions, out ho_RegionFills);
                HOperatorSet.SelectShapeStd(ho_RegionFills, out ho_SelectedRegions, "max_area", 70);
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row2, out hv_Column2,
                    out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3,
                    hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3,
                    out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30,
                    0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 40, hv_RUPCol - 90,
                    hv_Phi3, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow + 15, hv_RUPCol + 25,
                    hv_Phi3, iLength1, iLength2);


                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2991)).TupleConcat(3159)).TupleConcat(3546), ((((new HTuple(3912)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4224)).TupleConcat(3912));
                HOperatorSet.ReduceDomain(ho_ImageRRR, ho_Rect4, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, ithresCorner); //定位角阈值
                HOperatorSet.OpeningRectangle1(ho_Region, out ho_RegionOpening1, 20, 3);
                HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions);
                HOperatorSet.FillUp(ho_ConnectedRegions, out ho_RegionFills);
                HOperatorSet.SelectShapeStd(ho_RegionFills, out ho_SelectedRegions, "max_area", 70);
                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row2, out hv_Column2,
                    out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4,
                    hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4,
                    out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30,
                    0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 40, hv_RDPCol - 90,
                    hv_Phi4, 25, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow - 15, hv_RDPCol + 25,
                    hv_Phi4, iLength1, iLength2);
                #endregion


                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce1, out ho_Line1, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line1, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上横向剥离线无定位";
                    ////绘制矩形
                    //List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Col, hv_Phi, hv_Length1, hv_Length2);
                    //listObj2Draw.Add("多边形");
                    //listObj2Draw.Add(lnBarcode.ToArray());
                    //listObj2Draw.Add("OK");
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line1, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon", 1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours, "direction", hv_Phi1 - hv_Eps, hv_Phi1 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_MaxLine1);


                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce2, out ho_Line2, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line2, out hv_Num2);
                if ((int)(new HTuple(hv_Num2.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line2, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon", 1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi1 + (hv_pi / 2)) - hv_Eps, (hv_Phi1 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine2, out ho_MaxLine2);

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma, out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce3, out ho_Line3, hv_Sigma, hv_Low, hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line3, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-左下横向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line3, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon", 1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours, "direction", hv_Phi2 - hv_Eps, hv_Phi2 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-左下横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine3, out ho_MaxLine3);

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce4, out ho_Line4, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line4, out hv_Num2);
                if ((int)(new HTuple(hv_Num2.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line4, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);

                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi2 + (hv_pi / 2)) - hv_Eps, (hv_Phi2 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine4, out ho_MaxLine4);

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce5, out ho_Line5, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line5, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右上横向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line5, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", hv_Phi3 - hv_Eps, hv_Phi3 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右上横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine5, out ho_MaxLine5);

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce6, out ho_Line6, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line6, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line6, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi3 + (hv_pi / 2)) - hv_Eps, (hv_Phi3 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine6, out ho_MaxLine6);

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce7, out ho_Line7, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line7, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右下横向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line7, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", hv_Phi4 - hv_Eps, hv_Phi4 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右下横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine7, out ho_MaxLine7);

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma, out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce8, out ho_Line8, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line8, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                    return listObj2Draw;
                }
                //*select_contours_xld (Line8, SelectedContours, 'direction', 0.5, 200, -0.5, 0.5)
                HOperatorSet.GenPolygonsXld(ho_Line8, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi4 + (hv_pi / 2)) - hv_Eps, (hv_Phi4 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine8, out ho_MaxLine8);

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_UHPoint1Row = new HTuple();
                HTuple hv_UHPoint1Col = new HTuple();
                HTuple hv_UHPoint2Row = new HTuple();
                HTuple hv_UHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_UHPoint1Row.TupleConcat(hv_UHPoint2Row), hv_UHPoint1Col.TupleConcat(hv_UHPoint2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_UHPoint1Col.D, (float)hv_UHPoint1Row.D, (float)hv_UHPoint2Col.D, (float)hv_UHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_DHPoint1Row = new HTuple();
                HTuple hv_DHPoint1Col = new HTuple();
                HTuple hv_DHPoint2Row = new HTuple();
                HTuple hv_DHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_DHPoint1Row.TupleConcat(hv_DHPoint2Row), hv_DHPoint1Col.TupleConcat(hv_DHPoint2Col));
                RectangleF rectLine2 = new RectangleF((float)hv_DHPoint1Col.D, (float)hv_DHPoint1Row.D, (float)hv_DHPoint2Col.D, (float)hv_DHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_LVPoint1Row = new HTuple();
                HTuple hv_LVPoint1Col = new HTuple();
                HTuple hv_LVPoint2Row = new HTuple();
                HTuple hv_LVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_LVPoint1Row.TupleConcat(hv_LVPoint2Row), hv_LVPoint1Col.TupleConcat(hv_LVPoint2Col));
                RectangleF rectLine3 = new RectangleF((float)hv_LVPoint1Col.D, (float)hv_LVPoint1Row.D, (float)hv_LVPoint2Col.D, (float)hv_LVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_RVPoint1Row = new HTuple();
                HTuple hv_RVPoint1Col = new HTuple();
                HTuple hv_RVPoint2Row = new HTuple();
                HTuple hv_RVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_RVPoint1Row.TupleConcat(hv_RVPoint2Row), hv_RVPoint1Col.TupleConcat(hv_RVPoint2Col));
                RectangleF rectLine4 = new RectangleF((float)hv_RVPoint1Col.D, (float)hv_RVPoint1Row.D, (float)hv_RVPoint2Col.D, (float)hv_RVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion

                HObject ho_Cross12, ho_Cross11, ho_Cross10, ho_Line10, ho_Cross8, ho_Cross9, ho_Cross7, ho_RVLinePara, ho_Cross6, ho_Cross5, ho_DHLinePara, ho_Cross4, ho_Cross3, ho_Cross2, ho_UHLinePara, ho_ImageEmph, ho_LVLinePara, ho_Cross, ho_Edges, ho_XXX, ho_LongEdge, ho_Cross1;
                HTuple hv_Select, hv_Dist11, hv_Transition, hv_Dist12, hv_Dist10, hv_Dist8, hv_Dist9, hv_Dist7, hv_Dist5, hv_Dist6, hv_Dist4, hv_Dist2, hv_Dist3, hv_RowEdge, hv_ColumnEdge, hv_Amplitude, hv_Distance, hv_MeasureHandle, hv_Row, hv_Column, hv_IsOverlapping, hv_Row1, hv_Column1, hv_Dist1;
                HTuple hv_RowEdgeFirst, hv_ColumnEdgeFirst, hv_AmplitudeFirst, hv_RowEdgeSecond, hv_ColumnEdgeSecond, hv_AmplitudeSecond, hv_IntraDistance, hv_InterDistance;

                HOperatorSet.Emphasize(hoImage, out hoImage, 4, 4, 2);

                PointF[] ptsLeft = new PointF[12];
                hv_Transition = "positive";
                hv_Select = "all";

                //横向矩形参数
                HTuple hv_Eps2 = hv_Eps / 2;

                HTuple hv_RowP, hv_ColumnP, hv_DistanceMax, hv_RowCross, hv_ColumnCross, hv_Parameter, hv_MetrologyHandle, hv_RowQQQ, hv_ColumnQQQ, hv_A, hv_B, hv_N, hv_x, hv_Col2, hv_Col1, hv_Index;
                HObject ho_RVLinePara1, ho_RVLinePara2, ho_BigLine, ho_Contours, ho_Contour, ho_LVLinePara1, ho_LVLinePara2, ho_RegionL, ho_SortedRegions, ho_Rect;

                #region*********************************计算左上三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara1, "regression_normal", 50);
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara2, "regression_normal", -100);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                HTuple hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左上竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi1 - hv_Eps2, hv_Phi1 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist1, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_LVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[0] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[0]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleLess(hv_RowP))) != 0)
                {
                    hv_Dist1 = 0 - hv_Dist1;
                }

                HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", -hv_Dist1 + iParallNum);
                HOperatorSet.IntersectionContoursXld(ho_LVLine, ho_UHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi1, iHengRectLength1, iHengRectLength2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, iHengRectThres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左上横向距离失败";
                    return listObj2Draw;
                }

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ;

                //HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad()  );
                ptsLeft[1] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[1]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = hv_RowQQQ + 1 + 0.3 * iParallNum;
                hv_Col2 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad());
                ptsLeft[2] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[2]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_LVLine, hv_Row1, hv_Col1, out hv_Dist2, out hv_x);
                HOperatorSet.DistancePc(ho_LVLine, hv_Row2, hv_Col2, out hv_Dist3, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist2 = 0 - hv_Dist2;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist3 = 0 - hv_Dist3;
                }

                #endregion

                #region*********************************计算左下三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara1, "regression_normal", 50);
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara2, "regression_normal", -100);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左下竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi2 - hv_Eps2, hv_Phi2 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist4, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_LVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[3] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[3]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleGreater(hv_RowP))) != 0)
                {
                    hv_Dist4 = 0 - hv_Dist4;
                }

                HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist4 - iParallNum);
                HOperatorSet.IntersectionContoursXld(ho_LVLine, ho_DHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi2, iHengRectLength1, iHengRectLength2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, iHengRectThres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad()  );
                ptsLeft[4] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[4]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = hv_RowQQQ + 1 + 0.3 * iParallNum;
                hv_Col2 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad()  );
                ptsLeft[5] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[5]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_LVLine, hv_Row1, hv_Col1, out hv_Dist5, out hv_x);
                HOperatorSet.DistancePc(ho_LVLine, hv_Row2, hv_Col2, out hv_Dist6, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist5 = 0 - hv_Dist5;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist6 = 0 - hv_Dist6;
                }

                #endregion

                #region*********************************计算右上三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara1, "regression_normal", 100);
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara2, "regression_normal", -50);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 - 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算右上竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi3 - hv_Eps2, hv_Phi3 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist7, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_RVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[6] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[6]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_RowCross.TupleLess(hv_RowP))) != 0)
                {
                    hv_Dist7 = 0 - hv_Dist7;
                }

                HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", -hv_Dist7 + iParallNum);
                HOperatorSet.IntersectionContoursXld(ho_RVLine, ho_UHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi3, iHengRectLength1, iHengRectLength2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, iHengRectThres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross8, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad() );
                ptsLeft[7] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[7]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = hv_RowQQQ + 1 + 0.3 * iParallNum;
                hv_Col2 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross9, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad() );
                ptsLeft[8] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[8]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_RVLine, hv_Row1, hv_Col1, out hv_Dist8, out hv_x);
                HOperatorSet.DistancePc(ho_RVLine, hv_Row2, hv_Col2, out hv_Dist9, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist8 = 0 - hv_Dist8;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist9 = 0 - hv_Dist9;
                }

                #endregion

                #region*********************************计算右下三个距离**********************************
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara1, "regression_normal", 100);
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara2, "regression_normal", -50);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLinePara1, "all", out hv_Row1, out hv_Column1, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLinePara2, "all", out hv_Row2, out hv_Column2, out hv_IsOverlapping);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row1 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column1);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Row2 + 10);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Column2);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", 50);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", 30);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-计算右下竖向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectContoursXld(ho_Contour, out ho_SelectedContours, "direction", hv_Phi4 - hv_Eps2, hv_Phi4 + hv_Eps2, -0.5, 0.5);
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_BigLine);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_RowCross, out hv_ColumnCross, out hv_IsOverlapping);
                HOperatorSet.DistancePc(ho_BigLine, hv_RowCross, hv_ColumnCross, out hv_Dist10, out hv_DistanceMax);
                HOperatorSet.IntersectionContoursXld(ho_BigLine, ho_RVLine, "all", out hv_RowP, out hv_ColumnP, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross10, hv_RowP, hv_ColumnP, 6, (new HTuple(45)).TupleRad());
                ptsLeft[9] = new PointF((float)hv_ColumnP.DArr[0], (float)hv_RowP.DArr[0]);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[9]);
                listObj2Draw.Add("OK");

                if ((int)(new HTuple(hv_Row.TupleLess(hv_Row1))) != 0)
                {
                    hv_Dist10 = 0 - hv_Dist10;
                }

                HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist10 - iParallNum);
                HOperatorSet.IntersectionContoursXld(ho_RVLine, ho_DHLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect, hv_Row, hv_Column, hv_Phi4, iHengRectLength1, iHengRectLength2);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, iHengRectThres);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                HOperatorSet.SortRegion(ho_ConnectedRegions, out ho_SortedRegions, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleLess(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionL, 1);
                HOperatorSet.SmallestRectangle1(ho_RegionL, out hv_A, out hv_B, out hv_RowQQQ, out hv_ColumnQQQ);
                hv_Row1 = hv_RowQQQ - 1;
                hv_Col1 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_Row1, hv_Col1, 6, (new HTuple(45)).TupleRad() );
                ptsLeft[10] = new PointF((float)hv_Col1.D, (float)hv_Row1.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[10]);
                listObj2Draw.Add("OK");

                HOperatorSet.SelectObj(ho_SortedRegions, out ho_RegionR, hv_N);
                HOperatorSet.SmallestRectangle1(ho_RegionR, out hv_RowQQQ, out hv_ColumnQQQ, out hv_A, out hv_B);
                hv_Row2 = hv_RowQQQ + 1 + 0.3 * iParallNum;
                hv_Col2 = hv_ColumnQQQ;
                //HOperatorSet.GenCrossContourXld(out ho_Cross12, hv_Row2, hv_Col2, 6, (new HTuple(45)).TupleRad() );
                ptsLeft[11] = new PointF((float)hv_Col2.D, (float)hv_Row2.D);
                listObj2Draw.Add("小十字");
                listObj2Draw.Add(ptsLeft[11]);
                listObj2Draw.Add("OK");

                HOperatorSet.DistancePc(ho_RVLine, hv_Row1, hv_Col1, out hv_Dist11, out hv_x);
                HOperatorSet.DistancePc(ho_RVLine, hv_Row2, hv_Col2, out hv_Dist12, out hv_x);
                if ((int)(new HTuple(hv_Column.TupleLess(hv_Col1))) != 0)
                {
                    hv_Dist11 = 0 - hv_Dist11;
                }
                if ((int)(new HTuple(hv_Column.TupleGreater(hv_Col2))) != 0)
                {
                    hv_Dist12 = 0 - hv_Dist12;
                }

                #endregion

                HTuple hv_Rowaaa, hv_Columnaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;

                #region *************计算四个交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);
                #endregion

                //Metrology参数
                HTuple hv_PRt2Row, hv_PRt2Col, hv_PRt1Row, hv_PRt1Col, hv_PLf2Row, hv_PLf2Col, hv_PLf1Row, hv_PLf1Col, hv_PDn2Row, hv_PDn2Col, hv_PDn1Row, hv_PDn1Col, hv_PUp2Row, hv_PUp2Col, hv_PUp1Row, hv_PUp1Col, hv_RowR, hv_ColR, hv_RowL, hv_ColL;
                HObject ho_LineRT, ho_LineLF, ho_LineDN, ho_CrossLineUPddd, ho_CrossLineUPbbb, ho_LineUP, ho_CrossL, ho_CrossR, ho_ContourL, ho_ContourR, ho_SortedContours, ho_CrossLineUPaaa, ho_CrossLineUPccc;

                #region *************Metrology找基板上边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa - 110, hv_Columnaaa, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc - 110, hv_Columnccc, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa - 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc - 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板上边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PUp1Row = hv_RowL.TupleSelect(0);
                hv_PUp1Col = hv_ColL.TupleSelect(0);
                hv_PUp2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PUp2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PUp1Row, hv_PUp1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PUp2Row, hv_PUp2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineUP, hv_PUp1Row.TupleConcat(hv_PUp2Row), hv_PUp1Col.TupleConcat(hv_PUp2Col));
                RectangleF rectLineUP = new RectangleF((float)hv_PUp1Col.D, (float)hv_PUp1Row.D, (float)hv_PUp2Col.D, (float)hv_PUp2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineUP);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板下边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb + 110, hv_Columnbbb, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd + 110, hv_Columnddd, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb + 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd + 110);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板下边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PDn1Row = hv_RowL.TupleSelect(0);
                hv_PDn1Col = hv_ColL.TupleSelect(0);
                hv_PDn2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PDn2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PDn1Row, hv_PDn1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PDn2Row, hv_PDn2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineDN, hv_PDn1Row.TupleConcat(hv_PDn2Row), hv_PDn1Col.TupleConcat(hv_PDn2Col));
                RectangleF rectLineDN = new RectangleF((float)hv_PDn1Col.D, (float)hv_PDn1Row.D, (float)hv_PDn2Col.D, (float)hv_PDn2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineDN);
                listObj2Draw.Add("OK");

                #endregion

                #region *************Metrology找基板左边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa, hv_Columnaaa - 190, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb, hv_Columnbbb - 190, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa - 190);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb - 190);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板左边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left", "true", "row");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PLf1Row = hv_RowL.TupleSelect(0);
                hv_PLf1Col = hv_ColL.TupleSelect(0);
                hv_PLf2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PLf2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PLf1Row, hv_PLf1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PLf2Row, hv_PLf2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineLF, hv_PLf1Row.TupleConcat(hv_PLf2Row), hv_PLf1Col.TupleConcat(hv_PLf2Col));
                RectangleF rectLineLF = new RectangleF((float)hv_PLf1Col.D, (float)hv_PLf1Row.D, (float)hv_PLf2Col.D, (float)hv_PLf2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineLF);
                listObj2Draw.Add("OK");

                #endregion

                #region *************Metrology找基板右边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc, hv_Columnccc + 190,
                    60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd, hv_Columnddd + 190,
                    60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc + 190);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd + 190);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板右边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left",
                    "true", "row");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PRt1Row = hv_RowL.TupleSelect(0);
                hv_PRt1Col = hv_ColL.TupleSelect(0);
                hv_PRt2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PRt2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PRt1Row, hv_PRt1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PRt2Row, hv_PRt2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineRT, hv_PRt1Row.TupleConcat(hv_PRt2Row), hv_PRt1Col.TupleConcat(hv_PRt2Col));
                RectangleF rectLineRT = new RectangleF((float)hv_PRt1Col.D, (float)hv_PRt1Row.D, (float)hv_PRt2Col.D, (float)hv_PRt2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineRT);
                listObj2Draw.Add("OK");

                #endregion

                HTuple hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD;

                #region *************计算四个基板边缘线交点*************
                //*左上交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                    hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowLU, out hv_ColumnLU,
                    out hv_IsOverlapping);
                //*左下交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                    hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowLD, out hv_ColumnLD,
                    out hv_IsOverlapping);
                //*右上交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                    hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowRU, out hv_ColumnRU,
                    out hv_IsOverlapping);
                //*右下交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                    hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowRD, out hv_ColumnRD,
                    out hv_IsOverlapping);
                #endregion

                HTuple hv_Dist13, hv_Dist14, hv_Dist15, hv_Dist16, hv_Dist21, hv_Dist22, hv_Dist23, hv_Dist24;
                #region *************计算边缘线边长*************
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowRU, hv_ColumnRU, out hv_Dist21);   //上边长
                HOperatorSet.DistancePp(hv_RowLD, hv_ColumnLD, hv_RowRD, hv_ColumnRD, out hv_Dist22);   //下边长
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, out hv_Dist23);   //左边长
                HOperatorSet.DistancePp(hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD, out hv_Dist24);   //右边长
                #endregion

                HTuple hv_ColumnKKK1, hv_RowKKK1, hv_Dist17, hv_ColumnKKK2, hv_RowKKK2, hv_Dist18, hv_ColumnKKK3, hv_RowKKK3, hv_Dist19, hv_ColumnKKK4, hv_RowKKK4, hv_Dist20, hv_ColumnKKK5, hv_RowKKK5, hv_ColumnKKK6, hv_RowKKK6, hv_ColumnKKK7, hv_RowKKK7, hv_ColumnKKK8, hv_RowKKK8;
                HObject ho_CrossKKK1, ho_CrossKKK2, ho_CrossKKK3, ho_CrossKKK4, ho_CrossKKK5, ho_CrossKKK6, ho_CrossKKK7, ho_CrossKKK8;
                #region *************计算剥离线交点到基板边缘距离*************
                //*计算左上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK1,
                out hv_ColumnKKK1, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK1, hv_RowKKK1, hv_ColumnKKK1,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK1, hv_ColumnKKK1,
                out hv_Dist13);
                //*计算左上剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK2,
                out hv_ColumnKKK2, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK2, hv_RowKKK2, hv_ColumnKKK2,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK2, hv_ColumnKKK2,
                out hv_Dist14);
                //*计算左下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK3,
                out hv_ColumnKKK3, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK3, hv_RowKKK3, hv_ColumnKKK3,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK3, hv_ColumnKKK3,
                out hv_Dist15);
                //*计算左下剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK4,
                out hv_ColumnKKK4, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK4, hv_RowKKK4, hv_ColumnKKK4,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK4, hv_ColumnKKK4,
                out hv_Dist16);
                //*计算右上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK5,
                out hv_ColumnKKK5, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK5, hv_RowKKK5, hv_ColumnKKK5,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK5, hv_ColumnKKK5,
                out hv_Dist17);
                //*计算右上剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK6,
                out hv_ColumnKKK6, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK6, hv_RowKKK6, hv_ColumnKKK6,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK6, hv_ColumnKKK6,
                out hv_Dist18);
                //*计算右下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK7,
                out hv_ColumnKKK7, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK7, hv_RowKKK7, hv_ColumnKKK7,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK7, hv_ColumnKKK7,
                out hv_Dist19);
                //*计算右下剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK8,
                out hv_ColumnKKK8, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK8, hv_RowKKK8, hv_ColumnKKK8,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK8, hv_ColumnKKK8,
                out hv_Dist20);
                #endregion

                HTuple hv_DistF1Sum, hv_DistF1, hv_DistF2Sum, hv_DistF2;
                #region *************计算F1 F2 *************
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_Rowccc, hv_Columnccc, out hv_DistF1Sum);
                hv_DistF1 = hv_DistF1Sum / 42;
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_Rowddd, hv_Columnddd, out hv_DistF2Sum);
                hv_DistF2 = hv_DistF2Sum / 42;
                #endregion


                //返回距离值
                string RetStr = hv_Dist1.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist2.D.ToString("0.0000") + "#" + hv_Dist3.D.ToString("0.0000") + "#"     //  LR
                              + hv_Dist4.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#"
                              + hv_Dist7.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist8.D.ToString("0.0000") + "#" + hv_Dist9.D.ToString("0.0000") + "#"
                              + hv_Dist10.D.ToString("0.0000") + "#" + "-10000" + "#" + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"

                            + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"  //剥离线交点坐标 X Y
                            + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                            + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                            + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000") + "#"

                              + hv_Dist13.D.ToString("0.0000") + "#" + hv_Dist14.D.ToString("0.0000") + "#"  // D
                              + hv_Dist15.D.ToString("0.0000") + "#" + hv_Dist16.D.ToString("0.0000") + "#"
                              + hv_Dist17.D.ToString("0.0000") + "#" + hv_Dist18.D.ToString("0.0000") + "#"
                              + hv_Dist19.D.ToString("0.0000") + "#" + hv_Dist20.D.ToString("0.0000") + "#"

                            + hv_Dist21.D.ToString("0.0000") + "#" + hv_Dist22.D.ToString("0.0000") + "#"    // W H 
                            + hv_Dist23.D.ToString("0.0000") + "#" + hv_Dist24.D.ToString("0.0000") + "#"

                            + hv_DistF1.D.ToString("0.0000") + "#" + hv_DistF2.D.ToString("0.0000");      //F1 F2;

                dhDll.frmMsg.Log("syPrintCheck0603_0_1", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck0603_0_1" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }

            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }

            #endregion
        }

        public static List<object> syPrintCheck1206_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 1206前端对位  ***

            if (bUseMutex) muDetect8.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);
            try
            {
                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);                 //粗定位阈值  20
                int hv_MaxLineWidth = int.Parse(strUserParam[8]);           //高斯线宽    5
                int hv_Contrast = int.Parse(strUserParam[9]);               //高斯对比度  5  
                int iLength1 = int.Parse(strUserParam[10]);                  //横向剥离线区域半宽  30
                int iLength2 = int.Parse(strUserParam[11]);                  //横向剥离线区域半高  30
                float iMinScore = float.Parse(strUserParam[12]);            //边缘最小得分 0.1
                int iMeasureTthreshold = int.Parse(strUserParam[13]);       //边缘阈值     20
                int iNumMeasures = int.Parse(strUserParam[14]);             //边缘卡尺个数 200

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_RegionClosing, ho_RegInters, ho_ImageCenter, ho_CenterReg, ho_CenterRegs, ho_BiggerRegs, ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_N, hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************
                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);  //粗定位阈值  20
                HOperatorSet.OpeningRectangle1(ho_RegionBlack, out ho_RegionBlackOpen, 8, 4);
                HOperatorSet.ClosingCircle(ho_RegionBlackOpen, out ho_RegionClosing, 3.5);
                HOperatorSet.Connection(ho_RegionClosing, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_BiggerRegs, "area", "and", 10000, 9999999999999);
                HOperatorSet.SelectShape(ho_BiggerRegs, out ho_CenterRegs, "width", "and", (4486 - 840) - 500, (4486 - 840) + 500);
                HOperatorSet.CountObj(ho_CenterRegs, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.Union1(ho_CenterRegs, out ho_CenterReg);
                HOperatorSet.ReduceDomain(hoImage, ho_CenterReg, out ho_ImageCenter);
                #endregion

                //0402两次寻找剥离线

                #region ****** 寻找左上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1538)).TupleConcat(649)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1337)).TupleConcat(1460)).TupleConcat(660));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect1, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1,
                    out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 50, hv_LUPCol + 10, hv_Phi1, 40, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow - 5, hv_LUPCol - 30, hv_Phi1, iLength1, iLength2);
                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(2760)).TupleConcat(
                    3534)).TupleConcat(3534)).TupleConcat(2952)).TupleConcat(2760), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(2100)).TupleConcat(1506)).TupleConcat(665));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect2, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 50, hv_LDPCol + 10, hv_Phi2, 40, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow + 5, hv_LDPCol - 30, hv_Phi2, iLength1, iLength2);
                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3610)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3610));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect3, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 50, hv_RUPCol - 10, hv_Phi3, 40, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow - 5, hv_RUPCol + 30, hv_Phi3, iLength1, iLength2);
                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2791)).TupleConcat(3013)).TupleConcat(3546), ((((new HTuple(3412)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4062)).TupleConcat(3412));
                HOperatorSet.Intersection(ho_CenterReg, ho_Rect4, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 50, hv_RDPCol - 10, hv_Phi4, 40, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow + 5, hv_RDPCol + 30, hv_Phi4, iLength1, iLength2);
                #endregion


                HTuple hv_LightDark = "light";
                HTuple hv_Success;

                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);

                f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast,
                 hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);

                f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);

                f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);

                f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);

                f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);

                f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_UHPoint1Row = new HTuple();
                HTuple hv_UHPoint1Col = new HTuple();
                HTuple hv_UHPoint2Row = new HTuple();
                HTuple hv_UHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_UHPoint1Row.TupleConcat(hv_UHPoint2Row), hv_UHPoint1Col.TupleConcat(hv_UHPoint2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_UHPoint1Col.D, (float)hv_UHPoint1Row.D, (float)hv_UHPoint2Col.D, (float)hv_UHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_DHPoint1Row = new HTuple();
                HTuple hv_DHPoint1Col = new HTuple();
                HTuple hv_DHPoint2Row = new HTuple();
                HTuple hv_DHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_DHPoint1Row.TupleConcat(hv_DHPoint2Row), hv_DHPoint1Col.TupleConcat(hv_DHPoint2Col));
                RectangleF rectLine2 = new RectangleF((float)hv_DHPoint1Col.D, (float)hv_DHPoint1Row.D, (float)hv_DHPoint2Col.D, (float)hv_DHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_LVPoint1Row = new HTuple();
                HTuple hv_LVPoint1Col = new HTuple();
                HTuple hv_LVPoint2Row = new HTuple();
                HTuple hv_LVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_LVPoint1Row.TupleConcat(hv_LVPoint2Row), hv_LVPoint1Col.TupleConcat(hv_LVPoint2Col));
                RectangleF rectLine3 = new RectangleF((float)hv_LVPoint1Col.D, (float)hv_LVPoint1Row.D, (float)hv_LVPoint2Col.D, (float)hv_LVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_RVPoint1Row = new HTuple();
                HTuple hv_RVPoint1Col = new HTuple();
                HTuple hv_RVPoint2Row = new HTuple();
                HTuple hv_RVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_RVPoint1Row.TupleConcat(hv_RVPoint2Row), hv_RVPoint1Col.TupleConcat(hv_RVPoint2Col));
                RectangleF rectLine4 = new RectangleF((float)hv_RVPoint1Col.D, (float)hv_RVPoint1Row.D, (float)hv_RVPoint2Col.D, (float)hv_RVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion


                HTuple hv_IsOverlapping, hv_Columnaaa, hv_Rowaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;
                #region *************计算四个交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);
                #endregion

                //Metrology 参数
                HTuple hv_min_score = iMinScore;
                HTuple hv_measure_threshold = iMeasureTthreshold;
                HTuple hv_num_measures = iNumMeasures;
                HTuple hv_PRt2Row, hv_PRt2Col, hv_PRt1Row, hv_PRt1Col, hv_PLf2Row, hv_PLf2Col, hv_NumDn, hv_RowMax, hv_PLf1Col, hv_PLf1Row, hv_NumUp, hv_RowMin, hv_PDn2Row, hv_PDn2Col, hv_PDn1Row, hv_PDn1Col, hv_PUp2Col, hv_PUp2Row, hv_Numright, hv_ColMax, hv_PUp1Row, hv_PUp1Col, hv_Numleft, hv_ColMin, hv_RowTmp, hv_ColTmp, hv_I, hv_Parameter, hv_Column, hv_Row, hv_MetrologyHandle, hv_Index;
                HObject ho_LineRT, ho_LineLF, ho_LineDN, ho_CrossLineUPddd, ho_CrossLineUPbbb, ho_LineUP, ho_CrossL, ho_CrossR, ho_ContourTmp, ho_Contour, ho_Cross, ho_Contours, ho_CrossLineUPaaa, ho_CrossLineUPccc;

                #region *************Metrology找基板上边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa - 95, hv_Columnaaa, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc - 95, hv_Columnccc, 60, 0.5);
                HTuple hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa - 95);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc - 95);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板上边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                HTuple hv_RowAll = new HTuple();
                HTuple hv_ColAll = new HTuple();
                HTuple end_val430 = hv_N;
                HTuple step_val430 = 1;
                for (hv_I = 1; hv_I.Continue(end_val430, step_val430); hv_I = hv_I.TupleAdd(step_val430))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PUp1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PUp1Col = hv_ColAll.TupleSelect(hv_Numleft);
                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PUp2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PUp2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PUp1Row, hv_PUp1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PUp2Row, hv_PUp2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineUP, hv_PUp1Row.TupleConcat(hv_PUp2Row), hv_PUp1Col.TupleConcat(hv_PUp2Col));
                RectangleF rectLineUP = new RectangleF((float)hv_PUp1Col.D, (float)hv_PUp1Row.D, (float)hv_PUp2Col.D, (float)hv_PUp2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineUP);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板下边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb + 95, hv_Columnbbb, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd + 95, hv_Columnddd, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb + 95);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd + 95);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-寻找基板下边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val486 = hv_N;
                HTuple step_val486 = 1;
                for (hv_I = 1; hv_I.Continue(end_val486, step_val486); hv_I = hv_I.TupleAdd(step_val486))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_ColAll, out hv_ColMin);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMin, out hv_Numleft);
                if ((int)(new HTuple((new HTuple(hv_Numleft.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numleft = hv_Numleft.TupleSelect(0);
                }
                hv_PDn1Row = hv_RowAll.TupleSelect(hv_Numleft);
                hv_PDn1Col = hv_ColAll.TupleSelect(hv_Numleft);
                HOperatorSet.TupleMax(hv_ColAll, out hv_ColMax);
                HOperatorSet.TupleFind(hv_ColAll, hv_ColMax, out hv_Numright);
                if ((int)(new HTuple((new HTuple(hv_Numright.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_Numright = hv_Numright.TupleSelect(0);
                }
                hv_PDn2Row = hv_RowAll.TupleSelect(hv_Numright);
                hv_PDn2Col = hv_ColAll.TupleSelect(hv_Numright);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PDn1Row, hv_PDn1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PDn2Row, hv_PDn2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineDN, hv_PDn1Row.TupleConcat(hv_PDn2Row), hv_PDn1Col.TupleConcat(hv_PDn2Col));
                RectangleF rectLineDN = new RectangleF((float)hv_PDn1Col.D, (float)hv_PDn1Row.D, (float)hv_PDn2Col.D, (float)hv_PDn2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineDN);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板左边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa, hv_Columnaaa - 170, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb, hv_Columnbbb - 170, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa - 170);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb - 170);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板左边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val542 = hv_N;
                HTuple step_val542 = 1;
                for (hv_I = 1; hv_I.Continue(end_val542, step_val542); hv_I = hv_I.TupleAdd(step_val542))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PLf1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PLf1Col = hv_ColAll.TupleSelect(hv_NumUp);

                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PLf2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PLf2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PLf1Row, hv_PLf1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PLf2Row, hv_PLf2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineLF, hv_PLf1Row.TupleConcat(hv_PLf2Row), hv_PLf1Col.TupleConcat(hv_PLf2Col));
                RectangleF rectLineLF = new RectangleF((float)hv_PLf1Col.D, (float)hv_PLf1Row.D, (float)hv_PLf2Col.D, (float)hv_PLf2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineLF);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板右边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc, hv_Columnccc + 170, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd, hv_Columnddd + 170, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc + 170);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd + 170);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam, 20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures", hv_num_measures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1", 40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", hv_measure_threshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation", "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", hv_min_score);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板右边缘失败";
                    return listObj2Draw;
                }
                //*取出所有轮廓坐标，筛选左右两个坐标
                hv_RowAll = new HTuple();
                hv_ColAll = new HTuple();
                HTuple end_val599 = hv_N;
                HTuple step_val599 = 1;
                for (hv_I = 1; hv_I.Continue(end_val599, step_val599); hv_I = hv_I.TupleAdd(step_val599))
                {
                    HOperatorSet.SelectObj(ho_Contour, out ho_ContourTmp, hv_I);
                    HOperatorSet.GetContourXld(ho_ContourTmp, out hv_RowTmp, out hv_ColTmp);
                    HOperatorSet.TupleConcat(hv_RowTmp, hv_RowAll, out hv_RowAll);
                    HOperatorSet.TupleConcat(hv_ColTmp, hv_ColAll, out hv_ColAll);
                }
                HOperatorSet.TupleMin(hv_RowAll, out hv_RowMin);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMin, out hv_NumUp);
                if ((int)(new HTuple((new HTuple(hv_NumUp.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumUp = hv_NumUp.TupleSelect(0);
                }
                hv_PRt1Row = hv_RowAll.TupleSelect(hv_NumUp);
                hv_PRt1Col = hv_ColAll.TupleSelect(hv_NumUp);
                HOperatorSet.TupleMax(hv_RowAll, out hv_RowMax);
                HOperatorSet.TupleFind(hv_RowAll, hv_RowMax, out hv_NumDn);
                if ((int)(new HTuple((new HTuple(hv_NumDn.TupleLength())).TupleGreater(1))) != 0)
                {
                    hv_NumDn = hv_NumDn.TupleSelect(0);
                }
                hv_PRt2Row = hv_RowAll.TupleSelect(hv_NumDn);
                hv_PRt2Col = hv_ColAll.TupleSelect(hv_NumDn);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PRt1Row, hv_PRt1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PRt2Row, hv_PRt2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineRT, hv_PRt1Row.TupleConcat(hv_PRt2Row), hv_PRt1Col.TupleConcat(hv_PRt2Col));
                RectangleF rectLineRT = new RectangleF((float)hv_PRt1Col.D, (float)hv_PRt1Row.D, (float)hv_PRt2Col.D, (float)hv_PRt2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineRT);
                listObj2Draw.Add("OK");
                #endregion

                HTuple hv_RowRD, hv_ColumnRD, hv_RowRU, hv_ColumnRU, hv_ColumnLD, hv_RowLD, hv_ColumnLU, hv_RowLU;
                #region *************计算四个基板边缘线交点*************
                //*左上交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowLU, out hv_ColumnLU,
                out hv_IsOverlapping);
                //*左下交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowLD, out hv_ColumnLD,
                out hv_IsOverlapping);
                //*右上交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowRU, out hv_ColumnRU,
                out hv_IsOverlapping);
                //*右下交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowRD, out hv_ColumnRD,
                out hv_IsOverlapping);
                #endregion

                HTuple hv_Dist9, hv_Dist10, hv_Dist11, hv_Dist12;
                #region *************计算边缘线边长*************
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowRU, hv_ColumnRU, out hv_Dist9);   //上边长
                HOperatorSet.DistancePp(hv_RowLD, hv_ColumnLD, hv_RowRD, hv_ColumnRD, out hv_Dist10);  //下边长
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, out hv_Dist11);  //左边长
                HOperatorSet.DistancePp(hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD, out hv_Dist12);  //右边长
                #endregion

                HTuple hv_ColumnKKK1, hv_RowKKK1, hv_Dist1, hv_ColumnKKK2, hv_RowKKK2, hv_Dist2, hv_ColumnKKK3, hv_RowKKK3, hv_Dist3, hv_ColumnKKK4, hv_RowKKK4, hv_Dist4, hv_ColumnKKK5, hv_RowKKK5, hv_Dist5, hv_ColumnKKK6, hv_RowKKK6, hv_Dist6, hv_ColumnKKK7, hv_RowKKK7, hv_Dist7, hv_ColumnKKK8, hv_RowKKK8, hv_Dist8;
                HObject ho_CrossKKK1, ho_CrossKKK2, ho_CrossKKK3, ho_CrossKKK4, ho_CrossKKK5, ho_CrossKKK6, ho_CrossKKK7, ho_CrossKKK8;
                #region *************计算剥离线交点到基板边缘距离*************
                //*计算左上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK1,
                out hv_ColumnKKK1, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK1, hv_RowKKK1, hv_ColumnKKK1,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK1, hv_ColumnKKK1,
                out hv_Dist1);
                //*计算左上剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK2,
                out hv_ColumnKKK2, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK2, hv_RowKKK2, hv_ColumnKKK2,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK2, hv_ColumnKKK2,
                out hv_Dist2);
                //*计算左下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK3,
                out hv_ColumnKKK3, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK3, hv_RowKKK3, hv_ColumnKKK3,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK3, hv_ColumnKKK3,
                out hv_Dist3);
                //*计算左下剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK4,
                out hv_ColumnKKK4, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK4, hv_RowKKK4, hv_ColumnKKK4,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK4, hv_ColumnKKK4,
                out hv_Dist4);
                //*计算右上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK5,
                out hv_ColumnKKK5, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK5, hv_RowKKK5, hv_ColumnKKK5,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK5, hv_ColumnKKK5,
                out hv_Dist5);
                //*计算右上剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK6,
                out hv_ColumnKKK6, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK6, hv_RowKKK6, hv_ColumnKKK6,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK6, hv_ColumnKKK6,
                out hv_Dist6);
                //*计算右下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK7,
                out hv_ColumnKKK7, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK7, hv_RowKKK7, hv_ColumnKKK7,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK7, hv_ColumnKKK7,
                out hv_Dist7);
                //*计算右下剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK8,
                out hv_ColumnKKK8, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK8, hv_RowKKK8, hv_ColumnKKK8,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK8, hv_ColumnKKK8,
                out hv_Dist8);
                #endregion

                HTuple hv_DistF1Sum, hv_DistF1, hv_DistF2Sum, hv_DistF2;
                #region *************计算F1 F2 *************
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_Rowccc, hv_Columnccc, out hv_DistF1Sum);
                hv_DistF1 = hv_DistF1Sum / 21;
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_Rowddd, hv_Columnddd, out hv_DistF2Sum);
                hv_DistF2 = hv_DistF2Sum / 21;
                #endregion

                //返回距离值
                string RetStr =
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +      // L R 
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#" +
                                "-10000" + "#" + "-10000" + "#" + "-10000" + "#" + "-10000" + "#"

                                + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"  //剥离线交点坐标 X Y
                                + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                                + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                                + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000") + "#"

                              + hv_Dist1.D.ToString("0.0000") + "#" + hv_Dist2.D.ToString("0.0000") + "#"  // D
                              + hv_Dist3.D.ToString("0.0000") + "#" + hv_Dist4.D.ToString("0.0000") + "#"
                              + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#"
                              + hv_Dist7.D.ToString("0.0000") + "#" + hv_Dist8.D.ToString("0.0000") + "#"

                              + hv_Dist9.D.ToString("0.0000") + "#" + hv_Dist10.D.ToString("0.0000") + "#"  // W H 
                              + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"

                              + hv_DistF1.D.ToString("0.0000") + "#" + hv_DistF2.D.ToString("0.0000");      //F1 F2


                //dhDll.frmMsg.Log("syPrintCheck1206_0", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;
            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck1206_0" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }
            #endregion
        }

        public static List<object> syPrintCheck1206_0_1(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 1206后端对位  ***

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {

                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);             //粗定位阈值  20

                int hv_MaxLineWidth = int.Parse(strUserParam[8]);       //高斯线宽  5
                int hv_Contrast = int.Parse(strUserParam[9]);           //高斯对比度  5

                int hv_Sigma1 = int.Parse(strUserParam[10]);            //MeasurePairs边缘对提取平滑系数  4
                int hv_Threshold_1 = int.Parse(strUserParam[11]);       //MeasurePairs边缘对提取阈值  20 

                int hv_LineParaShift_1 = int.Parse(strUserParam[12]);    //测量平行线偏移值 4

                float iMinScore = float.Parse(strUserParam[14]);               //边缘最小得分 0.1
                int iMeasureThreshold = int.Parse(strUserParam[15]);           //边缘阈值20
                int iNumMeasures = int.Parse(strUserParam[16]);                //边缘卡尺个数 200


                //int ithresMax = 15;           //粗定位阈值
                //int hv_LineParaShift_1 = 4;   //测量平行线偏移值
                //int hv_MaxLineWidth = 5;      //高斯线宽
                //int hv_Contrast = 5;          //高斯对比度
                //int hv_Sigma1 = 4;            //MeasurePairs边缘对提取平滑系数
                //int hv_Threshold_1 = 20;      //MeasurePairs边缘对提取阈值

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_RegInters, ho_ImageCenter, ho_RegionDilation, ho_CenterReg, ho_CenterRegs, ho_BiggerRegs, ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_N, hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************

                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);
                HOperatorSet.OpeningRectangle1(ho_RegionBlack, out ho_RegionBlackOpen, 45, 25);
                HOperatorSet.Connection(ho_RegionBlackOpen, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_BiggerRegs, "area", "and", 130000, 9999999999999);
                HOperatorSet.SelectShape(ho_BiggerRegs, out ho_CenterRegs, "width", "and", (4535 - 893) - 500, (4535 - 893) + 500);
                HOperatorSet.CountObj(ho_CenterRegs, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.Union1(ho_CenterRegs, out ho_CenterReg);
                HOperatorSet.DilationRectangle1(ho_CenterReg, out ho_RegionDilation, 5, 20);
                HOperatorSet.ReduceDomain(hoImage, ho_RegionDilation, out ho_ImageCenter);
                #endregion

                #region ****** 寻找左上顶点  ******

                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1538)).TupleConcat(649)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1337)).TupleConcat(1460)).TupleConcat(660));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect1, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1, out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 40, hv_LUPCol + 190, hv_Phi1, 45, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow + 85, hv_LUPCol - 30, hv_Phi1, 20, 35);

                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(2760)).TupleConcat(
                    3534)).TupleConcat(3534)).TupleConcat(2952)).TupleConcat(2760), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(2100)).TupleConcat(1506)).TupleConcat(665));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect2, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 40, hv_LDPCol + 190, hv_Phi2, 45, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow - 85, hv_LDPCol - 18, hv_Phi2, 20, 35);

                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3610)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3610));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect3, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 40, hv_RUPCol - 190, hv_Phi3, 45, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow + 85, hv_RUPCol + 20, hv_Phi3, 20, 35);

                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2791)).TupleConcat(3013)).TupleConcat(3546), ((((new HTuple(3412)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4062)).TupleConcat(3412));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect4, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 40, hv_RDPCol - 190, hv_Phi4, 45, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow - 85, hv_RDPCol + 20, hv_Phi4, 20, 35);

                #endregion

                HTuple hv_LightDark = "light";
                HTuple hv_Success;

                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);

                f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast,
                 hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce1, out ho_MaxLine1, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce2, out ho_MaxLine2, hv_MaxLineWidth, hv_Contrast - 2, hv_Phi1, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce3, out ho_MaxLine3, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);

                f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce4, out ho_MaxLine4, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi2, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);

                f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce5, out ho_MaxLine5, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);

                f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce6, out ho_MaxLine6, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi3, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);

                f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce7, out ho_MaxLine7, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "H", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下横向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);

                f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast,
                    hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                {
                    f_find_check_line(ho_ImageReduce8, out ho_MaxLine8, hv_MaxLineWidth, hv_Contrast - 2,
                        hv_Phi4, hv_Eps, "V", hv_LightDark, hv_pi, out hv_Success);
                    if ((int)(new HTuple(hv_Success.TupleEqual(0))) != 0)
                    {
                        listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                        return listObj2Draw;
                    }
                }

                #endregion

                #region ****** 显示所有找到的剥离线  ******

                //OK,显示所有找到的线
                //listObj2Draw[1] = "OK";
                //syShowXLD(ho_MaxLine1, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine2, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine3, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine4, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine5, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine6, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine7, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine8, ref listObj2Draw, "OK");
                //return listObj2Draw;

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_UHPoint1Row = new HTuple();
                HTuple hv_UHPoint1Col = new HTuple();
                HTuple hv_UHPoint2Row = new HTuple();
                HTuple hv_UHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_UHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_UHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_UHPoint1Row.TupleConcat(hv_UHPoint2Row), hv_UHPoint1Col.TupleConcat(hv_UHPoint2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_UHPoint1Col.D, (float)hv_UHPoint1Row.D, (float)hv_UHPoint2Col.D, (float)hv_UHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_DHPoint1Row = new HTuple();
                HTuple hv_DHPoint1Col = new HTuple();
                HTuple hv_DHPoint2Row = new HTuple();
                HTuple hv_DHPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_DHPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_DHPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_DHPoint1Row.TupleConcat(hv_DHPoint2Row), hv_DHPoint1Col.TupleConcat(hv_DHPoint2Col));
                RectangleF rectLine2 = new RectangleF((float)hv_DHPoint1Col.D, (float)hv_DHPoint1Row.D, (float)hv_DHPoint2Col.D, (float)hv_DHPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_LVPoint1Row = new HTuple();
                HTuple hv_LVPoint1Col = new HTuple();
                HTuple hv_LVPoint2Row = new HTuple();
                HTuple hv_LVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_LVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_LVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_LVPoint1Row.TupleConcat(hv_LVPoint2Row), hv_LVPoint1Col.TupleConcat(hv_LVPoint2Col));
                RectangleF rectLine3 = new RectangleF((float)hv_LVPoint1Col.D, (float)hv_LVPoint1Row.D, (float)hv_LVPoint2Col.D, (float)hv_LVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_RVPoint1Row = new HTuple();
                HTuple hv_RVPoint1Col = new HTuple();
                HTuple hv_RVPoint2Row = new HTuple();
                HTuple hv_RVPoint2Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_RVPoint2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_RVPoint2Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_RVPoint1Row.TupleConcat(hv_RVPoint2Row), hv_RVPoint1Col.TupleConcat(hv_RVPoint2Col));
                RectangleF rectLine4 = new RectangleF((float)hv_RVPoint1Col.D, (float)hv_RVPoint1Row.D, (float)hv_RVPoint2Col.D, (float)hv_RVPoint2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion

                HObject ho_Cross12, ho_Cross11, ho_Cross10, ho_Line10, ho_Cross8, ho_Cross9, ho_Cross7, ho_RVLinePara, ho_Cross6, ho_Cross5, ho_DHLinePara, ho_Cross4, ho_Cross3, ho_Cross2, ho_UHLinePara, ho_ImageEmph, ho_LVLinePara, ho_Cross, ho_Edges, ho_XXX, ho_LongEdge, ho_Cross1;
                HTuple hv_Threshold, hv_Select, hv_Dist11, hv_Transition, hv_Dist12, hv_Dist10, hv_Dist8, hv_Dist9, hv_Dist7, hv_Dist5, hv_Dist6, hv_Dist4, hv_Dist2, hv_Dist3, hv_RowEdge, hv_ColumnEdge, hv_Amplitude, hv_Distance, hv_MeasureHandle, hv_Row, hv_Column, hv_IsOverlapping, hv_Row1, hv_Column1, hv_Dist1;
                HTuple hv_RowEdgeFirst, hv_ColumnEdgeFirst, hv_AmplitudeFirst, hv_RowEdgeSecond, hv_ColumnEdgeSecond, hv_AmplitudeSecond, hv_IntraDistance, hv_InterDistance;

                HOperatorSet.Emphasize(hoImage, out hoImage, 4, 4, 2);

                PointF[] ptsLeft = new PointF[16];

                hv_Sigma = hv_Sigma1;
                hv_Threshold = hv_Threshold_1;
                hv_Transition = "positive";
                hv_Select = "all";

                HTuple hv_LineParaShift = hv_LineParaShift_1;

                HObject ho_Cross15, ho_Cross16, ho_Rect8, ho_Cross13, ho_Cross14, ho_Rect7, ho_Rect6, ho_Rect5, ho_DnMaxRegOpen, ho_DnMaxReg, ho_DnRegion, ho_UpMaxRegOpen, ho_UpMaxReg, ho_RegsCnn, ho_RegionDiff, ho_UpRegion, ho_SortedRegions, ho_RegionConns;
                HTuple hv_Dist15, hv_Dist16, hv_Dist13, hv_Dist14, hv_AreaDn, hv_RowDn, hv_ColumnDn, hv_AreaUp, hv_RowUp, hv_ColumnUp;

                #region*********************************计算左上4个距离**********************************
                //*计算左上竖向距离
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara, "regression_normal", 80);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect1, hv_Row, hv_Column, hv_Phi1 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi1 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[0] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[0]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[1] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[1]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist1);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist2);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist1 = 0 - hv_Dist1;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist2 = 0 - hv_Dist2;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[1] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[1]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[0] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[0]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist2);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist1);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist1 = 0 - hv_Dist1;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist2 = 0 - hv_Dist2;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算左上横向距离
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi1, 60, 40);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    //ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", -(hv_Dist1 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi1, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi1, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[2] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[2]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[3] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[3]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist3);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist4);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist3 = 0 - hv_Dist3;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist4 = 0 - hv_Dist4;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", hv_Dist2 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi1, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi1, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[2] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[2]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[3] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[3]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist3);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist4);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist3 = 0 - hv_Dist3;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist4 = 0 - hv_Dist4;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************计算左下4个距离**********************************
                //*计算左下竖向距离
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara, "regression_normal", 80);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect3, hv_Row, hv_Column, hv_Phi2 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi2 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[4] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[4]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[5] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[5]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist5);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist6);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist5 = 0 - hv_Dist5;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist6 = 0 - hv_Dist6;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[5] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[5]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[4] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[4]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist6);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist5);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist5 = 0 - hv_Dist5;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist6 = 0 - hv_Dist6;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算左下横向距离
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi2, 60, 40);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    //ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", -(hv_Dist5 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect4, hv_Row, hv_Column, hv_Phi2, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi2, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[6] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[6]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross8, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[7] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[7]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist7);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist8);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist7 = 0 - hv_Dist7;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist8 = 0 - hv_Dist8;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist6 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect4, hv_Row, hv_Column, hv_Phi2, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi2, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[6] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[6]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross8, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[7] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[7]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist7);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist8);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist7 = 0 - hv_Dist7;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist8 = 0 - hv_Dist8;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************计算右上4个距离**********************************
                //*计算右上竖向距离
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara, "regression_normal", -80);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect5, hv_Row, hv_Column, hv_Phi3 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi3 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross9, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[8] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[8]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross10, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[9] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[9]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist9);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist10);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist9 = 0 - hv_Dist9;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist10 = 0 - hv_Dist10;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross10, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[9] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[9]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross9, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[8] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[8]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist10);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist9);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist9 = 0 - hv_Dist9;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist10 = 0 - hv_Dist10;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算右上横向距离
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi3, 60, 40);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    //ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", -(hv_Dist9 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect6, hv_Row, hv_Column, hv_Phi3, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi3, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[10] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[10]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross12, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[11] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[11]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist11);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist12);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist11 = 0 - hv_Dist11;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist12 = 0 - hv_Dist12;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", hv_Dist10 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect6, hv_Row, hv_Column, hv_Phi3, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi3, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[10] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[10]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross12, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[11] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[11]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist11);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist12);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist11 = 0 - hv_Dist11;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist12 = 0 - hv_Dist12;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************计算右下4个距离**********************************
                //*计算右下竖向距离
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara, "regression_normal", -80);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect7, hv_Row, hv_Column, hv_Phi4 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi4 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross13, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[12] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[12]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross14, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[13] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[13]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist13);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist14);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist13 = 0 - hv_Dist13;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist14 = 0 - hv_Dist14;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross14, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[13] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[13]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross13, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[12] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[12]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist14);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist13);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist13 = 0 - hv_Dist13;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist14 = 0 - hv_Dist14;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算右下横向距离
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi4, 60, 40);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    // ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", -(hv_Dist13 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect8, hv_Row, hv_Column, hv_Phi4, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi4, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross15, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[14] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[14]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross16, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[15] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[15]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist15);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist16);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist15 = 0 - hv_Dist15;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist16 = 0 - hv_Dist16;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist14 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect8, hv_Row, hv_Column, hv_Phi4, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi4, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross15, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[14] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[14]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross16, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[15] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[15]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist15);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist16);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist15 = 0 - hv_Dist15;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist16 = 0 - hv_Dist16;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                HTuple hv_Rowaaa, hv_Columnaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;
                HObject ho_CenterCross;

                #region *************计算四个剥离线交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);

                #endregion

                //Metrology参数
                HTuple hv_Index, hv_PRt2Row, hv_PRt2Col, hv_PRt1Row, hv_PRt1Col, hv_PLf2Row, hv_PLf2Col, hv_PLf1Row, hv_PLf1Col, hv_PDn2Row, hv_PDn2Col, hv_PDn1Row, hv_PDn1Col, hv_PUp2Row, hv_PUp2Col, hv_PUp1Row, hv_PUp1Col, hv_RowR, hv_ColR, hv_RowL, hv_ColL, hv_MetrologyHandle, hv_shapeParam, hv_Parameter;
                HObject ho_LineRT, ho_LineLF, ho_LineDN, ho_CrossLineUPddd, ho_CrossLineUPbbb, ho_LineUP, ho_CrossL, ho_CrossR, ho_ContourL, ho_ContourR, ho_SortedContours, ho_Contour, ho_Contours, ho_CrossLineUPaaa, ho_CrossLineUPccc;

                #region *************Metrology找基板上边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa - 160, hv_Columnaaa, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc - 160, hv_Columnccc, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa - 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc - 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    60);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle, "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板上边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PUp1Row = hv_RowL.TupleSelect(0);
                hv_PUp1Col = hv_ColL.TupleSelect(0);
                hv_PUp2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PUp2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PUp1Row, hv_PUp1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PUp2Row, hv_PUp2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineUP, hv_PUp1Row.TupleConcat(hv_PUp2Row), hv_PUp1Col.TupleConcat(hv_PUp2Col));
                RectangleF rectLineUP = new RectangleF((float)hv_PUp1Col.D, (float)hv_PUp1Row.D, (float)hv_PUp2Col.D, (float)hv_PUp2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineUP);
                listObj2Draw.Add("OK");
                #endregion

                #region *************Metrology找基板下边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb + 160, hv_Columnbbb, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd + 160, hv_Columnddd, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb + 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd + 160);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    60);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板下边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left", "true", "column");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PDn1Row = hv_RowL.TupleSelect(0);
                hv_PDn1Col = hv_ColL.TupleSelect(0);
                hv_PDn2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PDn2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PDn1Row, hv_PDn1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PDn2Row, hv_PDn2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineDN, hv_PDn1Row.TupleConcat(hv_PDn2Row), hv_PDn1Col.TupleConcat(hv_PDn2Col));
                RectangleF rectLineDN = new RectangleF((float)hv_PDn1Col.D, (float)hv_PDn1Row.D, (float)hv_PDn2Col.D, (float)hv_PDn2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineDN);
                listObj2Draw.Add("OK");

                #endregion

                #region *************Metrology找基板左边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPaaa, hv_Rowaaa, hv_Columnaaa - 310, 60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPbbb, hv_Rowbbb, hv_Columnbbb - 310, 60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowaaa);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnaaa - 310);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowbbb);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnbbb - 310);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    60);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "first");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板左边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left", "true", "row");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PLf1Row = hv_RowL.TupleSelect(0);
                hv_PLf1Col = hv_ColL.TupleSelect(0);
                hv_PLf2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PLf2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PLf1Row, hv_PLf1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PLf2Row, hv_PLf2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineLF, hv_PLf1Row.TupleConcat(hv_PLf2Row), hv_PLf1Col.TupleConcat(hv_PLf2Col));
                RectangleF rectLineLF = new RectangleF((float)hv_PLf1Col.D, (float)hv_PLf1Row.D, (float)hv_PLf2Col.D, (float)hv_PLf2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineLF);
                listObj2Draw.Add("OK");

                #endregion

                #region *************Metrology找基板右边缘*************
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPccc, hv_Rowccc, hv_Columnccc + 310,
                    60, 0.5);
                HOperatorSet.GenCrossContourXld(out ho_CrossLineUPddd, hv_Rowddd, hv_Columnddd + 310,
                    60, 0.5);
                hv_shapeParam = new HTuple();
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowccc);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnccc + 310);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Rowddd);
                hv_shapeParam = hv_shapeParam.TupleConcat(hv_Columnddd + 310);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, 5496, 3672);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "line", hv_shapeParam,
                    20, 5, 1, 30, new HTuple(), new HTuple(), out hv_Index);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition",
                    "all");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",
                    iNumMeasures);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances",
                    40);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_sigma",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length1",
                    60);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_length2",
                    1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold",
                    iMeasureThreshold);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_interpolation",
                    "bicubic");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select",
                    "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score",
                    iMinScore);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 6, 0.785398);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out hv_Parameter);
                HOperatorSet.GetMetrologyObjectResultContour(out ho_Contour, hv_MetrologyHandle,
                    "all", "all", 0.5);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                HOperatorSet.CountObj(ho_Contour, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-寻找基板右边缘失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortContoursXld(ho_Contour, out ho_SortedContours, "upper_left",
                    "true", "row");
                HOperatorSet.CountObj(ho_SortedContours, out hv_N);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourL, 1);
                HOperatorSet.SelectObj(ho_SortedContours, out ho_ContourR, hv_N);
                HOperatorSet.GetContourXld(ho_ContourL, out hv_RowL, out hv_ColL);
                HOperatorSet.GetContourXld(ho_ContourR, out hv_RowR, out hv_ColR);
                hv_PRt1Row = hv_RowL.TupleSelect(0);
                hv_PRt1Col = hv_ColL.TupleSelect(0);
                hv_PRt2Row = hv_RowR.TupleSelect((new HTuple(hv_RowR.TupleLength())) - 1);
                hv_PRt2Col = hv_ColR.TupleSelect((new HTuple(hv_ColR.TupleLength())) - 1);
                HOperatorSet.GenCrossContourXld(out ho_CrossL, hv_PRt1Row, hv_PRt1Col, 100, 0.785398);
                HOperatorSet.GenCrossContourXld(out ho_CrossR, hv_PRt2Row, hv_PRt2Col, 100, 0.785398);
                HOperatorSet.GenContourPolygonXld(out ho_LineRT, hv_PRt1Row.TupleConcat(hv_PRt2Row), hv_PRt1Col.TupleConcat(hv_PRt2Col));
                RectangleF rectLineRT = new RectangleF((float)hv_PRt1Col.D, (float)hv_PRt1Row.D, (float)hv_PRt2Col.D, (float)hv_PRt2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLineRT);
                listObj2Draw.Add("OK");

                #endregion

                HTuple hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD;

                #region *************计算四个基板边缘线交点*************
                //*左上交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                    hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowLU, out hv_ColumnLU,
                    out hv_IsOverlapping);
                //*左下交点
                HOperatorSet.IntersectionLines(hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col,
                    hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowLD, out hv_ColumnLD,
                    out hv_IsOverlapping);
                //*右上交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                    hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowRU, out hv_ColumnRU,
                    out hv_IsOverlapping);
                //*右下交点
                HOperatorSet.IntersectionLines(hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col,
                    hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowRD, out hv_ColumnRD,
                    out hv_IsOverlapping);
                #endregion

                HTuple hv_Dist17, hv_Dist18, hv_Dist19, hv_Dist20, hv_Dist21, hv_Dist22, hv_Dist23, hv_Dist24, hv_Dist25, hv_Dist26, hv_Dist27, hv_Dist28;
                #region *************计算边缘线边长*************
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowRU, hv_ColumnRU, out hv_Dist25);   //上边长
                HOperatorSet.DistancePp(hv_RowLD, hv_ColumnLD, hv_RowRD, hv_ColumnRD, out hv_Dist26);   //下边长
                HOperatorSet.DistancePp(hv_RowLU, hv_ColumnLU, hv_RowLD, hv_ColumnLD, out hv_Dist27);   //左边长
                HOperatorSet.DistancePp(hv_RowRU, hv_ColumnRU, hv_RowRD, hv_ColumnRD, out hv_Dist28);   //右边长
                #endregion

                HTuple hv_ColumnKKK1, hv_RowKKK1, hv_ColumnKKK2, hv_RowKKK2, hv_ColumnKKK3, hv_RowKKK3, hv_ColumnKKK4, hv_RowKKK4, hv_ColumnKKK5, hv_RowKKK5, hv_ColumnKKK6, hv_RowKKK6, hv_ColumnKKK7, hv_RowKKK7, hv_ColumnKKK8, hv_RowKKK8;
                HObject ho_CrossKKK1, ho_CrossKKK2, ho_CrossKKK3, ho_CrossKKK4, ho_CrossKKK5, ho_CrossKKK6, ho_CrossKKK7, ho_CrossKKK8;
                #region *************计算剥离线交点到基板边缘距离*************
                //*计算左上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK1,
                out hv_ColumnKKK1, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK1, hv_RowKKK1, hv_ColumnKKK1,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK1, hv_ColumnKKK1,
                out hv_Dist17);
                //*计算左上剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK2,
                out hv_ColumnKKK2, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK2, hv_RowKKK2, hv_ColumnKKK2,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_RowKKK2, hv_ColumnKKK2,
                out hv_Dist18);
                //*计算左下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_LVPoint1Row, hv_LVPoint1Col, hv_LVPoint2Row,
                hv_LVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK3,
                out hv_ColumnKKK3, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK3, hv_RowKKK3, hv_ColumnKKK3,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK3, hv_ColumnKKK3,
                out hv_Dist19);
                //*计算左下剥离线交点到左边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PLf1Row, hv_PLf1Col, hv_PLf2Row, hv_PLf2Col, out hv_RowKKK4,
                out hv_ColumnKKK4, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK4, hv_RowKKK4, hv_ColumnKKK4,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_RowKKK4, hv_ColumnKKK4,
                out hv_Dist20);
                //*计算右上剥离线交点到上边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PUp1Row, hv_PUp1Col, hv_PUp2Row, hv_PUp2Col, out hv_RowKKK5,
                out hv_ColumnKKK5, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK5, hv_RowKKK5, hv_ColumnKKK5,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK5, hv_ColumnKKK5,
                out hv_Dist21);
                //*计算右上剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_UHPoint1Row, hv_UHPoint1Col, hv_UHPoint2Row,
                hv_UHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK6,
                out hv_ColumnKKK6, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK6, hv_RowKKK6, hv_ColumnKKK6,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowccc, hv_Columnccc, hv_RowKKK6, hv_ColumnKKK6,
                out hv_Dist22);
                //*计算右下剥离线交点到下边缘距离
                HOperatorSet.IntersectionLines(hv_RVPoint1Row, hv_RVPoint1Col, hv_RVPoint2Row,
                hv_RVPoint2Col, hv_PDn1Row, hv_PDn1Col, hv_PDn2Row, hv_PDn2Col, out hv_RowKKK7,
                out hv_ColumnKKK7, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK7, hv_RowKKK7, hv_ColumnKKK7,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK7, hv_ColumnKKK7,
                out hv_Dist23);
                //*计算右下剥离线交点到右边缘距离
                HOperatorSet.IntersectionLines(hv_DHPoint1Row, hv_DHPoint1Col, hv_DHPoint2Row,
                hv_DHPoint2Col, hv_PRt1Row, hv_PRt1Col, hv_PRt2Row, hv_PRt2Col, out hv_RowKKK8,
                out hv_ColumnKKK8, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_CrossKKK8, hv_RowKKK8, hv_ColumnKKK8,
                60, 0.785398);
                HOperatorSet.DistancePp(hv_Rowddd, hv_Columnddd, hv_RowKKK8, hv_ColumnKKK8,
                out hv_Dist24);
                #endregion

                HTuple hv_DistF1Sum, hv_DistF1, hv_DistF2Sum, hv_DistF2;
                #region *************计算F1 F2 *************
                HOperatorSet.DistancePp(hv_Rowaaa, hv_Columnaaa, hv_Rowccc, hv_Columnccc, out hv_DistF1Sum);
                hv_DistF1 = hv_DistF1Sum / 19;
                HOperatorSet.DistancePp(hv_Rowbbb, hv_Columnbbb, hv_Rowddd, hv_Columnddd, out hv_DistF2Sum);
                hv_DistF2 = hv_DistF2Sum / 19;
                #endregion

                //返回16个距离值+4个剥离线交点坐标

                HTuple L1, L2, L3, L4, L5, L6, L7, L8;
                HTuple R1, R2, R3, R4, R5, R6, R7, R8;

                L1 = hv_Dist1;
                L2 = hv_Dist2;
                L3 = hv_Dist4;
                L4 = hv_Dist3;

                L5 = hv_Dist6;
                L6 = hv_Dist5;
                L7 = hv_Dist8;
                L8 = hv_Dist7;

                R1 = hv_Dist9;
                R2 = hv_Dist10;
                R3 = hv_Dist12;
                R4 = hv_Dist11;

                R5 = hv_Dist14;
                R6 = hv_Dist13;
                R7 = hv_Dist16;
                R8 = hv_Dist15;

                string RetStr = hv_Dist1.D.ToString("0.0000") + "#" + hv_Dist2.D.ToString("0.0000") + "#" + hv_Dist3.D.ToString("0.0000") + "#" + hv_Dist4.D.ToString("0.0000") + "#"   // L R 
                              + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#" + hv_Dist7.D.ToString("0.0000") + "#" + hv_Dist8.D.ToString("0.0000") + "#"
                              + hv_Dist9.D.ToString("0.0000") + "#" + hv_Dist10.D.ToString("0.0000") + "#" + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"
                              + hv_Dist13.D.ToString("0.0000") + "#" + hv_Dist14.D.ToString("0.0000") + "#" + hv_Dist15.D.ToString("0.0000") + "#" + hv_Dist16.D.ToString("0.0000") + "#"

                                + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"  //剥离线交点坐标 X Y
                                + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                                + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                                + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000") + "#"

                              + hv_Dist17.D.ToString("0.0000") + "#" + hv_Dist18.D.ToString("0.0000") + "#"  // D
                              + hv_Dist19.D.ToString("0.0000") + "#" + hv_Dist20.D.ToString("0.0000") + "#"
                              + hv_Dist21.D.ToString("0.0000") + "#" + hv_Dist22.D.ToString("0.0000") + "#"
                              + hv_Dist23.D.ToString("0.0000") + "#" + hv_Dist24.D.ToString("0.0000") + "#"

                            + hv_Dist25.D.ToString("0.0000") + "#" + hv_Dist26.D.ToString("0.0000") + "#"  // W H 
                            + hv_Dist27.D.ToString("0.0000") + "#" + hv_Dist28.D.ToString("0.0000") + "#"

                            + hv_DistF1.D.ToString("0.0000") + "#" + hv_DistF2.D.ToString("0.0000");      //F1 F2


                dhDll.frmMsg.Log("syPrintCheck1206_0_1", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck1206_0_1" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }

            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }

            #endregion
        }

        public static List<object> syPrintCheck0805_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 0805对位  ***

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {

                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity, hvRowkkkkkkkkkkk, hvColumnkkkkkkkkkkkk;

                HObject ho_Region, ho_MinLine8, ho_MaxLine8, ho_Line8, ho_ImageReduce8, ho_Rectangle8, ho_MinLine7, ho_MaxLine7, ho_Line7, ho_ImageReduce7, ho_Rectangle7, ho_MinLine6, ho_MaxLine6, ho_Line6, ho_ImageReduce6, ho_Rectangle6, ho_MinLine5, ho_MaxLine5, ho_Line5, ho_ImageReduce5, ho_Rectangle5, ho_MinLine4, ho_MaxLine4, ho_Line4, ho_ImageReduce4, ho_Rectangle4, ho_MinLine3, ho_MaxLine3, ho_Line3, ho_ImageReduce3, ho_Rectangle3, ho_MinLine2, ho_MaxLine2, ho_Line2, ho_ImageReduce2, ho_Rectangle2, ho_MinLine1, ho_MaxLine1, ho_SelectedContours, ho_SplitContours, ho_Polygons, ho_Line1, ho_RegionBlack, ho_RegionsBlack, ho_SelectedRegion, ho_RegionFill, ho_RegionOpening, ho_Rectangle, ho_Rectangle1, ho_ImageReduce1;
                HTuple hv_Num2, hv_Num1, hv_pi, hv_Eps, hv_Row3, hv_Column3, hv_Phi, hv_Length1, hv_Length2, hv_Low, hv_High;

                HObject ho_UHLine, ho_DHLine, ho_LVLine, ho_RVLine;
                HTuple hv_Point4Row, hv_Point4Col, hv_Point6Row, hv_Point6Col, hv_Point8Row, hv_Point8Col, hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd, hv_Nr, hv_Nc, hv_Dist, hv_Point2Row, hv_Point2Col;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                //int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                #region ******读取参数******
                string[] strUserParam = strParams.Split('#');

                int ithresMax = int.Parse(strUserParam[4]);             //粗定位阈值
                int hv_LineParaShift_1 = int.Parse(strUserParam[6]);    //测量平行线偏移值
                int hv_MaxLineWidth = int.Parse(strUserParam[8]);       //高斯线宽
                int hv_Contrast = int.Parse(strUserParam[9]);           //高斯对比度
                int hv_Sigma1 = int.Parse(strUserParam[10]);            //MeasurePairs边缘对提取平滑系数
                int hv_Threshold_1 = int.Parse(strUserParam[11]);       //MeasurePairs边缘对提取阈值

                //int ithresMax = 20;           //粗定位阈值
                //int hv_LineParaShift_1 = 4;   //测量平行线偏移值
                //int hv_MaxLineWidth = 5;      //高斯线宽
                //int hv_Contrast = 5;          //高斯对比度
                //int hv_Sigma1 = 5;            //MeasurePairs边缘对提取平滑系数
                //int hv_Threshold_1 = 30;      //MeasurePairs边缘对提取阈值

                #endregion

                hv_pi = ((new HTuple(0)).TupleAcos()) * 2;
                hv_Eps = hv_pi / 16;

                HObject ho_RegionClose, ho_RegInters, ho_ImageCenter, ho_RegionDilation, ho_CenterReg, ho_CenterRegs, ho_BiggerRegs, ho_ImageRRR, ho_RegionFillRRR, ho_RegionR, ho_SelectedRegionsholes, ho_RegionsConn, ho_RegionBlackOpen, ho_RegionFills, ho_RegionOpening1, ho_Rect77777, ho_Rect88888, ho_Rect4, ho_Rect55555, ho_Rect66666, ho_CrossRUP, ho_Rect3, ho_Rect44444, ho_Rect33333, ho_CrossLDP, ho_Rect2, ho_Rect22222, ho_Rect11111, ho_CrossLUP, ho_Rect1, ho_ImageReduce, ho_ConnectedRegions, ho_SelectedRegions, ho_Rectangle9;
                HTuple hv_N, hv_areaRRR, hv_rowRRR, hv_colRRR, hv_Num, hv_Sigma, hv_RDPRow, hv_RDPCol, hv_Phi4, hv_RUPCol, hv_RUPRow, hv_Phi3, hv_LDPCol, hv_LDPRow, hv_Phi2, hv_Cos, hv_Sin, hv_LUPCol, hv_LUPRow, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21, hv_CornerRow, hv_CornerCol;

                #region ******reduce 中间电阻区域************
                HOperatorSet.Threshold(hoImage, out ho_RegionBlack, 0, ithresMax);
                HOperatorSet.OpeningRectangle1(ho_RegionBlack, out ho_RegionBlackOpen, 15, 4);
                HOperatorSet.ClosingRectangle1(ho_RegionBlackOpen, out ho_RegionClose, 5, 3);
                HOperatorSet.Connection(ho_RegionClose, out ho_RegionsConn);
                HOperatorSet.SelectShape(ho_RegionsConn, out ho_BiggerRegs, "area", "and", 130000, 9999999999999);
                //area_center (BiggerRegs, Area, Row3, Column3)
                HOperatorSet.SelectShape(ho_BiggerRegs, out ho_CenterRegs, "width", "and", (4471 - 1013) - 300, (4471 - 1013) + 300);
                HOperatorSet.CountObj(ho_CenterRegs, out hv_N);
                if ((int)(new HTuple(hv_N.TupleEqual(0))) != 0)
                {
                    //OK无基板
                    listObj2Draw[1] = "OK-无基板";
                    return listObj2Draw;
                }
                HOperatorSet.Union1(ho_CenterRegs, out ho_CenterReg);
                HOperatorSet.DilationRectangle1(ho_CenterReg, out ho_RegionDilation, 5, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningRectangle1(ho_RegionDilation, out ExpTmpOutVar_0, 45, 2);
                    //ho_RegionDilation.Dispose();
                    ho_RegionDilation = ExpTmpOutVar_0;
                }
                HOperatorSet.ReduceDomain(hoImage, ho_RegionDilation, out ho_ImageCenter);

                #endregion

                #region ****** 寻找左上顶点  ******

                HOperatorSet.GenRegionPolygonFilled(out ho_Rect1, ((((new HTuple(131)).TupleConcat(
                    1538)).TupleConcat(649)).TupleConcat(131)).TupleConcat(131), ((((new HTuple(660)).TupleConcat(
                    660)).TupleConcat(1337)).TupleConcat(1460)).TupleConcat(660));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect1, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi1, out hv_CornerRow, out hv_CornerCol);
                if ((int)(new HTuple(hv_Phi1.TupleLess(0))) != 0)
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 + (hv_pi / 2);
                }
                else
                {
                    HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_LUPRow);
                    HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_LUPCol);
                    hv_Phi1 = hv_Phi1 - (hv_pi / 2);
                }
                HOperatorSet.GenCrossContourXld(out ho_CrossLUP, hv_LUPRow, hv_LUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi1, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi1, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect11111, hv_LUPRow - 40, hv_LUPCol + 110, hv_Phi1, 35, 20);
                HOperatorSet.GenRectangle2(out ho_Rect22222, hv_LUPRow + 65, hv_LUPCol - 20, hv_Phi1, 20, 25);

                #endregion

                #region ****** 寻找左下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect2, ((((new HTuple(2760)).TupleConcat(
                    3534)).TupleConcat(3534)).TupleConcat(2952)).TupleConcat(2760), ((((new HTuple(665)).TupleConcat(
                    665)).TupleConcat(2100)).TupleConcat(1506)).TupleConcat(665));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect2, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi2, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi2, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi2, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 2, out hv_LDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 2, out hv_LDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossLDP, hv_LDPRow, hv_LDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi2, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi2, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect33333, hv_LDPRow + 40, hv_LDPCol + 110, hv_Phi2, 35, 20);
                HOperatorSet.GenRectangle2(out ho_Rect44444, hv_LDPRow - 65, hv_LDPCol - 20, hv_Phi2, 20, 25);

                #endregion

                #region ****** 寻找右上顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect3, ((((new HTuple(156)).TupleConcat(
                    606)).TupleConcat(776)).TupleConcat(156)).TupleConcat(156), ((((new HTuple(3610)).TupleConcat(
                    4027)).TupleConcat(4800)).TupleConcat(4800)).TupleConcat(3610));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect3, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi3, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi3, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi3, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 0, out hv_RUPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 0, out hv_RUPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RUPRow, hv_RUPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi3, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi3, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect55555, hv_RUPRow - 40, hv_RUPCol - 110, hv_Phi3, 35, 20);
                HOperatorSet.GenRectangle2(out ho_Rect66666, hv_RUPRow + 65, hv_RUPCol + 20, hv_Phi3, 20, 25);

                #endregion

                #region ****** 寻找右下顶点  ******
                HOperatorSet.GenRegionPolygonFilled(out ho_Rect4, ((((new HTuple(3546)).TupleConcat(
                    3546)).TupleConcat(2791)).TupleConcat(3013)).TupleConcat(3546), ((((new HTuple(3412)).TupleConcat(
                    4800)).TupleConcat(4800)).TupleConcat(4062)).TupleConcat(3412));
                HOperatorSet.Intersection(ho_RegionDilation, ho_Rect4, out ho_RegInters);
                HOperatorSet.SmallestRectangle2(ho_RegInters, out hv_Row2, out hv_Column2, out hv_Phi4, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle9, hv_Row2, hv_Column2, hv_Phi4, hv_Length11, hv_Length21);
                f_get_conner_rectangle2(hv_Length11, hv_Length21, hv_Row2, hv_Column2, hv_Phi4, out hv_CornerRow, out hv_CornerCol);
                HOperatorSet.TupleSelect(hv_CornerRow, 3, out hv_RDPRow);
                HOperatorSet.TupleSelect(hv_CornerCol, 3, out hv_RDPCol);
                HOperatorSet.GenCrossContourXld(out ho_CrossRUP, hv_RDPRow, hv_RDPCol, 30, 0.5);
                HOperatorSet.TupleCos(hv_Phi4, out hv_Cos);
                HOperatorSet.TupleSin(hv_Phi4, out hv_Sin);
                HOperatorSet.GenRectangle2(out ho_Rect77777, hv_RDPRow + 40, hv_RDPCol - 110, hv_Phi4, 35, 20);
                HOperatorSet.GenRectangle2(out ho_Rect88888, hv_RDPRow - 65, hv_RDPCol + 20, hv_Phi4, 20, 25);

                #endregion


                #region ****** 寻找左上剥离线  ******
                //********************************左上***************************
                //*定位左上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect22222, out ho_ImageReduce1);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce1, out ho_Line1, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line1, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上横向剥离线无定位";
                    ////绘制矩形
                    //List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Col, hv_Phi, hv_Length1, hv_Length2);
                    //listObj2Draw.Add("多边形");
                    //listObj2Draw.Add(lnBarcode.ToArray());
                    //listObj2Draw.Add("OK");
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line1, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon", 1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours, "direction", hv_Phi1 - hv_Eps, hv_Phi1 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine1, out ho_MaxLine1);


                //*定位左上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect11111, out ho_ImageReduce2);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce2, out ho_Line2, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line2, out hv_Num2);
                if ((int)(new HTuple(hv_Num2.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line2, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon", 1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi1 + (hv_pi / 2)) - hv_Eps, (hv_Phi1 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    listObj2Draw[1] = "NG-左上竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine2, out ho_MaxLine2);

                #endregion

                #region ****** 寻找左下剥离线  ******
                //********************************左下***************************
                //*定位左下横向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect44444, out ho_ImageReduce3);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma, out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce3, out ho_Line3, hv_Sigma, hv_Low, hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line3, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-左下横向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line3, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon", 1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours, "direction", hv_Phi2 - hv_Eps, hv_Phi2 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-左下横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine3, out ho_MaxLine3);

                //*定位左下竖向剥离线大致区域
                HOperatorSet.ReduceDomain(hoImage, ho_Rect33333, out ho_ImageReduce4);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce4, out ho_Line4, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line4, out hv_Num2);
                if ((int)(new HTuple(hv_Num2.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line4, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);

                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi2 + (hv_pi / 2)) - hv_Eps, (hv_Phi2 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-左下竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine4, out ho_MaxLine4);

                #endregion

                #region ****** 寻找右上剥离线  ******
                //********************************右上***************************
                //*定位右上横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect66666, out ho_ImageReduce5);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce5, out ho_Line5, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line5, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右上横向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line5, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", hv_Phi3 - hv_Eps, hv_Phi3 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右上横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine5, out ho_MaxLine5);

                //*定位右上竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect55555, out ho_ImageReduce6);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce6, out ho_Line6, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line6, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line6, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi3 + (hv_pi / 2)) - hv_Eps, (hv_Phi3 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右上竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine6, out ho_MaxLine6);

                #endregion

                #region ****** 寻找右下剥离线  ******
                //********************************右下***************************
                //*定位右下横向剥离线大致区域
                //*寻找横向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect88888, out ho_ImageReduce7);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma,
                    out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce7, out ho_Line7, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line7, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右下横向剥离线无定位";
                    return listObj2Draw;
                }
                HOperatorSet.GenPolygonsXld(ho_Line7, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", hv_Phi4 - hv_Eps, hv_Phi4 + hv_Eps, -0.5, 0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右下横向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine7, out ho_MaxLine7);

                //*定位右下竖向剥离线大致区域
                //*寻找竖向剥离线
                HOperatorSet.ReduceDomain(hoImage, ho_Rect77777, out ho_ImageReduce8);
                calculate_lines_gauss_parameters(hv_MaxLineWidth, hv_Contrast, out hv_Sigma, out hv_Low, out hv_High);
                HOperatorSet.LinesGauss(ho_ImageReduce8, out ho_Line8, hv_Sigma, hv_Low,
                    hv_High, "light", "true", "bar-shaped", "true");
                HOperatorSet.CountObj(ho_Line8, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到任何xld
                    listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                    return listObj2Draw;
                }
                //*select_contours_xld (Line8, SelectedContours, 'direction', 0.5, 200, -0.5, 0.5)
                HOperatorSet.GenPolygonsXld(ho_Line8, out ho_Polygons, "ramer", 1);
                HOperatorSet.SplitContoursXld(ho_Polygons, out ho_SplitContours, "polygon",
                    1, 5);
                HOperatorSet.SelectContoursXld(ho_SplitContours, out ho_SelectedContours,
                    "direction", (hv_Phi4 + (hv_pi / 2)) - hv_Eps, (hv_Phi4 + (hv_pi / 2)) + hv_Eps, -0.5,
                    0.5);
                HOperatorSet.CountObj(ho_SelectedContours, out hv_Num1);
                if ((int)(new HTuple(hv_Num1.TupleEqual(0))) != 0)
                {
                    //返回：出错，未找到符合标准的xld
                    listObj2Draw[1] = "NG-右下竖向剥离线无定位";
                    return listObj2Draw;
                }
                select_min_max_length_contour(ho_SelectedContours, out ho_MinLine8, out ho_MaxLine8);

                #endregion

                #region ****** 显示所有找到的剥离线  ******

                //OK,显示所有找到的线
                //listObj2Draw[1] = "OK";
                //syShowXLD(ho_MaxLine1, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine2, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine3, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine4, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine5, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine6, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine7, ref listObj2Draw, "OK");
                //syShowXLD(ho_MaxLine8, ref listObj2Draw, "OK");
                //return listObj2Draw;

                #endregion

                #region ****** 拟合第1条线和第5条线生成上横线  ******
                HTuple hv_RowOut, hv_ColOut;
                //*拟合第1条线和第5条线生成上横线
                HTuple hv_Point1Row = new HTuple();
                HTuple hv_Point1Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine1, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point1Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point1Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine5, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point2Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point2Col = (hv_ColBegin + hv_ColEnd) * 0.5;

                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point1Row = " + hv_Point1Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point1Col = " + hv_Point1Col, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point2Row = " + hv_Point2Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point2Col = " + hv_Point2Col, null, dhDll.logDiskMode.Error, 0);

                HOperatorSet.GenContourPolygonXld(out ho_UHLine, hv_Point1Row.TupleConcat(hv_Point2Row), hv_Point1Col.TupleConcat(hv_Point2Col));
                RectangleF rectLine1 = new RectangleF((float)hv_Point1Col.D, (float)hv_Point1Row.D, (float)hv_Point2Col.D, (float)hv_Point2Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine1);
                listObj2Draw.Add("OK");

                #endregion

                #region ****** 拟合第3条线和第7条线生成下横线  ******
                //*拟合第3条线和第7条线生成下横线
                HTuple hv_Point3Row = new HTuple();
                HTuple hv_Point3Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine3, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point3Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point3Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine7, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point4Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point4Col = (hv_ColBegin + hv_ColEnd) * 0.5;

                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point3Row = " + hv_Point3Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point3Col = " + hv_Point3Col, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point4Row = " + hv_Point4Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point4Col = " + hv_Point4Col, null, dhDll.logDiskMode.Error, 0);

                HOperatorSet.GenContourPolygonXld(out ho_DHLine, hv_Point3Row.TupleConcat(hv_Point4Row), hv_Point3Col.TupleConcat(hv_Point4Col));
                RectangleF rectLine2 = new RectangleF((float)hv_Point3Col.D, (float)hv_Point3Row.D, (float)hv_Point4Col.D, (float)hv_Point4Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine2);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第2条线和第4条线生成左竖线  ******
                //*拟合第2条线和第4条线生成左竖线
                HTuple hv_Point5Row = new HTuple();
                HTuple hv_Point5Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine2, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point5Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point5Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine4, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point6Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point6Col = (hv_ColBegin + hv_ColEnd) * 0.5;

                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point5Row = " + hv_Point5Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point5Col = " + hv_Point5Col, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point6Row = " + hv_Point6Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point6Col = " + hv_Point6Col, null, dhDll.logDiskMode.Error, 0);

                HOperatorSet.GenContourPolygonXld(out ho_LVLine, hv_Point5Row.TupleConcat(hv_Point6Row), hv_Point5Col.TupleConcat(hv_Point6Col));
                RectangleF rectLine3 = new RectangleF((float)hv_Point5Col.D, (float)hv_Point5Row.D, (float)hv_Point6Col.D, (float)hv_Point6Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine3);
                listObj2Draw.Add("OK");
                #endregion

                #region ****** 拟合第6条线和第8条线生成右竖线  ******
                //*拟合第6条线和第8条线生成右竖线
                HTuple hv_Point7Row = new HTuple();
                HTuple hv_Point7Col = new HTuple();
                HOperatorSet.FitLineContourXld(ho_MaxLine6, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point7Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point7Col = (hv_ColBegin + hv_ColEnd) * 0.5;
                HOperatorSet.FitLineContourXld(ho_MaxLine8, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                    out hv_Dist);
                hv_Point8Row = (hv_RowBegin + hv_RowEnd) * 0.5;
                hv_Point8Col = (hv_ColBegin + hv_ColEnd) * 0.5;

                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point7Row = " + hv_Point7Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point7Col = " + hv_Point7Col, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point8Row = " + hv_Point8Row, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Point8Col = " + hv_Point8Col, null, dhDll.logDiskMode.Error, 0);

                HOperatorSet.GenContourPolygonXld(out ho_RVLine, hv_Point7Row.TupleConcat(hv_Point8Row), hv_Point7Col.TupleConcat(hv_Point8Col));
                RectangleF rectLine4 = new RectangleF((float)hv_Point7Col.D, (float)hv_Point7Row.D, (float)hv_Point8Col.D, (float)hv_Point8Row.D);
                listObj2Draw.Add("线");
                listObj2Draw.Add(rectLine4);
                listObj2Draw.Add("OK");
                #endregion

                HObject ho_Cross12, ho_Cross11, ho_Cross10, ho_Line10, ho_Cross8, ho_Cross9, ho_Cross7, ho_RVLinePara, ho_Cross6, ho_Cross5, ho_DHLinePara, ho_Cross4, ho_Cross3, ho_Cross2, ho_UHLinePara, ho_ImageEmph, ho_LVLinePara, ho_Cross, ho_Edges, ho_XXX, ho_LongEdge, ho_Cross1;
                HTuple hv_Threshold, hv_Select, hv_Dist11, hv_Transition, hv_Dist12, hv_Dist10, hv_Dist8, hv_Dist9, hv_Dist7, hv_Dist5, hv_Dist6, hv_Dist4, hv_Dist2, hv_Dist3, hv_RowEdge, hv_ColumnEdge, hv_Amplitude, hv_Distance, hv_MeasureHandle, hv_Row, hv_Column, hv_IsOverlapping, hv_Row1, hv_Column1, hv_Dist1;
                HTuple hv_RowEdgeFirst, hv_ColumnEdgeFirst, hv_AmplitudeFirst, hv_RowEdgeSecond, hv_ColumnEdgeSecond, hv_AmplitudeSecond, hv_IntraDistance, hv_InterDistance;

                HOperatorSet.Emphasize(hoImage, out hoImage, 4, 4, 3);  //3

                PointF[] ptsLeft = new PointF[16];

                hv_Sigma = hv_Sigma1;
                hv_Threshold = hv_Threshold_1;
                hv_Transition = "positive";
                hv_Select = "all";

                HTuple hv_LineParaShift = hv_LineParaShift_1;

                HObject ho_Cross15, ho_Cross16, ho_Rect8, ho_Cross13, ho_Cross14, ho_Rect7, ho_Rect6, ho_Rect5, ho_DnMaxRegOpen, ho_DnMaxReg, ho_DnRegion, ho_UpMaxRegOpen, ho_UpMaxReg, ho_RegsCnn, ho_RegionDiff, ho_UpRegion, ho_SortedRegions, ho_RegionConns;
                HTuple hv_Dist15, hv_Dist16, hv_Dist13, hv_Dist14, hv_AreaDn, hv_RowDn, hv_ColumnDn, hv_AreaUp, hv_RowUp, hv_ColumnUp;

                #region*********************************计算左上4个距离**********************************
                //*计算左上竖向距离
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara, "regression_normal", 50);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect1, hv_Row, hv_Column, hv_Phi1 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi1 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[0] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[0]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[1] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[1]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist1);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist2);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist1 = 0 - hv_Dist1;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist2 = 0 - hv_Dist2;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[1] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[1]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[0] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[0]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist2);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist1);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist1 = 0 - hv_Dist1;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist2 = 0 - hv_Dist2;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算左上横向距离
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi1, 40, 30);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    //ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    listObj2Draw[1] = "NG-计算左上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", -(hv_Dist1 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi1, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi1, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[2] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[2]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[3] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[3]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist3);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist4);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist3 = 0 - hv_Dist3;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist4 = 0 - hv_Dist4;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", hv_Dist2 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi1, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi1, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        listObj2Draw[1] = "NG-计算左上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[2] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[2]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross4, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[3] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[3]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist3);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist4);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist3 = 0 - hv_Dist3;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist4 = 0 - hv_Dist4;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************计算左下4个距离**********************************
                //*计算左下竖向距离
                HOperatorSet.GenParallelContourXld(ho_LVLine, out ho_LVLinePara, "regression_normal", 50);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect3, hv_Row, hv_Column, hv_Phi2 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi2 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[4] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[4]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[5] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[5]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist5);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist6);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist5 = 0 - hv_Dist5;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist6 = 0 - hv_Dist6;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross6, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[5] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[5]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross5, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[4] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[4]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist6);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist5);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist5 = 0 - hv_Dist5;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist6 = 0 - hv_Dist6;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算左下横向距离
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi2, 40, 30);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    //ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算左下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", -(hv_Dist5 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect4, hv_Row, hv_Column, hv_Phi2, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi2, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[6] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[6]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross8, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[7] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[7]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist7);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist8);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist7 = 0 - hv_Dist7;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist8 = 0 - hv_Dist8;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist6 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_LVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect4, hv_Row, hv_Column, hv_Phi2, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi2, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算左下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross7, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[6] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[6]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross8, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[7] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[7]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist7);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist8);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist7 = 0 - hv_Dist7;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist8 = 0 - hv_Dist8;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************计算右上4个距离**********************************
                //*计算右上竖向距离
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara, "regression_normal", -50);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect5, hv_Row, hv_Column, hv_Phi3 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi3 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross9, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[8] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[8]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross10, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[9] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[9]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist9);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist10);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist9 = 0 - hv_Dist9;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist10 = 0 - hv_Dist10;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross10, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[9] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[9]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross9, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[8] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[8]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist10);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist9);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist9 = 0 - hv_Dist9;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist10 = 0 - hv_Dist10;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算右上横向距离
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi3, 40, 30);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    //ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右上横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", -(hv_Dist9 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect6, hv_Row, hv_Column, hv_Phi3, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi3, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[10] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[10]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross12, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[11] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[11]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist11);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist12);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist11 = 0 - hv_Dist11;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist12 = 0 - hv_Dist12;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_UHLine, out ho_UHLinePara, "regression_normal", hv_Dist10 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_UHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect6, hv_Row, hv_Column, hv_Phi3, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi3, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右上横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross11, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[10] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[10]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross12, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[11] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[11]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist11);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist12);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist11 = 0 - hv_Dist11;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist12 = 0 - hv_Dist12;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************计算右下4个距离**********************************
                //*计算右下竖向距离
                HOperatorSet.GenParallelContourXld(ho_RVLine, out ho_RVLinePara, "regression_normal", -50);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLinePara, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                HOperatorSet.GenRectangle2(out ho_Rect7, hv_Row, hv_Column, hv_Phi4 - (0.5 * hv_pi), 30, 5);
                HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi4 - (0.5 * hv_pi), 30, 5, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                    hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                    out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                    out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下竖向距离失败";
                    return listObj2Draw;
                }
                if ((int)(new HTuple(hv_RowEdgeFirst.TupleLess(hv_RowEdgeSecond))) != 0)
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross13, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[12] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[12]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross14, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[13] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[13]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist13);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist14);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist13 = 0 - hv_Dist13;
                    }
                    if ((int)(new HTuple(hv_Row.TupleGreater(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist14 = 0 - hv_Dist14;
                    }
                }
                else
                {
                    HOperatorSet.GenCrossContourXld(out ho_Cross14, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[13] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[13]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross13, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[12] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[12]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist14);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist13);
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeSecond))) != 0)
                    {
                        hv_Dist13 = 0 - hv_Dist13;
                    }
                    if ((int)(new HTuple(hv_Row.TupleLess(hv_RowEdgeFirst))) != 0)
                    {
                        hv_Dist14 = 0 - hv_Dist14;
                    }
                }
                HOperatorSet.CloseMeasure(hv_MeasureHandle);

                //*计算右下横向距离
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                HOperatorSet.GenRectangle2(out ho_Rect2, hv_Row, hv_Column, hv_Phi4, 40, 30);
                HOperatorSet.ReduceDomain(hoImage, ho_Rect2, out ho_ImageReduce);
                HOperatorSet.Threshold(ho_ImageReduce, out ho_Region, 0, 20);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.OpeningCircle(ho_Region, out ExpTmpOutVar_0, 2);
                    // ho_Region.Dispose();
                    ho_Region = ExpTmpOutVar_0;
                }
                HOperatorSet.Connection(ho_Region, out ho_RegionConns);
                HOperatorSet.SelectShape(ho_RegionConns, out ho_SelectedRegions, "area", "and", 500, 99999);
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_N);
                if ((int)(new HTuple(hv_N.TupleNotEqual(2))) != 0)
                {
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-计算右下横向距离失败";
                    return listObj2Draw;
                }
                HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point", "true", "row");
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_UpRegion, 1);
                HOperatorSet.SmallestRectangle2(ho_UpRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle1, ho_UpRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_UpMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_UpMaxReg, out ho_UpMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_UpMaxRegOpen, out hv_AreaUp, out hv_RowUp, out hv_ColumnUp);
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_DnRegion, 2);
                HOperatorSet.SmallestRectangle2(ho_DnRegion, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row1, hv_Column1, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.Difference(ho_Rectangle2, ho_DnRegion, out ho_RegionDiff);
                HOperatorSet.Connection(ho_RegionDiff, out ho_RegsCnn);
                HOperatorSet.SelectShapeStd(ho_RegsCnn, out ho_DnMaxReg, "max_area", 70);
                HOperatorSet.OpeningCircle(ho_DnMaxReg, out ho_DnMaxRegOpen, 2);
                HOperatorSet.AreaCenter(ho_DnMaxRegOpen, out hv_AreaDn, out hv_RowDn, out hv_ColumnDn);

                if ((int)(new HTuple(hv_AreaUp.TupleGreater(hv_AreaDn))) != 0)
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", -(hv_Dist13 + hv_LineParaShift));
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect8, hv_Row, hv_Column, hv_Phi4, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi4, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross15, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[14] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[14]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross16, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[15] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[15]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist15);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist16);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist15 = 0 - hv_Dist15;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist16 = 0 - hv_Dist16;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }
                else
                {
                    HOperatorSet.GenParallelContourXld(ho_DHLine, out ho_DHLinePara, "regression_normal", hv_Dist14 + hv_LineParaShift);
                    HOperatorSet.IntersectionContoursXld(ho_DHLinePara, ho_RVLine, "all", out hv_Row, out hv_Column, out hv_IsOverlapping);
                    HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row, hv_Column, 20, hv_pi / 4);
                    HOperatorSet.GenRectangle2(out ho_Rect8, hv_Row, hv_Column, hv_Phi4, 50, 1);
                    HOperatorSet.GenMeasureRectangle2(hv_Row, hv_Column, hv_Phi4, 50, 1, 5496, 3672, "nearest_neighbor", out hv_MeasureHandle);
                    HOperatorSet.MeasurePairs(hoImage, hv_MeasureHandle, hv_Sigma, hv_Threshold,
                        hv_Transition, hv_Select, out hv_RowEdgeFirst, out hv_ColumnEdgeFirst,
                        out hv_AmplitudeFirst, out hv_RowEdgeSecond, out hv_ColumnEdgeSecond,
                        out hv_AmplitudeSecond, out hv_IntraDistance, out hv_InterDistance);
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeFirst.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    if ((int)(new HTuple((new HTuple(hv_RowEdgeSecond.TupleLength())).TupleNotEqual(1))) != 0)
                    {
                        //HDevelopStop();
                        listObj2Draw[1] = "NG-计算右下横向距离失败";
                        return listObj2Draw;
                    }
                    HOperatorSet.GenCrossContourXld(out ho_Cross15, hv_RowEdgeFirst, hv_ColumnEdgeFirst, 6, 0.5);
                    ptsLeft[14] = new PointF((float)hv_ColumnEdgeFirst.DArr[0], (float)hv_RowEdgeFirst.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[14]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.GenCrossContourXld(out ho_Cross16, hv_RowEdgeSecond, hv_ColumnEdgeSecond, 6, 0.5);
                    ptsLeft[15] = new PointF((float)hv_ColumnEdgeSecond.DArr[0], (float)hv_RowEdgeSecond.DArr[0]);
                    listObj2Draw.Add("小十字");
                    listObj2Draw.Add(ptsLeft[15]);
                    listObj2Draw.Add("OK");

                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeFirst, hv_ColumnEdgeFirst, out hv_Dist15);
                    HOperatorSet.DistancePp(hv_Row, hv_Column, hv_RowEdgeSecond, hv_ColumnEdgeSecond, out hv_Dist16);
                    if ((int)(new HTuple(hv_ColumnEdgeFirst.TupleGreater(hv_Column))) != 0)
                    {
                        hv_Dist15 = 0 - hv_Dist15;
                    }
                    if ((int)(new HTuple(hv_ColumnEdgeSecond.TupleLess(hv_Column))) != 0)
                    {
                        hv_Dist16 = 0 - hv_Dist16;
                    }
                    HOperatorSet.CloseMeasure(hv_MeasureHandle);
                }

                #endregion

                #region*********************************打印12个距离**********************************
                //打印12个距离
                //dhDll.frmMsg.Log("sySixSideDetect34", "################################## " , null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist1 = " + hv_Dist1, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist2 = " + hv_Dist2, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist3 = " + hv_Dist3, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist4 = " + hv_Dist4, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist5 = " + hv_Dist5, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist6 = " + hv_Dist6, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist7 = " + hv_Dist7, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist8 = " + hv_Dist8, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist9 = " + hv_Dist9, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist10 = " + hv_Dist10, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist11 = " + hv_Dist11, null, dhDll.logDiskMode.Error, 0);
                //dhDll.frmMsg.Log("sySixSideDetect34", "hv_Dist12 = " + hv_Dist12, null, dhDll.logDiskMode.Error, 0);
                #endregion

                HTuple hv_Rowaaa, hv_Columnaaa, hv_Rowbbb, hv_Columnbbb, hv_Rowccc, hv_Columnccc, hv_Rowddd, hv_Columnddd;
                HObject ho_CenterCross;

                #region *************计算四个交点*************
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_LVLine, "all", out hv_Rowaaa,
                    out hv_Columnaaa, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_LVLine, "all", out hv_Rowbbb,
                    out hv_Columnbbb, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_UHLine, ho_RVLine, "all", out hv_Rowccc,
                    out hv_Columnccc, out hv_IsOverlapping);
                HOperatorSet.IntersectionContoursXld(ho_DHLine, ho_RVLine, "all", out hv_Rowddd,
                    out hv_Columnddd, out hv_IsOverlapping);

                #endregion

                //返回16个距离值+4个剥离线交点坐标
                string RetStr = hv_Dist1.D.ToString("0.0000") + "#" + hv_Dist2.D.ToString("0.0000") + "#" + hv_Dist3.D.ToString("0.0000") + "#" + hv_Dist4.D.ToString("0.0000") + "#"
                              + hv_Dist5.D.ToString("0.0000") + "#" + hv_Dist6.D.ToString("0.0000") + "#" + hv_Dist7.D.ToString("0.0000") + "#" + hv_Dist8.D.ToString("0.0000") + "#"
                              + hv_Dist9.D.ToString("0.0000") + "#" + hv_Dist10.D.ToString("0.0000") + "#" + hv_Dist11.D.ToString("0.0000") + "#" + hv_Dist12.D.ToString("0.0000") + "#"
                              + hv_Dist13.D.ToString("0.0000") + "#" + hv_Dist14.D.ToString("0.0000") + "#" + hv_Dist15.D.ToString("0.0000") + "#" + hv_Dist16.D.ToString("0.0000") + "#"
                              + hv_Columnaaa.D.ToString("0.0000") + "#" + hv_Rowaaa.D.ToString("0.0000") + "#"
                              + hv_Columnbbb.D.ToString("0.0000") + "#" + hv_Rowbbb.D.ToString("0.0000") + "#"
                              + hv_Columnccc.D.ToString("0.0000") + "#" + hv_Rowccc.D.ToString("0.0000") + "#"
                              + hv_Columnddd.D.ToString("0.0000") + "#" + hv_Rowddd.D.ToString("0.0000");
                //dhDll.frmMsg.Log("syPrintCheck0805_0", "RetStr = " + RetStr, null, dhDll.logDiskMode.Error, 0);
                listObj2Draw[1] = "OK";
                listObj2Draw[2] = RetStr;
                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("syPrintCheck0805_0" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }

            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }

            #endregion
        }





























        public static List<object> sySixSideDetect8_old(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 六面机 正反面  ***

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {

                HObject hoReduced, hoConcate, hoRegion, hoClosing, hoOpening, hoConnection, hoFillup, hoSelect, hoDiff, hoContour, hoUnion, hoTrans, hoErosion, hoDilation;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount, hvConvexity;

                List<string> lsInfo2Draw = new List<string>();

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                string[] strUserParam = strParams.Split('#');

                int ithreshmin = int.Parse(strUserParam[4]);//提取整体阈值下限
                int iclosing = int.Parse(strUserParam[5]);//提取整体的闭运算参数
                float fconvex = float.Parse(strUserParam[6]);//整体的凸度
                int iopenRectangle1 = int.Parse(strUserParam[7]);//开运算

                int iareamin = int.Parse(strUserParam[8]);//整体提取时候的最小面积
                int iBaohuThresh = int.Parse(strUserParam[9]);//黑色保护层提取的阈值 68
                float iBeidaoThreshExp = float.Parse(strUserParam[10]);//白色背导两边提取的阈值 2 exp

                int iexpClosing = int.Parse(strUserParam[12]);//EXP之后的 闭运算半径 7
                int iexpOpening = int.Parse(strUserParam[13]);//开运算，切断关联的半径 5

                int iBeidaoCenterThresh = int.Parse(strUserParam[14]);//背导中间部分，黑色提取阈值  30
                int iBeidaoCenterArea = int.Parse(strUserParam[15]);//背导中间部分，黑色提取面积  300


                HOperatorSet.Threshold(hoReduced, out hoRegion, ithreshmin, 99999);
                HOperatorSet.ClosingCircle(hoRegion, out hoClosing, iclosing);
                HOperatorSet.OpeningRectangle1(hoClosing, out hoOpening, 1, iopenRectangle1);
                HOperatorSet.FillUp(hoOpening, out hoFillup);
                HOperatorSet.Connection(hoFillup, out hoConnection);

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                HOperatorSet.SelectShape(hoConnection, out hoSelect, "area", "and", 28000, 99999);
                HOperatorSet.CountObj(hoSelect, out hvCount);
                if (hvCount.I == 0)
                {
                    HOperatorSet.SelectShape(hoConnection, out hoSelect, "area", "and", iareamin, 99999);
                    HOperatorSet.CountObj(hoSelect, out hvCount);
                }

                syShowRegionBorder(hoSelect, ref listObj2Draw, "NG");

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时312," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                if (hvCount.I == 1 || hvCount.I == 2)
                {
                    listObj2Draw[5] = "OK";
                    if (hvCount.I == 2) listObj2Draw[8] = "OK";
                }

                if (hvCount.I != 1 && hvCount.I != 2)
                {
                    //没找到产品
                    listObj2Draw[1] = "NG-无定位";
                    return listObj2Draw;
                }
                else if (hvCount.I == 11)
                {
                    #region *** 找到一个部分，可能是背面，可能是正面粘连在一起  ***

                    //先判断凸度，低于0.87的认为是粘连
                    HOperatorSet.Convexity(hoSelect, out hvConvexity);
                    if (hvConvexity.D < fconvex)
                    {
                        listObj2Draw[1] = "NG-产品变形1";
                        listObj2Draw[5] = "NG";
                        return listObj2Draw;
                    }

                    #region  *** 找到 左右背导  ***

                    HOperatorSet.ShapeTrans(hoSelect, out hoTrans, "convex");
                    HOperatorSet.ReduceDomain(hoImage, hoTrans, out hoReduced);

                    HObject hoExp, hoMax;

                    if (iBeidaoThreshExp > 0)
                    {
                        HOperatorSet.ExpImage(hoReduced, out hoExp, iBeidaoThreshExp);
                        HOperatorSet.ScaleImageMax(hoExp, out hoMax);
                        HOperatorSet.Threshold(hoMax, out hoRegion, 0, 100);
                    }
                    else
                    {
                        HOperatorSet.GrayRangeRect(hoReduced, out hoExp, 5, 5);
                        HOperatorSet.Threshold(hoExp, out hoRegion, -iBeidaoThreshExp, 255);
                    }

                    HOperatorSet.ErosionCircle(hoTrans, out hoErosion, 5);
                    HOperatorSet.ClosingCircle(hoRegion, out hoClosing, iexpClosing);

                    HOperatorSet.Difference(hoErosion, hoClosing, out hoDiff);
                    HOperatorSet.Connection(hoDiff, out hoConnection);
                    HOperatorSet.SelectShapeStd(hoConnection, out hoContour, "max_area", 70);
                    HOperatorSet.Difference(hoErosion, hoContour, out hoDiff);

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时313," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    HOperatorSet.OpeningCircle(hoDiff, out hoOpening, iexpOpening);
                    HOperatorSet.Connection(hoOpening, out hoConnection);
                    HOperatorSet.ShapeTrans(hoConnection, out hoFillup, "convex");

                    HTuple hvfeature = new HTuple();
                    hvfeature[0] = "area";
                    hvfeature[1] = "width";
                    hvfeature[2] = "height";

                    HTuple hvMin = new HTuple();
                    hvMin[0] = 2500;
                    hvMin[1] = 20;
                    hvMin[2] = 20;

                    HTuple hvMax = new HTuple();
                    hvMax[0] = 999999;
                    hvMax[1] = 1200;
                    hvMax[2] = 1000;

                    HOperatorSet.SelectShape(hoFillup, out hoSelect, hvfeature, "and", hvMin, hvMax);

                    HOperatorSet.CountObj(hoSelect, out hvCount);

                    if (hvCount.I != 2)
                    {
                        listObj2Draw[1] = "NG-产品变形2";

                        syShowRegionBorder(hoSelect, ref listObj2Draw, "NG");

                        return listObj2Draw;
                    }

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时314," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    syShowRegionBorder(hoSelect, ref listObj2Draw, "OK");

                    #endregion

                    #region  *** 背导破洞，主要是黑色脏污  ***

                    //HOperatorSet.ErosionCircle(hoTrans, out hoErosion, 5);
                    //HOperatorSet.DilationCircle(hoSelect, out hoDilation, 5);
                    //HOperatorSet.Difference(hoErosion, hoDilation, out hoDiff);
                    //HOperatorSet.OpeningCircle(hoDiff, out hoOpening, 5);
                    //HOperatorSet.ErosionCircle(hoOpening, out hoErosion, 5);
                    //HOperatorSet.Connection(hoErosion, out hoConnection);
                    //HOperatorSet.SelectShapeStd(hoConnection, out hoOpening, "max_area", 70);

                    HOperatorSet.ClosingCircle(hoContour, out hoFillup, 11);
                    HOperatorSet.ReduceDomain(hoImage, hoFillup, out hoReduced);

                    //*根据设定的阈值，提取白色或者黑色的区域 面积
                    HOperatorSet.Threshold(hoReduced, out hoRegion, 0, iBeidaoCenterThresh);
                    HOperatorSet.ClosingCircle(hoRegion, out hoClosing, 3.5);
                    HOperatorSet.Connection(hoClosing, out hoConnection);
                    HOperatorSet.SelectShape(hoConnection, out hoUnion, "area", "and", iBeidaoCenterArea, 99999);

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时315," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    HOperatorSet.CountObj(hoUnion, out hvCount);

                    if (hvCount.I > 0)
                    {
                        listObj2Draw[1] = "NG-背导破洞";

                        syShowRegionBorder(hoUnion, ref listObj2Draw, "NG");

                        return listObj2Draw;
                    }
                    #endregion

                    #region  *** 获取当前背导尺寸  ***

                    HOperatorSet.SmallestRectangle2(hoSelect, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);

                    float fWidthLeft, fWidthRight, fHeightLeft, fHeightRight;
                    if (hvLength1.DArr[0] > hvLength2.DArr[0])
                    {
                        fWidthLeft = (float)hvLength2.DArr[0] * 2;
                        fWidthRight = (float)hvLength2.DArr[1] * 2;

                        fHeightLeft = (float)hvLength1.DArr[0] * 2;
                        fHeightRight = (float)hvLength1.DArr[1] * 2;
                    }
                    else
                    {
                        fWidthLeft = (float)hvLength1.DArr[0] * 2;
                        fWidthRight = (float)hvLength1.DArr[1] * 2;

                        fHeightLeft = (float)hvLength2.DArr[0] * 2;
                        fHeightRight = (float)hvLength2.DArr[1] * 2;
                    }

                    #endregion

                    #region  *** 高度 宽度 参数 基准 ***

                    //判断宽度高度范围            
                    //54检测背导高度 55背导高度左右差值 56背导高度左右相加
                    //57检测背导宽度 58背导宽度左右差值 59背导宽度左右相加
                    //60圆度 61凸度 62矩形度 63密度 64长宽比 65二阶矩 66二阶矩

                    //126背导高度腐蚀参数 127背导高度下限 128上限 129差值 130和的下限 131和的上限
                    //132背导宽度腐蚀参数 133背导宽度下限 134上限 135差值 136和的下限 137和的上限
                    //138圆度下限 139圆度上限 140凸度下限 141凸度上限 142矩形度下限 143矩形度上限 144密度下限 145密度上限
                    //146长宽比下限 147长宽比上限 148二阶矩下限 149二阶矩上限 150二阶矩下限 151二阶矩上限

                    #endregion

                    #region  *** 背导高度计算  ***

                    float fHeightMin = float.Parse(strUserParam[127]);
                    float fHeightMax = float.Parse(strUserParam[128]);


                    if (strUserParam[54] == "1")
                    {
                        //z正导高度范围
                        if (fHeightLeft < fHeightMin || fHeightLeft > fHeightMax || fHeightRight < fHeightMin || fHeightRight > fHeightMax)
                        {
                            listObj2Draw[1] = "NG-背导高度超标";
                            listObj2Draw[5] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[55] == "1")
                    {
                        //正导高度左右差值
                        float fDaltaHeight = float.Parse(strUserParam[129]);
                        if (Math.Abs(fHeightRight - fHeightLeft) > fDaltaHeight)
                        {
                            listObj2Draw[1] = "NG-背导高度差超标";
                            listObj2Draw[5] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[56] == "1")
                    {
                        //正导高度左右相加的和
                        fHeightMin = float.Parse(strUserParam[130]);
                        fHeightMax = float.Parse(strUserParam[131]);

                        float fHeightSum = fHeightLeft + fHeightRight;

                        if (fHeightSum < fHeightMin || fHeightSum > fHeightMax)
                        {
                            listObj2Draw[1] = "NG-背导高度和超标";
                            listObj2Draw[5] = "NG";
                            return listObj2Draw;
                        }

                    }
                    #endregion

                    #region  *** 背导宽度计算  ***

                    float fWidthMin = float.Parse(strUserParam[133]);
                    float fWidthMax = float.Parse(strUserParam[134]);


                    if (strUserParam[57] == "1")
                    {
                        //z正导宽度范围
                        if (fWidthLeft < fWidthMin || fWidthLeft > fWidthMax || fWidthRight < fWidthMin || fWidthRight > fWidthMax)
                        {
                            listObj2Draw[1] = "NG-背导宽度超标";
                            listObj2Draw[5] = "NG";

                            lsInfo2Draw.Add("最小" + fWidthMin.ToString() + ",最大" + fWidthMax.ToString() + ",当前" + fWidthLeft.ToString() + "," + fWidthRight.ToString());
                            lsInfo2Draw.Add("NG");

                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));

                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[58] == "1")
                    {
                        //正导宽度左右差值
                        float fDaltaWdith = float.Parse(strUserParam[135]);
                        if (Math.Abs(fWidthRight - fWidthLeft) > fDaltaWdith)
                        {
                            listObj2Draw[1] = "NG-背导宽度差超标";
                            listObj2Draw[5] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[59] == "1")
                    {
                        //正导高度左右相加的和
                        fWidthMin = float.Parse(strUserParam[136]);
                        fWidthMax = float.Parse(strUserParam[137]);

                        float fWidthSum = fWidthLeft + fWidthRight;

                        if (fWidthSum < fWidthMin || fWidthSum > fWidthMax)
                        {
                            listObj2Draw[1] = "NG-背导宽度和超标";
                            listObj2Draw[5] = "NG";
                            return listObj2Draw;
                        }

                    }
                    #endregion

                    #region  *** 圆度 凸度 矩形度 长宽比计算  ***

                    //        eccentricity (RegionFillUp, Anisometry, Bulkiness, StructureFactor)
                    //        convexity (RegionFillUp, Convexity)


                    #endregion



                    #endregion
                }
                else if (hvCount.I == 12)
                {
                    #region *** 正面  找到两块不连在一起的白色区域  ***

                    #region  *** 计算左右正导宽度 高度  ***

                    HOperatorSet.Union1(hoSelect, out hoUnion);
                    HOperatorSet.ShapeTrans(hoUnion, out hoTrans, "rectangle2");
                    HOperatorSet.ReduceDomain(hoImage, hoTrans, out hoReduced);

                    HOperatorSet.Threshold(hoReduced, out hoRegion, iBaohuThresh, 255);
                    HOperatorSet.ClosingCircle(hoRegion, out hoClosing, 5);
                    HOperatorSet.OpeningRectangle1(hoClosing, out hoOpening, 1, iopenRectangle1);

                    HOperatorSet.Connection(hoClosing, out hoConnection);

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时316," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    HTuple hvfeature = new HTuple();
                    hvfeature[0] = "area";
                    hvfeature[1] = "width";
                    hvfeature[2] = "height";

                    HTuple hvMin = new HTuple();
                    hvMin[0] = 500;
                    hvMin[1] = 10;
                    hvMin[2] = 10;

                    HTuple hvMax = new HTuple();
                    hvMax[0] = 999999;
                    hvMax[1] = 1200;
                    hvMax[2] = 1000;

                    HOperatorSet.SelectShape(hoConnection, out hoSelect, hvfeature, "and", hvMin, hvMax);
                    HOperatorSet.FillUp(hoSelect, out hoFillup);

                    //HObject hoSorted, hoLeft, hoRight;
                    //HOperatorSet.SortRegion(hoSelect, out hoSorted, "first_point", "true", "column");
                    //HOperatorSet.SelectObj(hoSelect, out hoLeft, 1);
                    //HOperatorSet.SelectObj(hoSelect, out hoRight, 2);
                    HOperatorSet.SmallestRectangle2(hoFillup, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时318", "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    if (hvRow.Length != 2)
                    {
                        //没找到产品
                        listObj2Draw[1] = "NG-无定位";
                        return listObj2Draw;
                    }

                    float fWidthLeft, fWidthRight, fHeightLeft, fHeightRight;
                    if (hvLength1.DArr[0] > hvLength2.DArr[0])
                    {
                        fWidthLeft = (float)hvLength2.DArr[0] * 2;
                        fWidthRight = (float)hvLength2.DArr[1] * 2;

                        fHeightLeft = (float)hvLength1.DArr[0] * 2;
                        fHeightRight = (float)hvLength1.DArr[1] * 2;
                    }
                    else
                    {
                        fWidthLeft = (float)hvLength1.DArr[0] * 2;
                        fWidthRight = (float)hvLength1.DArr[1] * 2;

                        fHeightLeft = (float)hvLength2.DArr[0] * 2;
                        fHeightRight = (float)hvLength2.DArr[1] * 2;
                    }
                    #endregion

                    #region  *** 高度 宽度 参数 基准 ***

                    //判断宽度高度范围
                    //41检测正导高度 42正导高度左右差值 43正导高度左右相加
                    //44检测正导宽度 45正导宽度左右差值 46正导宽度左右相加
                    //47圆度 48凸度 49矩形度 50密度 51长宽比 52二阶矩 53二阶矩

                    //100正导高度腐蚀参数 101正导高度下限 102上限 103差值 104和的下限 105和的上限
                    //106正导宽度腐蚀参数 107正导宽度下限 108上限 109差值 110和的下限 111和的上限
                    //112圆度下限 113圆度上限 114凸度下限 115凸度上限 116矩形度下限 117矩形度上限 118密度下限 119密度上限
                    //120长宽比下限 121长宽比上限 122二阶矩下限 123二阶矩上限 124二阶矩下限 125二阶矩上限

                    #endregion

                    #region  *** 正导高度计算  ***

                    float fHeightMin = float.Parse(strUserParam[101]);
                    float fHeightMax = float.Parse(strUserParam[102]);


                    if (strUserParam[41] == "1")
                    {
                        //z正导高度范围
                        if (fHeightLeft < fHeightMin || fHeightLeft > fHeightMax || fHeightRight < fHeightMin || fHeightRight > fHeightMax)
                        {
                            listObj2Draw[1] = "NG-正导高度超标";
                            listObj2Draw[5] = "NG"; listObj2Draw[8] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[42] == "1")
                    {
                        //正导高度左右差值
                        float fDaltaHeight = float.Parse(strUserParam[103]);
                        if (Math.Abs(fHeightRight - fHeightLeft) > fDaltaHeight)
                        {
                            listObj2Draw[1] = "NG-正导高度差超标";
                            listObj2Draw[5] = "NG"; listObj2Draw[8] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[43] == "1")
                    {
                        //正导高度左右相加的和
                        fHeightMin = float.Parse(strUserParam[104]);
                        fHeightMax = float.Parse(strUserParam[105]);

                        float fHeightSum = fHeightLeft + fHeightRight;

                        if (fHeightSum < fHeightMin || fHeightSum > fHeightMax)
                        {
                            listObj2Draw[1] = "NG-正导高度和超标";
                            listObj2Draw[5] = "NG"; listObj2Draw[8] = "NG";
                            return listObj2Draw;
                        }

                    }
                    #endregion

                    #region  *** 正导宽度计算  ***

                    float fWidthMin = float.Parse(strUserParam[107]);
                    float fWidthMax = float.Parse(strUserParam[108]);


                    if (strUserParam[44] == "1")
                    {
                        //z正导宽度范围
                        if (fWidthLeft < fWidthMin || fWidthLeft > fWidthMax || fWidthRight < fWidthMin || fWidthRight > fWidthMax)
                        {
                            listObj2Draw[1] = "NG-正导宽度超标";
                            listObj2Draw[5] = "NG"; listObj2Draw[8] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[45] == "1")
                    {
                        //正导宽度左右差值
                        float fDaltaWdith = float.Parse(strUserParam[109]);
                        if (Math.Abs(fWidthRight - fWidthLeft) > fDaltaWdith)
                        {
                            listObj2Draw[1] = "NG-正导宽度差超标";
                            listObj2Draw[5] = "NG"; listObj2Draw[8] = "NG";
                            return listObj2Draw;
                        }
                    }
                    if (strUserParam[46] == "1")
                    {
                        //正导高度左右相加的和
                        fWidthMin = float.Parse(strUserParam[110]);
                        fWidthMax = float.Parse(strUserParam[111]);

                        float fWidthSum = fWidthLeft + fWidthRight;

                        if (fWidthSum < fWidthMin || fWidthSum > fWidthMax)
                        {
                            listObj2Draw[1] = "NG-正导宽度和超标";
                            listObj2Draw[5] = "NG"; listObj2Draw[8] = "NG";
                            return listObj2Draw;
                        }

                    }
                    #endregion

                    #region  *** 圆度 凸度 矩形度 长宽比计算  ***

                    //        eccentricity (RegionFillUp, Anisometry, Bulkiness, StructureFactor)
                    //        convexity (RegionFillUp, Convexity)


                    #endregion

                    #region  *** 保护层破洞计算  ***

                    //88保护层提取方法 =0表示自动增强 =69表示以69为阈值提取  
                    //89分界阈值 90加法低值 91加法高值  92-低值的最小面积 93保护层破洞面积

                    float fEdgeThresh = float.Parse(strUserParam[89]);
                    float fValmin = float.Parse(strUserParam[90]);
                    float fValmax = float.Parse(strUserParam[91]);
                    int iValAreamin = int.Parse(strUserParam[92]);
                    int iValAreamax = int.Parse(strUserParam[93]);

                    HOperatorSet.ErosionCircle(hoTrans, out hoErosion, 5);
                    HOperatorSet.ShapeTrans(hoFillup, out hoTrans, "rectangle2");
                    HOperatorSet.DilationCircle(hoFillup, out hoDilation, 5);
                    HOperatorSet.Difference(hoErosion, hoDilation, out hoDiff);
                    HOperatorSet.OpeningCircle(hoDiff, out hoOpening, 5);
                    HOperatorSet.ErosionCircle(hoOpening, out hoDiff, 5);
                    HOperatorSet.FillUp(hoDiff, out hoFillup);

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时317," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    HOperatorSet.ReduceDomain(hoImage, hoFillup, out hoReduced);

                    if (strUserParam[88] == "0")
                    {
                        HObject hoLog, hoMax;
                        HTuple hvMean, hvDeviation;
                        HOperatorSet.LogImage(hoReduced, out hoLog, "e");
                        HOperatorSet.ScaleImageMax(hoLog, out hoMax);
                        HOperatorSet.Intensity(hoMax, hoMax, out hvMean, out hvDeviation);

                        //if (hvMean.D < 60)
                        //{
                        //    return lmmResult;
                        //}

                        #region ---- *** 超时处理  *** ----

                        if (sw.ElapsedMilliseconds > iTimeout)
                        {
                            sw.Stop();
                            listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时319," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                            return listObj2Draw;
                        }
                        #endregion

                        HOperatorSet.Threshold(hoMax, out hoRegion, hvMean + fValmin, 256);
                        HOperatorSet.Connection(hoRegion, out hoConnection);

                        hvfeature = new HTuple();
                        hvfeature[0] = "area";
                        hvfeature[1] = "anisometry";

                        hvMin = new HTuple();
                        hvMin[0] = iValAreamin;
                        hvMin[1] = 1;

                        hvMax = new HTuple();
                        hvMax[0] = 9999;
                        hvMax[1] = 5;

                        HOperatorSet.SelectShape(hoConnection, out hoSelect, hvfeature, "and", hvMin, hvMax);


                        //大破洞
                        HOperatorSet.CountObj(hoSelect, out hvDeviation);
                        if (hvDeviation.I > 0)
                        {
                            listObj2Draw[1] = "NG-保护层破洞";

                            syShowRegionBorder(hoSelect, ref listObj2Draw, "NG");

                            return listObj2Draw;
                        }

                        //小破洞
                        if (hvMean.D > fEdgeThresh)
                        {
                            HOperatorSet.Threshold(hoMax, out hoRegion, hvMean + fValmax, 256);
                        }
                        else
                        {
                            HOperatorSet.Threshold(hoMax, out hoRegion, hvMean + fValmin, 256);
                        }

                        #region ---- *** 超时处理  *** ----

                        if (sw.ElapsedMilliseconds > iTimeout)
                        {
                            sw.Stop();
                            listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时318," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                            return listObj2Draw;
                        }
                        #endregion

                        HOperatorSet.OpeningCircle(hoRegion, out hoOpening, 1.5);
                        HOperatorSet.ClosingCircle(hoOpening, out hoClosing, 3);
                        HOperatorSet.ErosionCircle(hoClosing, out hoErosion, 3);
                        HOperatorSet.Connection(hoErosion, out hoConnection);

                        hvfeature = new HTuple();
                        hvfeature[0] = "area";
                        hvfeature[1] = "anisometry";

                        hvMin = new HTuple();
                        hvMin[0] = iValAreamax;
                        hvMin[1] = 1;

                        hvMax = new HTuple();
                        hvMax[0] = 9999;
                        hvMax[1] = 5;

                        HOperatorSet.SelectShape(hoConnection, out hoSelect, hvfeature, "and", hvMin, hvMax);

                        #region ---- *** 超时处理  *** ----

                        if (sw.ElapsedMilliseconds > iTimeout)
                        {
                            sw.Stop();
                            listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时321," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                            return listObj2Draw;
                        }
                        #endregion

                        HOperatorSet.CountObj(hoSelect, out hvDeviation);
                        if (hvDeviation.I > 0)
                        {
                            listObj2Draw[1] = "NG-保护层破洞";

                            syShowRegionBorder(hoSelect, ref listObj2Draw, "NG");

                            return listObj2Draw;
                        }

                        #region ---- *** 超时处理  *** ----

                        if (sw.ElapsedMilliseconds > iTimeout)
                        {
                            sw.Stop();
                            listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时320," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                            return listObj2Draw;
                        }
                        #endregion

                    }

                    #endregion


                    #endregion
                }

                return listObj2Draw;

            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect34", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect8.ReleaseMutex();
            }

            #endregion
        }

        public static List<string> syAutoPrintMark(HObject hoImage, List<PointF[]> lkkPolygon, string strParams, ref List<object> listObj2Draw)
        {
            #region  *** 全自动印刷机 MARK  ***

            List<string> lmmResult = new List<string>();

            HObject hoRegion, hoConcate, hoReduced, hoClosing, hoConnection, hoSelect, hoContour, hoUnion;
            HTuple hvRow, hvRow1, hvRow2, hvColumn, hvColumn1, hvColumn2, hvNr, hvNc, hvDist;

            #region ****** 生成区域ROI  ******

            HOperatorSet.GenEmptyObj(out hoConcate);
            for (int igg = 0; igg < lkkPolygon.Count; igg++)
            {
                if (lkkPolygon[igg][0].X == 3)
                {
                    PointF pgg1 = lkkPolygon[igg][1];
                    PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                    double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                    HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                    HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                }
                else if (lkkPolygon[igg][0].X == 8)
                {
                    PointF pgg1 = lkkPolygon[igg][1];
                    PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                    HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                    HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                }
                else
                {
                    PointF pgg1 = lkkPolygon[igg][1];
                    PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                    HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                    HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                }
            }

            HOperatorSet.Union1(hoConcate, out hoUnion);
            HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

            #endregion

            string[] strUserParam = strParams.Split('#');

            int ithreshMin = int.Parse(strUserParam[4]);
            int ithreshMax = int.Parse(strUserParam[5]);
            int iclosing = int.Parse(strUserParam[6]);

            HOperatorSet.Threshold(hoReduced, out hoRegion, ithreshMin, ithreshMax);
            HOperatorSet.ClosingCircle(hoRegion, out hoClosing, iclosing);
            HOperatorSet.Connection(hoClosing, out hoConnection);
            HOperatorSet.SelectShapeStd(hoConnection, out hoSelect, "max_area", 80);
            HOperatorSet.Skeleton(hoSelect, out hoRegion);
            HOperatorSet.GenContoursSkeletonXld(hoRegion, out hoContour, 100, "filter");
            HOperatorSet.UnionCollinearContoursXld(hoContour, out hoUnion, 20, 2, 12, 0.1, "attr_keep");
            HOperatorSet.FitLineContourXld(hoUnion, "tukey", -1, 0, 5, 2, out hvRow1, out hvColumn1, out hvRow2, out hvColumn2, out hvNr, out hvNc, out hvDist);

            if (hvRow2.Length == 2)
            {
                HOperatorSet.IntersectionLines(hvRow1[0], hvColumn1[0], hvRow2[0], hvColumn2[0], hvRow1[1], hvColumn1[1], hvRow2[1], hvColumn2[1], out hvRow, out hvColumn, out hvDist);
                lmmResult.Add(hvColumn.D.ToString());
                lmmResult.Add(hvRow.D.ToString());
            }
            else
            {
                listObj2Draw[1] = "NG-没找到MARK";
            }

            return lmmResult;

            #endregion
        }

        public static List<object> syDetectKH(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 口红角度检测  ***

            if (bUseMutex) muDetect12.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("OK"); listObj2Draw.Add(888);

            try
            {

                HObject hoRegion, hoReduced, hoConcate, hoConnection, hoSelect, hoContour, hoUnion, hoOpening, hoFillup;
                HTuple hvRow, hvColumn, hvPhi, hvLength1, hvLength2, hvCount;

                #region ****** 生成区域ROI  ******

                HOperatorSet.GenEmptyObj(out hoConcate);
                for (int igg = 0; igg < lkkPolygon.Count; igg++)
                {
                    if (lkkPolygon[igg][0].X == 3)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//圆形ROI的直径
                        double ddistance = Math.Sqrt(Math.Pow(pgg2.X - pgg1.X, 2) + Math.Pow(pgg2.Y - pgg1.Y, 2));

                        HOperatorSet.GenCircle(out hoRegion, pgg1.Y, pgg1.X, ddistance);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else if (lkkPolygon[igg][0].X == 8)
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][2];//矩形的宽度 高度

                        HOperatorSet.GenRectangle1(out hoRegion, pgg1.Y, pgg1.X, pgg1.Y + pgg2.Y, pgg1.X + pgg2.X);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                    else
                    {
                        PointF pgg1 = lkkPolygon[igg][1];
                        PointF pgg2 = lkkPolygon[igg][3];//rectangle2的宽度 高度

                        HOperatorSet.GenRectangle2(out hoRegion, pgg1.Y, pgg1.X, lkkPolygon[igg][2].X / 10000, pgg2.X, pgg2.Y);
                        HOperatorSet.ConcatObj(hoConcate, hoRegion, out hoConcate);
                    }
                }

                HOperatorSet.Union1(hoConcate, out hoUnion);
                HOperatorSet.ReduceDomain(hoImage, hoUnion, out hoReduced);

                #endregion

                string[] strUserParam = strParams.Split('#');

                int ithreshmin = int.Parse(strUserParam[4]);
                int ithreshmax = int.Parse(strUserParam[5]);
                int iclosing = int.Parse(strUserParam[6]);

                //int iwidthmax = int.Parse(strUserParam[11]);
                //int iheightmin = int.Parse(strUserParam[12]);
                //int iheightmax = int.Parse(strUserParam[13]);

                HOperatorSet.Threshold(hoReduced, out hoRegion, ithreshmin, ithreshmax);

                HOperatorSet.OpeningCircle(hoRegion, out hoOpening, 5);
                HOperatorSet.Connection(hoOpening, out hoConnection);
                HOperatorSet.FillUp(hoConnection, out hoFillup);
                HOperatorSet.SelectShape(hoFillup, out hoContour, "area", "and", 5000, 9999999);
                HOperatorSet.SelectShapeStd(hoContour, out hoSelect, "max_area", 80);
                HOperatorSet.SmallestRectangle2(hoSelect, out hvRow, out hvColumn, out hvPhi, out hvLength1, out hvLength2);

                HOperatorSet.CountObj(hoSelect, out hvCount);
                if (hvCount.I == 0)
                {
                    listObj2Draw[1] = "NG-无定位"; return listObj2Draw;
                }
                else
                {
                    List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hvRow, hvColumn, hvPhi, hvLength1, hvLength2);

                    //listObj2Draw.Add("多边形");
                    //listObj2Draw.Add(lnBarcode.ToArray());

                    //if (hvCount.I == 1)
                    //{
                    //    listObj2Draw.Add("OK");
                    //}
                    //else
                    //{
                    //    listObj2Draw.Add("NG");
                    //}


                    if (hvCount.I == 1)
                    {

                    }
                    else
                    {
                        listObj2Draw[1] = "NG-区域不符";
                        return listObj2Draw;
                    }
                }

                //拟合椭圆
                HTuple hv_Width = null, hv_Height = null, hv_MetrologyHandle = null;
                HTuple hv_Index = null, hv_StartPhi, hv_EndPhi;
                HTuple hv_PointOrder = null;
                // Initialize local and output iconic variables 

                int iarealength1 = int.Parse(strUserParam[8]);
                int iarealength2 = int.Parse(strUserParam[9]);
                int ithresh = int.Parse(strUserParam[10]);
                double dStandardAngle = double.Parse(strUserParam[12]);
                double dOrientation = double.Parse(strUserParam[13]);

                HOperatorSet.GetImageSize(hoImage, out hv_Width, out hv_Height);
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                HOperatorSet.AddMetrologyObjectEllipseMeasure(hv_MetrologyHandle, hvRow,
                    hvColumn, hvPhi, hvLength1, hvLength2, iarealength1, iarealength2, 2, ithresh, new HTuple(), new HTuple(), out hv_Index);
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_measures",300);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "num_instances", 1);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_select", "last");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_transition", "negative");
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "min_score", 0.5);
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, "all", "measure_threshold", ithresh);
                HOperatorSet.ApplyMetrologyModel(hoImage, hv_MetrologyHandle);

                HOperatorSet.GetMetrologyObjectResultContour(out hoContour, hv_MetrologyHandle, "all", "all", 1.5);
                //HOperatorSet.GetMetrologyObjectMeasures(out hoContour, hv_MetrologyHandle, "all", "all", out hvRow, out hvColumn);

                HTuple hv_RowCenter, hv_ColumnCenter, hv_Phi_Result, hv_Radius_A, hv_Radius_B;
                HOperatorSet.FitEllipseContourXld(hoContour, "fitzgibbon", -1, 0, 0, 200, 3,
                    2, out hv_RowCenter, out hv_ColumnCenter, out hv_Phi_Result, out hv_Radius_A,
                    out hv_Radius_B, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);

                if (hv_RowCenter.Length != 1)
                {
                    listObj2Draw[1] = "NG-椭圆搜索出错";
                    return listObj2Draw;
                }

                double dAngle = hv_Phi_Result.D * 180d / Math.PI;

                List<PointF> lpsRect2 = dhFindVerticesOfRectangle2(hv_RowCenter, hv_ColumnCenter, hv_Phi_Result, hv_Radius_A, hv_Radius_B);
                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lpsRect2.ToArray());
                listObj2Draw.Add("OK");

                PointF p1 = new PointF((lpsRect2[0].X + lpsRect2[1].X) / 2, (lpsRect2[0].Y + lpsRect2[1].Y) / 2);
                PointF p2 = new PointF((lpsRect2[2].X + lpsRect2[3].X) / 2, (lpsRect2[2].Y + lpsRect2[3].Y) / 2);
                listObj2Draw.Add("线段");
                listObj2Draw.Add(new RectangleF(p1.X, p1.Y, p2.X, p2.Y));
                listObj2Draw.Add("OK");

                PointF p3 = new PointF((lpsRect2[0].X + lpsRect2[3].X) / 2, (lpsRect2[0].Y + lpsRect2[3].Y) / 2);
                PointF p4 = new PointF((lpsRect2[2].X + lpsRect2[1].X) / 2, (lpsRect2[2].Y + lpsRect2[1].Y) / 2);
                listObj2Draw.Add("线段");
                listObj2Draw.Add(new RectangleF(p3.X, p3.Y, p4.X, p4.Y));
                listObj2Draw.Add("OK");

                double drealAngle = (dAngle - dStandardAngle) * dOrientation;

                List<string> listInfo2Draw = new List<string>();
                listInfo2Draw.Add("中心:X=" + hv_ColumnCenter.D.ToString("0.000") + ",Y=" + hv_RowCenter.D.ToString("0.000"));
                listInfo2Draw.Add("OK");
                listInfo2Draw.Add("旋转角度:Angle=" + dAngle.ToString("0.000"));
                listInfo2Draw.Add("OK");
                listInfo2Draw.Add("真实角度:Angle=" + drealAngle.ToString("0.000"));
                listInfo2Draw.Add("NG");

                listObj2Draw.Add("字符串");
                listObj2Draw.Add(listInfo2Draw);
                listObj2Draw.Add(new PointF(1800, 100));

                listObj2Draw[2] = drealAngle;






                return listObj2Draw;
            }
            catch (Exception exc)
            {
                listObj2Draw[1] = "NG-程序出错";
                dhDll.frmMsg.Log("sySixSideDetect56", "", exc, dhDll.logDiskMode.Error, 0);
                return listObj2Draw;
            }
            finally
            {
                sw.Stop();
                if (bUseMutex) muDetect12.ReleaseMutex();
            }

            #endregion
        }

        public static void syShowXLD(HObject hoXLD, ref List<object> lobjdrawing, string strOKNG)
        {
            #region  *** 将一个XLD添加到绘图  ***
            try
            {
                HObject hoIndex, hoContour;
                HTuple hvCount, hvRow, hvColumn;

                HOperatorSet.GetContourXld(hoXLD, out hvRow, out hvColumn);
                List<PointF> lpsTemp = new List<PointF>();
                for (int ikk = 0; ikk < hvRow.Length; ikk++)
                {
                    lpsTemp.Add(new PointF((float)hvColumn.DArr[ikk], (float)hvRow.DArr[ikk]));
                }

                lobjdrawing.Add("轮廓");
                lobjdrawing.Add(lpsTemp.ToArray());
                lobjdrawing.Add(strOKNG);

            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("将一个XLD添加到绘图出错" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
            }
            #endregion
        }

        public static void syShowRegionBorder(HObject hoImage, ref List<object> lobjdrawing, string strOKNG)
        {
            #region  *** 将一个区域轮廓添加到绘图  ***
            try
            {
                HObject hoIndex, hoContour;
                HTuple hvCount, hvRow, hvColumn;
                HOperatorSet.CountObj(hoImage, out hvCount);
                if (hvCount.I > 0)
                {
                    for (int igg = 1; igg <= hvCount.I; igg++)
                    {
                        HOperatorSet.SelectObj(hoImage, out hoIndex, igg);

                        HOperatorSet.GenContourRegionXld(hoIndex, out hoContour, "border");
                        HOperatorSet.GetContourXld(hoContour, out hvRow, out hvColumn);
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
                dhDll.frmMsg.Log("将一个区域轮廓添加到绘图出错" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
            }
            #endregion
        }

        #region ******* 输入rectangle2 输出4个边缘交点坐标函数  ******

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
    }
}
