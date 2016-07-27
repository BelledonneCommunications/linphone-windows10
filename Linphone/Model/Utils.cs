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
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using BelledonneCommunications.Linphone.Native;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Linphone.Model
{
    public class Utils
    {
        private const int LOCAL_IMAGES_QUALITY = 100;
        private const int THUMBNAIL_WIDTH = 420;
        private const string LOCAL_IMAGES_PATH = "ChatImages";

        public static Paragraph FormatText(String text)
        {
            Paragraph paragraph = new Paragraph();
            if (text.Length > 0)
            {
                Run run = new Run();
                if (text.Contains("http://") || text.Contains("https://"))
                {
                    string[] split = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in split)
                    {
                        run.Text = word;
                        if (word.StartsWith("http://") || word.StartsWith("https://"))
                        {
                            Hyperlink link = new Hyperlink();
                            link.NavigateUri = new Uri(word);
                            link.Inlines.Add(run);
                            link.Foreground = (Brush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
                            paragraph.Inlines.Add(link);
                        }
                        else
                        {
                            paragraph.Inlines.Add(run);
                        }
                    }
                }
                else
                {
                    run.Text = text;
                    paragraph.Inlines.Add(run);
                }
            }
            return paragraph;
        }

        public static string FormatDate(long time)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(time * TimeSpan.TicksPerSecond);
            DateTime date = new DateTime(unixStart.Ticks + unixTimeStampInTicks).ToLocalTime();
            DateTime now = DateTime.Now;
            if (now.Year == date.Year && now.Month == date.Month && now.Day == date.Day)
                return String.Format("{0:HH:mm}", date);
            else if (now.Year == date.Year)
                return String.Format("{0:ddd d MMM, HH:mm}", date);
            else
                return String.Format("{0:ddd d MMM yyyy, HH:mm}", date);
        }

        /// <summary>
        /// Saves an image sent or received in the media library of the device.
        /// </summary>
        /// <param name="fileName">File's name in the isolated storage</param>
        /// <returns>true if the operation succeeds</returns>
        public static async System.Threading.Tasks.Task<bool> SavePictureInMediaLibrary(string fileName)
        {
            FolderPicker picker = new FolderPicker();
            try
            {
                picker.FileTypeFilter.Add("*");
                StorageFolder folder = await picker.PickSingleFolderAsync();
                if(folder != null)
                {
                    var tempFolder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await tempFolder.GetFileAsync(fileName);
                    await file.CopyAsync(folder);
                } 
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
        public static string SaveImageInIsolatedFolder(BitmapImage image, string fileName)
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
                      /*  WriteableBitmap bitmap = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
                          bitmap.Save(bitmap, file, bitmap.PixelWidth, bitmap.PixelHeight, 0, LOCAL_IMAGES_QUALITY);
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
                        //return "";
                    }
                }
            }
            catch { }
            return null;
        }

        public static async System.Threading.Tasks.Task<string> SaveImageInLocalFolder(StorageFile image)
        {
            try
            {
                var tempFolder = ApplicationData.Current.LocalFolder;
                StorageFile tempFile = await image.CopyAsync(tempFolder, GetFileName());
                return tempFile.Name;
            } catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        public static async System.Threading.Tasks.Task<string> GetImageTempFileName()
        {
            var tempFolder = ApplicationData.Current.LocalFolder;         
            String fileName = GetFileName() + ".jpg";
            var tempFile = await tempFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            return tempFile.Name;
        }

        public static async System.Threading.Tasks.Task<BitmapImage> ReadImageFromTempStorage(String name)
        {
            var tempFolder = ApplicationData.Current.LocalFolder;
            try
            {
                var tempFile = await tempFolder.GetFileAsync(name);
                Windows.Storage.Streams.IRandomAccessStream fileStream = await tempFile.OpenAsync(FileAccessMode.Read);
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    return bitmapImage;
                }
                catch (Exception e)
                {
                }                  
            } catch (Exception e)
            {
            }
            return null;
        }

        public static string GetImageRandomFileName()
        {
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string hash = GetFileName();
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

        private static string GetFileName()
        {
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000000; //Convert windows ticks to seconds
            String timestamp = ticks.ToString();
            return timestamp;
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
