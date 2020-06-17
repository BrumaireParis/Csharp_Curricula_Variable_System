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
    public partial class Admin : Form
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
        public Admin()
        {
            InitializeComponent();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        DataTable dt;
        SqlConnection conn;
        private void button2_Click(object sender, EventArgs e)
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            conn = null;
            conn = new SqlConnection(connStr);
            conn.Open();
            try
            {
                string sql = "SELECT TRIM(ID) as 学号, TRIM(Sname) as 姓名, TRIM(Ssex) as 性别,Sage as 年龄, Trim(Sdept) as 院系, Trim(Sclass) as 班级,Trim(Stel) as 手机号 FROM Student";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridViewS.DataSource = dt.DefaultView;
                dataGridViewS.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
            conn.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            conn = null;
            conn = new SqlConnection(connStr);
            conn.Open();
            try
            {
                string sql = "SELECT TRIM(ID) as 工号, TRIM(Tname) as 姓名, TRIM(Tsex) as 性别,Tage as 年龄, Trim(Tdept) as 院系, Trim(Tlev) as 职称,Trim(Ttel) as 手机号 FROM Teacher";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridViewT.DataSource = dt.DefaultView;
                dataGridViewT.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            conn = null;
            conn = new SqlConnection(connStr);
            conn.Open();
            try
            {
                string sql = "SELECT TRIM(ID) as 学号, TRIM(Cno) as 课序号, Grade as 成绩 FROM SC";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridViewSC.DataSource = dt.DefaultView;
                dataGridViewSC.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string connStr = "Data Source=.;Initial Catalog=SCHOOL;Persist Security Info=True;User ID=sa;Password=a123456";
            conn = null;
            conn = new SqlConnection(connStr);
            conn.Open();
            try
            {
                string sql = "SELECT TRIM(Cno) as 课序号, TRIM(Cname) as 课程名, TRIM(Cpno) as 先修课序号,Ccredit as 学分, Trim(ID) as 工号 FROM Course";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridViewC.DataSource = dt.DefaultView;
                dataGridViewC.MultiSelect = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询错误！" + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable changeDt = dt.GetChanges();
                foreach (DataRow dr in changeDt.Rows)
                {
                    string sql = string.Empty;
                    if (dr.RowState == System.Data.DataRowState.Added)
                    {
                        sql = "INSERT INTO Student(ID,Sname,Ssex,Sage,Sdept,Sclass,Stel)" +
                            "VALUES('" + dr["学号"].ToString() + "','" + dr["姓名"].ToString() + "','" + dr["性别"].ToString() + "','" + dr["年龄"].ToString() + "'," +
                            "'" + dr["院系"].ToString() + "','" + dr["班级"].ToString() + "','" + dr["手机号"].ToString() + "')";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Deleted)
                    {
                        sql = "DELETE FROM Student WHERE Student.ID='"+dr["学号",DataRowVersion.Original].ToString()+"'";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Modified)
                    {
                        sql = "UPDATE Student SET Ssex='" + dr["性别"].ToString() + "'," +
                        "Sage='"+Convert.ToInt32(dr["年龄"])+"',Sdept='" + dr["院系"].ToString() + "',Sclass='" + dr["班级"].ToString() + "',Stel='" + dr["手机号"].ToString() + "'" +
                        "WHERE ID='"+ dr["学号"].ToString() +"'";  
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch(Exception)
            {
                MessageBox.Show("保存失败!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable changeDt = dt.GetChanges();
                foreach (DataRow dr in changeDt.Rows)
                {
                    string sql = string.Empty;
                    if (dr.RowState == System.Data.DataRowState.Added)
                    {
                        sql = "INSERT INTO Teacher(ID,Tname,Tsex,Tage,Tdept,Tlev,Ttel)" +
                            "VALUES('" + dr["工号"].ToString() + "','" + dr["姓名"].ToString() + "','" + dr["性别"].ToString() + "','" + dr["年龄"].ToString() + "'," +
                            "'" + dr["院系"].ToString() + "','" + dr["职称"].ToString() + "','" + dr["手机号"].ToString() + "')";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Deleted)
                    {
                        sql = "DELETE FROM Teacher WHERE Teacher.ID='" + dr["工号", DataRowVersion.Original].ToString() + "'";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Modified)
                    {
                        sql = "UPDATE Teacher SET Tsex='" + dr["性别"].ToString() + "'," +
                        "Tage='" + Convert.ToInt32(dr["年龄"]) + "',Tdept='" + dr["院系"].ToString() + "',Tlev='" + dr["职称"].ToString() + "',Ttel='" + dr["手机号"].ToString() + "'" +
                        "WHERE ID='" + dr["工号"].ToString() + "'";
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败!");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable changeDt = dt.GetChanges();
                foreach (DataRow dr in changeDt.Rows)
                {
                    string sql = string.Empty;
                    if (dr.RowState == System.Data.DataRowState.Added)
                    {
                        sql = "INSERT INTO SC(ID,Cno,Grade)" +
                            "VALUES('" + dr["学号"].ToString() + "','" + dr["课序号"].ToString() + "','" + dr["成绩"].ToString() + "')";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Deleted)
                    {
                        sql = "DELETE FROM SC WHERE ID='" + dr["学号", DataRowVersion.Original].ToString() + "' AND Cno='"+ dr["课序号"].ToString() + "'";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Modified)
                    {
                        sql = "UPDATE SC SET Grade='" + Convert.ToInt32(dr["成绩"]) + "'" +
                        "WHERE ID='" + dr["学号"].ToString() + "' AND Cno='" + dr["课序号"].ToString() + "'";
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败!");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable changeDt = dt.GetChanges();
                foreach (DataRow dr in changeDt.Rows)
                {
                    string sql = string.Empty;
                    if (dr.RowState == System.Data.DataRowState.Added)
                    {
                        sql = "INSERT INTO Course(Cno,Cname,Cpno,Ccredit,ID)" +
                            "VALUES('" + dr["课序号"].ToString() + "','" + dr["课程名"].ToString() + "','" + dr["先修课序号"].ToString() + "','" + dr["工号"].ToString() + "')";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Deleted)
                    {
                        sql = "DELETE FROM Course WHERE Cno='" + dr["课序号", DataRowVersion.Original].ToString() + "'";
                    }
                    else if (dr.RowState == System.Data.DataRowState.Modified)
                    {
                        sql = "UPDATE Course SET Cname='" + dr["课程名"].ToString() + "'," +
                        "Ccredit='" + Convert.ToInt32(dr["学分"]) + "',ID='" + dr["工号"].ToString() + "',Cpno='" + dr["先修课序号"].ToString() + "'" +
                        "WHERE Cno='" + dr["课序号"].ToString() + "'";
                    }
                    SqlCommand comm = new SqlCommand(sql, conn);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败!");
            }
        }
    }
}
