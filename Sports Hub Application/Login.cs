﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Subscriptions_Application
{
    public partial class Login : KryptonForm
    {
        private string ConnectionString;
        public static string LoggedInUsername { get; private set; }
        public static int LoggedInUserRole { get; private set; }


        private float _initialFormWidth;
        private float _initialFormHeight;
        private ControlInfo[] _controlsInfo;


        public Login()
        {
            InitializeComponent();
            ConnectionString = DatabaseConfig.connectionString;
            this.AcceptButton = loginbtn; // Set the AcceptButton property
            
            






            _initialFormWidth = this.Width;
            _initialFormHeight = this.Height;

            // Store initial size and location of all controls
            _controlsInfo = new ControlInfo[this.Controls.Count];
            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                _controlsInfo[i] = new ControlInfo(c.Left, c.Top, c.Width, c.Height, c.Font.Size);
            }

            // Set event handler for form resize
            this.Resize += Home_Resize;

        }

        private void Home_Resize(object sender, EventArgs e)
        {
            float widthRatio = this.Width / _initialFormWidth;
            float heightRatio = this.Height / _initialFormHeight;
            ResizeControls(this.Controls, widthRatio, heightRatio);
        }

        private void ResizeControls(Control.ControlCollection controls, float widthRatio, float heightRatio)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                Control control = controls[i];
                ControlInfo controlInfo = _controlsInfo[i];

                control.Left = (int)(controlInfo.Left * widthRatio);
                control.Top = (int)(controlInfo.Top * heightRatio);
                control.Width = (int)(controlInfo.Width * widthRatio);
                control.Height = (int)(controlInfo.Height * heightRatio);

                // Adjust font size
                control.Font = new Font(control.Font.FontFamily, controlInfo.FontSize * Math.Min(widthRatio, heightRatio));
            }
        }

        private class ControlInfo
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public float FontSize { get; set; }

            public ControlInfo(int left, int top, int width, int height, float fontSize)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
                FontSize = fontSize;
            }
        }


        private void loginbtn_Click(object sender, EventArgs e)
        {
            string username = Usertxt.Text;
            string password = passwordtxt.Text;

            if (ValidateLogin(username, password, out int roleID))
            {
                LoggedInUsername = username;
                LoggedInUserRole = roleID;

                // Create and show the main form based on role
                Form mainForm = CreateFormBasedOnRole(roleID);
                mainForm.FormClosed += (s, args) => Application.Exit(); // Exit the application when the main form is closed

                this.Hide(); // Hide the login form
                mainForm.Show(); // Show the main form
            }
            else
            {
                MessageBox.Show("Username or Password is Incorrect.");
            }
        }



        private bool ValidateLogin(string username, string password, out int roleID)
        {
            bool isValid = false;
            roleID = 0; // Default value for roleID

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT PasswordHash, RoleID FROM MixedGymDB.dbo.CashierDetails WHERE Username = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            string storedPasswordHash = reader["PasswordHash"] as string;
                            roleID = (int)reader["RoleID"];

                            if (storedPasswordHash != null && storedPasswordHash == password)
                            {
                                isValid = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message);
                    }
                }
            }

            return isValid;
        }

        private Form CreateFormBasedOnRole(int roleID)
        {
            switch (roleID)
            {
                case 1:
                    this.Hide();
                    Cashier Cashier = new Cashier(LoggedInUsername);
                    Cashier.ShowDialog();
                    this.Close  ();
                    return this;

                    // Return form for Cashier
                    
                case 2:

                    this.Hide();
                    Cashier Cashiers = new Cashier(LoggedInUsername);
                    Cashiers.ShowDialog();
                    this.Close();
                    return this;

                case 3:

                    this.Hide();
                    Home Home = new Home(LoggedInUsername);
                    Home.ShowDialog();
                    this.Close();
                    return this;
                // Return form for Admin
                //return new AdminForm(LoggedInUsername);
                case 4:
                    this.Hide();
                    Home Homes = new Home(LoggedInUsername);
                    Homes.ShowDialog();
                    this.Close();
                    return this;
                // Return form for Control
                // return new ControlForm(LoggedInUsername);
                default:
                    // Default form or error handling
                    throw new InvalidOperationException("Invalid Role Call ur Software Company 'Selim'   01155003537");
            }
        }

        private void kryptonPalette1_PalettePaint(object sender, PaletteLayoutEventArgs e)
        {

        }

        private void kryptonLabel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
