using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoSliceFinder
{
    public partial class FormVideo : Form
    {
        public FormVideo()
        {
            InitializeComponent();
        }

        int currIndex = 0;
        int countOfVideo = 0;
        int timeLen = 12;
        Timer tmr = null;

        void PlayNextVideo()
        {
            if (currIndex < countOfVideo)
            {
                mediaPlayer.URL = FormMain.instance.GetPathToFilm(currIndex);
                mediaPlayer.Ctlcontrols.currentPosition = FormMain.instance.GetTimeMoment(currIndex) - 4;
                mediaPlayer.Ctlcontrols.play();

                if (tmr != null)
                {
                    tmr.Stop();
                    tmr.Dispose();
                }

                tmr = new Timer();
                tmr.Tick += new EventHandler(TimerEventProcessor);
                tmr.Interval = (timeLen) * 1000;
                tmr.Start();
            }
            else
            {
                if (tmr != null)
                    tmr.Stop();
            }
        }

        private void FormVideo_Load(object sender, EventArgs e)
        {
            countOfVideo = FormMain.instance.GetCountOfResults();

            mediaPlayer.uiMode = "mini";
            PlayNextVideo();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            mediaPlayer.Ctlcontrols.stop();

            currIndex++;
            PlayNextVideo();
        }

        private void FormVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tmr != null)
            {
                tmr.Stop();
                tmr.Dispose();
            }
        }
    }
}
