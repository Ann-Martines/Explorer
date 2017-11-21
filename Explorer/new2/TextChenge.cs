using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace new2
{
	public partial class TextChenge : Form
	{
		public TextChenge()
		{
			InitializeComponent();
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
	{
			var t = @"\/:*?'<>|";

			foreach (var i in t)
			{
				if (this.textBox1.Text.Contains(i))
				{
					MessageBox.Show(@"The name of the file shouldn't contain the following signs: \/:*?'<>|");
					this.textBox1.Text = "";
				}
				
			}


		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
