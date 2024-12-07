using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace ElectronicDigitalSignature
{
    public sealed partial class MainWindow : Window
    {
        private RSASignature _rsa;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void ShowMessage(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private void GenerateKeysButton_Click(object sender, RoutedEventArgs e)
        {
            _rsa = new RSASignature("key container");
            var publicKey = _rsa.ExportPublicKey();
            PublicKeyTextBlock.Text = publicKey;
        }

        private void SignButton_Click(object sender, RoutedEventArgs e)
        {
            if(_rsa == null)
            {
                ShowMessage("Error", "Generate keys first!");
                return;
            }

            var message = MessageTextBox.Text;
            if(string.IsNullOrWhiteSpace(message))
            {
                ShowMessage("Error", "Enter a message to sign!");
                return;
            }

            var signature = _rsa.GenerateSignature(message);
            SignatureTextBlock.Text = signature;
        }

        private void VerifyButton_Click(object sender, RoutedEventArgs e)
        {
            var publicKey = PublicKeyTextBlock.Text;
            var signature = SignatureTextBlock.Text;
            var message = MessageTextBox.Text;

            if(string.IsNullOrWhiteSpace(publicKey))
            {
                ShowMessage("Error", "Enter the public key!");
                return;
            }

            if(string.IsNullOrWhiteSpace(signature))
            {
                ShowMessage("Error", "Enter the signature!");
                return;
            }

            if(string.IsNullOrWhiteSpace(message))
            {
                ShowMessage("Error", "Enter a message to verify!");
                return;
            }

            try
            {
                _rsa = new RSASignature("MyKeyContainer");
                _rsa.ImportPublicKey(publicKey);
                var isValid = _rsa.VerifySignature(message, signature);
                VerificationTextBlock.Text = isValid ? "Signature is valid." : "Signature is invalid.";
            }
            catch(Exception ex)
            {
                ShowMessage("Error", $"An error occurred: {ex.Message}");
            }
        }
    }
}
