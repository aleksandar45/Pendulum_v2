using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PendulumApp.ViewModel.OpenGLRender
{
    class OpenGLDispatcher
    {
        public const int EMG_BUFFER_LENGTH = 2000;
        public const int ACC_BUFFER_LENGTH = 400;
        public const int GY_BUFFER_LENGTH = 400;

        OpenGlDisplay openGlDisplay1;
        OpenGlDisplay openGlDisplay2;
        OpenGlDisplay openGlDisplay3;
        OpenGlDisplay openGlDisplay4;
        OpenGlDisplay openGlDisplay5;
        OpenGlDisplay openGlDisplay6;

        public List<IntArray> emgChannels;
        private int _emgIndex = 0;
        public int emgIndex
        {
            get
            {
                return _emgIndex;
            }
            set
            {
                _emgIndex = value;

                if (emgChannels != null)
                {
                    if (_emgIndex >= emgChannels.ElementAt(0).size)
                        _emgIndex = 0;
                    for (int i = 0; i < emgChannels.Count; i++)
                    {
                        emgChannels.ElementAt(i).index = _emgIndex;
                    }
                }
                
            }
        }

        public List<IntArray> accChannels;
        private int _accIndex = 0;
        public int accIndex
        {
            get
            {
                return _accIndex;
            }
            set
            {
                _accIndex = value;

                if (accChannels != null)
                {
                    if (_accIndex >= accChannels.ElementAt(0).size)
                        _accIndex = 0;
                    for (int i = 0; i < accChannels.Count; i++)
                    {
                        accChannels.ElementAt(i).index = _accIndex;
                    }
                }
                
            }
        }

        
        public List<IntArray> gyChannels;
        private int _gyIndex = 0;
        public int gyIndex
        {
            get
            {
                return _gyIndex;
            }
            set
            {
                _gyIndex = value;
                if (gyChannels != null)
                {
                    if (_gyIndex >= gyChannels.ElementAt(0).size)
                        _gyIndex = 0;
                    for (int i = 0; i < gyChannels.Count; i++)
                    {
                        gyChannels.ElementAt(i).index = _gyIndex;
                    }
                }
            }
        }


        public OpenGLDispatcher(OpenGlDisplay display1, OpenGlDisplay display2, OpenGlDisplay display3, OpenGlDisplay display4, OpenGlDisplay display5, OpenGlDisplay display6, int emgNoCh, int accNoCh, int gyNoCh)
        {            
            openGlDisplay1 = display1;
            openGlDisplay2 = display2;
            openGlDisplay3 = display3;
            openGlDisplay4 = display4;
            openGlDisplay5 = display5;
            openGlDisplay6 = display6;
            emgChannels = new List<IntArray>(emgNoCh);
            for (int i = 0; i < emgNoCh; i++)
                emgChannels.Add(new IntArray(EMG_BUFFER_LENGTH));

            accChannels = new List<IntArray>(accNoCh);
            for (int i = 0; i < accNoCh; i++)
                accChannels.Add(new IntArray(ACC_BUFFER_LENGTH));

            gyChannels = new List<IntArray>(gyNoCh);
            for (int i = 0; i < gyNoCh; i++)
                gyChannels.Add(new IntArray(GY_BUFFER_LENGTH));            

        }
        public void renderDisplays()
        {
         
            openGlDisplay1.render();
            openGlDisplay2.render();
            openGlDisplay3.render();
            openGlDisplay4.render();
            openGlDisplay5.render();
            openGlDisplay6.render();         

        }
        public void disableDisplay(int i)
        {
            switch (i)
            {
                case 1:
                    openGlDisplay1.Visible = false;
                    break;
                case 2:
                    openGlDisplay2.Visible = false;
                    break;
                case 3:
                    openGlDisplay3.Visible = false;
                    break;
                case 4:
                    openGlDisplay4.Visible = false;
                    break;
                case 5:
                    openGlDisplay5.Visible = false;
                    break;
                case 6:
                    openGlDisplay6.Visible = false;
                    break;
            }
        }
        public void enableDisplay(int i)
        {
            switch (i)
            {
                case 1:
                    openGlDisplay1.Visible = true;
                    break;
                case 2:
                    openGlDisplay2.Visible = true;
                    break;
                case 3:
                    openGlDisplay3.Visible = true;
                    break;
                case 4:
                    openGlDisplay4.Visible = true;
                    break;
                case 5:
                    openGlDisplay5.Visible = true;
                    break;
                case 6:
                    openGlDisplay6.Visible = true;
                    break;
            }
        }
        public bool linkDisplay(int displayNum, string type, int ch_no)
        {
            OpenGlDisplay tempDisplay;
            switch (displayNum)
            {
                case 1:
                    tempDisplay = openGlDisplay1;
                    break;
                case 2:
                    tempDisplay = openGlDisplay2;
                    break;
                case 3:
                    tempDisplay = openGlDisplay3;
                    break;
                case 4:
                    tempDisplay = openGlDisplay4;
                    break;
                case 5:
                    tempDisplay = openGlDisplay5;
                    break;
                case 6:
                    tempDisplay = openGlDisplay6;
                    break;
                default:
                    tempDisplay = openGlDisplay1;
                    break;
            }
            if (String.Compare(type, "EMG") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(emgChannels.ElementAt(ch_no - 1));
                    return true;
                }
                else return false;
            }
            if (String.Compare(type, "ACC") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(accChannels.ElementAt(ch_no - 1));                    
                }
            }
            if (String.Compare(type, "GY") == 0)
            {
                if (tempDisplay != null)
                {
                    tempDisplay.linkArray(gyChannels.ElementAt(ch_no - 1));                    
                }
            }            
            return false;
        }        
        public void dispose()
        {

        }
    }
}
