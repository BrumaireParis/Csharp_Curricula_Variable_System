using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;

namespace Test
{
    public partial class ResetP : Form
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
        public ResetP()
        {
            InitializeComponent();
            //button1.Text = "NONO";
        }

        private void PictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private string code;
        private int cnt1, cnt2;

        public static string CreateRandomCode(int length)  //生成由数字和大小写字母组成的验证码
        {
            string list = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            Random random = new Random();
            string code = "";   //验证码
            for (int i = 0; i < length; i++)   //循环6次得到一个伪随机的六位数验证码
            {
                code += list[random.Next(0, list.Length - 1)];
            }
            return code;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string userid = textBox1.Text.Trim();   //账号
            string email = textBox2.Text.Trim();    //邮箱
            if (!String.IsNullOrEmpty(userid) && !String.IsNullOrEmpty(email))  //账号、邮箱非空
            {
                SqlConnection connection = new SqlConnection("Data Source=.;Initial Catalog=SCHOOL;User ID=sa;Password=a123456");
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT Accounts,Email FROM Account WHERE Accounts='" + userid + "' AND Email='" + email + "'", connection);
                SqlDataReader data = command.ExecuteReader();
                if (data.HasRows)   //若输入的电子邮箱是账号注册时填写的邮箱
                {
                    try
                    {
                        MailMessage mail = new MailMessage();  //实例化一个发送邮件类
                        mail.From = new MailAddress(Program.Email163);   //发件人邮箱地址
                        mail.To.Add(new MailAddress(email));    //收件人邮箱地址
                        mail.Subject = "【选课管理系统V1.0】找回密码";    //邮件标题
                        code = CreateRandomCode(6);   //生成伪随机的6位数验证码
                        mail.Body = "验证码是: " + code + "，请在5分钟内进行验证。验证码提供给他人可能导致账号被盗，请勿泄露，谨防被骗。系统邮件请勿回复。";  //邮件内容          
                        SmtpClient client = new SmtpClient("smtp.163.com");   //实例化一个SmtpClient类。
                        client.EnableSsl = true;    //使用安全加密连接
                        client.Credentials = new NetworkCredential(Program.Email163, Program.AuthorizationCode);//验证发件人身份(发件人的邮箱，邮箱里的生成授权码);        
                        client.Send(mail);
                        //计时器初始化
                        cnt1 = 600;
                        cnt2 = 3000;
                        timer1.Enabled = true;   //time1用来记录1分钟
                        timer2.Enabled = true;   //time2用来记录5分钟
                        button1.Enabled = false;  //发送按钮不可点击
                        MessageBox.Show("发送成功！");
                    }
                    catch
                    {
                        MessageBox.Show("发送失败！\n请检查邮箱是否输入有误。", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("该邮箱不是账号绑定的邮箱。", "失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("请将账号和邮箱填写完整！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)    //发送完邮件,需要60秒后才能再次发送邮件
        {
            if (cnt1 > 0)
            {
                cnt1--;
                button1.Text = "发送(" + cnt1/10 + ")";
            }
            else
            {
                timer1.Enabled = false;
                button1.Enabled = true;
                button1.Text = "发送";
            }
        }

        private void Timer2_Tick(object sender, EventArgs e)    //验证码5分钟内有效,但是如果有新的验证码出现,旧验证码就会GG
        {
            if (cnt2 == 0)
            {
                timer2.Enabled = false;
                code = CreateRandomCode(6);    //旧的验证码过期,生成一个新的验证码
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string rpassword = Program.EncryptWithMD5(textBox4.Text.Trim());
            string ac = textBox1.Text.Trim();
            if(string.Compare(textBox3.Text.Trim(),code,true)==0)
            {
                try
                {
                    string connString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";//数据库连接字符串
                    SqlConnection connection = new SqlConnection(connString);//创建connection对象
                    string sql = "Update Account set Passwords = '"+rpassword+"'Where Accounts='"+ac+"'";
                    SqlCommand command = new SqlCommand(sql, connection);

                    SqlParameter sqlParameter = new SqlParameter("@userid", ac);
                    command.Parameters.Add(sqlParameter);
                    sqlParameter = new SqlParameter("@userpassword", rpassword);
                    command.Parameters.Add(sqlParameter);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("更改密码成功！");
                    this.Close();

                }
                catch(SqlException)
                {
                    MessageBox.Show("数据库连接错误!");
                }
            }
            else
            {
                MessageBox.Show("验证码错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
