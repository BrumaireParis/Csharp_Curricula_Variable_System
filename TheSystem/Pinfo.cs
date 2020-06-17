using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Pinfo : Form
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
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 163 && this.ClientRectangle.Contains(this.PointToClient(new Point(m.LParam.ToInt32()))) && m.WParam.ToInt32() == 2)
                m.WParam = (IntPtr)1;
            base.WndProc(ref m);
            if (m.Msg == 132 && m.Result.ToInt32() == 1)
                m.Result = (IntPtr)2;
        }
        public Pinfo()
        {
            InitializeComponent();
        }

        private void Pinfo_Load(object sender, EventArgs e)
        {
            if (Program.LoginType.Trim() == "学生")
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
                        textBox1.Text = read1["Sname"].ToString().Trim();
                        textBox2.Text = read1["Sage"].ToString().Trim();
                        comboBox1.Text = read1["Ssex"].ToString().Trim();
                        textBox3.Text = read1["Sdept"].ToString().Trim();
                        textBox4.Text = read1["Sclass"].ToString().Trim();
                        textBox5.Text = read1["Stel"].ToString().Trim();
                    }
                    read1.Close();
                    myconnection.Close();
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
                        textBox1.Text = read2["Tname"].ToString().Trim();
                        textBox2.Text = read2["Tage"].ToString().Trim();
                        comboBox1.Text = read2["Tsex"].ToString().Trim();
                        textBox3.Text = read2["Tdept"].ToString().Trim();
                        textBox4.Text = read2["Tlev"].ToString().Trim();
                        textBox5.Text = read2["Ttel"].ToString().Trim();
                    }
                    read2.Close();
                    cconnection.Close();
                    label6.Text = "职称：";
                }
                catch (Exception) { MessageBox.Show("无法查询教师类账户！"); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            { 
                if (control.GetType().Name == "TextBox") 
                {
                    ((TextBox)control).Text = string.Empty; 
                } 
            }
            comboBox1.Text = "-请选择-";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            bool flag = true;
            string text = textBox2.Text.Trim();
            for (int i = 0; i <= text.Length-1; i++)
            {
                if((int)text[i]<48||(int)text[i]>58)
                {
                    flag = false;
                    break;
                }
            }
            if (textBox2.Text.Trim() != "")
            {
                if (flag == true)
                { }
                else
                {
                    MessageBox.Show("请输入数字！");
                    textBox2.Text = "";
                    textBox2.Focus();
                }
            }
            else
                MessageBox.Show("信息不能为空");
        }
        private void comboBox1_Leave(object sender,EventArgs e)
        {
            if (comboBox1.Text.Trim() != "男" || comboBox1.Text.Trim() != "女" || comboBox1.Text.Trim() != "其它")
                MessageBox.Show("请不要输入奇怪的信息～");
        }
  
        private void textBox5_Leave(object sender, EventArgs e)
        {
            bool flag = true;
            string text = textBox5.Text.Trim();
            for (int i = 0; i <= text.Length - 1; i++)
            {
                if ((int)text[i] == 43 || (int)text[i] == 45)
                    break;
                if ((int)text[i] < 48 || (int)text[i] > 58)
                {
                    flag = false;
                    break;
                }
            }
            if (textBox5.Text.Trim() != "")
            {
                if (flag == true)
                { }
                else
                {
                    MessageBox.Show("请输入正确的手机号码格式！");
                    //textBox5.Text = "";
                    textBox5.Focus();
                }
            }
            else
                MessageBox.Show("信息不能为空");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string age = textBox2.Text.Trim();
            string sex = comboBox1.Text.Trim();
            string dept = textBox3.Text.Trim();
            string Lclass = textBox4.Text.Trim();
            string tel = textBox5.Text.Trim();

            if (username == "" || age == "" || sex == "" || dept == "" || Lclass == "" || tel == "")
            {
                MessageBox.Show("有信息为空，操作失败!");
                return;
            }
            string myConnString = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";

            SqlConnection sqlConnection = new SqlConnection(myConnString);  //实例化连接对象
            sqlConnection.Open();

            if (Program.LoginType.Trim() == "学生")
            {
                /*try
                {*/
                string sql = "Update Student set Sname= @usern, Sage=@usera,Ssex=@users,Sdept=@userd,Sclass=@userc,Stel=@usert Where ID='" + Program.loginID + "'";
                SqlCommand command = new SqlCommand(sql, sqlConnection);

                SqlParameter sqlParameter = new SqlParameter("@usern", username);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@usera", age);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@users", sex);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@userd", dept);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@userc", Lclass);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@usert", tel);
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();
                /*}
                catch (Exception) { MessageBox.Show("数据更新出错"); }*/
            }
            else if (Program.LoginType.Trim() == "教师")
            {
                /*try
                {*/
                string sql = "Update Teacher set Tname= @usern, Tage=@usera,Tsex=@users,Tdept=@userd,Tlev=@userc,Ttel=@usert Where ID='" + Program.loginID + "'";
                SqlCommand command = new SqlCommand(sql, sqlConnection);

                SqlParameter sqlParameter = new SqlParameter("@usern", username);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@usera", age);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@users", sex);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@userd", dept);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@userc", Lclass);
                command.Parameters.Add(sqlParameter);
                sqlParameter = new SqlParameter("@usert", tel);
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();
                /*}
                catch (Exception) { MessageBox.Show("数据更新出错"); }*/
            }
            else
            {
                MessageBox.Show("管理员不配拥有资料哦!");
                return;
            }
            MessageBox.Show("更改成功!");
            sqlConnection.Close();
            this.Close();
        }
  
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
