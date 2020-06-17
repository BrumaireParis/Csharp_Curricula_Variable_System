using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Reg : Form
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
        public Reg()
        {
            InitializeComponent();
        }
        bool formMove = false;//窗体是否移动
        Point formPoint;//记录窗体的位置
        private void Reg_MouseDown(object sender, MouseEventArgs e)//鼠标按下
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
        private void Reg_MouseMove(object sender, MouseEventArgs e)//鼠标移动
        {
            if (formMove == true)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(formPoint.X, formPoint.Y);
                Location = mousePos;
            }
        }
        private void Reg_MouseUp(object sender, MouseEventArgs e)//鼠标松开
        {
            if (e.Button == MouseButtons.Left)//按下的是鼠标左键
            {
                formMove = false;//停止移动
            }
        }
        private string code;
        private void Reg_Load(object sender, EventArgs e)
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
            label7.Text = code;
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

        private void Button1_Click(object sender, EventArgs e)
        {
            string r_username = textBox1.Text.Trim();
            string password1 = textBox2.Text.Trim();
            string password2 = textBox3.Text.Trim();
            string type = comboBox1.Text.Trim();
            string sid = textBox4.Text.Trim();
            string checkcode = textBox5.Text.Trim();
            string mail = textBox6.Text.Trim();

            foreach (Control control in this.Controls)
            { if (control.GetType().Name == "TextBox") { ((TextBox)control).Text = string.Empty; } }
            if (r_username==""||password2==""||sid==""||mail=="" )
            {
                MessageBox.Show("请完整填写信息！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Reg_Load();
                return;
            }
            if (string.Equals(password1, password2) == false)
            {
                MessageBox.Show("两次输入密码不相符！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Reg_Load();
                return;
            }
            //MessageBox.Show(type);
            if (type!="学生"&&type!="教师"&&type!="管理员")
            {
                MessageBox.Show("未选择账号类型！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Reg_Load();
                return;
            }
            if (string.Compare(checkcode,code,true)!=0)
            {
                MessageBox.Show("验证码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Reg_Load();
                return;
            }
            try
            {
                string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";//数据库连接字符串
                SqlConnection connection = new SqlConnection(connString);//创建connection对象
                string sql = "INSERT INTO Account (Accounts,Passwords,ID,Type,Email) VALUES (@username, @userpassword,@userid,@usertype,@usermail)";
                SqlCommand command = new SqlCommand(sql, connection);

                string sql2 = "SELECT Accounts FROM Account WHERE ID='" + sid + "'OR Accounts='"+r_username+"'";
                SqlCommand command2 = new SqlCommand(sql2, connection);

                SqlParameter sqlParameter = new SqlParameter("@username", r_username);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@userpassword", EncryptWithMD5(password1));
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@usertype", type);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@userid", sid);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@usermail", mail);
                command.Parameters.Add(sqlParameter);

                //打开数据库连接
                connection.Open();
                SqlDataReader sqlDataReader = command2.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    MessageBox.Show("该ID或账号已被注册!\nError:003", "notice", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Reg_Load();
                    return;
                }
                sqlDataReader.Close();
                command.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("注册成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        this.Close();
        //MessageBox.Show("看到这条表示程序还在运行","警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Reg_Load()
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
            label7.Text = code;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() != "")
            {
                //使用regex（正则表达式）进行格式设置 至少有数字、大写字母、小写字母各一个。最少6个字符、最长16个字符。
                Regex regex = new Regex(@"(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).{6,16}");

                if (regex.IsMatch(textBox2.Text))//判断格式是否符合要求
                {
                    //MessageBox.Show("输入密码格式正确!");
                }
                else
                {
                    MessageBox.Show("至少有数字、大写字母、小写字母各一个。最少6个字符、最长16个字符！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    textBox2.Text = string.Empty;
                    Reg_Load();
                    textBox2.Focus();
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1、密码必须大于等于6位且小于等于16位;\n2、密码至少含有一个大写字母、小写字母和数字;\n3、每人仅可注册一个账号，如有管理员权限请注册为管理员;\n4、验证码不区分大小写。", "帮助信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Label7_Click(object sender, EventArgs e)
        {
            Reg_Load();
        }
    }
}
