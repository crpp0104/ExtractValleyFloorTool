using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Desktop;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ArcCatalog;
using ESRI.ArcGIS.ArcCatalogUI;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using LeastSquaresFitStraightLine;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.Threading;

namespace ValleyBottomLineAddin
{
    public partial class GetValleyBottomLineWinForm : Form
    {
        #region 全局变量
        public IApplication app;                 //当前应用程序
        public IDocument currentDocument;        //当前应用程序中的地图文档
        public IMap currentMap;                  //当前应用程序的地图对象
        public IFeatureLayer currentFeatureLayer; //当前输入的矢量图层数据
        public IRasterLayer currentRasterLayer;   //当前输入的DEM文件
        public int RiverSegmentDistance;          //当前输入的河流分段距离
        public double inputFittingThresoldAngel;  //当前输入的河流分段拟合阈值
        public double inputSlopeAngel;            //当前输入的坡度阈值
        public double exploringDistance;          //当前输入的探索间距
        public string resultSavePath;             //结果保存路径
        #endregion
        public GetValleyBottomLineWinForm()
        {
            InitializeComponent();
            app = null;
            currentDocument = null;
            currentMap = null;
            currentFeatureLayer = null;
            currentRasterLayer = null;
            RiverSegmentDistance = 0;
            inputFittingThresoldAngel = 0;
            inputSlopeAngel = 0;
            exploringDistance = 0;
            resultSavePath = "";
        }

        private void GetValleyBottomLineWinForm_Load(object sender, EventArgs e)
        {

         
        }

        private void comboBoxSelectShapeFile_DrawItem(object sender, DrawItemEventArgs e)
        {
           
        }

        private void comboBoxSelectDemFile_DrawItem(object sender, DrawItemEventArgs e)
        {
           
        }

        private void buttonSelectshapeFile_Click(object sender, EventArgs e)
        {

            app = ArcMap.Application;
            textBoxSelectShapeFile.Text = "";
            IGxObjectFilterCollection ipFilterCollection = new GxDialogClass();
            IEnumGxObject ipSelectedObjects;
            IGxObjectFilter ipFilter8 = new GxFilterShapefilesClass();
            ipFilterCollection.AddFilter(ipFilter8, true);

            IGxDialog ipGxDialog = (IGxDialog)(ipFilterCollection);
            ipGxDialog.RememberLocation = true;
            ipGxDialog.Title = "Input river polyline features";
            ipGxDialog.AllowMultiSelect = false;
            ipGxDialog.RememberLocation = true;
            ipGxDialog.DoModalOpen((int)(Handle.ToInt32()), out ipSelectedObjects);
            IGxObject selectedObject = ipSelectedObjects.Next();
            if (selectedObject == null)
            {
                return;     //如果出现用户打开了窗口，但是没添加数据就关闭了窗口这种情况，就直接返回
            }
            IGxObject selectedObjectParent = selectedObject.Parent;

            if (selectedObjectParent is IGxDatabase)
            {

                IWorkspaceFactory pFileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                IWorkspace pWorkspace = pFileGDBWorkspaceFactory.OpenFromFile(selectedObjectParent.FullName, 0);
                IFeatureWorkspace featureWorkSpace = (IFeatureWorkspace)pWorkspace;
                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = featureWorkSpace.OpenFeatureClass(selectedObject.Name);
                pFeatureLayer.Name = selectedObject.Name;
                currentDocument = app.Document;
                IMxDocument currentmxDocument = currentDocument as IMxDocument;
                currentmxDocument.AddLayer(pFeatureLayer);
                currentmxDocument.UpdateContents();
                app.RefreshWindow();
                textBoxSelectShapeFile.Text = selectedObject.Name;
                currentFeatureLayer = pFeatureLayer;//给全局变量赋值
          
            }
            else if (selectedObject is IGxDataset)
            {
                IGxDataset gxDs = selectedObject as IGxDataset;
                if (gxDs != null)
                {

                    IWorkspace pWorkSpace = gxDs.Dataset.Workspace;
                    IFeatureWorkspace pFeatureWorkSpace = (IFeatureWorkspace)pWorkSpace;
                    IFeatureClass pFeatureclass = pFeatureWorkSpace.OpenFeatureClass(selectedObject.Name);
                    IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.FeatureClass = pFeatureclass;
                    pFeatureLayer.Name = selectedObject.Name;
                    currentDocument = app.Document;
                    IMxDocument currentmxDocument = currentDocument as IMxDocument;
                    currentmxDocument.AddLayer(pFeatureLayer);
                    currentmxDocument.UpdateContents();
                    app.RefreshWindow();
                    textBoxSelectShapeFile.Text = selectedObject.Name;
                    currentFeatureLayer = pFeatureLayer;//给全局变量赋值
                }
              
            }
            else
            {
                IWorkspaceFactory pWorkSpaceFactory = new ShapefileWorkspaceFactoryClass();
                string SelectShapeFileDictionaryName = System.IO.Path.GetDirectoryName(selectedObject.FullName);
                IWorkspace pWorkSpace = pWorkSpaceFactory.OpenFromFile(SelectShapeFileDictionaryName, 0);
                IFeatureWorkspace featureWorkSpace = (IFeatureWorkspace)pWorkSpace;
                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = featureWorkSpace.OpenFeatureClass(selectedObject.Name);
                pFeatureLayer.Name = selectedObject.Name;
                currentDocument = app.Document;
                IMxDocument currentmxDocument = currentDocument as IMxDocument;
                currentmxDocument.AddLayer(pFeatureLayer);
                currentmxDocument.UpdateContents();
                app.RefreshWindow();
                textBoxSelectShapeFile.Text = selectedObject.Name;
                currentFeatureLayer = pFeatureLayer;//给全局变量赋值
              

            }
        }

        private void comboBoxSelectShapeFile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonSelectDemFile_Click(object sender, EventArgs e)
        {
           
            app = ArcMap.Application;
            IGxObjectFilterCollection ipFilterCollection = new GxDialogClass();
            IEnumGxObject ipSelectedObjects;
            IGxObjectFilter ipFilter1 = new GxFilterRasterCatalogDatasetsClass();
            IGxObjectFilter ipFilter2 = new GxFilterRasterDatasetsClass();
            ipFilterCollection.AddFilter(ipFilter1, true);
            ipFilterCollection.AddFilter(ipFilter2, true);
            IGxDialog ipGxDialog = (IGxDialog)(ipFilterCollection);
            ipGxDialog.RememberLocation = true;
            ipGxDialog.Title = "Add DEM file";
            ipGxDialog.AllowMultiSelect = false;
            ipGxDialog.RememberLocation = true;
            ipGxDialog.DoModalOpen((int)(Handle.ToInt32()), out ipSelectedObjects);
            IGxObject selectedObject = ipSelectedObjects.Next();
            if (selectedObject == null)
            {
                return;     //如果出现用户打开了窗口，但是没添加数据就关闭了窗口这种情况，就直接返回
            }
            IGxObject selectedObjectParent = selectedObject.Parent;


            if (selectedObjectParent is IGxDatabase)
            {
                IWorkspaceFactory pFileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                IWorkspace pWorkspace = pFileGDBWorkspaceFactory.OpenFromFile(selectedObjectParent.FullName, 0);
                IRasterWorkspaceEx pRasterWorkSpace = (IRasterWorkspaceEx)pWorkspace;
                IRasterDataset rasterDataset = pRasterWorkSpace.OpenRasterDataset(selectedObject.Name);
                RasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(rasterDataset);
                pRasterLayer.Name = selectedObject.Name;
                currentDocument = app.Document;
                IMxDocument currentmxDocument = currentDocument as IMxDocument;
                currentmxDocument.AddLayer(pRasterLayer as ILayer);
          
                textBoxSelectDEMFile.Text = selectedObject.Name;
                currentRasterLayer = pRasterLayer;  //给全局变量赋值
                string units = currentmxDocument.FocusMap.MapUnits.ToString().Substring(4);
                if (units == "UnknownUnits")
                {
                    units = "UnknownUnits";
                    MessageBox.Show("Please specify the unit of data！");
                    buttonOK.Enabled = false;
                }
                labelExploringDistance.Text = "Input the distance that the river explores along the vertical" + " (Units:" + units + ")";

            }
            else if (selectedObject is IGxDataset)
            {

                IWorkspaceFactory pWorkSpaceFactory = new RasterWorkspaceFactoryClass();
                string SelectDEMFileDictionaryName = System.IO.Path.GetDirectoryName(selectedObject.FullName);
                IWorkspace pWorkSpace = pWorkSpaceFactory.OpenFromFile(SelectDEMFileDictionaryName, 0);
                IRasterWorkspace pRasterWorkSpace = (IRasterWorkspace)pWorkSpace;
                IRasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(pRasterWorkSpace.OpenRasterDataset(selectedObject.Name));
                pRasterLayer.Name = selectedObject.Name;
                currentDocument = app.Document;
                IMxDocument currentmxDocument = currentDocument as IMxDocument;
                currentmxDocument.AddLayer(pRasterLayer as ILayer);
                textBoxSelectDEMFile.Text = selectedObject.Name;
                string units = currentmxDocument.FocusMap.MapUnits.ToString().Substring(4);
                currentRasterLayer = pRasterLayer;  //给全局变量赋值
                if (units == "UnknownUnits")
                {
                    units = "UnknownUnits";
                    MessageBox.Show("Please specify the unit of data！");
                    buttonOK.Enabled = false;
                }
                labelExploringDistance.Text = "Input the distance that the river explores along the vertical" + " (Units:" + units + ")";
            }
            else if (selectedObject is IGxRasterDataset)
            {
                IWorkspaceFactory pWorkSpaceFactory = new RasterWorkspaceFactoryClass();
                string SelectDEMFileDictionaryName = System.IO.Path.GetDirectoryName(selectedObject.FullName);
                IWorkspace pWorkSpace = pWorkSpaceFactory.OpenFromFile(SelectDEMFileDictionaryName, 0);
                IRasterWorkspace pRasterWorkSpace = (IRasterWorkspace)pWorkSpace;
                IRasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(pRasterWorkSpace.OpenRasterDataset(selectedObject.Name));
                pRasterLayer.Name = selectedObject.Name;
                currentDocument = app.Document;
                IMxDocument currentmxDocument = currentDocument as IMxDocument;
                currentmxDocument.AddLayer(pRasterLayer as ILayer);
                textBoxSelectDEMFile.Text = selectedObject.Name;
                string units = currentmxDocument.FocusMap.MapUnits.ToString().Substring(4);
                currentRasterLayer = pRasterLayer;  //给全局变量赋值
                if (units == "UnknownUnits")
                {
                    units = "UnknownUnits";
                    MessageBox.Show("Please specify the unit of data！");
                    buttonOK.Enabled = false;
                }
                labelExploringDistance.Text = "Input the distance that the river explores along the vertical" + " (Units:" + units + ")";
            }

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
          
            inputFittingThresoldAngel = Convert.ToDouble(textBoxFittingThresold.Text);
            inputSlopeAngel = Convert.ToDouble(textBoxSlopeThreshod.Text);
            exploringDistance = Convert.ToDouble(textBoxExploringDistance.Text);
            RiverSegmentDistance = Convert.ToInt32(Math.Round(Convert.ToDouble(textBoxRiverSegmentDistance.Text)));
            app = ArcMap.Application;
            currentDocument = app.Document;
            IMxDocument mxCurrentDocument = (IMxDocument)currentDocument;
            currentMap = mxCurrentDocument.FocusMap;
            this.Close();
            #region
            // Create a CancelTracker
            ESRI.ArcGIS.esriSystem.ITrackCancel trackCancel = new ESRI.ArcGIS.Display.CancelTrackerClass();
            ESRI.ArcGIS.Framework.IProgressDialogFactory progressDialogFactory = new ESRI.ArcGIS.Framework.ProgressDialogFactoryClass();
             //Set the properties of the Step Progressor
            System.Int32 int32_hWnd =ArcMap.Application.hWnd;
            ESRI.ArcGIS.esriSystem.IStepProgressor stepProgressor = progressDialogFactory.Create(trackCancel, int32_hWnd);
            stepProgressor.MinRange = 0;
            stepProgressor.MaxRange = 100000;
            stepProgressor.StepValue = 1;
            stepProgressor.Message = "processing.....";
            stepProgressor.Hide();
            // Create the ProgressDialog. This automatically displays the dialog
            ESRI.ArcGIS.Framework.IProgressDialog2 progressDialog2 = (ESRI.ArcGIS.Framework.IProgressDialog2)stepProgressor; // Explict Cast

            // Set the properties of the ProgressDialog
            progressDialog2.CancelEnabled = true;
            progressDialog2.Title = "ExtractValleyFloorTool";
            progressDialog2.Animation = ESRI.ArcGIS.Framework.esriProgressAnimationTypes.esriProgressSpiral;
           
            // Step. Do your big process here.
           
            System.Int32 i = 0;
            for (i = 0; i <= 1; i++)
            {
                ESRI.ArcGIS.esriSystem.IStatusBar statusBar = ArcMap.Application.StatusBar;

                statusBar.set_Message(0, "processing...");
            
                //TODO:
                //Ideally you would call another sub/function/method from here to do the
                //work. For example read all files of a specified types on disk, loop
                //through a recordset, etc.
                //...
              

                LeastSquaresFitStraightLine.ExtractValleyBottomLine extractValleyBottomLine = new ExtractValleyBottomLine();
                extractValleyBottomLine.BottomLineofValley(currentFeatureLayer, currentMap, inputFittingThresoldAngel, resultSavePath,
                                                          inputSlopeAngel, currentRasterLayer, exploringDistance, RiverSegmentDistance);

                IWorkspaceFactory pWorkSpaceFactory = new ShapefileWorkspaceFactoryClass();
                IWorkspace pWorkSpace = pWorkSpaceFactory.OpenFromFile(resultSavePath, 0);
                IFeatureWorkspace featureWorkSpace = (IFeatureWorkspace)pWorkSpace;
                IFeatureLayer ValleyFloorOutlineLayer = new FeatureLayerClass();
                IFeatureLayer RiverReachLineLayer = new FeatureLayerClass();
                IFeatureLayer RiverVertLineLayer = new FeatureLayerClass();
                IFeatureClass ValleyFloorOutlineClass = featureWorkSpace.OpenFeatureClass("ValleyFloorOutline");
                ValleyFloorOutlineLayer.FeatureClass = ValleyFloorOutlineClass;
                ValleyFloorOutlineLayer.Name = ValleyFloorOutlineClass.AliasName;
                IFeatureClass RiverReachLineClass = featureWorkSpace.OpenFeatureClass("RiverReachLine");
                RiverReachLineLayer.FeatureClass = RiverReachLineClass;
                RiverReachLineLayer.Name = RiverReachLineClass.AliasName;
                IFeatureClass RiverVertLineClass = featureWorkSpace.OpenFeatureClass("RiverVertLine");
                RiverVertLineLayer.FeatureClass = RiverVertLineClass;
                RiverVertLineLayer.Name = RiverVertLineClass.AliasName;
                IMxDocument currentmxDocument = currentDocument as IMxDocument;
                currentmxDocument.AddLayer(ValleyFloorOutlineLayer);
                currentmxDocument.UpdateContents();
                app.RefreshWindow();
                currentmxDocument.AddLayer(RiverReachLineLayer);
                currentmxDocument.UpdateContents();
                app.RefreshWindow();
                //currentmxDocument.AddLayer(RiverVertLineLayer);
                currentmxDocument.UpdateContents();
                app.RefreshWindow();
                app.RefreshWindow();
                progressDialog2.Description = "Finish";
                //progressDialog2.HideDialog();
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(progressDialogFactory);
                //ESRI.ArcGIS.ADF.ComReleaser.ReleaseCOMObject(progressDialog2);
                //Check if the cancel button was pressed. If so, stop process
                return;
                
               
            }
            #endregion
          


        }

        private void buttonselectResultLocation_Click(object sender, EventArgs e)
        {
            textBoxSelectResultLocation.Text = "";
            IGxObjectFilterCollection ipFilterCollection = new GxDialogClass();
            IGxObjectFilter ipFilter1 = new GxFilterFileFolderClass();
            ipFilterCollection.AddFilter(ipFilter1, true);
            IGxDialog ipGxDialog = (IGxDialog)(ipFilterCollection);
            ipGxDialog.RememberLocation = true;
            ipGxDialog.Title = "Select the result to save the location";
            ipGxDialog.AllowMultiSelect = false;
            ipGxDialog.RememberLocation = true;
            ipGxDialog.DoModalSave((int)(Handle.ToInt32()));
            string ipGxDialogName = ipGxDialog.Name;
            IGxObject saveResultObject = ipGxDialog.FinalLocation;
            string Savepath = saveResultObject.FullName + "\\" + ipGxDialogName;
            if (!System.IO.Directory.Exists(Savepath))
            {
                System.IO.Directory.CreateDirectory(Savepath);
            }
            textBoxSelectResultLocation.Text = Savepath;
            resultSavePath = Savepath;    //给全局变量赋值，为了后面计算
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        
        }


    }
        
    

