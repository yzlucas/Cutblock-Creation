using System;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;




/*  <add two columns to Cutblock table>:
 1. Pass the windows form log ID to database
 2. Add message column to cutblock table,
 */
namespace CutblockCreation
{
    public partial class Form1 : Form
    { 
        DateTime CreationDate = DateTime.Now;
        String username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("WESTERNFOREST\\", "");
        String LogId = System.Guid.NewGuid().ToString();
        string constring = Properties.Settings.Default.connectionString.ToString();
        DataSet ds = new DataSet();
        public Form1() 
        {
       
            //indow.Current.SetTitleBar(System.Security.Principal.WindowsIdentity.GetCurrent().Name)

            InitializeComponent();
            this.Text = "Welcome " + username;
            Mes = "Session Initialized";         
            DatabaseCaller(LogId, username, selectedOperation, selectedBlockname, CreationDate,Mes);

        }

        private void TableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private string selectedOperation = string.Empty;
        private string selectedBlockname = string.Empty;
        private string Mes = string.Empty;

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedOperation = (string)comboBox1.SelectedItem;
        }

        //Create Button's Action
        private void Button1_Click(object sender, EventArgs e)
        {
            DateTime CreationDate = DateTime.Now;
         
            string dest = Path.Combine(Properties.Settings.Default.destination.ToString(), selectedOperation, "Cutblocks", selectedBlockname);
            string templateSource = Properties.Settings.Default.templateSource.ToString();
                //@"\\westernforest\transfer\LLi\CutblockStructure";

            bool exists01 = System.IO.Directory.Exists(dest);
            bool existsTemplate = System.IO.Directory.Exists(templateSource);
            

            if (!exists01 & existsTemplate & selectedBlockname!=string.Empty & selectedOperation!=string.Empty & !selectedBlockname.Contains("\\") & !selectedBlockname.Contains("/"))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    CloneDirectory(templateSource, dest);
                    Cursor.Current = Cursors.Default;                   
                    Mes = "Creation Success";                    
                    DatabaseCaller(LogId, username, selectedOperation, selectedBlockname, CreationDate, Mes);
                    MessageBox.Show(selectedBlockname + " Created!");


                }
                catch(Exception ex)
                {
                    DatabaseCaller(LogId, username, selectedOperation, selectedBlockname, CreationDate, ex.Message.ToString());
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            
            else
            {

                if (exists01 & selectedBlockname!=string.Empty)
                {
                    Mes = "Error, Cutblock Already Exists!";                  
                    DatabaseCaller(LogId, username, selectedOperation, selectedBlockname, CreationDate, Mes);
                    MessageBox.Show("Cutblock Already Exists!");
                   
                }
                else if (!existsTemplate)
                {
                    Mes = "Error, Template does not Exists, please contact admin";                    
                    DatabaseCaller(LogId, username, selectedOperation, selectedBlockname, CreationDate, Mes);
                    MessageBox.Show("Template does not Exists, please contact admin"); 
                }
                else 
                {
                    if (selectedBlockname.Contains("\\")|| selectedBlockname.Contains("/"))
                    {
                        Mes = "Error, do not contain \\ or /";                       
                        DatabaseCaller(LogId, username, selectedOperation, selectedBlockname, CreationDate, Mes);
                        MessageBox.Show("Error, do not contain \\ or / ");
                        
                    }
                    else
                    {
                        Mes = "Error, operation and block name cannot be empty";
                        DatabaseCaller(LogId,username,selectedOperation,selectedBlockname,CreationDate,Mes);
                        MessageBox.Show(Mes);                    
                    }
                }
                
            }

        }
        
       private static void DatabaseCaller(String logid, String username, String selectedoperation, String selectedblockname, DateTime creationdate, String mes)
        {
            string constring = Properties.Settings.Default.connectionString.ToString();
            DataSet ds = new DataSet();
            String Query = " DECLARE @return_value int EXEC @return_value = [dbo].[CutblockCreation_writeLog]  @Logid = '" + logid + "',@username = '" + username + "',@selectedOperation = '" + selectedoperation + "',@selectedBlockName = '" + selectedblockname + "',@CreationDate = '" + creationdate + "',@Message = '" + mes + "';";
            using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.connectionString.ToString()))
            {
                SqlCommand cmd = new SqlCommand(Query, connection);
                using (SqlDataAdapter sa = new SqlDataAdapter(cmd))
                {
                    sa.Fill(ds);
                }
            }
        }
            
        
        private static void CloneDirectory(string root, string dest)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                string dirName = Path.GetFileName(directory);
                if (!Directory.Exists(Path.Combine(dest, dirName)))
                {
                    
                        Directory.CreateDirectory(Path.Combine(dest, dirName));
                   
                }
                CloneDirectory(directory, Path.Combine(dest, dirName));
            }
            foreach (var file in Directory.GetFiles(root))
            {
                File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
            }
        }
        
        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void BlockName_TextChanged(object sender, EventArgs e)
        {

                selectedBlockname = BlockName.Text;

            
          
        }

       
    }
}
