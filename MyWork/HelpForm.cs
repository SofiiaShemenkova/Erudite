using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyWork
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {
            lbl_rules.Text = "В начале игры каждому даётся по 7 фишек. За один ход можно выложить несколько слов. Каждое новое слово должно соприкасаться с ранне выложенными словами." +
                " Слова читаются слева направо и по вертикали сверху вниз. Если игрок не хочет или не может выложить ни одного слова, - он имеет право поменять любое количество своих букв, пропустив при этом ход." +
                " Любая последовательность букв по горизонтали и вертикали должна являться словом. То есть, в игре не допускается появление случайных буквосочетаний." +
                " После каждого хода необходимо добрать новых букв до семи. Побежает игрок, набравший наибольшее количество очков.";
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
