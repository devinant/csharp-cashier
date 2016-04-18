using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cashier
{
    class Validator
    {
        /// <summary>
        /// The minimum length for IsNumber, IsAlnum and IsLetter
        /// </summary>
        public static int MinimumLength { get; set; }


        /// <summary>
        /// The minimum length for IsPIN
        /// </summary>
        public static int MinimumPINLength { get; set; }


        /// <summary>
        /// Validates if a TextBox contains MinimumLength characters and contains only characters
        /// </summary>
        /// <param name="TextBox">The TextBox to validate</param>
        /// <returns>true if the TextBox is valid, otherwise false</returns>
        public static Boolean IsNumber(TextBox TextBox)
        {
            String text = TextBox.Text;
            return (text.Length == MinimumLength && text.All(char.IsDigit));
        }


        /// <summary>
        /// Validates if a TextBox contains MinimumPINLength characters and contains only numbers
        /// </summary>
        /// <param name="TextBox">The TextBox to validate</param>
        /// <returns>true if the TextBox is valid, otherwise false</returns>
        public static Boolean IsPIN(TextBox TextBox)
        {
            String text = TextBox.Text;
            return (text.Length == MinimumPINLength && text.All(char.IsDigit));
        }


        /// <summary>
        /// Validates if a TextBox contains MinimumLength characters and contains only letters
        /// </summary>
        /// <param name="TextBox">The TextBox to validate</param>
        /// <returns>true if the TextBox is valid, otherwise false</returns>
        public static Boolean IsLetter(TextBox TextBox)
        {
            String text = TextBox.Text;
            return (text.Length >= MinimumLength && text.All(char.IsLetter));
        }


        /// <summary>
        /// Validates if a TextBox contains MinimumLength characters
        /// </summary>
        /// <param name="TextBox">The TextBox to validate</param>
        /// <returns>true if the TextBox is valid, otherwise false</returns>
        public static Boolean IsAlnum(TextBox TextBox)
        {
            return (TextBox.Text.Length >= MinimumLength);
        }


        /// <summary>
        /// Validates if a array of TextBox using Validator.isNumber
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Boolean ContainsNumbers(params TextBox[] parameters)
        {
            Boolean IsValid = true;

            for (int i = 0; IsValid && i < parameters.Length; i++)
                IsValid = Validator.IsNumber(parameters[i]);

            return IsValid;
        }


        /// <summary>
        /// Validates if a array of TextBox using Validator.isNumber
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Boolean ContainsPIN(params TextBox[] parameters)
        {
            Boolean IsValid = true;

            for (int i = 0; IsValid && i < parameters.Length; i++)
                IsValid = Validator.IsPIN(parameters[i]);

            return IsValid;
        }


        /// <summary>
        /// Validates if a array of TextBox using Validator.isLetter
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Boolean ContainsLetters(params TextBox[] parameters)
        {
            Boolean IsValid = true;

            for (int i = 0; IsValid && i < parameters.Length; i++)
                IsValid = Validator.IsLetter(parameters[i]);

            return IsValid;
        }


        /// <summary>
        /// Validates if a array of TextBox using Validator.isAlnum
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Boolean ContainsAlnum(params TextBox[] parameters)
        {
            Boolean IsValid = true;

            for (int i = 0; IsValid && i < parameters.Length; i++)
                IsValid = Validator.IsAlnum(parameters[i]);

            return IsValid;
        }
    }
}
