/*
Utils.cs
Copyright (C) 2015  Belledonne Communications, Grenoble, France
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using BelledonneCommunications.Linphone.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Model
{
    /// <summary>
    /// Static utiliy methods, mostly image management
    /// </summary>
    public class Utils
    {
        private const int LOCAL_IMAGES_QUALITY = 100;
        private const int THUMBNAIL_WIDTH = 420;
        private const string LOCAL_IMAGES_PATH = "ChatImages";

        /// <summary>
        /// Saves an image sent or received in the media library of the device.
        /// </summary>
        /// <param name="fileName">File's name in the isolated storage</param>
        /// <returns>true if the operation succeeds</returns>
        public static bool SavePictureInMediaLibrary(string fileName)
        {
          //  MediaLibrary library = new MediaLibrary();
            byte[] data;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        data = new byte[file.Length];
                        file.Read(data, 0, data.Length);
                    }
                }

               // library.SavePicture(fileName, data);
                return true;
            }
            catch { }
            return false;
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

                    }
                }

              //  MemoryStream ms = new MemoryStream(data);
                BitmapImage image = new BitmapImage();
                //image.SetSource(ms);
                //ms.Close();
                return image;
            }
            catch { }

            return null;
        }

        public static string GetUsernameFromAddress(Address addr)
        {
            if (addr.DisplayName != null && addr.DisplayName.Length > 0)
            { 
                return addr.DisplayName;
            }
            else
            {
                if(addr.UserName != null && addr.UserName.Length > 0)
                {
                    return addr.UserName;
                } else
                {
                    return addr.AsStringUriOnly();
                }
            }
        }

        /// <summary>
        /// Saves a BitmapImage as a JPEG file in the local storage
        /// </summary>
        /// <param name="image">The bitmap image to save</param>
        /// <param name="fileName">The file's name to use</param>
        /// <returns>true if the operation succeeds</returns>
        public static string SaveImageInLocalFolder(BitmapImage image, string fileName)
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.DirectoryExists(LOCAL_IMAGES_PATH))
                    {
                        store.CreateDirectory(LOCAL_IMAGES_PATH);
                    }
                    string filePath = Path.Combine(LOCAL_IMAGES_PATH, fileName);
                    if (store.FileExists(filePath))
                    {
                        store.DeleteFile(filePath);
                    }

                    using (IsolatedStorageFileStream file = store.CreateFile(filePath))
                    {
                        WriteableBitmap bitmap = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                        /*  Extensions.SaveJpeg(bitmap, file, bitmap.PixelWidth, bitmap.PixelHeight, 0, LOCAL_IMAGES_QUALITY);
                          file.Flush();
                          string hash = GetRandomHash();
                          file.Close();
                          bitmap = null;
                          string newFilePath = Path.Combine(LOCAL_IMAGES_PATH, hash + Path.GetExtension(filePath));
                          store.MoveFile(filePath, newFilePath);
                          IsolatedStorageFileStream newFile = store.OpenFile(newFilePath, FileMode.Open);
                          fileName = newFile.Name;
                          newFile.Close();*/
                        // return fileName;
                        return "";
                    }
                }
            }
            catch { }
            return null;
        }

        public static string GetImageRandomFileName()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string hash = GetRandomHash();
                    string filePath = Path.Combine(LOCAL_IMAGES_PATH, hash + ".jpg");
                    if (!store.DirectoryExists(LOCAL_IMAGES_PATH))
                    {
                        store.CreateDirectory(LOCAL_IMAGES_PATH);
                    }
                    IsolatedStorageFileStream newFile = store.OpenFile(filePath, FileMode.OpenOrCreate);
                    string fileName = newFile.ToString();     
                    return fileName;
                }
            }
            catch { }
            return null;
        }

        private static string GetRandomHash()
        {
            byte[] bytes = new byte[1024];
            /* RNGCryptoServiceProvider cryptProv = new RNGCryptoServiceProvider();
             cryptProv.GetBytes(bytes);
             SHA1Managed s = new SHA1Managed();*/
            // byte[] hashBytes = s.ComputeHash(bytes);
            // return BitConverter.ToString(hashBytes).Replace("-", "");
            return "";
        }

        public static string ReplacePlusInUri(string uri)
        {
            if (uri != null && uri.Contains("+"))
            {
                uri = uri.Replace("+", "%2B");
            }
            return uri;
        }
    }
}
