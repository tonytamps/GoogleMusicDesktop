using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Google_Music_Desktop
{
    [Serializable]
    internal class FormSizeSaverSettings
    {
        public double Height, Width, YLoc, XLoc;
        public bool Maximized;

        public override string ToString()
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                return Convert.ToBase64String(buffer);
            }
        }

        internal static FormSizeSaverSettings FromString(string value)
        {
            try
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(value)))
                {
                    var bf = new BinaryFormatter();
                    return (FormSizeSaverSettings)bf.Deserialize(ms);
                }
            }
            catch (Exception)
            {
                return new FormSizeSaverSettings();
            }
        }
    }
}