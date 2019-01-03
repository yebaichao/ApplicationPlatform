using ApplicationPlatform.Site.Utilities;
using ApplicationPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EmailSend
{
    public partial class EmailSend : Form
    {
        private System.Timers.Timer timer { get; set; }
        private static string TimeLine{get;set;}
        public EmailSend()
        {
            InitializeComponent();
        }

        private void EmailSend_Load(object sender, EventArgs e)
        {
            timer = new System.Timers.Timer();
            timer.AutoReset = true;
            timer.Enabled = true;
            double timeInerval = 10 * 1000;
            timer.Interval = timeInerval;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }
        private static int inTimer = 0; 
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Interlocked.Exchange(ref inTimer, 1) == 0)
            {
                string tempTime = System.DateTime.Now.ToString("yyyy-MM-dd");
                if (tempTime != TimeLine)
                {
                    try
                    {
                        XMLHelp xmlHelp = new XMLHelp();
                        var emails = xmlHelp.GetEmailDeliveryVariables();
                        string path = System.Environment.CurrentDirectory;
                        path = Directory.GetParent(path) + "\\EmailTemplate.xml";
                        EmailTools.SendEmailUsers("All Requirements", emails, path);
                        //执行一次后赋予TimeLine初始值
                        TimeLine = System.DateTime.Now.ToString("yyyy-MM-dd");
                    }catch (Exception ex)
                    { ExceptionLogHelp.WriteLog(ex); }
                }
                Interlocked.Exchange(ref inTimer, 0);
            }
        }


        private void EmailSend_SizeChanged(object sender, EventArgs e)
        {
            //判断是否选择的是最小化按钮
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void EmailSend_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Exit application？", "Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                timer.Stop();
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            } 
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
