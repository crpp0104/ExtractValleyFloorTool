using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ValleyBottomLineAddin
{
    public class GetValleyBottomLineButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        private GetValleyBottomLineWinForm getValleyBottomLineForm;

        public GetValleyBottomLineButton()
        {
            try
            {

            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            
            }
        }

        protected override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //
            try
            {
                ArcMap.Application.CurrentTool = null;
                getValleyBottomLineForm = new GetValleyBottomLineWinForm();
                getValleyBottomLineForm.Show();
            }
            catch(Exception ex)
            {
               
                MessageBox.Show(ex.Message);
            
            }
        }
     
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}
