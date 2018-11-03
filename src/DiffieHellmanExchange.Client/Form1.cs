using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiffieHellmanExchange.Shared.Services;
using Microsoft.AspNetCore.SignalR.Client;

namespace DiffieHellmanExchange.Client
{
    public partial class Form1 : Form
    {
        private KeyService _keyService;

        public Form1()
        {
            InitializeComponent();
            _keyService = new KeyService();
            _keyService.GeneratePrimeTable();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/main")
                .Build();

            connection.On<int, int, BigInteger>("RecieveQAY", async (q, a, ys) =>
            {
                var xc = _keyService.GetX(q);
                var yc = _keyService.GetY(xc, q, a);
                var key = _keyService.GetKey(ys, xc, q);

                await connection.InvokeAsync("RecieveY", yc);

                textBox1.Invoke(new Action(() => textBox1.AppendText($"Generated key: {key}")));
            });

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("GetAQY");
            }
            catch (Exception ex)
            {
                textBox1.AppendText($"Failed to connect: {ex.Message}");
            }
        }
    }
}
