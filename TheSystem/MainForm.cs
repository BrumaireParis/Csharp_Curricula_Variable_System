using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Test
{
    public partial class MainForm : Form
    {
        int PanelWidth;
        bool isCollapsed;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;//用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }
        public MainForm()
        {
            InitializeComponent();
            PanelWidth = 0;//panel1.Left;
            isCollapsed = true;
        }

        bool formMove = false;//窗体是否移动
        Point formPoint;//记录窗体的位置
        private void MainForm_MouseDown(object sender, MouseEventArgs e)//鼠标按下
        {
            formPoint = new Point();
            int xOffset;
            int yOffset;
            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height;
                formPoint = new Point(xOffset, yOffset);
                formMove = true;//开始移动
            }
        }
        private void MainForm_MouseMove(object sender, MouseEventArgs e)//鼠标移动
        {
            if (formMove == true)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(formPoint.X, formPoint.Y);
                Location = mousePos;
            }
        }
        private void MainForm_MouseUp(object sender, MouseEventArgs e)//鼠标松开
        {
            if (e.Button == MouseButtons.Left)//按下的是鼠标左键
            {
                formMove = false;//停止移动
            }
        }
        protected override void OnResize(EventArgs e)
        {
            this.Region = null;
            SetWindowR();

        }

        private void SetWindowR()
        {
            System.Drawing.Drawing2D.GraphicsPath 
            gPath = new System.Drawing.Drawing2D.GraphicsPath();
            Rectangle rect = new Rectangle(0, 5, this.Width, this.Height - 5);
            gPath = GetRoundedRP(rect, 30); //后面的30是圆的角度，数值越大圆角度越大
            this.Region = new Region(gPath);
        }

        private System.Drawing.Drawing2D.GraphicsPath GetRoundedRP(Rectangle rect, int a)
        {
            int diameter = a;
            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddArc(arcRect, 180, 90);
            arcRect.X = rect.Right - diameter;
            gp.AddArc(arcRect, 270, 90);
            arcRect.Y = rect.Bottom - diameter;
            gp.AddArc(arcRect, 0, 90);
            arcRect.X = rect.Left;
            gp.AddArc(arcRect, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            timer1.Start();
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            SoundPlayer play = new SoundPlayer(Test.Properties.Resources.GF_Lobby); 
            play.PlayLooping();
            try
            {
                string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
                SqlConnection connection = new SqlConnection(connString);
                string sql = "SELECT Photo FROM Account WHERE Accounts='"+Program.loginName+"'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader read = command.ExecuteReader();

                while (read.Read())
                    Program.photoPath = read["Photo"].ToString();
                read.Close();
                connection.Close();
                //判断是否为空，为空时的不执行
                if (Program.photoPath != null)
                {
                    // 将图片放置在 PictureBox 中
                    this.pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pictureBox10.BackgroundImage = Image.FromFile(@Program.photoPath);
                }
            }
            catch (Exception)
            { 
                //MessageBox.Show("未能加载到照片，请尽快提交！");
                pictureBox10.BackgroundImage = Test.Properties.Resources._404logo;
            }
            if(Program.LoginType.Trim()=="学生")
            {
                try
                {
                    string conString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
                    SqlConnection myconnection = new SqlConnection(conString);
                    string sql2 = "SELECT * FROM Student WHERE ID='" + Program.loginID + "'";
                    SqlCommand sqlCommand = new SqlCommand(sql2, myconnection);
                    myconnection.Open();
                    SqlDataReader read1 = sqlCommand.ExecuteReader();

                    while (read1.Read())
                    {
                        Program.realName = read1["Sname"].ToString();
                        label11.Text = read1["Sage"].ToString();
                        label16.Text = read1["Ssex"].ToString();
                        label14.Text = read1["Sdept"].ToString();
                        label20.Text = read1["Sclass"].ToString();
                        label18.Text = read1["Stel"].ToString();
                    }
                    read1.Close();
                    myconnection.Close();
                    button3.Enabled = false;
                    button4.Enabled = false;
                    button5.Enabled = false;
                }
                catch (Exception) 
                {
                    MessageBox.Show("无法初始化学生类账户！");
                }
            }
            else if (Program.LoginType.Trim() == "教师")
            {
                try
                {
                    string cconString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
                    SqlConnection cconnection = new SqlConnection(cconString);
                    string sql2 = "SELECT * FROM Teacher WHERE ID='" + Program.loginID + "'";
                    SqlCommand sqlCCommand = new SqlCommand(sql2, cconnection);
                    cconnection.Open();
                    SqlDataReader read2 = sqlCCommand.ExecuteReader();

                    while (read2.Read())
                    {
                        Program.realName = read2["Tname"].ToString();
                        label11.Text = read2["Tage"].ToString();
                        label16.Text = read2["Tsex"].ToString();
                        label14.Text = read2["Tdept"].ToString();
                        label20.Text = read2["Tlev"].ToString();
                        label18.Text = read2["Ttel"].ToString();
                    }
                    read2.Close();
                    cconnection.Close();
                    label19.Text = "职称：";
                    button3.Enabled = false;
                    button4.Enabled = false;
                }
                catch (Exception) { MessageBox.Show("无法初始化教师类账户！"); }
            }
            label3.Text = Program.loginName;
            labelLG.Text = Program.loginName;
            label7.Text = Program.realName;
            label9.Text = Program.loginID;
            label21.Text = Program.LoginType;
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            Random ran = new Random();
            int r = ran.Next(0, 5);
            pictureBox6.Visible=false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
            if (r == 1)
                pictureBox6.Visible = true;
            if (r == 2)
                pictureBox7.Visible = true;
            if (r == 3)
                pictureBox8.Visible = true;
            if (r == 4)
                pictureBox9.Visible = true;
            timer2.Start();

        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            //timer2.Stop();
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;
            pictureBox9.Visible = false;
        }

        private void PictureBox4_Click(object sender, EventArgs e)
        {
            this.Dispose();
            LoginForm lg = new LoginForm();
            lg.Show();
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                panel1.Left = panel1.Left + 50;
                if (panel1.Left >= PanelWidth)
                {
                    timer3.Stop();
                    isCollapsed = false;
                    this.Refresh();
                }
            }
            else
            {
                panel1.Left = panel1.Left - 50;
                if (panel1.Left <= -500)
                {
                    timer3.Stop();
                    isCollapsed = true;
                    this.Refresh();
                }
            }
        }

        private void PictureBox5_Click(object sender, EventArgs e)
        {
            timer3.Start();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Title = "请选择要上传的图片";
            openfile.Filter = "图片(*.jpg;*.bmp;*png)|*.jpeg;*.jpg;*.bmp;*.png|AllFiles(*.*)|*.*";
            if (DialogResult.OK == openfile.ShowDialog())
            {
                try
                {
                    string picturePath = openfile.FileName;
                    string picName = openfile.SafeFileName;
                    string pSaveFilePath = "D:\\LocalPic\\";//指定存储的路径
                    if (!System.IO.Directory.Exists(@"D:\LocalPic"))
                    {
                        System.IO.Directory.CreateDirectory(@"D:\LocalPic");//不存在就创建目录
                    }
                    string picSave = pSaveFilePath + picName;
                    Program.photoPath = picSave;
                    //MessageBox.Show(Program.photoPath);
                    if (File.Exists(picturePath))//必须判断要复制的文件是否存在
                            File.Copy(picturePath, picSave, true);
                    
                    string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456"; 
                    SqlConnection connection = new SqlConnection(connString);
                    string sql = "Update Account set Photo = '"+picSave+"'WHERE Accounts = '"+Program.loginName+"'";
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    this.pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pictureBox10.BackgroundImage = Image.FromFile(@Program.photoPath);
                    MessageBox.Show("上传成功！");
                }
                catch(Exception) { MessageBox.Show("上传图片出现错误！"); }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Pinfo pinfo = new Pinfo();
            pinfo.ShowDialog();
            MainForm_Load(sender,e);
            pinfo.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel4.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel3.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Update与Delete已整合在一起。");
            Admin admin = new Admin();
            admin.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Update与Delete已整合在一起。");
            Admin admin = new Admin();
            admin.Show();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel5.Visible = true;
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "EXEC Proc_TRANSFER1 " +
                             "SELECT* FROM SC_Avg";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView3.DataSource = ds.Tables[0];
                dataGridView3.ReadOnly = true;
                dataGridView3.AllowUserToAddRows = false;
                dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView3.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "今の時間:" + System.DateTime.Now.ToString();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity >= 0.025)
                this.Opacity -= 0.025;
            else
            {
                timer.Stop();
                Application.Exit();
            }
        }

        private bool xscx;
        private bool cxcj;

        private void Panel3_Load(object sender,EventArgs e)
        {
            label22.Visible = false;
            textBox1.Visible = false;
            button11.Visible = false;
            label23.Visible = false;
            if (Program.LoginType.Trim()=="教师")
            {
                button8.BackgroundImage = Test.Properties.Resources.skcx;
                button9.BackgroundImage = Test.Properties.Resources.xscx;
                pictureBox11.BackgroundImage = Test.Properties.Resources.txgz;
                pictureBox12.BackgroundImage = Test.Properties.Resources.lyco;
                xscx = false;
            }
            else
            {
                cxcj = false;
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            dataGridView1.Visible = false;
            label22.Visible = false;
            label23.Visible = false;
            textBox1.Visible = false;
            button11.Visible = false;
        }

        //查询全部课程
        private void StuCourse()
        {
            //数据库连接串
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            //创建SqlConnection的实例
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                //打开数据库
                conn.Open();
                string sql = "SELECT TRIM(Sname) as 姓名, TRIM(Cno) as 课程号, TRIM(Cname) as 课程名,Ccredit as 学分, Trim(Tname) as 任课老师 FROM Stuc WHERE Sname='" + Program.realName+"'";
                //创建SqlDataAdapter类的对象
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                //创建DataSet类的对象
                DataSet ds = new DataSet();
                //使用SqlDataAdapter对象sda将查新结果填充到DataSet对象ds中
                da.Fill(ds,"Stuc");
                //设置表格控件的DataSource属性
                dataGridView1.DataSource = ds.Tables["Stuc"];//.DefaultView;//.Tables[0];
                //设置数据表格为只读
                dataGridView1.ReadOnly = true;
                //不允许添加行
                dataGridView1.AllowUserToAddRows = false;
                //整行选中
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }

        private void StuGrade()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            //创建SqlConnection的实例
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                //打开数据库
                conn.Open();
                string sql = "SELECT Trim(Sname) as  姓名 ,Trim(SC.Cno) as 课序号 , Trim(Cname) as 课程名 ,Ccredit AS 学分,Grade as 成绩 FROM Course, SC, Student WHERE Student.ID = SC.ID AND SC.Cno = Course.Cno AND Student.ID='"+Program.loginID+"'";
                string sql1 = "SELECT Trim(SC.Cno) as 课序号 , Trim(Cname) as 课程名 ,Ccredit AS 学分,Grade as 成绩 FROM Course, SC, Student WHERE Student.ID = SC.ID AND SC.Cno = Course.Cno AND Cname like '%"+textBox1.Text.Trim()+"%' and Student.ID = '"+Program.loginID+"' ";
                //创建SqlDataAdapter类的对象
                if (textBox1.Text.Trim() != "ALL")
                {
                    SqlDataAdapter db = new SqlDataAdapter(sql1, conn);
                    DataSet dss = new DataSet();
                    db.Fill(dss);
                    dataGridView1.DataSource = dss.Tables[0];
                }
                else
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                } 
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }

        private void TeaCourse()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            //创建SqlConnection的实例
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "SELECT Trim(Tname) as  姓名 ,Trim(Course.Cno) as 课序号 , Trim(Cname) as 课程名 ,Ccredit as 学分 ,Trim(Cpno) as 先修课序号 FROM Course, Teacher WHERE Course.ID = Teacher.ID AND Teacher.ID='"+Program.loginID+"' ";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }

        private void TeacherStu()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            //创建SqlConnection的实例
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                //打开数据库
                conn.Open();
                string sql = "SELECT Trim(SC.Cno) as 课序号 , Trim(Cname) as 课程名 ,Trim(Student.ID) as 学号,trim(Sname) as 学生姓名,Grade as 成绩 FROM Teacher, Course, SC, Student WHERE Student.ID = SC.ID AND SC.Cno = Course.Cno and Course.ID = Teacher.ID And Teacher.ID='" + Program.loginID + "'order by SC.Cno ASC; ";
                string sql1 = "SELECT Trim(Student.ID) AS 学号, Trim(Sname) as 学生名,Trim(SC.Cno) as 课序号 , Trim(Cname) as 课程名,trim(Tname) as 教师名 FROM Course, SC, Student, Teacher WHERE Teacher.ID = Course.ID AND SC.Cno = Course.Cno AND SC.ID = Student.ID and Sname like '%" + textBox1.Text.Trim() + "%' and Teacher.ID = '" + Program.loginID + "'";
                if (textBox1.Text.Trim() != "ALL")
                {
                    SqlDataAdapter daa = new SqlDataAdapter(sql1, conn);
                    DataSet dss = new DataSet();
                    daa.Fill(dss);
                    dataGridView1.DataSource = dss.Tables[0];
                }
                else
                {
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    dataGridView1.DataSource = ds.Tables[0];
                }
                //设置数据表格为只读
                dataGridView1.ReadOnly = true;
                //不允许添加行
                dataGridView1.AllowUserToAddRows = false;
                //整行选中
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            label22.Visible = false;
            label23.Visible = false;
            textBox1.Visible = false;
            button11.Visible = false;
            if (Program.LoginType.Trim() == "学生")
            {
                StuCourse();
            }
            else if (Program.LoginType.Trim() == "教师")
            {
                TeaCourse();
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = true;
            label22.Visible = true;
            textBox1.Visible = true;
            label23.Visible = true;
            button11.Visible = true;
            dataGridView1.DataSource = null;
            if (Program.LoginType.Trim() == "学生")
            {
                cxcj = true;
                xscx = false;
                label22.Text = "请输入你要查询的课程名：";

            }
            if (Program.LoginType.Trim() == "教师")
            {
                xscx = true;
                cxcj = false;
                label22.Text = "请输入你要查询的学生名：";
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if(cxcj==true)
            {
                dataGridView1.DataSource = null;
                StuGrade();
            }
            else if(xscx==true)
            {
                dataGridView1.DataSource = null;
                TeacherStu();
            }
        }

        private bool studentsel;
        private bool studentdel;
        private bool teachersel;
        private bool teacherdel;
        private bool teachersel2 = false;
        private bool teacherdel2 = false;
        private string sid;
        private string ccid;
        private void Panel4_Load(object sender,EventArgs e)
        {
            label24.Visible = false;
            button15.Visible = false;
            if (Program.LoginType.Trim()=="教师")
            {
                pictureBox13.BackgroundImage = Test.Properties.Resources.NPC_Seele;
                pictureBox14.BackgroundImage = Test.Properties.Resources._404logo;
                button12.BackgroundImage = Test.Properties.Resources.jszr;
                button13.BackgroundImage = Test.Properties.Resources.jstk;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            label24.Visible = false;
            button15.Visible = false;
            dataGridView2.DataSource = null;
            dataGridView2.Visible = false;
        }
        private void StuSle()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "SELECT distinct TRIM(Course.Cno) as 课序号, TRIM(Cname) as 课程名, TRIM(Cpno) as 先修课序号,Ccredit as 学分, Trim(Tname) as 任课老师  FROM Course, Teacher, SC Where Cname not in  (SELECT Cname From SC, Course where SC.ID = '"+Program.loginID+"' and Course.Cno = SC.Cno) and Teacher.ID = Course.ID; ";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView2.DataSource = ds.Tables[0];
                dataGridView2.ReadOnly = true;
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.MultiSelect = false;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        private void StuDel()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "SELECT TRIM(Sname) as 姓名, TRIM(Cno) as 课程号, TRIM(Cname) as 课程名,Ccredit as 学分, Trim(Tname) as 任课老师 FROM Stuc WHERE Sname='" + Program.realName + "'";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "Stuc");
                dataGridView2.DataSource = ds.Tables["Stuc"];//.DefaultView;//.Tables[0];
                dataGridView2.ReadOnly = true;
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView2.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        private void TeaSel()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "SELECT TRIM(ID) as 学号, TRIM(Sname) as 姓名, TRIM(Ssex) as 性别,Sage as 年龄, Trim(Sdept) as 院系, Trim(Sclass) as 班级 FROM Student";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView2.DataSource = ds.Tables[0];
                dataGridView2.ReadOnly = true;
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView2.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }
        private void TeaDel()
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connStr);
                conn.Open();
                string sql = "SELECT Trim(Tname) as  姓名 ,Trim(Course.Cno) as 课序号 , Trim(Cname) as 课程名 ,Ccredit as 学分 ,Trim(Cpno) as 先修课序号 FROM Course, Teacher WHERE Course.ID = Teacher.ID AND Teacher.ID='" + Program.loginID + "' ";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dataGridView2.DataSource = ds.Tables[0];
                dataGridView2.ReadOnly = true;
                dataGridView2.AllowUserToAddRows = false;
                dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView2.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    //关闭数据库连接
                    conn.Close();
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            dataGridView2.Visible = true;
            label24.Visible = true;
            button15.Visible = true;
            if (Program.LoginType.Trim() == "学生")
            {
                studentsel = true;
                studentdel = false;
                teachersel = false;
                teacherdel = false;
                StuSle();
            }
            else
            {
                label24.Text = "请选择您要置入课程的学生：";
                TeaSel();
                studentsel = false;
                studentdel = false;
                teachersel = true;
                teacherdel = false;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = null;
            dataGridView2.Visible = true;
            label24.Visible = true;
            button15.Visible = true;
            if (Program.LoginType.Trim() == "学生")
            {
                studentsel = false;
                studentdel = true;
                teachersel = false;
                teacherdel = false;
                label24.Text = "请选择你要退选的课：";
                StuDel();
            }
            else
            {
                label24.Text = "请选择您要移除学生的课程：";
                TeaDel();
                studentsel = false;
                studentdel = false;
                teachersel = false;
                teacherdel = true;
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (studentsel == true)
            {
                int a = dataGridView2.CurrentRow.Index;
                string cid = dataGridView2.Rows[a].Cells[0].Value.ToString();
                DialogResult dr = MessageBox.Show("你确定要选修这门课吗？", "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";//数据库连接字符串
                        SqlConnection connection = new SqlConnection(connString);//创建connection对象
                        string sql = "insert into SC (ID,Cno,Grade) " +
                                                                "values (@uid, @uno,NULL)";
                        SqlCommand command = new SqlCommand(sql, connection);

                        SqlParameter sqlParameter = new SqlParameter("@uid", Program.loginID.Trim());
                        command.Parameters.Add(sqlParameter);
                        sqlParameter = new SqlParameter("@uno", cid);
                        command.Parameters.Add(sqlParameter);

                        //打开数据库连接
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("选课成功！");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("选课失败!");
                    }
                }
                else
                    return;
            }
            else if (studentdel == true)
            {
                int a = dataGridView2.CurrentRow.Index;
                string cid = dataGridView2.Rows[a].Cells[1].Value.ToString();
                DialogResult dr = MessageBox.Show("你确定要退选这门课吗？", "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";//数据库连接字符串
                        SqlConnection connection = new SqlConnection(connString);//创建connection对象
                        string sql = "Delete From SC Where Cno = '" + cid + "'and ID='" + Program.loginID + "'";
                        SqlCommand command = new SqlCommand(sql, connection);

                        //打开数据库连接
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("退课成功！");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("退课失败!");
                    }
                }
                else
                    return;
            }
            else if (teachersel == true && teachersel2==false)
            {
                int a = dataGridView2.CurrentRow.Index;
                sid = dataGridView2.Rows[a].Cells[0].Value.ToString();
                MessageBox.Show("选定学生成功!");
                dataGridView2.DataSource = null;
                label24.Text = "请选择您要置入的课程(仅能置入您教授的课)：";
                string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(connStr);
                    conn.Open();
                    string sql = "SELECT Trim(Tname) as  教师姓名 ,Trim(Course.Cno) as 课序号 , Trim(Cname) as 课程名 ,Ccredit as 学分 ,Trim(Cpno) as 先修课序号 FROM Course, Teacher WHERE Course.ID = Teacher.ID AND Teacher.ID='" + Program.loginID + "' ";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    dataGridView2.DataSource = ds.Tables[0];
                    dataGridView2.ReadOnly = true;
                    dataGridView2.AllowUserToAddRows = false;
                    dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView2.MultiSelect = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("查询错误！" + ex.Message);
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
                teachersel2 = true;
                return;
            }
            else if (teacherdel == true && teacherdel2 == false)
            {
                int a = dataGridView2.CurrentRow.Index;
                ccid = dataGridView2.Rows[a].Cells[1].Value.ToString();
                MessageBox.Show("选定课程成功!");
                dataGridView2.DataSource = null;
                label24.Text = "请选择您要退课的学生(仅能去除您教授的学生)：";
                string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(connStr);
                    conn.Open();
                    string sql = "SELECT TRIM(Student.ID) as 学号, TRIM(Sname) as 姓名, TRIM(Ssex) as 性别,Sage as 年龄, Trim(Sdept) as 院系, Trim(Sclass) as 班级 FROM Student,SC  Where Student.ID=SC.ID and SC.Cno='"+ccid+"'";
                    SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    dataGridView2.DataSource = ds.Tables[0];
                    dataGridView2.ReadOnly = true;
                    dataGridView2.AllowUserToAddRows = false;
                    dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView2.MultiSelect = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("查询错误！" + ex.Message);
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
                teacherdel2 = true;
                return;
            }
            else if(teachersel2==true&&teachersel==true)
            {
                int a = dataGridView2.CurrentRow.Index;
                ccid = dataGridView2.Rows[a].Cells[1].Value.ToString();
                DialogResult dr = MessageBox.Show("你确定要置入这门课吗？", "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";//数据库连接字符串
                        SqlConnection connection = new SqlConnection(connString);//创建connection对象
                        string sql = "insert into SC (ID,Cno,Grade) " +
                                                                "values (@uid, @uno,NULL)";
                        SqlCommand command = new SqlCommand(sql, connection);

                        SqlParameter sqlParameter = new SqlParameter("@uid", sid);
                        command.Parameters.Add(sqlParameter);
                        sqlParameter = new SqlParameter("@uno", ccid);
                        command.Parameters.Add(sqlParameter);

                        //打开数据库连接
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("置入成功！");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("置入失败!");
                    }
                    teachersel2 = false;
                    this.button12_Click(sender, e);

                }
                else
                {
                    label24.Text = "请选择您要置入课程的学生：";
                    this.button12_Click(sender,e);
                    teachersel2 = false;
                    return;
                }
            }
            else if(teacherdel2==true&&teacherdel==true)
            {
                int a = dataGridView2.CurrentRow.Index;
                sid = dataGridView2.Rows[a].Cells[0].Value.ToString();
                DialogResult dr = MessageBox.Show("你确定要删除这名学生的选课吗？", "通知", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    try
                    {
                        string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";//数据库连接字符串
                        SqlConnection connection = new SqlConnection(connString);//创建connection对象
                        string sql = "Delete from SC where ID='"+sid+"' and Cno='"+ccid+"'";
                        SqlCommand command = new SqlCommand(sql, connection);

                        //打开数据库连接
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        MessageBox.Show("删除成功！");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("删除失败!");
                    }
                    teacherdel2 = false;
                    this.button13_Click(sender, e);
                }
                else
                {
                    this.button13_Click(sender, e);
                    label24.Text = "请选择您要删除学生的课程：";
                    teacherdel2 = false;
                    return;
                }
            }
            else
                MessageBox.Show("Debug~~");
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            panel5.Visible = false;
            dataGridView3.DataSource = null;
        }
    }
}
