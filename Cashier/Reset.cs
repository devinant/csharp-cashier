using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cashier
{
    class Reset
    {
        /// <summary>
        /// Empties a TextBox
        /// </summary>
        /// <param name="_TextBox">The textbox to empty</param>
        public static void TextBox(Control _TextBox)
        {
            ((TextBox)_TextBox).Text = string.Empty;
        }


        /// <summary>
        /// Resets a DateTimePicker to the value of Today
        /// </summary>
        /// <param name="_DateTimePicker">A DateTimePicker to reset to its default value</param>
        public static void DateTimePicker(Control _DateTimePicker)
        {
            ((DateTimePicker)_DateTimePicker).Value = DateTime.Now;
        }


        /// <summary>
        /// Resets a ComboBox to its initial value
        /// </summary>
        /// <param name="_ComboBox">The ComboBox</param>
        public static void ComboBox(Control _ComboBox)
        {
            ((ComboBox)_ComboBox).SelectedIndex = 0;
        }


        /// <summary>
        /// Resets an entire Group and all its components
        /// </summary>
        /// <param name="_GroupBox">A reference to the GroupBox</param>
        public static void Group(ref GroupBox _GroupBox)
        {
            foreach (Control _Control in _GroupBox.Controls)
                if (_Control is TextBox)
                    Reset.TextBox(_Control);
                else if (_Control is DateTimePicker)
                    Reset.DateTimePicker(_Control);
                else if (_Control is ComboBox || _Control is ComboBoxPopulator)
                    Reset.ComboBox(_Control);
        }

        /// <summary>
        /// Enables all or disables all components in a group
        /// </summary>
        /// <param name="enabled">Set to true to enable, false to disable</param>
        public static void Toggle(ref GroupBox _GroupBox, Boolean enabled)
        {
            foreach (Control _Control in _GroupBox.Controls)
                _Control.Enabled = enabled;
        }
    }
}
