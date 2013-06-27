using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Linphone.Model
{
    /// <summary>
    /// Static utiliy methods, mostly image management
    /// </summary>
    public class Utils
    {
        private const int LOCAL_IMAGES_QUALITY = 100;
        private const int THUMBNAIL_WIDTH = 420;

        /// <summary>
        /// Saves an image sent or received in the media library of the device.
        /// </summary>
        /// <param name="fileName">File's name in the isolated storage</param>
        public static void SavePictureInMediaLibrary(string fileName)
        {
            MediaLibrary library = new MediaLibrary();
            byte[] data;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                        file.Close();
                    }
                }

                library.SavePicture(fileName, data);
            }
            catch { }
        }

        /// <summary>
        /// Get a thumbnail of a picture (used for chat images)
        /// </summary>
        /// <param name="image">The image to size down</param>
        /// <returns>The thumbnail picture</returns>
        public static BitmapImage GetThumbnailBitmapFromImage(BitmapImage image)
        {
            if (image == null)
                return null;

            if (image.PixelWidth <= THUMBNAIL_WIDTH)
                return image;

            MemoryStream ms = new MemoryStream();
            WriteableBitmap bitmap = new WriteableBitmap(image);

            int w, h;
            w = THUMBNAIL_WIDTH;
            h = (image.PixelHeight * w) / image.PixelWidth;

            bitmap.SaveJpeg(ms, w, h, 0, LOCAL_IMAGES_QUALITY);

            BitmapImage thumbnail = new BitmapImage();
            thumbnail.SetSource(ms);

            return thumbnail;
        }

        /// <summary>
        /// Returns a BitmapImage of a file stored in isolated storage
        /// </summary>
        /// <param name="fileName">Name of the file to open</param>
        /// <returns>a BitmapImage or null</returns>
        public static BitmapImage ReadImageFromIsolatedStorage(string fileName)
        {
            byte[] data;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                        file.Close();
                    }
                }

                MemoryStream ms = new MemoryStream(data);
                BitmapImage image = new BitmapImage();
                image.SetSource(ms);
                ms.Close();
                return image;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Download an image received using the given url and store it locally
        /// </summary>
        /// <param name="url">Url to download the image</param>
        /// <param name="message">ChatMessage linked to the image</param>
        /// <returns>The Bitmap Image for display</returns>
        public static BitmapImage DownloadImageAndStoreItInIsolatedStorage(string url, ChatMessage message)
        {
            //Download image
            BitmapImage image = new BitmapImage(new Uri(url));
            image.ImageOpened += (sender, e) =>
            {
                //Store it in isolated storage
                string fileName = url.Substring(url.LastIndexOf("/") + 1);
                SaveImageInLocalFolder(image, fileName);

                //Update image url in database to point on local image
                message.ImageURL = fileName;
                DatabaseManager.Instance.SubmitChanges();
            };

            return image;
        }

        public static void SaveImageInLocalFolder(BitmapImage image, string fileName)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(fileName))
                    {
                        store.DeleteFile(fileName);
                    }

                    using (IsolatedStorageFileStream file = store.CreateFile(fileName))
                    {
                        WriteableBitmap bitmap = new WriteableBitmap(image);
                        Extensions.SaveJpeg(bitmap, file, bitmap.PixelWidth, bitmap.PixelHeight, 0, LOCAL_IMAGES_QUALITY);
                        file.Flush();
                        file.Close();
                        bitmap = null;
                    }
                }
            }
            catch { }
        }
    }
}
