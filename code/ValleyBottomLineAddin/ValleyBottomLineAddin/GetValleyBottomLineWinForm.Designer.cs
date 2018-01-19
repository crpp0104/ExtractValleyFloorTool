namespace ValleyBottomLineAddin
{
    partial class GetValleyBottomLineWinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetValleyBottomLineWinForm));
            this.labelInputshapeFile = new System.Windows.Forms.Label();
          
            this.buttonSelectshapeFile = new System.Windows.Forms.Button();
            this.labelFittingThresold = new System.Windows.Forms.Label();
            this.textBoxFittingThresold = new System.Windows.Forms.TextBox();
            this.labelSlopeThreshod = new System.Windows.Forms.Label();
            this.textBoxSlopeThreshod = new System.Windows.Forms.TextBox();
            this.labelInputDemFile = new System.Windows.Forms.Label();
            this.buttonSelectDemFile = new System.Windows.Forms.Button();
            this.labelExploringDistance = new System.Windows.Forms.Label();
            this.textBoxExploringDistance = new System.Windows.Forms.TextBox();
            this.textBoxSelectResultLocation = new System.Windows.Forms.TextBox();
            this.labelSelectResultLocation = new System.Windows.Forms.Label();
            this.buttonselectResultLocation = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxSelectShapeFile = new System.Windows.Forms.TextBox();
            this.textBoxSelectDEMFile = new System.Windows.Forms.TextBox();
            this.labelSegmentDistance = new System.Windows.Forms.Label();
            this.textBoxRiverSegmentDistance = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.imageListLayerIcon = new System.Windows.Forms.ImageList(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelInputshapeFile
            // 
            this.labelInputshapeFile.AutoSize = true;
            this.labelInputshapeFile.BackColor = System.Drawing.Color.Transparent;
            this.labelInputshapeFile.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInputshapeFile.Location = new System.Drawing.Point(32, 19);
            this.labelInputshapeFile.Name = "labelInputshapeFile";
            this.labelInputshapeFile.Size = new System.Drawing.Size(155, 15);
            this.labelInputshapeFile.TabIndex = 4;
            this.labelInputshapeFile.Text = "Input river polyline features";
            // 
            // ovalShape1
            // 
        
            // 
            // shapeContainer1
            // 
          
            // 
            // ovalShape6
            // 
        
            // 
            // ovalShape5
            // 
           
            // 
            // ovalShape4
            // 
           
            // 
            // ovalShape3
            // 
      
            // 
            // ovalShape2
            // 
           
            // 
            // buttonSelectshapeFile
            // 
            this.buttonSelectshapeFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonSelectshapeFile.Image")));
            this.buttonSelectshapeFile.Location = new System.Drawing.Point(411, 46);
            this.buttonSelectshapeFile.Name = "buttonSelectshapeFile";
            this.buttonSelectshapeFile.Size = new System.Drawing.Size(27, 27);
            this.buttonSelectshapeFile.TabIndex = 15;
            this.buttonSelectshapeFile.UseVisualStyleBackColor = true;
            this.buttonSelectshapeFile.Click += new System.EventHandler(this.buttonSelectshapeFile_Click);
            // 
            // labelFittingThresold
            // 
            this.labelFittingThresold.AutoSize = true;
            this.labelFittingThresold.BackColor = System.Drawing.Color.Transparent;
            this.labelFittingThresold.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFittingThresold.Location = new System.Drawing.Point(32, 149);
            this.labelFittingThresold.Name = "labelFittingThresold";
            this.labelFittingThresold.Size = new System.Drawing.Size(268, 15);
            this.labelFittingThresold.TabIndex = 16;
            this.labelFittingThresold.Text = "Input reach fitting angle threshold (Units:degree)";
            // 
            // textBoxFittingThresold
            // 
            this.textBoxFittingThresold.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFittingThresold.Location = new System.Drawing.Point(33, 174);
            this.textBoxFittingThresold.Name = "textBoxFittingThresold";
            this.textBoxFittingThresold.Size = new System.Drawing.Size(403, 25);
            this.textBoxFittingThresold.TabIndex = 17;
            // 
            // labelSlopeThreshod
            // 
            this.labelSlopeThreshod.AutoSize = true;
            this.labelSlopeThreshod.BackColor = System.Drawing.Color.Transparent;
            this.labelSlopeThreshod.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSlopeThreshod.Location = new System.Drawing.Point(30, 276);
            this.labelSlopeThreshod.Name = "labelSlopeThreshod";
            this.labelSlopeThreshod.Size = new System.Drawing.Size(366, 15);
            this.labelSlopeThreshod.TabIndex = 18;
            this.labelSlopeThreshod.Text = "Input the slope threshold on both sides of the valley (Units:degree)";
            // 
            // textBoxSlopeThreshod
            // 
            this.textBoxSlopeThreshod.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSlopeThreshod.Location = new System.Drawing.Point(33, 303);
            this.textBoxSlopeThreshod.Name = "textBoxSlopeThreshod";
            this.textBoxSlopeThreshod.Size = new System.Drawing.Size(403, 25);
            this.textBoxSlopeThreshod.TabIndex = 19;
            // 
            // labelInputDemFile
            // 
            this.labelInputDemFile.AutoSize = true;
            this.labelInputDemFile.BackColor = System.Drawing.Color.Transparent;
            this.labelInputDemFile.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInputDemFile.Location = new System.Drawing.Point(30, 85);
            this.labelInputDemFile.Name = "labelInputDemFile";
            this.labelInputDemFile.Size = new System.Drawing.Size(99, 15);
            this.labelInputDemFile.TabIndex = 21;
            this.labelInputDemFile.Text = "Input basin DEM";
            // 
            // buttonSelectDemFile
            // 
            this.buttonSelectDemFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonSelectDemFile.Image")));
            this.buttonSelectDemFile.Location = new System.Drawing.Point(411, 110);
            this.buttonSelectDemFile.Name = "buttonSelectDemFile";
            this.buttonSelectDemFile.Size = new System.Drawing.Size(27, 27);
            this.buttonSelectDemFile.TabIndex = 22;
            this.buttonSelectDemFile.UseVisualStyleBackColor = true;
            this.buttonSelectDemFile.Click += new System.EventHandler(this.buttonSelectDemFile_Click);
            // 
            // labelExploringDistance
            // 
            this.labelExploringDistance.AutoSize = true;
            this.labelExploringDistance.BackColor = System.Drawing.Color.Transparent;
            this.labelExploringDistance.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExploringDistance.Location = new System.Drawing.Point(30, 341);
            this.labelExploringDistance.Name = "labelExploringDistance";
            this.labelExploringDistance.Size = new System.Drawing.Size(317, 15);
            this.labelExploringDistance.TabIndex = 23;
            this.labelExploringDistance.Text = "Input the distance that the river explores along the vertical";
            // 
            // textBoxExploringDistance
            // 
            this.textBoxExploringDistance.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxExploringDistance.Location = new System.Drawing.Point(33, 367);
            this.textBoxExploringDistance.Name = "textBoxExploringDistance";
            this.textBoxExploringDistance.Size = new System.Drawing.Size(403, 25);
            this.textBoxExploringDistance.TabIndex = 24;
            // 
            // textBoxSelectResultLocation
            // 
            this.textBoxSelectResultLocation.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSelectResultLocation.Location = new System.Drawing.Point(33, 435);
            this.textBoxSelectResultLocation.Name = "textBoxSelectResultLocation";
            this.textBoxSelectResultLocation.Size = new System.Drawing.Size(370, 25);
            this.textBoxSelectResultLocation.TabIndex = 25;
            // 
            // labelSelectResultLocation
            // 
            this.labelSelectResultLocation.AutoSize = true;
            this.labelSelectResultLocation.BackColor = System.Drawing.Color.Transparent;
            this.labelSelectResultLocation.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectResultLocation.Location = new System.Drawing.Point(30, 408);
            this.labelSelectResultLocation.Name = "labelSelectResultLocation";
            this.labelSelectResultLocation.Size = new System.Drawing.Size(224, 15);
            this.labelSelectResultLocation.TabIndex = 26;
            this.labelSelectResultLocation.Text = "Output outlines and width of valley floor";
            // 
            // buttonselectResultLocation
            // 
            this.buttonselectResultLocation.Image = ((System.Drawing.Image)(resources.GetObject("buttonselectResultLocation.Image")));
            this.buttonselectResultLocation.Location = new System.Drawing.Point(409, 433);
            this.buttonselectResultLocation.Name = "buttonselectResultLocation";
            this.buttonselectResultLocation.Size = new System.Drawing.Size(27, 27);
            this.buttonselectResultLocation.TabIndex = 27;
            this.buttonselectResultLocation.UseVisualStyleBackColor = true;
            this.buttonselectResultLocation.Click += new System.EventHandler(this.buttonselectResultLocation_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.Location = new System.Drawing.Point(255, 552);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 28;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(347, 552);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 29;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.textBoxSelectShapeFile);
            this.panel1.Controls.Add(this.textBoxSelectDEMFile);
            this.panel1.Controls.Add(this.textBoxFittingThresold);
            this.panel1.Controls.Add(this.labelSegmentDistance);
            this.panel1.Controls.Add(this.textBoxRiverSegmentDistance);
            this.panel1.Controls.Add(this.labelSlopeThreshod);
            this.panel1.Controls.Add(this.textBoxSlopeThreshod);
            this.panel1.Controls.Add(this.labelExploringDistance);
            this.panel1.Controls.Add(this.textBoxExploringDistance);
            this.panel1.Controls.Add(this.labelSelectResultLocation);
            this.panel1.Controls.Add(this.buttonselectResultLocation);
            this.panel1.Controls.Add(this.textBoxSelectResultLocation);
            this.panel1.Controls.Add(this.labelInputDemFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 525);
            this.panel1.TabIndex = 30;
            // 
            // textBoxSelectShapeFile
            // 
            this.textBoxSelectShapeFile.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSelectShapeFile.Location = new System.Drawing.Point(33, 44);
            this.textBoxSelectShapeFile.Name = "textBoxSelectShapeFile";
            this.textBoxSelectShapeFile.Size = new System.Drawing.Size(373, 25);
            this.textBoxSelectShapeFile.TabIndex = 31;
            // 
            // textBoxSelectDEMFile
            // 
            this.textBoxSelectDEMFile.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSelectDEMFile.Location = new System.Drawing.Point(33, 108);
            this.textBoxSelectDEMFile.Name = "textBoxSelectDEMFile";
            this.textBoxSelectDEMFile.Size = new System.Drawing.Size(373, 25);
            this.textBoxSelectDEMFile.TabIndex = 30;
            // 
            // labelSegmentDistance
            // 
            this.labelSegmentDistance.AutoSize = true;
            this.labelSegmentDistance.BackColor = System.Drawing.Color.Transparent;
            this.labelSegmentDistance.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSegmentDistance.Location = new System.Drawing.Point(30, 209);
            this.labelSegmentDistance.Name = "labelSegmentDistance";
            this.labelSegmentDistance.Size = new System.Drawing.Size(179, 15);
            this.labelSegmentDistance.TabIndex = 29;
            this.labelSegmentDistance.Text = "Input the river segment distance";
            // 
            // textBoxRiverSegmentDistance
            // 
            this.textBoxRiverSegmentDistance.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxRiverSegmentDistance.Location = new System.Drawing.Point(33, 236);
            this.textBoxRiverSegmentDistance.Name = "textBoxRiverSegmentDistance";
            this.textBoxRiverSegmentDistance.Size = new System.Drawing.Size(403, 25);
            this.textBoxRiverSegmentDistance.TabIndex = 28;
            this.textBoxRiverSegmentDistance.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(450, 594);
            this.panel2.TabIndex = 31;
            // 
            // imageListLayerIcon
            // 
            this.imageListLayerIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLayerIcon.ImageStream")));
            this.imageListLayerIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListLayerIcon.Images.SetKeyName(0, "diamond_orange_24px.png");
            // 
            // GetValleyBottomLineWinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.ClientSize = new System.Drawing.Size(450, 594);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonSelectDemFile);
            this.Controls.Add(this.labelFittingThresold);
            this.Controls.Add(this.buttonSelectshapeFile);
            this.Controls.Add(this.labelInputshapeFile);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
          
            this.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GetValleyBottomLineWinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Extract Valley Floor Outline";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.GetValleyBottomLineWinForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelInputshapeFile;
   
        private System.Windows.Forms.Button buttonSelectshapeFile;
        private System.Windows.Forms.Label labelFittingThresold;
        private System.Windows.Forms.TextBox textBoxFittingThresold;
        private System.Windows.Forms.Label labelSlopeThreshod;
        private System.Windows.Forms.TextBox textBoxSlopeThreshod;
        private System.Windows.Forms.Label labelInputDemFile;
        private System.Windows.Forms.Button buttonSelectDemFile;
       
        private System.Windows.Forms.Label labelExploringDistance;
        private System.Windows.Forms.TextBox textBoxExploringDistance;
        private System.Windows.Forms.TextBox textBoxSelectResultLocation;
        private System.Windows.Forms.Label labelSelectResultLocation;
        private System.Windows.Forms.Button buttonselectResultLocation;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ImageList imageListLayerIcon;
        private System.Windows.Forms.Label labelSegmentDistance;
        private System.Windows.Forms.TextBox textBoxRiverSegmentDistance;
        private System.Windows.Forms.TextBox textBoxSelectShapeFile;
        private System.Windows.Forms.TextBox textBoxSelectDEMFile;
    }
}