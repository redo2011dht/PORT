using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace ConnectLab
{
    public class AppUntil
    {
        public static bool Write(Object sourceObject, string FileName)
        {
            FileStream ft = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            try
            {

                bf.Serialize(ft, sourceObject);
                ft.Flush();
                ft.Close();
                ft.Dispose();
                bf = null;

            }
            catch
            {
                return false;
            }
            return true;
        }
        public static object Read(object desObjtect, string FileName)
        {
            FileStream ft = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryFormatter bf = new BinaryFormatter();
            object obj = null;
            try
            {

                obj = bf.Deserialize(ft);

            }
            catch
            {

            }
            finally
            {

                ft.Close();
                if (obj != null && desObjtect != null)
                {
                    Type sType = obj.GetType();
                    PropertyInfo[] desPI = desObjtect.GetType().GetProperties();
                    foreach (PropertyInfo pi in desPI)
                    {
                        pi.SetValue(desObjtect, sType.GetProperty(pi.Name).GetValue(obj, null), null);
                    }
                }
            }
            if (obj == null) return null;
            return desObjtect;
        }
        public static string GetMD5(string chuoi)
        {
            string str_md5 = "";
            byte[] mang = System.Text.Encoding.UTF8.GetBytes(chuoi);

            MD5CryptoServiceProvider my_md5 = new MD5CryptoServiceProvider();
            mang = my_md5.ComputeHash(mang);

            foreach (byte b in mang)
            {
                str_md5 += b.ToString("X2");
            }

            return str_md5;
        }
    }
}
