using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CustomFileProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class MyTextFile
    {

        #region Local Variables
        string m_fileName;
        string m_text;
        string m_folderPath;
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="folderPath"></param>
        public MyTextFile(string fileName, string text, string folderPath)
        {
            m_folderPath = folderPath;
            m_fileName = fileName;
            m_text = text;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName
        {
            get
            {
                return m_fileName;
            }
            set
            {
                m_fileName = value;
            }
        }

        /// <summary>
        /// Text in the file
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }

        /// <summary>
        /// Path of the folder containing the file
        /// </summary>
        public string FolderPath
        {
            get
            {
                return m_folderPath;
            }
            set
            {
                m_folderPath = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a Text File
        /// </summary>
        public void Create()
        {
            using (FileStream fs1 = File.Open(Path.Combine(FolderPath, FileName), FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs1, Text);
                fs1.Flush();
            }
        }

        /// <summary>
        /// Updates a Text File 
        /// </summary>
        public void Update()
        {
            Delete();
            Create();
        }

        /// <summary>
        ///  Deletes a Text File
        /// </summary>
        public void Delete()
        {
            File.Delete(Path.Combine(FolderPath, FileName));
        }

        /// <summary>
        /// Returns the file specified by file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static MyTextFile GetFileByFileName(string fileName, string folderName)
        {

            string _text = string.Empty;
            using (TextReader tr = new StreamReader(Path.Combine(folderName, fileName)))
            {
                _text = tr.ReadToEnd();
            }
            return new MyTextFile(fileName, _text, folderName);
        }

        /// <summary>
        /// Returns the list of file names and their last modified date
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static Dictionary<string, DateTime> GetUpdatedFileNames(string folderName)
        {
            Dictionary<string, DateTime> fileNames = new Dictionary<string, DateTime>();

            DirectoryInfo df = new DirectoryInfo(folderName);

            foreach (FileInfo  fi in df.GetFiles())
            {
                fileNames.Add(fi.Name, fi.LastWriteTime);
            }
            return fileNames;
        }

        #endregion

    }
}
