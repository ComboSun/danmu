using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.Timers;
using System.Collections;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CZDanmu
{
    public partial class DanmuMain : Form
    {
        private Thread thread;     
        int[] c = { 50, 100, 150, 200, 250 };//高度

        public DanmuMain()
        {
            this.TransparencyKey = Color.White;
            this.BackColor = Color.White;
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public void main()
        {
            System.Timers.Timer btimer = new System.Timers.Timer();
            btimer.Interval = 1500;//1500ms查询一次数据库
            btimer.Enabled = true;
            btimer.Elapsed += new ElapsedEventHandler(delegate(object source, System.Timers.ElapsedEventArgs e)
            {
                DanmuUtil.arr = null;
                string i;//定义已查询id
                SqlConnection conn = new SqlConnection("server=192.168.1.6;database=OA_Site;uid=sa;pwd=flybarrier");
                string selsql = string.Format(@"SELECT TOP 1 w.id fid,w.creattime,w.canceltype,w.content premessage FROM wx_uesrmessage_fhf w
                                             inner join HrmResource h on w.creatuser=h.loginid
                                             where canceltype='0'order by w.id asc ");

                SqlDataAdapter sda = new SqlDataAdapter(selsql, conn);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)//如果查出的数据不为空
                {
                    DanmuUtil.arr = dt.Rows[0]["premessage"].ToString();//获取弹幕内容
                    i = dt.Rows[0]["fid"].ToString();                   //获取弹幕id
                    string upsql = string.Format(@"update wx_uesrmessage_fhf set canceltype='1' where id='{0}'", i);    //标志位置“1”
                    SqlDataAdapter sda1 = new SqlDataAdapter(upsql, conn);
                    DataTable dt1 = new DataTable();
                    sda1.Fill(dt1);
                    conn.Close();
                    conn.Dispose();
                    if (DanmuUtil.arr.Length < 21&&DanmuUtil.arr.Length!=0)
                    {
                        thread = new Thread(new ThreadStart(makelbl));
                        try
                        {
                            thread.Start();
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    }
                    else { DanmuUtil.arr = null; }
                }
                //否则置空
                else { DanmuUtil.arr = null;  }
            });
            btimer.Start();
        }

        public void makelbl()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { makelbl(); }));
                return;
            }

            Label lbl = new Label();
           
            this.Controls.Add(lbl);

            lbl.Text = DanmuUtil.arr;
            
            int h = GetRandomScreenHeight(c);
            if (h != DanmuUtil.height)
            {
                DanmuUtil.height = h;
                lbl.Location = new Point(this.Width, h);
            }
            else if (c[0] == DanmuUtil.height)
            {
                int j = GetRandomScreenheight0(c, 0);
                lbl.Location = new Point(this.Width, j);
                DanmuUtil.height = j;
            }
            else if (c[1] == DanmuUtil.height)
            {
                int j = GetRandomScreenheight0(c, 1);
                lbl.Location = new Point(this.Width, j);
                DanmuUtil.height = j;
            }
            else if (c[2] == DanmuUtil.height)
            {
                int j = GetRandomScreenheight0(c, 2);
                lbl.Location = new Point(this.Width, j);
                DanmuUtil.height = j;
            }
            else if (c[3] == DanmuUtil.height)
            {
                int j = GetRandomScreenheight0(c, 3);
                lbl.Location = new Point(this.Width, j);
                DanmuUtil.height = j;
            }
            else if (c[4] == DanmuUtil.height)
            {
                int j = GetRandomScreenheight0(c, 4);
                lbl.Location = new Point(this.Width, j);
                DanmuUtil.height = j;
            }
            
            lbl.Font = new Font("宋体", 19, FontStyle.Bold);
            
            lbl.Size = new System.Drawing.Size(31 * lbl.Text.Length, 30);
            
            lbl.ForeColor =  Color.FromArgb(new Random().Next(0, 255 * 255 * 255));

            System.Timers.Timer atimer = new System.Timers.Timer();
            atimer.Enabled = true;
            atimer.Interval = 40;
            atimer.Elapsed += new ElapsedEventHandler(delegate(object source, System.Timers.ElapsedEventArgs e)
            {
                lbl.Left -= 10;
                if (lbl.Right < 0)
                {
                    thread.Abort();
                    atimer.Stop();
                }
            });
            atimer.Start();
        }

        public static Color GetRandomColor(Color[] col)
        {
            Random rnd = new Random();
            int index = rnd.Next(col.Length);
            return col[index];
        }

        //随机选取字体
        public static string GetRandomFontStyle(string[] b)
        {
            Random rnd = new Random();
            int index = rnd.Next(b.Length);
            return b[index];
        }

        //随机获取屏幕位置
        public static int GetRandomScreenHeight(int[] c)
        {
            Random rnd = new Random();
            int index = rnd.Next(c.Length);
            return c[index];
        }

        public static int GetRandomScreenheight0(int[] c, int i)
        {
            ArrayList al = new ArrayList(c);
            al.RemoveAt(i);
            c = (int[])al.ToArray(typeof(int));
            Random rnd = new Random();
            int index = rnd.Next(c.Length);
            return c[index];
        }

        //写日志；
        public static void WriteLog(string strLog)
        {
            string LogAddress = "";
            string path = Environment.CurrentDirectory.ToString() + '\\' + "CZdanmu_Log";
            //如果目录下无 CZsp_log 的文件夹，那么新建此文件夹
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //如果日志文件为空，则默认在Debug目录下新建 YYYY-mm-dd_Log.log文件
            if (LogAddress == "")
            {
                LogAddress = path + '\\' +
                DateTime.Now.Year + '-' +
                DateTime.Now.Month + '-' +
                DateTime.Now.Day + "_Log.log";
            }
            StreamWriter fs = new StreamWriter(LogAddress, true);
            fs.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "   ---   " + strLog);
            fs.Close();
        }

        private void 显示ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Visible = true;
            //全屏显示无边框
            this.SetVisibleCore(false);
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.SetVisibleCore(true);
        }

        private void 开启弹幕ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Visible = true;
            main();
        }

        private void 关闭弹幕ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void 退出ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
