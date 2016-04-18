using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cashier
{
    /// <summary>
    /// Creates a default ComboBox and uses its inherited values to construct a default text and default key item. 
    /// Items are populated using a view in MSSQL.
    /// 
    /// Before using this class, make sure that the following conditions are met:
    /// 
    /// MSSQL
    /// 1.  Your view in MSSQL exists and is functional: in this example we'll use the view called 'VPerson'
    /// 2.  Your view 'VPerson' returns two columns: in this example we'll use 'id' and 'name' (you can specify what columns to use in the designer)
    ///     One of the columns must return an INT.
    ///     
    /// Using our VPerson as example, when the view is retrieved from the database it should return a result like follows:
    /// +-------+-----------+
    /// |   id  |   name    |
    /// +-------+-----------+
    /// |   1   |   Ana     |
    /// |   2   |   Eva     |
    /// |   3   |   Emma    |
    /// +-------+-----------+
    /// 
    /// 
    /// Designer Toolbox:
    /// 1.  In the Data item: you have set DisplayMember to 'name'. 
    ///     This is what the ComboBox will display in the list.
    ///     
    /// 2.  In the Data item: you have set ValueMember to 'id'. 
    ///     This is the value of the DisplayMember in the list. This MUST be a INT value.
    /// 
    /// 3.  In the MSSQL item: you have to set View to 'VPerson'. 
    ///     This is the View ComboBoxPopulator will select from.
    ///     Internally, all it does is 'SELECT * FROM {View}', or as in our example: 'SELECT * FROM VPerson'
    ///     
    /// <optional>
    /// 4.  If you wish to have a default item displayed first in the list, like 'Select a Person':
    ///     In the MSSQL item: you have to set DefaultItemDisplay to 'Select a Person'
    /// 
    /// 5.  If you wish to have a default item value in the list, like '5000':
    ///     In the MSSQL item: you have to set DefaultItemValue to '5000'
    ///     
    ///     If the DefaultItemValue is not set, the default will be '0'.
    /// 
    /// </optional>
    /// </summary>
    class ComboBoxPopulator : System.Windows.Forms.ComboBox
    {
        public ComboBoxPopulator(String ValueMember, String DisplayMember, int DefaultKey, String DefaultValue, String MSSQLView)
        {
            this.ValueMember = ValueMember;
            this.DisplayMember = DisplayMember;

            this.DefaultKey = DefaultKey;
            this.DefaultValue = DefaultValue;

            this.View = MSSQLView;
        }

        public ComboBoxPopulator(int DefaultKey, String DefaultValue, String View)
            : this("Key", "Value", DefaultKey, DefaultValue, View) {}

        public ComboBoxPopulator(int DefaultKey, String DefaultValue)
            : this("Key", "Value", DefaultKey, DefaultValue, "") {}

        public ComboBoxPopulator()
            : this("", "", 0, "", "") {}

        [Category("MSSQL"), PropertyTab("Column Key"), Description("The column defined as a 'key'"), Browsable(true)]
        public String ColumnKey { get; set; }

        [Category("MSSQL"), PropertyTab("Column Value"), Description("The column defined as a 'value'"), Browsable(true)]
        public String ColumnValue { get; set; }

        [Category("MSSQL"), PropertyTab("View"), Description("Defines what view to call to populate the box with."), Browsable(true)]
        public String View { get; set; }

        [Category("MSSQL"), PropertyTab("First Item Key"), Description("The first item key in the box. FirstItemValue must be set for this to work."), Browsable(true)]
        public int DefaultKey { get; set; }

        [Category("MSSQL"), PropertyTab("First Item Value"), Description("The first item value in the box."), Browsable(true)]
        public String DefaultValue { get; set; }


        /// <summary>
        /// Removes and clears all ComboBox items. You must use this method when using Fill
        /// </summary>
        private void Empty()
        {
            this.DataSource = null;
            this.Items.Clear();
        }


        /// <summary>
        /// Fills (and empties) the inherited ComboBox with data acquired from the View
        /// </summary>
        /// <param name="source">A Dictionary with data that will be added to items in the ComboBox</param>
        public void Fill(Dictionary<int, string> source)
        {
            this.Empty();
            // Dictionary uses "Value" for DisplayMember and 
            // "Key" for ValueMember.
            this.DisplayMember = this.ColumnValue;
            this.ValueMember = this.ColumnKey;

            // If the object was acquired from another context, use it.
            source[this.DefaultKey] = this.DefaultValue;

            // Bind the <Key, Value> to values from the database
            this.DataSource = new BindingSource(source, null);
        }


        /// <summary>
        /// Returns the integer value of a selected item
        /// </summary>
        public Int32 Key { 
            get {
                return Convert.ToInt32(this.SelectedValue); 
            } 
        }


        /// <summary>
        /// Returns the text of a selected item
        /// </summary>
        public String Value { 
            get { 
                return this.Text; 
            } 
        }


        /// <summary>
        /// Queries the database with the specified View, and adds the results into its corresponding
        /// DisplayMember and ValueMember. Using the example in the class documentation (Ana, Eva, Emma) the
        /// returned dictionary object would return:
        /// Dictionary<int, string>
        /// [
        ///     KeyValuePair<int, string> (1, Ana)
        ///     KeyValuePair<int, string> (2, Eva)
        ///     KeyValuePair<int, string> (3, Emma)
        /// ]
        /// 
        /// Additionally, if this.DefaultValue is defined (for example: 'Select a Person') it will be added as 
        /// the first item to the Dictionary. The returned dictionary object would then return:
        /// Dictionary<int, string>
        /// [
        ///     KeyValuePair<int, string> (0, Select a Person)
        ///     KeyValuePair<int, string> (1, Ana)
        ///     KeyValuePair<int, string> (2, Eva)
        ///     KeyValuePair<int, string> (3, Emma)
        /// ]
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>A Dictionary with the populated data</returns>
        public Dictionary<int, string> generateDataSource(Connector connector)
        {
            // Allows for <Key, Value> use
            Dictionary<int, string> source = new Dictionary<int, string>();

            // Add the default values only if we have something to add.
            // The default key for defaultValue is 0
            if (!string.IsNullOrEmpty(this.DefaultValue))
                source.Add(this.DefaultKey, this.DefaultValue);
            
            SqlConnection connection = new SqlConnection(connector.getConnectionString());
            connection.Open();
            
            using (SqlCommand command = new SqlCommand(String.Format("SELECT * FROM {0}", this.View), connection))
            {
                SqlDataReader reader = command.ExecuteReader();

                // Populate all the fields and map ValueMember to Key and DisplayMember to Value
                while (reader.Read())
                    source.Add(Convert.ToInt32(reader[this.ValueMember].ToString()), reader[this.DisplayMember].ToString());

                reader.Close();
            }

            return source;
        }
    }
}
