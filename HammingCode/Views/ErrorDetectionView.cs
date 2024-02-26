using HammingCode.App.Domain;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HammingCode.Views
{
    public partial class ErrorDetectionView : Form
    {
        private Transmitter _transmitter;
        private Receiver _receiver;
        private HammingBlock _hammingBlock;

        public ErrorDetectionView()
        {
            InitializeComponent();
            _hammingBlock = new HammingBlock();
            _transmitter = new Transmitter(_hammingBlock);
            _receiver = new Receiver(_hammingBlock);
        }

        private void enviarButton_Click(object sender, EventArgs e)
        {
            try
            {
                string message = mensajeText.Text;
                bool noisyChannel = ruidoCheck.Checked;

                _transmitter.SendMessage(message, _receiver, noisyChannel);

                var sentMessage = _transmitter.LastSent;
                enviadoText.Text = sentMessage.RealMessage;
                enviadoAsciiText.Text = sentMessage.ASCIIString;
                paridadText.Text = _hammingBlock.CalculateRequiredParityBits(sentMessage.Size).ToString();
                hammingText.Text = _hammingBlock.GetResult(sentMessage).ASCIIString.Aggregate(string.Empty, (c, i) => c + i + ' ');


                recibidoText.Text = _receiver.ReceivedMessage.ASCIIString.Aggregate(string.Empty, (c, i) => c + i + ' ');
                // Darle responsabilidad al receiver
                //string innerMessage = _hammingBlock.ExtractInnerMessage(_receiver.ReceivedMessage);
                string innerMessage = _receiver.GetInnerMessage();
                recibidoAsciiText.Text = innerMessage;
                
                contenidoText.Text = _hammingBlock.TranslateAsciiStringToString(innerMessage);
                if (message.Length > 4)
                {
                    MessageBox.Show(
                        "Debido a una limitación del algoritmo empleado, no es posible mostrar el contenido recibido correspondiente a un mensaje con longitud mayor a 4 caracteres.", 
                        "Longitud de mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    contenidoText.Text = "-";
                }

                posicionErrorText.Text = _receiver.GetErrorPosition().ToString();

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK);
            }

        }
    }
}
