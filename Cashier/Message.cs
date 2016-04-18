using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cashier
{
    class Message
    {
        /// <summary>
        /// Holds the component which is regarded as "Title"
        /// </summary>
        public static Control TitleComponent { get; set; }


        /// <summary>
        /// Holds the component which is regarded as "Message"
        /// </summary>
        public static Control MessageComponent { get; set; }


        /// <summary>
        /// Builds the message and places it in the Control
        /// </summary>
        /// <param name="Title">The title of the message</param>
        /// <param name="_Message">The message itself</param>
        /// <param name="isErroneous">Set to true if the message is erroneous, else set to false</param>
        private static void Build(String Title, String _Message, Boolean isErroneous)
        {
            TitleComponent.Text = Title;
            TitleComponent.ForeColor = isErroneous ? Color.DarkRed : Color.DarkGreen;
            MessageComponent.Text = _Message;
            MessageComponent.ForeColor = Color.Black;
        }


        /// <summary>
        /// Shorthand for Message.Build(Control, Control, true)
        /// </summary>
        /// <param name="Title">The title of the message</param>
        /// <param name="_Message">The message itself</param>
        public static void Fatal(String Title, String _Message)
        {
            Build(Title, _Message, true);
        }


        /// <summary>
        /// Shorthand for Message.Build(Control, Control, false)
        /// </summary>
        /// <param name="Title">The title of the message</param>
        /// <param name="_Message">The message itself</param>
        public static void Inform(String Title, String _Message)
        {
            Build(Title, _Message, false);
        }
    }
}
