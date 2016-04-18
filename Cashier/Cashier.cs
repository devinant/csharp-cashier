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

namespace Cashier
{
    public partial class Cashier : Form
    {
	// INSERT YOUR DATABASE DETAILS HERE
        private Connector Connector = new Connector("HOST", "DB", "PASSWORD");
        
        public Cashier()
        {
            InitializeComponent();

            // Set the Message defaults
            Message.TitleComponent = this.statusGroupBox;
            Message.MessageComponent = this.statusMessageLabel;

            // Minimum length of all fields
            Validator.MinimumLength = 2;
            Validator.MinimumPINLength = 4;

            // Fill all data
            this.FillAccountHolders();
            this.FillAccounts();

            try
            {
                using (SqlConnection connection = new SqlConnection(Connector.getConnectionString()))
                {
                    connection.Open();
                    Message.Inform("Self Check OK", "Your client is now usable.");
                }
            }
            catch
            {
                Message.Inform("Self Check Failed", "You are not connected to the database. Quit the application and contact your local technician.");
                this.lockAllComponents(false);
            }
        }


        /// <summary>
        /// Locks all components and make them uneditable until the lock is released
        /// </summary>
        /// <param name="enabled"></param>
        private void lockAllComponents(Boolean enabled)
        {
            Reset.Toggle(ref this.accountHoldersGroupBox, enabled);
            Reset.Toggle(ref this.accountsGroupBox, enabled);
            Reset.Toggle(ref this.accountsLinkingGroupBox, enabled);
        }


        /// <summary>
        /// Fills all account holder ComboBoxes
        /// </summary>
        private void FillAccountHolders()
        {
            // Both ComboBoxes contain the same data. No need to query twice. Just use
            // the first query
            var source = this.newAccountHolderComboBox.generateDataSource(this.Connector);
            this.newAccountHolderComboBox.Fill(source);
            this.linkAccountHolderComboBox.Fill(source);
        }


        /// <summary>
        /// Fills all Account ComboBoxes
        /// </summary>
        private void FillAccounts()
        {
            this.linkAccountComboBox.Fill(this.linkAccountComboBox.generateDataSource(this.Connector));
        }


        /// <summary>
        /// Event
        /// 
        /// Creates a new account holder when the Cashier clicks "Create Holder".
        /// </summary>
        /// <param name="sender">The component that sent the event</param>
        /// <param name="e">The arguments sent with the event</param>
        private void createHolderButton_Click(object sender, EventArgs e)
        {
            if (Validator.ContainsLetters(this.firstNameTextBox, this.lastNameTextBox, this.cityTextBox) &&
                Validator.ContainsPIN(this.pinTextBox, this.pinConfirmTextBox) &&
                Validator.ContainsAlnum(this.addressTextBox))
            {
                this.lockAllComponents(false);

                // Connect to the database
                SqlConnection connection = new SqlConnection(this.Connector.getConnectionString());
                connection.Open();

                using (SqlCommand command = new SqlCommand("spAddAccountHolder", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@first_name", this.firstNameTextBox.Text);
                    command.Parameters.AddWithValue("@last_name", this.lastNameTextBox.Text);
                    command.Parameters.AddWithValue("@pin_code", Convert.ToInt32(this.pinTextBox.Text));
                    command.Parameters.AddWithValue("@street", this.addressTextBox.Text);
                    command.Parameters.AddWithValue("@city", this.cityTextBox.Text);
                    command.Parameters.AddWithValue("@birth_date", this.dateOfBirthPicker.Value.ToShortDateString());

                    command.ExecuteNonQuery();

                    Message.Inform("Successful account holder creation", String.Format("Successfully created the account holder '{0} {1}'.", this.firstNameTextBox.Text, this.lastNameTextBox.Text));

                    // Reset account holders and fill
                    Reset.Group(ref this.accountHoldersGroupBox);
                    this.FillAccountHolders();
                }

                this.lockAllComponents(true);
            }
            else
                Message.Fatal("Failed to create account holder", "First Name, Last Name, Address and City must all consist of of minimum 2 characters. The PIN must contain 4 characters and be numeric.");
        }


        /// <summary>
        /// Event
        /// 
        /// Creates a new account when the Cashier clicks "Create Account". 
        /// If the Cashier selects a Holder from the ComboBox, then the account is linked to the
        /// selected Holder.
        /// </summary>
        /// <param name="sender">The component that sent the event</param>
        /// <param name="e">The arguments sent with the event</param>
        private void createAccountButton_Click(object sender, EventArgs e)
        {
            if (!Validator.IsAlnum(this.newAccountNameTextBox))
                Message.Fatal("Failed to create account", "You must define a account name.");
            else 
            {
                // Lock all components
                this.lockAllComponents(false);

                // Connect to the database
                SqlConnection connection = new SqlConnection(this.Connector.getConnectionString());
                connection.Open();

                using (SqlCommand command = new SqlCommand("spAddAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@name", this.newAccountNameTextBox.Text);
                    command.Parameters.AddWithValue("@holder_id", this.newAccountHolderComboBox.Key);

                    SqlParameter output = command.Parameters.AddWithValue("@account_id", 0);
                    output.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    if (this.newAccountHolderComboBox.Key > 0)
                        Message.Inform("Successful account creation and linking",
                            String.Format("Created the account '{0}', it's now linked to '{1}'.", this.newAccountNameTextBox.Text, this.newAccountHolderComboBox.Value));
                    else
                        Message.Inform("Succcessful account creation", String.Format("Created the account '{0}'.", this.newAccountNameTextBox.Text));

                    // An account was added, refill all the accounts
                    Reset.Group(ref this.accountsGroupBox);
                    this.FillAccounts();
                }

                // Lock all components
                this.lockAllComponents(true);
            }
        }


        /// <summary>
        /// Event
        /// 
        /// Links a account and a holder when the Cashier clicks "Link account and holder".
        /// </summary>
        /// <param name="sender">The component that sent the event</param>
        /// <param name="e">The arguments sent with the event</param>
        private void linkAccountAndHolderButton_Click(object sender, EventArgs e)
        {
            if (this.linkAccountComboBox.Key == 0 || this.linkAccountHolderComboBox.Key == 0)
                Message.Fatal("Failed to link", "You must select a item from both lists when linking a account to a account holder.");
            else
            {
                // Lock all components
                this.lockAllComponents(false);

                // Connect to the database
                SqlConnection connection = new SqlConnection(this.Connector.getConnectionString());
                connection.Open();

                using (SqlCommand command = new SqlCommand("spLinkAccountHolderToAccount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@holder_id", this.linkAccountHolderComboBox.Key);
                    command.Parameters.AddWithValue("@account_id", this.linkAccountComboBox.Key);

                    SqlParameter output = command.Parameters.AddWithValue("@inserted", 0);
                    output.Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    if (!Convert.ToBoolean(command.Parameters["@inserted"].Value))
                        Message.Fatal("Failed to link", String.Format("'{0}' is already linked to '{1}'.", this.linkAccountHolderComboBox.Value, this.linkAccountComboBox.Value));
                    else
                        Message.Inform("Successful linking", String.Format("'{0}' is now linked to '{1}'.", this.linkAccountHolderComboBox.Value, this.linkAccountComboBox.Value));

                    Reset.Group(ref this.accountsLinkingGroupBox);
                }

                // Lock all components
                this.lockAllComponents(true);
            }
        }
    }
}
