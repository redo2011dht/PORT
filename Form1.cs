using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// Thêm 3 em này vào là OK, để sài SerialPort
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Threading;
using System.Configuration;
// Bắt đầu code
namespace HIENCLUBVN_RS232
{
    public partial class Form1 : Form
    {
       // string filepath = @"d:\file.txt";
        string STX = ASCIIEncoding.ASCII.GetString(new byte[] { (Byte)0x02 });
        string ETX = ASCIIEncoding.ASCII.GetString(new byte[] { (Byte)0x03 });
        string text1,s;
        SerialPort P = new SerialPort(); // Khai báo 1 Object SerialPort mới.
        string InputData = String.Empty; // Khai báo string buff dùng cho hiển thị dữ liệu sau này.
        delegate void SetTextCallback(string text); // Khai bao delegate SetTextCallBack voi tham so string
        
        public Form1()
        {

            InitializeComponent();
            s = "";
            // Cài đặt các thông số cho COM
            // Mảng string port để chứ tất cả các cổng COM đang có trên máy tính
            string[] ports = SerialPort.GetPortNames();

            // Thêm toàn bộ các COM đã tìm được vào combox cbCom
            cbCom.Items.AddRange(ports); // Sử dụng AddRange thay vì dùng foreach
            P.Encoding = ASCIIEncoding.ASCII;
            P.ReadTimeout = 1;
            // Khai báo hàm delegate bằng phương thức DataReceived của Object SerialPort;
            // Cái này khi có sự kiện nhận dữ liệu sẽ nhảy đến phương thức DataReceive
            // Nếu ko hiểu đoạn này bạn có thể tìm hiểu về Delegate, còn ko cứ COPY . Ko cần quan tâm
            P.DataReceived += new SerialDataReceivedEventHandler(DataReceive);

            // Cài đặt cho BaudRate
            string[] BaudRate = { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            cbRate.Items.AddRange(BaudRate);

            // Cài đặt cho DataBits
            string[] Databits = { "6", "7", "8" };
            cbBits.Items.AddRange(Databits);

            //Cho Parity
            string[] Parity = { "None", "Odd", "Even" };
            cbParity.Items.AddRange(Parity);

            //Cho Stop bit
            string[] stopbit = { "1", "1.5", "2" };
            cbBit.Items.AddRange(stopbit);
            // Mấy cái này khá đơn giản, bạn đừng hỏi vì sao.  cứ COPY paste cho nhanh. :D
            
        }
        private void cbCom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen) 
            {
                P.Close(); // Nếu đang mở Port thì phải đóng lại
            }
            P.PortName = cbCom.SelectedItem.ToString(); // Gán PortName bằng COM đã chọn 
        }

        private void cbRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            P.BaudRate = Convert.ToInt32(cbRate.Text);
        }

        private void cbBits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            P.DataBits = Convert.ToInt32(cbBits.Text);
        }

        private void cbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            // Với thằng Parity hơn lằng nhằng. Nhưng cũng OK thôi. ^_^
            switch (cbParity.SelectedItem.ToString())
            {
                case "Odd":
                    P.Parity = Parity.Odd;
                    break;
                case "None":
                    P.Parity = Parity.None;
                    break;
                case "Even":
                    P.Parity = Parity.Even;
                    break;
            }
        }

        private void cbBit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                P.Close();
            }
            switch (cbBit.SelectedItem.ToString())
            {
                case "1":
                    P.StopBits = StopBits.One;
                    P.Handshake = Handshake.XOnXOff;
                    break;
                case "1.5":
                    P.StopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    P.StopBits = StopBits.Two;
                    break;
            }
        }
        // Hàm này được sự kiện nhận dử liệu gọi đến. Mục đích để hiển thị thôi
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
           // Thread.Sleep(100);
            InputData = P.ReadExisting();
            if (InputData != String.Empty)
            {
                // txtIn.Text = InputData; // Ko dùng đc như thế này vì khác threads .
                SetText(InputData); // Chính vì vậy phải sử dụng ủy quyền tại đây. Gọi delegate đã khai báo trước đó.
            }
        }
        // Hàm của em nó là ở đây. Đừng hỏi vì sao lại thế.
        
        private void SetText(string text)
        {
            if (this.txtkq.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText); // khởi tạo 1 delegate mới gọi đến SetText
                this.Invoke(d, new object[] { text });

                text1 += text;
                if (text1.Contains(STX) && text1.Contains(ETX))
                {
                    try
                    {
                        List<string> testluu = ExtractFromString(text1, STX, ETX);
                        foreach (string RX in testluu)
                        {
                            text1 = text1.Replace(STX + RX + ETX, "");
                            if (RX.Length >= 38)
                            {
                                if (RX.Substring(0, 1).Equals("R"))
                                {
                                    string barcode = RX.Substring(28);

                                    // xử lý lấy chuỗi yêu cầu từ pm
                                    barcode.Trim();
                                    string macode = "";
                                    //tạo chuỗi gửi qua máy au480
                                    s = STX + "S" + RX.Substring(1) + "    E" + macode + ETX;
                                    P.Write(s);
                                    s = "";
                                }
                                //xử lý lưu dữ liệu luu
                                else
                                {
                                    if (RX.Substring(0, 2).Equals("D "))
                                    {
                                        //lstr.Remove(str);
                                        string pathfilekq = @"D:\testrs232\" + RX.Substring(28, 11) + ".txt";
                                        FileStream fw = new FileStream(pathfilekq, FileMode.Create);
                                        StreamWriter swt = new StreamWriter(fw, Encoding.ASCII);
                                        string RX1 = RX.Replace("E", "");
                                        RX1 = RX1.Substring(43);
                                        for (int i = 8; i < RX1.Length; i += 11)
                                        {
                                            swt.WriteLine(RX1.Substring(i - 8, 9));
                                            swt.Flush();
                                        }
                                        swt.Close();
                                        fw.Close();
                                        swt.Dispose();
                                        fw.Dispose();
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {


                    }
                }
            }
            else this.txtkq.Text += text;
        }
        private static List<string> ExtractFromString(string text, string startString, string endString)
        {
            List<string> matched = new List<string>();
            int indexStart = 0, indexEnd = 0;
            bool exit = false;
            while (!exit)
            {
                indexStart = text.IndexOf(startString);
                indexEnd = text.IndexOf(endString);
                if (indexStart != -1 && indexEnd != -1)
                {
                    matched.Add(text.Substring(indexStart + startString.Length,indexEnd - indexStart - startString.Length));
                    text = text.Substring(indexEnd + endString.Length);
                }
                else
                    exit = true;
            }
            return matched;
        }
        // Toàn bộ cái này bạn nên COPY, nó cũng làm tôi đau đầu. :D
        // Đến hàm gửi data xuống COM
        private void btSend_Click(object sender, EventArgs e)
        {
            if (P.IsOpen)
            {
                string[] lines = File.ReadAllLines(@"D:\text.txt");
                foreach (string ss in lines)
                {
                    P.Write(ss);
                    //Thread.Sleep(2000);
                }
                //if (txtSend.Text == "") MessageBox.Show("Chưa có dữ liệu!","Thông Báo");
                //else P.Write(txtSend.Text);
            }
            else MessageBox.Show("COM chưa mở.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            txtSend.Clear();
        }
        // Đến đây coi như mọi việc đã ngon lành cành đào rồi.
        // Để chọn tiện trong việc Test thì chúng ta sẽ làm thêm bước nữa. cho mấy cái thông số
        // hay dùng được chọn. Ko cần thiết nếu bạn cảm thấy ko cần.
        private void Form1_Load(object sender, EventArgs e) // sẽ được gọi khi mở chương trình.
        {
            cbCom.SelectedIndex = 0;
            cbRate.SelectedIndex = 3; // 9600
            cbBits.SelectedIndex = 1; // 8
            cbParity.SelectedIndex = 0; // None
            cbBit.SelectedIndex = 0; // None
            // Hiện thị Status cho Pro tí
            status.Text = "Hãy chọn 1 cổng COM để kết nối.";
        }

        private void btKetNoi_Click(object sender, EventArgs e)
        {
           
            try
            { 
                P.Open();
                btNgat.Enabled = true;
                btKetNoi.Enabled = false;
                // Hiện thị Status
                status.Text = "Đang kết nối với cổng " + cbCom.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được.","Thử lại",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void btNgat_Click(object sender, EventArgs e)
        {
            //fs.Close();
            P.Close();
            btKetNoi.Enabled = true;
            btNgat.Enabled = false;
            // Hiện thị Status
            status.Text = "Đã Ngắt Kết Nối";
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            DialogResult kq = MessageBox.Show("Bạn thực sự muốn thoát", "HIENCLUBVN", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (kq == DialogResult.Yes)
            {
                MessageBox.Show("Cảm ơn bạn đã sử dụng chương trình","HIENCLUBVN");
                this.Close();
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            txtkq.Text = "";
            txtSend.Text = "";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.ShowDialog();
        }

        
    }
}
