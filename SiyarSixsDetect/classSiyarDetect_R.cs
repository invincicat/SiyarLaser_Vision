
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using hvppleDotNet;
using System.Drawing;

using System.Diagnostics;


namespace SiyarSixsDetect
{
   
 public class classSiyarDetect_R

    {
        //调试界面前
        private static int iTimeout = 100;//12.26修改
        private static bool bUseMutex = false;
        private static Mutex muDetect12 = new Mutex();
        private static Mutex muDetect56 = new Mutex();
        private static Mutex muDetect8 = new Mutex();
        static int hv_m = 0; //五号相机图片计数
        static int hv_n = 0; //六号相机图片计数
        //static HTuple hv_num3;
        static HTuple hv_num4;


        #region 0402R
        //12相机 六面机左右面  引用文件 6
        public static List<object> sySixSideDetect_0402R_Camera12(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
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
                HTuple hv_Num2;
                HObject hoSelectedRegions3, ho_RegionClosing, ho_RegionTrans, ho_ConnectedRegionDark2;
                HObject ho_Region, ho_RegionFillUp2, ho_RegionClosing1, ho_ConnectedRegions1, ho_SelectedRegions, ho_RegionFillUp3;

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
                int iLength1Scale = int.Parse(strUserParam[5]); //iLength1Scale = 10 半长变化值
                int iLength2 = int.Parse(strUserParam[6]);      //iLength2  = 60     半宽
                int iLength2Scale = int.Parse(strUserParam[7]); //iLength2Scale = 10 半宽变化值
                int iErrThres = int.Parse(strUserParam[8]);     //iErrThres  = 40    缺陷阈值40
                int iErrArea = int.Parse(strUserParam[9]);      //iErrArea = 50      缺陷面积50
                float iRecty = float.Parse(strUserParam[10]);   //轮廓矩形度 0.85
                int iEroradis = int.Parse(strUserParam[11]);    //iEroradis=8 矩形腐蚀半径 8
                int iErrAll = int.Parse(strUserParam[12]);     //iErrAll  = 160    全局阈值160
                int GrayClosingRect1 = int.Parse(strUserParam[13]);     //GrayClosingRect1  = 11   
                int ScaleImage1 = int.Parse(strUserParam[14]);     //200
                int ClosingCircle1 = int.Parse(strUserParam[15]);//ClosingCircle1=7
                int ErosionCircle1 = int.Parse(strUserParam[16]);//4
                int AreaDuanmian = int.Parse(strUserParam[17]);//40000
                int iErrArea2 = int.Parse(strUserParam[18]);      //iErrArea = 50      缺陷面积50
                int iEroradis2 = int.Parse(strUserParam[19]);      //矩形腐蚀半径5
                int ErosionCircle2 = int.Parse(strUserParam[20]);      //缺陷腐蚀半径4     缺陷面积50
                int iAnisometry = int.Parse(strUserParam[21]);      //长宽比10
                int iBianjie = int.Parse(strUserParam[22]);      //长宽比10




                HObject ho_RegionErrDConn, ho_RegionErrD, ho_ConnectedRegionDark, ho_RegionDark, ho_ImageReduced2, ho_RegionErosion, ho_RegionOpening2, ho_RectPu, ho_RegionOpening, ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionBinary, ho_ConnectedRegions, ho_MaxRegion, ho_RegionFillUp;
                HTuple hv_Number, NChannel, hv_UsedThreshold;

                //***判断电阻是否靠近边界
                HTuple hv_Area7, hv_Row10, hv_Column10;
                HObject ho_RegionDifference2, ho_RegionErosion3, ho_RegionIntersection;

                //判断彩色还是黑白
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

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion


                //HOperatorSet.GrayClosingRect(ho_ImageReduced, out ho_ImageClosing, GrayClosingRect1, GrayClosingRect1);
                //HOperatorSet.BinaryThreshold(ho_ImageClosing, out ho_RegionBinary, "max_separability",
                //    "light", out hv_UsedThreshold);

                HOperatorSet.ScaleImage(ho_ImageReduced, out ho_ImageClosing, 3, ScaleImage1);
                HOperatorSet.Threshold(ho_ImageClosing, out ho_RegionBinary, iErrAll, 255); //12号参数
                //       HObject ho_RegionBinaryThreshold;
                //       HTuple hv_UsedThreshold2;
                //       HOperatorSet.BinaryThreshold(ho_ImageReduced, out ho_RegionBinaryThreshold, "max_separability",
                //"light", out hv_UsedThreshold2);
                //       HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionBinary, hv_UsedThreshold2 + 30,
                //           255);

                HOperatorSet.ClosingCircle(ho_RegionBinary, out ho_RegionClosing, ClosingCircle1);//15
                HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_MaxRegion, "max_area", 70);

                HOperatorSet.FillUp(ho_MaxRegion, out ho_RegionFillUp);
                HOperatorSet.OpeningRectangle1(ho_RegionFillUp, out ho_RegionOpening, 10, 5);//10,5

                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegionOpening);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegionOpening, out ho_RegionOpening, "max_area", 70);

                ho_SelectedRegions = ho_RegionOpening;
                HOperatorSet.AreaCenter(ho_SelectedRegions, out Areakkk, out Rowkkk, out Colkkk);


                if (Areakkk < 0.5 * AreaDuanmian)  //如果端面面积小于设置的最小端面面积的一半，判定无定位
                {
                    #region***判断端面面积，过小直接无定位
                    listObj2Draw[1] = "NG-无定位"; //区域面积不符合要求
                                                //输出NG详情
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num);



                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_SelectedRegions, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    lsInfo2Draw.Add("无定位-端面最小面积：" + (0.5 * AreaDuanmian).ToString() + "pix * ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + Areakkk.D.ToString("0.0") + " pix * ");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }


                if (Areakkk < AreaDuanmian)  //确定端面面积51963,
                {
                    #region***判断端面面积，过小直接无定位
                    listObj2Draw[1] = "NG-尺寸不符"; //区域面积不符合要求
                                                 //输出NG详情
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num);



                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_SelectedRegions, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    lsInfo2Draw.Add("端面最小面积：" + AreaDuanmian.ToString() + "pix * ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + Areakkk.D.ToString("0.0") + " pix * ");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }





                //*判断矩形度
                HOperatorSet.Rectangularity(ho_SelectedRegions, out hv_Rectangularity);
                if (hv_Rectangularity < iRecty)
                {
                    #region***判断端面矩形度，过小NG
                    //HDevelopStop();
                    if (hv_Rectangularity < (iRecty * 0.9))
                    {
                        #region***判断端面矩形度，过小直接无定位
                        listObj2Draw[1] = "NG-尺寸不符";//"NG-切割不良"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_SelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("最小矩形度：" + iRecty.ToString() + "pix * ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前矩形度：" + hv_Rectangularity.D.ToString("0.000") + " pix * ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    else
                    {
                        #region***端面矩形度小于合格阈值，大于无定位阈值，判定尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符";//"NG-切割不良"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_SelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_SelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("最小矩形度：" + iRecty.ToString() + "pix * ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前矩形度：" + hv_Rectangularity.D.ToString("0.000") + " pix * ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion
                }


                HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column,
                  out hv_Phi, out hv_Length1, out hv_Length2);

                HOperatorSet.GenRectangle2(out ho_RectPu, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);


                #region***判断电阻是否靠近边界      
                //0919
                HObject ho_Rect19, ho_Rect20;
                HTuple hv_Row11, hv_Column11, hv_Row22, hv_Column22;

                HOperatorSet.SmallestRectangle1(hoUnion, out hv_Row11, out hv_Column11, out hv_Row22, out hv_Column22);
                HOperatorSet.GenRectangle1(out ho_Rect19, hv_Row11, hv_Column11, hv_Row22, hv_Column22);

                HTuple hv_Row1_1, hv_Column1_1, hv_Row2_2, hv_Column2_2;
                HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row1_1, out hv_Column1_1, out hv_Row2_2, out hv_Column2_2);
                HOperatorSet.GenRectangle1(out ho_Rect20, hv_Row1_1, hv_Column1_1, hv_Row2_2, hv_Column2_2);

                //if (hv_Column11<50|| hv_Row11 < 50|| hv_Row21>430|| hv_Column21 > 590)

                if (hv_Row1_1 < hv_Row11 + iBianjie || hv_Column1_1 < hv_Column11 + iBianjie || hv_Row2_2 > hv_Row22 - iBianjie || hv_Column2_2 > hv_Column22 - iBianjie)
                {
                    #region
                    listObj2Draw[1] = "NG-尺寸不符";//角度歪斜
                    syShowRegionBorder(ho_Rect19, ref listObj2Draw, "NG");
                    syShowRegionBorder(ho_Rect20, ref listObj2Draw, "NG");
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add("NG");

                    //输出NG详情
                    lsInfo2Draw.Add("靠近边界/歪斜");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                #region
                HOperatorSet.ErosionCircle(ho_ImageReduced, out ho_RegionErosion3, 1.1);
                HOperatorSet.Intersection(ho_RegionErosion3, ho_RectPu, out ho_RegionIntersection);
                HOperatorSet.Difference(ho_RectPu, ho_RegionIntersection, out ho_RegionDifference2);
                HOperatorSet.AreaCenter(ho_RegionDifference2, out hv_Area7, out hv_Row10, out hv_Column10);
                if ((int)(new HTuple(hv_Area7.TupleGreater(0))) != 0)
                {
                    #region
                    listObj2Draw[1] = "NG-尺寸不符";//角度歪斜
                    List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcode.ToArray());
                    listObj2Draw.Add("NG");

                    //输出NG详情
                    lsInfo2Draw.Add("靠近边界");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }
                #endregion
                #endregion


                //0508更改，判断产品角度，歪斜过大直接无定位 正负10度
                HTuple Deg;
                HTuple iAngleScale = 10;
                HOperatorSet.TupleDeg(hv_Phi, out Deg);
                if (Deg > iAngleScale || Deg < (0 - iAngleScale))
                {
                    #region***判断产品角度，歪斜过大直接无定位 正负10度
                    listObj2Draw[1] = "NG-无定位";//角度歪斜
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
                    #endregion
                }


                //*判断产品尺寸（）
                HTuple hv_Length1Scale = iLength1Scale;
                HTuple hv_Length2Scale = iLength2Scale;

                if ((int)((new HTuple(hv_Length1.TupleLess(iLength1 - hv_Length1Scale))).TupleOr(new HTuple(hv_Length1.TupleGreater(
                    iLength1 + hv_Length1Scale)))) != 0)
                {
                    #region
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG1 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG1.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";//"NG-尺寸不符";//NG-无定位

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                    iLength2 + hv_Length2Scale)))) != 0)
                {
                    #region

                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";// "NG-尺寸不符";

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
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


                //ceshi01-主要检测部分
                HObject ho_RegionDifference, ho_RegionErosion1, ho_ConnectedRegions2, ho_SelectedRegions1,
                    ho_SelectedRegions3, ho_SelectedRegions4;
                HTuple hv_Row2, hv_Column2, hv_Area1;

                //HOperatorSet.ErosionCircle(ho_RectPu, out ho_RegionErosion, iEroradis);  //腐蚀半径8、11号参数

                HOperatorSet.ShapeTrans(ho_SelectedRegions, out ho_RegionTrans, "convex");
                HOperatorSet.Difference(ho_RegionTrans, ho_MaxRegion, out ho_RegionDifference);
                //HOperatorSet.Difference(ho_RegionTrans, ho_SelectedRegions, out ho_RegionDifference);

                //HOperatorSet.ReduceDomain(ho_ImageClosing, ho_RegionTrans, out ho_ImageReduced2);
                //HOperatorSet.Threshold(ho_ImageReduced2, out ho_RegionDark, 0, iErrThres);
                HOperatorSet.ErosionCircle(ho_RegionDifference, out ho_RegionErosion1, ErosionCircle1);//16号参数
                HOperatorSet.Connection(ho_RegionErosion1, out ho_ConnectedRegions2);

                HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions3, "anisometry",
                    "and", 0, 20);

                HOperatorSet.SelectShape(ho_SelectedRegions3, out ho_SelectedRegions1, "area",
                    "and", iErrArea, 99999);

                //HOperatorSet.SelectShapeStd(ho_SelectedRegions3, out ho_SelectedRegions4, "max_area", 70);
                hv_Num = 0;
                HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Num);
                HOperatorSet.Union1(ho_SelectedRegions1, out hoUnion);
                HOperatorSet.AreaCenter(hoUnion, out hv_Area1, out hv_Row2, out hv_Column2);
                if (hv_Num > 0)
                {
                    #region
                    listObj2Draw[1] = "NG-端头崩碎";
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("222缺陷最大面积：" + iErrArea.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Area1.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }



                //***端头缺陷检测-次要检测部分
                //HOperatorSet.OpeningCircle(ho_RegionOpening, out ho_RegionOpening2, 20);
                if (true)
                {
                    HObject ho_RegionAnisometry;
                    HOperatorSet.ErosionCircle(ho_RectPu, out ho_RegionErosion, iEroradis2);  //腐蚀半径8
                    //HOperatorSet.ReduceDomain(ho_ImageClosing, ho_RegionErosion, out ho_ImageReduced2);
                    HOperatorSet.Difference(ho_RegionErosion, ho_MaxRegion, out ho_ImageReduced2);

                    HOperatorSet.ErosionCircle(ho_ImageReduced2, out ho_ConnectedRegionDark2, ErosionCircle2);// ErosionCircle1
                    HOperatorSet.Connection(ho_ConnectedRegionDark2, out ho_ConnectedRegionDark);
                    HOperatorSet.SelectShape(ho_ConnectedRegionDark, out ho_RegionAnisometry, "anisometry", "and", 0, iAnisometry);

                    HOperatorSet.SelectShape(ho_RegionAnisometry, out ho_RegionErrD, "area", "and", iErrArea2, 99999);

                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrD, out hv_Num);
                    HOperatorSet.Union1(ho_RegionErrD, out hoUnion);
                    HOperatorSet.AreaCenter(hoUnion, out hv_Area1, out hv_Row2, out hv_Column2);
                    if (hv_Num > 0)
                    {
                        #region
                        listObj2Draw[1] = "NG-端头崩碎";
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_RegionErrD, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("111缺陷最大面积：" + iErrArea2.ToString() + " pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area1.D.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }



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




        //34相机 六面机前后面  引用文件 7
        public static List<object> sySixSideDetect_0402R_Camera34(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
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
                HTuple hv_Column1, hv_Max1, hv_Max2, hv_area;

                //***判断电阻是否靠近边界
                HTuple hv_Area7, hv_Row10, hv_Column10;
                HObject ho_RegionDifference2, ho_RegionErosion3, ho_RegionIntersection8;


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
                //3,4相机参数
                int iLength1 = int.Parse(strUserParam[4]);      //iLength1  = 145    半长
                int iLength1Scale = int.Parse(strUserParam[5]); //iLength1Scale = 15 半长变化值
                int iLength2 = int.Parse(strUserParam[6]);      //iLength2  = 45     半宽
                int iLength2Scale = int.Parse(strUserParam[7]); //iLength2Scale = 15 半宽变化值
                int iErrThres = int.Parse(strUserParam[8]);     //iErrThres  = 40    缺陷阈值40
                int iErrArea = int.Parse(strUserParam[9]);      //iErrArea = 50      缺陷面积500
                int iLouduArea1 = int.Parse(strUserParam[10]);    //iLouduArea1=15000     侧面挂锡区域面积15000
                int iclosing_rext = int.Parse(strUserParam[11]);//iclosing_rext=1(未启用) 初始掩模尺寸
                int iLouduArea2 = int.Parse(strUserParam[12]);    //iLouduArea2=1500    侧面挂锡区域面积1500
                int AreaDuanmian = int.Parse(strUserParam[13]);    //iLouduArea2=1500    侧面挂锡区域面积1500
                int iScale = int.Parse(strUserParam[14]);    //scale_image参数




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
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion




                HOperatorSet.ScaleImage(ho_ImageReduced, out ho_ImageClosing, 3, -iScale);

                //HOperatorSet.GrayClosingRect(ho_ImageReduced, out ho_ImageClosing, iclosing_rext, iclosing_rext);//11,11
                HOperatorSet.BinaryThreshold(ho_ImageClosing, out ho_RegionBinary, "max_separability",
                    "light", out hv_UsedThreshold);
                HOperatorSet.Connection(ho_RegionBinary, out ho_ConnectedRegions);
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_MaxRegion, "max_area", 70);
                HOperatorSet.AreaCenter(ho_MaxRegion, out Areakkk, out Rowkkk, out Colkkk);


                if (Areakkk < 0.5 * AreaDuanmian)  //****如果端面面积小于设置的最小端面面积的一半，判定无定位
                {
                    #region***粗定位面积不符合要求,判定无定位
                    listObj2Draw[1] = "NG-无定位"; //区域面积不符合要求
                                                //输出NG详情
                    HOperatorSet.CountObj(ho_MaxRegion, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_MaxRegion, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    lsInfo2Draw.Add("无定位-端面最小面积：" + (0.5 * AreaDuanmian).ToString() + "pix * ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + Areakkk.D.ToString("0.0") + " pix * ");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }


                if (Areakkk < AreaDuanmian)  //****粗定位面积54830
                {
                    #region***粗定位面积不符合要求,判定无定位
                    listObj2Draw[1] = "NG-尺寸不符"; //区域面积不符合要求
                                                 //输出NG详情
                    HOperatorSet.CountObj(ho_MaxRegion, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_MaxRegion, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    lsInfo2Draw.Add("端面最小面积：" + AreaDuanmian.ToString() + "pix * ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + Areakkk.D.ToString("0.0") + " pix * ");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }




                HOperatorSet.FillUp(ho_MaxRegion, out ho_RegionFillUp);
                HOperatorSet.OpeningRectangle1(ho_RegionFillUp, out ho_RegionOpening, 5, 5);
                HOperatorSet.SmallestRectangle2(ho_RegionOpening, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_RectPu, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);

                #region***判断电阻是否靠近边界                   
                //HOperatorSet.ErosionCircle(ho_ImageReduced, out ho_RegionErosion3, 1.1);
                //HOperatorSet.Intersection(ho_RegionErosion3, ho_RectPu, out ho_RegionIntersection8);
                //HOperatorSet.Difference(ho_RectPu, ho_RegionIntersection8, out ho_RegionDifference2);
                //HOperatorSet.AreaCenter(ho_RegionDifference2, out hv_Area7, out hv_Row10, out hv_Column10);
                //if ((int)(new HTuple(hv_Area7.TupleGreater(0))) != 0)
                //{
                //    #region
                //    listObj2Draw[1] = "NG-无定位";//角度歪斜
                //    List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                //    listObj2Draw.Add("多边形");
                //    listObj2Draw.Add(lnBarcode.ToArray());
                //    listObj2Draw.Add("NG");

                //    //输出NG详情
                //    lsInfo2Draw.Add("靠近边界");
                //    lsInfo2Draw.Add("NG");
                //    listObj2Draw.Add("字符串");
                //    listObj2Draw.Add(lsInfo2Draw);
                //    listObj2Draw.Add(new PointF(1800, 100));
                //    return listObj2Draw;
                //    #endregion
                //}
                #endregion

                //*判断产品尺寸（145 * 45）
                HTuple hv_Length1Scale = iLength1Scale;
                HTuple hv_Length2Scale = iLength2Scale;

                if ((int)((new HTuple(hv_Length1.TupleLess(iLength1 - hv_Length1Scale))).TupleOr(new HTuple(hv_Length1.TupleGreater(
                    iLength1 + hv_Length1Scale)))) != 0)
                {
                    #region
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG1 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG1.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";  //0508更改 NG-尺寸不符

                    //输出NG详情
                    lsInfo2Draw.Add("1标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                    iLength2 + hv_Length2Scale)))) != 0)
                {
                    #region                
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";  //0508更改 NG-尺寸不符

                    //输出NG详情
                    lsInfo2Draw.Add("1标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                //if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                //   iLength2 + hv_Length2Scale)))) != 0)
                //{
                #region
                //    //HDevelopStop();
                //    //HDevelopStop();
                //    //NG绘制红色矩形
                //    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                //    listObj2Draw.Add("多边形");
                //    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                //    listObj2Draw.Add("NG");

                //    listObj2Draw[1] = "NG-尺寸不符";  //0508更改 NG-尺寸不符

                //    //输出NG详情
                //    lsInfo2Draw.Add("2标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                //    lsInfo2Draw.Add("OK");
                //    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                //    lsInfo2Draw.Add("NG");
                //    listObj2Draw.Add("字符串");
                //    listObj2Draw.Add(lsInfo2Draw);
                //    listObj2Draw.Add(new PointF(1800, 100));
                //    return listObj2Draw;
                #endregion
                //}


                //OK绘制蓝色矩形
                List<PointF> lnBarcodeOK = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                listObj2Draw.Add("多边形");
                listObj2Draw.Add(lnBarcodeOK.ToArray());
                listObj2Draw.Add("OK");

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion


                HObject ho_Rectangle2, ho_RegionIntersection;
                HTuple hv_Area4, hv_Row3, hv_Column3;

                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row, hv_Column, hv_Phi, hv_Length1 * (120 / 214), hv_Length2);
                //*检测挂锡
                HObject ho_EdgeAmplitude, ho_Region1, ho_RegionClosing, ho_ImageReduced3;
                HTuple hv_Area3, hv_Row2, hv_Column2;
                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RectPu, out ho_ImageReduced3);
                HOperatorSet.SobelAmp(ho_ImageReduced3, out ho_EdgeAmplitude, "sum_abs", 3);
                HOperatorSet.Threshold(ho_EdgeAmplitude, out ho_Region1, 25, 255);
                HOperatorSet.ClosingCircle(ho_Region1, out ho_RegionClosing, 7);
                HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening2, 8);
                HOperatorSet.AreaCenter(ho_RegionOpening2, out hv_Area3, out hv_Row2, out hv_Column2);



                HOperatorSet.Intersection(ho_RegionOpening2, ho_Rectangle2, out ho_RegionIntersection);
                HOperatorSet.AreaCenter(ho_RegionIntersection, out hv_Area4, out hv_Row3, out hv_Column3);

                //中间区域挂锡面积
                if (hv_Area4 > iLouduArea2)
                {
                    #region
                    listObj2Draw[1] = "NG-侧面挂锡";
                    HOperatorSet.Connection(ho_RegionIntersection, out ho_RegionErrDConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrDConn, out hv_Num);
                    //hv_Max2= hv_Area[0]
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionErrDConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iLouduArea2.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Area4.ToString() + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion  
                }

                //焊锡总面积
                if (hv_Area3 > iLouduArea1)
                {
                    #region
                    listObj2Draw[1] = "NG-侧面挂锡";
                    HOperatorSet.Connection(ho_RegionOpening2, out ho_RegionErrDConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrDConn, out hv_Num);
                    //hv_Max2= hv_Area[0]
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionErrDConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iLouduArea1.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Area3.ToString() + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion  
                }

                //*检测内部缺陷
                HTuple hv_Row4, hv_Column4, hv_Phi1, hv_Length11, hv_Length21, hv_Area5, hv_Row5, hv_Column5;
                HObject ho_Rectangle1, ho_Rectangle3, ho_ConnectedDark, ho_SelectedRegions1;
                HOperatorSet.SmallestRectangle2(ho_RegionOpening2, out hv_Row4, out hv_Column4,
                    out hv_Phi1, out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row4, hv_Column4, hv_Phi1,
                    hv_Length11, hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_Row4, hv_Column4, hv_Phi1,
                hv_Length11 * (2 / 3), hv_Length21 * (4 / 7));
                //HOperatorSet.ErosionRectangle1(ho_Rectangle1, out ho_Rectangle3, 150, 80);


                HOperatorSet.ReduceDomain(ho_ImageReduced, ho_Rectangle3, out ho_ImageReduced2);
                HOperatorSet.Threshold(ho_ImageReduced2, out ho_RegionDark, 0, 80);
                HOperatorSet.Connection(ho_RegionDark, out ho_ConnectedDark);
                HOperatorSet.SelectShapeStd(ho_ConnectedDark, out ho_SelectedRegions1, "max_area", 70);
                HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area5, out hv_Row5, out hv_Column5);

                if (hv_Area5 > iErrArea)
                {
                    #region
                    //HDevelopStop();
                    listObj2Draw[1] = "NG-产品沾污";
                    HOperatorSet.Connection(ho_SelectedRegions1, out ho_RegionErrDConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrDConn, out hv_Num);
                    HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area, out hv_Row1, out hv_Column1);
                    HOperatorSet.TupleMax(hv_Area, out hv_Max1);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionErrDConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + iErrArea.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Area5.ToString() + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                    #endregion
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



      

        //56相机 六面机 正反面 引用文件 8
        private static long Number111 = 0;
        private static long Number555 = 0;//用于保存图片时排序
        private static long Number666 = 0;//用于保存图片时排序

        public static List<object> sySixSideDetect_0402R_Camera56(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  *** 56相机 六面机 正反面  ***

            string[] strUserParam = strParams.Split('#');

            //dhDll.clsFunction.writeTxtFile("D:\\1111.txt", System.IO.FileMode.Create, new List<string>(strUserParam));

            if (bUseMutex) muDetect8.WaitOne();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("NG"); listObj2Draw.Add(888);


            try
            {
                HObject hoReduced = null, hoConcate = null, hoRegion = null, hoUnion = null, ho_RegionSel = null, hoRegionsConn = null, hoSelectedRegions = null, ho_Rectangle = null, ho_ImageReduce = null, ho_RectangleDia = null, ho_Edges = null, ho_ShortEdges = null;
                HObject ho_RegionLight = null, ho_Err_RegionConn = null, ho_RegionConn = null, Rectbbb = null, ho_EdgeAmp1 = null, ho_RegionLie = null, ho_RegionLies = null, ho_ImageMean = null, ho_DarkPix = null;
                HObject ho_Image1, ho_ImageReduced, ho_Image3, ho_RegionUnion = null, ho_RegionClose = null, ho_Regionpen = null, ho_RegionTrans = null, ho_RegionDiff = null, ho_ImageReduce1 = null, ho_RegionDark = null, ho_ImageAmp = null;

                HTuple NChannel, hv_Num, hv_Length1, hv_Length2, Rowbbb, Colbbb, Phibbb, Length1bbb, Length2bbb;
                HTuple hv_Row, hv_Column, hv_Phi, hv_Area, hv_Mean, hv_Dev, hv_Row111, hv_Column111, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD;
                HObject ho_Rects, ho_RegionDifference, ho_RegionOpen, ho_RegionUnion2, ho_RegionOpen111, ho_ImageReduced222, ho_Regions2, ho_Regionpen2;
                HTuple hv_Area2, hv_Row2, hv_Column2;
                HObject hoRegion3, hoSelectedRegions3, ho_RegionOpen222, ho_Err_RegionConn333, ho_ImageReduce2, ho_ImageScaled;
                HObject ho_RegionClosing4, hoSelectedRegions1, ho_RegionFillUp1;
                HTuple hv_Num2, hv_heidianthr, hv_Ratio;

                // ***漏镀
                HObject ho_RegionDilation, ho_ImageSub, ho_Region2, ho_ConnectedRegions2, ho_SelectedRegions5, ho_Image11, ho_Image21, ho_Image31, ho_RegionFillUp2, ho_RegionErosion1;
                HTuple hv_Area3, hv_Row3, hv_Column3;

                HObject ho_RegionIntersection, ho_RegionIntersection2, ho_Rectangle2, ho_RegionOpen333, ho_RegionUnion3, ho_SortedRegions;

                //*使用动态阈值检测面积较大的黑色斑点
                HObject ho_DarkPixels, ho_ConnectedRegions, ho_RegionFillUp3, ho_SelectedRegions2, ho_RegionClosing, ho_Rectangle5;

                HTuple Length1DD, Length2DD;

                //***保护层边界区域IIG崩碎检测                     
                //****0820-exp
                HObject ho_Region, ho_SelectedRegions8;
                HTuple hv_Number;

                //0919像素值换算为实际值
                HTuple ipix = 0.003;//单像素精度(um/pix)
                HTuple ipix2 = 9;//面积单像素精度(um^2/pix^2)

                HTuple ixip = 1000 / 165;//反单像素精度

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

                //5,6相机参数

                int hv_leixing = int.Parse(strUserParam[3]);  //检测类型：车规-0、常规-1、不检-2、抗硫化-3、中兴抗硫化-4、HKRK-5、中兴-6、手机-7、华为-8、华为抗硫化-9
                //int hv_L1 = int.Parse(strUserParam[4]);  //
                int iProductCode = int.Parse(strUserParam[5]);  //产品类别：0--0402 ； 1--0402RMS



                //第一页
                int iFixThres = int.Parse(strUserParam[30]);  //粗定位阈值30
                int iAngleScale = int.Parse(strUserParam[31]); //歪斜角度正负极限 10
                int iBorderScale = int.Parse(strUserParam[32]); //产品到图像边界最大距离 50
                int iFixThres2 = int.Parse(strUserParam[33]); //产品到图像边界最大距离 50



                int Ilenth1 = int.Parse(strUserParam[34]);//产品长度1000
                int iLength1Scale = int.Parse(strUserParam[35]);//产品长度偏差100
                int Ilenth2 = int.Parse(strUserParam[36]);//产品宽度500
                int iLength2Scale = int.Parse(strUserParam[37]);//产品宽度偏差100





                int Ilenth4 = int.Parse(strUserParam[38]);// 电极宽度
                int Ilenth4Scale = int.Parse(strUserParam[39]);// 电极宽度偏差
                int Ilenth4Sum = int.Parse(strUserParam[40]);// 两电极宽度和
                int Ilenth4diff = int.Parse(strUserParam[41]);// 两电极宽度差

                int Ilenth3Scale = int.Parse(strUserParam[42]);// 电极长度偏差
                int Ilenth3diff = int.Parse(strUserParam[43]); // 两电极长度差



                int IWidthciti = int.Parse(strUserParam[44]);//瓷体长度最大值
                int iSaveImg = int.Parse(strUserParam[45]);//是否保存5、6相机的OK图片

                //第二页参数
                int iSmallestArea = int.Parse(strUserParam[46]); //电极最小面积
                int iBiggstArea = int.Parse(strUserParam[47]);   //电极最大面积
                int AutoThreshold1 = int.Parse(strUserParam[48]);//平均宽度-电极提取
                int ierosion_height = int.Parse(strUserParam[49]);//电极腐蚀高度

                //int iProtectexp2 = int.Parse(strUserParam[48]);//                            
                //int iIIG1 = int.Parse(strUserParam[49]);//

                //int iIIG2 = int.Parse(strUserParam[50]);//
                float Rectangularity1 = float.Parse(strUserParam[50]);//背面电极矩形度判定
                int iBaseErosion = int.Parse(strUserParam[51]);  //基板腐蚀半径15
                int izhanxi = int.Parse(strUserParam[52]);  //基板腐蚀半径15

                //int iIIG3 = int.Parse(strUserParam[51]);//
                //int iIIG4 = int.Parse(strUserParam[52]);//
                //int iIIG5 = int.Parse(strUserParam[53]);//IIG大范围磨损

                int iBlackArea1 = int.Parse(strUserParam[54]);//背面-全局沾污面积
                int iBlackArea2 = int.Parse(strUserParam[55]);//背面-电极沾污面积
                int iBlackArea3 = int.Parse(strUserParam[56]);//背面-瓷体沾污面积

                int iIIG1 = int.Parse(strUserParam[57]);//

                //int iIIG2 = int.Parse(strUserParam[50]);//
                //float Rectangularity1 = float.Parse(strUserParam[50]);//背面电极矩形度判定
                int iIIG3 = int.Parse(strUserParam[58]);//旧
                int iIIG4 = int.Parse(strUserParam[59]);//不明显IIG崩碎
                int iIIG5 = int.Parse(strUserParam[60]);//IIG大范围磨损
                int expYN = int.Parse(strUserParam[61]);//是否启用exp检测边界缺陷
                                                        //int iScalMult = int.Parse(strUserParam[57]);//

                //int iScalAdd = int.Parse(strUserParam[58]);//    
                ////int iArea2 = int.Parse(strUserParam[59]);//上爬不足总面积-150

                //int ierosion_height = int.Parse(strUserParam[61]);//保护层腐蚀高度

                //11
                //第三页参数
                //int iProtectBrokenArea3 = int.Parse(strUserParam[62]);//保护层挂锡面积2-100
                //int EroWidth2 = int.Parse(strUserParam[63]);//保护层腐蚀宽度2
                //int EroHeight2 = int.Parse(strUserParam[64]);//保护层腐蚀高度2
                //int IIGbianjie = int.Parse(strUserParam[65]);//IIG边界检测


                int iProtectBrokenArea = int.Parse(strUserParam[62]);//保护层破损面积
                int iProtectexp = int.Parse(strUserParam[63]); //焊锡缺陷检测，exp阈值-20
                int iProtectBrokenArea3 = int.Parse(strUserParam[64]);//保护层挂锡面积
                int iProtectBrokenResThres = int.Parse(strUserParam[65]); //保护层破损相对阈值 30





                //int EroWidth3 = int.Parse(strUserParam[66]);//保护层腐蚀宽度3
                //int EroHeight3 = int.Parse(strUserParam[67]);//保护层腐蚀高度3
                //int iIIGThres3 = int.Parse(strUserParam[68]);//IIG崩碎检测对比度阈值3
                //int iIIGArea3 = int.Parse(strUserParam[69]);//IIG崩碎检测面积阈值3

                int iProtectArea4 = int.Parse(strUserParam[66]);//不明显IIG崩碎面积-500
                int iProtectArea5 = int.Parse(strUserParam[67]);//大范围磨损面积-3800
                //int iIIGThres3 = int.Parse(strUserParam[68]);//IIG崩碎检测对比度阈值3
                //int iIIGArea3 = int.Parse(strUserParam[69]);//IIG崩碎检测面积阈值3

                int EroWidth4 = int.Parse(strUserParam[70]);//保护层腐蚀宽度4
                int EroHeight4 = int.Parse(strUserParam[71]);//保护层腐蚀高度4
                int EroWidth3 = int.Parse(strUserParam[72]);//保护层腐蚀宽度3
                int EroHeight3 = int.Parse(strUserParam[73]);//保护层腐蚀高度3

                //IIG崩碎-exp检测边界缺陷
                //int expYN = int.Parse(strUserParam[72]);//是否启用exp检测边界缺陷
                //int IIGexp2 = int.Parse(strUserParam[73]);//exp阈值
                //EroWidth1


                int EroWidth1 = int.Parse(strUserParam[74]);//闭运算阈值                                                                     
                int EroHeight1 = int.Parse(strUserParam[75]);//背面电极提取cirorrect
                int iProtectrThr4 = int.Parse(strUserParam[76]);  //不明显IIG崩碎提取阈值



                //第四页参数
                //背导提取
                int iScale_height_1 = int.Parse(strUserParam[78]);// 上爬高度-1     电极上爬不足高度收窄
                int iScale_width_1 = int.Parse(strUserParam[79]);//  上爬宽度-1      电极上爬不足宽度收窄
                int iwidth_aoxian = int.Parse(strUserParam[80]);//上爬不足凹陷宽度
                int iArea_shangpa3 = int.Parse(strUserParam[81]);//凹陷面积

                int iScale_height_2 = int.Parse(strUserParam[82]);//上爬高度-2
                int iScale_width_2 = int.Parse(strUserParam[83]);// 上爬宽度-2 
                int iRegionOpe = int.Parse(strUserParam[84]);// 焊锡缺陷检测开运算阈值                            
                int iArea_shangpa1 = int.Parse(strUserParam[85]);// 上爬不足总面积



                int iScale_height_3 = int.Parse(strUserParam[86]);//上爬高度-3
                int iScale_width_3 = int.Parse(strUserParam[87]);// 上爬宽度-3           
                int iwidth = int.Parse(strUserParam[88]);//上爬不足最小宽度
                int iArea = int.Parse(strUserParam[89]);//焊锡缺陷检测，最小面积

                //漏镀检测参数

                int hv_iloudu_mean = int.Parse(strUserParam[90]);//漏镀检测-电极区域三通道灰度值（小于该值视作漏镀）
                int hv_iloudu_divstd = int.Parse(strUserParam[91]);//漏镀检测-电极区域与瓷体区域灰度图像平均灰度值的差值（大于该值视作漏镀）

                int hv_AutoThreshold1 = int.Parse(strUserParam[92]);//背导区域提取阈值20
                int hv_KirschThr = int.Parse(strUserParam[93]);//瓷体区域提取阈值30
                //int hv_KirschClosing = int.Parse(strUserParam[46]);//瓷体区域闭运算3
                //int hv_KirschOpening = int.Parse(strUserParam[47]);//瓷体区域开运算5
                int hv_KirschClosing = 3;//瓷体区域闭运算3
                int hv_KirschOpening = 5;//瓷体区域开运算5

                //背面挂锡检测
                HTuple hv_Area4;
                HTuple iAreaLie2 = 100;//未启用
                //漏镀
                HTuple iAreaLoudu = 4000;
                HTuple iProtectBrokenArea2 = 200;//保护层挂锡面积1-200

                //
                HTuple iAreaDiff = 70000; //左右面积最大差异64000
                HTuple iBaseResThres = 50; //基板相对缺陷阈值60
                HTuple iBaseBlackArea = 300; //基板黑点面积最大300


                HObject RegionCnn, ho_RegionMax;
                HTuple hv_Area1, hv_L1;



                //0814
                HObject ho_ExpImage, ho_ImageScaleMax, ho_RegionDynThresh;
                HTuple hv_Rectangularity;//判断背面电极矩形度

                //***判断电阻是否靠近边界
                HTuple hv_Area7, hv_Row10, hv_Column10, hv_Width1, hv_Height1;
                HObject ho_SelectedRegions, ho_RegionDifference2, ho_RegionErosion3;
                HObject ho_Err_RegionConn444, ho_Err_RegionConn555;

                HObject ho_RegionOpening, ho_RegionClosing1, ho_RegionBinary;

                HObject ho_Regions, ho_SelectedRegions1, ho_GrayImage, ho_ImageEdgeAmp, ho_RegionErosion;
                HTuple hv_Row1, hv_Column1;

                //获取图像尺寸，用于检测五六号相机电阻是否靠近图像边缘
                HOperatorSet.GetImageSize(hoImage, out hv_Width1, out hv_Height1);

                #region***iProductCode-区分5、6号相机
                if (iProductCode == 5)
                {
                    hv_m = hv_m + 1;

                }
                else if (iProductCode == 6)
                {
                    hv_n = hv_n + 1;
                }
                #endregion




                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }


                #endregion



                //判断彩色还是黑白
                #region****判断彩色还是黑白
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
                #endregion

                //开始检测 ho_ImageReduced

                HOperatorSet.Threshold(ho_ImageReduced, out hoRegion, iFixThres, 255);   //参数：粗定位阈值30
                HOperatorSet.OpeningRectangle1(hoRegion, out ho_Regionpen, 5, 10);       //  5/10
                HOperatorSet.Connection(ho_Regionpen, out hoRegionsConn);
                HOperatorSet.SelectShape(hoRegionsConn, out hoSelectedRegions, "area", "and", 500, 99999);  //6000        
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);

                #region***粗定位电阻提取失败-黑图
                if (hv_Num == 0)
                {
                    listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                    syShowRegionBorder(hoUnion, ref listObj2Draw, "NG");  //显示搜索边界
                    //输出NG详情
                    lsInfo2Draw.Add("黑图");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;

                }
                #endregion

                #region****粗定位-产品整体角度、尺寸判定、图像是否靠近边缘、图像中是否有异物
                HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row111, out hv_Column111, out hv_Phi, out hv_Length1, out hv_Length2);

                HTuple Deg;
                HOperatorSet.TupleDeg(hv_Phi, out Deg);
                if (Deg > iAngleScale || Deg < (0 - iAngleScale))
                {
                    #region***判断产品角度，歪斜过大直接无定位 正负10度
                    listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                    syShowRegionBorder(ho_RegionTrans, ref listObj2Draw, "NG");
                    //输出NG详情
                    lsInfo2Draw.Add("歪斜角度:" + Deg.D.ToString("0.0") + " 度");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                HTuple hv_Length1_1, hv_Length2_2;
                HTuple hv_Row_std, hv_Column_std, hv_Phi_std, hv_Length1_std, hv_Length2_std;
                HTuple Ilenth1_min, Ilenth1_max;
                HObject ho_RegionTrans_std, ho_Rectangle_std;

                HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row_std, out hv_Column_std, out hv_Phi_std, out hv_Length1_std, out hv_Length2_std);
                HOperatorSet.GenRectangle2(out ho_Rectangle_std, hv_Row_std, hv_Column_std, hv_Phi_std, hv_Length1_std, hv_Length2_std);
                hv_Length1 = hv_Length1_std * 2 * ipix * 1000; //像素长度转换为实际距离
                hv_Length2 = hv_Length2_std * 2 * ipix * 1000; //像素长度转换为实际距离
                //Ilenth1_min = Ilenth1 - iLengthScale;
                //Ilenth1_max = Ilenth1 + iLengthScale;

                //粗定位-检测电阻长宽尺寸
                if ((hv_Length1 < (Ilenth1 - iLength1Scale)) || (hv_Length1 > (Ilenth1 + iLength1Scale)) || (hv_Length2 < (Ilenth2 - iLength2Scale)) || (hv_Length2 > (Ilenth2 + iLength2Scale)))
                {
                    #region****检测电阻整体长宽尺寸
                    listObj2Draw[1] = "NG-无定位";//尺寸差异较大                
                    syShowRegionBorder(ho_Rectangle_std, ref listObj2Draw, "NG");
                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + Ilenth1.ToString() + " um*" + Ilenth2.ToString() + "um");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " um * " + hv_Length2.D.ToString("0.0") + " um");
                    lsInfo2Draw.Add("NG");

                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                //判断电阻主体是否在图像边缘，如果在则无定位。              
                HTuple hv_Row11, hv_Column11, hv_Row21, hv_Column21;
                HOperatorSet.SmallestRectangle1(ho_RegionTrans, out hv_Row11, out hv_Column11, out hv_Row21, out hv_Column21);
                HOperatorSet.GenRectangle1(out ho_Rectangle_std, hv_Row11, hv_Column11, hv_Row21, hv_Column21);

                if (hv_Row11 < iBorderScale || hv_Column11 < iBorderScale || hv_Row21 > hv_Height1 - iBorderScale || hv_Column21 > hv_Width1 - iBorderScale)
                {
                    #region****无定位-电阻靠近图像边缘
                    listObj2Draw[1] = "NG-无定位";//电阻靠近图像边缘                                             
                    syShowRegionBorder(ho_Rectangle_std, ref listObj2Draw, "NG");
                    //输出NG详情
                    lsInfo2Draw.Add("电阻靠近图像边缘");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }


                #endregion

                HOperatorSet.SelectShape(hoRegionsConn, out hoSelectedRegions, "area", "and", 3500, 99999);  //6000        
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                HTuple hv_Num22 = hv_Num;


                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                    //listObj2Draw[1] = "NG-溅射深";//"NG-尺寸异常";
                    //return listObj2Draw;

                }
                #endregion

                //Interlocked.Increment(ref Number111);

                //ho_ImageReduced *****原始灰度图
                //hoSelectedRegions *****NG-粗定位高亮度区域/OK-电极区域


                #region ****设置正背面缺陷不检，但保留无定位检测
                if (hv_leixing == 2)
                {
                    listObj2Draw[1] = "OK";
                    lsInfo2Draw.Add("正背面不检");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;

                }
                #endregion




                if (hv_Num == 1) //背导
                {



                    #region ***电极尺寸参数设置***
                    if (true)
                    {
                        Ilenth4 = int.Parse(strUserParam[8]);// 电极宽度
                        Ilenth4Scale = int.Parse(strUserParam[9]);// 电极宽度偏差
                        Ilenth4Sum = int.Parse(strUserParam[10]);// 两电极宽度和
                        Ilenth4diff = int.Parse(strUserParam[11]);// 两电极宽度差
                    }
                    #region  ***华为背导尺寸***
                    if (hv_leixing == 8)  //华为正背面尺寸
                    {
                        hv_leixing = 6;//检测同中兴
                        Ilenth4 = 250;
                        Ilenth4Scale = 50;
                        Ilenth4Sum = 600;
                        Ilenth4diff = 100;
                    }

                    if (hv_leixing == 9) //华为抗硫化 
                    {
                        hv_leixing = 0; //检测同车规
                        Ilenth4 = 240;
                        Ilenth4Scale = 60;
                        Ilenth4Sum = 600;
                        Ilenth4diff = 100;
                    }
                    #endregion

                    #endregion

                    //背导尺寸设置

                    #region ---- *** 背导朝上  *** ----

                    //0508更改，选取最大黑点作为缺陷


                    HOperatorSet.OpeningRectangle1(hoSelectedRegions, out hoSelectedRegions1, 30, 5);
                    HOperatorSet.Union1(hoSelectedRegions1, out hoRegion);
                    HOperatorSet.SmallestRectangle2(hoRegion, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);



                    #region//检查全局黑点
                    HOperatorSet.FillUp(hoSelectedRegions1, out ho_RegionFillUp1);
                    HOperatorSet.ShapeTrans(ho_RegionFillUp1, out ho_RegionTrans, "convex");
                    HOperatorSet.ErosionCircle(ho_RegionTrans, out hoRegion, iBaseErosion);//参数第13位    //检查黑点矩形腐蚀半径15
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);
                    HOperatorSet.Threshold(ho_ImageReduce, out hoRegion, 0, 80);         //黑点阈值iBlackThres = 75
                    HOperatorSet.OpeningCircle(hoRegion, out ho_Regionpen, 5);
                    HOperatorSet.Connection(ho_Regionpen, out RegionCnn);
                    HOperatorSet.SelectShapeStd(RegionCnn, out ho_RegionMax, "max_area", 70);
                    HOperatorSet.FillUp(ho_RegionMax, out ho_RegionFillUp1);
                    HOperatorSet.AreaCenter(ho_RegionFillUp1, out hv_Area1, out hv_Row, out hv_Column);
                    hv_Area = hv_Area1 * ipix2;
                    if (hv_Area > iBlackArea1)      //参数44：黑点最大面积iBlackArea = 500
                    {
                        #region
                        listObj2Draw[1] = "NG-产品沾污"; //"NG-产品异常";
                        HOperatorSet.Connection(ho_RegionMax, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情
                        lsInfo2Draw.Add("全局沾污-最大面积：" + iBlackArea1.ToString("0.0") + "um^2");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "um^2");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));

                        return listObj2Draw;
                        #endregion
                    }

                    #endregion

                    HObject ho_ImageReduced2, ho_SelectedRegions6, ho_SelectedRegions_citi;
                    HObject ho_SelectedRegions100, ho_SelectedRegions101;//背面瓷体区域

                    #region ****背导电极提取、尺寸判断、面积判断
                    ho_GrayImage = ho_ImageReduced;

                    HOperatorSet.AutoThreshold(ho_GrayImage, out ho_Regions, AutoThreshold1);
                    HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions);
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, ((new HTuple("area")).TupleConcat(
                        "width")).TupleConcat("height"), "and", ((new HTuple(9999)).TupleConcat(
                        260)).TupleConcat(100), ((new HTuple(9999999)).TupleConcat(420)).TupleConcat(
                        280));



                    HOperatorSet.ClosingCircle(ho_SelectedRegions1, out ho_RegionClosing, 5);
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area", "and", 150, 99999);


                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "convex");
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionTrans, out ho_ImageReduced2);


                    HOperatorSet.KirschAmp(ho_ImageReduced2, out ho_ImageEdgeAmp);
                    HOperatorSet.Threshold(ho_ImageEdgeAmp, out ho_Region, 0, 58);



                    HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 1.5);
                    HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening, 5);
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions100, "max_area", 70);
                    HOperatorSet.FillUp(ho_SelectedRegions100, out ho_SelectedRegions);
                    ho_SelectedRegions_citi = ho_SelectedRegions;
                    HTuple hv_Height_citi, hv_Width_citi, hv_Height, hv_Width;
                    #region ****车规检测瓷体长度
                    if (hv_leixing == 0)
                    {
                        HOperatorSet.HeightWidthRatio(ho_SelectedRegions100, out hv_Height, out hv_Width, out hv_Ratio);
                        hv_Width_citi = hv_Width * ipix * 1000;
                        if (hv_Width_citi < 300 || hv_Width_citi > IWidthciti)
                        {
                            #region***瓷体宽度不符，判定尺寸不符
                            listObj2Draw[1] = "NG-尺寸不符";
                            HOperatorSet.Connection(ho_SelectedRegions100, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("瓷体宽度范围：" + "300-" + IWidthciti + "um");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前宽度：" + hv_Width_citi.ToString() + "um");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                        }
                    }
                    #endregion


                    //HObject ho_RegionDifference111;
                    //HOperatorSet.Difference(ho_RegionTrans, ho_SelectedRegions, out ho_RegionDifference111);
                    HOperatorSet.Difference(ho_RegionTrans, ho_SelectedRegions, out ho_RegionDifference);

                    HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionErosion, 1, ierosion_height);
                    HOperatorSet.OpeningCircle(ho_RegionErosion, out ho_RegionOpening, 5);
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);

                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions6, "area", "and", 1500, 99999);
                    HOperatorSet.CountObj(ho_SelectedRegions6, out hv_Num);
                    if (hv_Num != 2)
                    {
                        #region*** 两电极提取失败-尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.Connection(ho_SelectedRegions6, out ho_ConnectedRegions);
                        HOperatorSet.CountObj(ho_ConnectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_ConnectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情

                        lsInfo2Draw.Add("尺寸-电极提取失败" + hv_Num);
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    HOperatorSet.SortRegion(ho_SelectedRegions6, out ho_SortedRegions, "first_point", "true", "column");
                    HOperatorSet.AreaCenter(ho_SortedRegions, out hv_Area, out hv_Row, out hv_Column);
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1_1, out hv_Length2_2);

                    hoSelectedRegions = ho_SortedRegions;
                    //判断电极矩形度                               
                    HOperatorSet.Rectangularity(hoSelectedRegions, out hv_Rectangularity);
                    if ((int)((new HTuple(((hv_Rectangularity.TupleSelect(0))).TupleLess(Rectangularity1))).TupleOr(
                        new HTuple(((hv_Rectangularity.TupleSelect(1))).TupleLess(Rectangularity1)))) != 0)
                    {
                        #region***电极矩形度不符，判定尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符";//0813修改;
                                                    //HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("矩形度下限：" + Rectangularity1 + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Rectangularity.TupleSelect(0) + "," + hv_Rectangularity.TupleSelect(1) + "pix");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    //平均宽度测量 
                    #region****平均宽度测量                 
                    HTuple hv_width;
                    hv_width = (hv_Area / (hv_Length1_1 * 2)) / 2;
                    //Length2DDD = hv_width;

                    Length2DDD = hv_width * 2 * ipix * 1000; //像素长度转换为实际距离
                    hv_width = Length2DDD;
                    //hv_Length2 = hv_Length2_2 * 2 * ipix * 1000; //像素长度转换为实际距离


                    if ((Length2DDD.TupleSelect(0) < (Ilenth4 - Ilenth4Scale)) || (Length2DDD.TupleSelect(1) < (Ilenth4 - Ilenth4Scale)))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过小
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情

                        lsInfo2Draw.Add("平均-电极宽度上下限：" + (Ilenth4 - Ilenth4Scale) + "-" + (Ilenth4 + Ilenth4Scale) + "um ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("1当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((Length2DDD.TupleSelect(0) > (Ilenth4 + Ilenth4Scale)) || (Length2DDD.TupleSelect(1) > (Ilenth4 + Ilenth4Scale)))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情

                        lsInfo2Draw.Add("平均-电极宽度上下限：" + (Ilenth4 - Ilenth4Scale) + "-" + (Ilenth4 + Ilenth4Scale) + "um ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("2当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length2DDD.TupleSelect(0) - Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4diff))
                    {
                        #region*** 两电极宽度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("平均-电极宽度最大差值：" + Ilenth4diff + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("3当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D - Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length2DDD.TupleSelect(0) + Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4Sum))
                    {
                        #region*** 两电极宽度Sum值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("平均-电极宽度最大Sum值：" + Ilenth4Sum + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前Sum值：" + Math.Abs(Length2DDD.TupleSelect(0).D + Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion

                    //电极面积判定
                    #region***电极面积判定
                    //检查导体面积、尺寸
                    HOperatorSet.AreaCenter(hoSelectedRegions, out hv_Area, out hv_Row, out hv_Column);
                    hv_Area = hv_Area * ipix2;
                    if ((hv_Area.TupleSelect(0) < iSmallestArea) || (hv_Area.TupleSelect(1) < iSmallestArea))    //面积小于iSmallestArea = 6000
                    {
                        #region***判断电极面积是否过小，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";//"NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积下限：" + iSmallestArea.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea) || (hv_Area.TupleSelect(1) > iBiggstArea))      //面积大于iBiggstArea = 17000
                    {
                        #region***判断电极面积是否过大，如果是则溅射深
                        listObj2Draw[1] = "NG-溅射深"; //"NG-电极面积过大"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积上限：" + iBiggstArea.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于iAreaDiff = 8000
                    {
                        #region***判断两电极面积差值是否过大，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";// "NG-电极大小端";  //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "um^2 ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    #endregion

                    //rect-长宽判定
                    #region****电极长宽判定

                    HOperatorSet.SortRegion(hoSelectedRegions, out ho_SortedRegions, "first_point", "true", "column");
                    //导体长边不能小于0.75 *85
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                    HOperatorSet.GenRectangle2(out ho_Rects, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);
                    //HOperatorSet.HeightWidthRatio(ho_Rects, out Length1DD, out Length2DD, out hv_Ratio);

                    //Length1DDD = Length1DD / 2;
                    //Length2DDD = Length2DD / 2;

                    Length1DDD = hv_Length1_1 * 2 * ipix * 1000; //像素长度转换为实际距离
                    Length2DDD = hv_Length2_2 * 2 * ipix * 1000; //像素长度转换为实际距离                 

                    if ((Length1DDD.TupleSelect(0) < (Ilenth2 - Ilenth3Scale)) || (Length1DDD.TupleSelect(1) < (Ilenth2 - Ilenth3Scale)))

                    {
                        #region  ****电极长度波动幅度-电极长度过小
                        //if ((Length1DDD.TupleSelect(0) < Iwave2 * Ilenth2) || (Length1DDD.TupleSelect(1) < Iwave2 * Ilenth2)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length1DDD.TupleSelect(0) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0 || (int)(new HTuple(((((Length1DDD.TupleSelect(1) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0)
                        listObj2Draw[1] = "NG-上爬不足";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + (Ilenth2 - Ilenth3Scale) + "-" + (Ilenth2 + Ilenth3Scale) + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }



                    if ((Length1DDD.TupleSelect(0) > (Ilenth2 + Ilenth3Scale)) || (Length1DDD.TupleSelect(1) > (Ilenth2 + Ilenth3Scale)))

                    {
                        #region  ****电极长度波动幅度-电极长度过大
                        listObj2Draw[1] = "NG-溅射深";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + (Ilenth2 - Ilenth3Scale) + "-" + (Ilenth2 + Ilenth3Scale) + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length1DDD.TupleSelect(0) - Length1DDD.TupleSelect(1)).TupleAbs() > Ilenth3diff))
                    {
                        #region***两电极长度差值
                        listObj2Draw[1] = "NG-上爬不足"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("c电极长度最大差值：" + Ilenth3diff + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length1DDD.TupleSelect(0).D - Length1DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }



                    #region  ****电极宽度波动幅度-电极宽度过小
                    //if ((Length2DDD.TupleSelect(0) < (Ilenth3 * (1 - Iwave3))) || (Length2DDD.TupleSelect(1) < (Ilenth3 * (1 - Iwave3))))
                    //{
                    //    
                    //    //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                    //    //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)

                    //    listObj2Draw[1] = "NG-上爬不足";//"NG-电极宽度过窄";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("2电极宽度上下限：" + ((1 - Iwave3) * Ilenth3) + "-" + ((1 + Iwave3) * Ilenth3) + "um ");

                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //   
                    //}
                    #endregion
                    #region  ****电极宽度波动幅度-电极宽度过大
                    //if ((Length2DDD.TupleSelect(0) > (Ilenth3 * (1 + Iwave3))) || (Length2DDD.TupleSelect(1) > (Ilenth3 * (1 + Iwave3))))
                    //{
                    //   
                    //    //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                    //    //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                    //    listObj2Draw[1] = "NG-溅射深";//"NG-电极宽度过窄";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("2电极宽度上下限：" + ((1 - Iwave3) * Ilenth3) + "-" + ((1 + Iwave3) * Ilenth3) + "um ");

                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //   
                    //}
                    #endregion



                    if ((Length2DDD.TupleSelect(0) > (hv_width.TupleSelect(0) + (120))) || (Length2DDD.TupleSelect(1) > (hv_width.TupleSelect(1) + (120))))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                        listObj2Draw[1] = "NG-溅射深";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + (hv_width.TupleSelect(0) - (120)) + "-" + (hv_width.TupleSelect(0) + (120)) + "um ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("延锡-当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    if (((Length2DDD.TupleSelect(0) - Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4diff))
                    {
                        #region***两电极宽度差值
                        listObj2Draw[1] = "NG-上爬不足"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("k电极宽度最大差值：" + Ilenth4diff + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D - Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }



                    #endregion

                    #endregion


                    HObject ho_Region_dianji, ho_Region_citi;
                    //检测电极上的黑色斑点
                    ho_Regionpen = hoSelectedRegions;
                    ho_Region_dianji = ho_Regionpen;
                    ho_Region_citi = ho_SelectedRegions_citi;
                    #region 检测电极上的黑色斑点
                    HOperatorSet.Union1(ho_Regionpen, out ho_SelectedRegions2);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_SelectedRegions2, out ho_ImageReduced222);
                    HOperatorSet.Threshold(ho_ImageReduced222, out ho_Regions2, 0, 90);//87
                    HOperatorSet.OpeningCircle(ho_Regions2, out ho_Regionpen2, 4);
                    HOperatorSet.Connection(ho_Regionpen2, out hoRegion3);
                    HOperatorSet.SelectShape(hoRegion3, out ho_SelectedRegions2, "area", "and", 50, 999999);  //选取大于3500的区域作为焊锡区域
                    HOperatorSet.Union1(ho_SelectedRegions2, out ho_ConnectedRegions);
                    HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row2, out hv_Column2);
                    hv_Area2 = hv_Area * ipix2;
                    //HOperatorSet.CountObj(hoSelectedRegions3, out hv_Num2);
                    if (hv_Area2 > iBlackArea2) //检测电极上的黑色斑点
                    {
                        #region
                        listObj2Draw[1] = "NG-上爬不足";// "NG-电极损伤";
                        HOperatorSet.Connection(ho_ConnectedRegions, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("电极-缺损上限：" + iBlackArea2 + "um^2");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area2.TupleSelect(0).D.ToString("0.0") + "um^2 ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    #endregion



                    //检查陶瓷基板区域                   
                    //HOperatorSet.Union1(hoSelectedRegions, out hoRegion);
                    //HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    //HOperatorSet.Difference(ho_RegionTrans, hoRegion, out ho_RegionDiff);
                    ////HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 10);  //腐蚀半径14
                    HOperatorSet.ErosionRectangle1(ho_SelectedRegions_citi, out hoRegion, 30, 10);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce1);

                    // 使用动态阈值检测面积较大的黑色斑点

                    if (true)
                    {
                        #region****产品沾污检测
                        HOperatorSet.MeanImage(ho_ImageReduce1, out ho_ImageMean, 60, 60);//0617修改：30,30
                        HOperatorSet.DynThreshold(ho_ImageReduce1, ho_ImageMean, out ho_DarkPixels, izhanxi, "dark");//15
                        HOperatorSet.ClosingCircle(ho_DarkPixels, out ho_RegionClosing, 3);
                        //HOperatorSet.ErosionRectangle1(ho_DarkPixels, out ho_RegionClosing, 30, 12);//12 25,//0618修改，25，12//0816:20,8
                        HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                        HOperatorSet.FillUp(ho_ConnectedRegions, out ho_RegionFillUp3);
                        //HOperatorSet.SelectShapeStd(ho_RegionFillUp3, out ho_SelectedRegions2, "max_area", 70);
                        HOperatorSet.SelectShape(ho_RegionFillUp3, out ho_SelectedRegions2, "area", "and", 100, 999999);  //选取大于3500的区域作为焊锡区域
                        HOperatorSet.Union1(ho_SelectedRegions2, out ho_ConnectedRegions);
                        HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area2, out hv_Row2, out hv_Column2);
                        hv_Area2 = hv_Area2 * ipix2;
                        if (hv_Area2 > iBlackArea3)  //斑点面积
                        {
                            #region
                            listObj2Draw[1] = "NG-产品沾污";//"NG-非导电性侧裂";//"NG-保护层异常"
                            HOperatorSet.Connection(ho_ConnectedRegions, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //syShowRegionBorder(hoRegion, ref listObj2Draw, "NG");

                            //输出NG详情
                            lsInfo2Draw.Add("瓷体-面积上限：" + iBlackArea3.ToString() + "um^2 ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area2.D.ToString("0.0") + "um^2  ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                        }

                        #endregion
                    }

                    //使用0603R算法检测背面电极和瓷体
                    ho_GrayImage = hoReduced;
                    HTuple hv_Parameter_BM = new HTuple();
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_AutoThreshold1);
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_KirschThr);
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_KirschClosing);
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_KirschOpening);

                    syShowRegionBorder(ho_Region_dianji, ref listObj2Draw, "OK");
                    syShowRegionBorder(ho_Region_citi, ref listObj2Draw, "OK");

                    //dianji_beimian(ho_GrayImage, out ho_Region_dianji, out ho_Region_citi, hv_Parameter_BM);

                    //syShowRegionBorder(ho_Region_dianji, ref listObj2Draw, "NG");
                    //syShowRegionBorder(ho_Region_citi, ref listObj2Draw, "NG");

                    //漏镀检测
                    #region ***漏镀缺陷检测
                    HTuple hv_Parameter_LD = new HTuple();
                    HTuple hv_loudu_Mean = new HTuple();
                    HTuple hv_loudu_div = new HTuple();
                    HTuple mean_MK = new HTuple();
                    HTuple hv_NGCode = new HTuple();


                    hv_Parameter_LD = hv_Parameter_LD.TupleConcat(hv_iloudu_mean);
                    hv_Parameter_LD = hv_Parameter_LD.TupleConcat(hv_iloudu_divstd);

                    //loudu_2(hoReduced, ho_Region_dianji, hv_Parameter_LD, out hv_NGCode, out mean_MK);


                    loudu_2(hoReduced, ho_Region_dianji, ho_Region_citi, hv_Parameter_LD, out hv_NGCode, out hv_loudu_Mean, out hv_loudu_div);








                    #region ***程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion

                    HTuple is_Debug;
                    is_Debug = 1;
                    #region 调试模式

                    if (is_Debug)
                    {
                        syShowRegionBorder(ho_Region_dianji, ref listObj2Draw, "NG");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    }

                    #endregion



                    if ((int)(new HTuple(hv_NGCode.TupleEqual(40))) != 0)

                    {
                        #region*** NG-产品背电极-漏镀

                        listObj2Draw[1] = "NG-漏镀";
                        syShowRegionBorder(ho_Region_dianji, ref listObj2Draw, "NG");
                        //输出NG详情
                        lsInfo2Draw.Add("标准电极三通道灰度值：" + hv_iloudu_mean.ToString("0.0"));
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前电极三通道灰度值：" + hv_loudu_Mean.D.ToString("0.0"));
                        lsInfo2Draw.Add("NG");
                        lsInfo2Draw.Add("标准电极瓷体灰度差值：" + hv_iloudu_divstd.ToString("0.0"));
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前当前电极瓷体灰度差值：" + hv_loudu_div.D.ToString("0.0"));
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }

                    //ho_Region_return = ho_Region_dianji;
                    #endregion




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

                else if (hv_Num == 2)//正导
                {
                    #region ***电极尺寸参数设置***

                    if (true)
                    {
                        Ilenth4 = int.Parse(strUserParam[12]);// 电极宽度
                        Ilenth4Scale = int.Parse(strUserParam[13]);// 电极宽度偏差
                        Ilenth4Sum = int.Parse(strUserParam[14]);// 两电极宽度和
                        Ilenth4diff = int.Parse(strUserParam[15]);// 两电极宽度差
                    }
                    #region  ***华为正导尺寸***
                    if (hv_leixing == 8)  //华为正背面尺寸
                    {
                        hv_leixing = 6;//检测同中兴
                        Ilenth4 = 200;
                        Ilenth4Scale = 100;
                        Ilenth4Sum = 600;
                        Ilenth4diff = 150;
                    }

                    if (hv_leixing == 9) //华为抗硫化 
                    {
                        hv_leixing = 0; //检测同车规
                        Ilenth4 = 250;
                        Ilenth4Scale = 100;
                        Ilenth4Sum = 650;
                        Ilenth4diff = 150;
                    }
                    #endregion
                    #endregion
                    //正导尺寸设置

                    #region ---- *** 正导朝上  *** ----


                    #region ---- *** 无定位+尺寸检测  *** ----

                    #region ****电极提取
                    //输入：ho_ImageReduced
                    //输出：hoSelectedRegionsIIG，hoSelectedRegions_dianji_zhengmian（电极region）
                    HTuple hv_UsedThreshold;

                    //0822修改
                    HOperatorSet.BinaryThreshold(ho_ImageReduced, out ho_Region, "max_separability", "light", out hv_UsedThreshold);
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionBinary, hv_UsedThreshold + iFixThres2, 255); //10
                    HOperatorSet.ClosingRectangle1(ho_RegionBinary, out ho_RegionClosing1, 8, 3);
                    HOperatorSet.Connection(ho_RegionClosing1, out hoRegionsConn);
                    HOperatorSet.SelectShape(hoRegionsConn, out hoSelectedRegions, "area", "and", 3500, 99999);  //4000、、、400
                    HObject hoSelectedRegionsIIG = hoSelectedRegions;
                    HObject hoSelectedRegions_dianji_zhengmian = hoSelectedRegions;
                    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    if (hv_Num != 2)
                    {
                        #region***电极提取失败-尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符";//"NG-尺寸异常";                                           
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("111电极提取失败");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion

                    #region ****电阻整体角度判定+长宽判定
                    //输入：hoSelectedRegions_dianji_zhengmian
                    //输出：ho_RegionTrans_dianji_zhengmian
                    //输出：ho_RegionRect2_dianji_zhengmian
                    HObject ho_RegionRect2_dianji_zhengmian, ho_RegionTrans_dianji_zhengmian;
                    HOperatorSet.Union1(hoSelectedRegions_dianji_zhengmian, out hoRegion);
                    HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row111, out hv_Column111, out hv_Phi, out hv_Length1_std, out hv_Length2_std);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, hv_Length1_std, hv_Length2_std);
                    ho_RegionRect2_dianji_zhengmian = ho_Rectangle;
                    ho_RegionTrans_dianji_zhengmian = ho_RegionTrans;


                    //0508更改，判断产品角度，歪斜过大直接无定位 正负10度

                    HOperatorSet.TupleDeg(hv_Phi, out Deg);
                    if (Deg > iAngleScale || Deg < (0 - iAngleScale))
                    {
                        #region***判断产品角度，歪斜过大直接无定位 正负10度
                        listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                        syShowRegionBorder(ho_Rectangle, ref listObj2Draw, "NG");
                        //输出NG详情
                        lsInfo2Draw.Add("歪斜角度:" + Deg.D.ToString("0.0") + " 度");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    hv_Length1 = hv_Length1_std * 2 * ipix * 1000; //像素长度转换为实际距离
                    hv_Length2 = hv_Length2_std * 2 * ipix * 1000; //像素长度转换为实际距离
                                                                   //检测电阻长宽尺寸
                    if ((hv_Length1 < (Ilenth1 - iLength1Scale)) || (hv_Length1 > (Ilenth1 + iLength1Scale)) || (hv_Length2 < (Ilenth2 - iLength2Scale)) || (hv_Length2 > (Ilenth2 + iLength2Scale)))
                    {
                        #region****检测电阻整体长宽尺寸
                        listObj2Draw[1] = "NG-无定位";//尺寸差异较大                
                        syShowRegionBorder(ho_RegionRect2_dianji_zhengmian, ref listObj2Draw, "NG");
                        //输出NG详情
                        lsInfo2Draw.Add("标准尺寸：" + Ilenth1.ToString() + " um*" + Ilenth2.ToString() + "um");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " um * " + hv_Length2.D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion

                    #region****电极面积判定

                    HOperatorSet.AreaCenter(hoSelectedRegions_dianji_zhengmian, out hv_Area, out hv_Row, out hv_Column);
                    hv_Area = hv_Area * ipix2;
                    if ((hv_Area.TupleSelect(0) < iSmallestArea) || (hv_Area.TupleSelect(1) < iSmallestArea))    //面积小于iSmallestArea = 6000
                    {
                        #region***判断电极面积是否过小，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";//"NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积下限：" + iSmallestArea.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea) || (hv_Area.TupleSelect(1) > iBiggstArea))      //面积大于iBiggstArea = 17000
                    {
                        #region***判断电极面积是否过大，如果是则溅射深
                        listObj2Draw[1] = "NG-溅射深"; //"NG-电极面积过大"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积上限：" + iBiggstArea.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于iAreaDiff = 8000
                    {
                        #region***判断两电极面积差值是否过大，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";// "NG-电极大小端";  //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "um^2 ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion

                    #region****电极长宽判定                   

                    HOperatorSet.SortRegion(hoSelectedRegions_dianji_zhengmian, out ho_SortedRegions, "first_point", "true", "column");
                    //导体长边不能小于0.75 *85
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                    HOperatorSet.GenRectangle2(out ho_Rects, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);
                    //HOperatorSet.HeightWidthRatio(ho_Rects, out Length1DD, out Length2DD, out hv_Ratio);

                    //Length1DDD = Length1DD / 2;
                    //Length2DDD = Length2DD / 2;

                    Length1DDD = hv_Length1_1 * 2 * ipix * 1000; //像素长度转换为实际距离
                    Length2DDD = hv_Length2_2 * 2 * ipix * 1000; //像素长度转换为实际距离                 

                    #region ****电极长度测量
                    if ((Length1DDD.TupleSelect(0) < (Ilenth2 - Ilenth3Scale)) || (Length1DDD.TupleSelect(1) < (Ilenth2 - Ilenth3Scale)))
                    {
                        #region  ****电极长度波动幅度-电极长度过小
                        //if ((Length1DDD.TupleSelect(0) < Iwave2 * Ilenth2) || (Length1DDD.TupleSelect(1) < Iwave2 * Ilenth2)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length1DDD.TupleSelect(0) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0 || (int)(new HTuple(((((Length1DDD.TupleSelect(1) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0)
                        listObj2Draw[1] = "NG-上爬不足";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + (Ilenth2 - Ilenth3Scale) + "-" + (Ilenth2 + Ilenth3Scale) + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((Length1DDD.TupleSelect(0) > (Ilenth2 + Ilenth3Scale)) || (Length1DDD.TupleSelect(1) > (Ilenth2 + Ilenth3Scale)))

                    {
                        #region  ****电极长度波动幅度-电极长度过大
                        listObj2Draw[1] = "NG-溅射深";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + (Ilenth2 - Ilenth3Scale) + "-" + (Ilenth2 + Ilenth3Scale) + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length1DDD.TupleSelect(0) - Length1DDD.TupleSelect(1)).TupleAbs() > Ilenth3diff))
                    {
                        #region***两电极长度差值
                        listObj2Draw[1] = "NG-上爬不足"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("c电极长度最大差值：" + Ilenth3diff + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length1DDD.TupleSelect(0).D - Length1DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion


                    #region ****电极宽度测量

                    if ((Length2DDD.TupleSelect(0) < (Ilenth4 - Ilenth4Scale)) || (Length2DDD.TupleSelect(1) < (Ilenth4 - Ilenth4Scale)))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过小
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)

                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + (Ilenth4 - Ilenth4Scale) + "-" + (Ilenth4 + Ilenth4Scale) + "pix ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    if ((Length2DDD.TupleSelect(0) > (Ilenth4 + Ilenth4Scale)) || (Length2DDD.TupleSelect(1) > (Ilenth4 + Ilenth4Scale)))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + (Ilenth4 - Ilenth4Scale) + "-" + (Ilenth4 + Ilenth4Scale) + "pix ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    if (((Length2DDD.TupleSelect(0) + Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4Sum))
                    {
                        #region***两电极宽度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("k电极宽度最大和：" + Ilenth4Sum + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D + Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    if (((Length2DDD.TupleSelect(0) - Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4diff))
                    {
                        #region***两电极宽度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("k电极宽度最大差值：" + Ilenth4diff + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D - Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }




                    #endregion 


                    #endregion

                    #region***上爬不足检测
                    if (true)
                    {
                        HOperatorSet.SortRegion(hoSelectedRegions_dianji_zhengmian, out ho_SortedRegions, "first_point", "true", "column");
                        HOperatorSet.SmallestRectangle2(ho_SortedRegions, out RowDDD, out ColDDD, out PhiDDD, out Length1DDD, out Length2DDD);
                        HOperatorSet.AreaCenter(ho_SortedRegions, out hv_Area, out hv_Row, out hv_Column);

                        #region****上爬不足检测test-1
                        HOperatorSet.GenRectangle2(out ho_Rects, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD);
                        HOperatorSet.GenRectangle2(out ho_Rectangle2, RowDDD, ColDDD, PhiDDD, Length1DDD - 30, Length2DDD);

                        HObject ho_Rects3, ho_Rects4, ho_RegionUnion_3, ho_RegionUnion_4, ho_SelectedRegions3, ho_Rects_std, ho_RegionUnion_std, ho_RegionDifference1, ho_RegionOpening5,
                            ho_RegionIntersection1, ho_ConnectedRegions4, ho_SelectedRegions4;
                        HTuple hv_Row4, hv_Column4, hv_Deg, hv_Length11, hv_Length22, hv_Row5, hv_Column5, hv_L;
                        hv_Row3 = RowDDD;
                        hv_Column3 = ColDDD;
                        hv_Phi = PhiDDD;
                        hv_Length11 = Length1DDD;
                        hv_Length22 = Length2DDD;

                        HObject ho_RegionIntersection3, ho_ConnectedRegions1, ho_RegionIntersection4;
                        HTuple hv_Row6, hv_Column6, hv_Row7, hv_Column7;

                        if (hv_leixing == 0 || hv_leixing == 3 || hv_leixing == 4) //0-车规、3-抗硫化、4-中兴抗硫化
                        {
                            #region****上爬不足-车规（同车规）、抗硫化、中兴抗硫化

                            HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);
                            hv_Row4 = hv_Row3.Clone();
                            hv_L = (hv_Area / (Length1DDD * 2)) / 2;

                            hv_Column4 = new HTuple();
                            hv_Column4[0] = hv_Column3.TupleSelect(0) - hv_Length22.TupleSelect(0) + hv_L.TupleSelect(0);
                            hv_Column4[1] = hv_Column3.TupleSelect(1) + hv_Length22.TupleSelect(1) - hv_L.TupleSelect(1);


                            HOperatorSet.GenRectangle2(out ho_Rects_std, hv_Row4, hv_Column4, hv_Phi, hv_Length11, hv_L);
                            HOperatorSet.Union1(ho_Rects_std, out ho_RegionUnion_std);

                            hv_Row4 = hv_Row3.Clone();
                            hv_L1 = iScale_width_1;
                            hv_L = new HTuple();
                            hv_L = hv_L.TupleConcat(hv_L1);
                            hv_L = hv_L.TupleConcat(hv_L1);
                            hv_Column4 = new HTuple();
                            hv_Column4[0] = hv_Column3.TupleSelect(0) - hv_Length22.TupleSelect(0) + hv_L.TupleSelect(0);
                            hv_Column4[1] = hv_Column3.TupleSelect(1) + hv_Length22.TupleSelect(1) - hv_L.TupleSelect(1);


                            HOperatorSet.GenRectangle2(out ho_Rects3, hv_Row4, hv_Column4, hv_Phi, hv_Length11 - iScale_height_1, hv_L);
                            HOperatorSet.Union1(ho_Rects3, out ho_RegionUnion);


                            HOperatorSet.Intersection(ho_RegionUnion_std, hoRegion, out ho_RegionIntersection3);
                            HOperatorSet.Connection(ho_RegionIntersection3, out ho_ConnectedRegions1);

                            HOperatorSet.SmallestRectangle2(ho_ConnectedRegions1, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                            HOperatorSet.GenRectangle2(out ho_Rectangle, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);

                            HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDifference1);
                            HOperatorSet.Intersection(ho_RegionDifference1, ho_RegionUnion, out ho_RegionIntersection4);
                            HOperatorSet.OpeningCircle(ho_RegionIntersection4, out ho_RegionOpen222, iRegionOpe);//4                      
                            HOperatorSet.Intersection(ho_RegionRect2_dianji_zhengmian, ho_RegionOpen222, out ho_RegionIntersection);
                            //ho_RegionIntersection = ho_RegionOpen222;
                            HOperatorSet.Connection(ho_RegionIntersection, out ho_Err_RegionConn333);
                            ho_Err_RegionConn555 = ho_Err_RegionConn333;

                            HOperatorSet.SelectShape(ho_Err_RegionConn555, out ho_SelectedRegions5, "width", "and", iwidth, 99999);

                            HOperatorSet.SelectShape(ho_SelectedRegions5, out ho_SelectedRegions4, "area", "and", iArea, 99999);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_SelectedRegions4, out hv_Num);
                            HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion3);

                            HOperatorSet.AreaCenter(ho_RegionUnion3, out hv_Area2, out hv_Row5, out hv_Column5);
                            //
                            if (hv_Area2 > iArea_shangpa1)//
                            {
                                #region
                                //HOperatorSet.Intensity(ho_SelectedRegions4, ho_ImageReduced, out hv_Mean, out hv_Dev);                         
                                listObj2Draw[1] = "NG-上爬不足";
                                for (int i = 1; i <= hv_Num; i++)
                                {
                                    HOperatorSet.SelectObj(ho_SelectedRegions4, out ho_RegionSel, i);
                                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                }


                                //输出NG详情
                                lsInfo2Draw.Add("0,3,4-缺陷最大面积：" + iArea_shangpa1 + "pix ");
                                lsInfo2Draw.Add("OK");
                                lsInfo2Draw.Add("当前面积：" + hv_Area2.D.ToString("0.0") + "pix ");
                                lsInfo2Draw.Add("NG");
                                listObj2Draw.Add("字符串");
                                listObj2Draw.Add(lsInfo2Draw);
                                listObj2Draw.Add(new PointF(1800, 100));
                                return listObj2Draw;

                                #endregion
                            }
                            #endregion

                            if (true)
                            {
                                #region****上爬不足检测test-2
                                HOperatorSet.SmallestRectangle2(ho_SortedRegions, out RowDDD, out ColDDD, out PhiDDD, out Length1DDD, out Length2DDD);

                                HOperatorSet.GenRectangle2(out ho_Rects, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD);
                                HOperatorSet.GenRectangle2(out ho_Rectangle2, RowDDD, ColDDD, PhiDDD, Length1DDD - 10, Length2DDD - 10);//-40,-10

                                HOperatorSet.Intersection(ho_Rectangle2, hoRegion, out ho_RegionIntersection2);
                                HOperatorSet.SmallestRectangle2(ho_RegionIntersection2, out hv_Row111, out hv_Column111, out hv_Phi, out hv_Length1, out hv_Length2);
                                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, hv_Length1, hv_Length2);

                                HOperatorSet.Union1(ho_Rectangle, out ho_RegionUnion);
                                HOperatorSet.Difference(ho_RegionUnion, ho_RegionIntersection2, out ho_RegionDifference);
                                //HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpen222, iRegionOpe);//6
                                HOperatorSet.OpeningRectangle1(ho_RegionDifference, out ho_RegionOpen222, 12, iRegionOpe);//20/6
                                HOperatorSet.Connection(ho_RegionOpen222, out ho_Err_RegionConn333);


                                HOperatorSet.SelectShapeStd(ho_Err_RegionConn333, out ho_RegionOpen333, "max_area", 70);

                                HOperatorSet.AreaCenter(ho_RegionOpen222, out hv_Area, out hv_Row, out hv_Column);
                                if (hv_Area > iArea_shangpa3)   //
                                {
                                    #region                          
                                    listObj2Draw[1] = "NG-上爬不足";
                                    HOperatorSet.Connection(ho_RegionOpen222, out ho_Err_RegionConn);
                                    hv_Num = 0;
                                    HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                                    for (int i = 1; i <= hv_Num; i++)
                                    {
                                        HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                    }
                                    //输出NG详情
                                    lsInfo2Draw.Add("11缺陷最大面积：" + iArea_shangpa3 + "pix ");
                                    lsInfo2Draw.Add("OK");
                                    lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                                    lsInfo2Draw.Add("NG");
                                    listObj2Draw.Add("字符串");
                                    listObj2Draw.Add(lsInfo2Draw);
                                    listObj2Draw.Add(new PointF(1800, 100));
                                    return listObj2Draw;
                                    #endregion
                                }
                                //判断凹陷的长度，如果过长则NG
                                HTuple hv_Height2, hv_Width2, hv_Ratio1, hv_Max;
                                HOperatorSet.HeightWidthRatio(ho_Err_RegionConn333, out hv_Height2, out hv_Width2, out hv_Ratio1);
                                HOperatorSet.TupleMax(hv_Width2, out hv_Max);

                                if (hv_Max > iwidth_aoxian)   //上爬不足最大凹陷宽度
                                {
                                    #region                          
                                    listObj2Draw[1] = "NG-上爬不足";
                                    HOperatorSet.Connection(ho_RegionOpen222, out ho_Err_RegionConn);
                                    hv_Num = 0;
                                    HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                                    for (int i = 1; i <= hv_Num; i++)
                                    {
                                        HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                    }
                                    //输出NG详情
                                    lsInfo2Draw.Add("最大凹陷宽度：" + iwidth_aoxian + "pix ");
                                    lsInfo2Draw.Add("OK");
                                    lsInfo2Draw.Add("当前最大凹陷宽度：" + hv_Max.D.ToString("0.0") + "pix ");
                                    lsInfo2Draw.Add("NG");
                                    listObj2Draw.Add("字符串");
                                    listObj2Draw.Add(lsInfo2Draw);
                                    listObj2Draw.Add(new PointF(1800, 100));
                                    return listObj2Draw;
                                    #endregion
                                }



                                #endregion
                            }

                        }


                        if (hv_leixing == 5)
                        {
                            #region****上爬不足-HKRK
                            #region
                            HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);
                            hv_Row4 = hv_Row3.Clone();
                            hv_L = (hv_Area / (Length1DDD * 2)) / 2;

                            hv_Column4 = new HTuple();
                            hv_Column4[0] = hv_Column3.TupleSelect(0) - hv_Length22.TupleSelect(0) + hv_L.TupleSelect(0);
                            hv_Column4[1] = hv_Column3.TupleSelect(1) + hv_Length22.TupleSelect(1) - hv_L.TupleSelect(1);

                            HOperatorSet.GenRectangle2(out ho_Rects_std, hv_Row4, hv_Column4, hv_Phi, hv_Length11, hv_L);
                            HOperatorSet.Union1(ho_Rects_std, out ho_RegionUnion_std);

                            HOperatorSet.GenRectangle2(out ho_Rects3, hv_Row4, hv_Column4, hv_Phi, hv_Length11 - iScale_height_2, hv_L);
                            HOperatorSet.Union1(ho_Rects3, out ho_RegionUnion_3);

                            //上爬宽度
                            hv_Row4 = hv_Row3.Clone();
                            //hv_L = (hv_Area / (Length1DDD * 2)) / 2 - iScale_width;
                            hv_L1 = iScale_width_2;
                            hv_L = new HTuple();
                            hv_L = hv_L.TupleConcat(hv_L1);
                            hv_L = hv_L.TupleConcat(hv_L1);



                            hv_Column4 = new HTuple();
                            hv_Column4[0] = hv_Column3.TupleSelect(0) - hv_Length22.TupleSelect(0) + hv_L.TupleSelect(0);
                            hv_Column4[1] = hv_Column3.TupleSelect(1) + hv_Length22.TupleSelect(1) - hv_L.TupleSelect(1);

                            HOperatorSet.GenRectangle2(out ho_Rects4, hv_Row4, hv_Column4, hv_Phi, hv_Length11, hv_L);
                            HOperatorSet.Union1(ho_Rects4, out ho_RegionUnion_4);

                            HOperatorSet.Union2(ho_RegionUnion_3, ho_RegionUnion_4, out ho_RegionUnion);
                            #endregion

                            HOperatorSet.Intersection(ho_RegionUnion_std, hoRegion, out ho_RegionIntersection3);
                            HOperatorSet.Connection(ho_RegionIntersection3, out ho_ConnectedRegions1);

                            HOperatorSet.SmallestRectangle2(ho_ConnectedRegions1, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                            HOperatorSet.GenRectangle2(out ho_Rectangle, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);

                            HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDifference1);
                            HOperatorSet.Intersection(ho_RegionDifference1, ho_RegionUnion, out ho_RegionIntersection4);
                            HOperatorSet.OpeningCircle(ho_RegionIntersection4, out ho_RegionOpen222, iRegionOpe);//4                      
                            HOperatorSet.Intersection(ho_RegionRect2_dianji_zhengmian, ho_RegionOpen222, out ho_RegionIntersection);
                            //ho_RegionIntersection = ho_RegionOpen222;
                            HOperatorSet.Connection(ho_RegionIntersection, out ho_Err_RegionConn333);
                            ho_Err_RegionConn555 = ho_Err_RegionConn333;


                            HOperatorSet.SelectShape(ho_Err_RegionConn555, out ho_SelectedRegions4, "area", "and", iArea, 99999);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_SelectedRegions4, out hv_Num);
                            HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion3);

                            HOperatorSet.AreaCenter(ho_RegionUnion3, out hv_Area2, out hv_Row5, out hv_Column5);
                            //
                            if (hv_Area2 > iArea_shangpa1)//
                            {
                                #region
                                listObj2Draw[1] = "NG-上爬不足";
                                for (int i = 1; i <= hv_Num; i++)
                                {
                                    HOperatorSet.SelectObj(ho_SelectedRegions4, out ho_RegionSel, i);
                                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                }
                                syShowRegionBorder(ho_Rects3, ref listObj2Draw, "NG");
                                syShowRegionBorder(ho_Rects4, ref listObj2Draw, "OK");

                                //输出NG详情
                                lsInfo2Draw.Add("5-缺陷最大面积：" + iArea_shangpa1 + "pix ");
                                lsInfo2Draw.Add("OK");
                                lsInfo2Draw.Add("当前面积：" + hv_Area2.D.ToString("0.0") + "pix ");
                                lsInfo2Draw.Add("NG");
                                listObj2Draw.Add("字符串");
                                listObj2Draw.Add(lsInfo2Draw);
                                listObj2Draw.Add(new PointF(1800, 100));
                                return listObj2Draw;

                                #endregion
                            }


                            #endregion

                        }


                        if (hv_leixing == 6 || hv_leixing == 7)
                        {
                            #region****上爬不足-中兴、手机

                            HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);
                            hv_Row4 = hv_Row3.Clone();
                            hv_L = (hv_Area / (Length1DDD * 2)) / 2;

                            hv_Column4 = new HTuple();
                            hv_Column4[0] = hv_Column3.TupleSelect(0) - hv_Length22.TupleSelect(0) + hv_L.TupleSelect(0);
                            hv_Column4[1] = hv_Column3.TupleSelect(1) + hv_Length22.TupleSelect(1) - hv_L.TupleSelect(1);

                            HOperatorSet.GenRectangle2(out ho_Rects_std, hv_Row4, hv_Column4, hv_Phi, hv_Length11, hv_L);
                            HOperatorSet.Union1(ho_Rects_std, out ho_RegionUnion_std);

                            hv_Row4 = hv_Row3.Clone();
                            hv_L1 = iScale_width_3;
                            hv_L = new HTuple();
                            hv_L = hv_L.TupleConcat(hv_L1);
                            hv_L = hv_L.TupleConcat(hv_L1);
                            hv_Column4 = new HTuple();
                            hv_Column4[0] = hv_Column3.TupleSelect(0) - hv_Length22.TupleSelect(0) + hv_L.TupleSelect(0);
                            hv_Column4[1] = hv_Column3.TupleSelect(1) + hv_Length22.TupleSelect(1) - hv_L.TupleSelect(1);


                            HOperatorSet.GenRectangle2(out ho_Rects3, hv_Row4, hv_Column4, hv_Phi, hv_Length11 - iScale_height_3, hv_L);
                            HOperatorSet.Union1(ho_Rects3, out ho_RegionUnion);


                            HOperatorSet.Intersection(ho_RegionUnion_std, hoRegion, out ho_RegionIntersection3);
                            HOperatorSet.Connection(ho_RegionIntersection3, out ho_ConnectedRegions1);

                            HOperatorSet.SmallestRectangle2(ho_ConnectedRegions1, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                            HOperatorSet.GenRectangle2(out ho_Rectangle, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);

                            HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDifference1);
                            HOperatorSet.Intersection(ho_RegionDifference1, ho_RegionUnion, out ho_RegionIntersection4);
                            HOperatorSet.OpeningCircle(ho_RegionIntersection4, out ho_RegionOpen222, iRegionOpe);//4                      
                            HOperatorSet.Intersection(ho_RegionRect2_dianji_zhengmian, ho_RegionOpen222, out ho_RegionIntersection);
                            //ho_RegionIntersection = ho_RegionOpen222;
                            HOperatorSet.Connection(ho_RegionIntersection, out ho_Err_RegionConn333);
                            ho_Err_RegionConn555 = ho_Err_RegionConn333;


                            HOperatorSet.SelectShape(ho_Err_RegionConn555, out ho_SelectedRegions4, "area", "and", iArea, 99999);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_SelectedRegions4, out hv_Num);
                            HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion3);

                            HOperatorSet.AreaCenter(ho_RegionUnion3, out hv_Area2, out hv_Row5, out hv_Column5);
                            //
                            syShowRegionBorder(ho_Rects3, ref listObj2Draw, "NG");
                            if (hv_Area2 > iArea_shangpa1)//
                            {
                                #region
                                //HOperatorSet.Intensity(ho_SelectedRegions4, ho_ImageReduced, out hv_Mean, out hv_Dev);                         
                                listObj2Draw[1] = "NG-上爬不足";
                                for (int i = 1; i <= hv_Num; i++)
                                {
                                    HOperatorSet.SelectObj(ho_SelectedRegions4, out ho_RegionSel, i);
                                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                }

                                syShowRegionBorder(ho_Rects3, ref listObj2Draw, "NG");


                                //输出NG详情
                                lsInfo2Draw.Add("6,7-缺陷最大面积：" + iArea_shangpa1 + "pix ");
                                lsInfo2Draw.Add("OK");
                                lsInfo2Draw.Add("当前面积：" + hv_Area2.D.ToString("0.0") + "pix ");
                                lsInfo2Draw.Add("NG");
                                listObj2Draw.Add("字符串");
                                listObj2Draw.Add(lsInfo2Draw);
                                listObj2Draw.Add(new PointF(1800, 100));
                                return listObj2Draw;

                                #endregion
                            }
                            #endregion
                        }


                        #region****rect-电极外角缺陷-上爬高度
                        //HObject ho_RegionIntersection3, ho_ConnectedRegions1, ho_RegionIntersection4;
                        //HTuple hv_Row6, hv_Column6, hv_Row7, hv_Column7;
                        //HOperatorSet.Intersection(ho_RegionUnion_std, hoRegion, out ho_RegionIntersection3);
                        //HOperatorSet.Connection(ho_RegionIntersection3, out ho_ConnectedRegions1);

                        ////HOperatorSet.SmallestRectangle1(ho_ConnectedRegions1, out hv_Row6, out hv_Column6, out hv_Row7, out hv_Column7);
                        ////HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row6, hv_Column6, hv_Row7, hv_Column7);

                        //HOperatorSet.SmallestRectangle2(ho_ConnectedRegions1, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                        //HOperatorSet.GenRectangle2(out ho_Rectangle, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);

                        //HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDifference1);
                        //HOperatorSet.Intersection(ho_RegionDifference1, ho_RegionUnion, out ho_RegionIntersection4);
                        //HOperatorSet.OpeningCircle(ho_RegionIntersection4, out ho_RegionOpen222, iRegionOpe);//4                      
                        //HOperatorSet.Intersection(ho_RegionRect2_dianji_zhengmian, ho_RegionOpen222, out ho_RegionIntersection);
                        ////ho_RegionIntersection = ho_RegionOpen222;
                        //HOperatorSet.Connection(ho_RegionIntersection, out ho_Err_RegionConn333);
                        //ho_Err_RegionConn555 = ho_Err_RegionConn333;


                        //HOperatorSet.SelectShape(ho_Err_RegionConn555, out ho_SelectedRegions4, "area", "and", iArea, 99999);
                        //hv_Num = 0;
                        //HOperatorSet.CountObj(ho_SelectedRegions4, out hv_Num);
                        //HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion3);

                        //HOperatorSet.AreaCenter(ho_RegionUnion3, out hv_Area2, out hv_Row5, out hv_Column5);
                        ////
                        //if (hv_Area2 > iArea2)//
                        //{
                        //    #region
                        //    //HOperatorSet.Intensity(ho_SelectedRegions4, ho_ImageReduced, out hv_Mean, out hv_Dev);                         
                        //    listObj2Draw[1] = "NG-上爬不足";
                        //    for (int i = 1; i <= hv_Num; i++)
                        //    {
                        //        HOperatorSet.SelectObj(ho_SelectedRegions4, out ho_RegionSel, i);
                        //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        //    }

                        //    syShowRegionBorder(ho_Rects4, ref listObj2Draw, "NG");
                        //    syShowRegionBorder(ho_Rects3, ref listObj2Draw, "OK");

                        //    //输出NG详情
                        //    lsInfo2Draw.Add("rect-上爬高度-缺陷最大面积：" + iArea2 + "pix ");
                        //    lsInfo2Draw.Add("OK");
                        //    lsInfo2Draw.Add("当前面积：" + hv_Area2.D.ToString("0.0") + "pix ");
                        //    lsInfo2Draw.Add("NG");
                        //    listObj2Draw.Add("字符串");
                        //    listObj2Draw.Add(lsInfo2Draw);
                        //    listObj2Draw.Add(new PointF(1800, 100));
                        //    return listObj2Draw;

                        //    #endregion
                        //}
                        #endregion
                        #region****rect-电极外角缺陷-上爬宽度

                        //HOperatorSet.Intersection(ho_RegionUnion_4, hoRegion, out ho_RegionIntersection3);
                        //HOperatorSet.Connection(ho_RegionIntersection3, out ho_ConnectedRegions1);

                        ////HOperatorSet.SmallestRectangle1(ho_ConnectedRegions1, out hv_Row6, out hv_Column6, out hv_Row7, out hv_Column7);
                        ////HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row6, hv_Column6, hv_Row7, hv_Column7);

                        //HOperatorSet.SmallestRectangle2(ho_ConnectedRegions1, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                        //HOperatorSet.GenRectangle2(out ho_Rectangle, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);



                        //HOperatorSet.Difference(ho_Rectangle, hoRegion, out ho_RegionDifference1);
                        ////0924
                        ////HOperatorSet.Intersection(ho_RegionDifference1, ho_Rects3, out ho_RegionIntersection4);

                        ////HOperatorSet.Difference(ho_RegionUnion, hoRegion, out ho_RegionDifference);

                        ////ho_RegionTrans
                        ////HOperatorSet.Intersection(ho_RegionTrans, ho_RegionDifference, out ho_RegionIntersection);


                        ////HOperatorSet.OpeningRectangle1(ho_RegionDifference, out ho_RegionOpen222, 10, iRegionOpe);//12/
                        //HOperatorSet.OpeningCircle(ho_RegionDifference1, out ho_RegionOpen222, iRegionOpe);//4


                        ////HOperatorSet.Difference(ho_RegionUnion, hoRegion, out ho_RegionDifference);
                        ////HOperatorSet.OpeningCircle(ho_RegionDifference, out ho_RegionOpen222, iRegionOpe);//6
                        ////HOperatorSet.Intersection(ho_Rectangle5, ho_RegionOpen222, out ho_RegionIntersection);
                        //HOperatorSet.Intersection(ho_RegionRect2_dianji_zhengmian, ho_RegionOpen222, out ho_RegionIntersection);
                        ////ho_RegionIntersection = ho_RegionOpen222;
                        //HOperatorSet.Connection(ho_RegionIntersection, out ho_Err_RegionConn333);
                        //ho_Err_RegionConn555 = ho_Err_RegionConn333;
                        ////HOperatorSet.Intersection(ho_Err_RegionConn333, ho_Rectangle2, out ho_RegionIntersection2);
                        ////HOperatorSet.Union1(ho_RegionIntersection2, out ho_RegionUnion3);
                        ////HOperatorSet.Difference(ho_RegionIntersection, ho_RegionUnion3, out ho_Err_RegionConn444);
                        ////HOperatorSet.Connection(ho_Err_RegionConn444, out ho_Err_RegionConn555);

                        //HOperatorSet.SelectShape(ho_Err_RegionConn555, out ho_SelectedRegions4, "area", "and", iArea, 99999);
                        //hv_Num = 0;
                        //HOperatorSet.CountObj(ho_SelectedRegions4, out hv_Num);
                        //HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion3);

                        //HOperatorSet.AreaCenter(ho_RegionUnion3, out hv_Area2, out hv_Row5, out hv_Column5);
                        ////
                        //if (hv_Area2 > iArea2)//
                        //{
                        //    #region
                        //    //HOperatorSet.Intensity(ho_SelectedRegions4, ho_ImageReduced, out hv_Mean, out hv_Dev);                         
                        //    listObj2Draw[1] = "NG-上爬不足";
                        //    for (int i = 1; i <= hv_Num; i++)
                        //    {
                        //        HOperatorSet.SelectObj(ho_SelectedRegions4, out ho_RegionSel, i);
                        //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        //    }

                        //    syShowRegionBorder(ho_Rects4, ref listObj2Draw, "NG");
                        //    //输出NG详情
                        //    lsInfo2Draw.Add("rect-上爬宽度-缺陷最大面积：" + iArea2 + "pix ");
                        //    lsInfo2Draw.Add("OK");
                        //    lsInfo2Draw.Add("当前面积：" + hv_Area2.D.ToString("0.0") + "pix ");
                        //    lsInfo2Draw.Add("NG");
                        //    listObj2Draw.Add("字符串");
                        //    listObj2Draw.Add(lsInfo2Draw);
                        //    listObj2Draw.Add(new PointF(1800, 100));
                        //    return listObj2Draw;

                        //    #endregion
                        //}
                        #endregion
                        #region****convex-电极内部缺陷
                        //HObject ho_RegionIntersection9, ho_ConnectedRegions9, ho_RegionTrans9, ho_RegionUnion9,
                        //    ho_RegionDifference9, ho_RegionOpening9, ho_RegionUnion10, ho_SelectedRegions9, ho_ConnectedRegions10;
                        //HTuple hv_Area9, hv_Row9, hv_Column9;
                        //HOperatorSet.Intersection(ho_RegionUnion, hoRegion, out ho_RegionIntersection9);
                        //HOperatorSet.Connection(ho_RegionIntersection9, out ho_ConnectedRegions9);
                        //HOperatorSet.ShapeTrans(ho_ConnectedRegions9, out ho_RegionTrans9, "convex");
                        //HOperatorSet.Union1(ho_RegionTrans9, out ho_RegionUnion9);
                        //HOperatorSet.Difference(ho_RegionUnion9, ho_RegionClosing1, out ho_RegionDifference9);
                        //HOperatorSet.OpeningCircle(ho_RegionDifference9, out ho_RegionOpening9, 2);//2
                        //HOperatorSet.Connection(ho_RegionOpening9, out ho_ConnectedRegions10);

                        //HOperatorSet.SelectShape(ho_ConnectedRegions10, out ho_SelectedRegions9, "area", "and", iArea, 99999);
                        //hv_Num = 0;
                        //HOperatorSet.CountObj(ho_SelectedRegions9, out hv_Num);

                        //HOperatorSet.Union1(ho_SelectedRegions9, out ho_RegionUnion10);
                        //HOperatorSet.AreaCenter(ho_RegionUnion10, out hv_Area9, out hv_Row9, out hv_Column9);

                        ////0924
                        ////if (hv_Area9 > iArea2)//
                        //if (false)//
                        //{
                        //    #region
                        //    //HOperatorSet.Intensity(ho_SelectedRegions4, ho_ImageReduced, out hv_Mean, out hv_Dev);                         
                        //    listObj2Draw[1] = "NG-上爬不足";
                        //    for (int i = 1; i <= hv_Num; i++)
                        //    {
                        //        HOperatorSet.SelectObj(ho_SelectedRegions9, out ho_RegionSel, i);
                        //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");

                        //    }

                        //    //输出NG详情
                        //    lsInfo2Draw.Add("convex-缺陷最大面积：" + iArea2 + "pix ");
                        //    lsInfo2Draw.Add("OK");
                        //    lsInfo2Draw.Add("当前面积：" + hv_Area9.D.ToString("0.0") + "pix ");
                        //    lsInfo2Draw.Add("NG");
                        //    listObj2Draw.Add("字符串");
                        //    listObj2Draw.Add(lsInfo2Draw);
                        //    listObj2Draw.Add(new PointF(1800, 100));
                        //    return listObj2Draw;

                        //    #endregion
                        //}

                        #endregion
                        #endregion
                    }



                    #endregion

                    #region***IIG崩碎检测
                    HObject ho_DarkPixels2, ho_RegionFillUp;
                    HTuple hv_Areauuuuuu, hv_Rowuuuuuuu, hv_Coluuuuuuu;
                    HTuple hv_Row55, hv_Column55, hv_Phi7, hv_Length66, hv_Length67;
                    HObject ho_Rectangle6;

                    #region***IIG崩碎检测区域提取

                    //检查保护层破洞
                    //HOperatorSet.Difference(ho_RegionTrans, hoRegion, out ho_RegionDiff);
                    HOperatorSet.SmallestRectangle2(hoSelectedRegions_dianji_zhengmian, out hv_Row55, out hv_Column55, out hv_Phi7, out hv_Length66, out hv_Length67);
                    HOperatorSet.GenRectangle2(out ho_Rectangle6, hv_Row55, hv_Column55, hv_Phi7, hv_Length66, hv_Length67);
                    HObject ho_Rectangle6_zhanxi = ho_Rectangle6;

                    if (hv_leixing == 0 || hv_leixing == 3 || hv_leixing == 4)
                    {
                        #region ****车规品检测                            
                        HOperatorSet.Difference(ho_RegionRect2_dianji_zhengmian, hoSelectedRegionsIIG, out ho_RegionDiff);
                        //dhDll.frmMsg.Log("背导ok" + ichegui.ToString() + "," + ichegui.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        #endregion
                    }
                    else
                    {
                        #region ****常规品检测                            
                        HOperatorSet.Difference(ho_RegionRect2_dianji_zhengmian, ho_Rectangle6_zhanxi, out ho_RegionDiff);
                        #endregion
                    }

                    //HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 12);    //腐蚀半径12
                    //IIG崩碎检测，腐蚀较多长边，减少与电极的粘连，腐蚀较少的短边，方便检测靠近边缘的IIG崩碎
                    HOperatorSet.ErosionRectangle1(ho_RegionDiff, out hoRegion, EroWidth1, EroHeight1);//22,8
                    HOperatorSet.AreaCenter(hoRegion, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce);
                    HObject ho_ImageReduce_baohuceng1 = ho_ImageReduce;
                    HOperatorSet.Intensity(hoRegion, ho_ImageReduce_baohuceng1, out hv_Mean, out hv_Dev);
                    if (hv_Mean > 30) //保护层平均灰度不应大于30 //
                    {
                        #region
                        listObj2Draw[1] = "NG-IIG崩碎";//
                        HOperatorSet.Connection(hoRegion, out ho_Err_RegionConn);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        return listObj2Draw;
                        #endregion
                    }

                    #endregion

                    HObject ho_RegionErosion2, ho_RegionDifference4, ho_RegionOpening4, hoRegionIIG;
                    HOperatorSet.Union1(hoSelectedRegionsIIG, out hoRegionIIG);
                    //0814新-IIG崩碎检测-exp_image 
                    //iProtectexp-exp保护层崩碎提取阈值
                    //iProtectBrokenArea-exp保护层崩碎面积阈值
                    //
                    if (iIIG1 > 0)
                    {
                        #region***IIG崩碎检测-exp_image 
                        //****对电阻区域进行exp处理                                
                        HOperatorSet.ExpImage(ho_ImageReduce_baohuceng1, out ho_ExpImage, iProtectexp);//25
                        HOperatorSet.ScaleImageMax(ho_ExpImage, out ho_ImageScaleMax);
                        HOperatorSet.Threshold(ho_ImageScaleMax, out ho_DarkPixels2, 200, 255);
                        HOperatorSet.ErosionCircle(ho_DarkPixels2, out ho_RegionErosion3, 1.5);
                        HOperatorSet.Connection(ho_RegionErosion3, out ho_RegionLies);

                        //0621增加限定缺陷长款博
                        HOperatorSet.SelectShape(ho_RegionLies, out ho_RegionOpen222, "anisometry", "and", 0, 15);
                        HOperatorSet.SelectShape(ho_RegionOpen222, out ho_RegionOpen111, "area", "and", iProtectBrokenArea, 99999);
                        HOperatorSet.Union1(ho_RegionOpen111, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_RegionOpen111, out hv_Num);

                        if (hv_Num > 0)
                        {   //IIG崩碎检测
                            #region ---- *** IIG崩碎检测-exp_image 
                            listObj2Draw[1] = "NG-IIG崩碎";
                            //HOperatorSet.Connection(ho_RegionLight, out ho_Err_RegionConn);
                            //hv_Num = 0;
                            //HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_RegionOpen111, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("exp新IIG崩碎-缺陷面积下限：" + iProtectBrokenArea + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                        }

                        #endregion
                    }



                    //0814旧
                    if (iIIG3 > 0)
                    {
                        #region VarThreshold-IIG崩碎检测

                        HOperatorSet.VarThreshold(ho_ImageReduce_baohuceng1, out ho_DarkPixels2, 50, 50, 0.2, iProtectBrokenResThres, "light");
                        HOperatorSet.Connection(ho_DarkPixels2, out ho_RegionLies);

                        //HOperatorSet.SelectShape(ho_RegionOpen111, out ho_SelectedRegions1, "area", "and", iArea, 99999);
                        //HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion3);
                        HOperatorSet.SelectShape(ho_RegionLies, out ho_RegionOpen222, "anisometry", "and", 0, 15);
                        HOperatorSet.SelectShapeStd(ho_RegionOpen222, out ho_SelectedRegions1, "max_area", 70);
                        HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                        HOperatorSet.AreaCenter(ho_RegionFillUp, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                        HOperatorSet.Intensity(ho_RegionFillUp, ho_ImageReduce, out hv_Mean, out hv_Dev);

                        //*******

                        ho_RegionLight = ho_RegionFillUp;

                        if (true)
                        {
                            #region ---- ***VarThreshold-IIG崩碎检测
                            HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                            if (hv_Area > iProtectBrokenArea)
                            {   //相对亮面积大于300
                                listObj2Draw[1] = "NG-IIG崩碎";
                                HOperatorSet.Connection(ho_RegionLight, out ho_Err_RegionConn);
                                hv_Num = 0;
                                HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                                for (int i = 1; i <= hv_Num; i++)
                                {
                                    HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                }
                                //输出NG详情
                                lsInfo2Draw.Add("旧IIG-缺陷面积下限：" + iProtectBrokenArea + "pix ");
                                lsInfo2Draw.Add("OK");
                                lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                                lsInfo2Draw.Add("NG");
                                listObj2Draw.Add("字符串");
                                listObj2Draw.Add(lsInfo2Draw);
                                listObj2Draw.Add(new PointF(1800, 100));
                                return listObj2Draw;
                            }
                            #endregion
                        }

                        if (hv_Mean > 80)
                        {
                            #region ---- ***保护层挂锡检测
                            HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                            if (hv_Area > (iProtectBrokenArea2))
                            {   //相对亮面积大于300
                                listObj2Draw[1] = "NG-IIG崩碎";
                                HOperatorSet.Connection(ho_RegionLight, out ho_Err_RegionConn);
                                hv_Num = 0;
                                HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                                for (int i = 1; i <= hv_Num; i++)
                                {
                                    HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                                }
                                //输出NG详情
                                lsInfo2Draw.Add("旧保护挂锡-缺陷面积下限：" + iProtectBrokenArea2 + "pix ");
                                lsInfo2Draw.Add("OK");
                                lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                                lsInfo2Draw.Add("NG");
                                listObj2Draw.Add("字符串");
                                listObj2Draw.Add(lsInfo2Draw);
                                listObj2Draw.Add(new PointF(1800, 100));
                                return listObj2Draw;
                            }
                            #endregion

                        }
                        #endregion
                    }



                    //IIG崩碎检测0808-不明显的IIG崩碎检测
                    //mean+dyn
                    //iProtectArea4-不明显IIG崩碎面积阈值
                    //iProtectrThr4-不明显IIG崩碎提取阈值
                    if (iIIG4 > 0)
                    {
                        #region***不明显的IIG崩碎检测

                        HOperatorSet.MeanImage(ho_ImageReduce_baohuceng1, out ho_ImageMean, 50, 50);//15,15
                        HOperatorSet.DynThreshold(ho_ImageReduce_baohuceng1, ho_ImageMean, out ho_DarkPixels2, iProtectrThr4, "light");  //iBlackThres
                                                                                                                                         //HOperatorSet.VarThreshold(ho_ImageReduce, out ho_DarkPixels2, 50, 50, 0.2, iProtectBrokenResThres, "light");
                        HOperatorSet.Connection(ho_DarkPixels2, out ho_RegionLies);

                        //0621增加限定缺陷长款博
                        HOperatorSet.SelectShape(ho_RegionLies, out ho_RegionOpen111, "anisometry", "and", 0, 15);
                        HOperatorSet.SelectShape(ho_RegionOpen111, out ho_RegionOpen333, "width", "and", 1, 100);
                        //0911
                        HOperatorSet.SelectShapeStd(ho_RegionOpen333, out ho_SelectedRegions1, "max_area", 70);
                        //HOperatorSet.SelectShapeStd(ho_RegionOpen222, out ho_SelectedRegions1, "max_area", 70);

                        HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                        HOperatorSet.AreaCenter(ho_RegionFillUp, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                        HOperatorSet.Intensity(ho_RegionFillUp, ho_ImageReduce, out hv_Mean, out hv_Dev);

                        if (hv_Areauuuuuu > iProtectArea4)
                        {   //相对亮面积大于300
                            #region
                            listObj2Draw[1] = "NG-IIG崩碎";
                            syShowRegionBorder(ho_RegionFillUp, ref listObj2Draw, "NG");

                            //输出NG详情
                            lsInfo2Draw.Add("不明显IIG崩碎-缺陷面积下限：" + iProtectArea4 + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Areauuuuuu.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                        }
                        #endregion
                    }

                    //0814添加-IIG大范围磨损检测 
                    //iProtectArea5-保护层大范围磨损面积
                    if (iIIG5 > 0)
                    {
                        #region***IIG大范围磨损检测            

                        HOperatorSet.MeanImage(ho_ImageReduce_baohuceng1, out ho_ImageMean, 50, 50);//15,15
                        HOperatorSet.DynThreshold(ho_ImageReduce_baohuceng1, ho_ImageMean, out ho_DarkPixels2, 5, "light");
                        HOperatorSet.AreaCenter(ho_DarkPixels2, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                        if (hv_Areauuuuuu > iProtectArea5)
                        {   //相对亮面积大于hv_Area5

                            #region ---- *** 普通0402  *** ----
                            HOperatorSet.AreaCenter(ho_DarkPixels2, out hv_Area, out hv_Row, out hv_Column);
                            listObj2Draw[1] = "NG-IIG崩碎";
                            HOperatorSet.Connection(ho_DarkPixels2, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("大范围磨损-缺陷面积下限：" + iProtectArea5 + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Areauuuuuu.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                        }
                        #endregion
                    }


                    //保护层挂锡检测
                    //iProtectZhanxiVarThr-保护层挂锡提取阈值
                    //iProtectZhanxiArea-保护层挂锡面积阈值
                    //iProtectZhanxiMean-保护层挂锡区域平均灰度值
                    #region ****保护层挂锡检测
                    #region***保护层挂锡检测区域提取 
                    HObject ho_ImageReduce_baohucengguaxi;
                    if (hv_leixing == 0 || hv_leixing == 3 || hv_leixing == 4)
                    {
                        #region ****车规品检测                            
                        HOperatorSet.Difference(ho_RegionRect2_dianji_zhengmian, hoSelectedRegionsIIG, out ho_RegionDiff);
                        #endregion
                    }
                    else
                    {
                        #region ****常规品检测                            
                        HOperatorSet.Difference(ho_RegionRect2_dianji_zhengmian, ho_Rectangle6_zhanxi, out ho_RegionDiff);
                        #endregion
                    }
                    HOperatorSet.ErosionRectangle1(ho_RegionDiff, out hoRegion, 20, 20);//20，20；EroWidth2, EroHeight2
                    HOperatorSet.AreaCenter(hoRegion, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                    HOperatorSet.ReduceDomain(ho_ImageReduced, hoRegion, out ho_ImageReduce_baohucengguaxi);
                    #endregion

                    #region***VarThreshold检测保护层挂锡

                    HOperatorSet.VarThreshold(ho_ImageReduce_baohucengguaxi, out ho_DarkPixels2, 50, 50, 0.2, iProtectBrokenResThres, "light");//iProtectZhanxiVarThr
                    HOperatorSet.Connection(ho_DarkPixels2, out ho_RegionLies);
                    HOperatorSet.SelectShape(ho_RegionLies, out ho_RegionOpen222, "anisometry", "and", 0, 15);

                    HOperatorSet.SelectShapeStd(ho_RegionOpen222, out ho_SelectedRegions1, "max_area", 70);
                    HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                    HOperatorSet.AreaCenter(ho_RegionFillUp, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                    HOperatorSet.Intensity(ho_RegionFillUp, ho_ImageReduce, out hv_Mean, out hv_Dev);

                    ho_RegionLight = ho_RegionFillUp;
                    if (hv_Mean > 60)
                    {
                        #region ---- ***保护层挂锡检测
                        HOperatorSet.AreaCenter(ho_RegionLight, out hv_Area, out hv_Row, out hv_Column);
                        if (hv_Area > (iProtectBrokenArea3))
                        {   //相对亮面积大于300
                            listObj2Draw[1] = "NG-IIG崩碎";
                            HOperatorSet.Connection(ho_RegionLight, out ho_Err_RegionConn);
                            hv_Num = 0;
                            HOperatorSet.CountObj(ho_Err_RegionConn, out hv_Num);
                            for (int i = 1; i <= hv_Num; i++)
                            {
                                HOperatorSet.SelectObj(ho_Err_RegionConn, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("中间区域 保护挂锡-缺陷面积下限：" + iProtectBrokenArea3 + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                        }
                        #endregion

                    }
                    #endregion
                    #endregion




                    if (expYN > 0)
                    //if (false)
                    {
                        #region***保护层边界区域提取
                        int iarea_exp = 100;  //保护层边界区域提取缺陷面积

                        HOperatorSet.Difference(ho_RegionTrans_dianji_zhengmian, hoRegionIIG, out ho_RegionDiff);
                        HOperatorSet.OpeningRectangle1(ho_RegionDiff, out ho_RegionOpening, 10, 30);//10,30
                        HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
                        HOperatorSet.ErosionRectangle1(ho_RegionFillUp, out hoRegion, EroWidth4, 3);//12 25,//0618修改，25，12//0816:20,8
                                                                                                    //HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, EroWidth4);    //腐蚀半径9
                                                                                                    //HOperatorSet.ErosionCircle(ho_RegionFillUp, out hoRegion, EroWidth4);    //腐蚀半径9

                        HOperatorSet.ErosionRectangle1(hoRegion, out ho_RegionErosion2, EroWidth3, EroHeight3);//1、50
                        HOperatorSet.Difference(hoRegion, ho_RegionErosion2, out ho_RegionDifference4);
                        HOperatorSet.OpeningCircle(ho_RegionDifference4, out ho_RegionOpening4, EroHeight4);//9
                                                                                                            //ho_RegionDifference4 = ho_RegionOpening4;
                                                                                                            //HOperatorSet.ErosionCircle(ho_RegionDiff, out hoRegion, 12);    //腐蚀半径12
                                                                                                            //IIG崩碎检测，腐蚀较多长边，减少与电极的粘连，腐蚀较少的短边，方便检测靠近边缘的IIG崩碎
                        HOperatorSet.AreaCenter(ho_RegionOpening4, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                        //HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionOpening4, out ho_ImageReduce);


                        #region***保护层边界区域IIG崩碎检测   

                        //****0820-exp   
                        //****对整张图片进行exp处理  
                        HTuple IIGClosingCircle2;
                        IIGClosingCircle2 = 15;
                        HOperatorSet.ExpImage(ho_ImageReduced, out ho_ExpImage, 45); //IIGexp2
                        HOperatorSet.ScaleImageMax(ho_ExpImage, out ho_ImageScaleMax);
                        HOperatorSet.Threshold(ho_ImageScaleMax, out ho_Region, 128, 255);
                        HOperatorSet.Intersection(ho_Region, ho_RegionOpening4, out ho_RegionIntersection);


                        HOperatorSet.ClosingCircle(ho_RegionIntersection, out ho_RegionClosing1, IIGClosingCircle2);//15
                        HOperatorSet.Connection(ho_RegionClosing1, out ho_ConnectedRegions);
                        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions8, ((new HTuple("area")).TupleConcat(
                            "anisometry")).TupleConcat("height"), "and", ((new HTuple(iarea_exp)).TupleConcat(
                            1)).TupleConcat(8), ((new HTuple(9999999)).TupleConcat(13)).TupleConcat(
                            999));
                        HOperatorSet.CountObj(ho_SelectedRegions8, out hv_Number);
                        if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                        {

                            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions8, (new HTuple("area")).TupleConcat(
                                "anisometry"), "and", (new HTuple(400)).TupleConcat(1), (new HTuple(9999999)).TupleConcat(
                                15));
                        }
                        HOperatorSet.CountObj(ho_SelectedRegions8, out hv_Number);
                        HOperatorSet.Union1(ho_SelectedRegions8, out ho_RegionUnion3);

                        HOperatorSet.AreaCenter(ho_RegionUnion3, out hv_Areauuuuuu, out hv_Rowuuuuuuu, out hv_Coluuuuuuu);
                        if (hv_Number > 0)
                        {
                            #region
                            listObj2Draw[1] = "NG-IIG崩碎";
                            for (int i = 1; i <= hv_Number; i++)
                            {
                                HOperatorSet.SelectObj(ho_SelectedRegions8, out ho_RegionSel, i);
                                syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                            }
                            //输出NG详情
                            lsInfo2Draw.Add("exp-保护层边界-缺陷面积下限：" + iarea_exp + "pix ");
                            lsInfo2Draw.Add("OK");
                            lsInfo2Draw.Add("当前面积：" + hv_Areauuuuuu.D.ToString("0.0") + "pix ");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                        }

                        #endregion

                        #endregion

                    }

                    #endregion

                    #endregion
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




                //执行到这里，OK 绘制 hoSelectedRegions
                listObj2Draw[1] = "OK";
                hv_Num = 0;
                HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                for (int i = 1; i <= hv_Num; i++)
                {
                    HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
                }

                //5、6号相机保存图片
                #region****保存ok图片
                if (iSaveImg > 0)
                {
                    //Number111 = Number111 + 1;
                    if (iProductCode == 5) //五号相机
                    {
                        if (hv_Num22 == 2) //正面
                        {
                            Number555 = Number555 + 1;
                            //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                            hv_num4 = "D:/六面外观机/杭州思元六面分选机视觉系统-0903/Images2/5/正面/" + DateTime.Now.Ticks.ToString() + "-" + Number555.ToString() + ".jpg";
                            HOperatorSet.WriteImage(hoImage, "jpg", 0, hv_num4);
                        }
                        else  //背面
                        {
                            Number555 = Number555 + 1;
                            //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                            hv_num4 = "D:/六面外观机/杭州思元六面分选机视觉系统-0903/Images2/5/背面/" + DateTime.Now.Ticks.ToString() + "-" + Number666.ToString() + ".jpg";
                            HOperatorSet.WriteImage(hoImage, "jpg", 0, hv_num4);

                        }
                    }



                    if (iProductCode == 6) //六号相机

                    {
                        if (hv_Num22 == 1)  //背面
                        {
                            Number666 = Number666 + 1;
                            //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                            hv_num4 = "D:/六面外观机/杭州思元六面分选机视觉系统-0903/Images2/6/背面/" + DateTime.Now.Ticks.ToString() + "-" + Number555.ToString() + ".jpg";
                            HOperatorSet.WriteImage(hoImage, "jpg", 0, hv_num4);
                        }
                        else  //正面
                        {
                            Number666 = Number666 + 1;
                            //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                            hv_num4 = "D:/六面外观机/杭州思元六面分选机视觉系统-0903/Images2/6/正面/" + DateTime.Now.Ticks.ToString() + "-" + Number666.ToString() + ".jpg";
                            HOperatorSet.WriteImage(hoImage, "jpg", 0, hv_num4);
                        }
                    }


                }
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

      


        //字码检测，引用文件 8
        public static List<object> sySixSideDetect_0402R_Camera56_OCR(HObject hoImage, HObject ho_ModelRegion, HTuple hv_ModelID, HTuple hv_ModelParam, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        #endregion


        #region  0603R
        //六面机0603电阻 12 相机算法 引用文件13
        public static List<object> sySixSideDetect_0603R_Camera12(HObject hoImage, List<PointF[]> lkkPolygon, string strParams, ref string strMessage)
        {

            #region  *** 12相机 六面机前后面 ***
            if (bUseMutex) muDetect12.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("NG"); listObj2Draw.Add(888);
            try
            {
                #region 变量声明
                HObject hoRegion, hoReduced, hoConcate, hoUnion;
                HObject ho_RegionSel;
                HTuple hv_Rectangularity;
                HTuple hv_Phi, hv_Length1, hv_Length2;
                HTuple hv_Row, hv_Column, Areakkk, Rowkkk, Colkkk, hv_Num;
                HTuple hv_Row2, hv_Column2, hv_Area1;
                HTuple hv_NGCode, hv_parameter;
                hv_NGCode = 0;
                HObject ho_Region_Duanmian;
                HObject ho_Region_Err2;

                HObject ho_Region_loudu;
                //缺陷显示
                HObject ho_Region_Display, ho_Union_Dispay, ho_Region_Sel;
                HTuple hv_Num_Display, hv_Area_Display, hv_Row_Display, hv_Column_Display;
               
                //ceshi01-主要检测部分
                HTuple NChannel;
                HObject ho_RectPu;
                // ***新声明
                HObject ho_GrayImage;              

                #endregion

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

                #region 调试模式初始化
                bool is_Debug;
                if (strMessage == "0")
                {
                    is_Debug = false;
                }
                else
                {
                    is_Debug = true;
                }


                //初始化调试输出内容
                string strDebug = "";
                #endregion





                #region  //12相机参数(接收界面传递的参数)
                string[] strUserParam = strParams.Split('#');


                int iLength1 = int.Parse(strUserParam[4]);      //iLength1  = 110    半长
                int iLength1Scale = int.Parse(strUserParam[5]); //iLength1Scale = 10 半长变化值
                int iLength2 = int.Parse(strUserParam[6]);      //iLength2  = 60     半宽
                int iLength2Scale = int.Parse(strUserParam[7]); //iLength2Scale = 10 半宽变化值

                int hv_iThr1 = int.Parse(strUserParam[8]);     //iErrThres  = 40    缺陷阈值40
                int hv_iThr2 = int.Parse(strUserParam[9]);      //iErrArea = 50      缺陷面积50
                int hv_iMaskWith = int.Parse(strUserParam[10]);   //轮廓矩形度 0.85
                int hv_iDyn_Thr = int.Parse(strUserParam[11]);    //iEroradis=8 矩形腐蚀半径 8

                int hv_iScaleMult = int.Parse(strUserParam[12]);     //iErrAll  = 160    全局阈值160
                int hv_iScaleAdd = int.Parse(strUserParam[13]);     //GrayClosingRect1  = 11   
                int hv_iErrAll = int.Parse(strUserParam[14]);           //200
                int hv_iOpening_Circle = int.Parse(strUserParam[15]);   //ClosingCircle1=7

                int hv_Area_Duanmian = int.Parse(strUserParam[16]);     //
                int hv_iAngleScale = int.Parse(strUserParam[17]);      //40000
                float hv_iRecty = float.Parse(strUserParam[18]) / 100;      //iErrArea = 50      缺陷面积50

                int hv_iErrArea2 = int.Parse(strUserParam[20]);      //矩形腐蚀半径5
                int iSobelSize = int.Parse(strUserParam[21]);      //缺陷腐蚀半径4     缺陷面积50
                int iThr1 = int.Parse(strUserParam[22]);
                float iClosCir = float.Parse(strUserParam[23]);      //长宽比10

                float iEroCir1 = float.Parse(strUserParam[24]) / 10;      //长宽比10
                float iEroCir2 = float.Parse(strUserParam[25]) / 10;      //长宽比10
                int iThr2 = int.Parse(strUserParam[26]);      //长宽比10

                int hv_iloudu_thr = int.Parse(strUserParam[27]);//漏镀缺陷阈值（80）
                int hv_iloudu_area = int.Parse(strUserParam[28]);//漏镀缺陷面积（100）
                float hv_iloudu_erosion = float.Parse(strUserParam[29]);//漏镀缺陷检测范围（10）


                int iProductCode = int.Parse(strUserParam[94]);     //区分各工位
                int iSaveImg = int.Parse(strUserParam[95]);     //是否保存图片，1-保存jpeg，2-保存bmp


               



                bool iCheckSelAll = false;   //是否全检还是依赖于外部选择
                                             //用户检测项可选项 strUserParam[94]开始  , 0：屏蔽 1：启用 ， 默认 1（打钩）
                int A_端头崩碎 = iCheckSelAll ? 1 : int.Parse(strUserParam[104]); //A_端头崩碎               
                int A_漏镀 = iCheckSelAll ? 1 : int.Parse(strUserParam[105]); //A_漏镀
                #endregion

                #region 参数传递
                hv_parameter = new HTuple();
                hv_parameter = hv_parameter.TupleConcat(hv_iThr1);
                hv_parameter = hv_parameter.TupleConcat(hv_iThr2);
                hv_parameter = hv_parameter.TupleConcat(hv_iMaskWith);
                hv_parameter = hv_parameter.TupleConcat(hv_iDyn_Thr);

                hv_parameter = hv_parameter.TupleConcat(hv_iScaleMult);
                hv_parameter = hv_parameter.TupleConcat(hv_iScaleAdd);
                hv_parameter = hv_parameter.TupleConcat(hv_iErrAll);
                hv_parameter = hv_parameter.TupleConcat(hv_iOpening_Circle);

                hv_parameter = hv_parameter.TupleConcat(hv_Area_Duanmian);
                hv_parameter = hv_parameter.TupleConcat(hv_iAngleScale);
                hv_parameter = hv_parameter.TupleConcat(hv_iRecty);
                hv_parameter = hv_parameter.TupleConcat(hv_iErrArea2);

                hv_parameter = hv_parameter.TupleConcat(iSobelSize);
                hv_parameter = hv_parameter.TupleConcat(iThr1);
                hv_parameter = hv_parameter.TupleConcat(iClosCir);
                hv_parameter = hv_parameter.TupleConcat(iEroCir1);

                hv_parameter = hv_parameter.TupleConcat(iEroCir2);
                hv_parameter = hv_parameter.TupleConcat(iThr2);

                hv_parameter = hv_parameter.TupleConcat(hv_iloudu_thr);
                hv_parameter = hv_parameter.TupleConcat(hv_iloudu_area);
                hv_parameter = hv_parameter.TupleConcat(hv_iloudu_erosion);


                #endregion

                #region  调试模式初始化

                int ErrorIsOn = 0;
                #region  A_端头崩碎
                ErrorIsOn = A_端头崩碎;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_端头崩碎" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_端头崩碎" + ":" + "检测" + "\n";
                }
                #endregion
                #region A_漏镀12
                ErrorIsOn = A_漏镀;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_漏镀" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_漏镀" + ":" + "检测" + "\n";
                }
                #endregion

                strMessage = DebugPrint(strDebug, is_Debug);
                #endregion
              




                #region****保存所有图片，不区分正反面

                //图片保存路径
                string ImageSavePathFive = @"D:\六面机检测程序内部图片保存\一号工位\";
                string ImageSavePathSix = @"D:\六面机检测程序内部图片保存\二号工位\";
                string ImageSaveFormat;

                if (iSaveImg == 1 | iSaveImg == 2)
                {
                    #region 图片格式判断
                    switch (iSaveImg)//判断保存图片的格式：1-jpeg，2-bmp
                    {
                        case 2:
                            ImageSaveFormat = "bmp";
                            break;
                        default:
                            ImageSaveFormat = "jpeg";
                            break;

                    }//判断保存图片的格式
                    #endregion

                    #region//图片保存，如果文件夹不存在则自动创建文件夹
                    if (iProductCode == 1) //五号相机
                    {
                        if (!System.IO.Directory.Exists(ImageSavePathFive))
                        {
                            System.IO.Directory.CreateDirectory(ImageSavePathFive);
                        }

                        Number555 = Number555 + 1;
                        //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        hv_num4 = ImageSavePathFive + DateTime.Now.Ticks.ToString() + "-" + Number555.ToString() + "." + ImageSaveFormat;

                        HOperatorSet.WriteImage(hoImage, ImageSaveFormat, 0, hv_num4);
                    }

                    if (iProductCode == 2) //六号相机
                    {
                        if (!System.IO.Directory.Exists(ImageSavePathSix))
                        {
                            System.IO.Directory.CreateDirectory(ImageSavePathSix);
                        }

                        Number666 = Number666 + 1;
                        //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        hv_num4 = ImageSavePathSix + DateTime.Now.Ticks.ToString() + "-" + Number666.ToString() + "." + ImageSaveFormat;
                        HOperatorSet.WriteImage(hoImage, ImageSaveFormat, 0, hv_num4);
                    }
                    #endregion
                }
                #endregion


                #region ***判断彩色还是黑白，彩色图像二值化
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    //HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                    HOperatorSet.Rgb1ToGray(hoReduced, out ho_GrayImage);
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_GrayImage, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion
                #endregion


             


                duanmian_dingwei(ho_GrayImage, out ho_Region_Duanmian, hv_parameter, out hv_NGCode);

                //端面参数预处理

                HObject ho_Rect_Duanmian ;
                HTuple Area_Duanmian, Row_Duanmian, Col_Duanmian, hv_Rectangularity_Duanmian, hv_Length1_Duanmian, hv_Length2_Duanmian;
                //面积
                HOperatorSet.AreaCenter(ho_Region_Duanmian, out Area_Duanmian, out Row_Duanmian, out Col_Duanmian);

                //尺寸
                HOperatorSet.SmallestRectangle2(ho_Region_Duanmian, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rect_Duanmian, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                hv_Length1_Duanmian = hv_Length1 * 2;
                hv_Length2_Duanmian = hv_Length2 * 2;
                 //角度
                 HTuple Deg_Duanmian;

                HOperatorSet.TupleDeg(hv_Phi, out Deg_Duanmian);
                //矩形度
                HOperatorSet.Rectangularity(ho_Region_Duanmian, out hv_Rectangularity_Duanmian);

                #region 程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                    dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    return listObj2Draw;
                }
                #endregion

                #region  调试模式

                if (is_Debug)  //调试状态输出信息
                {
                    //图像显示
                    //syShowRegionBorder(ho_Region_Duanmian, ref listObj2Draw, "OK");
                    //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                    strDebug += "（1）端面定位相关参数：\n";
                    //strDebug += "粗定位阈值:" + hv_iloudu_thr.ToString() + "\n";
                    strDebug += "端面长度:" + hv_Length1_Duanmian.D.ToString("0.0") + "pix" + "\n";
                    strDebug += "端面宽度:" + hv_Length2_Duanmian.D.ToString("0.0") + "pix" + "\n";
                    strDebug += "端面面积:" + Area_Duanmian.D.ToString("0.0") + "pix" + "\n";
                    strDebug += "端面角度:" + Deg_Duanmian.D.ToString("0.0") + " 度" + "\n";
                    strDebug += "端面矩形度:" + hv_Rectangularity_Duanmian.D.ToString("0.0") + "（0最小，1最大，越接近1越类似于矩形）" + "\n";


                }

                strDebug += "\n";
                strMessage = DebugPrint(strDebug, is_Debug);
                #endregion


                #region 缺陷显示（duanmian_dingwei）
               
                if ((int)(new HTuple(hv_NGCode.TupleEqual(1))) != 0) //可修改
                {
                    #region***判断端面面积，过小直接无定位
                    listObj2Draw[1] = "NG-无定位";//可修改
                                               
                    HOperatorSet.CountObj(ho_Region_Duanmian, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_Region_Duanmian, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }

                    lsInfo2Draw.Add("端面最小面积：" + hv_Area_Duanmian.ToString() + "pix");//可修改
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + Area_Duanmian.D.ToString("0.0") + " pix");//可修改
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }


                //*判断矩形度
              
                if ((int)(new HTuple(hv_NGCode.TupleEqual(2))) != 0)
                {
                    #region***判断端面矩形度，过小NG
                    //HDevelopStop();
                    if (hv_Rectangularity_Duanmian < (hv_iRecty * 0.9))
                    {
                        #region***判断端面矩形度，过小直接无定位
                        listObj2Draw[1] = "NG-尺寸不符";//"NG-切割不良"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_Duanmian, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_Duanmian, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("最小矩形度：" + hv_parameter[10].ToString() + "pix * ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前矩形度：" + hv_Rectangularity_Duanmian.D.ToString("0.000") + " pix * ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    else
                    {
                        #region***端面矩形度小于合格阈值，大于无定位阈值，判定尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符";//"NG-切割不良"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_Duanmian, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_Duanmian, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("最小矩形度：" + hv_iRecty.ToString() + "pix * ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前矩形度：" + hv_Rectangularity_Duanmian.D.ToString("0.000") + " pix * ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion
                }


              
                //0508更改，判断产品角度，歪斜过大直接无定位 正负10度
              
                if ((int)(new HTuple(hv_NGCode.TupleEqual(3))) != 0)
                {
                    #region***判断产品角度，歪斜过大直接无定位 正负10度
                    listObj2Draw[1] = "NG-无定位";//角度歪斜
                    List<PointF> lnBarcode = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcode.ToArray());
                    listObj2Draw.Add("NG");

                    //输出NG详情
                    lsInfo2Draw.Add("歪斜角度:" + Deg_Duanmian.D.ToString("0.0") + " 度");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }


                //*判断产品尺寸（）
                HTuple hv_Length1Scale = iLength1Scale;
                HTuple hv_Length2Scale = iLength2Scale;

                if ((int)((new HTuple(hv_Length1.TupleLess(iLength1 - hv_Length1Scale))).TupleOr(new HTuple(hv_Length1.TupleGreater(
                    iLength1 + hv_Length1Scale)))) != 0)
                {
                    #region
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG1 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG1.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";//"NG-尺寸不符";//NG-无定位

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                    iLength2 + hv_Length2Scale)))) != 0)
                {
                    #region

                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";// "NG-尺寸不符";

                    //输出NG详情
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }
                #endregion






                //端面缺陷检测
                Detect_Err_12(ho_GrayImage, ho_Region_Duanmian, out ho_Region_Err2, hv_parameter, out hv_NGCode);

                #region 程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                    dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    return listObj2Draw;
                }
                #endregion

                #region  调试模式

                if (is_Debug)  //调试状态输出信息
                {
                    //图像显示
                    //syShowRegionBorder(ho_Region_Duanmian, ref listObj2Draw, "OK");
                    //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                    strDebug += "(2)端头崩碎缺陷检测相关参数：\n";
                    strDebug += "端头崩碎相对阈值:" + iThr1.ToString() + "\n";
                    strDebug += "端头崩碎绝对阈值:" + iThr2.ToString() + "\n";
                    strDebug += "端头崩碎缺陷面积:" + hv_iErrArea2.ToString() + "\n";
                 
                    //strDebug += "端面长度:" + hv_Length1_Duanmian.D.ToString("0.0") + "pix" + "\n";

                    //int hv_iThr1 = int.Parse(strUserParam[8]);     //iErrThres  = 40    缺陷阈值40
                    //int hv_iThr2 = int.Parse(strUserParam[9]);      //iErrArea = 50      缺陷面积50


                }

                strDebug += "\n";
                strMessage = DebugPrint(strDebug, is_Debug);
                #endregion

                #region 缺陷显示（Detect_Err_12）


                ho_Region_Display = ho_Region_Err2;//可修改
                HOperatorSet.CountObj(ho_Region_Display, out hv_Num_Display);
                HOperatorSet.Union1(ho_Region_Display, out ho_Union_Dispay);
                HOperatorSet.AreaCenter(ho_Union_Dispay, out hv_Area_Display, out hv_Row_Display, out hv_Column_Display);
                if (A_端头崩碎 == 0) goto A_端头崩碎END;
                if ((int)(new HTuple(hv_NGCode.TupleEqual(4))) != 0)//可修改
                {
                    #region
                    listObj2Draw[1] = "NG-端头崩碎";//可修改
                    for (int i = 1; i <= hv_Num_Display; i++)
                    {
                        HOperatorSet.SelectObj(ho_Region_Display, out ho_Region_Sel, i);
                        syShowRegionBorder(ho_Region_Sel, ref listObj2Draw, "NG");
                    }

                    //输出NG详情
                    lsInfo2Draw.Add("端头缺陷最大面积：" + hv_iErrArea2.ToString() + " pix ");//可修改
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Area_Display.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }
            A_端头崩碎END:
                #endregion



                //漏镀检测
                Detect_loudu_12(hoReduced, ho_Region_Duanmian, out ho_Region_loudu, hv_parameter, out hv_NGCode);

                #region 程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                    dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    return listObj2Draw;
                }
                #endregion

                #region  调试模式
                
                if (is_Debug)  //调试状态输出信息
                {
                    //图像显示
                    syShowRegionBorder(ho_Region_Duanmian, ref listObj2Draw, "OK");
                    //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                    strDebug += "(3)漏镀缺陷检测相关参数：\n";
                    strDebug += "漏镀缺陷阈值:" + hv_iloudu_thr.ToString() + "\n";
                    strDebug += "漏镀缺陷面积:" + hv_iloudu_area.ToString() + "\n";
                    strDebug += "漏镀缺陷检测范围:" + hv_iloudu_erosion.ToString() + "\n";

                   

                   
                }

                strDebug += "\n";
                strMessage = DebugPrint(strDebug, is_Debug);
                #endregion

                #region 缺陷显示（Detect_loudu_12）


                ho_Region_Display = ho_Region_loudu;//可修改
                HOperatorSet.CountObj(ho_Region_Display, out hv_Num_Display);
                HOperatorSet.Union1(ho_Region_Display, out ho_Union_Dispay);
                HOperatorSet.AreaCenter(ho_Union_Dispay, out hv_Area_Display, out hv_Row_Display, out hv_Column_Display);
                if (A_漏镀 == 0) goto A_漏镀END;
                if ((int)(new HTuple(hv_NGCode.TupleEqual(40))) != 0)//可修改
                {
                    #region
                    listObj2Draw[1] = "NG-漏镀";
                    for (int i = 1; i <= hv_Num_Display; i++)
                    {
                        HOperatorSet.SelectObj(ho_Region_Display, out ho_Region_Sel, i);
                        syShowRegionBorder(ho_Region_Sel, ref listObj2Draw, "NG");
                    }

                    //输出NG详情
                    lsInfo2Draw.Add("漏镀缺陷最大面积：" + hv_iloudu_area.ToString() + " pix ");//可修改
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Area_Display.D.ToString("0.0") + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }
            A_漏镀END:
                #endregion



          



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

                //syShowRegionBorder(ho_Region_Duanmian, ref listObj2Draw, "OK");
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


        //六面机0603 34 相机 算法  引用文件14     
        public static List<object> sySixSideDetect_0603R_Camera34(HObject hoImage, List<PointF[]> lkkPolygon, string strParams, ref string strMessage)
        {

            #region  *** 34相机 六面机左右面 ***
            if (bUseMutex) muDetect12.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("NG"); listObj2Draw.Add(888);
            try
            {
                HObject hoRegion, hoReduced, hoConcate, hoUnion;
                HObject ho_RegionSel;
                HTuple hv_Area, hv_Phi, hv_Length1, hv_Length2, hv_Row1;
                HTuple hv_Row, hv_Column, hv_Num;
                HTuple hv_Column1, hv_Max1;
                //新声明
                HObject ho_GrayImage;
                HObject ho_RegionErrDConn, ho_Image1, ho_Image3;
                HTuple NChannel;

                HObject ho_Region_cemian, ho_Rectangle_Cemian;
                HTuple hv_NGCode, hv_parameter;

               



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

                #region  //34相机参数(接收界面传递的参数)
                string[] strUserParam = strParams.Split('#');
                //3,4相机参数             
                int iLength1 = int.Parse(strUserParam[4]);      //iLength1  = 145    半长
                int iLength1Scale = int.Parse(strUserParam[5]); //iLength1Scale = 15 半长变化值
                int iLength2 = int.Parse(strUserParam[6]);      //iLength2  = 45     半宽
                int iLength2Scale = int.Parse(strUserParam[7]); //iLength2Scale = 15 半宽变化值

                int hv_iThr_Cudingwei = int.Parse(strUserParam[8]);     //粗定位阈值
                int hv_AreaDuanmian = int.Parse(strUserParam[9]);      //侧面预期面积

                int hv_iPos_Sigma = int.Parse(strUserParam[10]);    //寻边参数1
                int hv_iPos_Thr1 = int.Parse(strUserParam[11]);      //寻边参数2
                int hv_iPos_Thr2 = int.Parse(strUserParam[12]);      //寻边参数2


                int hv_iMaskWith = int.Parse(strUserParam[13]);    //侧面提取mean函数参数
                int hv_iDyn_Thr = int.Parse(strUserParam[14]);    //侧面提取dyn_thr函数参数
                int hv_iOpen1 = int.Parse(strUserParam[15]);     //开运算

                float hv_iErrShink1 = float.Parse(strUserParam[16]) / 100;     //缺陷搜索范围宽度
                float hv_iErrShink2 = float.Parse(strUserParam[17]) / 100;     //缺陷搜索范围高度
                int hv_iErrThr = int.Parse(strUserParam[18]);     //内部缺陷阈值
                int hv_iErrArea = int.Parse(strUserParam[19]);     //内部缺陷面积

                int iProductCode = int.Parse(strUserParam[94]);     //区分各工位
                int iSaveImg = int.Parse(strUserParam[95]);     //是否保存图片，1-保存jpeg，2-保存bmp



                bool iCheckSelAll = false;   //是否全检还是依赖于外部选择
                                             //用户检测项可选项 strUserParam[94]开始  , 0：屏蔽 1：启用 ， 默认 1（打钩）
                int A_产品沾污 = iCheckSelAll ? 1 : int.Parse(strUserParam[104]); //A_上爬不足

                #endregion

                #region 调试模式初始化
                bool is_Debug;
                if (strMessage == "0")
                {
                    is_Debug = false;
                }
                else
                {
                    is_Debug = true;
                }


                //初始化调试输出内容
                string strDebug = "";

                #region  缺陷检测开关显示

                int ErrorIsOn = 0;
                #region  A_产品沾污
                ErrorIsOn = A_产品沾污;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_产品沾污" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_产品沾污" + ":" + "检测" + "\n";
                }
                #endregion
             

                strMessage = DebugPrint(strDebug, is_Debug);
                #endregion


                #endregion

                #region****保存所有图片，不区分正反面

                //图片保存路径
                string ImageSavePathFive = @"D:\六面机检测程序内部图片保存\三号工位\";
                string ImageSavePathSix = @"D:\六面机检测程序内部图片保存\四号工位\";
                string ImageSaveFormat;

                if (iSaveImg == 1 | iSaveImg == 2)
                {
                    #region 图片格式判断
                    switch (iSaveImg)//判断保存图片的格式：1-jpeg，2-bmp
                    {
                        case 2:
                            ImageSaveFormat = "bmp";
                            break;
                        default:
                            ImageSaveFormat = "jpeg";
                            break;

                    }//判断保存图片的格式
                    #endregion

                    #region//图片保存，如果文件夹不存在则自动创建文件夹
                    if (iProductCode == 3) //五号相机
                    {
                        if (!System.IO.Directory.Exists(ImageSavePathFive))
                        {
                            System.IO.Directory.CreateDirectory(ImageSavePathFive);
                        }

                        Number555 = Number555 + 1;
                        //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        hv_num4 = ImageSavePathFive + DateTime.Now.Ticks.ToString() + "-" + Number555.ToString() + "." + ImageSaveFormat;

                        HOperatorSet.WriteImage(hoImage, ImageSaveFormat, 0, hv_num4);
                    }

                    if (iProductCode == 4) //六号相机
                    {
                        if (!System.IO.Directory.Exists(ImageSavePathSix))
                        {
                            System.IO.Directory.CreateDirectory(ImageSavePathSix);
                        }

                        Number666 = Number666 + 1;
                        //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        hv_num4 = ImageSavePathSix + DateTime.Now.Ticks.ToString() + "-" + Number666.ToString() + "." + ImageSaveFormat;
                        HOperatorSet.WriteImage(hoImage, ImageSaveFormat, 0, hv_num4);
                    }
                    #endregion
                }
                #endregion



                #region ***判断彩色还是黑白***

                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_GrayImage, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_GrayImage, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion
                #endregion






                #region 参数传递
                hv_parameter = new HTuple();
                hv_parameter = hv_parameter.TupleConcat(hv_iThr_Cudingwei);
                hv_parameter = hv_parameter.TupleConcat(hv_AreaDuanmian);
                hv_parameter = hv_parameter.TupleConcat(hv_iPos_Sigma);
                hv_parameter = hv_parameter.TupleConcat(hv_iPos_Thr1);
                hv_parameter = hv_parameter.TupleConcat(hv_iPos_Thr2);

                hv_parameter = hv_parameter.TupleConcat(hv_iMaskWith);
                hv_parameter = hv_parameter.TupleConcat(hv_iDyn_Thr);
                hv_parameter = hv_parameter.TupleConcat(hv_iOpen1);
                hv_parameter = hv_parameter.TupleConcat(1111111);
                #endregion

                //侧面定位，输出侧面区域region
                cemian_dingwei(ho_GrayImage, out ho_Region_cemian, out ho_Rectangle_Cemian, hv_parameter, out hv_NGCode);

                #region 程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                    dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    return listObj2Draw;
                }
                #endregion

                //侧面参数预处理
                //HObject;
                HTuple hv_Length1_Cemian, hv_Length2_Cemian;

                HOperatorSet.SmallestRectangle2(ho_Region_cemian, out hv_Row, out hv_Column,
                       out hv_Phi, out hv_Length1, out hv_Length2);

                hv_Length1_Cemian = hv_Length1 * 2;
                hv_Length2_Cemian = hv_Length2 * 2;

                #region 调试模式
                if (is_Debug)
                {                   
                    //图像显示
                    HOperatorSet.Connection(ho_Rectangle_Cemian, out ho_RegionErrDConn);
                    syShowRegionBorder(ho_RegionErrDConn, ref listObj2Draw, "OK");


                    strDebug += "（1）侧面定位相关参数：\n";
                    //strDebug += "粗定位阈值:" + hv_iloudu_thr.ToString() + "\n";
                    strDebug += "侧面长度:" + hv_Length1_Cemian.D.ToString("0.0") + "pix" + "\n";
                    strDebug += "侧面宽度:" + hv_Length2_Cemian.D.ToString("0.0") + "pix" + "\n";

                   
                }
                strDebug += "\n";
                strMessage = DebugPrint(strDebug, is_Debug);

                #endregion

                #region 缺陷显示（cemian_dingwei）
                if ((int)(new HTuple(hv_NGCode.TupleEqual(1))) != 0)  //****粗定位面积54830
                {
                    #region***粗定位面积不符合要求,判定无定位
                    listObj2Draw[1] = "NG-无定位";
                    //输出NG详情
                    HOperatorSet.CountObj(ho_Rectangle_Cemian, out hv_Num);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_Rectangle_Cemian, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    return listObj2Draw;
                    #endregion
                }

     
                if ((int)(new HTuple(hv_NGCode.TupleEqual(2))) != 0)
                {
                    #region ***提取侧面-分割线提取失败
                    listObj2Draw[1] = "NG-尺寸不符";

                    syShowRegionBorder(ho_Rectangle_Cemian, ref listObj2Draw, "NG");
                    lsInfo2Draw.Add("提取侧面-分割线提取失败");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }



              


                #region ***侧面尺寸
                //*判断产品尺寸（）            
                HTuple hv_Length1Scale = iLength1Scale;
                HTuple hv_Length2Scale = iLength2Scale;

                if ((int)((new HTuple(hv_Length1.TupleLess(iLength1 - hv_Length1Scale))).TupleOr(new HTuple(hv_Length1.TupleGreater(
                    iLength1 + hv_Length1Scale)))) != 0)
                {
                    #region
                    //HDevelopStop();
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG1 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG1.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";  //0508更改 NG-尺寸不符

                    //输出NG详情
                    lsInfo2Draw.Add("1标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                if ((int)((new HTuple(hv_Length2.TupleLess(iLength2 - hv_Length2Scale))).TupleOr(new HTuple(hv_Length2.TupleGreater(
                    iLength2 + hv_Length2Scale)))) != 0)
                {
                    #region                
                    //NG绘制红色矩形
                    List<PointF> lnBarcodeNG2 = dhFindVerticesOfRectangle2(hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                    listObj2Draw.Add("多边形");
                    listObj2Draw.Add(lnBarcodeNG2.ToArray());
                    listObj2Draw.Add("NG");

                    listObj2Draw[1] = "NG-尺寸不符";  //0508更改 NG-尺寸不符

                    //输出NG详情
                    lsInfo2Draw.Add("1标准尺寸：" + iLength1.ToString() + "pix * " + iLength2.ToString() + "pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸：" + hv_Length1.D.ToString("0.0") + " pix * " + hv_Length2.D.ToString("0.0") + " pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                #endregion

                #endregion

                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时111," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion



                //*检测内部缺陷
                #region 参数传递
                HTuple hv_Parameter_Err;
                hv_Parameter_Err = new HTuple();
                hv_Parameter_Err = hv_Parameter_Err.TupleConcat(hv_iErrShink1);
                hv_Parameter_Err = hv_Parameter_Err.TupleConcat(hv_iErrShink2);
                hv_Parameter_Err = hv_Parameter_Err.TupleConcat(hv_iErrThr);
                hv_Parameter_Err = hv_Parameter_Err.TupleConcat(hv_iErrArea);

                HObject ho_Rectangle_Search, ho_Region_Err;
                #endregion

                Detect_Err_34(ho_GrayImage, ho_Region_cemian, out ho_Rectangle_Cemian,
                             out ho_Rectangle_Search, out ho_Region_Err, hv_Parameter_Err,
                             out hv_NGCode);

                #region 程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                    dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    return listObj2Draw;
                }
                #endregion

                #region  调试模式
                if (is_Debug)
                {
                    //图像显示
                    HOperatorSet.Connection(ho_Rectangle_Search, out ho_RegionErrDConn);
                    syShowRegionBorder(ho_RegionErrDConn, ref listObj2Draw, "NG");

                    strDebug += "（1）侧面缺陷检测相关参数：\n";
                    //strDebug += "粗定位阈值:" + hv_iloudu_thr.ToString() + "\n";
                    strDebug += "缺陷搜索范围宽度:" + hv_iErrShink1.ToString("0.0") + "\n";
                    strDebug += "缺陷搜索范围高度:" + hv_iErrShink2.ToString("0.0")  + "\n";

                    strDebug += "侧面缺陷阈值:" + hv_iErrThr.ToString("0.0")  + "\n";
                    strDebug += "侧面缺陷面积:" + hv_iErrArea.ToString("0.0")  + "\n";
                  
                   

                }
                strDebug += "\n";
                strMessage = DebugPrint(strDebug, is_Debug);
                #endregion


                #region 缺陷显示(Detect_Err_34)


                if (A_产品沾污 == 0) goto A_产品沾污END;

                if ((int)(new HTuple(hv_NGCode.TupleEqual(3))) != 0)
                {
                    #region                    
                    listObj2Draw[1] = "NG-产品沾污";
                    HOperatorSet.Connection(ho_Region_Err, out ho_RegionErrDConn);
                    hv_Num = 0;
                    HOperatorSet.CountObj(ho_RegionErrDConn, out hv_Num);
                    HOperatorSet.AreaCenter(ho_Region_Err, out hv_Area, out hv_Row1, out hv_Column1);
                    HOperatorSet.TupleMax(hv_Area, out hv_Max1);
                    for (int i = 1; i <= hv_Num; i++)
                    {
                        HOperatorSet.SelectObj(ho_RegionErrDConn, out ho_RegionSel, i);
                        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    }
                    //syShowRegionBorder(ho_Rectangle_Cemian, ref listObj2Draw, "NG");
                    //syShowRegionBorder(ho_Rectangle_Search, ref listObj2Draw, "OK");

                    //输出NG详情
                    lsInfo2Draw.Add("缺陷最大面积：" + hv_iErrArea.ToString() + " pix ");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前面积：" + hv_Max1.ToString() + "pix");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));

                    return listObj2Draw;
                    #endregion
                }
                A_产品沾污END:
                #endregion

 
                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时114," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion

                listObj2Draw[1] = "OK";
                syShowRegionBorder(ho_Rectangle_Cemian, ref listObj2Draw, "OK");
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
                if (bUseMutex) muDetect12.ReleaseMutex();
            }
            #endregion

        }

        //六面机0603 56 相机 算法  引用文件16  
        public static List<object> sySixSideDetect_0603R_Camera56_OCR(HObject hoImage, HObject ho_ModelRegion, HTuple hv_ModelID, HTuple hv_ModelParam, List<PointF[]> lkkPolygon, string strParams, ref string strMessage)
        {
            #region  *** 0603 带字码检测  ***

            #region  *** 56相机 六面机上下面  ***
         
            //dhDll.clsFunction.writeTxtFile("D:\\1111.txt", System.IO.FileMode.Create, new List<string>(strUserParam));

            if (bUseMutex) muDetect8.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<object> listObj2Draw = new List<object>();
            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("NG"); listObj2Draw.Add(888);
            try
            {
                HObject ho_GrayImage;
                HTuple hv_Length1_1, hv_Length2_2;
                HObject hoReduced = null, hoConcate = null, hoRegion = null, hoUnion = null, ho_RegionSel = null, hoRegionsConn = null, hoSelectedRegions = null, ho_Rectangle = null;
                HObject ho_RegionTrans = null;
                HTuple NChannel, hv_Num;
                HTuple hv_Area, hv_Row111, hv_Column111, RowDDD, ColDDD, PhiDDD, Length1DDD, Length2DDD;
                HObject ho_Rects;
                HObject hoSelectedRegions1;
                HObject ho_SortedRegions;
                HObject ho_ConnectedRegions;
                HObject ho_Region;




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

                #region ***5,6相机参数
                //5,6相机参数

                int hv_leixing = int.Parse(strUserParam[3]);  //检测类型：车规-0、常规-1、不检-2、抗硫化-3、中兴抗硫化-4、HKRK-5、中兴-6、手机-7、          

                //第一页
                int iFixThres = int.Parse(strUserParam[4]);  //粗定位阈值30
                int iAngleScale = int.Parse(strUserParam[5]); //歪斜角度正负极限 10
                int iBorderScale = int.Parse(strUserParam[6]); //产品到图像边界最大距离 50
                int iFixThres2 = int.Parse(strUserParam[7]); //10

                int iLength1 = int.Parse(strUserParam[8]);//产品长度1000
                int iLength1Scale = int.Parse(strUserParam[9]);//产品长度偏差100
                int iLength2 = int.Parse(strUserParam[10]);//产品宽度500
                int iLength2Scale = int.Parse(strUserParam[11]);//产品宽度偏差100
                int hv_iArea_heitu = int.Parse(strUserParam[12]);//黑图判定面积500
                int hv_iArea_cudingwei = int.Parse(strUserParam[13]);//// 8000;//粗定位，用于筛选电极，确定电极数量，判断正反面


                int hv_iFrontDianji_Width_Min = int.Parse(strUserParam[14]);// 面电极最小宽度
                int hv_iFrontDianji_Width_max = int.Parse(strUserParam[15]);// 面电极最大宽度

                int hv_iBackDianji_Width_Min = int.Parse(strUserParam[16]);// 背电极最小宽度
                int hv_iBackDianji_Width_Max = int.Parse(strUserParam[17]);// 背电极最大宽度

                int hv_iFrontDianji_Height_Min = int.Parse(strUserParam[18]);// 面电极最小高度
                int hv_iFrontDianji_Height_max = int.Parse(strUserParam[19]);// 面电极最大高度

                int hv_iBackDianji_Height_Min = int.Parse(strUserParam[20]);// 背电极最小高度
                int hv_iBackDianji_Height_Max = int.Parse(strUserParam[21]);// 背电极最大高度               

                int Ilenth3diff_zhengmian = int.Parse(strUserParam[22]); // 面电极高度差
                int Ilenth3diff_beimian = int.Parse(strUserParam[23]); // 背电极高度差


                int Ilenth4Sum_miandianji = int.Parse(strUserParam[24]);// 面电极电极宽度和
                int Ilenth4diff_miandianji = int.Parse(strUserParam[25]);//面两电极宽度差
                int iSmallestArea_miandianji = int.Parse(strUserParam[26]); //面电极最小面积
                int iBiggstArea_miandianji = int.Parse(strUserParam[27]);   //面电极最大面积

                int Ilenth4Sum_beidianji = int.Parse(strUserParam[28]);// 背电极宽度和
                int Ilenth4diff_beidianji = int.Parse(strUserParam[29]);// 背电极宽度差
                int iSmallestArea_beidianji = int.Parse(strUserParam[30]); //背电极最小面积
                int iBiggstArea_beidianji = int.Parse(strUserParam[31]);   //背电极最大面积

                float Length2DDD_zhengmian_Std = float.Parse(strUserParam[32]); //正面左右电极比值
                float Length2DDD_beimian_Std = float.Parse(strUserParam[33]);   //背面左右电极比值

                


                float Rectangularity1 = float.Parse(strUserParam[36]);//背面电极矩形度判定
                Rectangularity1 = Rectangularity1 / 100;






                //int IWidthciti = int.Parse(strUserParam[44]);//瓷体长度最大值

                int hv_AutoThreshold1 = int.Parse(strUserParam[44]);//背导区域提取阈值20
                int hv_KirschThr = int.Parse(strUserParam[45]);//瓷体区域提取阈值30
                int hv_KirschClosing = int.Parse(strUserParam[46]);//瓷体区域闭运算3
                int hv_KirschOpening = int.Parse(strUserParam[47]);//瓷体区域开运算5

                int hv_iBlackThr = int.Parse(strUserParam[48]);//背面-瓷体沾污阈值
                int hv_iBlackArea = int.Parse(strUserParam[49]);//背面-瓷体沾污面积

                int hv_iWhiteWidth = int.Parse(strUserParam[50]);  //瓷体沾锡-平滑参数        
                int hv_iWhiteExp = int.Parse(strUserParam[51]);    //瓷体沾锡-阈值        
                int hv_iWhiteClosing = int.Parse(strUserParam[52]);    //瓷体沾锡-阈值 3        

                int hv_iWhiteArea = int.Parse(strUserParam[53]);//背面-瓷体沾锡面积 

                //第二页参数

                int hv_iGlobalThr = int.Parse(strUserParam[54]);//背面-全局沾污面积                
                int iBlackArea1 = int.Parse(strUserParam[55]);//背面-全局沾污面积
                int hv_iBlackThr2 = int.Parse(strUserParam[56]);//背面-电极沾污面积               
                int iBlackArea2 = int.Parse(strUserParam[57]);//背面-电极沾污面积

                //电极的平均宽度与最小外接矩形宽度的差值（实际距离差值）（背面电极延锡判断）
                int iWidthDiff = int.Parse(strUserParam[61]);//

                int hv_iloudu_mean = int.Parse(strUserParam[62]);//漏镀检测-电极区域三通道灰度值（小于该值视作漏镀）
                int hv_iloudu_divstd = int.Parse(strUserParam[63]);//漏镀检测-电极区域与瓷体区域灰度图像平均灰度值的差值（大于该值视作漏镀）


                //保护层破损检测
                int hv_iErosionWidth_IIG2 = int.Parse(strUserParam[64]);//保护层崩碎腐蚀宽度
                int hv_iErosionHeight_IIG2 = int.Parse(strUserParam[65]);//保护层崩碎高度

                int hv_iProtectBrokenArea = int.Parse(strUserParam[66]);//保护层破损面积
                int hv_iProtectexp = int.Parse(strUserParam[67]); //焊锡缺陷检测，exp阈值-20

                int iProtectBrokenArea3 = int.Parse(strUserParam[68]);//保护层挂锡面积
                int iProtectBrokenResThres = int.Parse(strUserParam[69]); //保护层破损相对阈值 30

                int hv_iErrWidth = int.Parse(strUserParam[70]); //保护层崩碎缺陷宽度（小于该宽度不视作缺陷）
                int hv_iErrHeight = int.Parse(strUserParam[71]);//保护层崩碎缺陷高度（小于该高度不视作缺陷）


                int EroWidth3 = int.Parse(strUserParam[72]);//保护层腐蚀宽度3
                int EroHeight3 = int.Parse(strUserParam[73]);//保护层腐蚀高度3

                //电极漏磁、电极缺角检测           
                int hv_iShangpa = int.Parse(strUserParam[74]);  //是否检测，电极两端区域。1-检测电极两端区域，0（或除一以外）-不检
                int hv_iScale_height_1 = int.Parse(strUserParam[75]);// 上爬高度-1     电极上爬不足高度收窄
                int hv_iScale_width_1 = int.Parse(strUserParam[76]);//  上爬宽度-1      电极上爬不足宽度收窄


                int hv_iScale_height_2 = int.Parse(strUserParam[78]);// 上爬高度-1     电极上爬不足高度收窄
                int hv_iScale_width_2 = int.Parse(strUserParam[79]);//  上爬宽度-1      电极上爬不足宽度收窄
                int iwidth_aoxian = int.Parse(strUserParam[80]);//上爬不足凹陷宽度
                int hv_iArea_shangpa1 = int.Parse(strUserParam[81]);//凹陷面积


                int hv_iScale_height_3 = int.Parse(strUserParam[82]);//上爬高度-2
                int iScale_width_2 = int.Parse(strUserParam[83]);// 上爬宽度-2 


                //字码缺陷检测
                int hv_iOnorOff = int.Parse(strUserParam[84]);//是否开启字符个数检测（1开启，0关闭）
                int hv_iNum_MK = int.Parse(strUserParam[85]);  //字码个数
                float hv_iScore_MK = float.Parse(strUserParam[86]);//字码匹配分数（0-100）
                hv_iScore_MK = hv_iScore_MK / 100;

                int hv_iArea_MK1 = int.Parse(strUserParam[87]);//标记断线面积
                int hv_iOpen_MK1 = int.Parse(strUserParam[88]);//开运算参数

                int hv_iArea_MK2 = int.Parse(strUserParam[89]);//标记不清面积
                int hv_iOpen_MK2 = int.Parse(strUserParam[90]);//开运算参数

                int hv_iClosing_duanxian = int.Parse(strUserParam[91]);//字符断线（越小对断线越敏感）
                int hv_iClosing_ZIMATIQV = int.Parse(strUserParam[92]);//字符提取




                //其它参数
                int iProductCode = int.Parse(strUserParam[94]);  //产品类别(5-五号工位，6-六号工位)：
                int iSaveImg = int.Parse(strUserParam[95]);//是否保存5、6相机的OK图片（1-保存jpeg图片，2-保存bmp图片，其他不保存图片）
                float ipix = float.Parse(strUserParam[96]);  //像素距离到实际距离的转换系数
                ipix = ipix / 1000000;

                int iSaveImg2 = int.Parse(strUserParam[97]);//

                /*无定位
                 * 超时
                 * 程序出错
                 */

                bool iCheckSelAll = false;   //是否全检还是依赖于外部选择
               //用户检测项可选项 strUserParam[94]开始  , 0：屏蔽 1：启用 ， 默认 1（打钩）               

                int A_上爬不足 = iCheckSelAll ? 1 : int.Parse(strUserParam[104]); //A_上爬不足

                int A_字码个数 = iCheckSelAll ? 1 : int.Parse(strUserParam[105]);     //A_字码个数
                int A_字码匹配失败 = iCheckSelAll ? 1 : int.Parse(strUserParam[106]); //A_字码匹配失败
                int A_标记断线 = iCheckSelAll ? 1 : int.Parse(strUserParam[107]); //A_标记断线
                int A_标记不清 = iCheckSelAll ? 1 : int.Parse(strUserParam[108]); //A_标记不清

                int A_保护层崩碎 = iCheckSelAll ? 1 : int.Parse(strUserParam[109]); //A_保护层崩碎
                int A_保护层沾锡 = iCheckSelAll ? 1 : int.Parse(strUserParam[110]);     //A_保护层沾锡

                int A_漏镀 = iCheckSelAll ? 1 : int.Parse(strUserParam[111]);         //A_漏镀   
                             
                int A_电阻正面整体角度判定和长宽判定 = iCheckSelAll ? 1 : int.Parse(strUserParam[112]);   //A_电阻正面整体角度判定和长宽判定
                int A_正面电极面积判定 = iCheckSelAll ? 1 : int.Parse(strUserParam[113]); //A_正面电极面积判定                                                  
                int A_正面电极长宽判定 = iCheckSelAll ? 1 : int.Parse(strUserParam[114]); //A_正面电极长宽判定

                int A_电阻背面整体角度判定和长宽判定 = iCheckSelAll ? 1 : int.Parse(strUserParam[115]); //A_电阻背面整体角度判定和长宽判定
                int A_背面电极面积判定 = iCheckSelAll ? 1 : int.Parse(strUserParam[116]); //A_背面电极面积判定              
                int A_背面电极长宽判定 = iCheckSelAll ? 1 : int.Parse(strUserParam[117]);     //A_背面电极长宽判定

               
                int A_瓷体挂锡 = iCheckSelAll ? 1 : int.Parse(strUserParam[118]);   //A_瓷体挂锡
                int A_瓷体脏片 = iCheckSelAll ? 1 : int.Parse(strUserParam[119]); //A_瓷体脏片     
                //int A_背面延锡 = iCheckSelAll ? 1 : int.Parse(strUserParam[118]);     //A_背面延锡                                                


                #region 调试模式初始化
                bool is_Debug;
                if (strMessage == "0")
                {
                    is_Debug = false;
                }
                else
                {
                    is_Debug = true;
                }

                //初始化调试输出内容
                string strDebug = "";

                #endregion


               


                #region 缺陷检测选项
                int ErrorIsOn;
                strDebug += "缺陷检测选项:\n";
                #region
                ErrorIsOn = A_上爬不足;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_上爬不足" + ":" + "不检测" + "\n";
                }
               
                else
                {
                    strDebug += "A_上爬不足 "+  ":" + "检测" + "\n";
                }
                #endregion

                #region
                 ErrorIsOn = A_字码个数;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_字码个数" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_字码个数" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_字码匹配失败;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_字码匹配失败" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_字码匹配失败" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_标记断线;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_标记断线" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_标记断线" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_标记不清;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_标记不清" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_标记不清" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_上爬不足;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_上爬不足" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_上爬不足" + ":" + "检测" + "\n";
                }
                #endregion

                #region
                 ErrorIsOn = A_保护层崩碎;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_保护层崩碎" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_保护层崩碎" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_保护层沾锡;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_保护层沾锡" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_保护层沾锡" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_漏镀;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_漏镀" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_漏镀" + ":" + "检测" + "\n";
                }
                #endregion

                #region
                 ErrorIsOn = A_电阻正面整体角度判定和长宽判定;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_电阻正面整体角度判定和长宽判定" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_电阻正面整体角度判定和长宽判定" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_正面电极面积判定;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_正面电极面积判定" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_正面电极面积判定" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                 ErrorIsOn = A_正面电极长宽判定;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_正面电极长宽判定" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_正面电极长宽判定" + ":" + "检测" + "\n";
                }
                #endregion

              

                #region
                ErrorIsOn = A_电阻背面整体角度判定和长宽判定;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_电阻背面整体角度判定和长宽判定" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_电阻背面整体角度判定和长宽判定" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                ErrorIsOn = A_背面电极面积判定;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_背面电极面积判定" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_背面电极面积判定" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                ErrorIsOn = A_背面电极长宽判定;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_背面电极长宽判定" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_背面电极长宽判定" + ":" + "检测" + "\n";
                }
                #endregion

                #region
                ErrorIsOn = A_瓷体挂锡;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_瓷体挂锡" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_瓷体挂锡" + ":" + "检测" + "\n";
                }
                #endregion
                #region
                ErrorIsOn = A_瓷体脏片;
                if (ErrorIsOn == 0)
                {
                    strDebug += "A_瓷体脏片" + ":" + "不检测" + "\n";
                }

                else
                {
                    strDebug += "A_瓷体脏片" + ":" + "检测" + "\n";
                }
                #endregion


                strMessage = DebugPrint(strDebug, is_Debug);

                #endregion


                HTuple Length2DDD_Max, Length2DDD_Min, Length2DDD_X, Length2DDD_X_Std;






                #region 2020-04-14  将参数写入文档
                //如果文件不存在，则创建；存在则覆盖
                //该方法写入字符数组换行显示           
                //System.IO.File.WriteAllLines(@"C:\testDir\test.txt", strUserParam, Encoding.UTF8);
                #endregion


                #endregion




                #region****保存所有图片，不区分正反面

                //图片保存路径            
                string ImageSavePathFive = @"D:\六面机检测程序内部图片保存\五号工位\";
                string ImageSavePathSix = @"D:\六面机检测程序内部图片保存\六号工位\";
                string ImageSaveFormat;

                if (iSaveImg == 1 | iSaveImg == 2 )
                {
                    #region 图片格式判断
                    switch (iSaveImg)//判断保存图片的格式：1-jpeg，2-bmp
                    {
                        case 2:
                             ImageSaveFormat = "bmp";
                            break;                    
                        default:
                             ImageSaveFormat = "jpeg";
                            break;

                    }//判断保存图片的格式
                    #endregion

                    #region//图片保存，如果文件夹不存在则自动创建文件夹
                    if (iProductCode == 5) //五号相机
                    {
                        if (!System.IO.Directory.Exists(ImageSavePathFive))
                        {
                            System.IO.Directory.CreateDirectory(ImageSavePathFive);
                        }

                        Number555 = Number555 + 1;
                        //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        hv_num4 = ImageSavePathFive + DateTime.Now.Ticks.ToString() + "-" + Number555.ToString() + "." + ImageSaveFormat;
                       
                        HOperatorSet.WriteImage(hoImage, ImageSaveFormat, 0, hv_num4);
                    }

                    if (iProductCode == 6) //六号相机
                    {
                        if (!System.IO.Directory.Exists(ImageSavePathSix))
                        {
                            System.IO.Directory.CreateDirectory(ImageSavePathSix);
                        }

                        Number666 = Number666 + 1;
                        //dhDll.frmMsg.Log("背导ok" + iProductCode.ToString() + "," + iProductCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                        hv_num4 = ImageSavePathSix + DateTime.Now.Ticks.ToString() + "-" + Number666.ToString() + "."+ ImageSaveFormat;
                        HOperatorSet.WriteImage(hoImage, ImageSaveFormat, 0, hv_num4);
                    }                                       
                    #endregion  
                }

                #endregion

                #region  调试模式-保存图片选项
                if (is_Debug)
                {
                    strDebug += "检测程序内部保存图片选项：";
                    if (iSaveImg == 1)
                    {
                        strDebug += "保存Jpeg格式";
                    }
                    else if (iSaveImg == 2)
                    {
                        strDebug += "保存Bmp格式";
                    }
                    else
                    {
                        strDebug += "不保存";
                    }

                    strMessage = DebugPrint(strDebug, is_Debug);
                    //HOperatorSet.Connection(ho_RegionDetection, out hoRegionsConn);
                    //syShowRegionBorder(hoRegionsConn, ref listObj2Draw, "OK");
                    //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                }
                #endregion



                #region 变量声明
                HTuple hv_NGCode;
                HTuple iAreaDiff = 64000; //左右面积最大差异8000


                HTuple hv_Rectangularity;//判断背面电极矩形度
                HTuple hv_Width1, hv_Height1;
                HTuple hv_Row1, hv_Column1;

                HTuple hv_Phi, hv_Length1_std, hv_Length2_std, Deg, hv_Row;
                HTuple hv_Column;
                HTuple hv_Length1, hv_Length2, hv_Deg;
                HTuple hv_UsedThreshold;

                HObject ho_RegionClosing1, ho_RegionBinary;
                HObject ho_RegionErr, ho_SelectedRegions, ho_RegionUnion;
                HObject ho_Region_ZiMa;
                HObject ho_RegionErr1, ho_RegionErr2, ho_RegionErosion;



                HObject ho_Region_cudingwei, ho_Rectangle1_wudingwei, ho_Rectangle2_wudingwei;
                HObject ho_Region_citi, ho_Region_dianji;
                HObject hoRegionsConn2;
                HObject ho_RegionRect2_dianji_zhengmian, ho_RegionTrans_dianji_zhengmian;
                HObject ho_RegionErr3, ho_Region_return;

                #endregion

                //获取图像尺寸，用于检测五六号相机电阻是否靠近图像边缘
                HOperatorSet.GetImageSize(hoImage, out hv_Width1, out hv_Height1);

                #region***iProductCode-区分5、6号相机
                if (iProductCode == 5)
                {
                    hv_m = hv_m + 1;

                }
                else if (iProductCode == 6)
                {
                    hv_n = hv_n + 1;
                }
                #endregion


                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;
                }
                #endregion



                //判断彩色还是黑白
                #region****判断彩色还是黑白，彩色图像二值化
                HOperatorSet.CountChannels(hoReduced, out NChannel);
                if (NChannel == 3) //三通道彩色
                {
                    //HOperatorSet.Decompose3(hoReduced, out ho_Image1, out ho_ImageReduced, out ho_Image3); //hoReduced 转换到 ho_ImageReduced
                    HOperatorSet.Rgb1ToGray(hoReduced, out ho_GrayImage);

                }
                else  //单通道黑白
                {
                    HOperatorSet.CopyObj(hoReduced, out ho_GrayImage, 1, 1);  //hoReduced 复制到 ho_ImageReduced
                }
                #endregion

                #region --- ***无定位检测 *** ---
                //开始检测 ho_GrayImage
                HTuple hv_iFixThres = iFixThres;
                HTuple hv_iArea1 = hv_iArea_heitu;   //500;  //检测是否为黑图的面积阈值，如果图像中符合特定灰度值的区域像素小于该值，判定无定位
                HTuple hv_iOpeWidth1 = 5;//粗定位，开运算宽度阈值
                HTuple hv_iOpeHeight = 10;//粗定位，开运算高度阈值
                HTuple hv_iAngleScale = iAngleScale;
                HTuple hv_iLength1 = iLength1;
                HTuple hv_iLength2 = iLength2;
                HTuple hv_iLength1Scale = iLength1Scale;
                HTuple hv_iLength2Scale = iLength2Scale;
                HTuple hv_iRowMin = iBorderScale;
                HTuple hv_iArea2 = hv_iArea_cudingwei;// 8000;//粗定位，用于筛选电极，确定电极数量，判断正反面
                HTuple hv_ipix = ipix;




               

                #region ---- *** 无定位 *** ----
                wudingwei(ho_GrayImage, out ho_Region_cudingwei, out ho_Rectangle1_wudingwei,
                          out ho_Rectangle2_wudingwei, hv_ipix, hv_iFixThres, hv_iArea1,
                          hv_iLength1, hv_iLength2, hv_iLength1Scale, hv_iLength2Scale,
                          hv_iAngleScale, hv_iOpeWidth1, hv_iOpeHeight, hv_iRowMin,
                          hv_iArea2, out hv_NGCode, out hv_Deg, out hv_Length1,
                          out hv_Length2);

                #region 程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                    dhDll.frmMsg.Log("程序出错-无定位检测" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    return listObj2Draw;
                }
                #endregion

                //if (is_Debug)
                //{
                //    strDebug += "(1)总定位相关参数:" + "\n";
                //    strDebug += "总定位阈值:" + iMainThres.ToString() + "\n";
                //    strDebug += "总定位开运算:" + iMainOpening.ToString() + "\n";
                //    strDebug += "总定位闭运算:" + iMainClosing.ToString() + "\n";
                //    strDebug += "电极过滤面积:" + iMainFilterArea.ToString() + "\n";

                //    if (hv_Number != 0)
                //    {
                //        HTuple Rowtmp, Coltmp, Areatmp;
                //        HOperatorSet.AreaCenter(ho_SelectedRegions, out Areatmp, out Rowtmp, out Coltmp);
                //        strDebug += "当前识别电极面积:" + Areatmp.TupleSelect(0).D.ToString("0.0") + "\n";
                //    }

                //    strDebug += "当前识别电极个数:" + hv_Number.D.ToString() + "\n";
                //    strDebug += "\n";
                //}




                if ((int)(new HTuple(hv_NGCode.TupleEqual(5))) != 0)
                {
                    #region***无定位-黑图
                    listObj2Draw[1] = "NG-无定位"; ;
                    syShowRegionBorder(hoUnion, ref listObj2Draw, "NG");  //显示搜索边界
                    //输出NG详情
                    lsInfo2Draw.Add("黑图");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;

                    #endregion
                }


                if ((int)(new HTuple(hv_NGCode.TupleEqual(6))) != 0)
                {
                    #region***判断产品角度，歪斜过大直接无定位 正负10度
                    listObj2Draw[1] = "NG-无定位"; ;
                    syShowRegionBorder(ho_Rectangle2_wudingwei, ref listObj2Draw, "NG");
                    //输出NG详情                   
                    lsInfo2Draw.Add("最大歪斜角度:" + hv_iAngleScale.D.ToString("0.0") + " 度");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("歪斜角度:" + hv_Deg.D.ToString("0.0") + " 度");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                if ((int)(new HTuple(hv_NGCode.TupleEqual(7))) != 0)
                {
                    #region****检测电阻整体长宽尺寸
                    listObj2Draw[1] = "NG-无定位";
                    syShowRegionBorder(ho_Rectangle2_wudingwei, ref listObj2Draw, "NG");
                    //输出NG详情
                    lsInfo2Draw.Add("宽度异常");
                    lsInfo2Draw.Add("NG");
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + " um*" + iLength2.ToString() + "um");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " um * " + hv_Length2.D.ToString("0.0") + " um");
                    lsInfo2Draw.Add("NG");

                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }


                if ((int)(new HTuple(hv_NGCode.TupleEqual(8))) != 0)
                {
                    #region****检测电阻整体长宽尺寸
                    listObj2Draw[1] = "NG-无定位";
                    syShowRegionBorder(ho_Rectangle2_wudingwei, ref listObj2Draw, "NG");
                    //输出NG详情
                    lsInfo2Draw.Add("高度异常");
                    lsInfo2Draw.Add("NG");
                    lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + " um*" + iLength2.ToString() + "um");
                    lsInfo2Draw.Add("OK");
                    lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " um * " + hv_Length2.D.ToString("0.0") + " um");
                    lsInfo2Draw.Add("NG");

                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                if ((int)(new HTuple(hv_NGCode.TupleEqual(9))) != 0)
                {
                    #region****无定位-电阻靠近图像边缘
                    listObj2Draw[1] = "NG-无定位";//电阻靠近图像边缘                                             
                    syShowRegionBorder(ho_Rectangle1_wudingwei, ref listObj2Draw, "NG");
                    //输出NG详情

                    lsInfo2Draw.Add("电阻靠近图像边缘");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;
                    #endregion
                }

                #endregion

                #endregion
             

                HOperatorSet.CountObj(ho_Region_cudingwei, out hv_Num);
                


                #region ---- *** 超时处理  *** ----

                if (sw.ElapsedMilliseconds > iTimeout)
                {
                    sw.Stop();
                    listObj2Draw[1] = "NG-超时";
                    dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                    return listObj2Draw;

                }
                #endregion

                #region ****设置正背面缺陷不检，但保留无定位检测
                if (hv_leixing == 2)
                {
                    listObj2Draw[1] = "OK";
                    lsInfo2Draw.Add("正背面不检");
                    lsInfo2Draw.Add("NG");
                    listObj2Draw.Add("字符串");
                    listObj2Draw.Add(lsInfo2Draw);
                    listObj2Draw.Add(new PointF(1800, 100));
                    return listObj2Draw;

                }
                #endregion
                          


                if (hv_Num == 1) //背导
                {
                    #region---- *** 背导朝上 *** ----

                   


                    #region ---- *** 背导朝上-尺寸不符检测 *** ----               
                    HOperatorSet.OpeningRectangle1(ho_Region_cudingwei, out hoSelectedRegions1, 30, 5);
                    HOperatorSet.Union1(hoSelectedRegions1, out hoRegion);
                    HOperatorSet.SmallestRectangle2(hoRegion, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);


                    #region ****背导电极提取、尺寸判断、面积判断                                 
                    HTuple hv_Parameter_BM = new HTuple();
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_AutoThreshold1);
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_KirschThr);
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_KirschClosing);
                    hv_Parameter_BM = hv_Parameter_BM.TupleConcat(hv_KirschOpening);

                    dianji_beimian(ho_GrayImage, out ho_Region_dianji, out ho_Region_citi, hv_Parameter_BM);

                    #region 调试模式
                    if (is_Debug)
                    {
                        //显示瓷体区域
                        syShowRegionBorder(ho_Region_citi, ref listObj2Draw, "NG");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    }

                    #endregion



                    HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                    if (hv_Num != 2)
                    {
                        #region*** 两电极提取失败-尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.Connection(ho_Region_dianji, out ho_ConnectedRegions);
                        HOperatorSet.CountObj(ho_ConnectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_ConnectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }

                        //输出NG详情

                        lsInfo2Draw.Add("背导-尺寸-电极提取失败" + hv_Num);
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    HOperatorSet.SortRegion(ho_Region_dianji, out ho_SortedRegions, "first_point", "true", "column");
                    HOperatorSet.AreaCenter(ho_SortedRegions, out hv_Area, out hv_Row, out hv_Column);
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out hv_Row1, out hv_Column1, out hv_Phi, out hv_Length1_1, out hv_Length2_2);


                    //判断电极矩形度                               
                    HOperatorSet.Rectangularity(ho_SortedRegions, out hv_Rectangularity);
                    if ((int)((new HTuple(((hv_Rectangularity.TupleSelect(0))).TupleLess(Rectangularity1))).TupleOr(
                        new HTuple(((hv_Rectangularity.TupleSelect(1))).TupleLess(Rectangularity1)))) != 0)
                    {
                        #region***电极矩形度不符，判定尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符";
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("矩形度下限：" + Rectangularity1);
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前矩形度：" + hv_Rectangularity.TupleSelect(0) + "," + hv_Rectangularity.TupleSelect(1));

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    //平均宽度测量 
                    #region****平均宽度测量                 
                    HTuple hv_width;
                    hv_width = (hv_Area / (hv_Length1_1 * 2)) / 2;
                    Length2DDD = hv_width * 2 * ipix * 1000; //像素长度转换为实际距离
                    hv_width = Length2DDD;


                    //平均高度测量
                    HTuple hv_height;
                    hv_height = (hv_Area / (hv_Length2_2 * 2)) / 2;


                    Length1DDD = hv_height * 2 * ipix * 1000; //像素长度转换为实际距离
                    hv_height = Length1DDD;



                 

                    if (A_背面电极长宽判定 == 0) goto A_背面电极长宽判定END;

                    if ((Length2DDD.TupleSelect(0) < hv_iBackDianji_Width_Min) || (Length2DDD.TupleSelect(1) < hv_iBackDianji_Width_Min))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过小
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情

                        lsInfo2Draw.Add("平均-电极宽度上下限：" + hv_iBackDianji_Width_Min + "-" + hv_iBackDianji_Width_Max + "um ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("1当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((Length2DDD.TupleSelect(0) > hv_iBackDianji_Width_Max) || (Length2DDD.TupleSelect(1) > hv_iBackDianji_Width_Max))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情

                        lsInfo2Draw.Add("平均-电极宽度上下限：" + hv_iBackDianji_Width_Min + "-" + hv_iBackDianji_Width_Max + "um ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("2当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length2DDD.TupleSelect(0) - Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4diff_beidianji))
                    {
                        #region*** 两电极宽度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("平均-电极宽度最大差值：" + Ilenth4diff_beidianji + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("3当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D - Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length2DDD.TupleSelect(0) + Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4Sum_beidianji))
                    {
                        #region*** 两电极宽度Sum值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("平均-电极宽度最大Sum值：" + Ilenth4Sum_beidianji + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前Sum值：" + Math.Abs(Length2DDD.TupleSelect(0).D + Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    HOperatorSet.TupleMax(Length2DDD, out Length2DDD_Max);
                    HOperatorSet.TupleMin(Length2DDD, out Length2DDD_Min);
                    Length2DDD_X = Length2DDD_Max / Length2DDD_Min;

                    if (Length2DDD_X > Length2DDD_beimian_Std)
                    {
                        #region***两电极宽度比值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("左右电极宽度比值：" + Length2DDD_beimian_Std);
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前比值：" + Length2DDD_X);
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                  


                A_背面电极长宽判定END:
                    #endregion


                    //电极面积判定
                    #region***电极面积判定
                    //检查导体面积、尺寸
                    HOperatorSet.AreaCenter(ho_Region_dianji, out hv_Area, out hv_Row, out hv_Column);
                    //hv_Area = hv_Area * ipix2;

                   

                    if (A_背面电极面积判定 == 0) goto A_背面电极面积判定END;
                    if ((hv_Area.TupleSelect(0) < iSmallestArea_beidianji) || (hv_Area.TupleSelect(1) < iSmallestArea_beidianji))    //面积小于iSmallestArea = 6000
                    {
                        #region***判断电极面积是否过小，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";//"NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积下限：" + iSmallestArea_beidianji.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea_beidianji) || (hv_Area.TupleSelect(1) > iBiggstArea_beidianji))      //面积大于iBiggstArea = 17000
                    {
                        #region***判断电极面积是否过大，如果是则溅射深
                        listObj2Draw[1] = "NG-溅射深"; //"NG-电极面积过大"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积上限：" + iBiggstArea_beidianji.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于iAreaDiff = 8000
                    {
                        #region***判断两电极面积差值是否过大，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";// "NG-电极大小端";  //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "um^2 ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    A_背面电极面积判定END:
                    #endregion

                    //rect-长宽判定
                    #region****电极长宽判定


                    //导体长边不能小于
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                    HOperatorSet.GenRectangle2(out ho_Rects, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);


                    Length1DDD = hv_height; //像素长度转换为实际距离
                    Length2DDD = hv_Length2_2 * 2 * ipix * 1000; //像素长度转换为实际距离       



                   

                    if (A_背面电极长宽判定 == 0) goto A_背面电极长宽Rect判定END;
                    if ((Length1DDD.TupleSelect(0) < hv_iBackDianji_Height_Min) || (Length1DDD.TupleSelect(1) < hv_iBackDianji_Height_Min))

                    {
                        #region  ****电极长度波动幅度-电极长度过小
                        //if ((Length1DDD.TupleSelect(0) < Iwave2 * Ilenth2) || (Length1DDD.TupleSelect(1) < Iwave2 * Ilenth2)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length1DDD.TupleSelect(0) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0 || (int)(new HTuple(((((Length1DDD.TupleSelect(1) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0)
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + hv_iBackDianji_Height_Min + "-" + hv_iBackDianji_Height_Max + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }



                    if ((Length1DDD.TupleSelect(0) > hv_iBackDianji_Height_Max) || (Length1DDD.TupleSelect(1) > hv_iBackDianji_Height_Max))

                    {
                        #region  ****电极长度波动幅度-电极长度过大
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + hv_iBackDianji_Height_Min + "-" + hv_iBackDianji_Height_Max + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length1DDD.TupleSelect(0) - Length1DDD.TupleSelect(1)).TupleAbs() > Ilenth3diff_beimian))
                    {
                        #region***两电极长度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("c电极长度最大差值：" + Ilenth3diff_beimian + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length1DDD.TupleSelect(0).D - Length1DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }



                    #region  ****电极宽度波动幅度-电极宽度过小
                    //if ((Length2DDD.TupleSelect(0) < (Ilenth3 * (1 - Iwave3))) || (Length2DDD.TupleSelect(1) < (Ilenth3 * (1 - Iwave3))))
                    //{
                    //    
                    //    //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                    //    //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)

                    //    listObj2Draw[1] = "NG-上爬不足";//"NG-电极宽度过窄";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("2电极宽度上下限：" + ((1 - Iwave3) * Ilenth3) + "-" + ((1 + Iwave3) * Ilenth3) + "um ");

                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //   
                    //}
                    #endregion
                    #region  ****电极宽度波动幅度-电极宽度过大
                    //if ((Length2DDD.TupleSelect(0) > (Ilenth3 * (1 + Iwave3))) || (Length2DDD.TupleSelect(1) > (Ilenth3 * (1 + Iwave3))))
                    //{
                    //   
                    //    //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                    //    //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                    //    listObj2Draw[1] = "NG-溅射深";//"NG-电极宽度过窄";
                    //    hv_Num = 0;
                    //    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                    //    for (int i = 1; i <= hv_Num; i++)
                    //    {
                    //        HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                    //        syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                    //    }
                    //    //输出NG详情
                    //    lsInfo2Draw.Add("2电极宽度上下限：" + ((1 - Iwave3) * Ilenth3) + "-" + ((1 + Iwave3) * Ilenth3) + "um ");

                    //    lsInfo2Draw.Add("OK");
                    //    lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                    //    lsInfo2Draw.Add("NG");
                    //    listObj2Draw.Add("字符串");
                    //    listObj2Draw.Add(lsInfo2Draw);
                    //    listObj2Draw.Add(new PointF(1800, 100));
                    //    return listObj2Draw;
                    //   
                    //}
                    #endregion


                    //电极延锡判断（矩形宽度-平均宽度）
                    if ((Length2DDD.TupleSelect(0) > (hv_width.TupleSelect(0) + (iWidthDiff))))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                        listObj2Draw[1] = "NG-背面延锡";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + (hv_width.TupleSelect(0) - (iWidthDiff)) + "-" + (hv_width.TupleSelect(0) + (iWidthDiff)) + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("延锡-当前矩形宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        lsInfo2Draw.Add("延锡-当前平均宽度：" + hv_width.TupleSelect(0).D.ToString("0.0") + "um ," + hv_width.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((Length2DDD.TupleSelect(1) > (hv_width.TupleSelect(1) + (iWidthDiff))))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                        listObj2Draw[1] = "NG-背面延锡";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + (hv_width.TupleSelect(1) - (iWidthDiff)) + "-" + (hv_width.TupleSelect(1) + (iWidthDiff)) + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("延锡-当前矩形宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        lsInfo2Draw.Add("延锡-当前平均宽度：" + hv_width.TupleSelect(0).D.ToString("0.0") + "um ," + hv_width.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }




                    //int Ilenth4Sum_beidianji = int.Parse(strUserParam[28]);// 背电极宽度和
                    //int Ilenth4diff_beidianji = int.Parse(strUserParam[29]);// 背电极宽度差

                    if (((Length2DDD.TupleSelect(0) - Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4diff_beidianji))
                    {
                        #region***两电极宽度差值
                        listObj2Draw[1] = "NG-上爬不足"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(ho_Region_dianji, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(ho_Region_dianji, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("k电极宽度最大差值：" + Ilenth4diff_beidianji + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D - Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    A_背面电极长宽Rect判定END:

                    #endregion


                    //#region 调试模式
                   

                    //if (is_Debug)
                    //{
                    //    strDebug += "尺寸参数: \n";
                        
                            
                    //    strDebug += "背面参数\n";
                    //    strDebug += "背面-电极数量:" + bElectrodeNum.ToString() + "\n";
                  
                    //    strDebug += "背面-整体长度:" + bWholeLength.ToString() + "\n";
                    //    strDebug += "背面-整体宽度:" + bWholeWidth.ToString() + "\n";

                    //    strDebug += "背面-瓷体长度:" + bCeramicsLength.ToString() + "\n";
                    //    strDebug += "背面-瓷体宽度:" + bCeramicsWidth.ToString() + "\n";

                    //    strDebug += "背面电极-左电极长度:" + bLeftElectrodeLength.ToString() + "\n";
                    //    strDebug += "背面电极-左电极宽度:" + iLeftElectrodeWidth.ToString() + "\n";
                       
                    //    strDebug += "背面电极-右电极长度:" + bRightElectrodeLength.ToString() + "\n";
                    //    strDebug += "背面电极-右电极宽度:" + bRightElectrodeLength.ToString() + "\n";

                       

                    //}


                    //strMessage = DebugPrint(strDebug, is_Debug);
                    //#endregion



                    #endregion


                    #endregion

                    #region ---  *** 背导朝上-缺陷检测 *** ---                  

                    #region ***检查全局黑点***
                    HObject ho_ErrRegion;
                    HTuple hv_iGlobalWidth, hv_iGlobalHeight, hv_iGlobalOpen, hv_iGlobalArea;

                    hv_iGlobalWidth = 8;
                    hv_iGlobalHeight = 8;
                    //hv_iGlobalThr = 80;
                    hv_iGlobalOpen = 5;
                    hv_iGlobalArea = iBlackArea1;//400

                    HTuple hv_Parameter_GB = new HTuple();
                    hv_Parameter_GB = hv_Parameter_GB.TupleConcat(hv_iGlobalWidth);
                    hv_Parameter_GB = hv_Parameter_GB.TupleConcat(hv_iGlobalHeight);
                    hv_Parameter_GB = hv_Parameter_GB.TupleConcat(hv_iGlobalThr);
                    hv_Parameter_GB = hv_Parameter_GB.TupleConcat(hv_iGlobalOpen);
                    hv_Parameter_GB = hv_Parameter_GB.TupleConcat(hv_iGlobalArea);


                    global_contamination(ho_GrayImage, hoSelectedRegions1, out ho_ErrRegion, hv_Parameter_GB, out hv_NGCode);




                    #region ***程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion

                    if ((int)(new HTuple(hv_NGCode.TupleEqual(33))) != 0)

                    {
                        #region*** NG-产品沾污-电极-黑色沾污
                        ho_RegionErr = ho_ErrRegion;
                        HOperatorSet.Union1(ho_RegionErr, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-产品沾污";

                        HOperatorSet.Connection(ho_RegionErr, out ho_SelectedRegions);
                        syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("全局-黑色沾污-最大面积：" + iBlackArea1.ToString("0.0") + "pix");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }

                    #endregion

                    #region ***检查电极黑点***


                    HObject ho_BlackERR2;
                    HTuple hv_iBlackOpen2, hv_iBlackArea2;

                    //hv_iBlackThr2 = 90;
                    hv_iBlackOpen2 = 5;
                    hv_iBlackArea2 = iBlackArea2;

                    HTuple hv_Parameter_DJ = new HTuple();
                    hv_Parameter_DJ = hv_Parameter_DJ.TupleConcat(hv_iBlackThr2);
                    hv_Parameter_DJ = hv_Parameter_DJ.TupleConcat(hv_iBlackOpen2);
                    hv_Parameter_DJ = hv_Parameter_DJ.TupleConcat(hv_iBlackArea2);


                    dianji_heidian(ho_GrayImage, ho_Region_dianji, out ho_BlackERR2, hv_Parameter_DJ, out hv_NGCode);

                    #region ***程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion

                    if ((int)(new HTuple(hv_NGCode.TupleEqual(32))) != 0)
                    {
                        #region*** NG-产品沾污-电极-黑色沾污
                        ho_RegionErr = ho_BlackERR2;
                        HOperatorSet.Union1(ho_RegionErr, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-产品沾污";

                        HOperatorSet.Connection(ho_RegionErr, out ho_SelectedRegions);
                        syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("电极-黑色沾污-缺陷最大面积：" + hv_iBlackArea2 + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }

                    #endregion



                    #region  ***瓷体区域黑点+瓷体沾锡检测
                    HObject ho_BlackErr, ho_WhiteErr;
                    HTuple hv_iCitiWidth, hv_iCitiHeight, hv_iBlackOpen;
                    //HTuple hv_iWhiteWidth, hv_iWhiteExp;

                    hv_iCitiWidth = 30;
                    hv_iCitiHeight = 20;
                    //hv_iBlackThr = 90;
                    hv_iBlackOpen = 5;
                    //hv_iBlackArea = ;
                    //hv_iWhiteWidth = 10;
                    //hv_iWhiteExp = 10;
                    //hv_iWhiteClosing = 3.5;
                    //hv_iWhiteArea = iBlackArea4;


                    HTuple hv_Parameter_CT = new HTuple();
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iCitiWidth);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iCitiHeight);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iBlackThr);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iBlackOpen);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iBlackArea);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iWhiteWidth);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iWhiteExp);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iWhiteClosing);
                    hv_Parameter_CT = hv_Parameter_CT.TupleConcat(hv_iWhiteArea);

                    citi_zhangwu(ho_GrayImage, ho_Region_citi, out ho_BlackErr, out ho_WhiteErr, hv_Parameter_CT, out hv_NGCode);

                    #region ***程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion

                    
                    if (A_瓷体脏片 == 0) goto A_瓷体脏片END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(30))) != 0)
                    {
                        #region NG-产品沾污-瓷体-黑色沾污
                        ho_RegionErr = ho_BlackErr;
                        HOperatorSet.Union1(ho_RegionErr, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-产品沾污";

                        HOperatorSet.Connection(ho_RegionErr, out ho_SelectedRegions);
                        syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("瓷体-黑色沾污-缺陷最大面积：" + hv_iBlackArea + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_瓷体脏片END:

                    if (A_瓷体挂锡 == 0) goto A_瓷体挂锡END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(31))) != 0)
                    {
                        #region NG-产品沾污-瓷体-沾锡
                        ho_RegionErr = ho_WhiteErr;
                        HOperatorSet.Union1(ho_RegionErr, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-产品沾污";

                        HOperatorSet.Connection(ho_RegionErr, out ho_SelectedRegions);
                        syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("瓷体-沾锡-缺陷最大面积：" + hv_iWhiteArea + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_瓷体挂锡END:

                    #endregion


                    #endregion

                    #region ***漏镀缺陷检测



                    HTuple hv_Parameter_LD = new HTuple();
                    HTuple hv_loudu_Mean = new HTuple();
                    HTuple hv_loudu_div = new HTuple();
                    HTuple mean_MK = new HTuple();


                    hv_Parameter_LD = hv_Parameter_LD.TupleConcat(hv_iloudu_mean);
                    hv_Parameter_LD = hv_Parameter_LD.TupleConcat(hv_iloudu_divstd);



                    loudu_2(hoReduced, ho_Region_dianji, ho_Region_citi, hv_Parameter_LD, out hv_NGCode, out hv_loudu_Mean, out hv_loudu_div);


                    #region ***程序出错
                  
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
             
                    #endregion

                    #region 调试模式
                    if (is_Debug)
                    {
                        syShowRegionBorder(ho_Region_dianji, ref listObj2Draw, "NG");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        if (is_Debug)  //调试状态输出信息
                        {
                            strDebug += "漏镀缺陷检测相关参数：\n";
                            strDebug += "当前工位：56\n";
                            strDebug += "标准电极三通道灰度值:" + hv_iloudu_mean.ToString("0.0") + "\n";
                            strDebug += "当前电极三通道灰度值:" + hv_loudu_Mean.D.ToString("0.0") + "\n";
                            strDebug += "标准电极瓷体灰度差值:" + hv_iloudu_divstd.ToString("0.0") + "\n";
                            strDebug += "当前当前电极瓷体灰度差值:" + hv_loudu_div.D.ToString("0.0") + "\n";                          

                        }


                    }

                    #endregion

                    #region 缺陷显示（loudu_2）

                    if (A_漏镀 == 0) goto A_漏镀END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(40))) != 0)
                    {
                        #region*** NG-产品背电极-漏镀

                        listObj2Draw[1] = "NG-漏镀";
                        syShowRegionBorder(ho_Region_dianji, ref listObj2Draw, "NG");
                        //输出NG详情
                        lsInfo2Draw.Add("标准电极三通道灰度值：" + hv_iloudu_mean.ToString("0.0"));
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前电极三通道灰度值：" + hv_loudu_Mean.D.ToString("0.0"));
                        lsInfo2Draw.Add("NG");
                        lsInfo2Draw.Add("标准电极瓷体灰度差值：" + hv_iloudu_divstd.ToString("0.0"));
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前当前电极瓷体灰度差值：" + hv_loudu_div.D.ToString("0.0"));
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                     A_漏镀END:
                    #endregion





                    #endregion

                    ho_Region_return = ho_Region_dianji;


                    #region ---- *** 超时处理 *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion

                    #region 调试模式
                    if (is_Debug)
                    {
                        //syShowRegionBorder(ho_Region_Duanmian, ref listObj2Draw, "OK");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    }

                    if (is_Debug)
                    {
                        strDebug += "整体尺寸参数";
                    }


                    strMessage = DebugPrint(strDebug, is_Debug);
                    #endregion

                    #endregion
                }// 背导 



                else //正导
                {
                    #region ---- *** 正导朝上  *** ----


                    #region ---- *** 无定位+尺寸检测  *** ----

                    #region ****电极提取
                    //输入：ho_GrayImage
                    //输出：hoSelectedRegions_dianji_zhengmian（电极region）



                    /*2020-04-14
                     * "7"-"max_separability"
                     * 参数异常
                     */
                    HOperatorSet.BinaryThreshold(ho_GrayImage, out ho_Region, "max_separability", "light", out hv_UsedThreshold);
                    HOperatorSet.Threshold(ho_GrayImage, out ho_RegionBinary, hv_UsedThreshold + iFixThres2, 255);
                    HOperatorSet.ClosingRectangle1(ho_RegionBinary, out ho_RegionClosing1, 4, 3);
                    HOperatorSet.Connection(ho_RegionClosing1, out hoRegionsConn);
                    HOperatorSet.SelectShape(hoRegionsConn, out hoRegionsConn2, "area", "and", 3000, 99999);
                    HOperatorSet.SelectShape(hoRegionsConn2, out hoSelectedRegions, "height", "and", 150, 300);
                    HObject hoSelectedRegions_dianji_zhengmian = hoSelectedRegions;
                    HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);


                    if (hv_Num != 2)
                    {
                        #region***电极提取失败-尺寸不符
                        listObj2Draw[1] = "NG-尺寸不符";//"NG-尺寸异常";                                           
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("正导-电极提取失败");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    #endregion

                    #region ****电阻整体角度判定+长宽判定
                    //输入：hoSelectedRegions_dianji_zhengmian
                    //输出：ho_RegionTrans_dianji_zhengmian
                    //输出：ho_RegionRect2_dianji_zhengmian


                    HOperatorSet.Union1(hoSelectedRegions_dianji_zhengmian, out hoRegion);
                    HOperatorSet.ShapeTrans(hoRegion, out ho_RegionTrans, "convex");
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row111, out hv_Column111, out hv_Phi, out hv_Length1_std, out hv_Length2_std);
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row111, hv_Column111, hv_Phi, hv_Length1_std, hv_Length2_std);
                    ho_RegionRect2_dianji_zhengmian = ho_Rectangle;
                    ho_RegionTrans_dianji_zhengmian = ho_RegionTrans;


                    //判断产品角度，歪斜过大直接无定位 正负10度

                    HOperatorSet.TupleDeg(hv_Phi, out Deg);
                    
                    if (A_电阻正面整体角度判定和长宽判定 == 0) goto A_电阻整体角度判定END;
                    if (Deg > iAngleScale || Deg < (0 - iAngleScale))
                    {
                        #region***判断产品角度，歪斜过大直接无定位 正负10度
                        listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                        syShowRegionBorder(ho_Rectangle, ref listObj2Draw, "NG");
                        //输出NG详情
                        lsInfo2Draw.Add("歪斜角度:" + Deg.D.ToString("0.0") + " 度");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    A_电阻整体角度判定END:

                    hv_Length1 = hv_Length1_std * 2 * ipix * 1000; //像素长度转换为实际距离
                    hv_Length2 = hv_Length2_std * 2 * ipix * 1000; //像素长度转换为实际距离

                    //检测电阻长宽尺寸
                    if (A_电阻正面整体角度判定和长宽判定 == 0) goto A_电阻整体长宽判定END;
                    if ((hv_Length1 < (iLength1 - iLength1Scale)) || (hv_Length1 > (iLength1 + iLength1Scale)) || (hv_Length2 < (iLength2 - iLength2Scale)) || (hv_Length2 > (iLength2 + iLength2Scale)))
                    {
                        #region****检测电阻整体长宽尺寸
                        listObj2Draw[1] = "NG-无定位";//尺寸差异较大                
                        syShowRegionBorder(ho_RegionRect2_dianji_zhengmian, ref listObj2Draw, "NG");
                        //输出NG详情
                        lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + " um*" + iLength2.ToString() + "um");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " um * " + hv_Length2.D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    A_电阻整体长宽判定END:

                    #endregion

                    #region****电极面积判定

                    HOperatorSet.AreaCenter(hoSelectedRegions_dianji_zhengmian, out hv_Area, out hv_Row, out hv_Column);

                    if (A_正面电极面积判定 == 0) goto A_正面电极面积判定END;
                    if ((hv_Area.TupleSelect(0) < iSmallestArea_miandianji) || (hv_Area.TupleSelect(1) < iSmallestArea_miandianji))    //面积小于iSmallestArea = 6000
                    {
                        #region***判断电极面积是否过小，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";//"NG-电极异常";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积下限：" + iSmallestArea_miandianji.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");

                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((hv_Area.TupleSelect(0) > iBiggstArea_miandianji) || (hv_Area.TupleSelect(1) > iBiggstArea_miandianji))      //面积大于iBiggstArea = 17000
                    {
                        #region***判断电极面积是否过大，如果是则溅射深
                        listObj2Draw[1] = "NG-溅射深"; //"NG-电极面积过大"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积上限：" + iBiggstArea_miandianji.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.TupleSelect(0).D.ToString("0.0") + "um^2 ," + hv_Area.TupleSelect(1).D.ToString("0.0") + "um^2");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (Math.Abs(hv_Area.TupleSelect(0) - hv_Area.TupleSelect(1)) > iAreaDiff)   //左右面积差异大于iAreaDiff = 8000
                    {
                        #region***判断两电极面积差值是否过大，如果是则上爬不足
                        listObj2Draw[1] = "NG-上爬不足";// "NG-电极大小端";  //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("面积差异最大值：" + iAreaDiff.ToString() + "um^2 ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积差异：" + Math.Abs(hv_Area.TupleSelect(0).D - hv_Area.TupleSelect(1).D).ToString("0.0") + "um^2 ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }
                    A_正面电极面积判定END:

                    #endregion

                    #region****电极长宽判定                   

                    HOperatorSet.SortRegion(hoSelectedRegions_dianji_zhengmian, out ho_SortedRegions, "first_point", "true", "column");
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out RowDDD, out ColDDD, out PhiDDD, out hv_Length1_1, out hv_Length2_2);
                    HOperatorSet.GenRectangle2(out ho_Rects, RowDDD, ColDDD, PhiDDD, hv_Length1_1, hv_Length2_2);


                    Length1DDD = hv_Length1_1 * 2 * ipix * 1000; //像素长度转换为实际距离
                    Length2DDD = hv_Length2_2 * 2 * ipix * 1000; //像素长度转换为实际距离                 



                    #region ****电极长度测量
                    if (A_正面电极长宽判定 == 0) goto A_正面电极长宽判定END;
                    if ((Length1DDD.TupleSelect(0) < hv_iFrontDianji_Height_Min) || (Length1DDD.TupleSelect(1) < hv_iFrontDianji_Height_Min))
                    {
                        #region  ****电极长度波动幅度-电极长度过小
                        //if ((Length1DDD.TupleSelect(0) < Iwave2 * Ilenth2) || (Length1DDD.TupleSelect(1) < Iwave2 * Ilenth2)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length1DDD.TupleSelect(0) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0 || (int)(new HTuple(((((Length1DDD.TupleSelect(1) - Ilenth2)).TupleAbs())).TupleGreater(Ilenth2 * Iwave2))) != 0)
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + hv_iFrontDianji_Height_Min + "-" + hv_iFrontDianji_Height_max + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if ((Length1DDD.TupleSelect(0) > hv_iFrontDianji_Height_max) || (Length1DDD.TupleSelect(1) > hv_iFrontDianji_Height_max))

                    {
                        #region  ****电极长度波动幅度-电极长度过大
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极长度上下限：" + hv_iFrontDianji_Height_Min + "-" + hv_iFrontDianji_Height_max + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前长度：" + Length1DDD.TupleSelect(0).D.ToString("0.0") + "um ," + Length1DDD.TupleSelect(1).D.ToString("0.0") + " um");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    if (((Length1DDD.TupleSelect(0) - Length1DDD.TupleSelect(1)).TupleAbs() > Ilenth3diff_zhengmian))
                    {
                        #region***两电极长度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("c电极长度最大差值：" + Ilenth3diff_zhengmian + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length1DDD.TupleSelect(0).D - Length1DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                    #endregion


                    #region ****电极宽度测量

                    if ((Length2DDD.TupleSelect(0) < hv_iFrontDianji_Width_Min) || (Length2DDD.TupleSelect(1) < hv_iFrontDianji_Width_Min))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过小
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)

                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + hv_iFrontDianji_Width_Min + "-" + hv_iFrontDianji_Width_max + "pix ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    if ((Length2DDD.TupleSelect(0) > hv_iFrontDianji_Width_max) || (Length2DDD.TupleSelect(1) > hv_iFrontDianji_Width_max))
                    {
                        #region  ****电极宽度波动幅度-电极宽度过大
                        //if ((Length2DDD.TupleSelect(0) < Iwave3 * Ilenth3) || (Length1DDD.TupleSelect(1) < Iwave3 * Ilenth3)) //电极长边不能小于0.75 *85
                        //if ((int)(new HTuple(((((Length2DDD.TupleSelect(0) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0 || (int)(new HTuple(((((Length2DDD.TupleSelect(1) - Ilenth3)).TupleAbs())).TupleGreater(Ilenth3 * Iwave3))) != 0)
                        listObj2Draw[1] = "NG-电极不符";//"NG-电极宽度过窄";
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("2电极宽度上下限：" + hv_iFrontDianji_Width_Min + "-" + hv_iFrontDianji_Width_max + "pix ");

                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前宽度：" + Length2DDD.TupleSelect(0).D.ToString("0.0") + "pix ," + Length2DDD.TupleSelect(1).D.ToString("0.0") + " pix");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }





                    if (((Length2DDD.TupleSelect(0) + Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4Sum_miandianji))
                    {
                        #region***两电极宽度和
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("k电极宽度最大和：" + Ilenth4Sum_miandianji + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D + Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                    if (((Length2DDD.TupleSelect(0) - Length2DDD.TupleSelect(1)).TupleAbs() > Ilenth4diff_miandianji))
                    {
                        #region***两电极宽度差值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("k电极宽度最大差值：" + Ilenth4diff_miandianji + "um ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前差值：" + Math.Abs(Length2DDD.TupleSelect(0).D - Length2DDD.TupleSelect(1).D).ToString("0.0") + "um ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }


                  

                    HOperatorSet.TupleMax(Length2DDD, out Length2DDD_Max);                
                    HOperatorSet.TupleMin(Length2DDD, out Length2DDD_Min);
                    Length2DDD_X = Length2DDD_Max / Length2DDD_Min;

                    if (  Length2DDD_X > Length2DDD_zhengmian_Std)
                    {
                        #region***两电极宽度比值
                        listObj2Draw[1] = "NG-电极不符"; //"NG-电极异常"
                        hv_Num = 0;
                        HOperatorSet.CountObj(hoSelectedRegions, out hv_Num);
                        for (int i = 1; i <= hv_Num; i++)
                        {
                            HOperatorSet.SelectObj(hoSelectedRegions, out ho_RegionSel, i);
                            syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "NG");
                        }
                        //输出NG详情
                        lsInfo2Draw.Add("左右电极宽度比值：" + Length2DDD_zhengmian_Std);
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前比值：" + Length2DDD_X);
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;
                        #endregion
                    }

                 


                A_正面电极长宽判定END:


                    #endregion


                    #endregion

                    #endregion

                    #region ***上爬不足检测

                    //上爬不足检测函数

                    #region 参数传递
                    HTuple hv_Parameter_SP = new HTuple();
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_leixing);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iShangpa);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iScale_width_2);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iScale_height_2);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iArea_shangpa1);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iScale_height_3);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iScale_width_1);
                    hv_Parameter_SP = hv_Parameter_SP.TupleConcat(hv_iScale_height_1);

                    HObject ho_RegionDetection;
                    #endregion

                    shangpabuzu3(ho_GrayImage, hoSelectedRegions_dianji_zhengmian, out ho_RegionErr, out ho_RegionDetection, hv_Parameter_SP, out hv_NGCode);


                    #region  调试模式
                    if (is_Debug)
                    {
                        HOperatorSet.Connection(ho_RegionDetection, out hoRegionsConn);
                        syShowRegionBorder(hoRegionsConn, ref listObj2Draw, "OK");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    }
                    #endregion

                    #region 程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion



                    #region 缺陷显示
                    if (A_上爬不足 == 0) goto A_上爬不足END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(4))) != 0)
                    {
                        #region
                        HOperatorSet.Union1(ho_RegionErr, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-上爬不足";

                        HOperatorSet.Connection(ho_RegionErr, out ho_SelectedRegions);
                        syShowRegionBorder(ho_SelectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("0,3,4-缺陷最大面积：" + hv_iArea_shangpa1 + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_上爬不足END:
                    #endregion


                    #endregion

                    #region ***字码检测

                    #region 参数传递
                    HTuple hv_iExp_MK = 7;//需要和CreatModelMK函数中的参数对应

                    HTuple hv_Parameter_MK = new HTuple();
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iScore_MK);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iArea_MK1);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iArea_MK2);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(5);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iExp_MK);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_leixing);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iNum_MK);

                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iOpen_MK1);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iOpen_MK2);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iOnorOff);
                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iClosing_duanxian);

                    hv_Parameter_MK = hv_Parameter_MK.TupleConcat(hv_iClosing_ZIMATIQV);
                    #endregion


                    MK(ho_GrayImage, ho_ModelRegion, hoSelectedRegions_dianji_zhengmian,
                       out ho_Region_ZiMa, out ho_RegionErr1, out ho_RegionErr2,
                       out ho_RegionErosion, hv_ModelID, hv_ModelParam, hv_Parameter_MK
                       , out hv_NGCode);


                    #region 调试模式
                    syShowRegionBorder(ho_Region_ZiMa, ref listObj2Draw, "OK");//持续显示

                    if (is_Debug)
                    {
                        //HOperatorSet.Connection(ho_Region_ZiMa, out hoRegionsConn);
                        //syShowRegionBorder(hoRegionsConn, ref listObj2Draw, "OK");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    }
                    #endregion                   

                    #region 程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion

                    #region 缺陷显示（MK）

                    if (A_标记断线 == 0) goto A_标记断线END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(1))) != 0)
                    {
                        #region***标记断线
                        HOperatorSet.AreaCenter(ho_RegionErr1, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-标记断线";
                        HOperatorSet.Connection(ho_RegionErr1, out ho_ConnectedRegions);
                        syShowRegionBorder(ho_ConnectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("标记断线最大面积：" + hv_iArea_MK1 + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_标记断线END:

                    if (A_标记不清 == 0) goto A_标记不清END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(2))) != 0)
                    {
                        #region***标记不清
                        HOperatorSet.AreaCenter(ho_RegionErr2, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-标记不清";
                        HOperatorSet.Connection(ho_RegionErr2, out ho_ConnectedRegions);
                        syShowRegionBorder(ho_ConnectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("标记不清最大面积：" + hv_iArea_MK2 + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_标记不清END:

                    if (A_字码匹配失败 == 0) goto A_字码匹配失败END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(28))) != 0)
                    {
                        #region***字码匹配失败
                        listObj2Draw[1] = "NG-字码匹配失败";
                        HOperatorSet.Connection(ho_Region_ZiMa, out ho_ConnectedRegions);
                        syShowRegionBorder(ho_ConnectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("最小匹配分数：" + hv_iScore_MK + "pix ");
                        lsInfo2Draw.Add("OK");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_字码匹配失败END:

                    if (A_字码个数 == 0) goto A_字码个数END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(29))) != 0)
                    {
                        #region***字码个数不符
                        listObj2Draw[1] = "NG-标记断线";
                        HOperatorSet.Connection(ho_Region_ZiMa, out ho_ConnectedRegions);
                        HOperatorSet.CountObj(ho_ConnectedRegions, out hv_Num);
                        syShowRegionBorder(ho_ConnectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("字码个数不符");
                        lsInfo2Draw.Add("NG");
                        lsInfo2Draw.Add("字码个数：" + hv_Num);
                        lsInfo2Draw.Add("NG");

                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_字码个数END:

                    #endregion

                    #endregion

                    #region ***保护层检测                                  

                    #region 保护层崩碎缺陷检测
                    /*挂锡缺陷检测
                    * 
                    */
                    #region 参数传递
                    HObject ho_RegionDetection_IIG2;

                    HTuple hv_Parameter_IIG_2 = new HTuple();
                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_leixing);
                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_iProtectexp);
                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_iProtectBrokenArea);

                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_iErosionWidth_IIG2);
                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_iErosionHeight_IIG2);

                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_iErrWidth);//保护层崩碎缺陷宽度
                    hv_Parameter_IIG_2 = hv_Parameter_IIG_2.TupleConcat(hv_iErrHeight);//保护层崩碎缺陷高度
                    #endregion

                    baohuceng_bengsui_2(ho_GrayImage, hoSelectedRegions_dianji_zhengmian,
                      ho_Region_ZiMa, out ho_RegionErr3, out ho_RegionDetection_IIG2, hv_Parameter_IIG_2, out hv_NGCode);

                    #region 程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion


                    #region 调试模式
                    if (is_Debug)
                    {
                        HOperatorSet.Connection(ho_RegionDetection_IIG2, out hoRegionsConn);
                        syShowRegionBorder(hoRegionsConn, ref listObj2Draw, "OK");
                        //dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                    }
                    #endregion

                    #region 缺陷显示（baohuceng_bengsui_2）

                    if (A_保护层崩碎 == 0) goto A_保护层崩碎END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(3))) != 0)
                    {
                        #region
                        HOperatorSet.Union1(ho_RegionErr3, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-保护层崩碎";
                        HOperatorSet.Connection(ho_RegionErr3, out ho_ConnectedRegions);
                        syShowRegionBorder(ho_ConnectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("保护层崩碎最大面积：" + hv_iProtectBrokenArea + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_保护层崩碎END:
                    #endregion

                    #region 保护层挂锡缺陷检测
                    /*挂锡缺陷检测
                    * 
                    */
                    #region 参数传递
                    HTuple hv_Parameter_IIG = new HTuple();
                    hv_Parameter_IIG = hv_Parameter_IIG.TupleConcat(hv_leixing);
                    hv_Parameter_IIG = hv_Parameter_IIG.TupleConcat(iProtectBrokenResThres);
                    hv_Parameter_IIG = hv_Parameter_IIG.TupleConcat(iProtectBrokenArea3);

                    HObject ho_RegionDetection_IIG;
                    #endregion

                    baohuceng_bengsui(ho_GrayImage, hoSelectedRegions_dianji_zhengmian,
                    ho_Region_ZiMa, out ho_RegionErr3, out ho_RegionDetection_IIG, hv_Parameter_IIG, out hv_NGCode);

                    #region 程序出错
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                    {
                        listObj2Draw[1] = "NG-程序出错";
                        dhDll.frmMsg.Log("程序出错" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);
                        return listObj2Draw;
                    }
                    #endregion

                    #region 调试模式
                    if (is_Debug)
                    {
                        //HOperatorSet.Connection(ho_RegionDetection_IIG, out hoRegionsConn);
                        //syShowRegionBorder(hoRegionsConn, ref listObj2Draw, "OK");
                        ////dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                    }
                    #endregion

                    #region 缺陷显示（baohuceng_bengsui）
                    if (A_保护层沾锡 == 0) goto A_保护层沾锡END;
                    if ((int)(new HTuple(hv_NGCode.TupleEqual(3))) != 0)
                    {
                        #region
                        HOperatorSet.Union1(ho_RegionErr3, out ho_RegionUnion);
                        HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area, out hv_Row, out hv_Column);
                        listObj2Draw[1] = "NG-保护层崩碎";
                        HOperatorSet.Connection(ho_RegionErr3, out ho_ConnectedRegions);
                        syShowRegionBorder(ho_ConnectedRegions, ref listObj2Draw, "NG");

                        //输出NG详情
                        lsInfo2Draw.Add("(沾锡)保护层崩碎最大面积：" + iProtectBrokenArea3 + "pix ");
                        lsInfo2Draw.Add("OK");
                        lsInfo2Draw.Add("当前面积：" + hv_Area.D.ToString("0.0") + "pix ");
                        lsInfo2Draw.Add("NG");
                        listObj2Draw.Add("字符串");
                        listObj2Draw.Add(lsInfo2Draw);
                        listObj2Draw.Add(new PointF(1800, 100));
                        return listObj2Draw;

                        #endregion
                    }
                    A_保护层沾锡END:
                    #endregion


                    #endregion

                    #region 调试模式
                    if (is_Debug)
                    {
                        //syShowRegionBorder(ho_Region_Duanmian, ref listObj2Draw, "OK");
                        //dhDll.frmMsg.Log("背导ok" + "999999999" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                    }

                    if (is_Debug)
                    {
                        strDebug += "整体尺寸参数";
                    }


                    strMessage = DebugPrint(strDebug, is_Debug);
                    #endregion

                    ho_Region_return = hoSelectedRegions_dianji_zhengmian;

                    #endregion

                    #region ---- *** 超时处理  *** ----

                    if (sw.ElapsedMilliseconds > iTimeout)
                    {
                        sw.Stop();
                        listObj2Draw[1] = "NG-超时"; dhDll.frmMsg.Log("超时311," + sw.ElapsedMilliseconds.ToString(), "", null, dhDll.logDiskMode.Error, 0, true);
                        return listObj2Draw;
                    }
                    #endregion
                    #endregion
                    #endregion                
                }// 正导

                #endregion


                #region  程序出错
                if ((int)(new HTuple(hv_NGCode.TupleEqual(34))) != 0)
                {
                    listObj2Draw[1] = "NG-程序出错";
                   
                    return listObj2Draw;
                }

                #endregion

                //执行到这里，OK 绘制 hoSelectedRegions
                listObj2Draw[1] = "OK";
                hv_Num = 0;
                HOperatorSet.CountObj(ho_Region_return, out hv_Num);
                for (int i = 1; i <= hv_Num; i++)
                {
                    HOperatorSet.SelectObj(ho_Region_return, out ho_RegionSel, i);
                    syShowRegionBorder(ho_RegionSel, ref listObj2Draw, "OK");
                }
                         

                return listObj2Draw;

            }//try
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



        //创建字码模板
        public static void CreateModelMK(HObject ho_Image, out HObject ho_ModelRegion, out HTuple hv_ModelID, out HTuple hv_ModelParam, out HTuple hv_ERR)
        {

            HObject ho_ImageMean = null;
            HObject ho_ExpImage, ho_ImageScaleMax, ho_RegionDynThresh;
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

                    HOperatorSet.CreateShapeModel(ho_Image, "auto", (new HTuple(-180)).TupleRad()
                        , (new HTuple(180)).TupleRad(), "auto", "auto", "use_polarity", "auto",
                        "auto", out hv_ModelID);

                    HOperatorSet.ExpImage(ho_Image, out ho_ExpImage, 7);
                    HOperatorSet.ScaleImageMax(ho_ExpImage, out ho_ImageScaleMax);
                    HOperatorSet.Threshold(ho_ImageScaleMax, out ho_ModelRegion, 128, 255);



                    //HOperatorSet.MeanImage(ho_Image, out ho_ImageMean, 40, 40);
                    //HOperatorSet.DynThreshold(ho_Image, ho_ImageMean, out ho_ModelRegion, 10,
                    //    "light");

                    HObject ho_ConnectedRegions1, ho_SelectedRegions1;
                    HOperatorSet.Connection(ho_ModelRegion, out ho_ConnectedRegions1);

                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1, "area",
                        "and", 100, 99999);

                    HOperatorSet.Union1(ho_SelectedRegions1, out ho_ModelRegion);



                    HOperatorSet.FindShapeModel(ho_Image, hv_ModelID, -((new HTuple(10)).TupleRad()
                        ), (new HTuple(20)).TupleRad(), 0.3, 1, 0, "least_squares", 0, 0.9,
                        out hv_Row111, out hv_Column111, out hv_Angle111, out hv_Score111);
                    hv_ModelParam = new HTuple();
                    hv_ModelParam = hv_ModelParam.TupleConcat(hv_Row111);
                    hv_ModelParam = hv_ModelParam.TupleConcat(hv_Column111);
                    hv_ModelParam = hv_ModelParam.TupleConcat(hv_Angle111);
                    hv_ERR = 0;

                }
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ERR = 1;
                }


                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_ImageMean.Dispose();
                throw HDevExpDefaultException;
            }
        }

        #endregion





        #region ****函数

        #endregion


        #region

        //调试输出内容
        public static string DebugPrint(string Str, bool isDebug)
        {
            if (isDebug)
                return Str;

            else
                return "0";
        }


        public static void Detect_loudu_12(HObject ho_Image, HObject ho_Region_Duanmian, out HObject ho_Region_loudu,
      HTuple hv_parameter, out HTuple hv_NGCode)
        {




            // Local iconic variables 

            HObject ho_Image_R = null, ho_Image_G = null, ho_Image_B = null;
            HObject ho_Duanmian_Erosion1 = null, ho_Reduced_loudu = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions_loudu = null;

            // Local control variables 

            HTuple hv_iloudu_thr = new HTuple(), hv_iloudu_area = new HTuple();
            HTuple hv_iloudu_erosion = new HTuple(), hv_Number_loudu = new HTuple();
            HTuple hv_exc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_loudu);
            HOperatorSet.GenEmptyObj(out ho_Image_R);
            HOperatorSet.GenEmptyObj(out ho_Image_G);
            HOperatorSet.GenEmptyObj(out ho_Image_B);
            HOperatorSet.GenEmptyObj(out ho_Duanmian_Erosion1);
            HOperatorSet.GenEmptyObj(out ho_Reduced_loudu);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions_loudu);
            hv_NGCode = new HTuple();
            try
            {

                try
                {

                    //**漏镀检测
                    hv_iloudu_thr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iloudu_thr = hv_parameter.TupleSelect(
                            18);
                    }
                    hv_iloudu_area.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iloudu_area = hv_parameter.TupleSelect(
                            19);
                    }
                    hv_iloudu_erosion.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iloudu_erosion = hv_parameter.TupleSelect(
                            20);
                    }

                    //***端面漏镀检测
                    ho_Image_R.Dispose(); ho_Image_G.Dispose(); ho_Image_B.Dispose();
                    HOperatorSet.Decompose3(ho_Image, out ho_Image_R, out ho_Image_G, out ho_Image_B
                        );
                    //intensity (Region_Duanmian, Image_R, Mean, Deviation)

                    ho_Duanmian_Erosion1.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Region_Duanmian, out ho_Duanmian_Erosion1,
                        hv_iloudu_erosion, hv_iloudu_erosion);
                    ho_Reduced_loudu.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_R, ho_Duanmian_Erosion1, out ho_Reduced_loudu
                        );
                    ho_Region_loudu.Dispose();
                    HOperatorSet.Threshold(ho_Reduced_loudu, out ho_Region_loudu, 0, hv_iloudu_thr);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region_loudu, out ho_ConnectedRegions);
                    ho_SelectedRegions_loudu.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions_loudu,
                        "area", "and", hv_iloudu_area, 99999);
                    ho_Region_loudu.Dispose();
                    ho_Region_loudu = new HObject(ho_SelectedRegions_loudu);
                    hv_Number_loudu.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions_loudu, out hv_Number_loudu);

                    if ((int)(new HTuple(hv_Number_loudu.TupleGreater(0))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 40;
                        ho_Image_R.Dispose();
                        ho_Image_G.Dispose();
                        ho_Image_B.Dispose();
                        ho_Duanmian_Erosion1.Dispose();
                        ho_Reduced_loudu.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions_loudu.Dispose();

                        hv_iloudu_thr.Dispose();
                        hv_iloudu_area.Dispose();
                        hv_iloudu_erosion.Dispose();
                        hv_Number_loudu.Dispose();
                        hv_exc.Dispose();

                        return;
                    }




                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_Image_R.Dispose();
                ho_Image_G.Dispose();
                ho_Image_B.Dispose();
                ho_Duanmian_Erosion1.Dispose();
                ho_Reduced_loudu.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions_loudu.Dispose();

                hv_iloudu_thr.Dispose();
                hv_iloudu_area.Dispose();
                hv_iloudu_erosion.Dispose();
                hv_Number_loudu.Dispose();
                hv_exc.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Image_R.Dispose();
                ho_Image_G.Dispose();
                ho_Image_B.Dispose();
                ho_Duanmian_Erosion1.Dispose();
                ho_Reduced_loudu.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions_loudu.Dispose();

                hv_iloudu_thr.Dispose();
                hv_iloudu_area.Dispose();
                hv_iloudu_erosion.Dispose();
                hv_Number_loudu.Dispose();
                hv_exc.Dispose();

                throw HDevExpDefaultException;
            }
        }




        public static void loudu_2(HObject ho_Image, HObject ho_Region_dianji, HObject ho_Region_citi,
      HTuple hv_Parameter_LD, out HTuple hv_NGCode, out HTuple hv_loudu_Mean, out HTuple hv_loudu_div)
        {




            // Local iconic variables 

            HObject ho_RegionUnion1 = null, ho_Image1 = null;
            HObject ho_Image2 = null, ho_Image3 = null;

            // Local control variables 

            HTuple hv_iloudu_mean = new HTuple(), hv_iloudu_divstd = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_Mean3 = new HTuple(), hv_exc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            hv_NGCode = new HTuple();
            hv_loudu_Mean = new HTuple();
            hv_loudu_div = new HTuple();
            try
            {
                try
                {
                    hv_iloudu_mean.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iloudu_mean = hv_Parameter_LD.TupleSelect(
                            0);
                    }
                    hv_iloudu_divstd.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iloudu_divstd = hv_Parameter_LD.TupleSelect(
                            1);
                    }
                    ho_RegionUnion1.Dispose();
                    HOperatorSet.Union1(ho_Region_dianji, out ho_RegionUnion1);
                    ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                    HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3
                        );
                    hv_Mean.Dispose(); hv_Deviation.Dispose();
                    HOperatorSet.Intensity(ho_RegionUnion1, ho_Image3, out hv_Mean, out hv_Deviation);

                    hv_Mean3.Dispose(); hv_Deviation.Dispose();
                    HOperatorSet.Intensity(ho_Region_citi, ho_Image3, out hv_Mean3, out hv_Deviation);


                    hv_loudu_Mean.Dispose();
                    hv_loudu_Mean = new HTuple(hv_Mean);
                    hv_loudu_div.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_loudu_div = ((hv_Mean3 - hv_Mean)).TupleAbs()
                            ;
                    }

                    if ((int)(new HTuple(hv_Mean.TupleLess(hv_iloudu_mean))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 40;
                        ho_RegionUnion1.Dispose();
                        ho_Image1.Dispose();
                        ho_Image2.Dispose();
                        ho_Image3.Dispose();

                        hv_iloudu_mean.Dispose();
                        hv_iloudu_divstd.Dispose();
                        hv_Mean.Dispose();
                        hv_Deviation.Dispose();
                        hv_Mean3.Dispose();
                        hv_exc.Dispose();

                        return;
                    }

                    if ((int)(new HTuple(hv_loudu_div.TupleGreater(hv_iloudu_divstd))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 40;
                        ho_RegionUnion1.Dispose();
                        ho_Image1.Dispose();
                        ho_Image2.Dispose();
                        ho_Image3.Dispose();

                        hv_iloudu_mean.Dispose();
                        hv_iloudu_divstd.Dispose();
                        hv_Mean.Dispose();
                        hv_Deviation.Dispose();
                        hv_Mean3.Dispose();
                        hv_exc.Dispose();

                        return;
                    }


                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_RegionUnion1.Dispose();
                ho_Image1.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();

                hv_iloudu_mean.Dispose();
                hv_iloudu_divstd.Dispose();
                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Mean3.Dispose();
                hv_exc.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_RegionUnion1.Dispose();
                ho_Image1.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();

                hv_iloudu_mean.Dispose();
                hv_iloudu_divstd.Dispose();
                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Mean3.Dispose();
                hv_exc.Dispose();

                throw HDevExpDefaultException;
            }
        }


        public static void global_contamination(HObject ho_ImageReduced, HObject ho_region_dianzu,
      out HObject ho_ErrRegion, HTuple hv_Parameter_GB, out HTuple hv_NGCode1)
        {




            // Local iconic variables 

            HObject ho_RegionTrans2 = null, ho_RegionErosion1 = null;
            HObject ho_ImageReduced1 = null, ho_Region1 = null, ho_RegionOpening11 = null;
            HObject ho_ConnectedRegions1 = null, ho_SelectedRegions1 = null;
            HObject ho_RegionFillUp = null;

            // Local control variables 

            HTuple hv_iGlobalWidth = new HTuple(), hv_iGlobalHeight = new HTuple();
            HTuple hv_iGlobalThr = new HTuple(), hv_iGlobalOpen = new HTuple();
            HTuple hv_iGlobalArea = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_exc = new HTuple(), hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ErrRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans2);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening11);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            hv_NGCode1 = new HTuple();
            try
            {
                try
                {
                    //**参数
                    hv_iGlobalWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iGlobalWidth = hv_Parameter_GB.TupleSelect(
                            0);
                    }
                    hv_iGlobalHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iGlobalHeight = hv_Parameter_GB.TupleSelect(
                            1);
                    }
                    hv_iGlobalThr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iGlobalThr = hv_Parameter_GB.TupleSelect(
                            2);
                    }
                    hv_iGlobalOpen.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iGlobalOpen = hv_Parameter_GB.TupleSelect(
                            3);
                    }
                    hv_iGlobalArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iGlobalArea = hv_Parameter_GB.TupleSelect(
                            4);
                    }
                    //**电阻整体黑色斑点检测
                    ho_RegionTrans2.Dispose();
                    HOperatorSet.ShapeTrans(ho_region_dianzu, out ho_RegionTrans2, "convex");
                    ho_RegionErosion1.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionTrans2, out ho_RegionErosion1, hv_iGlobalWidth,
                        hv_iGlobalHeight);
                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionErosion1, out ho_ImageReduced1
                        );

                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, 0, hv_iGlobalThr);
                    ho_RegionOpening11.Dispose();
                    HOperatorSet.OpeningCircle(ho_Region1, out ho_RegionOpening11, hv_iGlobalOpen);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening11, out ho_ConnectedRegions1);
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1,
                        "max_area", 70);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                    hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_RegionFillUp, out hv_Area, out hv_Row1, out hv_Column1);
                    ho_ErrRegion.Dispose();
                    ho_ErrRegion = new HObject(ho_RegionFillUp);
                    if ((int)(new HTuple(hv_Area.TupleGreater(hv_iGlobalArea))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 33;
                        ho_RegionTrans2.Dispose();
                        ho_RegionErosion1.Dispose();
                        ho_ImageReduced1.Dispose();
                        ho_Region1.Dispose();
                        ho_RegionOpening11.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iGlobalWidth.Dispose();
                        hv_iGlobalHeight.Dispose();
                        hv_iGlobalThr.Dispose();
                        hv_iGlobalOpen.Dispose();
                        hv_iGlobalArea.Dispose();
                        hv_Area.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }


                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }
                ho_RegionTrans2.Dispose();
                ho_RegionErosion1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening11.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionFillUp.Dispose();

                hv_iGlobalWidth.Dispose();
                hv_iGlobalHeight.Dispose();
                hv_iGlobalThr.Dispose();
                hv_iGlobalOpen.Dispose();
                hv_iGlobalArea.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_RegionTrans2.Dispose();
                ho_RegionErosion1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening11.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionFillUp.Dispose();

                hv_iGlobalWidth.Dispose();
                hv_iGlobalHeight.Dispose();
                hv_iGlobalThr.Dispose();
                hv_iGlobalOpen.Dispose();
                hv_iGlobalArea.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }



        public static void dianji_heidian(HObject ho_ImageReduced, HObject ho_Region_dianji,
      out HObject ho_BlackERR2, HTuple hv_Parameter_DJ, out HTuple hv_NGCode1)
        {




            // Local iconic variables 

            HObject ho_RegionUnion1 = null, ho_ImageReduced3 = null;
            HObject ho_Region1 = null, ho_RegionOpening11 = null, ho_ConnectedRegions1 = null;
            HObject ho_SelectedRegions = null;

            // Local control variables 

            HTuple hv_iBlackThr2 = new HTuple(), hv_iBlackOpen2 = new HTuple();
            HTuple hv_iBlackArea2 = new HTuple(), hv_Mean1 = new HTuple();
            HTuple hv_Deviation = new HTuple(), hv_Mean2 = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_exc = new HTuple();
            HTuple hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_BlackERR2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced3);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening11);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            hv_NGCode1 = new HTuple();
            try
            {
                try
                {

                    hv_iBlackThr2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBlackThr2 = hv_Parameter_DJ.TupleSelect(
                            0);
                    }
                    hv_iBlackOpen2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBlackOpen2 = hv_Parameter_DJ.TupleSelect(
                            1);
                    }
                    hv_iBlackArea2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBlackArea2 = hv_Parameter_DJ.TupleSelect(
                            2);
                    }
                    //**电极部分黑色斑点检测

                    ho_RegionUnion1.Dispose();
                    HOperatorSet.Union1(ho_Region_dianji, out ho_RegionUnion1);
                    ho_ImageReduced3.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionUnion1, out ho_ImageReduced3
                        );

                    hv_Mean1.Dispose(); hv_Deviation.Dispose();
                    HOperatorSet.Intensity(ho_RegionUnion1, ho_ImageReduced, out hv_Mean1, out hv_Deviation);
                    //threshold (ImageReduced3, Region1, 0, iBlackThr2)

                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced3, out ho_Region1, 0, hv_Mean1 - hv_iBlackThr2);
                    ho_RegionOpening11.Dispose();
                    HOperatorSet.OpeningCircle(ho_Region1, out ho_RegionOpening11, hv_iBlackOpen2);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening11, out ho_ConnectedRegions1);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions, "area",
                        "and", 50, 99999);
                    ho_BlackERR2.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_BlackERR2);


                    hv_Mean2.Dispose(); hv_Deviation.Dispose();
                    HOperatorSet.Intensity(ho_BlackERR2, ho_ImageReduced, out hv_Mean2, out hv_Deviation);



                    hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_BlackERR2, out hv_Area, out hv_Row1, out hv_Column1);
                    ho_BlackERR2.Dispose();
                    ho_BlackERR2 = new HObject(ho_SelectedRegions);

                    if ((int)(new HTuple(hv_Area.TupleGreater(hv_iBlackArea2))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 32;
                        ho_RegionUnion1.Dispose();
                        ho_ImageReduced3.Dispose();
                        ho_Region1.Dispose();
                        ho_RegionOpening11.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_SelectedRegions.Dispose();

                        hv_iBlackThr2.Dispose();
                        hv_iBlackOpen2.Dispose();
                        hv_iBlackArea2.Dispose();
                        hv_Mean1.Dispose();
                        hv_Deviation.Dispose();
                        hv_Mean2.Dispose();
                        hv_Area.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }

                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_RegionUnion1.Dispose();
                ho_ImageReduced3.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening11.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions.Dispose();

                hv_iBlackThr2.Dispose();
                hv_iBlackOpen2.Dispose();
                hv_iBlackArea2.Dispose();
                hv_Mean1.Dispose();
                hv_Deviation.Dispose();
                hv_Mean2.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_RegionUnion1.Dispose();
                ho_ImageReduced3.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening11.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions.Dispose();

                hv_iBlackThr2.Dispose();
                hv_iBlackOpen2.Dispose();
                hv_iBlackArea2.Dispose();
                hv_Mean1.Dispose();
                hv_Deviation.Dispose();
                hv_Mean2.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void citi_zhangwu(HObject ho_ImageReduced, HObject ho_Region_citi, out HObject ho_BlackErr,
      out HObject ho_WhiteErr, HTuple hv_Parameter_CT, out HTuple hv_NGCode1)
        {




            // Local iconic variables 

            HObject ho_RegionFillUp = null, ho_RegionErosion = null;
            HObject ho_ImageReduced3 = null, ho_Region1 = null, ho_RegionOpening11 = null;
            HObject ho_ConnectedRegions1 = null, ho_SelectedRegions1 = null;
            HObject ho_ImageMean1 = null, ho_RegionDynThresh1 = null, ho_RegionClosing2 = null;
            HObject ho_ConnectedRegions7 = null, ho_RegionFillUp3 = null;
            HObject ho_SelectedRegions8 = null;

            // Local control variables 

            HTuple hv_iCitiWidth = new HTuple(), hv_iCitiHeight = new HTuple();
            HTuple hv_iBlackThr = new HTuple(), hv_iBlackOpen = new HTuple();
            HTuple hv_iBlackArea = new HTuple(), hv_iWhiteWidth = new HTuple();
            HTuple hv_iWhiteExp = new HTuple(), hv_iWhiteClosing = new HTuple();
            HTuple hv_iWhiteArea = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Area6 = new HTuple(), hv_Row9 = new HTuple();
            HTuple hv_Column9 = new HTuple(), hv_exc = new HTuple();
            HTuple hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_BlackErr);
            HOperatorSet.GenEmptyObj(out ho_WhiteErr);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced3);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening11);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions7);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions8);
            hv_NGCode1 = new HTuple();
            try
            {
                try
                {

                    hv_iCitiWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iCitiWidth = hv_Parameter_CT.TupleSelect(
                            0);
                    }
                    hv_iCitiHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iCitiHeight = hv_Parameter_CT.TupleSelect(
                            1);
                    }

                    hv_iBlackThr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBlackThr = hv_Parameter_CT.TupleSelect(
                            2);
                    }
                    hv_iBlackOpen.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBlackOpen = hv_Parameter_CT.TupleSelect(
                            3);
                    }
                    hv_iBlackArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBlackArea = hv_Parameter_CT.TupleSelect(
                            4);
                    }

                    hv_iWhiteWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iWhiteWidth = hv_Parameter_CT.TupleSelect(
                            5);
                    }
                    hv_iWhiteExp.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iWhiteExp = hv_Parameter_CT.TupleSelect(
                            6);
                    }
                    hv_iWhiteClosing.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iWhiteClosing = hv_Parameter_CT.TupleSelect(
                            7);
                    }
                    hv_iWhiteArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iWhiteArea = hv_Parameter_CT.TupleSelect(
                            8);
                    }


                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_Region_citi, out ho_RegionFillUp);
                    //shape_trans (RegionFillUp, RegionTrans, 'rectangle2')
                    //erosion_rectangle1 (RegionFillUp, RegionErosion, iCitiWidth, iCitiHeight)

                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionFillUp, out ho_RegionErosion, hv_iCitiWidth,
                        hv_iCitiHeight);
                    ho_ImageReduced3.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionErosion, out ho_ImageReduced3
                        );

                    //**检测瓷体上的黑色脏污
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced3, out ho_Region1, 0, hv_iBlackThr);
                    ho_RegionOpening11.Dispose();
                    HOperatorSet.OpeningCircle(ho_Region1, out ho_RegionOpening11, hv_iBlackOpen);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening11, out ho_ConnectedRegions1);
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1,
                        "max_area", 70);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                    hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_RegionFillUp, out hv_Area, out hv_Row1, out hv_Column1);
                    ho_BlackErr.Dispose();
                    ho_BlackErr = new HObject(ho_RegionFillUp);
                    if ((int)(new HTuple(hv_Area.TupleGreater(hv_iBlackArea))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 30;
                        ho_RegionFillUp.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_ImageReduced3.Dispose();
                        ho_Region1.Dispose();
                        ho_RegionOpening11.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_ImageMean1.Dispose();
                        ho_RegionDynThresh1.Dispose();
                        ho_RegionClosing2.Dispose();
                        ho_ConnectedRegions7.Dispose();
                        ho_RegionFillUp3.Dispose();
                        ho_SelectedRegions8.Dispose();

                        hv_iCitiWidth.Dispose();
                        hv_iCitiHeight.Dispose();
                        hv_iBlackThr.Dispose();
                        hv_iBlackOpen.Dispose();
                        hv_iBlackArea.Dispose();
                        hv_iWhiteWidth.Dispose();
                        hv_iWhiteExp.Dispose();
                        hv_iWhiteClosing.Dispose();
                        hv_iWhiteArea.Dispose();
                        hv_Area.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_Area6.Dispose();
                        hv_Row9.Dispose();
                        hv_Column9.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }


                    //**检测瓷体上的白色脏污（沾锡）
                    ho_ImageMean1.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced3, out ho_ImageMean1, hv_iWhiteWidth,
                        hv_iWhiteWidth);
                    ho_RegionDynThresh1.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageReduced3, ho_ImageMean1, out ho_RegionDynThresh1,
                        hv_iWhiteExp, "light");
                    ho_RegionClosing2.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionDynThresh1, out ho_RegionClosing2, hv_iWhiteClosing);
                    ho_ConnectedRegions7.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing2, out ho_ConnectedRegions7);
                    ho_RegionFillUp3.Dispose();
                    HOperatorSet.FillUp(ho_ConnectedRegions7, out ho_RegionFillUp3);
                    ho_SelectedRegions8.Dispose();
                    HOperatorSet.SelectShapeStd(ho_RegionFillUp3, out ho_SelectedRegions8, "max_area",
                        70);
                    hv_Area6.Dispose(); hv_Row9.Dispose(); hv_Column9.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegions8, out hv_Area6, out hv_Row9, out hv_Column9);
                    ho_WhiteErr.Dispose();
                    ho_WhiteErr = new HObject(ho_SelectedRegions8);
                    if ((int)(new HTuple(hv_Area6.TupleGreater(hv_iWhiteArea))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 31;
                        ho_RegionFillUp.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_ImageReduced3.Dispose();
                        ho_Region1.Dispose();
                        ho_RegionOpening11.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_ImageMean1.Dispose();
                        ho_RegionDynThresh1.Dispose();
                        ho_RegionClosing2.Dispose();
                        ho_ConnectedRegions7.Dispose();
                        ho_RegionFillUp3.Dispose();
                        ho_SelectedRegions8.Dispose();

                        hv_iCitiWidth.Dispose();
                        hv_iCitiHeight.Dispose();
                        hv_iBlackThr.Dispose();
                        hv_iBlackOpen.Dispose();
                        hv_iBlackArea.Dispose();
                        hv_iWhiteWidth.Dispose();
                        hv_iWhiteExp.Dispose();
                        hv_iWhiteClosing.Dispose();
                        hv_iWhiteArea.Dispose();
                        hv_Area.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_Area6.Dispose();
                        hv_Row9.Dispose();
                        hv_Column9.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }


                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_RegionFillUp.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced3.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening11.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh1.Dispose();
                ho_RegionClosing2.Dispose();
                ho_ConnectedRegions7.Dispose();
                ho_RegionFillUp3.Dispose();
                ho_SelectedRegions8.Dispose();

                hv_iCitiWidth.Dispose();
                hv_iCitiHeight.Dispose();
                hv_iBlackThr.Dispose();
                hv_iBlackOpen.Dispose();
                hv_iBlackArea.Dispose();
                hv_iWhiteWidth.Dispose();
                hv_iWhiteExp.Dispose();
                hv_iWhiteClosing.Dispose();
                hv_iWhiteArea.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Area6.Dispose();
                hv_Row9.Dispose();
                hv_Column9.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_RegionFillUp.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced3.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening11.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ImageMean1.Dispose();
                ho_RegionDynThresh1.Dispose();
                ho_RegionClosing2.Dispose();
                ho_ConnectedRegions7.Dispose();
                ho_RegionFillUp3.Dispose();
                ho_SelectedRegions8.Dispose();

                hv_iCitiWidth.Dispose();
                hv_iCitiHeight.Dispose();
                hv_iBlackThr.Dispose();
                hv_iBlackOpen.Dispose();
                hv_iBlackArea.Dispose();
                hv_iWhiteWidth.Dispose();
                hv_iWhiteExp.Dispose();
                hv_iWhiteClosing.Dispose();
                hv_iWhiteArea.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Area6.Dispose();
                hv_Row9.Dispose();
                hv_Column9.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }
        public static void duanmian_dingwei(HObject ho_GrayImage, out HObject ho_Region_Duanmian,
      HTuple hv_parameter, out HTuple hv_NGCode)
        {




            // Local iconic variables 

            HObject ho_Region1 = null, ho_ConnectedRegions2 = null;
            HObject ho_SelectedRegions1 = null, ho_Rectangle1 = null, ho_ImageReduced1 = null;
            HObject ho_Region3 = null, ho_RegionTrans = null, ho_RegionDilation = null;
            HObject ho_ImageReduced2 = null, ho_ImageMean = null, ho_RegionDynThresh = null;
            HObject ho_ConnectedRegions3 = null, ho_SelectedRegions2 = null;
            HObject ho_RegionFillUp2 = null, ho_Rectangle2 = null, ho_ImageReduced4 = null;
            HObject ho_ImageScaled1 = null, ho_Region4 = null, ho_RegionOpening1 = null;
            HObject ho_ConnectedRegions4 = null, ho_SelectedRegions3 = null;
            HObject ho_RegionFillUp3 = null, ho_RegionOpening2 = null, ho_ConnectedRegions6 = null;
            HObject ho_SelectedRegions4 = null, ho_Rectangle_Duanmian = null;

            // Local control variables 

            HTuple hv_iThr1 = new HTuple(), hv_iThr2 = new HTuple();
            HTuple hv_iMaskWith = new HTuple(), hv_iDyn_Thr = new HTuple();
            HTuple hv_iScaleMult = new HTuple(), hv_iScaleAdd = new HTuple();
            HTuple hv_iErrAll = new HTuple(), hv_iOpening_Circle = new HTuple();
            HTuple hv_Area_Duanmian = new HTuple(), hv_iAngleScale = new HTuple();
            HTuple hv_iRecty = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Row4 = new HTuple(), hv_Column4 = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Rectangularity = new HTuple(), hv_Row5 = new HTuple();
            HTuple hv_Column5 = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Length11 = new HTuple(), hv_Length21 = new HTuple();
            HTuple hv_Deg = new HTuple(), hv_exc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_Duanmian);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region3);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced4);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
            HOperatorSet.GenEmptyObj(out ho_Region4);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions4);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp3);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions6);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions4);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_Duanmian);
            hv_NGCode = new HTuple();
            try
            {
                try
                {
                    hv_iThr1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iThr1 = hv_parameter.TupleSelect(
                            0);
                    }
                    hv_iThr2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iThr2 = hv_parameter.TupleSelect(
                            1);
                    }
                    hv_iMaskWith.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iMaskWith = hv_parameter.TupleSelect(
                            2);
                    }
                    hv_iDyn_Thr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iDyn_Thr = hv_parameter.TupleSelect(
                            3);
                    }

                    hv_iScaleMult.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScaleMult = hv_parameter.TupleSelect(
                            4);
                    }
                    hv_iScaleAdd.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScaleAdd = hv_parameter.TupleSelect(
                            5);
                    }
                    hv_iErrAll.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrAll = hv_parameter.TupleSelect(
                            6);
                    }
                    hv_iOpening_Circle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iOpening_Circle = hv_parameter.TupleSelect(
                            7);
                    }

                    hv_Area_Duanmian.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Area_Duanmian = hv_parameter.TupleSelect(
                            8);
                    }
                    hv_iAngleScale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iAngleScale = hv_parameter.TupleSelect(
                            9);
                    }
                    hv_iRecty.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iRecty = hv_parameter.TupleSelect(
                            10);
                    }

                    //**粗定位，确定端面主题区域
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_GrayImage, out ho_Region1, hv_iThr1, 255);
                    ho_ConnectedRegions2.Dispose();
                    HOperatorSet.Connection(ho_Region1, out ho_ConnectedRegions2);
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions2, out ho_SelectedRegions1,
                        "max_area", 70);
                    hv_Area1.Dispose(); hv_Row3.Dispose(); hv_Column3.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area1, out hv_Row3, out hv_Column3);
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row3, hv_Column3, 0, 250,
                        20);

                    //**粗定位2，根据确定端面区域中心点，再根据已知的端面区域大小
                    //**确定端面搜索范围
                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle1, out ho_ImageReduced1
                        );
                    ho_Region3.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region3, hv_iThr2, 255);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_Region3, out ho_RegionTrans, "rectangle1");
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionTrans, out ho_RegionDilation, 50,
                        160);
                    ho_ImageReduced2.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionDilation, out ho_ImageReduced2
                        );

                    //**使用mean_image 函数锁定端面区域
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced2, out ho_ImageMean, hv_iMaskWith,
                        hv_iMaskWith);
                    ho_RegionDynThresh.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageReduced2, ho_ImageMean, out ho_RegionDynThresh,
                        hv_iDyn_Thr, "light");
                    ho_ConnectedRegions3.Dispose();
                    HOperatorSet.Connection(ho_RegionDynThresh, out ho_ConnectedRegions3);
                    ho_SelectedRegions2.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions3, out ho_SelectedRegions2,
                        "max_area", 70);
                    ho_RegionFillUp2.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions2, out ho_RegionFillUp2);
                    hv_Row4.Dispose(); hv_Column4.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_RegionFillUp2, out hv_Row4, out hv_Column4,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle2.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row4, hv_Column4, hv_Phi,
                        hv_Length1, hv_Length2);
                    ho_ImageReduced4.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle2, out ho_ImageReduced4
                        );

                    //**使用scale_image精确确定端面区域
                    //**（上一步mean+dyn主要作用是为这一步排除干扰）

                    ho_ImageScaled1.Dispose();
                    HOperatorSet.ScaleImage(ho_ImageReduced4, out ho_ImageScaled1, hv_iScaleMult,
                        -hv_iScaleAdd);
                    ho_Region4.Dispose();
                    HOperatorSet.Threshold(ho_ImageScaled1, out ho_Region4, hv_iErrAll, 255);
                    ho_RegionOpening1.Dispose();
                    HOperatorSet.OpeningCircle(ho_Region4, out ho_RegionOpening1, hv_iOpening_Circle);
                    ho_ConnectedRegions4.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions4);
                    ho_SelectedRegions3.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions4, out ho_SelectedRegions3,
                        "max_area", 70);

                    ho_RegionFillUp3.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions3, out ho_RegionFillUp3);
                    ho_RegionOpening2.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionFillUp3, out ho_RegionOpening2, 10,
                        5);
                    ho_ConnectedRegions6.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening2, out ho_ConnectedRegions6);
                    ho_SelectedRegions4.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions6, out ho_SelectedRegions4,
                        "max_area", 70);

                    ho_Region_Duanmian.Dispose();
                    ho_Region_Duanmian = new HObject(ho_SelectedRegions4);

                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_Region_Duanmian, out hv_Area, out hv_Row, out hv_Column);
                    if ((int)(new HTuple(hv_Area.TupleLess(hv_Area_Duanmian))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 1;
                        ho_Region1.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_Rectangle1.Dispose();
                        ho_ImageReduced1.Dispose();
                        ho_Region3.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_RegionDilation.Dispose();
                        ho_ImageReduced2.Dispose();
                        ho_ImageMean.Dispose();
                        ho_RegionDynThresh.Dispose();
                        ho_ConnectedRegions3.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_RegionFillUp2.Dispose();
                        ho_Rectangle2.Dispose();
                        ho_ImageReduced4.Dispose();
                        ho_ImageScaled1.Dispose();
                        ho_Region4.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_ConnectedRegions4.Dispose();
                        ho_SelectedRegions3.Dispose();
                        ho_RegionFillUp3.Dispose();
                        ho_RegionOpening2.Dispose();
                        ho_ConnectedRegions6.Dispose();
                        ho_SelectedRegions4.Dispose();
                        ho_Rectangle_Duanmian.Dispose();

                        hv_iThr1.Dispose();
                        hv_iThr2.Dispose();
                        hv_iMaskWith.Dispose();
                        hv_iDyn_Thr.Dispose();
                        hv_iScaleMult.Dispose();
                        hv_iScaleAdd.Dispose();
                        hv_iErrAll.Dispose();
                        hv_iOpening_Circle.Dispose();
                        hv_Area_Duanmian.Dispose();
                        hv_iAngleScale.Dispose();
                        hv_iRecty.Dispose();
                        hv_Area1.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Row4.Dispose();
                        hv_Column4.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi1.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Deg.Dispose();
                        hv_exc.Dispose();

                        return;
                    }


                    hv_Rectangularity.Dispose();
                    HOperatorSet.Rectangularity(ho_Region_Duanmian, out hv_Rectangularity);
                    if ((int)(new HTuple(hv_Rectangularity.TupleLess(hv_iRecty))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 2;
                        ho_Region1.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_Rectangle1.Dispose();
                        ho_ImageReduced1.Dispose();
                        ho_Region3.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_RegionDilation.Dispose();
                        ho_ImageReduced2.Dispose();
                        ho_ImageMean.Dispose();
                        ho_RegionDynThresh.Dispose();
                        ho_ConnectedRegions3.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_RegionFillUp2.Dispose();
                        ho_Rectangle2.Dispose();
                        ho_ImageReduced4.Dispose();
                        ho_ImageScaled1.Dispose();
                        ho_Region4.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_ConnectedRegions4.Dispose();
                        ho_SelectedRegions3.Dispose();
                        ho_RegionFillUp3.Dispose();
                        ho_RegionOpening2.Dispose();
                        ho_ConnectedRegions6.Dispose();
                        ho_SelectedRegions4.Dispose();
                        ho_Rectangle_Duanmian.Dispose();

                        hv_iThr1.Dispose();
                        hv_iThr2.Dispose();
                        hv_iMaskWith.Dispose();
                        hv_iDyn_Thr.Dispose();
                        hv_iScaleMult.Dispose();
                        hv_iScaleAdd.Dispose();
                        hv_iErrAll.Dispose();
                        hv_iOpening_Circle.Dispose();
                        hv_Area_Duanmian.Dispose();
                        hv_iAngleScale.Dispose();
                        hv_iRecty.Dispose();
                        hv_Area1.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Row4.Dispose();
                        hv_Column4.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi1.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Deg.Dispose();
                        hv_exc.Dispose();

                        return;
                    }

                    hv_Row5.Dispose(); hv_Column5.Dispose(); hv_Phi1.Dispose(); hv_Length11.Dispose(); hv_Length21.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_Duanmian, out hv_Row5, out hv_Column5,
                        out hv_Phi1, out hv_Length11, out hv_Length21);
                    ho_Rectangle_Duanmian.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_Duanmian, hv_Row5, hv_Column5,
                        hv_Phi1, hv_Length11, hv_Length21);

                    hv_Deg.Dispose();
                    HOperatorSet.TupleDeg(hv_Phi1, out hv_Deg);

                    if ((int)(new HTuple(hv_Deg.TupleGreater(hv_iAngleScale))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 3;
                        ho_Region1.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_Rectangle1.Dispose();
                        ho_ImageReduced1.Dispose();
                        ho_Region3.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_RegionDilation.Dispose();
                        ho_ImageReduced2.Dispose();
                        ho_ImageMean.Dispose();
                        ho_RegionDynThresh.Dispose();
                        ho_ConnectedRegions3.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_RegionFillUp2.Dispose();
                        ho_Rectangle2.Dispose();
                        ho_ImageReduced4.Dispose();
                        ho_ImageScaled1.Dispose();
                        ho_Region4.Dispose();
                        ho_RegionOpening1.Dispose();
                        ho_ConnectedRegions4.Dispose();
                        ho_SelectedRegions3.Dispose();
                        ho_RegionFillUp3.Dispose();
                        ho_RegionOpening2.Dispose();
                        ho_ConnectedRegions6.Dispose();
                        ho_SelectedRegions4.Dispose();
                        ho_Rectangle_Duanmian.Dispose();

                        hv_iThr1.Dispose();
                        hv_iThr2.Dispose();
                        hv_iMaskWith.Dispose();
                        hv_iDyn_Thr.Dispose();
                        hv_iScaleMult.Dispose();
                        hv_iScaleAdd.Dispose();
                        hv_iErrAll.Dispose();
                        hv_iOpening_Circle.Dispose();
                        hv_Area_Duanmian.Dispose();
                        hv_iAngleScale.Dispose();
                        hv_iRecty.Dispose();
                        hv_Area1.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Row4.Dispose();
                        hv_Column4.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi1.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Deg.Dispose();
                        hv_exc.Dispose();

                        return;
                    }


                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_Region1.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region3.Dispose();
                ho_RegionTrans.Dispose();
                ho_RegionDilation.Dispose();
                ho_ImageReduced2.Dispose();
                ho_ImageMean.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_Rectangle2.Dispose();
                ho_ImageReduced4.Dispose();
                ho_ImageScaled1.Dispose();
                ho_Region4.Dispose();
                ho_RegionOpening1.Dispose();
                ho_ConnectedRegions4.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_RegionFillUp3.Dispose();
                ho_RegionOpening2.Dispose();
                ho_ConnectedRegions6.Dispose();
                ho_SelectedRegions4.Dispose();
                ho_Rectangle_Duanmian.Dispose();

                hv_iThr1.Dispose();
                hv_iThr2.Dispose();
                hv_iMaskWith.Dispose();
                hv_iDyn_Thr.Dispose();
                hv_iScaleMult.Dispose();
                hv_iScaleAdd.Dispose();
                hv_iErrAll.Dispose();
                hv_iOpening_Circle.Dispose();
                hv_Area_Duanmian.Dispose();
                hv_iAngleScale.Dispose();
                hv_iRecty.Dispose();
                hv_Area1.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Row4.Dispose();
                hv_Column4.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Rectangularity.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Deg.Dispose();
                hv_exc.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Region1.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region3.Dispose();
                ho_RegionTrans.Dispose();
                ho_RegionDilation.Dispose();
                ho_ImageReduced2.Dispose();
                ho_ImageMean.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_Rectangle2.Dispose();
                ho_ImageReduced4.Dispose();
                ho_ImageScaled1.Dispose();
                ho_Region4.Dispose();
                ho_RegionOpening1.Dispose();
                ho_ConnectedRegions4.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_RegionFillUp3.Dispose();
                ho_RegionOpening2.Dispose();
                ho_ConnectedRegions6.Dispose();
                ho_SelectedRegions4.Dispose();
                ho_Rectangle_Duanmian.Dispose();

                hv_iThr1.Dispose();
                hv_iThr2.Dispose();
                hv_iMaskWith.Dispose();
                hv_iDyn_Thr.Dispose();
                hv_iScaleMult.Dispose();
                hv_iScaleAdd.Dispose();
                hv_iErrAll.Dispose();
                hv_iOpening_Circle.Dispose();
                hv_Area_Duanmian.Dispose();
                hv_iAngleScale.Dispose();
                hv_iRecty.Dispose();
                hv_Area1.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Row4.Dispose();
                hv_Column4.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Rectangularity.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Deg.Dispose();
                hv_exc.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void Detect_Err_12(HObject ho_GrayImage, HObject ho_Region_Duanmian, out HObject ho_Region_Err2,
      HTuple hv_parameter, out HTuple hv_NGCode)
        {




            // Local iconic variables 

            HObject ho_RegionErosion = null, ho_ImageReduced = null;
            HObject ho_ImageMean1 = null, ho_Region1 = null, ho_Region2 = null;
            HObject ho_RegionErosion3 = null, ho_ImageReduced6 = null, ho_Region6 = null;
            HObject ho_ConnectedRegions1 = null, ho_SelectedRegions7 = null;

            // Local control variables 

            HTuple hv_iErrArea2 = new HTuple(), hv_iSobelSize = new HTuple();
            HTuple hv_iThr1 = new HTuple(), hv_iClosCir = new HTuple();
            HTuple hv_iEroCir1 = new HTuple(), hv_iEroCir2 = new HTuple();
            HTuple hv_iThr2 = new HTuple(), hv_UsedThreshold = new HTuple();
            HTuple hv_Number1 = new HTuple(), hv_exc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_Err2);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageMean1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion3);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced6);
            HOperatorSet.GenEmptyObj(out ho_Region6);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions7);
            hv_NGCode = new HTuple();
            try
            {


                try
                {
                    hv_iErrArea2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrArea2 = hv_parameter.TupleSelect(
                            11);
                    }
                    hv_iSobelSize.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iSobelSize = hv_parameter.TupleSelect(
                            12);
                    }
                    hv_iThr1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iThr1 = hv_parameter.TupleSelect(
                            13);
                    }
                    hv_iClosCir.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iClosCir = hv_parameter.TupleSelect(
                            14);
                    }
                    hv_iEroCir1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iEroCir1 = hv_parameter.TupleSelect(
                            15);
                    }
                    hv_iEroCir2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iEroCir2 = hv_parameter.TupleSelect(
                            16);
                    }
                    hv_iThr2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iThr2 = hv_parameter.TupleSelect(
                            17);
                    }

                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Region_Duanmian, out ho_RegionErosion,
                        7, 7);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionErosion, out ho_ImageReduced
                        );

                    ho_ImageMean1.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean1, 3, 3);
                    ho_Region1.Dispose(); hv_UsedThreshold.Dispose();
                    HOperatorSet.BinaryThreshold(ho_ImageMean1, out ho_Region1, "max_separability",
                        "dark", out hv_UsedThreshold);
                    ho_Region2.Dispose();
                    HOperatorSet.Threshold(ho_ImageMean1, out ho_Region2, 0, hv_UsedThreshold - hv_iThr1);
                    ho_RegionErosion3.Dispose();
                    HOperatorSet.ErosionCircle(ho_Region2, out ho_RegionErosion3, 3);

                    ho_ImageReduced6.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionErosion3, out ho_ImageReduced6
                        );
                    ho_Region6.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced6, out ho_Region6, 0, hv_iThr2);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_Region6, out ho_ConnectedRegions1);
                    ho_SelectedRegions7.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions7, "area",
                        "and", hv_iErrArea2, 99999);
                    ho_Region_Err2.Dispose();
                    ho_Region_Err2 = new HObject(ho_SelectedRegions7);
                    hv_Number1.Dispose();
                    HOperatorSet.CountObj(ho_Region_Err2, out hv_Number1);
                    if ((int)(new HTuple(hv_Number1.TupleGreater(0))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 4;
                        ho_RegionErosion.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageMean1.Dispose();
                        ho_Region1.Dispose();
                        ho_Region2.Dispose();
                        ho_RegionErosion3.Dispose();
                        ho_ImageReduced6.Dispose();
                        ho_Region6.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_SelectedRegions7.Dispose();

                        hv_iErrArea2.Dispose();
                        hv_iSobelSize.Dispose();
                        hv_iThr1.Dispose();
                        hv_iClosCir.Dispose();
                        hv_iEroCir1.Dispose();
                        hv_iEroCir2.Dispose();
                        hv_iThr2.Dispose();
                        hv_UsedThreshold.Dispose();
                        hv_Number1.Dispose();
                        hv_exc.Dispose();

                        return;
                    }




                    //threshold (EdgeAmplitude1, Region, 30, 255)
                    //closing_circle (Region, RegionClosing, iClosCir)
                    //erosion_circle (RegionClosing, RegionErosion1, iEroCir1)
                    //fill_up (RegionErosion1, RegionFillUp)
                    //difference (RegionErosion, RegionClosing, RegionDifference2)
                    //iSobelSize
                    //exp_image (ImageReduced5, ExpImage, 2)

                    //scale_image_max (ExpImage, ImageScaleMax)


                    //threshold (ImageScaleMax, Region5, 200, 255)

                    //closing_circle (Region5, RegionClosing, iClosCir)
                    //erosion_circle (RegionClosing, RegionErosion1, iEroCir1)
                    //fill_up (RegionErosion1, RegionFillUp)
                    //difference (RegionErosion, RegionErosion1, RegionDifference2)

                    //erosion_circle (RegionDifference2, RegionErosion2, iEroCir2)
                    //reduce_domain (GrayImage, RegionErosion2, ImageReduced6)
                    //threshold (ImageReduced6, Region6, 0, 120)
                    //connection (Region6, ConnectedRegions1)
                    //select_shape (ConnectedRegions1, SelectedRegions7, 'area', 'and', iErrArea2, 99999)
                    //Region_Err2 := SelectedRegions7
                    //count_obj (Region_Err2, Number1)
                    //if (Number1>0)
                    //NGCode := 4
                    //return ()
                    //endif


                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_RegionErosion.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean1.Dispose();
                ho_Region1.Dispose();
                ho_Region2.Dispose();
                ho_RegionErosion3.Dispose();
                ho_ImageReduced6.Dispose();
                ho_Region6.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions7.Dispose();

                hv_iErrArea2.Dispose();
                hv_iSobelSize.Dispose();
                hv_iThr1.Dispose();
                hv_iClosCir.Dispose();
                hv_iEroCir1.Dispose();
                hv_iEroCir2.Dispose();
                hv_iThr2.Dispose();
                hv_UsedThreshold.Dispose();
                hv_Number1.Dispose();
                hv_exc.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_RegionErosion.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean1.Dispose();
                ho_Region1.Dispose();
                ho_Region2.Dispose();
                ho_RegionErosion3.Dispose();
                ho_ImageReduced6.Dispose();
                ho_Region6.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions7.Dispose();

                hv_iErrArea2.Dispose();
                hv_iSobelSize.Dispose();
                hv_iThr1.Dispose();
                hv_iClosCir.Dispose();
                hv_iEroCir1.Dispose();
                hv_iEroCir2.Dispose();
                hv_iThr2.Dispose();
                hv_UsedThreshold.Dispose();
                hv_Number1.Dispose();
                hv_exc.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void cemian_dingwei(HObject ho_GrayImage, out HObject ho_Region_cemian,
      out HObject ho_Rectangle_Cemian, HTuple hv_parameter, out HTuple hv_NGCode)
        {




            // Local iconic variables 

            HObject ho_Region = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_RegionTrans = null, ho_Rectangle = null;
            HObject ho_Rectangle3 = null, ho_ImageReduced = null, ho_ImageMean = null;
            HObject ho_RegionDynThresh = null, ho_RegionOpening = null;
            HObject ho_ConnectedRegions1 = null;

            // Local control variables 

            HTuple hv_iThr_Cudingwei = new HTuple(), hv_AreaDuanmian = new HTuple();
            HTuple hv_iPos_Sigma = new HTuple(), hv_iPos_Thr1 = new HTuple();
            HTuple hv_iPos_Thr2 = new HTuple(), hv_iMaskWith = new HTuple();
            HTuple hv_iDyn_Thr = new HTuple(), hv_iOpen1 = new HTuple();
            HTuple hv_WindowHandle = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Area2 = new HTuple(), hv_Row4 = new HTuple();
            HTuple hv_Column4 = new HTuple(), hv_pai = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Phi2 = new HTuple(), hv_Length12 = new HTuple();
            HTuple hv_Length22 = new HTuple(), hv_MeasureHandle = new HTuple();
            HTuple hv_RowEdge1 = new HTuple(), hv_ColumnEdge1 = new HTuple();
            HTuple hv_Amplitude1 = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_RowEdge2 = new HTuple(), hv_ColumnEdge2 = new HTuple();
            HTuple hv_Amplitude2 = new HTuple(), hv_RowEdge = new HTuple();
            HTuple hv_ColumnEdge = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length13 = new HTuple(), hv_Length23 = new HTuple();
            HTuple hv_exc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_cemian);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_Cemian);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            hv_NGCode = new HTuple();
            try
            {
                try
                {

                    //**参数传递
                    hv_iThr_Cudingwei.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iThr_Cudingwei = hv_parameter.TupleSelect(
                            0);
                    }
                    hv_AreaDuanmian.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreaDuanmian = hv_parameter.TupleSelect(
                            1);
                    }
                    hv_iPos_Sigma.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iPos_Sigma = hv_parameter.TupleSelect(
                            2);
                    }
                    hv_iPos_Thr1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iPos_Thr1 = hv_parameter.TupleSelect(
                            3);
                    }
                    hv_iPos_Thr2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iPos_Thr2 = hv_parameter.TupleSelect(
                            4);
                    }

                    hv_iMaskWith.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iMaskWith = hv_parameter.TupleSelect(
                            5);
                    }
                    hv_iDyn_Thr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iDyn_Thr = hv_parameter.TupleSelect(
                            6);
                    }
                    hv_iOpen1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iOpen1 = hv_parameter.TupleSelect(
                            7);
                    }
                    hv_WindowHandle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WindowHandle = hv_parameter.TupleSelect(
                            8);
                    }

                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width, out hv_Height);

                    //threshold (GrayImage, Region, iThr_Cudingwei, 255)
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_GrayImage, out ho_Region, 100, 255);

                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions,
                        "max_area", 70);
                    hv_Area1.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area1, out hv_Row, out hv_Column);

                    //**粗定位失败，显示区域
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_SelectedRegions, out ho_RegionTrans, "convex");
                    ho_Rectangle_Cemian.Dispose();
                    ho_Rectangle_Cemian = new HObject(ho_RegionTrans);
                    if ((int)(new HTuple(hv_Area1.TupleLess(hv_AreaDuanmian))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 1;
                        ho_Region.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageMean.Dispose();
                        ho_RegionDynThresh.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions1.Dispose();

                        hv_iThr_Cudingwei.Dispose();
                        hv_AreaDuanmian.Dispose();
                        hv_iPos_Sigma.Dispose();
                        hv_iPos_Thr1.Dispose();
                        hv_iPos_Thr2.Dispose();
                        hv_iMaskWith.Dispose();
                        hv_iDyn_Thr.Dispose();
                        hv_iOpen1.Dispose();
                        hv_WindowHandle.Dispose();
                        hv_Width.Dispose();
                        hv_Height.Dispose();
                        hv_Area1.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Area2.Dispose();
                        hv_Row4.Dispose();
                        hv_Column4.Dispose();
                        hv_pai.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length22.Dispose();
                        hv_MeasureHandle.Dispose();
                        hv_RowEdge1.Dispose();
                        hv_ColumnEdge1.Dispose();
                        hv_Amplitude1.Dispose();
                        hv_Distance.Dispose();
                        hv_RowEdge2.Dispose();
                        hv_ColumnEdge2.Dispose();
                        hv_Amplitude2.Dispose();
                        hv_RowEdge.Dispose();
                        hv_ColumnEdge.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Row2.Dispose();
                        hv_Column2.Dispose();
                        hv_Phi.Dispose();
                        hv_Length13.Dispose();
                        hv_Length23.Dispose();
                        hv_exc.Dispose();

                        return;
                    }

                    hv_Area2.Dispose(); hv_Row4.Dispose(); hv_Column4.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area2, out hv_Row4, out hv_Column4);



                    //**设置找边区域
                    hv_pai.Dispose();
                    hv_pai = 3.1415926;
                    hv_Row3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row3 = hv_Row4 + 80;
                    }
                    hv_Column3.Dispose();
                    hv_Column3 = new HTuple(hv_Column4);
                    hv_Phi2.Dispose();
                    hv_Phi2 = 1.5708;
                    hv_Length12.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length12 = (480 - hv_Row4) - 140;
                    }
                    hv_Length22.Dispose();
                    hv_Length22 = 255.714;





                    //disp_cross (WindowHandle, Row1, Column1, 6, Phi1)
                    //disp_cross (WindowHandle, Row3, Column3, 6, Phi1)
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row3, hv_Column3, hv_Phi2,
                        hv_Length12, hv_Length22);

                    hv_MeasureHandle.Dispose();
                    HOperatorSet.GenMeasureRectangle2(hv_Row3, hv_Column3, hv_Phi2, hv_Length12,
                        hv_Length22, hv_Width, hv_Height, "nearest_neighbor", out hv_MeasureHandle);
                    hv_RowEdge1.Dispose(); hv_ColumnEdge1.Dispose(); hv_Amplitude1.Dispose(); hv_Distance.Dispose();
                    HOperatorSet.MeasurePos(ho_GrayImage, hv_MeasureHandle, hv_iPos_Sigma, hv_iPos_Thr1,
                        "negative", "first", out hv_RowEdge1, out hv_ColumnEdge1, out hv_Amplitude1,
                        out hv_Distance);

                    //disp_cross (WindowHandle, RowEdge1, ColumnEdge1, 6, 0)


                    hv_MeasureHandle.Dispose();
                    HOperatorSet.GenMeasureRectangle2(hv_Row3, hv_Column3, hv_Phi2 + hv_pai, hv_Length12,
                        hv_Length22, hv_Width, hv_Height, "nearest_neighbor", out hv_MeasureHandle);
                    hv_RowEdge2.Dispose(); hv_ColumnEdge2.Dispose(); hv_Amplitude2.Dispose(); hv_Distance.Dispose();
                    HOperatorSet.MeasurePos(ho_GrayImage, hv_MeasureHandle, hv_iPos_Sigma, hv_iPos_Thr2,
                        "negative", "first", out hv_RowEdge2, out hv_ColumnEdge2, out hv_Amplitude2,
                        out hv_Distance);
                    //disp_cross (WindowHandle, RowEdge2, ColumnEdge2, 6, 0)



                    //**分割点提取失败，区域显示
                    ho_Rectangle_Cemian.Dispose();
                    ho_Rectangle_Cemian = new HObject(ho_Rectangle);

                    //**两个分割点位都为提取到，判断NG-尺寸不符
                    if ((int)((new HTuple((new HTuple(hv_Amplitude1.TupleLength())).TupleEqual(
                        0))).TupleAnd(new HTuple((new HTuple(hv_Amplitude2.TupleLength())).TupleEqual(
                        0)))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 2;
                        ho_Region.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageMean.Dispose();
                        ho_RegionDynThresh.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions1.Dispose();

                        hv_iThr_Cudingwei.Dispose();
                        hv_AreaDuanmian.Dispose();
                        hv_iPos_Sigma.Dispose();
                        hv_iPos_Thr1.Dispose();
                        hv_iPos_Thr2.Dispose();
                        hv_iMaskWith.Dispose();
                        hv_iDyn_Thr.Dispose();
                        hv_iOpen1.Dispose();
                        hv_WindowHandle.Dispose();
                        hv_Width.Dispose();
                        hv_Height.Dispose();
                        hv_Area1.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Area2.Dispose();
                        hv_Row4.Dispose();
                        hv_Column4.Dispose();
                        hv_pai.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length22.Dispose();
                        hv_MeasureHandle.Dispose();
                        hv_RowEdge1.Dispose();
                        hv_ColumnEdge1.Dispose();
                        hv_Amplitude1.Dispose();
                        hv_Distance.Dispose();
                        hv_RowEdge2.Dispose();
                        hv_ColumnEdge2.Dispose();
                        hv_Amplitude2.Dispose();
                        hv_RowEdge.Dispose();
                        hv_ColumnEdge.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Row2.Dispose();
                        hv_Column2.Dispose();
                        hv_Phi.Dispose();
                        hv_Length13.Dispose();
                        hv_Length23.Dispose();
                        hv_exc.Dispose();

                        return;
                    }

                    //**两个分割点位一个提取成功，一个提取失败，
                    if ((int)(new HTuple((new HTuple(hv_Amplitude1.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        hv_RowEdge1.Dispose();
                        hv_RowEdge1 = new HTuple(hv_RowEdge2);
                        hv_ColumnEdge1.Dispose();
                        hv_ColumnEdge1 = new HTuple(hv_ColumnEdge2);
                    }

                    if ((int)(new HTuple((new HTuple(hv_Amplitude2.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        hv_RowEdge2.Dispose();
                        hv_RowEdge2 = new HTuple(hv_RowEdge1);
                        hv_ColumnEdge2.Dispose();
                        hv_ColumnEdge2 = new HTuple(hv_ColumnEdge1);
                    }

                    hv_RowEdge.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowEdge = (hv_RowEdge1 + hv_RowEdge2) / 2;
                    }
                    hv_ColumnEdge.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColumnEdge = (hv_ColumnEdge1 + hv_ColumnEdge2) / 2;
                    }
                    hv_Length11.Dispose();
                    hv_Length11 = 300;
                    hv_Length21.Dispose();
                    hv_Length21 = 100;
                    ho_Rectangle3.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_RowEdge - hv_Length21, hv_ColumnEdge,
                        0, hv_Length11, hv_Length21);


                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle3, out ho_ImageReduced
                        );
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, hv_iMaskWith, hv_iMaskWith);
                    ho_RegionDynThresh.Dispose();
                    HOperatorSet.DynThreshold(ho_ImageReduced, ho_ImageMean, out ho_RegionDynThresh,
                        hv_iDyn_Thr, "light");
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionDynThresh, out ho_RegionOpening, hv_iOpen1);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions1);
                    ho_Region_cemian.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_Region_cemian, "max_area",
                        70);
                    {
                        HObject
                          ExpTmpLocalVar_Region_cemian = new HObject(ho_Region_cemian);
                        ho_Region_cemian.Dispose();
                        ho_Region_cemian = ExpTmpLocalVar_Region_cemian;
                    }

                    hv_Row2.Dispose(); hv_Column2.Dispose(); hv_Phi.Dispose(); hv_Length13.Dispose(); hv_Length23.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_cemian, out hv_Row2, out hv_Column2,
                        out hv_Phi, out hv_Length13, out hv_Length23);
                    ho_Rectangle_Cemian.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_Cemian, hv_Row2, hv_Column2,
                        hv_Phi, hv_Length13, hv_Length23);
                    {
                        HObject
                          ExpTmpLocalVar_Rectangle_Cemian = new HObject(ho_Rectangle_Cemian);
                        ho_Rectangle_Cemian.Dispose();
                        ho_Rectangle_Cemian = ExpTmpLocalVar_Rectangle_Cemian;
                    }





                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle3.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions1.Dispose();

                hv_iThr_Cudingwei.Dispose();
                hv_AreaDuanmian.Dispose();
                hv_iPos_Sigma.Dispose();
                hv_iPos_Thr1.Dispose();
                hv_iPos_Thr2.Dispose();
                hv_iMaskWith.Dispose();
                hv_iDyn_Thr.Dispose();
                hv_iOpen1.Dispose();
                hv_WindowHandle.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Area1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Area2.Dispose();
                hv_Row4.Dispose();
                hv_Column4.Dispose();
                hv_pai.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Phi2.Dispose();
                hv_Length12.Dispose();
                hv_Length22.Dispose();
                hv_MeasureHandle.Dispose();
                hv_RowEdge1.Dispose();
                hv_ColumnEdge1.Dispose();
                hv_Amplitude1.Dispose();
                hv_Distance.Dispose();
                hv_RowEdge2.Dispose();
                hv_ColumnEdge2.Dispose();
                hv_Amplitude2.Dispose();
                hv_RowEdge.Dispose();
                hv_ColumnEdge.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_Phi.Dispose();
                hv_Length13.Dispose();
                hv_Length23.Dispose();
                hv_exc.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle3.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions1.Dispose();

                hv_iThr_Cudingwei.Dispose();
                hv_AreaDuanmian.Dispose();
                hv_iPos_Sigma.Dispose();
                hv_iPos_Thr1.Dispose();
                hv_iPos_Thr2.Dispose();
                hv_iMaskWith.Dispose();
                hv_iDyn_Thr.Dispose();
                hv_iOpen1.Dispose();
                hv_WindowHandle.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Area1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Area2.Dispose();
                hv_Row4.Dispose();
                hv_Column4.Dispose();
                hv_pai.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Phi2.Dispose();
                hv_Length12.Dispose();
                hv_Length22.Dispose();
                hv_MeasureHandle.Dispose();
                hv_RowEdge1.Dispose();
                hv_ColumnEdge1.Dispose();
                hv_Amplitude1.Dispose();
                hv_Distance.Dispose();
                hv_RowEdge2.Dispose();
                hv_ColumnEdge2.Dispose();
                hv_Amplitude2.Dispose();
                hv_RowEdge.Dispose();
                hv_ColumnEdge.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_Phi.Dispose();
                hv_Length13.Dispose();
                hv_Length23.Dispose();
                hv_exc.Dispose();

                throw HDevExpDefaultException;
            }
        }



        public static List<object> OutputFinal(HObject hoImage, HObject ho_Err_Region, HObject ho_Err_Region2, HTuple hv_NGCode, List<PointF[]> lkkPolygon, string strParams)
        {
            #region  
            if (bUseMutex) muDetect56.WaitOne();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> lsInfo2Draw = new List<string>();
            List<object> listObj2Draw = new List<object>();

            //添加元素
            listObj2Draw.Add(0); listObj2Draw.Add("NG"); listObj2Draw.Add(888);

            lsInfo2Draw.Add("黑图");
            lsInfo2Draw.Add("NG");
            listObj2Draw.Add("字符串");
            listObj2Draw.Add(lsInfo2Draw);
            listObj2Draw.Add(new PointF(1800, 100));
            return listObj2Draw;


            try
            {

                // Initialize local and output iconic variables 
                if ((int)(new HTuple(hv_NGCode.TupleNotEqual(0))) != 0)
                {
                    switch (hv_NGCode.I)
                    {
                        case 1:
                            //write_string (WindowHandle, '产品NG! NG代码为1 ,产品不完整1 ')
                            break;
                        case 2:
                            //write_string (WindowHandle, '产品NG! NG代码为2 ,产品不完整2 ')
                            break;
                        case 3:
                            //write_string (WindowHandle, '产品NG! NG代码为3 ,长宽比值超差，产品不完整3 ')
                            break;
                        case 4:
                            //write_string (WindowHandle, '产品NG! NG代码为4 ,长宽比值超差，产品不完整4 ')
                            break;
                        case 5:
                            #region***无定位-黑图
                            listObj2Draw[1] = "NG-无定位";//"NG-尺寸异常";
                            syShowRegionBorder(ho_Err_Region, ref listObj2Draw, "NG");  //显示搜索边界
                                                                                        //输出NG详情
                            lsInfo2Draw.Add("黑图");
                            lsInfo2Draw.Add("NG");
                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;

                            #endregion
                            break;
                        case 6:
                            //write_string (WindowHandle, '产品NG! NG代码为6 ,平均灰度异常 ')
                            break;
                        case 7:
                            #region****检测电阻整体长宽尺寸
                            listObj2Draw[1] = "NG-无定位";//尺寸差异较大                
                            syShowRegionBorder(ho_Err_Region, ref listObj2Draw, "NG");
                            //输出NG详情
                            //lsInfo2Draw.Add("宽度异常");
                            //lsInfo2Draw.Add("NG");
                            //lsInfo2Draw.Add("标准尺寸：" + iLength1.ToString() + " um*" + iLength2.ToString() + "um");
                            //lsInfo2Draw.Add("OK");
                            //lsInfo2Draw.Add("当前尺寸:" + hv_Length1.D.ToString("0.0") + " um * " + hv_Length2.D.ToString("0.0") + " um");
                            //lsInfo2Draw.Add("NG");

                            dhDll.frmMsg.Log("背导ok" + "5555555555555" + "," + hv_NGCode.ToString(), "", null, dhDll.logDiskMode.Error, 0);

                            listObj2Draw.Add("字符串");
                            listObj2Draw.Add(lsInfo2Draw);
                            listObj2Draw.Add(new PointF(1800, 100));
                            return listObj2Draw;
                            #endregion
                            break;
                        case 8:
                            //write_string (WindowHandle, '产品NG! NG代码为8 ,左右面积差异过大 ')

                            break;
                    }
                }
                //else
                //{

                //}
            }
            // catch (Exception) 
            catch (hvppleException HDevExpDefaultException)
            {


                throw HDevExpDefaultException;
            }


            listObj2Draw[1] = "OK";
            return listObj2Draw;
        }



        public static void Detect_Err_34(HObject ho_GrayImage, HObject ho_Region_cemian, out HObject ho_Rectangle_Cemian,
      out HObject ho_Rectangle_Search, out HObject ho_Region_Err, HTuple hv_Parameter_Err,
      out HTuple hv_NGCode)
        {




            // Local iconic variables 

            HObject ho_ImageReduced = null, ho_Region = null;
            HObject ho_ConnectedRegions = null;

            // Local control variables 

            HTuple hv_iErrShink1 = new HTuple(), hv_iErrShink2 = new HTuple();
            HTuple hv_iErrThr = new HTuple(), hv_iErrArea = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_exc = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle_Cemian);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_Search);
            HOperatorSet.GenEmptyObj(out ho_Region_Err);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            hv_NGCode = new HTuple();
            try
            {

                try
                {


                    hv_iErrShink1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrShink1 = hv_Parameter_Err.TupleSelect(
                            0);
                    }
                    hv_iErrShink2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrShink2 = hv_Parameter_Err.TupleSelect(
                            1);
                    }
                    hv_iErrThr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrThr = hv_Parameter_Err.TupleSelect(
                            2);
                    }
                    hv_iErrArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrArea = hv_Parameter_Err.TupleSelect(
                            3);
                    }

                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_cemian, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle_Cemian.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_Cemian, hv_Row, hv_Column, hv_Phi,
                        hv_Length1, hv_Length2);

                    ho_Rectangle_Search.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_Search, hv_Row, hv_Column, hv_Phi,
                        hv_Length1 * hv_iErrShink1, hv_Length2 * hv_iErrShink2);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_Rectangle_Search, out ho_ImageReduced
                        );
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 0, hv_iErrThr);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_Region_Err.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_Region_Err, "max_area",
                        70);
                    {
                        HObject
                          ExpTmpLocalVar_Region_Err = new HObject(ho_Region_Err);
                        ho_Region_Err.Dispose();
                        ho_Region_Err = ExpTmpLocalVar_Region_Err;
                    }
                    hv_Area.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_Region_Err, out hv_Area, out hv_Row1, out hv_Column1);
                    if ((int)(new HTuple(hv_Area.TupleGreater(hv_iErrArea))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 3;
                        ho_ImageReduced.Dispose();
                        ho_Region.Dispose();
                        ho_ConnectedRegions.Dispose();

                        hv_iErrShink1.Dispose();
                        hv_iErrShink2.Dispose();
                        hv_iErrThr.Dispose();
                        hv_iErrArea.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Area.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_exc.Dispose();

                        return;
                    }

                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_ImageReduced.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();

                hv_iErrShink1.Dispose();
                hv_iErrShink2.Dispose();
                hv_iErrThr.Dispose();
                hv_iErrArea.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_exc.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_ImageReduced.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();

                hv_iErrShink1.Dispose();
                hv_iErrShink2.Dispose();
                hv_iErrThr.Dispose();
                hv_iErrArea.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_exc.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void MK(HObject ho_Image, HObject ho_ModelRegion, HObject ho_Region_dianji_zhengmian,
      out HObject ho_Region_zima, out HObject ho_RegionErr1, out HObject ho_RegionErr2,
      out HObject ho_RegionMK, HTuple hv_ModelID, HTuple hv_ModelParam, HTuple hv_Parameter_MK,
      out HTuple hv_NGCode1)
        {




            // Local iconic variables 

            HObject ho_Rectangle1 = null, ho_RegionUnion1 = null;
            HObject ho_RegionTrans = null, ho_RegionUnion = null, ho_Regionrectangle2 = null;
            HObject ho_RegionDifference = null, ho_ImageMK = null, ho_Region = null;
            HObject ho_RegionOpening = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_Rectangle = null, ho_EroStruct = null;
            HObject ho_ExpImage = null, ho_ImageScaleMax = null, ho_RegionDynThresh = null;
            HObject ho_RegionClosing = null, ho_ConnectedRegions3 = null;
            HObject ho_SelectedRegions3 = null, ho_UnionMK = null, ho_RegionAffineTrans1 = null;
            HObject ho_RegionMKErr1 = null, ho_RegionMKErr1open = null;
            HObject ho_RegionMKErr2 = null, ho_RegionMKErr2open = null;
            HObject ho_ConnectedRegions1 = null, ho_ConnectedRegions2 = null;
            HObject ho_SelectedRegions1 = null, ho_SelectedRegions2 = null;

            // Local control variables 

            HTuple hv_iScore_MK = new HTuple(), hv_iArea_MK1 = new HTuple();
            HTuple hv_iArea_MK2 = new HTuple(), hv_iOpen_MK = new HTuple();
            HTuple hv_iExp_MK = new HTuple(), hv_leixing = new HTuple();
            HTuple hv_iNum_MK = new HTuple(), hv_iOpen_MK1 = new HTuple();
            HTuple hv_iOpen_MK2 = new HTuple(), hv_iOnorOff = new HTuple();
            HTuple hv_iClosing_duanxian = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Rowaaa = new HTuple(), hv_Columnaaa = new HTuple();
            HTuple hv_Phiaaa = new HTuple(), hv_Length1aaa = new HTuple();
            HTuple hv_Length2aaa = new HTuple(), hv_Number_zima = new HTuple();
            HTuple hv_Number = new HTuple(), hv_FoundRow = new HTuple();
            HTuple hv_FoundColumn = new HTuple(), hv_FoundAngle = new HTuple();
            HTuple hv_Score = new HTuple(), hv_HomMat2D = new HTuple();
            HTuple hv_Area1 = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Area2 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_exc = new HTuple(), hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_zima);
            HOperatorSet.GenEmptyObj(out ho_RegionErr1);
            HOperatorSet.GenEmptyObj(out ho_RegionErr2);
            HOperatorSet.GenEmptyObj(out ho_RegionMK);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Regionrectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ImageMK);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_EroStruct);
            HOperatorSet.GenEmptyObj(out ho_ExpImage);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_UnionMK);
            HOperatorSet.GenEmptyObj(out ho_RegionAffineTrans1);
            HOperatorSet.GenEmptyObj(out ho_RegionMKErr1);
            HOperatorSet.GenEmptyObj(out ho_RegionMKErr1open);
            HOperatorSet.GenEmptyObj(out ho_RegionMKErr2);
            HOperatorSet.GenEmptyObj(out ho_RegionMKErr2open);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            hv_NGCode1 = new HTuple();
            try
            {

                try
                {
                    hv_iScore_MK.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScore_MK = hv_Parameter_MK.TupleSelect(
                            0);
                    }
                    hv_iArea_MK1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iArea_MK1 = hv_Parameter_MK.TupleSelect(
                            1);
                    }
                    hv_iArea_MK2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iArea_MK2 = hv_Parameter_MK.TupleSelect(
                            2);
                    }
                    hv_iOpen_MK.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iOpen_MK = hv_Parameter_MK.TupleSelect(
                            3);
                    }
                    hv_iExp_MK.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iExp_MK = hv_Parameter_MK.TupleSelect(
                            4);
                    }
                    hv_leixing.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_leixing = hv_Parameter_MK.TupleSelect(
                            5);
                    }
                    hv_iNum_MK.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iNum_MK = hv_Parameter_MK.TupleSelect(
                            6);
                    }

                    hv_iOpen_MK1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iOpen_MK1 = hv_Parameter_MK.TupleSelect(
                            7);
                    }
                    hv_iOpen_MK2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iOpen_MK2 = hv_Parameter_MK.TupleSelect(
                            8);
                    }

                    hv_iOnorOff.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iOnorOff = hv_Parameter_MK.TupleSelect(
                            9);
                    }
                    hv_iClosing_duanxian.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iClosing_duanxian = hv_Parameter_MK.TupleSelect(
                            10);
                    }


                    //**字码朝上
                    //**提取字码区域

                    //**获取电极延长区域
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_dianji_zhengmian, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi,
                        hv_Length1 + 50, hv_Length2);
                    ho_RegionUnion1.Dispose();
                    HOperatorSet.Union1(ho_Rectangle1, out ho_RegionUnion1);

                    //**由电极区域到电阻区域
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_Region_dianji_zhengmian, out ho_RegionTrans, "rectangle2");
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_RegionTrans, out ho_RegionUnion);
                    ho_Regionrectangle2.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_Regionrectangle2, "rectangle2");

                    //**保护层区域
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_Regionrectangle2, ho_RegionUnion1, out ho_RegionDifference
                        );
                    ho_RegionMK.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionMK, 20,
                        25);
                    ho_ImageMK.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_RegionMK, out ho_ImageMK);

                    //**字符区域提取
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageMK, out ho_Region, 45, 255);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_Region, out ho_RegionOpening, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 800, 99999);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    hv_Rowaaa.Dispose(); hv_Columnaaa.Dispose(); hv_Phiaaa.Dispose(); hv_Length1aaa.Dispose(); hv_Length2aaa.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Rowaaa, out hv_Columnaaa,
                        out hv_Phiaaa, out hv_Length1aaa, out hv_Length2aaa);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Rowaaa, hv_Columnaaa, hv_Phiaaa,
                        hv_Length1aaa, hv_Length2aaa);
                    ho_EroStruct.Dispose();
                    HOperatorSet.GenRectangle2(out ho_EroStruct, hv_Rowaaa, hv_Columnaaa, hv_Phiaaa,
                        8, 8);
                    ho_RegionMK.Dispose();
                    HOperatorSet.Dilation1(ho_Rectangle, ho_EroStruct, out ho_RegionMK, 1);
                    ho_ImageMK.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_RegionMK, out ho_ImageMK);




                    //**字码提取
                    ho_ExpImage.Dispose();
                    HOperatorSet.ExpImage(ho_ImageMK, out ho_ExpImage, hv_iExp_MK);
                    ho_ImageScaleMax.Dispose();
                    HOperatorSet.ScaleImageMax(ho_ExpImage, out ho_ImageScaleMax);
                    ho_RegionDynThresh.Dispose();
                    HOperatorSet.Threshold(ho_ImageScaleMax, out ho_RegionDynThresh, 128, 255);

                    //字码个数判断
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionDynThresh, out ho_RegionClosing, hv_iClosing_duanxian);
                    ho_ConnectedRegions3.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions3);
                    ho_SelectedRegions3.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions3, out ho_SelectedRegions3, "area",
                        "and", 100, 99999);
                    hv_Number_zima.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions3, out hv_Number_zima);

                    //字码提取
                    //closing_circle (RegionDynThresh, RegionClosing, iClosing_duanxian)
                    ho_ConnectedRegions3.Dispose();
                    HOperatorSet.Connection(ho_RegionDynThresh, out ho_ConnectedRegions3);
                    ho_SelectedRegions3.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions3, out ho_SelectedRegions3, "area",
                        "and", 100, 99999);
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions3, out hv_Number);
                    ho_UnionMK.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions3, out ho_UnionMK);



                    //**输出字码region
                    //union1 (SelectedRegions3, UnionMK)
                    //shape_trans (UnionMK, Region_zima, 'convex')
                    //**字码的region区域
                    ho_Region_zima.Dispose();
                    ho_Region_zima = new HObject(ho_SelectedRegions3);
                    if ((int)(new HTuple(hv_iOnorOff.TupleEqual(1))) != 0)
                    {
                        if ((int)(new HTuple(hv_Number_zima.TupleNotEqual(hv_iNum_MK))) != 0)
                        {
                            hv_NGCode1.Dispose();
                            hv_NGCode1 = 29;
                            ho_Rectangle1.Dispose();
                            ho_RegionUnion1.Dispose();
                            ho_RegionTrans.Dispose();
                            ho_RegionUnion.Dispose();
                            ho_Regionrectangle2.Dispose();
                            ho_RegionDifference.Dispose();
                            ho_ImageMK.Dispose();
                            ho_Region.Dispose();
                            ho_RegionOpening.Dispose();
                            ho_ConnectedRegions.Dispose();
                            ho_SelectedRegions.Dispose();
                            ho_Rectangle.Dispose();
                            ho_EroStruct.Dispose();
                            ho_ExpImage.Dispose();
                            ho_ImageScaleMax.Dispose();
                            ho_RegionDynThresh.Dispose();
                            ho_RegionClosing.Dispose();
                            ho_ConnectedRegions3.Dispose();
                            ho_SelectedRegions3.Dispose();
                            ho_UnionMK.Dispose();
                            ho_RegionAffineTrans1.Dispose();
                            ho_RegionMKErr1.Dispose();
                            ho_RegionMKErr1open.Dispose();
                            ho_RegionMKErr2.Dispose();
                            ho_RegionMKErr2open.Dispose();
                            ho_ConnectedRegions1.Dispose();
                            ho_ConnectedRegions2.Dispose();
                            ho_SelectedRegions1.Dispose();
                            ho_SelectedRegions2.Dispose();

                            hv_iScore_MK.Dispose();
                            hv_iArea_MK1.Dispose();
                            hv_iArea_MK2.Dispose();
                            hv_iOpen_MK.Dispose();
                            hv_iExp_MK.Dispose();
                            hv_leixing.Dispose();
                            hv_iNum_MK.Dispose();
                            hv_iOpen_MK1.Dispose();
                            hv_iOpen_MK2.Dispose();
                            hv_iOnorOff.Dispose();
                            hv_iClosing_duanxian.Dispose();
                            hv_Row.Dispose();
                            hv_Column.Dispose();
                            hv_Phi.Dispose();
                            hv_Length1.Dispose();
                            hv_Length2.Dispose();
                            hv_Rowaaa.Dispose();
                            hv_Columnaaa.Dispose();
                            hv_Phiaaa.Dispose();
                            hv_Length1aaa.Dispose();
                            hv_Length2aaa.Dispose();
                            hv_Number_zima.Dispose();
                            hv_Number.Dispose();
                            hv_FoundRow.Dispose();
                            hv_FoundColumn.Dispose();
                            hv_FoundAngle.Dispose();
                            hv_Score.Dispose();
                            hv_HomMat2D.Dispose();
                            hv_Area1.Dispose();
                            hv_Row1.Dispose();
                            hv_Column1.Dispose();
                            hv_Area2.Dispose();
                            hv_Row2.Dispose();
                            hv_Column2.Dispose();
                            hv_exc.Dispose();
                            hv_NGCode.Dispose();

                            return;
                        }

                    }



                    hv_FoundRow.Dispose(); hv_FoundColumn.Dispose(); hv_FoundAngle.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindShapeModel(ho_ImageMK, hv_ModelID, 0, 360, hv_iScore_MK,
                        1, 0, "least_squares", 0, 0.9, out hv_FoundRow, out hv_FoundColumn, out hv_FoundAngle,
                        out hv_Score);

                    if ((int)(new HTuple(hv_Score.TupleGreater(0))) != 0)
                    {
                        hv_HomMat2D.Dispose();
                        HOperatorSet.VectorAngleToRigid(hv_ModelParam.TupleSelect(0), hv_ModelParam.TupleSelect(
                            1), hv_ModelParam.TupleSelect(2), hv_FoundRow, hv_FoundColumn, hv_FoundAngle,
                            out hv_HomMat2D);
                        ho_RegionAffineTrans1.Dispose();
                        HOperatorSet.AffineTransRegion(ho_ModelRegion, out ho_RegionAffineTrans1,
                            hv_HomMat2D, "true");

                        //**求缺失
                        ho_RegionMKErr1.Dispose();
                        HOperatorSet.Difference(ho_RegionAffineTrans1, ho_UnionMK, out ho_RegionMKErr1
                            );
                        ho_RegionMKErr1open.Dispose();
                        HOperatorSet.OpeningCircle(ho_RegionMKErr1, out ho_RegionMKErr1open, hv_iOpen_MK1);
                        //**求多余
                        ho_RegionMKErr2.Dispose();
                        HOperatorSet.Difference(ho_UnionMK, ho_RegionAffineTrans1, out ho_RegionMKErr2
                            );
                        ho_RegionMKErr2open.Dispose();
                        HOperatorSet.OpeningCircle(ho_RegionMKErr2, out ho_RegionMKErr2open, hv_iOpen_MK2);

                        ho_ConnectedRegions1.Dispose();
                        HOperatorSet.Connection(ho_RegionMKErr1open, out ho_ConnectedRegions1);
                        ho_ConnectedRegions2.Dispose();
                        HOperatorSet.Connection(ho_RegionMKErr2open, out ho_ConnectedRegions2);

                        ho_SelectedRegions1.Dispose();
                        HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1,
                            "max_area", 70);
                        ho_SelectedRegions2.Dispose();
                        HOperatorSet.SelectShapeStd(ho_ConnectedRegions2, out ho_SelectedRegions2,
                            "max_area", 70);

                        ho_RegionErr1.Dispose();
                        ho_RegionErr1 = new HObject(ho_SelectedRegions1);
                        ho_RegionErr2.Dispose();
                        ho_RegionErr2 = new HObject(ho_SelectedRegions2);
                        hv_Area1.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                        HOperatorSet.AreaCenter(ho_SelectedRegions1, out hv_Area1, out hv_Row1,
                            out hv_Column1);
                        hv_Area2.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                        HOperatorSet.AreaCenter(ho_SelectedRegions2, out hv_Area2, out hv_Row2,
                            out hv_Column2);
                        if ((int)(new HTuple(hv_Area1.TupleGreater(hv_iArea_MK1))) != 0)
                        {
                            hv_NGCode1.Dispose();
                            hv_NGCode1 = 1;
                            ho_Rectangle1.Dispose();
                            ho_RegionUnion1.Dispose();
                            ho_RegionTrans.Dispose();
                            ho_RegionUnion.Dispose();
                            ho_Regionrectangle2.Dispose();
                            ho_RegionDifference.Dispose();
                            ho_ImageMK.Dispose();
                            ho_Region.Dispose();
                            ho_RegionOpening.Dispose();
                            ho_ConnectedRegions.Dispose();
                            ho_SelectedRegions.Dispose();
                            ho_Rectangle.Dispose();
                            ho_EroStruct.Dispose();
                            ho_ExpImage.Dispose();
                            ho_ImageScaleMax.Dispose();
                            ho_RegionDynThresh.Dispose();
                            ho_RegionClosing.Dispose();
                            ho_ConnectedRegions3.Dispose();
                            ho_SelectedRegions3.Dispose();
                            ho_UnionMK.Dispose();
                            ho_RegionAffineTrans1.Dispose();
                            ho_RegionMKErr1.Dispose();
                            ho_RegionMKErr1open.Dispose();
                            ho_RegionMKErr2.Dispose();
                            ho_RegionMKErr2open.Dispose();
                            ho_ConnectedRegions1.Dispose();
                            ho_ConnectedRegions2.Dispose();
                            ho_SelectedRegions1.Dispose();
                            ho_SelectedRegions2.Dispose();

                            hv_iScore_MK.Dispose();
                            hv_iArea_MK1.Dispose();
                            hv_iArea_MK2.Dispose();
                            hv_iOpen_MK.Dispose();
                            hv_iExp_MK.Dispose();
                            hv_leixing.Dispose();
                            hv_iNum_MK.Dispose();
                            hv_iOpen_MK1.Dispose();
                            hv_iOpen_MK2.Dispose();
                            hv_iOnorOff.Dispose();
                            hv_iClosing_duanxian.Dispose();
                            hv_Row.Dispose();
                            hv_Column.Dispose();
                            hv_Phi.Dispose();
                            hv_Length1.Dispose();
                            hv_Length2.Dispose();
                            hv_Rowaaa.Dispose();
                            hv_Columnaaa.Dispose();
                            hv_Phiaaa.Dispose();
                            hv_Length1aaa.Dispose();
                            hv_Length2aaa.Dispose();
                            hv_Number_zima.Dispose();
                            hv_Number.Dispose();
                            hv_FoundRow.Dispose();
                            hv_FoundColumn.Dispose();
                            hv_FoundAngle.Dispose();
                            hv_Score.Dispose();
                            hv_HomMat2D.Dispose();
                            hv_Area1.Dispose();
                            hv_Row1.Dispose();
                            hv_Column1.Dispose();
                            hv_Area2.Dispose();
                            hv_Row2.Dispose();
                            hv_Column2.Dispose();
                            hv_exc.Dispose();
                            hv_NGCode.Dispose();

                            return;
                        }

                        if ((int)(new HTuple(hv_Area2.TupleGreater(hv_iArea_MK2))) != 0)
                        {
                            hv_NGCode1.Dispose();
                            hv_NGCode1 = 2;
                            ho_Rectangle1.Dispose();
                            ho_RegionUnion1.Dispose();
                            ho_RegionTrans.Dispose();
                            ho_RegionUnion.Dispose();
                            ho_Regionrectangle2.Dispose();
                            ho_RegionDifference.Dispose();
                            ho_ImageMK.Dispose();
                            ho_Region.Dispose();
                            ho_RegionOpening.Dispose();
                            ho_ConnectedRegions.Dispose();
                            ho_SelectedRegions.Dispose();
                            ho_Rectangle.Dispose();
                            ho_EroStruct.Dispose();
                            ho_ExpImage.Dispose();
                            ho_ImageScaleMax.Dispose();
                            ho_RegionDynThresh.Dispose();
                            ho_RegionClosing.Dispose();
                            ho_ConnectedRegions3.Dispose();
                            ho_SelectedRegions3.Dispose();
                            ho_UnionMK.Dispose();
                            ho_RegionAffineTrans1.Dispose();
                            ho_RegionMKErr1.Dispose();
                            ho_RegionMKErr1open.Dispose();
                            ho_RegionMKErr2.Dispose();
                            ho_RegionMKErr2open.Dispose();
                            ho_ConnectedRegions1.Dispose();
                            ho_ConnectedRegions2.Dispose();
                            ho_SelectedRegions1.Dispose();
                            ho_SelectedRegions2.Dispose();

                            hv_iScore_MK.Dispose();
                            hv_iArea_MK1.Dispose();
                            hv_iArea_MK2.Dispose();
                            hv_iOpen_MK.Dispose();
                            hv_iExp_MK.Dispose();
                            hv_leixing.Dispose();
                            hv_iNum_MK.Dispose();
                            hv_iOpen_MK1.Dispose();
                            hv_iOpen_MK2.Dispose();
                            hv_iOnorOff.Dispose();
                            hv_iClosing_duanxian.Dispose();
                            hv_Row.Dispose();
                            hv_Column.Dispose();
                            hv_Phi.Dispose();
                            hv_Length1.Dispose();
                            hv_Length2.Dispose();
                            hv_Rowaaa.Dispose();
                            hv_Columnaaa.Dispose();
                            hv_Phiaaa.Dispose();
                            hv_Length1aaa.Dispose();
                            hv_Length2aaa.Dispose();
                            hv_Number_zima.Dispose();
                            hv_Number.Dispose();
                            hv_FoundRow.Dispose();
                            hv_FoundColumn.Dispose();
                            hv_FoundAngle.Dispose();
                            hv_Score.Dispose();
                            hv_HomMat2D.Dispose();
                            hv_Area1.Dispose();
                            hv_Row1.Dispose();
                            hv_Column1.Dispose();
                            hv_Area2.Dispose();
                            hv_Row2.Dispose();
                            hv_Column2.Dispose();
                            hv_exc.Dispose();
                            hv_NGCode.Dispose();

                            return;
                        }

                    }
                    else
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 28;
                        ho_Rectangle1.Dispose();
                        ho_RegionUnion1.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_Regionrectangle2.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_ImageMK.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_Rectangle.Dispose();
                        ho_EroStruct.Dispose();
                        ho_ExpImage.Dispose();
                        ho_ImageScaleMax.Dispose();
                        ho_RegionDynThresh.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions3.Dispose();
                        ho_SelectedRegions3.Dispose();
                        ho_UnionMK.Dispose();
                        ho_RegionAffineTrans1.Dispose();
                        ho_RegionMKErr1.Dispose();
                        ho_RegionMKErr1open.Dispose();
                        ho_RegionMKErr2.Dispose();
                        ho_RegionMKErr2open.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_SelectedRegions2.Dispose();

                        hv_iScore_MK.Dispose();
                        hv_iArea_MK1.Dispose();
                        hv_iArea_MK2.Dispose();
                        hv_iOpen_MK.Dispose();
                        hv_iExp_MK.Dispose();
                        hv_leixing.Dispose();
                        hv_iNum_MK.Dispose();
                        hv_iOpen_MK1.Dispose();
                        hv_iOpen_MK2.Dispose();
                        hv_iOnorOff.Dispose();
                        hv_iClosing_duanxian.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Rowaaa.Dispose();
                        hv_Columnaaa.Dispose();
                        hv_Phiaaa.Dispose();
                        hv_Length1aaa.Dispose();
                        hv_Length2aaa.Dispose();
                        hv_Number_zima.Dispose();
                        hv_Number.Dispose();
                        hv_FoundRow.Dispose();
                        hv_FoundColumn.Dispose();
                        hv_FoundAngle.Dispose();
                        hv_Score.Dispose();
                        hv_HomMat2D.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_Area2.Dispose();
                        hv_Row2.Dispose();
                        hv_Column2.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }




                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }

                ho_Rectangle1.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionTrans.Dispose();
                ho_RegionUnion.Dispose();
                ho_Regionrectangle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageMK.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_Rectangle.Dispose();
                ho_EroStruct.Dispose();
                ho_ExpImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_UnionMK.Dispose();
                ho_RegionAffineTrans1.Dispose();
                ho_RegionMKErr1.Dispose();
                ho_RegionMKErr1open.Dispose();
                ho_RegionMKErr2.Dispose();
                ho_RegionMKErr2open.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_SelectedRegions2.Dispose();

                hv_iScore_MK.Dispose();
                hv_iArea_MK1.Dispose();
                hv_iArea_MK2.Dispose();
                hv_iOpen_MK.Dispose();
                hv_iExp_MK.Dispose();
                hv_leixing.Dispose();
                hv_iNum_MK.Dispose();
                hv_iOpen_MK1.Dispose();
                hv_iOpen_MK2.Dispose();
                hv_iOnorOff.Dispose();
                hv_iClosing_duanxian.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Rowaaa.Dispose();
                hv_Columnaaa.Dispose();
                hv_Phiaaa.Dispose();
                hv_Length1aaa.Dispose();
                hv_Length2aaa.Dispose();
                hv_Number_zima.Dispose();
                hv_Number.Dispose();
                hv_FoundRow.Dispose();
                hv_FoundColumn.Dispose();
                hv_FoundAngle.Dispose();
                hv_Score.Dispose();
                hv_HomMat2D.Dispose();
                hv_Area1.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Area2.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Rectangle1.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionTrans.Dispose();
                ho_RegionUnion.Dispose();
                ho_Regionrectangle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_ImageMK.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_Rectangle.Dispose();
                ho_EroStruct.Dispose();
                ho_ExpImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_UnionMK.Dispose();
                ho_RegionAffineTrans1.Dispose();
                ho_RegionMKErr1.Dispose();
                ho_RegionMKErr1open.Dispose();
                ho_RegionMKErr2.Dispose();
                ho_RegionMKErr2open.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_SelectedRegions2.Dispose();

                hv_iScore_MK.Dispose();
                hv_iArea_MK1.Dispose();
                hv_iArea_MK2.Dispose();
                hv_iOpen_MK.Dispose();
                hv_iExp_MK.Dispose();
                hv_leixing.Dispose();
                hv_iNum_MK.Dispose();
                hv_iOpen_MK1.Dispose();
                hv_iOpen_MK2.Dispose();
                hv_iOnorOff.Dispose();
                hv_iClosing_duanxian.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Rowaaa.Dispose();
                hv_Columnaaa.Dispose();
                hv_Phiaaa.Dispose();
                hv_Length1aaa.Dispose();
                hv_Length2aaa.Dispose();
                hv_Number_zima.Dispose();
                hv_Number.Dispose();
                hv_FoundRow.Dispose();
                hv_FoundColumn.Dispose();
                hv_FoundAngle.Dispose();
                hv_Score.Dispose();
                hv_HomMat2D.Dispose();
                hv_Area1.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Area2.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }






        public static void wudingwei(HObject ho_GrayImage, out HObject ho_Region_cudingwei, out HObject ho_Rectangle1_wudingwei,
      out HObject ho_Rectangle2_wudingwei, HTuple hv_ipix, HTuple hv_iFixThres, HTuple hv_iArea1,
      HTuple hv_iLength1, HTuple hv_iLength2, HTuple hv_iLength1Scale, HTuple hv_iLength2Scale,
      HTuple hv_iAngleScale, HTuple hv_iOpeWidth1, HTuple hv_iOpeHeight, HTuple hv_iRowMin,
      HTuple hv_iArea2, out HTuple hv_NGCode1, out HTuple hv_Deg, out HTuple hv_Length1,
      out HTuple hv_Length2)
        {




            // Local iconic variables 

            HObject ho_Region = null, ho_RegionOpening = null;
            HObject ho_ConnectedRegions = null, ho_Rectangle7 = null, ho_RegionUnion5 = null;
            HObject ho_RegionTrans2 = null, ho_Rectangle3 = null, ho_Rectangle4 = null;

            // Local control variables 

            HTuple hv_Width1 = new HTuple(), hv_Height1 = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Row16 = new HTuple();
            HTuple hv_Column16 = new HTuple(), hv_Phi4 = new HTuple();
            HTuple hv_Length14 = new HTuple(), hv_Length24 = new HTuple();
            HTuple hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
            HTuple hv_Row12 = new HTuple(), hv_Column12 = new HTuple();
            HTuple hv_Phi2 = new HTuple(), hv_Length12 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_exc = new HTuple();
            HTuple hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_cudingwei);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1_wudingwei);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2_wudingwei);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle7);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion5);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans2);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_Rectangle4);
            hv_NGCode1 = new HTuple();
            hv_Deg = new HTuple();
            hv_Length1 = new HTuple();
            hv_Length2 = new HTuple();
            try
            {
                //*****
                try
                {


                    hv_Width1.Dispose(); hv_Height1.Dispose();
                    HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width1, out hv_Height1);


                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_GrayImage, out ho_Region, hv_iFixThres, 255);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_Region, out ho_RegionOpening, hv_iOpeWidth1,
                        hv_iOpeHeight);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    ho_Region_cudingwei.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_Region_cudingwei, "area",
                        "and", hv_iArea1, 999999);

                    //**黑图检测
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_Region_cudingwei, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 5;
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle7.Dispose();
                        ho_RegionUnion5.Dispose();
                        ho_RegionTrans2.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_Rectangle4.Dispose();

                        hv_Width1.Dispose();
                        hv_Height1.Dispose();
                        hv_Number.Dispose();
                        hv_Row16.Dispose();
                        hv_Column16.Dispose();
                        hv_Phi4.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_Row11.Dispose();
                        hv_Column11.Dispose();
                        hv_Row21.Dispose();
                        hv_Column21.Dispose();
                        hv_Row12.Dispose();
                        hv_Column12.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length21.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;

                    }


                    hv_Row16.Dispose(); hv_Column16.Dispose(); hv_Phi4.Dispose(); hv_Length14.Dispose(); hv_Length24.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_cudingwei, out hv_Row16, out hv_Column16,
                        out hv_Phi4, out hv_Length14, out hv_Length24);
                    ho_Rectangle7.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle7, hv_Row16, hv_Column16, hv_Phi4,
                        hv_Length14, hv_Length24);
                    ho_RegionUnion5.Dispose();
                    HOperatorSet.Union1(ho_Region_cudingwei, out ho_RegionUnion5);
                    ho_RegionTrans2.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion5, out ho_RegionTrans2, "convex");

                    //粗定位-smallest_rectangle1-检测靠近边界
                    hv_Row11.Dispose(); hv_Column11.Dispose(); hv_Row21.Dispose(); hv_Column21.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_RegionTrans2, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    ho_Rectangle3.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle3, hv_Row11, hv_Column11, hv_Row21,
                        hv_Column21);
                    //粗定位-smallest_rectangle2-检测角度以及尺寸
                    hv_Row12.Dispose(); hv_Column12.Dispose(); hv_Phi2.Dispose(); hv_Length12.Dispose(); hv_Length21.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans2, out hv_Row12, out hv_Column12,
                        out hv_Phi2, out hv_Length12, out hv_Length21);
                    ho_Rectangle4.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle4, hv_Row12, hv_Column12, hv_Phi2,
                        hv_Length12, hv_Length21);

                    ho_Rectangle2_wudingwei.Dispose();
                    ho_Rectangle2_wudingwei = new HObject(ho_Rectangle4);
                    ho_Rectangle1_wudingwei.Dispose();
                    ho_Rectangle1_wudingwei = new HObject(ho_Rectangle3);

                    //**无定位-产品角度
                    hv_Deg.Dispose();
                    HOperatorSet.TupleDeg(hv_Phi2, out hv_Deg);
                    if ((int)(new HTuple(((hv_Deg.TupleAbs())).TupleGreater(hv_iAngleScale))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 6;
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle7.Dispose();
                        ho_RegionUnion5.Dispose();
                        ho_RegionTrans2.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_Rectangle4.Dispose();

                        hv_Width1.Dispose();
                        hv_Height1.Dispose();
                        hv_Number.Dispose();
                        hv_Row16.Dispose();
                        hv_Column16.Dispose();
                        hv_Phi4.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_Row11.Dispose();
                        hv_Column11.Dispose();
                        hv_Row21.Dispose();
                        hv_Column21.Dispose();
                        hv_Row12.Dispose();
                        hv_Column12.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length21.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }

                    //**无定位-产品尺寸
                    hv_Length1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length1 = ((hv_Length12 * 2) * hv_ipix) * 1000;
                    }
                    hv_Length2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length2 = ((hv_Length21 * 2) * hv_ipix) * 1000;
                    }
                    if ((int)(new HTuple(((((hv_Length1 - hv_iLength1)).TupleAbs())).TupleGreater(
                        hv_iLength1Scale))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 7;
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle7.Dispose();
                        ho_RegionUnion5.Dispose();
                        ho_RegionTrans2.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_Rectangle4.Dispose();

                        hv_Width1.Dispose();
                        hv_Height1.Dispose();
                        hv_Number.Dispose();
                        hv_Row16.Dispose();
                        hv_Column16.Dispose();
                        hv_Phi4.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_Row11.Dispose();
                        hv_Column11.Dispose();
                        hv_Row21.Dispose();
                        hv_Column21.Dispose();
                        hv_Row12.Dispose();
                        hv_Column12.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length21.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }

                    if ((int)(new HTuple(((((hv_Length2 - hv_iLength2)).TupleAbs())).TupleGreater(
                        hv_iLength2Scale))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 8;
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle7.Dispose();
                        ho_RegionUnion5.Dispose();
                        ho_RegionTrans2.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_Rectangle4.Dispose();

                        hv_Width1.Dispose();
                        hv_Height1.Dispose();
                        hv_Number.Dispose();
                        hv_Row16.Dispose();
                        hv_Column16.Dispose();
                        hv_Phi4.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_Row11.Dispose();
                        hv_Column11.Dispose();
                        hv_Row21.Dispose();
                        hv_Column21.Dispose();
                        hv_Row12.Dispose();
                        hv_Column12.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length21.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }

                    //**无定位-产品接近边界
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_Row11.TupleLess(hv_iRowMin))).TupleOr(
                        new HTuple(hv_Column11.TupleLess(hv_iRowMin))))).TupleOr(new HTuple(hv_Column21.TupleGreater(
                        hv_Width1 - hv_iRowMin))))).TupleOr(new HTuple(hv_Row21.TupleGreater(hv_Height1 - hv_iRowMin)))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 9;
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle7.Dispose();
                        ho_RegionUnion5.Dispose();
                        ho_RegionTrans2.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_Rectangle4.Dispose();

                        hv_Width1.Dispose();
                        hv_Height1.Dispose();
                        hv_Number.Dispose();
                        hv_Row16.Dispose();
                        hv_Column16.Dispose();
                        hv_Phi4.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_Row11.Dispose();
                        hv_Column11.Dispose();
                        hv_Row21.Dispose();
                        hv_Column21.Dispose();
                        hv_Row12.Dispose();
                        hv_Column12.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length12.Dispose();
                        hv_Length21.Dispose();
                        hv_exc.Dispose();
                        hv_NGCode.Dispose();

                        return;
                    }

                    ho_Region_cudingwei.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_Region_cudingwei, "area",
                        "and", hv_iArea2, 999999);
                    //Region_cudingwei := Region_cudingwei

                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_Rectangle7.Dispose();
                ho_RegionUnion5.Dispose();
                ho_RegionTrans2.Dispose();
                ho_Rectangle3.Dispose();
                ho_Rectangle4.Dispose();

                hv_Width1.Dispose();
                hv_Height1.Dispose();
                hv_Number.Dispose();
                hv_Row16.Dispose();
                hv_Column16.Dispose();
                hv_Phi4.Dispose();
                hv_Length14.Dispose();
                hv_Length24.Dispose();
                hv_Row11.Dispose();
                hv_Column11.Dispose();
                hv_Row21.Dispose();
                hv_Column21.Dispose();
                hv_Row12.Dispose();
                hv_Column12.Dispose();
                hv_Phi2.Dispose();
                hv_Length12.Dispose();
                hv_Length21.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_Rectangle7.Dispose();
                ho_RegionUnion5.Dispose();
                ho_RegionTrans2.Dispose();
                ho_Rectangle3.Dispose();
                ho_Rectangle4.Dispose();

                hv_Width1.Dispose();
                hv_Height1.Dispose();
                hv_Number.Dispose();
                hv_Row16.Dispose();
                hv_Column16.Dispose();
                hv_Phi4.Dispose();
                hv_Length14.Dispose();
                hv_Length24.Dispose();
                hv_Row11.Dispose();
                hv_Column11.Dispose();
                hv_Row21.Dispose();
                hv_Column21.Dispose();
                hv_Row12.Dispose();
                hv_Column12.Dispose();
                hv_Phi2.Dispose();
                hv_Length12.Dispose();
                hv_Length21.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void shangpabuzu3(HObject ho_ImageReduced, HObject ho_Region_dianji, out HObject ho_RegionErr,
      out HObject ho_RegionDetection_shangpa, HTuple hv_Parameter_SP, out HTuple hv_NGCode1)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_SortedRegions = null, ho_Rectangle4 = null;
            HObject ho_RegionDilation = null, ho_RegionUnion2 = null, ho_ImageReduced2 = null;
            HObject ho_ImageScaled = null, ho_Region = null, ho_ConnectedRegions2 = null;
            HObject ho_SelectedRegions1 = null, ho_SortedRegions1 = null;
            HObject ho_RegionUnion1 = null, ho_RegionTrans = null, ho_Rectangle = null;
            HObject ho_Rectangle_33 = null, ho_Rectangle_1 = null, ho_RegionUnion_11 = null;
            HObject ho_Rectangle_std = null, ho_Rectangle_11 = null, ho_Rectangle_22 = null;
            HObject ho_RegionUnion_std = null, ho_RegionErosion = null;
            HObject ho_RegionIntersection = null, ho_ConnectedRegions = null;
            HObject ho_Rectangle3 = null, ho_RegionDifference = null, ho_RegionIntersection1 = null;
            HObject ho_ConnectedRegions1 = null, ho_RegionOpening = null;
            HObject ho_RegionIntersection2 = null, ho_ConnectedRegions3 = null;
            HObject ho_SelectedRegions = null, ho_RegionUnion = null;

            // Local control variables 

            HTuple hv_leixing = new HTuple(), hv_iShangpa = new HTuple();
            HTuple hv_iScale_width_2 = new HTuple(), hv_iScale_height_2 = new HTuple();
            HTuple hv_iArea_shangpa1 = new HTuple(), hv_iScale_height_3 = new HTuple();
            HTuple hv_iScale_width_1 = new HTuple(), hv_iScale_height_1 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Deg = new HTuple(), hv_Row1 = new HTuple(), hv_hv_L = new HTuple();
            HTuple hv_column1 = new HTuple(), hv_hv_L1 = new HTuple();
            HTuple hv_column2 = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Length11 = new HTuple(), hv_Length21 = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row4 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionErr);
            HOperatorSet.GenEmptyObj(out ho_RegionDetection_shangpa);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle4);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_33);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion_11);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_std);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_11);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_22);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion_std);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            hv_NGCode1 = new HTuple();
            try
            {
                //*****
                try
                {
                    hv_leixing.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_leixing = hv_Parameter_SP.TupleSelect(
                            0);
                    }
                    hv_iShangpa.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iShangpa = hv_Parameter_SP.TupleSelect(
                            1);
                    }
                    hv_iScale_width_2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScale_width_2 = hv_Parameter_SP.TupleSelect(
                            2);
                    }
                    hv_iScale_height_2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScale_height_2 = hv_Parameter_SP.TupleSelect(
                            3);
                    }
                    hv_iArea_shangpa1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iArea_shangpa1 = hv_Parameter_SP.TupleSelect(
                            4);
                    }
                    hv_iScale_height_3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScale_height_3 = hv_Parameter_SP.TupleSelect(
                            5);
                    }
                    hv_iScale_width_1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScale_width_1 = hv_Parameter_SP.TupleSelect(
                            6);
                    }
                    hv_iScale_height_1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iScale_height_1 = hv_Parameter_SP.TupleSelect(
                            7);
                    }
                    //**将电极从左到右排序
                    ho_SortedRegions.Dispose();
                    HOperatorSet.SortRegion(ho_Region_dianji, out ho_SortedRegions, "first_point",
                        "true", "column");
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_SortedRegions, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle4.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle4, hv_Row, hv_Column, hv_Phi,
                        hv_Length1, hv_Length2);
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_Rectangle4, out ho_RegionDilation, 11,
                        11);
                    ho_RegionUnion2.Dispose();
                    HOperatorSet.Union1(ho_RegionDilation, out ho_RegionUnion2);
                    ho_ImageReduced2.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionUnion2, out ho_ImageReduced2
                        );

                    //**精确定位电极
                    ho_ImageScaled.Dispose();
                    HOperatorSet.ScaleImage(ho_ImageReduced2, out ho_ImageScaled, 3, -100);
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageScaled, out ho_Region, 128, 255);
                    ho_ConnectedRegions2.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions2);
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions1, "area",
                        "and", 8000, 99999);
                    ho_SortedRegions1.Dispose();
                    HOperatorSet.SortRegion(ho_SelectedRegions1, out ho_SortedRegions1, "first_point",
                        "true", "column");


                    ho_RegionUnion1.Dispose();
                    HOperatorSet.Union1(ho_SortedRegions1, out ho_RegionUnion1);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion1, out ho_RegionTrans, "rectangle2");
                    hv_Area.Dispose(); hv_Row2.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_SortedRegions, out hv_Area, out hv_Row2, out hv_Column1);
                    //**电极区域矩形
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1,
                        hv_Length2);
                    //**电极区域矩形-电极两端不捡
                    ho_Rectangle_33.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_33, hv_Row, hv_Column, hv_Phi,
                        hv_Length1 - hv_iScale_height_3, hv_Length2);


                    hv_Deg.Dispose();
                    HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);
                    hv_Row1.Dispose();
                    hv_Row1 = new HTuple(hv_Row);
                    hv_hv_L.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_hv_L = (hv_Area / (hv_Length1 * 2)) / 2;
                    }

                    if (hv_column1 == null)
                        hv_column1 = new HTuple();
                    hv_column1[0] = ((hv_Column.TupleSelect(0)) - (hv_Length2.TupleSelect(0))) + (hv_hv_L.TupleSelect(
                        0));
                    if (hv_column1 == null)
                        hv_column1 = new HTuple();
                    hv_column1[1] = ((hv_Column.TupleSelect(1)) + (hv_Length2.TupleSelect(1))) - (hv_hv_L.TupleSelect(
                        1));

                    //**使用平均宽度绘制的检测范围
                    ho_Rectangle_1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_1, hv_Row1, hv_column1, hv_Phi,
                        hv_Length1, hv_hv_L);
                    ho_RegionUnion_11.Dispose();
                    HOperatorSet.Union1(ho_Rectangle_1, out ho_RegionUnion_11);
                    ho_Rectangle_std.Dispose();
                    ho_Rectangle_std = new HObject(ho_Rectangle_1);
                    ho_Rectangle_11.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Rectangle_1, out ho_Rectangle_11, hv_iScale_width_1,
                        hv_iScale_height_1);

                    //************
                    hv_Row2.Dispose();
                    hv_Row2 = new HTuple(hv_Row);
                    hv_hv_L1.Dispose();
                    hv_hv_L1 = new HTuple(hv_iScale_width_2);
                    hv_hv_L.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_hv_L = new HTuple();
                        hv_hv_L = hv_hv_L.TupleConcat(hv_hv_L1);
                        hv_hv_L = hv_hv_L.TupleConcat(hv_hv_L1);
                    }

                    if (hv_column2 == null)
                        hv_column2 = new HTuple();
                    hv_column2[0] = ((hv_Column.TupleSelect(0)) - (hv_Length2.TupleSelect(0))) + (hv_hv_L.TupleSelect(
                        0));
                    if (hv_column2 == null)
                        hv_column2 = new HTuple();
                    hv_column2[1] = ((hv_Column.TupleSelect(1)) + (hv_Length2.TupleSelect(1))) - (hv_hv_L.TupleSelect(
                        1));
                    //**使用自定义宽度绘制的检测范围
                    ho_Rectangle_22.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_22, hv_Row2, hv_column2, hv_Phi,
                        hv_Length1 - hv_iScale_height_2, hv_hv_L);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Union1(ho_Rectangle_22, out ExpTmpOutVar_0);
                        ho_Rectangle_22.Dispose();
                        ho_Rectangle_22 = ExpTmpOutVar_0;
                    }



                    if ((int)(new HTuple(hv_iShangpa.TupleEqual(1))) != 0)
                    {
                        ho_RegionUnion_std.Dispose();
                        ho_RegionUnion_std = new HObject(ho_Rectangle_11);
                    }
                    else if ((int)(new HTuple(hv_iShangpa.TupleEqual(2))) != 0)
                    {
                        ho_RegionUnion_std.Dispose();
                        ho_RegionUnion_std = new HObject(ho_Rectangle_22);
                    }
                    else if ((int)(new HTuple(hv_iShangpa.TupleEqual(3))) != 0)
                    {
                        ho_RegionUnion_std.Dispose();
                        ho_RegionUnion_std = new HObject(ho_Rectangle_33);
                    }
                    else
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 34;
                        ho_SortedRegions.Dispose();
                        ho_Rectangle4.Dispose();
                        ho_RegionDilation.Dispose();
                        ho_RegionUnion2.Dispose();
                        ho_ImageReduced2.Dispose();
                        ho_ImageScaled.Dispose();
                        ho_Region.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_SortedRegions1.Dispose();
                        ho_RegionUnion1.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rectangle_33.Dispose();
                        ho_Rectangle_1.Dispose();
                        ho_RegionUnion_11.Dispose();
                        ho_Rectangle_std.Dispose();
                        ho_Rectangle_11.Dispose();
                        ho_Rectangle_22.Dispose();
                        ho_RegionUnion_std.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_RegionIntersection.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionIntersection1.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionIntersection2.Dispose();
                        ho_ConnectedRegions3.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();

                        hv_leixing.Dispose();
                        hv_iShangpa.Dispose();
                        hv_iScale_width_2.Dispose();
                        hv_iScale_height_2.Dispose();
                        hv_iArea_shangpa1.Dispose();
                        hv_iScale_height_3.Dispose();
                        hv_iScale_width_1.Dispose();
                        hv_iScale_height_1.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Area.Dispose();
                        hv_Row2.Dispose();
                        hv_Column1.Dispose();
                        hv_Deg.Dispose();
                        hv_Row1.Dispose();
                        hv_hv_L.Dispose();
                        hv_column1.Dispose();
                        hv_hv_L1.Dispose();
                        hv_column2.Dispose();
                        hv_Row3.Dispose();
                        hv_Column2.Dispose();
                        hv_Phi1.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row4.Dispose();
                        hv_Column3.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    //**检测区域输出
                    ho_RegionDetection_shangpa.Dispose();
                    ho_RegionDetection_shangpa = new HObject(ho_RegionUnion_std);



                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_Rectangle_std, out ho_RegionErosion, 5,
                        5);
                    ho_RegionIntersection.Dispose();
                    HOperatorSet.Intersection(ho_RegionErosion, ho_Region_dianji, out ho_RegionIntersection
                        );
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionIntersection, out ho_ConnectedRegions);
                    hv_Row3.Dispose(); hv_Column2.Dispose(); hv_Phi1.Dispose(); hv_Length11.Dispose(); hv_Length21.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_ConnectedRegions, out hv_Row3, out hv_Column2,
                        out hv_Phi1, out hv_Length11, out hv_Length21);
                    ho_Rectangle3.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_Row3, hv_Column2, hv_Phi1,
                        hv_Length11, hv_Length21);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_Rectangle3, ho_Region_dianji, out ho_RegionDifference
                        );
                    ho_RegionIntersection1.Dispose();
                    HOperatorSet.Intersection(ho_RegionDifference, ho_RegionUnion_std, out ho_RegionIntersection1
                        );


                    //intersection (RegionDifference, RegionUnion_1, RegionIntersection1)




                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionIntersection1, out ho_ConnectedRegions1);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_ConnectedRegions1, out ho_RegionOpening, 3);
                    ho_RegionIntersection2.Dispose();
                    HOperatorSet.Intersection(ho_RegionOpening, ho_RegionTrans, out ho_RegionIntersection2
                        );
                    ho_ConnectedRegions3.Dispose();
                    HOperatorSet.Connection(ho_RegionIntersection2, out ho_ConnectedRegions3);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions3, out ho_SelectedRegions, ((new HTuple("area")).TupleConcat(
                        "width")).TupleConcat("anisometry"), "and", hv_iArea_shangpa1.TupleConcat(
                        (new HTuple(6)).TupleConcat(0)), ((new HTuple(99999)).TupleConcat(1000)).TupleConcat(
                        15));
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    hv_Area1.Dispose(); hv_Row4.Dispose(); hv_Column3.Dispose();
                    HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area1, out hv_Row4, out hv_Column3);
                    ho_RegionErr.Dispose();
                    ho_RegionErr = new HObject(ho_SelectedRegions);



                    if ((int)(new HTuple(hv_Number.TupleNotEqual(0))) != 0)
                    {
                        hv_NGCode1.Dispose();
                        hv_NGCode1 = 4;
                        ho_SortedRegions.Dispose();
                        ho_Rectangle4.Dispose();
                        ho_RegionDilation.Dispose();
                        ho_RegionUnion2.Dispose();
                        ho_ImageReduced2.Dispose();
                        ho_ImageScaled.Dispose();
                        ho_Region.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_SortedRegions1.Dispose();
                        ho_RegionUnion1.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rectangle_33.Dispose();
                        ho_Rectangle_1.Dispose();
                        ho_RegionUnion_11.Dispose();
                        ho_Rectangle_std.Dispose();
                        ho_Rectangle_11.Dispose();
                        ho_Rectangle_22.Dispose();
                        ho_RegionUnion_std.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_RegionIntersection.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_Rectangle3.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionIntersection1.Dispose();
                        ho_ConnectedRegions1.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionIntersection2.Dispose();
                        ho_ConnectedRegions3.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();

                        hv_leixing.Dispose();
                        hv_iShangpa.Dispose();
                        hv_iScale_width_2.Dispose();
                        hv_iScale_height_2.Dispose();
                        hv_iArea_shangpa1.Dispose();
                        hv_iScale_height_3.Dispose();
                        hv_iScale_width_1.Dispose();
                        hv_iScale_height_1.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_Phi.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Area.Dispose();
                        hv_Row2.Dispose();
                        hv_Column1.Dispose();
                        hv_Deg.Dispose();
                        hv_Row1.Dispose();
                        hv_hv_L.Dispose();
                        hv_column1.Dispose();
                        hv_hv_L1.Dispose();
                        hv_column2.Dispose();
                        hv_Row3.Dispose();
                        hv_Column2.Dispose();
                        hv_Phi1.Dispose();
                        hv_Length11.Dispose();
                        hv_Length21.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row4.Dispose();
                        hv_Column3.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                }
                // catch (Exception) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_NGCode1.Dispose();
                    hv_NGCode1 = 34;
                }
                ho_SortedRegions.Dispose();
                ho_Rectangle4.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionUnion2.Dispose();
                ho_ImageReduced2.Dispose();
                ho_ImageScaled.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_SortedRegions1.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle_33.Dispose();
                ho_Rectangle_1.Dispose();
                ho_RegionUnion_11.Dispose();
                ho_Rectangle_std.Dispose();
                ho_Rectangle_11.Dispose();
                ho_Rectangle_22.Dispose();
                ho_RegionUnion_std.Dispose();
                ho_RegionErosion.Dispose();
                ho_RegionIntersection.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_Rectangle3.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionIntersection2.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();

                hv_leixing.Dispose();
                hv_iShangpa.Dispose();
                hv_iScale_width_2.Dispose();
                hv_iScale_height_2.Dispose();
                hv_iArea_shangpa1.Dispose();
                hv_iScale_height_3.Dispose();
                hv_iScale_width_1.Dispose();
                hv_iScale_height_1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row2.Dispose();
                hv_Column1.Dispose();
                hv_Deg.Dispose();
                hv_Row1.Dispose();
                hv_hv_L.Dispose();
                hv_column1.Dispose();
                hv_hv_L1.Dispose();
                hv_column2.Dispose();
                hv_Row3.Dispose();
                hv_Column2.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_Area1.Dispose();
                hv_Row4.Dispose();
                hv_Column3.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_SortedRegions.Dispose();
                ho_Rectangle4.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionUnion2.Dispose();
                ho_ImageReduced2.Dispose();
                ho_ImageScaled.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_SortedRegions1.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle_33.Dispose();
                ho_Rectangle_1.Dispose();
                ho_RegionUnion_11.Dispose();
                ho_Rectangle_std.Dispose();
                ho_Rectangle_11.Dispose();
                ho_Rectangle_22.Dispose();
                ho_RegionUnion_std.Dispose();
                ho_RegionErosion.Dispose();
                ho_RegionIntersection.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_Rectangle3.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionIntersection2.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();

                hv_leixing.Dispose();
                hv_iShangpa.Dispose();
                hv_iScale_width_2.Dispose();
                hv_iScale_height_2.Dispose();
                hv_iArea_shangpa1.Dispose();
                hv_iScale_height_3.Dispose();
                hv_iScale_width_1.Dispose();
                hv_iScale_height_1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row2.Dispose();
                hv_Column1.Dispose();
                hv_Deg.Dispose();
                hv_Row1.Dispose();
                hv_hv_L.Dispose();
                hv_column1.Dispose();
                hv_hv_L1.Dispose();
                hv_column2.Dispose();
                hv_Row3.Dispose();
                hv_Column2.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_Area1.Dispose();
                hv_Row4.Dispose();
                hv_Column3.Dispose();
                hv_Exception.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void baohuceng_bengsui(HObject ho_ImageReduced, HObject ho_Region_dianji_zhengmian,
      HObject ho_Region_zima, out HObject ho_RegionErr3, out HObject ho_RegionDetection_IIG,
      HTuple hv_Parameter_IIG, out HTuple hv_NGCode1)
        {




            // Local iconic variables 

            HObject ho_UnionMK = null, ho_Rectangle = null;
            HObject ho_Rectangle1 = null, ho_RegionUnion = null, ho_RegionTrans = null;
            HObject ho_Rectangle_chanpin = null, ho_RegionTrans1 = null;
            HObject ho_RegionClosing = null, ho_RegionDifference1 = null;
            HObject ho_RegionTrans2 = null, ho_RegionDilation = null, ho_RegionDifference2 = null;
            HObject ho_RegionDifference = null, ho_RegionDifference3 = null;
            HObject ho_RegionErosion = null, ho_ImageReduced_baohuceng = null;
            HObject ho_ImageMean = null, ho_ExpImage = null, ho_ImageScaleMax = null;
            HObject ho_Region = null, ho_RegionOpening = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_SelectedRegions1 = null;

            // Local copy input parameter variables 
            HObject ho_Region_zima_COPY_INP_TMP;
            ho_Region_zima_COPY_INP_TMP = new HObject(ho_Region_zima);



            // Local control variables 

            HTuple hv_leixing = new HTuple(), hv_iProtectexp = new HTuple();
            HTuple hv_iProtectBrokenArea = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Number = new HTuple();
            HTuple hv_exc = new HTuple(), hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionErr3);
            HOperatorSet.GenEmptyObj(out ho_RegionDetection_IIG);
            HOperatorSet.GenEmptyObj(out ho_UnionMK);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_chanpin);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans2);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference3);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced_baohuceng);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ExpImage);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            hv_NGCode1 = new HTuple();
            try
            {
                //***IIG崩碎区域提取
                try
                {
                    hv_leixing.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_leixing = hv_Parameter_IIG.TupleSelect(
                            0);
                    }
                    hv_iProtectexp.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iProtectexp = hv_Parameter_IIG.TupleSelect(
                            1);
                    }
                    hv_iProtectBrokenArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iProtectBrokenArea = hv_Parameter_IIG.TupleSelect(
                            2);
                    }


                    ho_UnionMK.Dispose();
                    HOperatorSet.Union1(ho_Region_zima_COPY_INP_TMP, out ho_UnionMK);
                    ho_Region_zima_COPY_INP_TMP.Dispose();
                    ho_Region_zima_COPY_INP_TMP = new HObject(ho_UnionMK);

                    //**获取电极延长区域
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_dianji_zhengmian, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1,
                        hv_Length2);
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi,
                        hv_Length1 + 50, hv_Length2);

                    //**获取产品区域
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_Region_dianji_zhengmian, out ho_RegionUnion);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "convex");
                    hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Phi1.Dispose(); hv_Length11.Dispose(); hv_Length21.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row1, out hv_Column1,
                        out hv_Phi1, out hv_Length11, out hv_Length21);
                    ho_Rectangle_chanpin.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_chanpin, hv_Row1, hv_Column1,
                        hv_Phi1, hv_Length11, hv_Length21);




                    ho_RegionTrans1.Dispose();
                    HOperatorSet.ShapeTrans(ho_Region_dianji_zhengmian, out ho_RegionTrans1,
                        "convex");
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region_dianji_zhengmian, out ho_RegionClosing,
                        5);

                    ho_RegionDifference1.Dispose();
                    HOperatorSet.Difference(ho_Rectangle_chanpin, ho_Rectangle1, out ho_RegionDifference1
                        );

                    ho_RegionTrans2.Dispose();
                    HOperatorSet.ShapeTrans(ho_Region_zima_COPY_INP_TMP, out ho_RegionTrans2,
                        "rectangle2");
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionTrans2, out ho_RegionDilation, 500,
                        1);
                    ho_RegionDifference2.Dispose();
                    HOperatorSet.Difference(ho_RegionDifference1, ho_RegionDilation, out ho_RegionDifference2
                        );




                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_RegionDifference1, ho_RegionDifference2, out ho_RegionDifference
                        );

                    ho_RegionDifference3.Dispose();
                    HOperatorSet.Difference(ho_RegionDifference, ho_Region_zima_COPY_INP_TMP,
                        out ho_RegionDifference3);

                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference3, out ho_RegionErosion,
                        10, 1);

                    ho_ImageReduced_baohuceng.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionErosion, out ho_ImageReduced_baohuceng
                        );

                    ho_RegionDetection_IIG.Dispose();
                    ho_RegionDetection_IIG = new HObject(ho_RegionErosion);

                    if ((int)(1) != 0)
                    {

                        ho_ImageMean.Dispose();
                        HOperatorSet.MeanImage(ho_ImageReduced_baohuceng, out ho_ImageMean, 9,
                            9);
                        //exp_image (ImageReduced_baohuceng, ExpImage, iProtectexp)
                        ho_ExpImage.Dispose();
                        HOperatorSet.ExpImage(ho_ImageMean, out ho_ExpImage, hv_iProtectexp);
                        ho_ImageScaleMax.Dispose();
                        HOperatorSet.ScaleImageMax(ho_ExpImage, out ho_ImageScaleMax);


                        ho_Region.Dispose();
                        HOperatorSet.Threshold(ho_ImageScaleMax, out ho_Region, 200, 255);
                        ho_RegionOpening.Dispose();
                        HOperatorSet.OpeningCircle(ho_Region, out ho_RegionOpening, 3.5);

                        ho_ConnectedRegions.Dispose();
                        HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                        ho_SelectedRegions.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "anisometry",
                            "and", 0, 15);
                        ho_SelectedRegions1.Dispose();
                        HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions1, "area",
                            "and", hv_iProtectBrokenArea, 99999);
                        hv_Number.Dispose();
                        HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number);
                        ho_RegionErr3.Dispose();
                        ho_RegionErr3 = new HObject(ho_SelectedRegions1);

                        if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                        {
                            hv_NGCode1.Dispose();
                            hv_NGCode1 = 3;
                            ho_Region_zima_COPY_INP_TMP.Dispose();
                            ho_UnionMK.Dispose();
                            ho_Rectangle.Dispose();
                            ho_Rectangle1.Dispose();
                            ho_RegionUnion.Dispose();
                            ho_RegionTrans.Dispose();
                            ho_Rectangle_chanpin.Dispose();
                            ho_RegionTrans1.Dispose();
                            ho_RegionClosing.Dispose();
                            ho_RegionDifference1.Dispose();
                            ho_RegionTrans2.Dispose();
                            ho_RegionDilation.Dispose();
                            ho_RegionDifference2.Dispose();
                            ho_RegionDifference.Dispose();
                            ho_RegionDifference3.Dispose();
                            ho_RegionErosion.Dispose();
                            ho_ImageReduced_baohuceng.Dispose();
                            ho_ImageMean.Dispose();
                            ho_ExpImage.Dispose();
                            ho_ImageScaleMax.Dispose();
                            ho_Region.Dispose();
                            ho_RegionOpening.Dispose();
                            ho_ConnectedRegions.Dispose();
                            ho_SelectedRegions.Dispose();
                            ho_SelectedRegions1.Dispose();

                            hv_leixing.Dispose();
                            hv_iProtectexp.Dispose();
                            hv_iProtectBrokenArea.Dispose();
                            hv_Row.Dispose();
                            hv_Column.Dispose();
                            hv_Phi.Dispose();
                            hv_Length1.Dispose();
                            hv_Length2.Dispose();
                            hv_Row1.Dispose();
                            hv_Column1.Dispose();
                            hv_Phi1.Dispose();
                            hv_Length11.Dispose();
                            hv_Length21.Dispose();
                            hv_Number.Dispose();
                            hv_exc.Dispose();
                            hv_NGCode.Dispose();

                            return;
                        }
                    }

                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }
                ho_Region_zima_COPY_INP_TMP.Dispose();
                ho_UnionMK.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle1.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle_chanpin.Dispose();
                ho_RegionTrans1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionDifference1.Dispose();
                ho_RegionTrans2.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionDifference2.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionDifference3.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced_baohuceng.Dispose();
                ho_ImageMean.Dispose();
                ho_ExpImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions1.Dispose();

                hv_leixing.Dispose();
                hv_iProtectexp.Dispose();
                hv_iProtectBrokenArea.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Region_zima_COPY_INP_TMP.Dispose();
                ho_UnionMK.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle1.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle_chanpin.Dispose();
                ho_RegionTrans1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionDifference1.Dispose();
                ho_RegionTrans2.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionDifference2.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionDifference3.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced_baohuceng.Dispose();
                ho_ImageMean.Dispose();
                ho_ExpImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions1.Dispose();

                hv_leixing.Dispose();
                hv_iProtectexp.Dispose();
                hv_iProtectBrokenArea.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void baohuceng_bengsui_2(HObject ho_ImageReduced, HObject ho_Region_dianji_zhengmian,
      HObject ho_Region_zima, out HObject ho_RegionErr3, out HObject ho_RegionDetection_IIG2,
      HTuple hv_Parameter_IIG, out HTuple hv_NGCode1)
        {




            // Local iconic variables 

            HObject ho_UnionMK = null, ho_Rectangle = null;
            HObject ho_Rectangle1 = null, ho_RegionUnion = null, ho_RegionTrans = null;
            HObject ho_Rectangle_chanpin = null, ho_RegionTrans1 = null;
            HObject ho_RegionClosing = null, ho_RegionDifference1 = null;
            HObject ho_RegionTrans2 = null, ho_RegionDilation = null, ho_RegionDifference2 = null;
            HObject ho_RegionErosion = null, ho_ImageReduced_baohuceng = null;
            HObject ho_ImageMean = null, ho_ExpImage = null, ho_ImageScaleMax = null;
            HObject ho_Region = null, ho_RegionIntersection = null, ho_RegionOpening = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_SelectedRegions2 = null, ho_SelectedRegions3 = null;
            HObject ho_SelectedRegions1 = null;

            // Local copy input parameter variables 
            HObject ho_Region_zima_COPY_INP_TMP;
            ho_Region_zima_COPY_INP_TMP = new HObject(ho_Region_zima);



            // Local control variables 

            HTuple hv_leixing = new HTuple(), hv_iProtectexp = new HTuple();
            HTuple hv_iProtectBrokenArea = new HTuple(), hv_iErosionWidth_IIG2 = new HTuple();
            HTuple hv_iErosionHeight_IIG2 = new HTuple(), hv_iErrWidth = new HTuple();
            HTuple hv_iErrHeight = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Phi1 = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length21 = new HTuple(), hv_Number = new HTuple();
            HTuple hv_exc = new HTuple(), hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionErr3);
            HOperatorSet.GenEmptyObj(out ho_RegionDetection_IIG2);
            HOperatorSet.GenEmptyObj(out ho_UnionMK);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_chanpin);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans2);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference2);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced_baohuceng);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ExpImage);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            hv_NGCode1 = new HTuple();
            try
            {
                //***IIG崩碎区域提取
                try
                {
                    hv_leixing.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_leixing = hv_Parameter_IIG.TupleSelect(
                            0);
                    }
                    hv_iProtectexp.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iProtectexp = hv_Parameter_IIG.TupleSelect(
                            1);
                    }
                    hv_iProtectBrokenArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iProtectBrokenArea = hv_Parameter_IIG.TupleSelect(
                            2);
                    }
                    hv_iErosionWidth_IIG2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErosionWidth_IIG2 = hv_Parameter_IIG.TupleSelect(
                            3);
                    }
                    hv_iErosionHeight_IIG2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErosionHeight_IIG2 = hv_Parameter_IIG.TupleSelect(
                            4);
                    }
                    hv_iErrWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrWidth = hv_Parameter_IIG.TupleSelect(
                            5);
                    }
                    hv_iErrHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iErrHeight = hv_Parameter_IIG.TupleSelect(
                            6);
                    }


                    ho_UnionMK.Dispose();
                    HOperatorSet.Union1(ho_Region_zima_COPY_INP_TMP, out ho_UnionMK);
                    ho_Region_zima_COPY_INP_TMP.Dispose();
                    ho_Region_zima_COPY_INP_TMP = new HObject(ho_UnionMK);

                    //**获取电极延长区域
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_dianji_zhengmian, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1,
                        hv_Length2);
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi,
                        hv_Length1 + 50, hv_Length2);

                    //**获取产品区域
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_Region_dianji_zhengmian, out ho_RegionUnion);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "convex");
                    hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Phi1.Dispose(); hv_Length11.Dispose(); hv_Length21.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_RegionTrans, out hv_Row1, out hv_Column1,
                        out hv_Phi1, out hv_Length11, out hv_Length21);
                    ho_Rectangle_chanpin.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle_chanpin, hv_Row1, hv_Column1,
                        hv_Phi1, hv_Length11, hv_Length21);




                    ho_RegionTrans1.Dispose();
                    HOperatorSet.ShapeTrans(ho_Region_dianji_zhengmian, out ho_RegionTrans1,
                        "convex");
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region_dianji_zhengmian, out ho_RegionClosing,
                        5);
                    ho_RegionDifference1.Dispose();
                    HOperatorSet.Difference(ho_Rectangle_chanpin, ho_Rectangle1, out ho_RegionDifference1
                        );

                    ho_RegionTrans2.Dispose();
                    HOperatorSet.ShapeTrans(ho_Region_zima_COPY_INP_TMP, out ho_RegionTrans2,
                        "rectangle2");
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationRectangle1(ho_RegionTrans2, out ho_RegionDilation, 500,
                        1);
                    ho_RegionDifference2.Dispose();
                    HOperatorSet.Difference(ho_RegionDifference1, ho_RegionDilation, out ho_RegionDifference2
                        );




                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference2, out ho_RegionErosion,
                        hv_iErosionWidth_IIG2, hv_iErosionHeight_IIG2);

                    ho_ImageReduced_baohuceng.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageReduced, ho_RegionTrans, out ho_ImageReduced_baohuceng
                        );
                    if ((int)(1) != 0)
                    {

                        ho_ImageMean.Dispose();
                        HOperatorSet.MeanImage(ho_ImageReduced_baohuceng, out ho_ImageMean, 9,
                            9);
                        //exp_image (ImageReduced_baohuceng, ExpImage, iProtectexp)
                        //exp_image (ImageMean, ExpImage, 30)
                        ho_ExpImage.Dispose();
                        HOperatorSet.ExpImage(ho_ImageMean, out ho_ExpImage, hv_iProtectexp);
                        ho_ImageScaleMax.Dispose();
                        HOperatorSet.ScaleImageMax(ho_ExpImage, out ho_ImageScaleMax);
                        ho_Region.Dispose();
                        HOperatorSet.Threshold(ho_ImageScaleMax, out ho_Region, 200, 255);

                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Region, ho_RegionErosion, out ho_RegionIntersection
                            );
                        ho_RegionOpening.Dispose();
                        HOperatorSet.OpeningCircle(ho_RegionIntersection, out ho_RegionOpening,
                            3.5);

                        ho_RegionDetection_IIG2.Dispose();
                        ho_RegionDetection_IIG2 = new HObject(ho_RegionErosion);

                        ho_ConnectedRegions.Dispose();
                        HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                        ho_SelectedRegions.Dispose();
                        HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "anisometry",
                            "and", 0, 15);

                        ho_SelectedRegions2.Dispose();
                        HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions2, "width",
                            "and", hv_iErrWidth, 99999);
                        ho_SelectedRegions3.Dispose();
                        HOperatorSet.SelectShape(ho_SelectedRegions2, out ho_SelectedRegions3,
                            "height", "and", hv_iErrHeight, 99999);
                        ho_SelectedRegions1.Dispose();
                        HOperatorSet.SelectShape(ho_SelectedRegions3, out ho_SelectedRegions1,
                            "area", "and", hv_iProtectBrokenArea, 99999);



                        hv_Number.Dispose();
                        HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number);
                        ho_RegionErr3.Dispose();
                        ho_RegionErr3 = new HObject(ho_SelectedRegions1);

                        if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                        {
                            hv_NGCode1.Dispose();
                            hv_NGCode1 = 3;
                            ho_Region_zima_COPY_INP_TMP.Dispose();
                            ho_UnionMK.Dispose();
                            ho_Rectangle.Dispose();
                            ho_Rectangle1.Dispose();
                            ho_RegionUnion.Dispose();
                            ho_RegionTrans.Dispose();
                            ho_Rectangle_chanpin.Dispose();
                            ho_RegionTrans1.Dispose();
                            ho_RegionClosing.Dispose();
                            ho_RegionDifference1.Dispose();
                            ho_RegionTrans2.Dispose();
                            ho_RegionDilation.Dispose();
                            ho_RegionDifference2.Dispose();
                            ho_RegionErosion.Dispose();
                            ho_ImageReduced_baohuceng.Dispose();
                            ho_ImageMean.Dispose();
                            ho_ExpImage.Dispose();
                            ho_ImageScaleMax.Dispose();
                            ho_Region.Dispose();
                            ho_RegionIntersection.Dispose();
                            ho_RegionOpening.Dispose();
                            ho_ConnectedRegions.Dispose();
                            ho_SelectedRegions.Dispose();
                            ho_SelectedRegions2.Dispose();
                            ho_SelectedRegions3.Dispose();
                            ho_SelectedRegions1.Dispose();

                            hv_leixing.Dispose();
                            hv_iProtectexp.Dispose();
                            hv_iProtectBrokenArea.Dispose();
                            hv_iErosionWidth_IIG2.Dispose();
                            hv_iErosionHeight_IIG2.Dispose();
                            hv_iErrWidth.Dispose();
                            hv_iErrHeight.Dispose();
                            hv_Row.Dispose();
                            hv_Column.Dispose();
                            hv_Phi.Dispose();
                            hv_Length1.Dispose();
                            hv_Length2.Dispose();
                            hv_Row1.Dispose();
                            hv_Column1.Dispose();
                            hv_Phi1.Dispose();
                            hv_Length11.Dispose();
                            hv_Length21.Dispose();
                            hv_Number.Dispose();
                            hv_exc.Dispose();
                            hv_NGCode.Dispose();

                            return;
                        }
                    }

                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }
                ho_Region_zima_COPY_INP_TMP.Dispose();
                ho_UnionMK.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle1.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle_chanpin.Dispose();
                ho_RegionTrans1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionDifference1.Dispose();
                ho_RegionTrans2.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionDifference2.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced_baohuceng.Dispose();
                ho_ImageMean.Dispose();
                ho_ExpImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_Region.Dispose();
                ho_RegionIntersection.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_SelectedRegions1.Dispose();

                hv_leixing.Dispose();
                hv_iProtectexp.Dispose();
                hv_iProtectBrokenArea.Dispose();
                hv_iErosionWidth_IIG2.Dispose();
                hv_iErosionHeight_IIG2.Dispose();
                hv_iErrWidth.Dispose();
                hv_iErrHeight.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Region_zima_COPY_INP_TMP.Dispose();
                ho_UnionMK.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle1.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle_chanpin.Dispose();
                ho_RegionTrans1.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionDifference1.Dispose();
                ho_RegionTrans2.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionDifference2.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced_baohuceng.Dispose();
                ho_ImageMean.Dispose();
                ho_ExpImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_Region.Dispose();
                ho_RegionIntersection.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_SelectedRegions1.Dispose();

                hv_leixing.Dispose();
                hv_iProtectexp.Dispose();
                hv_iProtectBrokenArea.Dispose();
                hv_iErosionWidth_IIG2.Dispose();
                hv_iErosionHeight_IIG2.Dispose();
                hv_iErrWidth.Dispose();
                hv_iErrHeight.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }


        public static void dianji_beimian_soble(HObject ho_GrayImage, out HObject ho_Region_dianji,
      out HObject ho_Region_citi, HTuple hv_param_0402_5_k, HTuple hv_AutoThreshold1)
        {




            // Local iconic variables 

            HObject ho_Regions = null, ho_ConnectedRegions2 = null;
            HObject ho_SelectedRegions1 = null, ho_RegionClosing = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionUnion = null, ho_RegionTrans = null, ho_ImageReduced = null;
            HObject ho_ImageEdgeAmp = null, ho_Region = null, ho_RegionOpening = null;
            HObject ho_RegionTrans1 = null, ho_RegionDifference = null;
            HObject ho_RegionErosion = null, ho_SelectedRegions2 = null;
            HObject ho_SortedRegions = null, ho_Rects = null, ho_Rectangle = null;
            HObject ho_Rect_citi = null, ho_RegionFillUp = null;

            // Local control variables 

            HTuple hv_iSmallestArea = new HTuple(), hv_iBiggstArea = new HTuple();
            HTuple hv_iAreaDiff = new HTuple(), hv_cRectangularity = new HTuple();
            HTuple hv_ipix = new HTuple(), hv_pWidthMax = new HTuple();
            HTuple hv_pWidthMin = new HTuple(), hv_eHeightStd = new HTuple();
            HTuple hv_eWidthStd = new HTuple(), hv_eHeightScale = new HTuple();
            HTuple hv_eWidthScale = new HTuple(), hv_eHeightDiff = new HTuple();
            HTuple hv_eWidthDiff = new HTuple(), hv_eHeightSum = new HTuple();
            HTuple hv_eWidthSum = new HTuple(), hv_eRectangularity = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_NGCode = new HTuple(), hv_Row5 = new HTuple();
            HTuple hv_Column5 = new HTuple(), hv_Phi2 = new HTuple();
            HTuple hv_Length14 = new HTuple(), hv_Length24 = new HTuple();
            HTuple hv_eHeight = new HTuple(), hv_eWidth = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length11 = new HTuple();
            HTuple hv_Length22 = new HTuple(), hv_Rectangularity = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Ratio = new HTuple(), hv_Exception = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_dianji);
            HOperatorSet.GenEmptyObj(out ho_Region_citi);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageEdgeAmp);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans1);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rects);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rect_citi);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            try
            {

                try
                {
                    hv_iSmallestArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iSmallestArea = hv_param_0402_5_k.TupleSelect(
                            0);
                    }
                    hv_iBiggstArea.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iBiggstArea = hv_param_0402_5_k.TupleSelect(
                            1);
                    }
                    hv_iAreaDiff.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iAreaDiff = hv_param_0402_5_k.TupleSelect(
                            2);
                    }
                    hv_cRectangularity.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_cRectangularity = hv_param_0402_5_k.TupleSelect(
                            3);
                    }
                    hv_ipix.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ipix = hv_param_0402_5_k.TupleSelect(
                            4);
                    }
                    hv_pWidthMax.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_pWidthMax = hv_param_0402_5_k.TupleSelect(
                            5);
                    }
                    hv_pWidthMin.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_pWidthMin = hv_param_0402_5_k.TupleSelect(
                            6);
                    }
                    hv_iAreaDiff.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_iAreaDiff = hv_param_0402_5_k.TupleSelect(
                            7);
                    }

                    hv_eHeightStd.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eHeightStd = hv_param_0402_5_k.TupleSelect(
                            8);
                    }
                    hv_eWidthStd.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eWidthStd = hv_param_0402_5_k.TupleSelect(
                            9);
                    }
                    hv_eHeightScale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eHeightScale = hv_param_0402_5_k.TupleSelect(
                            10);
                    }
                    hv_eWidthScale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eWidthScale = hv_param_0402_5_k.TupleSelect(
                            11);
                    }
                    hv_eHeightDiff.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eHeightDiff = hv_param_0402_5_k.TupleSelect(
                            12);
                    }
                    hv_eWidthDiff.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eWidthDiff = hv_param_0402_5_k.TupleSelect(
                            13);
                    }
                    hv_eHeightSum.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eHeightSum = hv_param_0402_5_k.TupleSelect(
                            14);
                    }
                    hv_eWidthSum.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eWidthSum = hv_param_0402_5_k.TupleSelect(
                            15);
                    }
                    hv_pWidthMax.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_pWidthMax = hv_param_0402_5_k.TupleSelect(
                            16);
                    }
                    hv_pWidthMin.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_pWidthMin = hv_param_0402_5_k.TupleSelect(
                            17);
                    }
                    hv_eRectangularity.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eRectangularity = hv_param_0402_5_k.TupleSelect(
                            18);
                    }

                    if (HDevWindowStack.IsOpen())
                    {
                        //dev_set_draw ('fill')
                    }
                    ho_Regions.Dispose();
                    HOperatorSet.AutoThreshold(ho_GrayImage, out ho_Regions, hv_AutoThreshold1);
                    ho_ConnectedRegions2.Dispose();
                    HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions2);
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions1, (
                        (new HTuple("area")).TupleConcat("width")).TupleConcat("height"), "and",
                        ((new HTuple(9999)).TupleConcat(300)).TupleConcat(100), ((new HTuple(209999)).TupleConcat(
                        550)).TupleConcat(350));
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_SelectedRegions1, out ho_RegionClosing, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 150, 999999);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "convex");

                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionTrans, out ho_ImageReduced
                        );
                    ho_ImageEdgeAmp.Dispose();
                    HOperatorSet.KirschAmp(ho_ImageReduced, out ho_ImageEdgeAmp);
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageEdgeAmp, out ho_Region, 0, 58);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 1.5);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions,
                        "max_area", 70);
                    //**2019-12-18：修改瓷体区域
                    //shape_trans (SelectedRegions, RegionTrans1, 'convex')
                    ho_RegionTrans1.Dispose();
                    HOperatorSet.FillUp(ho_SelectedRegions, out ho_RegionTrans1);

                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_RegionTrans, ho_RegionTrans1, out ho_RegionDifference
                        );
                    //erosion_circle (RegionDifference, RegionErosion, 5)

                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionErosion,
                        1, 11);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionErosion, out ho_RegionOpening, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    ho_SelectedRegions2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions2, "area",
                        "and", 3500, 99999);


                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions2, out hv_Number);
                    ho_SortedRegions.Dispose();
                    HOperatorSet.SortRegion(ho_SelectedRegions2, out ho_SortedRegions, "first_point",
                        "true", "column");
                    ho_Region_dianji.Dispose();
                    ho_Region_dianji = new HObject(ho_SortedRegions);
                    ho_Region_citi.Dispose();
                    ho_Region_citi = new HObject(ho_RegionTrans1);

                    //**电极面积检测
                    hv_Area1.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_Region_dianji, out hv_Area1, out hv_Row1, out hv_Column1);
                    if ((int)((new HTuple(((hv_Area1.TupleSelect(0))).TupleLess(hv_iSmallestArea))).TupleOr(
                        new HTuple(((hv_Area1.TupleSelect(1))).TupleLess(hv_iSmallestArea)))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 15;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)((new HTuple(((hv_Area1.TupleSelect(0))).TupleGreater(hv_iBiggstArea))).TupleOr(
                        new HTuple(((hv_Area1.TupleSelect(1))).TupleGreater(hv_iBiggstArea)))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 16;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)(new HTuple((((((hv_Area1.TupleSelect(0)) - (hv_Area1.TupleSelect(
                        1)))).TupleAbs())).TupleGreater(hv_iAreaDiff))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 17;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    //**电极长宽检测
                    //*****
                    hv_Area1.Dispose(); hv_Row1.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_Region_dianji, out hv_Area1, out hv_Row1, out hv_Column1);
                    hv_Row5.Dispose(); hv_Column5.Dispose(); hv_Phi2.Dispose(); hv_Length14.Dispose(); hv_Length24.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_dianji, out hv_Row5, out hv_Column5,
                        out hv_Phi2, out hv_Length14, out hv_Length24);
                    hv_eHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eHeight = ((hv_Length14 * 2) * hv_ipix) * 1000;
                    }
                    hv_eWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eWidth = ((hv_Length24 * 2) * hv_ipix) * 1000;
                    }

                    //***电极高度判断
                    if ((int)(new HTuple(hv_eHeight.TupleLess(hv_eHeightStd - hv_eHeightScale))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 18;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)(new HTuple(hv_eHeight.TupleGreater(hv_eHeightStd + hv_eHeightScale))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 19;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)(new HTuple((((((hv_eHeight.TupleSelect(0)) - (hv_eHeight.TupleSelect(
                        1)))).TupleAbs())).TupleGreater(hv_eHeightDiff))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 20;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    //if (abs(eHeight[0]+eHeight[1])>eHeightSum)
                    //stop ()
                    //endif

                    //***电极宽度判断
                    hv_Row3.Dispose(); hv_Column3.Dispose(); hv_Phi.Dispose(); hv_Length11.Dispose(); hv_Length22.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_dianji, out hv_Row3, out hv_Column3,
                        out hv_Phi, out hv_Length11, out hv_Length22);
                    ho_Rects.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rects, hv_Row3, hv_Column3, hv_Phi, hv_Length11,
                        hv_Length22);
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row3, hv_Column3, hv_Phi,
                        hv_Length11, hv_Length22);
                    //**电极平均宽度检测

                    hv_eWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_eWidth = ((((hv_Area1 / (hv_Length11 * 2)) / 2) * 2) * hv_ipix) * 1000;
                    }

                    if ((int)(new HTuple(hv_eWidth.TupleLess(hv_eWidthStd - hv_eWidthScale))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 21;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)(new HTuple(hv_eWidth.TupleGreater(hv_eWidthStd + hv_eWidthScale))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 22;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)(new HTuple((((((hv_eWidth.TupleSelect(0)) - (hv_eWidth.TupleSelect(
                        1)))).TupleAbs())).TupleGreater(hv_eWidthDiff))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 23;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    if ((int)(new HTuple((((((hv_eWidth.TupleSelect(0)) + (hv_eWidth.TupleSelect(
                        1)))).TupleAbs())).TupleGreater(hv_eWidthSum))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 24;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    //***电极矩形度检测
                    hv_Rectangularity.Dispose();
                    HOperatorSet.Rectangularity(ho_Region_dianji, out hv_Rectangularity);
                    if ((int)(new HTuple(hv_Rectangularity.TupleLess(hv_eRectangularity))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 25;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    //***瓷体宽度检测、瓷体矩形度检测-电阻正面无瓷体，不需要运行此函数
                    hv_Row3.Dispose(); hv_Column3.Dispose(); hv_Phi.Dispose(); hv_Length11.Dispose(); hv_Length22.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_Region_citi, out hv_Row3, out hv_Column3,
                        out hv_Phi, out hv_Length11, out hv_Length22);
                    ho_Rect_citi.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rect_citi, hv_Row3, hv_Column3, hv_Phi,
                        hv_Length11, hv_Length22);
                    hv_Height.Dispose(); hv_Width.Dispose(); hv_Ratio.Dispose();
                    HOperatorSet.HeightWidthRatio(ho_Rect_citi, out hv_Height, out hv_Width,
                        out hv_Ratio);

                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_Region_citi, out ho_RegionFillUp);
                    hv_Rectangularity.Dispose();
                    HOperatorSet.Rectangularity(ho_RegionFillUp, out hv_Rectangularity);
                    if ((int)(new HTuple(hv_Rectangularity.TupleLess(hv_cRectangularity))) != 0)
                    {
                        hv_NGCode.Dispose();
                        hv_NGCode = 26;
                        ho_Regions.Dispose();
                        ho_ConnectedRegions2.Dispose();
                        ho_SelectedRegions1.Dispose();
                        ho_RegionClosing.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegions.Dispose();
                        ho_RegionUnion.Dispose();
                        ho_RegionTrans.Dispose();
                        ho_ImageReduced.Dispose();
                        ho_ImageEdgeAmp.Dispose();
                        ho_Region.Dispose();
                        ho_RegionOpening.Dispose();
                        ho_RegionTrans1.Dispose();
                        ho_RegionDifference.Dispose();
                        ho_RegionErosion.Dispose();
                        ho_SelectedRegions2.Dispose();
                        ho_SortedRegions.Dispose();
                        ho_Rects.Dispose();
                        ho_Rectangle.Dispose();
                        ho_Rect_citi.Dispose();
                        ho_RegionFillUp.Dispose();

                        hv_iSmallestArea.Dispose();
                        hv_iBiggstArea.Dispose();
                        hv_iAreaDiff.Dispose();
                        hv_cRectangularity.Dispose();
                        hv_ipix.Dispose();
                        hv_pWidthMax.Dispose();
                        hv_pWidthMin.Dispose();
                        hv_eHeightStd.Dispose();
                        hv_eWidthStd.Dispose();
                        hv_eHeightScale.Dispose();
                        hv_eWidthScale.Dispose();
                        hv_eHeightDiff.Dispose();
                        hv_eWidthDiff.Dispose();
                        hv_eHeightSum.Dispose();
                        hv_eWidthSum.Dispose();
                        hv_eRectangularity.Dispose();
                        hv_Number.Dispose();
                        hv_Area1.Dispose();
                        hv_Row1.Dispose();
                        hv_Column1.Dispose();
                        hv_NGCode.Dispose();
                        hv_Row5.Dispose();
                        hv_Column5.Dispose();
                        hv_Phi2.Dispose();
                        hv_Length14.Dispose();
                        hv_Length24.Dispose();
                        hv_eHeight.Dispose();
                        hv_eWidth.Dispose();
                        hv_Row3.Dispose();
                        hv_Column3.Dispose();
                        hv_Phi.Dispose();
                        hv_Length11.Dispose();
                        hv_Length22.Dispose();
                        hv_Rectangularity.Dispose();
                        hv_Height.Dispose();
                        hv_Width.Dispose();
                        hv_Ratio.Dispose();
                        hv_Exception.Dispose();

                        return;
                    }

                    //pWidth := Width*ipix*1000
                    //if (pWidth>pWidthMax)
                    //stop ()
                    //endif
                    //if (pWidth<pWidthMin)
                    //stop ()
                    //endif

                }
                // catch (Exception) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                ho_Regions.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageEdgeAmp.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionTrans1.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionErosion.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_SortedRegions.Dispose();
                ho_Rects.Dispose();
                ho_Rectangle.Dispose();
                ho_Rect_citi.Dispose();
                ho_RegionFillUp.Dispose();

                hv_iSmallestArea.Dispose();
                hv_iBiggstArea.Dispose();
                hv_iAreaDiff.Dispose();
                hv_cRectangularity.Dispose();
                hv_ipix.Dispose();
                hv_pWidthMax.Dispose();
                hv_pWidthMin.Dispose();
                hv_eHeightStd.Dispose();
                hv_eWidthStd.Dispose();
                hv_eHeightScale.Dispose();
                hv_eWidthScale.Dispose();
                hv_eHeightDiff.Dispose();
                hv_eWidthDiff.Dispose();
                hv_eHeightSum.Dispose();
                hv_eWidthSum.Dispose();
                hv_eRectangularity.Dispose();
                hv_Number.Dispose();
                hv_Area1.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_NGCode.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi2.Dispose();
                hv_Length14.Dispose();
                hv_Length24.Dispose();
                hv_eHeight.Dispose();
                hv_eWidth.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Phi.Dispose();
                hv_Length11.Dispose();
                hv_Length22.Dispose();
                hv_Rectangularity.Dispose();
                hv_Height.Dispose();
                hv_Width.Dispose();
                hv_Ratio.Dispose();
                hv_Exception.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Regions.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageEdgeAmp.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionTrans1.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionErosion.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_SortedRegions.Dispose();
                ho_Rects.Dispose();
                ho_Rectangle.Dispose();
                ho_Rect_citi.Dispose();
                ho_RegionFillUp.Dispose();

                hv_iSmallestArea.Dispose();
                hv_iBiggstArea.Dispose();
                hv_iAreaDiff.Dispose();
                hv_cRectangularity.Dispose();
                hv_ipix.Dispose();
                hv_pWidthMax.Dispose();
                hv_pWidthMin.Dispose();
                hv_eHeightStd.Dispose();
                hv_eWidthStd.Dispose();
                hv_eHeightScale.Dispose();
                hv_eWidthScale.Dispose();
                hv_eHeightDiff.Dispose();
                hv_eWidthDiff.Dispose();
                hv_eHeightSum.Dispose();
                hv_eWidthSum.Dispose();
                hv_eRectangularity.Dispose();
                hv_Number.Dispose();
                hv_Area1.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_NGCode.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi2.Dispose();
                hv_Length14.Dispose();
                hv_Length24.Dispose();
                hv_eHeight.Dispose();
                hv_eWidth.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Phi.Dispose();
                hv_Length11.Dispose();
                hv_Length22.Dispose();
                hv_Rectangularity.Dispose();
                hv_Height.Dispose();
                hv_Width.Dispose();
                hv_Ratio.Dispose();
                hv_Exception.Dispose();

                throw HDevExpDefaultException;
            }
        }




        public static void dianji_beimian(HObject ho_GrayImage, out HObject ho_Region_dianji,
      out HObject ho_Region_citi, HTuple hv_Parameter_BM)
        {




            // Local iconic variables 

            HObject ho_Regions = null, ho_ConnectedRegions2 = null;
            HObject ho_SelectedRegions1 = null, ho_RegionClosing = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionUnion = null, ho_RegionTrans = null, ho_ImageReduced = null;
            HObject ho_ImageEdgeAmp = null, ho_Region = null, ho_RegionOpening = null;
            HObject ho_RegionDifference = null, ho_RegionErosion = null;
            HObject ho_ImageReduced1 = null, ho_Region1 = null, ho_SelectedRegions2 = null;
            HObject ho_SortedRegions = null;

            // Local control variables 

            HTuple hv_AutoThreshold1 = new HTuple(), hv_KirschThr = new HTuple();
            HTuple hv_KirschClosing = new HTuple(), hv_KirschOpening = new HTuple();
            HTuple hv_Number = new HTuple(), hv_exc = new HTuple();
            HTuple hv_NGCode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Region_dianji);
            HOperatorSet.GenEmptyObj(out ho_Region_citi);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageEdgeAmp);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            try
            {

                try
                {
                    hv_AutoThreshold1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AutoThreshold1 = hv_Parameter_BM.TupleSelect(
                            0);
                    }
                    hv_KirschThr.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KirschThr = hv_Parameter_BM.TupleSelect(
                            1);
                    }
                    hv_KirschClosing.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KirschClosing = hv_Parameter_BM.TupleSelect(
                            2);
                    }
                    hv_KirschOpening.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KirschOpening = hv_Parameter_BM.TupleSelect(
                            3);
                    }

                    //auto_threshold (GrayImage, Regions, 5)
                    ho_Regions.Dispose();
                    HOperatorSet.AutoThreshold(ho_GrayImage, out ho_Regions, hv_AutoThreshold1);

                    ho_ConnectedRegions2.Dispose();
                    HOperatorSet.Connection(ho_Regions, out ho_ConnectedRegions2);
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions1, (
                        (new HTuple("area")).TupleConcat("width")).TupleConcat("height"), "and",
                        ((new HTuple(9999)).TupleConcat(300)).TupleConcat(100), ((new HTuple(209999)).TupleConcat(
                        550)).TupleConcat(350));

                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_SelectedRegions1, out ho_RegionClosing, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 150, 999999);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    //shape_trans (RegionUnion, RegionTrans, 'convex')
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionUnion, out ho_RegionTrans, "rectangle2");



                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionTrans, out ho_ImageReduced
                        );
                    ho_ImageEdgeAmp.Dispose();
                    HOperatorSet.KirschAmp(ho_ImageReduced, out ho_ImageEdgeAmp);
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageEdgeAmp, out ho_Region, 0, hv_KirschThr);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, hv_KirschClosing);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening, hv_KirschOpening);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions,
                        "max_area", 70);



                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_RegionTrans, ho_SelectedRegions, out ho_RegionDifference
                        );
                    //erosion_circle (RegionDifference, RegionErosion, 5)
                    ho_Region_citi.Dispose();
                    ho_Region_citi = new HObject(ho_SelectedRegions);

                    ho_RegionErosion.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionDifference, out ho_RegionErosion,
                        1, 11);

                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionErosion, out ho_ImageReduced1
                        );
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, 20, 255);
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningCircle(ho_Region1, out ho_RegionOpening, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                    ho_SelectedRegions2.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions2, "area",
                        "and", 3500, 99999);
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions2, out hv_Number);
                    ho_SortedRegions.Dispose();
                    HOperatorSet.SortRegion(ho_SelectedRegions2, out ho_SortedRegions, "first_point",
                        "true", "column");
                    ho_Region_dianji.Dispose();
                    ho_Region_dianji = new HObject(ho_SortedRegions);


                }
                // catch (exc) 
                catch (hvppleException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_exc);
                    hv_NGCode.Dispose();
                    hv_NGCode = 34;
                }


                ho_Regions.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageEdgeAmp.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region1.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_SortedRegions.Dispose();

                hv_AutoThreshold1.Dispose();
                hv_KirschThr.Dispose();
                hv_KirschClosing.Dispose();
                hv_KirschOpening.Dispose();
                hv_Number.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                return;
            }
            catch (hvppleException HDevExpDefaultException)
            {
                ho_Regions.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageEdgeAmp.Dispose();
                ho_Region.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionErosion.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region1.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_SortedRegions.Dispose();

                hv_AutoThreshold1.Dispose();
                hv_KirschThr.Dispose();
                hv_KirschClosing.Dispose();
                hv_KirschOpening.Dispose();
                hv_Number.Dispose();
                hv_exc.Dispose();
                hv_NGCode.Dispose();

                throw HDevExpDefaultException;
            }
        }



        public static void shangpabuzu3(HObject ho_ImageReduced, HObject ho_Region_dianji, out HObject ho_RegionErr,
     HTuple hv_iScale_width_1, HTuple hv_iScale_height_1)
        {




            // Local iconic variables 

            HObject ho_SortedRegions, ho_RegionUnion1;
            HObject ho_RegionTrans, ho_Rectangle, ho_Rectangle1, ho_Rectangle_std;
            HObject ho_RegionUnion_std, ho_Rectangle2, ho_RegionUnion_1;
            HObject ho_RegionIntersection, ho_ConnectedRegions, ho_Rectangle3;
            HObject ho_RegionDifference, ho_RegionIntersection1, ho_ConnectedRegions1;
            HObject ho_RegionOpening, ho_RegionIntersection2, ho_SelectedRegions;
            HObject ho_RegionUnion;

            // Local control variables 

            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Deg = new HTuple(), hv_Row1 = new HTuple(), hv_hv_L = new HTuple();
            HTuple hv_column1 = new HTuple(), hv_hv_L1 = new HTuple();
            HTuple hv_column2 = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Length11 = new HTuple(), hv_Length21 = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row4 = new HTuple(), hv_Column3 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionErr);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_std);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion_std);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion_1);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            try
            {
                //*****
                //**将电极从左到右排序
                ho_SortedRegions.Dispose();
                HOperatorSet.SortRegion(ho_Region_dianji, out ho_SortedRegions, "first_point",
                    "true", "column");
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                HOperatorSet.SmallestRectangle2(ho_SortedRegions, out hv_Row, out hv_Column,
                    out hv_Phi, out hv_Length1, out hv_Length2);
                ho_RegionUnion1.Dispose();
                HOperatorSet.Union1(ho_SortedRegions, out ho_RegionUnion1);
                ho_RegionTrans.Dispose();
                HOperatorSet.ShapeTrans(ho_RegionUnion1, out ho_RegionTrans, "rectangle2");


                hv_Area.Dispose(); hv_Row2.Dispose(); hv_Column1.Dispose();
                HOperatorSet.AreaCenter(ho_SortedRegions, out hv_Area, out hv_Row2, out hv_Column1);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1,
                    hv_Length2);
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi, hv_Length1 - 30,
                    hv_Length2);


                hv_Deg.Dispose();
                HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);
                hv_Row1.Dispose();
                hv_Row1 = new HTuple(hv_Row);
                hv_hv_L.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_hv_L = (hv_Area / (hv_Length1 * 2)) / 2;
                }

                if (hv_column1 == null)
                    hv_column1 = new HTuple();
                hv_column1[0] = ((hv_Column.TupleSelect(0)) - (hv_Length2.TupleSelect(0))) + (hv_hv_L.TupleSelect(
                    0));
                if (hv_column1 == null)
                    hv_column1 = new HTuple();
                hv_column1[1] = ((hv_Column.TupleSelect(1)) + (hv_Length2.TupleSelect(1))) - (hv_hv_L.TupleSelect(
                    1));

                ho_Rectangle_std.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle_std, hv_Row1, hv_column1, hv_Phi,
                    hv_Length1, hv_hv_L);
                ho_RegionUnion_std.Dispose();
                HOperatorSet.Union1(ho_Rectangle_std, out ho_RegionUnion_std);
                //************
                hv_Row2.Dispose();
                hv_Row2 = new HTuple(hv_Row);
                hv_hv_L1.Dispose();
                hv_hv_L1 = new HTuple(hv_iScale_width_1);
                hv_hv_L.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_hv_L = new HTuple();
                    hv_hv_L = hv_hv_L.TupleConcat(hv_hv_L1);
                    hv_hv_L = hv_hv_L.TupleConcat(hv_hv_L1);
                }



                if (hv_column2 == null)
                    hv_column2 = new HTuple();
                hv_column2[0] = ((hv_Column.TupleSelect(0)) - (hv_Length2.TupleSelect(0))) + (hv_hv_L.TupleSelect(
                    0));
                if (hv_column2 == null)
                    hv_column2 = new HTuple();
                hv_column2[1] = ((hv_Column.TupleSelect(1)) + (hv_Length2.TupleSelect(1))) - (hv_hv_L.TupleSelect(
                    1));

                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row2, hv_column2, hv_Phi,
                    hv_Length1 - hv_iScale_height_1, hv_hv_L);
                ho_RegionUnion_1.Dispose();
                HOperatorSet.Union1(ho_Rectangle2, out ho_RegionUnion_1);




                ho_RegionIntersection.Dispose();
                HOperatorSet.Intersection(ho_RegionUnion_std, ho_Region_dianji, out ho_RegionIntersection
                    );
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionIntersection, out ho_ConnectedRegions);
                hv_Row3.Dispose(); hv_Column2.Dispose(); hv_Phi1.Dispose(); hv_Length11.Dispose(); hv_Length21.Dispose();
                HOperatorSet.SmallestRectangle2(ho_ConnectedRegions, out hv_Row3, out hv_Column2,
                    out hv_Phi1, out hv_Length11, out hv_Length21);
                ho_Rectangle3.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_Row3, hv_Column2, hv_Phi1,
                    hv_Length11, hv_Length21);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_Rectangle3, ho_Region_dianji, out ho_RegionDifference
                    );
                ho_RegionIntersection1.Dispose();
                HOperatorSet.Intersection(ho_RegionDifference, ho_RegionUnion_1, out ho_RegionIntersection1
                    );
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_RegionIntersection1, out ho_ConnectedRegions1);
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningCircle(ho_ConnectedRegions1, out ho_RegionOpening, 3);
                ho_RegionIntersection2.Dispose();
                HOperatorSet.Intersection(ho_RegionOpening, ho_RegionTrans, out ho_RegionIntersection2
                    );
                ho_SelectedRegions.Dispose();

                ho_RegionErr.Dispose();
                ho_RegionErr = new HObject(ho_RegionIntersection2);


                ho_SortedRegions.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionTrans.Dispose();
                ho_Rectangle.Dispose();
                ho_Rectangle1.Dispose();
                ho_Rectangle_std.Dispose();
                ho_RegionUnion_std.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionUnion_1.Dispose();
                ho_RegionIntersection.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_Rectangle3.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionIntersection2.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();

                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Area.Dispose();
                hv_Row2.Dispose();
                hv_Column1.Dispose();
                hv_Deg.Dispose();
                hv_Row1.Dispose();
                hv_hv_L.Dispose();
                hv_column1.Dispose();
                hv_hv_L1.Dispose();
                hv_column2.Dispose();
                hv_Row3.Dispose();
                hv_Column2.Dispose();
                hv_Phi1.Dispose();
                hv_Length11.Dispose();
                hv_Length21.Dispose();
                hv_Number.Dispose();
                hv_Area1.Dispose();
                hv_Row4.Dispose();
                hv_Column3.Dispose();

                return;
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("上爬不足检测失败" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                throw exc;
            }
        }

        public static void shangpabuzu4(HObject ho_ImageReduced, HObject ho_Region_dianji, out HObject ho_RegionErr, out HObject ho_Region_dianji2, out HObject ho_Rectangle_dianji2,
     HTuple hv_iScale_width_1, HTuple hv_iScale_height_1)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 
            HObject ho_SortedRegions, ho_Rectangle4, ho_RegionDilation;
            HObject ho_RegionUnion2, ho_ImageScaled, ho_Region, ho_ConnectedRegions2;
            HObject ho_SelectedRegions1, ho_SortedRegions1, ho_RegionUnion1;
            HObject ho_RegionTrans, ho_Rectangle, ho_Rectangle1, ho_Rectangle_std;
            HObject ho_RegionUnion_std, ho_Rectangle2, ho_RegionUnion_1;
            HObject ho_RegionIntersection, ho_ConnectedRegions, ho_Rectangle3;
            HObject ho_RegionDifference, ho_RegionIntersection1, ho_ConnectedRegions1;
            HObject ho_RegionOpening, ho_RegionIntersection2, ho_SelectedRegions;
            HObject ho_RegionUnion;

            // Local copy input parameter variables 
            HObject ho_ImageReduced_COPY_INP_TMP;
            ho_ImageReduced_COPY_INP_TMP = new HObject(ho_ImageReduced);



            // Local control variables 
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Deg = new HTuple(), hv_Row1 = new HTuple(), hv_hv_L = new HTuple();
            HTuple hv_column1 = new HTuple(), hv_hv_L1 = new HTuple();
            HTuple hv_column2 = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_Length11 = new HTuple(), hv_Length21 = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Row4 = new HTuple(), hv_Column3 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionErr);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle4);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion2);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle_std);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion_std);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion_1);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_Rectangle3);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            try
            {
                //*****
                //**将电极从左到右排序

                HOperatorSet.SortRegion(ho_Region_dianji, out ho_SortedRegions, "first_point", "true", "column");
                HOperatorSet.SmallestRectangle2(ho_SortedRegions, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle4, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.DilationRectangle1(ho_Rectangle4, out ho_RegionDilation, 11, 11);
                HOperatorSet.Union1(ho_RegionDilation, out ho_RegionUnion2);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageReduced_COPY_INP_TMP, ho_RegionUnion2, out ExpTmpOutVar_0);
                    ho_ImageReduced_COPY_INP_TMP = ExpTmpOutVar_0;
                }

                HOperatorSet.ScaleImage(ho_ImageReduced_COPY_INP_TMP, out ho_ImageScaled, 3, -100);
                HOperatorSet.Threshold(ho_ImageScaled, out ho_Region, 128, 255);
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions2);
                HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions1, "area", "and", 3500, 99999);
                HOperatorSet.SortRegion(ho_SelectedRegions1, out ho_SortedRegions1, "first_point", "true", "column");

                ho_Region_dianji = ho_SortedRegions1;


                HOperatorSet.Union1(ho_SortedRegions1, out ho_RegionUnion1);
                HOperatorSet.ShapeTrans(ho_RegionUnion1, out ho_RegionTrans, "rectangle2");
                HOperatorSet.SmallestRectangle2(ho_SortedRegions1, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);

                HOperatorSet.AreaCenter(ho_SortedRegions1, out hv_Area, out hv_Row2, out hv_Column1);
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);
                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, hv_Phi, hv_Length1 - 30, hv_Length2);


                //***使用平均宽度绘制的检测范围
                HOperatorSet.TupleDeg(hv_Phi, out hv_Deg);
                hv_Row1 = new HTuple(hv_Row);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_hv_L = (hv_Area / (hv_Length1 * 2)) / 2;
                }

                if (hv_column1 == null)
                    hv_column1 = new HTuple();
                hv_column1[0] = ((hv_Column.TupleSelect(0)) - (hv_Length2.TupleSelect(0))) + (hv_hv_L.TupleSelect(0));
                if (hv_column1 == null)
                    hv_column1 = new HTuple();
                hv_column1[1] = ((hv_Column.TupleSelect(1)) + (hv_Length2.TupleSelect(1))) - (hv_hv_L.TupleSelect(1));

                HOperatorSet.GenRectangle2(out ho_Rectangle_std, hv_Row1, hv_column1, hv_Phi, hv_Length1, hv_hv_L);
                HOperatorSet.Union1(ho_Rectangle_std, out ho_RegionUnion_std);

                //***使用自定义宽度绘制的检测范围
                hv_Row2 = new HTuple(hv_Row);
                hv_hv_L1 = new HTuple(hv_iScale_width_1);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_hv_L = new HTuple();
                    hv_hv_L = hv_hv_L.TupleConcat(hv_hv_L1);
                    hv_hv_L = hv_hv_L.TupleConcat(hv_hv_L1);
                }



                if (hv_column2 == null)
                    hv_column2 = new HTuple();
                hv_column2[0] = ((hv_Column.TupleSelect(0)) - (hv_Length2.TupleSelect(0))) + (hv_hv_L.TupleSelect(
                    0));
                if (hv_column2 == null)
                    hv_column2 = new HTuple();
                hv_column2[1] = ((hv_Column.TupleSelect(1)) + (hv_Length2.TupleSelect(1))) - (hv_hv_L.TupleSelect(
                    1));

                HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row2, hv_column2, hv_Phi, hv_Length1 - hv_iScale_height_1, hv_hv_L);
                HObject ho_RegionEro;
                HOperatorSet.Union1(ho_Rectangle2, out ho_RegionUnion_1);


                HOperatorSet.ErosionRectangle1(ho_RegionUnion_std, out ho_RegionEro, 5, 5);
                ho_Region_dianji2 = new HObject(ho_RegionEro);

                HOperatorSet.Intersection(ho_RegionEro, ho_Region_dianji, out ho_RegionIntersection);
                HOperatorSet.Connection(ho_RegionIntersection, out ho_ConnectedRegions);

                HOperatorSet.SmallestRectangle2(ho_ConnectedRegions, out hv_Row3, out hv_Column2, out hv_Phi1,
                    out hv_Length11, out hv_Length21);
                HOperatorSet.GenRectangle2(out ho_Rectangle3, hv_Row3, hv_Column2, hv_Phi1, hv_Length11, hv_Length21);

                ho_Rectangle_dianji2 = new HObject(ho_Rectangle3);

                HOperatorSet.Difference(ho_Rectangle3, ho_Region_dianji, out ho_RegionDifference);
                HOperatorSet.Intersection(ho_RegionDifference, ho_RegionUnion_1, out ho_RegionIntersection1);
                HOperatorSet.Connection(ho_RegionIntersection1, out ho_ConnectedRegions1);
                HOperatorSet.OpeningCircle(ho_ConnectedRegions1, out ho_RegionOpening, 3);
                HOperatorSet.Intersection(ho_RegionOpening, ho_RegionTrans, out ho_RegionIntersection2);

                HOperatorSet.SelectShape(ho_RegionIntersection2, out ho_SelectedRegions, ((new HTuple("area")).TupleConcat(
                    "width")).TupleConcat("anisometry"), "and", ((new HTuple(150)).TupleConcat(
                    10)).TupleConcat(0), ((new HTuple(99999)).TupleConcat(1000)).TupleConcat(
                    15));

                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                HOperatorSet.AreaCenter(ho_RegionUnion, out hv_Area1, out hv_Row4, out hv_Column3);
                ho_RegionErr = new HObject(ho_RegionUnion);


                return;
            }
            catch (Exception exc)
            {
                dhDll.frmMsg.Log("上爬不足检测失败" + exc.Message, "", null, dhDll.logDiskMode.Error, 0);
                throw exc;
            }
        }

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
                //throw new hvppleException("Wrong number of values of control parameter: 1");
            }
            if ((int)(((hv_MaxLineWidth_COPY_INP_TMP.TupleIsNumber())).TupleNot()) != 0)
            {
                //throw new hvppleException("Wrong type of control parameter: 1");
            }
            if ((int)(new HTuple(hv_MaxLineWidth_COPY_INP_TMP.TupleLessEqual(0))) != 0)
            {
                //throw new hvppleException("Wrong value of control parameter: 1");
            }
            if ((int)((new HTuple((new HTuple(hv_Contrast.TupleLength())).TupleNotEqual(1))).TupleAnd(
                new HTuple((new HTuple(hv_Contrast.TupleLength())).TupleNotEqual(2)))) != 0)
            {
                //throw new hvppleException("Wrong number of values of control parameter: 2");
            }
            if ((int)(new HTuple(((((hv_Contrast.TupleIsNumber())).TupleMin())).TupleEqual(
                0))) != 0)
            {
                //throw new hvppleException("Wrong type of control parameter: 2");
            }
            //Set and check ContrastHigh
            hv_ContrastHigh = hv_Contrast.TupleSelect(0);
            if ((int)(new HTuple(hv_ContrastHigh.TupleLess(0))) != 0)
            {
                //throw new hvppleException("Wrong value of control parameter: 2");
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
                //throw new hvppleException("Wrong value of control parameter: 2");
            }
            if ((int)(new HTuple(hv_ContrastLow.TupleGreater(hv_ContrastHigh))) != 0)
            {
                //throw new hvppleException("Wrong value of control parameter: 2");
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

            HTuple hv_Number = new HTuple();
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
            if ((int)((new HTuple(hv_Length1.TupleLess(0))).TupleOr(new HTuple(hv_Length2.TupleLess(0)))) != 0)
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
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static List<object> syPrintCheck0402_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }
        public static List<object> syPrintCheck0402_0_1(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static List<object> syPrintCheck0603_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }
        public static List<object> syPrintCheck0603_0_1(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static List<object> syPrintCheck1206_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static List<object> syPrintCheck1206_0_1(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static List<object> syPrintCheck0805_0(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }





        public static List<object> sySixSideDetect8_old(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static List<string> syAutoPrintMark(HObject hoImage, List<PointF[]> lkkPolygon, string strParams, ref List<object> listObj2Draw)
        {
            List<string> lnBarcode = null;
            return lnBarcode;
        }

        public static List<object> syDetectKH(HObject hoImage, List<PointF[]> lkkPolygon, string strParams)
        {
            List<object> lnBarcode = null;
            return lnBarcode;
        }

        public static void syShowXLD(HObject hoXLD, ref List<object> lobjdrawing, string strOKNG)
        {
            #region  *** 将一个XLD添加到绘图  ***
            try
            {

                HTuple hvRow, hvColumn;

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
                HObject hoIndex, hoContour, hoconnecation;
                HTuple hvCount, hvRow, hvColumn;
                //HOperatorSet.Connection(hoImage, out hoconnecation);
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

        #endregion

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

        #endregion
    }
}
