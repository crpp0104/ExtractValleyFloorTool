using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.DataManagementTools;
using System.Runtime.InteropServices;

namespace LeastSquaresFitStraightLine
{
    #region       构造一个队列类
    public class Queue
    {
        private ArrayList pqueue;
        public Queue()
        {
            pqueue = new ArrayList();
        }
        public void EnQueue(object item)
        {
            pqueue.Add(item);

        }
        public void DeQueue()
        {

            pqueue.RemoveAt(0);
        
        }
        public object Peek()
        {

            return pqueue[0];
        
        }
        public void Clearqueue()
        {

            pqueue.Clear();
        
        }
        public int count()
        {

            return pqueue.Count;

        }
        public object getline(int id)
        {


            return pqueue[id];
        
        
        }



    }
    #endregion

    public class ExtractValleyBottomLine
    {
       
        #region  定义全局变量
        private static IPointCollection AccessPointcol;                       //曲线上面的点的集合
        private static int pos;                                               //记录点读取的位置
        public double riverSegmentDistance;                                   //记录河流分段的距离
        public static ArrayList KValueList;                                   //存储每条分段拟合直线的斜率
        public static IPointArray MidPointList;                               //存储每段分段拟合直线的中点
        private static ArrayList ReductionList;                               //存储用于计算在垂线方向上的探索点的坐标的m,n
        public static ArrayList ReductionList2;                               //存储用于计算中点上下距离一米的点的坐标的m,n
        private IPointArray SlopeList;                                        //存储单侧的坡度变化点
        private IPointArray AnotherSlopeList;                                 //存储另外一侧的坡度变化点
        private static IPointArray TopMidPointCollection;                     //存储中点上方一米的点
        private static IPointArray BottomMidPoint;                            //存储中点下方一米的点
        private static IPointArray TopMidPtSlopeArray;                        //存储中点上方一米的点生成的一侧坡度变化点
        private static IPointArray midPtSlopeArray;                           //存储中点生成的一侧坡度变化点
        private static IPointArray BottomMidPtSlopeArray;                     //存储中点下方的一米位置的点生成的一侧坡度变化点
        public static IPointArray DistanceFilterSlopeArray;                   //存储距离筛选后的坡度变化点
        public IFeatureClass dissolvefeatureclass;
        public IFeatureClass BufferFeatureclass;
        public IFeatureClass FeatureToPolygon;
        public IFeatureClass SlopePointLeft;
        public IFeatureClass SlopePointRight;
        #endregion
        //提取谷底线类的构造函数
        public ExtractValleyBottomLine()
        {
            pos = 0;
            riverSegmentDistance = 0;
            KValueList = new ArrayList();
            MidPointList = new PointArrayClass();
            ReductionList = new ArrayList();
            ReductionList2 = new ArrayList();
            SlopeList = new PointArrayClass();
            AnotherSlopeList = new PointArrayClass();
            TopMidPointCollection = new PointArrayClass();
            BottomMidPoint = new PointArrayClass();
            TopMidPtSlopeArray = new PointArrayClass(); 
            midPtSlopeArray = new PointArrayClass();
            BottomMidPtSlopeArray = new PointArrayClass();
            DistanceFilterSlopeArray = new PointArrayClass();
            
        }

        #region 通过矢量河网提取河网经过的栅格像元的坐标点
        public IPointCollection getRasterPoint(IFeatureLayer pfeatureLayer,IMap Pmap,int SegmentationDistance,out bool sidetype)
        {
            IPointCollection pointSets = new PolylineClass();
            IPointCollection InversePointsets = new PolylineClass();
            sidetype = true;    //默认情况下，sidetype的值为true,表示该河网要素在生成单侧缓冲区时方位正常。
            ICurve pCurve;
            int n =0;    
            IFeatureClass pFC = pfeatureLayer.FeatureClass;
            IDataset Pdataset = (IDataset)pfeatureLayer.FeatureClass;
            IWorkspace pw = Pdataset.Workspace;
            Geoprocessor gp = new Geoprocessor();
            gp.OverwriteOutput = true;
            gp.AddOutputsToMap = false;
            Dissolve myDissolve = new Dissolve();
            string in_features = pw.PathName + "\\" + pFC.AliasName + ".shp";
            string out_features = pw.PathName +"\\" + "dissolve_result.shp";
            IFields fields = pFC.Fields;
            myDissolve.in_features = in_features;
            myDissolve.out_feature_class = out_features;
            gp.Execute(myDissolve, null);
            IFeatureClass dissolve_featureclass = (pw as IFeatureWorkspace).OpenFeatureClass("dissolve_result.shp");
            dissolvefeatureclass = dissolve_featureclass;
            IFeatureCursor pCursor = dissolve_featureclass.Search(null,false);
            IFeature pfeature = pCursor.NextFeature();
  
            while (pfeature!= null)
            {

                pCurve = pfeature.Shape as ICurve;
                pCurve.SpatialReference = Pmap.SpatialReference;
                ICurve pxCurve;
                for (int i = 0; i < pCurve.Length; i = i + SegmentationDistance)
                {
                    pCurve.GetSubcurve(i, SegmentationDistance + i, false, out pxCurve);
                    pxCurve.SpatialReference = Pmap.SpatialReference;
                    if (pxCurve.FromPoint.Y > pxCurve.ToPoint.Y)
                    {
                        sidetype = false; //方位异常
                    
                    }
                    if (i == 0)
                    {
                        IPoint pointform = pxCurve.FromPoint;
                        pointSets.AddPoint(pointform);
                    }
                    IPoint pointTo = pxCurve.ToPoint;
                    pointSets.AddPoint(pointTo);
                }

                    pfeature = pCursor.NextFeature();
                    n++;
            }
            return pointSets;
        }
        #endregion

        #region    读取曲线上面的全部已知点函数
        public void GetPoint(IPointCollection pointsets)
        {
            AccessPointcol = new PolylineClass();
            AccessPointcol.AddPointCollection(pointsets);

        }
        #endregion

        #region 计算最小二乘法系数函数
        //num 为参与计算拟合的点的个数
        private static double[] ObtainFittingParameters(int num)
        {

            IPointCollection currentPoint = new PolylineClass();
            double[] facty = new double[2];
            for (int i = 0; i < num; i++)
            {
                IPoint point = new PointClass();
                int position = pos;
                point = AccessPointcol.get_Point(position+i);
                currentPoint.AddPoint(point);
            }
            double A=0, B=0, C=0, D=0,E=0;
            double a = 0, b = 0;
            for (int i = 0; i < num; i++)
            {
                 IPoint p1 = currentPoint.get_Point(i) as IPoint;
                 A += Math.Round((p1.X * p1.Y),6);
                 B += Math.Round(p1.X,6);
                 C += Math.Round(p1.Y,6);
                 D += Math.Round(p1.X * p1.X,6);
                 E += Math.Round(p1.X * (i+1),6);

            }
            a=(double)Math.Round(((num*A-B*C)/(num*D-Math.Pow(B,2))),12);
            b = (double)Math.Round(((D * C - B * A) / (num * D - Math.Pow(E, 2))),12);
            facty[0] = a;
            facty[1] = b;
            return facty;        
        
        }
        #endregion

        #region   计算起点终点坐标函数
        // num 为参与计算起点终点的点的个数
        private static IPointCollection CalculationOfCoord(int num)
        {
            IPointCollection currentPoint = new PolylineClass();
            IPointCollection StartEndPoint = new PolylineClass();
            double[] facc = new double[2];
            facc = ObtainFittingParameters(num);
            for (int i = 0; i < num; i++)
            {
                int position = pos;
                currentPoint.AddPoint(AccessPointcol.get_Point(position + i));
            }
            StartEndPoint.AddPoint(currentPoint.get_Point(0));
            StartEndPoint.get_Point(0).X = (double)(currentPoint.get_Point(0).X);
            StartEndPoint.get_Point(0).Y = (double)(facc[0] * StartEndPoint.get_Point(0).X + facc[1]);
            StartEndPoint.AddPoint(currentPoint.get_Point(num - 1));
            StartEndPoint.get_Point(1).X = (double)(currentPoint.get_Point(num - 1).X);
            StartEndPoint.get_Point(1).Y = (double) (facc[0] * StartEndPoint.get_Point(1).X + facc[1]);
            return StartEndPoint;
        }
        #endregion

        #region  计算夹角余弦值函数
        //num 为参与夹角计算的点的个数
        private static double AngleCal(int num)
        {
            double CosA = 1;
            double AB, BC, AC;
            IPointCollection StartEndPoint = new PolylineClass();
            StartEndPoint = CalculationOfCoord(num);
            IPointCollection currentPoint = new PolylineClass();
            currentPoint.AddPoint(StartEndPoint.get_Point(0));
            IPoint p1 = currentPoint.get_Point(0);
            currentPoint.AddPoint(StartEndPoint.get_Point(1));
            IPoint p2 = currentPoint.get_Point(1);
            IPoint point = new PointClass();
            if (pos + num >= AccessPointcol.PointCount)
            {
               return 0;
            }
            int position = pos;
            point = AccessPointcol.get_Point(position + num) as IPoint;
            currentPoint.AddPoint(point);
            IPoint p0 = new PointClass();
            IPoint p7 = new PointClass();
            IPoint p8 = new PointClass();
            p0 = currentPoint.get_Point(0);
            p7 = currentPoint.get_Point(1);
            p8 = currentPoint.get_Point(2);
            AB = Math.Pow(p0.X - p7.X, 2) + Math.Pow(p0.Y - p7.Y, 2);
            AC = Math.Pow(p0.X - p8.X, 2) + Math.Pow(p0.Y - p8.Y, 2);
            BC = Math.Pow(p7.X - p8.X, 2) + Math.Pow(p7.Y - p8.Y, 2);
            CosA = (AB + BC-AC) / (2 * Math.Sqrt(AB) * Math.Sqrt(BC));
            if (CosA > 1)
            {
                CosA = 1;
            }
            else if (CosA < -1)
            {
                CosA = -1;
            }
               
                return CosA;
        }
        #endregion
        #region 直线分段拟合的执行函数
        //ThresholdAngle为角度阈值的余弦值
        public  Queue Piecewiselinear(double ThresholdAngle)
        {
            ArrayList poslist = new ArrayList();
            ThresholdAngle = Math.Round(ThresholdAngle, 4);
            double Angle =1;
            int num =0;
            Queue myQueue = new Queue();//存储计算出的直线
            IPointCollection StartEndPoint = new PolylineClass();
            for (int i = 0; i < AccessPointcol.PointCount-1; i++)
            {
                num = 1;
                Angle = 1;   //角度的余弦值
                if (pos + 2 > AccessPointcol.PointCount - 1)
                {
                    IPoint p1 = new PointClass();
                    p1 = AccessPointcol.get_Point(AccessPointcol.PointCount-2);
                    StartEndPoint.AddPoint(p1);
                    p1 = new PointClass();
                    p1 = AccessPointcol.get_Point(AccessPointcol.PointCount - 1);
                    StartEndPoint.AddPoint(p1);
                    IPolyline myline = new PolylineClass();
                    IPoint pPoint1 = new PointClass();
                    pPoint1.X = StartEndPoint.get_Point(0).X;
                    pPoint1.Y = StartEndPoint.get_Point(0).Y;
                    myline.FromPoint = pPoint1;
                    IPoint pPoint2 = new PointClass();
                    pPoint2.X = StartEndPoint.get_Point(1).X;
                    pPoint2.Y = StartEndPoint.get_Point(1).Y;
                    myline.ToPoint = pPoint2;
                    myQueue.EnQueue(myline);
                    break;
                }
                while (Angle > ThresholdAngle)
                {
                    num += 1;
                    if (pos + num > AccessPointcol.PointCount)
                    {
                        break;
                    }
                    StartEndPoint = CalculationOfCoord(num);
                    Angle = AngleCal(num);
                    string angle = Angle.ToString();
                    if (angle.Length > 6)
                    {
                        angle = angle.Substring(0, 6);
                        Angle = double.Parse(angle);
                    }
                }
              
                for (int j = 0; j < StartEndPoint.PointCount; j++)
                {
                    
                    IPolyline myline = new PolylineClass();
                    IPoint pPoint1 = new PointClass();
                    pPoint1.X = StartEndPoint.get_Point(j).X;
                    pPoint1.Y = StartEndPoint.get_Point(j).Y;
                    myline.FromPoint = pPoint1;
                    IPoint pPoint2 = new PointClass();
                    pPoint2.X = StartEndPoint.get_Point(j+1).X;
                    pPoint2.Y = StartEndPoint.get_Point(j+1).Y;
                    myline.ToPoint = pPoint2;
                    myQueue.EnQueue(myline);
                    j++;
                }
                    pos += num - 1;
                    i = pos-1;
                    StartEndPoint.RemovePoints(0,2);
            }
            return myQueue;

        }
        #endregion

        #region 求每条直线的中点
        public IPointArray GetMidPoint(Queue myqueue)
        {
            for (int i = 0; i < myqueue.count(); i++)
            {
                IPolyline  myLine = myqueue.getline(i) as IPolyline;
                IPoint myPointFrom = myLine.FromPoint;
                IPoint mypointTo = myLine.ToPoint;
                IPoint MidPoint = new PointClass();
                Double X = (myPointFrom.X + mypointTo.X) / 2;
                double Y = (myPointFrom.Y + mypointTo.Y) / 2;
                MidPoint.X = X;
                MidPoint.Y = Y;
                MidPointList.Add(MidPoint);
            }
            return MidPointList;
        }
        #endregion

        #region  计算每段直线的斜率
        public  ArrayList GetKValue(Queue myQueue)
        {
            for (int i = 0; i < myQueue.count(); i++)
            {
                IPolyline myLine = new PolylineClass();
                myLine = myQueue.getline(i) as IPolyline;
                IPoint myPointFrom = myLine.FromPoint;
                IPoint mypointTo = myLine.ToPoint;
                double kValue = (myLine.ToPoint.Y - myLine.FromPoint.Y) / (myLine.ToPoint.X - myLine.FromPoint.X);
                KValueList.Add(kValue);
            
            }
            return KValueList;

        }
        #endregion

        #region 求m,n  x=n,y=m;
        public static ArrayList SeekReduction(Double diatance,Queue myQueue) 
        {
            double m=0, n=0;
            for (int i = 0; i < KValueList.Count; i++)
            {
                if ((double)KValueList[i] == 0)
                {
                    m = diatance;
                    n = 0; 
                }
                if ((double)KValueList[i] > 0)
                 {
                     double A = Math.Pow(diatance, 2);
                     double B = Math.Pow((double)KValueList[i], 2) + 1;
                     m = (Math.Sqrt(A / B));
                     n = -((double)KValueList[i] * m);
                     if (double.IsPositiveInfinity((double)KValueList[i]))
                     {
                         n = -diatance;
                         m = 0;
                     }
                     
                 }
                else if ((double)KValueList[i] < 0)
                 {
                     double A = Math.Pow(diatance, 2);
                     double B = Math.Pow((double)KValueList[i], 2) + 1;
                     m = -(Math.Sqrt(A / B));
                     n = -((double)KValueList[i] * m);
                     if (double.IsNegativeInfinity((double)KValueList[i]))
                     {

                         n = -diatance;
                         m = 0;
                     }
                    
                 }
                 
                 double []reduct = new double[2];
                 reduct[0] = n;
                 reduct[1] = m;
                 ReductionList.Add(reduct);
            }
            return ReductionList;
        
        }
        #endregion

        #region 对SeekReduction进行重载，计算中点上方的点,求m,n  x=n,y=m
        public  ArrayList seekReduction(Queue myQueue)
        {
            double m = 0, n = 0;
            for (int i = 0; i < KValueList.Count; i++)
            {
                if ((double)KValueList[i] == 0)
                {
                    m = 0;
                    n = -1;
                }
                if ((double)KValueList[i] > 0)
                {
                    double A = 1;
                    double B = Math.Pow((double)KValueList[i], 2) + 1;
                    n = Math.Sqrt(A / B);
                    m = (double)KValueList[i] * n;
                    if (double.IsPositiveInfinity((double)KValueList[i]))
                    {
                        m = 1;
                        n = 0;
                    }

                }
                else if ((double)KValueList[i] < 0)
                {

                    double A = 1;
                    double B = Math.Pow((double)KValueList[i], 2) + 1;
                    n =-(Math.Sqrt(A / B));
                    m = (double)KValueList[i] * n;
                    if (double.IsNegativeInfinity((double)KValueList[i]))
                    {
                        n = 0;
                        m = 1;
                    }
                }
                double[] reduct = new double[2];
                reduct[0] = n;
                reduct[1] = m;
                ReductionList2.Add(reduct);
            }
            return ReductionList2;

        }
        #endregion


        #region 获取坐标点位置的栅格的像元值赋给坐标点的Z坐标
        private static IPoint GetExplorePoint(int n, int lineNumber, IRasterLayer myRaster, IPointArray pointArray, ArrayList ReductionList)  //n为距离中点距离的倍数  pointArray是分段河流上某个位置的点的集合（中点，中上点，中下点）
        {
            IPoint point = new PointClass();
            IPoint point1 = new PointClass();
            double[] reduction = new double[2];
            reduction = (double[])ReductionList[lineNumber];
            point = pointArray.get_Element(lineNumber);
            IIdentify identify = (IIdentify)myRaster;
            point1.PutCoords(point.X + n * reduction[0], point.Y + n * reduction[1]);
            int arraycount;
            IArray array = identify.Identify(point1);          //捕捉异常，如果探索的点超出了栅格图像的范围，返回空的point,point.Z = double.NaN
            try
            {
                arraycount = array.Count;
            }
            catch 
            {
                point1.Z = double.NaN;
                return point1;
            }
            try                                              //捕捉异常，如果探索的点所在的栅格的位置的像元值为Nodata,将point.Z置为double.NaN
            {
                for (int j = 0; j < arraycount; j++)
                {
                    IRasterIdentifyObj rasterIdentifyobj = (IRasterIdentifyObj)array.get_Element(j);

                    point1.Z = double.Parse(rasterIdentifyobj.MapTip);

                }
            }
            catch (FormatException ex)
            {
                point1.Z = double.NaN;
            }
            return point1;
        }
        #endregion

        #region   新的计算坡度方法
        public double calSlope(IRasterLayer myraster,IPoint point)              //参数栅格影像，需要计算坡度的点的坐标
        {
            double slopenumber = 0;
            IPointArray Neiborhoodpoint = new PointArrayClass();
            IWorkspaceFactory workspaceFact = new RasterWorkspaceFactoryClass();
            IRasterWorkspace2 rasterwr = (IRasterWorkspace2)workspaceFact.OpenFromFile(System.IO.Path.GetDirectoryName(myraster.FilePath), 0);
            IRasterDataset rasterdataset = rasterwr.OpenRasterDataset(System.IO.Path.GetFileName(myraster.FilePath));
            IRasterDataset2 rasterdataset2 = (IRasterDataset2)rasterdataset;
            IRaster raster1 = rasterdataset2.CreateFullRaster();
            IRasterProps props = (IRasterProps)raster1;
            IPoint topPoint = new PointClass();
            topPoint.PutCoords(point.X, point.Y + props.MeanCellSize().Y);
            IPoint BottomPoint = new PointClass();
            BottomPoint.PutCoords(point.X, point.Y - props.MeanCellSize().Y);
            IPoint leftpoint = new PointClass();
            leftpoint.PutCoords(point.X - props.MeanCellSize().X, point.Y);
            IPoint rightpoint = new PointClass();
            rightpoint.PutCoords(point.X + props.MeanCellSize().X, point.Y);
            IPoint lefttoppoint = new PointClass();
            lefttoppoint.PutCoords(leftpoint.X, leftpoint.Y + props.MeanCellSize().Y);
            IPoint righttoppoint = new PointClass();
            righttoppoint.PutCoords(rightpoint.X, rightpoint.Y +props.MeanCellSize().Y);
            IPoint leftbottompoint = new PointClass();
            leftbottompoint.PutCoords(leftpoint.X, leftpoint.Y - props.MeanCellSize().Y);
            IPoint rightbottompoint = new PointClass();
            rightbottompoint.PutCoords(rightpoint.X, rightpoint.Y - props.MeanCellSize().Y);
            Neiborhoodpoint.Add(leftbottompoint);
            Neiborhoodpoint.Add(BottomPoint);
            Neiborhoodpoint.Add(rightbottompoint);
            Neiborhoodpoint.Add(leftpoint);
            Neiborhoodpoint.Add(point);
            Neiborhoodpoint.Add(rightpoint);
            Neiborhoodpoint.Add(lefttoppoint);
            Neiborhoodpoint.Add(topPoint);
            Neiborhoodpoint.Add(righttoppoint);
            for (int i = 0; i < Neiborhoodpoint.Count; i++)
            {
                int arraycount;
                IIdentify identify = (IIdentify)myraster;
                IArray array = identify.Identify(Neiborhoodpoint.get_Element(i));          //捕捉异常，如果探索的点超出了栅格图像的范围，返回空的point,point.Z = double.NaN
                try
                {
                    arraycount = array.Count;
                    for (int j = 0; j < arraycount; j++)
                    {
                        IRasterIdentifyObj rasterIdentifyobj = (IRasterIdentifyObj)array.get_Element(j);
                        Neiborhoodpoint.get_Element(i).Z = double.Parse(rasterIdentifyobj.MapTip);
                    }
                }
                catch
                {
                    Neiborhoodpoint.get_Element(i).Z = 0;

                }
                
            }
            double A, B;
            A = (Neiborhoodpoint.get_Element(6).Z - Neiborhoodpoint.get_Element(0).Z + 2 * (Neiborhoodpoint.get_Element(7).Z - Neiborhoodpoint.get_Element(1).Z) + Neiborhoodpoint.get_Element(8).Z - Neiborhoodpoint.get_Element(2).Z) / (8 * props.MeanCellSize().X);
            B = (Neiborhoodpoint.get_Element(2).Z - Neiborhoodpoint.get_Element(0).Z + 2 * (Neiborhoodpoint.get_Element(5).Z - Neiborhoodpoint.get_Element(3).Z) + Neiborhoodpoint.get_Element(8).Z - Neiborhoodpoint.get_Element(6).Z) / (8 * props.MeanCellSize().Y);
            slopenumber = Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));
            return slopenumber;
        }
        #endregion
        #region 获取一侧的坡度变化点
        public IPointArray getSlope(double distance,Queue myQueue,IRasterLayer myRaster,double T0,IPointArray pointarray,ArrayList ReductionList) //T0角度的正切值
        {
            IPointArray SlopeCollection = new PointArrayClass();
            for (int i = 0; i < myQueue.count(); i++)
            {
                 int n = 1;
                 IPoint StartPoint = new PointClass();
                 StartPoint = GetExplorePoint(0, i, myRaster, pointarray, ReductionList);    //获取需要探索的点的开始点的高程
                 double StartHeight = StartPoint.Z;
                 do
                 {
                      IPoint point = new PointClass();
                      point =  GetExplorePoint(n, i, myRaster,pointarray,ReductionList);
                      if (double.IsNaN(point.Z))
                      {
                          SlopeCollection.Add(point);
                          break;
                      }
                      if(point.Z > StartHeight)
                      {
                          double slope = calSlope(myRaster, point);
                          if (slope> T0 || slope < 0)
                          {
                               int position;
                               position = n;
                               position++;
                               IPoint point1 = new PointClass();
                               point1 = GetExplorePoint(position, i, myRaster, pointarray, ReductionList);
                               if (double.IsNaN (point1.Z))
                               {
                                   SlopeCollection.Add(point1);
                                   break;
                               }
                               slope = calSlope(myRaster, point1);
                               if (slope > T0 || slope < 0)
                               {
                                  position++;
                                  IPoint point2 = new PointClass();
                                  point2 = GetExplorePoint(position, i, myRaster,pointarray,ReductionList);
                                  if (double.IsNaN(point2.Z))
                                  {
                                    SlopeCollection.Add(point2);
                                    break;
                                  }                           
                                  slope = calSlope(myRaster, point2);
                                  if(slope > T0 || slope < 0)
                                  {
                                     SlopeCollection.Add(point);
                                     break;
                             
                                  }
                                }
                          }
                      }
                      n++;
                 }            
                 while (n != 0);
                 }
                    return SlopeCollection;
                }
        #endregion

        #region 获取每段河流拟合直线的中点的沿河流拟合直线方向的往上1米距离的点
        public  IPointArray getTopMidPoint(Queue myqueue)
        {
            double[] seekreductArray = new double[2];
            for (int i = 0; i < myqueue.count(); i++)
            {
                seekreductArray = (double[])ReductionList2[i];
                double n = seekreductArray[0];
                double m = seekreductArray[1];
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(MidPointList.get_Element(i).X + n, MidPointList.get_Element(i).Y + m);
                TopMidPointCollection.Add(pPoint);
            }
            return TopMidPointCollection;
        }
        #endregion

        #region 获取每段河流拟合直线的中点的沿河流拟合直线方向的往下1米距离的点
        public   IPointArray getBottomMidPoint(IPointArray TopMidPointArray,Queue myqueue)
        {
            for (int i = 0;i<myqueue.count(); i++)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(MidPointList.get_Element(i).X * 2 - TopMidPointArray.get_Element(i).X, MidPointList.get_Element(i).Y * 2 - TopMidPointArray.get_Element(i).Y);
                BottomMidPoint.Add(pPoint);
            }
            return BottomMidPoint;
        }
        #endregion

        #region 将通过中点的垂线计算出的坡度变化点与通过中点前后两点的垂线计算出的一侧坡度变化点会聚到一起
        public  IPointArray GetSlopeList1(double distance, Queue myQueue, IRasterLayer myRaster, double T0)
        {
            KValueList = GetKValue(myQueue);
            ReductionList2 = seekReduction(myQueue);
            MidPointList = GetMidPoint(myQueue);
            TopMidPointCollection = getTopMidPoint(myQueue);
            BottomMidPoint = getBottomMidPoint(TopMidPointCollection, myQueue);
            ReductionList = SeekReduction(distance, myQueue);
            TopMidPtSlopeArray = getSlope(distance, myQueue, myRaster, T0, TopMidPointCollection,ReductionList);
            midPtSlopeArray = getSlope(distance, myQueue, myRaster, T0, MidPointList,ReductionList);
            BottomMidPtSlopeArray = getSlope(distance, myQueue, myRaster, T0, BottomMidPoint,ReductionList);
            for (int i = 0; i < myQueue.count(); i++)
            {
                IPoint p1 = new PointClass();
                p1 = TopMidPtSlopeArray.get_Element(i);
                SlopeList.Add(p1);
                p1 = new PointClass();
                p1 = midPtSlopeArray.get_Element(i);
                SlopeList.Add(p1);
                p1 = new PointClass();
                p1 = BottomMidPtSlopeArray.get_Element(i);
                SlopeList.Add(p1);
            }
            return SlopeList;
        
        }
        #endregion

        #region   将通过中点的垂线计算出的坡度变化点与通过中点前后两点的垂线计算出的另一侧坡度变化点会聚到一起
        public IPointArray GetSlopeList2(double distance, Queue myQueue, IRasterLayer myRaster, double T0)
        {
            KValueList = new ArrayList();
            KValueList = GetKValue(myQueue);
            ReductionList2 = new ArrayList();
            ReductionList2 = seekReduction(myQueue);
            ArrayList ReductionList1 = new ArrayList();
            ReductionList = new ArrayList();
            ReductionList = SeekReduction(distance, myQueue);
            for (int i = 0; i < ReductionList.Count; i++)
            {
                double[] reduct = new double[2];
                reduct = (double[])ReductionList[i];
                reduct[0] = -reduct[0];
                reduct[1] = -reduct[1];
                ReductionList1.Add(reduct);
            }
           
            MidPointList = GetMidPoint(myQueue);
            TopMidPointCollection = getTopMidPoint(myQueue);
            BottomMidPoint = getBottomMidPoint(TopMidPointCollection, myQueue);
            TopMidPtSlopeArray = getSlope(distance, myQueue, myRaster, T0, TopMidPointCollection, ReductionList1);
            midPtSlopeArray = getSlope(distance, myQueue, myRaster, T0, MidPointList, ReductionList1);
            BottomMidPtSlopeArray = getSlope(distance, myQueue, myRaster, T0, BottomMidPoint, ReductionList1);
            for (int i = 0; i < myQueue.count(); i++)
            {
                IPoint p1 = new PointClass();
                p1 = TopMidPtSlopeArray.get_Element(i);
                AnotherSlopeList.Add(p1);
                p1 = new PointClass();
                p1 = midPtSlopeArray.get_Element(i);
                AnotherSlopeList.Add(p1);
                p1 = new PointClass();
                p1 = BottomMidPtSlopeArray.get_Element(i);
                AnotherSlopeList.Add(p1);
            }
            return AnotherSlopeList;
        
        }
        #endregion

        #region 筛选出距离符合要求的坡度变化点
        public IPointArray Distancefilter( Queue myqueue,IPointArray slopearraypoint,out double averagedistance,out IPointArray Singlesidevertpt)
        {
            IPoint www = slopearraypoint.get_Element(0);
            MidPointList = new PointArrayClass();
            MidPointList = GetMidPoint(myqueue);
            TopMidPointCollection = new PointArrayClass();
            TopMidPointCollection = getTopMidPoint(myqueue);
            BottomMidPoint = new PointArrayClass();
            BottomMidPoint = getBottomMidPoint(TopMidPointCollection, myqueue);

            ArrayList distanceArray = new ArrayList();
            Singlesidevertpt = new PointArrayClass();
            double distance = 0 ;
            double sum = 0;
            averagedistance = 0;
            ArrayList currentList = new ArrayList();
            ArrayList midpointdistancelist = new ArrayList();
            object[] slopeUnitarray = new object[3];
            int k = 0;
            for (int i = 0; i < slopearraypoint.Count; i++)
            {
                k = i % 3;
                slopeUnitarray[k] = slopearraypoint.get_Element(i);
                if(k==1)
                {
                    IPoint point1111 = slopearraypoint.get_Element(i);
                    Singlesidevertpt.Add(slopearraypoint.get_Element(i));          //收集每条直线中点对应的坡度变化点，用于后面生成河流垂线
                }
                if ((i+1) % 3 == 0)
                {
                    currentList.Add(slopeUnitarray);
                    slopeUnitarray = new object[3];
                }
            }
            for (int i = 0; i < currentList.Count; i++)
            {

                IPoint p1 = new PointClass();
                object[] slopeUnitarray1 = (object[])currentList[i];
                for (int j = 0; j < 3; j++)
                {
                   
                   p1 = slopeUnitarray1[j] as IPoint;
                   if (j == 0)
                   {

                       if ((double.IsNaN(p1.Z)))
                       {
                           distance = double.NaN;
                           distanceArray.Add(distance);
                       }
                       else
                       {
                           IPoint p2 = new PointClass();

                           p2 = TopMidPointCollection.get_Element(i) as IPoint;
                           distance = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                           distanceArray.Add(distance);
                       }
                    }
                    if (j == 1)
                    {
                        if ((double.IsNaN(p1.Z)))
                        {
                            distance = double.NaN;
                            distanceArray.Add(distance);
                            midpointdistancelist.Add(distance);
                        }
                        else
                        {
                            IPoint p3 = new PointClass();
                            p3 = MidPointList.get_Element(i) as IPoint;
                            distance = Math.Sqrt(Math.Pow(p1.X - p3.X, 2) + Math.Pow(p1.Y - p3.Y, 2));
                            distanceArray.Add(distance);
                            midpointdistancelist.Add(distance);
                        }
                    }
                     if(j == 2)
                     {
                         if ((double.IsNaN(p1.Z)))
                         {
                             distance = double.NaN;
                             distanceArray.Add(distance);

                         }
                         else
                         {
                              IPoint p4 = new PointClass();
                              p4 = BottomMidPoint.get_Element(i) as IPoint;
                              distance = Math.Sqrt(Math.Pow(p1.X - p4.X, 2) + Math.Pow(p1.Y - p4.Y, 2));
                              distanceArray.Add(distance);
                         }
                    }
                }
                    
            }
            for (int i = 0; i < distanceArray.Count; i++)
            {
                if (double.IsNaN((double)distanceArray[i]))
                {
                    continue;
                }
                sum += (double)distanceArray[i];
   
            }
            averagedistance = sum / distanceArray.Count;
            int count = slopearraypoint.Count;
            for (int i = 0; i < distanceArray.Count; i++)
            {
                if (double.IsNaN( (double)distanceArray[i]))
                {
                    distanceArray.RemoveAt(i);
                    slopearraypoint.Remove(i);
                    i--;
                
                }
                else if((double)distanceArray[i] > (averagedistance*3))
                {
                    slopearraypoint.Remove(i);
                    distanceArray.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < midpointdistancelist.Count; i++)
            {
                if (double.IsNaN((double)midpointdistancelist[i]))
                {
                    continue;
                }
                else if ((double)midpointdistancelist[i] > (averagedistance*3) )
                {
                    Singlesidevertpt.get_Element(i).Z = double.NaN;
                }

            }
           averagedistance = averagedistance * 5;      //是缓冲区的距离大一些，尽可能多覆盖异常点
           return slopearraypoint;
        }
        #endregion

        #region 生成单侧缓冲区
        public void greatSingleBuffer(IFeatureLayer in_features,string out_features,double distancebuffer,string line_side)
        {
            Geoprocessor gp = new Geoprocessor();
            IGeoProcessorResult gpResult = new GeoProcessorResultClass();
            gp.OverwriteOutput = true;
            gp.AddOutputsToMap = false;
            ESRI.ArcGIS.AnalysisTools.Buffer pBuffer = new ESRI.ArcGIS.AnalysisTools.Buffer(in_features, out_features,distancebuffer.ToString());
            pBuffer.dissolve_option = "ALL";
            pBuffer.ParameterInfo[3] = line_side;
            gpResult = (IGeoProcessorResult)gp.Execute(pBuffer, null);
            BufferFeatureclass = gp.Open(gpResult.ReturnValue) as IFeatureClass;
        }
        #endregion
        #region 执行空间查询来获取不在同侧的坡度变化点,并将其剔除掉
        public IPointArray RemoveDifferSlopePoint(IMap pmap,string path,IPointArray slopefilter,IFeatureLayer SinglebufferLayer,string name,out IFeatureClass SlopePointFeatureClass)
        {
            IFeatureClass slopepoint = GreateFeatureClass(pmap, path, name, esriGeometryType.esriGeometryPoint);
            for (int i = 0; i < slopefilter.Count; i++)
            {
                IPoint point = new PointClass();
                point = slopefilter.get_Element(i) as IPoint;
                IFeature SlopePointFeature = slopepoint.CreateFeature();
                SlopePointFeature.Shape = point;
                SlopePointFeature.Store();   
            }
            IFeatureLayer slopepointfLayer = new FeatureLayerClass();
            slopepointfLayer.FeatureClass = slopepoint;
            IFeatureCursor singlebufferCursor = SinglebufferLayer.Search(null, false);
            IFeature singlebufferFeature = singlebufferCursor.NextFeature();
            IGeometry singlebuffergeometry = singlebufferFeature.Shape;
            ISpatialFilter pSF = new SpatialFilterClass();
            pSF.Geometry = singlebuffergeometry;
            pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;//空间查询规则：点包含在面中
            IFeatureCursor pCursor = slopepointfLayer.FeatureClass.Search(pSF, false);
            IPointArray differslopeArray = new PointArrayClass();
            IFeature myfeature = pCursor.NextFeature();
            while ((myfeature!= null)) 
            {

                IPoint ppoint = new PointClass();
                ppoint = myfeature.Shape as IPoint;
                differslopeArray.Add(ppoint);
                myfeature = pCursor.NextFeature();
                
            }
        
            for (int i = 0; i < differslopeArray.Count; i++)
            {

                IPoint pPoint1 = differslopeArray.get_Element(i);
                for (int j = 0; j < slopefilter.Count; j++)
                {

                    IPoint ppoint2 = slopefilter.get_Element(j);
                    double X1 = Math.Round(pPoint1.X, 3);
                    double X2 = Math.Round(ppoint2.X, 3);
                    double Y1 = Math.Round(pPoint1.Y, 3);
                    double Y2 = Math.Round(ppoint2.Y, 3);
                    if ((X1 == X2)&&(Y1 ==Y2))
                    {
                      
                        slopefilter.Remove(j);
                        break;

                    }
                }
                for (int k = 1; k < slopepoint.FeatureCount(null)+1; k++)
                {
                    IFeature pf;
                    try
                    {
                      pf = slopepoint.GetFeature(k);
                      IPoint feature_Point = pf.Shape as IPoint;
                      if ((feature_Point.X == pPoint1.X) && (feature_Point.Y == pPoint1.Y))
                      {

                          pf.Delete();
                          break;

                      }
                    }
                    catch
                    {
                        continue;
                    }
                    
                   
                }
            
            }
            SlopePointFeatureClass = slopepoint;
            return slopefilter;
         
        }
        #endregion

        #region 生成谷底线
        public IFeatureClass greateValleyBottomLine(IPointArray slopelist, IPointArray slopeAnotherList, string filepath, string featureclassname, IMap pmap, IFeatureClass riververtline)
        {
            IFeatureClass slopebottomlineFC = GreateFeatureClass(pmap, filepath, featureclassname,esriGeometryType.esriGeometryPolyline);

            #region 第三种点的连接方法 先按照X或Y坐标的大小对点进行重新排序，然后在逐点搜索与其临近的点，进行连接
            IPointArray SortSlopePointArray = new PointArrayClass();
            for (int i = 0; i < slopelist.Count; i++)
            { 
               SortSlopePointArray.Add(slopelist.get_Element(i));
            }
            IPointArray SortAnotherPointArray = new PointArrayClass();
            for (int i = 0; i < AnotherSlopeList.Count; i++)
            {
                SortAnotherPointArray.Add(AnotherSlopeList.get_Element(i));
            }
             
            IFeatureCursor pcursor = dissolvefeatureclass.Search(null, false);
            IFeature dissolve_feature = pcursor.NextFeature();
            IPolyline pline = dissolve_feature.Shape as IPolyline;
            IPoint pline_pointFrom = pline.FromPoint;
            IPoint pline_pointTo = pline.ToPoint;
            double X1 = Math.Abs(pline_pointFrom.X - pline_pointTo.X);
            double Y1 = Math.Abs(pline_pointFrom.Y - pline_pointTo.Y);
            if (X1 < Y1)
            {
                double POINT_X;
                double POINT_Y;
                double POINT_Z;
                for (int i = SortSlopePointArray.Count; i > 0; i--)
                {
                    for (int j = 0; j < i - 1; j++)
                    {

                        if (SortSlopePointArray.get_Element(j).Y > SortSlopePointArray.get_Element(j + 1).Y)
                        {

                            POINT_X = SortSlopePointArray.get_Element(j).X;
                            POINT_Y = SortSlopePointArray.get_Element(j).Y;
                            POINT_Z = SortSlopePointArray.get_Element(j).Z;
                            SortSlopePointArray.get_Element(j).X = SortSlopePointArray.get_Element(j + 1).X;
                            SortSlopePointArray.get_Element(j).Y = SortSlopePointArray.get_Element(j + 1).Y;
                            SortSlopePointArray.get_Element(j).Z = SortSlopePointArray.get_Element(j + 1).Z;
                            SortSlopePointArray.get_Element(j + 1).X = POINT_X;
                            SortSlopePointArray.get_Element(j + 1).Y = POINT_Y;
                            SortSlopePointArray.get_Element(j + 1).Z = POINT_Z;

                        }

                    }

                }
                POINT_X = POINT_Y = POINT_Z = 0;
                for (int i = SortAnotherPointArray.Count; i > 0; i--)
                {
                    for (int j = 0; j < i - 1; j++)
                    {

                        if (SortAnotherPointArray.get_Element(j).Y > SortAnotherPointArray.get_Element(j + 1).Y)
                        {

                            POINT_X = SortAnotherPointArray.get_Element(j).X;
                            POINT_Y = SortAnotherPointArray.get_Element(j).Y;
                            POINT_Z = SortAnotherPointArray.get_Element(j).Z;
                            SortAnotherPointArray.get_Element(j).X = SortAnotherPointArray.get_Element(j + 1).X;
                            SortAnotherPointArray.get_Element(j).Y = SortAnotherPointArray.get_Element(j + 1).Y;
                            SortAnotherPointArray.get_Element(j).Z = SortAnotherPointArray.get_Element(j + 1).Z;
                            SortAnotherPointArray.get_Element(j + 1).X = POINT_X;
                            SortAnotherPointArray.get_Element(j + 1).Y = POINT_Y;
                            SortAnotherPointArray.get_Element(j + 1).Z = POINT_Z;
                        }
                    }
                }
            }
            else
            {
                double POINT_X;
                double POINT_Y;
                double POINT_Z;
                for (int i = SortSlopePointArray.Count; i > 0; i--)
                {
                    for (int j = 0; j < i - 1; j++)
                    {

                        if (SortSlopePointArray.get_Element(j).X > SortSlopePointArray.get_Element(j + 1).X)
                        {

                            POINT_X = SortSlopePointArray.get_Element(j).X;
                            POINT_Y = SortSlopePointArray.get_Element(j).Y;
                            POINT_Z = SortSlopePointArray.get_Element(j).Z;
                            SortSlopePointArray.get_Element(j).X = SortSlopePointArray.get_Element(j + 1).X;
                            SortSlopePointArray.get_Element(j).Y = SortSlopePointArray.get_Element(j + 1).Y;
                            SortSlopePointArray.get_Element(j).Z = SortSlopePointArray.get_Element(j + 1).Z;
                            SortSlopePointArray.get_Element(j + 1).X = POINT_X;
                            SortSlopePointArray.get_Element(j + 1).Y = POINT_Y;
                            SortSlopePointArray.get_Element(j + 1).Z = POINT_Z;

                        }

                    }

                }
                POINT_X = POINT_Y = POINT_Z = 0;
                for (int i = SortAnotherPointArray.Count; i > 0; i--)
                {
                    for (int j = 0; j < i - 1; j++)
                    {

                        if (SortAnotherPointArray.get_Element(j).X > SortAnotherPointArray.get_Element(j + 1).X)
                        {

                            POINT_X = SortAnotherPointArray.get_Element(j).X;
                            POINT_Y = SortAnotherPointArray.get_Element(j).Y;
                            POINT_Z = SortAnotherPointArray.get_Element(j).Z;
                            SortAnotherPointArray.get_Element(j).X = SortAnotherPointArray.get_Element(j + 1).X;
                            SortAnotherPointArray.get_Element(j).Y = SortAnotherPointArray.get_Element(j + 1).Y;
                            SortAnotherPointArray.get_Element(j).Z = SortAnotherPointArray.get_Element(j + 1).Z;
                            SortAnotherPointArray.get_Element(j + 1).X = POINT_X;
                            SortAnotherPointArray.get_Element(j + 1).Y = POINT_Y;
                            SortAnotherPointArray.get_Element(j + 1).Z = POINT_Z;

                        }

                    }

                }

            }
            //计算左侧各点之间的距离，找出最大值，搜索半径确定为最大值
            int k = 0;
            double maxPointDistanceLeft = 0;
            ArrayList PointDistanceListLeft = new ArrayList();
            double searchradiusLeft = 0;
            for (int i = 0; i < SortSlopePointArray.Count; i++)
            {
                 k = i + 1;
                 if (k == SortSlopePointArray.Count)
                 {
                     break;
                 }
                 double pointdistanceLeft = Math.Sqrt(Math.Pow((SortSlopePointArray.get_Element(i).X - SortSlopePointArray.get_Element(k).X), 2) +
                                                  Math.Pow((SortSlopePointArray.get_Element(i).Y - SortSlopePointArray.get_Element(k).Y), 2));
                 PointDistanceListLeft.Add(pointdistanceLeft);
                 
            }
            PointDistanceListLeft.Sort();
            maxPointDistanceLeft = Convert.ToDouble(PointDistanceListLeft[PointDistanceListLeft.Count - 1]);
            searchradiusLeft = maxPointDistanceLeft;
            //左侧：逐点搜索附近的点，连接最近邻的点
            for (int i = 0; i < SortSlopePointArray.Count; i++)
            {
                int j =i+1;
                if (j == SortSlopePointArray.Count)
                {
                    break;
                
                }
                IPoint point1 = SortSlopePointArray.get_Element(i);
                IPoint point2 = SortSlopePointArray.get_Element(j);
                double tempdistance = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
                double mintempdistance = tempdistance;
                IPoint Topoint = point2;
                int pos = i + 1;
                while (tempdistance < searchradiusLeft)
                { 
                   j++;
                   if (j == SortSlopePointArray.Count)
                   {
                       break;
                   
                   }
                   point2 = SortSlopePointArray.get_Element(j);
                   tempdistance = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
                   if (mintempdistance > tempdistance)
                   {

                       mintempdistance = tempdistance;
                       Topoint = point2;
                       pos=j;
                   }
   
                }
                IPolyline SlopePolyline = new PolylineClass();
                SlopePolyline.FromPoint = point1;
                SlopePolyline.ToPoint = Topoint;
                IFeature slopePolylineFeature = slopebottomlineFC.CreateFeature();
                slopePolylineFeature.Shape = SlopePolyline;
                slopePolylineFeature.Store();
                i = pos-1;
            }
            //计算右侧各点之间的距离，找出最大值，搜索半径确定为最大值
            int m = 0;
            double maxPointDistanceRight = 0;
            ArrayList PointDistanceListRight = new ArrayList();
            double searchradiusRight = 0;
            for (int i = 0; i < SortAnotherPointArray.Count; i++)
            {
                m = i + 1;
                if (m == SortAnotherPointArray.Count)
                {
                    break;
                }
                double pointdistanceRight = Math.Sqrt(Math.Pow((SortAnotherPointArray.get_Element(i).X - SortAnotherPointArray.get_Element(m).X), 2) +
                                                 Math.Pow((SortAnotherPointArray.get_Element(i).Y - SortAnotherPointArray.get_Element(m).Y), 2));
                PointDistanceListRight.Add(pointdistanceRight);

            }
            PointDistanceListRight.Sort();
            maxPointDistanceRight = Convert.ToDouble(PointDistanceListRight[PointDistanceListRight.Count - 1]);
            searchradiusRight = maxPointDistanceRight;
            //右侧：逐点搜索附近的点，连接最近邻的点
            for (int i = 0; i < SortAnotherPointArray.Count; i++)
            {
                int j = i + 1;
                if (j == SortAnotherPointArray.Count)
                {
                    break;
                
                }
                IPoint point1 = SortAnotherPointArray.get_Element(i);
                IPoint point2 = SortAnotherPointArray.get_Element(j);
                double tempdistance = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
                double mintempdistance = tempdistance;
                IPoint Topoint = point2;
                int pos = i + 1;
                while (tempdistance < searchradiusRight)
                {
                    j++;
                    if (j == SortAnotherPointArray.Count)
                    {
                        break;
                    
                    }
                    point2 = SortAnotherPointArray.get_Element(j);
                    tempdistance = Math.Sqrt(Math.Pow((point1.X - point2.X), 2) + Math.Pow((point1.Y - point2.Y), 2));
                    if (mintempdistance > tempdistance)
                    {

                        mintempdistance = tempdistance;
                        Topoint = point2;
                        pos=j;
                    }

                }
                IPolyline SlopePolyline = new PolylineClass();
                SlopePolyline.FromPoint = point1;
                SlopePolyline.ToPoint = Topoint;
                IFeature slopePolylineFeature = slopebottomlineFC.CreateFeature();
                slopePolylineFeature.Shape = SlopePolyline;
                slopePolylineFeature.Store();
                i = pos - 1;
            }
            
            #endregion
            IPolyline closePolyline1 = new PolylineClass();
            IPolyline closePolyline2 = new PolylineClass();
            closePolyline1.FromPoint = slopelist.get_Element(0);
            closePolyline1.ToPoint =   slopeAnotherList.get_Element(0);
            closePolyline2.FromPoint = slopelist.get_Element(slopelist.Count - 1);
            closePolyline2.ToPoint = slopeAnotherList.get_Element(slopeAnotherList.Count - 1);
            IFeature closeFeature1 = slopebottomlineFC.CreateFeature();
            closeFeature1.Shape = closePolyline1;
            closeFeature1.Store();
            IFeature closeFeature2 = slopebottomlineFC.CreateFeature();
            closeFeature2.Shape = closePolyline2;
            closeFeature2.Store();
            string infeatures = filepath + "\\ValleyFloorOutline.shp";
            string outfeatures = filepath + "\\featuretoPolygon.shp";
            Geoprocessor GP1 = new Geoprocessor();
            IGeoProcessorResult pGeoResult1 = new GeoProcessorResultClass();
            IGeoProcessorResult gGeoResult2 = new GeoProcessorResultClass();
            GP1.OverwriteOutput = true;
            ESRI.ArcGIS.DataManagementTools.FeatureToPolygon pFeatureToPolygon = new FeatureToPolygon(infeatures, outfeatures);
            pGeoResult1 = (IGeoProcessorResult)GP1.Execute(pFeatureToPolygon, null);
            IFeatureClass FeatureToPolygon = GP1.Open(pGeoResult1.ReturnValue) as IFeatureClass;
            IDataset FeatureToPolygonDs = FeatureToPolygon as IDataset;
            IDataset riververtlineDs = riververtline as IDataset;
            string in_features = filepath + "\\RiverVertLine1.shp";
            string clip_features = filepath + "\\featuretoPolygon.shp";
            string out_features = filepath + "\\RiverVertLine.shp";
            Geoprocessor GP2 = new Geoprocessor();
            GP2.OverwriteOutput = true;
            ESRI.ArcGIS.AnalysisTools.Clip pClip = new ESRI.ArcGIS.AnalysisTools.Clip(in_features, clip_features, out_features);
            gGeoResult2 = (IGeoProcessorResult)GP2.Execute(pClip, null);
            IFeatureClass riververtlineFC = GP2.Open(gGeoResult2.ReturnValue) as IFeatureClass;
            IFeature CloseFeature1 = slopebottomlineFC.GetFeature(slopebottomlineFC.FeatureCount(null) - 1);
            IFeature CloseFeature2 = slopebottomlineFC.GetFeature(slopebottomlineFC.FeatureCount(null) - 2);
            CloseFeature1.Delete();
            CloseFeature2.Delete();
            FeatureToPolygonDs.Delete();
            riververtlineDs.Delete();
            Marshal.FinalReleaseComObject(FeatureToPolygonDs);
            Marshal.FinalReleaseComObject(riververtlineDs);
            return slopebottomlineFC;
        
        }
        #endregion

        #region 创建矢量要素集
        public IFeatureClass GreateFeatureClass(IMap Pmap, string filepath, string featureclassname, esriGeometryType geotype)
        {
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            //object ID字段
            IField oidField = new FieldClass();
            IFieldEdit oidFieldEdit = (IFieldEdit)oidField;
            oidFieldEdit.Name_2 = "OID";
            oidFieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(oidField);
            //创建几何定义对象
            IGeometryDef geometryDef = new GeometryDefClass();
            IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
            geometryDefEdit.GeometryType_2 = geotype;
            geometryDefEdit.SpatialReference_2 = Pmap.SpatialReference;
            ISpatialReference spatialReference = Pmap.SpatialReference;
            IField geometryField = new FieldClass();
            IFieldEdit geometryFieldEdit = (IFieldEdit)geometryField;
            geometryFieldEdit.Name_2 = "Shape";
            geometryFieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            geometryFieldEdit.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(geometryField);
            IWorkspaceFactory ShapeFile_Spacefactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFeatureWorkSpace = ShapeFile_Spacefactory.OpenFromFile(filepath, 0) as IFeatureWorkspace;
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IEnumFieldError enumFieldError = null;
            IFields validatedFields = null; //将传入字段 转成 validatedFields
            fieldChecker.ValidateWorkspace = (IWorkspace)pFeatureWorkSpace;
            fieldChecker.Validate(fields, out enumFieldError, out validatedFields);
            try
            {
                IDataset pdataset = pFeatureWorkSpace.OpenFeatureClass(featureclassname) as IDataset;
                pdataset.Delete();
            }
            catch
            {

            }
            IFeatureClass featureClass = pFeatureWorkSpace.CreateFeatureClass(featureclassname, validatedFields, null, null, esriFeatureType.esriFTSimple, "shape", "");
            return featureClass;
        }
        #endregion

        #region 向要素集中添加要素
        public void AddFeature(IPolyline myline,IFeatureClass pFC)
        {
            IFeature myfeature = pFC.CreateFeature();
            myfeature.Shape = myline;
            myfeature.Store();
        
        }
        #endregion

        #region 将河流分段直线直线转化成矢量图层，并将河谷宽度字段写进图层，并给宽度字段赋值
        public IFeatureClass SegmentedRiverToFc(string filepath, string featureclassname, IMap pmap, Queue myqueue)
        {
            IFeatureClass SegmentedRiverfeatureclass = GreateFeatureClass(pmap, filepath, featureclassname,esriGeometryType.esriGeometryPolyline);
            for (int i = 0; i < myqueue.count(); i++)
            {
                IPolyline SegmentedRiverLine = new PolylineClass();
                SegmentedRiverLine = myqueue.getline(i) as IPolyline;
                AddFeature(SegmentedRiverLine, SegmentedRiverfeatureclass);
            }
            return SegmentedRiverfeatureclass;
        }
        #endregion

        #region 生成河流垂线
        public IFeatureClass Riververticalline(IMap pmap, string filepath, IPointArray slopepoint1, IPointArray slopepoint2)
        {
            IFeatureClass riververtline = GreateFeatureClass(pmap, filepath, "RiverVertLine1", esriGeometryType.esriGeometryPolyline);
                
            for (int i = 0; i < slopepoint1.Count - 1; i++)
            {

                    IPoint p1 = new PointClass();
                    IPoint p2 = new PointClass();
                    p1 = slopepoint1.get_Element(i);
                    p2 = slopepoint2.get_Element(i);
 
                    if (((double.IsNaN(p1.X) == false) && (double.IsNaN(p1.Y) == false) && (double.IsNaN(p1.Z) == false)) && ((double.IsNaN(p2.X) == false) && (double.IsNaN(p2.Y) == false) && (double.IsNaN(p2.Z) == false)))
                    {
                        IPolyline riververpolyline = new PolylineClass();
                        riververpolyline.FromPoint = p1;
                        riververpolyline.ToPoint = p2;
                        IFeature riververtlinefeature = riververtline.CreateFeature();
                        riververtlinefeature.Shape = riververpolyline;
                        riververtlinefeature.Store();

                    }
               }
                return riververtline;
        }
        #endregion

        #region 提取谷底线的执行函数
        public void BottomLineofValley(IFeatureLayer MyfeatureLayer, IMap ActiveMap, double LinearFittingAngle, String File_Path, double SlopeAngle, IRasterLayer myRasterLayer, double exploreDistance,int SegmentionDistance)
        {
            IPointCollection pArray = new PolylineClass();
            Queue myQueue;
            double averagedistance = 0;
            IPointArray slopeArray1 = new PointArrayClass();
            IPointArray slopeArray2 = new PointArrayClass();
            IPointArray singlvertPoint1 = new PointArrayClass();
            IPointArray singlvertpoint2 = new PointArrayClass();
            bool sidetype;
            //生成拟合直线
            pArray = getRasterPoint(MyfeatureLayer,ActiveMap, SegmentionDistance, out sidetype);
            GetPoint(pArray);
            LinearFittingAngle = Math.Cos((LinearFittingAngle * 2 * Math.PI) / 360);
            myQueue = Piecewiselinear(LinearFittingAngle);
            IFeatureClass segmentriver = SegmentedRiverToFc(File_Path, "RiverReachLine", ActiveMap, myQueue);
            SlopeAngle = Math.Tan(SlopeAngle * 2 * Math.PI / 360);
            //获取左侧坡度变化点
            slopeArray1 = GetSlopeList1(exploreDistance, myQueue, myRasterLayer, SlopeAngle);
            IPointArray slopefilter1 = Distancefilter(myQueue, slopeArray1, out averagedistance, out singlvertPoint1);
            string out_feature = File_Path + "\\Right_Buffer.shp";
            if (sidetype)
            {
               greatSingleBuffer(MyfeatureLayer, out_feature, averagedistance, "RIGHT");
            }
            else
            {
                greatSingleBuffer(MyfeatureLayer, out_feature, averagedistance, "LEFT");

            }
         
            IFeatureLayer Rightbufferlayer = new FeatureLayerClass();
            Rightbufferlayer.FeatureClass = BufferFeatureclass;
            IPointArray removedslopepoint1 = RemoveDifferSlopePoint(ActiveMap, File_Path, slopefilter1, Rightbufferlayer, "SlopePoint_Left",out SlopePointLeft);
            IDataset BufferRightDs = BufferFeatureclass as IDataset;
            IDataset SlopePointLeftDs = SlopePointLeft as IDataset;
            BufferRightDs.Delete();
            Marshal.FinalReleaseComObject(BufferRightDs);
            Marshal.FinalReleaseComObject(Rightbufferlayer);
            //获取右侧坡度变化点
            slopeArray2 = GetSlopeList2(exploreDistance, myQueue, myRasterLayer, SlopeAngle);
            averagedistance = 0;
            IPointArray slopefilter2 = Distancefilter(myQueue, slopeArray2, out averagedistance, out singlvertpoint2);
            out_feature = File_Path + "\\Left_Buffer.shp";
            if (sidetype)
            {
                 greatSingleBuffer(MyfeatureLayer, out_feature, averagedistance, "LEFT");
            }
            else
            {

                greatSingleBuffer(MyfeatureLayer, out_feature, averagedistance, "RIGHT");
            }
            IFeatureLayer Leftbufferlayer = new FeatureLayerClass();
            Leftbufferlayer.FeatureClass = BufferFeatureclass;
            IPointArray removedslopepoint2 = RemoveDifferSlopePoint(ActiveMap, File_Path, slopefilter2, Leftbufferlayer, "SlopePoint_Right",out SlopePointRight);
            IDataset BufferLeftDs = BufferFeatureclass as IDataset;
            IDataset SlopePointRightDs = SlopePointRight as IDataset;
            BufferLeftDs.Delete();
            Marshal.FinalReleaseComObject(BufferLeftDs);
            Marshal.FinalReleaseComObject(Leftbufferlayer);
            //生成河流垂线
            IFeatureClass RiververtLineFC = Riververticalline(ActiveMap, File_Path, singlvertPoint1, singlvertpoint2);
            //生成谷底线
            IFeatureClass ValleyLine = greateValleyBottomLine(removedslopepoint1, removedslopepoint2, File_Path, "ValleyFloorOutline", ActiveMap, RiververtLineFC);
            SlopePointLeftDs.Delete();
            SlopePointRightDs.Delete();
            Marshal.FinalReleaseComObject(SlopePointLeftDs);
            Marshal.FinalReleaseComObject(SlopePointRightDs);
        }
        #endregion
    }



    }
   
        
    
    
    
  



