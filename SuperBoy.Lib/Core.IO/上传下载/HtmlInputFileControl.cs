using System;
using System.Web.UI.HtmlControls;
using System.IO;

namespace Core.IO
{
    /// <summary>
    /// HtmlInputFileControl ��ժҪ˵�����ϴ���
    /// </summary>
    public class HtmlInputFileControl
    {
      

        #region IsAllowedExtension�Ƿ��������չ���ϴ�

        public static bool IsAllowedExtension(HtmlInputFile hifile)
        {
            string strOldFilePath = "", strExtension = "";

            //�����ϴ�����չ�������Ըĳɴ������ļ��ж���
            string[] arrExtension = { ".gif", ".GIF", ".JPG", ".jpg", ".JPEG", ".BMP", ".PNG", ".jpeg", ".bmp", ".png" };

            if (hifile.PostedFile.FileName != string.Empty)
            {
                strOldFilePath = hifile.PostedFile.FileName;
                //ȡ���ϴ��ļ�����չ��
                strExtension = strOldFilePath.Substring(strOldFilePath.LastIndexOf("."));
                //�жϸ���չ���Ƿ�Ϸ�
                for (var i = 0; i < arrExtension.Length; i++)
                {
                    if (strExtension.Equals(arrExtension[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region IsAllowedLength�ж��ϴ��ļ���С�Ƿ񳬹����ֵ
        public static bool IsAllowedLength(HtmlInputFile hifile)
        {
            //�����ϴ��ļ���С�����ֵ,���Ա�����xml�ļ���,��λΪKB
            var i = 512;
            //����ϴ��ļ��Ĵ�С�������ֵ,����flase,���򷵻�true.
            if (hifile.PostedFile.ContentLength > i * 512)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region SaveFile�ϴ��ļ��������ļ���
        public static string SaveFile(HtmlInputFile hifile, string strAbsolutePath)
        {
            string strOldFilePath = "", strExtension = "", strNewFileName = "";

            if (hifile.PostedFile.FileName != string.Empty)
            {
                strOldFilePath = hifile.PostedFile.FileName;
                //ȡ���ϴ��ļ�����չ��
                strExtension = strOldFilePath.Substring(strOldFilePath.LastIndexOf("."));
                //�ļ��ϴ��������
                strNewFileName = GetUniqueString() + strExtension;
                if (strAbsolutePath.LastIndexOf("\\") == strAbsolutePath.Length)
                {
                    hifile.PostedFile.SaveAs(strAbsolutePath + strNewFileName);
                }
                else
                {
                    hifile.PostedFile.SaveAs(strAbsolutePath + "\\" + strNewFileName);
                }
            }
            return strNewFileName;
        }
        #endregion

        #region CoverFile�����ϴ��ļ���ɾ��ԭ���ļ�
        public static void CoverFile(HtmlInputFile ffFile, string strAbsolutePath, string strOldFileName)
        {
            //������ļ���
            var strNewFileName = GetUniqueString();

            if (ffFile.PostedFile.FileName != string.Empty)
            {
                //��ͼƬ��Ϊ��ʱ��ɾ����ͼƬ
                if (strOldFileName != string.Empty)
                {
                    DeleteFile(strAbsolutePath, strOldFileName);
                }
                SaveFile(ffFile, strAbsolutePath);
            }
        }
        #endregion

        #region DeleteFileɾ��ָ���ļ�
        public static void DeleteFile(string strAbsolutePath, string strFileName)
        {
            if (strAbsolutePath.LastIndexOf("\\") == strAbsolutePath.Length)
            {
                if (File.Exists(strAbsolutePath + strFileName))
                {
                    File.Delete(strAbsolutePath + strFileName);
                }
            }
            else
            {
                if (File.Exists(strAbsolutePath + "\\" + strFileName))
                {
                    File.Delete(strAbsolutePath + "\\" + strFileName);
                }
            }
        }
        #endregion

  
        #region ��ȡһ�����ظ����ļ���
        /// <summary>
        /// ��ȡһ�����ظ����ļ���
        /// </summary>
        /// <returns></returns>
        public static string GetUniqueString()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmss");
        }
        #endregion


    }
}
