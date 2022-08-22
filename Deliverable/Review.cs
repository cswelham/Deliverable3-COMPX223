﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Deliverable
{
    public partial class Review : Form
    {
        public Review()
        {
            InitializeComponent();
            //This line of code allows us to obscure the password visually and limit the max chars in textbox
            textBoxDescription.MaxLength = 52;         //max textbox character count

            //Setup boardgame combobox
            SQL.selectQuery("select * from boardgame order by name asc");
            if (SQL.read.HasRows)
            {
                while (SQL.read.Read())
                {
                    comboBoxId.Items.Add(SQL.read[1]);
                }
            }
            else
            {
                MessageBox.Show("No boardgames have been registered.");
                return;
            }

            //Set textbox to current logged in customer
            textBoxUsername.Text = CustomerUsername.Username;
        }

        /// <summary>
        /// Initialises all textboxes to blank text
        /// Re focus to first text box
        /// </summary>
        public void initialiseTextBoxes()
        {
            //goes through and clears all of the textboxes
            foreach (Control c in this.Controls)
            {
                if (c is TextBox && (c != textBoxUsername))
                {
                    (c as TextBox).Clear();
                }
                else if (c is ComboBox)
                {
                    (c as ComboBox).Text = "";
                }
                else if (c is RadioButton)
                {
                    (c as RadioButton).Checked = false;
                }
            }
        }

        /// <summary>
        /// Clears the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            initialiseTextBoxes();
        }

        /// <summary>
        /// Go to register a manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            //variables to be used
            string date="", description="", boardgameName = "", boardgameID = "";
            int rating = 0;
            string day = "", month = "", year = "";
            

            //Check that the text boxes has something typed in it using a method
            bool hasText = checkTextBoxes();
            if (!hasText)
            {
                MessageBox.Show("Please make sure all textboxes have text.");
                return;
            }

            //(1) GET the data from the textboxes and store into variables created above, good to put in a try catch with error message
            try
            {
                //Get the values from the textboxes
                boardgameName = comboBoxId.Text;
                description = textBoxDescription.Text.Trim();

                //Get the id of the boardgame selected
                SQL.selectQuery("select id from boardgame where name = '" + boardgameName + "'");
                if (SQL.read.HasRows)
                {
                    while (SQL.read.Read())
                    {
                        boardgameID = SQL.read[0].ToString();
                    }
                }
                else
                {
                    MessageBox.Show("No boardgames have been registered.");
                    return;
                }

                //Format date
                day = dateTimePickerDate.Value.Date.Day.ToString();
                month = dateTimePickerDate.Value.Date.Month.ToString();
                year = dateTimePickerDate.Value.Date.Year.ToString();
                
                if (day.Length == 1)
                {
                    day = "0" + day;
                }
                if (month.Length == 1)
                {
                    month = "0" + month;
                }
                date = year + "/" + month + "/" + day;

                //Checking radio buttons
                if (radioButton1.Checked == true)
                {
                    rating = 1;
                }
                else if (radioButton2.Checked == true)
                {
                    rating = 2;
                }
                else if (radioButton3.Checked == true)
                {
                    rating = 3;
                }
                else if (radioButton4.Checked == true)
                {
                    rating = 4;
                }
                else if (radioButton5.Checked == true)
                {
                    rating = 5;
                }
                else
                {
                    MessageBox.Show("Please select a rating.");
                    return;
                }

            }
            catch
            {
                //Error message, more useful when you are storing numbers etc. into the database.
                MessageBox.Show("Please make sure your text is in correct format.");
                return;
            }

            //(2) Execute the INSERT statement, making sure all quotes and commas are in the correct places.
            //      Practice first on SQL Server Management Studio to make sure it is entering the correct data and in the correct format,
            //      then copy across the statement and where there are string replace the actual text for the variables stored above.
            //Example query: " INSERT INTO Users VALUES ('jkc1', 'John', 'Middle', 'Carter', 'pass1') "
            try
            {
                //(2) SELECT statement getting all data from users, i.e. SELECT * FROM Users
                SQL.executeQuery("insert into reviews values('"+CustomerUsername.Username+"', '"+ boardgameID+ "', '"+date+"', '"+description+"', '"+rating.ToString()+"')");
                //success message for the user to know it worked
                MessageBox.Show("Review is Succesful! Game was given " + rating + " stars");
            }
            catch
            {
                MessageBox.Show("Could not review game. Please try again.");
                return;
            }

            initialiseTextBoxes();
        }

        /// <summary>
        /// Checks if they textboxes have data in them
        /// </summary>
        /// <returns>TRUE if all hold text, but FALSE if at least one does not hold data</returns>
        private bool checkTextBoxes()
        {
            bool holdsData = true;
            //go through all of the controls
            foreach (Control c in this.Controls)
            {
                //if its a textbox
                if (c is TextBox)
                {
                    //If it is not the case that it is empty
                    if ("".Equals((c as TextBox).Text.Trim()))
                    {
                        //set boolean to false because on textbox is empty
                        holdsData = false;
                    }
                }
                //if its a combobox
                else if (c is ComboBox)
                {
                    //If it is not the case that it is empty
                    if ((c as ComboBox).SelectedItem == null)
                    {
                        //set boolean to false because on textbox is empty
                        holdsData = false;
                    }
                }
            }
            //returns true or false based on if data is in all text boxs or not
            return holdsData;
        }

        /// <summary>
        /// Back to customer menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBack_Click(object sender, EventArgs e)
        {
            //Hides the login page form from user
            this.Hide();
            //Create a Menu Page object to change to
            CustomerMenu menu = new CustomerMenu();
            //show the manager page
            menu.ShowDialog();
            //close the login page we are currently on
            this.Close();
        }
    }
}
