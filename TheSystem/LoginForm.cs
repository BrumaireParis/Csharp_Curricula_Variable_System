using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Media;

namespace Test
{
    public partial class LoginForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;//用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }
        public LoginForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            SoundPlayer play = new SoundPlayer(Test.Properties.Resources.GF_Title); //Test.Properties.Resources.text4
            play.PlayLooping();
        }
        bool formMove = false;//窗体是否移动
        Point formPoint;//记录窗体的位置
        private void LoginForm_MouseDown(object sender, MouseEventArgs e)//鼠标按下
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
        private void LoginForm_MouseMove(object sender, MouseEventArgs e)//鼠标移动
        {
            if (formMove == true)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(formPoint.X, formPoint.Y);
                Location = mousePos;
            }
        }
        private void LoginForm_MouseUp(object sender, MouseEventArgs e)//鼠标松开
        {
            if (e.Button == MouseButtons.Left)//按下的是鼠标左键
            {
                formMove = false;//停止移动
            }
        }

        private string code;
        private void LoginForm_Load(object sender, EventArgs e)
        {
            Random ran = new Random();
            int number;
            char code1;
            for (int i = 0; i < 4; i++)
            {
                number = ran.Next();
                if (number % 2 == 0)
                    code1 = (char)('0' + (char)(number % 10));
                else
                    code1 = (char)('A' + (char)(number % 26));
                this.code += code1.ToString();
            }
            label6.Text = code;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string username = textBoxUserName.Text.Trim();  //取出账号
            string password = EncryptWithMD5(textBoxPassWord.Text.Trim());  //取出密码
            string checkcode = textBoxCode.Text.Trim();

            /*if (string.Compare(checkcode, code, true) != 0)
            {
                MessageBox.Show("验证码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                foreach (Control control in this.Controls)
                { if (control.GetType().Name == "TextBox") { ((TextBox)control).Text = string.Empty; } }
                LoginForm_Load();
                return;
            }*/
            //string connstr = ConfigurationManager.ConnectionStrings["connectionString"].ToString(); //读取连接字符串
            try
            {
                string myConnString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";

                SqlConnection sqlConnection = new SqlConnection(myConnString);  //实例化连接对象
                sqlConnection.Open();

                string sql = "select Accounts,Passwords from Account where Accounts = '" + username + "' and Passwords = '" + password + "'";                                          
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
            
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    SoundPlayer play = new SoundPlayer(Test.Properties.Resources.HK416);
                    play.Load();
                    play.Play();
                    play.Dispose();
                    MessageBox.Show("欢迎登录：" + username, "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    sqlDataReader.Close();
                    
                    string sql1 = "SELECT * FROM Account WHERE Accounts = '" + username + "'";
                    SqlCommand command = new SqlCommand(sql1, sqlConnection);
                    SqlDataReader read = command.ExecuteReader();
                    while (read.Read())
                    {
                        Program.loginName = read["Accounts"].ToString();
                        Program.photoPath = read["Photo"].ToString();
                        Program.loginID = read["ID"].ToString();
                        Program.LoginType = read["Type"].ToString();
                    }
                    MainForm mainform = new MainForm();
                    mainform.Show();
                    this.Hide();
                }
                else
                {
                    LoginForm_Load();
                    MessageBox.Show("账号密码错误!\nError:001", "notice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                sqlConnection.Close();
            }
            catch (SqlException) {
                LoginForm_Load();
                MessageBox.Show("数据库连接失败！请联系管理人员！\nError:002", "发生错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
        }

        private void LoginForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                Button1_Click(sender, e);
            }
        }
        private void LoginForm_Load()
        {
            //throw new NotImplementedException();
            Random ran = new Random();
            int number;
            char code1;
            code = null;
            for (int i = 0; i < 4; i++)
            {
                number = ran.Next();
                if (number % 2 == 0)
                    code1 = (char)('0' + (char)(number % 10));
                else
                    code1 = (char)('A' + (char)(number % 26));
                this.code += code1.ToString();
            }
            label6.Text = code;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Reg reg = new Reg();
            reg.StartPosition = FormStartPosition.CenterScreen;
            reg.ShowDialog();
        }
        public static string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }
            return strbul.ToString();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Label6_Click(object sender, EventArgs e)
        {
            LoginForm_Load();
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResetP resetp = new ResetP();
            resetp.StartPosition = FormStartPosition.CenterScreen;
            resetp.ShowDialog();
        }
    }
}
