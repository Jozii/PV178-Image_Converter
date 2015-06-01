using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageConverter.Extended_components
{
    class NumberTextBox : TextBox
    {
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            var numberFormat = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            var decimalSeparator = numberFormat.NumberDecimalSeparator;
            var groupSeparator = numberFormat.NumberGroupSeparator;
            var negativeSign = numberFormat.NegativeSign;

            var keyCharStr = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar)) return;
            if (e.KeyChar == '\b') return;
            if (decimalSeparator.Equals(keyCharStr)) return;
            if (groupSeparator.Equals(keyCharStr)) return;
            if (negativeSign.Equals(keyCharStr)) return;

            e.Handled = true;
        }

        public double DoubleValue
        {
            get { return double.Parse(this.Text); }
        }
    }
}
