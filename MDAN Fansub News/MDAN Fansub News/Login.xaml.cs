using System;
using System.ServiceModel.Channels;
using Windows.Security.Cryptography;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MDAN_App_Base
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login
    {
        readonly User _user = User.Instance;

        public Login()
        {
            InitializeComponent();
        }



        private void Login_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _user.Login(username.Text, password.Password);
            SaveUser();
        }

        private void SaveUser()
        {
            ApplicationData.Current.LocalSettings.Values["UserID"] = _user.Id;
            ApplicationData.Current.LocalSettings.Values["userName"] = _user.Username;
            ApplicationData.Current.LocalSettings.Values["status"] = _user.Status;
            ApplicationData.Current.LocalSettings.Values["Avatar"] = _user.Avatar;
            ApplicationData.Current.LocalSettings.Values["Tracker"] = _user.TrackerUri;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            _user.Login(username.Text, EncryptStringHelper(password.Password, "1234"));
            Frame.Navigate(typeof(UserPage));
        }

        private static string EncryptStringHelper(string plainString, string key)
        {
            try
            {
                var hashKey = GetMd5Hash(key);
                var decryptBuffer = CryptographicBuffer.ConvertStringToBinary(plainString, BinaryStringEncoding.Utf8);
                var aes = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
                var symmetricKey = aes.CreateSymmetricKey(hashKey);
                var encryptedBuffer = CryptographicEngine.Encrypt(symmetricKey, decryptBuffer, null);
                var encryptedString = CryptographicBuffer.EncodeToBase64String(encryptedBuffer);
                return encryptedString;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static IBuffer GetMd5Hash(string key)
        {
            var bufferUtf8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var hashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var hashBuffer = hashAlgorithmProvider.HashData(bufferUtf8Msg);
            if (hashBuffer.Length != hashAlgorithmProvider.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }
            return hashBuffer;
        }
    }
}
